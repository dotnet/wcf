// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ServiceModel.Channels
{
    public interface IConnectionPoolSettings
    {
        T GetConnectionPoolSetting<T>(string settingName);
        bool IsCompatible(IConnectionPoolSettings other);
    }
}
