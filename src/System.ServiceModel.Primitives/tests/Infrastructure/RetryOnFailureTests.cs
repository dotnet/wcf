// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Common;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

public class RetryOnFailureTests
{
    [WcfFact]
    public static void Attribute_Defaults_To_No_Delay()
    {
        RetryOnFailureAttribute attribute = new RetryOnFailureAttribute(3);

        Assert.Equal(3, attribute.MaxAttempts);
        Assert.Equal(0, attribute.RetryDelayMilliseconds);
    }

    [WcfTheory]
    [InlineData(1, 0)]
    [InlineData(3, 10000)]
    [InlineData(int.MaxValue, int.MaxValue)]
    public static void Attribute_Accepts_Valid_Settings(int maxAttempts, int retryDelayMilliseconds)
    {
        RetryOnFailureAttribute attribute = new RetryOnFailureAttribute(maxAttempts, retryDelayMilliseconds);

        Assert.Equal(maxAttempts, attribute.MaxAttempts);
        Assert.Equal(retryDelayMilliseconds, attribute.RetryDelayMilliseconds);
    }

    [WcfTheory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public static void Attribute_Rejects_Nonpositive_MaxAttempts(int maxAttempts)
    {
        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new RetryOnFailureAttribute(maxAttempts));

        Assert.Equal("maxAttempts", exception.ParamName);
    }

    [WcfTheory]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public static void Attribute_Rejects_Negative_Delays(int retryDelayMilliseconds)
    {
        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new RetryOnFailureAttribute(3, retryDelayMilliseconds));

