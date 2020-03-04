// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;

namespace System.ServiceModel.Channels
{
    public interface ISessionChannel<TSession> where TSession : ISession
    {
        TSession Session { get; }
    }
}
