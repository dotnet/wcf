// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Infrastructure.Common
{
    /// <summary>Provides for custom IXunitTestCase.</summary>
    internal class WcfTestCase : IXunitTestCase
    {
        private readonly IXunitTestCase _testCase;
        private readonly string _skippedReason;
        private readonly bool _isTheory;
        private readonly IMessageSink _diagnosticMessageSink;

        static TestEventListener s_testListener = new TestEventListener(new List<string>() { "Microsoft-Windows-Application Server-Applications" }, EventLevel.Verbose);

        internal WcfTestCase(IXunitTestCase testCase, string skippedReason = null, bool isTheory = false, IMessageSink diagnosticMessageSink = null)
        {
            _testCase = testCase;
            _skippedReason = skippedReason;
            _isTheory = isTheory;
            _diagnosticMessageSink = diagnosticMessageSink;
        }

        public string DisplayName { get { return _testCase.DisplayName; } }

        public IMethodInfo Method { get { return _testCase.Method; } }

        public string SkipReason { get { return _skippedReason; } }

        public ISourceInformation SourceInformation { get { return _testCase.SourceInformation; } set { _testCase.SourceInformation = value; } }

        public ITestMethod TestMethod { get { return _testCase.TestMethod; } }

        public object[] TestMethodArguments { get { return _testCase.TestMethodArguments; } }

        public Dictionary<string, List<string>> Traits { get { return _testCase.Traits; } }

        public string UniqueID { get { return _testCase.UniqueID; } }

        public void Deserialize(IXunitSerializationInfo info) { _testCase.Deserialize(info); }

        public async Task<RunSummary> RunAsync(
            IMessageSink diagnosticMessageSink, IMessageBus messageBus, object[] constructorArguments,
            ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
        {
            ConcurrentQueue<EventWrittenEventArgs> events = new ConcurrentQueue<EventWrittenEventArgs>();
	        s_testListener.EventWritten = events.Enqueue;

            RunSummary runsummary = await (_isTheory ? new XunitTheoryTestCaseRunner(this, DisplayName, _skippedReason, constructorArguments, _diagnosticMessageSink, messageBus, aggregator, cancellationTokenSource).RunAsync()
                                                     : new XunitTestCaseRunner(this, DisplayName, _skippedReason, constructorArguments, TestMethodArguments, messageBus, aggregator, cancellationTokenSource).RunAsync());

            s_testListener.EventWritten = null;
            if (runsummary.Failed > 0 && events.Count > 0)
            {
                StringBuilder etwOutput = new StringBuilder();
                etwOutput.AppendLine(string.Format("---ETW Trace for Test {0} Begins---", DisplayName));
                foreach (var item in events)
                {
                    try
                    {
                        etwOutput.AppendLine(string.Format(DisplayName + ": " + item.Message, item.Payload.ToArray()));
                    }
                    //The mumber of parameters in Payload does not match the number of arguments in the item.Message and thus cause a
                    // FormatException occationally, In this case, we catch and output all items in the payload and the Message without formatting the message.
                    // https://github.com/dotnet/wcf/issues/1440 is opened to investigate the root cause of the mismatch exception.
                    catch (FormatException e)
                    {
                        etwOutput.AppendLine(String.Format("ETW message encountered FormatException '{0}' using DisplayName '{1}', format '{2}', and '{3}' payload items",
                                             e.Message, DisplayName, item.Message, item.Payload.Count));

                        etwOutput.AppendLine(string.Format("ETW message: {0}, payload below was received", item.Message));
                        foreach (object payloadPara in item.Payload)
                        {
                            if (payloadPara != null)
                            {
                                etwOutput.AppendLine(string.Format("{0}: {1}", DisplayName, payloadPara.ToString()));
                            }
                        }
                    }
                }
                etwOutput.AppendLine(string.Format("---ETW Trace for Test {0} Ends---", DisplayName));

                Console.WriteLine(etwOutput);
            }

            return runsummary;
        }

        public void Serialize(IXunitSerializationInfo info) { _testCase.Serialize(info); }
    }
}
