# Legacy Porting and Modernization

Use this reference when porting .NET Framework code or modernizing legacy C#.

Modernization must preserve behavior. Newer syntax is not automatically better.

## Stage 1: Understand the Contract

Before editing, identify:

- public and explicit interface API
- reference-assembly ownership
- serialization and interop shape
- protocol and wire behavior
- exception types and messages
- threading, locks, and lazy initialization
- callback and inspector order
- correlation indexes
- disposal and lifetime
- target frameworks and language version
- platform conditions
- resource strings and tracing

Build or run the closest baseline before a large port when practical.

## Stage 2: Port the Minimum Implementation

- Start from the source that matches the intended client behavior.
- Copy only required types and helpers.
- Do not port service-host-only implementations into a client library.
- Preserve public and explicit interface members.
- Replace unavailable dependencies deliberately.
- Search for existing helpers before adding new ones.
- Re-audit helper files after removing unsupported branches.

## Stage 3: Apply Mechanical Style

Candidates include:

| Legacy pattern | Preferred form |
| --- | --- |
| `this.field` | `_field` |
| `private int count;` | `private int _count;` |
| `private static object cache;` | `private static object s_cache;` |
| `"parameter"` | `nameof(parameter)` when supported |
| `new T[0]` | `Array.Empty<T>()` |
| namespace-scoped usings | file-level usings |
| unbraced condition | Allman braced block |
| repeated type in `new Type()` | `var` when the type is obvious |
| explicit left type plus constructor | target-typed `new()` when supported |
| dead commented code | delete it |

Every candidate still requires compatibility review.

## Stage 4: Simplify Properties Carefully

Convert a field and property to an auto-property only when the field is pure
storage.

Do not convert:

- lazy properties
- validated properties
- delegated properties
- synchronized or volatile state
- serialized or interop fields
- fields used through reflection
- fields passed by `ref`
- fields that record explicit initialization
- fields that preserve tracing or exception behavior

Use expression-bodied members for simple returns. Keep block bodies for
behavior.

## Stage 5: Use Modern Language Features Selectively

Consider:

- pattern matching
- object and collection initializers
- target-typed `new`
- expression-bodied members
- null propagation
- throw expressions
- switch expressions
- Task-based internals

Use a feature only when:

- the target language version supports it
- every target framework supports required APIs
- the result is clearer
- behavior remains unchanged

## Stage 6: Remove Obsolete Patterns

Review:

- obsolete static-analysis suppressions
- copied framework helpers with maintained equivalents
- obsolete type forwarding
- custom pools or caches
- old APM implementation machinery
- service-only code in client packages
- dead compatibility branches
- stale comments and unused helpers

Do not remove compatibility code until supported scenarios are understood.

## Unsafe Style Transformations

These are not purely stylistic:

- changing public API shape
- changing nullable public annotations
- changing property storage semantics
- changing collection type or ordering
- replacing loops with LINQ in hot paths
- changing null comparisons with overloaded operators
- replacing project exception or resource helpers
- removing `ref` or `out`
- changing callback order or array indexes
- changing lock objects
- changing lazy initialization
- changing disposal behavior
- changing serialization attributes
- changing interop layout
- removing compatibility interfaces
- introducing APIs unsupported by older targets

These changes require separate design and test review.

## Generated and Public API Files

Before editing, determine whether a file is:

- generated
- reference assembly source
- baseline or fixture input
- public API surface
- imported build content
- owned by another generator

Do not manually format generated output. Update it through the owning process.

Public API changes must not be hidden inside a style-only change.

## Shared Source

If a source file is compiled into several projects:

1. Find every consumer.
2. Check each consumer's symbols, language version, and target frameworks.
3. Build and test all affected consumers.

## Port Completion Checklist

- [ ] Behavior and API shape are preserved.
- [ ] Unsupported service-host code was not copied into the client.
- [ ] Field and property conversions are semantically safe.
- [ ] Exception, resource, tracing, and fatal-filter paths are preserved.
- [ ] Locks, laziness, ordering, and disposal are unchanged.
- [ ] Every target framework can compile the syntax and APIs.
- [ ] Generated and reference files use their owning workflow.
- [ ] Focused tests cover the ported behavior.

