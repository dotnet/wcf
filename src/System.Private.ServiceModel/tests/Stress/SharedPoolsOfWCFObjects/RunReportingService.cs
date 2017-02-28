// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using ReportingService;

namespace SharedPoolsOfWCFObjects
{
    // This is a simple wrapper around the reporting stuff
    // The goal is to provide a non blocking "fire and forget" semantics for logging
    // while taking care of handling error cases
    public class RunReportingService : IDisposable
    {
        private string _url;
        private volatile IStressDataCollector _rsChannel = null;
        private long _connectAttempts = 0;
        private const long MaxConnectAttempts = 1000;
        private Task<Task<IStressDataCollector>> _ensureReportingChannelTask = null;
        private Func<Task<IStressDataCollector>> _ensureReportingChannelFunc = null;

        private RunId _rsRunId = null;
        private int _heartBeatFailures = 0;
        private const int MaxHeartBeatFailuresToReport = 10;

        public RunReportingService(string url)
        {
            _url = url;
            _ensureReportingChannelFunc = async () => { return await EnsureReportingChannelImplAsync(); };
        }

        #region blocking calls
        public void RunStarted(RunStartupData startupData)
        {
            try
            {
                _rsRunId = EnsureReportingChannelAsync().Result?.RunStarted(startupData);
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to call RunStarted. Further reporting will be disabled. Error: " + e.ToString());
            }
        }
        public void RunFinished(bool success, string message)
        {
            if (_rsRunId != null)
            {
                try
                {
                    EnsureReportingChannelAsync().Result?.RunFinishedAsync(_rsRunId, success, message);
                }
                catch { }
            }
        }
        #endregion

        #region  'Fire and forget' calls


        private IStressDataCollector __rsClient;
        public void HeartBeat(int threadId, long iteration)
        {
            if (_rsRunId != null)
            {
                var now = DateTime.Now;
                if (__rsClient == null)
                {
                    var rsEndPointAddress = new EndpointAddress(_url);
                    var rsBinding = new BasicHttpBinding();
                    var rsCF = new ChannelFactory<IStressDataCollector>(rsBinding, rsEndPointAddress);
                    var rsClient = rsCF.CreateChannel();
                    (rsClient as ICommunicationObject).Open();
                    __rsClient = rsClient;
                }
                try
                {
                    __rsClient.RunHeartBeatAsync(_rsRunId, threadId, iteration, now).Wait();
                }
                catch (Exception e)
                {
                    var hb = Interlocked.Increment(ref _heartBeatFailures);
                    if (hb < MaxHeartBeatFailuresToReport)
                    {
                        Console.WriteLine("HeartBeat Failed " + e.ToString());
                    }
                    else if (hb == MaxHeartBeatFailuresToReport)
                    {
                        Console.WriteLine("Too many heartbeat failures, ignoring from now.");
                    }
                }
            }
        }

        public void LogMessage(string message)
        {
            if (_rsRunId != null)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await (await EnsureReportingChannelAsync())?.LogMessageAsync(_rsRunId, message);
                    }
                    catch { Console.WriteLine("LogMessage Failed"); }
                });
            }
        }
        #endregion


        public void Dispose()
        {
            (_rsChannel as IClientChannel)?.Dispose();
        }

        private async Task<IStressDataCollector> EnsureReportingChannelImplAsync()
        {
            var rsEndPointAddress = new EndpointAddress(_url);
            var rsBinding = new BasicHttpBinding();
            var rsCF = new ChannelFactory<IStressDataCollector>(rsBinding, rsEndPointAddress);
            var rsClient = rsCF.CreateChannel();
            var rsCC = rsClient as IClientChannel;
            await Task.Factory.FromAsync(rsCC.BeginOpen, rsCC.EndOpen, TaskCreationOptions.None);
            return rsClient;
        }


        // Ensures that the reporting channel is ready to use.
        // The caller must still be prepared to get a disconnected channel or null in case of an error
        private async Task<IStressDataCollector> EnsureReportingChannelAsync()
        {
            if (String.IsNullOrEmpty(_url))
            {
                return null;
            }

            var channel = _rsChannel;
            if (channel != null && (channel as ICommunicationObject).State == CommunicationState.Opened)
            {
                // Hopefully this is the most common case - connected and usable
                return channel;
            }
            else
            {
                Console.Write("Connecting to reporting server... ");
                try
                {
                    // early exit in case we gave up on connecting
                    if (_connectAttempts > MaxConnectAttempts)
                    {
                        return null;
                    }

                    var tcs = new CancellationTokenSource();
                    var t = new Task<Task<IStressDataCollector>>(_ensureReportingChannelFunc, tcs.Token);

                    var connectTask = Interlocked.CompareExchange(ref _ensureReportingChannelTask, t, null);
                    if (connectTask == null)
                    {
                        // check the (re)connection limit before trying to connect
                        if (Interlocked.Increment(ref _connectAttempts) > MaxConnectAttempts)
                        {
                            // Cancel the cold connectTask
                            tcs.Cancel();
                            return null;
                        }

                        // attempt to connect
                        t.Start();
                        _rsChannel = await t.Unwrap();

                        // Success! But before we return the brand new channel we must null out 
                        // the _ensureReportingChannelTask so others can retry again if necessary
                        _ensureReportingChannelTask = null;
                        return _rsChannel;
                    }
                    else
                    {
                        // simply await for someone's else attempt to connect
                        await connectTask.Unwrap();
                        return _rsChannel;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Reporting unavailable. Error: " + e.ToString());
                    return null;
                }
                finally
                {
                    Console.WriteLine("done");
                }
            }
        }
    }
}
