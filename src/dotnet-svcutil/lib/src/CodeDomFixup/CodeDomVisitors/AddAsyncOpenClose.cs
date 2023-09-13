// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.CodeDom;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Linq;
using System.Xml.Linq;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class AddAsyncOpenClose : ClientClassVisitor
    {
        private bool _generateCloseAsync = false;
        private bool _addCondition = false;

        public AddAsyncOpenClose(CommandProcessorOptions options)
        {
            if (options.Project != null && options.Project.TargetFrameworks.Count() > 1 && options.Project.TargetFrameworks.Any(t => TargetFrameworkHelper.IsSupportedFramework(t, out FrameworkInfo netfxInfo) && !netfxInfo.IsDnx))
            {
                _generateCloseAsync = true;
                FrameworkInfo dnxInfo = null;
                var tfx = options.Project.TargetFrameworks.FirstOrDefault(t => TargetFrameworkHelper.IsSupportedFramework(t, out dnxInfo) && dnxInfo.IsDnx);
                if (!string.IsNullOrEmpty(tfx) && dnxInfo.Version.Major >= 6)
                {
                    _addCondition = true;
                }
            }
            else
            {
                if (options.TargetFramework.IsDnx)
                {
                    if (TargetFrameworkHelper.NetCoreVersionReferenceTable.TryGetValue(options.TargetFramework.Version, out var referenceTable))
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

            if(_addCondition && methodName.Equals("Close"))
            {
                CodeIfDirective ifStart = new CodeIfDirective(CodeIfMode.Start, "NETFRAMEWORK");
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
