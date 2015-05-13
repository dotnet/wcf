// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ServiceModel;
using System.ServiceModel.Channels;

namespace System.ServiceModel.Security
{
    public interface ISecuritySession : ISession
    {
        EndpointIdentity RemoteIdentity { get; }
    }
}
