// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
#if PRIVATE_RTLIB
using XmlNS = Microsoft.Xml;
#else
using System.Reflection;
using System.Xml.Schema;
using XmlNS = System.Xml;
#endif
using WsdlNS = System.Web.Services.Description;

namespace Microsoft.Tools.ServiceModel.Svcutil.Metadata
{
    /// <summary>
    /// Loads metadata from a service URI (service URL or XML files).
    /// </summary>
    public class MetadataDocumentLoader
    {
        public enum LoadState
        {
            NotStarted,
            Started,
            Successful,
            Failed
        }

        private readonly IHttpCredentialsProvider _httpCredentialsProvider;
        private readonly IClientCertificateProvider _clientCertificatesProvider;
        private readonly IServerCertificateValidationProvider _serverCertificateValidationProvider;

        private int? _hashCode;
        private readonly bool _resolveExternalDocs;
        private List<string> _processedUri = new List<string>();
        private List<string> _updatedUri = new List<string>();
        private readonly Object _thisLock = new Object();

        /// <summary>
        /// The web server/service URL when the metadata is downloaded from a remote URI.
        /// </summary>
        public Uri MetadataSourceUrl { get; private set; }

        /// <summary>
        /// The collection of file paths when the metadata is loaded from wsdl/schema files in disk.
        /// </summary>
        private List<Uri> metadataSourceFiles { get; set; } = new List<Uri>();
        public IEnumerable<Uri> MetadataSourceFiles { get { return this.metadataSourceFiles; } }

        /// <summary>
        /// The Metadata sections parsed from the metadata documents
        /// </summary>
        private readonly List<MetadataSection> _metadataSections = new List<MetadataSection>();
        public IEnumerable<MetadataSection> MetadataSections { get { return _metadataSections; } }

        /// <summary>
        /// A collection of exceptions (if any) thrown during the loading of remote or local documents.
        /// </summary>
        private readonly List<Exception> _documentLoadExceptions = new List<Exception>();
        public IEnumerable<Exception> DocumentLoadExceptions { get { return _documentLoadExceptions; } }

        /// <summary>
        /// The object LoadState value.
        /// </summary>
        public LoadState State { get; set; }


