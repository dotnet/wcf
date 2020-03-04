// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.ServiceModel.Description;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal abstract class CodeFixup : IFixup
    {
        public static IFixup[] GetFixups(ServiceContractGenerator generator)
        {
            return new IFixup[]
                {
                    new VisitorFixup(generator),
                };
        }

        protected ServiceContractGenerator generator;

        protected CodeFixup(ServiceContractGenerator generator)
        {
            this.generator = generator;
        }

        public abstract void Fixup(CommandProcessorOptions options);

    }

}
