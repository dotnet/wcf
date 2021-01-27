// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using WsdlNS = System.Web.Services.Description;
using System.Xml.Schema;

namespace Microsoft.Tools.ServiceModel.Svcutil.Metadata
{
    public class MetadataExchangeResolver : MetadataExchangeClient
    {
        private const int MaximumResolvedReferencesDefault = 2000; // Arbitrary big-enough number.
        private const int MaxNameTableCharCount = 1024 * 1024; //1MB
        private const long MaxRecievedMexMessageSize = (long)(520 * 1024 * 1024); // 520MB
        private const int MaxDepth = 320; // Arbitrary big-enough number, the default is 32
        private const int MaxBytesPerRead = 900 * 1024; // 900KB, 1MB default stack size
        private const string mexUri = "/mex";
        private const string wsdlQuery = "?wsdl";
        private const string singleWsdlQuery = "?singlewsdl";

        private X509Certificate _clientCertificate;
        private NetworkCredential _userCredentials;
        private HttpWebRequest _currentRequest;
        private MetadataSet _metadataSet;
        private Exception _metadataException;
        private bool _clientAuthenticated;

        private MetadataExchangeResolver(Binding binding) : base(binding)
        {
        }

        public EndpointAddress EndpointAddress { get; private set; }

        public IHttpCredentialsProvider HttpCredentialsProvider { get; private set; }

        public IClientCertificateProvider ClientCertificatesProvider { get; private set; }

        public IServerCertificateValidationProvider ServerCertificateValidationProvider { get; private set; }

        public bool HasServiceMetadata
        {
            get
            {
                // success when there's at least one endpoint in the metadata docs which can be also xml/xsd describing types used.
                return _metadataSet != null && _metadataSet.MetadataSections.Any((section) => section.Metadata is XmlSchema || section.Metadata is WsdlNS.ServiceDescription);
            }
        }

        public static MetadataExchangeResolver Create(
            EndpointAddress endpointAddress,
            IHttpCredentialsProvider userCredentialsProvider,
            IClientCertificateProvider clientCertificatesProvider,
            IServerCertificateValidationProvider serverCertificateValidationProvider)
        {
            if (endpointAddress == null)
            {
                throw new ArgumentNullException(nameof(endpointAddress));
            }

            MetadataExchangeResolver resolver = CreateMetadataExchangeClient(endpointAddress);

            resolver.EndpointAddress = endpointAddress;
            resolver.HttpCredentialsProvider = userCredentialsProvider;
            resolver.ClientCertificatesProvider = clientCertificatesProvider;
            resolver.ServerCertificateValidationProvider = serverCertificateValidationProvider;
            resolver.OperationTimeout = TimeSpan.MaxValue;
            resolver.ResolveMetadataReferences = true;
            resolver.MaximumResolvedReferences = MaximumResolvedReferencesDefault;
            resolver.HttpCredentials = System.Net.CredentialCache.DefaultCredentials;

            return resolver;
        }

