// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace WcfService
{
    public class WcfRestartService : IWcfRestartService
    {
        public static Dictionary<Guid, ServiceHost> serviceHostDictionary = new Dictionary<Guid, ServiceHost>();

        public string RestartService(Guid uniqueIdentifier)
        {
            ServiceHost host;

            if (!serviceHostDictionary.TryGetValue(uniqueIdentifier, out host))
            {
                throw new FaultException(String.Format("Failed to get the ServiceHost from the Dictionary./nThe provided Guid did not match any key in the Dictionary./nThe provided Guid was: {0}", uniqueIdentifier.ToString()));
            }
            host.Close();
            // cleanup dictionary
            serviceHostDictionary.Remove(uniqueIdentifier);
            return "This should never get back to the client.";
        }
    }
}
