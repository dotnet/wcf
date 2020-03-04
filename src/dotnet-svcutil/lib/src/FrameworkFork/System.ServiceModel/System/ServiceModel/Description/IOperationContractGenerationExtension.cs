// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.ServiceModel.Description
{
    using System;
    using System.CodeDom;

    public interface IOperationContractGenerationExtension
    {
        void GenerateOperation(OperationContractGenerationContext context);
    }

}
