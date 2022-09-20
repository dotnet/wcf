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
        private bool _generateCloseAsync = false;

        public AddAsyncOpenClose(CommandProcessorOptions options)
        {
            if (TargetFrameworkHelper.IsSupportedFramework(options.Project.TargetFramework, out var frameworkInfo))
            {
                if (frameworkInfo.IsDnx)
                {
                    if (TargetFrameworkHelper.NetCoreVersionReferenceTable.TryGetValue(frameworkInfo.Version, out var referenceTable))
                    {
                        string version = referenceTable.FirstOrDefault().Version;
                        string[] vers = version.Split('.');
                        if (vers.Length > 1)
                        {
                            Version v = new Version(int.Parse(vers[0]), int.Parse(vers[1]));
                            // For .NETCore targetframework found in the referenced table, generate CloseAsync() when WCF package version is less than 4.10
                            if (v.CompareTo(new Version(4, 10)) < 0)
                            {
                                _generateCloseAsync = true;
                            }
                        }
                    }
                }
                else
                {
                    // For supported non-Dnx target frameworks (eg: net472, net48), generate CloseAsync() as before
                    _generateCloseAsync = true;
                }
            }
        }

        protected override void VisitClientClass(CodeTypeDeclaration type)
        {
            base.VisitClientClass(type);

            using (NameScope nameScope = new CodeTypeNameScope(type))
            {
                type.Members.Add(GenerateTaskBasedAsyncMethod("Open", nameScope));
                if(_generateCloseAsync)
                {
                    type.Members.Add(GenerateTaskBasedAsyncMethod("Close", nameScope));
                }
            }
        }

        private static CodeMemberMethod GenerateTaskBasedAsyncMethod(string methodName, NameScope nameScope)
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

            return implMethod;
        }

        // ((ICommunicationObject)this).BeginOpen(null, null)
        private static CodeMethodInvokeExpression GenerateBeginMethodInvokeExpression(string methodName)
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