        public async Task<IEnumerable<MetadataSection>> ResolveMetadataAsync(CancellationToken cancellationToken)
        {
            bool metadataResolved = false;

            string uriQuery = this.EndpointAddress.Uri.Query;
            string baseUri = this.EndpointAddress.Uri.AbsoluteUri.Trim('/');
            bool isQueryUri = !string.IsNullOrEmpty(uriQuery);
            bool isMexUri = this.EndpointAddress.Uri.AbsoluteUri.EndsWith(mexUri, StringComparison.OrdinalIgnoreCase);
            bool isHttp = this.EndpointAddress.Uri.Scheme == MetadataConstants.Uri.UriSchemeHttp || this.EndpointAddress.Uri.Scheme == MetadataConstants.Uri.UriSchemeHttps;

            try
            {
                this.ServerCertificateValidationProvider?.BeforeServerCertificateValidation(this.EndpointAddress.Uri);

                if (isQueryUri)
                {
                    if (isHttp)
                    {
                        metadataResolved = await ResolveMetadataAsync(this.EndpointAddress.Uri, MetadataExchangeClientMode.HttpGet, true, cancellationToken).ConfigureAwait(false);
                    }
                    if (!metadataResolved)
                    {
                        baseUri = this.EndpointAddress.Uri.AbsoluteUri.Remove(this.EndpointAddress.Uri.AbsoluteUri.Length - uriQuery.Length);
                        metadataResolved = await ResolveMetadataAsync(new Uri(baseUri + mexUri), MetadataExchangeClientMode.MetadataExchange, false, cancellationToken).ConfigureAwait(false);
                    }
                }
                else if (isMexUri)
                {
                    metadataResolved = await ResolveMetadataAsync(this.EndpointAddress.Uri, MetadataExchangeClientMode.MetadataExchange, true, cancellationToken).ConfigureAwait(false);
                    if (!metadataResolved && isHttp)
                    {
                        baseUri = this.EndpointAddress.Uri.AbsoluteUri.Remove(this.EndpointAddress.Uri.AbsoluteUri.Length - mexUri.Length);
                        metadataResolved = await ResolveMetadataAsync(new Uri(baseUri + wsdlQuery), MetadataExchangeClientMode.HttpGet, false, cancellationToken).ConfigureAwait(false);
                    }
                }
                else
                {
                    // do http before /mex for two reasons: 1. http authentication may be needed and 2. httpGet is usually enabled with the wsdl query.
                    if (isHttp)
                    {
                        // try downloading the doc first, the MEX client may not be able to handle it (as when the doc contains non-schema/wsdl sections like a stylesheet).
                        using (var stream = await DownloadMetadataFileAsync(cancellationToken).ConfigureAwait(false))
                        {
                            metadataResolved = await ResolveMetadataAsync(stream, baseUri, cancellationToken).ConfigureAwait(false);
                        }

                        if (!metadataResolved)
                        {
                            metadataResolved = await ResolveMetadataAsync(new Uri(baseUri + wsdlQuery), MetadataExchangeClientMode.HttpGet, false, cancellationToken).ConfigureAwait(false);
                            if (!metadataResolved)
                            {
                                // it might be a URI pointing to a wsdl file directly.
                                metadataResolved = await ResolveMetadataAsync(new Uri(baseUri), MetadataExchangeClientMode.HttpGet, true, cancellationToken).ConfigureAwait(false);
                            }
                        }
                    }

                    // last attempt using /mex
                    if (!metadataResolved)
                    {
                        metadataResolved = await ResolveMetadataAsync(new Uri(baseUri), MetadataExchangeClientMode.MetadataExchange, false, cancellationToken).ConfigureAwait(false);
                        if (!metadataResolved)
                        {
                            metadataResolved = await ResolveMetadataAsync(new Uri(baseUri + mexUri), MetadataExchangeClientMode.MetadataExchange, false, cancellationToken).ConfigureAwait(false);
                        }
                    }
                }
            }
            finally
            {
                this.ServerCertificateValidationProvider?.AfterServerCertificateValidation(this.EndpointAddress.Uri);
            }

            if (!metadataResolved)
            {
                if (_metadataException == null)
                {
                    _metadataException = new MetadataExchangeException(MetadataResources.ErrUnableToConnectToUriFormat, this.EndpointAddress.Uri.AbsoluteUri, MetadataResources.EnableMetadataHelpMessage);
                }
                throw _metadataException;
            }

            return _metadataSet.MetadataSections;
        }

        private async Task<bool> ResolveMetadataAsync(Uri serviceUri, MetadataExchangeClientMode metadataExchangeMode, bool captureException, CancellationToken cancellationToken)
        {
            bool authenticateUser;

            do
            {
                try
                {
                    authenticateUser = false;
                    _currentRequest = null;
                    cancellationToken.ThrowIfCancellationRequested();

                    IAsyncResult result = this.BeginGetMetadata(serviceUri, metadataExchangeMode, null, null);

                    while (!result.IsCompleted)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            _currentRequest?.Abort();
                            cancellationToken.ThrowIfCancellationRequested();
                        }
                        await Task.Delay(100);
                    }

                    _metadataSet = this.EndGetMetadata(result);
                    _metadataException = null;
                    _clientAuthenticated = true;
                }
                catch (Exception ex)
                {
                    if (Utils.IsFatalOrUnexpected(ex)) throw;

                    authenticateUser = RequiresAuthentication(ex, serviceUri, out bool isAuthError);
                    if (isAuthError)
                    {
                        // the user could not be authenticated.
                        // observe that we don't throw if the user doesn't need to be authenticated exceptions can be capture below.
                        throw;
                    }

                    if (captureException)
                    {
                        _metadataException = ex;
                    }
                }
            }
            while (authenticateUser);

