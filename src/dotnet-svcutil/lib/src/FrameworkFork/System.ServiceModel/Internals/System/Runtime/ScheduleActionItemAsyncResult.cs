// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Runtime
{
    // An AsyncResult that schedules work for later on the IOThreadScheduler
    internal abstract class ScheduleActionItemAsyncResult : AsyncResult
    {
        private static Action<object> s_doWork = new Action<object>(DoWork);

        // Implement your own constructor taking in necessary parameters
        // Constructor needs to call "Schedule()" to schedule work 
        // Cache all parameters
        // Implement OnDoWork to do work! 

        protected ScheduleActionItemAsyncResult(AsyncCallback callback, object state) : base(callback, state) { }

        protected void Schedule()
        {
            ActionItem.Schedule(s_doWork, this);
        }

        private static void DoWork(object state)
        {
            ScheduleActionItemAsyncResult thisPtr = (ScheduleActionItemAsyncResult)state;
            Exception completionException = null;
            try
            {
                thisPtr.OnDoWork();
            }
            catch (Exception ex)
            {
                if (Fx.IsFatal(ex))
                {
                    throw;
                }
                completionException = ex;
            }

            thisPtr.Complete(false, completionException);
        }

        protected abstract void OnDoWork();

        public static void End(IAsyncResult result)
        {
            AsyncResult.End<ScheduleActionItemAsyncResult>(result);
        }
    }
}
