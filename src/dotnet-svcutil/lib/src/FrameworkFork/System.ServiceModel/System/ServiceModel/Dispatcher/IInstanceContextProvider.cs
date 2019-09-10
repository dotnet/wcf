// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ServiceModel.Channels;

namespace System.ServiceModel.Dispatcher
{
    public interface IInstanceContextProvider
    {
        InstanceContext GetExistingInstanceContext(Message message, IContextChannel channel);
    }
}


