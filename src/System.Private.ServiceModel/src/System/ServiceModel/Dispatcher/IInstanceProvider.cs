// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ServiceModel.Channels;

namespace System.ServiceModel.Dispatcher
{
    public interface IInstanceProvider
    {
        object GetInstance(InstanceContext instanceContext);
        object GetInstance(InstanceContext instanceContext, Message message);
        void ReleaseInstance(InstanceContext instanceContext, object instance);
    }
}