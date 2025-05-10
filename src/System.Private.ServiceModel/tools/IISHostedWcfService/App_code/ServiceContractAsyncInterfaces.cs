// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF;
#else
using System.ServiceModel;
#endif

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

        [OperationContract]
        void Request2(out UniqueType uniqueTypeResponse, string stringRequest);
    }

    [ServiceContract]
    public interface IServiceContractUniqueTypeRefSyncService
    {
        [OperationContract]
        void Request(string stringRequest, ref UniqueType uniqueTypeResponse);
    }
}
