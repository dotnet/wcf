//-----------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (C) Microsoft Corporation. All Rights Reserved.
// </copyright>
//-----------------------------------------------------------------------------

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
