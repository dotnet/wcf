// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using WcfService;

namespace SelfHostedWCFService
{
    public class SelfHostedWCFService
    {
        private static void Main()
        {
            // Setup certificates
            Console.WriteLine("Installing certificates...");
            CertGenLib.SetupCerts();

            Console.WriteLine("Starting all service hosts...");
            TestDefinitionHelper.StartHosts();

            //Start the crlUrl service last as the client use it to ensure all services have been started
            TestHostServiceHost host = new TestHostServiceHost();
            host.StartService();

            Console.WriteLine("All service hosts have started.");
            do
            {
                Console.WriteLine("Type <Exit> to terminate the self service Host.");
                string input = Console.ReadLine();
                if (string.Compare(input, "exit", true) == 0)
                {
                    return;
                }
            } while (true);
        }
    }
}
