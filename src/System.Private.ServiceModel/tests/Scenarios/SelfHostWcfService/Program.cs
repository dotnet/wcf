// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;

namespace WcfService
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using (ServiceHost wcfservicehost = new ServiceHost(typeof(WcfService)))
            using (ServiceHost userNameService = new ServiceHost(typeof(WcfUserNameService)))
            {
                wcfservicehost.Open();
                Console.WriteLine("WcfService is ready");
                userNameService.Open();
                Console.WriteLine("Wcf user name service is ready");
                Console.WriteLine("Press ENTER to close services...");
                Console.ReadLine();
            }
        }
    }
}
