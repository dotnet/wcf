// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF;
#else
using System.Collections.Generic;
using System.ServiceModel;
#endif

namespace WcfService
{
    [ServiceContract]
    public interface IDataContractResolverService
    {
        [OperationContract(Action = "http://tempuri.org/IDataContractResolverService/GetAllEmployees")]
        List<Employee> GetAllEmployees();

        [OperationContract(Action = "http://tempuri.org/IDataContractResolverService/AddEmployee")]
        void AddEmployee(Employee employee);
    }
}
