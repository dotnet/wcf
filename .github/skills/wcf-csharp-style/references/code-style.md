# C# Style Rules

Use these rules for new or intentionally modified C# code. Do not reformat
unrelated legacy code.

## Formatting

### Indentation

- Use spaces, not tabs.
- Use four spaces for C#.
- Do not align declarations with large runs of spaces.
- Remove trailing whitespace.
- End files with a newline.
- Preserve the existing file's line endings.

### Braces

Use Allman braces and always brace control-flow bodies.

```csharp
if (isEnabled)
{
    Process();
}
else
{
    Stop();
}
```

Do not write:

```csharp
if (isEnabled)
    Process();
```

### Blank lines

- Keep one blank line between members.
- Use blank lines to separate logical steps.
- Avoid multiple consecutive blank lines.
- After a completed control-flow block, keep one blank line before the next
  statement at the same scope.
- Do not add that blank line before `else`, `catch`, `finally`, or `}`.

Good:

```csharp
if (count > 0)
{
    Process(first, second);
}

Target value = (Target)source;
```

Avoid:

```csharp
if (count > 0)
{
    Process(first, second);
}
Target value = (Target)source;
```

### Spacing

- Use one space after control-flow keywords.
- Use spaces around binary operators.
- Use one space after commas.
- Do not use spaces inside parentheses or brackets.
- Do not add a space between a method name and `(`.
- Do not add a space after a cast.

Good:

```csharp
if (count > 0)
{
    Process(first, second);
}

Target value = (Target)source;
```

Avoid:

```csharp
if(count>0)
{
    Process ( first , second );
}
```

## File Structure

### License header

- Preserve the required license header.
- If the file begins with a license or copyright block, keep exactly one blank
  line after the final comment line.
- The first using, namespace, attribute, or declaration follows that single
  blank line.

```csharp
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
```

### Usings

- Place usings outside the namespace.
- Put `System.*` namespaces first.
- Sort consistently.
- Remove unused usings.
- Use one directive per line.

### Namespaces

- Follow the owning project's namespace style.
- Preserve block-scoped namespaces in existing files.
- Use file-scoped namespaces for new files only when the component clearly
  permits them.
- Do not mass-convert namespace style.

### Member order

Use this general order:

1. Constants.
2. Static fields.
3. Instance fields.
4. Constructors.
5. Events.
6. Properties.
7. Public and internal methods.
8. Private helpers.
9. Nested types.

## Naming and Modifiers

| Symbol | Style |
| --- | --- |
| Namespace, type, method, property, event | PascalCase |
| Interface | `I` + PascalCase |
| Parameter and local | camelCase |
| Constant | PascalCase |
| Private/internal instance field | `_camelCase` |
| Private/internal static field | `s_camelCase` |
| Type parameter | `T` or descriptive `TName` |

Use positive boolean names such as `isEnabled`, `hasValue`, and `canRetry`.

State accessibility explicitly:

```csharp
private static void Validate()
{
}
```

Use a consistent modifier order:

```text
public, private, protected, internal, static, extern, new, virtual,
abstract, sealed, override, readonly, unsafe, volatile, async
```

Use readonly fields when they are assigned only during initialization. Do not
use readonly when mutation is intentional through `ref`, `Interlocked`, lazy
initialization, pooling, or serialization.

Make utility classes static. Seal internal implementation types when
inheritance is not required. Do not seal public extension points without API
review.

## Avoid Unnecessary `this.`

Before:

```csharp
this._items.Add(item);
return this.CreateResult();
```

After:

```csharp
_items.Add(item);
return CreateResult();
```

Use `this.` only to resolve real ambiguity or when required by the language.

## Local Types and Object Creation

Make the type obvious at the declaration.

Use `var` when the right-hand side names the exact type:

```csharp
var factory = new ChannelFactory();
var lookup = new Dictionary<string, object>();
```

Avoid redundant repetition:

```csharp
ChannelFactory factory = new ChannelFactory();
```

An explicit type with target-typed `new()` is also clear:

```csharp
Dictionary<string, object> properties = new();
```

