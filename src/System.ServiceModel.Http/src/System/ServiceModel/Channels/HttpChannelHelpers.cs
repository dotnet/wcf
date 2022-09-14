// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Contracts;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Security.Policy;
using System.Security.Principal;
using System.ServiceModel.Security;
using System.ServiceModel.Security.Tokens;
using System.Text;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal enum HttpAbortReason
    {
        None,
        Aborted,
        TimedOut
    }

    internal static class HttpChannelUtilities
    {
        internal static class StatusDescriptionStrings
        {
            internal const string HttpContentTypeMissing = "Missing Content Type";
            internal const string HttpContentTypeMismatch = "Cannot process the message because the content type '{0}' was not the expected type '{1}'.";
            internal const string HttpStatusServiceActivationException = "System.ServiceModel.ServiceActivationException";
        }

        internal const string HttpStatusCodeExceptionKey = "System.ServiceModel.Channels.HttpInput.HttpStatusCode";
        internal const string HttpStatusDescriptionExceptionKey = "System.ServiceModel.Channels.HttpInput.HttpStatusDescription";
        internal const string HttpRequestHeadersTypeName = "System.Net.Http.Headers.HttpRequestHeaders";


        internal const int ResponseStreamExcerptSize = 1024;
        internal const string MIMEVersionHeader = "MIME-Version";
        internal const string ContentEncodingHeader = "Content-Encoding";

        internal const uint CURLE_SSL_CERTPROBLEM = 58;
        internal const uint CURLE_SSL_CACERT = 60;

        internal const uint ERROR_INVALID_HANDLE = 6;

        internal const uint WININET_E_NAME_NOT_RESOLVED = 0x80072EE7;
        internal const uint WININET_E_CONNECTION_RESET = 0x80072EFF;
        internal const uint WININET_E_INCORRECT_HANDLE_STATE = 0x80072EF3;
        internal const uint ERROR_WINHTTP_SECURE_FAILURE = 0x80072f8f;

        public static Task<(NetworkCredential networkCredential, TokenImpersonationLevel impersonationLevel, AuthenticationLevel authenticationLevel)> GetCredentialAsync(
            AuthenticationSchemes authenticationScheme, SecurityTokenProviderContainer credentialProvider,
            TimeSpan timeout)
        {
            if (authenticationScheme == AuthenticationSchemes.Anonymous)
            {
                var result = ((NetworkCredential)null, TokenImpersonationLevel.None, AuthenticationLevel.None);
                return Task.FromResult(result);
            }

            return GetCredentialCoreAsync(authenticationScheme, credentialProvider, timeout);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static async Task<(NetworkCredential credential, TokenImpersonationLevel impersonationLevel, AuthenticationLevel authenticationLevel)>GetCredentialCoreAsync(
            AuthenticationSchemes authenticationScheme, SecurityTokenProviderContainer credentialProvider, TimeSpan timeout)
        {
            switch (authenticationScheme)
            {
                case AuthenticationSchemes.Basic:
                    var userNameCreds = await TransportSecurityHelpers.GetUserNameCredentialAsync(credentialProvider, timeout);
                    return (userNameCreds, TokenImpersonationLevel.Delegation, AuthenticationLevel.None);

                case AuthenticationSchemes.Digest:
                    return await TransportSecurityHelpers.GetSspiCredentialAsync(credentialProvider, timeout);

                case AuthenticationSchemes.Negotiate:
                    return await TransportSecurityHelpers.GetSspiCredentialAsync(credentialProvider, timeout);

                case AuthenticationSchemes.Ntlm:
                case AuthenticationSchemes.IntegratedWindowsAuthentication: // IWA could use NTLM
                    var result = await TransportSecurityHelpers.GetSspiCredentialAsync(credentialProvider, timeout);
                    if (result.authenticationLevel == AuthenticationLevel.MutualAuthRequired)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                            new InvalidOperationException(SR.CredentialDisallowsNtlm));
                    }
                    return result;

                default:
                    // The setter for this property should prevent this.
                    throw Fx.AssertAndThrow("GetCredential: Invalid authentication scheme");
            }
        }

        public static HttpResponseMessage ProcessGetResponseWebException(HttpRequestException requestException, HttpRequestMessage request, HttpAbortReason abortReason)
        {
            var inner = requestException.InnerException;
            if (inner != null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(ConvertHttpRequestException(requestException, request, abortReason));
            }
            else
            {
                // There is no inner exception so there's not enough information to be able to convert to the correct WCF exception.
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationException(requestException.Message, requestException));
            }
        }

        public static Exception ConvertHttpRequestException(HttpRequestException exception, HttpRequestMessage request, HttpAbortReason abortReason)
        {
            Contract.Assert(exception.InnerException != null, "InnerException must be set to be able to convert");

            uint hresult = (uint)exception.InnerException.HResult;
            var innerSocketException = exception.InnerException as SocketException;
            if (innerSocketException != null)
            {
                SocketError socketErrorCode = innerSocketException.SocketErrorCode;
                switch (socketErrorCode)
                {
                    case SocketError.TryAgain:
                    case SocketError.NoRecovery:
                    case SocketError.NoData:
                    case SocketError.HostNotFound:
                        return new EndpointNotFoundException(SR.Format(SR.EndpointNotFound, request.RequestUri.AbsoluteUri), exception);
                    default:
                        break;
                }
            }

            if (exception.InnerException is AuthenticationException)
            {
                return new SecurityNegotiationException(SR.Format(SR.TrustFailure, request.RequestUri.Authority), exception);
            }

            switch (hresult)
            {
                // .Net Native HttpClientHandler sometimes reports an incorrect handle state when a connection is aborted, so we treat it as a connection reset error
                case WININET_E_INCORRECT_HANDLE_STATE:
                    goto case WININET_E_CONNECTION_RESET;
                case WININET_E_CONNECTION_RESET:
                    return new CommunicationException(SR.Format(SR.HttpReceiveFailure, request.RequestUri), exception);
                // Linux HttpClient returns ERROR_INVALID_HANDLE in the endpoint-not-found case, so map to EndpointNotFoundException
                case ERROR_INVALID_HANDLE:
                case WININET_E_NAME_NOT_RESOLVED:
                    return new EndpointNotFoundException(SR.Format(SR.EndpointNotFound, request.RequestUri.AbsoluteUri), exception);
                case CURLE_SSL_CACERT:
                case CURLE_SSL_CERTPROBLEM:
                case ERROR_WINHTTP_SECURE_FAILURE:
                    return new SecurityNegotiationException(SR.Format(SR.TrustFailure, request.RequestUri.Authority), exception);
                default:
                    return new CommunicationException(exception.Message, exception);
            }
        }

        internal static Exception CreateUnexpectedResponseException(HttpResponseMessage response)
        {
            string statusDescription = response.ReasonPhrase;
            if (string.IsNullOrEmpty(statusDescription))
            {
                statusDescription = response.StatusCode.ToString();
            }

            return TraceResponseException(
                new ProtocolException(SR.Format(SR.UnexpectedHttpResponseCode,
                (int)response.StatusCode, statusDescription)));
        }

        internal static string GetResponseStreamExcerptString(Stream responseStream, ref int bytesToRead)
        {
            long bufferSize = bytesToRead;

            if (bufferSize < 0 || bufferSize > ResponseStreamExcerptSize)
            {
                bufferSize = ResponseStreamExcerptSize;
            }

            byte[] responseBuffer = Fx.AllocateByteArray(checked((int)bufferSize));
            bytesToRead = responseStream.Read(responseBuffer, 0, (int)bufferSize);
            responseStream.Dispose();

            return Encoding.UTF8.GetString(responseBuffer, 0, bytesToRead);
        }

        internal static Exception TraceResponseException(Exception exception)
        {
            return exception;
        }

        internal static ProtocolException CreateHttpProtocolException(string message, HttpStatusCode statusCode, string statusDescription)
        {
            ProtocolException exception = new ProtocolException(message);
            exception.Data.Add(HttpChannelUtilities.HttpStatusCodeExceptionKey, statusCode);
            if (statusDescription != null && statusDescription.Length > 0)
            {
                exception.Data.Add(HttpChannelUtilities.HttpStatusDescriptionExceptionKey, statusDescription);
            }

            return exception;
        }
    }
}
