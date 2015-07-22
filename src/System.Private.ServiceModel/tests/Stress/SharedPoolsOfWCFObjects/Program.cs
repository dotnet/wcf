// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace WCFClientStressTests
{
    public class Program
    {
        public void Main(string[] args)
        {
            ProcessRunOptions(args);

            DoTheRun(3000000);

            Console.WriteLine("Done. Press Enter to GC.");
            Console.ReadLine();
            GC.Collect();
            Console.WriteLine("After GC");
            Console.ReadLine();
        }

        private void ProcessRunOptions(string[] args)
        {
            // All hardcoded right now, but we need to extract the following from the parameters:
            //  -   HostName
            //  -   AppName
            //  -   TCP/HTTP
            //  -   duration and/or # of iterations to run
            //  -   # factories to pool
            //  -   # channels to pool
            //  -   etc
            TestHelpers.SetHostAndProtocol(useHttp: false, hostName: "localhost", appName: "WcfService1");
        }

        private void DoTheRun(int iterations)
        {
            Console.WriteLine("Start");
            Task[] allTasks = new Task[23];
            for (int t = 0; t < allTasks.Length; t++)
            {
                int tt = t;
                allTasks[t] = Task.Run(() =>
                {
                    var ttt = tt;
                    for (int i = 0; i < iterations; i++)
                    {
                        try
                        {
                            RunAllStressTests();
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debugger.Break();
                        }

                        if (i % 1000 == 0)
                        {
                            Console.WriteLine(ttt + " " + i);
                        }
                    }
                    Console.WriteLine(ttt + ": done");
                });
            }
            Task.WaitAll(allTasks);

            Console.WriteLine("Dispose all");
            StaticDisposablesHelper.DisposeAll();
        }

        public void RunAllStressTests()
        {
            CreateAndCloseFactoryAndChannelFullCycleTest.CreateFactoriesAndChannelsUseAllOnceCloseAll();
            PooledFactoriesAndChannels.UseChannelsInPooledFactoriesAndChannels();
            // will get stuck with wcf/issues/108
            // RecyclablePooledFactoriesAndChannels.RunAllScenariosWithWeights(100, 1, 1); 
            RecyclablePooledFactoriesAndChannels_OpenOnce.RunAllScenariosWithWeights(1000, 1, 1);
            RecyclablePooledFactoriesAndChannelsAsync_OpenOnce.RunAllScenariosWithWeightsAsync(1000, 1, 1).Wait();
            PooledFactories.CreateUseAndCloseChannels();
            RecyclablePooledFactories.RunAllScenariosWithWeights(100, 1);
        }
    }
}
