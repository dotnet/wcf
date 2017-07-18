Please always start your contribution with an Issue, per [.NET Core Contributions](https://github.com/dotnet/corefx/blob/master/Documentation/project-docs/contributing.md). The recommended contribution workflow is described in those guidelines.

The team will prefer incoming PRs and issues that align with the project priorities. 

However, it is OK to create a broad range of issues. There may be other developers that are interested in the same topic. If a strong community viewpoint establishes itself on an issue outside of the core priority, then the team will likely engage on the issue.

Contributing to WCF Repo
----------------------------

The .NET Core team maintains several guidelines for contributing to the .NET Core repos, which are provided below. Many of these are straightforward, while others may seem  subjective. A .NET Core team member will be happy to explain why a guideline is defined as it is. We can also update the documentation, as needed.

- [Contribution Bar](#contribution-bar) describes the general bar that the team uses to accept changes.
- [.NET Core Contributions](https://github.com/dotnet/corefx/blob/master/Documentation/project-docs/contributing.md) for general .NET Contribution guidelines.


Contribution "Bar"
------------------

Because this WCF repository is a work in progress, our highest priority is completing the implementations for the existing libraries. In general, code that throws PlatformNotSupportedException indicates code under construction. We will be making and accepting changes that provide implementations for these.

Regarding new features, the changes must have a **demonstrably broad impact** of a **mainline scenario** and be **low risk** in order to be accepted. They must also satisfy the published guidelines for .NET Core. 

Please see [Breaking Changes](https://github.com/dotnet/corefx/blob/master/Documentation/coding-guidelines/breaking-changes.md) to understand our requirements on changes that could impact compatibility. Please pay the most attention to changes that affect the [Public Contract](https://github.com/dotnet/corefx/blob/master/Documentation/coding-guidelines/breaking-changes.md#bucket-1-public-contract). We will not accept changes that break compatibility.