        Assert.Equal("retryDelayMilliseconds", exception.ParamName);
    }

    [WcfTheory]
    [InlineData(nameof(RetrySubject.Passes), 1, 0)]
    [InlineData(nameof(RetrySubject.FailsThenPasses), 3, 0)]
    [InlineData(nameof(RetrySubject.FailsThenPassesAsync), 3, 0)]
    [InlineData(nameof(RetrySubject.AlwaysFails), 3, 1)]
    [InlineData(nameof(RetrySubject.SingleAttempt), 1, 1)]
    public static async Task Facts_Report_Only_The_Final_Outcome(string methodName, int attempts, int failures)
    {
        RetrySubject.Reset();

        ExecutionResult result = await ExecuteAsync(typeof(RetrySubject), methodName);

        Assert.Equal(attempts, RetrySubject.Attempts);
        Assert.Equal(attempts, RetrySubject.Created);
        Assert.Equal(attempts, RetrySubject.Initialized);
        Assert.Equal(attempts, RetrySubject.AsyncDisposed);
        Assert.Equal(attempts, RetrySubject.Disposed);
        Assert.Equal(1, result.Summary.Total);
        Assert.Equal(failures, result.Summary.Failed);
        Assert.Single(result.Messages.OfType<ITestStarting>());
        Assert.Single(result.Messages.OfType<ITestFinished>());
        Assert.Equal(failures, result.Messages.OfType<ITestFailed>().Count());
        Assert.Equal(1 - failures, result.Messages.OfType<ITestPassed>().Count());
        Assert.Equal(attempts - 1, result.Diagnostics.OfType<IDiagnosticMessage>().Count());

        if (failures > 0)
        {
            ITestFailed failure = Assert.Single(result.Messages.OfType<ITestFailed>());
            Assert.Contains($"Failure on attempt {attempts}", Assert.Single(failure.Messages));
        }
    }

    [WcfTheory]
    [InlineData(typeof(UnannotatedSubject), 1, 1)]
    [InlineData(typeof(MethodRetrySubject), 3, 0)]
    [InlineData(typeof(ClassRetrySubject), 3, 0)]
    [InlineData(typeof(InheritedClassRetrySubject), 3, 0)]
    [InlineData(typeof(SingleAttemptClassSubject), 1, 1)]
    public static async Task Policies_Apply_To_Methods_And_Actual_Test_Classes(Type testClass, int attempts, int failures)
    {
        UnannotatedSubject.Attempts = 0;

        ExecutionResult result = await ExecuteAsync(testClass, nameof(UnannotatedSubject.Run));

        Assert.Equal(attempts, UnannotatedSubject.Attempts);
        Assert.Equal(failures, result.Summary.Failed);
    }

    [WcfTheory]
    [InlineData(true, false)]
    [InlineData(false, false)]
    [InlineData(true, true)]
    [InlineData(false, true)]
    public static async Task Theories_Retry_Only_Failing_Rows(bool preEnumerateTheories, bool nonSerializableData)
    {
        TheorySubject.Attempts = new int[3];

        string methodName = nonSerializableData ? nameof(TheorySubject.WithNonSerializableRows) : nameof(TheorySubject.Run);
        ExecutionResult result = await ExecuteAsync(typeof(TheorySubject), methodName, preEnumerateTheories);

        Assert.Equal(new[] { 1, 2, 3 }, TheorySubject.Attempts);
        Assert.Equal(preEnumerateTheories && !nonSerializableData ? 3 : 1, result.DiscoveredCases);
        Assert.Equal(3, result.Summary.Total);
        Assert.Equal(1, result.Summary.Failed);
        Assert.Equal(0, result.Summary.Skipped);
        Assert.Equal(3, result.Messages.OfType<ITestStarting>().Count());
        Assert.Equal(3, result.Messages.OfType<ITestFinished>().Count());
        Assert.Equal(2, result.Messages.OfType<ITestPassed>().Count());
        Assert.Single(result.Messages.OfType<ITestFailed>());
        Assert.Equal(3, result.Diagnostics.OfType<IDiagnosticMessage>()
            .Count(diagnostic => diagnostic.Message.Contains("Retrying in")));
    }

    [WcfTheory]
    [InlineData(true)]
    [InlineData(false)]
    public static async Task Method_Policy_Can_Disable_Theory_Retries(bool preEnumerateTheories)
    {
        TheorySubject.Attempts = new int[3];

        ExecutionResult result = await ExecuteAsync(
            typeof(TheorySubject), nameof(TheorySubject.SingleAttempt), preEnumerateTheories);

        Assert.Equal(new[] { 1, 1, 1 }, TheorySubject.Attempts);
        Assert.Equal(3, result.Summary.Total);
        Assert.Equal(2, result.Summary.Failed);
        Assert.Single(result.Messages.OfType<ITestPassed>());
        Assert.Equal(2, result.Messages.OfType<ITestFailed>().Count());
        Assert.Empty(result.Diagnostics);
    }

    [WcfTheory]
    [InlineData(true)]
    [InlineData(false)]
    public static async Task Theories_Preserve_Skipped_Rows(bool preEnumerateTheories)
    {
        TheorySubject.Attempts = new int[3];

        ExecutionResult result = await ExecuteAsync(
            typeof(TheorySubject), nameof(TheorySubject.WithSkippedRow), preEnumerateTheories);

        Assert.Equal(new[] { 0, 2, 0 }, TheorySubject.Attempts);
        Assert.Equal(2, result.Summary.Total);
        Assert.Equal(0, result.Summary.Failed);
        Assert.Equal(1, result.Summary.Skipped);
        Assert.Equal("Skipped data row", Assert.Single(result.Messages.OfType<ITestSkipped>()).Reason);
        Assert.Single(result.Messages.OfType<ITestPassed>());
        Assert.Single(result.Diagnostics.OfType<IDiagnosticMessage>());
    }

    [WcfTheory]
    [InlineData(nameof(RetrySubject.ExplicitSkip))]
    [InlineData(nameof(RetrySubject.ConditionSkip))]
    [InlineData(nameof(RetrySubject.IssueSkip))]
    public static async Task Skipped_Tests_Are_Not_Invoked_Or_Retried(string methodName)
    {
        RetrySubject.Reset();

        ExecutionResult result = await ExecuteAsync(typeof(RetrySubject), methodName);

        Assert.Equal(0, RetrySubject.Attempts);
        Assert.Equal(0, RetrySubject.Created);
        Assert.Equal(1, result.Summary.Total);
        Assert.Equal(1, result.Summary.Skipped);
        Assert.Single(result.Messages.OfType<ITestSkipped>());
        Assert.Empty(result.Diagnostics);
    }

    [WcfFact]
    public static async Task Serialization_Preserves_Retry_Delays_And_Attempts()
    {
        RetrySubject.Reset();

        ExecutionResult result = await ExecuteAsync(typeof(RetrySubject), nameof(RetrySubject.DelayedRetry));

        Assert.Equal(2, RetrySubject.Attempts);
        Assert.Equal(0, result.Summary.Failed);
        double intervalMilliseconds = (RetrySubject.AttemptTimestamps[1] - RetrySubject.AttemptTimestamps[0])
            * 1000.0 / Stopwatch.Frequency;
        // Task.Delay uses the system clock, which can have a roughly 16 ms resolution.
        Assert.True(intervalMilliseconds >= 84, $"The retry interval was only {intervalMilliseconds} ms.");
        Assert.True(result.Summary.Time >= 0.084m);
        Assert.Contains("Retrying in 100 ms.", Assert.Single(result.Diagnostics.OfType<IDiagnosticMessage>()).Message);
    }

    [WcfTheory]
    [InlineData(nameof(RetrySubject.AlwaysFails))]
    [InlineData(nameof(RetrySubject.LongDelay))]
    public static async Task Cancellation_Stops_Retries_Without_Waiting_For_The_Delay(string methodName)
    {
        RetrySubject.Reset();

        using (CancellationTokenSource cancellation = new CancellationTokenSource())
        {
            ExecutionResult result = await ExecuteAsync(typeof(RetrySubject), methodName,
                cancellationTokenSource: cancellation,
                onDiagnosticMessage: message =>
                {
                    if (message is IDiagnosticMessage diagnostic && diagnostic.Message.Contains("Retrying in"))
                    {
                        cancellation.Cancel();
                    }
                });

            Assert.True(cancellation.IsCancellationRequested);
            Assert.Equal(1, RetrySubject.Attempts);
            Assert.Equal(1, result.Summary.Failed);
            Assert.Single(result.Diagnostics.OfType<IDiagnosticMessage>());
        }
    }

    [WcfFact]
    public static async Task Existing_Setup_Failures_Are_Not_Retried()
    {
        RetrySubject.Reset();
        ExceptionAggregator aggregator = new ExceptionAggregator();
        aggregator.Add(new InvalidOperationException("Fixture setup failed"));

        ExecutionResult result = await ExecuteAsync(
            typeof(RetrySubject), nameof(RetrySubject.Passes), aggregator: aggregator);

        Assert.Equal(0, RetrySubject.Attempts);
        Assert.Equal(0, RetrySubject.Created);
        Assert.Equal(1, result.Summary.Failed);
        Assert.Contains("Fixture setup failed", Assert.Single(Assert.Single(result.Messages.OfType<ITestFailed>()).Messages));
        Assert.Empty(result.Diagnostics);
    }

    [WcfFact]
    public static async Task Output_From_All_Attempts_Is_Preserved()
    {
        OutputSubject.Attempts = 0;

        ExecutionResult result = await ExecuteAsync(typeof(OutputSubject), nameof(OutputSubject.Run),
            constructorArguments: new object[] { new Func<TestOutputHelper>(() => new TestOutputHelper()) });

        Assert.Equal(3, OutputSubject.Attempts);
        ITestPassed passed = Assert.Single(result.Messages.OfType<ITestPassed>());
        Assert.Contains("Attempt 1", passed.Output);
        Assert.Contains("Attempt 2", passed.Output);
        Assert.Contains("Attempt 3", passed.Output);
        Assert.Equal(3, result.Messages.OfType<ITestOutput>().Count());
        Assert.Empty(result.Messages.OfType<ITestFailed>());
    }

    private static async Task<ExecutionResult> ExecuteAsync(Type testClass, string methodName,
        bool preEnumerateTheories = true, CancellationTokenSource cancellationTokenSource = null,
        Action<IMessageSinkMessage> onDiagnosticMessage = null, ExceptionAggregator aggregator = null,
        object[] constructorArguments = null)
    {
        MessageRecorder diagnostics = new MessageRecorder(onDiagnosticMessage);
        MessageRecorder messages = new MessageRecorder();
        ReflectionTypeInfo typeInfo = new ReflectionTypeInfo(testClass);
        ITestAssembly testAssembly = new TestAssembly(new ReflectionAssemblyInfo(testClass.Assembly));
        ITestCollection testCollection = new TestCollection(testAssembly, null, nameof(RetryOnFailureTests));
        ITestClass testClassInfo = new TestClass(testCollection, typeInfo);
        IMethodInfo methodInfo = typeInfo.GetMethods(true).Single(method => method.Name == methodName);
        ITestMethod testMethod = new TestMethod(testClassInfo, methodInfo);
        IAttributeInfo attribute = methodInfo.GetCustomAttributes(typeof(FactAttribute)).Single();
        IXunitTestCaseDiscoverer discoverer = testClass.GetMethod(methodName).IsDefined(typeof(TheoryAttribute), inherit: true)
            ? (IXunitTestCaseDiscoverer)new WcfTheoryDiscoverer(diagnostics)
            : new WcfFactDiscoverer(diagnostics);
        DiscoveryOptions options = new DiscoveryOptions();
        options.SetValue("xunit.discovery.PreEnumerateTheories", preEnumerateTheories);
        IXunitTestCase[] testCases = discoverer.Discover(options, testMethod, attribute).ToArray();
        RunSummary summary = new RunSummary();

        using (CancellationTokenSource timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30)))
        using (CancellationTokenSource cancellation = cancellationTokenSource == null
            ? CancellationTokenSource.CreateLinkedTokenSource(timeout.Token)
            : CancellationTokenSource.CreateLinkedTokenSource(timeout.Token, cancellationTokenSource.Token))
        {
            foreach (IXunitTestCase discoveredTestCase in testCases)
            {
                IXunitTestCase testCase = SerializationHelper.Deserialize<IXunitTestCase>(
                    SerializationHelper.Serialize(discoveredTestCase));
                summary.Aggregate(await testCase.RunAsync(
                    diagnostics, messages, constructorArguments ?? Array.Empty<object>(),
                    aggregator ?? new ExceptionAggregator(), cancellation));

                if (cancellation.IsCancellationRequested)
                {
                    break;
                }
            }
        }

        return new ExecutionResult(summary, messages.Messages, diagnostics.Messages, testCases.Length);
    }

    private sealed class ExecutionResult
    {
        public ExecutionResult(RunSummary summary, List<IMessageSinkMessage> messages,
            List<IMessageSinkMessage> diagnostics, int discoveredCases)
        {
            Summary = summary;
            Messages = messages;
            Diagnostics = diagnostics;
            DiscoveredCases = discoveredCases;
        }

        public RunSummary Summary { get; }

        public List<IMessageSinkMessage> Messages { get; }

        public List<IMessageSinkMessage> Diagnostics { get; }

        public int DiscoveredCases { get; }
    }

    private sealed class MessageRecorder : LongLivedMarshalByRefObject, IMessageBus, IMessageSink
    {
        private readonly Action<IMessageSinkMessage> _onMessage;

        public MessageRecorder(Action<IMessageSinkMessage> onMessage = null)
        {
            _onMessage = onMessage;
        }

        public List<IMessageSinkMessage> Messages { get; } = new List<IMessageSinkMessage>();

        public bool OnMessage(IMessageSinkMessage message)
        {
            Messages.Add(message);
            _onMessage?.Invoke(message);
            return true;
        }

        public bool QueueMessage(IMessageSinkMessage message) => OnMessage(message);

        public void Dispose()
        {
        }
    }

    private sealed class DiscoveryOptions : ITestFrameworkDiscoveryOptions
    {
        private readonly Dictionary<string, object> _values = new Dictionary<string, object>();

        public TValue GetValue<TValue>(string name) =>
            _values.TryGetValue(name, out object value) ? (TValue)value : default;

        public void SetValue<TValue>(string name, TValue value) => _values[name] = value;
    }

