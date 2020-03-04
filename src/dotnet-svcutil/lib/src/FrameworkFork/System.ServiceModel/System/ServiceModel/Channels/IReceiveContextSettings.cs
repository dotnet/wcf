// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.ServiceModel.Channels
{
    public interface IReceiveContextSettings
    {
        bool Enabled { get; set; }
        TimeSpan ValidityDuration { get; }
    }
}
