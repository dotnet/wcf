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
            using (ServiceHost wcfDuplexService = new ServiceHost(typeof(WcfDuplexService)))
            {
                wcfservicehost.Open();
                Console.WriteLine("WcfService is ready");
                userNameService.Open();
                Console.WriteLine("Wcf user name service is ready");
                wcfDuplexService.Open();
                Console.WriteLine("WcfDuplexService is ready");

                DumpEndpoints(wcfservicehost, userNameService, wcfDuplexService);

                Console.WriteLine("Press ENTER to close services...");
                Console.ReadLine();
            }
        }

         static void DumpEndpoints(params ServiceHost[] hosts)
         {
             foreach (var host in hosts)
             {
                 Console.WriteLine("Host : " +  host.GetType().Name);
                 foreach (var endpoint in host.Description.Endpoints)
                 {
                     Console.WriteLine("\t" + endpoint.ListenUri);
                 }
             }
         }
    }
}
