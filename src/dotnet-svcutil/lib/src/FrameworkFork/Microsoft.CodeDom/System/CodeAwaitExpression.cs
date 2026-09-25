// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.CodeDom
{
    // CodeDOM has no await node, so svcutil uses this expression to emit equivalent C# and VB helpers.
    public sealed class CodeAwaitExpression : CodeExpression
    {
        internal const string AsyncMethodUserDataKey = "Microsoft.CodeDom.AsyncMethod";

        public CodeAwaitExpression(CodeExpression expression)
        {
            Expression = expression;
        }

        public CodeExpression Expression { get; }
    }
}
