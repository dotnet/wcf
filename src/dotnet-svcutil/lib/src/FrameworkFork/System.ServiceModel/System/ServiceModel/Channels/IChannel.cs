// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.ServiceModel;

namespace System.ServiceModel.Channels
{
    public interface IChannel : ICommunicationObject
    {
        T GetProperty<T>() where T : class;
    }
}
