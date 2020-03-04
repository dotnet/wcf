// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.ServiceModel.Channels
{
    internal interface IMergeEnabledMessageProperty
    {
        bool TryMergeWithProperty(object propertyToMerge);
    }
}
