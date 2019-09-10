// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace System.ServiceModel
{
    public enum CommunicationState
    {
        Created,
        Opening,
        Opened,
        Closing,
        Closed,
        Faulted
    }
}
