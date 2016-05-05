// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ServiceModel;

[ServiceContract]
public interface IUtil
{
    [OperationContract]
    byte[] GetClientCert(bool exportAsPem);

    [OperationContract]
    byte[] GetRootCert(bool exportAsPem);

    [OperationContract]
    string GetFQDN();
}
