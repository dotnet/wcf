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

    public class ServiceContractComplexOutService : IServiceContractComplexOutService
    {
        public void Request(string stringRequest, out ComplexCompositeType complexResponse)
        {
            complexResponse = ScenarioTestHelpers.GetInitializedComplexCompositeType();
            complexResponse.StringValue = stringRequest;
        }
    }

    public class ServiceContractIntRefService : IServiceContractIntRefService
    {
        public void Request(string stringRequest, ref int referencedInteger)
        {
            referencedInteger = stringRequest.Count<char>();
        }
    }

    class ServiceContractComplexRefService : IServiceContractComplexRefService
    {
        public void Request(string stringRequest, ref ComplexCompositeType complexResponse)
        {
            complexResponse = ScenarioTestHelpers.GetInitializedComplexCompositeType();
            complexResponse.StringValue = stringRequest;
        }
    }
}
