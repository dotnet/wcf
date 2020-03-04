// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.ServiceModel.Description
{
    public interface IServiceContractGenerationExtension
    {
        void GenerateContract(ServiceContractGenerationContext context);
    }
}
