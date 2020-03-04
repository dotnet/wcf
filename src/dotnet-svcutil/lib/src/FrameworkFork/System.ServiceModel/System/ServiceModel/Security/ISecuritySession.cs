// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.ServiceModel;
using System.ServiceModel.Channels;

namespace System.ServiceModel.Security
{
    public interface ISecuritySession : ISession
    {
        EndpointIdentity RemoteIdentity { get; }
    }
}