            return this.HasServiceMetadata;
        }

        private async Task<bool> ResolveMetadataAsync(Stream stream, string baseUri, CancellationToken cancellationToken)
        {
            var loader = new MetadataDocumentLoader(baseUri, this.HttpCredentialsProvider, this.ClientCertificatesProvider, this.ServerCertificateValidationProvider);

            try
            {
                await loader.LoadFromStreamAsync(stream, baseUri, string.Empty, cancellationToken).ConfigureAwait(false);
                _metadataSet = new MetadataSet(loader.MetadataSections);
            }
            catch
            {
            }

            return this.HasServiceMetadata;
        }

        public async Task<Stream> DownloadMetadataFileAsync(CancellationToken cancellationToken)
        {
            bool authenticateUser;

            Stream memoryStream = null;
            string errorMsg = null;
            HttpWebResponse webResponse = null;

            do
            {
                try
                {
                    authenticateUser = false;

                    this.GetWebRequest(this.EndpointAddress.Uri, null, null);
                    IAsyncResult result = _currentRequest.BeginGetResponse(null, null);

                    while (!result.IsCompleted)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            _currentRequest?.Abort();
                            cancellationToken.ThrowIfCancellationRequested();
                        }
                        await Task.Delay(100);
                    }

                    webResponse = (HttpWebResponse)_currentRequest.EndGetResponse(result);
                    memoryStream = HttpWebRequestHelper.ResponseToMemoryStream(webResponse);

                    _clientAuthenticated = true;

                    if (webResponse.StatusCode != HttpStatusCode.OK)
                    {
                        Encoding encoding = HttpWebRequestHelper.GetEncoding(webResponse.ContentType);
                        errorMsg = new StreamReader(memoryStream, encoding, true).ReadToEnd();
                        memoryStream.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    if (Utils.IsFatalOrUnexpected(ex)) throw;

                    authenticateUser = RequiresAuthentication(ex, this.EndpointAddress.Uri, out bool _);
                    if (!authenticateUser)
                    {
                        // no need to authenticate user then throw.
                        throw;
                    }
                }
            }
            while (authenticateUser);

            if (errorMsg != null)
            {
                throw new WebException(errorMsg, null, WebExceptionStatus.ProtocolError, webResponse);
            }

            return memoryStream;
        }

        private bool RequiresAuthentication(Exception exception, Uri serviceUri, out bool isAuthenticationError)
        {
            bool authRequired = false;
            isAuthenticationError = false;

            if (!_clientAuthenticated)
            {
                WebException webException = HttpAuthenticationHelper.GetException<WebException>(exception);

                if (webException != null)
                {
                    if (webException.Status == WebExceptionStatus.TrustFailure)
                    {
                        // trust failure: server cert validation failed.
                        isAuthenticationError = true;
                    }
                    else
                    {
                        if (HttpAuthenticationHelper.GetUnauthorizedResponse(webException) != null)
                        {
                            if (this.HttpCredentialsProvider != null)
                            {
                                _userCredentials = this.HttpCredentialsProvider.GetCredentials(serviceUri, webException);
                                isAuthenticationError = _userCredentials == null;
                                authRequired = !isAuthenticationError;
                            }
                        }
                        else if (HttpAuthenticationHelper.GetForbiddenResponse(webException) != null)
                        {
                            if (this.ClientCertificatesProvider != null)
                            {
                                _clientCertificate = this.ClientCertificatesProvider.GetCertificate(serviceUri);
                                isAuthenticationError = _clientCertificate == null;
                                authRequired = !isAuthenticationError;
                            }
                        }
                    }
                }
            }

            return authRequired;
        }

        internal protected override HttpWebRequest GetWebRequest(Uri location, string dialect, string identifier)
        {
            HttpWebRequest request = base.GetWebRequest(location, dialect, identifier);
            _currentRequest = request;
#if !NETCORE10
            _currentRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; MS Web Services Client Protocol " + System.Environment.Version.ToString() + ")";
#endif
            if (_userCredentials != null)
            {
                request.Credentials = _userCredentials;
            }

#if !NETCORE10
            if (_clientCertificate != null)
            {
                // When the user has been authenticated IIS will require a client cert for a subsequent request but it will not fail authentication (401) 
                // if it is not provided, instead it fails with Forbidden (403) status. Always set cert (if available).
                request.ClientCertificates.Add(_clientCertificate);
            }
