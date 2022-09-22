// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ServiceModel.Description;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class VisitorFixup : CodeFixup
    {
        private static CodeDomVisitor[] GetVisitors(ServiceContractGenerator generator, CommandProcessorOptions options)
        {
            ArrayOfXElementTypeHelper arrayOfXElementTypeHelper = new ArrayOfXElementTypeHelper((generator.Options & ServiceContractGenerationOptions.InternalTypes) == ServiceContractGenerationOptions.InternalTypes, generator.TargetCompileUnit);

            CodeDomVisitor[] visitors = new CodeDomVisitor[]
                    {
                        new CodeNamespaceUniqueTypeFixer(),
                        new AttributeFixer(generator),
                        new ConstructorFixer(),
                        // Visitors to remove sync methods if !options.Sync
                        new MakeOldAsyncMethodsPrivate(),
                        new RemoveExtensibleDataObjectImpl(),
                        new XmlDomAttributeFixer(),
                        new SpecialIXmlSerializableRemapper(arrayOfXElementTypeHelper),
                        new EnsureAdditionalAssemblyReference(),
                        new CreateCallbackImpl((generator.Options & ServiceContractGenerationOptions.TaskBasedAsynchronousMethod) == ServiceContractGenerationOptions.TaskBasedAsynchronousMethod, generator),
                        new AddAsyncOpenClose(options), // this one need to run after CreateCallbakImpl which provide name of VerifyCallbackEvents method
                    };

            if (options.Sync != true)
            {
                visitors = AddSyncVisitors(visitors);
            }

            return visitors;
        }

        public VisitorFixup(ServiceContractGenerator generator) : base(generator) { }

        public override void Fixup(CommandProcessorOptions options)
        {
            CodeDomVisitor.Visit(GetVisitors(generator, options), generator.TargetCompileUnit);
        }

        private static CodeDomVisitor[] AddSyncVisitors(CodeDomVisitor[] visitors)
        {
            List<CodeDomVisitor> list = new List<CodeDomVisitor>(visitors);

            list.InsertRange(4, new CodeDomVisitor[] {
                new RemoveSyncMethodsFromInterface(),
                new RemoveSyncMethodsFromClientClass()
            });

            return list.ToArray();
        }
    }
}

