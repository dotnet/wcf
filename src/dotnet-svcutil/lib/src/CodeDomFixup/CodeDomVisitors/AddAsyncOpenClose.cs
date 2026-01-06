// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.CodeDom;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Linq;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class AddAsyncOpenClose : ClientClassVisitor
    {
        private readonly bool _isVisualBasic;

        public AddAsyncOpenClose(bool isVisualBasic)
        {
            _isVisualBasic = isVisualBasic;
        }

        protected override void VisitClientClass(CodeTypeDeclaration type)
        {
            base.VisitClientClass(type);

            using (NameScope nameScope = new CodeTypeNameScope(type))
            {
                type.Members.Add(GenerateTaskBasedAsyncMethod("Open", nameScope));
                type.Members.Add(GenerateTaskBasedAsyncMethod("Close", nameScope));
            }
        }

        private CodeMemberMethod GenerateTaskBasedAsyncMethod(string methodName, NameScope nameScope)
        {
            CodeTypeReference delegateType = new CodeTypeReference(typeof(Action<>));
            delegateType.TypeArguments.Add(new CodeTypeReference(typeof(IAsyncResult)));

            // public System.Threading.Tasks.Task OpenAsync()
            CodeMemberMethod implMethod = new CodeMemberMethod
            {
                Name = nameScope.UniqueMemberName(methodName + "Async"),
                Attributes = MemberAttributes.Public,
                ReturnType = new CodeTypeReference(typeof(Task)),
            };

            // new Action<IAsyncResult>((ICommunicationObject)this).EndOpen)
            CodeDelegateCreateExpression delegateOfEndCall =
                new CodeDelegateCreateExpression(
                    delegateType,
                    new CodeCastExpression()
                    {
                        TargetType = new CodeTypeReference(typeof(ICommunicationObject)),
                        Expression = new CodeThisReferenceExpression(),
                    },
                     "End" + methodName);

            // return System.Threading.Tasks.Task.Factory.FromAsync(((ICommunicationObject)this).BeginOpen(null, null), new Action<IAsyncResult>(((ICommunicationObject)this).EndOpen));
            implMethod.Statements.Add(
                new CodeMethodReturnStatement(
                    new CodeMethodInvokeExpression(
                        new CodePropertyReferenceExpression(
                            new CodeTypeReferenceExpression(implMethod.ReturnType),
                            "Factory"),
                    "FromAsync",
                    GenerateBeginMethodInvokeExpression(methodName),
                    delegateOfEndCall)));

            if (methodName.Equals("Close"))
            {
                string condition = _isVisualBasic ? "Not NET6_0_OR_GREATER" : "!NET6_0_OR_GREATER";
                CodeIfDirective ifStart = new CodeIfDirective(CodeIfMode.Start, condition);
                CodeIfDirective ifEnd = new CodeIfDirective(CodeIfMode.End, "");
                implMethod.StartDirectives.Add(ifStart);
                implMethod.EndDirectives.Add(ifEnd);
            }

            return implMethod;
        }

        // ((ICommunicationObject)this).BeginOpen(null, null)
        private CodeMethodInvokeExpression GenerateBeginMethodInvokeExpression(string methodName)
        {
            return new CodeMethodInvokeExpression()
            {
                Method = new CodeMethodReferenceExpression()
                {
                    TargetObject = new CodeCastExpression()
                    {
                        TargetType = new CodeTypeReference(typeof(ICommunicationObject)),
                        Expression = new CodeThisReferenceExpression(),
                    },
                    MethodName = "Begin" + methodName,
                },
                Parameters =
                {
                    new CodePrimitiveExpression(null),
                    new CodePrimitiveExpression(null),
                },
            };
        }
    }
}
