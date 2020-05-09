﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
                throw new FaultException(String.Format("Failed to get the ServiceHost from the Dictionary.\nThe provided Guid did not match any key in the Dictionary.\nThe provided Guid was: {0}", uniqueIdentifier.ToString()));
            }
            host.Close();
            // cleanup dictionary
            serviceHostDictionary.Remove(uniqueIdentifier);
            return "This should never get back to the client.";
        }

        public string NonRestartService(Guid uniqueIdentifier)
        {
            ServiceHost host;

            if (!serviceHostDictionary.TryGetValue(uniqueIdentifier, out host))
            {
                throw new FaultException(String.Format("Failed to get the ServiceHost from the Dictionary.\nThe provided Guid did not match any key in the Dictionary.\nThe provided Guid was: {0}", uniqueIdentifier.ToString()));
            }
            return "Success!";
        }
    }
}
