// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
        private WsSerializationContext _wsSerializationContextTrustFeb2005;
        private WsSerializationContext _wsSerializationContextTrust1_3;
        private WsSerializationContext _wsSerializationContextTrust1_4;

        /// <summary>
        /// Constructs a <see cref="WSTrustChannel" />.
        /// </summary>
        /// <param name="requestChannel">The <see cref="IRequestChannel" /> this channel will be used to send a <see cref="WsTrustRequest" /> to the STS.</param>
        public WSTrustChannel(IRequestChannel requestChannel)
        {
            RequestChannel = requestChannel ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(requestChannel));
            if (requestChannel.State != CommunicationState.Created)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(LogHelper.FormatInvariant(SR.GetResourceString(SR.IRequestChannelMustBeCreated), requestChannel.State)));

            MessageVersion = RequestChannel.GetProperty<MessageVersion>();
            if (MessageVersion == null || MessageVersion == MessageVersion.None)
                MessageVersion = MessageVersion.Default;

            EndpointAddress endpointAddress = RequestChannel.GetProperty<EndpointAddress>();
            if (endpointAddress != null)
                Address = endpointAddress.Uri?.AbsoluteUri;
        }

        private string Address { get; }

        /// <summary>
        /// The <see cref="IRequestChannel" /> this class uses for sending and receiving <see cref="Message" /> objects.
        /// </summary>
        private IRequestChannel RequestChannel { get; }

        /// <summary>
        /// Gets the version of WS-Trust this channel will use for serializing <see cref="Message" /> objects.
        /// </summary>
        private MessageVersion MessageVersion { get; }

        /// <summary>
        /// Gets the version of WS-Trust this channel will use for serializing <see cref="Message" /> objects.
        /// </summary>
        //private WsSerializationContext SerializationContext
        //{
        //    get; set;
        //}

        ///// <summary>
        ///// Gets the <see cref="WsTrustSerializer" /> this channel will used for serializing WS-Trust request messages.
        ///// </summary>
        private WsTrustSerializer TrustSerializer { get; } = new WsTrustSerializer();

        /// <summary>
        /// Creates a <see cref="Message"/> that represents a the <see cref="WsTrustRequest"/>.
        /// </summary>
        /// <param name="trustRequest">The <see cref="WsTrustRequest"/> to serialize into the message.</param>
        /// <returns>The <see cref="Message" /> that represents the <see cref="WsTrustRequest"/>.</returns>
        protected virtual Message CreateRequest(WsTrustRequest trustRequest)
        {
            _ = trustRequest ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(trustRequest));
            return Message.CreateMessage(MessageVersion,
                                         GetRequestAction(trustRequest),
                                         new WSTrustRequestBodyWriter(trustRequest, TrustSerializer));
        }

        /// <summary>
        /// Gets the WS-Addressing SOAP action that corresponds to the <see cref="WsTrustRequest"/>.RequestType and <see cref="WsTrustRequest"/>.WsTrustVersion.
        /// </summary>
        /// <param name="trustRequest">The <see cref="WsTrustRequest"/> to generate the WS-Addressing action.</param>
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

        /// <summary>
        /// Gets the <see cref="WsSerializationContext"/> to use when serializing the <see cref="WsTrustRequest"/>.
        /// </summary>
        /// <param name="trustRequest">The <see cref="WsTrustRequest"/> that will be serialized.</param>
        /// <returns>The <see cref="WsSerializationContext"/> for the <see cref="WsTrustRequest"/>.</returns>
        private WsSerializationContext GetSerializationContext(WsTrustRequest trustRequest)
        {
            _ = trustRequest ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(trustRequest));
            if (trustRequest.WsTrustVersion == WsTrustVersion.TrustFeb2005)
            {
                if (_wsSerializationContextTrustFeb2005 == null)
                    _wsSerializationContextTrustFeb2005 = new WsSerializationContext(trustRequest.WsTrustVersion);

                return _wsSerializationContextTrustFeb2005;
            }
            else if (trustRequest.WsTrustVersion == WsTrustVersion.Trust13)
            {
                if (_wsSerializationContextTrust1_3 == null)
                    _wsSerializationContextTrust1_3 = new WsSerializationContext(trustRequest.WsTrustVersion);

                return _wsSerializationContextTrust1_3;
            }
            else if (trustRequest.WsTrustVersion == WsTrustVersion.Trust14)
            {
                if (_wsSerializationContextTrust1_4 == null)
                    _wsSerializationContextTrust1_4 = new WsSerializationContext(trustRequest.WsTrustVersion);

                return _wsSerializationContextTrust1_4;
            }

            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SR.GetResourceString(SR.WsTrustVersionNotSupported, trustRequest.WsTrustVersion.ToString())));
        }

        #region IChannel Members
        /// <summary>
        /// Returns a typed object requested, if present, from the appropriate layer in the channel stack.
        /// </summary>
        /// <typeparam name="T">The typed object for which the method is querying.</typeparam>
        /// <returns>The typed object <typeparamref name="T"/> requested if present, null if not found.</returns>
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
        /// Sends a <see cref="WsTrustRequest"/> to a STS to obtain a <see cref="WCFSecurityToken"/>.
        /// </summary>
        /// <param name="trustRequest">The <see cref="WsTrustRequest" /> sent to the STS.</param>
        /// <returns>A <see cref="WCFSecurityToken" /> issued by the STS.</returns>
        public async virtual Task<WCFSecurityToken> IssueAsync(WsTrustRequest trustRequest)
        {
            _ = trustRequest ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(trustRequest));

            Message requestMessage = CreateRequest(trustRequest);
            Message response = await Task.Factory.FromAsync(RequestChannel.BeginRequest, RequestChannel.EndRequest, requestMessage, null, TaskCreationOptions.None).ConfigureAwait(false);
            if (response.IsFault)
            {
                MessageFault fault = MessageFault.CreateFault(response, FaultMaxBufferSize);
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(FaultException.CreateFault(fault, response.Headers?.Action));
            }

            WsTrustResponse trustResponse = TrustSerializer.ReadResponse(response.GetReaderAtBodyContents());
            WCFSecurityToken token = WSTrustUtilities.CreateGenericXmlSecurityToken(trustRequest, trustResponse, GetSerializationContext(trustRequest), null);
            if (token == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityTokenException(SR.Format(SR.TokenProviderUnableToGetToken, string.IsNullOrEmpty(Address) ? ToString() : Address)));
            
            return token;
        }
        #endregion
    }
}
