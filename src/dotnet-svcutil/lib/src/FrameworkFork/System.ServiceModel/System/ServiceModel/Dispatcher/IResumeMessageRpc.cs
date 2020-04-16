// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Dispatcher
{
    internal interface IResumeMessageRpc
    {
        InstanceContext GetMessageInstanceContext();

        void Resume();
        void Resume(out bool alreadyResumedNoLock);
        void Resume(IAsyncResult result);
        void Resume(object instance);
        void SignalConditionalResume(IAsyncResult result);
    }
}
