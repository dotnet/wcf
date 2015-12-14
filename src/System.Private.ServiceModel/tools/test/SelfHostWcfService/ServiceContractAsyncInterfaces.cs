// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ServiceModel;

namespace WcfService
{
    [ServiceContract]
    public interface IServiceContractIntOutService
    {
        [OperationContract]
        void Request(string stringRequest, out int intResponse);
    }

    [ServiceContract]
    public interface IServiceContractUniqueTypeOutService
    {
        [OperationContract]
        void Request(string stringRequest, out UniqueType uniqueTypeResponse);
    }

    [ServiceContract]
    public interface IServiceContractIntRefService
    {
        [OperationContract]
        void Request(string stringRequest, ref int referencedInteger);
    }

    [ServiceContract]
    public interface IServiceContractUniqueTypeRefService
    {
        [OperationContract]
        void Request(string stringRequest, ref UniqueType uniqueTypeResponse);
    }

    [ServiceContract]
    public interface IServiceContractUniqueTypeOutSyncService
    {
        [OperationContract]
        void Request(string stringRequest, out UniqueType uniqueTypeResponse);
    }

    [ServiceContract]
    public interface IServiceContractUniqueTypeRefSyncService
    {
        [OperationContract]
        void Request(string stringRequest, ref UniqueType uniqueTypeResponse);
    }
}
