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
            //TestDefinitionHelper.StartHosts()
            var runHostTask = Task.Run(() => TestDefinitionHelper.StartHosts());
            //runHostTask.Wait();
            Task.Delay(5000).Wait();

            //Start the crlUrl service last as the client use it to ensure all services have been started
            TestHostServiceHost host = new TestHostServiceHost();
            //host.StartService();
            var runTestHostTask = Task.Run(() => host.StartService());
            //runTestHostTask.Wait();

            // Ping to make sure the service is started
            bool result = ServiceHostHelper.Ping("http://localhost:8081/TestHost.svc/Ping");
            if (!result) { return; }

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
