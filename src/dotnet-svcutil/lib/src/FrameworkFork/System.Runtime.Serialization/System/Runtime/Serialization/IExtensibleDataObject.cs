// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.Runtime.Serialization
{
    public interface IExtensibleDataObject
    {
        ExtensionDataObject ExtensionData { get; set; }
    }
}
