using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Infrastructure.Common;
using TestTypes;

namespace ClientMessageInspector
{
    public enum AuthenticationType
    {
        None,
        Live
    }

    public class MI_ClientBase_ClientAuth : MI_ClientBase<IUser>
    {
        public MI_ClientBase_ClientAuth(string authType, string accessToken, BasicHttpBinding binding, EndpointAddress endPoint)
            : base(authType, accessToken, binding, endPoint)
        {
        }

        public ResultObject<string> GetAuthToken()
        {
            return base.Call<ResultObject<string>>(delegate
            {
                return this.Channel.UserGetAuthToken();
            });
        }

        public Dictionary<string, string> ValidateHeaders()
        {
            return this.Channel.ValidateMessagePropertyHeaders();
        }
    }

    public class MI_ClientBase<T> : ClientBase<T> where T : class
    {
        public MI_ClientBase(string authType, string accessToken, BasicHttpBinding binding, EndpointAddress endPoint)
            : base(binding, endPoint)
        {
            base.Endpoint.EndpointBehaviors.Add(new ClientAuthorizationBehavior(authType, accessToken));
        }

        public TResult Call<TResult>(Func<TResult> action) where TResult : class
        {
            try
            {
                using (new OperationContextScope((IContextChannel)base.InnerChannel))
                {
                    return action();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public class ClientAuthorizationBehavior : IEndpointBehavior
    {
        private readonly string _accessToken;
        private readonly string _authenticationType;

        public ClientAuthorizationBehavior(string authenticationType, string accessToken)
        {
            _accessToken = "";
            _authenticationType = "";
            _accessToken = accessToken;
            _authenticationType = authenticationType;
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.ClientMessageInspectors.Add(new ClientMessageInspector(_authenticationType, _accessToken));
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }
    }

    public class ClientMessageInspector : IClientMessageInspector
    {
        private string _accessToken;
        private string _authenticationType;

        public ClientMessageInspector(string authenticationType, string accessToken)
        {
            _authenticationType = "";
            _accessToken = "";
            _authenticationType = authenticationType;
            _accessToken = accessToken;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            HttpRequestMessageProperty property;
            object obj2;
            if (request.Properties.TryGetValue(HttpRequestMessageProperty.Name, out obj2))
            {
                property = obj2 as HttpRequestMessageProperty;
                property.Headers["authType"] = _authenticationType;
                property.Headers[HttpRequestHeader.Authorization] = _accessToken;
            }
            else
            {
                property = new HttpRequestMessageProperty();
                property.Headers["authType"] = _authenticationType;
                property.Headers[HttpRequestHeader.Authorization] = _accessToken;
                request.Properties.Add(HttpRequestMessageProperty.Name, property);
            }
            return null;
        }
    }
}
