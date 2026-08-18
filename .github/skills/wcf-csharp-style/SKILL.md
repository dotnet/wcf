---
name: wcf-csharp-style
description: >-
  Use this skill whenever creating, editing, reviewing, porting, modernizing,
  or formatting C# code or C# tests in dotnet/wcf. Applies repository code
  style, legacy-porting safeguards, analyzer guidance, and changed-code-only
  formatting. Use for implementation work, test changes, PR cleanup, code
  review, analyzer fixes, and .NET Framework ports. Never mass-format existing
  source or hand-format generated, reference, or baseline files.
license: MIT
compatibility: Requires git and a .NET SDK. Optional helper scripts use PowerShell.
metadata:
  audience: coding-agents
  scope: dotnet-wcf-csharp
---

# WCF C# Style

Apply consistent C# style without creating unrelated formatting churn or
changing behavior.

## Automatic Use

Use this skill before editing and again before final validation whenever a task
creates, modifies, reviews, ports, or formats C# code.

Load only the reference files needed for the current task:

| Task | Read |
| --- | --- |
| Normal C# implementation or review | `references/code-style.md` |
| Unit or scenario tests | `references/testing.md` |
| .NET Framework port or modernization | `references/modernization.md` |
| Formatting, generated files, or diff scope | `references/scope-and-formatting.md` |

## When Not to Use

Do not use this skill to:

- Reformat the repository, solution, project, or directory wholesale.
- Clean up unrelated legacy code in a file being edited.
- Hand-edit generated, reference-assembly, or baseline output.
- Make a public API, serialization, threading, or protocol change under the
  label of formatting.
- Replace project-specific exception, resource, tracing, or test helpers solely
  with newer syntax.

## Inputs

| Input | Required | Description |
| --- | --- | --- |
| Target files or feature area | Yes | Files that will be created, changed, or reviewed |
| Mode | No | `pre-edit`, `post-edit`, or `review`; default is both pre- and post-edit |
| Base ref | No | Branch comparison base, normally `upstream/main` or `origin/main` |
| Validation scope | No | Smallest affected project and focused tests |

## Authority Order

Apply guidance in this order:

1. Owning project configuration and target frameworks.
2. Root and nested `.editorconfig` files.
3. Repository Copilot instructions.
4. Established style in nearby maintained source.
5. This skill and its references.

If the target project uses an older language version, do not introduce syntax
that it cannot compile.

## Non-Negotiable Scope Rules

1. Modify only files required by the task.
2. Within an existing file, format only new or intentionally modified code.
3. Never run solution-wide `dotnet format` without explicit user approval.
4. Never accept formatter-only changes outside intended hunks.
5. Do not hand-format generated, reference, baseline, or imported build output.
6. Preserve behavior, public API, wire contracts, serialization, threading,
   locking, ordering, correlation indexes, disposal, and exception behavior.
7. Inspect the final diff for unrelated whitespace or line-ending changes.

`dotnet format` operates on complete files, not changed line ranges. The helper
scripts therefore:

- Verify all changed C# files without writing by default.
- Apply whole-file formatting automatically only to new C# files.
- Require an explicit override before applying to modified existing files.

## Pre-Edit Workflow

### 1. Inspect the target

Before editing:

1. Read `.editorconfig` and the owning project file.
2. Check the project's target frameworks and language version.
3. Determine whether the file is generated, shared, a reference surface, or a
   baseline.
4. Read representative nearby files.
5. Identify the smallest build and test projects that cover the change.

### 2. Determine current C# scope

```powershell
pwsh .github/skills/wcf-csharp-style/scripts/Get-ChangedCSharpFiles.ps1
```

For branch work, provide the intended base explicitly:

```powershell
pwsh .github/skills/wcf-csharp-style/scripts/Get-ChangedCSharpFiles.ps1 `
  -BaseRef upstream/main
