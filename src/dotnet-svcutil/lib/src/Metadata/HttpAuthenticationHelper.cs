// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Net;
using System.Text;

namespace Microsoft.Tools.ServiceModel.Svcutil.Metadata
{
    //
    // Note: some of this code was copied/edited from wizard/vsdesigner/Microsoft.VSDesigner.WCF/WebDiscovery/HttpUtils.cs
    //

    internal static class HttpAuthenticationHelper
    {
        /// <summary>
        /// Used to fish out authentication related exceptions from the specified exception.
        /// </summary>
        public static T GetException<T>(Exception exception) where T : class
        {
            while (exception != null)
            {
                if (exception is T) return exception as T;
                exception = exception.InnerException;
            }
            return null;
        }

        /// <summary>
        /// Get unauthorized response from exception (if any).
        /// </summary>
        public static HttpWebResponse GetUnauthorizedResponse(WebException webException)
        {
            while (webException != null)
            {
                if (webException.Response is HttpWebResponse response &&      // user auth required.
                   ((response.StatusCode == HttpStatusCode.Unauthorized) || (response.StatusCode == HttpStatusCode.ProxyAuthenticationRequired)))
                {
                    return response;
                }
                webException = GetException<WebException>(webException.InnerException);
            }
            return null;
        }

        /// <summary>
        /// Get forbidden response from exception (if any).
        /// </summary>
        public static HttpWebResponse GetForbiddenResponse(WebException webException)
        {
            if (webException != null && webException.Status == WebExceptionStatus.ProtocolError)
            {
                while (webException != null)
                {
                    if (webException.Response is HttpWebResponse response && response.StatusCode == HttpStatusCode.Forbidden)
                    {
                        return response;
                    }
                    webException = GetException<WebException>(webException.InnerException);
                }
            }
            return null;
        }

        /// <summary>
        /// Get authentication information from the web reponse.
        /// </summary>
        public static void GetAuthenticationInformation(HttpWebResponse httpResponse, out string[] authenticationSchemes, out string realm)
        {
            if (httpResponse == null)
            {
                throw new ArgumentNullException(nameof(httpResponse));
            }
            string challenge = httpResponse.Headers[HttpResponseHeader.WwwAuthenticate];
            if (challenge != null)
            {
                GetAuthenticationInformation(challenge, out authenticationSchemes, out realm);
            }
            else
            {
                authenticationSchemes = new string[0];
                realm = string.Empty;
            }
        }

        /// <summary>
        /// extract information from challenge string
        /// </summary>
        private static void GetAuthenticationInformation(string challenge, out string[] authenticationSchemes, out string realm)
        {
            if (challenge == null)
            {
                throw new ArgumentNullException(nameof(challenge));
            }
            realm = string.Empty;
            string schemesString = null;
            int spaceIndex = challenge.IndexOf(' ');
            if (spaceIndex >= 0)
            {
                schemesString = challenge.Substring(0, spaceIndex);
                realm = GetRealmFromChallenge(challenge.Substring(spaceIndex + 1).Trim());
            }
            else
            {
                schemesString = challenge;
            }
            authenticationSchemes = schemesString.Split(',');
        }


        /// <summary>
        /// Extract a quote string value
        /// </summary>
        private static string GetQuoteString(string originalString, ref int index)
        {
            StringBuilder quoteString = new StringBuilder();
            while (index < originalString.Length)
            {
                if (originalString[index] == '"')
                {
                    return quoteString.ToString();
                }

                if (originalString[index] == '\\' && index + 1 < originalString.Length)
                {
                    index++;
                }
                quoteString.Append(originalString[index]);

                index++;
            }
            return string.Empty;
        }

        /// <summary>
        /// helper function to extract realm from challenge string
        /// </summary>
        private static string GetRealmFromChallenge(string challenge)
        {
            const string realmTokenStart = "realm=\"";
            while (challenge.Length > 0)
            {
                if (challenge.StartsWith(realmTokenStart, StringComparison.OrdinalIgnoreCase))
                {
                    int quotedIndex = 0;
                    string value = challenge.Substring(realmTokenStart.Length);
                    string quoteValue = GetQuoteString(value, ref quotedIndex);
                    return quoteValue;
                }

                // find first ',' which is not in a quoted string
                int i = 0;
                for (i = 0; i < challenge.Length; i++)
                {
                    if (challenge[i] == ',')
                    {
                        break;
                    }
                    else if (challenge[i] == '"')
                    {
                        i++;

                        // skip a quote string
                        GetQuoteString(challenge, ref i);
                    }
                }

                // skip them
                if (i + 1 < challenge.Length)
                {
                    challenge = challenge.Substring(i + 1);
                }
                else
                {
                    break;
                }
            }
            return string.Empty;
        }
    }
}
