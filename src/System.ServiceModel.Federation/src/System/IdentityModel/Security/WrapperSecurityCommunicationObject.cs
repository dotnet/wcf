// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Federation.System.Runtime;
using System.ServiceModel.Security;

namespace System.IdentityModel.Security
{
    internal class WrapperSecurityCommunicationObject : CommunicationObject
    {
        ISecurityCommunicationObject _innerCommunicationObject;

        public WrapperSecurityCommunicationObject(ISecurityCommunicationObject innerCommunicationObject) : base()
        {
            _innerCommunicationObject = innerCommunicationObject ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(innerCommunicationObject));
        }

        protected override Type GetCommunicationObjectType()
        {
            return _innerCommunicationObject.GetType();
        }

        protected override TimeSpan DefaultCloseTimeout
        {
            get { return _innerCommunicationObject.DefaultCloseTimeout; }
        }

        protected override TimeSpan DefaultOpenTimeout
        {
            get { return _innerCommunicationObject.DefaultOpenTimeout; }
        }

        protected override void OnAbort()
        {
            _innerCommunicationObject.OnAbort();
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _innerCommunicationObject.OnCloseAsync(timeout).ToApm(callback, state);
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _innerCommunicationObject.OnOpenAsync(timeout).ToApm(callback, state);
        }

        protected override void OnClose(TimeSpan timeout)
        {
            _innerCommunicationObject.OnCloseAsync(timeout).GetAwaiter().GetResult();
        }

        protected override void OnClosed()
        {
            _innerCommunicationObject.OnClosed();
            base.OnClosed();
        }

        protected override void OnClosing()
        {
            _innerCommunicationObject.OnClosing();
            base.OnClosing();
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            result.ToApmEnd();
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            result.ToApmEnd();
        }

        protected override void OnFaulted()
        {
            _innerCommunicationObject.OnFaulted();
            base.OnFaulted();
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            _innerCommunicationObject.OnOpenAsync(timeout).GetAwaiter().GetResult();
        }

        protected override void OnOpened()
        {
            _innerCommunicationObject.OnOpened();
            base.OnOpened();
        }

        protected override void OnOpening()
        {
            _innerCommunicationObject.OnOpening();
            base.OnOpening();
        }

        internal void ThrowIfClosedOrNotOpen()
        {
            switch (State)
            {
                case CommunicationState.Created:
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateNotOpenException(), Guid.Empty, this);

                case CommunicationState.Opening:
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateNotOpenException(), Guid.Empty, this);

                case CommunicationState.Opened:
                    break;

                case CommunicationState.Closing:
                    break;

                case CommunicationState.Closed:
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateClosedException(), Guid.Empty, this);

                case CommunicationState.Faulted:
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateFaultedException(), Guid.Empty, this);

                default:
                    Fx.Assert("ThrowIfClosedOrNotOpen: Unknown CommunicationObject.state");
                    throw new Exception("ThrowIfClosedOrNotOpen: Unknown CommunicationObject.state");
            }
        }

        private Exception CreateNotOpenException()
        {
            return new InvalidOperationException(SR.Format(SR.CommunicationObjectCannotBeUsed, GetCommunicationObjectType().ToString(), State.ToString()));
        }

        private Exception CreateClosedException()
        {
            return new ObjectDisposedException(GetCommunicationObjectType().ToString());
        }

        internal Exception CreateFaultedException()
        {
            string message = SR.Format(SR.CommunicationObjectFaulted1, GetCommunicationObjectType().ToString());
            return new CommunicationObjectFaultedException(message);
        }
    }
}