Use an explicit type when a method call or complex expression hides the result:

```csharp
Result result = GetResult();
ProcessResult value = service.Process();
```

Avoid:

```csharp
var result = GetResult();
var value = service.Process();
```

Use language keywords:

```csharp
int count;
string name;
bool isEnabled;
object value;
```

Use `nameof` for parameter and member names when supported:

```csharp
throw new ArgumentNullException(nameof(value));
```

Do not use `nameof` for user-visible messages, protocol values, serialized
names, or configuration keys.

Use reusable empty values when identity is not important:

```csharp
return Array.Empty<object>();
```

## Properties and Fields

### Auto-properties

Use auto-properties for pure storage.

Before:

```csharp
private bool _isEnabled;

public bool IsEnabled
{
    get { return _isEnabled; }
    set { _isEnabled = value; }
}
```

After:

```csharp
public bool IsEnabled { get; set; }
```

Keep a backing field when it:

- validates or normalizes values
- delegates to another object
- performs tracing
- invalidates a cache
- is lazily initialized
- is synchronized, volatile, or accessed through `Interlocked`
- is passed by `ref`
- is a lock object
- has field-level attributes
- affects serialization, reflection, or interop layout
- records whether a value was explicitly set
- preserves a specific exception path

### Expression bodies

Prefer `=>` for a simple getter or single-expression member.

Before:

```csharp
public int ReceiveRetryCount
{
    get { return _receiveRetryCount; }
}
```

After:

```csharp
public int ReceiveRetryCount => _receiveRetryCount;
```

Fixed values use the same style:

```csharp
public DeliveryGuarantee Guarantee => DeliveryGuarantee.ExactlyOnce;
```

Simple accessors may use:

```csharp
public int ReceiveRetryCount
{
    get => _receiveRetryCount;
    set => _receiveRetryCount = value;
}
```

Keep block bodies for validation, logging, tracing, locking, lazy
initialization, comments, or multiple operations.

Properties should be quick, deterministic, and free of surprising side
effects. Use methods for expensive or state-changing operations.

## Nullability and Validation

Follow the project's nullable configuration.

- Use `?` only when null is valid.
- Initialize non-nullable members before use.
- Validate values entering through configuration, reflection, serialization,
  interop, dependency injection, or unannotated assemblies.
- Do not add redundant checks solely to silence uncertainty.
- Use `!` only when an invariant is proven and cannot be expressed safely.
- Do not change public nullable annotations without compatibility review.

Prefer reference-pattern checks when testing reference identity:

```csharp
if (value is null)
{
    throw new ArgumentNullException(nameof(value));
}
```

Use `== null` or `!= null` when overloaded equality semantics are intentionally
required. Do not mechanically replace existing null comparisons on custom
types without checking their operators.

Use the component's existing validation and exception helpers. Do not replace
them merely to use a newer API.

Use null propagation and coalescing when clear:

```csharp
scope?.Dispose();
string displayName = name ?? string.Empty;
```

Use throw expressions for simple constructor assignment:

```csharp
_binding = binding ?? throw new ArgumentNullException(nameof(binding));
```

Use a normal guard block when validation needs tracing, formatting, or several
steps.

## Exceptions, Resources, and Logging

Preserve exception type, parameter name, resource message, wrapping, stack
behavior, fatal filtering, and tracing.

Rethrow with:

```csharp
catch
{
    throw;
}
```

Never use `throw exception;` to rethrow the current exception.

- Catch only exceptions that can be handled.
- Do not add broad catches that hide failure.
- Do not return a success-shaped fallback after an unexpected exception.
- Keep user-visible text in the component's resource system.
- Format a resource only when it has placeholders.
- Do not replace project exception or resource helpers solely for style.

Use structured logging:

```csharp
logger.LogDebug("Accepted connection {ConnectionId}", connectionId);
```

Avoid interpolated logging in hot paths:

```csharp
logger.LogDebug($"Accepted connection {connectionId}");
```

Do not log credentials, tokens, message bodies, or sensitive data. Use cached
or source-generated logging delegates in hot paths when the component provides
them.