#pragma warning disable xUnit1000 // These private subjects are discovered and executed explicitly by the tests above.
    [RetryOnFailure(3)]
    private sealed class RetrySubject : IDisposable, IAsyncLifetime
    {
        private bool _invoked;

        public RetrySubject()
        {
            Created++;
        }

        public static int Attempts { get; private set; }

        public static int Created { get; private set; }

        public static int Initialized { get; private set; }

        public static int AsyncDisposed { get; private set; }

        public static int Disposed { get; private set; }

        public static List<long> AttemptTimestamps { get; } = new List<long>();

        public static void Reset()
        {
            Attempts = 0;
            Created = 0;
            Initialized = 0;
            AsyncDisposed = 0;
            Disposed = 0;
            AttemptTimestamps.Clear();
        }

        public Task InitializeAsync()
        {
            Initialized++;
            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            AsyncDisposed++;
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Disposed++;
        }

        private int RecordAttempt()
        {
            Assert.False(_invoked, "Each attempt must receive a fresh test class instance.");
            _invoked = true;
            AttemptTimestamps.Add(Stopwatch.GetTimestamp());
            return ++Attempts;
        }

        [WcfFact]
        [RetryOnFailure(3, int.MaxValue)]
        public void Passes() => RecordAttempt();

        [WcfFact]
        public void FailsThenPasses()
        {
            int attempt = RecordAttempt();
            Assert.True(attempt == 3, $"Failure on attempt {attempt}");
        }

        [WcfFact]
        public async Task FailsThenPassesAsync()
        {
            await Task.Yield();
            FailsThenPasses();
        }

        [WcfFact]
        public void AlwaysFails() => Assert.Fail($"Failure on attempt {RecordAttempt()}");

        [WcfFact]
        [RetryOnFailure(1, int.MaxValue)]
        public void SingleAttempt() => AlwaysFails();

        [WcfFact]
        [RetryOnFailure(2, 100)]
        public void DelayedRetry() => Assert.True(RecordAttempt() == 2);

        [WcfFact]
        [RetryOnFailure(3, int.MaxValue)]
        public void LongDelay() => AlwaysFails();

        [WcfFact(Skip = "Explicit skip")]
        public void ExplicitSkip() => AlwaysFails();

        [WcfFact]
        [Condition(nameof(NeverRun))]
        public void ConditionSkip() => AlwaysFails();

        [WcfFact]
        [Issue(9999999)]
        public void IssueSkip() => AlwaysFails();

        public static bool NeverRun => false;
    }

    private class UnannotatedSubject
    {
        public UnannotatedSubject()
        {
        }

        public static int Attempts { get; set; }

        [WcfFact]
        public virtual void Run() => Assert.True(++Attempts == 3);
    }

    private sealed class MethodRetrySubject : UnannotatedSubject
    {
        public MethodRetrySubject()
        {
        }

        [WcfFact]
        [RetryOnFailure(3)]
        public override void Run() => base.Run();
    }

    [RetryOnFailure(3)]
    private class ClassRetrySubject : UnannotatedSubject
    {
        public ClassRetrySubject()
        {
        }
    }

    private sealed class InheritedClassRetrySubject : ClassRetrySubject
    {
        public InheritedClassRetrySubject()
        {
        }
    }

    [RetryOnFailure(1)]
    private sealed class SingleAttemptClassSubject : ClassRetrySubject
    {
        public SingleAttemptClassSubject()
        {
        }
    }

    [RetryOnFailure(3)]
    private sealed class TheorySubject
    {
        public TheorySubject()
        {
        }

        public static int[] Attempts { get; set; }

        [WcfTheory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public async Task Run(int row)
        {
            await Task.Yield();
            int attempt = ++Attempts[row];
            Assert.True(row != 2 && attempt > row, $"Failure for row {row}, attempt {attempt}");
        }

        [WcfTheory]
        [RetryOnFailure(1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public Task SingleAttempt(int row) => Run(row);

        [WcfTheory]
        [InlineData(0, Skip = "Skipped data row")]
        [InlineData(1)]
        public void WithSkippedRow(int row) => Assert.True(++Attempts[row] == 2);

        public static IEnumerable<object[]> GetNonSerializableRows()
        {
            for (int row = 0; row < 3; row++)
            {
                yield return new object[] { new TheoryArgument(row) };
            }
        }

        [WcfTheory]
        [MemberData(nameof(GetNonSerializableRows))]
        public Task WithNonSerializableRows(TheoryArgument argument) => Run(argument.Row);
    }

    private sealed class TheoryArgument
    {
        public TheoryArgument(int row)
        {
            Row = row;
        }

        public int Row { get; }
    }

    private sealed class OutputSubject
    {
        private readonly ITestOutputHelper _output;

        public OutputSubject(ITestOutputHelper output)
        {
            _output = output;
        }

        public static int Attempts { get; set; }

        [WcfFact]
        [RetryOnFailure(3)]
        public void Run()
        {
            int attempt = ++Attempts;
            _output.WriteLine($"Attempt {attempt}");
            Assert.True(attempt == 3);
        }
    }
#pragma warning restore xUnit1000
}