#endif
            return request;
        }

        private static MetadataExchangeResolver CreateMetadataExchangeClient(EndpointAddress endpointAddress)
        {
            MetadataExchangeResolver metadataExchangeClient = null;

            string scheme = endpointAddress.Uri.Scheme;

            if (String.Compare(scheme, MetadataConstants.Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) == 0)
            {
                WSHttpBinding binding = (WSHttpBinding)MetadataExchangeBindings.CreateMexHttpBinding();
                binding.MaxReceivedMessageSize = MaxRecievedMexMessageSize;
                binding.ReaderQuotas.MaxNameTableCharCount = MaxNameTableCharCount;
                binding.ReaderQuotas.MaxDepth = MaxDepth;
                binding.ReaderQuotas.MaxBytesPerRead = MaxBytesPerRead;
                metadataExchangeClient = new MetadataExchangeResolver(binding);
            }
            else if (String.Compare(scheme, MetadataConstants.Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) == 0)
            {
                WSHttpBinding binding = (WSHttpBinding)MetadataExchangeBindings.CreateMexHttpsBinding();
                binding.MaxReceivedMessageSize = MaxRecievedMexMessageSize;
                binding.ReaderQuotas.MaxNameTableCharCount = MaxNameTableCharCount;
                binding.ReaderQuotas.MaxDepth = MaxDepth;
                binding.ReaderQuotas.MaxBytesPerRead = MaxBytesPerRead;
                metadataExchangeClient = new MetadataExchangeResolver(binding);
            }
            else if (String.Compare(scheme, MetadataConstants.Uri.UriSchemeNetTcp, StringComparison.OrdinalIgnoreCase) == 0)
            {
                CustomBinding binding = (CustomBinding)MetadataExchangeBindings.CreateMexTcpBinding();
                binding.Elements.Find<TcpTransportBindingElement>().MaxReceivedMessageSize = MaxRecievedMexMessageSize;
                metadataExchangeClient = new MetadataExchangeResolver(binding);
            }
            else if (String.Compare(scheme, MetadataConstants.Uri.UriSchemeNetPipe, StringComparison.OrdinalIgnoreCase) == 0)
            {
                CustomBinding binding = (CustomBinding)MetadataExchangeBindings.CreateMexNamedPipeBinding();
                binding.Elements.Find<NamedPipeTransportBindingElement>().MaxReceivedMessageSize = MaxRecievedMexMessageSize;
                metadataExchangeClient = new MetadataExchangeResolver(binding);
            }
            else
            {
                throw new MetadataExchangeException(MetadataResources.ErrCannotCreateAMetadataExchangeClientFormat, endpointAddress.Uri.OriginalString, scheme);
            }

            return metadataExchangeClient;
        }

        private static class HttpWebRequestHelper
        {
            // utility functions copied from System.Web.Services code.

            public static MemoryStream ResponseToMemoryStream(HttpWebResponse webResponse)
            {
                int idx;
                int bufferSize = 1 * 1024; // KB
                var memoryStream = new MemoryStream(bufferSize);
                byte[] buffer = new byte[bufferSize];

                using (var responseStream = webResponse.GetResponseStream())
                {
                    while ((idx = responseStream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        memoryStream.Write(buffer, 0, idx);
                    }

                    memoryStream.Position = 0;
                }
                return memoryStream;
            }

            public static Encoding GetEncoding(string contentType)
            {
                try
                {
                    string charset = GetContentTypeParamValue(contentType, "charset");
                    if (!string.IsNullOrWhiteSpace(charset))
                    {
                        return Encoding.GetEncoding(charset);
                    }
                }
                catch
                {
                }

                return new ASCIIEncoding();
            }

            private static string GetContentTypeParamValue(string contentType, string paramName)
            {
                string[] paramDecls = contentType.Split(new char[] { ';' });
                for (int i = 1; i < paramDecls.Length; i++)
                {
                    string paramDecl = paramDecls[i].TrimStart(null);
                    if (String.Compare(paramDecl, 0, paramName, 0, paramName.Length, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        int equals = paramDecl.IndexOf('=', paramName.Length);
                        if (equals >= 0)
                        {
                            return paramDecl.Substring(equals + 1).Trim(new char[] { ' ', '\'', '\"', '\t' });
                        }
                    }
                }
                return null;
            }
        }
    }
}
