// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel.Security;

namespace System.ServiceModel.Channels
{
    public abstract class StreamUpgradeBindingElement : BindingElement
    {
        protected StreamUpgradeBindingElement()
        {
        }

        protected StreamUpgradeBindingElement(StreamUpgradeBindingElement elementToBeCloned)
            : base(elementToBeCloned)
        {
        }

        public abstract StreamUpgradeProvider BuildClientStreamUpgradeProvider(BindingContext context);
    }
}
