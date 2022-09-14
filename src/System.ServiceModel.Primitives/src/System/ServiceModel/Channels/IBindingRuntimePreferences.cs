// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.ServiceModel.Channels
{
    // This is an optional interface that a binding can implement to specify preferences about 
    // how it should be used by a runtime.
    public interface IBindingRuntimePreferences
    {
        bool ReceiveSynchronously { get; }
    }
}

