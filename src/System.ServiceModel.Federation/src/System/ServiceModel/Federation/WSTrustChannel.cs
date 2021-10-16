// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IdentityModel.Tokens;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.WsTrust;
using Microsoft.IdentityModel.Tokens;
using WCFSecurityToken = System.IdentityModel.Tokens.SecurityToken;

namespace System.ServiceModel.Federation
{
    /// <summary>
    /// A channel that is used to send WS-Trust messages to an STS.
    /// </summary>
    public class WSTrustChannel : IWSTrustChannelContract, IChannel, ICommunicationObject
    {
        private const int FaultMaxBufferSize = 20 * 1024;

        /// <summary>
        /// Constructs a <see cref="WSTrustChannel" />.
        /// </summary>
        /// <param name="channelfactory">The <see cref="ChannelFactory" /> that is creating this object.</param>
        /// <param name="trustVersion">The version of WS-Trust this channel will use for serializing <see cref="Message" /> objects.</param>
        public WSTrustChannel(ChannelFactory channelFactory,
                              WsTrustVersion trustVersion)
        {
            if (!(channelFactory is IRequestChannel requestChannel))
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.GetResourceString(SR.ChannelFactoryMustSupportIRequestChannel)));

            Initialize(channelFactory, requestChannel, trustVersion);
        }

        /// <summary>
        /// Constructs a <see cref="WSTrustChannel" />.
        /// </summary>
        /// <param name="channelfactory">The <see cref="Federation.ChannelFactory" /> that is creating this object.</param>
        /// <param name="requestChannel">The <see cref="IRequestChannel" /> this object will be used to send and receive <see cref="Message" /> objects.</param>
        /// <param name="trustVersion">The version of WS-Trust this channel will use for serializing <see cref="Message" /> objects.</param>
        public WSTrustChannel(ChannelFactory channelfactory,
                              IRequestChannel requestChannel,
                              WsTrustVersion trustVersion)
        {
            Initialize(channelfactory, requestChannel, trustVersion);
        }

        private void Initialize(ChannelFactory channelfactory,
                              IRequestChannel requestChannel,
                              WsTrustVersion trustVersion)
        {
            _ = channelfactory ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(channelfactory));
            _ = requestChannel ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(requestChannel));
            _ = trustVersion ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(trustVersion));

            RequestChannel = requestChannel;
            SerializationContext = new WsSerializationContext(trustVersion);
            TrustSerializer = new WsTrustSerializer();

            // If possible use the Binding's MessageVersion for creating our requests.
            if (channelfactory?.Endpoint?.Binding?.MessageVersion != null)
                MessageVersion = channelfactory.Endpoint.Binding.MessageVersion;

            if (channelfactory?.Endpoint?.Address?.Uri?.AbsoluteUri != null)
                Address = channelfactory.Endpoint.Address.Uri.AbsoluteUri;
        }

        private string Address { get; set; }

        /// <summary>
        /// The <see cref="IRequestChannel" /> this class uses for sending and receiving <see cref="Message" /> objects.
        /// </summary>
        private IRequestChannel RequestChannel
        {
            get; set;
        }

        /// <summary>
        /// Gets the version of WS-Trust this channel will use for serializing <see cref="Message" /> objects.
        /// </summary>
        private MessageVersion MessageVersion
        {
            get; set;

        } = MessageVersion.Default;

        /// <summary>
        /// Gets the version of WS-Trust this channel will use for serializing <see cref="Message" /> objects.
        /// </summary>
        private WsSerializationContext SerializationContext
        {
            get; set;
        }

        ///// <summary>
        ///// Gets the <see cref="WsTrustSerializer" /> this channel will used for serializing WS-Trust request messages.
        ///// </summary>
        private WsTrustSerializer TrustSerializer
        {
            get; set;
        }

        /// <summary>
        /// Creates a <see cref="Message"/> object that represents a WS-Trust RST message.
        /// </summary>
        /// <param name="trustRequest">The <see cref="WsTrustRequest"/> to serialize into the message.</param>
        /// <returns>The <see cref="Message" /> object that represents the WS-Trust message.</returns>
        protected virtual Message CreateRequest(WsTrustRequest trustRequest)
        {
            _ = trustRequest ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(trustRequest));
            return Message.CreateMessage(MessageVersion,
                                         GetRequestAction(trustRequest),
                                         new WSTrustRequestBodyWriter(trustRequest, TrustSerializer));
        }

        /// <summary>
        /// Gets the WS-Addressing SOAP action that corresponds to the requestType and WS-Trust version.
        /// </summary>
        /// <param name="requestType">The type of WS-Trust request. This parameter must be one of the
        /// string constants in <see cref="WsTrustActions" />.</param>
        /// <param name="trustVersion">The <see cref="WsTrustVersion" /> of the request.</param>
        /// <returns>The WS-Addressing action to use.</returns>
        public static string GetRequestAction(WsTrustRequest trustRequest)
        {
            _ = trustRequest ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(trustRequest));

            WsTrustActions wsTrustActions;
            if (trustRequest.WsTrustVersion == WsTrustVersion.Trust13)
                wsTrustActions = WsTrustActions.Trust13;
            else if (trustRequest.WsTrustVersion == WsTrustVersion.TrustFeb2005)
                wsTrustActions = WsTrustActions.TrustFeb2005;
            else if (trustRequest.WsTrustVersion == WsTrustVersion.Trust14)
                wsTrustActions = WsTrustActions.Trust14;
            else
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SR.GetResourceString(SR.WsTrustVersionNotSupported, trustRequest.WsTrustVersion.ToString())));

            if (trustRequest.RequestType.Equals(wsTrustActions.Issue))
                return wsTrustActions.IssueRequest;
            else if (trustRequest.RequestType.Equals(wsTrustActions.Cancel))
                return wsTrustActions.CancelRequest;
            else if (trustRequest.RequestType.Equals(wsTrustActions.Renew))
                return wsTrustActions.RenewRequest;
            else if (trustRequest.RequestType.Equals(wsTrustActions.Validate))
                return wsTrustActions.ValidateRequest;
            else
               throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SR.GetResourceString(SR.RequestTypeNotSupported, trustRequest.RequestType)));
        }

        #region IChannel Members
        /// <summary>
        /// Returns a typed object requested, if present, from the appropriate layer in the channel stack.
        /// </summary>
        /// <typeparam name="T">The typed object for which the method is querying.</typeparam>
        /// <returns>The typed object <typeparamref name="T"/> requested if it is present or nullNothingnullptra null reference (Nothing in Visual Basic) if it is not.</returns>
        public T GetProperty<T>() where T : class
        {
            return RequestChannel.GetProperty<T>();
        }
        #endregion

        #region ICommunicationObject Members
        /// <summary>
        /// Occurs when the communication object completes its transition from the closing state into the closed state.
        /// </summary>
        event EventHandler ICommunicationObject.Closed
        {
            add { RequestChannel.Closed += value; }
            remove { RequestChannel.Closed -= value; }
        }

        /// <summary>
        /// Occurs when the communication object first enters the closing state.
        /// </summary>
        event EventHandler ICommunicationObject.Closing
        {
            add { RequestChannel.Closing += value; }
            remove { RequestChannel.Closing -= value; }
        }

        /// <summary>
        /// Occurs when the communication object first enters the faulted state.
        /// </summary>
        event EventHandler ICommunicationObject.Faulted
        {
            add { RequestChannel.Faulted += value; }
            remove { RequestChannel.Faulted -= value; }
        }

        /// <summary>
        /// Occurs when the communication object completes its transition from the opening state into the opened state.
        /// </summary>
        event EventHandler ICommunicationObject.Opened
        {
            add { RequestChannel.Opened += value; }
            remove { RequestChannel.Opened -= value; }
        }

        /// <summary>
        /// Occurs when the communication object first enters the opening state.
        /// </summary>
        event EventHandler ICommunicationObject.Opening
        {
            add { RequestChannel.Opening += value; }
            remove { RequestChannel.Opening -= value; }
        }

        /// <summary>
        /// Causes a communication object to transition immediately from its current state into the closed state. 
        /// </summary>
        void ICommunicationObject.Abort()
        {
            RequestChannel.Abort();
        }

        /// <summary>
        /// Begins an asynchronous operation to close a communication object with a specified timeout.
        /// </summary>
        /// <param name="timeout">
        /// The <see cref="TimeSpan" /> that specifies how long the close operation has to complete before timing out.
        /// </param>
        /// <param name="callback">
        /// The <see cref="AsyncCallback" /> delegate that receives notification of the completion of the asynchronous 
        /// close operation.
        /// </param>
        /// <param name="state">
        /// An object, specified by the application, that contains state information associated with the asynchronous 
        /// close operation.
        /// </param>
        /// <returns>The <see cref="IAsyncResult" /> that references the asynchronous close operation.</returns>
        IAsyncResult ICommunicationObject.BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return RequestChannel.BeginClose(timeout, callback, state);
        }

        /// <summary>
        /// Begins an asynchronous operation to close a communication object.
        /// </summary>
        /// <param name="callback">
        /// The <see cref="AsyncCallback" /> delegate that receives notification of the completion of the asynchronous 
        /// close operation.
        /// </param>
        /// <param name="state">
        /// An object, specified by the application, that contains state information associated with the asynchronous 
        /// close operation.
        /// </param>
        /// <returns>The <see cref="IAsyncResult" /> that references the asynchronous close operation.</returns>
        IAsyncResult ICommunicationObject.BeginClose(AsyncCallback callback, object state)
        {
            return RequestChannel.BeginClose(callback, state);
        }

        /// <summary>
        /// Begins an asynchronous operation to open a communication object within a specified interval of time.
        /// </summary>
        /// <param name="timeout">
        /// The <see cref="TimeSpan" /> that specifies how long the open operation has to complete before timing out.
        /// </param>
        /// <param name="callback">
        /// The <see cref="AsyncCallback" /> delegate that receives notification of the completion of the asynchronous 
        /// close operation.
        /// </param>
        /// <param name="state">
        /// An object, specified by the application, that contains state information associated with the asynchronous 
        /// close operation.
        /// </param>
        /// <returns>The <see cref="IAsyncResult" /> that references the asynchronous open operation.</returns>
        IAsyncResult ICommunicationObject.BeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return RequestChannel.BeginOpen(timeout, callback, state);
        }

        /// <summary>
        /// Begins an asynchronous operation to open a communication object.
        /// </summary>
        /// <param name="callback">
        /// The <see cref="AsyncCallback" /> delegate that receives notification of the completion of the asynchronous 
        /// close operation.
        /// </param>
        /// <param name="state">
        /// An object, specified by the application, that contains state information associated with the asynchronous 
        /// close operation.
        /// </param>
        /// <returns>The <see cref="IAsyncResult" /> that references the asynchronous open operation.</returns>
        IAsyncResult ICommunicationObject.BeginOpen(AsyncCallback callback, object state)
        {
            return RequestChannel.BeginOpen(callback, state);
        }

        /// <summary>
        /// Causes a communication object to transition from its current state into the closed state.
        /// </summary>
        /// <param name="timeout">
        /// The <see cref="TimeSpan" /> that specifies how long the open operation has to complete before timing out.
        /// </param>
        void ICommunicationObject.Close(TimeSpan timeout)
        {
            RequestChannel.Close(timeout);
        }

        /// <summary>
        /// Causes a communication object to transition from its current state into the closed state.
        /// </summary>
        void ICommunicationObject.Close()
        {
            RequestChannel.Close();
        }

        /// <summary>
        /// Completes an asynchronous operation to close a communication object.
        /// </summary>
        /// <param name="result">The <see cref="IAsyncResult" /> that is returned by a call to the BeginClose() method.</param>
        void ICommunicationObject.EndClose(IAsyncResult result)
        {
            RequestChannel.EndClose(result);
        }

        /// <summary>
        /// Completes an asynchronous operation to open a communication object.
        /// </summary>
        /// <param name="result">The <see cref="IAsyncResult" /> that is returned by a call to the BeginClose() method.</param>
        void ICommunicationObject.EndOpen(IAsyncResult result)
        {
            RequestChannel.EndOpen(result);
        }

        /// <summary>
        /// Causes a communication object to transition from the created state into the opened state within a specified interval of time.
        /// </summary>
        /// <param name="timeout">
        /// The <see cref="TimeSpan" /> that specifies how long the open operation has to complete before timing out.
        /// </param>
        void ICommunicationObject.Open(TimeSpan timeout)
        {
            RequestChannel.Open(timeout);
        }

        /// <summary>
        /// Causes a communication object to transition from the created state into the opened state. 
        /// </summary>
        void ICommunicationObject.Open()
        {
            RequestChannel.Open();
        }

        /// <summary>
        /// Gets the current state of the communication-oriented object.
        /// </summary>
        CommunicationState ICommunicationObject.State
        {
            get { return RequestChannel.State; }
        }
        #endregion

        #region IWSTrustChannelContract Members
        /// <summary>
        /// Sends a WS-Trust Issue message to an endpoint STS
        /// </summary>
        /// <param name="trustRequest">The <see cref="WsTrustRequest" /> that represents the request to the STS.</param>
        /// <returns>A <see cref="SecurityToken" /> that represents the token issued by the STS.</returns>
        public async virtual Task<WCFSecurityToken> IssueAsync(WsTrustRequest trustRequest)
        {
            Message requestMessage = CreateRequest(trustRequest);
            Message response = await Task.Factory.FromAsync(RequestChannel.BeginRequest, RequestChannel.EndRequest, requestMessage, null, TaskCreationOptions.None).ConfigureAwait(false);
            if (response.IsFault)
            {
                MessageFault fault = MessageFault.CreateFault(response, WSTrustChannel.FaultMaxBufferSize);
                string action = null;
                if (response.Headers != null)
                    action = response.Headers.Action;

                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(FaultException.CreateFault(fault, action));
            }

            WsTrustResponse trustResponse = TrustSerializer.ReadResponse(response.GetReaderAtBodyContents());
            WCFSecurityToken token = WSTrustUtilities.CreateGenericXmlSecurityToken(trustRequest, trustResponse, SerializationContext, null);

            if (token == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityTokenException(SR.Format(SR.TokenProviderUnableToGetToken, string.IsNullOrEmpty(Address) ? ToString() : Address)));
            
            return token;
        }
        #endregion

        #region IWSTrustContract  Members
        /// <summary>
        /// Sends a WS-Trust Cancel message to an endpoint.
        /// </summary>
        /// <param name="message">The <see cref="Message" /> that contains the instructions for the request to the STS.</param>
        /// <returns>The <see cref="Message" /> returned from the STS.</returns>
        public async Task<Message> CancelAsync(Message message)
        {
            return await Task.Factory.FromAsync(RequestChannel.BeginRequest, RequestChannel.EndRequest, message, null, TaskCreationOptions.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a WS-Trust Issue message to an endpoint.
        /// </summary>
        /// <param name="message">The <see cref="Message" /> that contains the instructions for the request to the STS</param>
        /// <returns>The <see cref="Message" /> returned from the STS</returns>
        public async Task<Message> IssueAsync(Message message)
        {
            return await Task.Factory.FromAsync(RequestChannel.BeginRequest, RequestChannel.EndRequest, message, null, TaskCreationOptions.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a WS-Trust Renew message to a STS.
        /// </summary>
        /// <param name="message">The <see cref="Message" /> that contains the Renew request to a STS.</param>
        /// <returns>The <see cref="Message" /> returned from the STS.</returns>
        public async Task<Message> RenewAsync(Message message)
        {
            return await Task.Factory.FromAsync(RequestChannel.BeginRequest, RequestChannel.EndRequest, message, null, TaskCreationOptions.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a WS-Trust Validate request to a STS.
        /// </summary>
        /// <param name="message">the <see cref="Message"/> that contains the Validate request.</param>
        /// <returns>the <see cref="Message" /> returned from the STS.</returns>
        public async Task<Message> ValidateAsync(Message message)
        {
            return await Task.Factory.FromAsync(RequestChannel.BeginRequest, RequestChannel.EndRequest, message, null, TaskCreationOptions.None).ConfigureAwait(false);
        }
        #endregion
    }
}
