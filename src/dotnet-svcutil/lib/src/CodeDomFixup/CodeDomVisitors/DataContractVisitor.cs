//-----------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (C) Microsoft Corporation. All Rights Reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Runtime.Serialization;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal abstract class DataContractVisitor : TypeWithAttributeVisitor<DataContractAttribute> { }
}