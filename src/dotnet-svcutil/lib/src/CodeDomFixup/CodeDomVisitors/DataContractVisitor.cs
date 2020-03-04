// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.Runtime.Serialization;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal abstract class DataContractVisitor : TypeWithAttributeVisitor<DataContractAttribute> { }
}
