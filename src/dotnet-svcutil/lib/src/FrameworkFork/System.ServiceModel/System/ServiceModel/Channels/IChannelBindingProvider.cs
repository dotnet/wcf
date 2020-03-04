// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.ServiceModel.Channels
{
    internal interface IChannelBindingProvider
    {
        void EnableChannelBindingSupport();
        bool IsChannelBindingSupportEnabled { get; }
    }
}
