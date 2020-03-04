// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;

namespace System.ServiceModel.Channels
{
    public interface IOutputSessionChannel
        : IOutputChannel, ISessionChannel<IOutputSession>
    {
    }
}
