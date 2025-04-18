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
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class DataContractResolverService : IDataContractResolverService
    {
        public static Employee ee;
        public List<Employee> GetAllEmployees()
        {
            List<Employee> list = new List<Employee>();
            list.Add(ee);
            return list;
        }

        public void AddEmployee(Employee employee)
        {
            ee = employee;
        }
    }
}
