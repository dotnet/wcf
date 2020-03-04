// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.ServiceModel
{
    public interface IServiceChannel : IContextChannel
    {
        Uri ListenUri { get; }
    }
}
