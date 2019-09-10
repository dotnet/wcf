// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