```

An empty result means there is no current C# formatting scope. Do not fall back
to formatting the solution.

### 3. Load the relevant rules

- Read `references/code-style.md` for normal source work.
- Also read `references/testing.md` when tests are touched.
- Also read `references/modernization.md` for ported or legacy code.
- Read `references/scope-and-formatting.md` before running a formatter.

## Apply Style While Editing

Use these high-frequency rules during implementation:

- Keep the required license header, followed by exactly one blank line.
- Use four spaces, Allman braces, and no trailing whitespace.
- Keep one blank line after a completed control-flow block before the next
  same-scope statement, except before `else`, `catch`, `finally`, or `}`.
- Place usings outside the namespace and sort `System.*` first.
- Use `_camelCase` instance fields, `s_camelCase` static fields, and PascalCase
  constants.
- Omit unnecessary `this.`.
- Use `var` when the right-hand side names the exact type, such as
  `new ChannelFactory()`.
- Use an explicit type when a method call or complex expression hides the
  result type.
- Target-typed `new()` is clear when the left-hand type is explicit.
- Prefer auto-properties only for pure storage.
- Prefer `=>` for a simple getter or single-expression member.
- Keep block bodies for validation, tracing, locking, laziness, or multiple
  operations.
- Use language keywords and `nameof` when supported.
- Follow nullable annotations and do not hide warnings with unjustified `!`.
- Observe every task; do not use `.Result` or `.Wait()` in async flows.
- Follow the owning component's exception, resource, tracing, and cleanup
  helpers.
- Preserve explicit loops where order, indexes, allocations, or mutation
  matter.

The topical references contain the full rules and examples.

## Changed-Code-Only Formatting

### Verify changed C# files

Verification is the safe default:

```powershell
pwsh .github/skills/wcf-csharp-style/scripts/Invoke-ChangedCSharpFormat.ps1 `
  -VerifyOnly `
  -BaseRef upstream/main
```

The script uses `dotnet format` with an explicit changed-file list and
`--verify-no-changes`. It never writes in verification mode.

Changed files are grouped into the main client solution, `dotnet-svcutil`
solution, or XML serializer solution. Use `-Workspace <solution>` only when a
different owning workspace is known.

### Apply formatting to new files

Whole-file formatting is safe for newly added files:

```powershell
pwsh .github/skills/wcf-csharp-style/scripts/Invoke-ChangedCSharpFormat.ps1 `
  -Apply `
  -BaseRef upstream/main
```

By default, `-Apply` skips modified existing files.

### Modified existing files

For modified existing files:

1. Format the new or changed hunks while editing.
2. Run verification against the changed-file set.
3. Review diagnostics in the context of the touched lines.
4. Make targeted fixes with precise edits.
5. Inspect `git diff` and reject unrelated formatter churn.

Do not automatically use `-AllowModifiedFiles`. That switch is reserved for an
explicit user request to allow whole-file formatting of already modified files.

### Formatting categories

The formatter helper accepts:

- `All` - whitespace, style, and analyzers.
- `Whitespace`
- `Style`
- `Analyzers`

Use `All` for final verification. Use a narrower category when diagnosing or
applying a specific class of change.

## Exclusions

The changed-file script excludes these by default:

- Reference assembly directories and generated reference source.
- Build outputs and local SDK directories.
- Generated assembly information and generated C# naming patterns.
- `.notsupported.cs` and `AsmOffsets.cs`.
- Tool baselines and fixture input trees.
- Arcade-managed `eng/common` content.

If an API or generated artifact intentionally needs an update, use its owning
generation process. Do not bypass exclusions merely to make it look formatted.

## Validation

After editing:

1. Run `git diff --check`.
2. Run changed-file formatting verification.
3. Inspect `git diff --stat` and the full diff.
4. Confirm no excluded or unrelated files changed.
5. Build the smallest affected project.
6. Run the smallest focused test set.
7. Check the test count and skipped tests.
8. Expand validation only when the change is shared or cross-cutting.

Typical targeted flow after restore:

```powershell
dotnet build <affected-project.csproj> --no-restore
dotnet test <affected-tests.csproj> --no-build --no-restore `
  --filter "FullyQualifiedName~RelevantArea"
```

Use repository build scripts when they are required to bootstrap the local SDK
or test infrastructure.

## Review Mode

When reviewing code:

1. Determine the changed files and hunks.
2. Apply the same style rules without requesting unrelated cleanup.
3. Report style issues only when they are in changed code or directly required
   for correctness.
4. Distinguish analyzer-enforced issues from advisory preferences.
5. Do not propose public API or behavior changes as style fixes.

## Stop and Ask

Request guidance when:

- Public API or reference-surface ownership is unclear.
- A formatter wants to change untouched legacy lines.
- The target language version or framework support is unclear.
- Serialization, interop, locking, ordering, or disposal might change.
- A generated or baseline file appears to require hand editing.
- Multiple established styles conflict in the target component.

## Completion Report

Report:

- Files created or modified.
- References loaded.
- Formatting mode and exact scope.
- Excluded or skipped files.
- Build and tests run.
- Any style exception or unresolved warning.
