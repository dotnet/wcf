// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.ServiceModel.Description
{
    internal interface IContractResolver
    {
        ContractDescription ResolveContract(string contractName);
    }
}
