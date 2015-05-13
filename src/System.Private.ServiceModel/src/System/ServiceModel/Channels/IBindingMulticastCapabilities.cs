// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.ServiceModel.Channels
{
    public interface IBindingMulticastCapabilities
    {
        // Indicates that messages sent out may come back.
        // One use of this is to avoid sending standard faults.
        bool IsMulticast { get; }
    }
}
