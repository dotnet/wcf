# Changed-Code-Only Formatting

Use this reference before running formatting or analyzer fixes.

## Fundamental Limitation

`dotnet format` is file-scoped, not line-scoped. `--include` limits which files
are processed, but the formatter may change untouched legacy lines inside an
included modified file.

Therefore:

- Verification can safely target all changed files.
- Automatic whole-file application is safe by default only for new files.
- Existing modified files require hunk-level edits and diff review.

## Determine Scope

The helper combines:

- committed branch changes relative to a merge base
- unstaged tracked changes
- staged changes
- untracked C# files

```powershell
pwsh .github/skills/wcf-csharp-style/scripts/Get-ChangedCSharpFiles.ps1 `
  -BaseRef upstream/main
```

It excludes generated, reference, baseline, build-output, and managed
infrastructure paths by default.

The formatter helper groups changed files into their owning workspace:

- Main client libraries and tests use `System.ServiceModel.sln`.
- `src/dotnet-svcutil/` uses `dotnet-svcutil.sln`.
- `src/svcutilcore/` uses `dotnet-svcutil.xmlserializer.sln`.

Use `-Workspace <solution>` to override automatic grouping for a known special
case.

## Verify

Use verification after manual edits:

```powershell
pwsh .github/skills/wcf-csharp-style/scripts/Invoke-ChangedCSharpFormat.ps1 `
  -VerifyOnly `
  -Category All `
  -BaseRef upstream/main
```

Verification uses:

- the repository workspace
- `--no-restore`
- an explicit changed-file list
- `--verify-no-changes`

It does not write files.

## Apply to New Files

```powershell
pwsh .github/skills/wcf-csharp-style/scripts/Invoke-ChangedCSharpFormat.ps1 `
  -Apply `
  -Category All `
  -BaseRef upstream/main
```

Default apply mode:

- formats newly added eligible C# files
- warns about modified existing C# files
- skips those existing files

## Modified Existing Files

Format modified code while editing:

1. Apply the guide to the changed hunks.
2. Run verification.
3. Read diagnostics for the touched code.
4. Make targeted edits.
5. Inspect `git diff --word-diff` or a normal patch.
6. Reject unrelated whitespace and line-ending changes.

Do not run automatic whole-file apply merely because a modified file appears in
the branch.

The formatter helper exposes `-AllowModifiedFiles`, but agents must not use it
automatically. Use it only when the user explicitly authorizes whole-file
formatting and accepts possible legacy churn.

## Exclusions

Default exclusions include:

```text
**/bin/**
**/obj/**
**/.dotnet/**
**/ref/**
eng/common/**
src/dotnet-svcutil/lib/tests/Baselines/**
src/dotnet-svcutil/lib/tests/TestCases/**
**/*.g.cs
**/*.generated.cs
**/*.designer.cs
**/*AssemblyInfo.cs
**/*.notsupported.cs
**/AsmOffsets.cs
```

Do not bypass exclusions when:

- the task does not own the generated file
- the file is a golden baseline
- a reference surface must be generated
- the file is managed by shared infrastructure

If an excluded file legitimately needs an update, use the owning process and
validate the resulting artifact separately.

## Diff Hygiene

After formatting:

```powershell
git diff --check
git diff --stat
git diff
```

Confirm:

- only intended files changed
- no unrelated lines were reformatted
- no generated or baseline files changed unexpectedly
- no line-ending-only diff appeared
- comments and behavior were not rewritten by analyzer fixes

## Build and Test

Formatting verification is not compilation verification.

Run:

1. the smallest affected build
2. focused unit tests
3. scenario tests only when behavior requires them

Check test counts and skips.

## Failure Handling

If formatting wants to change untouched lines:

1. Do not accept the whole-file result.
2. Restore or manually reverse only the formatter-created unrelated hunks.
3. Keep the intended implementation changes.
4. Apply style directly to the touched code.
5. Run verification again.

Do not discard the user's implementation changes while cleaning formatter
output.
