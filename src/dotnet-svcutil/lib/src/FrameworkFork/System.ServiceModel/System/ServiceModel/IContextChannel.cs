// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.Net.Security;
using System.ServiceModel.Security.Tokens;
using System.ServiceModel.Channels;

namespace System.ServiceModel
{
    public interface IContextChannel : IChannel, IExtensibleObject<IContextChannel>
    {
        bool AllowOutputBatching { get; set; }
        IInputSession InputSession { get; }
        EndpointAddress LocalAddress { get; }
        TimeSpan OperationTimeout { get; set; }
        IOutputSession OutputSession { get; }
        EndpointAddress RemoteAddress { get; }
        string SessionId { get; }
    }
}
