//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

namespace System.ServiceModel.Description
{
    public interface IServiceContractGenerationExtension
    {
        void GenerateContract(ServiceContractGenerationContext context);
    }
}
