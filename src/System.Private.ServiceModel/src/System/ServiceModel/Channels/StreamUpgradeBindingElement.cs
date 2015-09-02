// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
