// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF;
using CoreWCF.Channels;
using CoreWCF.Description;
using CoreWCF.Dispatcher;
#else
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
#endif
using System.Collections.ObjectModel;
using System.Net;
using System.Security.Principal;
using System.Text;

namespace WcfService
{
    public abstract class BasicServiceAuthorizationManager : ServiceAuthorizationManager, IServiceBehavior
    {
        private readonly string _realm;

        public BasicServiceAuthorizationManager(string realm)
        {
            if (realm == null)
            {
                realm = string.Empty;
            }

            _realm = realm;
        }

        private bool Authorized(BasicAuthenticationState basicState, OperationContext operationContext, ref Message message)
        {
            object identitiesListObject;
            if (!operationContext.ServiceSecurityContext.AuthorizationContext.Properties.TryGetValue("Identities",
                out identitiesListObject))
            {
                identitiesListObject = new List<IIdentity>(1);
                operationContext.ServiceSecurityContext.AuthorizationContext.Properties.Add("Identities", identitiesListObject);
            }

            var identities = identitiesListObject as IList<IIdentity>;
            identities.Add(new GenericIdentity(basicState.Username, "GenericPrincipal"));

            return true;
        }

        [Obsolete]
        public override bool CheckAccess(OperationContext operationContext, ref Message message)
        {
            var basicState = new BasicAuthenticationState(operationContext, GetRealm(ref message));
            if (!basicState.IsRequestBasicAuth)
            {
                return UnauthorizedResponse(basicState);
            }

            string password;
            if (!GetPassword(ref message, basicState.Username, out password))
            {
                return UnauthorizedResponse(basicState);
            }

            if(basicState.Password != password)
            {
                // According to RFC2616, a forbidden response should be in response to valid credentials where the
                // authenticated user is not allowed to use the site but WCF responds with Forbbiden with an incorrect
                // password. We should be returning Unauthorized, but this matches WCF behavior.
                return ForbiddenResponse(basicState);
            }

            return Authorized(basicState, operationContext, ref message);
        }

        public virtual string GetRealm(ref Message message)
        {
            return _realm;
        }

        public abstract bool GetPassword(ref Message message, string username, out string password);

        private bool UnauthorizedResponse(BasicAuthenticationState basicState)
        {
            basicState.SetChallengeResponse(HttpStatusCode.Unauthorized, "Access Denied");
            return false;
        }

        private bool ForbiddenResponse(BasicAuthenticationState basicState)
        {
            basicState.SetChallengeResponse(HttpStatusCode.Forbidden, "Access Denied");
            return false;
        }

        #region IServiceBehavior
        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase) { }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints,
            BindingParameterCollection bindingParameters)
        { }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            serviceHostBase.Authorization.ServiceAuthorizationManager = this;
            foreach (ChannelDispatcherBase t in serviceHostBase.ChannelDispatchers)
            {
                ChannelDispatcher channelDispatcher = t as ChannelDispatcher;
                foreach (EndpointDispatcher endpointDispatcher in channelDispatcher.Endpoints)
                {
                    endpointDispatcher.DispatchRuntime.ServiceAuthorizationManager = this;
                }
            }
        }
        #endregion

        private struct BasicAuthenticationState
        {
            private const string BasicAuthenticationMechanism = "Basic ";
            private const int BasicAuthenticationMechanismLength = 6; // BasicAuthenticationMechanism.Length;
            private const string RealmAuthenticationParameter = "realm";
            private const string AuthenticationChallengeHeaderName = "WWW-Authenticate";
            private const string AuthorizationHeaderName = "Authorization";
            private readonly string _authorizationHeader;
            private readonly OperationContext _operationContext;
            private readonly string _realm;
            private string _username;
            private string _password;

            public BasicAuthenticationState(OperationContext operationContext, string realm)
            {
                _operationContext = operationContext;
                _realm = realm;
                _username = _password = string.Empty;
                _authorizationHeader = GetAuthorizationHeader(operationContext);
                if (_authorizationHeader.Length < BasicAuthenticationMechanismLength)
                {
                    return;
                }

                string authBase64Encoded = _authorizationHeader.Substring(BasicAuthenticationMechanismLength);
                var authDecodedBytes = Convert.FromBase64String(authBase64Encoded);
                var authDecoded = Encoding.UTF8.GetString(authDecodedBytes);
                int colonPos = authDecoded.IndexOf(':');
                if(colonPos <= 0)
                {
                    return;
                }
                _username = authDecoded.Substring(0, colonPos);
                _password = authDecoded.Substring(colonPos + 1);
            }

            public bool IsRequestBasicAuth { get { return _authorizationHeader.StartsWith(BasicAuthenticationMechanism); } }

            public string Username { get { return _username; } }

            public string Password { get { return _password; } }

            public void SetChallengeResponse(HttpStatusCode statusCode, string statusDescription)
            {
                StringBuilder authChallenge = new StringBuilder(BasicAuthenticationMechanism);
                authChallenge.AppendFormat(RealmAuthenticationParameter + "=\"{0}\", ", _realm);

                object responsePropertyObject;
                if (!_operationContext.OutgoingMessageProperties.TryGetValue(HttpResponseMessageProperty.Name, out responsePropertyObject))
                {
                    responsePropertyObject = new HttpResponseMessageProperty();
                    _operationContext.OutgoingMessageProperties[HttpResponseMessageProperty.Name] = responsePropertyObject;
                }

                var responseMessageProperty = (HttpResponseMessageProperty)responsePropertyObject;
                responseMessageProperty.Headers[AuthenticationChallengeHeaderName] = authChallenge.ToString();
                responseMessageProperty.StatusCode = statusCode;
                responseMessageProperty.StatusDescription = statusDescription;
            }

            private static string GetAuthorizationHeader(OperationContext operationContext)
            {
                object requestMessagePropertyObject;
                if (!operationContext.IncomingMessageProperties.TryGetValue(HttpRequestMessageProperty.Name,
                        out requestMessagePropertyObject))
                {
                    throw new InvalidOperationException("Not an HTTP request");
                }

                HttpRequestMessageProperty requestMessageProperties = (HttpRequestMessageProperty)requestMessagePropertyObject;
                var requestHeaders = requestMessageProperties.Headers;

                var authorizationHeader = requestHeaders[AuthorizationHeaderName];
                if (authorizationHeader == null)
                {
                    authorizationHeader = string.Empty;
                }

                return authorizationHeader.Trim();
            }
        }
    }
}
