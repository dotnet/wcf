// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !NET
using System.Linq;
#endif

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

    internal class ServiceContractUniqueTypeRefService : IServiceContractUniqueTypeRefService
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

        public void Request2(out UniqueType uniqueTypeResponse, string stringRequest)
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
