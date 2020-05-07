// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.CodeDom;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class ConstructorFixer : ClientClassVisitor
    {
        public ConstructorFixer()
        {
        }
        private static Type[][] s_validCtors = new Type[][]
                    {
                        Array.Empty<Type>(),
                        new Type[] { typeof(string), },
                        new Type[] { typeof(InstanceContext), },
                        new Type[] { typeof(string), typeof(string), },
                        new Type[] { typeof(string), typeof(EndpointAddress), },
                        new Type[] { typeof(InstanceContext), typeof(string), },
                        new Type[] { typeof(InstanceContext), typeof(string), typeof(string), },
                        new Type[] { typeof(InstanceContext), typeof(string), typeof(EndpointAddress), },
                        new Type[] { typeof(Binding), typeof(EndpointAddress), },
                        new Type[] { typeof(InstanceContext), typeof(Binding), typeof(EndpointAddress), },
                    };

        protected override void VisitClientClass(CodeTypeDeclaration type)
        {
            base.VisitClientClass(type);

            CollectionHelpers.Filter<CodeConstructor> ctorFilter;
            ctorFilter = delegate (CodeConstructor ctor)
            { return IsValidConstructor(ctor, s_validCtors); };
            CollectionHelpers.MapList<CodeConstructor>(type.Members, ctorFilter, null);
        }

        private static bool IsValidConstructor(CodeConstructor ctor, Type[][] validCtors)
        {
            for (int i = 0; i < validCtors.Length; i++)
            {
                if (CodeDomHelpers.MatchSignatures(ctor.Parameters, validCtors[i]))
                    return true;
            }
            return false;
        }
    }
}
