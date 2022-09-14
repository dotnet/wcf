// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime;

namespace System.ServiceModel.Diagnostics
{
    internal abstract class TraceAsyncResult : AsyncResult
    {
        private static Action<AsyncCallback, IAsyncResult> s_waitResultCallback = new Action<AsyncCallback, IAsyncResult>(DoCallback);

        protected TraceAsyncResult(AsyncCallback callback, object state) :
            base(callback, state)
        {
            if (TraceUtility.MessageFlowTracingOnly)
            {
                base.VirtualCallback = s_waitResultCallback;
            }
            else if (DiagnosticUtility.ShouldUseActivity)
            {
                CallbackActivity = ServiceModelActivity.Current;
                if (CallbackActivity != null)
                {
                    base.VirtualCallback = s_waitResultCallback;
                }
            }
        }

        public ServiceModelActivity CallbackActivity
        {
            get;
            private set;
        }

        private static void DoCallback(AsyncCallback callback, IAsyncResult result)
        {
            if (result is TraceAsyncResult)
            {
                TraceAsyncResult thisPtr = result as TraceAsyncResult;
                Fx.Assert(thisPtr.CallbackActivity != null, "this shouldn't be hooked up if we don't have a CallbackActivity");

                if (TraceUtility.MessageFlowTracingOnly)
                {
                    thisPtr.CallbackActivity = null;
                }

                using (ServiceModelActivity.BoundOperation(thisPtr.CallbackActivity))
                {
                    callback(result);
                }
            }
        }
    }
}
