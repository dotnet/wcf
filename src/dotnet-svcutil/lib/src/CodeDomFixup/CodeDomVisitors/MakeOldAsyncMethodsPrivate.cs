// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.CodeDom;
using System.Collections.Generic;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class MakeOldAsyncMethodsPrivate : ClientClassVisitor
    {
        Dictionary<string, PrivateInterfaceMethod> privateIfaceMethods;

        class PrivateInterfaceMethod
        {
            internal PrivateInterfaceMethod(CodeTypeReference ifaceType)
            {
                this.InterfaceType = ifaceType;
            }
            internal readonly CodeTypeReference InterfaceType;
        }

        protected override void VisitClientClass(CodeTypeDeclaration type)
        {
            base.VisitClientClass(type);

            privateIfaceMethods = new Dictionary<string, PrivateInterfaceMethod>();
            CollectionHelpers.MapList<CodeMemberMethod>(type.Members, MapMethodFirstPass, null);
        }

        // first-pass looks at each public BeginFoo or EndFoo method and makes it private.
        // for methods that are interface implementations, we need to remember them so that we can update
        // the calling code in the second pass
        bool MapMethodFirstPass(CodeMemberMethod method)
        {
            if (method != null && (CodeDomHelpers.IsBeginMethod(method) || CodeDomHelpers.IsEndMethod(method)) && IsPublic(method.Attributes))
            {
                if (method.ImplementationTypes.Count == 0)
                {
                    // doesn't impl an iface method, just make it private, and remember it for the second pass
                    method.Attributes = MakePrivate(method.Attributes);

                    // clobber existing entries -- non iface-methods take precedence
                    privateIfaceMethods[method.Name] = new PrivateInterfaceMethod(null);
                }
                else
                {
                    // impls an iface method, make it a private impl, and remember it for the second pass
                    CodeTypeReference ifaceType = method.ImplementationTypes[0];
                    method.ImplementationTypes.Clear();
                    method.PrivateImplementationType = ifaceType;

                    if (!privateIfaceMethods.ContainsKey(method.Name))
                    {
                        // only add it if it wasn't already there -- non-iface methods take precedence
                        privateIfaceMethods.Add(method.Name, new PrivateInterfaceMethod(ifaceType));
                    }
                }
            }
            return true; // don't remove
        }

        protected override void OnExitSpecificType()
        {
            base.OnExitSpecificType();

            this.privateIfaceMethods = null;
        }

        protected override void Visit(CodeMethodInvokeExpression methodInvoke)
        {
            base.Visit(methodInvoke);

            if (this.privateIfaceMethods != null)
            {
                // we got a method call, let's see if it's one of the private impls we need to update
                // few criteria:
                //   a) TargetObject will be a CodeThisReferenceExpression
                //   b) MethodName will be in our table
                //   c) PrivateInterfaceMethod.InterfaceType will be set
                PrivateInterfaceMethod targetMethod = null;
                if (methodInvoke.Method.TargetObject is CodeThisReferenceExpression &&
                    this.privateIfaceMethods.TryGetValue(methodInvoke.Method.MethodName, out targetMethod))
                {
                    if (targetMethod.InterfaceType != null)
                    {
                        methodInvoke.Method.TargetObject = new CodeCastExpression(targetMethod.InterfaceType, methodInvoke.Method.TargetObject);
                    }
                }
            }
        }

        static bool IsPublic(MemberAttributes attrs)
        {
            return (attrs & MemberAttributes.Public) == MemberAttributes.Public;
        }

        static MemberAttributes MakePrivate(MemberAttributes attrs)
        {
            return (attrs & ~MemberAttributes.AccessMask) | MemberAttributes.Private;
        }
    }
}
