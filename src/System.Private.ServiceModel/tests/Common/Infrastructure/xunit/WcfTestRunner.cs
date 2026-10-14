// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Infrastructure.Common
{
    internal sealed class WcfTestRunner : XunitTestRunner
    {
        private readonly IMessageSink _diagnosticMessageSink;
        private readonly int _maxAttempts;
        private readonly int _retryDelayMilliseconds;

        internal WcfTestRunner(ITest test, IMessageBus messageBus, Type testClass, object[] constructorArguments,
            MethodInfo testMethod, object[] testMethodArguments, string skipReason,
            IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes, ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource, IMessageSink diagnosticMessageSink,
            int maxAttempts, int retryDelayMilliseconds)
            : base(test, messageBus, testClass, constructorArguments, testMethod, testMethodArguments, skipReason,
                beforeAfterAttributes, aggregator, cancellationTokenSource)
        {
            _diagnosticMessageSink = diagnosticMessageSink;
            _maxAttempts = maxAttempts;
            _retryDelayMilliseconds = retryDelayMilliseconds;
        }

        protected override async Task<decimal> InvokeTestMethodAsync(ExceptionAggregator aggregator)
        {
            if (_maxAttempts == 1)
            {
                return await base.InvokeTestMethodAsync(aggregator);
            }

            Stopwatch stopwatch = Stopwatch.StartNew();
            ExceptionAggregator attemptAggregator = new ExceptionAggregator(aggregator);

            for (int attempt = 1; ; attempt++)
            {
                // Each invocation creates and disposes its own test class. Only the final
                // attempt's exceptions reach xUnit's result-reporting layer.
                await base.InvokeTestMethodAsync(attemptAggregator);

                if (!attemptAggregator.HasExceptions || attempt >= _maxAttempts || CancellationTokenSource.IsCancellationRequested)
                {
                    break;
                }

                _diagnosticMessageSink.OnMessage(new DiagnosticMessage(
                    "Test '{0}' failed on attempt {1} of {2}. Retrying in {3} ms.{4}{5}",
                    DisplayName, attempt, _maxAttempts, _retryDelayMilliseconds,
                    Environment.NewLine, attemptAggregator.ToException()));

                if (_retryDelayMilliseconds > 0)
                {
                    try
                    {
                        await Task.Delay(_retryDelayMilliseconds, CancellationTokenSource.Token);
                    }
                    catch (OperationCanceledException) when (CancellationTokenSource.IsCancellationRequested)
                    {
                        break;
                    }
                }

                if (CancellationTokenSource.IsCancellationRequested)
                {
                    break;
                }

                attemptAggregator = new ExceptionAggregator(aggregator);
            }

            aggregator.Aggregate(attemptAggregator);
            return (decimal)stopwatch.Elapsed.TotalSeconds;
        }
    }
}
