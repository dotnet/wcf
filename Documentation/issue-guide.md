### Overview
This page outlines how the WCF team thinks about and handles issues.  In general, we follow the guidance given at [CoreFx Issue Guidelines](https://github.com/dotnet/corefx/blob/master/Documentation/issue-guide.md).

Add new issues or track existing ones at [Issues](https://github.com/dotnet/wcf/issues).

### Labels
We use GitHub labels on our issues in order to classify them.  We have the following categories per issue:
* **Area**: These labels call out the assembly or assemblies the issue applies to. In addition to tags per assembly, we have a few other tags: Infrastructure, for issues that relate to our build or test infrastructure, and Meta for issues that deal with the repository itself, the direction of the .NET Core Platform, our processes, etc.
* **Type**: These labels classify the type of issue.  We use the following types:
 * [api addition](https://github.com/dotnet/wcf/labels/api%20addition): Issues which would add APIs to an assembly.
 * [bug](https://github.com/dotnet/wcf/labels/bug): Issues for bugs in an assembly.
 * [documentation](https://github.com/dotnet/wcf/labels/documentation): Issues relating to documentation (e.g. incorrect documentation, enhancement requests)
 * [enhancement](https://github.com/dotnet/wcf/labels/enhancement): Issues related to an assembly that improve it, but do not add new APIs (e.g performance improvements, code cleanup)
 * [feature request](https://github.com/dotnet/wcf/labels/feature%20request) is a label we assign to issues that are requests for new features. This is intended as a way to tag issues where we want active community discussion on the importance of a proposed new features
 * [not yet supported](https://github.com/dotnet/wcf/labels/not%20yet%20supported) is a label we assign to issues for features that work in Windows Store but throw PlatformNotSupportedException from the libraries in this repo. Most of these are under development and are expected to be fixed.
 * [stress](https://github.com/dotnet/wcf/labels/stress) is a label we assign for issues discovered when running in stress labs or heavily loaded applications
 * [test bug](https://github.com/dotnet/wcf/labels/test%20bug) is a label we assign to all issues related to testing. We label these further with "infrastructure" (for test infrastructure changes) or "enhancement" (for missing tests or those that need enhancement).

**Ownership**: These labels are used to specify who owns specific issue. Issues without an ownership tag are still considered "up for discussion" and haven't been approved yet. We have the following different types of ownership:
 * [up-for-grabs](https://github.com/dotnet/wcf/labels/up-for-grabs): Small sections of work which we believe are well scoped. These sorts of issues are a good place to start if you are new.  Anyone is free to work on these issues.
 * [feature approved](https://github.com/dotnet/wcf/labels/feature%20approved): Larger scale issues.  Like up-for-grabs, anyone is free to work on these issues, but they may be trickier or require more work.
 * [grabbed by community](https://github.com/dotnet/wcf/labels/grabbed%20by%20community): Someone outside the CoreFx team has assumed responsibility for addressing this issue and is working on a fix.  The comments for the issue will call out who is working on it.  You shouldn't try to address the issue without coordinating with the owner.
 * [grabbed by assignee](https://github.com/dotnet/wcf/labels/grabbed%20by%20assignee): Like grabbed by community, except the person the issue is assigned to is making a fix.  This will be someone on the CoreFx team.
* **Project Management**: These labels are used to facilitate the team's [Kanban Board](https://huboard.com/dotnet/wcf).  Labels indicate the current status and swim lane.  
 * [0 - Backlog](https://github.com/dotnet/wcf/issues?q=is%3Aopen+is%3Aissue+label%3A%220+-+Backlog%22): Tasks that are not yet ready for development or are not yet prioritized for the current development cycle.
 * [1 - Up Next](https://github.com/dotnet/wcf/issues?q=is%3Aopen+is%3Aissue+label%3A%221+-+Up+Next%22): Tasks that are ready for development and prioritized above the rest of the backlog.
 * [2 - In Progress](https://github.com/dotnet/wcf/issues?q=is%3Aopen+is%3Aissue+label%3A%222+-+In+Progress%22): Tasks that are under active development.
 * [3 - Done](https://github.com/dotnet/wcf/issues?q=is%3Aopen+is%3Aissue+label%3A%223+-+Done%22): Tasks that are finished.  There should be no open issues in the Done stage.


In addition to the above, we have labels for each of the individual libraries within the WCF repo to help categorize them:
 * [System.ServiceModel.Primitives](https://github.com/dotnet/wcf/labels/System.ServiceModel.Primitives)
 * [System.ServiceModel.Http](https://github.com/dotnet/wcf/labels/System.ServiceModel.Http)
 * [System.ServiceModel.NetTcp](https://github.com/dotnet/wcf/labels/System.ServiceModel.NetTcp)
 * [System.ServiceModel.Duplex](https://github.com/dotnet/wcf/labels/System.ServiceModel.Duplex)
 * [System.ServiceModel.Security](https://github.com/dotnet/wcf/labels/System.ServiceModel.Security)

### Assignee
We assign each issue to a WCF team member.  In most cases, the assignee will not be the one who ultimately fixes the issue (that only happens in the case where the issue is tagged "grabbed by assignee"). The purpose of the assignee is to act as a point of contact between the WCF team and the community for the issue and make sure it's driven to resolution.  If you're working on an issue and get stuck, please reach out to the assignee (just at mention them)  and they will work to help you out.
