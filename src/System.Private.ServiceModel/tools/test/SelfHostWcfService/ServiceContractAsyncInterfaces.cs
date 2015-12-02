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
    public interface IServiceContractComplexOutService
    {
        [OperationContract]
        void Request(string stringRequest, out ComplexCompositeType complexResponse);
    }

    [ServiceContract]
    public interface IServiceContractIntRefService
    {
        [OperationContract]
        void Request(string stringRequest, ref int referencedInteger);
    }

    [ServiceContract]
    public interface IServiceContractComplexRefService
    {
        [OperationContract]
        void Request(string stringRequest, ref ComplexCompositeType complexResponse);
    }
}
