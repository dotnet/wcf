//-----------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (C) Microsoft Corporation. All Rights Reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.ServiceModel;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal abstract class MessageContractVisitor : TypeWithAttributeVisitor<MessageContractAttribute> { }
}