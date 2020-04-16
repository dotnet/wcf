// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel.Channels;
using System.ServiceModel.Diagnostics;
using System.IdentityModel.Selectors;
using System.Runtime.Diagnostics;
using System.Threading.Tasks;

namespace System.ServiceModel.Security
{
    internal class WrapperSecurityCommunicationObject : CommunicationObject
    {
        private ISecurityCommunicationObject _innerCommunicationObject;

        public WrapperSecurityCommunicationObject(ISecurityCommunicationObject innerCommunicationObject)
            : base()
        {
            if (innerCommunicationObject == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("innerCommunicationObject");
            }
            _innerCommunicationObject = innerCommunicationObject;
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
            return _innerCommunicationObject.OnBeginClose(timeout, callback, state);
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _innerCommunicationObject.OnBeginOpen(timeout, callback, state);
        }

        protected override void OnClose(TimeSpan timeout)
        {
            _innerCommunicationObject.OnClose(timeout);
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
            _innerCommunicationObject.OnEndClose(result);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            _innerCommunicationObject.OnEndOpen(result);
        }

        protected override void OnFaulted()
        {
            _innerCommunicationObject.OnFaulted();
            base.OnFaulted();
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            _innerCommunicationObject.OnOpen(timeout);
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

        new internal void ThrowIfDisposedOrImmutable()
        {
            base.ThrowIfDisposedOrImmutable();
        }

        protected internal override async Task OnCloseAsync(TimeSpan timeout)
        {
            var asyncInnerCommunicationObject = _innerCommunicationObject as IAsyncCommunicationObject;
            if (asyncInnerCommunicationObject != null)
            {
                await asyncInnerCommunicationObject.CloseAsync(timeout);
            }
            else
            {
                this.OnClose(timeout);
            }
        }

        protected internal override async Task OnOpenAsync(TimeSpan timeout)
        {
            var asyncInnerCommunicationObject = _innerCommunicationObject as IAsyncCommunicationObject;
            if (asyncInnerCommunicationObject != null)
            {
                await asyncInnerCommunicationObject.OpenAsync(timeout);
            }
            else
            {
                this.OnOpen(timeout);
            }
        }
    }

    internal abstract class CommunicationObjectSecurityTokenProvider : SecurityTokenProvider, ICommunicationObject, ISecurityCommunicationObject
    {
        private EventTraceActivity _eventTraceActivity;
        private WrapperSecurityCommunicationObject _communicationObject;

        protected CommunicationObjectSecurityTokenProvider()
        {
            _communicationObject = new WrapperSecurityCommunicationObject(this);
        }

        internal EventTraceActivity EventTraceActivity
        {
            get
            {
                if (_eventTraceActivity == null)
                {
                    _eventTraceActivity = EventTraceActivity.GetFromThreadOrCreate();
                }
                return _eventTraceActivity;
            }
        }

        protected WrapperSecurityCommunicationObject CommunicationObject
        {
            get { return _communicationObject; }
        }

        public event EventHandler Closed
        {
            add { _communicationObject.Closed += value; }
            remove { _communicationObject.Closed -= value; }
        }

        public event EventHandler Closing
        {
            add { _communicationObject.Closing += value; }
            remove { _communicationObject.Closing -= value; }
        }

        public event EventHandler Faulted
        {
            add { _communicationObject.Faulted += value; }
            remove { _communicationObject.Faulted -= value; }
        }

        public event EventHandler Opened
        {
            add { _communicationObject.Opened += value; }
            remove { _communicationObject.Opened -= value; }
        }

        public event EventHandler Opening
        {
            add { _communicationObject.Opening += value; }
            remove { _communicationObject.Opening -= value; }
        }

        public CommunicationState State
        {
            get { return _communicationObject.State; }
        }

        public virtual TimeSpan DefaultOpenTimeout
        {
            get { return ServiceDefaults.OpenTimeout; }
        }

        public virtual TimeSpan DefaultCloseTimeout
        {
            get { return ServiceDefaults.CloseTimeout; }
        }

        // communication object
        public void Abort()
        {
            _communicationObject.Abort();
        }

        public void Close()
        {
            _communicationObject.Close();
        }

        public void Close(TimeSpan timeout)
        {
            _communicationObject.Close(timeout);
        }

        public IAsyncResult BeginClose(AsyncCallback callback, object state)
        {
            return _communicationObject.BeginClose(callback, state);
        }

        public IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _communicationObject.BeginClose(timeout, callback, state);
        }

        public void EndClose(IAsyncResult result)
        {
            _communicationObject.EndClose(result);
        }

        public void Open()
        {
            _communicationObject.Open();
        }

        public void Open(TimeSpan timeout)
        {
            _communicationObject.Open(timeout);
        }

        public IAsyncResult BeginOpen(AsyncCallback callback, object state)
        {
            return _communicationObject.BeginOpen(callback, state);
        }

