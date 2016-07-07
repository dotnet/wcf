// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel;

[ServiceContract]
public interface IUtil
{
    [OperationContract]
    byte[] GetClientCert(bool exportAsPem);

    [OperationContract]
    byte[] GetRootCert(bool exportAsPem);

    [OperationContract]
    byte[] GetPeerCert(bool exportAsPem);

    [OperationContract]
    string GetFQDN();
}
