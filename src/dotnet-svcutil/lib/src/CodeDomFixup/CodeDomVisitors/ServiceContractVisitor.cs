// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.ServiceModel;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal abstract class ServiceContractVisitor : TypeWithAttributeVisitor<ServiceContractAttribute> { }
}
