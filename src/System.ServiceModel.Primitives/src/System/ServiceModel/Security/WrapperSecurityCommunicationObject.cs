// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel.Channels;
using System.IdentityModel.Selectors;
using System.Runtime.Diagnostics;
using System.Threading.Tasks;
using System.Runtime;

namespace System.ServiceModel.Security
{
    internal class WrapperSecurityCommunicationObject : CommunicationObject
    {
        private ISecurityCommunicationObject _innerCommunicationObject;

        public WrapperSecurityCommunicationObject(ISecurityCommunicationObject innerCommunicationObject)
            : base()
        {
            _innerCommunicationObject = innerCommunicationObject ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(innerCommunicationObject));
            SupportsAsyncOpenClose = true;
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
            return OnCloseAsync(timeout).ToApm(callback, state);
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return OnOpenAsync(timeout).ToApm(callback, state);
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

        new internal void ThrowIfDisposedOrImmutable()
        {
            base.ThrowIfDisposedOrImmutable();
        }

        protected internal override Task OnCloseAsync(TimeSpan timeout)
        {
            return _innerCommunicationObject.OnCloseAsync(timeout);
        }

        protected internal override Task OnOpenAsync(TimeSpan timeout)
        {
            return _innerCommunicationObject.OnOpenAsync(timeout);
        }
    }

    internal abstract class CommunicationObjectSecurityTokenProvider : SecurityTokenProvider, IAsyncCommunicationObject, ISecurityCommunicationObject
    {
        private EventTraceActivity _eventTraceActivity;

        protected CommunicationObjectSecurityTokenProvider()
        {
            CommunicationObject = new WrapperSecurityCommunicationObject(this);
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

        protected WrapperSecurityCommunicationObject CommunicationObject { get; }

        public event EventHandler Closed
        {
            add { CommunicationObject.Closed += value; }
            remove { CommunicationObject.Closed -= value; }
        }

        public event EventHandler Closing
        {
            add { CommunicationObject.Closing += value; }
            remove { CommunicationObject.Closing -= value; }
        }

        public event EventHandler Faulted
        {
            add { CommunicationObject.Faulted += value; }
            remove { CommunicationObject.Faulted -= value; }
        }

        public event EventHandler Opened
        {
            add { CommunicationObject.Opened += value; }
            remove { CommunicationObject.Opened -= value; }
        }

        public event EventHandler Opening
        {
            add { CommunicationObject.Opening += value; }
            remove { CommunicationObject.Opening -= value; }
        }

        public CommunicationState State
        {
            get { return CommunicationObject.State; }
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
            CommunicationObject.Abort();
        }

        public void Close()
        {
            CommunicationObject.Close();
        }

        public Task CloseAsync(TimeSpan timeout)
        {
            return ((IAsyncCommunicationObject)CommunicationObject).CloseAsync(timeout);
        }

        public void Close(TimeSpan timeout)
        {
            CommunicationObject.Close(timeout);
        }

        public IAsyncResult BeginClose(AsyncCallback callback, object state)
        {
            return CommunicationObject.BeginClose(callback, state);
        }

        public IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return CommunicationObject.BeginClose(timeout, callback, state);
        }

        public void EndClose(IAsyncResult result)
        {
            CommunicationObject.EndClose(result);
        }

        public void Open()
        {
            CommunicationObject.Open();
        }

        public Task OpenAsync(TimeSpan timeout)
        {
            return ((IAsyncCommunicationObject)CommunicationObject).OpenAsync(timeout);
        }

        public void Open(TimeSpan timeout)
        {
            CommunicationObject.Open(timeout);
        }

        public IAsyncResult BeginOpen(AsyncCallback callback, object state)
        {
            return CommunicationObject.BeginOpen(callback, state);
        }

        public IAsyncResult BeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return CommunicationObject.BeginOpen(timeout, callback, state);
        }

        public void EndOpen(IAsyncResult result)
        {
            CommunicationObject.EndOpen(result);
        }

        public void Dispose()
        {
            Close();
        }

        // ISecurityCommunicationObject methods
        public virtual void OnAbort()
        {
        }

        public virtual Task OnCloseAsync(TimeSpan timeout)
        {
            return Task.CompletedTask;
        }

        public virtual void OnClosed()
        {
        }

        public virtual void OnClosing()
        {
        }

        public virtual void OnFaulted()
        {
            OnAbort();
        }

        public virtual Task OnOpenAsync(TimeSpan timeout)
        {
            return Task.CompletedTask;
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
        protected CommunicationObjectSecurityTokenAuthenticator()
        {
            CommunicationObject = new WrapperSecurityCommunicationObject(this);
        }

        protected WrapperSecurityCommunicationObject CommunicationObject { get; }

        public event EventHandler Closed
        {
            add { CommunicationObject.Closed += value; }
            remove { CommunicationObject.Closed -= value; }
        }

        public event EventHandler Closing
        {
            add { CommunicationObject.Closing += value; }
            remove { CommunicationObject.Closing -= value; }
        }

        public event EventHandler Faulted
        {
            add { CommunicationObject.Faulted += value; }
            remove { CommunicationObject.Faulted -= value; }
        }

        public event EventHandler Opened
        {
            add { CommunicationObject.Opened += value; }
            remove { CommunicationObject.Opened -= value; }
        }

        public event EventHandler Opening
        {
            add { CommunicationObject.Opening += value; }
            remove { CommunicationObject.Opening -= value; }
        }

        public CommunicationState State
        {
            get { return CommunicationObject.State; }
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
            CommunicationObject.Abort();
        }

        public void Close()
        {
            CommunicationObject.Close();
        }

        public void Close(TimeSpan timeout)
        {
            CommunicationObject.Close(timeout);
        }

        public IAsyncResult BeginClose(AsyncCallback callback, object state)
        {
            return CommunicationObject.BeginClose(callback, state);
        }

        public IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return CommunicationObject.BeginClose(timeout, callback, state);
        }

        public void EndClose(IAsyncResult result)
        {
            CommunicationObject.EndClose(result);
        }

        public void Open()
        {
            CommunicationObject.Open();
        }

        public void Open(TimeSpan timeout)
        {
            CommunicationObject.Open(timeout);
        }

        public IAsyncResult BeginOpen(AsyncCallback callback, object state)
        {
            return CommunicationObject.BeginOpen(callback, state);
        }

        public IAsyncResult BeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return CommunicationObject.BeginOpen(timeout, callback, state);
        }

        public void EndOpen(IAsyncResult result)
        {
            CommunicationObject.EndOpen(result);
        }

        public void Dispose()
        {
            Close();
        }

        // ISecurityCommunicationObject methods
        public virtual void OnAbort()
        {
        }

        public IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new OperationWithTimeoutAsyncResult(OnClose, timeout, callback, state);
        }

        public IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new OperationWithTimeoutAsyncResult(OnOpen, timeout, callback, state);
        }

        public virtual void OnClose(TimeSpan timeout)
        {
        }

        public Task OnCloseAsync(TimeSpan timeout)
        {
            return Task.CompletedTask;
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
            OnAbort();
        }

        public virtual void OnOpen(TimeSpan timeout)
        {
        }

        public Task OnOpenAsync(TimeSpan timeout)
        {
            return Task.CompletedTask;
        }

        public virtual void OnOpened()
        {
        }

        public virtual void OnOpening()
        {
        }
    }
}
