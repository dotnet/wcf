// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.ServiceModel
{
    internal interface IOperationContractAttributeProvider
    {
        OperationContractAttribute GetOperationContractAttribute();
    }
}
