// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal abstract class MessageContractVisitor : TypeWithAttributeVisitor<MessageContractAttribute> { }
}
