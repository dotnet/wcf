// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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
