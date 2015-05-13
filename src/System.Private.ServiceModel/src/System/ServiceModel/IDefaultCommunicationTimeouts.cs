// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.ServiceModel
{
    public interface IDefaultCommunicationTimeouts
    {
        TimeSpan CloseTimeout { get; }
        TimeSpan OpenTimeout { get; }
        TimeSpan ReceiveTimeout { get; }
        TimeSpan SendTimeout { get; }
    }
}
