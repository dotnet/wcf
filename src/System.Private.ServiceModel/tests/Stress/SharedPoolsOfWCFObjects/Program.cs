// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

using System.Threading;

namespace SharedPoolsOfWCFObjects
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

        private static void ProcessRunOptions(string[] args)
        {
            // All hardcoded right now, but we need to extract the following from the parameters:
            //  -   HostName
            //  -   AppName
            //  -   TCP/HTTP
            //  -   duration and/or # of iterations to run
            //  -   # factories to pool
            //  -   # channels to pool
            //  -   etc
            var hostName = Environment.GetEnvironmentVariable("hostName");
            if (String.IsNullOrEmpty(hostName))
            {
                hostName = "localhost";
            }
            Console.WriteLine("hostName: " + hostName);

            TestHelpers.SetHostAndProtocol(useHttp: false, hostName: hostName, appName: "WcfService1");
        }

        private static void DoTheRun(int iterations)
        {
            Console.WriteLine("Start");
            Task[] allTasks = new Task[13];
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
                            //RunAllHttpStressTests();    //  for TestHelpers.SetHostAndProtocol(useHttp: true
                            RunAllNetTcpStressTests();  //  for TestHelpers.SetHostAndProtocol(useHttp: false

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

        public static void RunAllStressTests()
        {
            //A full cycle of create/use/destroy
            CreateAndCloseFactoryAndChannelFullCycleTest<WcfService1.IService1, HelloWorldTest>.CreateFactoriesAndChannelsUseAllOnceCloseAll();
            //CreateAndCloseFactoryAndChannelFullCycleTest<WcfService1.IDuplexService, DuplexTest>.CreateFactoriesAndChannelsUseAllOnceCloseAll();
            CreateAndCloseFactoryAndChannelFullCycleTest<WcfService1.IStreamingService, StreamingTest>.CreateFactoriesAndChannelsUseAllOnceCloseAll();

            // Pool channel factories and always create/destroy channels
            PooledFactories<WcfService1.IService1, HelloWorldTest>.CreateUseAndCloseChannels();
            //PooledFactories<WcfService1.IDuplexService, DuplexTest>.CreateUseAndCloseChannels();
            PooledFactories<WcfService1.IStreamingService, StreamingTest>.CreateUseAndCloseChannels();

            // Pool channel factories, always create/destroy channels, recycle factories while using them
            RecyclablePooledFactories<WcfService1.IService1, HelloWorldTest>.RunAllScenariosWithWeights(100, 1);
            //RecyclablePooledFactories<WcfService1.IDuplexService, DuplexTest>.RunAllScenariosWithWeights(100, 1);
            RecyclablePooledFactories<WcfService1.IStreamingService, StreamingTest>.RunAllScenariosWithWeights(100, 1);

            // Pool both channel factories and channels, never recycle
            PooledFactoriesAndChannels<WcfService1.IService1, HelloWorldTest>.UseChannelsInPooledFactoriesAndChannels();
            //PooledFactoriesAndChannels<WcfService1.IDuplexService, DuplexTest>.UseChannelsInPooledFactoriesAndChannels();
            PooledFactoriesAndChannels<WcfService1.IStreamingService, StreamingTest>.UseChannelsInPooledFactoriesAndChannels();


            //// These will get stuck with wcf/issues/108
            //// So we comment them out and use *_OpenOnce workaround instead
            //RecyclablePooledFactoriesAndChannels<WcfService1.IService1, HelloWorldTestTemplate>.RunAllScenariosWithWeights(100, 1, 1);
            //RecyclablePooledFactoriesAndChannels<WcfService1.IDuplexService, DuplexTestTemplate>.RunAllScenariosWithWeights(100, 1, 1);
            //RecyclablePooledFactoriesAndChannels<WcfService1.IStreamingService, StreamingTest>.RunAllScenariosWithWeights(100, 1, 1);

            // Pool both channel factories and channels, recycle both factories and channels while using them 
            RecyclablePooledFactoriesAndChannels_OpenOnce<WcfService1.IService1, HelloWorldTest>.RunAllScenariosWithWeights(100, 1, 1);
            //RecyclablePooledFactoriesAndChannels_OpenOnce<WcfService1.IDuplexService, DuplexTest>.RunAllScenariosWithWeights(100, 1, 1);
            RecyclablePooledFactoriesAndChannels_OpenOnce<WcfService1.IStreamingService, StreamingTest>.RunAllScenariosWithWeights(100, 1, 1);
            

            // Pool both factories and channels, use their async methods, recycle both factories and channels while using them
            RecyclablePooledFactoriesAndChannelsAsync_OpenOnce<WcfService1.IService1, HelloWorldTest>.RunAllScenariosWithWeightsAsync(1000, 1, 1).Wait();
            //RecyclablePooledFactoriesAndChannelsAsync_OpenOnce<WcfService1.IDuplexService, DuplexTest>.RunAllScenariosWithWeightsAsync(1000, 1, 1).Wait();
            //RecyclablePooledFactoriesAndChannelsAsync_OpenOnce<WcfService1.IStreamingService, StreamingTest>.RunAllScenariosWithWeightsAsync(1000, 1, 1).Wait();

            // Add more async tests to achieve parity with sync tests
        }

        // We need a better grouping of tests per binding/feature/stress scenario.
        // But for now we simply group them in 2 methods below for Http&Net.Tcp binding.
        // The switch between the protocol being used is in the call to TestHelpers.SetHostAndProtocol(useHttp: true/false, ...)

        // This is a copy of RunAllStressTests with the tests that are not supported by Http binding commented out
        public static void RunAllHttpStressTests()
        {
            //A full cycle of create/use/destroy
            CreateAndCloseFactoryAndChannelFullCycleTest<WcfService1.IService1, HelloWorldTest>.CreateFactoriesAndChannelsUseAllOnceCloseAll();
            //CreateAndCloseFactoryAndChannelFullCycleTest<WcfService1.IDuplexService, DuplexTest>.CreateFactoriesAndChannelsUseAllOnceCloseAll();
            CreateAndCloseFactoryAndChannelFullCycleTest<WcfService1.IStreamingService, StreamingTest>.CreateFactoriesAndChannelsUseAllOnceCloseAll();

            // Pool channel factories and always create/destroy channels
            PooledFactories<WcfService1.IService1, HelloWorldTest>.CreateUseAndCloseChannels();
            //PooledFactories<WcfService1.IDuplexService, DuplexTest>.CreateUseAndCloseChannels();
            PooledFactories<WcfService1.IStreamingService, StreamingTest>.CreateUseAndCloseChannels();

            // Pool channel factories, always create/destroy channels, recycle factories while using them
            RecyclablePooledFactories<WcfService1.IService1, HelloWorldTest>.RunAllScenariosWithWeights(100, 1);
            //RecyclablePooledFactories<WcfService1.IDuplexService, DuplexTest>.RunAllScenariosWithWeights(100, 1);
            RecyclablePooledFactories<WcfService1.IStreamingService, StreamingTest>.RunAllScenariosWithWeights(100, 1);

            // Pool both channel factories and channels, never recycle
            PooledFactoriesAndChannels<WcfService1.IService1, HelloWorldTest>.UseChannelsInPooledFactoriesAndChannels();
            //PooledFactoriesAndChannels<WcfService1.IDuplexService, DuplexTest>.UseChannelsInPooledFactoriesAndChannels();
            PooledFactoriesAndChannels<WcfService1.IStreamingService, StreamingTest>.UseChannelsInPooledFactoriesAndChannels();


            //// These will get stuck with wcf/issues/108
            //// So we comment them out and use *_OpenOnce workaround instead
            //RecyclablePooledFactoriesAndChannels<WcfService1.IService1, HelloWorldTestTemplate>.RunAllScenariosWithWeights(100, 1, 1);
            //RecyclablePooledFactoriesAndChannels<WcfService1.IDuplexService, DuplexTestTemplate>.RunAllScenariosWithWeights(100, 1, 1);
            //RecyclablePooledFactoriesAndChannels<WcfService1.IStreamingService, StreamingTest>.RunAllScenariosWithWeights(100, 1, 1);

            // Pool both channel factories and channels, recycle both factories and channels while using them 
            RecyclablePooledFactoriesAndChannels_OpenOnce<WcfService1.IService1, HelloWorldTest>.RunAllScenariosWithWeights(100, 1, 1);
            //RecyclablePooledFactoriesAndChannels_OpenOnce<WcfService1.IDuplexService, DuplexTest>.RunAllScenariosWithWeights(100, 1, 1);
            RecyclablePooledFactoriesAndChannels_OpenOnce<WcfService1.IStreamingService, StreamingTest>.RunAllScenariosWithWeights(100, 1, 1);


            // Pool both factories and channels, use their async methods, recycle both factories and channels while using them
            RecyclablePooledFactoriesAndChannelsAsync_OpenOnce<WcfService1.IService1, HelloWorldTest>.RunAllScenariosWithWeightsAsync(1000, 1, 1).Wait();
            //RecyclablePooledFactoriesAndChannelsAsync_OpenOnce<WcfService1.IDuplexService, DuplexTest>.RunAllScenariosWithWeightsAsync(1000, 1, 1).Wait();
            //RecyclablePooledFactoriesAndChannelsAsync_OpenOnce<WcfService1.IStreamingService, StreamingTest>.RunAllScenariosWithWeightsAsync(1000, 1, 1).Wait();

        }

        // This is a copy of RunAllStressTests with the tests that are not supported by Net.Tcp binding commented out
        public static void RunAllNetTcpStressTests()
        {
            //A full cycle of create/use/destroy
            CreateAndCloseFactoryAndChannelFullCycleTest<WcfService1.IService1, HelloWorldTest>.CreateFactoriesAndChannelsUseAllOnceCloseAll();
            CreateAndCloseFactoryAndChannelFullCycleTest<WcfService1.IDuplexService, DuplexTest>.CreateFactoriesAndChannelsUseAllOnceCloseAll();
            //CreateAndCloseFactoryAndChannelFullCycleTest<WcfService1.IStreamingService, StreamingTest>.CreateFactoriesAndChannelsUseAllOnceCloseAll();

            // Pool channel factories and always create/destroy channels
            PooledFactories<WcfService1.IService1, HelloWorldTest>.CreateUseAndCloseChannels();
            PooledFactories<WcfService1.IDuplexService, DuplexTest>.CreateUseAndCloseChannels();
            //PooledFactories<WcfService1.IStreamingService, StreamingTest>.CreateUseAndCloseChannels();

            // Pool channel factories, always create/destroy channels, recycle factories while using them
            RecyclablePooledFactories<WcfService1.IService1, HelloWorldTest>.RunAllScenariosWithWeights(100, 1);
            RecyclablePooledFactories<WcfService1.IDuplexService, DuplexTest>.RunAllScenariosWithWeights(100, 1);
            //RecyclablePooledFactories<WcfService1.IStreamingService, StreamingTest>.RunAllScenariosWithWeights(100, 1);

            // Pool both channel factories and channels, never recycle
            PooledFactoriesAndChannels<WcfService1.IService1, HelloWorldTest>.UseChannelsInPooledFactoriesAndChannels();
            PooledFactoriesAndChannels<WcfService1.IDuplexService, DuplexTest>.UseChannelsInPooledFactoriesAndChannels();
            //PooledFactoriesAndChannels<WcfService1.IStreamingService, StreamingTest>.UseChannelsInPooledFactoriesAndChannels();


            //// These will get stuck with wcf/issues/108
            //// So we comment them out and use *_OpenOnce workaround instead
            //RecyclablePooledFactoriesAndChannels<WcfService1.IService1, HelloWorldTestTemplate>.RunAllScenariosWithWeights(100, 1, 1);
            //RecyclablePooledFactoriesAndChannels<WcfService1.IDuplexService, DuplexTestTemplate>.RunAllScenariosWithWeights(100, 1, 1);
            //RecyclablePooledFactoriesAndChannels<WcfService1.IStreamingService, StreamingTest>.RunAllScenariosWithWeights(100, 1, 1);

            // Pool both channel factories and channels, recycle both factories and channels while using them 
            RecyclablePooledFactoriesAndChannels_OpenOnce<WcfService1.IService1, HelloWorldTest>.RunAllScenariosWithWeights(100, 1, 1);
            RecyclablePooledFactoriesAndChannels_OpenOnce<WcfService1.IDuplexService, DuplexTest>.RunAllScenariosWithWeights(100, 1, 1);
            //RecyclablePooledFactoriesAndChannels_OpenOnce<WcfService1.IStreamingService, StreamingTest>.RunAllScenariosWithWeights(100, 1, 1);


            // Pool both factories and channels, use their async methods, recycle both factories and channels while using them
            RecyclablePooledFactoriesAndChannelsAsync_OpenOnce<WcfService1.IService1, HelloWorldTest>.RunAllScenariosWithWeightsAsync(1000, 1, 1).Wait();
            RecyclablePooledFactoriesAndChannelsAsync_OpenOnce<WcfService1.IDuplexService, DuplexTest>.RunAllScenariosWithWeightsAsync(1000, 1, 1).Wait();
            //RecyclablePooledFactoriesAndChannelsAsync_OpenOnce<WcfService1.IStreamingService, StreamingTest>.RunAllScenariosWithWeightsAsync(1000, 1, 1).Wait();
        }
    }
}
