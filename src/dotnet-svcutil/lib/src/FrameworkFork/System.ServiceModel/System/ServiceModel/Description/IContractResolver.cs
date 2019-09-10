// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.ServiceModel.Description
{
    internal interface IContractResolver
    {
        ContractDescription ResolveContract(string contractName);
    }
}
