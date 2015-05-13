// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ServiceModel.Channels;

namespace System.ServiceModel.Dispatcher
{
    public interface IErrorHandler
    {
        void ProvideFault(Exception error, MessageVersion version, ref Message fault);
        bool HandleError(Exception error);
    }
}