        public IAsyncResult BeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _communicationObject.BeginOpen(timeout, callback, state);
        }

        public void EndOpen(IAsyncResult result)
        {
            _communicationObject.EndOpen(result);
        }

        public void Dispose()
        {
            this.Close();
        }

        // ISecurityCommunicationObject methods
        public virtual void OnAbort()
        {
        }

        public IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new OperationWithTimeoutAsyncResult(this.OnClose, timeout, callback, state);
        }

        public IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new OperationWithTimeoutAsyncResult(this.OnOpen, timeout, callback, state);
        }

        public virtual void OnClose(TimeSpan timeout)
        {
        }

        public virtual void OnClosed()
        {
        }

        public virtual void OnClosing()
        {
        }

        public void OnEndClose(IAsyncResult result)
        {
            OperationWithTimeoutAsyncResult.End(result);
        }

        public void OnEndOpen(IAsyncResult result)
        {
            OperationWithTimeoutAsyncResult.End(result);
        }

        public virtual void OnFaulted()
        {
            this.OnAbort();
        }

        public virtual void OnOpen(TimeSpan timeout)
        {
        }

        public virtual void OnOpened()
        {
        }

        public virtual void OnOpening()
        {
        }
    }

    internal abstract class CommunicationObjectSecurityTokenAuthenticator : SecurityTokenAuthenticator, ICommunicationObject, ISecurityCommunicationObject
    {
        private WrapperSecurityCommunicationObject _communicationObject;

        protected CommunicationObjectSecurityTokenAuthenticator()
        {
            _communicationObject = new WrapperSecurityCommunicationObject(this);
        }

        protected WrapperSecurityCommunicationObject CommunicationObject
        {
            get { return _communicationObject; }
        }

        public event EventHandler Closed
        {
            add { _communicationObject.Closed += value; }
            remove { _communicationObject.Closed -= value; }
        }

        public event EventHandler Closing
        {
            add { _communicationObject.Closing += value; }
            remove { _communicationObject.Closing -= value; }
        }

        public event EventHandler Faulted
        {
            add { _communicationObject.Faulted += value; }
            remove { _communicationObject.Faulted -= value; }
        }

        public event EventHandler Opened
        {
            add { _communicationObject.Opened += value; }
            remove { _communicationObject.Opened -= value; }
        }

        public event EventHandler Opening
        {
            add { _communicationObject.Opening += value; }
            remove { _communicationObject.Opening -= value; }
        }

        public CommunicationState State
        {
            get { return _communicationObject.State; }
        }

        public virtual TimeSpan DefaultOpenTimeout
        {
            get { return ServiceDefaults.OpenTimeout; }
        }

        public virtual TimeSpan DefaultCloseTimeout
        {
            get { return ServiceDefaults.CloseTimeout; }
        }

        // communication object
        public void Abort()
        {
            _communicationObject.Abort();
        }

        public void Close()
        {
            _communicationObject.Close();
        }

        public void Close(TimeSpan timeout)
        {
            _communicationObject.Close(timeout);
        }

        public IAsyncResult BeginClose(AsyncCallback callback, object state)
        {
            return _communicationObject.BeginClose(callback, state);
        }

        public IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _communicationObject.BeginClose(timeout, callback, state);
        }

        public void EndClose(IAsyncResult result)
        {
            _communicationObject.EndClose(result);
        }

        public void Open()
        {
            _communicationObject.Open();
        }

        public void Open(TimeSpan timeout)
        {
            _communicationObject.Open(timeout);
        }

        public IAsyncResult BeginOpen(AsyncCallback callback, object state)
        {
            return _communicationObject.BeginOpen(callback, state);
        }

        public IAsyncResult BeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _communicationObject.BeginOpen(timeout, callback, state);
        }

        public void EndOpen(IAsyncResult result)
        {
            _communicationObject.EndOpen(result);
        }

        public void Dispose()
        {
            this.Close();
        }

        // ISecurityCommunicationObject methods
        public virtual void OnAbort()
        {
        }

        public IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new OperationWithTimeoutAsyncResult(this.OnClose, timeout, callback, state);
        }

        public IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new OperationWithTimeoutAsyncResult(this.OnOpen, timeout, callback, state);
        }

        public virtual void OnClose(TimeSpan timeout)
        {
        }

        public virtual void OnClosed()
        {
        }

        public virtual void OnClosing()
        {
        }

        public void OnEndClose(IAsyncResult result)
        {
            OperationWithTimeoutAsyncResult.End(result);
        }

        public void OnEndOpen(IAsyncResult result)
        {
            OperationWithTimeoutAsyncResult.End(result);
        }

        public virtual void OnFaulted()
        {
            this.OnAbort();
        }

        public virtual void OnOpen(TimeSpan timeout)
        {
        }

        public virtual void OnOpened()
        {
        }

        public virtual void OnOpening()
        {
        }
    }
}
