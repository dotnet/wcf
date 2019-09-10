// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;
using System.ServiceModel.Security;

namespace System.ServiceModel.Channels
{
    public abstract class StreamSecurityUpgradeInitiator : StreamUpgradeInitiator
    {
        protected StreamSecurityUpgradeInitiator()
        {
        }

        public abstract SecurityMessageProperty GetRemoteSecurity(); // works after call to AcceptUpgrade

        internal static SecurityMessageProperty GetRemoteSecurity(StreamUpgradeInitiator upgradeInitiator)
        {
            StreamSecurityUpgradeInitiator securityInitiator = upgradeInitiator as StreamSecurityUpgradeInitiator;
            if (securityInitiator != null)
            {
                return securityInitiator.GetRemoteSecurity();
            }
            else
            {
                return null;
            }
        }
    }
}