## Control Flow

- Prefer early returns that reduce nesting.
- Use pattern matching when it combines a type check and cast.
- Use switch expressions for direct value mapping.
- Use switch statements for state machines, tracing, mutations, or complex
  protocol logic.
- Avoid nested conditional expressions.
- Make local functions static when they do not capture state.

Use explicit loops when order, indexes, allocations, mutations, early exit, or
exception boundaries matter.

```csharp
for (int i = 0; i < inspectors.Length; i++)
{
    states[i] = inspectors[i].CreateState();
}
```

Use LINQ for simple queries when it improves readability. Avoid hidden multiple
enumeration and do not mechanically convert loops.

## Objects, Collections, and Dependency Injection

Use object and collection initializers when clearer.

Choose collection types deliberately:

- `IReadOnlyList<T>` for ordered read-only access.
- `IReadOnlyCollection<T>` for count and enumeration.
- `Dictionary<TKey, TValue>` for mutable key lookup.
- Immutable collections for fixed shared state.
- Specialized collections when they enforce ordering or validation.

Do not change collection types when callback, middleware, serialization,
protocol, or correlation order matters.

For dependency injection:

- Prefer constructor injection for required dependencies.
- Store dependencies in readonly fields or get-only properties.
- Validate required dependencies.
- Avoid global service location.
- Contain framework-required service location at the adapter boundary.
- Do not dispose dependencies owned by the container.

## Async and Concurrency

- Methods returning `Task`, `Task<T>`, `ValueTask`, or `ValueTask<T>` should
  normally end with `Async`.
- Do not rename overrides, interface implementations, or public compatibility
  APIs whose names are fixed.
- Return `Task` or `Task<T>` by default.
- Use `ValueTask` only for measured high-frequency or existing ValueTask-based
  contracts.
- Avoid `async void` except true event-handler contracts.
- Do not block with `.Result` or `.Wait()`.
- Pass cancellation tokens through supported APIs.
- Observe every task by awaiting, returning, or explicitly routing failures.
- Do not mass-add or remove `ConfigureAwait(false)`; follow the component's
  execution-context convention.
- Preserve public APM compatibility members during modernization.
- Keep the final return on its own line.

For `TaskCompletionSource`, normally use:

```csharp
TaskCompletionSource<object?> completion =
    new(TaskCreationOptions.RunContinuationsAsynchronously);
```

Use `TrySetResult`, `TrySetException`, and `TrySetCanceled` when completion can
race. Use explicit synchronization when publishing or replacing shared
completion state.

Cancellation sources:

- Dispose owned and linked sources.
- Do not create per-iteration sources without a real need.
- Do not reuse canceled sources.
- Preserve the correct token when propagating cancellation.

## Disposal

Use the owning project's using style.

Use a block when disposal must happen before later statements:

```csharp
using (Stream stream = OpenStream())
{
    Process(stream);
}

ContinueAfterDisposal();
```

Use a declaration when the resource should live until the scope ends:

```csharp
using Stream stream = OpenStream();
Process(stream);
```

Preserve established close-or-abort patterns for faultable communication
objects. Do not introduce asynchronous disposal when callers depend on
synchronous cleanup.

## Platform and Performance Code

- Use platform annotations when available.
- Use runtime platform checks before platform-specific behavior.
- Keep interop isolated.
- Do not suppress platform diagnostics without a real guard.

Use allocation-focused features only in measured hot paths:

- cache static delegates
- use static lambdas when no capture is needed
- use spans and memory only with clear lifetime rules
- use `stackalloc` only for small bounded buffers
- never stack-allocate a size controlled by untrusted input
- avoid unnecessary sequence copies
- document non-obvious lifetime and pooling rules

Do not introduce spans, pooling, or caches without measurements showing the
allocation matters.

## Comments and Documentation

- Explain why, not what the code already states.
- Remove commented-out code and unused helpers.
- Fix stale or incorrect comments.
- Preserve compatibility, threading, security, and protocol explanations.
- Use XML documentation for public APIs when required.
- Use regions sparingly; do not hide an oversized type with regions.
