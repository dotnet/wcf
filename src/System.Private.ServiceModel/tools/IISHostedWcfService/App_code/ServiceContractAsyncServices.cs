// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;

namespace WcfService
{
    public class ServiceContractIntOutService : IServiceContractIntOutService
    {
        public void Request(string request, out int response)
        {
            int number = request.Count<char>();
            response = number;
        }
    }

    public class ServiceContractUniqueTypeOutService : IServiceContractUniqueTypeOutService
    {
        public void Request(string stringRequest, out UniqueType uniqueTypeResponse)
        {

            uniqueTypeResponse = new UniqueType();
            uniqueTypeResponse.stringValue = stringRequest;
        }
    }

    public class ServiceContractIntRefService : IServiceContractIntRefService
    {
        public void Request(string stringRequest, ref int referencedInteger)
        {
            referencedInteger = stringRequest.Count<char>();
        }
    }

    class ServiceContractUniqueTypeRefService : IServiceContractUniqueTypeRefService
    {
        public void Request(string stringRequest, ref UniqueType uniqueTypeResponse)
        {
            uniqueTypeResponse = new UniqueType();
            uniqueTypeResponse.stringValue = stringRequest;
        }
    }

    public class ServiceContractUniqueTypeOutSyncService : IServiceContractUniqueTypeOutSyncService
    {
        public void Request(string stringRequest, out UniqueType uniqueTypeResponse)
        {
            uniqueTypeResponse = new UniqueType();
            uniqueTypeResponse.stringValue = stringRequest;
        }
    }

    public class ServiceContractUniqueTypeRefSyncService : IServiceContractUniqueTypeRefSyncService
    {
        public void Request(string stringRequest, ref UniqueType uniqueTypeResponse)
        {
            uniqueTypeResponse.stringValue = stringRequest;
        }
    }
}
