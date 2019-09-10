// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.ServiceModel.Channels
{
    // This is an optional interface that a binding can implement to specify preferences about 
    // how it should be used by a runtime.
    public interface IBindingRuntimePreferences
    {
        bool ReceiveSynchronously { get; }
    }
}

