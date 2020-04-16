// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