        /// <summary>
        /// </summary>
        /// <param name="uri">The service Uri to load service metadata from.</param>
        /// <param name="httpCredentialsProvider"></param>
        /// <param name="clientCertificatesProvider"></param>
        /// <param name="serverCertificateValidationProvider"></param>
        public MetadataDocumentLoader(string uri, IHttpCredentialsProvider httpCredentialsProvider, IClientCertificateProvider clientCertificatesProvider, IServerCertificateValidationProvider serverCertificateValidationProvider)
        {
            _httpCredentialsProvider = httpCredentialsProvider;
            _clientCertificatesProvider = clientCertificatesProvider;
            _serverCertificateValidationProvider = serverCertificateValidationProvider;

            if (string.IsNullOrEmpty(uri))
            {
                throw new ArgumentNullException(nameof(uri));
            }

            if (!CanLoad(uri, string.Empty, Directory.GetCurrentDirectory(), out Uri metadataUri))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, MetadataResources.ErrInvalidUriFormat, uri));
            }

            if (metadataUri.IsFile)
            {
                // the input could be a reference to a list of files specified with wildcards, let's check.
                var fileInfoList = MetadataFileNameManager.ResolveFiles(metadataUri.LocalPath);

                // when only the wsdl file is specified, we need to resolve any schema references.
                _resolveExternalDocs = fileInfoList.Length == 1;
                this.metadataSourceFiles.AddRange(fileInfoList.Select(fi => new Uri(fi.FullName, UriKind.Absolute)));
            }
            else
            {
                _resolveExternalDocs = true;
                this.MetadataSourceUrl = metadataUri;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="metadataFiles">A collection containing the location of wsdl/schema documents to load service metadata from.</param>
        /// <param name="resolveExternalDocuments">if true, WSDL imports and XmlSchema Includes will be attempted to be resolved.</param>
        /// <param name="httpCredentialsProvider"></param>
        /// <param name="clientCertificatesProvider"></param>
        /// <param name="serverCertificateValidationProvider"></param>
        public MetadataDocumentLoader(IEnumerable<string> metadataFiles, bool resolveExternalDocuments, IHttpCredentialsProvider httpCredentialsProvider, IClientCertificateProvider clientCertificatesProvider, IServerCertificateValidationProvider serverCertificateValidationProvider)
        {
            _httpCredentialsProvider = httpCredentialsProvider;
            _clientCertificatesProvider = clientCertificatesProvider;
            _serverCertificateValidationProvider = serverCertificateValidationProvider;

            if (metadataFiles == null)
            {
                throw new ArgumentNullException(nameof(metadataFiles));
            }

            foreach (var uri in metadataFiles)
            {
                if (!CanLoad(uri, string.Empty, Directory.GetCurrentDirectory(), out Uri metadataUri))
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, MetadataResources.ErrInvalidUriFormat, uri));
                }

                if (!metadataUri.IsFile)
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, MetadataResources.ErrUrlNotAllowedOnMultipleInputsFormat, metadataUri));
                }

                var fileInfoList = MetadataFileNameManager.ResolveFiles(metadataUri.LocalPath);
                this.metadataSourceFiles.AddRange(fileInfoList.Select(fi => new Uri(fi.FullName, UriKind.Absolute)));
            }

            _resolveExternalDocs = resolveExternalDocuments;
        }

        /// <summary>
        /// By default, external references are not resolved so no need for authentication providers as documents are local.
        /// </summary>
        /// <param name="metadataFiles"></param>
        public MetadataDocumentLoader(IEnumerable<string> metadataFiles) : this(metadataFiles, false, null, null, null)
        {
        }

        public async Task LoadAsync(CancellationToken cancellationToken)
        {
            lock (_thisLock)
            {
                // to ensure the object integrity we need to make sure the load operation is executed only once.
                switch (this.State)
                {
                    case LoadState.NotStarted:
                        this.State = LoadState.Started;
                        break;
                    case LoadState.Started:
                    case LoadState.Failed:
                        throw new InvalidOperationException();
                    case LoadState.Successful:
                        return;
                }
            }

            try
            {
                if (this.MetadataSourceUrl != null)
                {
                    await LoadAsync(this.MetadataSourceUrl.AbsoluteUri, string.Empty, Directory.GetCurrentDirectory(), cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    foreach (var fileUri in this.MetadataSourceFiles)
                    {
                        await LoadAsync(fileUri.LocalPath, string.Empty, Directory.GetCurrentDirectory(), cancellationToken).ConfigureAwait(false);
                    }
                }

                FixupChameleonSchemas(_metadataSections);

                cancellationToken.ThrowIfCancellationRequested();

                // The MetadataExchangeClient doesn't update the SourceUri on the Metadata object but on the MetadataSection object.
                UpdateMetadataSourceUri();

                this.State = LoadState.Successful;
            }
            catch
            {
                this.State = LoadState.Failed;
                throw;
            }
        }

        private async Task LoadAsync(string uri, string baseUrl, string basePath, CancellationToken cancellationToken)
        {
            if (CanLoad(uri, baseUrl, basePath, out Uri serviceUri))
            {
                if (serviceUri.Scheme == MetadataConstants.Uri.UriSchemeFile)
                {
                    var fileInfoList = MetadataFileNameManager.ResolveFiles(serviceUri.LocalPath);
                    foreach (var fileInfo in fileInfoList)
                    {
                        if (!IsUriProcessed(fileInfo.FullName))
                        {
                            using (var fileStream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
                            {
                                await LoadFromStreamAsync(fileStream, fileInfo.FullName, fileInfo.DirectoryName, cancellationToken).ConfigureAwait(false);
                            }
                        }
                    }
                }
                else
                {
                    if (!IsUriProcessed(serviceUri.AbsoluteUri))
                    {
                        // Clone providers as they may be in use for a different URI.
                        MetadataExchangeResolver metadataExchangeResolver = MetadataExchangeResolver.Create(
                            new EndpointAddress(serviceUri),
                            _httpCredentialsProvider?.Clone() as IHttpCredentialsProvider,
                            _clientCertificatesProvider?.Clone() as IClientCertificateProvider,
                            _serverCertificateValidationProvider?.Clone() as IServerCertificateValidationProvider);

                        var metadataSections = await metadataExchangeResolver.ResolveMetadataAsync(cancellationToken).ConfigureAwait(false);
                        _metadataSections.AddRange(metadataSections);
                    }
                }
            }
            else
            {
                throw new MetadataExchangeException(MetadataResources.ErrInvalidUriFormat, uri);
            }
        }

        public static bool CanLoad(string uri, out Uri serviceUri)
        {
            return CanLoad(uri, string.Empty, string.Empty, out serviceUri);
        }

        public static bool CanLoad(string uri, string baseUrl, string basePath, out Uri serviceUri)
        {
            serviceUri = null;

            try
            {
                if (!string.IsNullOrWhiteSpace(uri))
                {
                    uri = uri.Trim(new char[] { '"' }).Trim();

                    var isUrl = uri.StartsWith(MetadataConstants.Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) ||
                                uri.StartsWith(MetadataConstants.Uri.UriSchemeNetTcp, StringComparison.OrdinalIgnoreCase) ||
                                uri.StartsWith(MetadataConstants.Uri.UriSchemeNetPipe, StringComparison.OrdinalIgnoreCase);

                    if (Uri.TryCreate(uri, UriKind.Absolute, out serviceUri) ||
                        isUrl && Uri.TryCreate(baseUrl, UriKind.Absolute, out Uri baseUri) && Uri.TryCreate(baseUri, uri, out serviceUri) ||
                       !isUrl && Uri.TryCreate(Path.Combine(basePath, uri), UriKind.Absolute, out serviceUri))
                    {
                        return serviceUri.Scheme == MetadataConstants.Uri.UriSchemeHttp ||
                            serviceUri.Scheme == MetadataConstants.Uri.UriSchemeHttps ||
                            serviceUri.Scheme == MetadataConstants.Uri.UriSchemeNetTcp ||
                            serviceUri.Scheme == MetadataConstants.Uri.UriSchemeNetPipe ||
                            serviceUri.Scheme == MetadataConstants.Uri.UriSchemeFile;
                    }
                }
            }
            catch
            {
            }

            return false;
        }

        internal async Task LoadFromStreamAsync(Stream stream, string uri, string basePath, CancellationToken cancellationToken)
        {
            using (var reader = XmlNS.XmlReader.Create(stream, new XmlNS.XmlReaderSettings() { XmlResolver = null, DtdProcessing = XmlNS.DtdProcessing.Ignore }))
            {
                MetadataFileType fileType = DetermineFileType(reader);

                switch (fileType)
                {
                    case MetadataFileType.Xsd:
                        await LoadAsXmlSchemaAsync(reader, uri, basePath, cancellationToken).ConfigureAwait(false);
                        break;
                    case MetadataFileType.Wsdl:
                        await LoadAsWsdlAsync(reader, uri, basePath, cancellationToken).ConfigureAwait(false);
                        break;
                    case MetadataFileType.Policy:
                        LoadAsPolicy(reader);
                        break;
                    case MetadataFileType.Epr:
                        await LoadAsEPRAsync(reader, cancellationToken).ConfigureAwait(false);
                        break;
                    case MetadataFileType.UnknownXml:
                        LoadAsUnknownXml(reader);
                        break;
                    default:
                        throw new MetadataExchangeException(MetadataResources.ErrNotXmlMetadataFileFormat, uri);
                }
            }
        }

        private async Task LoadAsXmlSchemaAsync(XmlNS.XmlReader reader, string uri, string basePath, CancellationToken cancellationToken)
        {
            var schema = await AsyncHelper.RunAsync(() => XmlNS.Schema.XmlSchema.Read(reader, null), cancellationToken).ConfigureAwait(false);

            _metadataSections.Add(MetadataSection.CreateFromSchema(schema));
            schema.SourceUri = uri;

            if (_resolveExternalDocs)
            {
                await LoadAsXmlSchemaIncludesAsync(schema, uri, basePath, cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task LoadAsXmlSchemaIncludesAsync(XmlNS.Schema.XmlSchema schema, string uri, string basePath, CancellationToken cancellationToken)
        {
            foreach (XmlNS.Schema.XmlSchemaExternal externalSchema in schema.Includes)
            {
                if (!string.IsNullOrEmpty(externalSchema.SchemaLocation))
                {
                    var schemaImport = externalSchema as XmlNS.Schema.XmlSchemaImport;
                    var schemaNamespace = schemaImport != null ? schemaImport.Namespace : schema.TargetNamespace;
                    var resolvedLocation = await LoadAsSchemaImportLocationAsync(externalSchema.SchemaLocation, uri, basePath, schemaNamespace, ".xsd", cancellationToken).ConfigureAwait(false);
                    externalSchema.SchemaLocation = resolvedLocation;
                }
            }
        }

        private async Task LoadAsWsdlAsync(XmlNS.XmlReader reader, string uri, string basePath, CancellationToken cancellationToken)
        {
            WsdlNS.ServiceDescription wsdl = await AsyncHelper.RunAsync(() => WsdlNS.ServiceDescription.Read(reader), cancellationToken).ConfigureAwait(false);

            wsdl.RetrievalUrl = uri;

            if (_resolveExternalDocs)
            {
                foreach (WsdlNS.Import import in wsdl.Imports)
                {
                    if (!string.IsNullOrWhiteSpace(import.Location))
                    {
                        var resolvedLocation = await LoadAsSchemaImportLocationAsync(import.Location, uri, basePath, import.Namespace, ".wsdl", cancellationToken).ConfigureAwait(false);
                        import.Location = resolvedLocation;
                    }
                }

                foreach (XmlNS.Schema.XmlSchema schema in wsdl.Types.Schemas)
                {
                    schema.SourceUri = uri;
                    await LoadAsXmlSchemaIncludesAsync(schema, wsdl.RetrievalUrl, basePath, cancellationToken).ConfigureAwait(false);
                }
            }

            _metadataSections.Add(MetadataSection.CreateFromServiceDescription(wsdl));
        }

        private async Task<string> LoadAsSchemaImportLocationAsync(string schemaLocation, string baseUrl, string basePath, string specNamespace, string fileExtension, CancellationToken cancellationToken)
        {
            string resolvedLocation = schemaLocation;
            try
            {
                if (TryGetSchemaUriFromSchemaImport(schemaLocation, baseUrl, basePath, specNamespace, fileExtension, out Uri schemaUri))
                {
                    if (schemaUri.Scheme == MetadataConstants.Uri.UriSchemeFile)
                    {
                        await LoadAsync(schemaUri.LocalPath, baseUrl, basePath, cancellationToken).ConfigureAwait(false);
                        resolvedLocation = schemaUri.LocalPath;
                    }
                    else
                    {
                        if (!IsUriProcessed(schemaUri.AbsoluteUri))
                        {
                            using (var stream = await DownloadSchemaImportAsync(schemaUri, cancellationToken).ConfigureAwait(false))
                            {
                                await LoadFromStreamAsync(stream, schemaUri.AbsoluteUri, basePath, cancellationToken).ConfigureAwait(false);
                                resolvedLocation = schemaUri.AbsoluteUri;
                            }
                        }
                    }
                }
                else
                {
                    throw new MetadataExchangeException(MetadataResources.ErrInvalidUriFormat, schemaLocation);
                }
            }
            catch (Exception ex)
            {
                if (Utils.IsFatalOrUnexpected(ex)) throw;
                _documentLoadExceptions.Add(ex);
            }

            return resolvedLocation;
        }

        private void LoadAsPolicy(XmlNS.XmlReader reader)
        {
            var doc = new XmlNS.XmlDocument();
            doc.XmlResolver = null;
            doc.Load(reader);
            XmlNS.XmlElement policy = doc.DocumentElement;
            _metadataSections.Add(MetadataSection.CreateFromPolicy(policy, null));
        }

        private void LoadAsUnknownXml(XmlNS.XmlReader reader)
        {
            var doc = new XmlNS.XmlDocument();
            doc.XmlResolver = null;
            doc.Load(reader);
            XmlNS.XmlElement unknownXml = doc.DocumentElement;
            _metadataSections.Add(new MetadataSection(null, null, unknownXml));
        }

        private async Task LoadAsEPRAsync(XmlNS.XmlReader reader, CancellationToken cancellationToken)
        {
            using (var dictionaryReader = XmlNS.XmlDictionaryReader.CreateDictionaryReader(reader))
            {
                EndpointAddress epr = await AsyncHelper.RunAsync(() => EndpointAddress.ReadFrom(dictionaryReader), cancellationToken).ConfigureAwait(false);

                MetadataExchangeResolver resolver = MetadataExchangeResolver.Create(epr, null, null, null);
                IEnumerable<MetadataSection> resolvedMetadata = await resolver.ResolveMetadataAsync(cancellationToken).ConfigureAwait(false);
                _metadataSections.AddRange(resolvedMetadata);
            }
        }

        private bool IsUriProcessed(string uri)
        {
            return CollectionContainsOrAdd(_processedUri, uri);
        }

        private bool IsSchemaUpdated(string uri)
        {
            return CollectionContainsOrAdd(_updatedUri, uri);
        }

        private bool CollectionContainsOrAdd(List<string> uriCollection, string uri)
        {
            var exists = false;

            if (!string.IsNullOrEmpty(uri))
            {
                var uriKey = uri.ToUpperInvariant();

                exists = uriCollection.Contains(uriKey);
                if (!exists)
                {
                    uriCollection.Add(uriKey);
                }
            }

            return exists;
        }

        private bool TryGetSchemaUriFromSchemaImport(string schemaLocation, string baseUrl, string basePath, string specNamespace, string extension, out Uri schemaUri)
        {
            schemaUri = null;

            // Try to resolve the URI for relative (local/remote) metadata doc location.
            // Examples:
            // schemaLocation = "myservice.xsd"
            // baseUrl = http://myhost/myservice.svc
            // basePath = C:\MyService\wsdl
            // specNamespace = "http://temporg.com"
            // extension = ".xsd"
            // schemaUri => http:\\myhost\myservice.xsd OR C:\MyService\wsld\myservice.xsd (if already downloded).

            if (Uri.TryCreate(schemaLocation, UriKind.Absolute, out schemaUri))
            {
                if (TryGetExistingSchemaFile(schemaUri, schemaLocation, basePath, specNamespace, extension, out Uri schemaFile))
                {
                    schemaUri = schemaFile;
                }
            }
            else
            {
                // check if the schema location refers to an existing file.
                if (Uri.TryCreate(Path.Combine(basePath, schemaLocation), UriKind.Absolute, out Uri schemaFile))
                {
                    if (TryGetExistingSchemaFile(schemaFile, schemaLocation, basePath, specNamespace, extension, out schemaFile))
                    {
                        schemaUri = schemaFile;
                    }
                }

                if (schemaUri == null && Uri.TryCreate(baseUrl, UriKind.Absolute, out Uri baseUri))
                {
                    // make it an absolute URL so it can be downloaded.
                    Uri.TryCreate(baseUri, schemaLocation, out schemaUri);
                }
            }

            return schemaUri != null;
        }

        /// <summary>
        /// Attempts to resolve a metadata document in disk (wsdl/schema file).
        /// </summary>
        /// <param name="schemaUri">The resolved URI for the specified schema file.</param>
        /// <param name="schemaLocation">The original location of the schema file, this could be a relative path.</param>
        /// <param name="basePath">The wsdl docs path in disk.</param>
        /// <param name="specNamespace">The namespace for the file document (schema/wsdl)</param>
        /// <param name="extension">The extension of the document file in disk (.xsd/.wsdl)</param>
        /// <param name="schemaFile">The resolved URI for the document file.</param>
        /// <returns>True, if the file exists; false otherwise.</returns>
        private bool TryGetExistingSchemaFile(Uri schemaUri, string schemaLocation, string basePath, string specNamespace, string extension, out Uri schemaFile)
        {
            // Try to get the file name.

            string fullFileName = schemaUri.IsFile ? schemaUri.LocalPath : schemaLocation;
            schemaFile = null;

            // check if the file has already been downloaded using some heuristics.

            if (!File.Exists(fullFileName))
            {
                if (schemaUri.Segments.Length > 0)
                {
                    // the URI may contain the file name.
                    fullFileName = Path.Combine(basePath, schemaUri.Segments[schemaUri.Segments.Length - 1]);
                }

                if (!File.Exists(fullFileName) && !string.IsNullOrWhiteSpace(specNamespace))
                {
                    // reconstruct the local file name for the remote location (assuming files downloaded by MetadataDocumentSaver):
                    // <xsd:import schemaLocation="http://serverhost/servcicename.svc?xsd=xsd0" namespace="http://tempuri.org/" />
                    // observe that schema include specs are not processed because on download they are made part of the container schema (must belong to the same namespace).
                    // <xsd:include schemaLocation="xsd1.xsd" />
                    fullFileName = MetadataFileNameManager.GetFilePathFromNamespace(basePath, specNamespace, extension);
                }

                if (!File.Exists(fullFileName) && !Path.IsPathRooted(schemaLocation))
                {
                    fullFileName = Path.Combine(basePath, schemaLocation);

                    if (!File.Exists(fullFileName))
                    {
                        fullFileName = Path.Combine(basePath, Path.GetFileName(schemaLocation));
                    }
                }
            }

            if (File.Exists(fullFileName))
            {
                Uri.TryCreate(fullFileName, UriKind.Absolute, out schemaFile);
            }

            return schemaFile != null;
        }

        private async Task<Stream> DownloadSchemaImportAsync(Uri schemaUri, CancellationToken cancellationToken)
        {
            MetadataExchangeResolver metadataExchangeResolver = MetadataExchangeResolver.Create(
                                new EndpointAddress(schemaUri),
                                _httpCredentialsProvider?.Clone() as IHttpCredentialsProvider,
                                _clientCertificatesProvider?.Clone() as IClientCertificateProvider,
                                _serverCertificateValidationProvider?.Clone() as IServerCertificateValidationProvider);

            return await metadataExchangeResolver.DownloadMetadataFileAsync(cancellationToken).ConfigureAwait(false);
        }

        private static void FixupChameleonSchemas(IEnumerable<MetadataSection> metadataSections)
        {
            // Chameleon schemas are those w/o a target namespace, types from these schemas become part of the namespace of the enclosing schema.

            var documents = metadataSections.Select((s) => s.Metadata);
            var schemas = documents.OfType<XmlNS.Schema.XmlSchema>();
            var chameleonSchemas = schemas.Where(s => string.IsNullOrEmpty(s.TargetNamespace));

            foreach (var schema in schemas)
            {
                foreach (XmlNS.Schema.XmlSchemaExternal include in schema.Includes)
                {
                    var chameleonSchema = chameleonSchemas.FirstOrDefault(c =>
                        c != schema && MetadataFileNameManager.UriEqual(MetadataFileNameManager.GetComposedUri(c.SourceUri, null), MetadataFileNameManager.GetComposedUri(schema.SourceUri, include.SchemaLocation)));

                    if (chameleonSchema != null)
                    {
                        include.Schema = chameleonSchema;
                    }
                }
            }
        }

        private static MetadataFileType DetermineFileType(XmlNS.XmlReader reader)
        {
            try
            {
                if (reader.IsStartElement(MetadataConstants.WSDL.Elements.Root, MetadataConstants.WSDL.NamespaceUri))
                {
                    return MetadataFileType.Wsdl;
                }
                else if (reader.IsStartElement(MetadataConstants.XmlSchema.Elements.Root, MetadataConstants.XmlSchema.NamespaceUri))
                {
                    return MetadataFileType.Xsd;
                }
                else if (reader.IsStartElement(MetadataConstants.WSPolicy.Elements.Policy, MetadataConstants.WSPolicy.NamespaceUri)
                    || reader.IsStartElement(MetadataConstants.WSPolicy.Elements.Policy, MetadataConstants.WSPolicy.NamespaceUri15))
                {
                    return MetadataFileType.Policy;
                }
                else if (reader.IsStartElement(MetadataConstants.WSAddressing.Elements.EndpointReference, MetadataConstants.WSAddressing.NamespaceUri))
                {
                    return MetadataFileType.Epr;
                }
                else
                {
                    return MetadataFileType.UnknownXml;
                }
            }
            catch (XmlNS.XmlException)
            {
                //This must mean that the document isn't an XML Document so we continue trying other things...
                return MetadataFileType.NonXml;
            }
        }

        private void UpdateMetadataSourceUri()
        {
            foreach (var section in _metadataSections)
            {
                if (section.Dialect == MetadataSection.ServiceDescriptionDialect)
                {
                    var wsdl = section.Metadata as WsdlNS.ServiceDescription;
                    if (string.IsNullOrEmpty(wsdl.RetrievalUrl))
                    {
                        wsdl.RetrievalUrl = GetSourceUrl(section);
                    }
                    foreach (XmlNS.Schema.XmlSchema embeddedSchema in wsdl.Types.Schemas)
                    {
                        UpdateSchemaSourceUri(embeddedSchema, wsdl.RetrievalUrl);
                    }
                    continue;
                }

                if (section.Dialect == MetadataSection.XmlSchemaDialect)
                {
                    UpdateSchemaSourceUri(section.Metadata as XmlNS.Schema.XmlSchema, GetSourceUrl(section));
                    continue;
                }
            }
        }

        private void UpdateSchemaSourceUri(XmlNS.Schema.XmlSchema schema, string sourceUri)
        {
            if (schema != null && !IsSchemaUpdated(schema.SourceUri))
            {
                if (string.IsNullOrEmpty(schema.SourceUri))
                {
                    schema.SourceUri = sourceUri;
                }

                foreach (XmlNS.Schema.XmlSchemaExternal include in schema.Includes)
                {
                    if (include.Schema != null)
                    {
                        UpdateSchemaSourceUri(include.Schema, schema.SourceUri);
                    }
                }
            }
        }

        protected string GetSourceUrl(MetadataSection section)
        {
            // The SourceUrl property of the MetadataSection is set by the WCF MetadataExchangeClient when downloading metadata from a web server.
            // This property is not set when the metadata comes from a WSDL file in disk as the WSDL file processing code is not in WCF.
#if NETCORE
            return section.SourceUrl;
#else
            return section.GetType().GetProperty("SourceUrl", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(section) as string;
#endif
        }

        public override bool Equals(object obj)
        {
            var other = obj as MetadataDocumentLoader;
            return other != null && this.GetHashCode() == other.GetHashCode();
        }

        /// <summary>
        /// An instance of this class must be associated with a specific hash code in order to ensure the object's state is valid.
        /// in the case the load operation is cancelled the object could still be updated from a background thread that have not yet been cancelled for instance.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if (!_hashCode.HasValue)
            {
                const string MexUri = "/MEX";
                string key = null;

                if (this.metadataSourceFiles.Count == 1)
                {
                    var metadataUri = metadataSourceFiles[0];

                    // remove any '?wsdl' query or the '/mex' param.
                    key = metadataUri.AbsoluteUri.ToUpperInvariant();

                    if (!string.IsNullOrEmpty(metadataUri.Query))
                    {
                        key = key.Remove(key.Length - metadataUri.Query.Length);
                    }
                    if (key.EndsWith(MexUri, StringComparison.OrdinalIgnoreCase))
                    {
                        key = key.Remove(key.Length - MexUri.Length);
                    }
                }
                else
                {
                    StringBuilder keyBuilder = new StringBuilder();
                    var orderedFileNames = this.metadataSourceFiles.Select(u => u.ToString().ToUpperInvariant()).OrderBy(u => u);
                    foreach (var fileName in orderedFileNames)
                    {
                        keyBuilder.Append(fileName);
                    }
                    key = keyBuilder.ToString();
                }

                _hashCode = key.GetHashCode();
            }

            return _hashCode.Value;
        }

        private enum MetadataFileType
        {
            NonXml,
            Wsdl,
            Xsd,
            Policy,
            Epr,
            UnknownXml
        }
    }
}
