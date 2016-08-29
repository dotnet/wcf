//----------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace System.ServiceModel.Security
{
    using System.Runtime;
    using System.Runtime.InteropServices;
    using System.ServiceModel.Channels;

    abstract class ApplySecurityAndSendAsyncResult<MessageSenderType> : AsyncResult
        where MessageSenderType : class
    {
        readonly MessageSenderType _channel;
        readonly SecurityProtocol _binding;
        volatile bool _secureOutgoingMessageDone;
        static AsyncCallback sharedCallback = Fx.ThunkCallback(new AsyncCallback(SharedCallback));
        SecurityProtocolCorrelationState _newCorrelationState;
        TimeoutHelper _timeoutHelper;

        public ApplySecurityAndSendAsyncResult(SecurityProtocol binding, MessageSenderType channel, TimeSpan timeout,
            AsyncCallback callback, object state)
            : base(callback, state)
        {
            this._binding = binding;
            this._channel = channel;
            this._timeoutHelper = new TimeoutHelper(timeout);
        }

        protected SecurityProtocolCorrelationState CorrelationState
        {
            get { return _newCorrelationState; }
        }
        
        protected SecurityProtocol SecurityProtocol
        {
            get { return this._binding; }
        }

        protected void Begin(Message message, SecurityProtocolCorrelationState correlationState)
        {
            IAsyncResult result = this._binding.BeginSecureOutgoingMessage(message, _timeoutHelper.RemainingTime(), correlationState, sharedCallback, this);
            if (result.CompletedSynchronously)
            {
                this._binding.EndSecureOutgoingMessage(result, out message, out _newCorrelationState);
                bool completedSynchronously = this.OnSecureOutgoingMessageComplete(message);
                if (completedSynchronously)
                {
                    Complete(true);
                }
            }
        }

        protected static void OnEnd(ApplySecurityAndSendAsyncResult<MessageSenderType> self)
        {
            AsyncResult.End<ApplySecurityAndSendAsyncResult<MessageSenderType>>(self);
        }

        bool OnSecureOutgoingMessageComplete(Message message)
        {
            if (message == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("message"));
            }
            this._secureOutgoingMessageDone = true;
            IAsyncResult result = BeginSendCore(this._channel, message, _timeoutHelper.RemainingTime(), sharedCallback, this);
            if (!result.CompletedSynchronously)
            {
                return false;
            }
            EndSendCore(this._channel, result);
            return this.OnSendComplete();
        }

        protected abstract IAsyncResult BeginSendCore(MessageSenderType channel, Message message, TimeSpan timeout, AsyncCallback callback, object state);

        protected abstract void EndSendCore(MessageSenderType channel, IAsyncResult result);

        bool OnSendComplete()
        {
            OnSendCompleteCore(_timeoutHelper.RemainingTime());
            return true;
        }

        protected abstract void OnSendCompleteCore(TimeSpan timeout);

        static void SharedCallback(IAsyncResult result)
        {
            throw ExceptionHelper.PlatformNotSupported();   // Issue #31 and #1494 in progress

            //            if (result == null)
            //            {
            //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("result"));
            //            }
            //            if (result.CompletedSynchronously)
            //            {
            //                return;
            //            }
            //            ApplySecurityAndSendAsyncResult<MessageSenderType> self = result.AsyncState as ApplySecurityAndSendAsyncResult<MessageSenderType>;
            //            if (self == null)
            //            {
            //                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SR.Format(SR.InvalidAsyncResult), "result"));
            //            }

            //            bool completeSelf = false;
            //            Exception completionException = null;
            //            try
            //            {
            //                if (!self._secureOutgoingMessageDone)
            //                {
            //                    Message message;
            //                    self._binding.EndSecureOutgoingMessage(result, out message, out self._newCorrelationState);
            //                    completeSelf = self.OnSecureOutgoingMessageComplete(message);
            //                }
            //                else
            //                {
            //                    self.EndSendCore(self._channel, result);
            //                    completeSelf = self.OnSendComplete();
            //                }
            //            }
            //#pragma warning suppress 56500 // covered by FxCOP
            //            catch (Exception e)
            //            {
            //                if (Fx.IsFatal(e))
            //                {
            //                    throw;
            //                }

            //                completeSelf = true;
            //                completionException = e;
            //            }
            //            if (completeSelf)
            //            {
            //                self.Complete(false, completionException);
            //            }
        }
    }
}


