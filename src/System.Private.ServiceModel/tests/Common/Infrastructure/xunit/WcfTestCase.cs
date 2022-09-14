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
    internal class WcfTestCase : XunitTestCase, IXunitTestCase
    {
        private string _skippedReason;
        private bool _isTheory;
        private TimeSpan _failFastDuration;
        private readonly IMessageSink _diagnosticMessageSink;

        static TestEventListener s_testListener = new TestEventListener(new List<string>() { "Microsoft-Windows-Application Server-Applications" }, EventLevel.Verbose);

        [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
        public WcfTestCase()
        {
        }

        internal WcfTestCase(XunitTestCase testCase,
                             TestMethodDisplay defaultMethodDisplay,
                             TimeSpan failFastDuration,
                             string skippedReason = null,
                             bool isTheory = false,
                             IMessageSink diagnosticMessageSink = null)
            : base(diagnosticMessageSink, defaultMethodDisplay, TestMethodDisplayOptions.None, testCase.TestMethod, testCase.TestMethodArguments)
        {
            _skippedReason = skippedReason;
            _isTheory = isTheory;
            _failFastDuration = failFastDuration;
            _diagnosticMessageSink = diagnosticMessageSink;
        }

        public override async Task<RunSummary> RunAsync(
            IMessageSink diagnosticMessageSink, IMessageBus messageBus, object[] constructorArguments,
            ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
        {
            ConcurrentQueue<EventWrittenEventArgs> events = new ConcurrentQueue<EventWrittenEventArgs>();
	        s_testListener.EventWritten = events.Enqueue;
            Timer timer = null;
            if (_failFastDuration != System.Threading.Timeout.InfiniteTimeSpan && !System.Diagnostics.Debugger.IsAttached)
            {
                timer = new Timer((s) => Environment.FailFast($"Test timed out after duration {_failFastDuration}"),
                                  null,
                                  (int)_failFastDuration.TotalMilliseconds,
                                  System.Threading.Timeout.Infinite);
            }

            RunSummary runsummary;
            using (timer)
            {
                runsummary = await (_isTheory ? new XunitTheoryTestCaseRunner(this, DisplayName, _skippedReason, constructorArguments, _diagnosticMessageSink, messageBus, aggregator, cancellationTokenSource).RunAsync()
                                              : new XunitTestCaseRunner(this, DisplayName, _skippedReason, constructorArguments, TestMethodArguments, messageBus, aggregator, cancellationTokenSource).RunAsync());
            }

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
                    // The mumber of parameters in Payload does not match the number of arguments in the item.Message and thus cause a
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

        public override void Serialize(IXunitSerializationInfo data)
        {
            base.Serialize(data);
            data.AddValue("_isTheory", _isTheory);
            data.AddValue("_skippedReason", _skippedReason);
            data.AddValue("_failFastDuration", _failFastDuration.ToString());
        }

        public override void Deserialize(IXunitSerializationInfo data)
        {
            _isTheory = data.GetValue<bool>("_isTheory");
            _skippedReason = data.GetValue<string>("_skippedReason");
            string failFastDurationStr = data.GetValue<string>("_failFastDuration");
            if (!string.IsNullOrEmpty(failFastDurationStr))
            {
                _failFastDuration = TimeSpan.Parse(failFastDurationStr);
            }
            else
            {
                _failFastDuration = System.Threading.Timeout.InfiniteTimeSpan;
            }

            base.Deserialize(data);
        }
    }
}
