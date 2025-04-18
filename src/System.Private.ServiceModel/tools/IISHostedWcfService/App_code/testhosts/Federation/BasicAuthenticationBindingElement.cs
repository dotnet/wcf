// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !NET
using System;
using System.IdentityModel.Policy;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Text;

namespace WcfService
{
    public class BasicAuthenticationBindingElement : BindingElement
    {
        public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if(typeof(TChannel) == typeof(IReplyChannel))
                return (IChannelListener<TChannel>)new BasicAuthenticationChannelListener(context);

            throw new ArgumentException("Only IReplyChannel supported", "TChannel");
        }

        public override bool CanBuildChannelListener<TChannel>(BindingContext context)
        {
            if (typeof(TChannel) != typeof(IReplyChannel))
                return false;

            return base.CanBuildChannelListener<TChannel>(context);
        }

        public override BindingElement Clone()
        {
            return this;
        }

        public override T GetProperty<T>(BindingContext context)
        {
            return context.GetInnerProperty<T>();
        }
    }

    internal class BasicAuthenticationChannelListener : IChannelListener<IReplyChannel>
    {
        private IChannelListener<IReplyChannel> _innerListener;
        private ServiceCredentials _serviceCredentials;
        private CustomUserNameSecurityTokenAuthenticator _userNameTokenAuthenticator;

        public BasicAuthenticationChannelListener(BindingContext context)
        {
            _serviceCredentials = context.BindingParameters.Find<ServiceCredentials>();
            if (_serviceCredentials.UserNameAuthentication.UserNamePasswordValidationMode != UserNamePasswordValidationMode.Custom)
            {
                throw new InvalidOperationException("You must use UserNamePasswordValidationMode.Custom");
            }

            _userNameTokenAuthenticator = new CustomUserNameSecurityTokenAuthenticator(_serviceCredentials.UserNameAuthentication.CustomUserNamePasswordValidator);
            _innerListener = context.BuildInnerChannelListener<IReplyChannel>();
        }

        internal SecurityTokenAuthenticator UserNameTokenAuthenticator { get { return _userNameTokenAuthenticator; } }

        public Uri Uri
        {
            get
            {
                return _innerListener.Uri;
            }
        }

        public CommunicationState State
        {
            get
            {
                return _innerListener.State;
            }
        }

        public event EventHandler Closed
        {
            add { _innerListener.Closed += value; }
            remove { _innerListener.Closed -= value; }
        }

        public event EventHandler Closing
        {
            add { _innerListener.Closing += value; }
            remove { _innerListener.Closing -= value; }
        }

        public event EventHandler Faulted
        {
            add { _innerListener.Faulted += value; }
            remove { _innerListener.Faulted -= value; }
        }

        public event EventHandler Opened
        {
            add { _innerListener.Opened += value; }
            remove { _innerListener.Opened -= value; }
        }

        public event EventHandler Opening
        {
            add { _innerListener.Opening += value; }
            remove { _innerListener.Opening -= value; }
        }

        public IReplyChannel AcceptChannel()
        {
            var inner = _innerListener.AcceptChannel();
            if (inner != null)
            {
                return new BasicAuthenticationChannel(this, inner);
            }

            return null;
        }

        public IReplyChannel AcceptChannel(TimeSpan timeout)
        {
            var inner = _innerListener.AcceptChannel(timeout);
            if (inner != null)
            {
                return new BasicAuthenticationChannel(this, inner);
            }

            return null;
        }

        public IAsyncResult BeginAcceptChannel(AsyncCallback callback, object state)
        {
            return _innerListener.BeginAcceptChannel(callback, state);
        }

        public IAsyncResult BeginAcceptChannel(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _innerListener.BeginAcceptChannel(timeout, callback, state);
        }

        public IReplyChannel EndAcceptChannel(IAsyncResult result)
        {
            var inner = _innerListener.EndAcceptChannel(result);
            if (inner != null)
            {
                return new BasicAuthenticationChannel(this, inner);
            }

            return null;
        }

        public bool WaitForChannel(TimeSpan timeout)
        {
            return _innerListener.WaitForChannel(timeout);
        }

        public IAsyncResult BeginWaitForChannel(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _innerListener.BeginWaitForChannel(timeout, callback, state);
        }

        public bool EndWaitForChannel(IAsyncResult result)
        {
            return _innerListener.EndWaitForChannel(result);
        }

        public void Abort()
        {
            _innerListener.Abort();
        }

        public void Close()
        {
            _innerListener.Close();
        }

        public void Close(TimeSpan timeout)
        {
            _innerListener.Close(timeout);
        }

        public IAsyncResult BeginClose(AsyncCallback callback, object state)
        {
            return _innerListener.BeginClose(callback, state);
        }

        public IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _innerListener.BeginClose(timeout, callback, state);
        }

        public void EndClose(IAsyncResult result)
        {
            _innerListener.EndClose(result);
        }

        public void Open()
        {
            _innerListener.Open();
        }

        public void Open(TimeSpan timeout)
        {
            _innerListener.Open(timeout);
        }

        public IAsyncResult BeginOpen(AsyncCallback callback, object state)
        {
            return _innerListener.BeginOpen(callback, state);
        }

        public IAsyncResult BeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _innerListener.BeginOpen(timeout, callback, state);
        }

        public void EndOpen(IAsyncResult result)
        {
            _innerListener.EndOpen(result);
        }

        public T GetProperty<T>() where T : class
        {
            return _innerListener.GetProperty<T>();
        }
    }

    internal class BasicAuthenticationChannel : IReplyChannel
    {
        private IReplyChannel _innerChannel;
        private static PropertyInfo s_defaultFaultAction = typeof(AddressingVersion).GetProperty("DefaultFaultAction", BindingFlags.NonPublic | BindingFlags.Instance);
        private BasicAuthenticationChannelListener _parentListener;

        public BasicAuthenticationChannel(BasicAuthenticationChannelListener basicAuthenticationChannelListener, IReplyChannel innerChannel)
        {
            _parentListener = basicAuthenticationChannelListener;
            _innerChannel = innerChannel;
        }

        public EndpointAddress LocalAddress
        {
            get
            {
                return _innerChannel.LocalAddress;
            }
        }

        public CommunicationState State
        {
            get
            {
                return _innerChannel.State;
            }
        }

        public event EventHandler Closed
        {
            add { _innerChannel.Closed += value; }
            remove { _innerChannel.Closed -= value; }
        }

        public event EventHandler Closing
        {
            add { _innerChannel.Closing += value; }
            remove { _innerChannel.Closing -= value; }
        }

        public event EventHandler Faulted
        {
            add { _innerChannel.Faulted += value; }
            remove { _innerChannel.Faulted -= value; }
        }

        public event EventHandler Opened
        {
            add { _innerChannel.Opened += value; }
            remove { _innerChannel.Opened -= value; }
        }

        public event EventHandler Opening
        {
            add { _innerChannel.Opening += value; }
            remove { _innerChannel.Opening -= value; }
        }

        public void Abort()
        {
            _innerChannel.Abort();
        }

        public void Close()
        {
            _innerChannel.Close();
        }

        public void Close(TimeSpan timeout)
        {
            _innerChannel.Close(timeout);
        }

        public IAsyncResult BeginClose(AsyncCallback callback, object state)
        {
            return _innerChannel.BeginClose(callback, state);
        }

        public IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _innerChannel.BeginClose(timeout, callback, state);
        }

        public void EndClose(IAsyncResult result)
        {
            _innerChannel.EndClose(result);
        }

        public void Open()
        {
            _innerChannel.Open();
        }

        public void Open(TimeSpan timeout)
        {
            _innerChannel.Open(timeout);
        }

        public IAsyncResult BeginOpen(AsyncCallback callback, object state)
        {
            return _innerChannel.BeginOpen(callback, state);
        }

        public IAsyncResult BeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _innerChannel.BeginOpen(timeout, callback, state);
        }

        public void EndOpen(IAsyncResult result)
        {
            _innerChannel.EndOpen(result);
        }

        public RequestContext ReceiveRequest()
        {
            return _innerChannel.ReceiveRequest();
        }

        public RequestContext ReceiveRequest(TimeSpan timeout)
        {
            return _innerChannel.ReceiveRequest(timeout);
        }

        public IAsyncResult BeginReceiveRequest(AsyncCallback callback, object state)
        {
            return ReceiveRequestAsync().ToApm(callback, state);
        }

        public IAsyncResult BeginReceiveRequest(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return ReceiveRequestAsync(timeout).ToApm(callback, state);
        }

        public RequestContext EndReceiveRequest(IAsyncResult result)
        {
            return ((Task<RequestContext>)result).Result;
        }

        internal async Task<RequestContext> ReceiveRequestAsync()
        {
            Func<AsyncCallback, object, IAsyncResult> beginDelegate = _innerChannel.BeginReceiveRequest;
            Func<IAsyncResult, RequestContext> endDelegate = _innerChannel.EndReceiveRequest;
            while (true)
            {
                var requestContext = await Task<RequestContext>.Factory.FromAsync(beginDelegate, endDelegate, TaskCreationOptions.RunContinuationsAsynchronously);
                if (requestContext == null)
                {
                    return null;
                }

                if (ProcessAuthentication(requestContext))
                {
                    return requestContext;
                }

                await ReplyAuthenticationRequiredAsync(requestContext);
            }
        }

        internal async Task<RequestContext> ReceiveRequestAsync(TimeSpan timeout)
        {
            var deadline = DateTime.UtcNow + timeout;
            Func<TimeSpan, AsyncCallback, object, IAsyncResult> beginDelegate = _innerChannel.BeginReceiveRequest;
            Func<IAsyncResult, RequestContext> endDelegate = _innerChannel.EndReceiveRequest;
            while (true)
            {
                timeout = deadline - DateTime.UtcNow;
                var requestContext = await Task<RequestContext>.Factory.FromAsync(beginDelegate, endDelegate, timeout, TaskCreationOptions.RunContinuationsAsynchronously);
                if (requestContext == null)
                {
                    return null;
                }

                if (ProcessAuthentication(requestContext))
                {
                    return requestContext;
                }

                await ReplyAuthenticationRequiredAsync(requestContext);
            }
        }

        public bool TryReceiveRequest(TimeSpan timeout, out RequestContext context)
        {
            return _innerChannel.TryReceiveRequest(timeout, out context);
        }

        public IAsyncResult BeginTryReceiveRequest(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return TryReceiveRequestAsync(timeout).ToApm(callback, state);
        }

        public bool EndTryReceiveRequest(IAsyncResult result, out RequestContext context)
        {
            var tuple = ((Task<Tuple<bool, RequestContext>>)result).Result;
            context = tuple.Item2;
            return tuple.Item1;
        }

        internal async Task<Tuple<bool, RequestContext>> TryReceiveRequestAsync(TimeSpan timeout)
        {
            while (true)
            {
                var tuple = await TryReceiveRequestCoreAsync(timeout);
                if(tuple.Item1)
                {
                    if(tuple.Item2 == null || ProcessAuthentication(tuple.Item2))
                    {
                        return tuple;
                    }

                    await ReplyAuthenticationRequiredAsync(tuple.Item2);
                }
            }
        }

        internal Task<Tuple<bool, RequestContext>> TryReceiveRequestCoreAsync(TimeSpan timeout)
        {
            var tcs = new TaskCompletionSource<Tuple<bool, RequestContext>>();
            _innerChannel.BeginTryReceiveRequest(timeout,
                ar =>
                {
                    var _tcs = ar.AsyncState as TaskCompletionSource<Tuple<bool, RequestContext>>;
                    bool result;
                    RequestContext context;
                    try
                    {
                        result = _innerChannel.EndTryReceiveRequest(ar, out context);
                        _tcs.TrySetResult(Tuple.Create(result, context));
                    }
                    catch(Exception e)
                    {
                        _tcs.TrySetException(e);
                    }
                }, tcs);

            return tcs.Task;
        }

        public bool WaitForRequest(TimeSpan timeout)
        {
            return _innerChannel.WaitForRequest(timeout);
        }

        public IAsyncResult BeginWaitForRequest(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _innerChannel.BeginWaitForRequest(timeout, callback, state);
        }

        public bool EndWaitForRequest(IAsyncResult result)
        {
            return _innerChannel.EndWaitForRequest(result);
        }

        public T GetProperty<T>() where T : class
        {
            return _innerChannel.GetProperty<T>();
        }

        #region BasicAuthentication
        private const string BasicAuthenticationMechanism = "Basic ";
        private const int BasicAuthenticationMechanismLength = 6; // BasicAuthenticationMechanism.Length;
        private const string RealmAuthenticationParameter = "realm";
        private const string AuthenticationChallengeHeaderName = "WWW-Authenticate";
        private const string AuthorizationHeaderName = "Authorization";

        private bool ProcessAuthentication(RequestContext requestContext)
        {
            var authorizationHeader = GetAuthorizationHeader(requestContext.RequestMessage);
            if (!authorizationHeader.StartsWith(BasicAuthenticationMechanism, StringComparison.Ordinal))
            {
                return false;
            }

            string authBase64Encoded = authorizationHeader.Substring(BasicAuthenticationMechanismLength);
            var authDecodedBytes = Convert.FromBase64String(authBase64Encoded);
            var authDecoded = Encoding.UTF8.GetString(authDecodedBytes);
            int colonPos = authDecoded.IndexOf(':');
            if (colonPos <= 0)
            {
                return false;
            }

            var username = authDecoded.Substring(0, colonPos);
            var password = authDecoded.Substring(colonPos + 1);
            SecurityToken securityToken = new UserNameSecurityToken(username, password);
            ReadOnlyCollection<IAuthorizationPolicy> authorizationPolicies = _parentListener.UserNameTokenAuthenticator.ValidateToken(securityToken);
            SecurityMessageProperty security = new SecurityMessageProperty();
            security.TransportToken = new SecurityTokenSpecification(securityToken, authorizationPolicies);
            security.ServiceSecurityContext = new ServiceSecurityContext(authorizationPolicies);
            requestContext.RequestMessage.Properties.Security = security;
            return true;
        }

        private Task ReplyAuthenticationRequiredAsync(RequestContext requestContext)
        {
            var realm = "NoRealm";
            var faultException = CreateAccessDeniedFaultException();
            var messageFault = faultException.CreateMessageFault();
            string action = GetDefaultFaultAction(requestContext.RequestMessage.Version.Addressing);
            var replyMessage = Message.CreateMessage(requestContext.RequestMessage.Version, messageFault, action);
            StringBuilder authChallenge = new StringBuilder(BasicAuthenticationMechanism);
            authChallenge.AppendFormat(RealmAuthenticationParameter + "=\"{0}\", ", realm);

            HttpResponseMessageProperty responseMessageProperty = new HttpResponseMessageProperty();
            replyMessage.Properties[HttpResponseMessageProperty.Name] = responseMessageProperty;

            responseMessageProperty.Headers[AuthenticationChallengeHeaderName] = authChallenge.ToString();
            responseMessageProperty.StatusCode = HttpStatusCode.Unauthorized;
            responseMessageProperty.StatusDescription = "Access Denied";

            return Task.Factory.FromAsync(requestContext.BeginReply, requestContext.EndReply, replyMessage, null);
        }

        private string GetAuthorizationHeader(Message message)
        {
            object requestMessagePropertyObject;
            if (!message.Properties.TryGetValue(HttpRequestMessageProperty.Name,
                    out requestMessagePropertyObject))
            {
                return null; //Not an HTTP request
            }

            HttpRequestMessageProperty requestMessageProperties = (HttpRequestMessageProperty)requestMessagePropertyObject;
            var requestHeaders = requestMessageProperties.Headers;

            var authorizationHeader = requestHeaders[AuthorizationHeaderName] ?? string.Empty;
            return authorizationHeader.Trim();
        }

        private string GetDefaultFaultAction(AddressingVersion addressing)
        {
            return (string)s_defaultFaultAction.GetValue(addressing);
        }

        internal static FaultException CreateAccessDeniedFaultException()
        {
            SecurityVersion wss = SecurityVersion.WSSecurity11;
            FaultCode faultCode = FaultCode.CreateSenderFaultCode("FailedAuthentication", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
            FaultReason faultReason = new FaultReason(new FaultReasonText("Access is denied", CultureInfo.CurrentCulture));
            return new FaultException(faultReason, faultCode);
        }

        #endregion // BasicAuthentication
    }

    internal static class TaskHelpers
    {
        internal static Task<TResult> ToApm<TResult>(this Task<TResult> task, AsyncCallback callback, object state)
        {
            var tcs = new TaskCompletionSource<TResult>(state);

            task.ContinueWith(delegate
            {
                if (task.IsFaulted)
                    tcs.TrySetException(task.Exception.InnerExceptions);
                else if (task.IsCanceled)
                    tcs.TrySetCanceled();
                else
                    tcs.TrySetResult(task.Result);

                if (callback != null)
                    callback(tcs.Task);

            }, CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.Default);

            return tcs.Task;
        }
    }
}
#endif
