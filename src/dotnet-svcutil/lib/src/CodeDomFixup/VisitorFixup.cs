// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System;
using System.ServiceModel.Description;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class VisitorFixup : CodeFixup
    {
        private static CodeDomVisitor[] GetVisitors(ServiceContractGenerator generator, CommandProcessorOptions options)
        {
            ArrayOfXElementTypeHelper arrayOfXElementTypeHelper = new ArrayOfXElementTypeHelper((generator.Options & ServiceContractGenerationOptions.InternalTypes) == ServiceContractGenerationOptions.InternalTypes, generator.TargetCompileUnit);
            bool isVisualBasic = IsVisualBasicLanguage(options?.Language);

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
                        new AddAsyncOpenClose(isVisualBasic), // this one need to run after CreateCallbakImpl which provide name of VerifyCallbackEvents method
                        new InternalTypeVisibilityFixer(options != null && options.InternalTypeAccess == true),
                        new TypeNameFixup()
                    };

            // Default behavior: Remove sync methods, so only async methods are generated.
            if (options.Sync != true && options.SyncOnly != true)
            {
                visitors = AddSyncVisitors(visitors);
            }
            // If --syncOnly specified, remove async methods, only sync methods remain.
            else if (options.SyncOnly == true)
            {
                visitors = AddAsyncVisitors(visitors);
            }
            // If --sync is specified (and --syncOnly is NOT specified), do nothing, keep both sync and async methods.

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

        private static CodeDomVisitor[] AddAsyncVisitors(CodeDomVisitor[] visitors)
        {
            List<CodeDomVisitor> list = new List<CodeDomVisitor>(visitors);

            list.InsertRange(4, new CodeDomVisitor[] {
                new RemoveAsyncMethodsFromInterface(),
                new RemoveAsyncMethodsFromClientClass()
            });

            return list.ToArray();
        }

        private static bool IsVisualBasicLanguage(string language)
        {
            if (string.IsNullOrWhiteSpace(language))
            {
                return false;
            }

            language = language.Trim();

            return language.Equals("vb", StringComparison.OrdinalIgnoreCase) ||
                   language.Equals("visualbasic", StringComparison.OrdinalIgnoreCase) ||
                   language.Equals("visual basic", StringComparison.OrdinalIgnoreCase);
        }
    }
}

