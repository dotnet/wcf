// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
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

        internal WcfTestCase(IXunitTestCase testCase, string skippedReason)
        {
            _testCase = testCase;
            _skippedReason = skippedReason;
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

        public Task<RunSummary> RunAsync(
            IMessageSink diagnosticMessageSink, IMessageBus messageBus, object[] constructorArguments,
            ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
        {
            return new XunitTestCaseRunner(this, DisplayName, _skippedReason, constructorArguments, TestMethodArguments, messageBus, aggregator, cancellationTokenSource).RunAsync();
        }

        public void Serialize(IXunitSerializationInfo info) { _testCase.Serialize(info); }
    }
}
