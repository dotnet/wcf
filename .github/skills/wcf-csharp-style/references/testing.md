# Test Style

Load this reference whenever C# tests or test infrastructure are created,
modified, reviewed, or ported.

## Follow the Owning Test Project

- Use the test framework and custom attributes already established by the
  project.
- Scenario and integration tests use the repository's WCF-aware fact, theory,
  condition, issue, and outer-loop infrastructure.
- Package unit-test projects may use standard xUnit attributes where that is
  the established pattern.
- Do not mechanically replace custom attributes with plain facts or theories.
- Add tests to an existing relevant file when practical.

## Names

Use a descriptive pattern such as:

```text
Member_Condition_ExpectedResult
```

Scenario-style names are acceptable when they communicate binding, transport,
configuration, and behavior more clearly.

```csharp
public async Task HttpRequestReplyEchoString()
{
}
```

## Test Behavior

- Test one clear behavior.
- Async tests return `Task` or a supported `ValueTask`, never `async void`.
- Use async assertion APIs for asynchronous failures.
- Use precise assertions.
- Verify that the intended path executed.
- Assert identity when identity matters.
- Assert invocation order when order matters.
- Assert argument parameter names when relevant.
- For faults or protocol failures, assert codes and details, not only the
  exception type.
- Avoid arbitrary sleeps.
- Clean up resources in `finally` or through established helpers.
- Check filtered test run counts and skipped tests.

Prefer xUnit assertions over throwing generic exceptions from assertion
helpers.

## Data-Driven Tests

- Use inline data for small constant matrices.
- Use named member-data factories for complex or non-constant cases.
- Give data factories scenario-oriented names.
- Keep each row understandable in failure output.

## Conditions and Platforms

- Prefer project-provided platform, runtime, issue, and skip attributes.
- Do not repeat ad-hoc operating-system checks in test bodies.
- Keep skip reasons specific and actionable.
- Pin culture through test infrastructure for culture-dependent assertions.

## Fixtures and Hosts

- Use existing helpers to create hosts, clients, factories, and service
  providers.
- Use class fixtures for expensive per-class setup.
- Use collection fixtures for exclusive external resources.
- Inject test output or logging through the test framework.
- Configure test services through the test host or factory.
- Do not mutate production global state when a scoped test host can be used.
- Dispose hosts, clients, scopes, channels, and factories through their
  established cleanup helpers.

## Timeouts and Determinism

- Prefer ephemeral ports over fixed ports.
- Await observable state instead of sleeping.
- Bound asynchronous, network, and external-resource waits with a realistic
  timeout.
- Use very short timeouts only when timeout behavior itself is under test.
- Increase timeouts under a debugger through shared infrastructure.
- Isolate tests requiring exclusive OS resources or external containers.
- Capture unexpected server exceptions through test logging.
- Document known product limitations instead of hiding them with retries.

## Readability

Prefer explicit setup over a highly abstract test DSL.

Do not add comments that only say:

```text
Arrange
Act
Assert
```

The method name and code should make the phases clear.

## Test Completion Checklist

- [ ] Correct fact/theory and condition attributes are used.
- [ ] The test name identifies the behavior.
- [ ] Async work is awaited.
- [ ] Identity and order are asserted when relevant.
- [ ] Fault details are asserted when relevant.
- [ ] No arbitrary sleep is used.
- [ ] Resources are cleaned up.
- [ ] The focused filter ran the expected test count.
- [ ] Platform and culture conditions use shared infrastructure.

