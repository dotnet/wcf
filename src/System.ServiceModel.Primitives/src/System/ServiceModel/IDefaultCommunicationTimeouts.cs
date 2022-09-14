// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.ServiceModel
{
    public interface IDefaultCommunicationTimeouts
    {
        TimeSpan CloseTimeout { get; }
        TimeSpan OpenTimeout { get; }
        TimeSpan ReceiveTimeout { get; }
        TimeSpan SendTimeout { get; }
    }
}
