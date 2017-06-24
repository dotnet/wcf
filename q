[33mcommit f3aeb84bd29fd192188aa528a322b2f7c2da8ef0[m
Author: Gerardo Hurtado Lozano <kanegerardo_@hotmail.com>
Date:   Fri Jun 23 14:51:45 2017 -0700

    Fixed image issue where image tag was using original title and url, for RSS formatting
    Image issue fixed and added some tests

[33mcommit 6a1887cac3f6dd46320ce1e8bec347e6740b7aac[m
Merge: 027382c 0dffa28
Author: Gerardo Hurtado Lozano <kanegerardo_@hotmail.com>
Date:   Fri Jun 23 13:58:02 2017 -0700

    Merge branch 'CustomParser' into InitialSyndicationFeed
    Added custom parsing for RSS formats.
    Cleaned the code.
    Executed CodeFormatter tool.
    Added test for custom parsing.

[33mcommit 0dffa28379af03f76650313c4b80f1bc83378dac[m
Author: Gerardo Hurtado Lozano <kanegerardo_@hotmail.com>
Date:   Thu Jun 22 16:04:32 2017 -0700

    Test using custom parsers

[33mcommit dbe277e1ce465bfa4b69fc999f926373de76c08e[m
Author: Gerardo Hurtado Lozano <kanegerardo_@hotmail.com>
Date:   Thu Jun 22 14:51:05 2017 -0700

    Moved the call of itemParser to be called with each item

[33mcommit 04d25233cf2a96931c6c44d58eca16871598428e[m
Author: Gerardo Hurtado Lozano <kanegerardo_@hotmail.com>
Date:   Wed Jun 21 17:49:17 2017 -0700

    Formatted the code with CodeFormatter Tool

[33mcommit 49f4568f112eed94cc1c540f6f73a5ecfd93cedb[m
Author: Gerardo Hurtado Lozano <kanegerardo_@hotmail.com>
Date:   Wed Jun 21 17:34:26 2017 -0700

    Cleaned code from most of the unnecesary comments

[33mcommit 16fa0ff142a4346ddbb8e99f1af604ea1a4b383e[m
Author: Gerardo Hurtado Lozano <kanegerardo_@hotmail.com>
Date:   Wed Jun 21 16:53:46 2017 -0700

    Cleaned the code, deleted Diagnostics, fixed throwing resources

[33mcommit 0ac4065ecf93a09e8af9dd3a65e7bb527be1ddba[m
Author: Gerardo Hurtado Lozano <kanegerardo_@hotmail.com>
Date:   Wed Jun 21 10:40:44 2017 -0700

    Removed the dependence of the old SR class

[33mcommit 027382c74b5affc3e069d7648fc31d7a065b8654[m
Author: Gerardo Hurtado Lozano <kanegerardo_@hotmail.com>
Date:   Thu Jun 8 14:59:12 2017 -0700

    Initial SyndicationFeed

[33mcommit cf30e331860c9e805a23dd7e8e5e77050073f269[m
Author: Gerardo Hurtado Lozano <kanegerardo_@hotmail.com>
Date:   Wed Jun 21 09:32:21 2017 -0700

    Save changes

[33mcommit 994bb9aa8293b4dc4b15c489675f6d839a4e1b50[m
Author: Gerardo Hurtado Lozano <kanegerardo_@hotmail.com>
Date:   Tue Jun 20 17:41:24 2017 -0700

    Added custom parser delegates

[33mcommit a790965173a769f18a1e5b64971072512634d7f0[m
Author: Gerardo Hurtado Lozano <kanegerardo_@hotmail.com>
Date:   Mon Jun 19 19:26:38 2017 -0700

    Added more delegates as parsers

[33mcommit 3bf015439f2762572ac4f0f0bc776eab0c896c95[m
Author: Gerardo Hurtado Lozano <kanegerardo_@hotmail.com>
Date:   Mon Jun 19 11:25:36 2017 -0700

    Initial changes to add custom parsers

[33mcommit d58a3858e2615240f1250c2f1b9893368b8b41ff[m
Author: Gerardo Hurtado Lozano <kanegerardo_@hotmail.com>
Date:   Sat Jun 17 14:11:54 2017 -0700

    Added correct testnames and Copyright

[33mcommit d9639e5aa6d2c06a788ac88438577a2bc06700db[m
Author: Gerardo Hurtado Lozano <kanegerardo_@hotmail.com>
Date:   Fri Jun 16 14:30:02 2017 -0700

    Added the posibility that the user creates his own date parser when reading a feed from outside, also as part of our default date parser, if it can't parse the date it will just assign a default date to avoid the crash

[33mcommit 94f9971695715e9b7e7572de90bfa891e39f61ab[m
Author: Gerardo Hurtado Lozano <kanegerardo_@hotmail.com>
Date:   Thu Jun 15 10:23:45 2017 -0700

    Deleted some unused files

[33mcommit 56b7b207e747a8f44e384f7f12743327beb0b7ac[m
Author: Gerardo Hurtado Lozano <kanegerardo_@hotmail.com>
Date:   Mon Jun 12 17:59:30 2017 -0700

    Cleaning code for PR

[33mcommit 61f8625ed07ea54e787910bf71cebc52832a5303[m
Author: Gerardo Hurtado Lozano <kanegerardo_@hotmail.com>
Date:   Mon Jun 12 15:06:24 2017 -0700

    Fixed errors when reading feeds and replaced some buffers

[33mcommit 846ac0d403b748376351dcd08440a27de70f806b[m
Author: Gerardo Hurtado Lozano <kanegerardo_@hotmail.com>
Date:   Thu Jun 8 14:59:12 2017 -0700

    Changed some namespaces

[33mcommit d197fc152b604b66e2fc1bc9f0edc905205f1a2e[m
Author: Gerardo Hurtado Lozano <kanegerardo_@hotmail.com>
Date:   Thu Jun 8 13:37:32 2017 -0700

    Added the needed references to get the code to compile

[33mcommit c2a7a859b4db6f1b83976aaada3f7a98677deb20[m
Author: Gerardo Hurtado Lozano <kanegerardo_@hotmail.com>
Date:   Thu Jun 8 10:18:43 2017 -0700

    Adding the existing classes of SyndicationFeed from .net fx

[33mcommit 9c86ae7b9632c28bc9ed7730a25625e4ee51996a[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Wed Jun 7 09:51:48 2017 -0700

    Microsoft.ServiceModel.Syndication skeleton project

[33mcommit ba67a913a9f7c521a9cf4704fb57e34a435d0d54[m
Merge: 6b4976f 82b4305
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri May 26 14:54:21 2017 -0700

    Merge pull request #1954 from StephenBonikowsky/ThrowPNSEforISerializableExceptions
    
    Remove serializable attribute and throw PNSE.

[33mcommit 82b43059c05bff0ce4a187f6f7ff168d6cc03582[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri May 26 13:37:24 2017 -0700

    Remove serializable attribute and throw PNSE.
    
    * Specifically on WCF Exceptions.

[33mcommit 6b4976f31ae47a1f2dd81fef0c9df52d363630c5[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Thu May 25 11:52:03 2017 -0700

    Disable PeerTrust certificate validation on OSX

[33mcommit fbd300250b25fb9bbda4803d96fab5a4670debae[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu May 25 16:09:29 2017 -0700

    Remove unnecessary content from the common project.json in 2.1.
    
    * Since we are currently only building and running for ns2.0 and everything else is being harvested we don't need several sections.

[33mcommit de0788a139ced4e5fec7c68ab7b90de6dc7e1bca[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu May 25 16:59:23 2017 -0700

    Enable test after Peer trust fix in CoreFx.
    
    * Re-enable test case NetTcp_SecModeTrans_CertValMode_PeerTrust_Succeeds_In_TrustedPeople to run on any unix.

[33mcommit bdb174bc392d3f8483587600c11b9010f27bb3f9[m
Merge: bae6533 2de73fd
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu May 25 09:09:06 2017 -0700

    Merge pull request #1937 from StephenBonikowsky/FixCoreFxDependencies
    
    Updated baseline for non-shared framework CoreFx packages ns1.3

[33mcommit bae65338f9e449b6a8298f773a0afc386236f2bf[m
Merge: 0509342 84dcc17
Author: Hong Dai <hongdai@microsoft.com>
Date:   Wed May 24 17:24:57 2017 -0700

    Merge pull request #1940 from StephenBonikowsky/ReEnableTests
    
    Enable tests that were disabled with CoreFx blocking bugs.

[33mcommit 2de73fdffe8134d9a86b7e04b43f46c240b5bfdf[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue May 23 10:41:31 2017 -0700

    Updated baseline for non-shared framework CoreFx packages ns1.3
    
    * Using 4.3.0 baseline for System.Reflection.DispatchProxy and System.Security.Principal.Windows in our NetStandard 1.3 dependency tree.

[33mcommit 84dcc1715a1298abee5ac75272a6d76c7a34bf9d[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed May 24 10:16:33 2017 -0700

    Enable and fix tests that were affected by CoreFx breaking changes to CRL.
    
    * Fixes #1926
    * Fixes #1941

[33mcommit 050934281bf99bf8a83793c4ca699a2befa21c06[m
Merge: 625ae29 3c30b27
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu May 18 11:54:20 2017 -0700

    Merge pull request #1931 from StephenBonikowsky/RevPackageVersionTo4.5
    
    Rev package version to 4.5

[33mcommit 3c30b27fa2fc469d220a10a188653603da745d65[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu May 18 10:27:19 2017 -0700

    Correction to S.P.SM baseline version and version mappings.
    
    * Added comments to explain purpose of the file.

[33mcommit b842f21fc6e88fcf9d36f9dfa2b055086c617cd8[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed May 17 11:32:13 2017 -0700

    RevingWcfPackagesTo4.5

[33mcommit c4acfe65bc831ee62f47b6f6bac4746082a9407a[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue May 16 17:19:43 2017 -0700

    Rev version of WCF packages to 4.5 preview1

[33mcommit 625ae290abaf6f29b99ce3c5e5bc0ff3363f31a1[m
Merge: b6226ec 49ba2e9
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon May 15 13:30:56 2017 -0700

    Merge pull request #1928 from tarekgh/UpdateLicense
    
    Update License Info

[33mcommit 49ba2e906b3437d9e2cd07f9fdfcfad6b0c7aba7[m
Author: Tarek Mahmoud Sayed <tarekms@microsoft.com>
Date:   Mon May 15 12:05:15 2017 -0700

    Update License Info

[33mcommit b6226ec43cc17044869b133ff0b52aac964134d7[m
Merge: 86128d5 95d4028
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu May 11 11:14:30 2017 -0700

    Merge pull request #1915 from StephenBonikowsky/AddSupportedRIDsFor2.0
    
    Adding support for these two latest versions of OSX and Fedora.

[33mcommit 95d40288c8ed1e462506052d438dba31c71929e2[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu May 11 10:12:33 2017 -0700

    Adding support for these two latest versions of OSX and Fedora.
    
    * The VSO build and run process has been updated for these new OS versions but they need to be added as supported RIDs for test runtime.

[33mcommit 86128d5512d9d39419b5e17b076cd858fbe8862f[m
Merge: ccb3371 6e87f5a
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu May 11 10:14:58 2017 -0700

    Merge pull request #1900 from StephenBonikowsky/AddSerializableAttribute
    
    Add Serializable attribute to WCF Exception classes.

[33mcommit 6e87f5a7ad64cc3f78a5b0aa08b1b75074d26b68[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue May 9 14:40:39 2017 -0700

    Add Serializable attribute to WCF Exception classes.

[33mcommit ccb3371a1416e86ff99957ae2a4ef7e7d81a10df[m
Merge: e980fa7 68bc59b
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu May 11 09:41:09 2017 -0700

    Merge pull request #1914 from StephenBonikowsky/SkipTestsIssue1913
    
    Disable tests due to CoreFx breaking change.

[33mcommit 68bc59b941593659c25d1ead13a25c31c9282925[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu May 11 09:16:21 2017 -0700

    Disable tests due to CoreFx breaking change.
    
    * Issue #1913: Breaking change from cofefx causing Peer trust scenarios to fail on Linux

[33mcommit e980fa739587e35967c651a2792d7c2c44a4269b[m
Merge: 43ee715 8d9bfd8
Author: Hong Dai <hongdai@microsoft.com>
Date:   Wed May 10 21:57:26 2017 -0700

    Merge pull request #1910 from hongdai/1574
    
    Fix 1574 and enable tests

[33mcommit 43ee71535fd5f77b335e9a502a877c20c7592fe9[m
Author: KKhurin <kkhurin@microsoft.com>
Date:   Wed May 10 20:16:25 2017 -0700

    Fix for #1694 (#1904)
    
    The fix is to add an UnhandledExceptionFrame for timer callback in CallOnceManager.AsyncWaiter
    Addressing feedback

[33mcommit df9b61561b04ebe7a733ba2676f569c65f035a2f[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue May 9 17:00:54 2017 -0700

    Fixes for supporting 461 and reving S.SM.Duplex assembly minor version
    
    * The contract for Duplex changed with https://github.com/dotnet/wcf/pull/1786

[33mcommit 37b15e3b5afe07e928cdacbd8906d6d85615879a[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue May 9 15:00:28 2017 -0700

    Including Security and Duplex for net461 support.

[33mcommit 37021cd6733e46bb820d1af91d860e492665e172[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri May 5 15:43:36 2017 -0700

    Adding support for net 4.6.1

[33mcommit 787369322509880baba84eb81033b26dc8ee1ecf[m
Author: Shin Mao <shmao@microsoft.com>
Date:   Wed May 10 17:57:21 2017 -0700

    Enable Test EchoComositeType_XmlSerializerFormat_Soap (#1905)
    
    * Fix #1884
    
    * Update Microsoft.NETCore.App
    
    * Update Microsoft.NETCore.App

[33mcommit 04a4868c2eb4bf2345082cf2468b3df6347d09b2[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Tue May 9 12:39:30 2017 -0700

    Adding proxy support to HTTP

[33mcommit f6ba5e2ce52931d019668c474f7d1d8ac69b8928[m
Merge: c975ecc 84da668
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed May 10 14:24:19 2017 -0700

    Merge pull request #1911 from tarekgh/UpdateLicenseUrl
    
    Update the License Url for packages

[33mcommit 84da668729914afeb0576c330f4add337aa8fddb[m
Author: Tarek Mahmoud Sayed <tarekms@microsoft.com>
Date:   Wed May 10 13:32:53 2017 -0700

    Update the License Url for packages

[33mcommit 8d9bfd8295f1c173bc8cbbf3865b5671c0dd06e6[m
Author: hongdai <hongdai@microsoft.com>
Date:   Wed May 10 13:21:32 2017 -0700

    Fix 1574 and enable tests
    * Use X509Certificate2 copy constructor as it becomes available in 2.0.
    * Enable those TCP tests require service certificate. They pass by using
    the X509Certificate2 copy constructor.

[33mcommit c975ecc239947c3bd4a3d3402d8a81bf41f3c9a2[m
Author: KKhurin <kkhurin@microsoft.com>
Date:   Wed May 10 11:31:12 2017 -0700

    Fixing #1402 (#1898)
    
    Fixing #1402
    Enabling session tests
    Exposing OperationContext.GetCallbackChannel in the contract
    Addressing feedback about wrapping timer callback

[33mcommit f8bea43ff5205ba4ae5ebcc29c55e82c5b7b09df[m
Author: Zhenlan Wang <zhenlwa@microsoft.com>
Date:   Tue May 9 23:12:26 2017 -0700

    Update docs of package versions for 2.0 Preview 1 release

[33mcommit 15591b6ddb20261a17376afe68153ca51c28c82d[m
Author: Zhenlan Wang <zhenlwa@microsoft.com>
Date:   Tue May 9 23:51:05 2017 -0700

    Update README.md

[33mcommit 3944b685d39b9b86c3d70ae5d9c4c862b706a591[m
Author: Zhenlan Wang <zhenlwa@microsoft.com>
Date:   Tue May 9 23:47:04 2017 -0700

    Fixes readme.md
    
    @dotnet-bot skip ci please

[33mcommit 751134a1ce15cfb499d3807694c8ade65dcbc1ed[m
Merge: 0961d82 9ad2325
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue May 9 18:19:44 2017 -0700

    Merge pull request #1899 from StephenBonikowsky/UpdateCoreFxDependencyTo25309-01
    
    Updating WCF dependencies on CoreFx and NetStandard to 25309-01

[33mcommit 9ad232535bf2b09207554463a3be23e39653b093[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue May 9 17:54:29 2017 -0700

    Updating WCF dependencies on CoreFx and NetStandard to 25309-01

[33mcommit 0961d825c923432f2bdf392dcf3ddbb9bdf74002[m
Author: Shin Mao <shmao@microsoft.com>
Date:   Fri May 5 13:37:20 2017 -0700

    To support soap encoded message. (#1885)
    
    * To support soap encoded message.
    Exposed XmlSerializerOperationBehavior.GetXmlMappings.
    
    Fix #1549.
    Fix #1797.
    
    * Fixed Issue with BasicHttpSoapTestServiceHostFactory.
    
    * Add one more test for Soap.
    
    * Refactor the tests.
    
    * Change to use IssueAttribute.

[33mcommit 1a314cd84b359f9ca29aec8acecb75731865ab7c[m
Author: KKhurin <kkhurin@microsoft.com>
Date:   Thu May 4 16:45:01 2017 -0700

    Adding scenario tests for WCF #1402 (#1874)
    
    * Adding scenario tests for WCF #1402
    Failed tests are marked with IssueAttribute
    
    * Updating the tests
    
    * Update ScenarioTestTypes.cs
    
    * Update session tests for #1402

[33mcommit 8b8829a5a4f6e19758e5cd2cacb218bcaea95dd3[m
Merge: eb8b5c8 ee0fdf9
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu May 4 11:47:53 2017 -0700

    Merge pull request #1890 from StephenBonikowsky/UpdateMasterPkgsToPreview2
    
    Updating pkgs and dependencies to Preview2.

[33mcommit ee0fdf9767da8ad2f6aaec88615703c6b0651c91[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu May 4 10:37:00 2017 -0700

    Updating pkgs and dependencies to Preview2.

[33mcommit eb8b5c8ab9e4865d65f9786371691dd778f1d714[m
Merge: 5b04dd9 d968df5
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed May 3 16:21:39 2017 -0700

    Merge pull request #1887 from StephenBonikowsky/ActiveIssueOSXCertFailures
    
    Active issue test cases failing due to issue #1886

[33mcommit d968df5331471cb2be7388510b74f24fc199076c[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed May 3 14:56:18 2017 -0700

    Skipping flaky test.
    
    * StreamingTests.NetTcp_TransportSecurity_Streamed_TimeOut_Long_Running_Operation has been failing more often recently, adding Active Issue to it so CI can be clean while we investigate.

[33mcommit a26afb27f6807a21d64cc4b2ed3cb9f236e7d67a[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed May 3 10:59:27 2017 -0700

    Active issue test cases failing due to issue #1886
    
    * Fix certificate installation on OSX10.12  #1886

[33mcommit 5b04dd93a073ed7d1f75ec92e8e461689e7bc3b6[m
Merge: 5681627 3fc55d2
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed May 3 09:47:11 2017 -0700

    Merge pull request #1883 from StephenBonikowsky/UpdateToOfficialPreview1PkgDependencies
    
    Updating to official preview1 dependency builds.

[33mcommit 3fc55d2939ef7c54f140cbc8134f6e6d2f655b30[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue May 2 15:46:29 2017 -0700

    Updating to official preview1 dependency builds.
    
    * For netstandard: preview1-25301-01
    * For corefx: preview1-25302-01
    * For coreclr: preview1-25301-02

[33mcommit 56816277d4185ca1db65ec5cf2a83fd517243283[m
Merge: c1a8ff0 8c8775d
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue May 2 16:01:21 2017 -0700

    Merge pull request #1882 from StephenBonikowsky/AddTestForXSFA
    
    Add test for xsfa

[33mcommit 8c8775d2a32d1013b6f2ab3530e2468d77e7f138[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Apr 18 16:20:00 2017 -0700

    Client side changes for new XmlSerializerFormatAttribute test scenario

[33mcommit 77d2117853065aae1b604550b73e3fee0d460b35[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Apr 6 14:31:18 2017 -0700

    Server side changes for new XmlSerializerFormatAttribute test scenario
    
    * Added a new endpoint.

[33mcommit 49a2d7d4af0aa188199138b537808bb797df5bba[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Mar 31 13:38:24 2017 -0700

    Adding scenario test for XmlSerializerFormatAttribute
    
    * Specifically verifying that the "SupportFauls" property works as expected.

[33mcommit c1a8ff0e09f531adcac83debe6f0d3965938b947[m
Merge: c23971d a2ffce1
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue May 2 15:20:15 2017 -0700

    Merge pull request #1881 from StephenBonikowsky/UpdateBaselineVersions
    
    Updating baseline to reference correct version.

[33mcommit a2ffce1fd676790d4bd4c48c5f62c36b6f48a76d[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue May 2 15:18:51 2017 -0700

    Updating baseline to reference correct version.

[33mcommit c23971d566181b4e783cad5d784a8ff07c0809e5[m
Author: hongdai <hongdai@microsoft.com>
Date:   Tue May 2 08:55:22 2017 -0700

    Enable tests
    
    * Enable http ambient credential test as corefx fix is in.

[33mcommit d341c49a55bda161bea1c7187abc7464354cae12[m
Merge: 67ddf5a 92e061a
Author: Matt Mitchell <mmitche@microsoft.com>
Date:   Tue May 2 11:29:12 2017 -0700

    Merge pull request #1880 from dotnet/mmitche-patch-1
    
    Use correct OSX monikers

[33mcommit 92e061a08c59fd8f5b5aab2ba8459ebedebfc2c1[m
Author: Matt Mitchell <mmitche@microsoft.com>
Date:   Tue May 2 10:43:25 2017 -0700

    Use correct OSX monikers

[33mcommit 67ddf5a155a4ebff505ae588a34702fc63c53b94[m
Merge: 4a87c58 a18a49c
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue May 2 10:27:22 2017 -0700

    Merge pull request #1879 from StephenBonikowsky/UpdateWcfPkgVersion
    
    Updating WCF self references to latest package version.

[33mcommit a18a49c343ff6a090b209ed25c2509f83777f934[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue May 2 10:18:02 2017 -0700

    Updating WCF self references to latest package version.

[33mcommit 4a87c583b4148fc3c973fec32840fd35b4e9336f[m
Merge: 96c85ca 63d38a1
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue May 2 09:06:02 2017 -0700

    Merge pull request #1877 from StephenBonikowsky/UpdateCIGroovy
    
    Update CI to OSX 10.12

[33mcommit 63d38a13af0a8d162a5ee5969f4cc9984ea695a3[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon May 1 14:43:16 2017 -0700

    Update CI to OSX 10.12

[33mcommit 96c85ca4715e435c9c28f99cf57e54c34c188622[m
Merge: 5c2ac25 cc54788
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon May 1 14:35:17 2017 -0700

    Merge pull request #1876 from StephenBonikowsky/UpdatePreReleaseLabel
    
    Update pre release label

[33mcommit cc54788ed5d40559fe498a9c178b79f8e50fe199[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon May 1 11:16:00 2017 -0700

    Updating Microsoft.NETCore.App to preview1 version.
    
    * Also had to remove "win10-arm64" as a supported OS for "coreFx.Test.netcoreapp2.0"

[33mcommit 3743ee6257a0b4cf7b606f6a25692c94d7fe80d0[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Apr 28 17:26:22 2017 -0700

    Updating to latest WCF packages.

[33mcommit 382d37eba1145a2d655a5b5fe13c28fa2988be3d[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Apr 28 15:32:54 2017 -0700

    Fixing test-runtime/project.json
    
    * Removing fedora.23 as it is no longer supported.
    * coreFx.Test.netcore50 did not support runtimes for uap10.0 on win10-x86 and win10-x64

[33mcommit 23611f6d9b905d8947aa9ca44ea1a8bff13aec1b[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Apr 28 13:25:33 2017 -0700

    Changed the NETStandard.Library dependence to preview1.
    
    * We will update to preview2 after we have a good preview1 build.

[33mcommit 6b28fb186db41c3901613ce3f0421723e7e41cc2[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Apr 28 10:42:31 2017 -0700

    Cleaning up references to netcore50
    
    * Did not change the test-common project.json as we may want to run tests against different supported frameworks.

[33mcommit 0d6c78981d1c05fbcfb07e313070166f8b0bb333[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Apr 28 10:21:59 2017 -0700

    Updating prerelease label for wcf pkgs from beta to preview1.
    
    * Also updating updating dependencies of netstandard.library and other corefx libraries to the latest versions.

[33mcommit 5c2ac25bede5647856fde22ae5a0e857a2ad39b7[m
Merge: c22eb21 e3ce355
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Apr 28 09:58:14 2017 -0700

    Merge pull request #1871 from StephenBonikowsky/HarvestNS1.3
    
    Building ns2.0 harvesting ns1.3

[33mcommit e3ce355341c5db4e9a1d52b05cdf7a7f62dfb431[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Apr 27 16:45:30 2017 -0700

    Building ns2.0 harvesting ns1.3 update3

[33mcommit 3085dcc39b53cb7165a302fe45cb5f7615ed53c8[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Apr 26 16:34:40 2017 -0700

    Building ns2.0 harvesting ns1.3 update2

[33mcommit cc993958e5e65baa7f2b3eca3721ad92203f8f6d[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Apr 26 11:25:34 2017 -0700

    Building ns2.0 harvesting ns1.3 update1

[33mcommit 45ed3a9ac1bf36377c1d8397b5cd342115dc9d41[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Apr 25 18:02:27 2017 -0700

    Building ns2.0 harvesting ns1.3

[33mcommit c22eb2120fea2f1284e192e2b84d3da275b363f7[m
Author: KKhurin <kkhurin@microsoft.com>
Date:   Mon Apr 24 19:17:17 2017 -0700

    Removing extra slash from URLs generated by ServiceUtilHelper (#1869)

[33mcommit 20d881032f5339e16c2e35627b48e57a43adadc8[m
Merge: ffd17cc fd9cdf5
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Apr 24 15:06:59 2017 -0700

    Merge pull request #1868 from StephenBonikowsky/FixNetStandard2.0Import
    
    Fix test project sync error in VSO

[33mcommit fd9cdf569fc31736d91c80e097e1bf57022d4199[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Apr 24 14:43:24 2017 -0700

    Fix test project sync error in VSO
    
    * Attempt to import the netstandard2.0 package fails during the sync.cmd step which is trying to do a package restore, at this point the package to import does not yet exist.

[33mcommit ffd17cc18ae7d59d947571c9611e483e2b5a4409[m
Author: Shin Mao <shmao@microsoft.com>
Date:   Mon Apr 24 14:27:08 2017 -0700

    Add System.ServiceModel.OperationFormatUse. (#1863)
    
    * Add OperationFormatUse.
    Ref #1549.
    
    * Add OperationFormatUse in S.P.SM.

[33mcommit 4e7cacf94845db3a4a5e902e963c53f8af43b1b9[m
Merge: ea4d395 8369b06
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Apr 21 13:51:28 2017 -0700

    Merge pull request #1865 from StephenBonikowsky/UpdateWcfPkgTo25221.02
    
    Updating WCF packages for test common to 25221

[33mcommit 8369b0660735816b3eb8aca13460935aa50ec95b[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Apr 21 13:48:12 2017 -0700

    Updating WCF packages for test common to 25221
    
    * This is the most recent WCF version that was built targeting ns2.0.

[33mcommit ea4d3951fe4a81f9df5cd975159128d32c5c5d52[m
Merge: 6147acc ded42e7
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Apr 21 11:58:30 2017 -0700

    Merge pull request #1864 from StephenBonikowsky/TargetSPSMto20
    
    Target S.P.SM to netstandard2.0

[33mcommit ded42e7fd9445170393307bf730b92d300dffbaa[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Apr 20 19:19:24 2017 -0700

    Target S.P.SM to netstandard2.0
    
    *S.P.SM builds successfully, but facades have not been updated to target ns2.0
    *Test projects have also not been updated.
    *I think the tests pass because ns2.0 is backward compatible?

[33mcommit 6147acc8cbf918bf5ea8e3aa1e9b9d41b7f3f8cc[m
Merge: 0e2b78a 146302a
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Apr 20 14:03:45 2017 -0700

    Merge pull request #1862 from MattGal/Fix_build_against_packages
    
    Fix build against packages

[33mcommit 146302a56174806966e5d51addf71e79cddcd1cf[m
Author: Matt Galbraith <mattgal@microsoft.com>
Date:   Thu Apr 20 12:06:27 2017 -0700

    Fix build against packages, as having an exclude entry for NS 2.0 broke build.

[33mcommit 0e2b78a7fe9607f7bcc574160ad44a46c48a9799[m
Merge: bfa34fb 950078f
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Apr 20 08:44:36 2017 -0700

    Merge pull request #1860 from StephenBonikowsky/UpdateScenarioProjectsToNetstandard2.0
    
    Updating all scenario test projects to use netstandard2.0

[33mcommit 950078f9f0354070531fdebfe0d3349316421c59[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Apr 19 16:12:51 2017 -0700

    Updating all scenario test projects to use netstandard2.0

[33mcommit bfa34fb924794fa0062b51296c2c61830cc46854[m
Merge: fbf1699 1355d43
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Apr 19 14:05:55 2017 -0700

    Merge pull request #1858 from MattGal/NS_2.0_3
    
    Get tests to build and run against NS2.0 package

[33mcommit fbf169997f47ac8671077a4e7f33afcfc0370b52[m
Merge: 4d99c7f ac4ab23
Author: Eric StJohn <ericstj@microsoft.com>
Date:   Wed Apr 19 13:33:56 2017 -0700

    Merge pull request #1857 from ericstj/packageUpdates
    
    Post 1.1 package / version updates

[33mcommit 1355d43b94759e64b91b526e29f854fe7f0db9d2[m
Author: Matt Galbraith <mattgal@microsoft.com>
Date:   Wed Apr 19 13:08:46 2017 -0700

    Add NetCoreApp2.0 to builds

[33mcommit 3e25a0384c2dbe808047076cd06cf337571ada6d[m
Author: Matt Galbraith <mattgal@microsoft.com>
Date:   Wed Apr 19 12:54:48 2017 -0700

    PR Feedback

[33mcommit 8d13a14121643d7e9918075a1ea02a78c0acd057[m
Author: Wes Haggard <Wes.Haggard@microsoft.com>
Date:   Fri Feb 3 15:25:45 2017 -0800

    Get tests to build and run against NS2.0 package

[33mcommit ac4ab235e552ecd2be06d9c9e9b622eff60cfc52[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Tue Apr 18 21:15:32 2017 -0700

    Reversion WCF libs post 1.1
    
    Assembly versions must be incremented past what was shipped in 1.1.
    
    In doing so I noticed redundant definition of AssemblyVersions in
    the ref projects which I have cleaned up, as well as an unused
    depproj which I've also cleaned up.

[33mcommit a5c3bb5c5180d7198a5c1df1cf95cbd13fd79c96[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Tue Apr 18 20:44:11 2017 -0700

    Update WCF baseline to 1.1 packages
    
    This updates the package baseline to use the packages shipped in the
    .NET Core 1.1 release.  These packages will be used when harvesting
    assets and determining compatibility.

[33mcommit 4d99c7f136b1d87d02b2207205096e162fdb0083[m
Merge: 378dcc4 cbf94da
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Apr 18 17:28:46 2017 -0700

    Merge pull request #1856 from ericstj/packageUpdates
    
    Package updates

[33mcommit cbf94da72ea212d31d969651b995d8c8292ede59[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Tue Apr 18 16:21:04 2017 -0700

    Update to latest CoreFx package baseline

[33mcommit 4d9400f84b7cb702770a23b9694650f4bbc915a3[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Tue Apr 18 16:16:25 2017 -0700

    Revert packageIndex update
    
    Revert packageIndex portion of 433d20816291495ffaa10cc8503f9400952a787f

[33mcommit c60f05411443786fae6f0a5f8c33b7901aa16189[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Tue Apr 18 16:12:05 2017 -0700

    Update packages to build on latest
    
    The latest buildtools requires that packages declare the assets they
    intend to harvest.
    
    It also runs compatibility checks on all netstandard refs.  We need to
    supress errors that occur because some WCF packages do not have matching
    lib for ref.

[33mcommit bb349295acba0a2a36794a6b21c63ba67a0991ad[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Tue Apr 18 13:43:30 2017 -0700

    Update BuildTools 1.0.27-prerelease-01518-03

[33mcommit 378dcc45a6a47f1fee25836ed78f00e7d938ea0e[m
Merge: f3173b9 3477ecf
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Apr 18 08:38:25 2017 -0700

    Merge pull request #1853 from StephenBonikowsky/UpdateWcfPkgVersionTo_25217
    
    Update wcf package dependency to 25217

[33mcommit 3477ecf248a403ab657d043b57124ef461b17a26[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Apr 17 11:04:37 2017 -0700

    Update wcf package dependency to 25217
    
    * This should fix the test issue from PR #1850

[33mcommit f3173b9655c4cb3bd7c2e94ddbd13b2ed5fa75cd[m
Author: KKhurin <kkhurin@microsoft.com>
Date:   Thu Apr 13 11:38:48 2017 -0700

    update stress streaming test to capture both in and out stream for StreamEcho scenario (#1849)

[33mcommit b01a9afdda3135d0e78c1c216f6fcc643b9aedf7[m
Author: KKhurin <kkhurin@microsoft.com>
Date:   Wed Apr 12 21:30:14 2017 -0700

    Adding a doc on how to use WCF .NET Core in Windows and Linux containers (#1820)
    
    * Adding a document on how to use WCF .NET Core in Windows and Linux containers
    
    addressing feedback comments
    
    * A quick start on WCF Core client in containers
    Updated with latest comments and feedback.

[33mcommit c349742bb78acb2e8e06abad92435a2ac41c9d3b[m
Author: KKhurin <kkhurin@microsoft.com>
Date:   Tue Apr 11 13:07:07 2017 -0700

    Adding Session-related methods and types to (#1850)
    
    * Adding Session-related methods and types to System.ServiceModel.Primitives contract
    
    * Updating unit test to prevent failures due to localization

[33mcommit bd19a4bda216792308defebab614307664b9cb1f[m
Author: KKhurin <kkhurin@microsoft.com>
Date:   Mon Apr 3 16:21:06 2017 -0700

    Adding a new Rolling Stress mode (#1835)
    
    * Adding a new Rolling Stress mode
    and fixing minor bugs
    
    * Adding a parameter for rolling stress
    and fixing minor bugs

[33mcommit 0da3dd7bdff5cab0b1874b57394ca7ebebecc831[m
Merge: 99b7d75 423492f
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Apr 3 15:29:34 2017 -0700

    Merge pull request #1838 from StephenBonikowsky/AddContractDescriptionUnitTest
    
    Adding unit test for ContractDescription.GetContract

[33mcommit 99b7d75f084babdcef9deee7bc92b41d20993a36[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Thu Mar 30 15:34:39 2017 -0700

    Code cleanup - Removing unused SecurityAlgorithms

[33mcommit 423492f3b820bb5508a4f1dc5db54228211ce39e[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Mar 29 16:04:50 2017 -0700

    Adding unit test for ContractDescription.GetContract
    
    * PR #1836 exposed a method in the ContractDescription class, this commit adds a simple test to validate this method.
    * We already have extensive tests that validate the rest of the ContractDescription class.

[33mcommit 81eb35f153d6e547ba82a19964d867a4ca93daa8[m
Merge: c096642 9d552fa
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Mar 31 17:52:52 2017 -0700

    Merge pull request #1844 from StephenBonikowsky/UpdatePkgVersion_25201-02
    
    Update to latest WCF packages.

[33mcommit 9d552fab12a0090eccd2c00b27bfa7983d82ad11[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Mar 31 17:13:34 2017 -0700

    Update to latest WCF packages.
    
    * These are teh packages just built after the big merge of PR #1840

[33mcommit c096642e32922ed4510e5d989db0d8816942d3da[m
Merge: 3e7f19f 5ea240a
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Mar 31 16:31:14 2017 -0700

    Merge pull request #1840 from MattGal/Update_BuildTools_Version
    
    Update build tools and CLI version:

[33mcommit 5ea240aba09775ea76f67ad66a7a77aebcf8d79d[m
Author: Matt Galbraith <mattgal@microsoft.com>
Date:   Fri Mar 31 16:00:06 2017 -0700

    Work around dumpling issue

[33mcommit 46cc8ffb13406e3a1fad03e07a4c136f1f622f44[m
Author: Matt Galbraith <mattgal@microsoft.com>
Date:   Fri Mar 31 12:55:07 2017 -0700

    Workaround for missing Target, which should not be necessary when building test projects.

[33mcommit 1dde56a35f31f4496d20f7ea056f5fee46c4e855[m
Author: Matt Galbraith <mattgal@microsoft.com>
Date:   Thu Mar 30 17:39:07 2017 -0700

    Take PreventImplementationReference back out, unintentionally included.

[33mcommit d6782620f1f0fa6e4bbaf8563bbdba0fde59af5b[m
Author: Matt Galbraith <mattgal@microsoft.com>
Date:   Thu Mar 30 17:36:33 2017 -0700

    Provide values for CSharpCoreTargetsPath, VisualBasicCoreTargetsPath

[33mcommit ae5b632871646ca49b547b2c272c9f492cfa5b28[m
Author: Matt Galbraith <mattgal@microsoft.com>
Date:   Thu Mar 30 15:04:03 2017 -0700

    fix init-tools for *nix

[33mcommit 433d20816291495ffaa10cc8503f9400952a787f[m
Author: Matt Galbraith <mattgal@microsoft.com>
Date:   Wed Mar 29 15:13:53 2017 -0700

    Update build tools and CLI version:
     - Update some paths to Net46 for BuildTools update
     - Workaround lack of updated Microsoft.Private.PackageBaseline.  Created https://github.com/dotnet/corefx/issues/17675 to track this; once that's addressed we can roll this back and simply update the dependency in pkg\baseline\project.json
     - Add suppressions for reved inbox assms

[33mcommit 3e7f19fa962a5326f121e52d2a7707b70585e837[m
Merge: 06d5c56 e88b525
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Mar 29 16:02:04 2017 -0700

    Merge pull request #1836 from StephenBonikowsky/AddApiContractDescription_GetContract
    
    Add ContractDescription.GetContract method to public contract.

[33mcommit e88b52577d51ec5acd01c54feb8fd991a4b900d6[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Mar 29 14:14:10 2017 -0700

    Add ContractDescription.GetContract method to public contract.
    
    * It looks to be already fully implemented. Testing will be needed to verify functionality.

[33mcommit 06d5c56f48249f0038087824d714ba465a037d72[m
Merge: 8c52495 9dd0cc1
Author: KKhurin <kkhurin@microsoft.com>
Date:   Tue Mar 28 18:58:29 2017 -0700

    Merge pull request #1831 from imcarolwang/LogStreamPositionInStressTest
    
    logging stream position for streaming test

[33mcommit 8c5249564665984c6310305ea609388c904e0a59[m
Merge: 8b9d600 99c5e84
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Mar 28 13:57:42 2017 -0700

    Merge pull request #1833 from StephenBonikowsky/AddSecurityNegotiationExceptionTest
    
    Adding a unit test for the newly public SecurityNegotiationException API

[33mcommit 99c5e84733f825efc30b8180577d4250dd853d92[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Mar 28 10:30:09 2017 -0700

    Adding a unit test for the newly public SecurityNegotiationException API

[33mcommit 8b9d600f5e20fc46c2faacdc784cb139c5f1cf72[m
Merge: 1f343b2 78db0c9
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Mar 28 10:29:01 2017 -0700

    Merge pull request #1830 from StephenBonikowsky/AddApiSecurityNegotiationException
    
    Adding SecurityNegotiationException to the public contract.

[33mcommit 9dd0cc18fb168f852865d5cafb102254bc4fa7a7[m
Author: Carol Wang (Inspur Worldwide Services Ltd) <v-carwan@microsoft.com>
Date:   Tue Mar 28 03:25:34 2017 -0700

    logging stream position in stream details when reporting timeout in stress streaming test

[33mcommit 1f343b2d2b3d3f17221215d936eb86381482a9d8[m
Author: Karel Zikmund <karelz@users.noreply.github.com>
Date:   Mon Mar 27 21:48:49 2017 -0700

    Update issue-guide.md

[33mcommit 78db0c9ce2e6df7f00881012d180fba5c0a417b3[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Mar 27 15:59:42 2017 -0700

    Adding SecurityNegotiationException to the public contract.
    
    * It is already fully implemented, just needed to be added to the public contract.

[33mcommit 70b52d161a5881a6bbc10ec3adb1af2fbbbde59e[m
Author: Olof Olsson <borsna@users.noreply.github.com>
Date:   Mon Mar 27 23:00:04 2017 +0200

    change link to new label for up-for-grabs (#1827)
    
    change link to label up-for-grabs

[33mcommit e4084599eb595c8723c3517a1072136ab9a9a2dd[m
Merge: 048db5a fa831d2
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Mar 27 11:51:52 2017 -0700

    Merge pull request #1819 from StephenBonikowsky/AddUnderstoodHeadersScenarioTest
    
    Add scenario test for UnderstoodHeaders API

[33mcommit 048db5a0e2a1e9b5ad830aecad3a8ae605c53b3f[m
Merge: d323939 9f44648
Author: KKhurin <kkhurin@microsoft.com>
Date:   Thu Mar 23 17:33:27 2017 -0700

    Merge pull request #1814 from KKhurin/FixForTimeoutTracking
    
    Fixed timeout tracking for the tests that make multiple requests

[33mcommit fa831d213f519cce95dde5b429b7fbd4ee62260b[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Mar 23 16:21:37 2017 -0700

    Add scenario test for UnderstoodHeaders API
    
    * UnderstoodHeaders API was recently added to the public contract, was missing scenario tests for it.

[33mcommit 9f44648c4284954150cb67f5bdb4840e2ce3c3a3[m
Author: Konst Khurin <kkhurin@microsoft.com>
Date:   Mon Mar 20 20:36:50 2017 -0700

    Fixed timeout tracking for the tests that make multiple service requests per test call.
    Added more logging and tracing for timeout tracking.
    Simplified tests and minor code cleanup.

[33mcommit d323939611592ef619597859ab260930b34821ff[m
Author: Rich Lander <rlander@microsoft.com>
Date:   Mon Mar 13 13:36:38 2017 -0700

    Add 3PN entries (#1805)
    
    Add 3PN entries

[33mcommit 71f919b520353f9cd60808b7e04175feb1a6712f[m
Merge: c8a7d55 f53ad7c
Author: KKhurin <kkhurin@microsoft.com>
Date:   Fri Mar 3 12:26:35 2017 -0800

    Merge pull request #1788 from KKhurin/FixFor_1760_TimeoutTracking
    
    Fixing #1760 (timeout tracking)

[33mcommit c8a7d554875fb65f1bee27e7721bc7d98b5bab4c[m
Merge: bf024e6 f4e5061
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Mar 2 10:45:35 2017 -0800

    Merge pull request #1786 from StephenBonikowsky/AddS.SM.ExtensionCollection
    
    Add S.SM.ExtensionCollection to public contract.

[33mcommit f53ad7caa4fd3802a26f8752bdf1ab7200b081cf[m
Author: Konst Khurin <kkhurin@microsoft.com>
Date:   Tue Feb 28 15:19:18 2017 -0800

    Fixing #1760 (timeout tracking)
    This will allow us to detect stuck requests during stress (we saw signs of it in previous stress runs)

[33mcommit f4e5061befef1cab12fd03fa3270a1b63eb70e40[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Feb 27 14:06:55 2017 -0800

    Add S.SM.ExtensionCollection to public contract.
    
    * Including a basic unit test.
    * No scenario tests needed as it is already used internally by WCF source code.
    * Fixed the way this collection was being referenced from the InstanceContext class.

[33mcommit bf024e66b32a21f2a078c04434a3bf63b17bb44c[m
Merge: 344d5ba 1f0552f
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Feb 27 15:09:51 2017 -0800

    Merge pull request #1782 from StephenBonikowsky/AddS.SM.D.XmlSerializerOperationBehavior
    
    Adding S.SM.Description.XmlSerializerOperationBehavior to public cont…

[33mcommit 1f0552fb43ae589f244856fe42cf3cc44854f7b2[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Feb 17 09:30:22 2017 -0800

    Adding S.SM.Description.XmlSerializerOperationBehavior to public contract.
    
    * Also had to add KeyedByTypeCollection class.
    * Also had to add the Behaviors property in the OperationDescription class.
    * Including one unit test.

[33mcommit 344d5bad7cd1921b0463e9069fef7226dcbe1b6d[m
Merge: f03b5b4 1e6246c
Author: Zhenlan Wang <zhenlwa@microsoft.com>
Date:   Thu Feb 16 18:15:08 2017 -0800

    Merge pull request #1778 from morganbr/ResourceCleanup
    
    Resource string cleanup

[33mcommit 1e6246c8b972c4a6cabbe86c7e57ff68439987f5[m
Author: Morgan Brown <morganb@microsoft.com>
Date:   Tue Feb 14 18:34:48 2017 -0800

    Automated cleanup of unused resources in System.Private.ServiceModel. This removes about 75% of the file. The automated tool scanned for source references to resource strings. Verification was build+unit tests.

[33mcommit f03b5b46ae3d9c5281180155c08f53ceb08b4984[m
Merge: 692e3d2 348da28
Author: KKhurin <kkhurin@microsoft.com>
Date:   Thu Feb 9 12:50:31 2017 -0800

    Merge pull request #1770 from KKhurin/fix_for_1693
    
    Wrapping timeout callbacks to guard against unhandled exceptions …

[33mcommit 348da28b20acd9c189619927333f409d866b27e2[m
Author: Konst Khurin <kkhurin@microsoft.com>
Date:   Mon Feb 6 16:23:28 2017 -0800

    Wrapping timeout callbacks to guard against unhandled exceptions on timer threads (#1693)

[33mcommit 692e3d29e8c176c55748438f25a7cb14981368c4[m
Merge: 01dba50 9324476
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Feb 2 17:25:51 2017 -0800

    Merge pull request #1767 from StephenBonikowsky/AddUnderstoodHeadersAPI
    
    Add UnderstoodHeaders API and unit test.

[33mcommit 93244760b75c18c75e4917a7a865838e4c792f57[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Feb 2 11:32:56 2017 -0800

    Add UnderstoodHeaders API and unit test.
    
    * Add UnderstoodHeaders to the list of public APIs, implementation already exists.
    * Add a unit test to validate the class.

[33mcommit 01dba50e05b805cd2a2814e68a2b8919374ec513[m
Merge: 1170272 6c9abee
Author: KKhurin <kkhurin@microsoft.com>
Date:   Tue Jan 31 19:16:19 2017 -0800

    Merge pull request #1763 from KKhurin/UpdatingStressTests
    
    Updating stress tests with the latest version.

[33mcommit 6c9abeebecc71bbe5f0890b3f191b57d0d18260f[m
Author: Konst Khurin <kkhurin@microsoft.com>
Date:   Mon Jan 30 13:45:06 2017 -0800

    Updating stress tests with the latest version.
    The changes include:
    - adding parameters to control timeouts
    - adding a parameter to control channel pool size
    - fixing error handling
    - adding a missing instruction to readme

[33mcommit 11702724e44070c7a173b3c1bf8c8736d961ebf8[m
Merge: 30643d7 c4ec2f6
Author: KKhurin <kkhurin@microsoft.com>
Date:   Tue Jan 24 17:23:33 2017 -0800

    Merge pull request #1752 from KKhurin/FixFor1504
    
    Fixing #1504 leaking connections in nettcp streaming.

[33mcommit c4ec2f664fa5eb4fe35fb3d49c00470c95e0c6c6[m
Author: Konst Khurin <kkhurin@microsoft.com>
Date:   Mon Jan 23 19:56:43 2017 -0800

    Fixing leaking connections in nettcp streaming.
    We are adding a missing call to ClientSingletonConnectionReader.DoneSending to allow for correct connection usage tracking.
    This will let the connection to be properly returned to the pool or closed when not used.

[33mcommit 30643d7b7aaf33600a6997375842a116c5cb1926[m
Merge: 37abdba bb7e3f7
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Jan 24 15:12:54 2017 -0800

    Merge pull request #1751 from StephenBonikowsky/RemoveChannelPoolSettings
    
    Removing ChannelPoolSettings

[33mcommit bb7e3f7c2095739b299ca5d2ff24a029bf9ab41e[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Jan 23 15:27:03 2017 -0800

    Removing ChannelPoolSettings
    
    * Removing this class because it is not used anywhere else in WCF and the scenario for it's use by an app is an edge case that would first involve writing a custom channel pool.
    * On full desktop there is some server side WF code that uses it, it is not supported in net core.

[33mcommit 37abdbac9466dfa96c671c13c5306e7a7454426a[m
Merge: 722d75c 825c0a9
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Jan 12 10:16:23 2017 -0800

    Merge pull request #1744 from StephenBonikowsky/AddActiveIssue
    
    Adding ActiveIssue to ServiceRestart_Throws_CommunicationException test

[33mcommit 825c0a916efc367c6285ed4a26591862729484cd[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Jan 11 16:07:19 2017 -0800

    Adding ActiveIssue to ServiceRestart_Throws_CommunicationException test case
    
    * This test failure is due to an underlying problem in WinRTHttpClient behavior.
    * Until that gets fixed we can skip this test on netnative runs.
    * Amended this commit to also add an ActiveIssue for test case 'Message_With_MessageHeaders_RoundTrips'.

[33mcommit 722d75c62a0dbce8e716f5ff3d40905544915025[m
Merge: befdfc5 e98eff6
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Jan 11 09:16:37 2017 -0800

    Merge pull request #1738 from dotnet-bot/master-UpdateDependencies
    
    Update CoreClr, CoreFx, WCF to beta-24911-02, beta-24911-02, beta-24911-02, respectively (master)

[33mcommit e98eff63f90cc079e1670ac7bf0feb2ac5b10f96[m
Author: dotnet-bot <dotnet-bot@microsoft.com>
Date:   Wed Jan 11 15:55:11 2017 +0000

    Update CoreClr, CoreFx, WCF to beta-24911-02, beta-24911-02, beta-24911-02, respectively

[33mcommit befdfc5e7e5fc8f435957b2ace8fe090ff4a7995[m
Merge: 673c68b 8462bbd
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Jan 10 16:06:57 2017 -0800

    Merge pull request #1742 from StephenBonikowsky/UpdateTestRuntime
    
    Updating infrastructure dependencies.

[33mcommit 8462bbd0c7530294703bb0463d74d8a72519ac7a[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Jan 10 15:26:51 2017 -0800

    Updating infrastructure dependencies.
    
    * Several dependent packages were out-of-date as compared to corefx.
    * This may resolve some F5 infrastructure failures.

[33mcommit 673c68bba799e040b6a01b5f642ecfefcf4201b3[m
Merge: 682af6c 4f1df19
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Jan 10 09:06:04 2017 -0800

    Merge pull request #1737 from dotnet-bot/master-UpdateDependencies
    
    Update CoreClr, CoreFx, WCF to beta-24910-02, beta-24910-02, beta-24910-01, respectively (master)

[33mcommit 4f1df19661be18214f70ddf571b644231b2bb01e[m
Author: dotnet-bot <dotnet-bot@microsoft.com>
Date:   Tue Jan 10 15:36:02 2017 +0000

    Update CoreClr, CoreFx, WCF to beta-24910-02, beta-24910-02, beta-24910-01, respectively

[33mcommit 682af6c183dd9c6d7109bbc2b897582915f087a6[m
Author: Shin Mao <shmao@microsoft.com>
Date:   Mon Jan 9 15:32:44 2017 -0800

    Fix bug with sending Message with XmlElement MessageHeader (#1735)
    
    XmlElementMessageHeader.OnWriteHeaderAttributes threw PNSE because one required API was not available on NetCore. Adding the implementation for the method as the missing API is available now.
    
    Fix #702.

[33mcommit 9da5843240fc09169340e282620e1ce4a7cb32a1[m
Merge: 8a6830b f207d93
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Jan 9 13:41:04 2017 -0800

    Merge pull request #1734 from StephenBonikowsky/CleanupInternalSR
    
    Remove internal static strings in InternalSR.cs

[33mcommit f207d9386f7e6b73531f854da24f048480f7fb88[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Jan 9 11:48:30 2017 -0800

    Remove internal static strings in InternalSR.cs
    
    * These static strings are not used anywhere in our code base.

[33mcommit 8a6830bd59699fdb25cedb11c1d44bae024de47b[m
Author: Shin Mao <shmao@microsoft.com>
Date:   Mon Jan 9 11:43:06 2017 -0800

    Update WCF to Take Dependency on NetStandard2.0 (#1727)
    
    * Update WCF to Take Dependency on NetStandard2.0

[33mcommit ac980bbe1a96796ff17b6a3dcc2fc1e5dc3209df[m
Merge: 496a231 ebbcd04
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Jan 6 16:03:03 2017 -0800

    Merge pull request #1728 from StephenBonikowsky/TestingNetNative
    
    Adding a test to validate certain behavior on NetNative.

[33mcommit ebbcd04ae0141185b3d7b6438efb1ac2408e40e9[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Jan 6 09:56:17 2017 -0800

    Modifying a test to make it more robust.
    
    * Test: Scenarios\Client\ExpectedExceptions\ExpectedExceptionTests.4.1.0.cs - ServiceRestart_Throws_CommunicationException()
    * This test is currently failing on NetNative, this update should help us understand why.

[33mcommit 496a231babf0a13b2d0e37f2cab8d692fbb3a710[m
Merge: 55bd40c 42c9d6a
Author: Huangli Wu <huanwu@microsoft.com>
Date:   Thu Jan 5 11:25:17 2017 -0800

    Merge pull request #1724 from huanwu/DisableDataContractResolverTest
    
    Disable the DataContractResolver test in UWP.

[33mcommit 42c9d6ae61e6597d675f8a7df05b88a412e5f250[m
Author: huanwu <huanwu@microsoft.com>
Date:   Wed Jan 4 17:45:00 2017 -0800

    Disable the DataContractResolver test in UWP. This scenario currently doesn't work in UWP since the DataContractResolver doesn't work in UWP, which is a serialization bug.

[33mcommit 55bd40c42b2b7962500ddef6fc524e9b64a3a8c2[m
Merge: a25dc6f b3bb6a4
Author: KKhurin <kkhurin@microsoft.com>
Date:   Tue Jan 3 17:37:16 2017 -0800

    Merge pull request #1715 from KKhurin/wcf_issue_1695_1
    
    Adding message for AsyncResultCompletedTwice error for #1695

[33mcommit a25dc6f6080af974a60a963a34ae20f270b9c583[m
Merge: f3c9632 3270a1e
Author: KKhurin <kkhurin@microsoft.com>
Date:   Tue Jan 3 17:35:29 2017 -0800

    Merge pull request #1716 from KKhurin/StressReadMe
    
    #918: adding stress and perf readme.md

[33mcommit 3270a1e2c8a129ffb82a84d39322fbeb4ef921fe[m
Author: Konst Khurin <kkhurin@microsoft.com>
Date:   Wed Dec 28 21:41:33 2016 -0800

    adding readme.md for stress and perf tests

[33mcommit f3c9632627185d7c46d5f043121bf025b075887d[m
Merge: a91fb5e e3f3a72
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Jan 3 10:28:10 2017 -0800

    Merge pull request #1713 from dotnet-bot/master-UpdateDependencies
    
    Update CoreClr, CoreFx, WCF to beta-24903-03, beta-24903-02, beta-24903-01, respectively (master)

[33mcommit e3f3a72bea486b472cf2b420f685524ba2b1f807[m
Author: dotnet-bot <dotnet-bot@microsoft.com>
Date:   Tue Jan 3 17:37:26 2017 +0000

    Update CoreClr, CoreFx, WCF to beta-24903-03, beta-24903-02, beta-24903-01, respectively

[33mcommit b3bb6a4408426221fd3aae3ac6d64aed988cecb5[m
Author: Konst Khurin <kkhurin@microsoft.com>
Date:   Wed Dec 28 19:42:03 2016 -0800

    Adding message for AsyncResultCompletedTwice error

[33mcommit a91fb5e07e02ac530947ab3919cdd83e22dfb555[m
Merge: 58c43f6 fcdeaf6
Author: Zhenlan Wang <zhenlwa@microsoft.com>
Date:   Fri Dec 16 10:56:05 2016 -0800

    Merge pull request #1707 from dotnet/linkfix
    
    Fix the link to .NET Core 1.1 release notes.

[33mcommit 58c43f61d819d70beb8e0b9c38883dce5723077a[m
Merge: 82f2297 a8e7b0d
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Dec 14 14:26:01 2016 -0800

    Merge pull request #1705 from StephenBonikowsky/Fix_ServiceRestart_Test
    
    Fixing and updating test to use best practices.

[33mcommit fcdeaf68036efe5b08f5022e07434066896c7178[m
Author: Zhenlan Wang <zhenlwa@microsoft.com>
Date:   Wed Dec 14 10:49:22 2016 -0800

    Fix the link to .NET Core 1.1 release notes.
    
    skip CI please.

[33mcommit a8e7b0d1f06be248a235bcfa5b949310d85e4170[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Dec 13 16:09:54 2016 -0800

    Fixing and updating test to use best practices.
    
    * Removed usage of StringBuilder.
    * Added comments to better explain what the test does.

[33mcommit 82f2297392cf6e759fbe881631be1eb72f8b8787[m
Merge: 3bbd2c9 851d478
Author: Huangli Wu <huanwu@microsoft.com>
Date:   Mon Dec 12 12:35:38 2016 -0800

    Merge pull request #1700 from huanwu/DataContractResolver2
    
    Remove the stringbuilder and do some cleanup in DataContractResolverTest

[33mcommit 851d47843223a812eee1d7804a300da5388cf455[m
Author: huanwu <huanwu@microsoft.com>
Date:   Mon Dec 12 11:35:50 2016 -0800

    Remove the stringbuilder. Replace the "catch" with a Finally only to do some channel cleanup.

[33mcommit 3bbd2c91ad0489c6540eaed5d33537c765b79e96[m
Merge: 9cff0c8 0268bf3
Author: Hong Dai <hongdai@microsoft.com>
Date:   Mon Dec 12 10:27:20 2016 -0800

    Merge pull request #1696 from huanwu/DataContractResolver
    
    Fix the DataContractResolver issue and add a test for it.

[33mcommit 0268bf332d8f58bf0e4b11a7d4bb27a846aec030[m
Author: huanwu <huanwu@microsoft.com>
Date:   Fri Dec 9 15:47:32 2016 -0800

    Remove the operation name condition check.

[33mcommit 9d4cb77c43b393f4b7048f65efb08c2f39931bb1[m
Author: huanwu <huanwu@microsoft.com>
Date:   Fri Dec 9 15:43:27 2016 -0800

    Remove the change

[33mcommit 54fdd53b5b3ab72734ae75ef086fde12382e9a40[m
Author: huanwu <huanwu@microsoft.com>
Date:   Fri Dec 9 15:30:46 2016 -0800

    Some changes based on the feedback.

[33mcommit 78540ffa11d51469fc576e9dec7a222abc5dfcdd[m
Author: huanwu <huanwu@microsoft.com>
Date:   Thu Dec 8 16:27:24 2016 -0800

    Fix the DataContractResolver issue and add a test for it.

[33mcommit 9cff0c81e535ac177f60ab25dff904a3144d62b3[m
Merge: c9535eb 6c4570e
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Dec 7 09:53:32 2016 -0800

    Merge pull request #1692 from StephenBonikowsky/UpdateSecurityTests
    
    Updating scenario security tests.

[33mcommit 6c4570e1fcb32f1df05feac0690f3abd97791f59[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Dec 6 15:55:45 2016 -0800

    Updating scenario security tests.
    
    *Remove StringBuilder usage.
    *Add Asserts for validation.

[33mcommit c9535ebba1b6da7929b48e7a119acfdc4810b1e6[m
Merge: 3f2c90b caa808d
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Dec 6 10:06:55 2016 -0800

    Merge pull request #1691 from StephenBonikowsky/UpdateEncodingTests
    
    Updating scenario encoding tests.

[33mcommit caa808d0a75f181962f4253bf97e680af597fb59[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Dec 6 09:28:57 2016 -0800

    Updating scenario encoding tests.
    
    * Remove StringBuilder usage.
    
    * Add Asserts for validation.

[33mcommit 3f2c90ba8f44b821a0e972ca9cd45dc92e939957[m
Merge: a841520 2740421
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Dec 6 09:09:37 2016 -0800

    Merge pull request #1690 from StephenBonikowsky/UpdateContractTests
    
    Update scenario contract tests.

[33mcommit 27404212f3aad6a426c071463a75ef1e2be4290b[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Dec 5 15:13:08 2016 -0800

    Update scenario contract tests.
    
    * Remove StringBuilder
    * Add Asserts for validation.

[33mcommit a841520e7688d740414f91a4710e74bce40b0326[m
Merge: 21fa402 b8c1ff6
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Nov 30 13:34:02 2016 -0800

    Merge pull request #1685 from roncain/channelbase
    
    Add tests for ChannelBase<T>

[33mcommit b8c1ff64dae6b9a8e625825c3890ebb157adb561[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Nov 30 13:10:35 2016 -0800

    Improve test comments

[33mcommit 21fa40236c3e42362f03a91646e3b513742cb200[m
Merge: 98a3f73 5195180
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Nov 30 09:57:37 2016 -0800

    Merge pull request #1683 from StephenBonikowsky/UpdateTypedClientTests
    
    Updating scenario TypeClientTests

[33mcommit cf9b18c26872991bfb05e35bac86b873def89e3f[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Nov 29 12:01:51 2016 -0800

    Add tests for ChannelBase<T>
    
    This class is part of ClientBase<T> and provides a mechanism
    for custom channels that do not use dynamic proxy generation.

[33mcommit 98a3f732126ed454d56c2527da8f3131d5b62583[m
Merge: e5c0699 bfa58d3
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Nov 30 06:27:51 2016 -0800

    Merge pull request #1682 from roncain/update-uwp-docs
    
    Update docs to explain how to run tests as UWP

[33mcommit 51951805258c76638e7fc4b16e8a0570ad83e1ee[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Nov 29 13:47:05 2016 -0800

    Updating scenario TypeClientTests
    
    * Updated sln with a ref to common.infrastructure to fix intellisense in VS.
    * Removed StringBuilder wherever used and replaced with xunit Asserts.
    * Added setup/execute/validate/cleanup comments.
    * Instantiated most variables at the top of the test case and set to appropriate values in the setup section.

[33mcommit bfa58d39413b3363fb15c866e66def9c9ee5e11e[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Nov 29 09:27:35 2016 -0800

    Update docs to explain how to run tests as UWP
    
    Due to changes in the build tools, the instructions for how to
    run unit and scenario tests as UWP changed slightly.
    
    Fixes #1670

[33mcommit e5c0699ddb1490e7ca7714b220ec647668749b75[m
Merge: 97afc64 efbdf07
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Nov 28 13:09:58 2016 -0800

    Merge pull request #1677 from StephenBonikowsky/UpdateAssertsInTests
    
    Updating ExpectedException tests.

[33mcommit efbdf071b12e1d2c37e3e567b4f8f2b7e629b8a1[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Nov 22 17:52:21 2016 -0800

    Updating ExpectedException tests.
    
    * Updating with comments.
    * Removing StringBuilder wherever used.
    * Use Asserts correctly.

[33mcommit 97afc64cd74d665aed069ce7da2d257dff18abe1[m
Merge: ecc23af 0ba6913
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Nov 22 08:49:03 2016 -0800

    Merge pull request #1672 from StephenBonikowsky/FixUWPF5Runs
    
    Fix UWP F5 runs.

[33mcommit 0ba6913fab4e9c7c5e68bbd06bff7c928d6bd261[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Nov 21 13:46:51 2016 -0800

    Fix UWP F5 runs.
    
    * Need to add AppXRunnerVersion to dependencies.props file.

[33mcommit ecc23af323c9cbd9321da83da6dbfc75a2fe61c3[m
Merge: f73dbd1 cdd4910
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Nov 18 13:21:41 2016 -0800

    Merge pull request #1662 from roncain/xplat-docs
    
    Update instructions on running tests cross platform

[33mcommit f73dbd17a9ebf00d74857e286ec2fe93b8d26722[m
Merge: fc12776 2d7a1e9
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Nov 18 13:21:10 2016 -0800

    Merge pull request #1666 from roncain/clientbase-tests
    
    Improve test coverage of ClientBase

[33mcommit fc127762b9a86349f3b01953de4efa01c228f377[m
Merge: 190130a 27c5e63
Author: KKhurin <kkhurin@microsoft.com>
Date:   Thu Nov 17 12:20:43 2016 -0800

    Merge pull request #1663 from KKhurin/WcfStressTests_1_1
    
    WCF 1.1 Stress Tests

[33mcommit 2d7a1e9c31810f96368a083fce14dd3fd3805a4a[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Nov 16 11:41:28 2016 -0800

    Improve test coverage of ClientBase

[33mcommit 27c5e63f1548121a35e2d51ac048a0bca4d783de[m
Author: Konst Khurin <kkhurin@microsoft.com>
Date:   Wed Nov 16 20:56:52 2016 -0800

    Added various security bindings
    Refactored tests to support more stress scenarios
    Added numerous command line parameters to control both perf and stress tests
    Added basic stress results reporting
    Tuned perf scenario tests to achieve stable results
    Addressed some of the previous PR feedback

[33mcommit 190130a837819cec00e965fc50144c184335b8aa[m
Merge: 5225a57 08aaff7
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Nov 16 11:31:05 2016 -0800

    Merge pull request #1660 from StephenBonikowsky/FixUwpRuns
    
    Fix a break to UWP runs.

[33mcommit cdd4910e7a536aa7eb5469207ff417de1e45321a[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Nov 16 08:38:52 2016 -0800

    Update instructions on running tests cross platform
    
    Fixes #1639

[33mcommit 5225a57f5b69dc6bd58420c2517641b1e6f99094[m
Merge: fdd4dbf 1557e58
Author: Zhenlan Wang <zhenlwa@microsoft.com>
Date:   Wed Nov 16 08:28:27 2016 -0800

    Merge pull request #1661 from zhenlan/pkg1.1doc
    
    Update docs of package versions for 1.1 release

[33mcommit 1557e58af9526cf82651c140a3774c1e2b725e93[m
Author: Zhenlan Wang <zhenlwa@microsoft.com>
Date:   Tue Nov 15 23:21:16 2016 -0800

    Update docs of package versions for 1.1 release

[33mcommit 08aaff77a12f81cfa5442a75fa015a65e73f7d86[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Nov 15 15:36:16 2016 -0800

    Fix a break to UWP runs.
    
    * Expected to fix an "exceeds Max_Path" error.

[33mcommit fdd4dbffac2c19b5f22cdfffc45f94ee4afacffd[m
Merge: aff11e9 ae65356
Author: Hong Dai <hongdai@microsoft.com>
Date:   Tue Nov 15 16:33:51 2016 -0800

    Merge pull request #1619 from hongdai/releasefeature
    
    Support feature table for v1.1.0

[33mcommit ae653569caf9681c8ad60e14ae053b5711684f2d[m
Author: hongdai <hongdai@microsoft.com>
Date:   Tue Nov 15 16:29:50 2016 -0800

    Support feature table for v1.1.0
    
    * Note that all links are not working yet as v1.1.0 tags etc are
    not there yet.
    * I will do a final update once all links are available
    
    Fix #1613

[33mcommit aff11e9c98fe8a666cc79da125e84d7e953ca01f[m
Merge: 3bca3d4 e99e7bf
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Nov 15 05:55:06 2016 -0800

    Merge pull request #1653 from roncain/package-docs
    
    Add documentation on using pre-release packages

[33mcommit 3bca3d433f34070a3084d2c8d024d7663df286eb[m
Merge: c1711a5 c63b847
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Nov 14 13:51:05 2016 -0800

    Merge pull request #1658 from hongdai/disabletests
    
    Disable tests on Windows 7

[33mcommit c63b8476195fe01fadca307e18c27322773c2e3e[m
Author: hongdai <hongdai@microsoft.com>
Date:   Mon Nov 14 08:40:33 2016 -0800

    Disable tests on Windows 7
    * The two newly added tests need to be disabled on Windows 7

[33mcommit c1711a5968eec3aeff2470025ca7e190fa3c28fe[m
Merge: 24a97e8 b4ff682
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Nov 10 08:41:15 2016 -0800

    Merge pull request #1654 from StephenBonikowsky/FixFlakyTest
    
    Fixing a flaky test.

[33mcommit b4ff682238e0f2379881f9e210ee3cf90bf3d10f[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Nov 9 15:03:46 2016 -0800

    Fixing a flaky test.
    
    * Test case "NetTcp_TransportSecurity_Streamed_TimeOut_Long_Running_Operation" is just testing the 'SendTimeout' property on the binding, to make sure it is working as expected we only pass the test when the elapsed time falls within a narrow range.
    * This test has occasionally been failing by taking longer than expected, we don't believe this is a product bug most likely caused because we were not explicitly opening the channel before starting the Stopwatch, by doing this we are remove this small but variable amount of time that was getting added to the elapsed time calculation.

[33mcommit 24a97e85b9bc12d8d4f45021f38eafc772ba1384[m
Merge: 973fb36 68ab81e
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Nov 9 13:10:36 2016 -0800

    Merge pull request #1652 from StephenBonikowsky/FixServiceModelPackageReferences
    
    Adding references to the ServiceModel facades in test project.json.

[33mcommit e99e7bf187270604b57d2a74e78a5a1b2977efd6[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Nov 9 10:23:51 2016 -0800

    Add documentation on using pre-release packages
    
    Fixes #1607

[33mcommit 68ab81e8cedfd280cc3a55b93728334d0a30441e[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Nov 9 11:15:20 2016 -0800

    Adding references to the ServiceModel facades in test project.json.
    
    * After the recent change that consolidated all test project.jsons into one, I had not included references to ServiceModel facades, this didn't cause any issues in CI or locally due to the package project references that exist in the various test projects. In VSO we build test projects using actual packages so missing the ServiceModel package references in the project.json caused the Common test projects to fail.

[33mcommit 973fb36f0fcf8ff370d0ec763529d06dfa0adfb5[m
Merge: c5fcd54 5446748
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Nov 9 09:44:25 2016 -0800

    Merge pull request #1651 from StephenBonikowsky/FixDuplexTests
    
    Fix Duplex Channel shaped tests using http.

[33mcommit c5fcd540edd7a24e91b8165b667a8b5b83220f43[m
Merge: 8c22c3f 61ca300
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Nov 9 07:00:18 2016 -0800

    Merge pull request #1648 from crummel/updateBuildtools
    
    Update buildtools to take fix for coverage issue.

[33mcommit 544674810cf895a245da5d892e3bdb81b48bd5f7[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Nov 8 16:44:53 2016 -0800

    Fix Duplex Channel shaped tests using http.
    
    * One test was using the wrong binding, both needed seperate endpoints on the server configured to receive incoming messages using websockets.

[33mcommit 61ca30017fa0768c5b4b3f5401875556eef54c33[m
Author: Chris R <crummel@microsoft.com>
Date:   Tue Nov 8 12:32:02 2016 -0600

    Update buildtools to take fix for coverage issue.

[33mcommit 8c22c3fd790533edd65d56198e4039f5fb2b008f[m
Merge: df5ed7a ff152a3
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Nov 8 09:13:57 2016 -0800

    Merge pull request #1647 from StephenBonikowsky/UseSingleTestProjectJson
    
    Combine all test dependencies into one common project.json

[33mcommit ff152a364b00831c88dff7e465c374c1e4aa338c[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Nov 7 14:32:53 2016 -0800

    Combine all test dependencies into one common project.json
    
    * This was done in CoreFx with PR #12345
    * Although not as impactful as for CoreFx this will shorten the time it takes to sync.
    * In addition, as a simplification improvement it makes a lot of sense.

[33mcommit df5ed7a7f2fe98a2fe350f0ae8f8020320715b8e[m
Merge: 5523974 c8d02f0
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Nov 4 16:20:39 2016 -0700

    Merge pull request #1642 from StephenBonikowsky/UpdateWcfVersion-beta-24704-02
    
    Updating the WCF package version to latest.

[33mcommit 55239741e8481983812ae65b836f939f4aa19e21[m
Merge: 49b722f 45d13eb
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Nov 4 16:18:25 2016 -0700

    Merge pull request #1640 from StephenBonikowsky/AddMessageInspectorTests
    
    Add message inspector tests.

[33mcommit 49b722f8f30e3439fbacd9a4903cbab4facf2070[m
Merge: 5eaa9fd aac853c
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Nov 4 15:56:45 2016 -0700

    Merge pull request #1641 from roncain/bridge-cleanup
    
    Rename test properties and members to remove "Bridge"

[33mcommit c8d02f04a1f2842a2674945ad6f9bd593ba92b85[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Nov 4 11:58:21 2016 -0700

    Updating the WCF package version to latest.
    
    * New APIs were just added, updating to the package version just built with these changes so Helix test projects can build.

[33mcommit 45d13ebb9eee41ce4ce7de2e1289f9e5f6f49a92[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Nov 2 16:00:02 2016 -0700

    Add message inspector tests.
    
    * From the old mahjong app code.

[33mcommit aac853c6ca309cfc1ee543faa7b72b031f8f230a[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Nov 4 09:45:20 2016 -0700

    Rename test properties and members to remove "Bridge"
    
    This is the 2nd of 2 steps to remove the old infrastructure.
    The first step deleted all old infrastructure code.
    
    This follow-up PR just renames variables, constants, member names,
    etc. to remove "Bridge" from any names.

[33mcommit 5eaa9fd61f1621b627a9120e6deae2dc2f95cc9b[m
Merge: c27e261 b23e1e4
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Nov 4 05:44:07 2016 -0700

    Merge pull request #1630 from roncain/remove-bridge
    
    Clean up outdated infrastructure assets

[33mcommit c27e26164b23f67e4682e6bb741d1dfedf5d26bb[m
Merge: 6835fab a803605
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Nov 4 05:26:52 2016 -0700

    Merge pull request #1616 from roncain/messageheader
    
    Add message attribute APIs

[33mcommit a8036050d92990693f3b986adcb141cd5463a01a[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Oct 24 08:39:24 2016 -0700

    Support MessageHeaderArrayAttribute and MessagePropertyAttribute
    
    Adds these types to public API.
    Adds implementation code to support them.
    Adds tests for them.
    
    Fixes #1410

[33mcommit 6835fab8bf20e1af2ffdf2d3c751740fe1f5a742[m
Merge: 2dfce75 3f932c4
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Nov 3 11:56:30 2016 -0700

    Merge pull request #1631 from dotnet/mmitche-patch-1
    
    http://dotnet-ci.cloudapp.net -> https://ci.dot.net

[33mcommit 3f932c4b542cec046fd8d40c833833c3859f0c18[m
Author: Matt Mitchell <mmitche@microsoft.com>
Date:   Thu Nov 3 09:07:42 2016 -0700

    http://dotnet-ci.cloudapp.net -> https://ci.dot.net

[33mcommit b23e1e48bf595fcd8a25371e096c55bf98a6ea83[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Nov 3 07:14:58 2016 -0700

    Clean up outdated infrastructure assets
    
    This PR removes the prior testing infrastructure
    projects, including the Bridge, the old version
    of the self-host, certificate installer, etc.
    
    This PR only removes code.  Later PR's will
    rename some of the test properties to remove
    references to "bridge".
    
    Fixes #1626

[33mcommit 2dfce752d2be35f388037829eb2ddc988c1878e9[m
Merge: 6550a57 1ba58a0
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Oct 28 11:22:23 2016 -0700

    Merge pull request #1621 from StephenBonikowsky/SupportNetCore50TestRuns
    
    Enabling tests to run against netcore50 test tfm

[33mcommit 1ba58a03c3466f4778773576c17467ba8bb2bb24[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Oct 27 14:24:09 2016 -0700

    Enabling tests to run against netcore50 test tfm
    
    * WCF currently supports two TestTFMs, they need to be explicitly listed for test projects now so that the 'FilterToTestTFM' property used in Helix runs works as expected. This "may" fix the netcore50 runs in Helix.

[33mcommit 6550a570e98104c2931e35553cc88b1dcb60def6[m
Merge: d09df74 6b9a1f9
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Oct 27 11:54:20 2016 -0700

    Merge pull request #1617 from StephenBonikowsky/MoveTestsToNetcoreapp1.1
    
    Updating all tests to run using netcoreapp1.1 test TFM.

[33mcommit 6b9a1f92ae9582baa66637e59dfacac536fb9842[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Oct 27 09:40:30 2016 -0700

    Updating all tests to run using netcoreapp1.1 test TFM.
    
    * We are going to have to eventually, next step is to move product packages to NetStandard1.7

[33mcommit d09df746b786981c5684502561e1785cf1ab23a3[m
Merge: 3ae48a1 a06df91
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Oct 26 15:19:55 2016 -0700

    Merge pull request #1615 from StephenBonikowsky/CoreFxSync_10_26
    
    Updating several recent changes to shared build files.

[33mcommit a06df91eb06f53b6504571abb644b68e4a5d5b0c[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Oct 26 13:53:48 2016 -0700

    Updating several recent changes to shared build files.
    
    * Latest build tools version plus other changes.

[33mcommit 3ae48a111b1dc3bb4de03406f271d229e799664e[m
Merge: 553248c b2b47ab
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Oct 26 13:54:51 2016 -0700

    Merge pull request #1614 from StephenBonikowsky/UpdateWcfVersionTo_4.4.0-beta-24626-02
    
    Updating the WCF packages being used to version 4.4.0-beta-24626-02

[33mcommit b2b47ab266782cecec75e17c028499dbc856b8c3[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Oct 26 11:46:10 2016 -0700

    Updating the WCF packages being used to version 4.4.0-beta-24626-02
    
    * This version includes the latest API changes that @roncain just added earlier today.
    * At this point Helix still does not use the "just built" packages so unless we update the project.json files we will fail all builds due to the new APIs being referenced by tests.

[33mcommit 553248cc11bcdbc630a8b9a5efc7e7b76dda1b77[m
Merge: f66c165 558bb6a
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Oct 26 07:10:13 2016 -0700

    Merge pull request #1602 from roncain/IInteractiveChannelInitializer
    
    Adds IInteractiveChannelInitializer to public API

[33mcommit 558bb6ad94ca9b3611a6b62684a20956a1640fc2[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Oct 26 06:25:27 2016 -0700

    Adds IInteractiveChannelInitializer to public API
    
    Also adds ClientRuntime.InteractiveInitializers to public API,
     which is the only way to register interactive initializers.
    
    New mocks and unit tests added to verify the interative
     initializer is called properly.

[33mcommit f66c165926ee4655fc24d2e133fd93eb07c94996[m
Merge: 0165305 ece4321
Author: Zhenlan Wang <zhenlwa@microsoft.com>
Date:   Tue Oct 25 14:37:33 2016 -0700

    Merge pull request #1605 from StephenBonikowsky/Issue470
    
    Remove active issue 470.

[33mcommit 01653051cf077e83666cd7ad03ed3e935705dfd8[m
Merge: 12d528c 2b0fb79
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Oct 25 12:27:41 2016 -0700

    Merge pull request #1604 from roncain/IChannelInitializer
    
    Add IChannelInitializer to public API

[33mcommit 12d528c40804a5f12f6bff6656a863807ca772fa[m
Merge: 7a56e3f 12f0fab
Author: Hong Dai <hongdai@microsoft.com>
Date:   Tue Oct 25 08:59:31 2016 -0700

    Merge pull request #1611 from hongdai/FixWebService
    
    Fix test services for addding service reference

[33mcommit 12f0fab4a4d59efe6b15fa446a286b070a387016[m
Author: hongdai <hongdai@microsoft.com>
Date:   Mon Oct 24 16:53:09 2016 -0700

    Fix test services for addding service reference
    
    * We would like to leverge the same test server for WCF connected service
    testing.

[33mcommit 7a56e3f9f87b4dbf09612bcc1a989cef1d1420f2[m
Merge: adcec31 2b5fdc9
Author: Zhenlan Wang <zhenlwa@microsoft.com>
Date:   Mon Oct 24 10:53:05 2016 -0700

    Merge pull request #1609 from zhenlan/1.1p1pkg
    
    Update docs of package versions for .NET Core 1.1 Preview 1 release

[33mcommit 2b5fdc9bd246390f9e3d56a2b6eb45b9eba8529f[m
Author: Zhenlan Wang <zhenlwa@microsoft.com>
Date:   Mon Oct 24 01:32:00 2016 -0700

    Update docs of package versions for .NET Core 1.1 Preview 1 release

[33mcommit ece4321a62648a05dd12a8029b19407601a79cf7[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Oct 21 09:34:45 2016 -0700

    Remove active issue 470.
    
    * Root problem was fixed in corefx with issue: dotnet/corefx#4429

[33mcommit 2b0fb790bbc4ecbb82877cc86e776ee9b7507be2[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Oct 20 13:43:27 2016 -0700

    Add IChannelInitializer to public API
    
    Also adds ClientRuntime.ChannelInitializers to public API because
    this is needed to register a custom IChannelInitializer.
    
    Also creates new mock initializer and unit tests to verify it
    is called properly.
    
    Fixes #1422

[33mcommit adcec3145170af8c40be213bd33e3a5c60565089[m
Merge: d5625b8 7fb7223
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Oct 20 13:32:58 2016 -0700

    Merge pull request #1601 from roncain/cleanup-1449
    
    Remove comments regarding 1449 workaround

[33mcommit 7fb72239dcdebb8d37b425b5bcd5c3a9488c7680[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Oct 20 05:19:50 2016 -0700

    Remove comments regarding 1449 workaround
    
    The issue behind #1449 has been opened against the
    ILC, and the workaround already in place is adequate.
    This PR merely removes the unnecessary references to
    issue #1449.
    
    skip ci please
    
    Fixes #1449

[33mcommit d5625b850a93e89b82a725d140638309524a192a[m
Merge: 8e4a593 55ecd00
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Oct 19 11:50:08 2016 -0700

    Merge pull request #1600 from StephenBonikowsky/UpdateBuildTools_10-12
    
    Update build tools 10 12

[33mcommit 55ecd00100967d6548defc2c356f139923456b2e[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Oct 14 16:07:05 2016 -0700

    Update buildtools and sync all other CoreFx build related changes.
    
    * This is necessarily large due to the amount of changes in CoreFx since we last synced.
    * It was not really possible to have a series of well organized commits as we were immediately broken when updating just buildtools so from that point forward it has been an effort to discover the cause and slowly pulling in more and more changes until we got to this point.

[33mcommit 8e4a59358030fcdd809466dac9807a1a566b1206[m
Merge: 0abf825 0d9a74f
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Oct 18 16:17:53 2016 -0700

    Merge pull request #1597 from roncain/channelpoolsettings
    
    Add ChannelPoolSettings

[33mcommit 0abf8251a2c434c206c5314ab5424b26db50d355[m
Merge: 21bf575 a049c0b
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Oct 18 16:17:14 2016 -0700

    Merge pull request #1596 from roncain/peerhopcount
    
    Add PeerHopCountAttribute

[33mcommit 21bf575ef1be7f271bda6bf95b23770f44325e22[m
Merge: 58ca7db d06905e
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Oct 18 16:16:33 2016 -0700

    Merge pull request #1598 from roncain/messageattributes
    
    Add MessageHeaderArrayAttribute and MessagePropertyAttribute

[33mcommit d06905e0cea8a311593d8ca92368df21948ffbc7[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Oct 17 08:41:21 2016 -0700

    Add MessageHeaderArrayAttribute and MessagePropertyAttribute

[33mcommit 0d9a74febf55a507923f6b36107f764c3006ac3c[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Oct 17 08:20:04 2016 -0700

    Add ConnectionPoolSettings

[33mcommit a049c0bd2a39f0d427e2c9557fd8b6db090b8601[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Oct 17 07:33:56 2016 -0700

    Add PeerHopCountAttribute
    
    Adds the PeerHopCountAttribute implementation from the full framework.

[33mcommit 58ca7dbd4c1c9671540757122c9cfbb2d6d5ace5[m
Merge: 4e28595 7c7d15f
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Oct 18 11:56:25 2016 -0700

    Merge pull request #1594 from roncain/update-docs
    
    Update documentation for new dev workflow

[33mcommit 7c7d15f12fae865bb95a4672e1ca3a358d62e717[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Oct 18 11:54:27 2016 -0700

    Convert quoted paths in markdown files to be hyperlinks into repo

[33mcommit 3d335aba5305656892fa7dda66187d0aa0fca2f3[m
Merge: 4ba5c25 9ecf934
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Oct 12 13:32:59 2016 -0700

    Bump minor version S.SM.P plus many other build updates.
    
    * Bump minor version of System.ServiceModel.Primitives
    * Update to latest build tools.
    * Update with many changes to shared build files from CoreFx.

[33mcommit 4e28595ab07bc4026b1326215492e7289098242f[m
Merge: 4ba5c25 9ecf934
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Oct 12 13:32:59 2016 -0700

    Merge pull request #1595 from ericstj/bumpPrimitives
    
    Bump primitives

[33mcommit 9ecf934a5f09481909d9430eb98e3811f3dce652[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Wed Oct 12 11:31:45 2016 -0700

    Rev System.ServiceModel.Primitives
    
    The assembly version needs to be incremented to represent the API added
    with https://github.com/dotnet/wcf/commit/f433e72259fd5a25468d0495aeaf41d5a8f57570

[33mcommit 89bd438c4a18226677aa0888fd8dc13af5e003ca[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Wed Oct 12 11:25:32 2016 -0700

    Use package harvesting over depproj
    
    Rather than explicitly using depproj's to represent previously shipped
    binaries, rely on package harvesting.

[33mcommit 96c70ecee8d4edebeb5b65f5deb2180125de24d9[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Wed Oct 12 11:24:12 2016 -0700

    Ensure live ref is used for netcore50
    
    BuildTools turned off the functionality that copies to netcore50
    automatically.  As a result we need to explicitly target netcore50 if we
    want the live build to be preferred over harvesting.
    
    See
    https://github.com/dotnet/buildtools/commit/9aa54c6f694f92a8d38f88cf3caf76a9687caf7b
    https://github.com/dotnet/corefx/commit/87d339420f925d408132901c9446a4b51ef839e1

[33mcommit cd91eeba3aca0c5429c63e829f7784640d2742a7[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Oct 11 08:31:56 2016 -0700

    Update documentation for new dev workflow
    
    This PR also includes more detailed instructions
    for running tests as UWP.

[33mcommit 4ba5c2527bae16587c130f9a5f485f6c5109ee59[m
Merge: d92f922 45cf690
Author: Hong Dai <hongdai@microsoft.com>
Date:   Tue Oct 11 10:32:20 2016 -0700

    Merge pull request #1590 from hongdai/issue1569
    
    Fix the issue that we use WindowsIdentity.Current on *NIX

[33mcommit 45cf69064b96e1d2e89224fc639eb068f90cbfe6[m
Author: hongdai <hongdai@microsoft.com>
Date:   Tue Oct 11 09:01:32 2016 -0700

    Fix the issue that we use WindowsIdentity.Current on *NIX
    
    * We use try/catch to catch PlatformNotSupported exception
    and use the same way to set default user on UWP.
    
    Fix #1569

[33mcommit d92f922715a3ba6d2a8998fb6670c85171cd1399[m
Merge: 3cb666e 96e0c4d
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Oct 10 12:01:19 2016 -0700

    Merge pull request #1571 from roncain/custom_channel_fix
    
    Fix OnOpen/OnBeginOpen issues with custom channels

[33mcommit 96e0c4d137ff583eda836c2448b737bd54a8fb73[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Sep 27 08:55:32 2016 -0700

    Fix OnOpen/OnBeginOpen issues with custom channels
    
    Fixes the product issue by propagating knowledge of
    sync and async open/close requests down the channel
    stack so they stay sync or async based on the original
    call.
    
    Created mocks and unit tests for custom channels
    
    Fixes #1544

[33mcommit 3cb666eb43748caf0d119c44aa0f615c46b023ef[m
Merge: e50e3fa 844e22c
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Oct 7 14:58:44 2016 -0700

    Merge pull request #1586 from StephenBonikowsky/UpdateWCFPkgVersions
    
    Updating WCF pkg version to get new API

[33mcommit 844e22c8b7206d8a3ee98f515e9a03cc6ac483e8[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Oct 7 13:55:35 2016 -0700

    Updating WCF pkg version to get new API
    
    * Updating to WCF package version 4.3.0-beta-24607-01 which contains the new API Hong checked-in yesterday.
    * Also increased the timeout for one negative test, not sure why it changed but the test needed a little more time to get the expected exception.

[33mcommit e50e3fa215f96b061fe96e12acb4abbe555a1901[m
Merge: ab45a6d 9610b44
Author: Zhenlan Wang <zhenlwa@microsoft.com>
Date:   Fri Oct 7 09:45:35 2016 -0700

    Merge pull request #1579 from zhenlan/scriptfix
    
    Improvement of IIS hosted service setup script

[33mcommit ab45a6ddc4be73b9aef7aa811578cd049550008d[m
Merge: 4c2c915 f433e72
Author: Hong Dai <hongdai@microsoft.com>
Date:   Thu Oct 6 14:10:44 2016 -0700

    Merge pull request #1481 from hongdai/issue1421
    
    Add API System.ServiceModel.Dispatcher.FaultContractInfo

[33mcommit f433e72259fd5a25468d0495aeaf41d5a8f57570[m
Author: hongdai <hongdai@microsoft.com>
Date:   Thu Oct 6 10:59:35 2016 -0700

    Add API System.ServiceModel.Dispatcher.FaultContractInfo
    
    * Expose System.ServiceModel.Dispatcher.FaultContractInfo
    * Additonal APIs needed for customers to access FaultContractInfo
    * Add a scenario test and Unit test for SynchronizedCollection
    
    Fixes #1421

[33mcommit 4c2c915e24af025f656ef13262838ed322579019[m
Merge: 8f5b766 cb1df55
Author: Hong Dai <hongdai@microsoft.com>
Date:   Thu Oct 6 09:11:37 2016 -0700

    Merge pull request #1584 from hongdai/issue1260
    
    Enable test disabed by active issue 1260

[33mcommit cb1df556dc57259e8ffba4da96ab8d4ab5529c77[m
Author: hongdai <hongdai@microsoft.com>
Date:   Wed Oct 5 16:38:19 2016 -0700

    Enable test disabed by active issue 1260
    
    * I've done a manual verification and verifed this test now pass in
    the manual test.

[33mcommit 9610b44b1a90c06d228c26928592599cadc4fafc[m
Author: Zhenlan Wang <zhenlwa@microsoft.com>
Date:   Tue Oct 4 12:20:57 2016 -0700

    Improvement of IIS hosted service setup script
    - Ensure HTTPS binding is created during setup
    - Add more output information for better diagnostic experience

[33mcommit 8f5b76658b8923d422290b0f8a29e09a14b33fe8[m
Merge: c303c7a 1303ffc
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Mon Oct 3 14:52:08 2016 -0700

    Merge pull request #1575 from iamjasonp/x509-copy-ctor-workaround
    
    Work around X509Certificate2 copy constructor omission

[33mcommit c303c7aaf062a2b3da3f52100a3a4afc7d8cd673[m
Merge: 30688f4 677febc
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Oct 3 10:58:58 2016 -0700

    Merge pull request #1576 from dotnet-bot/master-UpdateDependencies
    
    Update CoreClr, CoreFx, WCF to beta-24603-03, beta-24601-02, beta-24603-01, respectively (master)

[33mcommit 677febcf3d747b8bbf87f5b0047a76607198ca5c[m
Author: dotnet-bot <dotnet-bot@microsoft.com>
Date:   Mon Oct 3 16:19:44 2016 +0000

    Update CoreClr, CoreFx, WCF to beta-24603-03, beta-24601-02, beta-24603-01, respectively

[33mcommit 1303ffc492755cdc22bef8ca26d7d8f09b562cdf[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Fri Sep 30 16:04:09 2016 -0700

    Enable previously disabled tests due to certificate issues

[33mcommit 3a158b5450f931895eab29556c347f1f4fab91c6[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Fri Sep 30 15:49:38 2016 -0700

    Work around X509Certificate2 copy constructor omission
    
    In .NET Core, the X509Certificate2 copy constructor was missing; where WCF code
    needed to clone a certificate, we replaced the references to the copy constructor
    with code that uses the certificate.Handle instead
    
    However, in Linux, OpenSSL does not provide the private key when using this handle,
    meaning the private key goes missing in all subsequent copies; as such, client cert
    authentication doesn't work.
    
    This is a stopgap solution only, as we await the inclusion of the X509Certificate2
    copy ctor in a future .NET Core version

[33mcommit 30688f4991ab32bb2acbb1232c1856c4572309b1[m
Merge: 236d191 9940266
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Sep 30 16:39:31 2016 -0700

    Merge pull request #1540 from dotnet-bot/master-UpdateDependencies
    
    Update CoreClr, CoreFx, WCF to beta-24529-02, beta-24529-03, beta-24529-01, respectively (master)

[33mcommit 99402667bbcd25e7b38ba34600b75e9d4d362b9d[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Sep 29 15:49:11 2016 -0700

    Needed test updates to eliminate package downgrades.
    
    * After adding NETStandard.Library to test-runtime test projects needed to have their references to some of those packages updated to latest so as not to downgrade the version being used for test runs.

[33mcommit 16c637581d35f48a0ba26055275ca212d8da77c5[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Thu Sep 29 08:53:32 2016 -0700

    Clean up downgrade warnings in test-runtime

[33mcommit 340d01e6b2ce76ec77d2648a8b1580e498ba0f34[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Sep 29 08:40:53 2016 -0700

    Merging PR #1572 to unblock PR

[33mcommit 6bef094c37754708db2d85cf1db39232a9cf1701[m
Author: dotnet-bot <dotnet-bot@microsoft.com>
Date:   Thu Sep 29 15:02:13 2016 +0000

    Update CoreClr, CoreFx, WCF to beta-24529-02, beta-24529-03, beta-24529-01, respectively

[33mcommit 236d1919d22f52d30628878a6466d6b03169b8c7[m
Merge: 247d18f f5059a9
Author: Hong Dai <hongdai@microsoft.com>
Date:   Wed Sep 28 14:06:46 2016 -0700

    Merge pull request #1570 from hongdai/issue1564
    
    Skip tests with known issue or should not get run on Linux

[33mcommit f5059a99a46d0e6ba141c0b96f9b5843dde9d18c[m
Author: hongdai <hongdai@microsoft.com>
Date:   Wed Sep 28 11:24:38 2016 -0700

    Skip tests with known issue or should not get run on Linux
    
    * Two tests are failing in Kerberos Manual tests due to known product issue.
    * One test need to validate server window credential on the client side,
    does not work on non window machines.
    
    Fixes #1564 and issue#1563

[33mcommit 247d18fed89a8c6040769af28d951991eda980e8[m
Merge: 6b43312 f72b738
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Sep 28 09:28:50 2016 -0700

    Merge pull request #1568 from StephenBonikowsky/RemoveToFSpecificTestCode
    
    Remove all the #if#defs needed for ToF

[33mcommit f72b7387cabf9586dc8645d621a8cda8e04e625e[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Sep 27 15:09:54 2016 -0700

    Remove all the #if#defs needed for ToF
    
    * Fixes #1527

[33mcommit 6b433121a0416e249d73c26eb6011aa7b3b62b91[m
Merge: b07247f 05a8980
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Sep 27 13:55:21 2016 -0700

    Merge pull request #1565 from StephenBonikowsky/VersionTestFilesToRelease1.1
    
    Renamed pre-release test files to 1.1 release.

[33mcommit 05a8980d9b0491a96a30671019aa69420f2eb62a[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Sep 27 10:07:29 2016 -0700

    Renamed pre-release test files to 1.1 release.
    
    * Included the new scenario infrastructure tests.

[33mcommit b07247f10e2ec0150bcad898bdb1cbb4cd24f534[m
Merge: 2d68ec9 3a04e3e
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Sep 23 15:45:35 2016 -0700

    Merge pull request #1560 from roncain/reenable-issue-10-tests
    
    Re-enable tests disabled in UWP for issue #10

[33mcommit 3a04e3e08755a14128f529de8bf844a3011aad02[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Sep 23 06:05:01 2016 -0700

    Re-enable tests disabled in UWP for issue #10

[33mcommit 2d68ec91d7041776a97b72654adf6755f631f082[m
Merge: 7dc2b24 74bbcad
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Sep 22 15:06:11 2016 -0700

    Merge pull request #1556 from StephenBonikowsky/TestTFM_Updates
    
    Updates to build scripts from CoreFx.

[33mcommit 7dc2b2449e518ff1d326c36c6aba337fa9e36d69[m
Merge: 5c10a39 a8c2b40
Author: Matt Connew <mconnew@microsoft.com>
Date:   Thu Sep 22 14:43:02 2016 -0700

    Merge pull request #1557 from mconnew/Fix-EventSource
    
    Some more fixes to some changes accidentally comitted to WcfEventSource

[33mcommit a8c2b40b69e7ee282d4aeb5865110dbdc406e25a[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Thu Sep 22 12:37:06 2016 -0700

    Some more fixes to some changes accidentally comitted to WcfEventSource

[33mcommit 74bbcadf3ee92d7efb1dc0f2e6aa8a19d154a3dc[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Sep 22 11:41:37 2016 -0700

    Updates to build scripts from CoreFx.
    
    Just part of the continuing effort to stay in sync with CoreFx.

[33mcommit 5c10a391c334d3a4a19bbe8f13174253cee9681d[m
Merge: 91366af ff3188b
Author: Matt Connew <mconnew@microsoft.com>
Date:   Thu Sep 22 10:28:34 2016 -0700

    Merge pull request #1554 from mconnew/Fix-EventSource
    
    Fixing mistake from earlier ETW event source fix

[33mcommit ff3188b951adc122db568a549dbe0dabff971db5[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Wed Sep 21 16:05:05 2016 -0700

    Fixing mistake from earlier ETW event source fix

[33mcommit 91366afbe60cc0f350cd544cc56bfbfe75f219b9[m
Merge: 6742547 0d030c4
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Sep 21 16:03:20 2016 -0700

    Merge pull request #1553 from StephenBonikowsky/Fix_TestTFM_Filtering
    
    Fix TestTFM filtering so it works for all and not just netcoreapp

[33mcommit 0d030c4a1523a685e6e81cd7edfaa5e294508909[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Sep 21 14:54:23 2016 -0700

    Fix TestTFM filtering so it works for all and not just netcoreapp
    
    * Porting dotnet/corefx#11819

[33mcommit 6742547e9a4ccc0eb3bd8080d17be6a877eb291c[m
Merge: befec81 9b60c9c
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Sep 21 12:03:03 2016 -0700

    Merge pull request #1547 from roncain/co-onopen
    
    Fix issue where CommunicationObject.OnOpen was not called.

[33mcommit 9b60c9c96f4ffb2b76d3d02b9a82fc2d2840d015[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Sep 19 10:13:23 2016 -0700

    Fix issue where CommunicationObject.OnOpen was not called
    
    Converting the synchronous CommunicationObject open/close paths
    introduced a bug that prevented the OnOpen() method to be called
    when the synchronous Open() method was used.
    
    The fix is to be aware of whether the caller invoked the synchronous
    Open or the asynchronous BeginOpen and invoke the appropriate
    synchronous or asynchronous override.
    
    Fixes #1544

[33mcommit befec815f8050b14d186ea3f6bd199d775114e54[m
Merge: ce0326b 1ca7b80
Author: Matt Connew <mconnew@microsoft.com>
Date:   Wed Sep 21 11:00:17 2016 -0700

    Merge pull request #1550 from mconnew/Issue1254
    
    Add WCF channel keywords to ETW events
    Fixes #1254

[33mcommit ce0326b54c5e835499904daff8d263a5336ba42c[m
Merge: a2c95ad ec39cef
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Sep 20 04:01:19 2016 -0700

    Merge pull request #1548 from roncain/reenable-10
    
    Re-enable tests disabled by issue 10

[33mcommit 1ca7b80e4d1fc35cee8433da23a7e146c9c18124[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Mon Sep 19 15:48:50 2016 -0700

    Add WCF channel keywords to ETW events
    
    Fixes #1254

[33mcommit ec39cefe903882d1c088eff660c080f4c7602634[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Sep 19 11:46:11 2016 -0700

    Re-enable tests disabled by issue 10, except for NET Native.
    
    PR #1502 fixed issue #10 by implementing UpnEndpointIdentity.
    But 2 tests were still disabled by issue 10, so this PR just
    re-enables those tests.  Both require manual setup to run, so
    we will not see them pass or fail in CI or VSO.
    
    These tests still fail in UWP and so remain disabled there.

[33mcommit a2c95ad9e5cb2fff465655ae234454cf6f1c0f83[m
Merge: b3a031c aa4df77
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Sep 16 16:37:46 2016 -0700

    Merge pull request #1545 from StephenBonikowsky/FixDependenciesFile
    
    Update caseing in dependencies.props

[33mcommit aa4df77a1dd6f9337ac956e1d14f76718690be93[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Sep 16 15:49:33 2016 -0700

    Update caseing in dependencies.props
    
    *  RemoteDependencyBuildInfo "WCF" needs to match the CurrentRef property but it is case sensitive.

[33mcommit b3a031cbda3b271c18624c6bebb7aacded45993b[m
Merge: 2f6cda5 dac98a7
Author: Matt Connew <mconnew@microsoft.com>
Date:   Fri Sep 16 13:04:15 2016 -0700

    Merge pull request #1502 from mconnew/Issue1454
    
    Fix UpnEndpointIdentity on UWP

[33mcommit dac98a73118fa5354d0b6f820d8be4d3d33b21a5[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Thu Sep 1 19:31:52 2016 -0700

    Enable UpnEndpointIdentity tests on UWP

[33mcommit 6a0aa8ac99feac0d8c768006c4a19faafdc29c6b[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Thu Sep 1 19:31:09 2016 -0700

    Add partial UpnEndpointIdentity support

[33mcommit 2f6cda58ef8e449dc2a04728cb1176f85234e5fb[m
Merge: 4da36ba 8859eae
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Fri Sep 16 17:06:35 2016 +0800

    Merge pull request #1543 from iamjasonp/update-netci-master
    
    Fix typo in netci.groovy to correct outerloop servers

[33mcommit 8859eaef90214926ff6a612efdedc1773f3bdb15[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Fri Sep 16 00:13:10 2016 -0700

    Fix typo in netci.groovy to correct outerloop servers

[33mcommit 4da36ba0b97e1ca4da853b8d64bf22cf3a42a07d[m
Merge: 0202bea 87ce201
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Fri Sep 16 02:43:28 2016 +0800

    Merge pull request #1535 from iamjasonp/netci-110-branch
    
    Update netci.groovy for new branches for outerloop

[33mcommit 87ce2017aedf9ea29b7a96906abbd1a54a2a7654[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Sep 15 11:03:46 2016 -0700

    Update netci.groovy for new branches for outerloop
    
    * release/1.1.0
    * ws-trust

[33mcommit 0202bea0bc52c7dcd858e04c95afa711a448b2a3[m
Merge: a8ff8f1 7e69cb2
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Sep 15 10:59:14 2016 -0700

    Merge pull request #1531 from StephenBonikowsky/DependencyValidation
    
    Port CoreFx PR #10868 Full-version package dependency verification.

[33mcommit 7e69cb2192767d441f44de5fa2b6c99daaaaca16[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Sep 14 16:24:06 2016 -0700

    Port CoreFx PR #10868 Full-version package dependency verification.
    
    * Moves package validation out of dir.props
    * Leverages dotnet/versions repo for updating and validating package versions.

[33mcommit a8ff8f1b0df6ce853a83e7182150a39a4ec6fc5d[m
Merge: 7d2bbeb 0a31f58
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Thu Sep 15 20:03:38 2016 +0800

    Merge pull request #1532 from iamjasonp/update-code-coverage
    
    Update code coverage job

[33mcommit 0a31f585bd7fb39bb44a4db2193ffff9e224e504[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Wed Sep 14 20:07:48 2016 -0700

    Update code coverage job
    
    After change to dev workflow, code coverage jobs are failing because we pass in
    a WithCategories=OuterLoop parameter. Remove this so we get code coverage again

[33mcommit 7d2bbebf395c1fd9f658601787157b0b5c3e4d8c[m
Merge: 721d0af a26844e
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Sep 14 07:23:49 2016 -0700

    Merge pull request #1475 from roncain/enable-1347
    
    Reenable test disabled by #1347

[33mcommit 721d0af4018e02ac41656ed506b2c8290ad2ef7e[m
Merge: 502ad56 4040b67
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Wed Sep 14 10:32:12 2016 +0800

    Merge pull request #1521 from iamjasonp/netci-push-trigger
    
    Add push triggers for outerloops

[33mcommit 502ad56348e1642e536049ec378bf1913d45375a[m
Merge: 1db4b1e d9cc0d1
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Sep 13 15:14:44 2016 -0700

    Merge pull request #1523 from StephenBonikowsky/CorefxPreReleaseGroupSupport
    
    Update buildtools and CoreFx pkg dependency versions.

[33mcommit 1db4b1ecbb6c3c8c173e251386e13e75ee0732eb[m
Merge: b723898 a2ac64c
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Sep 13 13:55:22 2016 -0700

    Merge pull request #1522 from roncain/testprop-fixes
    
    Miscellaneous test property fixes

[33mcommit d9cc0d128a32bb507bb686200a6cf4783f6a4fca[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Sep 13 09:54:53 2016 -0700

    Update buildtools and CoreFx pkg dependency versions.
    
    * Buildtools and Microsoft.Private.PackageBaseline (in CoreFx) have been updated to support 'PreReleaseGroup'.
    * This means that now in VSTS, the test runs will honor the package versions we specified for non-WCF packages.
    * The test runs will only ignore the specified WCF package versions in order to use the "just built" versions.
    * Also pulled in necessary changes due to dotnet/corefx#11387

[33mcommit a2ac64c86f71d0185033c6b988363c1077baa99e[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Sep 13 11:35:36 2016 -0700

    Miscellaneous test property fixes
    
    Trim test property and environment variable values to avoid
    issues we found with accidental whitespace.
    
    UPN_Available was not properly being passed into TestProperties,
    so this Condition was always false unless an environment variable
    was set for it on the machine where the tests were run.
    
    IssueAttribute parses the IncludeTestsWithIssues TestProperty to
    allow multiple issues to be specified.  But if semicolon separated,
    it causes a code-gen error in TestProperties.  So change it to allow
    a comma separated list.

[33mcommit 4040b6755c46491f167a285303bda4497b56aa94[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Tue Sep 13 09:18:11 2016 -0700

    Add push triggers for outerloops

[33mcommit b723898b4a536ff374ff89a9ea63cdd4d6ef0592[m
Merge: fe6bd42 2309f4b
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Tue Sep 13 23:54:17 2016 +0800

    Merge pull request #1519 from iamjasonp/update-prservice
    
    Update PRService to prevent hangs on sync

[33mcommit a26844e52ab917537fe6423e799ccc350aeaf48c[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Aug 15 08:49:04 2016 -0700

    Reenable test disabled by #1347
    
    Issue #1347 was closed but there was still a test being
    skipped on *nix machines due to an IssueAttribute for it.
    
    This PR removes the IssueAttribute to re-enable this test.

[33mcommit 2309f4ba6a05bb5af838345bb765a6f3e59acb53[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Tue Sep 13 01:33:48 2016 -0700

    Update PRService to deal with issues at merge
    
    The merge step to get master into a good state was causing an indefinite hang in
    certain cases, which would result in 1) git.exe to hang and 2) the PRService to return
    a 500 error and cause CI to fail intermittently
    
    This change removes the merge step and replaces it with a force checkout, deletes all
    branches, and then checks out to origin/master during the cleanup step. This results in
    a clean repo as well as sidesteps the merge step causing the hang

[33mcommit fe6bd42f59816e3e7a58c11bce7e11149bf67954[m
Merge: 89a16d4 ca6b358
Author: Matt Connew <mconnew@microsoft.com>
Date:   Mon Sep 12 21:20:04 2016 -0700

    Merge pull request #1501 from mconnew/Issue1482
    
    Removing IssueAttribute as test is working fine

[33mcommit ca6b358ff863f0e0fb37707f8ef0344b6b735d35[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Thu Sep 1 16:47:35 2016 -0700

    Removing IssueAttribute as test is working fine

[33mcommit 89a16d45a1846d2cc283a10b79619e108327b708[m
Merge: 9a2dd72 83869bf
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Sep 12 09:44:49 2016 -0700

    Merge pull request #1516 from StephenBonikowsky/PortCoreFxPR10231_ready
    
    Port core fx pr10231 ready

[33mcommit 83869bf8fe7103b22908ca7166f77d9f3fa778a8[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Sep 8 13:48:09 2016 -0700

    Final set of changes to complete the port of CoreFx PR 10231
    
    * Changes related to build.override.targets so that the needed targets are called at the correct time.
    * Moved a cleaning step for the self hosted service from build.override.targets to the clean.cmd because it has to occur before the call to "git clean -xdf".
    * Ran "git update-index --chmod=+x" against several .sh files so they are marked as an executable on Linux.
    * Including changes to netci.groovy

[33mcommit 4c76ca6b88a5c73645f42c032b2fdb8e3627983a[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Fri Sep 2 15:43:42 2016 -0700

    Switch to using packageIndex instead of lists

[33mcommit 0735159a36516f664bc86643fbc0de6d8f277835[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Aug 31 16:09:02 2016 -0700

    Update all project.json with newer versions.

[33mcommit a26fcf0e1392caf7508dc69421cc2d41b063c804[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Aug 31 14:59:22 2016 -0700

    Synced all shared scripts to CoreFx latest changes.
    
    * I did not include changes related to a change by Davis in CoreFx PR #10868

[33mcommit 4852d19a6b9cf6ce7a81811413f3dcdb6d5c7f66[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Aug 31 14:12:38 2016 -0700

    Updating files touched by Mariana in CoreFx PR #10231
    
    * Each of these files was either edited or added in Mariana's PR.
    * I synced each of these files to what is currently in CoreFx meaning that they will include any changes made since Mariana's PR.
    * The next commit will update all other shared scripts 'not' touched by Mariana's PR to make sure we don't have any partial changes from other people.
    * Mariana's PR touched many other Native files in CoreFx, WCF has none of these.
    * Mariana's PR also touched netci.groovy, Jason Pang owns the WCF version of this file so I have not touched it in this commit.

[33mcommit 9a2dd72cbb6ec6fca166152cf46485529c962782[m
Merge: 76f96f2 4b0bb6e
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Fri Sep 9 10:11:29 2016 +0800

    Merge pull request #1513 from dotnet/update-readme-badgs
    
    Update readme.md badges to show all CI builds

[33mcommit 76f96f24278f49ddbd1622af62bc42a762533817[m
Merge: c7e8694 cf43182
Author: Hong Dai <hongdai@microsoft.com>
Date:   Thu Sep 8 14:30:54 2016 -0700

    Merge pull request #1512 from hongdai/updatewebsetup
    
    Update IIS Server setup scripts to unlock authentication methods

[33mcommit cf4318263651e3a6155d486a1b582ba8e8b8d37f[m
Author: hongdai <hongdai@microsoft.com>
Date:   Thu Sep 8 10:25:30 2016 -0700

    Update IIS Server setup scripts to unlock authentication methods
    
    * Authentication methods sections need to be unlocked from the root
    for individual website to overrid them.
    
    Fixes #1511

[33mcommit 4b0bb6ef4a2be345909d688a459356f64f58c508[m
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Thu Sep 8 11:36:48 2016 +0800

    Add badges for selfhosted Windows NT outerloops

[33mcommit d226ed79680c7518bccfd5820e4dbbdf62aff403[m
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Thu Sep 8 11:30:01 2016 +0800

    Update readme.md badges to show all CI builds

[33mcommit c7e8694c8a810bc996ba0ddaaf199a06983cf9e9[m
Merge: 96f8ce5 3c9c997
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Wed Sep 7 18:12:52 2016 +0800

    Merge pull request #1509 from iamjasonp/update-cert-doc
    
    Update certificate generation documentation

[33mcommit 3c9c997d076b6ac0c13952d677fbb1023affa5aa[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Tue Sep 6 00:08:47 2016 -0700

    Update certificate generation documentation

[33mcommit 96f8ce55dad5b201b8e3a8474fa7815db3d93a65[m
Merge: b25f296 d717172
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Tue Sep 6 11:19:09 2016 +0800

    Merge pull request #1508 from iamjasonp/update-netci
    
    Update netci.groovy scripts

[33mcommit d7171726c1a94972788bd7e5a3ae77f92f640701[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Mon Sep 5 03:35:11 2016 -0700

    Add msbuild.log to archived outerloop jobs

[33mcommit a4f7831c74d296881c82a291aad9bc340338f1cf[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Mon Sep 5 02:19:21 2016 -0700

    Add a help job to wcf repo

[33mcommit b25f2969a34dff94a51debf773981d6d74266152[m
Merge: f2b9085 e677fd1
Author: Matt Connew <mconnew@microsoft.com>
Date:   Thu Sep 1 16:09:25 2016 -0700

    Merge pull request #1495 from mconnew/Issue1467
    
    Fix Net.Tcp duplex streaming with authentication
    Fixes #1443

[33mcommit e677fd1cfc69c8e146d8b33e80f0f38c66b335bd[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Mon Aug 29 11:34:59 2016 -0700

    Add Net.Tcp duplex streamed test using authentication

[33mcommit 1c01db2597f14d77d95004840799c94664a16499[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Mon Aug 29 11:34:15 2016 -0700

    Replace ReadAheadWrappingStream with BufferedReadStream and make it http only

[33mcommit fb0636b201b7aa1f39252dc64c04f7a795f387aa[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Mon Aug 29 11:28:55 2016 -0700

    Add helpers to schedule task completions on our own thread pool
    
    When all the threadpool threads are blocked on synchronous calls,
    Task completions can't continue. These helper classes enable the
    task completions to run on threads not part of the threadpool and
    allows completion. Without this, large drops in CPU occur with
    thread starvation.

[33mcommit f2b9085b982cf51a773fcc57cdd769fa39c86f62[m
Merge: 3d2a719 fc21efc
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Sep 1 11:13:49 2016 -0700

    Merge pull request #1497 from StephenBonikowsky/ActivateExceptionTest
    
    Activate test case, reason for being skipped no longer reproes.

[33mcommit fc21efc29f5efbdfa608b17d6fa9567094ab57f4[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Aug 31 13:34:25 2016 -0700

    Activate test case, reason for being skipped no longer reproes.
    
    * This test was observed to hang back when it was still being run under the ToF infrastructure.
    * Failure has not reproduced in current N run environment.
    * Fixes Issue #1250

[33mcommit 3d2a719c20d32e78a676a205229e462257515eb6[m
Merge: aedb049 4e54a8a
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Mon Aug 29 14:56:22 2016 -0700

    Merge pull request #1492 from iamjasonp/scheduled-restart-script
    
    Add script for scheduled self-hosted service restart

[33mcommit 0e0f015ad0a09c9565dc92cbd9146deed5ffa5f6[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Mon Aug 29 11:25:42 2016 -0700

    Fix null-ref exception when doing get request

[33mcommit 4e54a8a28741e275397fe9053d83395c15c31028[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Fri Aug 26 14:31:52 2016 -0700

    Extend CertificateGenerator cert expiration to 90 days
    
    Change expiry from 30 days to 90 days to match what infrastructure expects. We don't
    expect the dev experience to differ much changing the cert expiry from 30 to 90 days
    This allows us to avoid special-casing the infrastructure code

[33mcommit 7eab53e640ef8c4fe03fcd09b3a873dda41f9026[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Fri Aug 26 14:30:55 2016 -0700

    Add script for refreshing server certificates
    
    This was another magic script that lived somewhere in a file share, so rewriting and
    committing this for the ages so we have the ability to easily find and use this script

[33mcommit 69f4c95d0405b1a5308239a95d944c50cd398932[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Aug 25 17:23:14 2016 -0700

    Add script for scheduled self-hosted service restart
    
    Test infrastructure requires that the self-hosted WCF Service be restarted periodically
    Commit this script so that it's in source control rather than in somebody's file share
    internally :)

[33mcommit aedb049318058cbde1bd33692d3b2ff88a1b2886[m
Merge: 1472072 e43fc4e
Author: Immo Landwerth <immo@landwerth.net>
Date:   Tue Aug 23 16:11:36 2016 -0700

    Merge pull request #1488 from terrajobst/master
    
    Add section on how to file security bugs

[33mcommit e43fc4e7ee9b8ba90146f6fee195539ac23408db[m
Author: Barry Dorrans <Barry.Dorrans@microsoft.com>
Date:   Tue Aug 23 16:04:19 2016 -0700

    Add section on how to file security bugs

[33mcommit 1472072949c3d50c260c7890a67ba45197f09659[m
Merge: dd4a505 7e11933
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Aug 19 09:15:15 2016 -0700

    Merge pull request #1484 from roncain/cleanup-selfhost
    
    Cleanup selfhost

[33mcommit 7e11933fd4312416122338fa744eeaabf8a4a964[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Aug 18 13:45:39 2016 -0700

    Cleanup SelfHost code to make service endpoint creation easier
    
    Refactors the way ServiceHosts are instantiated and started.
    The prior 'copy/paste' approach was becoming unwieldy and
    easy to get wrong.
    
    This is largely a mechanical refactor of the existing code
    and does not alter any service host types, service types,
    or endpoint addresses.

[33mcommit dd4a505e7b33a09e755014ea691837ba2ffc30de[m
Merge: 3741268 e6a3c77
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Aug 18 11:35:19 2016 -0700

    Merge pull request #1483 from roncain/disable-failing-nettcp
    
    Disable NetTcp duplex callback test that fails in CI and Helix

[33mcommit e6a3c77715812e0044e63bf3dd1fa01c0c257a8b[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Aug 18 10:52:19 2016 -0700

    Disable NetTcp duplex callback test that fails in CI and Helix
    
    This test was passing 10 days ago when it was first added,
    but over the past 2 days it has been failing in both CI and Helix.
    
    Disabling it with IssueAttribute to return to having clean CI runs.

[33mcommit 37412680666f4360777c08ae4948f24087fb58a4[m
Merge: 0cb5820 bfffc05
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Wed Aug 17 09:42:02 2016 -0700

    Merge pull request #1479 from iamjasonp/fix-etw-test-reporting
    
    Fix reporting for ETW traces in testing

[33mcommit 0cb58200220d309ea615790bb0876497d3378a74[m
Merge: aa5c8be d96c97d
Author: Matt Connew <mconnew@microsoft.com>
Date:   Tue Aug 16 21:37:44 2016 -0700

    Merge pull request #1461 from mconnew/EndpointIdentityFix
    
    Enable reading endpoint identity in soap headers
    Fixes #1432

[33mcommit bfffc058c6f4e8daca89173fb9f5d42e28a6fafe[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Tue Aug 16 16:14:16 2016 -0700

    Fix reporting for ETW traces in testing

[33mcommit aa5c8be2719d975e86fb979b3ad3d925a9389994[m
Merge: 4f46f17 cdd91da
Author: Hong Dai <hongdai@microsoft.com>
Date:   Fri Aug 12 15:20:34 2016 -0700

    Merge pull request #1472 from hongdai/issue1262
    
    Depend on current version of System.NetSecurity

[33mcommit 4f46f170f5b8b2c1875fdad2b3aaee62fcb1ff4b[m
Merge: cc87c2c 059b2b8
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Fri Aug 12 10:32:10 2016 -0700

    Merge pull request #1456 from iamjasonp/enable-https-tests
    
    Enable HttpsTests for platforms that support it

[33mcommit cc87c2c1b72e0c5d479ef59875f1484f36ed6fe4[m
Merge: a68de12 b280aff
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Fri Aug 12 10:01:46 2016 -0700

    Merge pull request #1473 from iamjasonp/redirect-installcert-output
    
    Redirect InstallRootCertificate.sh c_rehash output

[33mcommit cdd91da146ef2045c4ba42af45ab2c191630565e[m
Author: hongdai <hongdai@microsoft.com>
Date:   Thu Aug 11 18:28:07 2016 -0700

    Depend on current version of System.NetSecurity
    
    * We need the fix for issue https://github.com/dotnet/corefx/issues/9160
    * I have verified manually we can now set Explict username/pwd for TCP.
    
    fixes #1262

[33mcommit b280aff08f13c1c43a18fd0893c9a76906ed8305[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Aug 11 16:45:04 2016 -0700

    Redirect InstallRootCertificate.sh c_rehash output
    
    In InstallRootCertificate.sh, redirect the c_rehash stdout to /dev/null, and
    continue to allow the stderr to print to console
    
    This will help reduce clutter in output when in our test setup steps,
    especially since in our tests we may run this script more than once and c_rehash
    output is quite verbose and prints every root cert out

[33mcommit 059b2b8ec5d848176cf611867fc86ceb83d7dbd0[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Aug 11 16:22:01 2016 -0700

    Convert cURL cert errors to SecurityNegotiationExceptions
    
    Currently, cURL certificate exceptions surface as CommunicationExceptions,but
    this causes a discrepancy with Windows versions. We need to correctly convert
    the exceptions to SecurityNegotiationException so the exceptions are consistent

[33mcommit 4e3edd9db164fbbd9efe60d29ea1430f9b416e81[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Fri Aug 5 11:18:43 2016 -0700

    Enable HttpsTests for platforms that support it

[33mcommit 629cf4b42502b628be68dc0ac04f359ff38c1df4[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Aug 11 14:11:18 2016 -0700

    Move X509Certificate package forward in test-runtime
    
    Move System.Security.Cryptography.X509Certificates version in test-runtime forward
    to match the runtime version in src/project.json

[33mcommit a68de123283967062d2a83fd4c7803b2cffaa1fa[m
Merge: e63f0a1 168bcf2
Author: Matt Connew <mconnew@microsoft.com>
Date:   Wed Aug 10 18:49:35 2016 -0700

    Merge pull request #1471 from mconnew/Issue1467
    
    Assert was presuming there would always be a client cert which isn't the case

[33mcommit 168bcf2eb9cfa746724fcbbe438c1653f1a20206[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Wed Aug 10 16:00:05 2016 -0700

    Assert was presuming there would always be a client cert which isn't the case.

[33mcommit e63f0a164a6d83d69565f49b6568dc2afaffe1af[m
Merge: 8fed415 2e10ed0
Author: Matt Connew <mconnew@microsoft.com>
Date:   Wed Aug 10 12:27:47 2016 -0700

    Merge pull request #1470 from mconnew/FixVSSolutions
    
    Fix project solution files
    
    Development should be more pleasant now as VS won't take 20+ minutes to load each time.

[33mcommit 2e10ed00c60fa1078a3424c17962424c009ab317[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Wed Aug 10 11:18:37 2016 -0700

    Fix project solution files
    
    Renamed S.P.SM.sln to S.P.SM.All.sln and made it not auto build test projects
    Added sln files for each test project
    Create S.P.SM.sln file which only includes the S.P.SM project

[33mcommit 8fed415d554403d2648d4bad16e9d5245ebb0834[m
Merge: 5d1cab5 d693760
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Aug 9 13:06:59 2016 -0700

    Merge pull request #1466 from roncain/disable-etw-empty
    
    Omit empty ETW "Begin" and "End" messages if no events to report

[33mcommit 5d1cab5c367aa0004b13b41cb63b15db7d3d5837[m
Merge: f022cc9 da38628
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Aug 9 13:06:26 2016 -0700

    Merge pull request #1462 from roncain/enable-testdata
    
    Fix UWP issue #1450 and re-enable tests disabled for it.

[33mcommit f022cc96d7cf9cf9ca49013c1b7a750d9b9f0c09[m
Merge: 69689f0 870dde4
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Aug 9 11:44:47 2016 -0700

    Merge pull request #1469 from roncain/fact-cleanup
    
    Convert FactAttributes to WcfFactAttribute in missed files

[33mcommit 69689f077166c79a5696cc95d9e0b8599405c286[m
Merge: b4e4ad4 7ba43bd
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Aug 9 11:39:38 2016 -0700

    Merge pull request #1464 from roncain/disable-uwp-test
    
    Adds IssueAttribute for NET Native inadvertantly omitted

[33mcommit 870dde4bbe4752b58a0e9f3b515f810bc2385a42[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Aug 9 11:26:33 2016 -0700

    Convert FactAttributes to WcfFactAttribute in missed files
    
    During the recent migration from using FactAttribute to
    WcfFactAttribute, a few files were missed.  This PR just
    applies the same manual boiler-plate changes made to all
    the others.

[33mcommit d96c97db363b76df1b9e6bd6aaae79c0bff043c7[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Mon Aug 8 15:13:19 2016 -0700

    Adding test which requires endpoint identity parsing to work

[33mcommit a4d658448b8212e882c7722b259d864a76710695[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Tue Aug 2 13:15:59 2016 -0700

    Fixes deserializing of endpoint identity from message header

[33mcommit d69376091373162f9f031a289b879f8540a73c2b[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Aug 9 09:14:38 2016 -0700

    Omit empty ETW "Begin" and "End" messages if no events to report
    
    The new logic to Console.WriteLine ETW events for tests that fail
    is writing out extraneous empty "Begin" and "End" blocks when run
    on UWP.  This is because UWP doesn't currently support ETW, and it
    has no events to report.
    
    The fix is to skip the entire Console.WriteLine block of code if
    there are no ETW events to report.
    
    Fixes #1465

[33mcommit 7ba43bd1c95e5386ca7b88c5703a69b2f872f661[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Aug 9 08:55:33 2016 -0700

    Adds IssueAttribute for NET Native inadvertantly omitted
    
    When converting all ActiveIssueAttributes to IssueAttribute in
    preparation for UWP testing, one was accidentally omitted. This
    test was supposed to be skipped due to this known issue, not
    executed and counted as a failure.

[33mcommit b4e4ad4f421ca01cfcfdd95f6b6e4890c6292c87[m
Merge: a3b9ab9 9a54163
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Aug 9 07:43:27 2016 -0700

    Merge pull request #1460 from roncain/fix-OSID
    
    Enhance OSID detection when running under NET Native

[33mcommit 9a54163b94638f274bb3c452d3f354f4a5d96d5f[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Aug 9 07:15:57 2016 -0700

    Enhance OSID detection when running under NET Native
    
    RuntimeInformation.OSDescription is hard-coded to return a generic
    "Microsoft Windows" description when compiled for NET Native. This
    masks from our OSID detection which version of Windows is running.
    
    Prior to this fix, we returned "AnyWindows" to indicate we did not
    know, but this caused some IssueAttributes to skip Win7 tests.
    
    The fix is to refine the OSID detection slightly when the hard-coded
    "Microsoft Windows" description is returned.
    
    Fixes #1459

[33mcommit a3b9ab90d4718699d98668ff9997d90528c61222[m
Merge: e03aeac f0eb9d6
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Aug 9 06:31:16 2016 -0700

    Merge pull request #1458 from roncain/fix-uwp-unit
    
    Reactivate unit tests disabled by issue #1449

[33mcommit da38628f2f67db2a91d93e22f3047efe332951ed[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Aug 9 05:25:25 2016 -0700

    Fix UWP issue #1450 and re-enable tests disabled for it.
    
    Issue #1450 was caused in UWP because the MemberData attribute
    was hiding specific Encoding types from the reducer, resulting in
    MissingMetadataException when xunit was doing test discovery.
    
    The fix is to add back a custom rd.xml file to the project containing
    the TestData class to grant Reflection permissions for types affected by this.
    
    Fixes #1450

[33mcommit e03aeac1735429a8acfb2248043e9f9fc5c36d65[m
Merge: 2075651 7821e05
Author: Hong Dai <hongdai@microsoft.com>
Date:   Mon Aug 8 10:06:36 2016 -0700

    Merge pull request #1430 from hongdai/etw1
    
    Emit WCF ETW events whenever a test fail

[33mcommit f0eb9d61f924270ec4c18279a3da9921309738dc[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Aug 8 08:52:38 2016 -0700

    Reactivate unit tests disabled by issue #1449
    
    Some unit tests were disabled when run against UWP due to
    a known issue in the toolchain with InlineDataAttribute(null).
    This PR applies a workaround to that toolchain issue and reactivates
    the tests.

[33mcommit 7821e054103e4bd4a74e5656f3f5fff690ec3185[m
Author: hongdai <hongdai@microsoft.com>
Date:   Fri Aug 5 15:16:07 2016 -0700

    Emit WCF ETW events whenever a test fail
    
    * This is to help analyze test failures, especially on non window
    platforms

[33mcommit 2075651e031c0fb074275f4205ec8108c6b5d9a9[m
Merge: e37096d 6790c15
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Aug 5 13:55:51 2016 -0700

    Merge pull request #1457 from roncain/istestproject
    
    Remove the custom rd.xml's initially required for UWP

[33mcommit 6790c15cc08b8758b2bf275f47328fc89910f381[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Aug 5 12:33:05 2016 -0700

    Remove the custom rd.xml's initially required for UWP
    
    We added custom rd.xml files for some of our test libraries
    for UWP testing because their types were not Reflectable.
    The issue was that build tools considered them framework
    projects because their names did not end with ".Tests.csproj".
    As a consequence, they were signed as framework assemblies,
    and the ILC prevented reflection into framework assemblies.
    
    The fix is to set $(IsTestProject) to true inside the .csproj of
    each of our test libraries.  And to remove the custom rd.xml's.

[33mcommit e37096da74843af040511aeb5100b4c165e5cb57[m
Merge: 0bc3066 918d39e
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Fri Aug 5 09:43:49 2016 -0700

    Merge pull request #1452 from iamjasonp/fix-1398-osx
    
    Remove Issue(1398) from tests requiring cert+OSX

[33mcommit 0bc30663af18d5b27ef4ef0add22517cb20b1930[m
Merge: 27bb672 2096a85
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Aug 5 07:02:27 2016 -0700

    Merge pull request #1435 from roncain/unit-test-wcffact
    
    Change unit tests to use WcfFact

[33mcommit 2096a85c7c015f7f6d73595ef6c9f8ae71d3a610[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Aug 5 06:47:15 2016 -0700

    Merge with #1455 and #1451 to allow running in UWP

[33mcommit 433697a4862c818ad538c48628920da45bc4d1c5[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Aug 3 09:07:49 2016 -0700

    Change some NetTcp unit tests to check PlatformNotSupportedException
    
    The test that formerly was marked with an IssueAttribute because
    it is not supported has been converted to a test that verifies
    we throw PlatformNotSupportedException.  And the IssueAttribute
    was removed.
    
    2 additional tests were added to assert we get the expected behavior
    from the default ctor and when setting MessageCredentialType to None.

[33mcommit 2838ad9d9f5cd42a9bb084a88874e485f5657bef[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Aug 1 13:17:48 2016 -0700

    Change unit tests to use WcfFact
    
    Modifies all the unit tests to use WcfFact instead of Fact
    and WcfTheory instead of Theory.
    
    We retain some conditional code temporarily to allow running
    in ToF as well as normally in GitHub.

[33mcommit 27bb672863076e82794ec4c1f066ec1ee26a88fc[m
Merge: 730fd78 4bb6c25
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Aug 5 06:06:30 2016 -0700

    Merge pull request #1451 from roncain/uwp-wcffact
    
    Move to xunit 2.2.0-prerelease to get UWP testing to work

[33mcommit 4bb6c256684c27c6005e8a8b4b583789aab1c95e[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Aug 5 05:35:14 2016 -0700

    Merge with PR #1455
    
    Merged with the changes to make this run in UWP.

[33mcommit 3ec2d733c759a5630692d9ee172c7ee7c3ea5cbf[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Aug 4 13:44:17 2016 -0700

    Move to xunit 2.2.0-prerelease to get UWP testing to work
    
    Preparing to get UWP testing working in Helix discovered
    WcfFactAttribute and WcfTheoryAttribute were not working
    properly.  One reason was due to mismatched xunit
    versions between the UWP runners and our Infrastructure.Common.
    Moving to the same xunit version allows that part to work.
    
    The second reason was that our custom Fact and Theory attributes
    and their discoverers were not permitted to do Reflection.  The fix
    was to add a custom rd.xml that allows that.  With these 2 changes
    WcfFact and WcfTheory work in UWP in Helix.

[33mcommit 730fd78309581fee12cf2f78465855d4929d1f82[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Aug 4 18:00:55 2016 -0700

    Testing xunit for uwp (#1455)
    
    Update test-runtime to support UAP
    
    With this change we are supposed to be able to run our tests against UWP
    
    * Test xunit attributes on UWP runs
    * All unit tests build and run with tfm netcore50
    * Added ActiveIssues for some failing tests.
    * Added an rd.xml for test type used by tests using the MemberData attribute.

[33mcommit 061723882a88257f2915e23551a791257550d461[m
Merge: 242cba3 85722d4
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Thu Aug 4 16:19:39 2016 -0700

    Merge pull request #1453 from iamjasonp/no-sudo-chmod
    
    Remove call to sudo when chmodding in certtest.props

[33mcommit 85722d400a397b21df18d4e427d22ea3dc9a5280[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Aug 4 15:33:44 2016 -0700

    Remove call to sudo when chmodding in certtest.props
    
    There should be no need to call sudo when attmepting to chmod the InstallRootCertificates.sh
    script. The file is owned by the test execution user, not root

[33mcommit 918d39ed987ca2ce3495246b8f7b8e67cfaba292[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Aug 4 14:51:48 2016 -0700

    Remove Issue(1398) from tests requiring cert+OSX

[33mcommit 242cba3abc8066e99eca9e805c5827403c49eb3c[m
Merge: 725c7fd 589f8ae
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Aug 4 06:43:37 2016 -0700

    Merge pull request #1437 from roncain/run-issues
    
    Add RunTestsWithIssues TestProperty

[33mcommit 589f8ae43bed6b89d709dbe838714b98b2fa6ccb[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Aug 2 10:00:26 2016 -0700

    Add IncludeTestsWithIssues TestProperty
    
    Setting this property to 'true' means that tests marked
    with IssuesAttribute will not be skipped. If the value
    is blank or 'false', all tests marked with Issue are skipped.
    
    This value can also be set to a semicolon-separated list
    of specific issue numbers to include in a test run

[33mcommit 725c7fd0145024d5a6d557bd651150b8e2a10ca7[m
Merge: ba4e8b5 946b73b
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Aug 3 15:16:40 2016 -0700

    Merge pull request #1446 from iamjasonp/update-project-json
    
    Update project.json to take in X509Certificate fix

[33mcommit ba4e8b540c58c9ac6df3aec6b3bdf3e0b6f6ce8f[m
Merge: 90cf5d6 d9663fd
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Wed Aug 3 14:31:29 2016 -0700

    Merge pull request #1442 from iamjasonp/update-netci-outerloop
    
    Update netci.groovy to enforce certificate tests in CI

[33mcommit 946b73bfd6707f150d53af2ac8bceaac76052812[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Wed Aug 3 13:52:23 2016 -0700

    Update project.json to take in X509Certificate fix

[33mcommit 90cf5d6165c46f6bb8b5c61c125c2a737bc1d84c[m
Merge: 81bb9b6 4f40671
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Wed Aug 3 13:47:50 2016 -0700

    Merge pull request #1441 from iamjasonp/rootcert-suse
    
    Modify InstallRootCertificate.sh to support OpenSUSE
    
    Fixes #1401

[33mcommit d9663fd9a512d3e84380a0c7770809a806bce0b0[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Wed Aug 3 11:29:03 2016 -0700

    Add /p:Peer_Certificate_Installed=true to CI build

[33mcommit 81bb9b641cd68f6c86c42521a795b3010e513374[m
Merge: 84d2c27 3ba8445
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Aug 3 05:09:12 2016 -0700

    Merge pull request #1439 from roncain/disable-ws-win7
    
    Use IssueAttribute to disable WebSockets tests on Win7

[33mcommit 84d2c27e6182f1d5b239471ada5286114574ca1c[m
Merge: bfef302 0241f42
Author: Zhenlan Wang <zhenlwa@microsoft.com>
Date:   Tue Aug 2 20:10:48 2016 -0700

    Merge pull request #1436 from zhenlan/path-param
    
    Add support to use existing repo for IIS hosted WCF test service setup

[33mcommit 0241f42d6a57687b43820bd0fdf462e4d4be0e50[m
Author: Zhenlan Wang <zhenlwa@microsoft.com>
Date:   Tue Aug 2 09:51:17 2016 -0700

    Add support to use existing repo for IIS hosted WCF test service setup
    
    Usage:
       /p: If an existing WCF repo is preferred to be used, use this parameter to provide the
           path to a WCF repo. Otherwise, the script will clone a new WCF repo at c:\git
           to use as the source of WCF test service.
    
    Examples:
       SetupWcfIISHostedService 42 /p:"c:\my\existing\wcf\repo"
       :Create an IIS hosted service named 'wcfService42' by using existing WCF repo located at "c:\my\existing\wcf\repo".

[33mcommit 20d97954ba91a44f2cd4736bf21ddc6c4c8ccda9[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Tue Aug 2 17:32:13 2016 -0700

    Update netci.groovy to enforce certificate tests in CI
    
    Add switches
    
    /p:SSL_Available=true /p:Root_Certificate_Installed=true
    /p:Client_Certificate_installed=true
    
    to CI so tests will always run certificate tests

[33mcommit 4f40671bd82dfeaa72e2cd41cc8f6ca96c9225e3[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Tue Aug 2 15:30:58 2016 -0700

    Modify InstallRootCertificate.sh to support OpenSUSE

[33mcommit 3ba84455650325921e4778b4d43a992d508d7dee[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Aug 2 13:19:14 2016 -0700

    Use IssueAttribute to disable WebSockets tests on Win7
    
    The Windows platforms we currently test against include Win7,
    but the WebSockets protocol is not supported on that OS.
    As a result, VSO runs report WebSockets failures on the Win7
    runs, forcing us to investigate each time.
    
    With this change, those tests will be skipped on Win7, and any
    VSO failures reported will be legitimate issues to investigate.
    This new IssueAttribute will not disable these tests anywhere else.

[33mcommit bfef302ab4b83153bf2ae47489e7319eed22ff04[m
Merge: 0d2c562 4a4ea07
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Aug 2 11:13:30 2016 -0700

    Merge pull request #1433 from roncain/fix-wcftheory
    
    Fix WcfTheory discoverer to use theory data

[33mcommit 0d2c562089f1a4ec4a20ca7e68bcdfd1e2a89161[m
Merge: e2a8290 ee92774
Author: Hong Dai <hongdai@microsoft.com>
Date:   Tue Aug 2 09:50:55 2016 -0700

    Merge pull request #1431 from hongdai/addExePermission
    
    Add execution permission to the cert installation file

[33mcommit 4a4ea07c1d470fcb3bd598ba628d1ca0a1982fd7[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Aug 2 05:55:57 2016 -0700

    Fix WcfTheory discoverer to use theory data
    
    The WcfTheoryAttribute test discover was creating an xunit
    test runner that worked only for Fact, but not for Theory.
    
    The solution was to refactor slightly and to create a
    theory-based xunit test runner for non-skipped tests.

[33mcommit e2a82905688e0e9e526fd5ef08a3ae02ae4f9045[m
Merge: c898c77 630cac7
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Aug 2 05:05:52 2016 -0700

    Merge pull request #1429 from roncain/move-to-wcffact
    
    Replace Fact, ConditionalFact and ActiveIssue

[33mcommit ee92774656bdb5b4b2cbafd14c8fb4460d1f4b7f[m
Author: hongdai <hongdai@microsoft.com>
Date:   Mon Aug 1 16:41:00 2016 -0700

    Add execution permission to the cert installation file
    
    * I'm getting "Command Not Found" error without this fix.
    * After this fix, cert tests are passing in Helix for Ubuntu

[33mcommit c898c77c690b80efab8ad251ed970541c6c07a21[m
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Mon Aug 1 13:25:34 2016 -0700

    Add better checks for detecting detached HEAD in PRService (#1406)
    
    Add better checks for detecting detached HEAD in PRService
    
    Without the check for a deteached HEAD state, we would attempt to delete some
    bogus branches. Now we're a little better about it and so we should see the
    PRService fail less often
    
    P.S. the algorithm would probably not pass if asked in an interview. Please do
         not copy for such purposes

[33mcommit 630cac721238e21c1d285d393f6cd2e018ee73de[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Aug 1 13:03:33 2016 -0700

    Add ActiveIssue back to Tof conditional code
    
    Put back the unconditional ActiveIssue attributes in the
    ToF conditional code to suppress those tests in N-on-K runs.

[33mcommit 3262f967bea488b453ee3c7b8eee1949de00a375[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Aug 1 11:44:32 2016 -0700

    Replace Fact, ConditionalFact and ActiveIssue
    
    Replace all FactAttribute usages with WcfFactAttribute
    Replace all ConditionalFactAttribute with ConditionAttribute
    Replace all ActiveIssueAttribute with IssueAttribute
    
    The exception to this is in the conditional code that compiles
    only under ToF, because the pseudo xunit does not support the
    xunit types they require.  This will eventually be removed.

[33mcommit 74a4e37be150758fe5444b103251136430daefdf[m
Merge: 2c00fa4 015ad3a
Author: Matt Connew <mconnew@microsoft.com>
Date:   Mon Aug 1 11:01:51 2016 -0700

    Merge pull request #1428 from mconnew/Issue1383
    
    Fix exception thrown in .Net native when a certificate has been revoked
    
    Fixes #1383

[33mcommit 2c00fa43466a9ce546dd3f959a3a2a873e8f979d[m
Merge: 4b7e89b 64a2582
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Aug 1 05:14:43 2016 -0700

    Merge pull request #1407 from roncain/detect-windows
    
    Improve detection of Windows OS

[33mcommit 015ad3ac1b00ce7a809c210dcee5c05b71013495[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Fri Jul 29 20:22:45 2016 -0700

    Fix exception thrown in .Net native when a certificate has been revoked
    
    Some certificate errors were still being handled by SocketStream. This change
    causes SocketStream to ignore more certificate validation errors and allows
    the WCF managed validation to be run.

[33mcommit 4b7e89be1f19952ed89f92bc6acbd311b2f94883[m
Merge: f1f1789 17e5f76
Author: Matt Connew <mconnew@microsoft.com>
Date:   Fri Jul 29 14:09:54 2016 -0700

    Merge pull request #1409 from mconnew/FixWebSocketStreamingTests
    
    Make WS tests concurrent safe

[33mcommit 17e5f7656a0634975a06a61e6ee378111a8c45f4[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Fri Jul 29 12:30:43 2016 -0700

    Make WS tests concurrent safe

[33mcommit f1f1789190da787531b493fac252f0619e63e6ff[m
Merge: 67318b4 e64e51a
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Jul 29 11:08:07 2016 -0700

    Merge pull request #1408 from StephenBonikowsky/master
    
    Update WCF packages to beta-24329-01

[33mcommit e64e51aae7b4c6646a0042953a0e579672f34905[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Jul 29 10:26:28 2016 -0700

    Update WCF packages to beta-24329-01

[33mcommit 64a258236a74e5fba1870f84c6ee36890d9bef87[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Jul 29 10:15:17 2016 -0700

    Add more OSID enum values for Windows
    
    Based on [this link](https://msdn.microsoft.com/en-us/library/windows/desktop/ms724832(v=vs.85).aspx)
    I've increased the Windows OSID enums to include a few more known
    versions of Windows.

[33mcommit 67318b429fa3f0ebb486bcc750cfc6f6bbc99f3d[m
Merge: aab13b7 38ebc5f
Author: Hong Dai <hongdai@microsoft.com>
Date:   Fri Jul 29 09:38:55 2016 -0700

    Merge pull request #1405 from hongdai/certauto1
    
    Change to use Content copy installRootCertificate

[33mcommit f1908526421b7caafaf7227ee92ca6f9d258776c[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Jul 29 08:27:34 2016 -0700

    Improve detection of Windows OS
    
    This PR improves the granularity of the detection of which
    version of Windows is currently running. It also moves the
    infrastructure tests for detection into a new file that uses
    FactAttribute to ensure they run on NET Native.

[33mcommit 38ebc5f55112926ee76231e60d7b57cab4031561[m
Author: hongdai <hongdai@microsoft.com>
Date:   Thu Jul 28 17:17:06 2016 -0700

    Change to use Content copy installRootCertificate
    
    * Move the file copy to certtest.props
    * Change from taget to Content Copy for Helix to work

[33mcommit aab13b713768e06a29936cd1d8e32e4119c875fb[m
Merge: 9d07e70 c1a263f
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Jul 28 13:06:53 2016 -0700

    Merge pull request #1404 from roncain/put-back-json
    
    Temporarily put back project.json files to unblock VSO

[33mcommit c1a263ff10a2086b8652abaa80593478116a2eb3[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Jul 28 09:57:37 2016 -0700

    Temporarily put back project.json files to unblock VSO

[33mcommit 9d07e70028378f15514012a220f8986a653328c7[m
Merge: 85a0bb9 38bf8f3
Author: Hong Dai <hongdai@microsoft.com>
Date:   Wed Jul 27 20:46:55 2016 -0700

    Merge pull request #1389 from hongdai/certauto
    
    Automate cert installation

[33mcommit 38bf8f325178414cad7887b975a62acee90998d3[m
Author: hongdai <hongdai@microsoft.com>
Date:   Wed Jul 27 15:49:50 2016 -0700

    Revert "This change will not merge"
    
    This reverts commit 78d0303c679a0fd155e55d897621ad3247702702.
    
    Revert change to SSL_AVAILABLE

[33mcommit a47b560132d11d4577df742e50e6d7db9ba4f232[m
Author: hongdai <hongdai@microsoft.com>
Date:   Wed Jul 27 13:47:27 2016 -0700

    Call installation script only needed and exclude OSX cert tests
    
    * This is to reduce the noise in the log
    * exclude cert tests on OSX as installation script does not work on OSX yet.

[33mcommit d52970fd519e60375b8916381e999c2fa24ffafb[m
Author: hongdai <hongdai@microsoft.com>
Date:   Sat Jul 23 09:37:56 2016 -0700

    Exclude tests need A libcurl built with OpenSSL is required
    
    * Client and Server cert custom validation tests need
    A libcurl built with OpenSSL for some Linux OSs.
    We have to disable the tests for all linux OSs because we don't
    have a way disable per OS.

[33mcommit df0113c9a2e896adbcafc6cd116e3fffc4d3b963[m
Author: hongdai <hongdai@microsoft.com>
Date:   Fri Jul 22 17:22:51 2016 -0700

    Use /tmp folder instead of ~/tmp

[33mcommit 78d0303c679a0fd155e55d897621ad3247702702[m
Author: hongdai <hongdai@microsoft.com>
Date:   Fri Jul 22 14:39:02 2016 -0700

    This change will not merge
    
    * Enable SSL available so that I can test cert installation
    in CI on non windows machine

[33mcommit e7d9824441e644961c355b3992b2ba8366948a58[m
Author: hongdai <hongdai@microsoft.com>
Date:   Fri Jul 22 13:41:22 2016 -0700

    Enable cert installation
    
    * copy cert installation script to the test output
    * we now have a testcommandlines can be used to call the installation
    script before tests get running.

[33mcommit 85a0bb9f3f7ea39101261ea740161c12fd0eb839[m
Merge: fa9874f fc627e6
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Jul 27 13:44:16 2016 -0700

    Merge pull request #1399 from StephenBonikowsky/ConsolidateScenarioJsonFiles
    
    Use one single master json file for all scenario test projects.

[33mcommit fa9874f02abe5043dc9ea6eae6d1ad9e0e4470d2[m
Merge: 93bf4d5 c96877a
Author: Matt Connew <mconnew@microsoft.com>
Date:   Wed Jul 27 13:22:57 2016 -0700

    Merge pull request #1371 from mconnew/Issue1111
    
    Enable basic auth tests with domainless custom authenticator
    Fixes #1111

[33mcommit 93bf4d51826c932e3d6e01e6a8b7f3bb6f70f0a7[m
Merge: 3138853 6258983
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Jul 27 13:11:36 2016 -0700

    Merge pull request #1388 from roncain/wcffact
    
    Add new [Fact] and [Theory] attributes to generalize ConditionalFact

[33mcommit fc627e6820a2f11341e4696fb9f38fe0bc5f1a07[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Jul 26 15:26:14 2016 -0700

    Use one single master json file for all scenario test projects.
    
    * Delete all the project.json files under Scenarios.
    * One 'master' json file added that will be copied to each scenario test dir at the beginning of the build.
    * Running the clean script will remove these json files and they have been added to the gitignore file.
    * The target that does the work has a condition based on a property's bool value, this is to support the next PR where we will have another target that will copy a different json file when that property is set otherwise.

[33mcommit 625898305d5acf5f06edabdfeac5fd8af1dcdaa5[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Jul 22 08:11:01 2016 -0700

    Add new [WcfFact] and [WcfTheory] attributes to generalize ConditionalFact
    
    This PR adds 2 new attributes and discoverers to take the place of
    ConditionalFact and ConditionalTheory.  The new discoverers have
    been generalized to reflect onto the test method for any of the new
    "skip" attributes (IssueAttribute and ConditionAttribute).
    
    It also adds enums for OSID and FrameworkID to identify the
    current OS and Framework, and it adds optional parameters to
    the new IssueAttribute to allow it to be applied only to specific
    OSes or Frameworks.
    
    For design discussion on this see issue #1034

[33mcommit 3138853fb5c431eab96e64132c2e406340f6febf[m
Merge: ce71fcf 6206ff5
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Tue Jul 26 17:16:28 2016 -0700

    Merge pull request #1400 from iamjasonp/netci-more-platforms
    
    Add new platforms for CI testing

[33mcommit 6206ff5b99fb196dcd72335f5676e02141723ba1[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Tue Jul 26 16:39:41 2016 -0700

    Add new platforms for CI testing
    
    Add new platforms for CI testing, e.g. other versions of Ubuntu, RedHat, etc.
    
    Defaults still to run only 'Windows_NT', 'Ubuntu14.04', 'CentOS7.1', 'OSX' every
    PR, and allows one to call:
    
    * "test all outerloop please" - to test all outerloops
    * "test all innerloop please" - to test all innerloops
    * "test {inner|outer}loop {os} please" - to test a particular innerloop

[33mcommit ce71fcfa4a80b569b0c519d99447c6514bad8d62[m
Merge: fb32131 cff543a
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Jul 26 14:57:01 2016 -0700

    Merge pull request #1394 from StephenBonikowsky/CleanMessageInterceptorTest
    
    Clean-up MessageInterceptor test.

[33mcommit c96877aa84370bed098a1cf6c7cf6f1552c4eaaa[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Mon Jul 18 17:31:54 2016 -0700

    Enable basic auth tests with domainless custom authenticator

[33mcommit fb32131d3f88609532af1bd223014d38d7452135[m
Merge: 05b0500 60f8ddb
Author: Matt Connew <mconnew@microsoft.com>
Date:   Tue Jul 26 13:01:23 2016 -0700

    Merge pull request #1396 from mconnew/UnixWebSockets
    
    Use WebSockets pkg version supported on unix and enable tests
    
    Fixes #625
    Fixes #420

[33mcommit 05b0500123330d8f0104e89d8b570d64166d62f6[m
Merge: 9b0ece1 c44f927
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Jul 26 12:01:24 2016 -0700

    Merge pull request #1397 from roncain/testruntime
    
    Add TestNugetRuntimeId to TestProperties

[33mcommit c44f927649992b1143f72cbb1bb22d8d39151f8b[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Jul 26 08:05:41 2016 -0700

    Add TestNugetRuntimeId to TestProperties
    
    This change adds a new entry to TestProperties to capture the
    MsBuild property $(TestNugetRuntimeId).  This property is set
    by Helix runs to indicate the package name for the runtime to
    use when running the tests.  The build tools also already set
    this property, so it is correct during the dev experience too.
    
    This provides the extra necessary information for PR #1388 to be
    able to provide more accurate values for the OS enum. For example,
    we will be able to distintuish Ubuntu 14.04 from 16.04 at test
    runtime based on this new TestProperty.
    
    The existing RuntimeInformation support does not provide this
    granularity explicitly and would require fragile string matching to
    try to extract from the RuntimeInformation's OS description alone.

[33mcommit 60f8ddba741d9331fc0ea7ac793ce3aee8adf87f[m
Author: Matt <mconnew@microsoft.com>
Date:   Mon Jul 25 14:14:58 2016 -0700

    Use WebSockets ppkg version supported on unix and enable tests

[33mcommit 9b0ece1b2bf4c789f2bd0ffee4ce808db289ad76[m
Merge: ef1b276 1caf5e0
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Mon Jul 25 09:36:52 2016 -0700

    Merge pull request #1390 from iamjasonp/fixup-rootcertinstaller
    
    Fix certificate generation and installation issues

[33mcommit cff543af5c893c84a2b79fcf6b04ee700b4b91ed[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Jul 25 08:48:57 2016 -0700

    Clean-up MessageInterceptor test.
    
    * Removing server side Console logging.

[33mcommit ef1b2764a29ffccce525d5d212e31250eb439cb8[m
Merge: f8c1810 46dfd1b
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Jul 22 16:25:35 2016 -0700

    Merge pull request #1391 from StephenBonikowsky/UpdateSharedScripts_7-22
    
    Update shared scripts and pre-release pkg versions.

[33mcommit 46dfd1b1a3ae766767c483dcbb7844112c017443[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Jul 22 15:53:11 2016 -0700

    Update shared scripts and pre-release pkg versions.
    
    * Synccing shared files between WCF and CoreFx, most of the changes are part of the following CoreFx PRs...
       https://github.com/dotnet/corefx/pull/10045
       https://github.com/dotnet/corefx/pull/9971
       https://github.com/dotnet/corefx/pull/10221
       https://github.com/dotnet/corefx/pull/10016
       https://github.com/dotnet/corefx/pull/9974

[33mcommit 1caf5e079ca64312e6ed779d2133eff26f087e4a[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Fri Jul 22 14:14:27 2016 -0700

    Modify TestHost.svc utility class
    
    * Specify a filename for the downloaded certificate files (useful for download
      as we get a default filename)
    * Remove Crl "serialNum" paramater as it's no longer used
    * Add MachineCert endpoint for retrieving machine certificates
      (useful for debugging purposes, e.g., when attempting to validate a certificate
      on a machine)

[33mcommit 19f55c6327ceba88131bc297ac8b17b5f2178ad5[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Fri Jul 22 14:07:29 2016 -0700

    Modify InstallRootCertificate.sh to output results of cURL

[33mcommit 801b314be2194e94fffcc273680a3d237d330a0b[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Fri Jul 22 14:05:15 2016 -0700

    Change CertificateGenerator certificate issuer name
    
    Change the CertificateGenerator issuer name to include the date/time of generation
    to work around a CRL caching issue that was preventing testing in Linux from
    succeeding when certs are generated, the Issuer Name hasn't changed, and the CRL
    Next Update time is too long

[33mcommit 76b005599e4932a971159889f0074cbdf0a92d43[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Mon Jul 11 15:21:25 2016 -0700

    Fixup InstallRotCertificate.sh hitting old endpoint

[33mcommit f8c1810ffb30d7916fa5ad5e6c7c1005f0be2b31[m
Merge: 1409482 00c4201
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Jul 21 09:23:57 2016 -0700

    Merge pull request #1385 from StephenBonikowsky/UpdateTestProjReferences
    
    Update Scenario test projects with .pkgproj references.

[33mcommit 14094822f9d2e834b261413f1deb58bd2040e476[m
Merge: 76b5961 36a018c
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Jul 21 08:56:13 2016 -0700

    Merge pull request #1382 from roncain/improve-peertrust
    
    Improve PeerTrust scenario tests

[33mcommit 36a018c5cffc228df7dbb3e5b382cecbedd87b00[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Jul 21 05:29:01 2016 -0700

    Rename PeerOrChainTrust endpoint to ChainTrust to clarify purpose.

[33mcommit 00c42010b281c14c07bf8f6467521173c9fcef60[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Jul 20 11:02:24 2016 -0700

    Update Scenario test projects with .pkgproj references.
    
    * Also clean-up all test project.json files to minimal set and to use last released version of non-WCF dependencies.

[33mcommit 2419e83dec83ce677856e14981eba651318367a4[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Jul 19 06:29:52 2016 -0700

    Improve PeerTrust scenario tests
    
    This adds a new certificate that can be used for PeerTrust,
    new test endpoints that use it, and new scenario tests that
    test PeerTrust, PeerOrChainTrust and ChainTrust.

[33mcommit 76b59615da09da541621019cfe4107f8caf704da[m
Merge: 9f5be20 505b870
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Jul 19 15:55:06 2016 -0700

    Merge pull request #1377 from StephenBonikowsky/UpdateTestProjReferences
    
    Update the Facade test projects to use .pkgproj references.

[33mcommit 505b8700b363fa81455ae3a69dc5dded2bba3020[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Jul 18 17:35:32 2016 -0700

    Update the Facade test projects to use .pkgproj references.
    
    * Facade test.csproj files now use .pkgproj references.
    * Facade project.json files have been cleaned-up.
    * UnitTests.Common.csproj and project.json files have been cleaned-up.
    * Wherever dependent packages need to be referenced use the last stable released version.
    
    * ToDo: Add a run in VSO to run tests against latest-latest (latest dependencies/latest WCF).
    * ToDo: Next PR to make same changes to Scenario test projects and related common code.

[33mcommit 9f5be203ad4755af9e5c89e3e8689282e1c5bafd[m
Merge: 49f24be 000610b
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Jul 18 15:09:53 2016 -0700

    Merge pull request #1376 from StephenBonikowsky/UpdateBuildToolsVersionTo618-01
    
    Updating to latest build tools version.

[33mcommit 000610b73ac8bfeba3851d8f9acdcf79523e1ac7[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Jul 18 12:52:00 2016 -0700

    Updating to latest build tools version.
    
    * Needed to fix VSO issues.

[33mcommit 49f24be7a3ad6e171f1adb0e35f779eccc053f16[m
Merge: cf60fa5 d7117ba
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Jul 15 17:07:10 2016 -0700

    Merge pull request #1375 from StephenBonikowsky/UpdateBuildToolsTo615-07
    
    Update BuildTools to version 00615-07

[33mcommit cf60fa51968894421056a0855a230ac5344c7ae6[m
Merge: 67dc590 229d11a
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Jul 15 16:52:56 2016 -0700

    Merge pull request #1366 from StephenBonikowsky/RenameTestFilesWithReleaseVersion
    
    Rename test files with release version

[33mcommit d7117ba876de81c6820e9cc6b95c75b93eb979c2[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Jul 15 16:33:23 2016 -0700

    Update BuildTools to version 00615-07
    
    * Needed for VSO builds.

[33mcommit 229d11a3dc445a81f337645ddd2c95ca21b8da5f[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Jul 15 13:31:58 2016 -0700

    Move tests to pre-release test files.
    
    * ServiceIdentityNotMatch_Throw_MessageSecurityException and TCP_ServiceCertFailedCustomValidate_Throw_Exception were both failing on netnative because the expected exception was different. This was fixed in PR #1337 therefore these tests need to be moved into current pre-release test files.
    * Tests under Scenarios/Contract/Message and Scenarios/Extensibility/MessageInterceptor needed supporting common code files located in their same directory to also be renamed so these files would also be picked up by the test run logic used to validate these tests against the last released libraries using the latest tool-chain.

[33mcommit df6eb3a178c519e356fafb1f9ae994456b2b4805[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Jul 12 16:55:34 2016 -0700

    Move PeerTrust tests into pre-release test files.
    
    * These tests can't work in released S.P.SM version 4.1.0 as needed product code has only been added since then, moving them into a new pre-release test file.

[33mcommit 6f9c3f19c0b04ceeb684f5c172988ba3f4ba2756[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Jul 12 16:17:48 2016 -0700

    Rename scenario test files to reflect most recent released version of S.P.SM
    
    - Appended 4.1.0 to the names of the test files.

[33mcommit 67dc59007d0f656a8dcaa4dfe172c1c9bd53a80f[m
Merge: ef494d8 afd6f41
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Jul 15 13:11:57 2016 -0700

    Merge pull request #1373 from StephenBonikowsky/SetDefaultFilterToTestTFM
    
    Adding default value for FilterToTestTFM property used in VSO runs.

[33mcommit afd6f41aedada524331848e56cabbe9f795668ef[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Jul 15 11:31:35 2016 -0700

    Adding default value for FilterToTestTFM property now being used in VSO runs.
    
    * Essentially a port of Karthik's CoreFx PR #10042

[33mcommit ef494d8a09693aeb813a3765ce2252591b5444a0[m
Merge: ea2f356 1d49321
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Fri Jul 15 10:16:27 2016 -0700

    Merge pull request #1372 from iamjasonp/installRootCert-sh
    
    Fixup InstallRootCertificate.sh hitting old endpoint

[33mcommit 1d4932173e4d7e7acca9bbca7a573ccf5e2409fa[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Mon Jul 11 15:21:25 2016 -0700

    Fixup InstallRootCertificate.sh to hit new endpoint
    
    1. We were masking errors by redirecting the result of cURL to /dev/null. I have
       removed the redirection
    2. The endpoint GetRootCertificate was changed to RootCert
    3. InstallRootCertificate.sh was edited in Windows, and so it needs to be
       chmodded to be executable

[33mcommit ea2f3565da9c0cd5b8b483aaf465de18507bb9bc[m
Merge: e15af19 38b5173
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Jul 14 07:58:33 2016 -0700

    Merge pull request #1369 from dotnet-bot/from-tfs
    
    Merge changes from TFS

[33mcommit 38b517377edfc6e28265d0247cffde63aaec0ec2[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Jul 13 16:22:09 2016 -0700

    Add Support For Xamarin and Mono for WCF Facades.
    
    [tfs-changeset: 1617567]

[33mcommit e15af190b5dbd94e3454cff036104dae17b69bce[m
Merge: 3b98af6 cf9ef23
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Jul 11 13:18:16 2016 -0700

    Merge pull request #1362 from StephenBonikowsky/UpdateSharedScriptsJuly11
    
    Latest CoreFx changes to various shared scripts.

[33mcommit cf9ef23030fa7d5f5afdd34af11e5aa1ca2d0f80[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Jul 11 11:49:54 2016 -0700

    Latest CoreFx changes to various shared scripts.
    
    * Covers numerous small changes.

[33mcommit 3b98af6028ddab4bc7e2dbed3518f8475c6f95b0[m
Merge: 0028173 5f6c6fc
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Jul 11 11:40:10 2016 -0700

    Merge pull request #1361 from roncain/fix-peertrust
    
    Fixes infrastructure issue with acquiring peer trust certificate

[33mcommit 5f6c6fcdaa186254160c1ef4ae48689606f809b6[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Jul 11 09:19:44 2016 -0700

    Fixes infrastructure issue with acquiring peer trust certificate
    
    Changes the TestHost REST service to download the peer trust
    certificate properly.
    
    Modifies the ping script to avoid writing a temp.crl file
    each time.
    
    Adds some console.writelines when accessing the TestHost
    REST service to aid debugging.
    
    Fixes #1360

[33mcommit 002817346f34206be000649463cf5c9571ff1d54[m
Merge: 859d0e8 9ad140c
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Fri Jul 8 17:43:22 2016 -0700

    Merge pull request #1358 from StephenBonikowsky/UpdatePkgsTo24308-02
    
    Update to version 24308-02

[33mcommit 9ad140c502985019d76778ae8242a5a9b03dedf9[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Jul 8 16:30:53 2016 -0700

    Update to version 24308-02

[33mcommit 859d0e88d837dd13a56c75d2cef0047948076069[m
Merge: 339ceff 6397957
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Fri Jul 8 15:15:55 2016 -0700

    Merge pull request #1352 from iamjasonp/root-certificate-refactor-refactor
    
    Change test infrastructure to use REST endpoints

[33mcommit 6397957209304a672864898399af793092ef67f3[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Fri Jul 8 08:48:14 2016 +0800

    Change UtilTestWebServiceHost to TestHostWebServiceHost

[33mcommit d3b93d7731156ad158ea1fc2ff1d5e8ac91b6f2e[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Fri Jul 8 08:40:03 2016 +0800

    Modify test clients to point to new REST endpoint

[33mcommit 73a5f31ce0b80c3d5d2b509c8b1176bdb0d3e533[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Fri Jul 8 05:29:28 2016 +0800

    Rename Util class to TestHost

[33mcommit 2b77c494004ac33ec89daf2587209c723019e084[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Jul 7 09:19:34 2016 +0800

    Remove TestRootCertificateInstaller
    
    We no longer need this tool; simply cURL to the correct URL and we will be able
    to acquire the certificate from the server

[33mcommit 5c36d8a9d0e771b48e469ab1197a514987722ff4[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Jul 7 08:29:53 2016 +0800

    Change test certificate acquisition to REST endpoint
    
    The certificate acquisition endpoint currently used requires a WCF client.
    This causes some complications when our scripts need to obtain certificates,
    as we need to spin up a WCF client capable of understanding a WCF response.
    
    This change moves certificate acquisition to come from a REST endpoint instead
    so we can (for example) use cURL to acquire the certificate instead of using
    a complicated workaround like the TestRootCertificateInstaller

[33mcommit 339ceff89408cc17cae6d10e3953b887e5f7b7f1[m
Merge: 3a6ec4f 915159a
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Jul 8 13:25:47 2016 -0700

    Merge pull request #1351 from mconnew/Issue1307
    
    Fix issue with ProducerConsumerStream/MessageContent not completing after entire request has been sent

[33mcommit 3a6ec4fe47ea2e11be6c84f3d538a178f7769dd0[m
Merge: 44fda5c f5adad6
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Jul 8 12:53:03 2016 -0700

    Merge pull request #1355 from roncain/fix-sln
    
    Update main SLN for new infrastructure test project location

[33mcommit 44fda5c02e8e3784c2074e93a4034cdcf310341c[m
Merge: 9ccd101 4d9c446
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Jul 8 12:52:39 2016 -0700

    Merge pull request #1357 from roncain/fix-conditionalfact
    
    Add explicit WCF dependencies to test-runtime project.json

[33mcommit 4d9c446a15535e9624824a9bace5aa2021de8e9f[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Jul 8 12:09:23 2016 -0700

    Add explicit WCF dependencies to test-runtime project.json
    
    The ConditionalFact detectors were experiencing failure to load
    assemblies when tests were not built with the same dependencies
    as those used by the ConditionalFact infrastructure.
    
    This change adds to the shared test-runtime project.json those
    entries that ensure tests use the same packages.
    
    Fixes #1353

[33mcommit 915159a6966257347d7656e8cf2e17a10ecb2172[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Thu Jul 7 20:36:34 2016 -0700

    Fix issue with ProducerConsumerStream/MessageContent not completing after entire request has been sent

[33mcommit f5adad68321261a72f55fbad369841ce370ce8df[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Jul 8 05:12:02 2016 -0700

    Update main SLN for new infrastructure test project location
    
    The Infrastructure test project was removed, and the source project
    was moved in a prior PR.  This PR contains only the fix ups needed
    to the main SLN file so the project can be used in VS again.

[33mcommit 9ccd10164df727ab2e174dd906a2cb51ff1c03dd[m
Merge: a9e1681 8cf608c
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Jul 7 13:56:30 2016 -0700

    Merge pull request #1346 from roncain/peertrust
    
    Peertrust

[33mcommit 8cf608cbfb123a34e226909bcb0e8556367cafe5[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Jul 7 13:27:32 2016 -0700

    Enable X509CertificateValidationMode PeerTrust
    
    Add back to the product the X509CertificateValidator support for PeerTrust.
     This code had been removed on the initial port from the full framework.
    
    This also requires new test support to fetch the peer trust certificate
     and install it into the client's TrustedPeople store so that the PeerTrust validation
     logic can find it.
    
    This also adds a new ConditionalFact to indicate that the peer trust certificate
     has been successfully installed into the TrustedPeople store.
    
    Fixes #1183

[33mcommit a9e16817e8ff5298e2f6e40d62df9cf71c1139fa[m
Merge: 1cc3338 841b1bd
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Jul 7 11:42:17 2016 -0700

    Merge pull request #1349 from StephenBonikowsky/CompileTestsNetStandard1.3v2
    
    Compile tests net standard1.3v2

[33mcommit 841b1bd852a5fb426f9ee2037647c6ebdf7f2487[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Jul 7 11:13:23 2016 -0700

    Updating to buildtools version 530-02 and additional changes.
    
    * Updating to the latest buildtools version.
    * Checked the merge history for each buildtools version update in corefx and also pulled in updates to any of the shared infrastructure files that were touched.

[33mcommit 77d5f50834af1de5f197d502bd5a1b9e8d3e87e5[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Jul 6 12:59:43 2016 -0700

    Fixing invalid comments in project.json files.

[33mcommit 30b9746c9c8c1997e55b13afec35fb079c2469c0[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Jul 6 11:22:24 2016 -0700

    Updating sh files per corefx PR #9469
    
    * CoreFx pr by @joperezr

[33mcommit 3ea4ed2d8be6d99da8ede92f4c4ee1f25be3d7fa[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Jul 5 15:06:50 2016 -0700

    Port of CoreFx PR #9129
    
    * Various 'shared files' have been updated with changes necessary to support this new way of determining what tests projects compile against and what TFM it runs on.
    * All test project.json files have been updated with the needed changes to support the new system.
    * All WCF 'Scenario' test projects have NOT been updated to reference the WCF Contract package projects. They continue to list a pre-release package of each needed WCF Contract in the their respective project.json files, then after the project is compiled the test .csproj file overwrites to the output directory the locally built System.Private.ServiceModel assembly which is what is then used for the test runtime.
    * All WCF 'Unit' test projects were updated to reference the WCF Contract package projects they needed, they also needed a reference to the System.Private.ServiceModel.pkgproj. In addition for 3 of the 5 test projects references to additional CoreFx package dependencies were explicitly added to the project.json files since transitive dependencies are not being pulled in via the package project references in the test projects.
    * That transitive dependencies are not being pulled in via the package references is why the Scenario test projects were not updated to reference package projects, it would then have required explicitly adding every required dependency to all the test projects which is counter to the whole point of using package references in the first place.
    * I will create a new Issue for this transitive dependencies problem as soon as I know the correct repo to do it in.

[33mcommit dec0bf60934e4f8658a0dbc8708c4eef4b86e0d0[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Jul 5 14:23:40 2016 -0700

    Removing tests of infrastructure code.
    
    * These tests are not really useful after moving to our new test infrastructure.

[33mcommit 1cc3338018a6865a52957b71df10cdbbcd38afd7[m
Merge: 6881f4f 2db4348
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Jul 5 04:46:16 2016 -0700

    Merge pull request #1348 from roncain/fix-digest-test
    
    Fix Digest authentication test that should not run on NET Native

[33mcommit 2db43482c703fc4f2d167f97fd6187d276d0ca76[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Jul 1 09:52:21 2016 -0700

    Fix Digest authentication test that should not run on NET Native
    
    Adds some NET Native versions of ConditionalFact checking to prevent
    this test from running under NET Native.

[33mcommit 6881f4f34ee0c3e05ba87fa07e7c07d652c9e312[m
Merge: 9cc1355 be94267
Author: Matt Connew <mconnew@microsoft.com>
Date:   Thu Jun 30 17:33:08 2016 -0700

    Merge pull request #1342 from mconnew/Issue1332
    
    Modify digest authenticator to accept realm\\username username

[33mcommit be942675b0a130eb2f6bdca2ae3e93b7900d57bc[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Tue Jun 28 17:58:02 2016 -0700

    Modify digest authenticator to accept realm\\username or realm\username username

[33mcommit 9cc1355c22384c32f26e2eb33b0ff1a69cbd74dc[m
Merge: 16358f6 b141f1a
Author: Hong Dai <hongdai@microsoft.com>
Date:   Wed Jun 29 21:56:51 2016 -0700

    Merge pull request #1334 from hongdai/releasenote1
    
    Update feature table

[33mcommit 16358f618c81fa2b2c8ba231d373a1418a53b066[m
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Wed Jun 29 18:23:01 2016 -0700

    Branchify repo CI queues (#1338)
    
    * Branchify repo CI queues
    * Workaround for WCF PRService serial numbers after branchifying

[33mcommit b141f1a3c82650aa9c659513e497708315780d81[m
Author: hongdai <hongdai@microsoft.com>
Date:   Wed Jun 29 15:36:51 2016 -0700

    Update feature table
    * Update known issue link to the WCF release note.
    * Simplify SPN and UPN columns. The detail has been added to the acutal issue.

[33mcommit 1d5390f0e802b748500eec44bfd025eedf619ff2[m
Merge: 6b6d86c f1b5ed3
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Jun 29 11:42:19 2016 -0700

    Merge pull request #1337 from roncain/fix-983
    
    Don't wrap all exceptions in SSLStreamSecurityUpgradeProvider

[33mcommit f1b5ed3b54ff47742ebbbef87d1d5560f7b70135[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Jun 27 13:09:50 2016 -0700

    Don't wrap all exceptions in SSLStreamSecurityUpgradeProvider
    
    Conditional code for NET Native was catching all exception types
    thrown when initiating and upgrade and and wrapping them in
    SecurityNegotiationException. This caused a behavioral change
    from the CoreCLR version, where the exceptions thrown by custom
    validators were converted into SecurityNegotiationExceptions.
    
    The fix is to not wrap Exceptions inside SecurityNegotiationException
    in NET Native unless we detect a non-default HRESULT.
    
    Fixes #983
    Fixes #1220
    Fixes #1320

[33mcommit 6b6d86c71938cc1af0e9dfc13f7e5093bc4ac76c[m
Merge: 240d693 5d3fd98
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Jun 29 11:07:57 2016 -0700

    Merge pull request #1343 from StephenBonikowsky/UpdatePkgVerToBeta-24229-01
    
    Updating to WCF package version beta-24229-01

[33mcommit 5d3fd98cbe9ee7eb8ccd36dc0cfcfd9af6edc5f8[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Jun 29 09:28:21 2016 -0700

    Updating to WCF package version beta-24229-01
    
    * This will pull in Ron's latest product fix, PR #1325

[33mcommit 240d693a473c669ecf58c0f539e833a2b16b8759[m
Merge: ab95fa6 fa9d0dc
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Jun 28 14:19:13 2016 -0700

    Merge pull request #1341 from StephenBonikowsky/UpdateToPkgVerBeta-24228-04
    
    Updated all wcf packages references to Beta-24228-04

[33mcommit fa9d0dcb78906ec409db89e43d174093fa8e14d7[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Jun 28 11:01:15 2016 -0700

    Updated all wcf packages references to Beta-24228-04
    
    * Also had to update the minor version for each wcf package.
    * ActiveIssuing DigestAuthentication_Echo_RoundTrips_String_No_Domain test, it is know to fail on Unix, OSX, NetNative and now on Windows after upgrading the wcf package versions. This test shouldn't block moving to the latest wcf package versions.

[33mcommit ab95fa6ec9ae45d85be29874caeeb2423bb4b6c5[m
Merge: 1de44bf f64114d
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Jun 28 06:56:41 2016 -0700

    Merge pull request #1325 from roncain/fix-messageinterceptor-test
    
    Fix issue where OnOpen and OnClose were not called on custom channels and factories

[33mcommit f64114d28778aa3d84d7b35f25fd069ffdb68ba2[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Jun 23 07:09:10 2016 -0700

    Fix MessageInterceptor test
    
    The MessageInterceptor scenario test was attempting to work around
    the product issue where custom channels and channel factories were
    not being opened or closed properly.  This workaround made the tests
    work, but was an incorrect approach.  On the desktop, these same tests
    failed for issue #1241.
    
    This commit removes the workaround from the MessageInterceptor tests
    so that they exactly mirror the corresponding sample code found
    [here](https://msdn.microsoft.com/library/ms751495(v=vs.100).aspx).
    
    Combined with the product fix, these tests now pass.  Moreover,
    when the prior workarounds are combined with the product fix, we now
    fail in NET Core the same way we fail against the full framework.
    So now we have behavioral parity with the full framework for both
    the positive (no workaround) and negative (with workaround) cases.

[33mcommit ab17582afcd91cec8cb286460793d4258015a2b3[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Jun 23 06:59:18 2016 -0700

    Fix issue where custom channels' OnOpen and OnClose were not called.
    
    We introduced IAsyncCommunicationObject and IAsyncChannelFactory to
    allow Task based methods within our product code.  However these are
    internal types, and customers cannot yet override/implement their
    methods.
    
    The default handling of these in CommunicationObject was effectively
    a NOP (returned a completed Task), which meant that when our internal
    code called into these methods, they did not actually call through to
    the custom channel's OnOpen or OnClose.  As a result, custom channels
    and channel factories failed becaue they were not opened or closed.
    
    This PR changes this behavior so that the default handling of OnOpenAsync
    and OnCloseAsync invokes the synchronous versions.  All our product code
    overrides these methods and do not hit the default handling in
    CommunicationObject.
    
    Fixes #1241

[33mcommit 1de44bff477f0c0199d1f762e4bde5061a9f68c5[m
Merge: 942e8f5 81b6eba
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Jun 27 17:48:11 2016 -0700

    Merge pull request #1339 from StephenBonikowsky/RemoveHelixConditionForTestCommonCode
    
    Removing HELIX condition for ServiceModel project reference.

[33mcommit 81b6ebaee174fe5e547681fd61a084314d488371[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Jun 27 17:35:51 2016 -0700

    Removing HELIX condition for ServiceModel project reference.
    
    * For some unknown reason in the VSO build pipeline we get a restore --packages failure if the unit test project for Infrastructure.Common does not have a reference to S.P.SM.
    * This un-blocks us for now so we can proceed with updating WCF package versions to the latest.
    * Additional changes in the next few days should significantly change how we run tests so this issue will either be addressed there or by-passed entirely.

[33mcommit 942e8f514025b29cf8d75477f717376fc3116a4b[m
Merge: 4c1fb4d 1708156
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Mon Jun 27 15:36:43 2016 -0700

    Merge pull request #1335 from iamjasonp/netci-auto-trigger
    
    Auto-trigger outerloop CI builds upon PR

[33mcommit 1708156d9222145e3b9835130b08a534f4ae5ac6[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Sat Jun 25 09:27:44 2016 +0800

    Auto-trigger outerloop CI builds upon PR
    
    * Change netci.groovy to auto-trigger all builds except code coverage on all
      builds rather than excluding outerloops as we currently do
    * Sync netci.groovy OS definitions to what we see in the dotnet/corefx repo

[33mcommit 4c1fb4d6d75a01ded84cee25c1037d34d38301fb[m
Merge: 8a56f26 32288b3
Author: Zhenlan Wang <zhenlwa@microsoft.com>
Date:   Mon Jun 27 08:41:00 2016 -0700

    Merge pull request #1331 from zhenlan/pkg-ver-update
    
    Update documentation of WCF package versions for .NET core 1.0

[33mcommit 8a56f26d3b09016a7e549deac991a84042291f86[m
Merge: 936e0c5 10019d6
Author: Matt Connew <mconnew@microsoft.com>
Date:   Fri Jun 24 16:58:41 2016 -0700

    Merge pull request #1328 from mconnew/Issue1045
    
    Re-enable digest no-domain tests

[33mcommit 936e0c5ebf2f51223894f6ac5cb292bae1c7fd88[m
Merge: 2be0647 a4b5326
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Jun 24 16:09:28 2016 -0700

    Merge pull request #1333 from StephenBonikowsky/AddConditionalProjectRef
    
    Add condition to proj ref of S.P.SM

[33mcommit a4b5326c8d43bf3d4049da9dfeab619a8b860c40[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Jun 24 15:37:17 2016 -0700

    Add condition to proj ref of S.P.SM
    
    * This project reference is causing strange build behavior in the VSO build pipeline. Since in that environment it is supposed to build test using the actual packages anyway these project references should not be used.

[33mcommit 2be064710bc3ad2523c4a507885b2f5284382fb9[m
Merge: 0be57c8 00c2e88
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Fri Jun 24 15:13:48 2016 -0700

    Merge pull request #1323 from iamjasonp/rename-bridge-certinstaller
    
    Rename BridgeCertificateIntsaller to TestRootCertificateInstaller

[33mcommit 10019d660c5ade7a3a42618929fcad553cb4913b[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Thu Jun 23 17:24:16 2016 -0700

    Re-enable digest no-domain tests on windows
    
    The HttpClient on Unix doesn't do digest authentication correctly so this test is still disabled for Unix

[33mcommit 32288b31e50939935eaca9596ab5816eadc84d58[m
Author: Zhenlan Wang <zhenlwa@microsoft.com>
Date:   Fri Jun 24 11:21:35 2016 -0700

    Update documentation of WCF package versions for .NET core 1.0

[33mcommit 0be57c81305e6a2c88999aa9ed77cb2c0df6d82a[m
Merge: 1f07ee2 025c772
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Jun 24 10:48:00 2016 -0700

    Merge pull request #1330 from StephenBonikowsky/FixRuntimes
    
    Json file was missing the new runtimes and causing VSO failure.

[33mcommit 025c772be734d9589ceb911318b422e7a2f2a4f4[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Jun 24 10:00:59 2016 -0700

    Json file was missing the new runtimes and causing VSO failure.
    
    * Just updated the .json file with recently addes runtimes.

[33mcommit 00c2e88033ec467263b69ce9124bd9d6d76980fa[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Sat Jun 25 00:48:50 2016 +0800

    Removing leftover reference to "Bridge" in TestRootCertificateInstaller

[33mcommit 1f07ee2b38e090293d5c55e83d95260c1dc06a7a[m
Merge: 8f8e853 22d89bc
Author: Hong Dai <hongdai@microsoft.com>
Date:   Fri Jun 24 09:45:35 2016 -0700

    Merge pull request #1319 from hongdai/releasenote
    
    RTM release feature table

[33mcommit 22d89bc85aaaf128013d035a8a5286b9e00a1b51[m
Author: hongdai <hongdai@microsoft.com>
Date:   Fri Jun 24 09:41:50 2016 -0700

    RTM release feature table
    
    * Linux OSs are in one column to simplify. They all have similar results.
    * Known issues points to a github label query.
    
    Fix #1281

[33mcommit 13cd57e9883e8b65564f85ad26a8e91f48a7f996[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Fri Jun 24 06:57:43 2016 +0800

    Change certificate installer scripts' corerun location
    
    init-tools.cmd will download a copy of corerun or corerun.exe; we should leverage
    that copy of corerun instead of relying on finding an already-built test; this
    will help with reliability of finding an appropriate corerun for running the
    TestRootCertificateInstaller on both Windows and Linux

[33mcommit fe31565e31bd0e2abe27c34ff124879c46a7490f[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Fri Jun 24 06:19:08 2016 +0800

    Move TestRootCertificateInstaller to tools directory

[33mcommit 2643256d18787f52ceb00dabcdbc816dad9ebb24[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Jun 23 05:42:20 2016 +0800

    Rename BridgeCertificateIntsaller to TestRootCertificateInstaller
    
    We have removed the Bridge, so we should rename all references to "Bridge" to
    something less technology specific
    
    Also moving the TestRootCertificateInstaller.(sh|cmd) scripts to the appropriate
    scripts directory

[33mcommit 8f8e85308d14d84933fa338787e4299f099cf7e1[m
Merge: 511bc0d ef0206e
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Jun 23 16:16:00 2016 -0700

    Merge pull request #1327 from StephenBonikowsky/FixingPackageVersions
    
    * Prepare to update packages to latest
    
    * Update packages to latest.
    
    This updates the CoreFx, TFS, and CoreCLR packages to a recent build.
    
    Where versions changed I've also incremented the package version
    referenced in addition to the prerelease portion.
    
    I made two additional fixes:
    1. test runtime updated to test against latest to be consistent with the
    tests themselves which were referencing latest.
    2. BridgeCertificateInstaller references latest CoreFx because it was
    referencing another infra library which was already referencing latest.
    
    * Remove setting of RuntimeIdGraphDefinitionFile
    
    This is no longer needed as of https://github.com/dotnet/buildtools/commit/758a9f2143cd403dee43ff5fb17db394230819f2

[33mcommit ef0206ef9c97f0b76241f58f647054964ac42fed[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Thu Jun 23 14:52:11 2016 -0700

    Remove setting of RuntimeIdGraphDefinitionFile
    
    This is no longer needed as of https://github.com/dotnet/buildtools/commit/758a9f2143cd403dee43ff5fb17db394230819f2

[33mcommit f6c23c3ebe748444dc1e342796238cf990c443f6[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Thu Jun 23 10:19:44 2016 -0700

    Prepare to update packages to latest

[33mcommit 36162774e44c0ced3c05f97dce14f9f80faf12cb[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Thu Jun 23 12:20:53 2016 -0700

    Update packages to latest.
    
    This updates the CoreFx, TFS, and CoreCLR packages to a recent build.
    
    Where versions changed I've also incremented the package version
    referenced in addition to the prerelease portion.
    
    I made two additional fixes:
    1. test runtime updated to test against latest to be consistent with the
    tests themselves which were referencing latest.
    2. BridgeCertificateInstaller references latest CoreFx because it was
    referencing another infra library which was already referencing latest.

[33mcommit 511bc0d4c7afa3cbbd87cf1b5675b33944d84e41[m
Merge: f006d07 e0c2793
Author: Zhenlan Wang <zhenlwa@microsoft.com>
Date:   Wed Jun 22 17:06:03 2016 -0700

    Merge pull request #1322 from zhenlan/codeofconduct
    
    Add code of conduct to README.md

[33mcommit e0c2793e06dd209111bd7c9202eed139939f3256[m
Author: Zhenlan Wang <zhenlwa@microsoft.com>
Date:   Wed Jun 22 15:18:23 2016 -0700

    Add code of conduct to README.md

[33mcommit f006d076f3a5a22de2bfdab30c97eb9a962a2588[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Jun 22 11:47:53 2016 -0700

    Port core fx p rs to shared scripts june21 (#1318)
    
    Port core fx p rs to shared scripts june21

[33mcommit 0b6914119c2af839f75e9ca188a5fa54e4705a04[m
Merge: fd5f8ff 03baf04
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Wed Jun 22 09:15:55 2016 -0700

    Merge pull request #1313 from iamjasonp/cert-os-detect
    
    Automate certificate install for OSX, other *nixes

[33mcommit fd5f8ff34f38815f3dc29078865938f901e94f5a[m
Merge: 47c903d 7d5a628
Author: Hong Dai <hongdai@microsoft.com>
Date:   Wed Jun 22 07:04:44 2016 -0700

    Merge pull request #1316 from hongdai/issue1248
    
    Enable test as issue 1248 is fixed

[33mcommit 03baf04e88d98cdb15422873184ba442d70d8529[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Fri Jun 17 08:24:15 2016 +0800

    Automate certificate install for OSX, other *nixes
    
    Adds support to BridgeCertificateInstaller.sh to install root certificates on
    non-Ubuntu OSes

[33mcommit 7d5a6284d4039ed5481e3139cb1ea88776054211[m
Author: hongdai <hongdai@microsoft.com>
Date:   Tue Jun 21 18:42:35 2016 -0700

    Enable test as issue 1248 is fixed
    
    * The issue is already fixed on the test server side, this is to enable
    the test.

[33mcommit 47c903ddd0a2e9a993241ff71fcf90bc4339c31e[m
Merge: 756b1d4 cfc1f06
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Jun 20 14:03:23 2016 -0700

    Merge pull request #1312 from StephenBonikowsky/UpdateSharedFiles
    
    Update shared files

[33mcommit 756b1d4162240c1f5f6de2f71c22dffb61593f7f[m
Merge: cb95ec6 d80faac
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Jun 20 11:44:43 2016 -0700

    Merge pull request #1305 from roncain/remove-1123-activeissue
    
    Remove ActiveIssue(1123) to re-enable tests that failed in CI

[33mcommit cfc1f065e74574f1af5def23b96850fba91b9871[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Jun 17 15:21:15 2016 -0700

    Porting corefx PR #9447
    
    * Updating BuildTools to use sharedRuntime

[33mcommit f5376425173fa087db4fc9b4dda16a2a50ed9c46[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Jun 17 14:34:52 2016 -0700

    Porting corefx PR #9183 and #8997
    
    * Add support to get intellisense files in our packages
    * Add rel-notes and project URL to nupkgs

[33mcommit be07293495819dd11b2775b9b746254427e83454[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Jun 17 11:00:40 2016 -0700

    Merge pull request #9365
    
    * Update buildtools to 00512-01

[33mcommit 48ebf780d676a8ca335b0e43b7706dbb82c12ee4[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Jun 17 08:46:17 2016 -0700

    Porting corefx PR #8397
    
    * Catching errors if they are thrown in the inner init-tools.

[33mcommit d80faac0cee8246171b6acaeb98a25408e20ce16[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Jun 17 09:09:04 2016 -0700

    Also replace ActiveIssue 1295 and 1297 with new ConditionalFact
    
    Issues 1295 and 1297 relate to the same problem that is
    addressed by this new ConditionalFact, so remove those
    ActiveIssues and use the new ConditionalFact instead.

[33mcommit 30afa6aa4871421fa9fac5d6d14b3093d8a5c552[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Jun 16 11:19:40 2016 -0700

    Remove ActiveIssue(1223) to re-enable tests that failed in CI
    
    We used ActiveIssue(1123) to disable some failing CI tests
    when we were first bringing up the new test infrastructure.
    
    This PR replaces ActiveIssue(1123) with a new ConditionalFact.
    This allows these tests to continue to run on machines known
    to be properly configured, and it allows CI and lab runs to
    tell the tests when the test client has been configured properly.

[33mcommit cb95ec660d52838d96f9aa7080ec18d7625dc43f[m
Merge: 0a5d2a2 34e0b74
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Jun 20 10:26:54 2016 -0700

    Merge pull request #1292 from roncain/validate-setup
    
    Make certificate ConditionalFacts allow explicit overrides

[33mcommit 34e0b7499209237adefd426de3fb94bfa2a0dcc9[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Jun 14 10:05:35 2016 -0700

    Make certificate ConditionalFacts allow explicit overrides
    
    This alters the behavior of the ConditionalFact conditions
    for Root_Certificate_Installed and Client_Certificate_Installed
    to respect if they have been set explicitly via the build or
    as Environment variables.  This allows CI or lab runs to
    control whether certificate related tests are run or skipped,
    independent of auto-detection logic.
    
    If not explicitly set, the prior auto-detection logic works
    as it did in the past.  If they can be installed and verified,
    then those ConditionalFact tests run, otherwise they are skipped.
    
    If explicitly set to false, those tests are skipped without
    attempting any certificate installation.
    
    If explicitly set to true, the ConditionalFact logic will
    attempt to install the certificates, and those ConditionalFact
    tests will be run, regardless whether the install succeeded.
    
    This PR also adds new ConditionalFact tests to explicitly
    validate whether the certificates have been installed
    correctly.  This permits easier diagnosis of all certificate
    related tests because these tests will report the installation
    failure.
    
    Fixes #1291

[33mcommit 0a5d2a2e3f072919a63a56a3aecddd66a6d6b9f5[m
Merge: 524d9cd ddce265
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Jun 20 09:09:29 2016 -0700

    Merge pull request #1311 from ericstj/bumpVersions-post-1.0.0
    
    Bump versions across WCF and move to beta

[33mcommit ddce2655b19e8f298eed74db383d1feca6209f65[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Fri Jun 17 10:20:24 2016 -0700

    Build product assemblies against stable packages
    
    I'm leaving tests compiling against pre-release.  Once we have a
    new build of "beta" packages we'll need to update tests to those
    versions.

[33mcommit c950b01e548cee82c67143f8904275c2c4d4f102[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Wed Jun 15 22:26:42 2016 -0700

    Bump versions across WCF and move to beta
    
    Now that we have stable packages for our current versions we need to
    bump the 3rd portion (bugfix) to represent code-changes without API
    additions.

[33mcommit 524d9cdfd223ee35b306126c442a56201aac7c57[m
Merge: e512627 75cc429
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Jun 16 16:33:43 2016 -0700

    Merge pull request #1306 from StephenBonikowsky/AddPSScriptForVSOPublishing
    
    Adding PS script needed to publish packages via VSO pipeline.

[33mcommit 75cc4295ed5030c87b078cf5b6004776e029a52d[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Jun 16 15:36:13 2016 -0700

    Adding PS script needed to publish packages via VSO pipeline.
    
    * File is UpdatePublishedVersions.ps1 and it should have no diffs to the version in corefx.

[33mcommit e5126272b85bf4c386267c4dbe953648db9f6b1d[m
Merge: 7dbee9f e3af9e1
Author: Wes Haggard <weshaggard@users.noreply.github.com>
Date:   Thu Jun 16 16:18:34 2016 -0700

    Merge pull request #1308 from dotnet-bot/from-tfs
    
    Merge changes from TFS

[33mcommit e3af9e13f1e7bbea5c552cd256b0d7ffc0c08a07[m
Author: Wes Haggard <Wes.Haggard@microsoft.com>
Date:   Thu Jun 16 16:07:30 2016 -0700

    Switch WCF away from Microsoft.NETCore.Console and transitiviely Microsoft.NETCore.ConsoleHost.
    
    [tfs-changeset: 1613213]

[33mcommit 7dbee9feccfd65877f5ca5edf3d564b0931472c8[m
Merge: d1ad3ae 20d7088
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Jun 16 11:04:54 2016 -0700

    Merge pull request #1301 from roncain/exclude-tcp-negotiate
    
    Disable TCP Negotiate tests using explicit credentials

[33mcommit 20d708805baed3c38c55a510213cab993a95c7b2[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Jun 15 12:00:30 2016 -0700

    Disable TCP Negotiate tests using explicit credentials
    
    Currently the TCP NegotiateStream tests that use explicit
    credentials fail.  Issue #1262 tracks the underlying
    issue https://github.com/dotnet/corefx/issues/9160
    
    These tests are currently run only manually because they
    require valid user credentials.

[33mcommit d1ad3ae2209aff7b8cbd94d44936394be401ad92[m
Merge: 288b1a3 7189726
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Jun 15 14:16:55 2016 -0700

    Merge pull request #1300 from StephenBonikowsky/MoveTestCase
    
    Moving test case into correct location.

[33mcommit 7189726988d964aaa1d78e571d0a3c6809721520[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Jun 15 09:11:18 2016 -0700

    Moving test case into correct location.
    
    * See Issue #1298 for details.
    * Fixes #1298

[33mcommit 288b1a3a986226e46c76fc172992ce5f115bb745[m
Merge: f6b4bad fe1c77a
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Jun 15 08:54:38 2016 -0700

    Merge pull request #1286 from roncain/explicit-user
    
    Replace all ActiveIssue(851) with ConditionalFact

[33mcommit fe1c77a17cde50e1ba0f42965de73eaea97a7de7[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Jun 15 06:42:58 2016 -0700

    Replace ActiveIssue 851 and 1265 with ConditionalFact
    
    Prior to these changes, issues #851 and #1265 were used as ActiveIssues
    to disable tests that required manual setup. After these changes,
    all these tests now use ConditionalFact to detect whether the
    manual setup has been done and 851 is no longer used as ActiveIssue.
    
    These changes also merge 2 different forms of TestProperties
    for username and password into a single pair. And it eliminates
    a separate configuration class used by the negotiation tests
    because it was redundant with TestProperties.
    
    Fixes #1265
    Fixes #1275

[33mcommit f6b4badce8825cc5d27276b26225ec3b4f444f48[m
Merge: 5c85f69 19ec550
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Tue Jun 14 17:01:52 2016 -0700

    Merge pull request #1289 from iamjasonp/prservice
    
    Update PRService to be robust when fetching new branches

[33mcommit 19ec55095237ff6c5009c1f561fa1bab8631ba63[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Tue Jun 14 10:12:04 2016 +0800

    Update PRService to be robust when fetching new branches
    
    The PRService looks up a list of valid branches before allowing a checkout to a
    branch on the query string. However, when a new branch is created, the PRService
    does not do a fetch before doing the branch name check, which means the PRService
    is not aware of the new branch and fails the check.
    
    This PR
    1. Causes the fetch to happen first
    2. Consolidates the fetch/cleanup logic into CleanupBranches
    
    Fixes #1287

[33mcommit 5c85f693286d364d9037f53619928262c6d534bc[m
Merge: 5d2f9d6 510f669
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Jun 14 16:41:21 2016 -0700

    Merge pull request #1261 from StephenBonikowsky/FixCondFactInTof
    
    Fixing issues related to full xunit not being supported in ToF.

[33mcommit 510f669177a66bfeaf9bbbfd9a619cafaacf2f3e[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Jun 6 20:46:58 2016 -0700

    Fixing issues related to full xunit not being supported in ToF.
    
    * Added a new constant variable only in ToF to if/def against in order to get around ConditionalFact not being supported as well as conditional ActiveIssue attribute.
    * Fixes #1221

[33mcommit 5d2f9d685a4348daa5a9b9cdef8c4240c9a0ce7c[m
Merge: 6af5543 18ae7d7
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Jun 14 13:00:18 2016 -0700

    Merge pull request #1294 from StephenBonikowsky/UpdateRuntimes
    
    Update project.json files to support additional runtimes.

[33mcommit 18ae7d7a370c1ee28dd22ee2d83efc497cf7a555[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Jun 14 11:50:05 2016 -0700

    Update project.json files to support additional runtimes.
    
    * Adding support for fedora, opensuse and ubuntu.16

[33mcommit 6af55434d7042b4859b6850da3911c3505736400[m
Merge: 2514928 b87bb90
Author: Zhenlan Wang <zhenlwa@microsoft.com>
Date:   Fri Jun 10 14:52:53 2016 -0700

    Merge pull request #1278 from zhenlan/unlockconfig
    
    Unlock the IIS config section to allow sslFlags to be overriden

[33mcommit b87bb905fa8c834508cb83a65a181229f143c8ce[m
Author: Zhenlan Wang <zhenlwa@microsoft.com>
Date:   Thu Jun 9 23:33:39 2016 -0700

    Unlock the IIS config section to allow sslFlags to be overriden

[33mcommit 25149288f88bd7981dacae1507b27dfbbe78afa7[m
Merge: fbf7a41 85c8ef7
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Jun 9 17:23:28 2016 -0700

    Merge pull request #1277 from StephenBonikowsky/SyncDir.PropsToCoreFx
    
    Sync dir.props to core fx

[33mcommit 85c8ef78a9f999bb2332a903dac84b660480bc3b[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Jun 9 16:23:11 2016 -0700

    Changes needed to move WCF from TFS to VSO build pipeline.
    
    * These changes diverge dir.props from the version in corefx.
    * If we can get VSO working then we can re-engineer dir.props to not be divergent.

[33mcommit f6bc1e8ec19cddade230d515bf42df03b8c70aa6[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Jun 9 15:40:15 2016 -0700

    Full sync of dir.props to the corefx version.
    
    * This included changes from several different commits done in corefx.

[33mcommit fbf7a41e79fb93f729333264d8d37f49b5881268[m
Merge: 651c622 46701de
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Jun 8 11:04:15 2016 -0700

    Merge pull request #1272 from StephenBonikowsky/UpdatePkgVersionToRc3-24206
    
    Updating to package version rc3-24206

[33mcommit 46701de633c6da2a0e91886bd26a3fbda7f4c0bf[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Jun 8 10:01:24 2016 -0700

    Updating to package version rc3-24206
    
    * Next we need to port this update to the 1.0.0 release branch.

[33mcommit 651c622bdadbea1305357bba1ec22bb536683eef[m
Author: Eric StJohn <ericstj@microsoft.com>
Date:   Tue Jun 7 13:18:31 2016 -0700

    Update buildtools to add Serviceable to nupkgs (#1267)
    
    This also requires an update to all tests to use the NETCoreApp1.0
    moniker.

[33mcommit 037a09867b6bdf01f889a72e7e88d94f95862dce[m
Merge: 5a62b1e a2fe46c
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Tue Jun 7 07:36:33 2016 -0700

    Merge pull request #1263 from iamjasonp/ActiveIssue578Remove
    
    Reactivate tests for Certificate Revocation

[33mcommit a2fe46cd433a3814734fd6825bff3c03a18982aa[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Tue Jun 7 13:56:38 2016 +0800

    Reactivate tests for Certificate Revocation
    
    Closes #578

[33mcommit 5a62b1e871a608edeb6dca090f81b5f8c26c48f8[m
Merge: 68749f4 ebd9c10
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Jun 6 08:17:04 2016 -0700

    Merge pull request #1236 from roncain/psexec_selfhost
    
    Enable self-host startup script to run as Local System conditionally

[33mcommit 68749f45db0cd1258ce05cd101082f04372bebde[m
Merge: 15d226b 834dd67
Author: Eric StJohn <ericstj@microsoft.com>
Date:   Sat Jun 4 01:33:57 2016 -0700

    Merge pull request #1253 from dotnet-bot/from-tfs
    
    Merge changes from TFS

[33mcommit 834dd671713d4b85393c316c1a67e84132247dbc[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Sat Jun 4 00:51:14 2016 -0700

    Fix Clean build of pkgproj without restore
    
    The internal build runs a Clean before restoring packages.
    This caused my check for the targets file to be hit during
    clean and fail the build.
    Move back to an exists check and run a target to make sure
    the baseline was imported during build.
    
    [tfs-changeset: 1610501]

[33mcommit 15d226bc3eb65113b82c3eb5798c336bfea7bff1[m
Merge: 49919e2 c5118f3
Author: Eric StJohn <ericstj@microsoft.com>
Date:   Fri Jun 3 16:44:16 2016 -0700

    Merge pull request #1247 from ericstj/fixBaselining
    
    Fix package baseline

[33mcommit 49919e2e4b537b986cd1ad9a2c3dbfa7c0ed642e[m
Merge: 5faa4a4 80d8a58
Author: Matt Connew <mconnew@microsoft.com>
Date:   Fri Jun 3 16:23:31 2016 -0700

    Merge pull request #1246 from mconnew/EtwPerformanceFix
    
    Disable event channel usage

[33mcommit c5118f33c380761e2c738fdeb06e3ec411ab15d2[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Fri Jun 3 14:58:56 2016 -0700

    Fix package baseline
    
    Package baselining has been broken since the first package update after
    I added it.  The problem was that the package that contains the corefx
    baseline was missing from the update list so the update script updated
    it to be out of sync with the CoreFxExpectedPrerelease property. I've
    added the baseline package to the list of packages coming from CoreFx.
    
    This was silently being dropped because we don't import the target if it
    doesn't exist.  I've updated the import condition to be less permissive.
    We just need it to not import before the restore so constraining it to
    pkgproj will do that, and fail the build if this issue should occur in
    the future.

[33mcommit 80d8a581ba5b3290b71cbfa46b87c5f2ecb8f14c[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Wed Jun 1 12:55:38 2016 -0700

    Disable event channel usage
    
    This is needed because the channels are getting mixed up and we're
    trying to send the wrong events to the wrong channels and this is
    causing a performance problem.

[33mcommit ebd9c105ff1601c7a92c1ef281b14f951bc192dc[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Jun 1 10:49:12 2016 -0700

    Enable self-host startup script to run as Local System conditionally
    
    Adds an environment variable that can be set to alter whether the
    self-hosted WCF services starts under the user account (default)
    or runs as Local System.

[33mcommit 5faa4a4e9e2eb4540fd6e11ff0dd3366bcc7467b[m
Merge: 86f4d57 3f93c6c
Author: Matt Connew <mconnew@microsoft.com>
Date:   Wed Jun 1 14:11:11 2016 -0700

    Merge pull request #1234 from mconnew/CurlHandlerDisposalFix
    
    Wait until message has been sent before completing HttpClient.SendAsync
    
    Fixes #1211

[33mcommit 3f93c6cdc86eb24a9902604cf1a56042879bd965[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Tue May 31 12:18:09 2016 -0700

    Wait until message has been sent before completing HttpClient.SendAsync
    
    Fixes #1211

[33mcommit 86f4d572990bb1e337fdaf82b51707ef892eb9b6[m
Merge: 937ea18 1fd17d5
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Jun 1 04:25:56 2016 -0700

    Merge pull request #1226 from roncain/longer-ping-wait
    
    Lengthen the ping wait time for starting self-host server

[33mcommit 937ea18dc4e481bc70d9b08806dd10629ea9cd59[m
Merge: 6214daf 9d8d532
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue May 31 08:41:25 2016 -0700

    Merge pull request #1225 from roncain/filter-tests
    
    Exclude scenario tests if not building for OuterLoop

[33mcommit 9d8d532ff1d08f90bc9c0d90f0f01b1cc27affbb[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri May 27 09:17:57 2016 -0700

    Exclude scenario tests if not building for OuterLoop
    
    The ConditionalFact detectors were being executed during
    innerloop-only test runs. This is due to the fact we build
    all test projects unconditionally and because xunit's trait
    and Fact discoverers are executed before deciding whether
    there are any tests to run. But unless the Self-host WCF
    service was running (which happens only during OuterLoop
    runs), this generated many exceptions in the console log.
    
    With this change, all projects under tests\Scenarios are
    excluded from the build unless OuterLoop has been requested.
    This means there are no are no binaries containing
    ConditionalFact when a normal non-OuterLoop build is done.
    
    Fixes #1224

[33mcommit 6214dafc5cbb08f83600f58ef2839acb8f61444e[m
Merge: 82c2413 6dbb900
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri May 27 12:08:37 2016 -0700

    Merge pull request #1191 from roncain/fix-duplex-async
    
    Fix async duplex issue that uses wrong OperationContext

[33mcommit 1fd17d5f327872a00f4fdab083a0c3339b02de7d[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri May 27 11:59:53 2016 -0700

    Lengthen the ping wait time for starting self-host server
    
    The scripts to self-start the self-hosted service will fail if
    the service does not respond with a ping in a short time.  But
    the script to start the service also builds it, and when package
    restore is slow can take longer than the ping time.  When this
    happens, the ping script yields control to a 2nd CMD file attempting
    to write to the same log file, and the WcfSetup target fails,
    causing the build to fail.
    
    This PR just lengthens the time we wait for the service to respond
    with a ping when self-starting.  We don't use this ping script for
    anything other than self-start, so the longer time will not adversely
    affect other "ping" scenarios.

[33mcommit 6dbb90033d6bae7a236a44970a05f779a20a9dac[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed May 25 12:13:07 2016 -0700

    Fix async duplex issue that uses wrong OperationContext
    
    Problem: under heavy load with multi-core machines, it was discovered
    the Task used for async operations could be reused by other async
    activities, leading to an inapropriate OperationContext.  This was
    detected internally and threw PlatformNotSupported.
    
    The fixes are to throw the correct InvalidOperationException, ensure
    the OperationContext is set appropriately before any TrySet calls,
    and alter the TimeoutHelper TCS to use ConfigureAwait.
    
    Fixes #1137
    
    Make TimeoutHelper continuations run asynchronously

[33mcommit 82c2413e69629e9c82a2b0ed0d3c8ca40b21442b[m
Merge: aaafdff c8b9490
Author: Hong Dai <hongdai@microsoft.com>
Date:   Fri May 27 09:29:13 2016 -0700

    Merge pull request #1223 from hongdai/securityfolder
    
    Create security subfolders in IIS host server

[33mcommit c8b94909bef2cc44f96835db2d73947392200f75[m
Author: hongdai <hongdai@microsoft.com>
Date:   Thu May 26 14:20:58 2016 -0700

    Create security subfolders in IIS host server
    
    * This will enable most security tests run in Domain machines.
    * This will enable client certificate tests in IIS hosted lab runs.
    * Fix tests with incorrect conditional facts.
    * Fix service side test issue as it's the first time we enable some of the security tests.

[33mcommit aaafdffd18bc16b179f17346aafb2463e3a5fd82[m
Merge: 6814c74 4ff5d1c
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu May 26 08:54:00 2016 -0700

    Merge pull request #1219 from StephenBonikowsky/AddHttpPeerTrustTests
    
    Adding tests for validating certificates with PeerTrust on https.

[33mcommit 4ff5d1cfbafd5a6c1a8be012ee714bcd3bcd6377[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed May 25 15:38:12 2016 -0700

    Adding tests for validating certificates with PeerTrust on https.
    
    * Adding 2 Https tests, one for each pivot. X509CertificateValidationMode.PeerTrust X509CertificateValidationMode.PeerOrChainTrust.
    * Fixes Issue #1192

[33mcommit 6814c74132e83ff004127618c4b31c9bc4024cf5[m
Merge: 1f8a680 a881eb4
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Wed May 25 15:58:10 2016 -0700

    Merge pull request #1218 from iamjasonp/update-tests-activeissue-592
    
    Update tests marked with ActiveIssue #592 to #945

[33mcommit a881eb45a344bf5b813357469b2cd585be1944c7[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu May 26 06:14:44 2016 +0800

    Update tests marked with ActiveIssue #592 to #945
    
    Update the comment in the test file itself, as #592 is no longer an accurate
    description for why this test must be skipped. #945 better tracks the issues around
    why this test is currently skipped.

[33mcommit 1f8a680c3d9479f98c58c2ed7374e95b52d84884[m
Merge: 49ba171 931230a
Author: Matt Connew <mconnew@microsoft.com>
Date:   Wed May 25 15:02:34 2016 -0700

    Merge pull request #1210 from mconnew/HttpCertificateUpdates
    
    Update to use latest HttpClientHandler which has improved certificate support

[33mcommit 931230a7286d20e50029bc178e4e4fbe3c64edd1[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Tue May 24 15:50:38 2016 -0700

    Fix permissions on sync.sh to make it executable

[33mcommit 79e995e13d376e776485ca9c6146447d8c2b9763[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Mon May 23 17:27:29 2016 -0700

    Update certificate installer script for new test service

[33mcommit 1b363d8f9ba58963fb504afa8df212e81c28ac35[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Mon May 23 12:19:41 2016 -0700

    Enable client certificates and server certificate validation using HttpClientHandler

[33mcommit 49ba171829afb97c825b44e8b7ee2b486e38afad[m
Merge: 3cbd7fc 6b905d5
Author: Hong Dai <hongdai@microsoft.com>
Date:   Wed May 25 13:58:51 2016 -0700

    Merge pull request #1212 from hongdai/fixtests
    
    Add conditional fact for the two new tests need root cert

[33mcommit 3cbd7fc106ca3e27c5de034246e7eff18c9e0a9f[m
Merge: 7717613 6018ab5
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed May 25 13:20:38 2016 -0700

    Merge pull request #1214 from StephenBonikowsky/UpdateVersionRC3-24117-00
    
    Update version rc3 24117 00

[33mcommit 6018ab581fa5d97e8134489f5690f6c14559b893[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed May 25 11:14:12 2016 -0700

    Port PR dotnet/corefx#8565
    
    * Porting corefx PR dotnet/corefx#8565, enabling API compat. This also required an update to the BuildToolsVersion.
    * Had to add System.ComponentModel.EventBasedAsync and System.ObjectModel to System.ServiceModel.Primitives\src\project.json with the latest rc3 version to fix some new errors.

[33mcommit 6c23da6a1d1fed9aff6047ac95bb8c069ec1a24d[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue May 24 17:28:03 2016 -0700

    Update package versions to rc3-24117-00
    
    * Had to explicitly add System.Security.Cryptography.X509Certificates to System.ServiceModel.NetTcp\ref\project.json otherwise it was pulling an old version of S.S.C.X509Certificates that didn't support netstandard1.3.

[33mcommit 7717613912f9b7d1f71f4172e6ef59f656428d08[m
Merge: bff8284 527ebab
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed May 25 11:42:03 2016 -0700

    Merge pull request #1213 from mellinoe/include-framework-references
    
    Include net45 framework references in all packages

[33mcommit bff828479de6a8df0aff92dbdc0605e318ab0d26[m
Merge: ff3b24d 34f56dc
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed May 25 11:29:41 2016 -0700

    Merge pull request #1208 from roncain/abort-channelfactory-test
    
    Make flakey outerloop test more stable

[33mcommit 527ebabcc21bbce242c23ae56e9abf7e5cedcb85[m
Author: Eric Mellino <erme@microsoft.com>
Date:   Wed May 25 10:46:36 2016 -0700

    Update buildtools to version 1.0.25-prerelease-00424-01

[33mcommit d6172fe18dc3e83e9ca23c7de619939072088fdf[m
Author: Eric Mellino <erme@microsoft.com>
Date:   Wed May 25 10:40:31 2016 -0700

    Automatically include framework references for net45 configurations.
    
    When referencing these packages on net45, a reference to
    System.ServiceModel.dll (part of the framework) should be automatically
    added, because all of the types live in that assembly. This information is
    automatically added for net46, because we actively build that facade.

[33mcommit 6b905d584b6d2e3f53abe8cbd68fa601427fe885[m
Author: hongdai <hongdai@microsoft.com>
Date:   Wed May 25 10:01:02 2016 -0700

    Add conditional fact for the two new tests need root cert
    
    * The tests are failing in the projectN lab.
    We need the contitional fact to ensure root certificate is installed
    to validate server certificate.

[33mcommit ff3b24ded3ed92978b11b54982e99a937653fc3c[m
Merge: 25e962d b794676
Author: Zhenlan Wang <zhenlwa@microsoft.com>
Date:   Tue May 24 17:58:20 2016 -0700

    Merge pull request #1209 from jhendrixMSFT/master
    
    Add retry logic when downloading the CLI from Azure blob storage.

[33mcommit b794676975d7b5382bc193766ed46d8e2225682b[m
Author: Joel Hendrix <jhendrix@microsoft.com>
Date:   Tue May 24 15:19:47 2016 -0700

    Add retry logic when downloading the CLI from Azure blob storage.
    
    Downloading from Azure can transiently fail for a number of reasons. Add
    some retry logic when downloading to avoid transient failures. For *nix
    variants add a retry value when using curl (wget retries by default).

[33mcommit 25e962d0a239c2aaf928166c3cffbb2a705a0eb9[m
Merge: 69a0425 8e9d713
Author: Zhenlan Wang <zhenlwa@microsoft.com>
Date:   Tue May 24 11:37:05 2016 -0700

    Merge pull request #1207 from zhenlan/cirtm
    
    Add the support of RTM branch to the CI

[33mcommit 8e9d71373224543d6613675742a8c1468f116d88[m
Author: Zhenlan Wang <zhenlwa@microsoft.com>
Date:   Mon May 23 21:49:56 2016 -0700

    Add the support of RTM branch to the CI

[33mcommit 69a042589a6d26f048237a5bb3174e0459bbabde[m
Merge: bfb8ca8 a9da7f4
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue May 24 05:26:58 2016 -0700

    Merge pull request #1206 from StephenBonikowsky/UpdateSharedFiles_5_23_2016
    
    Update shared files 5 23 2016

[33mcommit 34f56dcb6a97112c5f53b3846b049658a37077a3[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue May 24 05:07:19 2016 -0700

    Make flakey outerloop test more stable
    
    The scenario test to abort a channel factory with multiple operations
    running concurrently gave different results depending on timing, and
    therefore gave false failure intermittently.
    
    This change makes the abort test resemble the similar close tests
    in being tolerant of the exceptions thrown when the channel factory
    is aborted.

[33mcommit a9da7f4ae8b648da1ffe982ee8e005f0a577cd03[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon May 23 14:23:42 2016 -0700

    Update to build tools version 00420-01

[33mcommit 7aca419a2c5a94ffe2da1f14d658c8c7c8503c4f[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon May 23 13:43:30 2016 -0700

    Port PR dotnet/corefx#8590
    
    * Porting corefx PR dotnet/corefx#8590
    * Add System.IO.Compression to ExternalVersions

[33mcommit 07aa3071edbd454d9156cb751ca5f07e32854ec1[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon May 23 13:35:26 2016 -0700

    Port PR dotnet/corefx#8508
    
    * Porting corefx PR dotnet/corefx#8508
    * Fixing Forward-Slash Issue

[33mcommit 61701a8ff954c20cd489b993f6bf66bb0aa14b49[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon May 23 13:23:55 2016 -0700

    Port PR dotnet/corefx#8656
    
    * Porting corefx PR dotnet/corefx#8656

[33mcommit 8f7ccb0e198018c30bf2c62947c8f5f5d9e230f8[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon May 23 13:17:25 2016 -0700

    Port PR dotnet/corefx#8401
    
    * Porting corefx PR dotnet/corefx#8401

[33mcommit d7cc69aa83480649a33d5b34eaa1cee49d33d6d1[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon May 23 11:50:31 2016 -0700

    Port dotnet/corefx #8707
    
    *Porting corefx PR dotnet/corefx #8707.

[33mcommit bfb8ca8940f880ec0d5f6690fd1e46f23dc1d1a1[m
Merge: 2d586fb be5e91b
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon May 23 11:43:24 2016 -0700

    Merge pull request #1195 from StephenBonikowsky/VerifyWebSocketUsage
    
    Verify WebSockets are actually used when not using a callback.

[33mcommit be5e91bd5f5e36af21d4006ce5ebdc8392da6bef[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Wed May 18 13:02:44 2016 -0700

    Verify WebSockets are actually used when not using a callback.
    
    * Verify WebSockets are actually being used when WebSocketTransportUsage is set to Always.
    * Wcf automatically uses WebSockets when a callback is used, but this test verifies that it still uses
      WebSockets when not using a callback.
    * Fixes Issue #486

[33mcommit 2d586fb40b5bd37f01ed4e9a9b0076229a6b201d[m
Merge: 9eeab99 6ecb0a7
Author: Matt Ellis <matell@microsoft.com>
Date:   Sat May 21 01:22:12 2016 -0700

    Merge pull request #1197 from dotnet-bot/from-tfs
    
    Merge changes from TFS

[33mcommit 9eeab99d4280f233165a9be8df5986864c4e51f2[m
Merge: 198488c a103c24
Author: Zhenlan Wang <zhenlwa@microsoft.com>
Date:   Fri May 20 18:25:22 2016 -0700

    Merge pull request #1198 from zhenlan/docfix
    
    Update dev guide doc

[33mcommit a103c240e4c704dcf6de4c40ecf278bf67ee47d9[m
Author: Zhenlan Wang <zhenlwa@microsoft.com>
Date:   Fri May 20 18:20:42 2016 -0700

    Update dev guide doc

[33mcommit 6ecb0a78c3103b7f61d2b3b476897753c068d1d2[m
Author: Matt Ellis <matell@microsoft.com>
Date:   Fri May 20 18:19:30 2016 -0700

    Update pre-release tags to RC4
    
    The long term plan is to move our packages versions up and the
    prerelease tag back down to something like -beta, but for now we just
    need to pick something that isn't RC3 as that's what
    the release/1.0.0 branches are using.
    
    Doing this gives us time to do the right thing without having version
    clashes on myget.
    
    [tfs-changeset: 1606983]

[33mcommit 198488cb2d8420caf0376ff84174f6a425e61603[m
Merge: 9076bba a69e494
Author: Zhenlan Wang <zhenlwa@microsoft.com>
Date:   Fri May 20 18:00:05 2016 -0700

    Merge pull request #1186 from zhenlan/devdocs
    
    Update dev doc

[33mcommit a69e494f0f156617735e977c628cb8375a38e5c8[m
Author: Zhenlan Wang <zhenlwa@microsoft.com>
Date:   Fri May 20 01:34:28 2016 -0700

    Add scenario testing guide

[33mcommit 9076bba78c526d0bbfbdbd62c5717fc99b9a4dde[m
Merge: 5be79e3 bfb9726
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri May 20 16:04:10 2016 -0700

    Merge pull request #1194 from StephenBonikowsky/PeerTrustTests
    
    Adding tests for validating certificates with PeerTrust on tcp.

[33mcommit 5be79e361d6c52e7d1f3b4b14a5085b26ad05940[m
Merge: d2c827a 166493f
Author: Zhenlan Wang <zhenlwa@microsoft.com>
Date:   Fri May 20 15:42:24 2016 -0700

    Merge pull request #1190 from zhenlan/ciscript
    
    Add the script that is used to setup CI test server

[33mcommit d2c827a4b6590f4cc18085818f3e30c60dfc8740[m
Merge: 45dbfc2 a60182b
Author: Hong Dai <hongdai@microsoft.com>
Date:   Fri May 20 14:43:00 2016 -0700

    Merge pull request #1188 from hongdai/comments
    
    Add comments for recently made public classes do not match contract

[33mcommit bfb9726ef473a55bc0ebe5e41f7f33d674bc1080[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu May 19 09:41:29 2016 -0700

    Adding tests for validating certificates with PeerTrust on tcp.
    
    * Adding 2 Tcp tests, one for each pivot.
         X509CertificateValidationMode.PeerTrust
         X509CertificateValidationMode.PeerOrChainTrust
    * Fixes #480

[33mcommit 45dbfc2e98a346b6ef15d5223f62a244cd83418c[m
Merge: 0303d71 bbaeb92
Author: Khoa Dang <khdang@users.noreply.github.com>
Date:   Fri May 20 13:21:18 2016 -0700

    Merge pull request #1171 from khdang/add_scenario_tests_extensiblity_model
    
    Add scenario test for channel extensibility model

[33mcommit 166493f4c03f6cac3499aeb355de638e9ace55df[m
Author: Zhenlan Wang <zhenlwa@microsoft.com>
Date:   Fri May 20 10:00:20 2016 -0700

    Add the script that is used to setup CI test server
    
    Usage:
      Repeat n command [parameters]
               n: [Required] The number of iterations to repeat the command
         command: [Required] The command to run for n iterations
      parameters: [Optional] The parameters of the command
                  #: When parameters contain #, the last occurance in the parameter
                  will be replace with the number of iteration.
    
    Example:
      Repeat 42 work.cmd # param
      :The script 'work.cmd' will be called 42 times like
       work.cmd 1 param, work.cmd 2 param, ..., work.cmd 42 param

[33mcommit bbaeb921d25a7243bfc300d24610ca632a94c98b[m
Author: Khoa Dang <Khoa.Dang@microsoft.com>
Date:   Mon May 16 17:48:38 2016 -0700

    Add scenario test for channel extensibility model

[33mcommit a60182ba3db8ffca22405d9a876c07c131bdc038[m
Author: hongdai <hongdai@microsoft.com>
Date:   Fri May 20 09:13:18 2016 -0700

    Add comments for recently made public classes do not match contract
    
    * Add comments to explain why the classes are made public, but not on contract

[33mcommit e0e8f2f859b9c3960a94e249a3d3b980c436dc2a[m
Author: Zhenlan Wang <zhenlwa@microsoft.com>
Date:   Fri May 20 00:06:29 2016 -0700

    Update developer guide

[33mcommit 0303d711b42454ca1e1899ed30b5a1dfe4eddfc7[m
Merge: e572f15 94132fb
Author: Matt Connew <mconnew@microsoft.com>
Date:   Thu May 19 14:50:15 2016 -0700

    Merge pull request #1181 from mconnew/Issue1180
    
    Ensure _closedTcs is set and fix issue where first request to complete is closing the channel

[33mcommit e572f15167dde9c4922ce134fd8c6844d9261628[m
Merge: bda9d9e 94894d5
Author: Hong Dai <hongdai@microsoft.com>
Date:   Thu May 19 13:24:49 2016 -0700

    Merge pull request #1184 from hongdai/issue769
    
    Make OperationFault public so that we are able to reflect it

[33mcommit 94894d5a1b9a7dcaae26247d45ba8468560e9602[m
Author: hongdai <hongdai@microsoft.com>
Date:   Thu May 19 12:07:40 2016 -0700

    Make OperationFault public so that we are able to reflect it
    * We are currently unable to reflect OperationFault. The least risk option is
    to make it public.
    * In order to make it public, we need to make FaultFormatter and XmlObjectSerializerFault
    public as well.
    
    Fix #769

[33mcommit bda9d9ef856e376edbb0278b270e1a65300797f9[m
Merge: fdd836e 4ad08f8
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu May 19 09:41:29 2016 -0700

    Merge pull request #1182 from StephenBonikowsky/UpdateTestsWithActiveIssue1123
    
    Updating tests with ActiveIssue 1123.

[33mcommit 4ad08f805785057b2a5806def073d17cf19a0fd0[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu May 19 09:34:37 2016 -0700

    Updating tests with ActiveIssue 1123.
    
    * Some new tests added recently fail for known Issue 1123 on Linux platforms.
    * The correct fix is to create a conditional fact that skips the test based on an invalid environment setup.
    * The setup that is causing the failure is currently still under investigation.

[33mcommit fdd836ee977afe60361f94a575650719b15d1b1f[m
Merge: cf483f1 9d612a4
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu May 19 06:38:09 2016 -0700

    Merge pull request #1178 from roncain/detector-name-match
    
    Remove certificate matching by name

[33mcommit cf483f1f4f06c5d4ff4ab728a426cac36ba22d7b[m
Merge: 75213b9 8030e74
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu May 19 06:24:12 2016 -0700

    Merge pull request #1159 from roncain/async-close
    
    Fix channel factory async close path to not run synchronously

[33mcommit 8030e7491b44a2f73e3b30b30887b9b16f01efab[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri May 13 12:25:50 2016 -0700

    Fix channel factory async close path to not run synchronously
    
    HttpChannelFactory was handling OnCloseAsync but triggered
    a synchronous close path in the layers below it.  With this
    change, the underlying layers implement the async close path
    and HttpChannelFactory uses it.
    
    Fixes #1154

[33mcommit 94132fb71376f8bf8646ba71ed1225373b1a2010[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Wed May 18 14:52:14 2016 -0700

    Ensure _closedTcs is set and fix issue where first request to complete is closing the channel

[33mcommit 75213b9bde0b1965394871718c8fe2be9f81ffbc[m
Merge: f938449 517ce1e
Author: Matt Connew <mconnew@microsoft.com>
Date:   Wed May 18 14:28:24 2016 -0700

    Merge pull request #1163 from mconnew/UseWaitHandlePerfFix
    
    Prevent spin-waiting by using WaitHandle from Task

[33mcommit f9384492b2de81ea20f1d32b518ab007d2f0b400[m
Merge: 79967bd 57f13eb
Author: Matt Connew <mconnew@microsoft.com>
Date:   Wed May 18 13:02:44 2016 -0700

    Merge pull request #1164 from mconnew/HttpClientTimerPerfFix
    
    Prevent HttpClient from setting a timer on internal cancellation token

[33mcommit 517ce1e3ac053944aa17071cbe8612ac227ab27f[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Fri May 13 15:38:38 2016 -0700

    Created NoSpin variations of WaitForCompletion

[33mcommit 79967bd494c5e84e9fb4e616946887e68e022eea[m
Merge: 26ba234 82e54e5
Author: Matt Connew <mconnew@microsoft.com>
Date:   Wed May 18 12:54:22 2016 -0700

    Merge pull request #1162 from mconnew/SocketConnectionNullTimer
    
    Remove setting timer to null as it may be accessed in race condition of read/write callback and Abort

[33mcommit 57f13eb57864e99a68c72837bf08dde4260df4b5[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Fri May 13 16:13:39 2016 -0700

    Prevent HttpClient from setting a timer on internal cancellation token

[33mcommit 82e54e50495932beb7d7b378f8e9b20e6aa22afa[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Fri May 13 16:27:11 2016 -0700

    Remove setting timer to null as it may be accessed in race condition of read/write callback and Abort

[33mcommit 9d612a4157f454cba0c860316fe849bb915e5ae8[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed May 18 08:23:46 2016 -0700

    Remove certificate matching by name
    
    The ConditionalFact detector for certificates was using a fallback
    strategy to locate certificates based on IssuedBy and SubjectName
    when it could not locate by thumbprint.  But this allowed matches
    on left-over certificates that would not work.
    
    With this change, we match only on thumbprint or report that there
    is no certificate installed and let the ConditionalFact skip the
    certificate tests.
    
    Fixes #1172

[33mcommit 26ba234a6266aa3c0ba9d22dc63055344922c3e1[m
Author: Zhenlan Wang <zhenlwa@microsoft.com>
Date:   Tue May 17 18:26:25 2016 -0700

    Fix links etc. in the supported features doc (#1176)

[33mcommit 0277b0f7e15f9b660f694a02bd79309f71a9de19[m
Merge: e671f7c f641d96
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue May 17 15:31:56 2016 -0700

    Merge pull request #1160 from StephenBonikowsky/AddNetHttpsBindingTests
    
    Add NetHttpsBinding test coverage.

[33mcommit f641d96919051731b083345facb1467697828929[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu May 12 09:22:53 2016 -0700

    Add NetHttpsBinding test coverage.
    
    * Adding tests for NetHttpsBinding.
    * Fixes #507

[33mcommit e671f7c9d7ef159c3afae54a72ac1ad35abe6e24[m
Merge: 2d8fe49 ca56d30
Author: Matt Ellis <matell@microsoft.com>
Date:   Mon May 16 11:19:50 2016 -0700

    Merge pull request #1165 from ellismg/add-rids
    
    Add additional runtimes

[33mcommit 2d8fe499fc46557f4e788627930ce1ce681f1a00[m
Merge: 567086a 4e23479
Author: Zhenlan Wang <zhenlwa@microsoft.com>
Date:   Mon May 16 10:13:45 2016 -0700

    Merge pull request #1126 from StephenBonikowsky/UpdateRc2ReleaseNotes
    
    Update rc2 release notes

[33mcommit 567086a550de830f5e2b686084a611ef6f1f0c9b[m
Merge: 38ec076 31b7589
Author: Zhenlan Wang <zhenlwa@microsoft.com>
Date:   Mon May 16 10:11:09 2016 -0700

    Merge pull request #1167 from zhenlan/releasenotes
    
    Add rc2 to version history for all contract packages

[33mcommit 38ec0761eb18ea9dd0e9248870044db970003b98[m
Merge: d258f24 df417ac
Author: Davis Goodin <dagood@users.noreply.github.com>
Date:   Mon May 16 09:48:45 2016 -0500

    Merge pull request #1161 from dagood/upgrade-cli
    
    Update dotnet CLI to 1.0.0-preview2-002733

[33mcommit ca56d30e6e3713737262dc90073610fa0f267cbb[m
Author: Matt Ellis <matell@microsoft.com>
Date:   Sun May 15 16:00:57 2016 -0700

    Add additional runtimes
    
    - Add "debian.8-x64". This is the correct rid for Debian. I want to
      remove "debian.8.2-x64" before RTM.
    
    - Add a generic "linux-x64" rid which we can use in cases where we want
      to build for a distro where we don't yet have a set of runtime
      packages. This will let us restore the managed assets (since they are
      specific to "unix" or "linux" and then folks can do a native build to
      get the shims and runtime and copy them over (this is a more
      reasonable approach than our current strategy of just using
      ubuntu.14.04-x64)
    
    - Sort runtime sections

[33mcommit 31b7589b558ac1673e32ad564a4589b89e8f1d31[m
Author: Zhenlan Wang <zhenlwa@microsoft.com>
Date:   Sat May 14 18:18:37 2016 -0700

    Add rc2 to version history for all contract packages

[33mcommit 4e23479a4de2cf8038f0a1feb6f6516813e44cc4[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri May 6 13:28:18 2016 -0700

    Update WCF release notes for rc2.
    
    * Updated with a simple linked table of contents.
    * Updated with UWP not supported in rc-2, waiting for official link to a write-up describing the issue.
    * Still waiting for for final packages on nuget.

[33mcommit df417ace2bf29071526e73177265bf1c9c859d85[m
Author: Davis Goodin <dagood@microsoft.com>
Date:   Fri May 13 17:18:26 2016 -0500

    Update dotnet CLI to 1.0.0-preview2-002733.
    
    Remove RIDs on System.Private.ServiceModel Windows project.json to make restore work.

[33mcommit 8bda3d0aff56dd494f95a0c8e195e8c7dec6ae70[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed May 4 13:50:03 2016 -0700

    Update with latest changes before release.
    
    * Update UWP paltform as not supported and provide reason.

[33mcommit d258f24233c50b7ac2fdad1e9a9674368fa05299[m
Merge: fc8593d bd7ae7f
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri May 13 12:29:44 2016 -0700

    Merge pull request #1156 from roncain/fix-encoder-test
    
    Fix custom message encoder test to use supported code page

[33mcommit fc8593dd14b7270e2ebf2973cfb8033dbe73c98d[m
Merge: 2c50efd 0813cab
Author: Khoa Dang <khdang@users.noreply.github.com>
Date:   Fri May 13 12:07:57 2016 -0700

    Merge pull request #1103 from khdang/add_tests_faultcontract_serviceknowntype
    
    Add more tests for FaultContract and ServiceKnownType

[33mcommit 2c50efdf3f2bec667d8eb6e802d9594129e0e79e[m
Merge: 64b9652 c549b9a
Author: Shin Mao <shmao@microsoft.com>
Date:   Fri May 13 11:14:13 2016 -0700

    Merge pull request #1158 from shmao/nt_merge
    
    Added back tests removed by PR #1044.

[33mcommit 0813cab35e868b898f35b5523809fa7ae2af5ab0[m
Author: Khoa Dang <Khoa.Dang@microsoft.com>
Date:   Fri Apr 29 15:31:11 2016 -0700

    Add more tests for FaultContract and ServiceKnownType

[33mcommit c549b9af50dcaf0555099b512e3d2b99d5519339[m
Author: Shin Mao <shmao@microsoft.com>
Date:   Fri May 13 09:54:52 2016 -0700

    Added back tests removed by PR #1044.

[33mcommit bd7ae7fd114449b777bbe15e515f6cef10fc970e[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri May 13 07:13:58 2016 -0700

    Fix custom message encoder test to use supported code page
    
    The custom message encoder tests caused periodic hangs running
    on Linux.  Suspicion was use of an unsupported code page.  This
    PR changes to a supported code page, and the hangs have not
    been observed again.
    
    Fixes #1148

[33mcommit 64b965203037711716e1af8a60273ee7ac5e3bdf[m
Merge: 0bc5e78 d7cd976
Author: Shin Mao <shmao@microsoft.com>
Date:   Thu May 12 22:53:47 2016 -0700

    Merge pull request #1152 from shmao/nt_merge
    
    Re-organized the Scenario Tests.
    
    Ref #1048.

[33mcommit 0bc5e789fd5c47f5c4a5d01d62769c5354d174f8[m
Merge: b4b1152 29b8506
Author: Eric StJohn <ericstj@microsoft.com>
Date:   Thu May 12 17:55:02 2016 -0700

    Merge pull request #1149 from ericstj/nuget2.12
    
    Update buildtools 00412-03

[33mcommit b4b1152635303c2c647825c921383f1497c1c668[m
Merge: 9d01e90 9062be1
Author: Matt Connew <mconnew@microsoft.com>
Date:   Thu May 12 16:11:55 2016 -0700

    Merge pull request #1139 from mconnew/SslCertificateAuthentication
    
    Change test to use X509ServiceCertificateAuthentication constructor

[33mcommit 9d01e907d49a96dc5466c4538098e83f3589832a[m
Merge: b3e7f7d 4c8dc39
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu May 12 14:59:34 2016 -0700

    Merge pull request #1145 from StephenBonikowsky/UpdateSolution
    
    Updating folders and content for Scenario TransportSecurity tests.

[33mcommit d7cd9768b1e4e06ad1a8e4697ed6df6015375e9c[m
Author: Shin Mao <shmao@microsoft.com>
Date:   Thu May 12 12:57:25 2016 -0700

    Re-organized the scenario tests.
    
    Re-organized the scenario tests so that we can use them to cover the scenario where latest Net Native tools + last released stable packages are used.
    
    Ref #1048.

[33mcommit 29b850638b33fd8b437becf534521a5b1fe574ff[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Thu May 12 11:33:16 2016 -0700

    Update buildtools 00412-03

[33mcommit b3e7f7d3ba6d351049748943d23021e3f553df79[m
Merge: aba1e50 d51ff4e
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu May 12 11:52:25 2016 -0700

    Merge pull request #1147 from roncain/improve-cert-debugging
    
    Improve debugging of certificate installation

[33mcommit 4c8dc393b97cbcd9df3c65aa99d81775b6aea685[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed May 11 15:09:37 2016 -0700

    Updating folders and content for Scenario TransportSecurity tests.
    
    * Solution folders were missing and/or did not contain all the files.

[33mcommit d51ff4eb5534a7b86e460450b8e5201d4aa8b1b9[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu May 12 09:29:01 2016 -0700

    Improve debugging of certificate installation
    
    Adds Console output for the root and client certificate
    installers triggered by ConditionalFact.  The changes now
    show what exceptions occur, whether the cert was found in
    the store by thumbprint or whether it had to fallback to the
    name only.  It also shows information about the cert used.
    including valid dates, issuer, etc.
    
    On Linux, these changes allow us to see the reasons cert
    installation fail.  And the exceptions contain suggestions
    why it may have failed.
    
    On cert-based tests that fail for cert validity issues, it
    will likely help understand why.

[33mcommit aba1e5068d45147f11094f6b23ca98457c74e1f0[m
Merge: ce7358c fc872b8
Author: Shin Mao <shmao@microsoft.com>
Date:   Wed May 11 18:17:50 2016 -0700

    Merge pull request #1140 from shmao/1082
    
    Add async scenario tests to Client.ChannelLayer

[33mcommit ce7358c4387acfd502289463b0852df2a7497ef6[m
Merge: 1d0c7e8 6805106
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed May 11 17:07:44 2016 -0700

    Merge pull request #1144 from StephenBonikowsky/UpdateTestNameAndComments
    
    Move and rename some scenario tests.

[33mcommit 1d0c7e80138772785efa6a3e6927f5010be80091[m
Merge: ef76936 03b1d45
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed May 11 15:00:53 2016 -0700

    Merge pull request #1142 from roncain/credential-detectors
    
    Adds/renames some ConditionalFact conditions

[33mcommit 68051061e96beb012f07f954226eb76f84668dfd[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed May 11 14:25:48 2016 -0700

    Move and rename some scenario tests.
    
    * Moving a test that have nothing to do with ClientCredentialType out of that test class into appropriate locations.
    * Moving some very basic NetTcp tests to a new Tcp project folder under Bindings.
    * Renaming tests.
    * Remove unnecessary package reference in json file.
    * Sort and remove unneeded using statements.

[33mcommit 03b1d458371641e990c2db49c2886e62ec806e86[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed May 11 06:08:29 2016 -0700

    Renamed explicit credential TestProperty

[33mcommit fc0b282d290e233f813ccd04d47fca692829b539[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed May 11 05:41:46 2016 -0700

    Adds/renames some ConditionalFact conditions
    
    As per discussions in #958, this adds some new ConditionalFact
    conditions and renames some others.  The implementations of the
    detectors are mostly simple heuristics for now.  Actual detector
    implementations will follow in other PR's.

[33mcommit ef7693632bf11dd732ac41cece0aec7ba1250bfd[m
Merge: e130978 4c5dfdd
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed May 11 14:33:15 2016 -0700

    Merge pull request #1132 from roncain/detector-update
    
    Update [ConditionalFact] certificate detectors to install and check

[33mcommit e13097860095473e141c0ded2b4b0599f6713e15[m
Merge: 409a4b3 fe8f9b4
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed May 11 14:16:56 2016 -0700

    Merge pull request #1125 from roncain/clean-target
    
    Clean target

[33mcommit fe8f9b4a66b47d263073f838a386d2c06f04d99a[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri May 6 08:32:29 2016 -0700

    Add custom WCF clean target that guarantees self-host service is closed

[33mcommit 4c5dfddc25af2d77a48da0876074d9c053763ab1[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed May 11 14:06:19 2016 -0700

    Merged with CodeFormatter changes from other PR

[33mcommit 23194bf18ba2c3d385e62b8c1862e52f9151ee4f[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue May 10 08:22:21 2016 -0700

    Add Client_Certificate_Installed dependency where needed

[33mcommit 4042a8ee9d3bcc8277fce941cff09bbed7f3f373[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon May 9 13:55:48 2016 -0700

    Update [ConditionalFact] certificate detectors to install and check
    
    Changes the certificate [ConditionalFact] detectors to attempt to
    install the certificates but then also to verify that they exist
    in the store.  This permits scenarios where the install can fail
    but a certificate is already in the store.
    
    This change also renames and refactors BridgeClientCertificateManager
    to remove all unused Bridge code and to simplify for only the certificate
    operations currently required.

[33mcommit 409a4b3d3f2d38de8c66c1fc7ebdfccb3c3c46fd[m
Merge: a0e3296 3279a85
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed May 11 13:32:09 2016 -0700

    Merge pull request #1143 from roncain/websocket-test-fix
    
    Add ConditionalFact to WebSockets tests that need it

[33mcommit 3279a859d7664177df089535cd700008d4713525[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed May 11 08:58:19 2016 -0700

    Add ConditionalFact to WebSockets tests that need it
    
    Some WebSockets tests started failing under ToF because
    they really depended on there being a root certificate but
    didn't say so.  They passed in GitHub because other tests
    had caused them to be installed.
    
    This change adds ConditionalFact to those WebSockets tests
    that failed in ToF.

[33mcommit a0e3296cfed0daee9cdc719f3e54d19eb8d158d0[m
Merge: 88431c6 aa840fc
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed May 11 08:49:07 2016 -0700

    Merge pull request #1141 from StephenBonikowsky/CodeFormatterAndUpdateHeader
    
    Code formatter and update header

[33mcommit aa840fcf2f222f2fc26b92c32cf72980b1df1b2b[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue May 10 16:46:08 2016 -0700

    CodeFormatter rule 'ReadonlyFields'.
    
    * Only one issue found in one file.

[33mcommit 882cde159446160194675a9c1eb5ed43ed7970f0[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue May 10 16:39:55 2016 -0700

    Copyright updated for source files.
    
    * Copyright updates only.

[33mcommit 4b95b61dbf984cc1f8f8ad2f07b0d0dfff6ef148[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue May 10 16:30:44 2016 -0700

    Copyright updated for test, common and tools projects only.
    
    * Did not do any formatting rules for test, common, infrastructure or tools code.
    * Copyright headers were only added to all C# .cs files, not sure if we have to do anything for scripts.

[33mcommit fc872b8fc18b8fbc8b8c18f59b888fdd04ca9d91[m
Author: Shin Mao <shmao@microsoft.com>
Date:   Tue May 10 16:14:32 2016 -0700

    Add async scenario tests to Client.ChannelLayer
    
    Enhanced Scenarios\Client\ChannelLayer by adding async versions of each of its tests.
    
    Fix #1082.

[33mcommit 88431c65a5edea869a31b4ab4543cf871158d6c6[m
Merge: 4bdb6ce 4cd1f1f
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue May 10 15:12:33 2016 -0700

    Merge pull request #1138 from dotnet-bot/from-tfs
    
    Merge changes from TFS

[33mcommit 9062be10913ba413b1dce0a163104d5004e529b1[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Tue May 10 14:10:00 2016 -0700

    Change test to use X509ServiceCertificateAuthentication constructor

[33mcommit 4cd1f1f38b2cd0ec7565ba7c7540cfe31fdfdef7[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Tue May 10 14:05:21 2016 -0700

    Fix restore for WCF test utilities in TFS
    
    Further working around the Algorithms change that uses netstandard1.6.
    
    Our internal restore tool seems to still be inferring runtimes causing
    restore for these to fail but only in the TFS build.
    
    [tfs-changeset: 1603518]

[33mcommit 4bdb6ce1834f8eb2308d284d3aac9d0855cc0346[m
Merge: b047418 b1ed343
Author: Shin Mao <shmao@microsoft.com>
Date:   Tue May 10 13:58:57 2016 -0700

    Merge pull request #1044 from shmao/NT_Merge
    
    Re-organized the tests under \contract.
    
    Re-organized the tests under \contract so that we can use them to cover the scenario where latest Net Native tools + last released stable packages are used.
    
    Ref #1048.
    Fix #1121.

[33mcommit b04741836f829abc7c7b7d2a44fe0e84b9b45635[m
Merge: 60c738a 5eb2340
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue May 10 13:56:58 2016 -0700

    Merge pull request #1136 from StephenBonikowsky/UpdateBuildToolsVer_00410-01
    
    Update BuildToolsVersion to 00410-01

[33mcommit b1ed3435d5ef53d62c60750811079c2124c9c2e2[m
Author: Shin Mao <shmao@microsoft.com>
Date:   Tue May 10 13:14:07 2016 -0700

    Re-organized the tests under \contract.
    
    Re-organized the tests under \contract so that we can use them to cover the scenario where latest Net Native tools + last released stable packages are used.
    
    Ref #1048.
    Fix #1121.

[33mcommit 5eb234065b4513a34d1b4e9303167caab253eb4e[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue May 10 12:47:47 2016 -0700

    Update BuildToolsVersion to 00410-01
    
    * Pick up tools version with fix needed for .NET Helix runs on OSX.

[33mcommit 60c738a8fc0ba71f4b51e9ce7c4f0a83d7623c54[m
Merge: 4dc7e9f 591f14f
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue May 10 12:36:00 2016 -0700

    Merge pull request #1133 from StephenBonikowsky/UpdatePkgVer24109
    
    Updating to package version 24109.

[33mcommit 591f14f1701e12a43231487f6e06cba230b9b8a2[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon May 9 15:53:07 2016 -0700

    Updating to package version 24109.
    
    * Currently getting errors related to System.Private.ServiceModel\tests\Common\Infrastructure\src\Infrastructure.Common.csproj.

[33mcommit 4dc7e9f5833c1ff251e1c6e2b29a5bca96b2e94c[m
Merge: 2ed5c39 0748669
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue May 10 09:55:53 2016 -0700

    Merge pull request #1127 from StephenBonikowsky/UpdateSharedCorefxSharedScripts_May9
    
    Update shared corefx shared scripts may9

[33mcommit 2ed5c3996152afc91a3ebdee4c1f1a7934764d0f[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Mon May 9 18:16:11 2016 -0700

    Reduce the number of times the Timer is modified (#1116)
    
    Reduce the number of times the Timer is modified by creating a wrapping IOTimer with various techniques to avoid calling Timer.Change()

[33mcommit 07486691c7892b1995340964e2534fe670afd231[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon May 9 14:59:20 2016 -0700

    Porting corefx PR #8179
    
    * Change sync.cmd to use argument parsing logic from build.cmd

[33mcommit 71f9fb8358a43828d84bf003c214b590d835eacc[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon May 9 14:49:08 2016 -0700

    Porting Corefx PR #8318
    
    * Specify corefx packages to convert when generating a project.json for test.

[33mcommit f64a68d5e4698e33f7bb5d2b20e3952e6f0924c0[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon May 9 14:30:56 2016 -0700

    Porting corefx PR #8156
    
    * [WIP] Algorithm.dll base support for elliptic curves and export.

[33mcommit a5ba96534cae5c1bc19636c2d26f366bfe792422[m
Merge: ce5a5e7 eecf2f9
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon May 9 09:32:26 2016 -0700

    Merge pull request #1100 from roncain/fix-digest-test
    
    Fix Digest auth test to work when username and password are available

[33mcommit ce5a5e78b0e81ba96943b60ff4739f62ca0e4515[m
Merge: 13d6a1b 33a4d96
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri May 6 13:16:05 2016 -0700

    Merge pull request #1124 from StephenBonikowsky/UpdateBuildToolsVer_00405
    
    Update build tools version to 00405-05

[33mcommit 33a4d9639eedb29e1238780749122e868bb786b3[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri May 6 11:15:53 2016 -0700

    Update build tools version to 00405-05
    
    * This fixes the long list of package dependency warnings we have been getting.

[33mcommit eecf2f9a2f3fda3dab006a64cda7fdf82f2a1915[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu May 5 12:33:18 2016 -0700

    Fix Digest auth test to work when username and password are available
    
    Remove [ActiveIssue] from this test and add to exiting [ConditionalFact]
    that it should run only when username and password has been provided in
    TestProperties. Default is that they are not provided, in which case
    this test is reported as skipped.
    
    This test has been verified to run and pass using a command like:
      build.cmd /p:TestUserName=xxx /p:TestPassword=yyy ...
    where xxx and yyy are known good domain credentials.  It has also
    been proven to run and fail when given bad domain credentials.
    
    Fixes #979

[33mcommit 13d6a1b064f18573f3c471d3a8cec60f29e26a89[m
Merge: f686734 5264386
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu May 5 11:55:33 2016 -0700

    Merge pull request #1096 from roncain/detectors
    
    Add initial [ConditionalFact] detectors

[33mcommit 5264386dca4f5ccb893c767fb24fd6624bb9847f[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Apr 27 13:28:51 2016 -0700

    Add initial [ConditionalFact] detectors
    
    Prior to this, the [ConditionalFact] condition tests were
    simple heuristics.  This PR introduces runtime detectors that
    to detect root and client certificates are installed as well as
    whether the client is domain joined.

[33mcommit f686734e4d7f7e9a441d6dc39ae20b354311b208[m
Merge: 928ae08 5e8b6ae
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu May 5 10:05:25 2016 -0700

    Merge pull request #1122 from StephenBonikowsky/UpdateTestProgPattern
    
    Updating scenario tests with standard programming model.

[33mcommit 5e8b6aecfef75c324ed2e6f72942132af6e24ddd[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed May 4 14:59:28 2016 -0700

    Updating scenario tests with standard programming model.
    
    * Scenario/Binding/Custom tests.
    * Scenario/Binding/Http tests.
    * Scenario/Client/ChannelLayer tests.
    * Scenario/Client/ClientBase tests.

[33mcommit 928ae08e81979520ba087fa8b31039ce9480fa49[m
Merge: d21a11d 52c4b70
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed May 4 13:24:44 2016 -0700

    Merge pull request #1113 from roncain/selfhost-logging
    
    Capture log files during selfhost setup and cleanup

[33mcommit d21a11d4a651fdaacbab716f45630c96f99fb7e0[m
Merge: b280586 0859ea4
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed May 4 10:19:53 2016 -0700

    Merge pull request #1117 from StephenBonikowsky/MergeSharedScripts_May3
    
    Continuing updates to shared scripts between wcf and corefx.

[33mcommit 0859ea499b887d1a5ad676da48edd47820ea722f[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue May 3 15:23:20 2016 -0700

    Continuing updates to shared scripts between wcf and corefx.
    
    * Corefx PR dotnet/corefx#8129
    * Corefx PR dotnet/corefx#8088
    * Other small changes.

[33mcommit b28058628229291516f2eb9f459128e215d9eaa2[m
Merge: a60e011 98d0bf1
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed May 4 10:16:55 2016 -0700

    Merge pull request #1118 from StephenBonikowsky/DelayedExpErrorLevelNotNeeded
    
    Don't need ERRORLEVEL to be set/used by setup/cleanup scripts.

[33mcommit a60e0117fa6d6f8823e428eeafe4f25ea26d6dee[m
Merge: 72c5e47 f364ba2
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed May 4 10:16:35 2016 -0700

    Merge pull request #1119 from StephenBonikowsky/UpdateSHScripts
    
    Syncing .sh scripts with corefx

[33mcommit 52c4b7082acd4eeabafa13cc7d9de489db0013a0[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed May 4 08:43:57 2016 -0700

    Improve handling of non-elevated execution

[33mcommit f364ba2d08c5e3ac6f237a68e40b5d7dbb4c52e6[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue May 3 18:37:46 2016 -0700

    Syncing .sh scripts with corefx
    
    * Just bringing them up-to-date.

[33mcommit 98d0bf14eada225e15481d0fb82bd8033dbdcbe8[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue May 3 18:25:19 2016 -0700

    Don't need ERRORLEVEL to be set/used by setup/cleanup scripts.
    
    * Since we no longer execute setup and cleanup scripts from build.cmd we no longer need !ERRORLEVEL! after the call to msbuild, doesn't hurt to keep it but its out of sync with corefx and I think it makes more sense to change it in WCF since we don't technically need it anymore.

[33mcommit 72c5e47ea3d58608ff8c82944372054c9131f2f5[m
Merge: dc50cf3 ad6f652
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue May 3 15:09:47 2016 -0700

    Merge pull request #1115 from StephenBonikowsky/PortCorefx8072
    
    Porting corefx PR 8072

[33mcommit ad6f652895c280cf7f42eeec9c8db27e35983b8a[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue May 3 11:28:28 2016 -0700

    Porting corefx PR 8072
    
    * Dir.props only contains the changes made by PR 8072.
    * Additional changes to Contract .csproj files are necessary for WCF to not get package errors.

[33mcommit 08b8a58cbb9eca472fd4ed8b31e4ba8cd7ceb5e0[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue May 3 13:32:28 2016 -0700

    Capture log files during selfhost setup and cleanup
    
    To facilitate understanding failures when selfhost outerloops
    run in the lab, capture the output of each of the separate
    scripts and exe's and log them to msbuild output.
    
    Fixes #1112

[33mcommit dc50cf32288fac39bef421f6e1fd8f211eb57cdf[m
Merge: 95eba12 22e7656
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue May 3 09:37:27 2016 -0700

    Merge pull request #1094 from roncain/custom-binding-test
    
    Add new custom binding test to eliminate lab test failure reports

[33mcommit 95eba12a055157fa20068a923fbbd1fe3f88697b[m
Merge: e346716 61a1606
Author: Zhenlan Wang <zhenlwa@microsoft.com>
Date:   Tue May 3 09:32:54 2016 -0700

    Merge pull request #1104 from zhenlan/ServerSetupFix
    
    Server setup script fix

[33mcommit 61a1606d02770998b7c64b1e7249ebef84031b51[m
Author: Zhenlan Wang <zhenlwa@microsoft.com>
Date:   Sat Apr 30 15:18:10 2016 -0700

    Rename WcfTestServerSetup.cmd to SetupWcfIISHostedService.cmd
    so it follows the same "DoWhat" naming convention as the rest of scripts.

[33mcommit bfc38829eaee0c6ab2a15f39cb875c683cbbdb01[m
Author: Zhenlan Wang <zhenlwa@microsoft.com>
Date:   Sat Apr 30 14:33:15 2016 -0700

    Fix error handling in server setup script
    - Fixed `:Failure` that is completely messed up when `%_cmd%` contains quotes or parenthesis
    - Redirect both standard output and standard error to log so error information can be displayed properly in failure case

[33mcommit e34671651f3ae728cfbede23afc8ffbf516ec54e[m
Merge: add994a 31bfbfa
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue May 3 09:25:34 2016 -0700

    Merge pull request #1110 from StephenBonikowsky/UpdatePackagePublishingForAzure
    
    Update package publishing for azure

[33mcommit 31bfbfaf5ef555f8ef9c7de29e31aae17bdc8bb0[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Apr 22 16:29:54 2016 -0700

    Update with PRs from corefx impacting what tests build against.
    
    * PR dotnet/corefx#7604 :: Enable project reference to package dependency conversion for test builds.
    * PR dornet/corefx#8014 :: Set exit codes for wrapper scripts if there is an error.

[33mcommit add994a41c5b821c4f19e2e83332cde8d67cec10[m
Merge: c2a487b 1742f59
Author: Hong Dai <hongdai@microsoft.com>
Date:   Mon May 2 13:48:26 2016 -0700

    Merge pull request #1107 from hongdai/port80
    
    Add port80 to firewall because crl service need it

[33mcommit 99cd9e197f5c85b00b4b5a39afaccc100e1e241c[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Apr 22 15:48:33 2016 -0700

    Porting corefx PRs 7936 and 7989
    
    * PR #7936 - Stop the build if package restore failed. (Only touched build.proj)
    * PR #7989 - Validate that the results of a restore are exactly what was requested
       Required build tools update.
       Immediately caught version errors with System.Net.Http, System.Private.ServiceModel and System.Runtime.Extensions which are also fixed in this PR.

[33mcommit 1742f59938b35b03d4b6f8979c901fa7076e27f5[m
Author: hongdai <hongdai@microsoft.com>
Date:   Mon May 2 11:14:19 2016 -0700

    Add port80 to firewall because crl service need it
    
    * This is because there is only one server certificate created per machine.
    To run both selfhost and IIS hosted server on the same machine, we need to
    use the same port.

[33mcommit c2a487bf08dddb3afa7720e3a3a6d98bd3dea8ca[m
Merge: 3db18da 04c9c0e
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon May 2 10:49:13 2016 -0700

    Merge pull request #1106 from dotnet-bot/from-tfs
    
    Merge changes from TFS

[33mcommit 04c9c0e5aea9f40910909fb092ebb7443fe58ad3[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Mon May 2 09:02:39 2016 -0700

    Use netcoreapp1.0 TFM for .NETCore validation
    
    Update to the latest buildtools and switch to using netcoreapp1.0
    instead of netstandardapp* for validation.
    
    [tfs-changeset: 1600752]

[33mcommit 3db18da3abfa0d1d8a8f89b3cb02beedf8a9d8dd[m
Merge: 3bc0949 d839f70
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Fri Apr 29 09:51:25 2016 +0800

    Merge pull request #1097 from iamjasonp/netci-outerloop
    
    Modify netci.groovy to support selfhosted outerloop

[33mcommit 3bc09496fe3b8ea0d7f36facfed7e653ca4f9ddf[m
Merge: 1c7c01e 969b3ac
Author: Hong Dai <hongdai@microsoft.com>
Date:   Thu Apr 28 15:19:49 2016 -0700

    Merge pull request #1083 from hongdai/buildcmd
    
    Integrate dev experience to build

[33mcommit 969b3ac368086193bf8e192a444b4ba38e1122da[m
Author: hongdai <hongdai@microsoft.com>
Date:   Thu Apr 28 14:39:31 2016 -0700

    * Hook Start and clean up service into build
    * If the build starts the service, it will clean up the service as well.
    * If the service already started, the build will not restart and clean up.
    * The start service will be elevated.
    * Need feedback on whether "BuildAllProjects" target is the best target to ensure
    set up get run before tests start.
    
    In the second iteration:
    * The build will start from StartWCFSelfHostedSvc.cmd.
    * Devs who want to start it themselves and keep it runing should directly
    call StartWCFSelfHostedSvc.cmd.
    
    In the third iteration:
    * Fix several script errors.
    * Make a few improvement so that any errors will show up in build log if it's
    running from an Admin window.

[33mcommit 1c7c01e2ee72bab13ab34c754b43e40709d4b5dc[m
Merge: 1266187 e2076ec
Author: Zhenlan Wang <zhenlwa@microsoft.com>
Date:   Thu Apr 28 08:58:12 2016 -0700

    Merge pull request #1090 from zhenlan/ServerSetupScript
    
    Update server setup script to use one central PRService

[33mcommit d839f705498f1bdcdfc1669ba0e4a019d6bca0c8[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Apr 28 18:04:21 2016 +0800

    Modify netci.groovy to support selfhosted outerloop
    
    PR #1083 adds support for testing outerloops against SelfHostedWcfService, so we
    need a similar mechanism to do so in CI as well

[33mcommit e2076ec7e16ada0a71ce72ba7e52918dc12cbd19[m
Author: Zhenlan Wang <zhenlwa@microsoft.com>
Date:   Tue Apr 26 12:35:48 2016 -0700

    Update server setup script to use one central PRService only
    - Changed to setup PRService only if the id is "Master"
    - Changed to grant app pool access to folders by inheritance instead of explicit-set
    - Changed to run this script only if it is elevated
    - Other cosmetic fixes

[33mcommit 126618740d4ce49275e6c0aa247b0719646e05c9[m
Merge: 96dd29c fd9b51e
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Thu Apr 28 10:40:33 2016 +0800

    Merge pull request #1093 from iamjasonp/netci-outerloop
    
    Add "test all outerloop" phrase to CI

[33mcommit 96dd29ca6027218d3cf26499086050e0956519c5[m
Merge: 543646a f378465
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Apr 27 09:22:19 2016 -0700

    Merge pull request #1091 from dagood/remove-infer-runtimes
    
    Remove --infer-runtimes restore argument

[33mcommit 22e7656d502e2b79f155dfa87a936348b9cf9c34[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Apr 27 04:58:54 2016 -0700

    Add new custom binding test to eliminate lab test failure reports
    
    Adds a single new CustomBinding test that uses Http rather than
    the existing Https test. Done for 2 reasons:
      1. Slightly better coverage
      2. The existing Https test has [ConditionalFact] and is the only test in
         this project.  When a lab run in Helix or ToF chooses not to run
         this test, they both report failure due to having no tests to run
         in the project.  Having a 2nd test eliminates these spurious error
         reports.

[33mcommit fd9b51e3bc5b2f594b52f862ddffdaa43f8b08ea[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Wed Apr 27 18:30:49 2016 +0800

    Add "test all outerloop" phrase to CI
    
    We used to have the phrase "test outerloop please" for testing, but it eventually
    got removed in the process of merging the groovy files from corefx. Change the triggers
    so that we allow using "test all outerloop" as a phrase to trigger CI builds

[33mcommit f37846550b9bb368030cb6c245a671122b001571[m
Author: Davis Goodin <dagood@microsoft.com>
Date:   Tue Apr 26 17:50:06 2016 -0500

    Remove --infer-runtimes restore arg: runtimes specified in project.json files.

[33mcommit 543646aded0424f51add3fff4306c8c91b85b334[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Tue Apr 26 15:40:34 2016 -0700

    Implements Ssl security upgrade for NetTcp for UWP apps (#1086)
    
    Implements Ssl security upgrade for NetTcp for UWP apps
    Fixes #833
    Fixes #1057

[33mcommit f566fa0852b35e989401adfb9836d188e355dff1[m
Merge: 3ee9c34 8a85d7b
Author: Matt Connew <mconnew@microsoft.com>
Date:   Tue Apr 26 13:53:38 2016 -0700

    Merge pull request #1037 from mconnew/SslCertificateAuthentication
    
    Make X509ServiceCertificateAuthentication constructable

[33mcommit 3ee9c343c957f286dfe1f7692afbde7fd1c63896[m
Merge: b5799d1 8a3531c
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Apr 26 13:40:04 2016 -0700

    Merge pull request #1081 from roncain/reenable-custom-encoder-tests
    
    Port custom text encoder scenario tests to new new infrastructure

[33mcommit 8a3531c60c63e8af48c2eb45824db1682e9df0cb[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Apr 22 13:50:30 2016 -0700

    Port custom text encoder scenario tests to new infrastructure
    
    This PR just ports the custom text encoder scenario tests and converts
    their Bridge endpoint resources to the new test infrastructure's
    ServiceHosts.
    
    Fixes #1042

[33mcommit b5799d1155f7bd1f862e8131a9940d43685e064b[m
Merge: 54f4966 3fe60c1
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Apr 26 04:33:38 2016 -0700

    Merge pull request #1071 from roncain/async-close
    
    Make async channelfactory close stay asynchronous

[33mcommit 54f49662daeff33f3076758e73016e47e073b6f6[m
Merge: 09bde42 8e8c0cf
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Apr 25 14:35:34 2016 -0700

    Merge pull request #1076 from StephenBonikowsky/MergeSharedScriptsWithCorefx
    
    Porting PRs from corefx.

[33mcommit 8e8c0cf966404a7f679deca11541efe228e20f26[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Apr 22 15:48:33 2016 -0700

    Porting corefx PRs 7936 and 7989
    
    * PR #7936 - Stop the build if package restore failed. (Only touched build.proj)
    * PR #7989 - Validate that the results of a restore are exactly what was requested
       Required build tools update.
       Immediately caught version errors with System.Net.Http, System.Private.ServiceModel and System.Runtime.Extensions which are also fixed in this PR.

[33mcommit 1647f91c21642f8f6fab6fc8e613295943394595[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Apr 22 14:13:28 2016 -0700

    Porting corefx PR 7604
    
    * Not including all the changes to sync.cmd because a later PR (#7693) further changed this file. Will pull together all the changes to sync.cmd in a separate commit.

[33mcommit ac9660f60582f0e8978689ab2840fafe695e6a68[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Apr 21 17:39:02 2016 -0700

    Porting PR #6975, unify .gitignore
    
    * Removing WCF specific section after fixing in corefx in .gitignore file.
    * Porting PR: https://github.com/dotnet/corefx/pull/6975

[33mcommit 09bde424dd1b256b212a8b6ab6d2f257d248460a[m
Merge: 1624dfd 33b50ba
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Apr 25 13:38:46 2016 -0700

    Merge pull request #1084 from roncain/reenable-ntlm-test
    
    Fix broken NTLM functional test when self-hosted

[33mcommit 33b50ba7dd88544615e20a15e0ca14635709e44c[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Apr 25 11:31:41 2016 -0700

    Fix broken NTLM functional test when self-hosted
    
    The new self-hosted WCF service was creating the NTLM test
    service host with incorrect parameters.
    
    Fixes #1056

[33mcommit 1624dfdcd3339492aecdeda3c8ecabc2f184d45d[m
Merge: 0ed1b9c e002faf
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Mon Apr 25 17:10:42 2016 +0800

    Merge pull request #1077 from iamjasonp/netci-codecov
    
    Turn on CentOS7.1 builds for CI

[33mcommit e002faf1fb1222f5bc1862a99c551c5aa72c4f66[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Mon Apr 25 16:36:12 2016 +0800

    Clean up netci.groovy
    
    * Clean up dead sections of netci.groovy
    * Change innerloop build status names
    * Remove rc2 builds from non Windows_NT outerloops due to dependency on Bridge
      As such, these builds will always fail

[33mcommit f768ee0b180ceb0c78fbae41ee54bc99b2b27f0b[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Mon Apr 25 16:34:17 2016 +0800

    Turn on CentOS7.1 builds for CI

[33mcommit 0ed1b9c9de30ed488fb7a6d38e89325a25bab38b[m
Merge: fc78b10 2753fca
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Mon Apr 25 14:50:10 2016 +0800

    Merge pull request #1075 from joperezr/FixInitToolsIssue
    
    Fix issues in init-tools when path to git repo contains spaces

[33mcommit fc78b104ceed663d94d5b3f3e063e4fe6c71c70f[m
Merge: 3c7c798 db4ebb7
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Mon Apr 25 13:24:33 2016 +0800

    Merge pull request #1068 from iamjasonp/update-buildtools
    
    Update buildtools and dotnet-cli version

[33mcommit 2753fcabdf3c5dcb37fa6b06bd95153427dcd5a3[m
Author: Jose Perez Rodriguez <joperezr@microsoft.com>
Date:   Fri Apr 22 15:34:46 2016 -0700

    Fix issues in init-tools when path to git repo contains spaces

[33mcommit 3c7c798d7fff8ab50879467cca36ed521bf87cb9[m
Merge: cc1e5b6 bd7f4d3
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Sat Apr 23 01:51:46 2016 +0800

    Merge pull request #1070 from iamjasonp/netci-codecov
    
    Disable concurrent builds in code coverage

[33mcommit cc1e5b61f0406684844265cc54413427ad52ad59[m
Merge: 33ad8bd 590be54
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Apr 22 09:53:58 2016 -0700

    Merge pull request #1067 from StephenBonikowsky/AddSetupAndTeardownHooks
    
    Move setup and cleanup steps to msbuild.

[33mcommit 590be547a244923025e8d8977fbdd3501467d3c6[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Apr 21 09:36:50 2016 -0700

    Add build.override.targets import to build.proj
    
    * Just adding the hook needed in build.proj that we can use to do other things such as setup and cleanup tasks.
    * Also being added in corefx.

[33mcommit 3fe60c148eefb28345b9172a102fd614bcf1916a[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Apr 22 09:44:57 2016 -0700

    Make async channelfactory close stay asynchronous
    
    The asynchronous channelfactory close path executed
    synchronous close logic, leading to a sync/async
    "sandwich" and eventual thread starvation.
    
    These changes adds helper methods and makes the async
    close stay asynchronous.
    
    It also adds functional tests for async opens and closes
    of both channelfactories and proxies because those code
    paths were not being covered yet.
    
    Fixes #1022

[33mcommit bd7f4d3e88396de6a51ca06f4d2166147b1d38f2[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Fri Apr 22 17:42:15 2016 +0800

    Disable concurrent builds in code coverage

[33mcommit 33ad8bd02180cf3a63d406fa1c0e47ecddd4b24b[m
Merge: 62caca7 a2e4587
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Fri Apr 22 17:27:10 2016 +0800

    Merge pull request #1069 from iamjasonp/netci-codecov
    
    Update netci.groovy for code coverage runs

[33mcommit a2e45870cf5591b28d416466c416dbaa4d8ce5cc[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Fri Apr 22 17:17:15 2016 +0800

    Update netci.groovy for code coverage runs
    
    The code coverage CI jobs were not passing in the ServiceUri parameter when
    running; this is needed for outerloop tests, which code coverage requires

[33mcommit db4ebb7bc358d756db02ec9941db008288453be4[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Fri Apr 22 17:00:24 2016 +0800

    Upgrade dotnet CLI to 1.0.0-rc2-002468

[33mcommit 48205a3684bda878975f19bc0f342f0947486ad4[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Fri Apr 22 16:59:29 2016 +0800

    Update buildtools to v 1.0.25-prerelease-00321-01

[33mcommit 62caca7ec9d641ba9da4e2522acaaa1a4ef7abc1[m
Merge: 11ba862 6496d5b
Author: Zhenlan Wang <zhenlwa@microsoft.com>
Date:   Fri Apr 22 00:55:39 2016 -0700

    Merge pull request #1036 from zhenlan/ServerSetup
    
    Script to setup WCF test services hosted on IIS for WCF Core testing

[33mcommit 11ba862e1607c66c2ac5c1a3e0bdadfe4224aa5b[m
Merge: d6cbaa2 84d3de3
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Apr 21 12:47:21 2016 -0700

    Merge pull request #1063 from roncain/disable-tracing
    
    Disable svclog tracing in IIS-hosted test services

[33mcommit d6cbaa2ebabd39eb1f010740b49dde620c5d66bf[m
Merge: 681df85 66292e6
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Fri Apr 22 02:41:15 2016 +0800

    Merge pull request #1064 from iamjasonp/netci-elevated
    
    Fix typo in netci.groovy

[33mcommit 66292e654c79646d6666c95d12d03783c5f014f4[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Fri Apr 22 01:51:08 2016 +0800

    Disable concurrent building for outerloop builds

[33mcommit db9ded7a083d873323567400acd5bf0b54c5b6d4[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Fri Apr 22 01:18:06 2016 +0800

    Turn on outerloop CI for Ubuntu/Mac builds

[33mcommit 0c4da5f70173ef10486e15d36386e5ac95edc2f7[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Fri Apr 22 01:16:41 2016 +0800

    Fix typo in netci.groovy
    
    An errant brace caused windows-elevated machines to not affinitize properly

[33mcommit 681df85ef2a337530d93df9b3eddee1554b7aaea[m
Merge: 31a70f8 91f7d74
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Apr 21 09:22:57 2016 -0700

    Merge pull request #1062 from roncain/fix-1043
    
    Re-enable temporarily disabled WebSockets tests

[33mcommit 84d3de32ce629ede4739d924207cc001ee6c83eb[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Apr 21 09:16:27 2016 -0700

    Disable svclog tracing in IIS-hosted test services
    
    This changes the default behavior of IIS-hosted test services
    so that they do not trace to svclog files.  The web.config section
    is preserved as a comment to allow it to be re-enabled to debug.

[33mcommit 31a70f8520afec61bb12e480176c4d9cfd756ac8[m
Merge: eea8a6f 8c523b1
Author: Matt Mitchell <mmitche@microsoft.com>
Date:   Thu Apr 21 08:42:04 2016 -0700

    Merge pull request #1061 from iamjasonp/netci-elevated
    
    Change windows-elevated runs to use autoscaling pool

[33mcommit 91f7d743283510a1abecb304f9331b49e2e0da82[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Apr 21 07:12:20 2016 -0700

    Re-enable temporarily disabled WebSockets tests
    
    Moving to the new test infrastructure made these tests
    fail due to using the BridgeHost property to determine
    if they were running as localhost. We disabled them
    temporarily during infrastructure integration.
    
    The fix is to use the ServiceUri instead of BridgeHost.
    
    Fixes #1043

[33mcommit eea8a6fc1cbbf762199fb11e520deea25a9a37c9[m
Merge: 9b5c6a6 910eba9
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Thu Apr 21 18:36:17 2016 +0800

    Merge pull request #1060 from iamjasonp/masterPR
    
    PRService: update sync-pr scripts for new PRService

[33mcommit 8c523b17ee44292ba4c5fcd1c94884b702a03f0f[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Apr 21 18:24:24 2016 +0800

    Change windows-elevated runs to use autoscaling pool

[33mcommit 910eba929181df8d23061d7d2eec3c5aa884d95b[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Apr 21 17:43:20 2016 +0800

    PRService: update sync-pr scripts for new PRService
    
    The earlier iteration of PRService relied on the URI query string passing repo=repo_id, however
    we pushed a change that changed the repo ID identifier be 'id' on the query string
    
    Modify the two scripts so they call the PR service correctly

[33mcommit 9b5c6a6c05b5220b5e0c727627e58a162aa9711d[m
Merge: f528b08 5f15084
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Thu Apr 21 17:24:31 2016 +0800

    Merge pull request #1054 from iamjasonp/masterPR
    
    Consolidate various PRServices into one endpoint

[33mcommit 5f150848a4ede7b017546e07faf44507b423f4d5[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Wed Apr 20 16:52:29 2016 +0800

    Centralize the PRService into one endpoint
    
    Rather than have multiple PRServices (one for each repo) on the PR service machine,
    centralize them into one endpoint, and instead pass the repo id. This has the
    benefit of allowing only one copy of the service to be maintained, having fewer
    points of entry, having one consistent service across all repos, and finally,
    preventing accidental syncs from killing the service and requiring manual
    intervention

[33mcommit f528b08f93c4eed5dcd21a236cbc6b3a3811b399[m
Merge: 62bdec3 1fbd0a5
Author: Eric StJohn <ericstj@microsoft.com>
Date:   Wed Apr 20 13:12:27 2016 -0700

    Merge pull request #1050 from ericstj/packageBaseline
    
    Add package baseline

[33mcommit 62bdec3491d5cd3852cb163cd660446ccf4e771a[m
Merge: 3d65117 94bd734
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Apr 20 12:21:41 2016 -0700

    Merge pull request #990 from roncain/reenable-osx-tests
    
    Re-enable scenario tests disabled in OS X due to SetLingerOption issue.

[33mcommit 94bd734a1ab27628e5abb480f7bd09ac90780a61[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Apr 20 10:52:33 2016 -0700

    Re-enable scenario tests disabled in OS X due to SetLingerOption issue
    
    Issue dotnet/corefx#7403 caused multiple tests
     to fail in OS X, so we disabled them with [ActiveIssue].
    
    Now that 7403 is fixed, these tests can be re-enabled. Testing these
     in the labs will require updating to latest CoreFx packages.

[33mcommit 1fbd0a58fa436ba73cb3207ff50beaaf304c1daa[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Tue Apr 19 15:24:54 2016 -0700

    Update buildtools to 1.0.25-prerelease-00319-03

[33mcommit 93a679d4abf6ea4b4219ed7b2d0a32e0b8a27637[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Tue Apr 19 14:36:51 2016 -0700

    Add package baseline
    
    Package baseline is now defined in individual repos instead of
    buildtools.
    
    Pick up CoreFx's baseline from the Microsoft.Private.PackageBaseline
    package and add a WCF specific baseline.

[33mcommit 3d65117ef784ce1ba662bb3617e7ca1df8e753c5[m
Merge: 880f6a4 7eb5bc9
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Apr 20 09:27:17 2016 -0700

    Merge pull request #1051 from StephenBonikowsky/RemoveBridgeFromBuildScript
    
    Removing Bridge related code from build script.

[33mcommit 880f6a41f11760f25d6a191d6daa5c8b7a9fcd7c[m
Merge: 7af1e3b 553c25e
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Wed Apr 20 18:42:26 2016 +0800

    Merge pull request #1055 from iamjasonp/sync-pr
    
    Revert "Simplify CI triggers" for innerloop tests

[33mcommit 553c25ee57ae72f1b117501ddae612253d3f25ac[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Wed Apr 20 18:34:38 2016 +0800

    Revert "Simplify CI triggers" for innerloop tests
    
    This reverts part of commit 0ab55905c26be86a96d4c66266a5da37a232bdd6.
    Triggers simplified in this way for innerloop seems to cause problems due to the
    way the utilities groovy class handles the optional parameter - basically
    specifying that parameter prevents innerloops from auto-triggering.
    For now, revert the simplification for innerloop tests

[33mcommit 7af1e3b5070a49240bb7bfcba4e995822471f07b[m
Merge: 68d5152 0ab5590
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Wed Apr 20 16:52:11 2016 +0800

    Merge pull request #1052 from iamjasonp/sync-pr
    
    Modify/simplify CI triggers and remove infrastructure branch

[33mcommit 0ab55905c26be86a96d4c66266a5da37a232bdd6[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Wed Apr 20 11:28:52 2016 +0800

    Simplify CI triggers
    
    Previously, we required a phrase "test [inner/outerloop] [platform] [Release/Debug]
    Remove the need to specify the Release/Debug phrase as we typically want to test
    both in one go anyway

[33mcommit dfa7406641cf3cdb706283ef9d639b891f75835d[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Wed Apr 20 11:26:55 2016 +0800

    Remove infrastructure branch from netci.groovy
    
    After the merge of #1041, we can remove testing from CI for the
    */infrastructure branch

[33mcommit 7eb5bc93c0d56d3d6ed8a00bc6908d5359cf7818[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Apr 19 15:41:30 2016 -0700

    Removing Bridge related code from build script.
    
    * With recent infrastructure changes we no longer need the bridge to be started by the build script.
    * At the same time PR dotnet/corefx #7894 adds setup and cleanup extension points to the build script.
    * As soon as this is merged in corefx I will port it to wcf.

[33mcommit 68d51527d9b050a16797b290cb69d968d56572d1[m
Merge: 3e4c6f8 e574674
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Apr 19 14:13:25 2016 -0700

    Merge pull request #1040 from roncain/apply-post-merge
    
    Update [Theory] to [ConditionalTheory] for failing test.

[33mcommit 3e4c6f8ae8eb07ab1a56422b6754d51fa39b6241[m
Merge: 71d610a 24d49c4
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Apr 19 13:58:39 2016 -0700

    Merge pull request #1049 from roncain/fix-startwcf
    
    Fix StartWcfService.cmd to reference exes relative to itself

[33mcommit 24d49c473d11f389637ef70b2151b3d5e9785721[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Apr 19 12:39:30 2016 -0700

    Fix StartWcfService.cmd to reference exes relative to itself
    
    Modified StartWcfService.cmd to locate its built executables
    relative to itself rather than the current working directory.
    
    Without this, running the script from any other folder will fail
    to start the self-hosted service.

[33mcommit 71d610a8f0e32e9ddb29e9fbb011583a0aa9c24a[m
Merge: ea3993f 4d6b887
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Apr 19 13:13:22 2016 -0700

    Merge pull request #1035 from StephenBonikowsky/MergeSharedScriptsWithCorefx
    
    Updating shared scripts.

[33mcommit e574674fac46bfdc9f508f4910ba2a852c088ba5[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Apr 19 13:02:56 2016 -0700

    Remove ActiveIssue from test fixed by this PR

[33mcommit d6e7a91d661e335c874f53372b62a6fdad7edac4[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Apr 19 06:17:32 2016 -0700

    Update [Theory] to [ConditionalTheory] for failing *nix test.
    
    Updates a single [Theory] to use [ConditionalTheory] so that a test
    requiring certificates does not fail when the setup steps are skipped.
    
    PR #1026 should have included this change, but I didn't see it until
    I ran the tests under Linux and deliberately skipped the cert setup steps.

[33mcommit ea3993fbf392bb49a76b6bc0920acac83ab25ab3[m
Merge: 532e680 c3a7ec9
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Apr 19 12:19:05 2016 -0700

    Merge pull request #1041 from roncain/merge-infrastructure-master
    
    Merge infrastructure branch to master

[33mcommit c3a7ec9ac3d81f026f78787417a89aa775372b47[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Apr 19 11:23:36 2016 -0700

    Add [ActiveIssue] to test that fails only in CI Outerloop

[33mcommit 7dbb7a4fc3dcc162ad3f947b76b2e13f9d7f3a12[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Apr 19 10:04:36 2016 -0700

    Add [ActiveIssue] to tests failing under new test infrastructure
    
    After rebasing on master and running tests under the new test
    infrastructure, several tests fail.  I opened new issues and
    applied [ActiveIssue] to only those tests that failed after the
    rebase.  After this PR, all tests should either pass or be skipped.

[33mcommit 4d6b887100d385ec4aba0e59b3446da20bfa1cb3[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Apr 18 15:00:00 2016 -0700

    Updating shared scripts.
    
    * Porting corefx PR #7724
    * Removing some WCF specific code that is no longer needed.

[33mcommit 6ee3ffbaf62333328ef6c219709087bda527697d[m
Author: hongdai <hongdai@microsoft.com>
Date:   Fri Apr 15 13:49:30 2016 -0700

    Get service FQDN from the server
    
    * We do not need to specify BridgeHost if we get it from the server.
    * The service side already has the GetFQDN method, no need to update the service.

[33mcommit f60e41fadadcfddd4721af3bfebb2c88cdaa6176[m
Author: hongdai <hongdai@microsoft.com>
Date:   Tue Apr 12 15:50:37 2016 -0700

    Init checkin of client changes
    * Allow Test property ServiceUri.
    * Note that we in the self hosted case, we only allow localhost, not multiple
    service instances(localhost/wcfservice1).
    * Change all endpoints point to the new services.

[33mcommit 532e680b58a3b6550f3535e299e7cae2164b37a2[m
Merge: 18cd79b 3293142
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Apr 19 05:47:37 2016 -0700

    Merge pull request #1026 from roncain/apply-conditionalfact
    
    Apply [ConditionalFact] to some tests

[33mcommit 3293142e0924810aa8287974c032aac190bbaa20[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Apr 19 05:12:49 2016 -0700

    Apply [ConditionalFact] to tests known to need them.
    
    Cleans up the implementation of ConditionalWcfTest, and
    adds the initial set of conditions.  This iteration uses
    only a simple heuristic to "detect" whether these conditions
    are true, and more complete detectors will follow after
    integration with the new test infrastructure.
    
    This PR also adds [ConditionalFact] to those tests known to
    require them right now.  More detailed additions will follow
    after integration with the new test infrastructure.

[33mcommit 18cd79b3ee4883f72adcc2b070b3258b5cd06fc9[m
Merge: dee1307 467ef93
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Tue Apr 19 15:41:51 2016 +0800

    Merge pull request #1039 from iamjasonp/sync-pr
    
    Fix netci.groovy code coverage jobs

[33mcommit 467ef93f46025ba2da0e11a2abca8f1d4afba44a[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Tue Apr 19 15:33:41 2016 +0800

    Fix netci.groovy code coverage jobs

[33mcommit dee13077e2b30bec7cb93018b264dd17781f3ddb[m
Merge: 3b7369b 4012a7c
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Tue Apr 19 15:09:37 2016 +0800

    Merge pull request #1032 from iamjasonp/sync-pr
    
    Change netci.groovy to pass ServiceUri in outerloops

[33mcommit 3b7369b00a79fad96328e0ad7919a77483b128a6[m
Merge: cda9253 072d4a4
Author: Matt Connew <mconnew@microsoft.com>
Date:   Mon Apr 18 17:06:50 2016 -0700

    Merge pull request #1031 from jamesqo/async
    
    Remove uses of async/await where not needed

[33mcommit 8a85d7b3ac9fb26205509da904dc59b09622f4fe[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Mon Apr 18 15:12:43 2016 -0700

    Make X509ServiceCertificateAuthentication constructable
    
    Also modified a test to make enabling this being tested quick (remove a line,
    uncomment a line).

[33mcommit 6496d5b6736fbc9cd450fb42e044107eadc90613[m
Author: Zhenlan Wang <zhenlwa@microsoft.com>
Date:   Mon Apr 18 16:00:35 2016 -0700

    Script to setup WCF test services hosted on IIS for the testing of WCF on .NET Core

[33mcommit cda9253c8cd4bf3b85ac021fc648b903aa6a2525[m
Merge: 3fab381 3c95841
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Apr 18 13:07:59 2016 -0700

    Merge pull request #1033 from StephenBonikowsky/UpdateActiveIssueForIssue834
    
    Update the ActiveIssue number to point to correct Issue.

[33mcommit 4012a7c78c0b81811f2bc15358ab9387ceee7547[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Tue Apr 19 02:57:51 2016 +0800

    Fix typo in netci.groovy for non-Windows outerloop
    
    The Outerloop tests for Linux were scripted as a Windows batch file rather
    than as a shell script, causing all non-Windows outerloop tests to fail

[33mcommit 3fab3819fee004facef2d0ef8836a93aabefdf03[m
Merge: 0f9671b 0087c3d
Author: Hong Dai <hongdai@microsoft.com>
Date:   Mon Apr 18 11:44:36 2016 -0700

    Merge pull request #1027 from hongdai/startwcfservice
    
    Init checkin StartWCFService.cmd

[33mcommit 3c95841af6e98c413134b02d4fcb0d2003af24ba[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Apr 18 11:16:08 2016 -0700

    Update the ActiveIssue number to point to correct Issue.
    
    * The fix for Active Issue #833 will also fix Issue #834, therefore updating the two tests in question to point to 833 so we can close Issue #834.
    * Fixes #834

[33mcommit 163e9f527534171903169a98894ea2bebb00e25b[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Tue Apr 19 02:05:37 2016 +0800

    Change netci.groovy to pass ServiceUri in outerloops
    
    The new WCF infrastructure will require the ServiceUri to be passed in via command
    line when running outerloop tests.
    
    As an aside, also change references to "Url" to "Uri", to stay consistent

[33mcommit 0f9671b6e784359224283a8581fcbf3998705003[m
Merge: e12cf24 65e8ccf
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Apr 18 10:01:27 2016 -0700

    Merge pull request #1030 from StephenBonikowsky/UsingNewPSScriptToUpdatePackageVersion
    
    Update version to 24015-00 using new PS script.

[33mcommit 0087c3de868afcc72ee2981536651d8b9e839bed[m
Author: hongdai <hongdai@microsoft.com>
Date:   Mon Apr 18 09:45:18 2016 -0700

    Init checkin StartWCFService.cmd
    
    * also move BuildWCFSelfHostedService.cmd to scripts dir.

[33mcommit 072d4a4b997409b69a45fb4dc93602b6fdbf54f2[m
Author: James Ko <jamesqko@gmail.com>
Date:   Sat Apr 16 00:07:21 2016 -0400

    Remove `return awaits` where not needed

[33mcommit 87a32780875f748b341c58016b0067d6eb5d987d[m
Author: James Ko <jamesqko@gmail.com>
Date:   Fri Apr 15 23:56:00 2016 -0400

    Avoid unnecessary async/await in MessageContent

[33mcommit 65e8ccf1eef848436586e0f7916ef445fe7366cc[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Apr 15 15:09:04 2016 -0700

    Update version to 24015-00 using new PS script.
    
    * Checking-in a simpler version of a corefx script to update our repo to use the latest package version.
    * Updating from rc3-23931-00 to rc3-24015-00

[33mcommit e12cf24fab1bad02268425b1582b3fe024eebe40[m
Merge: 761659e 7079d3c
Author: Matt Connew <mconnew@microsoft.com>
Date:   Fri Apr 15 13:58:52 2016 -0700

    Merge pull request #974 from mconnew/StreamingPerformance
    
    Implementing async readahead with sync read on StreamFormatter

[33mcommit 761659ef03e4f9f78cac6d148b9320329d65bc9c[m
Merge: ab7a259 aef3027
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Apr 15 11:02:19 2016 -0700

    Merge pull request #1018 from StephenBonikowsky/PortBuild-testsScript
    
    April 14 Script Sync

[33mcommit ab7a2598e885f5a1ff1c73508a0dfaafb58d0935[m
Merge: b47c0cc e5744a4
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Sat Apr 16 01:02:55 2016 +0800

    Merge pull request #1023 from iamjasonp/sync-pr
    
    Change CI Windows Outerloop trigger phrase

[33mcommit aef302746c37f2f11a08d0eaaadd0f7b0784e482[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Apr 14 10:03:21 2016 -0700

    Updating .sh files and other changes.
    
    * Synced or added .sh files including'run-test.sh' even though we don't currently use it, we may need to at some point.
    
    * .sh files not tested on Linux.
    
    * Added the publish-packages files but did not test them.
    
    * Pulled in a few more changes to various files from PRs in corefx occuring within the last 24 hrs.

[33mcommit 7079d3c448f5de62a8cae60a2ea38e0aeb657082[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Wed Apr 13 19:21:37 2016 -0700

    Adding new scenario test with custom message encoder

[33mcommit c4de4021e7858c5dd8188ce31d296db85333101b[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Fri Mar 25 20:02:54 2016 -0700

    Implementing async readahead with sync read on StreamFormatter

[33mcommit b47c0cc0883486aa8b8fee88865074658e0fffd5[m
Merge: 633d25f 2151978
Author: Eric StJohn <ericstj@microsoft.com>
Date:   Fri Apr 15 08:59:48 2016 -0700

    Merge pull request #1025 from ericstj/harvestInternalPackageDeps
    
    Harvest internal package deps

[33mcommit 633d25f30e0ffea7522b59cce494dedaa4beabde[m
Merge: 650056e 7cbdcdd
Author: Hong Dai <hongdai@microsoft.com>
Date:   Fri Apr 15 07:41:28 2016 -0700

    Merge pull request #1020 from hongdai/certutil
    
    Add BouncyCastle package dependency

[33mcommit 21519788adaf99f918b23686c120110a5f87753c[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Fri Apr 15 06:23:35 2016 -0700

    Update buildtools to 1.0.25-prerelease-00314-03

[33mcommit 6f9d5c97f12a02abe96b0ed2a939c62d8db1067a[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Thu Apr 14 17:18:07 2016 -0700

    Remove internal project package references
    
    These are no longer needed with the latest buildtools.  They'll be
    harvested just like other dependencies.

[33mcommit 650056ebf0a03ec89a6243c4738a72c4105d1bfa[m
Merge: 9d782af 6bd168e
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Apr 15 05:46:00 2016 -0700

    Merge pull request #1001 from roncain/callonce_fix
    
    Fixes CallOnceManager to signal its blocked waiters when channel is unusable

[33mcommit e5744a457094d8f1cb3692be6a245c8df0fe34bc[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Fri Apr 15 15:53:22 2016 +0800

    Change CI Windows Outerloop trigger phrase

[33mcommit 9d782af7713525043848e27c3ea081f5063f54c2[m
Merge: 03753b6 7511d0c
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Fri Apr 15 13:11:45 2016 +0800

    Merge pull request #1013 from iamjasonp/sync-pr
    
    Add support for building rc2 branch in CI

[33mcommit 7511d0cc28a2c80d2d1583ae19e4e5c41b7a3221[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Fri Apr 15 13:08:56 2016 +0800

    Remove CentOS from outerloop testing platforms
    
    ... until we get innerloops building successfully

[33mcommit b958816778c4508ed706663e9757fee07440904b[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Wed Apr 13 15:56:16 2016 +0800

    Add support for building rc2 branch in CI

[33mcommit 5aea308aafe8b8522eb8fd215073410abc52d639[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Apr 14 09:13:30 2016 -0700

    Porting corefx PR 6572 - Adding test builds files
    
    * Adopt the changes made via corefx PR 6572 to shared scripts.
    * Add new test .builds files for WCF.

[33mcommit da2335a5f3c3475efb5157606aee61db9398d181[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Apr 13 15:18:38 2016 -0700

    Build-tests.cmd file now in sync
    
    * This PR adds build-tests.cmd and makes sure any corefx PRs that touched this file have been synced as well.

[33mcommit 03753b6d8466353357f001541d289b9dca76c286[m
Merge: 3a45eda 3963186
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Apr 14 19:28:47 2016 -0700

    Merge pull request #1021 from StephenBonikowsky/WorkaroundBuildBreak
    
    PR 1019 broke CertificateCleanup.csproj

[33mcommit 39631868ba3d202d879444e6b643e199a2f24028[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Apr 14 19:00:24 2016 -0700

    PR 1019 broke CertificateCleanup.csproj
    
    * This is a temporary workaround to get CI checks working again.

[33mcommit 7cbdcdd38f4dce4e74df8eb4927110f6f7c8c865[m
Author: hongdai <hongdai@microsoft.com>
Date:   Thu Apr 14 16:39:26 2016 -0700

    Add BouncyCastle package dependency
    
    * The build will fail on a new machine because the dependency is not defined

[33mcommit 3a45edabe3ab36606113cb027966f5664bca71ff[m
Merge: d406edc 23db1ad
Author: Hong Dai <hongdai@microsoft.com>
Date:   Thu Apr 14 13:15:43 2016 -0700

    Merge pull request #1019 from hongdai/certutil
    
    Init certutil checkin

[33mcommit 23db1ad990dd976ada3b77f2ad2ae64ecca10c75[m
Author: hongdai <hongdai@microsoft.com>
Date:   Thu Apr 14 12:24:58 2016 -0700

    Init certutil checkin
    * I have created dir.* in the tools directory, will remove the ones
    in the SelfHostedWcfService direcotry in the future.

[33mcommit d406edcbb03eb39033cf56fd6f3add1f84e5f942[m
Merge: 1ce2f4d 0d819e4
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Fri Apr 15 00:50:35 2016 +0800

    Merge pull request #1014 from iamjasonp/update-readme-badges
    
    Update README.md to reflect new queue names

[33mcommit 1ce2f4dd4f517c1e4b0682cac5f41037e50b85a8[m
Merge: 3140f39 99f56b7
Author: Hong Dai <hongdai@microsoft.com>
Date:   Thu Apr 14 09:13:28 2016 -0700

    Merge pull request #1011 from hongdai/selfhostservice
    
    Init check in self host service

[33mcommit 3140f39fe9d54dd7dba63f731b9a0dd5ef79fcd0[m
Merge: c6a01e7 9d09dbd
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Apr 14 09:00:31 2016 -0700

    Merge pull request #1009 from StephenBonikowsky/April13ScriptSync
    
    April13 script sync

[33mcommit 0d819e4034a1e8ad59d3c67c4f1724e8e684903f[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Apr 14 16:16:43 2016 +0800

    Update README.md to reflect new queue names

[33mcommit 99f56b77fa306b820a971f52d287fc7327ad1abb[m
Author: hongdai <hongdai@microsoft.com>
Date:   Wed Apr 13 20:29:06 2016 -0700

    Init check in self host service
    
    * Also add missing TcpDefault service activation. It has caused serveral security
    tests failures.

[33mcommit 9d09dbd0da46f88abda67b3a1e85aca54dd9dc40[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Apr 13 14:21:19 2016 -0700

    Porting corefx PR 7565 and 7610 plus other small changes.
    
    * SHA:a188beafd2a96366a7ef9fa6e52d14904040fe83 - Move PerfTesting from src\dirs.proj to build.proj
    * SHA:0a04be98211d540d9b484526137e86062ddf8a32 - Add NETStandard.Library to CoreFx

[33mcommit 08e4b3e58172e4011fcc25f6698fe4bfe7977df9[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Apr 13 11:21:46 2016 -0700

    Porting corefx PR 7419 and 7199 and 7251 and 7373
    
    * SHA:2d03d3edfe819253aba263b42d0d4d1b43af3b4e - Make sure Packagesdir exists before restoring packages
    * SHA:5b72e230b8c60bd926d03590d74ca1286eb23387 - Minor fix to clean when called without parameters
    * SHA:011b0103351455243b7c770281a055503e56900f - Make "@echo off" configurable.
    * SHA:d2e94e5dadd7ffeaf8eab38d8810a3ffab909f9f - Add support for additional properties passed to msbuild
       - Did not update the .sh file, want to update those types of files separately and all together.
       - The file 'build-tests.cmd' has not been added yet.

[33mcommit bb4a47e0fe2a1e89abc408a08afbd3cb620af3f5[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Apr 13 09:51:01 2016 -0700

    Porting corefx PR 7327
    
    * SHA:eb3a7f65489ff8aff0baeb4dd73279b4da13eeed - Add autogeneration for netstandard support table

[33mcommit c6a01e7e981cd4510e3f4e857d73d2566b7138b7[m
Merge: b1f7748 1901578
Author: Eric StJohn <ericstj@microsoft.com>
Date:   Wed Apr 13 10:48:24 2016 -0700

    Merge pull request #1008 from ericstj/pcl-inbox
    
    Add placeholders for PCL profiles

[33mcommit 1901578da7ac1f82dfb09c57d87a18ce9d3e7cf9[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Wed Apr 13 10:37:13 2016 -0700

    Add placeholders for PCL profiles
    
    This is the WCF version of https://github.com/dotnet/corefx/pull/7665
    
    We need this in ASAP because the WCF repo is broken in internal builds
    since it is built with the CoreFx version of buildtools.

[33mcommit b1f7748fe563ab80d17a196d814f0eb85340bab4[m
Merge: b453654 b0b5ee3
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Apr 13 09:35:17 2016 -0700

    Merge pull request #1004 from roncain/xunit.netcore.extensions
    
    Update to latest xunit.netcore.extensions

[33mcommit b0b5ee396d42c541d0fa2b4d4e7852333a2ad26a[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Apr 13 09:23:49 2016 -0700

    Updated dir.props to enforce the right xunit.netcore.extensions version

[33mcommit d63196bb0485bac4d239d249a76337f5ddc59ce0[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Apr 13 06:20:57 2016 -0700

    Update to latest xunit.netcore.extensions
    
    This update allows us to use multiple conditions in
    [ConditionalFact] tests.
    
    This feature was added to xunit.netcore.extensions with
    PR https://github.com/dotnet/buildtools/pull/619, and now
    that the latest package is available, we can use it.

[33mcommit b4536548525b6357b1c7ebb854a1c4a2cdfd9efb[m
Merge: 3d2f9b3 68ed5a3
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Apr 13 09:01:21 2016 -0700

    Merge pull request #1003 from StephenBonikowsky/April11ScriptSync
    
    April11 script sync

[33mcommit 3d2f9b346c79b798a530f1a2ab5e602b35e009ea[m
Merge: 364631d 2883507
Author: Zhenlan Wang <zhenlwa@microsoft.com>
Date:   Tue Apr 12 19:16:09 2016 -0700

    Merge pull request #1000 from dotnet/DisableDebugCompilation
    
    Disable the debug mode compilation of WCF service

[33mcommit 68ed5a349d2be91cfbeb6ba5f664fe0361657c50[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Apr 12 17:08:46 2016 -0700

    Porting corefx PR 7651 and 7509
    
    * SHA:677ffb86351f93b5017beeddab1ca91821abaf79 - Fix project.json and csproj errors noticed by ericstj
    * SHA:c3703d3b151073c41c8d9d57cc4e19c795864577 - Fix ReportGenerator task

[33mcommit 3f4173f2702c37f43cfd22a24372c7ea5877531e[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Apr 12 16:48:42 2016 -0700

    Porting corefx PRs 7135 and 7056
    
    * SHA: 0f268ed1079c182d71cdf49cde3f339a01d67863 - Temporarily revert implementation assembly version to match package version
    * SHA:25bbfea64828d423becd32e4aa56bd821d93860e - Move dependency validation patterns to Item metadata and add programmatic names.
    * Also includes a few additional minor old changes.

[33mcommit 3849823856f57e6f22a4bf2dd69831a6cb7290a3[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Apr 12 15:57:12 2016 -0700

    Porting corefx PRs 7420 and 7422
    
    * SHA:36692c3f15467d7f53fe2a506612f155134e754a - Add test runtime project.json support running against desktop
    * SHA:4c426808e2c1c1b122d5950a782665d656d2f030 - Add initial support for publishing packages.

[33mcommit 159931e83a364ed4ed9e81a4b838bd3e74c0fa43[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Apr 12 15:38:36 2016 -0700

    Porting corefx PR 7401
    
    * SHA:e569cb5849c72b12f1ced7a634ea7dfb8b40c181 - Enable running the analyzers from BuildTools during the CoreFx build.

[33mcommit 1bb39a4eebc274769529a2a810cfdfffa2dd8999[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Apr 12 10:09:49 2016 -0700

    Porting corefx PR 7309
    
    * SHA:d46ecb53011b18a0a30cd200390a537402096971 - Consuming new versioning changes

[33mcommit b9e9d881dcfa997b8f5d76d9d24ff778bb61615a[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Apr 12 09:41:47 2016 -0700

    Porting corefx PRs 7147 and 7336
    
    SHA:59844e6bf00b6df5b95e795630619140ce149bb6 - Build packages step
    SHA:2fc4efcb6c7a977ccc679964586ef3bf0b23311f - Add SkipNativePackageBuild and SkipManagedPackageBuild to disable native or managed package build

[33mcommit 364631dc444578083d48d7744af2efeea134bc78[m
Merge: b0a3256 0eeb5f5
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Apr 12 14:57:52 2016 -0700

    Merge pull request #996 from mellinoe/test-runtime-projectref
    
    Update buildtools, update use of test-runtime

[33mcommit 0eeb5f540528e55b2a67b1d1232dba964d4da343[m
Author: Eric Mellino <erme@microsoft.com>
Date:   Tue Apr 12 13:32:13 2016 -0700

    Include all runtimes in project.json
    
    This change adds centos, rhel, and debian to the list of runtimes in each test project.json.

[33mcommit 6bd168e47b8a58e5b4347bfcb2c812c1310c93b2[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Apr 12 12:05:10 2016 -0700

    Fixes issue where CallOnceManager did not signal its waiters
    
    Background: When a proxy is not opened explicitly before use, the
     CallOnceManager class does the open on the first service call.
     Any other calls made during the open are queued, and when the
     open completes are called in FIFO order.
    
    Problem: the queued calls block on a WaitHandle that is set
     only by the previous operation succeeding or by timeout. But
     if the channel faults, there is no logic to set the WaitHandle,
     so it will be set only after the timeout expires. And then the
     next queued operation begins its own timed wait and so on. Code
     in this state can easily lead to large numbers of blocked threads
    
    Solution: detect when the channel faults or is closed and signal
     the waiter's WaitHandles to wake up and complete their operation
     (typically a failure because the channel is not usable by then).
    
    Fixes #108

[33mcommit 28835073e3b29f64cbd640c7aece6c65ca6ac7b6[m
Author: Zhenlan Wang <zhenlwa@microsoft.com>
Date:   Tue Apr 12 11:35:48 2016 -0700

    Disable the debug mode compilation of WCF service
    
    Debug mode could take way too long and slow down our start up unnecessarily.

[33mcommit b0a3256f61598df2f1a4540591526e94c76f87a2[m
Merge: 0a9f9be 4f4889e
Author: Hong Dai <hongdai@microsoft.com>
Date:   Tue Apr 12 10:17:51 2016 -0700

    Merge pull request #995 from hongdai/iiswcfservice
    
    Init Check in IIS Hosted WCF Test Service

[33mcommit 4f4889e5aa11b7d9d6e9b6c19236a85e33996d95[m
Author: hongdai <hongdai@microsoft.com>
Date:   Tue Apr 12 10:07:21 2016 -0700

    Init Check in IIS Hosted WCF Test Service
    
    * It includes all the sources needed for the IIS hosted WCF Test Services.
    * Web.config defines services activation.

[33mcommit a439d4c981cc0eb2abfafe6e6fbae91900d1df7b[m
Author: Eric Mellino <erme@microsoft.com>
Date:   Tue Apr 12 07:19:54 2016 -0700

    Update to latest version of buildtools

[33mcommit 0a9f9be414920c550f203b10853afc7a7b6d4272[m
Merge: d9516fe a02eb88
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Tue Apr 12 11:34:16 2016 +0800

    Merge pull request #994 from iamjasonp/sync-pr
    
    Remove latest dependences build from netci.groovy

[33mcommit 0eaf4e46b3e90d5e661c2a15f552622a44c2d6a6[m
Author: Eric Mellino <erme@microsoft.com>
Date:   Mon Apr 11 11:42:15 2016 -0700

    Update buildtools, update use of test-runtime

[33mcommit a02eb88df64c668bebc6c24662ec5b0844a59fd6[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Tue Apr 12 00:56:31 2016 +0800

    Remove latest dependences build from netci.groovy
    
    ... but for now, keep the command around as a comment in case we need it for
    the near future. We can remove the comment later should we find that we haven't
    needed it for a long time

[33mcommit d9516fe6a1186f32923148924bb3b0bb8aea5202[m
Merge: c96ddec b01d424
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Mon Apr 11 17:28:04 2016 +0800

    Merge pull request #993 from iamjasonp/sync-pr
    
    Change quotation in netci.groovy

[33mcommit b01d42495c128e361e7a4dd345076fe23d06efa2[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Mon Apr 11 17:02:15 2016 +0800

    Change quotation in netci.groovy
    
    WCF netci.groovy was previously using ''' as a quoting mechanism, but that prevented
    variables from being evaluated. Change the ''' to "

[33mcommit c96ddecf762175ebca7bceda59df5aa827f4b524[m
Merge: 3e49e9f c655755
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Mon Apr 11 16:17:09 2016 +0800

    Merge pull request #992 from iamjasonp/sync-pr
    
    Disable non Windows_NT outerloops in netci.groovy

[33mcommit c655755e9d51dc1000a085bf4466e5c10e133d9f[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Mon Apr 11 16:08:23 2016 +0800

    Disable non Windows_NT outerloops in netci.groovy
    
    Disable the outerloop builds that require an external server because the server
    has not yet been fully provisioned

[33mcommit 3e49e9f0e029a4bd86f3c0fb48c9b92cb09d3acd[m
Merge: 65f3ff2 dc3e199
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Mon Apr 11 14:47:49 2016 +0800

    Merge pull request #991 from iamjasonp/sync-pr
    
    Modify netci.groovy ${osGroup} vs ${os} mapping

[33mcommit dc3e199d222a863c7d71d0d87390f049299587ef[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Mon Apr 11 14:32:45 2016 +0800

    Modify netci.groovy ${osGroup} vs ${os} mapping

[33mcommit 65f3ff2f87e023a26958a6f1916c9bc534704855[m
Merge: 3e304e1 a75a029
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Mon Apr 11 14:05:28 2016 +0800

    Merge pull request #932 from iamjasonp/sync-pr
    
    Modify netci.groovy to include new sync-pr scripts

[33mcommit a75a0295e200454fdfd5249a0498f0c9c9ad02b1[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Sat Apr 9 02:24:49 2016 +0800

    Sync netci.groovy to corefx and rewrite

[33mcommit 3e304e1d2a1cc1e9d748fd039eff782ebea3659e[m
Merge: e78c57e 748c7cf
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Apr 8 04:13:17 2016 -0700

    Merge pull request #978 from roncain/conditional-tests
    
    Add support for [ConditionalFact] based on TestProperties

[33mcommit f10f0ed23ebd9d76b6bcaf16fb05fd654fd9448a[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Fri Apr 8 14:47:36 2016 +0800

    Modify netci.groovy for PRService outerloops
    
    This change allows outerloops in Jenkins to access the PR service in two modes:
    - PR mode, which is used for _prtest builds
    - Branch mode, which is used for the timed and post checkin builds

[33mcommit e7771faf288d3b31b27d059a8fe9fee7bdfdec33[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Apr 7 20:02:40 2016 +0800

    Update sync-pr scripts to allow for branch and pr

[33mcommit e78c57eb8792130e898a6ba2269d493db6b45ebf[m
Merge: 5fe1c1d 0a859ad
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Fri Apr 8 15:24:36 2016 +0800

    Merge pull request #989 from iamjasonp/sync-pr-scripts
    
    Update sync-pr.cmd and sync-pr.sh

[33mcommit 0a859ad6015224bdb81460be44889ebbbd13375e[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Fri Apr 8 15:15:13 2016 +0800

    Move sync-pr.cmd/sh to tools/scripts directory

[33mcommit 2f903358c9fa218531aa1f6eec79ccc62b82dbcb[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Fri Apr 8 15:14:03 2016 +0800

    Update sync-pr.cmd and sync-pr.sh
    
    * sync-pr.cmd and sync-pr.cmd used some old (presumed) service URIs
      Update the usage banners to more closely reflect the new URIs that we have
      finalized in the last round of planning
    * Update sync-pr scripts to allow for branch and pr synchronization

[33mcommit 5fe1c1de849af967adc57bb50fe296bf8c6b14ee[m
Merge: 3ea4dbb 04512f8
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Fri Apr 8 14:06:23 2016 +0800

    Merge pull request #987 from iamjasonp/prservice
    
    Add ability to use PRService on branches

[33mcommit 04512f8aa0ad29d09baf15d76c1d64b75ebc6690[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Apr 7 17:02:20 2016 +0800

    Add ability to use PRService on branches
    
    * Change some error handling mechanisms

[33mcommit 3ea4dbbd640c6b41dc5f5e1dc22605f5e4ad896e[m
Merge: 61b93e0 d5c3b95
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Fri Apr 8 12:11:17 2016 +0800

    Merge pull request #980 from iamjasonp/update-manualtestguide
    
    Add clarifications to manualtest-guide.md

[33mcommit 61b93e035cde9a79e6df8fb1ac77ca7deda4edc8[m
Merge: 9f65e9b 5fdb56e
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Apr 7 06:55:14 2016 -0700

    Merge pull request #984 from ericstj/fixPrivate.SM.package.dep
    
    Fix TFMs for System.Private.ServiceModel deps

[33mcommit 3a0cb51cafdf15762ae0cea0014c6f4ef3d5d069[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Apr 7 15:02:41 2016 +0800

    Modify netci.groovy to reflect real PRService URI

[33mcommit d5c3b9524b74dd79e8955e7e3ce7d69b1a796ea9[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Tue Apr 5 17:59:19 2016 +0800

    Add clarifications to manualtest-guide.md
    
    * Clarified how to set up for Kerberos on Linux
    * Removed manual guide for Https/ClientCredentialTypeTests.cs

[33mcommit 71324ed1369e2a16028199274b6f485df4dbd8a8[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Mar 31 16:14:28 2016 +0800

    Change order of commands in netci.groovy
    
    Modifies the order of the commands being run so that the call out to the PR
    sync service comes before build.sh/build.cmd command

[33mcommit 477572ae1e2dbef8a4763d50b6d0546ce7fcb5b8[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Wed Mar 23 14:39:31 2016 +0800

    Modify netci.groovy to include new sync-pr scripts

[33mcommit 9f65e9b724d35af531c6fc33231024aa14bb55fc[m
Merge: a22619e 7403026
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Thu Apr 7 14:17:13 2016 +0800

    Merge pull request #986 from iamjasonp/prservice
    
    Fix pr.ashx returning false negatives running git

[33mcommit 7403026eda2a3c1a59ada90d043e17396b0e0e33[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Apr 7 14:05:08 2016 +0800

    Fix pr.ashx returning false negatives running git
    
    RunGitCommands() would return false in the case where the method was passed
    zero commands to run. Now, if it's passed with a null or empty array, we will
    bail out early and return true to prevent the rest of the script from failing

[33mcommit a22619e4cce51f29470a3d90f73e4199b90db0c3[m
Merge: 5db2ebd eb99824
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Thu Apr 7 10:50:41 2016 +0800

    Merge pull request #981 from iamjasonp/prservice
    
    Add pull request service for test infrastructure

[33mcommit eb99824f454cde7a8e67c28c685fbf7c8b600696[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Wed Apr 6 14:55:41 2016 +0800

    Rename PR service and improve error handling
    
    * Rename sync.ashx to pr.ashx
    * Remove SHA sync handling
    * Change error handling - show more helpful and detailed error messages
    * Add distinction between 400 and 500 type errors
    * Added copyright notice
    * More description on top of file about assumptions held by script

[33mcommit 5fdb56e6d603756fe24fa0798d72d3ba9774a053[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Wed Apr 6 14:50:44 2016 -0700

    Fix TFMs for System.Private.ServiceModel deps
    
    The TFM for these dependencies is incorrect, causing the S.P.SM to be
    missed when not targeting netstandardapp1.5 (eg for a portable app).
    
    Fix this TFM to match the implementation TFM.
    
    I've filed a buildtools issue https://github.com/dotnet/buildtools/issues/617
    to help eliminate the need for this completely.

[33mcommit 748c7cf525483233c97e6135ea22b0b7bb7bfdcc[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Apr 4 10:30:33 2016 -0700

    Add support for [ConditionalFact] based on TestProperties
    
    We need a way to optionally run tests that are not normally
    run by default.  This includes tests that have special requirements
    for setup or environment.
    
    We also need such tests to show as "skipped" rather than being
    silently stripped from the test results, as happens with the
    xunit -notrait option.
    
    This PR introduces the framework to compose these kinds of test
    conditions using TestProperties and adds some of the known conditions.
    Adding [ConditionalFact] to individual tests will be done in separate PR's.

[33mcommit 5db2ebd950ea7501403136b0696b34d4ee95bcea[m
Merge: 2f7f055 87654bf
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Wed Apr 6 23:25:48 2016 +0800

    Merge pull request #982 from iamjasonp/ambientcredentials
    
    Modify NegotiateStream ambient credential tests

[33mcommit 87654bf779be52ed14e27e0cac7cc238017b3662[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Wed Apr 6 16:45:22 2016 +0800

    Modify NegotiateStream ambient credential tests
    
    In Helix, the runs do not spew out as much information as they do when running
    locally. We need to remove the Assert.True() at the end (used for showing the
    credentials used) so that the call stacks are shown.

[33mcommit f682c603c43ae9a6efe188e8aa78855cc8660f64[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Tue Apr 5 19:50:07 2016 +0800

    Add pull request service for test infrastructure

[33mcommit 2f7f055a1d9dc6b13ba0405f1dece0fa68931b26[m
Merge: 64d63b5 f85dc3c
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Apr 4 04:18:55 2016 -0700

    Merge pull request #976 from jkotas/remove-r2r-workaround
    
    Remove ReadyToRun workaround

[33mcommit f85dc3cc3481131b12462be1d97e3118a72fe302[m
Author: Jan Kotas <jkotas@microsoft.com>
Date:   Sun Apr 3 08:43:13 2016 -0700

    Remove ReadyToRun workaround

[33mcommit 64d63b511cf1ae7c0f71da730a0558d8e6ee775d[m
Merge: 714b079 f38b008
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Apr 1 15:33:22 2016 -0700

    Merge pull request #975 from StephenBonikowsky/UpdateRC2SupportedFeatures
    
    Update rc2 supported features

[33mcommit f38b008091cc8626e1f12b46d435cdd5522cb507[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Apr 1 13:52:13 2016 -0700

    Update the Supported Features for rc-2 release

[33mcommit f729d8ff40330348614b1f23a53fbfd28ad743f3[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Mar 25 14:00:06 2016 -0700

    Update the Supported Features markup file for RC2.
    
    * Adding table columns for Linux flavors.

[33mcommit 714b0795c8ff2054d321d34a992616f76cc39b30[m
Merge: b3a51c7 ff9abeb
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Apr 1 09:55:39 2016 -0700

    Merge pull request #964 from StephenBonikowsky/Issue835_UpdateTest
    
    Updating test to be a little clearer about what it is doing.

[33mcommit ff9abeb0327355126ec7221642b63c349067b8a8[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Mar 31 14:44:06 2016 -0700

    Updating test to be a little clearer about what it is doing.
    
    * Also updating it to use more recent test syntax and to use Assert.Throw to verify expected exceptions.

[33mcommit b3a51c7e4eb94207938147dd35a659aac28e5c0a[m
Merge: 5f47487 a2d1d9b
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Apr 1 09:30:58 2016 -0700

    Merge pull request #966 from roncain/update-versions
    
    Update to latest package versions

[33mcommit 5f4748719ef2a68de8f2846bb51678dab7e3dd88[m
Merge: e187b23 bd4a602
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Apr 1 09:17:25 2016 -0700

    Merge pull request #965 from StephenBonikowsky/Issue951_OSX_SetLingerOption
    
    Adding OS X ActiveIssue for tests failing due to Issue #951

[33mcommit bd4a6023aaffe8c02900595d5866c3da5cd199e3[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Mar 31 15:31:01 2016 -0700

    Adding OS X ActiveIssue for tests failing due to Issue #951
    
    * Issue #951 is blocked by dotnet/corefx#7403

[33mcommit a2d1d9b9bac2b21c627eb2b22dbdd6ca406f119a[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Apr 1 08:39:48 2016 -0700

    Update xunit.netcore.extensions version
    
    This is necessary so that [ActiveIssue] uses the correct
    version of System.Runtime.InteropServices.RuntimeInformation.

[33mcommit f256aa1eb0e40e45b6973dd362e00939163ca805[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Apr 1 07:07:25 2016 -0700

    Update dir.props
    
    Updating dir.props to match https://github.com/dotnet/corefx/pull/7411 .
    This is necessary to combine withe the update to build tools version.

[33mcommit aa78873461f3c4b69df10d995e327e1488558653[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Apr 1 06:53:15 2016 -0700

    Update to latest package versions
    
    Updates to package versions 23931-00 and latest build tools.

[33mcommit e187b238dce5fb5d7f87dea59af027e745102053[m
Merge: bfa505b f6b8e05
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Fri Apr 1 12:37:13 2016 +0800

    Merge pull request #947 from iamjasonp/update-manualtestguide
    
    Update manual test guide for Linux HTTPS/Kerberos tests

[33mcommit bfa505b56babe837fc5ae7d89a07dd2d33d3a682[m
Merge: 9fb979a 475aaa1
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Mar 31 13:55:00 2016 -0700

    Merge pull request #963 from dagood/upgrade-cli
    
    Upgrade dotnet cli to 1.0.0-beta-002173

[33mcommit 9fb979a3cf37a3d59464ed2fbf52cf2707417dc7[m
Merge: b69da09 d902089
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Mar 31 13:22:32 2016 -0700

    Merge pull request #961 from StephenBonikowsky/master
    
    Updating active issue numbers on two tests.

[33mcommit 475aaa1a7185e6f14c390f99edf0b678ca0aecff[m
Author: Davis Goodin <dagood@microsoft.com>
Date:   Thu Mar 31 14:55:16 2016 -0500

    Upgrade dotnet cli to 1.0.0-beta-002173.

[33mcommit d9020890fff60d7e23f9a396aba867bef7913c14[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Mar 31 11:13:36 2016 -0700

    Updating active issue numbers on two tests.
    
    * Two tests using active issue #544 have been updated to different and more accurate issues.
    * Fixes #544

[33mcommit b69da092b5ec7c1de41bc8ff1e51ccc45cf4dc97[m
Merge: 6559a1b 310996d
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Mar 31 06:19:25 2016 -0700

    Merge pull request #957 from dagood/upgrade-cli
    
    Upgrade dotnet cli to take win7 fix

[33mcommit f6b8e0589408e8bad37e9f0133782e8502eb1912[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Mar 31 14:38:04 2016 +0800

    Add clarifications to ambient credential test instructions

[33mcommit 310996df85bd93c12b48b2e2bef78ecd2f7d7c64[m
Author: Davis Goodin <dagood@microsoft.com>
Date:   Wed Mar 30 17:24:32 2016 -0500

    Upgrade dotnet cli to take win7 fix.

[33mcommit 6559a1b3a0b0aca71c7cf390af36c3170e1702db[m
Merge: 20ffb5a 8ed1f98
Author: Josh Free <joshfree@users.noreply.github.com>
Date:   Wed Mar 30 09:06:21 2016 -0700

    Merge pull request #950 from dagood/upgrade-cli
    
    Upgrade dotnet cli version

[33mcommit 8ed1f98789d1b9f8ff0d7322b1d8f2647dfe7b44[m
Author: Davis Goodin <dagood@microsoft.com>
Date:   Wed Mar 30 10:18:30 2016 -0500

    Upgrade dotnet cli version.

[33mcommit af5862d19c92d9ebcd20b3f34b2f3f9605a075de[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Wed Mar 30 17:49:30 2016 +0800

    Update manual test guide for Linux HTTPS/Kerberos tests

[33mcommit 20ffb5a2ac8642ab26a440b312b43b27f304f458[m
Merge: 6f00b7c e87eccc
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Mar 29 04:00:44 2016 -0700

    Merge pull request #941 from roncain/fix-netnative-timeout-exception
    
    Fix async stream timeout and abort test for NET Native

[33mcommit 6f00b7cfc427dceb8cd247532f5cbf82d0e830b7[m
Merge: be6afd5 18b9a7f
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Tue Mar 29 15:19:42 2016 +0800

    Merge pull request #898 from iamjasonp/NegotiateStreamTests
    
    NegotiateStream tests

[33mcommit e87eccc01038f86705f001a28832b38d45cfb94c[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Mar 28 11:23:18 2016 -0700

    Fix async stream timeout and abort test for NET Native
    
    The regression test added to verify the fix for #931 failed on
    NET Native due to the way it was written.  By aborting the channel
    before timing out, the underlying NET Native HttpClient code
    terminated the connection rather than cancelling the cancellation
    token, so a TimeoutException was not thrown.  By reversing the
    order in the test to timeout, then abort, the cancellation token
    was properly cancelled, and a TimeoutException was thrown.
    
    The test was manually verified in the debugger to determine it
    still executed the code path fixed by #931.
    
    A 2nd test as added to test only the timeout scenario without
    an explicit abort.
    
    Fixes #939

[33mcommit 18b9a7fc9d05920547b1770da88f18cb4b572e57[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Mon Mar 28 16:49:36 2016 +0800

    Add tests to Common\Infrastructure\tests for TestProperties

[33mcommit be6afd58c2af0bdd3c2561403934deed4d2bb5d9[m
Merge: 4576c70 0670560
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Mar 25 07:03:39 2016 -0700

    Merge pull request #933 from roncain/update-xunit-extensions
    
    Update version of xunit.netcore.extensions to 187

[33mcommit 06705602131dee1577a81d017d4920fc839ca64b[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Mar 25 05:30:35 2016 -0700

    Update build tools version to 220
    
    Moving forward to 220 to take fixes in build tools as well
    as new syntax for requesting OuterLoop tests.

[33mcommit 0c7a50a82fd02e625b5a6f53a53506078ef904bf[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Mar 25 05:28:49 2016 -0700

    Update version of xunit.netcore.extensions to 187
    
    Updates the version of xunit.netcore.extensions to 187.
    This is to stay in sync with CoreFx and take some of the
    fixes and enhancements in the test case discoverers.

[33mcommit 4576c701087cd3d8dd2667c4f6d52b329cab9ea0[m
Merge: 7431073 e0f2da8
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Mar 25 05:04:20 2016 -0700

    Merge pull request #937 from roncain/disable_cert_revoked_test
    
    Disable certificate revoked scenario test

[33mcommit 74310735c4a7b2ee8c3a195df0cf0f5926f7d5ed[m
Merge: 4a4b529 c28ebbb
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Mar 25 05:04:02 2016 -0700

    Merge pull request #936 from roncain/async-stream-AV-test
    
    Adds scenario test for AV during Http async stream read

[33mcommit c6edb67c546b13950cd54647d5a6326ed6c2619a[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Mar 24 19:44:21 2016 +0800

    Add some debugging info to check on TestProperties

[33mcommit e0f2da8c440ed549e3494a616193daa54a62ea37[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Mar 24 13:46:20 2016 -0700

    Disable certificate revoked scenario test
    
    This test has been failing in ToF 100% of the time for me recently.
    
    Now attempting to move to latest build tools, it fails 100% of the
    time in OSS too.  It is a negative test that should be rewritten to
    be smarter about testing after the cert has been revoked.

[33mcommit 4a4b529c7a5f54fe39aada034ed4c70adc381fdd[m
Merge: 3d9ff66 6f39077
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Mar 24 13:31:18 2016 -0700

    Merge pull request #935 from roncain/fix-SendRequestAsync-AV
    
    Fix NullReferenceException in SendRequestAsync during async streaming

[33mcommit c28ebbb1e9b6a17b622a4281c147f529cc455018[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Mar 24 08:13:11 2016 -0700

    Adds scenario test for AV during Http async stream read
    
    Issue #931 was discovered by stress tests, where an aborted
    Http request doing an asynchronous read would throw a
    NullReferenceException.
    
    The cause is not a race condition but instead incorrect use of
    member fields in exception handling.  This PR adds a scenario test
    that exactly duplicates the stress test and causes NullRefException.
    
    This test will be merged only after #935 that contains the fix to
    issue #931.  The changes are being kept separate to allow selective
    cherry-picking into the RC2 branch.

[33mcommit 8589e9bec9172ee3e44ddf749eafe4919eede4da[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Mar 24 18:05:56 2016 +0800

    Modify NegotiateStream tests to use TestProperties

[33mcommit 3a776bd41062c081963190f94ccb5a990d42bef5[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Mar 10 19:01:12 2016 +0800

    Implement NegotiateStream tests

[33mcommit 3870e7fdaab526d201d7c5424699f71e34f3e8cc[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Wed Mar 9 17:41:28 2016 +0800

    Implement supported parts of Spn/UpnEndpointIdentity

[33mcommit 3d9ff6612a43d4186aa8746963f6b315984ff356[m
Merge: 9d14d7d 7b1a6a0
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Mar 23 13:25:55 2016 -0700

    Merge pull request #921 from StephenBonikowsky/UpdateBuildScripts
    
    Update build scripts based on changes in corefx.

[33mcommit 9d14d7d0d9fec62c3e7e1bfd320a360e9d38c9e0[m
Merge: 0c6de6d c9bd57b
Author: Matt Connew <mconnew@microsoft.com>
Date:   Wed Mar 23 12:55:36 2016 -0700

    Merge pull request #930 from mconnew/EtwPerformanceFixes
    
    Performance improvements related to ETW events

[33mcommit 6f39077259b03edab2d7ded942f4675bc369ec2a[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Mar 23 12:23:20 2016 -0700

    Fix NullReferenceException in SendRequestAsync during async streaming
    
    Problem: HttpChannelFactory.SendRequestAsync keeps the HttpRequestMessage
    in a member field and uses it during async processing.  But a Fault or Abort
    will null the HttpRequestMessage field.  If this happens while the
    SendRequestAsync method is executing, it leads to NullReferenceException
    during error handling.
    
    Solution: keep a local variable copy of the HttpRequestMessage for use in
    error handling.  Even if the original is disposed or the field set to null,
    error handling only requires access to the RequestUri property.
    
    Fixes #931

[33mcommit 7b1a6a08e963ca3fad67181e873fe70b74b3f21c[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Mar 18 13:31:18 2016 -0700

    Update build scripts based on changes in corefx.
    
    * Including build tools version and dotnet CLI version.

[33mcommit c9bd57b72b911f898c36e1cc6a9f6806c43dfcd6[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Tue Mar 22 14:12:13 2016 -0700

    Performance improvements related to ETW events

[33mcommit 0c6de6dc0b538cf440097cbbcd9c0678d1605281[m
Merge: 0a7c69a cd3eb92
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Mar 22 10:13:24 2016 -0700

    Merge pull request #925 from ericstj/update23918
    
    Update23918

[33mcommit cd3eb92bd27ec457e4ab433e3a4f5249cd366915[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Mon Mar 21 08:19:14 2016 -0700

    Fix restore errors after upgrading packages
    
    The System.Private.ServiceModel windows build was depending on package
    versions that didn't include support for the targeted platform.  Upgrade
    these to satisfy guardrails.
    
    Some test projects were depending on non-existent package versions
    (major.minor.build invalid) which have been updated to valid versions.
    
    xunit.extensibility.execution was bringing in an old version of
    Linq.Expressions that doesn't support DNXCore50, add a dependency to
    any project consuming this to update that dependency to latest.
    
    We removed the lifting from the lineup so tests should reference the
    latest packages in order to ensure an implementation is available.

[33mcommit 0a7c69a47cf2a968112a761d5e97a2b500ac2df7[m
Merge: 1d80c8b cdfeae6
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Tue Mar 22 15:51:02 2016 +0800

    Merge pull request #924 from iamjasonp/sync-pr
    
    Add PR sync scripts to repo for CI

[33mcommit cdfeae66ef221ab2f279c7a28d312270bb8c933a[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Tue Mar 22 13:50:45 2016 +0800

    Fix sync-pr scripts for DOS variable expansion issues
    Modify verbage on sync-pr scripts per PR comments

[33mcommit be04c542853e7c8fd772f5d0a98a16b8b8cf1f2e[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Mon Mar 21 19:10:41 2016 +0800

    Add PR sync scripts to repo for CI

[33mcommit 8dc2721bc6b77c34643bf1f6af8d95d867cb1206[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Mon Mar 21 07:56:39 2016 -0700

    Update packages to rc3-23921

[33mcommit 1d80c8b777668a828873ff0b30e63ebd305dc122[m
Merge: 41b7a16 30beda5
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Mar 21 06:31:11 2016 -0700

    Merge pull request #920 from roncain/fix-async-close-race
    
    Fix async close race condition in RequestChannel

[33mcommit 30beda5c15179ff7fa27363960bafd0f6cff5f6a[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Mar 21 05:16:52 2016 -0700

    Fix async close race condition
    
    Stress tests revealed a race condition during an async close
     of RequestChannel. A TCS used to signal completion of the close
     could become null if the final pending request completed
     asynchronously during the close.

[33mcommit 41b7a168bc384a9d5435bb0d4c17b338dd9c91b1[m
Merge: fcb863e faa1bbb
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Mar 18 11:21:11 2016 -0700

    Merge pull request #919 from StephenBonikowsky/LatestCleanScriptFromCorefx
    
    Syncing to the latest version of the Clean script.

[33mcommit faa1bbbdbd1218b53f3fe1fd75dd1c7a0fdb0844[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Mar 17 11:46:29 2016 -0700

    Syncing to the latest version of the Clean script.
    
    * The script was updated to check for and kill the long running VBCSCompiler.exe task before continuing cleaning tasks.

[33mcommit fcb863e95c01b2bcd9dee5bd55a08c0b5b500a95[m
Merge: abf781e b84caec
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Mar 17 10:03:04 2016 -0700

    Merge pull request #917 from StephenBonikowsky/ActiveIssueForNetNative
    
    Adding ActiveIssues for tests failing only on NET Native

[33mcommit b84caeca6003b86907d74c33cd3894e5f538bfc1[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Mar 16 11:37:22 2016 -0700

    Adding ActiveIssues for tests failing only on NET Native
    
    * By wrapping the ActiveIssue in an #if #endif based on FEATURE_NETNATIVE we should be able to only skip these test on NET Native and not on NET Core.

[33mcommit abf781e21fd89c0beca024ed1ae0ce7313ff11f7[m
Merge: b40d635 555fa83
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Mar 16 09:55:32 2016 -0700

    Merge pull request #910 from StephenBonikowsky/UseStableVersions
    
    Updating some packages to use stable versions.

[33mcommit b40d635f469e6b70c0204f5758304c6168dc5c3a[m
Merge: 0c7882e 1f1b337
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Wed Mar 16 19:29:42 2016 +0800

    Merge pull request #913 from iamjasonp/impl-spnlookuptime
    
    Implement SpnEndpointIdentity.SpnLookupTime

[33mcommit 555fa83d61761fe31a0c7d193018d312eca3a099[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Mar 10 11:54:21 2016 -0800

    Updating some packages to use stable versions.
    
    * Using stable version of System.Net.Http
    * Removing extra digit from S.Collections.Concurrent version.
    * Using correct version for S.SM.Primitives in WebSockets json file.

[33mcommit 0c7882e342425c24034c60bf4a0625b7d37d7a38[m
Merge: a0460c4 2e3e909
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Mar 15 14:07:50 2016 -0700

    Merge pull request #916 from StephenBonikowsky/UpdateSIOCompression
    
    Update S.IO.Compression to pre-release version.

[33mcommit 2e3e9094a3bf9992c6c3f68585dfeb43f09aa5a2[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Mar 15 14:04:07 2016 -0700

    Update S.IO.Compression to pre-release version.
    
    * Latest version of System.Net.Http.WinHttpHandler requires a move to the pre-release version of S.IO.Compression

[33mcommit a0460c43815566b8b75f0524641693cff249ec82[m
Merge: cad6565 6fddeee
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Mar 15 13:57:35 2016 -0700

    Merge pull request #914 from ericstj/update23915
    
    Update packages to rc3-23915

[33mcommit 6fddeee40badb4ccde36b2b70272a21f0d86a1c6[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Tue Mar 15 08:42:26 2016 -0700

    Update crypto dependencies
    
    System.Security.Cryptography.Algorithms and
    System.Security.Cryptography.X509Certificates have updated their
    versions to 4.1 and now target both NETStandard1.3 and
    NETStandard1.4.
    
    Update references to these packages and update any project that is
    restoring for DNXCore50 but packaging as NETStandard to restore for
    NETStandard as well so that it gets the correct references.

[33mcommit d15571b545a4bf9cc7dec004bf2c23f003c4d4b5[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Tue Mar 15 06:58:06 2016 -0700

    Update package versions to 23915

[33mcommit cad6565df121d552be0b51e2e34d03295e37ff3c[m
Merge: 226e1e6 ef3620c
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Mar 15 04:59:20 2016 -0700

    Merge pull request #911 from roncain/fix-sln
    
    Remove System.Private.ServiceModel.Tests residue from SLN

[33mcommit 226e1e61958c584d7737c87ab89f1a10387df44d[m
Merge: 656818f f09c2c6
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Mar 15 04:58:49 2016 -0700

    Merge pull request #912 from roncain/fix-basic-auth-test
    
    Fix test failure when BasicAuth tests were first to run

[33mcommit 1f1b3375f4d33d2ece0df5a8636ac26eb2b114f6[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Tue Mar 15 17:17:53 2016 +0800

    Implement SpnEndpointIdentity.SpnLookupTime
    
    Fixes #902

[33mcommit f09c2c6ab2134813b4161e743abca3da648a36a0[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Mar 14 12:21:55 2016 -0700

    Fix test failure when BasicAuth tests were first to run
    
    The Https BasicAuthentication tests failed were they were the first
    scenario tests run after a fresh start of the Bridge.  But they succeeded
    when other scenario tests ran first.
    
    Root cause was that the BasicAuthResource was not explicitly ensuring
    the SSL port certificate was installed.  It passed if another scenario
    test did that first.
    
    The fix was to duplicate what the other Https test resources do and
    explicitly ensure the SSL port certificate is installed.
    
    Fixes #788

[33mcommit ef3620cff5128578ea3a2ab86761a41ba9df7a8a[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Mar 14 11:10:06 2016 -0700

    Remove System.Private.ServiceModel.Tests residue from SLN
    
    The System.Private.ServiceModel.Tests.csproj was deprecated
    a few weeks ago, but some references were left in the SLN file
    causing warnings on opening.  This PR removes those references.

[33mcommit 656818f4d4b6b24af5ab56c965c049db7b328c0e[m
Merge: f43d2f7 3a35756
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Mar 11 13:38:41 2016 -0800

    Merge pull request #907 from roncain/disable-tcp-stream-tests-linux
    
    Disable tcp secure streaming tests on Linux

[33mcommit f43d2f73ad0125a28dfdf233d62b2f9f7f469fc8[m
Merge: d64adc5 5dddccf
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Mar 11 11:22:08 2016 -0800

    Merge pull request #905 from roncain/fix-cookies
    
    Fix AllowCookies logic to not set cookie container to null

[33mcommit d64adc51b679b12ecf1e1140b38cbbb8ceeca0a5[m
Merge: ea2b4cc eeb3969
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Mar 11 09:51:05 2016 -0800

    Merge pull request #906 from chcosta/fatpackages
    
    Fat package conversion of System.Private.ServiceModel

[33mcommit 3a357562db6a7efbd3ca5fcc537bd0597d6f72d3[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Mar 11 08:42:10 2016 -0800

    Disable tcp secure streaming tests on Linux
    
    The recent enabling of NegotiateStream in WCF Linux code
    caused all the existing tcp secure streaming tests to fail
    on Linux because CoreFx has not yet checked in the
    implementation of NegotiateStream.
    
    After discussion with @iamjasonp we decided to disable these
    tests for the same issue being addressed by PR #898.  Once
    CoreFx publishes a working NegotiateStream, we will either
    reactivate these tests or fold them into the new NegotiateStream
    tests in #898.

[33mcommit eeb3969a3166d7f74c9762b8a3fea06101f92a19[m
Author: Christopher Costa <chcosta@microsoft.com>
Date:   Fri Mar 11 08:41:10 2016 -0800

    Update sln file from Linux to Unix

[33mcommit 30ef7e6a95639e7b866a2da00741c0e019f86c2b[m
Author: Christopher Costa <chcosta@microsoft.com>
Date:   Fri Mar 11 08:38:34 2016 -0800

    Fat package conversion of System.Private.ServiceModel

[33mcommit 5dddccf9caa9111129f0dd9c685a8334c86cc37f[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Mar 11 08:23:41 2016 -0800

    Fix AllowCookies logic to not set cookie container to null
    
    The recent PR #883 for AllowCookies added code to assign the
    cookie container to null when AllowCookies was false.  But on
    NET Native, this causes an ArgumentNullException.  This caused
    all Http OuterLoop tests to fail.
    
    The fix is to set the cookie container only when AllowCookies is true.

[33mcommit ea2b4cc2bfdf6e9793dc4558b4dda0ba5dab7dbb[m
Merge: a1a5b30 a54191d
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Mar 10 16:46:46 2016 -0800

    Merge pull request #903 from StephenBonikowsky/FixMsCorlibRefForNetNative
    
    Fix .NET Native build of S.P.SM due to mscorlib reference.

[33mcommit a54191dada44f7cbdd12bb3efa1f43ea714a0077[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Mar 10 16:19:04 2016 -0800

    Fix .NET Native build of S.P.SM due to mscorlib reference.
    
    * The NET Native assembly for System.private.ServiceModel can't resolve the references in Windows.winmd, because they refer to mscorlib.
    * Adding a reference to Microsoft.NETCore.Portable.Compatibility, which has an mscorlib façade in it fixes the problem.

[33mcommit a1a5b300193008b63a67d0105d1bf15d4a9bd63f[m
Merge: 646d16b 1e5b715
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Mar 10 13:34:39 2016 -0800

    Merge pull request #897 from StephenBonikowsky/RestoreRoslyn
    
    Restoring build scripts back to using Roslyn.

[33mcommit 646d16bb2f19aefa8d6fa78d48a6f69ab07286f0[m
Merge: 2cd6318 59cbfe5
Author: Shin Mao <shmao@microsoft.com>
Date:   Thu Mar 10 13:06:11 2016 -0800

    Merge pull request #899 from shmao/883
    
    Fix an Issue caused by PR #883.

[33mcommit 2cd6318e05c79fe6ac9cbc93300ed6b07596a334[m
Merge: 1492ef3 e04379d
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Mar 10 10:56:32 2016 -0800

    Merge pull request #901 from roncain/fix-tfs
    
    Add SpnLookupTime property back to SpnEndpointIdentity

[33mcommit 1e5b7157dcef7319727cbb2ef71813b2ce6932ef[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Mar 9 14:39:02 2016 -0800

    Restoring build scripts back to using Roslyn.
    
    * After removing the WCF dependency on the old version of MSBuild via Mono which was not supported in Roslyn we can now remove the temporary hack we had in place that allowed us to continue using the old version of MSBuild.

[33mcommit e04379dd1d09ef3590deb6da4479ba10298e6578[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Mar 10 10:34:35 2016 -0800

    Add SpnLookupTime property back to SpnEndpointIdentity
    
    This property exists in the public contract and needed to
    be added back to fix a build break in TFS.

[33mcommit 59cbfe5ec66e4d98590658200e847246f11355a7[m
Author: Shin Mao <shmao@microsoft.com>
Date:   Thu Mar 10 10:11:14 2016 -0800

    Fix an Issue caused by PR #883.
    
    PR #883 breaks one Tof test project because the PR used C#6 syntax.

[33mcommit 1492ef3f220b0c9018faa42f870d015f158f3861[m
Merge: 2f25bc2 4a9fe66
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Thu Mar 10 23:24:29 2016 +0800

    Merge pull request #893 from iamjasonp/negostream-enable-2
    
    Implement supported parts of Spn/UpnEndpointIdentity

[33mcommit 2f25bc214445bbf9f762f062dc2b7aa1b3fce2ab[m
Merge: 1a55034 3d49a44
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Mar 10 04:56:42 2016 -0800

    Merge pull request #894 from roncain/close-async
    
    Enable ChannelFactory to use async close of inner channel factory

[33mcommit 1a5503470d62da19a47eea9eb0108f3578559c34[m
Merge: 57b8f71 217e77d
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Mar 10 04:56:12 2016 -0800

    Merge pull request #895 from roncain/endpointnotfound
    
    Map ERROR_INVALID_HANDLE to EndpointNotFoundException

[33mcommit 4a9fe667bbe2eeb035306378bb9c3b09ba22db60[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Wed Mar 9 17:41:28 2016 +0800

    Implement supported parts of Spn/UpnEndpointIdentity

[33mcommit 57b8f717aa5490b9a7a17095a2b423f760336fb3[m
Merge: acf03c9 3eaf25f
Author: Shin Mao <shmao@microsoft.com>
Date:   Wed Mar 9 11:34:00 2016 -0800

    Merge pull request #883 from shmao/853
    
    Enable AllowCookies=true.

[33mcommit 217e77d32f9fc6c31536b8782fb4760e5635a286[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Mar 9 08:43:58 2016 -0800

    Map ERROR_INVALID_HANDLE to EndpointNotFoundException
    
    On Linux, HttpClient returns an HResult with ERROR_INVALID_HANDLE
    attempting to access an endpoint it cannot find. As a result, we
    throw CommunicationException rather than EndpointNotFoundException.
    
    This is inconsistent with Windows behavior and breaks a scenario
    test for this case.
    
    Until CoreFx provides a common way to map HResults to a predictable
    set of values across platforms, we must map this specific HResult
    manually in our exception conversion helpers.
    
    Fixes #419

[33mcommit 3d49a44b6b0f5a591c968ce864860b87305d394c[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Mar 9 07:12:58 2016 -0800

    Enable ChannelFactory to use async close of inner channel factory
    
    Problem: ChannelFactory's async close was invoking its inner channelfactory's
    async close only if it implemented IAsyncChannelFactory, otherwise it used
    the synchronous close.  This led to stress issues where asynchonous closes
    blocked threads.
    
    Solution: make all channel factories declare they implement IAsyncChannelFactory.
    IAsyncChannelFactory is only a tagging interface that wraps IChannelFactory and
    IAsyncCommunicationObject.  And in practice, all channel factories were already
    implementing both IChannelFactory as well as IAsyncCommunicationObject (because
    their base class is CommunicationObject).  So the fix was only to add
    IAsyncChannelFactory to their list of implemented interfaces.  This allowed
    ChannelFactory to be able to call through to the async close on the base class
    CommunicationObject.
    
    Fixes #819

[33mcommit 3eaf25f91c922760e88caa57b5252810e4e7e572[m
Author: Shin Mao <shmao@microsoft.com>
Date:   Tue Mar 8 15:05:51 2016 -0800

    Enable AllowCookies=true.
    
    Fix #853.

[33mcommit acf03c9f622072b6b4aa2db3d8b171e386d7bf5d[m
Merge: c3832cb e57de8b
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Mar 8 09:35:30 2016 -0800

    Merge pull request #888 from roncain/fix-testproperties
    
    Code generation of TestProperties no longer uses CodeTaskFactory

[33mcommit e57de8b5ef80aea603268f1585a727bb2a24da8f[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Mar 8 07:59:49 2016 -0800

    Code generation of TestProperties no longer uses CodeTaskFactory
    
    The CodeTaskFactory feature is not supported in the CoreCLR
    version of MsBuild.  This resulted in build failure on Linux
    when we attempted to code-gen TestProperties building
    tests/Common/Infrastructure.
    
    This PR eliminates the use of CodeTaskFactory and massively
    simplifies code generation to a single CDATA section.

[33mcommit c3832cb00b9c867ec7bae338ea1deaac6649ec9e[m
Merge: d642396 782d303
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Mar 8 05:43:54 2016 -0800

    Merge pull request #887 from dotnet/revert-852-negostream-enable
    
    Revert "Turn on build include NegoStream"
    This does not compile in NET Native, so I am reverting to unbreak the TFS build

[33mcommit 782d3037ae0b6899b2fa22d012bb1447b6360837[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Mar 8 05:27:24 2016 -0800

    Revert "Turn on build include NegoStream"

[33mcommit d642396bf7abba74a7c5d42192f455eccbc50363[m
Merge: a23fae0 220fd94
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Tue Mar 8 11:57:35 2016 +0800

    Merge pull request #852 from iamjasonp/negostream-enable
    
    Turn on build include NegoStream

[33mcommit a23fae0e9ec383a5c407aa9c5a5b98657b160512[m
Merge: 8c26beb ac1cf78
Author: Hong Dai <hongdai@microsoft.com>
Date:   Mon Mar 7 13:54:07 2016 -0800

    Merge pull request #884 from hongdai/fixHelixBuild
    
    Remove one of the duplicated commandline args

[33mcommit ac1cf78f132ed22526fed1d0c861d07b93267a82[m
Author: hongdai <hongdai@microsoft.com>
Date:   Mon Mar 7 11:36:48 2016 -0800

    Remove duplicate pass commandline args
    
    * commit e284227f820e2d0eb34d2b640e13acf71a7997e3 add
    set "__args=%*", make __args the same as %*. thus we
    pass commandline args twice to msbuild. This breaks
    Helix, as Helix target does not allow two responses files
    specified, even thougt they are the same
    * The fix is to remove the redundancy.

[33mcommit 8c26beba69ab3c0922cbec65c96595942febaf3c[m
Merge: a141091 1b5be97
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Mar 7 10:17:40 2016 -0800

    Merge pull request #882 from iamjasonp/build-sh
    
    Modify build.sh to skip native building

[33mcommit a141091a0710e4a4967f0eafafbdae0cab50d796[m
Merge: 80c6c0d 9645870
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Tue Mar 8 01:11:46 2016 +0800

    Merge pull request #881 from iamjasonp/update-certinstall-sh
    
    Change BuildCertificateInstaller.sh to chmod downloaded cert

[33mcommit 1b5be971b12e9b9810de8f7e5b21672cc065b5ec[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Mon Mar 7 18:22:14 2016 +0800

    Modify build.sh to skip native building

[33mcommit 80c6c0d6ecb4e79be314148b30dfe72b2b2a16df[m
Merge: 5debd7d 8d6e2d5
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Mar 7 05:33:33 2016 -0800

    Merge pull request #877 from roncain/nettcp-stream-secure-tests
    
    Add tests for NetTcp TransferMode.Streamed and SecurityMode.Transport

[33mcommit 9645870d8fc38d732886c9bf06aa8b701a200082[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Mon Mar 7 17:50:47 2016 +0800

    Change BuildCertificateInstaller.sh to chmod downloaded cert
    
    The downloaded cert was in some circumstances not chmodded to world writable, which
    meant that cURL would have trouble accessing the certificates when trying to
    establish an HTTPS connection

[33mcommit 5debd7d4598c736435ec6a313bd947e4392c34df[m
Merge: 45d6f41 1baaa3d
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Mon Mar 7 16:55:01 2016 +0800

    Merge pull request #879 from StephenBonikowsky/UseMonoForMSBuild
    
    Temporarily revert back to prior version of MSBuild for Linux

[33mcommit 45d6f41a3f5a54a276ed68473bdc39239bf35ba8[m
Merge: a598732 1cc0603
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Mon Mar 7 15:59:03 2016 +0800

    Merge pull request #880 from dotnet-bot/from-tfs
    
    Merge changes from TFS

[33mcommit 1cc0603cdb1139bae1e5053d061c8ab8537df706[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Fri Mar 4 16:43:43 2016 -0800

    Updating build tools to 1.0.25-prerelease-00188
    
    [tfs-changeset: 1581740]

[33mcommit 1baaa3d404695779de7a5bd70a702946439462a3[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Mar 4 14:22:36 2016 -0800

    Temporarily revert back to prior version of MSBuild for Linux
    
    * The most recent sync to corefx build scripts pulled in the switch to remove any dependency on Mono
    * The new version of MSBuild does not support CodeTaskFactory which we have used to generate TestProperties
    * We will have to solve this issue but in the meantime it blocks our ongoing work
    * This PR attempts to revert the specific changes that were made to remove the dependency on Mono

[33mcommit a5987323772aadc48d259fa02635063050c79b22[m
Merge: 2bd9942 e4f7393
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Mar 4 13:09:56 2016 -0800

    Merge pull request #878 from StephenBonikowsky/UpdateSHfiles
    
    Updating WCF version of .sh files

[33mcommit 2bd994284e243343519a8b93fcf798ce2e6480eb[m
Merge: 270ab9d e284227
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Mar 4 13:09:12 2016 -0800

    Merge pull request #872 from StephenBonikowsky/SyncBuildFiles
    
    Syncing build scripts to corefx.

[33mcommit e4f7393b22f67967928e8e1dbe1ac76f92da9cc3[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Mar 3 11:04:03 2016 -0800

    Updating WCF version of .sh files
    
    * Wrapping all sections where we differ with the corefx version with a WCF start/end comment so it is easier to see expected differences.
    * Will open either Issue or PR in corefx to make some content be more generic and not specifically say "corefx" unless necessary.
    * Will open another PR in corefx to update their myget feed links to use the enterprise feed.

[33mcommit 270ab9d6e689be046114616d1c6563465acbee3b[m
Merge: 6baf513 e2886ac
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Mar 4 11:21:42 2016 -0800

    Merge pull request #876 from roncain/stable-xml-version
    
    Update System.Runtime.Serialization.Xml dependency to stable version

[33mcommit e284227f820e2d0eb34d2b640e13acf71a7997e3[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Mar 1 11:37:51 2016 -0800

    Syncing build scripts to corefx.
    
    * Changes to dir.props and dir.targets made in corefx cause failures in WCF repo.
    * Use of Roslyn compiler keeps around the task "VBCSCompiler.exe" which must be manually killed after each local build before running "git clean -xdf"
    * There are a few files that either exist in corefx but not in Wcf or vice versa, am not using this PR to investigate those discrepancies.
    
    *** Updated With Feedback ***
    > Updated document links for WCF.
    > Removing all *.sh from this PR.
    > Reverted the change to LICENSE
    > Reverted and commented the change in .gitignore
    > Updated log name in init-tools.cmd
    > Rebased

[33mcommit 8d6e2d5e10d0b0c4e264cd2ba8a0b1fc8ac75d90[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Mar 4 06:08:09 2016 -0800

    Add tests for NetTcp TransferMode.Streamed and SecurityMode.Transport
    
    Fixes #854

[33mcommit e2886ac009f8cedf21e45fb67f39d9389e8ada33[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Mar 4 06:32:10 2016 -0800

    Update System.Runtime.Serialization.Xml dependency to stable version
    
    System.Runtime.Serialization.Xml 4.1.0 is now a stable package,
    and the RC versions will become deprecated. This change updates
    the previous dependencies from the RC to the stable version.

[33mcommit 6baf51301764a24246801848e9e5cd641f4c425d[m
Merge: 8b45c62 db1f53d
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Mar 3 09:09:40 2016 -0800

    Merge pull request #873 from dotnet-bot/from-tfs
    
    Merge changes from TFS

[33mcommit db1f53d9a48169d6a9c2ac057f93d29f892649ed[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Thu Mar 3 07:17:28 2016 -0800

    **Suppress implementation dependencies from compile.**
    
    Packages with ref and lib list all their dependencies
    in a single section for that target framework.
    
    This creates a couple problems:
    1. Folks end up taking a dependency on the fact that a package
    happens to use another package in its implementation.
    2. For packages that have multiple implementations for the
    same TFM and different RID all dependencies appear in the
    same section even though some may be RID specific.
    Those RID speicific dependencies will compatibility
    errors during restore since the packages won't be supported
    on all RIDs.
    In lieu of the NuGet feature to represent RID-specific
    dependencies https://github.com/nuget/home/issues/1660
    we can at least suppress the compatibility error by
    excluding these implementation specific depdencies from
    compile.
    
    Further details are here:
    https://github.com/dotnet/buildtools/commit/d40435b1c460416768cc53a27091e57d948be171#diff-abe065d40d7c72dbdc1ad1957148d23fR14
    
    [tfs-changeset: 1581170]

[33mcommit 220fd944f5997466efedf3fa41d4847e171ca810[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Mar 3 16:20:33 2016 +0800

    Add DNS/SPN/UPNIdentity tests

[33mcommit 8b45c62c5ed92603d6cfa6e71521337cc27eaa08[m
Merge: 9e0152e c13f3af
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Mar 2 10:12:59 2016 -0800

    Merge pull request #871 from StephenBonikowsky/FixBridgeCertInstProject_BinClash
    
    Add default Configuration to BridgeCertificateInstaller.csproj

[33mcommit c13f3afa9c33c7a55f0370b23956ad15dc8bf366[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Mar 2 10:04:53 2016 -0800

    Add default Configuration to BridgeCertificateInstaller.csproj
    
    * If not specificed it will trigger an AnyOS build of referenced assemblies in the project which triggers BinClasher errors.

[33mcommit 9e0152e9e0b7e2a9ab713b40db1f29947b85912e[m
Merge: e712823 d9149a6
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Wed Mar 2 22:53:27 2016 +0800

    Merge pull request #870 from iamjasonp/bci-autobuild
    
    Automatically build BridgeCertificateInstaller

[33mcommit b18b9f10ab41f69bb39807ef2e5ea8d6846f64c3[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Feb 25 18:25:45 2016 +0800

    Implement supported parts of Spn/UpnEndpointIdentity

[33mcommit 151a9ef41471c792195b1a324ad473bfbe9812b3[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Feb 25 17:27:49 2016 +0800

    Turn on build for NegotiateStream in *nix

[33mcommit d9149a60e58a28a03836d6c1aaf3a8534fa0d033[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Wed Mar 2 19:11:44 2016 +0800

    Automatically build BridgeCertificateInstaller

[33mcommit e7128235dafc5440c114359139af81209f1a43fc[m
Merge: eb08876 df4c260
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Mar 1 11:48:22 2016 -0800

    Merge pull request #869 from hongdai/fixbuild1
    
    Fix build script to pass command line parameters

[33mcommit df4c2602309eda012fd9482d2bd4b442b566a02c[m
Author: hongdai <hongdai@microsoft.com>
Date:   Tue Mar 1 11:35:30 2016 -0800

    Fix build script to pass command line parameters
    
    * Pull request 860 remove %* accidently. Adding it back to fix
    the issue that outerloop does not get run

[33mcommit eb0887616e0baf03d0a9265362d2b6eddcf3b106[m
Merge: 4201f0a 0153f12
Author: Hong Dai <hongdai@microsoft.com>
Date:   Tue Mar 1 11:22:56 2016 -0800

    Merge pull request #868 from hongdai/testtimeout
    
    Fix default test timeout

[33mcommit 0153f1267f1b5cfca33aec99f752bbb978831fd6[m
Author: hongdai <hongdai@microsoft.com>
Date:   Tue Mar 1 09:43:28 2016 -0800

    Fix default test timeout
    
    * Currenty test timeout default value is 20 minutes. A test
    could hang for 20 minutes before it times out. A typical test
    should not run more than 1 minute.

[33mcommit 4201f0adaca51a6856193a9935aa337666b7f678[m
Merge: 3948f4a 951adfc
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Tue Mar 1 16:43:27 2016 +0800

    Merge pull request #867 from dotnet-bot/from-tfs
    
    Merge changes from TFS

[33mcommit 951adfc69e92237df178819c8e90c1266adcf541[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Tue Mar 1 00:19:23 2016 -0800

    Update buildtools to fix Unix build
    
    Unix build was failing due to missing MicroBuild.Core.targets.
    
    [tfs-changeset: 1580191]

[33mcommit 6b79b1193b5f213cee8f93fb5fe98982c311ab1b[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Mon Feb 29 17:07:28 2016 -0800

    Fixing build breaks mirrored from open.
    
    Update validation of DNXCore50 > netstandardapp1.5
    Add new xamarin frameworks to TFS-only & WCF projects
    Update paths in MS.NETCore.Portable.Compatibility to use netstandard
    instead of dotnet and dnxcore50.
    Update buildtools version to fix incorrect baseline dependency
    calculation.
    
    [tfs-changeset: 1580002]

[33mcommit 3948f4a6d92be6101356b83ebf1d2d6052f86d3e[m
Merge: d402ebc ad8c699
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Feb 29 11:35:08 2016 -0800

    Merge pull request #860 from StephenBonikowsky/Issue859_BinClashLogger
    
    Add the BinClasher logger to our normal WCF build process.

[33mcommit ad8c699e5a9c64098c8cd8a0ddc32942f524e9cb[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Feb 26 14:52:09 2016 -0800

    Add the BinClasher logger to our normal WCF build process.
    
    * Fix the clashes the tool found.
    * Sync related files to the originals in corefx repo.
    * Fixes #859

[33mcommit d402ebc41b4fcdd7f1134d58f928f35a73b9f413[m
Merge: 7e9390c 486075e
Author: Shin Mao <shmao@microsoft.com>
Date:   Mon Feb 29 10:33:05 2016 -0800

    Merge pull request #842 from shmao/775
    
    Made a performance improvement in XmlSerializerOperationBehavior.

[33mcommit 7e9390c3014cf2aab972dde4fb66cc64efbebd33[m
Merge: 4b3f73b b8edcb5
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Mon Feb 29 17:52:12 2016 +0800

    Merge pull request #850 from iamjasonp/certinstaller
    
    Change BridgeCertificateInstaller.sh to sudo as current user

[33mcommit 4b3f73bda9941a1e07137307c980cede62a32476[m
Merge: e39a125 f20322e
Author: Hong Dai <hongdai@microsoft.com>
Date:   Sun Feb 28 21:01:23 2016 -0800

    Merge pull request #865 from dotnet/revert-858-issue855
    
    Revert "Fix FQDNs if the server hosted on a Cloud machine"

[33mcommit f20322eeb88f68bfa4a4840cfc782dde511fc2ff[m
Author: Hong Dai <hongdai@microsoft.com>
Date:   Sun Feb 28 20:28:20 2016 -0800

    Revert "Fix FQDNs if the server hosted on a Cloud machine"

[33mcommit e39a125a10a9cdecd2ce5790157d8ff8b61bfbe0[m
Merge: cd592dd a2545c7
Author: Hong Dai <hongdai@microsoft.com>
Date:   Sat Feb 27 19:26:32 2016 -0800

    Merge pull request #858 from hongdai/issue855
    
    Fix FQDNs if the server hosted on a Cloud machine

[33mcommit 486075ec648abbb50194e519dee9fdce34b3667b[m
Author: Shin Mao <shmao@microsoft.com>
Date:   Fri Feb 26 14:55:11 2016 -0800

    Add comments for ServiceKnownType_XmlSerializerFormat_TwoOperationsShareKnownTypes_Test.
    
    Ref #775.

[33mcommit cd592dda2773f862febb7d916031acf7b08e24df[m
Merge: 6911d70 ce5d1f7
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Feb 26 13:52:47 2016 -0800

    Merge pull request #848 from StephenBonikowsky/Issue822_TestWarnings
    
    Issue822 test warnings

[33mcommit ce5d1f7e00dc81a0329309d6e566d82dfe13d2b9[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Feb 24 14:17:03 2016 -0800

    Remove build warnings.
    
    * The S.P.SM.Tests.csproj was originally intended to test APIs that were not part of the public contracts, it causes type conlfict errors.
    * It only contains tests for one feature which we later discovered could be tested via the public Contract and new tests were added for it in S.SM.Primitives.Tests.csproj.
    * There is no significant loss of test coverage by just removing these tests.
    * Updating System.Net.Http to lowest stable version needed.
    * Fixes #822

[33mcommit 58da5bf3e060645fa4f8d6fd0e2c426b7c1ab8f6[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Feb 16 18:13:24 2016 -0800

    Fix test .json project references.
    
    * Invalid reverences were causing numerous build warnings.

[33mcommit a2545c77aaa08728e8e498193a7f00f2e85a9729[m
Author: hongdai <hongdai@microsoft.com>
Date:   Fri Feb 26 13:25:41 2016 -0800

    Fix FQDNs if the server hosted on a Cloud machine
    
    * On cloud machine, the current logic to get FQDN does not work.
    We need to append domain name cloudapp.net.
    
    Fix #855

[33mcommit 6911d70f43123cdac78dbb0685b4bbf737e7fbd0[m
Merge: 69365f5 c23b3a0
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Feb 26 10:48:16 2016 -0800

    Merge pull request #856 from roncain/fix-websocket-tests
    
    Modify WebSocket test asserts to be correct cross-machine

[33mcommit 69365f59cdeae05140fc1b13d758e48c2a8922ea[m
Merge: b4afc0e cb9cb38
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Feb 26 10:47:50 2016 -0800

    Merge pull request #847 from roncain/enable-nettcp-stream
    
    Enable nettcp stream

[33mcommit c23b3a0135d90b85bc1c796f8375c93c16737169[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Feb 26 09:37:05 2016 -0800

    Modify WebSocket test asserts to be correct cross-machine
    
    Some asserts in WebSockets tests were valid only when the
    Bridge was running as localhost, but failed when running on
    a different machine.
    
    The change only modifies the assert to be tested if the Bridge
    is running as localhost.
    
    Fixes #677

[33mcommit b4afc0efe64a610a2ba1411d3eb47a3d1aa03393[m
Merge: 0df3b51 d2438c8
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Feb 25 14:13:32 2016 -0800

    Merge pull request #845 from StephenBonikowsky/FixTestBuildAnyOSIssue
    
    Fix test projects to not build AnyOS flavor of S.P.SM

[33mcommit cb9cb3814a78565b03d12b5df1ae7d47f30894a4[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Feb 25 13:01:13 2016 -0800

    Additional code cleanup and added Asserts

[33mcommit d2438c894fabd99ad664b3760e8ecc6f86f6ad4f[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Feb 23 12:40:39 2016 -0800

    Fix test projects to not build AnyOS flavor of S.P.SM
    
    * Test projects were causing S.P.SM to be built as AnyOS.
    * S.P.SM forks based on whether it is a Windows build or not, so this was producing a non-windows build of S.P.SM being used with tests that expect a Windows build of S.P.SM.
    * Fixes #750
    * Fixes #838

[33mcommit c4fffafd50d5e6b19c9b052b12b701429dd06304[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Feb 24 12:14:06 2016 -0800

    Modify the raw full framework code to enable NetTcp streaming
    
    This commit makes the necessary modifications to the full framework
    files add in the prior commit to enable NetTcp streaming.
    
    It also adds some new NetTcp streaming tests and reactivates the ones
    that were formerly disabled.
    
    Fixes #713

[33mcommit 9d9ae1df86d6dc644fc7c493ef078454c3420a09[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Feb 24 11:58:32 2016 -0800

    Enable streaming transfer mode in NetTcp
    
    First commit moves necessary files from full framework.
    No changes have been made to these files, but they will
    provide a better diff experience after updating them.

[33mcommit b8edcb5c6d29dfd7ae27005697c0aaf10b888d15[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Feb 25 16:28:57 2016 +0800

    Change BridgeCertificateInstaller.sh to sudo as current user

[33mcommit 0df3b51e42ce69083882cf23dad6ff5ad37fffca[m
Merge: 2b92388 7fbfc42
Author: KKhurin <kkhurin@microsoft.com>
Date:   Wed Feb 24 20:36:16 2016 -0800

    Merge pull request #849 from KKhurin/PerfAndStressTests2
    
    Refactored stress tests and adopted them to run perf

[33mcommit 7fbfc42d68fa4ffaee9fe820af81008f9d65ff85[m
Author: Konst Khurin <kkhurin@microsoft.com>
Date:   Wed Feb 24 17:27:40 2016 -0800

    Refactored stress tests and adopted them to run perf
    -Parity between sync and async coverage
    -Merged with Hong's duplex changes for NetHttp binding
    -Added Duplex NetHttp streaming scenario
    -Added the server source code (will provide the instructions on how to setup the IIS server)

[33mcommit 2b92388dc60e3c511f348a2d7651c3435d7c44af[m
Merge: 38bfa6e 5a21c71
Author: Hong Dai <hongdai@microsoft.com>
Date:   Wed Feb 24 14:42:15 2016 -0800

    Merge pull request #843 from hongdai/addcijob
    
    Add a job for nightly runs against the latest dependencies

[33mcommit 5a21c71742e5d88460deb824537e20c5e1809515[m
Author: hongdai <hongdai@microsoft.com>
Date:   Wed Feb 24 14:06:24 2016 -0800

    Add a job to do test runs against the latest dependencies daily
    
    * I aslo add an optional PR trigger.

[33mcommit 38bfa6e2b76b68fe85de587abf335e1e81d62c8c[m
Merge: c1f6222 71cca2d
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Feb 24 06:53:05 2016 -0800

    Merge pull request #846 from dotnet-bot/from-tfs
    
    Merge changes from TFS

[33mcommit 71cca2d2d7ae606328fe2b78afbc4984d7127cda[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Tue Feb 23 20:18:17 2016 -0800

    Rename "dotnet" to "NETStandard"
    
    Fixes https://github.com/dotnet/corefx/issues/5707
    
    We are changing the .NET packages to no longer use the ‘dotnet’, ‘dotnet5.x’, and ‘dnxcore50’ monikers.  This is thee first stage of the change for dotnet->netstandard.
    
    The replacements are as follows
    
    Old moniker | New moniker
    ----------------- | ------------------
    dotnet5.x | netstandard1.y  (where y = x -1)
    DNXCore50 | netstandardapp1.5
    dotnet | netstandard1.3
    
    To prepare for this change you can do the following to your project.json.  This change will require a recent build of NuGet or dotnet.exe and can be done prior to consuming the packages with the breaking change.  These packages will not work with DNX.
    
    For a project targeting dotnet5.6
    ```
        "frameworks": {
            "netstandard1.5": {
                "imports": [ "dotnet5.6" ]
            }
        },
    ```
    
    For a project targeting dnxcore50
    ```
        "frameworks": {
            "netstandardapp1.5": {
                "imports": [ "dnxcore50", "portable-net45+win8" ]
            }
        },
    ```
    
    [tfs-changeset: 1578321]

[33mcommit c1f62220bb30f00bb56f456c3077e206e20e885e[m
Merge: cd8f735 a920359
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Wed Feb 24 11:23:03 2016 +0800

    Merge pull request #844 from dagood/new-cli
    
    Upgrade dotnet CLI version

[33mcommit a92035916be6f26acd03057f688e7d4a02b1584c[m
Author: Davis Goodin <dagood@microsoft.com>
Date:   Tue Feb 23 12:00:57 2016 -0600

    Upgrade build to new CLI version that uses xplat nuget rather than dnu.

[33mcommit cd8f7350e27227783966de674d3f4e2307d16f2c[m
Merge: 8905f80 427ad0e
Author: Shin Mao <shmao@microsoft.com>
Date:   Mon Feb 22 15:52:19 2016 -0800

    Merge pull request #818 from shmao/167
    
    Added a Unit Test Covering ChannelFactory.BeginOpen

[33mcommit 427ad0e50ed39e9146fda567c9ee54469ca0502a[m
Author: Shin Mao <shmao@microsoft.com>
Date:   Mon Feb 22 09:42:15 2016 -0800

    Updated Messages in ChannelFactory_Async_Open_Close.

[33mcommit 8905f806d00895dffc3289ee076e46e8a28633f1[m
Merge: adf55df e3e94bf
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Tue Feb 23 01:32:50 2016 +0800

    Merge pull request #840 from mmitche/update-badges
    
    Update badges

[33mcommit adf55df7800a942c39444fca5c94ed65926f7c92[m
Merge: e5d87d4 4fccc9f
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Feb 22 08:55:07 2016 -0800

    Merge pull request #839 from roncain/bridge-pass-args
    
    Allow MsBuild properties at build time to flow to the Bridge.

[33mcommit e3e94bf4cf36bba9420b8e10d0a9d46dce1231cc[m
Author: Matt Mitchell <mmitche@microsoft.com>
Date:   Fri Feb 19 12:52:26 2016 -0800

    Update badges

[33mcommit 0a3ff00112b925ec5dd967dfe8084dcb5ecc9b9b[m
Author: Shin Mao <shmao@microsoft.com>
Date:   Fri Feb 19 17:34:47 2016 -0800

    Made a performance improvement in XmlSerializerOperationBehavior.
    
    Fix #775.

[33mcommit 9f990939a5eed26cd29f6b26cc3bf5371cc1032f[m
Author: Shin Mao <shmao@microsoft.com>
Date:   Fri Feb 19 16:08:32 2016 -0800

    Added a Unit Test Covering ChannelFactory.BeginOpen.
    
    Fix #167

[33mcommit e5d87d42da045a0698ce104d0626187e193831bb[m
Merge: 32f2633 3c66eb7
Author: Matt Mitchell <mmitche@microsoft.com>
Date:   Fri Feb 19 12:32:54 2016 -0800

    Merge pull request #825 from mmitche/simplify-and-branchify
    
    [Do not Merge] Simplify and branchify the netci file

[33mcommit 3c66eb715e92bec84b89d45e37b8de8da515d75d[m
Author: Matt Mitchell <mmitche@microsoft.com>
Date:   Fri Feb 19 12:26:12 2016 -0800

    fixup

[33mcommit d6751543c0f0bd66cffbc9d5c6ee17275a328558[m
Author: Matt Mitchell <mmitche@microsoft.com>
Date:   Tue Feb 16 16:07:31 2016 -0800

    Simplify and branchify the netci file
    
    This does some basic refactoring and simplification of the CI definition, as well as moving WCF to the branch based model (see below)
    
    Move wcf to branch model
    
    This moves wcf to the branch-based CI model.  In this model, instead of having the CI generate jobs out of dotnet/wcf's master branch for all branches (using all sorts of odd naming suffixes of course), the master CI lists tell the CI which branches it should look at for CI config info.  It passes the branch name to the netci file, which then can use it to tell the SCM to pull code from specific branches or set branch specific PR triggers.

[33mcommit 4fccc9fae7158f43b5dc4d1f146b208b87c5aa10[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Feb 19 09:57:22 2016 -0800

    Allow MsBuild properties at build time to flow to the Bridge.
    
    This PR makes it possible for Bridge configuration properties
    expressed at build time to be built into the test binaries.
    And it also allows Build.exe to accept the MSBuild style of
    property arguments.
    
    This enables 2 important scenarios for our multi-machine effort.
    
    1. Doing "build /p:BridgeHost=xyz" will generate the appropriate
    TestProperties at compile time so that the generated test binaries
    carry this information. Then if they are uploaded to Helix to
    execute, they already carry the information necessary to communicate
    with a Bridge on a different machine.  There is no need to set
    environment variables on the execution machine for this.
    
    2. By allowing Bridge.exe to parse the MSBuild style property
    arguments, it allows our build scripts to flow Bridge configuration
    information to the Bridge startup/ping scripts so they agree with
    what the tests expect.
    
    Prior to this PR, it would have been necessary to set environment
    variables on both the client and execution machines to have a
    shared Bridge configuration.

[33mcommit 32f2633f6acd1ea510178447f215e1327b90a62b[m
Merge: 0de08c8 2023677
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Sat Feb 20 00:37:23 2016 +0800

    Merge pull request #837 from iamjasonp/update-codecover-version
    
    Update code coverage tooling versions

[33mcommit 0de08c8c212611412583a745980df7126b08ed80[m
Merge: b767b8a b4ff107
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Feb 19 04:00:06 2016 -0800

    Merge pull request #831 from roncain/nettcp-stream
    
    Add scenario tests for NetTcp streamed transfer mode

[33mcommit 2023677ebe5a50092f1def12b2d8e573fe3ea46a[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Fri Feb 19 18:15:43 2016 +0800

    Update code coverage tooling versions

[33mcommit b767b8a16372b5c1606eaca79dfc45f32a454ec0[m
Merge: 01a5274 fcbd392
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Feb 18 12:01:12 2016 -0800

    Merge pull request #827 from StephenBonikowsky/UpdateJsonFilesForXplatNuget
    
    Update json files for xplat nuget

[33mcommit 01a5274c90bf6701797ea08f984ee933e264b7e6[m
Merge: bc4db0c a20734b
Author: Shin Mao <shmao@microsoft.com>
Date:   Thu Feb 18 09:07:04 2016 -0800

    Merge pull request #824 from shmao/823
    
    Add Test for the Scenario where Two Contracts use same Namespace.

[33mcommit b4ff1076ad41353d33deafe57dd7f99cda200c6a[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Feb 17 13:44:49 2016 -0800

    Add scenario tests for NetTcp streamed transfer mode
    
    Issue #713 tracks the fact NetTcp streaming has not yet
    been implemented.
    
    This PR adds streaming tests for NetTcp which are marked
    with [ActiveIssue] to suppress them until #713 is fixed.
    
    Fixes #830

[33mcommit bc4db0ca7652770fc15b155c2505f6400a17099f[m
Merge: 0dbf3b3 e8db583
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Fri Feb 19 00:25:26 2016 +0800

    Merge pull request #829 from iamjasonp/negostream-investigate
    
    Fix ClientCredential.Clone method which missed Windows credentials

[33mcommit e8db5835cca7451ecec58e879fd91dbb1e7b6b02[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Feb 18 18:12:39 2016 +0800

    Fix ClientCredential.Clone method which missed Windows credentials
    
    The initial porting work meant that some methods had functionality stripped to
    allow for compile. In this case, we stripped out the clone capability for
    the WindowsClientCredential and neglected to add it back when we were turning
    this capability back on
    
    This is required for WindowsStreamSecurityUpgradePRovider to to successfully
    pass our creds along to the underlying NegotiateStream

[33mcommit fcbd392e9fc6a9a94ba970831b6cc4a4838c5c10[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Feb 17 14:43:56 2016 -0800

    Fork .json files for win8 runtimes.
    
    * Create a second .json file for netcore50 specifying win8 runtimes.

[33mcommit e9c5bb858a40a9aa98758abf9f05895da4dcc0b7[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Feb 17 10:06:13 2016 -0800

    Recreating branch content prior to reverting UpdateBuildSystemforWCF.
    
    * Cherry-picked 36a913236b8ca4bbd4338f07da017410192e5e84 "Package S.P.SM as dotnet5.4 and consolidate facade builds."
    * Cherry-picked f36128b71fe4760c226fbca9f56a889574630ce5 "Removing validation skip logic, adding S.P.SM package reference."

[33mcommit 0dbf3b307dce2012175ac078149f76d239498769[m
Merge: f17dac9 46438c7
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Feb 16 18:05:00 2016 -0800

    Merge pull request #826 from dotnet/revert-793-UpdateBuildSystemforWCF
    
    Revert "Package S.P.SM as dotnet5.4 and consolidate facade builds."

[33mcommit 46438c741866895f969631f2e62400f4b3b6c06a[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Feb 16 18:02:56 2016 -0800

    Revert "Package S.P.SM as dotnet5.4 and consolidate facade builds."

[33mcommit a20734bcbc9b813bef90b2623227d8b9404bf76f[m
Author: Shin Mao <shmao@microsoft.com>
Date:   Tue Feb 16 16:13:00 2016 -0800

    Add Test for the Scenario where Two Contracts use same Namespace.
    
    Fix #823

[33mcommit f17dac98426a036cd6f9c192777b7d06f524ea2b[m
Merge: 6f89986 b34f29f
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Feb 16 14:11:08 2016 -0800

    Merge pull request #821 from dotnet-bot/from-tfs
    
    Merge changes from TFS

[33mcommit 6f899867f8c720cba91a66983bf7de2420aff6af[m
Merge: fda6c3d f36128b
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Feb 16 13:52:20 2016 -0800

    Merge pull request #793 from StephenBonikowsky/UpdateBuildSystemforWCF
    
    Package S.P.SM as dotnet5.4 and consolidate facade builds.

[33mcommit b34f29f73cf9988d6422434809fa25acf7020e7a[m
Author: dotnet-bot <dotnet-bot@microsoft.com>
Date:   Tue Feb 16 11:40:36 2016 -0800

    Internally, restore solely from a VSTS feed. Add <clear/> back to NuGet.Config files.
    
    Updated internal xplat nuget fork to allow the <clear/> and to bring in other fixes.
    
    Fixed project.jsons to allow parallel restore. xplat nuget has a race condition when dependencies on the same package have different capitalization, so those were normalized towards uppercase.
    
    [tfs-changeset: 1575564]

[33mcommit f36128b71fe4760c226fbca9f56a889574630ce5[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Feb 16 11:33:51 2016 -0800

    Removing validation skip logic, adding S.P.SM package reference.
    
    * Removing the logic from wcf.targets file that would skip validation of S.P.SM.
    * Adding a reference to S.P.SM to the *.json files for each facade in order to validate the dependencies pulled in via the reference to S.P.SM.csproj.
    * Removed the "Windows_Debug" configuration from S.P.SM.csproj which could result in a race condition related to the OSGroup global property.
    * Added back the OSGroup to the reference to S.P.SM.csproj for each facade, otherwise it would trigger another build of S.P.SM for "AnyOS".

[33mcommit 36a913236b8ca4bbd4338f07da017410192e5e84[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Feb 8 14:39:37 2016 -0800

    Package S.P.SM as dotnet5.4 and consolidate facade builds.
    
    * Package System.Private.ServiceModel as target framework .Net5.4
    * The 5 ServiceModel facades only need to be built once for WcfCore.
    * Fixes #698
    * Fixes #697

[33mcommit fda6c3d87f2a112de85d44dce36a33a131bd7e98[m
Merge: a3c80b9 8e64bec
Author: Hong Dai <hongdai@microsoft.com>
Date:   Mon Feb 15 22:35:42 2016 -0800

    Merge pull request #811 from hongdai/rdxmlchange
    
    Add OperationFault to rd.xml

[33mcommit a3c80b96a471ff416a6c00e7dbe5748379009b71[m
Merge: 1fa38d6 2cc1c82
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Mon Feb 15 16:05:24 2016 +0800

    Merge pull request #820 from iamjasonp/netci-groovy2
    
    Change the verbage of dotnet-ci PR triggers

[33mcommit 2cc1c822282fe85f24a58636711d68b7edfbb036[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Mon Feb 15 15:43:23 2016 +0800

    Change the verbage of dotnet-ci PR triggers
    
    Change the dotnet-ci trigger phrases to call out "build and test" and
    call out "innerloop"

[33mcommit 1fa38d6df337751d612f586cb196543ade27bdff[m
Merge: a548bdf 54d62e4
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Mon Feb 15 15:01:11 2016 +0800

    Merge pull request #807 from iamjasonp/readme-changes
    
    Update README.md to reflect new builds / packages

[33mcommit 8e64bec70cd732eeb48436de1a4dc49be9d5d763[m
Author: hongdai <hongdai@microsoft.com>
Date:   Fri Feb 12 12:10:47 2016 -0800

    Add OperationFault to rd.xml
    
    * This is need to fix issue #769 from out side.
    * Toolchain does not know we need this type and remove it during reduce
    dependencies. Adding to rd.xml to explictly tell the toolchain we need it.

[33mcommit 54d62e417fbe8274df72c2cfb22ee003960847b7[m
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Sat Feb 13 03:03:56 2016 +0800

    Update README.md to reflect new builds / packages

[33mcommit a548bdf15238fd3b49c5c1dd166780f547f242f0[m
Merge: 57bcb44 06ed63c
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Sat Feb 13 01:53:36 2016 +0800

    Merge pull request #806 from iamjasonp/codecoverage
    
    Update test-runtime/project.json for code coverage
    Update netci.groovy to remove overzealous use of quotes

[33mcommit 06ed63cef7a407b287f30ed36923605d7ef1a8be[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Sat Feb 13 01:41:31 2016 +0800

    Fixing netci.groovy code coverage command escaping

[33mcommit 4b36a6a008cca4a7f9d1d64bb94ae50052a5c386[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Sat Feb 13 01:07:46 2016 +0800

    Update test-runtime/project.json for code coverage

[33mcommit 57bcb44029853b01a49ee306b730d77e0c6d4283[m
Merge: e7fd46d 0c39749
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Sat Feb 13 01:10:23 2016 +0800

    Merge pull request #804 from iamjasonp/netci-groovy-package
    
    Modify CI to skip package build for linux and outerloop builds

[33mcommit 0c39749fae40d92a6c086fa896d68af1d1b3e538[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Fri Feb 12 16:17:16 2016 +0800

    Modify CI to skip package build for linux and outerloop builds

[33mcommit e7fd46d4433fa9dcf7a685b05cbd38f8236ee554[m
Merge: 05e2eba b6b0abb
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Fri Feb 12 12:57:50 2016 +0800

    Merge pull request #803 from iamjasonp/buildsh-v2feed
    
    Move build.sh back to v2 myget feeds to work around CI issue

[33mcommit b6b0abb03f42b58106cc736fa7bfb95430ee8f82[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Fri Feb 12 05:16:03 2016 +0800

    Move build.sh back to v2 myget feeds to work around CI issue
    
    The CI machines don't seem to like using the v3 myget feeds right now, so move
    back to v2 feeds to unblock Linux testing in CI

[33mcommit 05e2ebadeeb25410849c34b1dc497892778f89d9[m
Merge: c63eca7 390c0b5
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Fri Feb 12 03:26:34 2016 +0800

    Merge pull request #801 from iamjasonp/netci-groovy
    
    Change CI to use build.sh on Linux

[33mcommit c63eca7f300b9a5fb6be6f89fce9c142cf72b91d[m
Merge: 157d152 41d4f00
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Fri Feb 12 01:14:00 2016 +0800

    Merge pull request #799 from iamjasonp/init-tools-perms
    
    Add +x permissions for init-tools.sh

[33mcommit 390c0b5a05b6c5de3f21ec6f971af59a465ddac1[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Feb 11 17:02:46 2016 +0800

    Change CI to use build.sh on Linux
    
    This change allows us to use build.sh on Linux rather than using build.cmd on
    a Windows machine and then using a build flow to propagate the binaries to a
    Linux box for testing
    
    It allows us to run CI against published packages rather than built packages
    from the latest CI run from corefx

[33mcommit 41d4f00a9ac22fe84d1c628e9b81b6bedebece0a[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Feb 11 13:05:59 2016 +0800

    Add +x permissions for init-tools.sh

[33mcommit 157d1526aabd3241fa443d1ba8ef67dd4a530d06[m
Merge: af27407 5b76e4d
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Feb 10 11:24:42 2016 -0800

    Merge pull request #798 from dotnet-bot/from-tfs
    
    Merge changes from TFS

[33mcommit 5b76e4db7473d9ffbbb24e3bba48beaffdea0786[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Wed Feb 10 09:34:54 2016 -0800

    Add lineup to WCF tool to fix restore
    
    This tool was failing the gaurdrails check during restore.
    Fix that by adding a lineup to let nuget see the implementation
    assemblies.
    
    [tfs-changeset: 1574205]

[33mcommit baa02b234d09cea906cb37906429fa55e886ed8a[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Wed Feb 10 09:33:40 2016 -0800

    Fix dependency on non-existent System.Reflection version
    
    [tfs-changeset: 1574204]

[33mcommit af27407806d9186a143c6ec5eff74ba3ac4ee7b5[m
Merge: 600c028 1619ed1
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Feb 10 08:48:39 2016 -0800

    Merge pull request #797 from dotnet-bot/from-tfs
    
    Merge changes from TFS

[33mcommit 1619ed1a956ad1362aa36a7208cdaeae215a75fc[m
Author: dotnet-bot <dotnet-bot@microsoft.com>
Date:   Wed Feb 10 08:22:42 2016 -0800

    Build: Change dependency from System.Reflection 4.0.11 prerelease to 4.1.0 prerelease. System.Reflection 4.0.11-rc3-23809 doesn't exist.
    
    [tfs-changeset: 1574196]

[33mcommit 600c0284c112f99b93ee79ea7d5b3a4c9c1a2996[m
Merge: 7d03d44 3c0d6ea
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Thu Feb 11 00:05:46 2016 +0800

    Merge pull request #795 from iamjasonp/update-build-sh
    
    Update build sh and runt-test.sh to use enterprise myget feed

[33mcommit 7d03d4425bfa8dbd40be29f28c801c699c6608a8[m
Merge: 5d1d3b1 898b012
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Thu Feb 11 00:05:06 2016 +0800

    Merge pull request #781 from iamjasonp/update-package-rc3-23802
    
    Update packages to rc3-23809

[33mcommit 3c0d6eaacd00087760b12414ada333b24c146a1f[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Wed Feb 10 18:00:53 2016 +0800

    Update build.sh, run-test.sh to use enterprise myget feed

[33mcommit 898b012a5b83b3ae37b420df3d07a8d5faacbc74[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Wed Feb 10 15:05:22 2016 +0800

    Add references in project.json for test projects
    
    Before updating to rc3-23809, we were able to use transitive references in the
    System.ServiceModel.* references to resolve references. We now need to reference
    these packages explicitly.

[33mcommit 77fd82cae3ae83763ec71feb21a677543db56923[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Wed Feb 10 14:27:35 2016 +0800

    Consume v3 versions of the new feeds
    
    * We have moved our feeds to our enterprise myget
      account (dotnet.myget.org). Start pulling from there and use the v3
      feeds since we are restoring with tools that understand them.
    * Remove some no longer needed NuGet commands and sources

[33mcommit b64b4b068b62482c0c56870a99d9c06bc6d28f87[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Sat Feb 6 13:04:00 2016 +0800

    Update package versions to rc3-23802
    
    Also update Packaging.props and dir.props

[33mcommit 5d1d3b14e711cd21585c0656fa9bf1a7f73f211f[m
Merge: b4a8661 ca0619a
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Wed Feb 10 14:12:32 2016 +0800

    Merge pull request #794 from dotnet-bot/from-tfs
    
    Merge changes from TFS

[33mcommit b4a86616f8a5b4bec530af6d1daf5ba6cee1fd68[m
Merge: b586755 8c5d926
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Wed Feb 10 12:29:26 2016 +0800

    Merge pull request #790 from dotnet-bot/from-tfs
    
    Merge changes from TFS

[33mcommit b5867554fe565454d0b760be0bdc5768010e0dbe[m
Merge: 32d25fc 495dc6c
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Wed Feb 10 12:27:56 2016 +0800

    Merge pull request #776 from iamjasonp/cert-bootstrapper
    
    Create Certificate Authority "bootstrapper" for Linux

[33mcommit ca0619a91853351a7fa012ac587dad992e531ac5[m
Author: dotnet-bot <dotnet-bot@microsoft.com>
Date:   Tue Feb 9 14:20:02 2016 -0800

    Adds custom xplat nuget that can restore from VSTS feed, adds the VSTS feed, and upgrades project.jsons throughout NDP\FxCore to pass new nuget compatability checks.
    
    Some fixes (adding imports, adding Platforms package) were tooled, but more complex ones (forking project.jsons, editing csproj's) were not.
    
    Also makes dependency version validation rules case-insensitive. (I used this fix to update the versions of some packages.)
    
    [tfs-changeset: 1573868]

[33mcommit 8c5d926696b27ebc726a9e24a7aa5e457707fd67[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Tue Feb 9 13:28:58 2016 -0800

    Fix race conditions during package build
    
    I've run my binclash logger on the packaging build and identified all
    the problems that result in multiple builds evaluations producing the
    same output.
    
    In some cases this was due to global properties from traversal leaking
    into library build.  I have fixed these as follows:
      Dummy
        This property was just being used to trigger batching to run
        multiple builds of the same project in a batch.  Instead of
        triggering batching by defining this in the properties, which
        has side effects, I used the %(Identity) in a condition that would
        always evaluate to true, which has no side effects.
      DefaultBuildAllTarget
        Similar to FilterToOSGroup this only applies for traversal projects
        so I undefine it when building a non-traversal project.
      PackageTargetFramework
        We were using this property to package the output of the library
        twice in different paths.  Rather than doing this from the package
        project I just have the library project declare that it should be
        packaged twice.
    
    [tfs-changeset: 1573841]

[33mcommit 32d25fcba97564cfc943a15e8ab565c8136449c8[m
Merge: 11dbdc3 7399804
Author: Matt Connew <mconnew@microsoft.com>
Date:   Tue Feb 9 11:31:12 2016 -0800

    Merge pull request #787 from mconnew/BridgePortMaangerFix
    
    Fix PortManager firewall remoteaddresses for resource domain

[33mcommit 11dbdc348d3d21d76c9468b8d3c0026d1b97adca[m
Merge: 64c4229 72a1818
Author: Hong Dai <hongdai@microsoft.com>
Date:   Tue Feb 9 08:56:03 2016 -0800

    Merge pull request #774 from hongdai/latestdep
    
    Enable running against latest dependencies

[33mcommit 72a18188adcc8c6a8ea4244e58acaee805d0a402[m
Author: hongdai <hongdai@microsoft.com>
Date:   Mon Feb 8 15:26:20 2016 -0800

    Enable running against latest dependencies
    
    * Use FloatingTestRuntimeDependencies=true to run against latest dependencies
    * Fix the build script to allow config TestFixedRuntimeProjectJson
    * Note that versions "4.0.11-rc3-23725" will be changed to "4.0.11-*" in
    the build process and copy to a location used by the tests.
    * Add default configration explcitly to work around the issue it will fail
    if no build configration is specified
    
    Signed-off-by: hongdai <hongdai@microsoft.com>

[33mcommit 64c4229579285c1233202b184c23f66944e98c2d[m
Merge: 36b366b b1a795e
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Feb 8 09:26:23 2016 -0800

    Merge pull request #786 from StephenBonikowsky/Issue785_S.P.SM_version_update
    
    Updating package version of S.Private.ServiceModel

[33mcommit 739980435c0df293a3ac6ab2bdbd9029e5c7cc82[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Sun Feb 7 13:15:36 2016 -0800

    Fix PortManager addresses for resource domain

[33mcommit b1a795eb19f74e812dbc65e6c46544b4ef331fdd[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Feb 5 14:00:51 2016 -0800

    Updating package version of S.Private.ServiceModel
    
    * Fixes Issue #785

[33mcommit 36b366b5877d290e2f510497c5f04fe0cede849b[m
Merge: ca1bc1c 3d81793
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Sat Feb 6 04:10:13 2016 +0800

    Merge pull request #782 from iamjasonp/netci
    
    Add unpack step to netci.groovy for corefx binaries

[33mcommit 3d8179342d9f1616e7add46166ff0a234995326d[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Fri Feb 5 19:54:25 2016 +0800

    Add unpack step to netci.groovy for corefx binaries
    
    Our Linux tests were not being run as corefx binaries were not being unpacked
    from the corefx build.pack. Unpack the artifacts from build.pack so that
    ./run-tests.sh can run in CI and produce us with test results

[33mcommit 495dc6c543b423cbba87b0dfa6be23bd6c16b961[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Feb 4 14:12:18 2016 +0800

    Rename BridgeCertificateBootstrapper to BridgeCertificateInstaller

[33mcommit ca1bc1c27d4078626f6335d74dd07f986c5012df[m
Merge: a94ec4b 93ed09f
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Feb 3 11:55:41 2016 -0800

    Merge pull request #770 from roncain/bridge-manual-start
    
    Allow Bridge to know whether it was manually started

[33mcommit 7b54aabf660e0eaab39a085e44a850fcd69d64e9[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Wed Feb 3 14:30:31 2016 +0800

    Bootstrapping script to install CA certificate from Bridge

[33mcommit 93ed09f7de62d3f3a20fff220346b2c7aa12a638[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Feb 3 09:05:04 2016 -0800

    Change to using CMD exit codes rather than environment variables
    
    EnsureBridgeRunning.cmd now returns different exit codes to indicate
    whether the Bridge was started or not, and it does not set environment
    variables.
    
    Build.cmd decides whether to reset or stop the Bridge based only
    on exit codes from EnsureBridgeRunning.

[33mcommit 479532593906cda0d9f9cd3f9711a8eaf2ba90eb[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Jan 28 02:26:38 2016 +0800

    Add Bridge Certificate "bootstrapper" for CA certs
    
    The BridgeCertificateBootstrapper:
    
    * Makes an initial connection to the Bridge
    * Downloads the certificate as a PEM to a filesystem path
    
    This tool allows *nix builds to install the CA root certificates prior to
    the tests running. This is because we do not have access via .NET APIs to the
    Root certificate store in OpenSSL
    
    Windows does not need this tool as we have direct access to the Root cert store

[33mcommit 73ea3bf0c391c30424169e535a7d1deaeb4e02e4[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Feb 2 13:47:03 2016 -0800

    Remove BridgeAutoStart and let scripts decide how to close Bridge
    
    The BridgeAutoStart property has been removed because it was too
    confusing.  Instead, 'Bridge.exe /require' will choose whether or
    not to start the Bridge and return an exit code with its decision.
    
    EnsureBridgeRunning.cmd sets an environment variable based on this
    exit code so that our normal build.cmd scripts can choose to stop
    the Bridge only if the script started it.  The rule is "If you
    start the Bridge, you are responsible for its shutdown. But if you
    find it already running, you are allowed to reset it (release its
    resources) but not to shut it down."
    
    This retains the ability to use StartBridge to obtain a running
    Bridge that tests will not automatically teardown.

[33mcommit a2d4dcc0b248f39003ab0e0683e631551da92461[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Feb 2 06:43:12 2016 -0800

    Rename Bridge property to BridgeAutoStart
    
    The property was renamed and its polarity reversed to match.
    This makes it more consistent with Bridge.exe -stopIfAutoStart

[33mcommit 52296e5eaf87edbfe6e3cc0895e3275a2888847b[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Feb 1 14:01:23 2016 -0800

    Allow Bridge to know whether it was manually started
    
    To enable automation of the Bridge in OuterLoop test
    runs in the lab, it is necessary for the Bridge itself
    to know whether it was manually started by the developer
    or automatically started by a test.
    
    The StartBridge.cmd script now propagates that information
    to the Bridge.  And a new -stopIfAutoStart switch to Bridge.exe
    will stop the Bridge only if it was not manually started.
    
    The old KeepBridgeRunning environment variable is no longer used.

[33mcommit a94ec4b1dab04fd2ef2f63ca27d488d0920e9e65[m
Merge: e20acd5 733e310
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Wed Feb 3 01:51:01 2016 +0800

    Merge pull request #771 from iamjasonp/update-package-rc3-23729
    
    Move project references to version rc3-23729

[33mcommit 733e310a0ee6ce3b46a9a816dfe4cbcee2c447ed[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Tue Feb 2 16:01:01 2016 +0800

    Make changes to run-test.sh for CI purposes
    
    CI relies on run-test.sh. Correct the syntax error in run-test.sh for
    Jenkins CI to work properly

[33mcommit ec50336448915241b31760a79df05a4a8dd471d3[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Tue Feb 2 14:33:27 2016 +0800

    Move project references to version rc3-23729

[33mcommit e20acd5b277ec83587da2cf3d9251d88b57038cc[m
Merge: 89da4d7 dd06d01
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Feb 1 08:51:51 2016 -0800

    Merge pull request #767 from roncain/remove-bridge-build-targets
    
    Deletes Bridge.Build.Tasks

[33mcommit dd06d01ee59650f1cc9855a37be48ab5c8aacdaf[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Jan 29 12:54:02 2016 -0800

    Deletes Bridge.Build.Tasks
    
    This project was originally intended to hold MSBuild tasks to
    deal with the Bridge, but is now obsolete and interferes with
    building within the ToF environment.  Its only existing task was
    to ask the Bridge to release its resources, but that functionality
    has since been moved into Bridge.exe itself and is invoked after
    tests have been run.

[33mcommit 89da4d7ac18498e80d43562e919cb804a5271c87[m
Merge: 8a2bc8c 525bdf4
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Feb 1 05:27:09 2016 -0800

    Merge pull request #768 from dotnet-bot/from-tfs
    
    Merge changes from TFS

[33mcommit 8a2bc8ca809e65b2b147147d3d9120be6e5d0300[m
Merge: f2f8e28 f9aa3c1
Author: Shin Mao <shmao@microsoft.com>
Date:   Fri Jan 29 17:07:25 2016 -0800

    Merge pull request #758 from shmao/676
    
    Tests covering using MessageHeaderAttribute with XmlSerializerFormatAttribute

[33mcommit 525bdf44931bdf2b928a2dc9d11a4f1b3dc258ff[m
Author: Christopher Costa <chcosta@microsoft.com>
Date:   Fri Jan 29 13:19:28 2016 -0800

    Fix casing of Packaging.props import.
    
    [tfs-changeset: 1570425]

[33mcommit f2f8e287025ef15359a0c72eca7bdda31116f992[m
Merge: 74c07ad e89f60a
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Jan 29 07:52:41 2016 -0800

    Merge pull request #766 from dotnet-bot/from-tfs
    
    Merge changes from TFS

[33mcommit 74c07ad06d50d6237dde8c2ebe02dc0b428edde3[m
Merge: 02c6563 e3a5404
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Jan 29 07:17:08 2016 -0800

    Merge pull request #765 from dotnet-bot/from-tfs
    
    Merge changes from TFS

[33mcommit e89f60a40f408bbf75a27fe939d4801cf07fafd4[m
Author: Wes Haggard <Wes.Haggard@microsoft.com>
Date:   Thu Jan 28 23:53:51 2016 -0800

    Fix build break when running GenFacades on the WCF projects by partially porting some of the slash changes introduced by recent updates to corefx and buildtools.
    
    [tfs-changeset: 1570315]

[33mcommit 02c65635ab4efc8ad8bbadad15da9f5d78501c27[m
Merge: 7b749b1 e672a7e
Author: Matt Connew <mconnew@microsoft.com>
Date:   Thu Jan 28 18:09:50 2016 -0800

    Merge pull request #762 from mconnew/FixWebSocketsStreaming
    
    Fix WebSockets duplex streaming when it completes async
    Fixes #760

[33mcommit 7b749b1bdaf75182a968ea40db9b2a505af5a3be[m
Merge: 70fa218 bf86566
Author: Matt Connew <mconnew@microsoft.com>
Date:   Thu Jan 28 18:09:16 2016 -0800

    Merge pull request #761 from mconnew/HttpsClientCertificates
    
    Enable client certificates on HTTPS on netcore50 and coreclr/windows
    Fixes #597

[33mcommit bf86566a6040c6fe6d2b8b826c987b3615356ecf[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Fri Jan 22 15:16:25 2016 -0800

    Test changes to test HTTP client certificate authentication

[33mcommit dea6c7bb8b59d5335101326cf6ca04e8c17ac1c1[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Fri Jan 22 15:13:00 2016 -0800

    Porting netcore HttpClient to modify and add Certificate support
    Also adding changes to HTTP binding code to enable client certificate usage

[33mcommit 19616da361bbb68c28b3beb795e4744a322dd47e[m
Author: Christopher Costa <chcosta@microsoft.com>
Date:   Thu Jan 28 16:06:01 2016 -0800

    Another update to remove "weird" characters in license files.
    
    [tfs-changeset: 1570039]

[33mcommit 70fa21889c056cf2a501ffcecdcdc5307ab8ce01[m
Merge: dbc40ee 1e3d5f0
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Jan 28 15:16:42 2016 -0800

    Merge pull request #754 from StephenBonikowsky/Issue749
    
    Add a unix package of System.Private.ServiceModel

[33mcommit 1e3d5f0c739f6340c32feedb1f5d0f7a661a87a2[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Jan 25 13:32:28 2016 -0800

    Add a unix package of System.Private.ServiceModel
    
    * WCF Core has a Windows and a Unix implementation both implemnentations need to be available in the package.
    * Fixes Issue #749

[33mcommit dbc40ee8d289bf6e0a62c62a319f988a7a11643c[m
Merge: 2e910f6 0a79ff2
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Jan 28 14:19:39 2016 -0800

    Merge pull request #763 from dotnet-bot/from-tfs
    
    Merge changes from TFS

[33mcommit e3a5404f2206774ddb239a7edc02a5b54e4113be[m
Author: Christopher Costa <chcosta@microsoft.com>
Date:   Thu Jan 28 12:52:01 2016 -0800

    Replace "weird" characters in dotnet_library_license.txt
    
    [tfs-changeset: 1569941]

[33mcommit f9aa3c1a6fd389de755ec508b35a2f478e4514ec[m
Author: Shin Mao <shmao@microsoft.com>
Date:   Thu Jan 28 12:16:35 2016 -0800

    Tests covering using MessageHeaderAttribute with XmlSerializerFormatterAttribute
    
       Added two tests covering using MessageHeaderAttribute with XmlSerializerFormatAttribute. One test is for the scenario where the request type uses [MessageHeader]; the other one is for the scenario where the response type uses [MessageHeader].
       The first test is currently failing due to #702. I've marked the test with [ActiveIssue].
       The second test currently passed in .Net Core, but failed in .Net Native. The fix for the second case would be mainly in toolchain, but it also includes some library changes (see XmlSerializerOperationBehavior.cs). These two parts of the fix have no dependency on each other.
    
    Fix #676

[33mcommit 0a79ff29a843f4aaa9e94d0fed24297a378d6b72[m
Author: Christopher Costa <chcosta@microsoft.com>
Date:   Thu Jan 28 11:24:02 2016 -0800

    Add 3rd party notice and eula to packages.
    
    [tfs-changeset: 1569920]

[33mcommit 2e910f61d9ad4b6414303db77f924d7d3dda9e13[m
Merge: 2c9b801 bdcaa3e
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Jan 28 07:03:03 2016 -0800

    Merge pull request #757 from roncain/require-bridge
    
    Adds new /require option to Bridge.exe to allow optional start

[33mcommit bdcaa3e73d54f8d2dfffe6974d977341809b0c25[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Jan 28 06:42:47 2016 -0800

    Update EnsureBridgeRunning.cmd to use new -require option in Bridge
    
    This change means EnsureBridgeRunning.cmd now is guaranteed to
    have started the Bridge in a new process when needed and established
    it is healthy before returning.
    
    This commit also updates the -require option to use seconds instead
    of TimeSpan and cleans up the help message for Bridge -?

[33mcommit 671f1d06e2f39eb1bd898cdc82f5388f103202c8[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Jan 27 13:06:06 2016 -0800

    Adds new /require option to Bridge.exe to allow optional start
    
    The new /require option will ensure the Bridge is running.
    If it is already running, it just returns.  But if it is not
    running it starts the Bridge.exe in a new process and waits for
    it to respond to pings.
    
    This new option is not yet used but will be in automated runs
    on Windows machines that require the Bridge is running before
    a test can execute.

[33mcommit 2c9b801ece45d9e59cbdc640947cc56c6c7ff8d3[m
Merge: 929f69b d5ca1c2
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Jan 28 04:57:53 2016 -0800

    Merge pull request #745 from roncain/bridge-timeout
    
    Make Bridge honor max idle timeout set from command line

[33mcommit e672a7e653fc949208ded378c1a9b4e8a7cc3c97[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Wed Jan 27 20:52:30 2016 -0800

    Fix WebSockets streaming when it completes async

[33mcommit 929f69ba11d8cfc5e213e8d14acb0f9304d48e75[m
Merge: 18f6873 0740ae8
Author: Hong Dai <hongdai@microsoft.com>
Date:   Wed Jan 27 11:18:02 2016 -0800

    Merge pull request #742 from hongdai/issue562
    
    Document how to collect and analyze ETW traces

[33mcommit 18f687325253b69a8d452f9de361f5dde0ec448b[m
Merge: 65cba6c fabdbba
Author: Shin Mao <shmao@microsoft.com>
Date:   Wed Jan 27 09:50:39 2016 -0800

    Merge pull request #751 from shmao/740
    
    Fixed a bug with MessageHeader.CreateHeader().

[33mcommit 0740ae8c3f758aef4f15ce2b1ce51f74adcc5ebe[m
Author: hongdai <hongdai@microsoft.com>
Date:   Wed Jan 27 09:17:42 2016 -0800

    Document how to collect and analyze ETW traces
    
    * This is to document how customers should collect and analyze ETW traces
    
    Fixes #562

[33mcommit fabdbbadea7f0e232a7e41192ddb06bb0d7799cb[m
Author: Shin Mao <shmao@microsoft.com>
Date:   Tue Jan 26 16:56:16 2016 -0800

    Add directive to rd.xml for MessageHeader.CreateHeader()
    
    When XmlSerializerFormatAttribute is marked on a OperationContract, .Net Native tool assums that the operation wants to serialize objects using XmlSerializer and so pre-generates XmlSerializer for all relevant types. However, MessageHeader.CreateHeader() internally used DataContractSerializer to serialize the header object, even if the operation is marked with XmlSerializerFormatAttribute.
    
    The fix is adding directives in the rd.xml to tell .Net Native tool to generate DataContractSerializer for the 'value' parameter of the MessageHeader.CreateHeader() method.
    
    I also added tests covering the case of using MessageHeader.CreateHeader together with XmlSerializerFormatAttribute.
    
    Fix #740

[33mcommit 65cba6c5f0d58f98e0c6aa9c6ca9222dc4fb8f1a[m
Merge: 5fa803f bcde10b
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Wed Jan 27 04:50:18 2016 +0800

    Merge pull request #753 from iamjasonp/buildtools-lineending
    
    Remove extra line ending in BuilToolsVersion.txt

[33mcommit bcde10b92dd0d32d8116664df6b24e1bd81ea98a[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Wed Jan 27 04:20:07 2016 +0800

    Remove extra line ending in BuilToolsVersion.txt
    
    This was causing a build break in Linux, and a directory was being created
    as "1.0.25-prerelease-00154^M" causing all sorts of weirdness

[33mcommit 5fa803ff79524f6536a654a13b26d885fd9c447a[m
Merge: bafc0af 144f6a9
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Jan 26 11:49:51 2016 -0800

    Merge pull request #752 from iamjasonp/update-packages-rc3-23725
    
    Update packages to rc3-23725

[33mcommit 144f6a9e05fa3a0ae43fa454261d38080557d747[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Wed Jan 27 00:54:11 2016 +0800

    Add System.NETCore.Platforms to S.Private.ServiceModel project.json
    
    This pulls in the runtime ID graph to map ubuntu -> unix during build

[33mcommit ea3557f8b50bffabc1795a7cdbdb34d15cdd7ab8[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Tue Jan 26 16:13:29 2016 +0800

    Update BuildTools version to 1.0.25-prerelease-00154

[33mcommit 41f67b85995633923c43826677f5cb39c69f9760[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Tue Jan 26 16:10:46 2016 +0800

    Add dir.props validation for TargetingPacks
    
    Add Microsoft.TargetingPack.Private.WinRT-1.0.0 to
    System.Private.ServiceModel/windows/project.json as rc3 packages no longer includes
    WinRT-specific libraries in netcore50 (.NET Native)

[33mcommit 8703464531e5b589fb4f6970b5e8fe0cde1ef97c[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Tue Jan 26 03:28:40 2016 +0800

    Add imports for dnxcore
    
    This avoids a compat issue with xunit which caused the assembly
    xunit.abstractions not to be resolved for test projects

[33mcommit 6cf89407133a7807c8050105a79c29a2a3cd9824[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Mon Jan 25 11:25:38 2016 +0800

    Update package versions to rc3-23725

[33mcommit d5ca1c240ef6b2e562b386bc895f66fca45b1c14[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Jan 25 12:12:14 2016 -0800

    Add script to conditionally start Bridge
    
    This CMD is meant to be called only on Windows machines
    that need to ensure the Bridge is running without the
    overhead checking whether it needs to be built.
    
    It is intended to be invoked in automated Windows test
    runs that require the Bridge.

[33mcommit bafc0af280484a5767a049e2f57bd36f35e055eb[m
Merge: df19dda eecdda8
Author: Davis Goodin <dagood@users.noreply.github.com>
Date:   Mon Jan 25 13:14:29 2016 -0600

    Merge pull request #746 from dagood/escape-validation-regex
    
    Escape *'s in validation regex

[33mcommit df19dda5e4a857f0deb7db2741e6894d0ca1c632[m
Merge: 93ffb6c ad6bd41
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Jan 25 09:09:50 2016 -0800

    Merge pull request #744 from StephenBonikowsky/NetciCommandFix
    
    Fixing mistake in PR #739 v2.

[33mcommit eecdda8e91923f8177102fa0e56c880f6c98dfb1[m
Author: dagood <dagood@microsoft.com>
Date:   Mon Jan 25 10:17:27 2016 -0600

    Escape *'s in validation regex and remove invalid TestData rule.

[33mcommit 6861c2f6e33325493b52edd28a794b2a398aae6b[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Jan 25 05:41:42 2016 -0800

    Make Bridge honor max idle timeout set from command line
    
    Prior to this change, the Bridge initialized its max idle
    timeout to a fixed value and did not adjust for a value being
    passed on the commandline at startup.
    
    It was necessary to fix this issue so that a Bridge launched
    on a lab machine can be given a maximum idle timeout.  This
    permits the Bridge to close gracefully regardless how a test
    run completes.

[33mcommit 93ffb6c578bfd581829fb0d82474ef00f1fd96e8[m
Merge: f735e9e ed62eaf
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Jan 25 04:42:32 2016 -0800

    Merge pull request #726 from roncain/update-rd-xml
    
    Update rd.xml to allow instantiation of open generic FaultException

[33mcommit f735e9e38682b7d8415613da9f19be35e78e3cc1[m
Merge: bb1763f 9d63045
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Sat Jan 23 21:43:14 2016 +0800

    Merge pull request #738 from iamjasonp/bridge-cert-expiry
    
    Increase Bridge CA cert expiry to 7 days

[33mcommit bb1763f07b91c4d229261080e4fa400a09ef8511[m
Merge: 78a1e24 c3d4f02
Author: Matt Connew <mconnew@microsoft.com>
Date:   Fri Jan 22 19:33:42 2016 -0800

    Merge pull request #743 from mconnew/FixGitIgnore
    
    Fixing rule to ignore /Tools folder

[33mcommit ad6bd413266f0515fffd87a50986c5a3b61d4237[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Jan 22 15:58:04 2016 -0800

    Fixing mistake in PR #739 v2.
    
    * Line 292 the variable 'os' is not known, hardcoding it since calls to build.cmd and running OuterLoops can only be on Windows.

[33mcommit c3d4f02dab3deb77d69bcc7ff3b574a1beaa253f[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Fri Jan 22 15:54:00 2016 -0800

    Fixing rule to ignore /Tools folder
    
    The rule was also matching src/System.Private.ServiceModel/tools/* which meant new files in that folder were ignored

[33mcommit 78a1e241e76b14b76cb7df0ad6321e3e0f59ce52[m
Merge: 653466b 2acd7df
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Jan 22 13:59:45 2016 -0800

    Merge pull request #741 from StephenBonikowsky/NetciCommandFix
    
    Fixing mistake in PR #739

[33mcommit 2acd7df75953babf2f71eade7421088688eb7ab3[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Jan 22 13:36:39 2016 -0800

    Fixing mistake in PR #739
    
    * Line 277 the variable 'os' is not known, hardcoding it since calls to build.cmd and running OuterLoops can only be on Windows.

[33mcommit 653466be60c2aa0332b7afaf9b2011ed344f5c81[m
Merge: f6d5c49 ec2c70a
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Jan 22 12:27:32 2016 -0800

    Merge pull request #739 from StephenBonikowsky/NetciCommandFix
    
    Fixing command line used by Jenkins to build and run CI tests.

[33mcommit ec2c70a105964ccbdd88c8f23267b3de4a9e0be2[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Jan 22 11:35:52 2016 -0800

    Fixing command line used by Jenkins to build and run CI tests.
    
    * The value used for the parameter "/p:Configuration" will now be the OS name plus either Debug or Release.
    * Like this: Windows_NT_Debug
    * When the configuration is just "Debug" or "Release" WCF OuterLoop test projects are running against the wrong build of the product, there is broken logic in the build process around this which has not yet been figured out, this is a short term solution.
    * Fixes Issue #731

[33mcommit 9d63045dc9f23470430a35bf56ede8144bf036ce[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Sat Jan 23 03:28:13 2016 +0800

    Increase Bridge CA cert expiry to 7 days
    
    Currently, the expiration time for certificates is only 1 day, which means
    that keeping the Bridge open over multiple days will result in certs expiring,
    making debugging more difficult. Change this expiry time to 7 days so that a
    Bridge can be kept up for a longer period of time.

[33mcommit f6d5c4947ff2ccecff42642e81999d55946e0fed[m
Merge: 6cffb5c a70321b
Author: Hong Dai <hongdai@microsoft.com>
Date:   Fri Jan 22 08:18:58 2016 -0800

    Merge pull request #729 from hongdai/issue690
    
    Enable ETW when throwing exceptions

[33mcommit 6cffb5c53ee96162186b787df71735f14d7db3ec[m
Merge: 2cf52ca d8e34fe
Author: Matt Connew <mconnew@microsoft.com>
Date:   Thu Jan 21 16:09:54 2016 -0800

    Merge pull request #725 from mconnew/MessageEncoderContentTypeFix
    
    Message encoder content type fix
    Fixes #733

[33mcommit 2cf52cacc1cea422c434e50732bfbde8e8733fa0[m
Merge: b402552 93f5aaa
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Jan 21 15:24:14 2016 -0800

    Merge pull request #732 from roncain/update-to-corefx
    
    Take recent CoreFx updates to buildtools and scripts

[33mcommit 93f5aaa4481327c6cc7d8834b5b13d828c514fed[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Jan 21 07:02:22 2016 -0800

    Take recent CoreFx updates to buildtools and scripts

[33mcommit a70321b2e610ee3c2f3a4f740e5f590889cc4a6b[m
Author: hongdai <hongdai@microsoft.com>
Date:   Thu Jan 21 10:14:06 2016 -0800

    Enable ETW when throwing exceptions
    
    * Change the base Event source name to System.ServiceModel as it
    becomes internal to us now.
    * The code logic for handling different Event level is the same as Desktop.
    
    Fixes #690

[33mcommit d8e34fe04af6f0021ab6f43fb6c0d92360f39200[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Wed Jan 20 13:24:25 2016 -0800

    Adding tests for MessageEncoder content type fix

[33mcommit ed62eaf76c5d8fc749317027b30b7649facbc98f[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Jan 20 13:23:18 2016 -0800

    Update rd.xml to allow instantiation of open generic FaultException
    
    The NET Native toolchain has been improved to allow the instantiation
    of open generics via a new entry in the rd.xml file.  This PR adds that
    new line.
    
    The rd.xml already had a similar line that permitted this
    for generic types where the T was a class. That line has been left
    unmodified to mitigate risk if a new WCF package is used with an
    older toolchain.
    
    An app was built containing this new syntax and compiled against the
    older toolchain to demonstrate the new syntax would not break it.
    
    Fixes #664

[33mcommit b6c27d13e4628a887e936785f0fb6869d597f662[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Tue Jan 19 11:28:11 2016 -0800

    Fix Message Encorder Content Type Fix

[33mcommit b402552caf98b3a8642e7df96c0e34ad775c5c91[m
Merge: 8149a9f a8b050e
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Jan 20 09:34:50 2016 -0800

    Merge pull request #723 from roncain/corefx-updates
    
    Take latest CoreFx changes to init build tools and CLI

[33mcommit a8b050e07aedecaa1fb20e7aabd0f804a36d83c8[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Jan 20 06:55:56 2016 -0800

    Take latest CoreFx changes to init build tools and CLI

[33mcommit 8149a9f58307e850cc4b15bd101ce11dcff429f1[m
Merge: 1ad386f faf59e6
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Jan 20 06:48:25 2016 -0800

    Merge pull request #722 from dotnet-bot/from-tfs
    
    Merge changes from TFS

[33mcommit faf59e682fcc04fba1ff20576cff81c505929e5a[m
Author: Matt Ellis <matell@microsoft.com>
Date:   Tue Jan 19 12:17:11 2016 -0800

    Update Prerelease Tag to RC3
    
    Since we have branched for RC2, we should use a different pre-release
    tag for builds out of ProjectK/master.
    
    [tfs-changeset: 1566436]

[33mcommit 1ad386f60c8722b7274e84e4c302259b9d535cdf[m
Merge: a2df46a af891ca
Author: Ron Cain <roncain@microsoft.com>
Date:   Sat Jan 16 16:24:42 2016 -0800

    Merge pull request #717 from weshaggard/FixRaceConditionForConflictingGlobalProperties
    
    Undefine PackagingTargetFramework property to avoid different global properties

[33mcommit af891ca9b607fac9234beee2e88d5c92b7e45116[m
Author: Wes Haggard <Wes.Haggard@microsoft.com>
Date:   Sat Jan 16 15:32:27 2016 -0800

    Undefine PackagingTargetFramework property to avoid different global properties
    
    By default the PackageTargetFramework flows down to ProjectReferences for pkgprojs,
    that is indended when referencing another pkgproj but not when you reference
    a .builds or .csproj file as that will mess with the global properties for those
    projects and cause race conditions.

[33mcommit a2df46a59d1c31ea34da142f4fed1d572208832a[m
Merge: 55a5cc5 1baa4fb
Author: Matt Connew <mconnew@microsoft.com>
Date:   Fri Jan 15 15:49:42 2016 -0800

    Merge pull request #715 from mconnew/UseMinimumDependencies
    
    Use minimum dependencies

[33mcommit 1baa4fb2c69e47589b203cf77e4e718d683fd994[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Fri Jan 15 14:29:14 2016 -0800

    Small updates to package versions

[33mcommit 42f76413a9d5d94130ad3a727dc4454191791b67[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Thu Jan 14 19:22:14 2016 -0800

    Fix unhandled exception caused by accessing disposed socket when tracing event

[33mcommit f5ee80e777c16e1c9e96293cb91785af44fb97ff[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Thu Jan 14 19:21:26 2016 -0800

    Change dependencies to the lowest stable versions needed

[33mcommit 1e491afd0af5bf1f0d488a6bc602d5233540f969[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Thu Jan 14 19:20:15 2016 -0800

    Code cleanup needed to be able to lower dependency version

[33mcommit 14a5467f75e3692739268cfbf9f3285b2c462d38[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Thu Jan 14 13:47:07 2016 -0800

    Add configurations to solution file to allow selecting product configurations in VS

[33mcommit 55a5cc52df9b907e6f00d613ee833d4b993518f6[m
Merge: 7c25057 9ab3352
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Jan 14 18:28:24 2016 -0800

    Merge pull request #707 from roncain/pkg-build
    
    Enable the packaging targets to build

[33mcommit 9ab335292e8690bfcc471c9cea18228e81b6b8eb[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Jan 14 14:07:51 2016 -0800

    Enable the packaging targets to build

[33mcommit 7c250575a0357741f4862425117ac25e4752681f[m
Merge: 2628132 4eeffa3
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Jan 14 14:49:41 2016 -0800

    Merge pull request #705 from dotnet-bot/from-tfs
    
    Merge changes from TFS

[33mcommit 4eeffa33572b753bd6cfa05cf212484228c2f53e[m
Author: dotnet-bot <dotnet-bot@microsoft.com>
Date:   Thu Jan 14 11:45:43 2016 -0800

    [ericstj] Fix WCF package build
    
    Many packages were missing supports tags
    for frameworks that included the contracts.
    
    Additionally, we were including a facade in the
    package when it should be provided by targeting
    pack / OS.
    
    [tfs-changeset: 1565098]

[33mcommit 26281323eb3ceeebcad3599e6700efc808c1bb5e[m
Merge: 7ce0eb5 ca6eb89
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Jan 14 11:22:51 2016 -0800

    Merge pull request #704 from dotnet-bot/from-tfs
    
    Merge changes from TFS

[33mcommit ca6eb89ec77fff54078c5da72033102f7c9c01b7[m
Author: dotnet-bot <dotnet-bot@microsoft.com>
Date:   Thu Jan 14 11:03:25 2016 -0800

    [WesH] Fix incorrect project name in .builds file.
    
    [tfs-changeset: 1565076]

[33mcommit 7ce0eb573d719b9585edcb45b1d01dd15e11c27e[m
Merge: 2d13521 b5a7448
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Jan 14 07:45:18 2016 -0800

    Merge pull request #683 from roncain/update-corefx-workflow
    
    Take new dev workflow changes from CoreFx

[33mcommit b5a744870f19d40ca474f07bfda358acabb2cfe4[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Jan 14 06:52:28 2016 -0800

    Disable BuildInParallel to eliminate race
    
    There appears to be a new race condition in building
    strings.resx, so set $(BuildInParallel) to false when
    building all projects.

[33mcommit d68be3b8ca6a9eb6ed5910ad521cab092fa7f450[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Jan 14 05:25:25 2016 -0800

    Take package project changes and latest CoreFx changes

[33mcommit 46bc083e4c04574753fee61e2d48a8ad9227f02b[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Jan 13 08:22:09 2016 -0800

    Take most recent build tool version and script updates

[33mcommit 77bd3160c93e63e410ef672ba85b5d3d9275b518[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Jan 12 09:21:40 2016 -0800

    Take new dev workflow changes from CoreFx

[33mcommit 2d13521454263241454b978779e70f235cff22e7[m
Merge: b9878a8 d8704e6
Author: Matt Connew <mconnew@microsoft.com>
Date:   Wed Jan 13 22:31:43 2016 -0800

    Merge pull request #699 from mconnew/Issue157
    
    Remove ActiveIssue for ServiceContract_TypedProxy_DuplexCallback

[33mcommit b9878a8b624a6b13c4677067a6e1dcaa8d33fbb8[m
Merge: ca20c81 a242520
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Thu Jan 14 11:15:38 2016 +0800

    Merge pull request #695 from iamjasonp/update-tests-negostream
    
    Enable NegotiateStream tests for non-Unix platforms

[33mcommit a242520096e53911ff783a008b0d3dcffe0d849c[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Jan 14 03:06:19 2016 +0800

    Enable NegotiateStream tests for non-Unix platforms

[33mcommit 7c043087901c24ad453dbf16733d1570ffe93a80[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Jan 14 03:03:17 2016 +0800

    Modify System.Private.ServiceModel to have default config
    
    * This change will bring us into line with corefx projects that have a dependency
      on the $(TargetsWindows) condition
    * Fix a small typo that was affecting $(DefineConstants)
    * Remove extra Debug/Release Configurations not present in the .builds file to
      prevent errors if attempting to build those configs in VS

[33mcommit d8704e6d3bc988a26df5eb6cef56cab18e8d2db7[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Wed Jan 13 17:11:19 2016 -0800

    Remove ActiveIssue for ServiceContract_TypedProxy_DuplexCallback
    
    fixes #157

[33mcommit ca20c81d9f02d0b49e8a01f7325f73ea1ad23bb0[m
Merge: 699ec76 9bed53a
Author: Matt Connew <mconnew@microsoft.com>
Date:   Wed Jan 13 17:02:28 2016 -0800

    Merge pull request #694 from StephenBonikowsky/pkgprojects
    
    Supporting package building in WCF.

[33mcommit 9bed53a3af5a918cf631f27235e8402eed22cb19[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Jan 13 16:53:02 2016 -0800

    Additional fixes.
    
    * Fixing OSGroup condition in dir.props that was causing wrong bits to get built and failing OuterLoop tests.
    * Typo in System.Private.ServiceModel.csproj that would cause the constant SUPPORTS_WINDOWSIDENTITY to not work.

[33mcommit 3260a445c0a45ed403b06f94d9d3fd2712ed4bd4[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Jan 13 16:18:50 2016 -0800

    Updated with feedback
    
    * Fixed names of files under src\System.Private.ServiceModel\pkg\
    * Fixed indent in src\System.ServiceModel.Security\pkg\System.ServiceModel.Security.pkgproj

[33mcommit 840b277cfffb9fcedc3f926cd69053a084f771a8[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Jan 13 09:31:25 2016 -0800

    Supporting package building in WCF.
    
    * Add .pkgproj files and .builds files to enable building WCF packages in OSS.

[33mcommit 699ec76b18f1dccd0cc3404272bc07427843b92e[m
Merge: 4b1cf08 65eab87
Author: Matt Connew <mconnew@microsoft.com>
Date:   Wed Jan 13 16:14:14 2016 -0800

    Merge pull request #687 from mconnew/FixNetNativeFlushing
    
    Fix bug where async write was reporting completion before the Flush completed
    fixes #524, fixes #521, fixes #587, fixes #520

[33mcommit 4b1cf0887521b121b6b99947df0cb045051f459a[m
Merge: ed53877 fa66ad2
Author: Matt Connew <mconnew@microsoft.com>
Date:   Wed Jan 13 16:02:22 2016 -0800

    Merge pull request #678 from mconnew/Eventing
    
    Enabled more ETW events, including adding activity id tracing

[33mcommit ed538777a6bacb2ed2f144dcb8659a8338d1c1ec[m
Merge: 5959d7c 3d39210
Author: Shin Mao <shmao@microsoft.com>
Date:   Wed Jan 13 11:12:43 2016 -0800

    Merge pull request #680 from shmao/672
    
    Use Assert instead of errorBuilder in MessageContractTests.cs

[33mcommit 5959d7ca260b7c55e46f294f0bfb7a6599b152a7[m
Merge: cd0f93a 42449e8
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Thu Jan 14 02:05:40 2016 +0800

    Merge pull request #693 from iamjasonp/update-project-json-23712
    
    Update project.json files to package version rc2-23712

[33mcommit 42449e8c65d390fa4b3c16ce7f6f8d04024613f3[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Wed Jan 13 16:19:27 2016 +0800

    Update project.json files to package version rc2-23712

[33mcommit cd0f93ae8f3183f87482ae0d8e24ea0df7853a1f[m
Merge: 7047a04 22e1960
Author: Hong Dai <hongdai@microsoft.com>
Date:   Tue Jan 12 22:02:45 2016 -0800

    Merge pull request #682 from hongdai/fixrc2
    
    Add In Progress

[33mcommit 7047a04881f7bbc3d738605bc90ae9558b8898df[m
Merge: 34c2d7a 0b17330
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Jan 12 17:42:03 2016 -0800

    Merge pull request #673 from StephenBonikowsky/Issue637
    
    Adding .builds file and applying new conventions to S.P.SM.csproj

[33mcommit 0b173306e83f16b7411d657523a8c3ce3b7bb6a9[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Jan 12 11:37:20 2016 -0800

    Updating infrastructure files in WCF root v2.
    
    * Pulling in changes based on corefx in order to support building the new .builds projects.
    * Based on review from Ron Cain after first attempt.

[33mcommit 3d39210bd45beb28c731fa8e1d0a8db817d9c4c8[m
Author: Shin Mao <shmao@microsoft.com>
Date:   Tue Jan 12 15:42:01 2016 -0800

    Use Assert instead of errorBuilder in MessageContractTests.cs
    
    Fixes #672

[33mcommit 65eab87ef3521cad3ecd807cb23984a073d82b6a[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Tue Jan 12 00:02:51 2016 -0800

    Fix bug where async write was reporting completion before the Flush completed

[33mcommit fa66ad265807b65dd45be62749edd4f7efef3d79[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Tue Jan 12 15:12:59 2016 -0800

    Remove events not used in OSS code base

[33mcommit 6d515edfbdc2aa408d1ad1b40eb7f30f726039d2[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Thu Jan 7 13:38:03 2016 -0800

    Enabled more ETW events, including adding activity id tracing

[33mcommit 22e1960350a8066c2ade4afef4f77e6116432ced[m
Author: hongdai <hongdai@microsoft.com>
Date:   Tue Jan 12 13:37:43 2016 -0800

    Add In Progress
    
    *Per Zhenglan's feedback, explicitly call out this is a work in progress.

[33mcommit df300f3f329ca25be3abadc30d162e6348f0cd6d[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Jan 11 14:07:49 2016 -0800

    Addressing feedback for PR on adding .builds files.
    
    * Remove the generic project include that defaults to 'AnyOS' in each of the .builds projects.
    * Explicitly pick the Windows_NT version of the S.P.SM project reference in each of the facade projects.

[33mcommit 0cd1578b496777b8bff0b38d94d2aafe08978d73[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Jan 6 16:18:44 2016 -0800

    Adding .builds file and applying new conventions to ServiceModel project.
    
    * Fixes #637

[33mcommit 34c2d7aba6e23f52ae7b6a9a08eb6f78d958bc33[m
Merge: 10b975d b8090df
Author: Shin Mao <shmao@microsoft.com>
Date:   Tue Jan 12 09:15:27 2016 -0800

    Merge pull request #675 from shmao/51
    
    Call Dispose() on the XmlWriter after use.

[33mcommit 10b975df331e2151345cbf4a83cdca4994a12d7e[m
Merge: 1c30373 275f3c6
Author: Hong Dai <hongdai@microsoft.com>
Date:   Tue Jan 12 08:47:04 2016 -0800

    Merge pull request #679 from hongdai/665
    
    Create supported-feature matrix

[33mcommit 275f3c68772f43ea838ebd41e399c8b234eb3699[m
Author: hongdai <hongdai@microsoft.com>
Date:   Mon Jan 11 11:34:34 2016 -0800

    Create supported-feature matrix
    
    * This is just to clone RC1 release note. Once RC2 test completes, it will
    be updated.

[33mcommit b8090dfaef5dea1c413a95567312493437f39e8d[m
Author: Shin Mao <shmao@microsoft.com>
Date:   Mon Jan 11 10:38:06 2016 -0800

    Call Dispose() on the XmlWriter after use.
    
    Fixes #51

[33mcommit 1c30373a0ed296c13095525f4175311aa3105d18[m
Merge: bea4df8 abe2264
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Fri Jan 8 19:16:44 2016 +0800

    Merge pull request #674 from iamjasonp/certificate-linux-validity
    
    Modify CertificateGenerator to fix Linux testing issues

[33mcommit bea4df8b99333ffccc0e42f42b45b6b16d9fed1d[m
Merge: 4babf1d 595bb2f
Author: Hong Dai <hongdai@microsoft.com>
Date:   Thu Jan 7 14:04:06 2016 -0800

    Merge pull request #662 from hongdai/issue655_1
    
    Add a test to cover OperationContextScope major scenario

[33mcommit 595bb2f2ed4e873675b37ee421662fce7ad3d62c[m
Author: hongdai <hongdai@microsoft.com>
Date:   Thu Jan 7 13:19:35 2016 -0800

    Add a test to cover OperationContextScope major scenario
    
    * we currently does not have OperationContextScope coverage. This is to
    ensure we cover the major usage.
    
    Fixes #655

[33mcommit 4babf1d8b686af1b788c9b3c4f4fce8979ef3710[m
Merge: ee35906 191e816
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Jan 7 05:22:55 2016 -0800

    Merge pull request #669 from roncain/faultexception
    
    Add scenario test for FaultException using primitive type.

[33mcommit abe226482af18bb68f6dc17752e8012e7506c688[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Jan 7 17:07:33 2016 +0800

    Allow Certificate Authority certificate to be exported as PEM
    
    To test with certs in Linux, we need to import the certificate to the root cert
    store as a PEM file. We don't currently allow certificates to be exportable
    easily (they get returned as JSON key-pairs for parsing at the client side).
    
    With this change we allow the cert to be retrieved from the Bridge in PEM format
    so we can script an import as necessary.

[33mcommit 2ba7990780c4629034c42a179c756f34ccf9de7e[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Wed Jan 6 19:16:59 2016 +0800

    Modify CertificateGenerator for OpenSSL compatibility
    
    * Fix bug in certificate issuer serial number - this was causing an issue when
      OpenSSL was verifing the root cert Authority Key Identifier
    * Modify some X509 Extensions to no longer be critical, as this was causing
      certificate validation issues when validating against OpenSSL. The certs
      now generated are more consistent with what one would typically see with
      public root CAs in terms of critical extensions
    * Increase the amount of grace period for CRL validity to deal with clock skew
      between machines

[33mcommit ee35906f8f9d17559741f17eb7ffb9265b1ea4c5[m
Merge: ec387aa 382f537
Author: Shin Mao <shmao@microsoft.com>
Date:   Wed Jan 6 16:17:12 2016 -0800

    Merge pull request #659 from shmao/632
    
    Add Tests for MessageHeaderAttribute.

[33mcommit ec387aa814ea9a59fcfc648b3f57586cc9a5bc8a[m
Merge: 2accbe4 0b85fa4
Author: Matt Connew <mconnew@microsoft.com>
Date:   Wed Jan 6 13:46:09 2016 -0800

    Merge pull request #663 from mconnew/Issue604
    
    Exclude System.Security.Principal.Windows for *NIX builds
    
    Fixes #604

[33mcommit 191e816a186c50af318730388a958fba7bbf7ed8[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Jan 6 10:15:44 2016 -0800

    This PR adds a new scenario test that demonstrates primitive types
     can be used in FaultContracts. It is known to succeed in CoreCLR
     but expected to fail in NET Native.
    
    Making this test succeed in NET Native will occur in a different PR.

[33mcommit 2accbe4da488a605f3d6b11702602416f8d0ca05[m
Merge: aeeea50 e2342e3
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Jan 6 08:31:26 2016 -0800

    Merge pull request #646 from dagood/remove-lockfiles-enable-validation
    
    Remove lockfiles, update dependency versions, and enable dependency upgrade/validation tooling

[33mcommit 0b85fa490d67f82751e311d00815ede9c3de815b[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Tue Jan 5 17:11:04 2016 -0800

    Remove mention of UWP in exception message
    
    Also cleaned up username returned from GetCurrentUserIdAsString when
    Windows Security isn't available.

[33mcommit 382f5376e0d9fcadc8606021e36579101daec227[m
Author: Shin Mao <shmao@microsoft.com>
Date:   Tue Jan 5 16:21:01 2016 -0800

    Add Tests for MessageHeaderAttribute.
    
    Fixes #632.

[33mcommit 2842d1d2e435b2b7d67d3c28ecae994c9208c86c[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Mon Jan 4 14:13:30 2016 -0800

    Exclude System.Security.Principal.Windows for *NIX builds

[33mcommit aeeea50337296580deba8fbdcad64c57a406ccfb[m
Merge: f815513 c65d5d0
Author: Hong Dai <hongdai@microsoft.com>
Date:   Tue Jan 5 09:58:33 2016 -0800

    Merge pull request #653 from hongdai/issue69
    
    Remove Digest test that requires a domain

[33mcommit e2342e3b3632bb4575b374f34b9ed40bcbd80681[m
Author: Davis Goodin <dagood@users.noreply.github.com>
Date:   Tue Jan 5 11:06:55 2016 -0600

    Add floating version property usage to developer guide.

[33mcommit f815513b71f70644f582ca170700fa1cfc5d0fb1[m
Merge: 6736b41 35789e3
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Tue Jan 5 13:35:15 2016 +0800

    Merge pull request #651 from iamjasonp/cert-claim-exception
    
    Change X509CertificateClaimSet to throw if static ctor thows

[33mcommit b6e1d8b6e96ad6e80e2bc5c3f1f177542fbde5d6[m
Author: dagood <dagood@microsoft.com>
Date:   Mon Jan 4 17:44:09 2016 -0600

    Remove reference to removed manual floating test-runtime.

[33mcommit 6736b41571a33fc585cef21ce8d1bf74a84e38e2[m
Merge: 677e283 fd28f6a
Author: Hong Dai <hongdai@microsoft.com>
Date:   Mon Jan 4 14:37:05 2016 -0800

    Merge pull request #660 from hongdai/changeflag
    
    Remove OpenExistingOnly flag

[33mcommit 22998db02e379092318754d728b3b17445d7aa1c[m
Author: dagood <dagood@microsoft.com>
Date:   Tue Dec 29 12:09:14 2015 -0600

    Generate copy of the test-runtime project.json that uses floating (*) dependencies when FloatingTestRuntimeDependencies is true.

[33mcommit 22a94aa6a7b001e87369154622220ad39438f977[m
Author: dagood <dagood@microsoft.com>
Date:   Mon Dec 14 14:19:47 2015 -0600

    Remove all lock files and ignore them.

[33mcommit 9c0fe60ddd7b9f2f5ebb86d5852c77b4a6530753[m
Author: dagood <dagood@microsoft.com>
Date:   Mon Dec 14 14:14:25 2015 -0600

    Update all project.json files to match rules.

[33mcommit 3ad12671d8b2bc50da1abf342648af6d65c5d5ef[m
Author: dagood <dagood@microsoft.com>
Date:   Mon Dec 14 14:13:06 2015 -0600

    Add dependency validation step and docs link.

[33mcommit 677e283513b2e3aff7c6e16e6942a70bcf2a09e7[m
Merge: bd0572f 6661fe8
Author: Matt Connew <mconnew@microsoft.com>
Date:   Mon Jan 4 13:48:04 2016 -0800

    Merge pull request #658 from mconnew/BuildOnUnix
    
    Enable and document linux (Ubuntu) build instructions

[33mcommit 6661fe83493229ba2a0b1945f0dc005d7d14c55f[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Mon Jan 4 13:09:42 2016 -0800

    Enable and document linux (Ubuntu) build instructions for WCF

[33mcommit a330ac9937d0a77dc02448f320add3da164ae92e[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Mon Jan 4 13:03:48 2016 -0800

    Copying build script and instructions from corefx repo

[33mcommit fd28f6a92b6e655cf8ec340ee03f6c544fc9f487[m
Author: hongdai <hongdai@microsoft.com>
Date:   Mon Jan 4 10:00:47 2016 -0800

    Remove OpenExistingOnly flag
    
    * This flag is no longer needed according to Jason. Removing it because it
    requires we pre create cert folders on Linux.

[33mcommit c65d5d04bd436b5451e4cf3436b776309a845ce4[m
Author: hongdai <hongdai@microsoft.com>
Date:   Mon Jan 4 09:20:01 2016 -0800

    Fix the issue type for Digest test that requires a domain
    
    Fixes #69

[33mcommit bd0572ffce86ca2d6a767ee6476fadab5b2dea8d[m
Merge: ea344e3 46c60f6
Author: Shin Mao <shmao@microsoft.com>
Date:   Wed Dec 30 13:00:51 2015 -0800

    Merge pull request #657 from shmao/23628
    
    Update project.json dependencies to rc2-23628.

[33mcommit 46c60f6fcf422843b7f0c41f1111d69b437aa94a[m
Author: Shin Mao <shmao@microsoft.com>
Date:   Mon Dec 28 16:58:33 2015 -0800

    Update project.json dependencies to rc2-23628.
    
    1. Updated project.json files.
    2. Changed to use NegotiateStream.AuthenticateAsServerAsync() instead of  NegotiateStream.AuthenticateAsServer() as the later one got removed.
    
    Fixes #656.

[33mcommit ea344e39d53551706700173d83294ce9c1f4418f[m
Merge: 7604283 103946a
Author: Matt Mitchell <mmitche@microsoft.com>
Date:   Tue Dec 22 10:28:11 2015 -0800

    Merge pull request #652 from mmitche/change-corefx-inputs
    
    Change the corefx inputs to be from the new corefx jobs

[33mcommit 103946a7603088817f806b5ceb3d7bf286a9081c[m
Author: Matt Mitchell <mmitche@microsoft.com>
Date:   Tue Dec 22 09:43:15 2015 -0800

    Change the corefx inputs to be from the new corefx jobs

[33mcommit 35789e33136b6c53b08be6ebc8b76931cb8e848d[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Mon Dec 21 17:32:44 2015 +0800

    Change X509CertificateClaimSet to throw if static ctor thows
    
    Exceptions happening on static ctors will be silently swallowed. This makes bugs
    really hard to find. Change X509CertificateClaimSet to throw when one of the
    properties is accessed and initialization was not successful

[33mcommit 7604283e8b7f4d4cc2a66029eee867e703788413[m
Merge: d6cddaa 4c76923
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Fri Dec 18 17:20:12 2015 +0800

    Merge pull request #603 from iamjasonp/san-check-cn
    
    Allow certificate claim checking to fall back to using CN

[33mcommit 4c76923567db76f145a393f8937704ee932dfe98[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Wed Dec 16 19:25:30 2015 +0800

    Add test for certificate claim check fallback to CN
    
    Verifies that when a Subject Alternative Name is not specified in an endpoint certificate,
    we are able to fall back to Subject CN to verify the endpoint

[33mcommit d6cddaa02d39c498f28f69ae96abd4d30c21664c[m
Merge: 965afaa 9157997
Author: Matt Connew <mconnew@microsoft.com>
Date:   Thu Dec 17 17:36:42 2015 -0800

    Merge pull request #650 from dotnet-bot/from-tfs
    
    Merge changes from TFS

[33mcommit 965afaa1b13308ac1ffddc3a48588f8c3046ca9e[m
Merge: 8ec79d5 21afa19
Author: Matt Connew <mconnew@microsoft.com>
Date:   Thu Dec 17 15:16:15 2015 -0800

    Merge pull request #649 from dotnet-bot/from-tfs
    
    Merge changes from TFS

[33mcommit 9157997f00376076192203c4ebc88ad7f1dfe07e[m
Author: Wes Haggard <Wes.Haggard@microsoft.com>
Date:   Thu Dec 17 14:10:17 2015 -0800

    Fix DNX and BuildTools versions in all the config locations for wcfopen.
    
    [tfs-changeset: 1558647]

[33mcommit 21afa19e3aa7bad6ce6830745e7cf8d097b81f16[m
Author: dotnet-bot <dotnet-bot@microsoft.com>
Date:   Thu Dec 17 14:00:37 2015 -0800

    [WesH/EricStJ] Fix a few more packaging build breaks from recent changes.
    
    [tfs-changeset: 1558643]

[33mcommit 8ec79d5da0f3c167773f0655d55437daf017e50a[m
Merge: 670492d f65b982
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Fri Dec 18 04:14:15 2015 +0800

    Merge pull request #648 from dotnet-bot/from-tfs
    
    Merge changes from TFS

[33mcommit f65b9820f2c114019c049607e4ca925697b493c8[m
Author: dotnet-bot <dotnet-bot@microsoft.com>
Date:   Thu Dec 17 11:27:05 2015 -0800

    [WesH] Fix build break caused by buildtools update and version update in wcfopen
    
    - This fixes the issue with System.ServiceModel.Http version change from 4.0.11.0 to 4.1.0.0
    - Updates wcfopen to BuildTools version 134 and make necesary adjustments in dir.props to respond to it. This is needed because of a breaking change in our PrereleaseResolveNugetAssets task.
    
    [tfs-changeset: 1558599]

[33mcommit 670492de855b634b0269348705c7e7e8b138ef52[m
Merge: d791b0a 42d1ba8
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Fri Dec 18 01:51:22 2015 +0800

    Merge pull request #647 from iamjasonp/update-http-csproj
    
    Update System.ServiceModel.Http.csproj to build v4.1.0.0

[33mcommit 42d1ba82aa7a91f130354ab97f2e6ade132a9211[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Fri Dec 18 01:36:52 2015 +0800

    Update System.ServiceModel.Http.csproj to build v4.1.0.0

[33mcommit d791b0a8a54eb31b97a6d76778303e00fb495543[m
Merge: 135599f 3c5328f
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Fri Dec 18 00:28:53 2015 +0800

    Merge pull request #629 from mellinoe/facade-port
    
    Update build tools, dir.props, and add facade projects

[33mcommit 135599fc05916f710f1f641fcbbf51b793c03e47[m
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Thu Dec 17 16:15:47 2015 +0800

    Update cross-platform-testing.md
    
    Small change in command

[33mcommit 6ec24fe454a6cd56efce1f02d3d9c61d2b4f66c4[m
Merge: a068aa0 0b4d4b1
Author: Shin Mao <shmao@microsoft.com>
Date:   Wed Dec 16 13:25:37 2015 -0800

    Merge pull request #633 from shmao/35
    
    Add MessageHeaderAttribute into the Contract.

[33mcommit a068aa08517d82c4324017ad22290622a1b5e07e[m
Merge: 26d9242 a6a7ed2
Author: Matt Connew <mconnew@microsoft.com>
Date:   Wed Dec 16 13:17:49 2015 -0800

    Merge pull request #634 from mconnew/AddHttpsToContract
    
    Add api surface area needed for Https

[33mcommit a6a7ed24499e227e748ee85e2d4914a4bed38c4d[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Mon Dec 14 18:01:23 2015 -0800

    Add api surface area needed for Https

[33mcommit 2e24d02cdb3a53ca5a617c8bc4abbee1c829cc2d[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Wed Dec 16 18:38:19 2015 +0800

    Add ability to specify certificate FriendlyName, SubjectAltNames to Bridge
    
    * Allows bridge to specify the FriendlyName of the certificate for identification purposes
    * Changes created certs so that endpoints creating certs now also set the FriendlyName
    * Allow Subject Alternative Names to be specified separate from the Subject
      for testing purposes - e.g., when we only want to specify the CN and not the SAN

[33mcommit 0b4d4b1f6b02b2192fb0f8eb3dd2cfa3300f109e[m
Author: Shin Mao <shmao@microsoft.com>
Date:   Tue Dec 15 17:29:32 2015 -0800

    Removed MessageHeaderAttribute.Relay and MessageHeaderAttribute.Actor from the contract.

[33mcommit 26d924259d7103d28c4161aa98a4ff31961a4024[m
Merge: d5b6b0b 11e6908
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Dec 15 16:31:03 2015 -0800

    Merge pull request #636 from StephenBonikowsky/Issue127
    
    Adding test coverage for XmlDictionaryReaderQuotas.CopyTo method

[33mcommit 11e6908e9b7f5b37b1fe6bf297fa2c1c8667bae9[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Dec 15 15:44:28 2015 -0800

    Adding test coverage for XmlDictionaryReaderQuotas.CopyTo method
    
    * Updated existing to get this coverage.
    * Fixes #127

[33mcommit 82699b9c4deede875178db760eca641ec78b7c33[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Tue Dec 15 12:47:02 2015 +0800

    Remove OID 2.5.29.7 from OIDs that we look for in the cert for SANs

[33mcommit c338e16da3c1de76ad74fad6eb142ff4559f37c7[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Mon Dec 14 16:52:10 2015 +0800

    Claims should only made for dNSNames in the SAN
    
    Previously, we blindly added all items contained in the SubjectAlternativeName,
    even though the SAN field could contain entries like iPAddress, eMail, URI, and
    we do not want to make claims for entries other than dNSName

[33mcommit d5b6b0b5fca2945429a28e4aeebd5cd9f05130b6[m
Merge: ada36ab f691743
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Dec 14 13:33:01 2015 -0800

    Merge pull request #630 from StephenBonikowsky/Issue595
    
    Add sync variations and update existing tests to use correctly attrib…

[33mcommit 0bf191d080370b26f6b02e628524e9b47dcea77c[m
Author: Shin Mao <shmao@microsoft.com>
Date:   Mon Dec 14 12:02:29 2015 -0800

    Add MessageHeaderAttribute into the Contract.
    
    Fix #631.

[33mcommit f6917433248f76ba048abd9b01d30ef266898e19[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Dec 11 11:17:24 2015 -0800

    Add sync variations and update existing tests to use correctly attributed type.
    
    * The existing tests were using a type attributed with 'DataContract' this would allow the Net Native toolchain
          to know what serializer it needed. Updated the tests to use a non-attributed type so the Net Native toolchain
          is properly tested.
    * Added synchronous variations of this scenario.
    * Fixes #586
    * Fixes #595

[33mcommit 3c5328f17185732ea313dbe505b42e82a0380eb1[m
Author: Eric Mellino <erme@microsoft.com>
Date:   Fri Dec 11 11:42:35 2015 -0800

    Re-lock all project.lock.json files

[33mcommit c27cd216b45f7b1c117fb4734be7e01dddd40721[m
Author: Eric Mellino <erme@microsoft.com>
Date:   Thu Dec 10 15:26:31 2015 -0800

    Update build tools, dir.props, and add facade projects
    
    This adds facade projects for all of the contract assemblies. These have
    build configurations for all of our supported platforms: CoreCLR, .NET
    Native, NetCoreForCoreCLR, and .NET Framework 4.6.

[33mcommit ada36ab6733a2dbeaaf738773d810cf88d6383f8[m
Merge: 287ddd7 10a69b1
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Dec 11 10:58:54 2015 -0800

    Merge pull request #627 from roncain/disable-principal-windows-tests-linux
    
    Disable Linux tests using System.Security.Principal.Windows

[33mcommit 287ddd732fd002c7b5dc26a5fdc28594484d34ad[m
Merge: d48a3d3 1bf117e
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Dec 11 10:58:41 2015 -0800

    Merge pull request #626 from roncain/disable-websocket-tests-linux
    
    Disable WebSockets tests on Linux

[33mcommit 10a69b1043dc0d703d5760694b03bbef433fd2a9[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Dec 11 07:08:34 2015 -0800

    Disable Linux tests using System.Security.Principal.Windows
    
    These tests invoke code that causes failures in OuterLoop due to
    the fact System.Security.Principal.Windows is not supported on Linux
    but we still depend on it.  This causes the Windows assembly to be
    used instead.
    
    When #604 is resolved to remove this dependency we will need to
    fix these tests or make them conditional on OS.

[33mcommit 1bf117e6391d6fe5e0748a2f1f6c2f353024419a[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Dec 11 06:23:33 2015 -0800

    Disable WebSockets tests on Linux
    
    Adds [ActiveIssue(625, PlatformID.AnyUnix)] to WebSockets tests
    because they cannot pass until support has been added.
    
    Support of WebSockets on Linux in CoreFx is tracked by issue
    https://github.com/dotnet/corefx/issues/2486

[33mcommit d48a3d34b9263c048c609875307c2d242aafa8a1[m
Merge: 9d539a6 781d2a2
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Dec 11 04:41:59 2015 -0800

    Merge pull request #622 from roncain/update-xplat-doc
    
    Update instructions for running tests on Linux.

[33mcommit 9d539a6424b931c1edf4951d60c4f7b719a62220[m
Merge: 7fcdb47 40193ab
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Fri Dec 11 13:38:07 2015 +0800

    Merge pull request #602 from iamjasonp/fix-pragma
    
    Change MessageContent to internal class to fix CLS compliance

[33mcommit 7fcdb478d2e8f67b33fe71f74fbceae3cd8cdb83[m
Merge: 540cd05 742dd49
Author: Shin Mao <shmao@microsoft.com>
Date:   Thu Dec 10 14:51:32 2015 -0800

    Merge pull request #588 from shmao/557
    
    Added scenario tests for MessageContractAttribute and ServiceKnownTypeAttribute.

[33mcommit 742dd498ddf5cb4b1fda1c7e78cd0e6a99fdc47f[m
Author: Shin Mao <shmao@microsoft.com>
Date:   Thu Dec 10 13:36:37 2015 -0800

    Added a Scenario Test For XmlSerializerFormat & MessageContract.
    
    Fix #557.
    
    Add Scenario Tests for ServiceKnownTypeAttribute.
    
    Fix #569.
    
    Added a scenarios test for XmlSerializerFormat.
    
    The test is for the case where a paramerter type contains a field never used. The test is to make sure the reflection info of the type of the unused field would be kept by Net Native toolchain.

[33mcommit 781d2a2ac31aa3780eba681c3b4d4a90924c2dc5[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Dec 10 11:45:21 2015 -0800

    Update instructions for running tests on Linux.

[33mcommit 540cd054b6790224509b0ba6efe8ac3de2b6317e[m
Merge: 3e76a61 f7d7278
Author: Hong Dai <hongdai@microsoft.com>
Date:   Thu Dec 10 11:14:11 2015 -0800

    Merge pull request #570 from hongdai/releasenote
    
    Update RC1 release notes

[33mcommit 3e76a61b24e244ab0f4ae15abf3290730da44fd9[m
Merge: 80b3a90 74f851c
Author: Matt Mitchell <mmitche@microsoft.com>
Date:   Thu Dec 10 10:05:35 2015 -0800

    Merge pull request #611 from mmitche/use-unpacker
    
    Use the packer/unpacker to reduce copy time for tests

[33mcommit 74f851cd5360c36af20bd167cad80c63532da520[m
Author: Matt Mitchell <mmitche@microsoft.com>
Date:   Thu Dec 10 09:56:50 2015 -0800

    Use the packer/unpacker to reduce copy time for tests

[33mcommit 80b3a90a0fa4b9956a607ffeef8e18cba3d1c975[m
Merge: b1488af b4b81d3
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Dec 10 07:27:48 2015 -0800

    Merge pull request #600 from roncain/update-version-23608
    
    Update version 23608

[33mcommit b4b81d37773be54f6ef198222b2226e974342394[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Dec 10 06:27:52 2015 -0800

    Update System.Net.Security wildcard dependencies to 23608

[33mcommit a1c86aff12635d20f6e96e9860af98498e517afd[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Dec 10 19:07:34 2015 +0800

    Allow certificate claim checking to fall back to using CN
    
    We currently check only the Subject Alternative Name when looking at certificates
    In order to match RFC 2818, in the event there are no DNS values in the SAN field
    (or no SAN extension at all), fall back to using CN

[33mcommit 40193abae33450bfa9203ccec89d6004e740d52c[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Dec 10 18:03:20 2015 +0800

    Remove unnecessary pragma warning disable from ProducerConsumerStream
    
    Fixes #117

[33mcommit 784fd82e86e0c6489ce462c8ca35424b7e1742ea[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Dec 10 17:23:23 2015 +0800

    Change MessageContent to internal class to fix CLS compliance

[33mcommit f7d72783a36a81224be4038e3f8407a73d211510[m
Author: Hong Dai <hongdai@microsoft.com>
Date:   Wed Dec 9 18:54:31 2015 -0800

    Update RC1 release notes
    * The scenario table are updated according to test results on Asp.Net 5 and UWP.

[33mcommit 8c4ed4e97fa82e1c55b8e87276fb904fd839cab8[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Dec 9 08:29:36 2015 -0800

    Regenerate project.lock.json files
    
    Build.cmd regenerated all these except for the one
    in src\windows.  This was regenerated by deleting the prior
    lock.json file and rebuilding System.Private.ServiceModel\src.

[33mcommit a8607a92090ba086941f7a21cf518c96f18ec8a2[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Dec 9 08:08:08 2015 -0800

    Update project.json dependencies to rc2-23608
    
    This is necessary to allow Linux OuterLoops to run again.

[33mcommit b1488af35916f37e178cc8965b4f79cc870572de[m
Merge: 8c510a6 592de42
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Dec 9 14:09:57 2015 -0800

    Merge pull request #599 from roncain/remove-ssl3
    
    Remove SSL3 from default protocols supported

[33mcommit 592de42d67113f28338cc3e6fc76b2d2217d9ceb[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Dec 9 04:56:26 2015 -0800

    Remove SSL3 from default protocols supported
    
    Starting with PR https://github.com/dotnet/corefx/pull/4483
    System.Net.Security does not support SSL3 in CoreFx.
    
    Therefore we remove it from WCF's list of transport defaults.
    
    Fixes #578, #579, #580, #581, #582, #583, #584, #585

[33mcommit 8c510a6b21739624107380c293a6a4f583223b21[m
Merge: 8398eaa 2111346
Author: Matt Connew <mconnew@microsoft.com>
Date:   Wed Dec 9 13:52:34 2015 -0800

    Merge pull request #598 from mconnew/Issue589
    
    Receiving chunked response when buffered is now async.

[33mcommit 2111346ea147d104dfa06de96fd1523b21072692[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Tue Dec 8 22:26:44 2015 -0800

    Receiving chunked response when buffered is now async.
    Also improved async-ness of streamed response.

[33mcommit 8398eaa58ec1b2d619e447425ced6e8faa2c1487[m
Merge: 8ac928c db8e672
Author: Matt Connew <mconnew@microsoft.com>
Date:   Tue Dec 8 22:44:57 2015 -0800

    Merge pull request #573 from mconnew/BufferedHttpStream
    
    Adding buffer to Http streamed mode transfer.

[33mcommit db8e67269a78b63b09d09aaa5c70c94d71514806[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Wed Dec 2 21:19:28 2015 -0800

    Adding buffer to Http streamed mode transfer.

[33mcommit 8ac928cf22f122527ba3b488f29936fade896e70[m
Merge: 23ccb48 23e1c2f
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Wed Dec 9 10:23:41 2015 +0800

    Merge pull request #591 from iamjasonp/enable-test-301
    
    Enable CreateChannel_Using_NetTcpBinding_Defaults test

[33mcommit 23ccb48d138812d682a1c12b91296d89b17a1e36[m
Merge: 25ecad2 6d057ae
Author: Matt Connew <mconnew@microsoft.com>
Date:   Tue Dec 8 14:41:43 2015 -0800

    Merge pull request #596 from mconnew/FixHttpsOnUnix
    
    Only set server certificate validation callback if server certificate provided

[33mcommit 6d057ae3b7576d2eae8f679a6ac3b53fa65f1b5d[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Tue Dec 8 12:26:14 2015 -0800

    Only set server certificate validation callback if server certificate provided

[33mcommit 25ecad2fa533c88f5eee10f706140a3c0fae56fa[m
Merge: a0563d6 a77557a
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Dec 8 10:13:17 2015 -0800

    Merge pull request #593 from StephenBonikowsky/Issue273
    
    Underlying issue fixed in PR #3000 in dotnet/corefx.

[33mcommit a0563d671f91922f9200c77b4cdae78e6074c507[m
Merge: fb402cc fe02883
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Dec 8 10:01:41 2015 -0800

    Merge pull request #590 from StephenBonikowsky/UpdatingServiceContractTestsSyntax
    
    Updating existing tests with updated syntax and validation checking.

[33mcommit a77557a36c4a143d7cee1a47433e6fdb9dbb12cb[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Dec 8 09:47:35 2015 -0800

    Underlying issue fixed in PR #3000 in dotnet/corefx.
    
    * Removing the active issue, need to keep our eye on it to make sure it is no longer flaky.
    * Fixes #273

[33mcommit fe0288383cd200f41068f2e49397b1b1f72e4ae8[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Dec 7 16:42:53 2015 -0800

    Updating existing tests with updated syntax and validation checking.
    
    * Using Asserts instead of StringBuilder for validation.
    * Adding comments to compartmentalize the test into separate sections.

[33mcommit 23e1c2f89e26c9265f12a6c9f083bd4c008a104c[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Tue Dec 8 17:34:23 2015 +0800

    Add more testing for BasicHttpSecurityMode modes

[33mcommit 288aac1774d5c0c501a01fc281c1c2bfe0d33a93[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Tue Dec 8 15:34:48 2015 +0800

    Modify ActiveIssue in test to reflect correct Issue number

[33mcommit 48bbce728dd933613badb0e841fb9b62214d61f6[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Tue Dec 8 11:49:53 2015 +0800

    Enable tests previously disabled due to NegotiateStream being missing
    
    Fixes #301

[33mcommit fb402cc19f435233173adef7155f6b1d320f2f09[m
Merge: 4970481 0b06ec2
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Tue Dec 8 12:22:14 2015 +0800

    Merge pull request #577 from iamjasonp/securitybindingelement-test
    
    Verify that SecurityBindingElement.IsSetKeyDerivation no longer throws

[33mcommit 0b06ec2a542d67ea0bb817a1ec6e17a2cbfbc8ff[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Tue Dec 8 10:40:54 2015 +0800

    Add tests to verify that SecurityBindingElement.IsSetKeyDerivation doesn't throw
    
    Resolves #561

[33mcommit 728af6283f58998dabc998bd3d9c19b5f7069940[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Mon Dec 7 18:42:27 2015 +0800

    Implement TransportSecurityBindingElement.GetProperty stub
    
    * Implement TransportSecurityBindingElement.GetProperty stub which
      does not throw unconditionally, only when GetProperty<ChannelProtectionRequirements>
      is called
    * Implement SecurityBindingElement.CanBuildChannelFactory<TChannel> as next step in
      turning on SecurityBindingElement
    * Uncomment a test that was disabled since GetProperty<T> was disabled

[33mcommit 4970481619c3d33ce0340465562980899518656f[m
Merge: 7356df1 cf24489
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Dec 7 11:07:38 2015 -0800

    Merge pull request #571 from StephenBonikowsky/Issue467
    
    Adding ServiceContract tests for keyword 'out' and 'ref' variations.

[33mcommit cf2448937f7e34a67ca66cfdaf4c86308c7afa0c[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Dec 2 14:22:12 2015 -0800

    Adding ServiceContract tests for keyword 'out' and 'ref' variations.
    
    Fixes #467

[33mcommit 7356df16d69cf134b7adaac747b28ecae3e1cb50[m
Merge: c55ee55 9fa49e3
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Fri Dec 4 14:58:01 2015 +0800

    Merge pull request #422 from iamjasonp/x509certclaimset-san
    
    Validate X509CertificateClaims by SAN instead of by CN

[33mcommit c55ee5556593e53eeafd7db4a6bd21d54226a866[m
Merge: 27928ac cbc61d0
Author: Hong Dai <hongdai@microsoft.com>
Date:   Thu Dec 3 13:51:58 2015 -0800

    Merge pull request #575 from hongdai/removedelay
    
    Remove 2 minutes delay

[33mcommit cbc61d0c3e1d56b0809968ba841afd43a92276d0[m
Author: Hong Dai <hongdai@microsoft.com>
Date:   Thu Dec 3 13:03:22 2015 -0800

    Remove 2 minutes delay
    * It's not necessary now as Jason has made window one cache entry per certificate

[33mcommit 9fa49e3932a295ce1a6e5328e1389167df40bd38[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Dec 3 19:35:08 2015 +0800

    Add tests for X509 verification
    
    Add test for validating X509CertificateClaims by SAN instead of by CN (#422)
    Enable test to ensure revoked certs fail when we try to verify them (Resolves #533)

[33mcommit 27928ac7c38d4e689460de315120d64ea31c809c[m
Merge: 4c878ba b81c413
Author: Hong Dai <hongdai@microsoft.com>
Date:   Wed Dec 2 11:31:38 2015 -0800

    Merge pull request #548 from hongdai/issue527
    
    MessageParameterAttribute for return response not working

[33mcommit 4c878baabfc8b8bc79a5e580e7d211fe839a435c[m
Merge: 4a85361 9e3d0bd
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Thu Dec 3 03:15:08 2015 +0800

    Merge pull request #565 from iamjasonp/update-project-json
    
    Update project.json files to consistently use -rc2-23602 build

[33mcommit 9e3d0bdfa0b8059ad8667578af6a60cc0b735dd1[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Dec 3 02:40:09 2015 +0800

    Update project.json files to consistently use -rc2-23602 build
    
    On advice of the corefx team, we need to move to specific build numbers rather
    than using -release-* builds, so taking this chance to update them all.
    
    Excluded are the ref projects, need to make sure that there are no repercussions
    in doing so.

[33mcommit 4a853611dc21cf6c1f77916f9199d60e586367fd[m
Merge: 672fafc 9df2819
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Wed Dec 2 23:01:49 2015 +0800

    Merge pull request #550 from iamjasonp/negostream
    
    Light up code for supporting Windows stream security

[33mcommit 04439650c0acfc670696282f1163dbf0af5cefd0[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Wed Oct 14 16:05:39 2015 +0800

    Validate X509CertificateClaims by SAN instead of by CN
    
    X509CertificateClaimSet should validate based on Subject Alternative Name
    instead of the Canonical Name to be consistent with Desktop .NET >= 4.6
    
    Today, We are checking against the Subject (e.g. CN=host) on the certificate,
    to determine whether or not the cert was issued for a particular server, as this
    was the behaviour in <= 4.5. However, this is inflexible as the host could be
    serving the same cert but bound to multiple endpoints.
    
    In the old case, for example, foo-bar.test.com != foo-bar, and a generated cert
    can only be valid for exactly one of these. In the new case, a certificate can
    have a Subject with a particular CN, but multiple Subject Alt Names for which the
    cert is also valid.
    
    In the wild, most certs issued already contain SANs for the names that the
    server can be, so we should be consistent with current behaviour and ditch the
    legacy way of checking certs against servers.
    
    Fixes #321

[33mcommit 672fafcef6c26a890b88de699881e2cd0425532d[m
Merge: df2085d 72ed478
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Wed Dec 2 18:00:50 2015 +0800

    Merge pull request #545 from iamjasonp/messagesecurity-pnse
    
    Implement SecurityBindingElement.IsSetKeyDerivation

[33mcommit 9df2819d09cfcc84f63bbe455322e249a8c6f006[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Wed Dec 2 17:57:17 2015 +0800

    Respond to PR comments

[33mcommit 2eb7240ab58c8a3d0457999131dcdb18297072a3[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Tue Dec 1 19:13:59 2015 +0800

    Disable NegotiateStream/WindowsStreamSecurity for .NET Native

[33mcommit b81c4136464cda5a05da9c13d36651d226f8cce3[m
Author: Hong Dai <hongdai@microsoft.com>
Date:   Tue Dec 1 21:19:29 2015 -0800

    MessageParameterAttribute for return response not working
    To enable it
    * add back MessageParameterAttribute in CreateParameterPartDescription
    * use methodInfo.ReturnParameter instead of methodInfo.ReturnType.
    In the full framework, we use ReturnTypeCustomAttributes, it's essentially
    ReturnParameter.
    * Add a scenario test.
    Fixes issue 527

[33mcommit df2085db955d84de34f782b2dd51a72a4d040b14[m
Merge: 85d1107 61bbf34
Author: Shin Mao <shmao@microsoft.com>
Date:   Tue Dec 1 09:17:48 2015 -0800

    Merge pull request #553 from shmao/551
    
    Fix DisablePayloadMasking_Property_Get_PNSE_Throws.

[33mcommit 4aca495e0ee0c888c4f76379cec14ba05fa84cba[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Mon Nov 30 16:47:37 2015 +0800

    Light up code for supporting Windows stream security
    
    The code still will not fully work yet as we are dependent on
    System.Net.Security.NegotiateStream being lit up. Once that is lit up we should
    be able to get to the inevitable other problems that will surface.
    
    Resolves #413

[33mcommit 85d1107be57cfd6e961abcf0f1b62582b8b90106[m
Merge: a31cdf8 e3df379
Author: Matt Connew <mconnew@microsoft.com>
Date:   Mon Nov 30 21:15:53 2015 -0800

    Merge pull request #554 from mconnew/DigestTests
    
    Adding digest tests without domain controller

[33mcommit e3df379bce77ea377a29541fb3f193065cdfcfa6[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Mon Nov 30 11:47:26 2015 -0800

    Adding digest tests without domain controller

[33mcommit a31cdf841b3ca970d262902aad9dbc0e87e82a5d[m
Merge: b2f2e49 22a7179
Author: Shin Mao <shmao@microsoft.com>
Date:   Mon Nov 30 10:36:17 2015 -0800

    Merge pull request #547 from shmao/issue535
    
    Added a scenario test for IClientMessageFormatter.

[33mcommit 61bbf348fb11505e20e48d41ec762d76fe1c4961[m
Author: Shin Mao <shmao@microsoft.com>
Date:   Mon Nov 30 10:25:18 2015 -0800

    Fix DisablePayloadMasking_Property_Get_PNSE_Throws.
    
    The above test didn't build in TOF (because the Assert types in ToF do not support Func.) Changed the func to an action.
    
    Fix #551

[33mcommit 3a90d626e232ebd1d17fc27460a66ff3fc963e93[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Fri Nov 27 18:46:51 2015 +0800

    Update test-runtime to 1.0.1-rc2-23525

[33mcommit b2f2e49aade3ce1f4435b749dac780d78ce49092[m
Merge: c3f86f7 59c6b80
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Thu Nov 26 11:14:45 2015 +0800

    Merge pull request #546 from shmao/issue537
    
    Make WebSocketSettings.DisablePayloadMasking throw PNSE.

[33mcommit 22a71792d1a24a3c5f1b44b62910436666ff8438[m
Author: Shin Mao <shmao@microsoft.com>
Date:   Wed Nov 25 11:55:28 2015 -0800

    Added a scenario test for IClientMessageFormatter.
    
    Added a scenario test that demonstrates the IClientMessageFormatter extensibility point works properly.
    
    Fix #535.

[33mcommit 59c6b80cbebfea1d1822e24b31627f2ea25969be[m
Author: Shin Mao <shmao@microsoft.com>
Date:   Wed Nov 25 09:29:01 2015 -0800

    Removed WebSocketDefaults.DisablePayloadMasking.

[33mcommit 8278e8e185b43fd2f1304b78916530fedf286110[m
Author: Shin Mao <shmao@microsoft.com>
Date:   Tue Nov 24 16:02:23 2015 -0800

    Make WebSocketSettings.DisablePayloadMasking throw PNSE.
    
    Fix #537.

[33mcommit c3f86f77e33473a34c846dc510eba71ec4973cb8[m
Merge: 77e5c48 4397170
Author: Matt Connew <mconnew@microsoft.com>
Date:   Tue Nov 24 11:23:21 2015 -0800

    Merge pull request #542 from mconnew/HttpCertificates
    
    Enable client certs for HTTPS plus server certificate validation

[33mcommit 43971707db4d53226ced6a10865bbfa3bac093dd[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Fri Nov 13 21:26:59 2015 -0800

    Enable client certs for HTTPS plus server certificate validation

[33mcommit 72ed478eb18d39072c737a037d804e24df5c8cd4[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Tue Nov 24 17:31:35 2015 +0800

    Implement SecurityBindingElement.IsSetKeyDerivation
    
    Now that we have started implementing the tokens/token parameters,
    we can now start implementing SecurityBindingElement.IsSetKeyDerivation instead
    of throwing `PlatformNotSupportedException`
    
    Fixes #540

[33mcommit 77e5c48385845a8f165978fc44a1e137e509a248[m
Merge: 4243068 f90f55a
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Nov 20 15:22:06 2015 -0800

    Merge pull request #539 from StephenBonikowsky/Issue532
    
    Adding additional Web Socket tests.

[33mcommit f90f55ac6709e24f2c28217839826f871eccacf1[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Nov 18 14:35:46 2015 -0800

    Adding additional Web Socket tests.
    
    * Fixes Issue #532
    * Includes some minor formatting changes and one bug in other WebSocket tests in the same file.

[33mcommit 4243068a37adb9b67d5ed280a00f8caf0fcf4ca1[m
Merge: b4ad34f a92604b
Author: Hong Dai <hongdai@microsoft.com>
Date:   Fri Nov 20 07:50:54 2015 -0800

    Merge pull request #517 from hongdai/issue369
    
    Implement NetTcpBinding Security.Transport Certification authority revokes a certificate

[33mcommit b4ad34f468515c5a3b27877a14736a072ba0dc4a[m
Merge: 4f68609 258599e
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Nov 20 05:16:58 2015 -0800

    Merge pull request #538 from roncain/add-release-notes
    
    Add release notes

[33mcommit 258599e99f9ad433c3f52165ae3229c7f0b75937[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Nov 20 05:05:33 2015 -0800

    Adds release-notes folder and content
    
    The format of the release-notes follows the conventions
    of https://github.com/dotnet/core

[33mcommit a92604b3ba31efe9c721fd9e72f3480518438c84[m
Author: Hong Dai <hongdai@microsoft.com>
Date:   Thu Nov 19 15:56:15 2015 -0800

    Implement NetTcpBinding Security.Transport Certification authority revokes a certificate
    * Add a test to check client call will fail if service cert is revocated.
    
    Fixes #369

[33mcommit a9490c74fa7d81553cb0b5924dc702d81d6dae10[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Nov 19 11:49:54 2015 -0800

    Add per-package release notes.
    
    This follows the convention in https://github.com/dotnet/core to
    maintain an ongoing markdown file per package describing the
    public releases.

[33mcommit 67d7056298e39c4979428a5adad422e49949b880[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Nov 19 11:34:38 2015 -0800

    Add release-notes folder and move supported feature table
    
    Moves the supported feature markdown to the new release-notes
    folder in preparation for adding individual per-package release notes.

[33mcommit 4f68609df99a254c6f27674b526ec1c4ba04af3b[m
Merge: a907f25 6a988bf
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Nov 19 11:29:46 2015 -0800

    Merge pull request #536 from roncain/supported-scenarios
    
    Fill in table of supported RC1 features

[33mcommit 6a988bf7da469df69f6fce256498b10a460a9931[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Nov 19 08:12:54 2015 -0800

    Fill in table of supported RC1 features
    
    First pass at specifying the features available in RC1.
    
    Fixes #493

[33mcommit a907f2538be49bde158e8608797d8b047e6834e9[m
Merge: 5cf5e85 100dafe
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Nov 18 13:07:54 2015 -0800

    Merge pull request #530 from StephenBonikowsky/MarkDown
    
    Supported scenarios table.

[33mcommit 100dafe8b5911994e12c4e14b3adc9ac7b0b4dc8[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Nov 16 15:15:33 2015 -0800

    Supported scenarios table.
    
    *First draft, handing off to Ron to flech out scenarios.
    *Using emojis insted of icons.

[33mcommit 5cf5e858fb7728a6db24f2f16993bcc44db76851[m
Merge: c7baaaf 0fa1bf9
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Nov 17 06:30:29 2015 -0800

    Merge pull request #531 from dotnet-bot/from-tfs
    
    Merge changes from TFS

[33mcommit c7baaaf0c79f1ea9c7b579e47d84e683120580cc[m
Merge: c15a61b 19d8ecd
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Tue Nov 17 03:13:47 2015 -0800

    Merge pull request #528 from dotnet-bot/from-tfs
    
    Merge changes from TFS

[33mcommit 0fa1bf9eadfd8a1286479ca0f0304caf3fa1a1ff[m
Author: Mariana Rios Flores <mariari@microsoft.com>
Date:   Mon Nov 16 16:35:26 2015 -0800

    [tfs-changeset: 1548846]

[33mcommit e6a8a000df25ed766eb9b73789dfbef6762dd8e4[m
Author: Mariana Rios Flores <mariari@microsoft.com>
Date:   Mon Nov 16 16:22:23 2015 -0800

    [tfs-changeset: 1548843]

[33mcommit 19d8ecddc13a7a48381969d8e587980578b7a575[m
Author: dotnet-bot <dotnet-bot@microsoft.com>
Date:   Mon Nov 16 12:19:47 2015 -0800

    Fixed the build break caused by https://github.com/dotnet/wcf/pull/509.
    
    [tfs-changeset: 1548785]

[33mcommit c15a61b136a195f57f07b658a8efc2ef950da2c3[m
Merge: 1e644df cb2c6ee
Author: Shin Mao <shmao@microsoft.com>
Date:   Fri Nov 13 15:02:55 2015 -0800

    Merge pull request #509 from shmao/issue303
    
     Light up async paths needed for Ssl StreamSecurity

[33mcommit 1e644df4a6388d7bb1d91bb316089134ad3aa6dd[m
Merge: 34b25e3 d4c3f32
Author: Matt Mitchell <mmitche@microsoft.com>
Date:   Fri Nov 13 10:24:30 2015 -0800

    Merge pull request #519 from roncain/unlock-coreclr-version
    
    Revert temporary fix to lock CoreCLR version

[33mcommit d4c3f32bb1d433871e2b9f754507ff064dafe2b6[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Nov 13 09:35:00 2015 -0800

    Revert temporary fix to lock CoreCLR version
    
    This reverts the temporary fix in 367c3c8218d5571e94dbb531e28e584f4b835e07.
    This was necessary to unblock Linux testing.
    
    Now that the WCF repo carries the correct runtime-tools, it is
    appropriate to use the latest successful builds, not a specific version.

[33mcommit cb2c6ee66643a83062fce6e959a6251172bd7dfa[m
Author: Shin Mao <shmao@microsoft.com>
Date:   Thu Nov 12 16:10:57 2015 -0800

    Light up async paths needed for Ssl StreamSecurity
    
    Uses async paths for doing the security upgrade and sending the preamble.
    
    Fixes issue 303.
    Fixes issue 304.

[33mcommit 34b25e3bc95143f0231d5140da93f5bdbe900c1d[m
Merge: 7c37340 ce8b4a2
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Nov 11 14:06:52 2015 -0800

    Merge pull request #514 from roncain/update-dependencies
    
    Update dependencies

[33mcommit 7c37340c135f1aa477e184a87fbd90ffdbef07d6[m
Merge: b0b2d80 93c6963
Author: Matt Connew <mconnew@microsoft.com>
Date:   Wed Nov 11 11:38:35 2015 -0800

    Merge pull request #516 from mconnew/Issue470
    
    Remove accidental addition of S.N.WebSockets namespace

[33mcommit ce8b4a256b1fef80db862353900e19ddf56e435a[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Nov 11 07:09:20 2015 -0800

    Update the generated project.lock.json files
    
    All these files were generated automatically doing
    a normal build.cmd.  They are kept in a separate commit
    for review simplicity.

[33mcommit f20be92e5f9d169b2543d07ccbf1eae3251478a5[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Nov 11 06:47:47 2015 -0800

    Update build-tools and runtime-tools dependencies
    
    This commit updates build-tools and runtime-tools
    version dependencies to match CoreFx.  It does not
    include the auto-generated project.lock.json files.

[33mcommit 93c6963c4d97d441c9580bec9d7b6c980588ed2c[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Tue Nov 10 14:25:18 2015 -0800

    Remove accidental addition of S.N.WebSockets namespace

[33mcommit b0b2d80f6948502839704962bdbb4698d0fce0ef[m
Merge: 5b4377e 9a362b1
Author: Matt Connew <mconnew@microsoft.com>
Date:   Tue Nov 10 13:27:32 2015 -0800

    Merge pull request #515 from mconnew/Issue513
    
    Stability improvments to WSDuplexService and Bridge
    Fixes #513

[33mcommit 9a362b1852dd15315bd9e4ad859a19a77a50ab20[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Tue Nov 10 12:57:25 2015 -0800

    Add global exception handler which should prevent process shutdown with unhandled exceptions

[33mcommit 2596b84dd3ef1835572d7fb50eb502dbf5473f0f[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Tue Nov 10 11:55:33 2015 -0800

    Changed usage of QueueUserWorkItem to TaskFactory.StartNew to avoid callback exceptions from crashing entire process

[33mcommit 5b4377ec6d4432e6672a2fdee9868e918df3b647[m
Merge: ad816ae 7551567
Author: Matt Connew <mconnew@microsoft.com>
Date:   Tue Nov 10 12:02:11 2015 -0800

    Merge pull request #512 from mconnew/Issue470
    
    Fixed secure websockets setup and disable failing tests

[33mcommit ad816ae2f7317f9a79ac4e87626aaaf7370f2530[m
Merge: 9fb0356 5413b54
Author: Peter Hsu <shhsu@microsoft.com>
Date:   Tue Nov 10 08:35:34 2015 -0800

    Merge pull request #511 from shhsu/master
    
    resubmit: Allow caller to await on TimeoutHelper.CancelationToken
    
    ref: https://github.com/dotnet/wcf/pull/427

[33mcommit 75515678ab53fba57b50ed9b21e8e5d0e3c71821[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Mon Nov 9 19:49:47 2015 -0800

    Adding ActiveIssue's to failing WebSocket tests

[33mcommit 5413b54f0632fad167ec929581115f6da018fddc[m
Author: Peter Hsu <shhsu@microsoft.com>
Date:   Mon Nov 9 17:34:51 2015 -0800

    resubmit: Allow caller to await on TimeoutHelper.CancelationToken

[33mcommit a5332aad0420534c5a9037618970f3c250f0b660[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Mon Nov 9 16:12:34 2015 -0800

    Fix secure WebSocket test setup

[33mcommit 9fb03564762ab14a45ad4cf300b06029877c9722[m
Merge: f2f6ca8 367c3c8
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Nov 5 12:36:20 2015 -0800

    Merge pull request #505 from ellismg/lock-coreclr-version
    
    Lock CoreCLR version

[33mcommit 367c3c8218d5571e94dbb531e28e584f4b835e07[m
Author: Matt Ellis <matell@microsoft.com>
Date:   Thu Nov 5 10:22:23 2015 -0800

    Lock CoreCLR version
    
    There are some incompatibilites between CoreFX and CoreCLR, we need to
    lock to a specific build of CoreCLR for testing for a bit of time.

[33mcommit f2f6ca8b7e3dc55c374300d149f9d4f405559018[m
Merge: e672b98 b94a173
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Nov 5 04:50:02 2015 -0800

    Merge pull request #503 from roncain/linux-no-corefx-bin-copy
    
    No longer copy CoreFx binaries when running Linux tests

[33mcommit e672b987a72ae3941d97c0ca89eedeb247ef0df9[m
Merge: 0fdd162 e2cdc43
Author: Matt Connew <mconnew@microsoft.com>
Date:   Wed Nov 4 22:19:07 2015 -0800

    Merge pull request #466 from mconnew/NetworkingFixes
    
    Use latest networking fixes

[33mcommit 0fdd16272ba04e6c76d85adc4cef479ebe3054c1[m
Merge: 3278c1d 86b2800
Author: Matt Connew <mconnew@microsoft.com>
Date:   Wed Nov 4 12:45:42 2015 -0800

    Merge pull request #472 from mconnew/Issue468
    
    Fix some WebSocket issues
    
    Fixes #468

[33mcommit 86b28000950dd55fd529fa08acba882c6cd5eb2a[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Wed Nov 4 12:01:31 2015 -0800

    Updating ActiveIssue attributes for WebSockets tests

[33mcommit 3278c1dcccfa1a1ff722bcd268f7d50fa4b0d6c4[m
Merge: 4a9651c ff96728
Author: Matt Mitchell <mmitche@microsoft.com>
Date:   Wed Nov 4 10:24:37 2015 -0800

    Merge pull request #504 from mmitche/pass-upstream-params
    
    Pass build input parameters to downstream builds

[33mcommit ff96728ff21cc2e57e4b1b3b4c6b7afae91463b6[m
Author: Matt Mitchell <mmitche@microsoft.com>
Date:   Wed Nov 4 10:14:16 2015 -0800

    Pass build input parameters to downstream builds

[33mcommit 4a9651c3023976081ca3ccd623cf07e249d8d753[m
Merge: c8bb928 cffef65
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Thu Nov 5 00:02:30 2015 +0800

    Merge pull request #502 from iamjasonp/fix-bridgeclient-tests
    
    Mark client certs installed by BridgeClient as PersistKeySet

[33mcommit b94a173d3673bde10455fa3d6a11728aaf5ea8bd[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Nov 4 07:55:26 2015 -0800

    No longer copy CoreFx binaries when running Linux tests
    
    Prior to this change, it was necessary to copy some binaries
    from the CoreFx repo built on Linux when running tests.
    This was necessary because the packages had incorrect binaries.
    
    But that situation was resolved in preparation for RC1, and
    it is no longer necessary to copy any binaries from CoreFx.
    
    Fixes #442

[33mcommit c8bb928411376248e25216811f2df404f32bb95c[m
Merge: 843fb26 2e52991
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Nov 4 05:49:59 2015 -0800

    Merge pull request #501 from roncain/linux-cert-cleanup
    
    Invoke CertificateCleanup.exe on Linux after all tests run

[33mcommit 843fb26c47511ca542625d49fd1794a3dcf09e06[m
Merge: 5e1a0f5 8d5ae1d
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Nov 4 05:48:58 2015 -0800

    Merge pull request #499 from roncain/invoke-cert-cleanup
    
    Invoke cert cleanup

[33mcommit cffef65162c17f5bdef38fee21b05a3632d1e3ce[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Wed Nov 4 19:04:49 2015 +0800

    Modify ClientCredentialTypeTests to use CurrentUser store
    
    TcpClientCredentialType_Certificate_CustomValidator_EchoString was using
    certificates from the LocalMachine store. Modify this to use the CurrentUser
    store as client certificates are installed there now

[33mcommit fcc2f84ddb89faa8a845fec2bda502d7978c5280[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Wed Nov 4 19:00:17 2015 +0800

    Mark client certs installed by BridgeClient to PersistKeySet
    
    Mark all installed certificates as PersistKeySet; this seems to be a difference when
    importing to the CurrentUser certificate store vs. the LocalMachine store in
    Windows. This caused issues when attemtping to use the certificates in tests and
    sems to avert the issue of the tcp tests running into the following excception:
    
    ```
    System.ComponentModel.Win32Exception :
    The credentials supplied to the package were not recognized
    ```
    
    Fixes #495

[33mcommit e2cdc43a2ba7f60dbda9376e0a97455a90b9a2b7[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Tue Nov 3 16:55:10 2015 -0800

    Switching to socket.ConnectAsync as now available in contract
    
    Fixes #437

[33mcommit f0979e0521e2ddca3fc7728591b4696282a328ba[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Tue Nov 3 16:07:41 2015 -0800

    Added new port to bridge configuration for secure websockets
    
    The secure (https/wss) test variants for websockets were using
    the same port number as the non-secure (http/ws) test variants.
    This was causing a conflict as http and https can't coexist on
    the same port.

[33mcommit 8479a4d6934b271ba23857f3d114b9c915d4b452[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Wed Oct 28 17:49:54 2015 -0700

    Fix minor issues with max timeouts and websockets, fix websockets streaming.

[33mcommit 8d5ae1d0d5d79a88857d59eee7541749cd4da773[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Nov 3 13:47:54 2015 -0800

    Invoke CertificateCleanup.exe on Windows after OuterLoop runs
    
    This change invokes the existing CertificateCleanup.exe logic
    after OuterLoop tests finish.
    
    It also adds the CertificateCleanup.csproj to the list of projects
    to be build after automatically restoring the build tools.

[33mcommit 2e5299123026347d5d66deb11040fb6b762af580[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Nov 3 13:23:45 2015 -0800

    Invoke CertificateCleanup.exe on Linux after all tests run
    
    This update the Linux script to run tests so that it invokes
    the certificate cleanup logic after all tests are run.

[33mcommit 789d193e3092affaefc2e126cc1d9dc0160c2f20[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Mon Oct 26 17:13:11 2015 -0700

    Fix some WebSocket issues
    1. HttpsChannelFactory was hard coded to throw on duplex contracts
    2. TimeoutStream didn't override ReadByte and WriteByte which cause WebSocketStream.Read/Write to be called
    Fixes #468

[33mcommit 5e1a0f52ee06c9fdde487f8bf4552c3af9e6b020[m
Merge: 2863998 06f531e
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Nov 3 09:54:52 2015 -0800

    Merge pull request #488 from StephenBonikowsky/Issue484
    
    Adding text encoder variations to WebSocket scenario tests.

[33mcommit 06f531ee18462ecc4f739ffdb4cf9d2b22101a33[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Oct 29 15:28:46 2015 -0700

    Adding text encoder variations to WebSocket scenario tests.
    
    * Fixes #484
    * Fixed an error getting the port number for the WebSocketHttpsDuplexBinaryStreamedResource, not caught earlier since all these tests are failing for other reasons.
    * These new tests fail for the same high level reasons as their binary counterparts, but @mconnew in fixing the high level issues has reached a lower level issue where the type of encoding matters, so these tests may help him with his debugging efforts.

[33mcommit 28639989bf19d66c1f37669e191ff75ba90a8d57[m
Merge: 1dce72f 5d6c2c1
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Tue Nov 3 02:04:19 2015 +0800

    Merge pull request #490 from iamjasonp/cleanup-certs
    
    Add a Certificate Cleanup tool to clean up Bridge-generated certs

[33mcommit 5d6c2c14e4f5fae31093902d419d0117cc053175[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Tue Nov 3 01:46:11 2015 +0800

    Update project.lock.json files
    
    After updating the project.lock.json files for the CertificateGenerator change,
    update the rest of them so we're consistent with versions across the repo

[33mcommit 1dce72f30a1eaa150e35d4b1eaafd6c335a53f1f[m
Merge: 525a76e e59b90e
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Nov 2 09:44:23 2015 -0800

    Merge pull request #496 from roncain/run-test-excludes
    
    run-test.sh no longer copies binaries from WCF bin folder

[33mcommit 04fe15fbc86e2ae53d326cd9052c0d7d19ff61f6[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Mon Nov 2 12:01:38 2015 +0800

    Add a Certificate Cleanup tool to clean up Bridge-generated certs

[33mcommit e59b90e83ec3ac2a741234e74a990d64dedb3bdb[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Nov 2 08:57:37 2015 -0800

    run-test.sh no longer copies binaries from WCF bin folder
    
    Prior to this change the run-test.sh script used to run the tests
    in Linux copied all the assemblies from the WCF bin folder into the
    WCF test folders.  This was harmless when the assemblies were the
    same but caused issues when the bin folder contains other versions
    of the assemblies.
    
    With this change, the script no longer copies assemblies from the
    WCF bin folder but relies on the test folders containing all they need.

[33mcommit 525a76e319b5736ac1a227487d202d53d478a168[m
Merge: 8a55b3e b18752d
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Nov 2 07:14:38 2015 -0800

    Merge pull request #482 from roncain/update-run-test
    
    Reduce number of binaries copied from CoreFx to run in Linux

[33mcommit 8a55b3e19eea91ddcbdb6c2033ec9b13f79ab204[m
Merge: 6c06c33 6000fb8
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Sun Nov 1 08:15:13 2015 -0800

    Merge pull request #489 from iamjasonp/delete-existing-certs
    
    Remove makecert-generated certificates from repo

[33mcommit 6c06c336b5182365d9d1578424fef9428ff03090[m
Merge: 74d23b1 2fd86f4
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Oct 30 20:58:00 2015 -0700

    Merge pull request #487 from StephenBonikowsky/Issue425
    
    Moving WebSocket tests to more correct location.

[33mcommit 74d23b13964596749a45d31a04510ded865df702[m
Merge: d48ffe3 3d85e84
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Sat Oct 31 04:15:12 2015 +0800

    Merge pull request #446 from iamjasonp/fix-bridgeclient-nix
    
    Add platform-dependent logic to BridgeClientCertificateManager

[33mcommit 3d85e843bf9eda61952cad91a26f4bc464a88e42[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Oct 29 19:16:15 2015 +0800

    Switch StoreLocations based on platform in BridgeClientCertificateManager
    
    In Windows, we read/write our certificates from/to the LocalMachine store.
    On *nix, however, we only have access to the User store. The BridgeClient needs
    to be aware of where it's running and switch the cert store used as necessary
    
    All user certs only to StoreName.My StoreLocation.CurrentUser
    All root certs should go to StoreName.Root StoreLocation.LocalMachine on Windows
    All root certs should go to StoreName.Root StoreLocation.CurrentUser on *nix
    
    Tcp_ClientCredentialTypeTests.TcpClientCredentialType_Certificate_EchoString needs
    to be modified as well to reflect this change
    
    Fixes #434

[33mcommit 2fd86f4fd4829f687009aee194c5a44e87b7815d[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Oct 29 17:21:09 2015 -0700

    Moving WebSocket tests to more correct location.
    
    * Fixes #425
    * Re-named the tests, a couple were inaccurate and one was completely wrong.
    * Improved comments.

[33mcommit d48ffe3740c4337347c69524f3ced7aa06202dd8[m
Merge: 99dbf91 4a79d38
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Oct 30 08:54:47 2015 -0700

    Merge pull request #483 from StephenBonikowsky/Issue367
    
    Adding test case for X509CertificateValidator scenario

[33mcommit 4a79d38d047767ca351fcc1bcb70c14e74fabe9f[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Oct 28 13:23:51 2015 -0700

    Adding test cases for X509CertificateValidator scenario
    
    * Fixes #367
    * Includes feedback from initial commit

[33mcommit 99dbf91947171fcfa63fb805da939734fe8b6488[m
Merge: 87c9289 4ed56bd
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Fri Oct 30 22:52:11 2015 +0800

    Merge pull request #481 from roncain/update-buildtools
    
    Update buildtools

[33mcommit 4ed56bdab0f558a6ca65d05c3a4985767fd68c34[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Oct 30 06:30:48 2015 -0700

    Update the project.lock.json files generated by build
    
    These were auto-generated just by running the normal
    build.cmd.

[33mcommit 7b30b1bc21990ea2bae6512bcc8fd8736ae1770e[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Oct 30 06:20:00 2015 -0700

    Take latest version of build tools and scripts from CoreFx
    
    Updates to latest build-tools (112) and clones some of the
    common build scripts to stay in sync with CoreFx

[33mcommit 6000fb8d877d6cda0a7967cc8d079c3362466986[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Fri Oct 30 15:34:20 2015 +0800

    Remove makecert-generated certificates from repo
    
    Move documentation from certificates to Documentation/archives as a reference
    for how we used to use makecert. The documentation might be useful in testing,
    but the certificates themselves are removed as they're expired and no longer work.

[33mcommit 87c9289a68bcae91e69adfcf8a92582585819fe8[m
Merge: 543e10c 8cdbd5b
Author: Hong Dai <hongdai@microsoft.com>
Date:   Thu Oct 29 11:00:40 2015 -0700

    Merge pull request #474 from hongdai/issue370
    
    Make cert parameters configurable and add TCP negative security tests

[33mcommit 8cdbd5b85631ff0d965479560563fb156f5fb65d[m
Author: Hong Dai <hongdai@microsoft.com>
Date:   Thu Oct 29 10:10:02 2015 -0700

    Make cert parameters configurable and add TCP negative security tests
    * Make cert parameters configurable by creating a CertificateCreationSettings.cs
    and pass it around(Thanks for Jason's suggestion!). I have only added parameters
    needed by the current invalid tests. It can grow as we are adding the tests.
    * Add expired certificate and custom validation fails tests.
    
    #Fix #371

[33mcommit b18752d047c0c975535cfcc3faa16e1d66377a84[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Oct 28 13:05:21 2015 -0700

    Reduce number of binaries copied from CoreFx to run in Linux

[33mcommit 543e10c5cfd0e6dad19130e37f3a088b88ecb52f[m
Merge: 34c6951 4286309
Author: Hong Dai <hongdai@microsoft.com>
Date:   Wed Oct 28 10:57:42 2015 -0700

    Merge pull request #477 from hongdai/projectfix1
    
    Fix test failure on TOF

[33mcommit 34c69519fd0b241a4fbe5dae04adbdddba28fe59[m
Merge: c603182 806cfb4
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Oct 28 09:38:11 2015 -0700

    Merge pull request #459 from StephenBonikowsky/Issue366
    
    Adding primary scenario test for Issue #366

[33mcommit 4286309dbb7cbf7280468b838a471f0b0efbd429[m
Author: Hong Dai <hongdai@microsoft.com>
Date:   Wed Oct 28 09:34:15 2015 -0700

    Fix test failure on TOF
    
    # TOF does not support some of the XUnit functionalities. Thus
    I need to make the change to only use what it supports.

[33mcommit 806cfb45d1cf01e5157d3b64eedf159be4a4dca8[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Oct 23 10:47:43 2015 -0700

    Adding primary scenario test for Issue #366
    
    * Added a property in BridgeClientCertificateManager to hold the thumbprint of the ClientCertificate so that a test case can get it.
    * Update initial PR with feedback.

[33mcommit c6031823dd5bba0f0696e778dec5ef0f9124fda6[m
Merge: 7185e27 21354e3
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Oct 27 12:12:29 2015 -0700

    Merge pull request #473 from iamjasonp/client-credential-clone-fix
    
    Fix System.ServiceModel.Description.ClientCredentials..ctor

[33mcommit 7185e2760370179583414780657b8e263ece03da[m
Merge: bf1f060 4b66810
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Oct 27 10:14:58 2015 -0700

    Merge pull request #426 from StephenBonikowsky/WebSocketStreamingTests
    
    Adding three WebSocket streaming scenario tests.

[33mcommit 4b668101ced15c40150cd49b7aa1163f5d1d2208[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Oct 13 10:24:46 2015 -0700

    Adding three WebSocket streaming scenario tests.
    
    * Opened Issue #468 for test case WebSocketHttpRequestReplyBinaryStreamed and WebSocketHttpDuplexBinaryStreamed.
    * Opened Issue #470 for test case     public static void WebSocketHttpsDuplexBinaryStreamed.
    * These tests run and pass on desktop using the Bridge.
    * These tests are a partial fix of issue 384
    * Placing these tests in a new folder called "Extensibility" based on the logic used for locating previously released WCF Samples.
    * Opened issue 425 to move other WebSocket tests to this location
    * The new Contracts include operations not used by current set of tests, leaving as is because forthcoming tests will use these operations.
    * Removed and sorted using statements wherever I saw that it needed it.
    * Incorporated feedback from initial commit

[33mcommit bf1f060c83647a490497220eec141fdd3b45faef[m
Merge: 87b0d51 87250c2
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Oct 27 08:11:50 2015 -0700

    Merge pull request #464 from roncain/reenable-stream-tests
    
    Reenable streaming tests after CoreFx fix

[33mcommit 21354e3b5b5574bf8fdb3f3382733b93356fe66c[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Sun Oct 25 22:12:53 2015 -0800

    Fix System.ServiceModel.Description.ClientCredentials..ctor
    
    The protected constructor and MakeReadOnly() method does not properly clone the
    _clientCertificate, _serverCertificate fields
    
    Fixes #458

[33mcommit 87b0d515beb243082dbf06e063441ac9bc1bda76[m
Merge: c78ef64 045d9b2
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Tue Oct 27 10:50:27 2015 +0800

    Merge pull request #461 from iamjasonp/fix-tcpclientcred-test
    
    Enable TCP client credential test

[33mcommit c78ef64d9bce8ab169fec94d140604e5aeb024fb[m
Merge: 24e6cdd 57aedb6
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Oct 26 14:29:04 2015 -0700

    Merge pull request #469 from dotnet/revert-427-master
    
    Revert "Allow caller to await on TimeoutHelper.CancelationToken"

[33mcommit 57aedb661b4b6d620e8e3c59ae5889234a9bf359[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Oct 26 14:12:45 2015 -0700

    Revert "Allow caller to await on TimeoutHelper.CancelationToken"

[33mcommit 24e6cddcda21fc0c6f0b56c7078632f9226ab9db[m
Merge: a960d98 bd8cdd1
Author: Peter Hsu <shhsu@microsoft.com>
Date:   Mon Oct 26 11:43:28 2015 -0700

    Merge pull request #427 from shhsu/master
    
    Allow caller to await on TimeoutHelper.CancelationToken

[33mcommit bd8cdd12526ba74df57c5a9fb8b6eb3b09c33f3b[m
Author: Peter Hsu <shhsu@microsoft.com>
Date:   Wed Oct 14 12:58:09 2015 -0700

    Since TimeoutHelper now generate CancelationToken in an asynchronous matter, we would expose the task so its client can choose to await on it
    
    (Performance progression at about 1% in the Http scenario)

[33mcommit a960d98ab7fb2b601250d6afcec043f8d941873d[m
Merge: 355a19d bb230d0
Author: Hong Dai <hongdai@microsoft.com>
Date:   Mon Oct 26 09:35:34 2015 -0700

    Merge pull request #457 from hongdai/issue368
    
    Update Identity positive test and add a negative test

[33mcommit bb230d03a2a8a7b58fa1dc5187dd02eaf3107690[m
Author: Hong Dai <hongdai@microsoft.com>
Date:   Mon Oct 26 09:23:36 2015 -0700

    Update Identity positive test and add a negative test
    * Update identity positve test per cert generation changes in bridge.
    * Enable the positve test because it's now pass as both product and cert
    functionalities are in place.
    * Add a negative test.
    
    Fixes issue #368

[33mcommit 87250c210bcc1fcce41020451d9ccebd924743d8[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Oct 26 09:06:59 2015 -0700

    Reenable streaming tests after CoreFx fix
    
    The streaming tests were disabled due to issue
    https://github.com/dotnet/corefx/issues/4077.
    
    But now that the issue is fixed, these tests pass again,
    so this PR re-enables them. Verified in Linux too.

[33mcommit 045d9b2a2ea2bf42999ea3268a3ee30755671d5e[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Sat Oct 24 23:51:38 2015 +0800

    Enable TCP client credential test
    
    Test was disabled due to System.Security.Claims..ctor issue where the test-runtime
    package was not up to date. Enable this test as test runtime package has now been
    updated
    
    Also change the endpoint so that we correctly obtain the DnsEndpointIdentity when
    testing when the Bridge is located at an address other than the FQDN (e.g., when
    testing against localhost or hostname)

[33mcommit 355a19dc38ed56b9d471c912a7adab22e6df6de8[m
Merge: d178895 435e543
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Mon Oct 26 14:43:28 2015 +0800

    Merge pull request #447 from roncain/disable-websockets-test-linux
    
    Disable WebSockets functional tests on Unix

[33mcommit d178895df549f890092564dd54361c504ea89613[m
Merge: 63878df 6720f0e
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Oct 23 14:17:28 2015 -0700

    Merge pull request #460 from roncain/fix-serialization-dep
    
    Lower dependency on System.Runtime.Serialization.Xml

[33mcommit 6720f0e60566e443d49f2ec84220923c8a150563[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Oct 23 12:12:10 2015 -0700

    Lower dependency on System.Runtime.Serialization.Xml
    
    The new dependency on 4.1.0 of System.Runtime.Serialization.Xml
    caused a build break on TFS for TestNet.  Reverting to the prior
    version solves the build break.
    
    The 4.1.0 version contains fixes that WCF requires, but this lower
    dependency is acceptable because the DNX will contain 4.1.0.

[33mcommit 63878df80ef7500329e37bdae9b7ebfc4f17c792[m
Merge: ca5f69b d098133
Author: Hong Dai <hongdai@microsoft.com>
Date:   Fri Oct 23 08:51:46 2015 -0700

    Merge pull request #456 from iamjasonp/certificate-privatekey
    
    Return certificates with private key from Bridge

[33mcommit d098133ad7ab97553c16bb8d034341d969de5520[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Fri Oct 23 18:41:23 2015 +0800

    Return certificates with private key from Bridge
    
    * Machine and user certs returned from the Bridge previously did not contain a
      private key, making some scenarios fail.
    * BridgeClientCertificateManager will request User rather than Machine certificates
    * BridgeClientCertificateManager will now check whether or not certificates were
      previously installed by opening the X509Store ReadOnly before attempting to
      install them
    * BridgeClientCertificateManager error message for when an CryptographicException
      "Access Denied" is thrown improved
    * Change the Bridge to install all "user" certs generated (not machine)
      We need this so that localhost tests don't require elevation

[33mcommit ca5f69bff8e505a4979dd7d814933c8402d27895[m
Merge: 2e5e8cd 64438d7
Author: Shin Mao <shmao@microsoft.com>
Date:   Thu Oct 22 18:13:51 2015 -0700

    Merge pull request #435 from shmao/issue217
    
    Use new Implemented Async Methods in System.Runtime.Serialization.Xml

[33mcommit 2e5e8cd3ddfd88216505b468f6c3b797808050d5[m
Merge: 8d7a52f 4b5b40a
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Oct 22 18:10:19 2015 -0700

    Merge pull request #455 from mconnew/UpdateSockets
    
    Update sockets

[33mcommit 4b5b40a4ac47f541dff2ec9e12903d8d78ee3a32[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Thu Oct 22 16:48:36 2015 -0700

    Made code using System.Security.Principle.Windows be conditionaly behind #if FEATURE_CORECLR

[33mcommit 1b868d2143010ee3b4d9308a452a5bb0e6c4d418[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Thu Oct 22 11:32:52 2015 -0700

    Fix missing receive timeout for WebSockets streamed mode

[33mcommit 50c0b006414c495ef42a5d29108a4de0c8d954c9[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Thu Oct 22 11:18:52 2015 -0700

    Disable Http streamed tests
    
    HttpClient is currently broken for chunked encoding so disabling streaming tests. See dotnet/corefx#4077

[33mcommit dbda7c087b571d8e7ce9c023b589b5c9422d1e12[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Thu Oct 22 11:53:01 2015 -0700

    Remove usage of Begin/EndConnect.
    
    This will need to be changed to ConnectAsync later

[33mcommit c5448ae775aa1547507976436ab8d5fd4e9164f9[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Thu Oct 22 11:50:43 2015 -0700

    Update dependencies and make tests use the same dependency versions when run

[33mcommit 8d7a52f325fba76ec4ba0b6f4b4f9e1239bfcc67[m
Merge: 7749b85 5f5f1e3
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Thu Oct 22 16:49:40 2015 +0800

    Merge pull request #454 from dotnet-bot/from-tfs
    
    Merge changes from TFS

[33mcommit 5f5f1e3f1972726bfbeedea16e0b6624fff0ca17[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Thu Oct 22 00:52:41 2015 -0700

    Update build tools to 1.0.25-prerelease-00109
    
    [tfs-changeset: 1540722]

[33mcommit 7749b8522b7fe348c907ff7438c327db4e45a83d[m
Merge: df2f33c cb5d38e
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Thu Oct 22 15:17:42 2015 +0800

    Merge pull request #453 from dotnet-bot/from-tfs
    
    Merge changes from TFS

[33mcommit cb5d38e5c98ba8e64755df31ddabcb4e393314f0[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Wed Oct 21 23:40:50 2015 -0700

    Fix restore failures
    
    Restore was failing because build-tools still contained an out-of-date project.lock.json
    Update to get that fix.
    
    Additionally we were not restoring some projects due to a DNX change,
    workaround this temporarily (dangerously close to commandline character
    limit) until DNX adds better control for specifying which projects to restore.
    
    [tfs-changeset: 1540655]

[33mcommit df2f33ccd90edb41ae89b4621430d496171ca4aa[m
Merge: 09cebd7 dde57e1
Author: Matt Connew <mconnew@microsoft.com>
Date:   Wed Oct 21 21:59:52 2015 -0700

    Merge pull request #451 from mconnew/FixNugetUpgrade
    
    Add rule to fetch packages for older reference contracts to work around nuget changes

[33mcommit dde57e1867e1861bf204f4018b4b028e295e59d0[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Wed Oct 21 21:26:38 2015 -0700

    Add rule to fetch packages for older reference contracts to work around nuget changes

[33mcommit 09cebd7f2356864055ec9c3bd51510f7b639ff93[m
Merge: e227b03 d7f791c
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Thu Oct 22 10:42:17 2015 +0800

    Merge pull request #449 from dotnet-bot/from-tfs
    
    Merge changes from TFS

[33mcommit e227b03155b566fa3ad042eb9976298673766708[m
Merge: c1b9f76 dfa66e1
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Thu Oct 22 10:07:11 2015 +0800

    Merge pull request #448 from dotnet-bot/from-tfs
    
    Merge changes from TFS

[33mcommit d7f791cb092261a2ac2b45c3b5f584b92548914c[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Wed Oct 21 18:59:09 2015 -0700

    Update lock files that were missed in previous restore.
    
    A change was made to DNX to no longer restore project.jsons
    that are in nested folders.  This caused my batch update to miss
    a number of tests and reference assembly projects.
    
    This fixes the stale lock files, but restore is still broken.
    
    [tfs-changeset: 1540448]

[33mcommit dfa66e1d8700e05cbd300d60ad05b44a6aa422d9[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Wed Oct 21 17:03:48 2015 -0700

    Update dnx to v1.0.0-rc1-15838, update dotnet projects to generations
    
    We need a new DNX to understand the latest packages.
    
    We also need to move all "dotnet" projects to use dotnetX.X (generations)
    now that the packages no longer have "dotnet" assets.
    
    The new version of DNX dropped support for the aspnetcore
    so I needed to update a few projects/packages that depended
    on old package versions that had this moniker.
    
    [tfs-changeset: 1540388]

[33mcommit 435e543517ae4c4e8b692e5420f37c8446259b25[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Oct 21 07:04:33 2015 -0700

    Disable WebSockets functional tests on Unix
    
    The WebSockets tests currently hang for long periods
    on Linux.  This PR just disables those tests on Linux.
    
    Issue #420 tracks fixing the issue and re-enabling the tests.

[33mcommit c1b9f7660389d885d5a1523904bb6cd21cb48385[m
Merge: 7ccc6ef 5d8b2ec
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Oct 21 04:34:26 2015 -0700

    Merge pull request #443 from roncain/new-run-test-sh
    
    Update run-test.sh to overwrite minimal CoreFx binaries

[33mcommit 7ccc6ef5d0642190852db6f09ea2bee8a14883a7[m
Merge: b754d0a 540f7e5
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Wed Oct 21 09:35:02 2015 +0800

    Merge pull request #445 from dotnet-bot/from-tfs
    
    Merge changes from TFS

[33mcommit 540f7e5cd876246035b09263f819123e47bc823f[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Tue Oct 20 16:54:56 2015 -0700

    BUILD: Fix build to not revaluate * dependencies
    
    Restore was regenerating lock files due to https://github.com/aspnet/dnx/issues/2771.
    
    This caused the build to pick up "live" packages after it hit the first invalid lock file.
    
    We are not yet ready to consume the live packages because they use generations
    and many of our projects are still using "dotnet".  I need to update all projects that
     use "dotnet" to use the appropriate generation before those projects can update
    their dependencies.  This is largely only a problem for our reference assembly
    projects since those are the only ones that use "dotnet".  Projects that use
    "dnxcore50" or "netcore50" are fine because NuGet/DNX maps that to the correct
    dotnet generation.
    
    This change fixes the problem lock files to avoid the bug.  We now expect to
    no longer regenerate lock files during the build.
    
    [tfs-changeset: 1539932]

[33mcommit 5d8b2ec031e8e0f8a4afe1e7a79d0b7a8da824ce[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Oct 20 12:20:57 2015 -0700

    Update run-test.sh to overwrite minimal CoreFx binaries
    
    Prior to this change, run-test.sh overwrote all the binaries
    from the CoreFx packages with those built in the CoreFx repo.
    
    After this change, it relies mostly on the binaries from the
    packages except for select list of binaries that are not correct
    in their package.

[33mcommit b754d0a31e662e518fdc242b1f34111ba15ea784[m
Merge: c3ea167 edefaff
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Tue Oct 20 14:19:31 2015 +0800

    Merge pull request #440 from dotnet-bot/from-tfs
    
    Merge changes from TFS

[33mcommit edefaff3407d23a4294356bec760fae4653a60b9[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Mon Oct 19 14:05:47 2015 -0700

    Add SslProtocols API to System.ServiceModel.NetTcp contract
    
    Adds
    * System.ServiceModel.Channels.SslStreamSecurityBindingElement.SslProtocols
    * System.ServiceModel.SslStreamSecurityBindingElement.SslProtocols
    
    APIs to the System.ServiceModel.NetTcp contract so that end users can specify
    the Ssl protocol version they use for secure transports
    
    Also fixes reference assemblies located in <assembly>/ref directories as well as
    .NET Generation changes that weren't picked up in S.SM.Primitives
    
    [tfs-changeset: 1539413]

[33mcommit c3ea1678f775bad03e34440afe9ed58c77853bd8[m
Merge: a3ecb9c 162f498
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Mon Oct 19 17:30:24 2015 +0800

    Merge pull request #436 from dotnet-bot/from-tfs
    
    Merge changes from TFS

[33mcommit 162f498de8e829de07dcf5bb0f580248439636cd[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Sun Oct 18 21:12:23 2015 -0700

    Switch from using "dotnet" to using "dotnet<generation>"
    
    All projects that previously used dotnet have been updated to specify the appropriate generation of the surface area they depend on or in the case of desktop inbox contracts, the generation that includes the set of platforms they can support.
    
    Now all packages contain all generations of API that they have ever shipped.
    
    In doing this I needed to change how we determine package version.  Before we ensured all assemblies would have the same version and used that.  Now we choose the max.
    
    Additionally I needed to change how we chose which asset to use for netcore50 when the dotnet asset was obscured by a placeholder for a previous netcore release (eg System.Runtime will have placeholders for Win8 since it was inbox there).  To do this I wrote a task that uses nuget to evaluate the package assets with and without the placeholders.  In this way it chooses the "best" dotnet implementation and reference assembly to use for netcore50.
    
    This new model made my old MSBuild-based validation impossible to carry forward, so I wrote better validation in a task that actually uses Nuget's asset resolution algorithm.  This uncovered many existing issues in packages that I have cleaned up.  The validation algorithm could use some polish but its working now.  Perf is not great, package build went from 15s to ~2 minutes.  I'll work on this some more by making it incremental (input: nuspec, output: marker created on success).  Profiling shows that most of the time is spent in calling the nuget APIs.  I'll see if I can reduce some of this with caching for things that don't change (eg: RID graph).  Ultimately I think its a reasonable tradeoff for the types of bugs I can find this way.
    
    [tfs-changeset: 1539216]

[33mcommit a3ecb9cbfbcf4f00cc7f0f9a4abb8e1f1f30dbff[m
Merge: 740a7eb 4158257
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Sun Oct 18 16:42:22 2015 +0800

    Merge pull request #423 from iamjasonp/client-cert-generation
    
    Separate User and Machine Certificate Resources in Bridge

[33mcommit 740a7eb996a0c280498a32b1357c930cbed47148[m
Merge: 5afe865 9cda8c1
Author: Hong Dai <hongdai@microsoft.com>
Date:   Fri Oct 16 15:30:22 2015 -0700

    Merge pull request #433 from hongdai/issue431
    
    Dispose websocket object after Close

[33mcommit 9cda8c1541c148f34631a916286ca23e7ecd546c[m
Author: Hong Dai <hongdai@microsoft.com>
Date:   Fri Oct 16 10:40:09 2015 -0700

    Dispose websocket object after Close
    
    * We use WebSocket.CloseAsync to close the WebSocket object. We do not dispose it
    in normal Close case. We need to aggressively dispose it as WebSocket
    is a disposable object and hold unmanaged resources.
    
    Fixes #431

[33mcommit 41582572f9eead8f79fc0abf2d835ca39ef29f01[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Sat Oct 17 01:03:26 2015 +0800

    Add explanatory comments to the Certificate Generation resources

[33mcommit 5afe86584e6eef604815b25d75082d24dc8ed46d[m
Merge: c291133 c08b6ea
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Fri Oct 16 18:07:41 2015 +0800

    Merge pull request #430 from StephenBonikowsky/Issue323
    
    Remove SR.Format calls and replace with string.Format

[33mcommit c291133baf9880f4189bc20ed6a242157b7bdf0a[m
Merge: 1f108ed 7198f48
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Fri Oct 16 17:25:27 2015 +0800

    Merge pull request #432 from iamjasonp/fix-commit-message
    
    Fix commit message for  "[tfs-changeset: 1538408]", PR #429

[33mcommit 7198f4878eeb0bd09a79c4b654f91efc280bd16e[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Fri Oct 16 11:52:03 2015 +0800

    Add X509Certificate-related APIs to System.ServiceModel.Primitives
    
    This change will allow us to move onto the next step to add new API to
    the System.ServiceModel.NetTcp contract
    
    This is a re-commit of 8e06f18 with a new commit message. It was originally
    checked in via TFS and merged via #429 with an empty commit message

[33mcommit f29755fe65735ab6348c2fff2237a240e9609b8d[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Fri Oct 16 11:50:35 2015 +0800

    Fix commit message for "[tfs-changeset: 1538408]"
    
    Due to infrastructure and package building reasons, we had to check in via TFS
    and let the change mirror to Git. Unfortunately, the checkin message was omitted
    when it was checked in via TFS - naturally the commit message in Git was blank too.
    We want to make it right so that things like git blame show useful data
    
    This reverts commit 8e06f18cf9e1af4ab353692ce79a38ab359b54ec. We will immediately
    re-commit this change to fix the blank commit message that was accidentally merged
    into our master branch

[33mcommit 1f108edafb4ffe46f995fc93191bb3be2a2e4a5d[m
Merge: 21a3788 8e06f18
Author: Matt Connew <mconnew@microsoft.com>
Date:   Thu Oct 15 18:46:17 2015 -0700

    Merge pull request #429 from dotnet-bot/from-tfs
    
    Merge changes from TFS

[33mcommit c08b6ea17700c3e6d7e00babb69125df43a727f1[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Oct 15 15:20:07 2015 -0700

    Remove SR.Format calls and replace with string.Format
    
    * Const strings had already been included with the expected ToString formatting but the two ToString methods
    had not been updated and were still using SR.Format
    * Fixes #323

[33mcommit 8e06f18cf9e1af4ab353692ce79a38ab359b54ec[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Oct 15 14:32:30 2015 -0700

    [tfs-changeset: 1538408]

[33mcommit 21a3788a45ff5e44db1a616e86244ca2f0565746[m
Merge: 7734796 31cdda2
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Oct 15 11:11:52 2015 -0700

    Merge pull request #428 from StephenBonikowsky/Issue398
    
    Disabling test due to incorrect behavior on the Bridge

[33mcommit 31cdda214045f88db69b0f18e9214b7b2769393c[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Oct 15 10:22:28 2015 -0700

    Disabling test due to incorrect behavior on the Bridge
    
    * Prior to moving to the Bridge this test work as expected, since the move it looks like the
    wrong ServiceHost is getting killed which can cause other tests to randomly fail.
    * Due to other higher priority tasks just disabling this test for the short term.

[33mcommit eed1ba44f396cc134aba23fe9d9754160b104821[m
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Thu Oct 15 02:02:19 2015 +0800

    Update documentation text for UserCertificateResource
    
    After adding the UserCertificateResource, update the documentation to reflect the new endpoint

[33mcommit a3432b1276107a9782fb0db18bd6242424595f83[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Wed Oct 14 18:29:52 2015 +0800

    Separate User and Machine Certificate Resources in Bridge
    
    Currently we do not differentiate between machine and user certificates in the
    MachineCertificateResource. The certs are slightly different, and will fail validation
    when it comes time to validate a cert (at least on a Windows box)
    
    In order to properly generate user certificates with User Principal Names,
    we need to add some different Subject Alt Name X509 extensions to the certificate
    compared to a cert for the machine name

[33mcommit 7734796f0be376764078a4dbedfbfe5fab008254[m
Merge: c8b4996 8a9d038
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Thu Oct 15 12:23:00 2015 +0800

    Merge pull request #414 from iamjasonp/cert-generation-docs
    
    Add documentation for certificate generation resource

[33mcommit c8b49962c70f617a9526fcee6cbcbb5e2a9114da[m
Merge: ac008d9 f287ffd
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Oct 14 06:09:30 2015 -0700

    Merge pull request #421 from roncain/reactivate-encoding-test
    
    Reactivate negative encoding tests disabled for Linux

[33mcommit f287ffdb811df4f1d6d8b8848043ecaa19f55775[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Oct 13 13:34:52 2015 -0700

    Reactivate negative encoding tests disabled for Linux
    
    These tests were deactivated on Linux because Encoding had
    not yet been implemented.  But this was fixed in CoreFx via
    issue https://github.com/dotnet/corefx/issues/2774.

[33mcommit ac008d9c2b55b5f0a33b6c93517bdb61d45988c2[m
Merge: aeeea64 04aa29e
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Tue Oct 13 16:02:45 2015 +0800

    Merge pull request #401 from iamjasonp/sm-update-claims
    
    Move System.Private.ServiceModel to use new S.Security.Claims version

[33mcommit 8a9d0381696005bc3bc0d7e29eb291582bb401eb[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Tue Oct 13 15:50:15 2015 +0800

    Add documentation for certificate generation resource
    
    After adding the certificate generation logic to the Bridge, we need to add some
    documentation to explain how it's used

[33mcommit 04aa29ef36506e7210380634d92dcd90c8c45c8e[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Mon Oct 12 23:01:38 2015 -0700

    Update project.lock.json files after rebase to master

[33mcommit 53891b96b38e99bb0749aaf183f41b8c1db5cc80[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Fri Oct 9 02:37:54 2015 +0800

    Move System.Private.ServiceModel to use new S.Security.Claims version
    
    There was a bug in System.Security.Claims that prevented our Ssl stream scenarios
    from working. Moving the dependency for S.Security.Claims to depend on the -beta
    versions so we can consume this change
    
    Resolves #387

[33mcommit aeeea64de977306642c3768f6b9bd08a3d3e3f52[m
Merge: a5222c4 6940f3c
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Tue Oct 13 10:50:22 2015 +0800

    Merge pull request #412 from iamjasonp/remove-contract-requires
    
    Remove Contract.Requires<Type> calls from files

[33mcommit 6940f3c887646e6232c2772e05094f0a717be9e2[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Sun Oct 11 12:03:31 2015 +0800

    Remove Contract.Requires<Type> calls from files
    
    Replace with Contract.Requires. With the generic Contract.Requires,
    an exception will be thrown in CoreCLR when this codepath is hit.

[33mcommit a5222c47c2a63a79a4757e9e117610837cfabcdd[m
Merge: 26ff8ef aff9b26
Author: Matt Connew <mconnew@microsoft.com>
Date:   Mon Oct 12 16:19:11 2015 -0700

    Merge pull request #410 from shhsu/CoalescedTimeoutTokens
    
    Used CoalescedTimeoutTokens to cut down thread contention

[33mcommit aff9b266125e5e780951bb159dca8ff08fadb15e[m
Author: Peter Hsu <shhsu@microsoft.com>
Date:   Fri Oct 9 13:27:35 2015 -0700

    Used CoalescedTimeoutTokens to cut down thread contention when disposing Timers created by Cancelation tokens with timeouts

[33mcommit 26ff8efc1d2f6054235dc59423e659343c19700b[m
Merge: b57cf15 87dfd51
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Oct 12 12:53:46 2015 -0700

    Merge pull request #390 from Kagamine/patch-1
    
    Correcting the word spelling

[33mcommit b57cf15be5761f9a59048f528a9308fe3103ec87[m
Merge: df03520 99c7966
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Oct 12 09:51:19 2015 -0700

    Merge pull request #405 from roncain/remove-use-system-net-security
    
    Remove code dependencies on System.Net.Security for NET Native

[33mcommit df035202d4ebef4842fc9b7a1952f5eb4a02e0f4[m
Merge: c9301b3 bdb87bd
Author: Matt Mitchell <mmitche@microsoft.com>
Date:   Fri Oct 9 09:56:18 2015 -0700

    Merge pull request #406 from mmitche/update-input-builds
    
    Update input coreclr builds to point to new locations

[33mcommit 99c7966bd9abd06ccc9760951b4c4412f683f93a[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Oct 9 09:28:32 2015 -0700

    Update project.json file to move System.Net.Security to dnxcore50
    
    This is part 2 of a 2-step PR.  It updates the project.json file
    to make System.Net.Security be only a dnxcore50 dependency.  It
    also updates the project.lock.json to match.
    
    Both commits of this PR are necessary in the merge to make a
    version that builds in CoreCLR and NET Native in TFS.
    
    But the first commit can be cherry-picked if necessary to move only
    the code changes to the release branch.  This 2nd commit however
    carries dependencies we cannot propogate into the release branch.

[33mcommit bdb87bd20050fe3e73a5045ac6da3049493cba08[m
Author: Matt Mitchell <mmitche@microsoft.com>
Date:   Fri Oct 9 09:21:27 2015 -0700

    Fix Linux PR test jobs so that they launch the right builds

[33mcommit 70de0f48fb2c764325d8633a9326322a28e9b013[m
Author: Matt Mitchell <mmitche@microsoft.com>
Date:   Fri Oct 9 08:56:38 2015 -0700

    Update input coreclr builds to point to new locations

[33mcommit e0023682b7b942b9d174be858dbe7c440eac457a[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Oct 9 08:41:58 2015 -0700

    Remove code dependencies on System.Net.Security for NET Native
    
    The System.Net.Security package is not available for UWP apps,
     but WCF currently lists it as a dependency and uses it,
     regardless of platform. This causes UWP apps to fail in beta8.
    
    The fix is to remove use of it when building for Net Native.
    
    This PR is the first of 2. The first adds conditional compilation
     directives around code using System.Net.Security to exclude usage
     when building for NET Native.
    
    The 2nd PR will modify the project.json and project.lock.json files
     to remove the dependency.
    
    It is being done in 2 stages like this so that the code changes
     can be integrated into the release branch for beta8 independently.
    
    The change to the project*.json files will require careful planning
     so that when integrated to the release branch it does not carry with
     it the new (post-beta8) dependencies currently in WCF.

[33mcommit c9301b3b869fc745c26b587f8329f22f953ffd5f[m
Merge: 8d1fa61 71978b5
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Oct 9 08:27:55 2015 -0700

    Merge pull request #400 from iamjasonp/update-xunit-210
    
    Update xunit to version 2.1.0

[33mcommit 8d1fa61cbe8baf08c42db2c4704df6a3df799910[m
Merge: 31ebb89 74ac1cf
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Oct 9 06:24:03 2015 -0700

    Merge pull request #403 from iamjasonp/fix-dns-uwp
    
    Remove dependency on S.Net.NameResolution for UWP in BridgeClient

[33mcommit 74ac1cfbfc4055357988e57b469622ef0ba7a94a[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Oct 8 16:56:55 2015 +0800

    Remove dependency on S.Net.NameResolution for UWP in BridgeClient
    
    BridgeClientCertificateManager took a dependency on S.Net.NameResolution, but
    .NET Native doesn't support the contract
    
    We can get away with a non-machine named cert on the client side for now
    
    Fixes #397

[33mcommit 71978b5138b9070efe377ca4d0c62c6ecb0ea9be[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Fri Oct 9 01:30:05 2015 +0800

    Update xunit to version 2.1.0

[33mcommit 31ebb898db6671b64242ebf853852aba29ce7519[m
Merge: ea756b7 974b787
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Thu Oct 8 02:23:33 2015 +0800

    Merge pull request #394 from iamjasonp/client-cert-after377
    
    Implement dynamic X509 client cert generation for Bridge

[33mcommit ea756b777d7c300cb99370580d5b87b1d1ecf2d0[m
Merge: 86047df 5626db5
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Oct 7 11:18:10 2015 -0700

    Merge pull request #383 from ericstj/ericstj/generationRefAssms
    
    Add reference assembly projects for significant past contract versions

[33mcommit 974b787382c3f74e00130fb413db6f51d3c0b9ef[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Oct 8 00:13:34 2015 +0800

    Clean up some strings and logic for certificate generation
    
    * Responding to PR comments and cleaning up loose ends
    * Rename some consts so they make more sense
    * Cleaning up some logic around CrlUriRelativePath and move the URLs out of the
      CertificateGenerator helper
    * Added some locks to prevent concurrent access in the BridgeClientCertificateManager

[33mcommit bba46282ec8e253a4d4d7206843e7b02d966f905[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Wed Oct 7 18:58:46 2015 +0800

    Look up certificates by thumbprint instead of subject in BridgeClient
    
    * Certificates previously were looked up by subject name, but we want to look
      up the certificates by thumbprint instead since subject names can be
      a little confusing, but thumbprints are unique
    * Clean up some BridgeClient methods per previous PR

[33mcommit e17c3492b9cc7af14edf61e1fb5f88f43cb381a2[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Wed Oct 7 18:55:31 2015 +0800

    Add ability to revoke certificates on Bridge CRL
    
    Allow PUT request to CertificateRevocationListResource to revoke certificates
    Certs can be revoked by serial number
    
    Also allows a GET request to the MachineCertificateResource to search by
    thumbprint instead of just by subject name
    
    Fixes #395

[33mcommit 9ec1db68fe2dd2e8767859296b49177dafd9b7d4[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Wed Sep 30 17:31:03 2015 +0800

    Implement dynamic X509 client cert generation for Bridge
    
    The bridge currently relies on static certificates for a certificate authority
    cert and localhost certificate signed by the authority.
    As such, cross-machine HTTP tests are dependent on a the well-known authority
    certificate for server cert validation, and cross-machine TCP tests will
    not work as the self-signed certs are generated for the host "localhost".
    
    This change allows the Bridge to accept requests to generate X509 Certificates
    dynamically and also serves up certificate revocation lists.
    
    Certificates are designed to be short-lived (24h by default) but this can
    be changed in the testproperties.props file along with other Bridge
    configuration.
    
    This commit addresses the following:
    * X509 certificates are now generated dynamically (Fixes #324)
    * Certificates are now generated via an IResource on the Bridge (Resolves #336)
    * Certs generated have a Subject Alternative Name (Fixes #320)
    * Support for Certificate Revocation Lists (Resolves #310)
    
    This also addresses the following issues:
    * Bridge can return raw data on a GET request (Fixes #373)
    * Lay the foundation to be able to release certificates based on a
      well-known name (#335, #365 not yet fixed entirely)
    
    This set of changes also unblocks scenarios relying on chain validation
    as chain validation will now succeed - this means that tests for (say)
    TCP will now work as expected, while we are waiting for new APIs to be put
    in place (such as #302)
    
    Add target reference to WcfTestBridgeCommon for package restore
    
    Merges changes from PR #377 for allowing addressable Resource URIs

[33mcommit 86047df847dc737f534950e192ea967321bad129[m
Merge: b4ebf7d 28a508b
Author: Matt Mitchell <mmitche@microsoft.com>
Date:   Tue Oct 6 21:59:30 2015 -0700

    Merge pull request #393 from mmitche/fix-netci
    
    Minor fixes to job definitions

[33mcommit 28a508bdaf90128a804ed50817767f4f906ed080[m
Author: Matt Mitchell <matchell@outlook.com>
Date:   Tue Oct 6 21:58:43 2015 -0700

    Minor fixes to job definitions

[33mcommit b4ebf7dcf3a22eb9a68a740f873517bcdc0b0243[m
Merge: eb338c5 34b1a8f
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Wed Oct 7 09:14:34 2015 +0800

    Merge pull request #392 from mmitche/update-badges
    
    Update badges to use the new jobs

[33mcommit 34b1a8f54ff3bbace2d7b03a7c91e47cec8f7a4d[m
Author: Matt Mitchell <mmitche@microsoft.com>
Date:   Tue Oct 6 16:56:43 2015 -0700

    Update badges to use the net code coverage job

[33mcommit eb338c56e9fa2df7ec6b82989f43218b46caa040[m
Merge: d51d6a2 d49adf6
Author: Matt Mitchell <mmitche@microsoft.com>
Date:   Tue Oct 6 16:55:14 2015 -0700

    Merge pull request #391 from mmitche/fix-include-pattern
    
    Should include bin/** from upstream linux build, not exclude them

[33mcommit d49adf6fb16eec834acbc8bd85d5cff993ac2a3d[m
Author: Matt Mitchell <mmitche@microsoft.com>
Date:   Tue Oct 6 16:54:17 2015 -0700

    Should include bin/** from upstream linux build, not exclude them

[33mcommit 5626db5dc1a2a3edd037158fffc50096538c97ce[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Tue Oct 6 16:19:46 2015 -0700

    Update build tools to 1.0.25-prerelease-00104

[33mcommit 5f584f6417f26ef5c0d4f4eb15e2ad7261aa2fd0[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Thu Oct 1 12:06:06 2015 -0700

    Add deployment projects for past reference assemblies.

[33mcommit 4bf9da857f2e666692df52669c9bcc66be4f1e70[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Thu Oct 1 12:01:01 2015 -0700

    Enable build of reference assembly projects

[33mcommit 87dfd51b00597c5b1d8c7cbe02a5a670327f02f1[m
Author: あまみや ゆうこ <1@1234.sh>
Date:   Wed Oct 7 07:09:55 2015 +0800

    Correcting the word spelling

[33mcommit d51d6a281f573c9d8274d50ec70bbdb79ae7c2e5[m
Merge: 283c7cf 857d940
Author: Matt Mitchell <mmitche@microsoft.com>
Date:   Tue Oct 6 14:30:55 2015 -0700

    Merge pull request #389 from mmitche/fix-build-flows
    
    Fix bug in build flow

[33mcommit 857d94085f26a19da6a55235289a6351a70a8857[m
Author: Matt Mitchell <mmitche@microsoft.com>
Date:   Tue Oct 6 14:30:04 2015 -0700

    Fix bug in build flow

[33mcommit 283c7cf23dc6ec42e98f6a001e2c8eb0dc169177[m
Merge: 201f173 7404234
Author: Matt Mitchell <mmitche@microsoft.com>
Date:   Tue Oct 6 13:11:46 2015 -0700

    Merge pull request #388 from mmitche/finish-wcf
    
    Add the rest of WCF's test functionality

[33mcommit 7404234f3d5e301258af019bb77ff259fb43f65a[m
Author: Matt Mitchell <mmitche@microsoft.com>
Date:   Tue Oct 6 09:34:43 2015 -0700

    Add the rest of WCF's test functionality
    
    This is 3 parts:
    
    1) Add outerloop testing definition.  Add an on demand PR trigger "@dotnet-bot test outerloop please" (tests debug and release outerloop) and a 4 hour rolling trigger.
    2) Add coverage testing definition.   Add an on demand PR trigger "@dotnet-bot test code coverage please"
    3) Move Linux testing into CI definition.  This is more complicated because of the number inputs and jobs required.  Will get simpler of course when there is native Linux building available.

[33mcommit 201f17329deace9694038cb963a639c4520ae027[m
Merge: ddfa87b 302e4da
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Tue Oct 6 12:05:48 2015 +0800

    Merge pull request #386 from iamjasonp/expose-cert-setters-pt1
    
    Bump System.Private.ServiceModel version to 4.1.0

[33mcommit 302e4da7a80c7648f7d53b894c0f5eb654513840[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Tue Oct 6 01:30:36 2015 +0800

    Bump System.Private.ServiceModel version to 4.1.0
    
    In preparation for API changes to System.ServiceModel.Primitives and
    System.ServiceModel.NetTcp, we need to bump the minor version of
    System.Private.ServiceModel and wait for the nightly build to push out
    a new package.

[33mcommit ddfa87b1822966450cbc52b0f14455845fc77210[m
Merge: a17aac9 7655d5d
Author: Matt Connew <mconnew@microsoft.com>
Date:   Fri Oct 2 10:40:27 2015 -0700

    Merge pull request #382 from mconnew/WebSockets
    
    Fix ws vs http uri schema for WebSockets and fix timeout bug

[33mcommit 7655d5dc95a82912df5303ac4d750ee265fc8393[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Thu Oct 1 18:51:07 2015 -0700

    Move to buildtools version 1.0.25-prerelease-00097
    
    This fixes the websockets tests

[33mcommit a17aac95ff84e2bb1b2fc997d6e3cdd8757f9420[m
Merge: 687f4f3 24e3d3a
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Oct 1 03:54:44 2015 -0700

    Merge pull request #381 from roncain/rename-infra-test-dll
    
    Rename DLL generated by Infrstructure.Common.Tests

[33mcommit d829712cce48f999389423933d549d6eaec767db[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Wed Sep 30 18:32:50 2015 -0700

    Fix ws vs http uri schema for WebSockets and fix timeout bug

[33mcommit 687f4f3a4a6808f84b1833297400c640d11d7397[m
Merge: 85d05d8 97a765a
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Sep 30 10:07:05 2015 -0700

    Merge pull request #362 from StephenBonikowsky/Issue3464
    
    Adding ActiveIssue for SendTimeout negative test.

[33mcommit 85d05d8c148736042b78c1179ac46b545bd2575b[m
Merge: ee31c32 75952fe
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Sep 30 10:04:31 2015 -0700

    Merge pull request #379 from StephenBonikowsky/AddWsWebSocketTest
    
    Adding WebSockets test using WS scheme.

[33mcommit 24e3d3a1402c011eb7450202ca983d655e063a49[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Sep 30 04:28:42 2015 -0700

    Rename DLL generated by Infrstructure.Common.Tests
    
    Renames the DLL generated by this project to match the
    name of the folder and project.  This is necessary to
    accommodate the run-test.sh conventions that assume the
    DLL's match their folder and project names.
    
    Fixes #376

[33mcommit ee31c3255753d86e6431b69c7af55fe10277616c[m
Merge: 7576f5e babdfee
Author: Sajay Antony <sajaya@microsoft.com>
Date:   Tue Sep 29 19:58:05 2015 -0700

    Merge pull request #377 from SajayAntony/ResourceUris
    
    Allow addressable resource URIs and allow query of resources.

[33mcommit babdfee997321981a124fa6ad6aa374bebaccc22[m
Author: Sajay Antony <sajaya@microsoft.com>
Date:   Tue Sep 29 16:15:30 2015 -0700

    Support listing out all available resources in the bridge.
    
    This will enable a user to query /resource and get a list of all
    available resources that have been loaded into the bridge.

[33mcommit 97a765af8ca099595d4e85c00e642cf530dbf981[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Sep 29 15:03:57 2015 -0700

    Updating PR #362 with feedback.
    
    * Updated timeout variable name to be more meaningful.
    * Changed validation lower range timespan to account for 15 ms accuracy.
    * Changed validation upper range to a more realistic 6 seconds.
    * Changed the serviceOperationTimeout for the zero second SendTimeout test to 5 seconds.

[33mcommit d2ad4bd827ea1ec6ba119bf81aa0b8e7eacbb1ea[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Sep 25 13:21:19 2015 -0700

    Adding ActiveIssue for SendTimeout negative test.
    
    * Test case name: SendTimeout_For_Long_Running_Operation_Throws_TimeoutException
    * Updated it to provide the service operation with the timeout value to use on the server.
    * Checking the elapsed time against an expected range still seems like the right thing to do.
    * Fixes #273

[33mcommit 75952fee0eda2aa05bda87aa74286c9603c83f4d[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Sep 29 13:38:08 2015 -0700

    Adding WebSockets test using WS scheme.
    
    * WebSockets does not work after the merge of PR #358
    * Adding an additional test using the ws scheme that was expected to work.
    * Also updating tests in the same file to use the new test helper to cleanup communication objects.

[33mcommit a260b68f929348f18d2e7d6368eac5ad18a9875d[m
Author: Sajay Antony <sajaya@microsoft.com>
Date:   Tue Sep 29 12:27:21 2015 -0700

    Allow addressable resource URI's

[33mcommit 7576f5eb6d2476193eb9c2e964a82706fa8c83f9[m
Merge: 16a2a1c 43ecd0f
Author: Matt Mitchell <mmitche@microsoft.com>
Date:   Tue Sep 29 09:52:31 2015 -0700

    Merge pull request #375 from mmitche/update-badges
    
    Update innerloop jobs to new badges

[33mcommit 43ecd0f3e80f2af05d50a88438494a1d2b80a848[m
Author: Matt Mitchell <mmitche@microsoft.com>
Date:   Tue Sep 29 08:49:06 2015 -0700

    Update innerloop jobs to new badges

[33mcommit 16a2a1ce4412ee0fea646309cb6a614a5880cf44[m
Merge: a4c59bb e89d33f
Author: Matt Mitchell <mmitche@microsoft.com>
Date:   Mon Sep 28 14:33:41 2015 -0700

    Merge pull request #364 from mmitche/add-netci
    
    Add netci

[33mcommit e89d33f34572bf00ffd8b99deaef42fcad746053[m
Author: Matt Mitchell <mmitche@microsoft.com>
Date:   Mon Sep 28 09:25:08 2015 -0700

    Initial commit of netci.groovy CI definition (replaces manual configuration)

[33mcommit a4c59bb43f74d4bdb6500b20b3cb98427f39a6a0[m
Merge: a8668ff fa7a009
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Sep 28 09:28:24 2015 -0700

    Merge pull request #363 from roncain/mark-run-tests-executable
    
    Mark run-tests.sh executable

[33mcommit fa7a0095f949b65ed4e78533cc3d2f3054ee2eb6[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Sep 28 06:04:38 2015 -0700

    Mark run-tests.sh executable

[33mcommit a8668ff8887374f3ddc75ab11c58b4155a6c8996[m
Merge: afe32b1 058e9f6
Author: Matt Connew <mconnew@microsoft.com>
Date:   Thu Sep 24 21:47:01 2015 -0700

    Merge pull request #358 from mconnew/WebSockets
    
    Implement WebSockets

[33mcommit 058e9f61fdbe4237c970f7216bb70cd0c02164f2[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Thu Sep 24 13:10:13 2015 -0700

    Move WebSockets dependency from coreclr to all platforms

[33mcommit 55ff2289ef0396bd88c3e7a54c6678ff3d66f352[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Thu Aug 6 16:45:24 2015 -0700

    Implement WebSockets

[33mcommit afe32b11b9f71a63ffd39d730c7cc466ef8ff41a[m
Merge: efaf665 0a6487f
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Thu Sep 24 06:31:41 2015 +0800

    Merge pull request #355 from StephenBonikowsky/FixingComments
    
    Fixing comments for ClientCredentialType tests.

[33mcommit efaf66523803c6db8e941bfb16e60e103b4c6272[m
Merge: fb9eedb fda8e09
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Sep 23 14:39:19 2015 -0700

    Merge pull request #354 from roncain/resource-parameters
    
    Enable IResources to receive and return name/value pairs

[33mcommit fda8e095d569d92d3e155baff4fb4330c001d423[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Sep 23 14:24:51 2015 -0700

    Prior to these changes, invoking the IResource interface
     on the Bridge did not pass any part of the request content
     to the IResource. After this change, the ResourceRequestContext
     contains name/value pairs deserialized from the request content.
    
    This change also solidifies the IResource contract so that the
     Get and Put methods return a ResourceResponse rather than object.
     This permits resources to return name/value pairs as well as to
     receive them.
    
    The Bridge Client was updated to expect a response containing
     name/value pairs as json instead of the regex match.
    
     The ResourceController allows name/value pairs for GET via query
     parameters.  PUT accepts either query parameters or name/value
     pairs in the content.
    
    The ResourceController was hardened to handle missing or
     incomplete request content and to trace the incoming and
     outgoing name/value pairs.
    
    Fixes #353

[33mcommit 0a6487f3eb6903b8821d43f0c50cc98934fbc052[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Sep 23 11:07:47 2015 -0700

    Fixing comments for ClientCredentialType tests.

[33mcommit fb9eedb3bbc464bdf5819ef454a4e21f31f45571[m
Merge: 4815f2e 142d42c
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Thu Sep 24 00:10:00 2015 +0800

    Merge pull request #347 from iamjasonp/pull-342
    
    Enable X509Certificates on .NET Native

[33mcommit 142d42ca4876db3b3d81d1175316192ba37c3dc4[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Sep 17 17:06:59 2015 -0700

    Enable X509Certificates on .NET Native
    
    Previously, there were #ifs commenting out X509Certificate in .NET Native since
    the packages were not supported yet. Packages recently got published for .NET
    Native, so take out the #ifs and turn on certificate support for .NET Native
    
    Fixes #306

[33mcommit 4815f2e6b5202f7da20955bb749491fae7e4f205[m
Merge: 887aa9b b5dde9f
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Tue Sep 22 23:51:04 2015 +0800

    Merge pull request #350 from dotnet/readme-badges
    
    Update README.md badges

[33mcommit 887aa9be2118762a6cbfc013ea70f910950c4012[m
Merge: d7413a6 55d6d67
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Tue Sep 22 23:50:55 2015 +0800

    Merge pull request #349 from iamjasonp/update-buildtools-00093
    
    Bump buildtools to version -00093 to fix code coverage

[33mcommit b5dde9f32c2efd3b9edeae1b24ac25b9d4a8c307[m
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Tue Sep 22 13:02:05 2015 +0800

    Update README.md badges
    
    * Switch to shields.io so we can customize labels on the build shields
    * Change the code coverage shield to display data
    * Added outerloop test shield

[33mcommit 55d6d67d8039d8f355dcfb18bb8b8e8a6da982ae[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Tue Sep 22 11:27:41 2015 +0800

    Bump buildtools to version -00093 to fix code coverage

[33mcommit d7413a62e079bfdfebbd1bf3f5ef1a0a9dc02171[m
Merge: 9913fea 966302b
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Tue Sep 22 10:59:02 2015 +0800

    Merge pull request #348 from roncain/linux-test-fixes
    
    Disable failing negative Encoding tests on Linux

[33mcommit 9913fea8c24b6caddad7eb448e362ef7ee4c37b8[m
Merge: c31163a 437a48f
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Tue Sep 22 10:19:29 2015 +0800

    Merge pull request #344 from khdang/fix292_xmlserializerformat
    
    Fix XmlSerializer to work on NetTcp/Duplex MEP

[33mcommit 437a48f158c15bde1cee0c6556efbe5e75728c57[m
Author: Khoa Dang <Khoa.Dang@microsoft.com>
Date:   Fri Sep 18 11:44:16 2015 -0700

    Fix XmlSerializer not working on NetTcp/Duplex MEP

[33mcommit 966302bb2044e103ef47ce83159bf706755fe556[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Sep 21 13:37:46 2015 -0700

    Disable failing negative Encoding tests on Linux
    
    Two negative tests using known-unsupported Encodings were
    failing due to issue https://github.com/dotnet/corefx/issues/2774
    which returned UTF-8 for every unrecognized encoding.
    
    Adds [ActiveIssue] to these tests using issue #333 which tracks
    https://github.com/dotnet/corefx/issues/2774.  The [ActiveIssue]
    disables these tests only on Linux.  They still execute on Windows.
    
    Also modifies run-test.sh to exclude the TestRuntime folder from
    CoreFx binaries when preparing the tests. The binaries in the WCF
    test folders already contain the correct dependent binaries, and
    allowing the TestRuntime folder to overwrite them gives out-of-date
    or platform-incorrect binaries.
    
    With these 2 changes, all unit tests not marked with [ActiveIssue]
    pass on Linux.

[33mcommit c31163a9205ed7b3719ff0bf474d64c56d1536a6[m
Merge: 2d885c9 3bff6be
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Sep 21 06:30:18 2015 -0700

    Merge pull request #345 from roncain/outerloop-xplat
    
    Enable OuterLoop testing for x-plat

[33mcommit 3bff6bed89a0164878285e558bf0bfd6f2026c97[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Sep 21 06:22:36 2015 -0700

    Add instructions how to run OuterLoop tests

[33mcommit 3d3ff8c4ca346218ef64ae254b4580b59ed75e85[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Sep 16 13:56:47 2015 -0700

    Enable OuterLoop testing for x-plat
    
    These changes make it possible to tell the run-test.sh script
    that OuterLoop tests should be included.  More work is required
    to allow a CI machine to host a 2nd machine, but this PR makes
    it possible to test manually.
    
    This PR also enhances the error reporting from BridgeClient to
    improve the debugging experience talking x-machine to the WCF
    Bridge component.

[33mcommit 2d885c98e51cb3104d702dc094f39328bf05074b[m
Merge: b93b9d4 5fe2746
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Mon Sep 21 15:34:52 2015 +0800

    Merge pull request #346 from iamjasonp/beta7-packages-2
    
    Update System.ServiceModel.* packages to beta7 references

[33mcommit 5fe2746ce8f56f13de5d0035e54a50011b450975[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Mon Sep 21 14:39:35 2015 +0800

    Update test project.json and project.lock.json for beta7 packages
    
    Fixes #331

[33mcommit bf0fe66ccbe320b7f3ff91f42621e8537e803618[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Mon Sep 21 14:38:44 2015 +0800

    Update System.Private.ServiceModel project.json for beta7
    
    Fixes #331

[33mcommit 237d56ed3ee0adacf11d7c21a8ac93518bff44a6[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Mon Sep 21 12:11:27 2015 +0800

    Update build tools to 00092

[33mcommit b93b9d4364770ffa2d9e1563f1d0f6b8aa5ce3cf[m
Merge: 81e95be cf4aea9
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Sep 18 09:31:57 2015 -0700

    Merge pull request #318 from StephenBonikowsky/Issue315
    
    Fix for Issue #315

[33mcommit 81e95be1b4fb0a811c5085bc9676a854185791a7[m
Merge: 240aeb4 45b4afe
Author: KKhurin <kkhurin@microsoft.com>
Date:   Thu Sep 17 21:16:24 2015 -0700

    Merge pull request #340 from KKhurin/stresstests
    
    Updating stress tests

[33mcommit cf4aea9858f1b4a58f3f7b406856e5d41022e971[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Sep 4 14:14:23 2015 -0700

    Fix for Issue #315
    
    Fixes #315

[33mcommit 45b4afe651df9ea7bcd74380ed57c0e62b3b152f[m
Author: Konst Khurin <kkhurin@microsoft.com>
Date:   Wed Sep 16 22:33:45 2015 -0700

    Updating stress tests
    - added more stress scenarios
    - added support for duplex
    - added support for streaming

[33mcommit 240aeb46e638916d932d66d3ee920a24765a1a2f[m
Merge: 08bfbf9 f7ab7e3
Author: Wes Haggard <weshaggard@users.noreply.github.com>
Date:   Wed Sep 16 20:07:26 2015 -0700

    Merge pull request #339 from dotnet-bot/from-tfs
    
    Merge changes from TFS

[33mcommit f7ab7e362937af79ef52ef454633a8099a646490[m
Author: Wes Haggard <Wes.Haggard@microsoft.com>
Date:   Wed Sep 16 19:56:33 2015 -0700

    Update project.json files to use xunit 2.1.0-rc1
    
    Updates all the lock files to account for the DNU beta7 changes
    and the xunit update.

[33mcommit a09ded384ea715b17dabc491ec068c9c22213b6b[m
Author: Wes Haggard <Wes.Haggard@microsoft.com>
Date:   Wed Sep 16 18:04:32 2015 -0700

    Fix build issue by upgrading wcfopen to DNU beta7 and build tools 89.
    
    [tfs-changeset: 1525997]

[33mcommit 64438d76f2ea060cd6b8cae844376d08490a9b34[m
Author: Shin Mao <shmao@microsoft.com>
Date:   Wed Sep 16 15:21:27 2015 -0700

    221,222:Used new XmlDictionaryWriter Async Methods

[33mcommit 7cb524ca38805bc9e03941ae47f1c792aefad8e6[m
Author: Shin Mao <shmao@microsoft.com>
Date:   Wed Sep 16 15:10:34 2015 -0700

    218:Added OperationFormatter.SerializeBodyAsync.

[33mcommit 08bfbf9d7c039619248903b1769b801a55e6105d[m
Merge: 9a47ac0 26c08b1
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Sep 16 08:27:14 2015 -0700

    Merge pull request #337 from roncain/bridge-config
    
    Set BridgeResourceFolder at Bridge startup

[33mcommit 26c08b12b67924ff771bd390b2da8eb491d1fe23[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Sep 16 07:26:33 2015 -0700

    Improved Bridge.exe console output to aid debugging

[33mcommit cbd1bef6f6ed95799c542583b1894618cb5e960a[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Sep 15 13:43:09 2015 -0700

    Set BridgeResourceFolder at Bridge startup
    
    Prior to this change, the BridgeResourceFolder was specified by
    BridgeClient when tests ran.  However, this was not a valid
    strategy for x-platform use, otherwise both client and Bridge
    would need to share a x-OS folder.
    
    With this change, Bridge.exe is required to have a BridgeResourceFolder
    when it is asked to start, and the BridgeClient no longer alters that.
    
    Also adds a 'reset' option to Bridge.exe so that when tests complete
    the Bridge can be asked to release all remote resources.  They will
    be reacquired the next time Bridge requests are made.
    
    Also move the Http DELETE handling to the ResourceController where it
    really belonged.  An Http DELETE to the /resource endpoint releases all
    resources.
    
    Fixes #334

[33mcommit 9a47ac0709d5234f8355477de989bc18008d5aef[m
Merge: 3e64ac3 de692b6
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Sep 15 07:09:39 2015 -0700

    Merge pull request #332 from roncain/x-plat
    
    Create script to run WCF tests in Linux

[33mcommit de692b603b3bf1625af12f052b69866b698a8549[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Sep 15 07:01:22 2015 -0700

    Remove code coverage section of run-test.sh
    
    This section was copied from CoreFx run-test.sh and is
    not currently ready for WCF.

[33mcommit c962b41641683362be683c32eb66eae2ecb822bb[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Sep 10 12:35:51 2015 -0700

    Create script to run WCF tests in Linux
    
    The run-test.sh script is based heavily on the CoreFx version
    and adds the additional logic to overlay WCF binaries.
    
    The cross-platform-testing.md is also based heavily on the CoreFx
    version and is updated for WCF.
    
    Using the steps in cross-platform-testing.md, most WCF unit tests
    pass in Linux.

[33mcommit 3e64ac3a6b359fb3d7906817d77a0a3ea90511c7[m
Merge: 10af38d b733598
Author: Matt Connew <mconnew@microsoft.com>
Date:   Tue Sep 8 17:44:57 2015 -0700

    Merge pull request #330 from mconnew/FixNetNativeBuild
    
    Adding back System.Net.Primitives dependency

[33mcommit 10af38d7d62a88eb679b7690e0f64f5665d44ab8[m
Merge: e4873bc d6f8805
Author: Dustin Metzgar <dmetzgar@users.noreply.github.com>
Date:   Tue Sep 8 17:20:54 2015 -0700

    Merge pull request #329 from dmetzgar/wcfeventsource_fix
    
    Fix to the WCF event source that will alleviate the problems with ToF CHK tests

[33mcommit b73359843309f2faba0198382014ce27e7ae0c82[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Tue Sep 8 15:19:19 2015 -0700

    Adding back System.Net.Primitives dependency
    
    This broke the internal .Net native build

[33mcommit d6f880597cbb58735ccc922dc310142c3f2bfd1b[m
Author: dmetzgar <dmetzgar@microsoft.com>
Date:   Tue Sep 8 15:00:23 2015 -0700

    Fix to the WCF event source that will alleviate the problems with ToF CHK tests

[33mcommit e4873bcc276b37898ff861620079b2af95d6c2c8[m
Merge: 8b9c3bc 34d2d6f
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Wed Sep 9 01:11:56 2015 +0800

    Merge pull request #316 from iamjasonp/client-cert
    
    Change certificates to support net.tcp scenarios

[33mcommit 34d2d6f08eb71f87979f7a9af99f5a75d1aaae11[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Tue Sep 8 18:19:56 2015 +0800

    Change X509CertificateClaimSet to check Subject Name
    
    On Desktop .NET <= 4.6 checks against the Subject (CN=hostname) rather than
    checking against the Subject Alternative Name. We should revert back to the 4.6
    functionality so that we're consistent with the current desktop behaviour
    once we're ready for it.

[33mcommit 001c09c50f4c091d1c0c7e8168626c515214c3ac[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Fri Sep 4 18:24:07 2015 +0800

    Change certificates to support net.tcp scenarios
    
    Renamed cert private keys/public cert files to be less confusing
    Updated README.txt to README.md

[33mcommit 8b9c3bc1ae2c17fd5dc7f53632c66aa11b165a7c[m
Merge: 97b018b cc7b937
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Wed Sep 9 00:55:05 2015 +0800

    Merge pull request #319 from iamjasonp/updateprojectjson
    
    Update build tools and clean project.json in tests

[33mcommit cc7b9372b53c536196f5f59e6da5e9492fb5a7b0[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Wed Sep 9 00:31:34 2015 +0800

    Update build tools version to 1.0.25-prerelease-00083

[33mcommit 7a13d7b4d8ad5847ed217429ba9a30c055fd648f[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Sep 3 18:07:27 2015 +0800

    Update build tools and clean project.json in tests
    
    Fixes #196

[33mcommit 97b018b035f8865436faf01293390d68424bb60d[m
Merge: f294b07 857f68b
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Mon Sep 7 14:55:16 2015 +0800

    Merge pull request #317 from StephenBonikowsky/FixIssue301
    
    Fix and re-enable disabled tests due to Issue #301

[33mcommit 857f68b92d90605b8717feaafa423855cbd92537[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Sep 4 10:41:10 2015 -0700

    Fix and re-enable disabled tests due to Issue #301
    
    Fixes #301

[33mcommit f294b07178258d9dacd5cdcc00f2c7e383486009[m
Merge: 7255151 780d476
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Fri Sep 4 12:25:19 2015 +0800

    Merge pull request #299 from StephenBonikowsky/Issue280
    
    Adding IOperationBehavior unit test

[33mcommit 780d4762765c9bf87d89dda23362b1073923f291[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Aug 28 17:04:53 2015 -0700

    Adding IOperationBehavior unit test
    
    * Includes changes based on initial PR feedback
    * Cleanup a few interfaces no longer needed in unit common code
    * Moved WcfDuplexServiceCallback to common code so it could be re-used
    * Fixes #280

[33mcommit 72551514e76d40f1c3de7436c34a5203a357ee78[m
Merge: 439448b 3609b82
Author: Hong Dai <hongdai@microsoft.com>
Date:   Wed Sep 2 14:11:31 2015 -0700

    Merge pull request #298 from hongdai/issue234
    
    Enable Ctor for DnsEndpointIdentity

[33mcommit 3609b82f8a0d401b16bb1f6473e700dc87c79ed9[m
Author: Hong Dai <hongdai@microsoft.com>
Date:   Wed Sep 2 11:29:56 2015 -0700

    Enable Ctor for DnsEndpointIdentity
    * Add unit tests.

[33mcommit 439448b37b833f01e9cf5b4d744ace0e7e1dbe73[m
Merge: c674a2a 56a4255
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Sep 2 11:00:14 2015 -0700

    Merge pull request #311 from iamjasonp/TransportSecurity-PR
    
    Add transport security support for net.tcp

[33mcommit 56a425519cf9e05a573b8ec849e2f8e7ee3851a8[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Wed Sep 2 17:57:42 2015 +0800

    Add transport security support for net.tcp
    
    * Add support for SecurityMode.Transport in net.tcp
    * Add in support for client X509Certificates for WCF Core
    * Add StreamSecurityBindingElement
    * Add SslStreamSecurity and supporting components
    * Add WindowsStreamSecurity stubs
    * Import IdentityModel code around X509 cert, tokens
    * Add support for DnsEndpointIdentity
    * Disable failing tests due to WindowsStreamSecurity being stubbed now (#301)
    * Cleanup of Tcp ClientCredentialType tests
    * Update project.lock.json files for new dependencies
    
    Fixes #9, #12, #81

[33mcommit c674a2ae1fd4dbd9b2ecafec0924cd5df2daf9e2[m
Merge: e299bba e56de4b
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Aug 31 12:05:51 2015 -0700

    Merge pull request #294 from StephenBonikowsky/Issue250and251
    
    Adding test cases to resolve Issues 250 and 251

[33mcommit e56de4b67e861a46037e7d812686b4544abbb165[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Aug 26 11:48:52 2015 -0700

    Adding test cases to resolve Issues 250 and 251
    
    * Including changes based on feedback
    * Fixes #250 and Fixes #251

[33mcommit e299bba3cdde892f6f38a57fb3d6edfcd4888122[m
Merge: 54a9d0c 3b72d45
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Aug 31 08:47:44 2015 -0700

    Merge pull request #297 from roncain/localhost-cert-fix
    
    Fixes issue matching wrong localhost certificate

[33mcommit 3b72d45509ad7ba6ef5790d0be94461e55bedbef[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Aug 31 07:03:18 2015 -0700

    Improve CertificateManager certificate matching
    
    Now matches by thumbprint to avoid issues when other
    certificates have the same SubjectName.
    
    Also refactored CertificateManager to simplify matching.

[33mcommit cea76f4873eca89d564fd10e60fc6d289ac60eea[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Aug 28 14:52:13 2015 -0700

    Fixes issue matching wrong localhost certificate
    
    The Bridge CertificateManager was using too simple
    an algorithm to find an existing "localhost" certificate
    and accidentally matched one already installed for IIS.
    This caused the CI machine to fail OuterLoop https tests.
    
    This change tightens the algorithm to locate only a
    "localhost" certificate with a known issuer.

[33mcommit 54a9d0c882ed6b616e3dabc57c82e7897e9abe66[m
Merge: 9524a00 87ddfcb
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Aug 28 13:08:09 2015 -0700

    Merge pull request #296 from roncain/fix-server2012-use
    
    Fix server 2012 use

[33mcommit 87ddfcbb61a2db012ceb02ea544b81545e03e320[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Aug 28 11:51:25 2015 -0700

    Allow Resource AppDomains to shutdown cleanly.
    
    When the Bridge exits, it now explicitly shuts down
    all AppDomains created for its resources.  This allows
    them to release the certificates and ports they acquired.
    
    Without this fix, the certs remain in the cert store and
    netsh shows the https SSL cert is still assigned to the
    https tests' port.

[33mcommit 56ece9d63ddf8762e59cf413088e71a14715246d[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Aug 28 10:44:23 2015 -0700

    Fixes OuterLoop test failure on Server 2012 machines
    
    The certificates needed to be regenerated with slightly
    different steps to ensure the exported files were written
    in ASCII.
    
    The CertificateManager also avoids loading certificates already
    loaded.

[33mcommit 9524a0015a31caadfdaf0630d7e01f248d57e439[m
Merge: 85ba4eb 95f8893
Author: Dustin Metzgar <dmetzgar@users.noreply.github.com>
Date:   Thu Aug 27 11:57:33 2015 -0700

    Merge pull request #293 from dmetzgar/remove_unused_events
    
    Removed unused events

[33mcommit 95f88935f3a17d1f40ee063a436600e0eb0de08a[m
Author: dmetzgar <dmetzgar@microsoft.com>
Date:   Wed Aug 26 12:53:42 2015 -0700

    Removed EventSource events that are not currently being used in the source code to handle manifest creation performance problems.

[33mcommit 85ba4eb1d06f933da2be7c30dcd81304b04e5f71[m
Merge: 9cf85ca cdeccaa
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Aug 25 06:34:04 2015 -0700

    Merge pull request #290 from roncain/bridge-as-admin
    
    Bridge as admin

[33mcommit cdeccaaa45a9991b737016d9b58962911bfe4dbd[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Aug 25 06:22:41 2015 -0700

    Bridge.exe runs elevated and controls all Bridge commands
    
    Bridge.exe now has an app manifest that allows it to run
    as admin (UAC prompting if appropriate).
    
    The Bridge.exe command line options have now been improved
    to match  the names in TestProperties and BridgeConfiguration.
    And it is possible to give Bridge.exe a .json file to initialize
    multiple Bridge options.
    
    The prior certificates used for https tests were replaced with
    newer versions, and they are installed and uninstalled by the
    Bridge as needed.
    
    All firewall rule and certificate cleanup that used to happen
    in other scripts is now handled by the Bridge itself. And those
    prior scripts have been removed.
    
    The BridgeController was added and a DELETE request to it will
    shutdown the Bridge cleanly.

[33mcommit 9cf85ca8a1d5c9ec07ec63c9b85ec3347eea05c7[m
Author: Dustin Metzgar <dmetzgar@users.noreply.github.com>
Date:   Sun Aug 23 12:40:16 2015 -0700

    Update cross-machine-test-guide.md

[33mcommit e9f0ea354a376a26d5333fc8cd6834895d42f01d[m
Merge: 3918b09 e930047
Author: Dustin Metzgar <dmetzgar@users.noreply.github.com>
Date:   Sun Aug 23 12:28:11 2015 -0700

    Merge pull request #265 from dmetzgar/eventsource
    
    Adding EventSource version of original WCF/WF ETW provider

[33mcommit 3918b095237e29650310bbcdd67715b1dd24595f[m
Merge: 2263a83 48f5031
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Aug 21 06:00:03 2015 -0700

    Merge pull request #288 from roncain/bridge-handles-certs
    
    Enable Bridge certificate configuration

[33mcommit 48f5031d4b6c9337488655df08146d786d2a794c[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Aug 20 12:34:40 2015 -0700

    Enable Bridge certificate configuration
    
    This PR allows the Bridge to install and uninstall certificates
    based on the resources that need them.
    
    Specific changes are:
      - Hard-coded knowledge of specific thumbprints removed
      - New .cer and .pfx files generated for localhost
      - New TestProperties and BridgeConfiguration properties
        to make these certificate names configurable
      - New CertificateManager class to manage installing and uninstalling

[33mcommit e930047bf1c958324f29afd8e7ae72086b9deb1c[m
Author: dmetzgar <dmetzgar@microsoft.com>
Date:   Wed Aug 19 13:01:09 2015 -0700

    Moved hardcoded event ids for event source to constants.

[33mcommit de2389a07a830782aa670d3442e3a8166d8ef5c8[m
Merge: 58ed606 2263a83
Author: dmetzgar <dmetzgar@microsoft.com>
Date:   Wed Aug 19 12:32:38 2015 -0700

    Merge branch 'master' into eventsource

[33mcommit 58ed606ce73faa75466433d57f370ef917399491[m
Author: dmetzgar <dmetzgar@microsoft.com>
Date:   Wed Aug 19 12:30:33 2015 -0700

    Fixed some of the events to have the right tasks. Found an issue when producing the payload for an exception where Exception.StackTrace can sometimes be null.

[33mcommit 2263a831f5bac89bd704695ed70122aa012f617a[m
Merge: 29d2aac a37392e
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Aug 19 06:41:08 2015 -0700

    Merge pull request #284 from roncain/bridge-port-configure
    
    Bridge port configure

[33mcommit a37392efd8d3075f5f76d0a18f6baf9587322144[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Aug 19 06:29:24 2015 -0700

    Clean script no longer requires working folder passed in

[33mcommit 63e3c89c48255a6a12567b4d0e0d402b83bb451e[m
Merge: 29d2aac cc55bc4
Author: dmetzgar <dmetzgar@microsoft.com>
Date:   Tue Aug 18 10:55:46 2015 -0700

    Merge branch 'eventsource' of https://github.com/dmetzgar/wcf into eventsource

[33mcommit d5bc47249e194269e0184bea3001e7be14acfed1[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Aug 18 06:31:05 2015 -0700

    Allow scenario tests to run when Bridge is local but not using localhost
    
    Adds logic to detect that when BridgeHost is not 'localhost' but
    the Bridge actually is running locally, it is acceptable for the
    BridgeResourceFolder to be local.  When the Bridge truly is running
    on a different machine, the BridgeResourceFolder cannot be local.

[33mcommit a1ce2b18b432fbf5882bcffb907a0d10479591cd[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Aug 18 05:33:28 2015 -0700

    Refactor some Bridge EndpointResources
    
    Add websocket port into test properties and Bridge configuration.
    
    Update web socket endpoint resource for new EndpointResource API
    
    Update BasicAuthResource to not use localhost
    
    Put HttpResource, HttpsResource and TcpResource into separate files.

[33mcommit 98ed28c802040ab4dfb918837362b1c6cd6e820d[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Aug 18 04:27:02 2015 -0700

    Make all ports used by the Bridge configurable

[33mcommit 29d2aacb9b2eb69a73f5aaf157b65f029d02f851[m
Merge: 04520b4 33e0f76
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Tue Aug 18 11:46:40 2015 +0800

    Merge pull request #283 from roncain/remote-test-failures
    
    Fixes EndpointNotFound test to work when Bridge is remote

[33mcommit 04520b4db71977cbd7ed2b71c639f2e3f9e7e35e[m
Merge: dda3f90 80c6b6d
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Aug 17 14:39:06 2015 -0700

    Merge pull request #278 from StephenBonikowsky/Issue168
    
    Scenario test helper for proper cleanup of all communication objects …

[33mcommit 80c6b6db9282bab7dc33f45150cf9335ec4c86f8[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Aug 14 10:25:35 2015 -0700

    Scenario test helper for proper cleanup of all communication objects used in tests.
    
    * We really need to close on Issue 168, this is my attempt at a proposal that takes into account everybodies feedback.
    * We should still attempt to Close communication objects in the Try block.
    * We should Close after doing validation, if both the scenario being tested and the call to Close Throw we would want to fix the scenario Throw first as it could be the cause for the Close throwing.
    * The helper method ensures all com objects have been cleaned up regardless of the test results.
    * Fixes #168

[33mcommit dda3f90d994330b2e3d063378337013dc63da433[m
Merge: b985e26 96238b4
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Aug 17 14:28:58 2015 -0700

    Merge pull request #276 from StephenBonikowsky/WebSocketScenarioTests
    
    Adding tests for WebSockets

[33mcommit 96238b4bf63760cb866aa3f100fa39721d95e035[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Aug 12 16:59:35 2015 -0700

    Adding tests for WebSockets

[33mcommit 33e0f76080c084d269f2dff58a72ccb5ff7a64a8[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Aug 17 11:14:57 2015 -0700

    Fixes EndpointNotFound test to work when Bridge is remote
    
    The test was assuming localhost for the host rather than where the
    Bridge was actually running.  It was also possible to remove the
    custom IResource for the endpoint-not-found case and replace it
    with use of a normal running service with a bogus endpoint suffix.

[33mcommit b985e26704b37cb5007d1481f04f9fe569d7d5f6[m
Merge: 48a8099 1b3008e
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Aug 17 08:19:25 2015 -0700

    Merge pull request #279 from roncain/improve-firewall-rules
    
    Improve Bridge firewall support

[33mcommit 1b3008e4c84e369b87ebc93ddcb8da44f5c1b6d2[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Aug 17 08:13:50 2015 -0700

    Add link to new readme to run tests across machine boundaries

[33mcommit ca44ea37429ea25ff26ca3f903a3dcc05eff359e[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Aug 17 07:50:58 2015 -0700

    Modify scripts to not require working directory on command line

[33mcommit 941a87b93ee3e22953a90655651f5cb8078c53b4[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Aug 14 11:19:37 2015 -0700

    Improve Bridge firewall support
    
    These changes allow the firewall rules created by the Bridge
    to be scoped to specific IP's to allow them to be secured against
    unauthorized access.
    
    Remotely starting the Bridge has been made easier with a new script.
    and a new markdown file describes how to run the Bridge remotely.

[33mcommit 48a80998f262e5d8b5b37860453ac29404e74b8e[m
Merge: f75e93b 82b4844
Author: Matt Connew <mconnew@microsoft.com>
Date:   Fri Aug 14 16:18:04 2015 -0700

    Merge pull request #277 from dotnet-bot/from-tfs
    
    Merge changes from TFS

[33mcommit 82b484407e35fbcb8892bbe519f4d95778e342e2[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Fri Aug 14 16:07:56 2015 -0700

    Add workaround for build breakage to create AssemblyInfoFileDir early in the build

[33mcommit d61df0397be929e2de858c76f98a02895c31c414[m
Merge: 2302b81 f75e93b
Author: Matt Connew <mconnew@microsoft.com>
Date:   Fri Aug 14 11:16:55 2015 -0700

    Merge remote-tracking branch 'upstream/master' into from-tfs
    
    Conflicts:
            dir.props
            src/.nuget/packages.Windows_NT.config

[33mcommit f75e93b20d3b9314a0b4952928f6529515f55ce7[m
Merge: 5547159 291e805
Author: Matt Connew <mconnew@microsoft.com>
Date:   Thu Aug 13 17:40:32 2015 -0700

    Merge pull request #272 from roncain/build-tools-70
    
    Update to build tools version 70 to match CoreFx

[33mcommit 2302b81b1d8783796fddea459c7461e00f446d84[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Thu Aug 13 17:07:57 2015 -0700

    Update the version of all assemblies
    
    This change updates the assembly versions of all of the corefx assemblies
    as needed after we shipped stable versions.  Assemblies with API differences
    get a minor version bump.  Assemblies with only bugfixes get a build version
    bump.
    
    In order to facilitate this I had to update the reference assemblies, so I took
    the opportunity to port them all to the open.
    
    [tfs-changeset: 1514419]

[33mcommit 5547159450e85de9b15f64e2240c3edec476db73[m
Merge: e2bb014 4eec867
Author: Hong Dai <hongdai@microsoft.com>
Date:   Thu Aug 13 15:46:51 2015 -0700

    Merge pull request #274 from hongdai/updatetest
    
    Enable NTLM test and Basic Auentication negative test

[33mcommit e2bb014728db01d67ec37a44b8cfcbc04753fb0e[m
Merge: 15b9be6 54e3ccd
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Aug 13 13:12:44 2015 -0700

    Merge pull request #254 from roncain/callbackcontract
    
    Enable duplex callback usage in NET Native

[33mcommit 54e3ccda67c1c1127e8e1b256f6dba3a6429d4b6[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Aug 13 06:46:44 2015 -0700

    Allow reflection for duplex callbackcontract
    
    Adds entries to the NET Native custom rd.xml to allow
    reflection over types named in ServiceContractAttribute's
    CallbackContractType.
    
    Also adds an entry to allow reflection over types of objects
    passed into the InstanceContract constructor.
    
    Fixes #110

[33mcommit 291e805e1ded72470c230dc1fe8e25762c8a94ea[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Aug 13 05:33:52 2015 -0700

    Update to build tools version 70 to match CoreFx

[33mcommit 4eec867d7b6f2765948eb0353c1fac56928caac3[m
Author: Hong Dai <hongdai@microsoft.com>
Date:   Wed Aug 12 17:54:59 2015 -0700

    Enable NTLM test and Basic Auentication negative test
    
    * The related product bug are fixed

[33mcommit 15b9be628edf36035da2b6d44fe8a3dc3c075b11[m
Merge: 1addcbf 36fae94
Author: Shin Mao <shmao@microsoft.com>
Date:   Wed Aug 12 15:34:43 2015 -0700

    Merge pull request #269 from dotnet-bot/from-tfs
    
    Merge changes from TFS

[33mcommit 36fae9427a215bd586533093dcafb954bfa4dd1b[m
Author: dotnet-bot <dotnet-bot@microsoft.com>
Date:   Wed Aug 12 14:57:00 2015 -0700

    [tfs-changeset: 1513845]

[33mcommit 1addcbfc72bb15e447e683fd6f60ee756f787a21[m
Merge: 6278558 c3ed3cb
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Aug 12 14:49:34 2015 -0700

    Merge pull request #262 from roncain/bridge-client-use-remote
    
    Bridge client use remote

[33mcommit c3ed3cb15da45b01e8ac6b03961c649f4fa910a4[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Aug 12 12:35:51 2015 -0700

    Use API's to modify firewall rules rather than processes

[33mcommit f795d19155df11f6a9be5ae03e8a01acb97fe793[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Aug 11 14:08:28 2015 -0700

    Add support for opening and closing ports in firewall dynamically

[33mcommit 1306cf45da79e25c189552ade9c7201b52e072ff[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Aug 11 12:18:38 2015 -0700

    Allow env var to prevent killing Bridge task in OuterLoop

[33mcommit ba4ed434ae222a9d88967653fda44dfcbba9cb4a[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Aug 11 09:53:42 2015 -0700

    Make test timeout configurable to aid debugging

[33mcommit 5cd7e2631cd50d1dc6b5b2a203272183cf4621c6[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Aug 11 08:58:06 2015 -0700

    Create Bridge.Build.Tasks assembly for custom tasks
    
    The initial custom MSBuild task invokes the Bridge to
    release all its resources.  This is necessary so a rebuild
    of the resources can copy them into the BridgeResourceFolder.

[33mcommit 566a237f991af95844728e0b4c97c1d19834d714[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Aug 11 06:54:38 2015 -0700

    Allow Bridge resource folder to be unloaded/reloaded
    
    These changes expose a DELETE verb on the ConfigController
    to release any resources it has, such as the AppDomain for
    the loaded resources.
    
    It also exposes a GET verb to query what resources are loaded.

[33mcommit 6278558c61982e367a5069f09c26693d09683368[m
Merge: 808beff ef5a1c5
Author: Matt Connew <mconnew@microsoft.com>
Date:   Wed Aug 12 13:22:38 2015 -0700

    Merge branch 'dotnet-bot-from-tfs'

[33mcommit ef5a1c5069f4d6e9215cdcb2c1b39b16c4342819[m
Merge: 808beff 212137e
Author: Matt Connew <mconnew@microsoft.com>
Date:   Wed Aug 12 13:22:03 2015 -0700

    Merge branch 'from-tfs' of git://github.com/dotnet-bot/wcf into dotnet-bot-from-tfs
    
    Conflicts:
            src/System.Private.ServiceModel/src/System/ServiceModel/ClientBase.cs

[33mcommit 808beff436f8af279fbf41dbdc730b8a0c0ee287[m
Merge: aa55555 cc62a74
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Aug 12 11:06:00 2015 -0700

    Merge pull request #266 from StephenBonikowsky/FixMergePR264
    
    Fixing build break caused by PR #252

[33mcommit cc62a74258eb985bc1353de23af3e09c1edf0c78[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Aug 12 10:17:10 2015 -0700

    Fixing build break caused by PR #252
    
    * In cleaning up ClientBase<TChannel> I removed two properties that were in the public contract. Adding them back.

[33mcommit cc55bc4ea28407cc35e4b2e7d73d69513da14d17[m
Author: Dustin Metzgar <dmetzgar@microsoft.com>
Date:   Wed Aug 12 06:31:56 2015 +0000

    Adding EventSource version of original WCF/WF ETW provider

[33mcommit 212137e6d3a1429e62b5fb9bdff3fae5a8643f4a[m
Author: Wes Haggard <Wes.Haggard@microsoft.com>
Date:   Tue Aug 11 21:11:10 2015 -0700

    Fix compat break by adding back two properties that exist in the S.SM.Primitives contract.
    
    [tfs-changeset: 1513519]

[33mcommit aa55555621e871017392af412a2d83697c22e26d[m
Merge: 6d36f24 265ae24
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Aug 11 14:20:48 2015 -0700

    Merge pull request #252 from StephenBonikowsky/Issue240
    
    Removing dead code associated with ClientBase<TChannel>

[33mcommit 6d36f24268333bc8cc34f1777dfb2d6a18fde07b[m
Merge: 20c1d17 b0f3e0b
Author: Shin Mao <shmao@microsoft.com>
Date:   Tue Aug 11 13:33:07 2015 -0700

    Merge pull request #258 from shmao/issue223
    
    Tests for Streamed Mode in SynchronizationContext

[33mcommit 20c1d17c909211e696cc6539edaacd67993edff8[m
Merge: f2fb369 883215e
Author: Hong Dai <hongdai@microsoft.com>
Date:   Tue Aug 11 11:53:08 2015 -0700

    Merge pull request #260 from hongdai/issue224
    
    Check null/empty user name

[33mcommit f2fb36956a5665d4c2bb34dcd3f78e1842aa3108[m
Merge: 6c31a8f 2633012
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Aug 11 10:11:36 2015 -0700

    Merge pull request #261 from StephenBonikowsky/CleanupXD.csFile
    
    Cleaning up XD class of static xml dictionaries not being used.

[33mcommit 263301282bac5d4d2e139003b0220869305703ab[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Aug 10 16:30:36 2015 -0700

    Cleaning up XD class of static xml dictionaries not being used.
    
    * In the past an xd.xml files was used to generate this cs file but that is no longer used.

[33mcommit 265ae24f59e61317fd3c3d908e1b2d83f2f3b02f[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Aug 10 13:28:11 2015 -0700

    Updating PR #252 with review feedback.
    
    * Updated NotSupportedExceptions to PlatformNotSupportedExceptions
    * Removed instances of "this" where not needed.
    * Removed EndpointTrait<TChannel> as it is no longer needed.
    * Updated all references to GetChannelFactory() to use the field directly, and removed the method.

[33mcommit 831976e8fe69e7dbf19daddda25b14a6e920dc74[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Aug 4 15:41:58 2015 -0700

    Removing dead code associated with ClientBase<TChannel>
    
    * Some constructors in DuplexClientBase<TChannel> and ClientBase<TChannel> are either not in contract or are in contract but are intended to be used in connection with Configuration files which is not a supported feature.
    * Cleaned up a lot more code in ClientBase<TChannel> related to caching of ChannelFactorys which is not used.
    * With the cleanup of DuplexClientBase<TChannel> and ClientBase<TChannel> several files were able to be completely removed.
    * Fixes #240

[33mcommit 6c31a8f062d694ba9b202f83d2892a5ddaab530f[m
Merge: 60cddc2 e5b4f90
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Aug 10 12:49:38 2015 -0700

    Merge pull request #259 from roncain/bridge-x-machine
    
    Bridge x machine

[33mcommit 883215e79ea0e23b257abd46f4f6b45eee076d2e[m
Author: Hong Dai <hongdai@microsoft.com>
Date:   Mon Aug 10 10:26:27 2015 -0700

    Check null/empty user name
    * Throw the same exception type and message as desktop
    in case of null/empty user name
    
    Fix issue #224

[33mcommit b0f3e0b0696c76690c50d5968193be04a720ae56[m
Author: Shin Mao <shmao@microsoft.com>
Date:   Thu Aug 6 16:58:09 2015 -0700

    Tests for Streamed Mode in SynchronizationContext

[33mcommit 60cddc275739e626b4bf20368c74365a4161b7f7[m
Merge: d6cb124 b6bf47a
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Aug 7 10:03:57 2015 -0700

    Merge pull request #256 from mconnew/package-updates
    
    Moving to xunit 2.1

[33mcommit e5b4f90804db34b2d20a0f6052d4af96b30c4d53[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Aug 7 08:33:21 2015 -0700

    Allow Bridge environment variables to configure Bridge use

[33mcommit 113b8fc11a7168156e92c9c5d1e1f0ef77b13890[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Aug 7 08:08:01 2015 -0700

    Separate Bridge URL configuration into host and port.

[33mcommit 170350e9ed608a71566dd98f6655e8fccf08137c[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Aug 6 14:16:34 2015 -0700

    Allow Bridge URL to be other than localHost
    
    Allows the Bridge to be started eith remote access enabled
    or disabled.
    
    Also updates the launch scripts to allow port, host, and allowRemote
    options to be passed through to Bridge.exe
    
    Fixes #248

[33mcommit d6cb124c59bd13ce036463994fd3b30b5a95b2ab[m
Merge: 234722b c1d327f
Author: Matt Connew <mconnew@microsoft.com>
Date:   Thu Aug 6 13:53:14 2015 -0700

    Merge pull request #249 from Maxwe11/closure-allocations
    
    Eliminate closure allocations
    
    Fixes a typo where a callback defined outside a delegate was referenced instead of the same callback passed
    in as a state object. This would cause a closure allocation on every call to capture the callback instead of
    using the state.

[33mcommit b6bf47aa8c2500877487cb9b18c01dcb0851f9b1[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Thu Aug 6 13:06:05 2015 -0700

    Moving to xunit 2.1

[33mcommit 234722b730a882959cfee72de240ad6b792364ae[m
Merge: 64182a8 c0dfc41
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Aug 6 10:01:31 2015 -0700

    Merge pull request #255 from StephenBonikowsky/dotnet-bot-from-tfs
    
    Fixing OuterLoop tests broken by PR #244

[33mcommit c0dfc41175dc1587f1dd47dac22ce28a9ad1336e[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Aug 6 09:39:25 2015 -0700

    Fixing OuterLoop tests broken by PR #244

[33mcommit 64182a8a3f7bac6b67b5bde998d70f92d14c71a2[m
Merge: e77dd51 e998ec8
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Aug 6 09:38:48 2015 -0700

    Merge pull request #244 from dotnet-bot/from-tfs
    
    Merge changes from TFS

[33mcommit e77dd51be91af02596731a5a8bbb238e31ad35db[m
Merge: 38268eb 15e6b62
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Aug 6 08:28:20 2015 -0700

    Merge pull request #253 from StephenBonikowsky/CleanupConfigResourceStrings
    
    Cleaning up ServiceModel string resources.

[33mcommit 15e6b62fa8bcf9684d544ecadcc5abaf23c065e8[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Aug 5 14:44:30 2015 -0700

    Cleaning up ServiceModel string resources.
    * Removed resource strings related to configuration files.
    * Removed resource strings related to reliable messaging and reliable sessions.

[33mcommit 38268eb9eb853c512943fbe7491944b0f413c48b[m
Merge: f5f1986 00e6202
Author: Hong Dai <hongdai@microsoft.com>
Date:   Tue Aug 4 11:37:07 2015 -0700

    Merge pull request #247 from hongdai/dns
    
    Verify Server Dns for NetTcp binding

[33mcommit 00e62020d2329f716542b22468c4e2fe23ff9cea[m
Author: Hong Dai <hongdai@microsoft.com>
Date:   Tue Aug 4 11:28:41 2015 -0700

    Verify Server Dns for NetTcp binding
    * This is one of the key scenarios for DnsEndpointIdentity
    * Note that it will fail until all dependent features mentioned in the test resolved.

[33mcommit f5f198685a661c64a11763e9a8a701d5cb7dc6ed[m
Merge: 84ebbad c1cddfe
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Aug 4 08:10:05 2015 -0700

    Merge pull request #246 from roncain/bridge-lock
    
    Bridge lock

[33mcommit c1cddfe6d4d56ef86d1431cd260d8072f946b786[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Aug 4 07:53:05 2015 -0700

    Make all Traces include time

[33mcommit c1d327fc49b589b90b71335740d103a5a5419121[m
Author: Dmitry Turin <turin.dmytro@gmail.com>
Date:   Tue Aug 4 17:28:54 2015 +0300

    Eliminate closure allocations

[33mcommit 84ebbade7d7767aeb5c15c223cc3cadf7ff68b6e[m
Merge: d25bce2 cdf6b89
Author: Matt Connew <mconnew@microsoft.com>
Date:   Mon Aug 3 15:45:34 2015 -0700

    Merge pull request #241 from mconnew/Issue166
    
    Add additional logging to help diagnose Issue #166

[33mcommit cdf6b89b659d563f69d07cb7302eb8c772491bb4[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Fri Jul 31 13:31:05 2015 -0700

    Add additional logging to help diagnose Issue #166

[33mcommit d25bce2106eb7e29acfbac5d1f4cc8e646d2d565[m
Merge: 7dae4e4 1938ae8
Author: Matt Connew <mconnew@microsoft.com>
Date:   Mon Aug 3 14:47:55 2015 -0700

    Merge pull request #245 from mconnew/Issue225
    
    Cleanup string resources
    
    Remove string resource usage for ToString format strings.
    Removed string resources for Peer and NamedPipe transports which aren't available in coreclr.

[33mcommit 1e75965c6b0efbfded18179976c47ed02ce96009[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Aug 3 13:20:22 2015 -0700

    Use short date format for Trace messages

[33mcommit e998ec8441f9b15c04a68b8d913cc187a50e1401[m
Author: Eric St. John <Eric.St.John@microsoft.com>
Date:   Mon Aug 3 12:19:06 2015 -0700

    Move System.ServiceModel tests back to pre-release versions temporarily to fix missing dependenciess
    
    The stable versioned packages omit dependencies on some contractrs since those are still pre-release
    but SerivceModel had to be marked stable for UWP.
        <PackageDependencyExclude Include="System.Net.Security" />
        <PackageDependencyExclude Include="System.Net.NameResolution" />
        <PackageDependencyExclude Include="System.Security.Principal.Windows" />
    These missing dependencies were causing tests to fail.
    
    Rather than just list the missing dependencies in the tests I am moving these tests back to
    pre-release packages for the System.ServiceModel contracts, since System.ServiceModel packages
    are under active developmentit is better to have the tests running against latest than the stable bits.
    
    Once we have newer versions published we'll update these tests to use those.

[33mcommit 706358a79a7490f5345c211389be65870764796a[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Aug 3 12:15:02 2015 -0700

    Disallow concurrent Bridge configuration or resource changes
    
    This change introduces a monitor the Bridge locks during configuration
    changes or resource instantiation.  Concurrent xunit test runs have
    shown sporadic race conditions when these are allowed to run concurrently.

[33mcommit 1938ae8ea04d1a4af30785facf1db6a4bb4d461e[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Fri Jul 31 16:54:43 2015 -0700

    Cleanup string resources
    
    We were using string resources for storing string formats used in ToString() methods. This
    meant on .Net Native release builds, the ToString() outputs would have the wrong value as
    the string resource values are stripped in .Net Native release builds.
    Also removed a lot of unused string resources which were errornously copied from .Net on desktop.

[33mcommit 370374b9d484f86f6bd4e29cc8ed4a05912e5be4[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Fri Jul 31 15:47:51 2015 -0700

    Use stable package dependencies and move our packages to post-stable versions
    
    This change modifies the build to consume the stable version where available.
    
    This disables building the dowlevel packages since we shipped them once as stable and we're done.
    From here on out, we'll only build the latest version (except for a servicing event, which would
    be handled by syncing to a hash).
    
    I can't rev the package versions in the normal way yet (by modifying the assembly version of the contents)
    because that turned out to be too big of a problem to tackle in this change. Instead I added a
    parameter to the task that calculates package versions to do the rev for anything that was
    stable, but only when building the package itself, not when calculating the dependencies.
    This has the downside of not permitting a dependency on a incremented pre-release version if we've
    shipped a stable version, but that is a small point in time problem until I can rev the assembly
    versions.
    
    [tfs-changeset: 1508664]

[33mcommit 7dae4e45921f6d470f8591c7ad7c6e0dc1b86e25[m
Merge: bd6720f 3eebf03
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Jul 31 06:35:15 2015 -0700

    Merge pull request #238 from roncain/bridge-timeout
    
    Make Bridge idle timeout configurable

[33mcommit 3eebf032638848ccf479a921ad600d84bbd69a57[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Jul 31 06:09:00 2015 -0700

    Notify listeners of individual config value changes

[33mcommit 036f911105e8006816c63bb7248770c61fd1ec4d[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Jul 30 13:31:31 2015 -0700

    Incorporate BridgeClient improvements

[33mcommit 58cc9f25370da36a21cfe2814eadd03a84f2d010[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Jul 30 07:26:14 2015 -0700

    Make Bridge idle timeout configurable
    
    This PR allows the Bridge to accept an
    idle timeout as one of the properties in
    the POST config name/value pairs.  And it
    sets the idle timeout manager to use that
    as its new idle timeout between requests.
    
    To avoid making the Bridge aware of all its
    configuration sensitive types, this PR also adds
    a new EventHandler for configuration changes.
    Types that care about configuration changes register
    to be notified of the change.
    
    The ConfigurationExtensions class was updated to
    use this same notification mechanism.  And it was
    renamed to reflect its true purpose -- AppDomainManager.
    
    Fixes #198

[33mcommit bd6720fa54c8687a5005436eee81b2c64611a393[m
Merge: b81220b a90cd0e
Author: Hong Dai <hongdai@microsoft.com>
Date:   Thu Jul 30 14:49:09 2015 -0700

    Merge pull request #239 from hongdai/addtrace
    
    Add HResult trace to help identify intermittent test failure

[33mcommit a90cd0e26ae77a3812494957f1217e4f5a9baad6[m
Author: Hong Dai <hongdai@microsoft.com>
Date:   Thu Jul 30 13:36:23 2015 -0700

    Add HResult trace to help identify intermittent test failure
    * Helpful to pinpoint issue #237

[33mcommit b81220b11ee816dc68e5e51c2db5b0745101ac7f[m
Merge: cd7ee57 803508e
Author: Hong Dai <hongdai@microsoft.com>
Date:   Thu Jul 30 13:38:09 2015 -0700

    Merge pull request #233 from hongdai/issue88
    
    Add Test Coverage

[33mcommit cd7ee57cd6d735aac4b59dcf73fe8eaa855d123c[m
Merge: 3454caa 487d844
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Jul 30 11:52:53 2015 -0700

    Merge pull request #235 from StephenBonikowsky/Issue140
    
    Pull request for Issue 140

[33mcommit 487d84481c7e219338f9e49032052af598da6d70[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Jul 30 11:04:49 2015 -0700

    All PR Feedback for Issue 140
    
    * Deleted all packages.config files except the one under Bridge
    * Removed the reference to System.Net.Sockets from the .json files.
    * Edited the projects used by NET Native.
    * Fixed one project name
    * Added a new line at the end of Infrastructure.Common.csproj
    * Fixes #Issue140

[33mcommit 3454caaab5d5ee29a07b13f3ef68112db088102d[m
Merge: 3428a5d 74253bc
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Jul 29 11:19:21 2015 -0700

    Merge pull request #236 from roncain/bridge-use-url-host
    
    Make Bridge service URLs configurable

[33mcommit 74253bcbe7b1d01971c687150b984582e1c8f7b7[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Jul 29 09:20:38 2015 -0700

    Make Bridge service URLs configurable
    
    This PR makes the Bridge no longer hard-code
    localhost as the host for all services but instead
    forms the URL based on the configuration information
    passed to the Bridge POST config.
    
    To allow this, all IResources PUT verb now accepts
    a context object providing access to the Bridge
    configuration.
    
    Fixes #187

[33mcommit 803508e440d830fd639324fddb3fd0527834741e[m
Author: Hong Dai <hongdai@microsoft.com>
Date:   Tue Jul 28 22:26:41 2015 -0700

    Add Test Coverage
    * Add a test to cover incorrect password.
    * Add a test to cover empty user name
    
    Fix #88

[33mcommit 5ed2602681f0b49c3861408cc6e9e29fcdf689d9[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Jul 28 17:28:25 2015 -0700

    All Edits for Issue 140
    
    * All the projects build and all tests run and pass.

[33mcommit a9558447520391efac0d6cc8a23f1fd808789d12[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Jul 28 09:44:54 2015 -0700

    All New Files for Issue 140
    
    * Added new files where needed, made no edits yet.

[33mcommit 197ecb0c51e21594cfea4bd5aa8961ffb965b1d6[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Jul 28 09:27:54 2015 -0700

    All Moves for Issue 140
    
    * This first commit is to move all existing common code files into their new locations.

[33mcommit 3428a5d76b64829e0ba750e22c1f99a05e9de9c4[m
Merge: 6c13572 bd1b93e
Author: Matt Connew <mconnew@microsoft.com>
Date:   Tue Jul 28 12:09:59 2015 -0700

    Merge pull request #216 from mconnew/CallbackFaults
    
    Fixing callback exception handling and adding tests

[33mcommit bd1b93eed857d2f44ad04a44bb9f11f274e969da[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Wed Jul 22 11:26:49 2015 -0700

    Fix dependency on System.Net.Sockets to be only for dnxcore50
    
    .Net Native doesn't use System.Net.Sockets but the dependency was listed as a general dependency. We
    depend on the 4.0.10 contract, but .Net Native only supports 4.0.0. The build system downgraded the version
    to 4.0.0, which isn't actually available for dnxcore50. This meant when building the tests, System.Net.Sockets
    wasn't copied across as there wasn't a matching version. We had previously worked around this by explicitly
    adding that dependency to the tests. This change corrects the dependency declaration for System.Private.ServiceModel
    and removes the explicit test dependencies.

[33mcommit e9919af1789a99917a6650be5547c67a7c077865[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Wed Jul 22 12:54:00 2015 -0700

    Fixed duplex fault test which was using mismatched Action and added new throwing variation
    
    There are two ways for an async method to result in an exception. Either the method call throws
    an exception directly, or it returns a faulted Task containing an exception. These two mechanisms
    cause different behavior for the caller when the caller isn't using await.

[33mcommit 245684f6555bf8bbb6fda695f90acc897a847cf7[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Tue Jul 21 09:43:53 2015 -0700

    Fix exception handling in TaskMethodInvoker and work around DataContractSerializer bug
    
    The returned task from the invoker delegate might be faulted, or the task it contains might be faulted.
    We need to check both tasks for an exception. Also, there is a bug introduced in the DataContractSerializer
    where it creates XmlDictionaryStrings with null IXmlDictionary's when passed the root name and namespace
    as strings. We need to use a different overload or wait for the bug to be fixed. I've changed which
    overload we use.

[33mcommit 6c13572a167aafcb0000426ba1f06e988a53fae7[m
Merge: fc43339 9d43787
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Jul 28 08:52:39 2015 -0700

    Merge pull request #230 from roncain/move-selfhostserver
    
    Move SelfHostWcfService under tools test folder

[33mcommit 9d43787a7e324ac27299f6676e2741e5acdd2fd1[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Jul 28 08:15:06 2015 -0700

    Move SelfHostWcfService under tools test folder
    
    PR #226 created a folder for tools related to tests
    and moved the Bridge project into it.  This PR moves
    the SelfHostWcfService into this same folder in
    preparation for keeping tools and infrastructure out
    of the folders meant to contain only tests.
    
    It also silences 3 build warnings related to fields
    that are not assigned to statically.

[33mcommit fc433392d997236325e292b7e64b07fc8137d5a2[m
Merge: 3534d5c 57466fe
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Jul 28 07:05:27 2015 -0700

    Merge pull request #226 from roncain/bridge-to-test-tools
    
    Move Bridge and setupfiles under tools folder

[33mcommit 57466fe33379bccad351b7e81387033464581226[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Jul 28 05:47:01 2015 -0700

    Move Bridge project under tools folder
    
    This PR moves the Bridge project under a new
     tools\test folder, and it moves the setupfiles
     folder under the new tools\setupfiles.
    
    It also fixes all relative project and file
     paths to reflect the new locations.
    
    Fixes #181

[33mcommit 3534d5cd92efe4ea6ced5d0e627b6562e1ab9ecc[m
Merge: 88d5ae5 3304d68
Author: Shin Mao <shmao@microsoft.com>
Date:   Fri Jul 24 14:18:07 2015 -0700

    Merge pull request #209 from shmao/issue37
    
    Support TransferMode.Streamed.

[33mcommit 3304d68da8bf759da7c4b4386a6d459736589402[m
Author: Shin Mao <shmao@microsoft.com>
Date:   Thu Jul 23 22:54:54 2015 -0700

    Support TransferMode.Streamed.

[33mcommit 88d5ae5a453dd61cb895e2c869b191b68d253a24[m
Merge: 96bd0fb 5dbd317
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Jul 24 09:59:49 2015 -0700

    Merge pull request #220 from roncain/stress-lock-files
    
    Modify the stress test project.*.json files

[33mcommit 5dbd317f8262c12be261d56aca360f2d02aba1d5[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Jul 24 09:24:09 2015 -0700

    Modify the stress test project.*.json files
    
    The prior versions of the .json files in
    the stress folder were considered invalid
    and caused a 'dnu restore' which updated
    multiple project.lock.json files during the
    build.
    
    This change changes the project.json file to
    resemble the other project.json's and also
    adds the project.lock.json file produced by
    the dnu restore.
    
    This prevents dnu restore from running during
    the build due to what it considered an invalid
    lock file.

[33mcommit 96bd0fbc5696465ab30e314d376ae44fa913c0a6[m
Merge: e433bce 771489b
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Jul 24 08:40:38 2015 -0700

    Merge pull request #219 from SajayAntony/stressTestRebased
    
    This is the initial commit for the stress tests.

[33mcommit 771489baa89876ecb237624a8dd3203120e1587a[m
Author: Konst Khurin <kkhurin@microsoft.com>
Date:   Tue Jul 21 22:19:46 2015 -0700

    This is the initial commit for the stress tests.
    Updated with the initial PR feedback and re-formatted.

[33mcommit e433bcebc504903018fb7798b024a0e60782b5db[m
Merge: d8cccb2 0d0e626
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Jul 23 06:52:48 2015 -0700

    Merge pull request #215 from roncain/bridge-config
    
    Modify Bridge config POST to accept name/value pairs

[33mcommit d8cccb208dbe1396c03b5667dc9cf46cadb3d141[m
Merge: 44808b7 f284942
Author: Hong Dai <hongdai@microsoft.com>
Date:   Wed Jul 22 18:45:52 2015 -0700

    Merge pull request #208 from hongdai/kerberos
    
    Document how to run Kerberos test

[33mcommit 0d0e62625e0d6b0c508abfffb7a8a11cf315303e[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Jul 22 14:13:23 2015 -0700

    Convert to use of HttpRequestMessage in controllers

[33mcommit f2849428ca1a987e6bc3d221c1c6bbc42acb81ed[m
Author: Hong Dai <hongdai@microsoft.com>
Date:   Wed Jul 22 13:49:34 2015 -0700

    Document how to run Kerberos test
    
    Fix #195

[33mcommit 44808b732060a58d380a348532e1736a89c591ac[m
Merge: 62fa2ed f53260c
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Thu Jul 23 00:33:09 2015 +0800

    Merge pull request #213 from iamjasonp/update-buildtools-63
    
    Update build tools to 1.0.25-prerelease-00063

[33mcommit fbe18c08cbd712a841cebf59c05902474332857f[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Jul 22 07:50:55 2015 -0700

    Modify Bridge config POST to accept name/value pairs
    
    Modifies the Bridge's POST handler to accept a dictionary
    of name/value pairs.  Also modifies the BridgeClient to
    invoke the POST using TestProperties.  This means the
    Bridge is configured with the current values of the
    TestProperties at runtime.

[33mcommit f53260cf227ce4b426c826f334af87b66f4c1e9e[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Wed Jul 22 18:04:51 2015 +0800

    Update build tools to 1.0.25-prerelease-00063

[33mcommit 62fa2ed229c051e8ea24ed78f8bcda15af333d02[m
Merge: a351e59 7cdebbc
Author: Hong Dai <hongdai@microsoft.com>
Date:   Tue Jul 21 14:58:41 2015 -0700

    Merge pull request #205 from hongdai/kerberos
    
    Make self-hosted test service works for Kerbero

[33mcommit 7cdebbc6ede1e03cc0235f50ecc67c6a50565dea[m
Author: Hong Dai <hongdai@microsoft.com>
Date:   Tue Jul 21 09:59:30 2015 -0700

    Make self-hosted test service works for Kerbero
    * Make self-hosted test service works for Kerberos
    * Integrate with Bridge
    * Make the existing test work on single machine
    
    Fix #193

[33mcommit a351e599952d7dad88becdb71dcf70f81c6a1fec[m
Merge: 83d4b26 110cac2
Author: Hong Dai <hongdai@microsoft.com>
Date:   Tue Jul 21 13:24:47 2015 -0700

    Merge pull request #207 from SajayAntony/TestUriBuilder
    
    Test fix to expose all parts of the host listener uri for #205

[33mcommit 110cac2a83c8df562f33682dfbf6eba26adf3007[m
Author: Sajay Antony <sajaya@microsoft.com>
Date:   Tue Jul 21 11:33:38 2015 -0700

    Test fix to expose all parts of the host listener uri for #205

[33mcommit 83d4b266e532ba62cff70361e0ace0b8a3863c5d[m
Merge: 8c0efd0 bd5eea2
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Jul 21 11:03:11 2015 -0700

    Merge pull request #206 from roncain/build-tools-61
    
    Update build tools to version 61

[33mcommit bd5eea222427f93840ea9e6b1074dee0f9750b0e[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Jul 21 10:53:48 2015 -0700

    Update build tools to version 61
    
    Code coverage builds were broken with the previous
    version of build-tools.  Version 61 fixes that break.

[33mcommit 8c0efd052416a58514fa6e94d126ddff7c8c346a[m
Merge: ef0a495 b5e8773
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Jul 21 09:50:37 2015 -0700

    Merge pull request #204 from roncain/fix-codegen-trigger
    
    Trigger TestProperties code-gen from CoreCompile

[33mcommit b5e8773634cbd7c94eb64e128623328885b65979[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Jul 21 08:26:10 2015 -0700

    Trigger TestProperties code-gen from CoreCompile
    
    The TestProperties code-gen target was previously listed
    as a DefaultTarget and executed when the project was built.
    
    However the build tools Coverage targets execute only the
    Build target and bypass this code-gen target.
    
    This change moves this target into $(CoreCompileDependsOn) to
    ensure it is called by any form of build.

[33mcommit ef0a495f5b3dba5df7465fb18a818ae51176e597[m
Merge: 84313dd 6784cc2
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Tue Jul 21 15:12:17 2015 +0800

    Merge pull request #203 from iamjasonp/fixup-p2p-refs
    
    Fixup p2p references and project.json files after #177

[33mcommit 6784cc24d478ee67e83b8f5c29651ef7894b8587[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Tue Jul 21 15:00:29 2015 +0800

    Fix up ExpectedExceptionTests to include contract references in project.json
    
    After #177, `ExpectedExceptions/project.json` omitted a couple of project references.
    For some reason though, this issue didn't show up in our previous private buddy builds
    or even in CI.

[33mcommit 045445641f6f9f82438c78b7b75f21b7d8fa4d82[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Tue Jul 21 15:00:17 2015 +0800

    Fix up TestTypes.cs to use the FaultException<T> ctor in contract
    
    5fe8647 (#200) was merged around the same time as #177, but 5fe8647
    was using a ctor that doesn't exist in the contract. When #177 was
    merged, this caused a build break.

[33mcommit 84313dd47fc59a8388f3ef6b6839e7adba1654cb[m
Merge: 13d0de8 ac8df33
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Tue Jul 21 13:48:08 2015 +0800

    Merge pull request #177 from iamjasonp/p2p-projectreferences
    
    Modify csproj and project.json files to use Contract references

[33mcommit 13d0de8043eda107cbb32dda04924a51c325b1c4[m
Merge: 5fe8647 66d72a2
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Tue Jul 21 13:47:54 2015 +0800

    Merge pull request #197 from iamjasonp/assert-true-test-messages
    
    Improve exception message reporting from Assert.True checks

[33mcommit 5fe8647664a5d932e9402322287b8c700d68ae11[m
Merge: cfab191 7ba5836
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Tue Jul 21 11:40:14 2015 +0800

    Merge pull request #200 from StephenBonikowsky/Issue169
    
    Adding FaultContract negative test for Duplex scenario.

[33mcommit cfab19193148e2fb1d7cbb80f55e0a2707cefe10[m
Merge: b742c97 53afc2a
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Tue Jul 21 11:32:22 2015 +0800

    Merge pull request #202 from roncain/resourcefolder
    
    Make Bridge use configurable location for resources

[33mcommit 7ba5836c4070fa26c0154d3d9d4e67303c33ba34[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Jul 17 14:12:29 2015 -0700

    Adding FaultContract negative test for Duplex scenario.
    
    * Test currently fails, opened Issue 194 and added the ActiveIssue attribute to the test.
    * Code review feedback has been incorporated to this commit.
    Fixes #169

[33mcommit b742c97ea7b23f650aec0efd6ef97b2086fd5b0a[m
Merge: 2b4bd31 6fec11f
Author: Sajay Antony <sajaya@microsoft.com>
Date:   Mon Jul 20 13:54:00 2015 -0700

    Merge pull request #201 from SajayAntony/idle_bridge
    
    Enable IdleTimeout for Bridge.exe . Part of #198

[33mcommit 53afc2ae69bb34f5ab49179f605d4bd93ae23b5f[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Jul 20 13:50:47 2015 -0700

    Make Bridge use configurable location for resources
    
    TestProperties contains a configurable property for the
    location of the Bridge's resources.  This PR makes the
    WcfService compile to that location, and the Bridge client
    configures the Bridge using this location.
    
    Fixes #186

[33mcommit 6fec11f4bf565e06e242c0252bf0bb5fe6d5ceb7[m
Author: Sajay Antony <sajaya@microsoft.com>
Date:   Mon Jul 20 11:44:03 2015 -0700

    Enable IdleTimeout for Bridge.exe . Part of #198

[33mcommit 66d72a214384076cca140c73ecfd1e0cf2cf07a3[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Mon Jul 20 17:38:43 2015 +0800

    Remove Assert.True string condition check in DuplexChannelFactoryTest
    
    DuplexChannelFactoryTest.CreateChannel_Using_Http_NoSecurity attempts to check
    for the string 'IRequestChannel' inside the exception string of the InvalidCastException.
    While CoreCLR reports this string, .NET Native does not report anything meaningful inside
    the exception string - only "Arg_InvalidCastException" with no further details.
    
    As such, we need to remove this check as it will never pass in .NET Native - even
    though this check will work fine in CoreCLR.
    
    Fixes #189

[33mcommit e8a3ca47671db8923563cdaa30b913996d3317b9[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Mon Jul 20 17:30:13 2015 +0800

    Improve exception message reporting from Assert.True checks

[33mcommit ac8df338ec02916ff268da86c48101ef093c9747[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Mon Jul 20 12:29:04 2015 +0800

    Add reference to System.Net.Sockets and System.Reflection.Emit.Lightweight to project.json

[33mcommit 30ca683015665d35a157038807fba48a9db17546[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Jul 16 19:06:36 2015 +0800

    Modify csproj and project.json files to use Contract references
    
    Fixes #172

[33mcommit 2b4bd31bb11329beb317f543c6694029203d5fd8[m
Merge: d118dd5 e1d64d3
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Jul 17 11:20:33 2015 -0700

    Merge pull request #190 from roncain/update-build-tools
    
    Update build-tools to version 58 to match CoreFx

[33mcommit d118dd5313610bc7e574351f0616e608fd54396d[m
Merge: 86ec20d 4a9ac25
Author: Sajay Antony <sajaya@microsoft.com>
Date:   Fri Jul 17 09:49:03 2015 -0700

    Merge pull request #183 from SajayAntony/FakeAddress
    
    Fixes #182 - Refactor BaseAddress and Endpoints classes for better isolation.

[33mcommit e1d64d3419f401d0bdeba591a33714aa689e14ba[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Jul 17 08:44:56 2015 -0700

    Update build-tools to version 58 to match CoreFx
    
    This change to build-tools permits F5 debugging in VS2015
    without needing to run build.cmd each time.
    
    It also reverts the work-around we had made to test.props for this.
    
    Fixes #188

[33mcommit 4a9ac25c7a6f43b03f257d80e18f1a6ce9ec9fee[m
Author: Sajay Antony <sajaya@microsoft.com>
Date:   Thu Jul 16 14:27:29 2015 -0700

    Moved Endpoints to Scenarios/ServiceModel.Scenarios.Common/Endpoints.cs. Fixes #182

[33mcommit 68f54166a544df574539f232897f543471acf14d[m
Author: Sajay Antony <sajaya@microsoft.com>
Date:   Thu Jul 16 14:17:52 2015 -0700

    Refactored to use FakeAddress. Addressing #182.

[33mcommit 0d69a45c5a5fae0233e3e207f87c5089d1c3ea32[m
Author: Sajay Antony <sajaya@microsoft.com>
Date:   Thu Jul 16 13:58:15 2015 -0700

    Renaming BaseAddress to FakeAddress.

[33mcommit 86ec20db83ab444b0008ea7792b65d560a1fe629[m
Merge: 3cadc82 dbd5bce
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Fri Jul 17 14:38:49 2015 +0800

    Merge pull request #180 from roncain/reenable-duplex-unit
    
    Re-enables Duplex unit tests previously disabled

[33mcommit dbd5bce0219658e9f42af4f8e553a23dc003d255[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Jul 16 12:42:10 2015 -0700

    Re-enables Duplex unit tests previously disabled
    
    The Duplex unit tests previously disabled with [ActiveIssue(178)]
    have been re-enabled and modified to use static BaseAddress URL's
    
    Fixes #178

[33mcommit 3cadc827b71d4bdeeba2c4a5934f6a6d5dc7510d[m
Merge: c19b9fe a7e89cf
Author: Dustin Metzgar <dmetzgar@users.noreply.github.com>
Date:   Thu Jul 16 12:04:24 2015 -0700

    Merge pull request #164 from dmetzgar/bridge_squashed_rebased
    
    Bridge rebased to latest from main

[33mcommit a7e89cfd9a6a473f03cf3358af8ecef2ace5ef43[m
Author: Dustin Metzgar <dmetzgar@microsoft.com>
Date:   Tue Jul 14 00:31:53 2015 +0000

    Bridge rebased to latest from main

[33mcommit c19b9fef57517d356b639b1a2f92bc05b755805a[m
Merge: 08ccc2d c0cb1db
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Jul 16 11:11:41 2015 -0700

    Merge pull request #179 from roncain/disable-duplex-unit-tests
    
    Disable Duplex unit tests using Endpoints

[33mcommit c0cb1dbe1f2ffbfe095743c6cab21887744b6f8f[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Jul 16 11:03:40 2015 -0700

    Disable Duplex unit tests using Endpoints
    
    The Endpoints class is reserved for [OuterLoop] tests and
    requires a running WCF Service.
    
    This PR adds [ActiveIssue(178)] to all the Duplex unit tests
    using Endpoints.  They need to be converted to use BaseAddress only.

[33mcommit 08ccc2da43c0a9308a8881b5021fe35264c46666[m
Merge: 8c54320 31c8f27
Author: Matt Connew <mconnew@microsoft.com>
Date:   Wed Jul 15 22:04:35 2015 -0700

    Merge pull request #176 from mconnew/Issue78
    
    Fix async channel open so that continuations don't use the current sync context

[33mcommit 31c8f27c1b083f65d039e89c45ddbed4b36ac238[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Wed Jul 15 20:21:10 2015 -0700

    Fix async channel open so that continuations don't use the current synchronization context.
    This was causing deadlocks when calling a Task based client synchronously, e.g. FooAsync().Result.
    Fixes #78

[33mcommit 8c543209dd13284d9fdd0a708f8cd04dfa2b6a71[m
Merge: b3a00f8 be8adfb
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Thu Jul 16 10:53:03 2015 +0800

    Merge pull request #174 from iamjasonp/modify-duplexclientbasetests
    
    Modify DuplexChannelFactoryTests to use APIs in public contract only

[33mcommit b3a00f841ac5a8f4346c640d255c1ab23e4e83b6[m
Merge: 13bf3f9 f2fd28a
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Thu Jul 16 01:30:42 2015 +0800

    Merge pull request #170 from iamjasonp/modify-client-basetests
    
    Simplify some ClientBaseTests per Issue #74

[33mcommit 13bf3f9568994c840e3dc67eb18ce8d95c306879[m
Merge: 246289a ed505c3
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Jul 15 10:18:18 2015 -0700

    Merge pull request #173 from roncain/integrate-corefx-changes
    
    Integrate recent CoreFx changes to props and targets files

[33mcommit be8adfb3864518b27aecfd6326e97350462171d5[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu Jul 16 00:53:20 2015 +0800

    Modify DuplexChannelFactoryTests to use APIs in public contract only
    
    The DuplexChannelFactoryTests use APIs appearing only in the P2P APIs
    that are not in the public contract. As such, these tests fail to compile when
    run in the experimental CoreCLR/NETNative test environment.
    
    It uses forms of the ctor that take an Object, which are not supported in the contract.
    Replace with using teh cotr that takes an InstanceContext instead
    
    Fixes #171

[33mcommit ed505c3fbe9a6bef942b4a597ee253362c5ead5b[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Jul 15 09:19:27 2015 -0700

    Integrate recent CoreFx changes to props and targets files
    
    CoreFx made some changes to alter how the DnuRestore worked
    and to enable warnings as errors (which is how the projects
    are built in the TFS mirrored location).  This PR brings those
    changes to WCF.
    
    It also fixes the one warning we had that caused the build to fail.

[33mcommit f2fd28ab5a71c72269ad8d3712f914d2c0d6ccd4[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Wed Jul 15 15:25:15 2015 +0800

    Simplify some ClientBaseTests per Issue #74

[33mcommit 246289af6f56249f24861f17b9b90914d57b93f6[m
Merge: 5411ddf 2241570
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Wed Jul 15 14:32:45 2015 +0800

    Merge pull request #163 from roncain/duplex-netnative-pnse
    
    Replace use of Type.GetInterfaceMap in TypeLoader

[33mcommit 5411ddfa3195e396f94103be9198e6dbc2cee81c[m
Merge: b779087 00cd5a1
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Wed Jul 15 14:31:18 2015 +0800

    Merge pull request #161 from iamjasonp/duplex-tests
    
    Unit and scenario tests for DuplexClientBase and DuplexChannelFactory

[33mcommit 00cd5a1ad7642ffe12ed91bd60df25c374f038c2[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Wed Jul 15 14:20:33 2015 +0800

    Re-enable disabled MessageTest and some cleanup

[33mcommit 8d3e082529ecb5f3383cc84edbee54bc7a154f74[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Wed Jul 15 14:19:52 2015 +0800

    Adding DuplexClientBase, DuplexChannelFactory tests
    
    Modify some existing ClientBase and ChannelFactory tests

[33mcommit b779087899fbe4a2ac5de0ff490bedecb6d90ca9[m
Merge: 29171d8 deb0ad9
Author: Matt Connew <mconnew@microsoft.com>
Date:   Tue Jul 14 10:45:57 2015 -0700

    Merge pull request #165 from mconnew/duplex
    
    Fixing Callback contracts with a Task<T> return type

[33mcommit 22415704e1818728583680a6ff33de4091f499b2[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Jul 14 09:55:08 2015 -0700

    Enhance the accuracy of MethodInfo comparison

[33mcommit deb0ad9f4e4de9c2dc29cd674b49a75aafc9f17a[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Tue Jul 14 06:53:35 2015 -0700

    Fixing Callback contracts with a Task<T> return type

[33mcommit 678c92b889014c4fdaff692d3136b135abad59b9[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Jul 13 13:54:13 2015 -0700

    Replace use of Type.GetInterfaceMap in TypeLoader
    
    TypeLoader uses Type.GetInterfaceMap() to find an implementation
    method corresponding to a method from an interface.  But in
    NET Native this method throws PlatformNotSupportedException.
    As a consequence, Duplex does not work in NET Native.
    
    This PR replaces the call to Type.GetInterfaceMap() with private
    methods that find the implementation method based on the method name
    and parameter types.
    
    Fixes #153

[33mcommit 29171d84ae2a9c0e3f4aff7ef2c0c528b637dd5e[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Fri Jul 10 14:25:25 2015 -0700

    Scenario test for Issue #138 Update1
    
    * Incorporated feedback.
    * Found an issue with the endpoint on the service that had to be fixed.
    * The above issue also meant I had to fix the endpoint problem for the previous Duplex test that I checked in.
    
    Scenario test for Issue #138 Update2
    
    * Missed a semicolon.

[33mcommit 5a082865de3d972a31877dc236f386d04e0588a9[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Jul 9 16:26:08 2015 -0700

    Scenario test for Issue #138
    
    * Test won't pass until Issue 138 is fixed.
    * Validated that it worked on Desktop.

[33mcommit 135b52ed8e619aa82120a17b0280375b53762f11[m
Merge: 6c5ca63 843f79f
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Mon Jul 13 15:48:00 2015 +0800

    Merge pull request #159 from dotnet-bot/from-tfs
    
    Merge changes from TFS

[33mcommit 6c5ca633db01c199fb07c51d8234c4094bc47058[m
Merge: c928a02 012f88b
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Mon Jul 13 10:23:54 2015 +0800

    Merge pull request #158 from roncain/enable-duplex-test
    
    Enable previously skipped duplex scenario test

[33mcommit c928a024d01e141ee6eaac68580bdc53b9399881[m
Merge: 7c49c66 3b1ced4
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Sat Jul 11 06:09:52 2015 +0800

    Merge pull request #156 from iamjasonp/expectedexception-tests
    
    Changing ExpectedExceptionTests to not use Assert.Contains

[33mcommit 843f79f63da6425b7e07926dc53c0b73d21d76d4[m
Author: dotnet-bot <dotnet-bot@microsoft.com>
Date:   Fri Jul 10 13:34:30 2015 -0700

    [tfs-changeset: 1499940]

[33mcommit 012f88b3856196d3fe0eef87d57acbf0f99cd44a[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Jul 10 12:30:18 2015 -0700

    Enable previously skipped duplex scenario test
    
    Enables DuplexClientBaseOfT_OverNetTcp_Call after PR #152 was merged
    and enabled duplex operation.
    
    Also updates [ActiveIssue] of ServiceContract_TypedProxy_DuplexCallback
    to refer to new issue #157.

[33mcommit 7c49c66f5e4ec8914f87e8cc999d54372124f1f9[m
Merge: 3de9d02 f35271a
Author: Matt Connew <mconnew@microsoft.com>
Date:   Fri Jul 10 11:42:47 2015 -0700

    Merge pull request #152 from mconnew/duplex
    
    Enable duplex service contracts

[33mcommit f35271a8860e4c29456797cbc0cbeb2a9865575e[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Fri Jul 10 11:19:38 2015 -0700

    Enable duplex service contracts

[33mcommit 3de9d028c56a405e5ae60454a9145b92215c7e66[m
Merge: 21e9771 b033b2d
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Sat Jul 11 02:23:17 2015 +0800

    Merge pull request #155 from dotnet-bot/from-tfs
    
    Merge changes from TFS

[33mcommit 3b1ced4b12f0760253332c3af596e373bff06be5[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Fri Jul 10 11:18:44 2015 -0700

    Changing ExpectedExceptionTests to not use Assert.Contains
    
    Our internal xunit-like framework for NET Native doesn't support the use of Assert.Contains,
    so for now, moving any Assert.Contains to using Assert.True instead

[33mcommit b033b2da63e8aa1df6984e3862ee0fe99a7af628[m
Author: dotnet-bot <dotnet-bot@microsoft.com>
Date:   Fri Jul 10 09:39:00 2015 -0700

    [tfs-changeset: 1499856]

[33mcommit 21e9771446432fc0f0015a7dc1562bd4a1861fe1[m
Merge: d916b95 ab3272a
Author: Matt Connew <mconnew@microsoft.com>
Date:   Thu Jul 9 13:43:46 2015 -0700

    Merge pull request #150 from roncain/escape-urls
    
    Escape URLs in dir.props

[33mcommit d916b9594fbcdadef978651f31a2ded75519b0a8[m
Merge: 4bdae37 4af3d5a
Author: Matt Connew <mconnew@microsoft.com>
Date:   Thu Jul 9 13:42:34 2015 -0700

    Merge pull request #148 from dotnet/update-readme-packagelinks
    
    Update README.md with myget.org package links

[33mcommit ab3272ae07e8b2025098e5d1bc3493e293382c6f[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Jul 9 10:26:41 2015 -0700

    Escape URLs in dir.props
    
    The double-slashes for URL's in dir.props needed
    to be escaped to match CoreFx's changes.

[33mcommit 4bdae37b61ede183797c435281d019c6d066e529[m
Merge: de6c043 a4ea1d4
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Jul 9 10:07:50 2015 -0700

    Merge pull request #149 from dotnet-bot/from-tfs
    
    Merge changes from TFS

[33mcommit a4ea1d449b60398bb148bab3dc543db0dc0a33ef[m
Author: Joel Hendrix <jhendrix@microsoft.com>
Date:   Thu Jul 9 09:41:43 2015 -0700

    The Mac build requires some content be pulled from nuget.org, see https://github.com/dotnet/corefx/issues/2236; this change adds it to the source list passed to nuget.exe.
    
    [tfs-changeset: 1499319]

[33mcommit 4af3d5a0d974d79d6d397d6a0887a492a68604a3[m
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Thu Jul 9 12:25:36 2015 +0800

    Update README.md with myget.org package links

[33mcommit de6c043aabee6e4eca84e0f012bb1a4ee6dc3f39[m
Merge: 3fe34f0 9bd7000
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Thu Jul 9 11:10:50 2015 +0800

    Merge pull request #146 from iamjasonp/expected-exception-tests
    
    Change some ExpectedExceptionTests to show better exception messages

[33mcommit 9bd7000ada43bcfdcef5afe636434f605db29b8a[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Wed Jul 8 09:37:47 2015 -0700

    Change test names to be more descriptive in ExpectedExceptionTests
    
    PR feedback, change some tests names that being used were a little short
    of descriptive; changing some test names to reflect that
    
    Also, changed some bounds checking for some time-based tests.
    We wanted to be a little bit more lenient with the bounds checking in case
    the tests ended up taking a bit longer due to CI machine issues or if
    a user's local machine was a bit slower

[33mcommit 97e16b4c69a599a840c4381cd516ede4fb82b7da[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Wed Jul 8 02:15:23 2015 -0700

    Change some ExpectedExceptionTests to show better exception messages
    
    See Issue #74
    Previously we swallowed up some exceptions when unexpected exceptions were
    thrown, which means that we lacked ability to diagnose these failures when
    they happened.

[33mcommit 3fe34f0a9f97569849f339528cb7ab90200e2262[m
Merge: 9dd4540 f9b5a2e
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Thu Jul 9 10:23:06 2015 +0800

    Merge pull request #147 from StephenBonikowsky/Issue119
    
    Adding back test accidentally removed.

[33mcommit f9b5a2ef7cda7946bb38f53b4cb9a0d95b28da36[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Jul 8 14:14:57 2015 -0700

    Adding back test accidentally removed.
    
    * This test was first added with PR #119.
    * On the following day PR #121 deleted the test.
    * This was not intentional, looks like the branch the PR came from was not synced with my PR from the day before.
    * Adding it back.

[33mcommit 9dd4540f2e58b67d362b95d2cdba76a318a0d020[m
Merge: 2c246b5 f1ea13c
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Jul 8 05:09:26 2015 -0700

    Merge pull request #145 from dotnet-bot/from-tfs
    
    Merge changes from TFS

[33mcommit 2c246b5d415f96535f8772d152a613df0710ead6[m
Merge: b716d08 da06c6e
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Jul 7 14:32:31 2015 -0700

    Merge pull request #142 from StephenBonikowsky/Issue66
    
    Removing the condition: Exclude="Scenarios*\

[33mcommit f1ea13c768796de3676bf9f3d1d8c3c48710fc79[m
Author: Joel Hendrix <jhendrix@microsoft.com>
Date:   Tue Jul 7 14:25:03 2015 -0700

    Title:
    
         Use -source instead of nuget.config
    
     Change Description:
    
         Use the -source argument to nuget.exe instead of nuget.config files.
         Added condition to add \\cpvsbuild source if it exists; this is to work around a bug in DNU that causes restore to fail if the share is unavailable.
         Cleaned up disable of -parallel option to match that of fxcore\open.
    
    [tfs-changeset: 1498317]

[33mcommit b716d0800c5861f1626f9e89e750877dc32a1ab7[m
Merge: def5b78 ba7ee0b
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Jul 7 13:51:31 2015 -0700

    Merge pull request #144 from roncain/refactor-sln
    
    Refactor SLN file

[33mcommit ba7ee0b15cd969dd818f3613f534d441033b837c[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Jul 7 13:44:33 2015 -0700

    Refactors the main SLN file
    
    Creates a 'unit' solution folder and moves all unit
    test projects into it.  Also creates a 'common'
    solution folder and moves both the src and tests folders
    into it.

[33mcommit def5b789fb271e3d7bc2efc1cc29fe6ec2bb35b5[m
Merge: 1efe383 fae770f
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Jul 7 13:40:30 2015 -0700

    Merge pull request #143 from roncain/common-tests
    
    Move TestPropertiesTest to new folder

[33mcommit fae770f568302e5fc24ee2c96100203e117c799f[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Jul 7 13:32:58 2015 -0700

    Move TestProperties test
    
    The unit test for TestProperties was not in the correct
    location when used in the experimental NET Native test
    environment.  This change creates a new project and folder
    explicitly for tests of the test infrastructure itself and
    moves the TestPropertiesTest into it.

[33mcommit da06c6eeb714d99e0ae83000f80592a37ddcb66b[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Tue Jul 7 11:00:22 2015 -0700

    Removing the condition: Exclude="Scenarios*\
    
    Fixes #66

[33mcommit 1efe383d27086f26bb35125f0bd597939bdf818b[m
Merge: b6bd219 ba372b7
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Jul 7 09:29:32 2015 -0700

    Merge pull request #141 from StephenBonikowsky/Issue75
    
    Fixing Issue75

[33mcommit ba372b73d45d089badd0dd54b420d74d5a8fbfa4[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Jul 6 13:37:28 2015 -0700

    Moved unit tests under System.Private.ServiceModel
    
    * Unit tests were under .../src/System.Private.ServiceModel/tests, moved them into a new folder called "Unit" so they will be distinct from the Scenario tests.

[33mcommit 65a06e5c35d44038ebb3dadae35b1515c12ba94d[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Jul 6 14:14:50 2015 -0700

    Moving test common code to better location.
    
    *Updated scenario projects to use the TestCommon project reference variable.

[33mcommit e009fda128073d464569dba1eb921b435615f04e[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Jul 6 11:21:47 2015 -0700

    Simplifying project references in test projects.

[33mcommit b6bd2190b265cc527a0fc35f75eea8a2d31c68b7[m
Merge: 4ca5431 42e79c9
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Jul 2 11:55:41 2015 -0700

    Merge pull request #135 from roncain/override-targets
    
    Add override.targets and wcf.targets

[33mcommit 4ca54316e46c7faa0c67c57e01b520c5525d85d5[m
Merge: 793fa18 65cb41b
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Jul 2 11:51:47 2015 -0700

    Merge pull request #136 from roncain/fix-duplex-test
    
    Fix DuplexClientBaseTests use of Close()

[33mcommit 42e79c9a2972761c64b14a6ee115eb0da0cad94c[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Jul 2 11:46:20 2015 -0700

    Add wcf.targets
    
    wcf.targets initializes properties and targets used by all product,
    tooling and test projects.

[33mcommit 65cb41b6754c11cd2dada5572425be73cdc604ae[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Jul 2 09:32:25 2015 -0700

    Fix DuplexClientBaseTests use of Close()
    
    DuplexClientBaseTests used ClientBase<T>.Close() which is public
    but not available in the public contract.  So the test fails when
    run against NET Native.  This change allows it to function in both
    CoreCLR and NET Native.

[33mcommit 793fa187cb4ea57a0bee0d09953d1e3013a04b4d[m
Merge: c2335da b50d29e
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Jul 1 10:47:52 2015 -0700

    Merge pull request #130 from roncain/update-packages
    
    Update package version dependencies

[33mcommit c2335da0d95e372188627f2beba2f79450772b4c[m
Merge: d959bd3 7c0bce5
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Jul 1 10:31:26 2015 -0700

    Merge pull request #131 from roncain/non-parallel-packagerestore
    
    Temporarily make package restore not run concurrently

[33mcommit 7c0bce5a2d487766ab2a520f18d29db88635c331[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Jul 1 10:10:22 2015 -0700

    Temporarily make package restore not run concurrently
    
    An issue with Dnu package restore running concurrently is preventing
    builds from working correctly.  This change is temporary and allows
    the build to succeed.  An issue will be opened to undo this change
    when Dnu is fixed.

[33mcommit b50d29e9b8352c3bbf263f74224cceb807cc0f62[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Jul 1 07:57:51 2015 -0700

    Update package version dependencies
    
    Updates project.json files to use the the latest beta versions
    of packages, using a wildcard to be more flexibile in the future.
    
    Added explicit some new System.Reflection.Emit dependencies that
    appear to be necessary now (copied from CoreFx repo)
    
    The project.lock.json files were updated automatically using:
        msbuild /t:RestorePackages /p:LockDependencies=true

[33mcommit d959bd38d81ae21eeabf1cae11363aee05b43c6c[m
Merge: e50d582 70998f1
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Jun 30 13:46:39 2015 -0700

    Merge pull request #120 from roncain/testproperties
    
    Adds TestProperties class to capture MSBuild properties

[33mcommit 70998f14a5fa022ee93fa524968400f0963b8686[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Jun 30 13:31:10 2015 -0700

    Adds TestProperties feature to pass parameters to tests
    
    TestProperties is a class used by tests to access at
    runtime known properties.  They can come from either
    build-time MSBuild properties or runtime environment
    variables.
    
    Fixes #29

[33mcommit e50d5822fd73562f53407825c25b8edb256ec0fb[m
Merge: 85cc00d 95362c5
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Tue Jun 30 17:12:05 2015 +0800

    Merge pull request #128 from dotnet-bot/from-tfs
    
    Merge changes from TFS

[33mcommit 95362c55caec4fbe55cad4170736d9c2f2cd6f36[m
Author: Joel Hendrix <jhendrix@microsoft.com>
Date:   Mon Jun 29 18:02:16 2015 -0700

    Port build time and reliability fixes from ProjectN.  This includes fixes from the following changesets.
    
    1485206
    1485208
    1486043
    1488531
    1493227
    1493288
    1493289
    1493328
    1493358
    1495328
    
    [tfs-changeset: 1495502]

[33mcommit 85cc00d1bff74bdef272da1a82bcea1816319e68[m
Merge: efb6f44 cf09295
Author: Sajay Antony <sajaya@microsoft.com>
Date:   Mon Jun 29 14:46:33 2015 -0700

    Merge pull request #121 from hongdai/addtest1
    
    Add test coverage for Async APIs with NetTcpBinding

[33mcommit efb6f44f132771b3989022929bf3652858f81095[m
Merge: 7fc19f0 7ec9fd3
Author: Ron Cain <roncain@microsoft.com>
Date:   Mon Jun 29 04:56:45 2015 -0700

    Merge pull request #125 from dotnet-bot/from-tfs
    
    Merge changes from TFS

[33mcommit 7ec9fd3b7a739626ff4b513cd1755c62f132ecc0[m
Author: Nick Guerrera <nicholg@microsoft.com>
Date:   Fri Jun 26 12:27:28 2015 -0700

    Adapt to internal changes in nuget packaging infrastructure
    
    GetFilesToPackage is replaced with just GetDocumentationFile. The other
    files are added to the package contents through a different process now.
    
    [tfs-changeset: 1494556]

[33mcommit 7fc19f0fb292f7095dbab76a88f4013775b37af4[m
Merge: 04d010a 3fb6ef2
Author: Sajay Antony <sajaya@microsoft.com>
Date:   Fri Jun 26 12:21:36 2015 -0700

    Merge pull request #124 from SajayAntony/vsdebug
    
    Enable building and debugging of test projects from visual studio

[33mcommit 3fb6ef2c7593b4bc496e1abd6f5f459d194f00dd[m
Author: Sajay Antony <sajaya@microsoft.com>
Date:   Fri Jun 26 12:04:44 2015 -0700

    Enable building and debugging of test projects from visual studio
    
    Once dotnet/buildtools#181 is fixed we should be able to remove this.

[33mcommit cf09295f2bbd9aac6cf53d3807eee2b91fdbeeb1[m
Author: Hong Dai <hongdai@microsoft.com>
Date:   Thu Jun 25 18:02:19 2015 -0700

    Add test coverage for Async APIs with NetTcpBinding
    
    * None of the tests are passing because of product issue 78
    
    Fixes #97

[33mcommit 04d010a765b31f44e1910a6f993e9412ff0b3f54[m
Merge: a2ed11a 3ec28ed
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Jun 25 09:48:35 2015 -0700

    Merge pull request #119 from StephenBonikowsky/Issue118
    
    Adding additional duplex scenario test.

[33mcommit 3ec28eda4b528a04924613bbc7881c6377fb27fb[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Jun 24 17:03:38 2015 -0700

    Adding additional duplex scenario test.
    
    * Adding a typed proxy variation using DuplexChannelFactory
    * Fixes #118

[33mcommit a2ed11af1eca8d1804d1469f329e5ae937dd940d[m
Merge: f382b26 06e5d88
Author: Matt Connew <mconnew@microsoft.com>
Date:   Wed Jun 24 13:57:02 2015 -0700

    Merge pull request #112 from StephenBonikowsky/Issue104
    
    Add Duplex callback service to WcfTestService (Issue #104)

[33mcommit 06e5d881ab0749e589bd4cc35356497995b50063[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Jun 24 12:02:40 2015 -0700

    Adding Duplex tests.
    
    * Addressed most of the comments.
    * Keeping it on a separate base endpoint for now because during debugging found strange traffic occuring on 809, wanted to make sure that was not because of this particular test.
    * Removed ServiceBehavior ConcurrencyMode.Reentrant and used Task.Run calls instead to switch threads on both server and client side.
    * Modified the WcfDuplexServiceCallback to make Sajay happy.
    * Fixes #104

[33mcommit f382b26a02628ba82258fc3f3f68fdb70103481d[m
Merge: f42d600 0dcae52
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Jun 24 09:42:05 2015 -0700

    Merge pull request #116 from roncain/fix-warnings
    
    Fix build warnings

[33mcommit 0dcae52e109b9c0509b3c3745fd1f08d3df9eae3[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Jun 24 06:10:16 2015 -0700

    Fix build warnings
    
    Build warnings were produced due to fields not being CLS-compliant
    and Equals being overridden without overriding GetHashcode.
    These warnings caused the build to fail when building the
    TFS mirrored sources with warnings treated as errors.

[33mcommit f42d60019674f92b3c3756ee32919d110aaa2f92[m
Merge: f8c4324 ee868cb
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Wed Jun 24 16:16:24 2015 +0800

    Merge pull request #114 from dotnet-bot/from-tfs
    
    Merge changes from TFS

[33mcommit f8c4324d580834d92b7ae309e959ea2bf565373b[m
Merge: be79539 ddcd6e4
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Wed Jun 24 16:13:46 2015 +0800

    Merge pull request #102 from mconnew/http-authentication
    
    Fix ability to replay http content as well as code cleanup

[33mcommit ee868cb8c8e243e211459a47d7d9740477afeeae[m
Author: Nick Guerrera <nicholg@microsoft.com>
Date:   Tue Jun 23 17:31:54 2015 -0700

    Mark assemblies as serviceable
    
    [tfs-changeset: 1492895]

[33mcommit ddcd6e462faa59063d18b82675cbc7f65efd5652[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Wed Jun 17 15:49:38 2015 -0700

    Simplify HttpClientRequestChannel and HttpClientChannelAsyncRequest. The send path now throws
    exceptions related to sending the request, and the receive path now deals with exceptions
    related to receiving the response. Simplified the logic for sending a Head request for pre-auth.
    No more juggling tasks of tasks to manage writing to the forwarding stream after the request to
    HttpClient has completed. This is possible because of the new MessageContent class.

[33mcommit 97b5a8c2b60a0e83da6f7ca0eabcf0f578cf7d3e[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Wed Jun 17 15:45:00 2015 -0700

    Adding stateful HttpRequestMessageHelper to do tasks related to an http request.
    Adding MessageContent, BufferedMessageContent and StreamedMessageContent to wrap a WCF message to produce
    and/or write to the streams that HttpClient needs.
    Adding ProducerConsumerStream which asynchronously blocks a read or write call until the other call completes
    to allow a bufferless/zero allocation passthrough stream.

[33mcommit 30eccdae259bfe710b788bb310b26106292aa965[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Wed Jun 17 15:42:41 2015 -0700

    Removing HttpInput and HttpOutput classes
    Removed unused helper code

[33mcommit be7953931bb550a7db19efbd2671c394658cd060[m
Merge: 16b56f5 87847c5
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Jun 22 17:24:15 2015 -0700

    Merge pull request #109 from iamjasonp/remove-constants
    
    Remove Constants.cs and LocalConstants.cs it's no longer used

[33mcommit 87847c5a9640ecf53d14a9c812ffa106bb53efb4[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Mon Jun 22 18:43:03 2015 +0800

    Remove Constants.cs and LocalConstants.cs it's no longer used

[33mcommit 16b56f5d8bbe45bcac1b2ff640d4d18c272a9913[m
Merge: a9b6fc9 1e58020
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Mon Jun 22 12:34:56 2015 +0800

    Merge pull request #106 from roncain/xunit-reduced
    
    Eliminate use of xunit Assert.IsAssignableFrom

[33mcommit a9b6fc98ec5e9a08eaf533cff67e7944d7e1e6e1[m
Merge: 39dba33 fa6fbb3
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Mon Jun 22 12:18:22 2015 +0800

    Merge pull request #107 from roncain/remove-sessionmode
    
    Remove use of ServiceContractAttribute.SessionMode

[33mcommit fa6fbb339d29566199ed3746316b6a12e8ba7911[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Jun 19 14:08:26 2015 -0700

    Remove use of ServiceContractAttribute.SessionMode
    
    This property is not part of the public contract but was
    being used inadvertantly by a unit test.  Removes that usage.

[33mcommit 1e58020c07488747858ecc6b633c7211396ea140[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Jun 19 13:24:00 2015 -0700

    Eliminate use of xunit Assert.IsAssignableFrom
    
    The NET Native version of xunit does not support
    Assert.IsAssignableFrom, so this change removes that
    usage and does explicit IsAssignable checks.

[33mcommit 39dba330a8e408c4dfc7568ddee4d82096ec4f98[m
Merge: 76fcbc6 9dc6e6f
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Fri Jun 19 17:59:52 2015 +0800

    Merge pull request #103 from roncain/callbackcontract
    
    Enables the CallbackContractType in ContractDescription

[33mcommit 76fcbc680f9c17e336ea464ad5f8a120bce80e19[m
Merge: ce2cfcb 0cad399
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Fri Jun 19 12:03:42 2015 +0800

    Merge pull request #105 from dotnet-bot/from-tfs
    
    Merge changes from TFS

[33mcommit 0cad3994d3af973a6546549af19d8d2c08db9b91[m
Author: Eric St. John <ericstj@microsoft.com>
Date:   Thu Jun 18 12:00:11 2015 -0700

    Improve perf of package build and remove hardcoded dependencies.
    
    This reduced msbuild /m of packages dir from 30s to 6s on my box.  I did this by limiting the number of P2P recursions and ensuring they happen in parallel.
    
    I've also cleaned up the cases where we hardcoded a dependency on package version and replaced with a project reference.
    
    I've diffed the output to ensure there are no unintentional side effects.
    
    [tfs-changeset: 1490598]

[33mcommit 9dc6e6f9bc1a8dedddcbfd01bb8d8c6cfef48030[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu Jun 18 08:33:56 2015 -0700

    Enables the CallbackContractType in ContractDescription
    
    Adds code to extract the ContractCallbackType property from
    ServiceContractAttribute and ensures it is properly initialized
    in the ContractDescription.
    
    Adds a unit test for this change and enhances the unit tests of the
    CallbackBehaviorAttribute that is instantiated by this change.
    
    Fixes #92

[33mcommit ce2cfcb74b106b29f0a90dcbc5db17fa1a322884[m
Merge: 8e98a14 a194b96
Author: Hong Dai <hongdai@microsoft.com>
Date:   Wed Jun 17 10:57:57 2015 -0700

    Merge pull request #85 from hongdai/basicauth
    
    Enable Basic Authentication test in OSS

[33mcommit a194b967ff19cd014a05c084ad95620f800df801[m
Author: Hong Dai <hongdai@microsoft.com>
Date:   Mon Jun 15 17:12:28 2015 -0700

    Enable Basic Authentication test in OSS
    
    * Eliminate the dependency on a domain user by using a custom user name/pwd validator
    
    Fixes #69

[33mcommit 8e98a14695c0d1cb2dbddc2c9b42978db8ba5ea3[m
Merge: 86a5738 67ee296
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Jun 17 09:18:15 2015 -0700

    Merge pull request #89 from mconnew/fix-test-proxy-usage
    
    Add support for tests to use fiddler proxy

[33mcommit 86a57386202be1d2c0b6772747d8fea67c0b9b23[m
Merge: d9ac565 6e92d06
Author: Zhenlan Wang <zhenlwa@microsoft.com>
Date:   Tue Jun 16 18:23:51 2015 -0700

    Merge pull request #86 from iamjasonp/internal-build
    
    Change some pragmas to format supported by internal build tools

[33mcommit 67ee29639ca356ff18494f6f720a61e6e8a05497[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Tue Jun 16 12:14:26 2015 -0700

    Add support for tests to use fiddler proxy
    With this change, you can specify /p:UseFiddler=true to build.cmd and the special hostname
    localhost.fiddler will be used. This causes the localhost proxy bypass to be skipped and the
    requests will be put through the defined proxy server. Fiddler then strips off the .fiddler
    from the hostname and passes on the request with the Host being just localhost.
    Some tests fail when using fiddler as the failure mode changes and there's not a lot we can
    do about that, but this does give greater flexibility to debugging product/test issues.

[33mcommit 6e92d06aaafc641a2d333ba07375fc1ad3e99ddf[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Wed Jun 17 02:08:53 2015 +0800

    Modify dir.props to allow dnu/nuget for resilience to trailing slashes

[33mcommit da9486715fc2251613f46b0cc9b61419aafef010[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Tue Jun 16 16:39:47 2015 +0800

    Change some pragmas to format supported by internal build tools
    
    There are some internal builds that use a slightly different compiler version,
    which means that while this code works with build.cmd from Git, it doesn't
    work with the toolchain we use for these builds.
    
    These changes will mean that we build on the build systems we're aware of.

[33mcommit d9ac56511178e7ff5e13a5af34089d2340827410[m
Merge: 08ca75a 6bf358f
Author: Hong Dai <hongdai@microsoft.com>
Date:   Fri Jun 12 09:32:51 2015 -0700

    Merge pull request #82 from StephenBonikowsky/Issue57
    
    Updated tests that were using the skip attribute.

[33mcommit 08ca75a38dfc3a819531bd0181220a8743574a3f[m
Merge: 503778e a59325b
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Fri Jun 12 18:20:38 2015 +0800

    Merge pull request #83 from zhenlan/fix-wiki-links
    
    Fix broken wiki links

[33mcommit 503778e0155ebd3e3e69a935a4f20d4ec422c951[m
Merge: f3a1d55 ce6f050
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Fri Jun 12 16:38:21 2015 +0800

    Merge pull request #84 from iamjasonp/update-buildtools
    
    Update buildtools to 1.0.25-prerelease-00053

[33mcommit ce6f050107e910f0b1e441b4f9b78b7b3e20867b[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Fri Jun 12 16:03:18 2015 +0800

    Update buildtools to 1.0.25-prerelease-00053
    
    * Update buildtools to 1.0.25-prerelease-00053
    * Fixes packages restore issues
    * Fixes CI issues

[33mcommit a59325b1e31e218df4561370fd16f278b00c84d5[m
Author: Zhenlan Wang <zhenlwa@microsoft.com>
Date:   Thu Jun 11 19:05:12 2015 -0700

    Fix broken wiki links
    
    The old links to wiki has been broken after we moved wiki to the documentation folder.
    The links are updated to pointing to the new documentation location.

[33mcommit 6bf358f5d22eef250a9e55420a99a5a0f5335bb2[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Thu Jun 11 16:12:40 2015 -0700

    Updated tests that were using the skip attribute.
    
    *Replaced the skip attribute with the ActiveIssue attribute.
    
    Fixes #57

[33mcommit f3a1d5535a60cfecc3419f5b5bce7798641a90d5[m
Merge: 2651637 d72a131
Author: Sajay Antony <sajaya@microsoft.com>
Date:   Wed Jun 10 16:17:24 2015 -0700

    Merge pull request #79 from StephenBonikowsky/ScenarioTestsRenaming
    
    Numerous changes to scenario tests per Issue #62

[33mcommit d72a1314b63a7d967aba21c629f646c08319da6c[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Jun 10 11:24:25 2015 -0700

    Numerous changes to scenario tests per Issue #62, including test class re-names and test method re-names.

[33mcommit 265163788013dc90f7ad72062e5e45c8da2ef7df[m
Merge: a891cd3 f9bb0df
Author: Matt Connew <mconnew@microsoft.com>
Date:   Tue Jun 9 15:59:00 2015 -0700

    Merge pull request #77 from zhenlan/fix-link
    
    Fix reference source link

[33mcommit f9bb0df977abfccfdef446a97cdd0e4657350b05[m
Author: Zhenlan Wang <zhenlwa@microsoft.com>
Date:   Tue Jun 9 14:15:58 2015 -0700

    Fix reference source link
    
    Update the link of reference source to pointing to github repo

[33mcommit a891cd38db0fdc2cbd6282a727a23884e7b42472[m
Merge: f06f7e1 4d9b13a
Author: Sajay Antony <sajaya@microsoft.com>
Date:   Fri Jun 5 15:15:37 2015 -0700

    Merge pull request #71 from roncain/netnative
    
    Enable conditional building for Net Native. Fixed #60

[33mcommit f06f7e1a1bddc0b48f0a65bcdca478f35542b81c[m
Merge: f918a45 1c7b2cd
Author: Sajay Antony <sajaya@microsoft.com>
Date:   Fri Jun 5 12:50:41 2015 -0700

    Merge pull request #72 from SajayAntony/docs
    
    Fix #56 - Move wiki content to documentation folder like CoreFx

[33mcommit 1c7b2cd4a9b34e15506ef5d068eb9a9bf7912a3e[m
Author: Sajay Antony <sajaya@microsoft.com>
Date:   Fri Jun 5 12:02:09 2015 -0700

    Fix #56 - Move wiki content to documentation folder like CoreFx

[33mcommit 4d9b13a3d6ef0071405e8b652815200ada7d31f9[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Fri Jun 5 10:30:38 2015 -0700

    Enable conditional building for Net Native
    
    These changes allow the main product to be built for
    NET Native conditionally.  This is not the default but
    is used when building the NuGet packages in the TFS mirror.
    
    This commit also adds an embedded rd.xml when the project
    is compiled for NET Native.
    
    Fixes #60

[33mcommit f918a45d9939b8fa87c98c23260d6400a9d17fe3[m
Merge: adec6c4 5ef5336
Author: Hong Dai <hongdai@microsoft.com>
Date:   Wed Jun 3 16:19:01 2015 -0700

    Merge pull request #68 from roncain/add-locked
    
    Set locked = true for all project.lock.json

[33mcommit adec6c48797b9f2486e4894e30b0edad7eefb3c0[m
Merge: 5c5123d c2d01a6
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Wed Jun 3 13:21:36 2015 -0700

    Merge pull request #65 from roncain/buildtools
    
    Copy common target files from CoreFx to restore build tools

[33mcommit 5ef533654fc7d19bede002dfbe52703f4e575b27[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed Jun 3 12:35:51 2015 -0700

    Set locked = true for all project.lock.json
    
    sets the 'locked' value to true for all project.lock.json files.
    Also adds project.lock.json files for refactored scenario tests.
    
    Setting the 'locked' value prevents package restore from
    attempting to resolve package versions again and update
    the project.lock.json file.
    
    This both locks down the actual versions as well as prevents
    the TFS mirror from attempting to write to a readonly file.

[33mcommit 5c5123ddbdb8237ce88f3c22e58d17c54ed3c4a0[m
Merge: f228f55 c042ed3
Author: Matt Connew <mconnew@microsoft.com>
Date:   Tue Jun 2 13:58:01 2015 -0700

    Merge pull request #63 from mconnew/http-authentication
    
    Enable Kerberos and Ntlm authentication for HTTP

[33mcommit c2d01a6c5dd0aeb760aa5689d3e8d8ae2248e195[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Jun 2 12:44:27 2015 -0700

    Copy common target files from CoreFx to restore build tools
    
    The CoreFx repro has updated its common targets files to
    restore the build tools.  We track whenever these change
    and keep our repo up to date. This is necessary to permit
    building the OSS projects in the TFS mirror.

[33mcommit c042ed34a6171d797ba0f62eb8b98dfd05237cbe[m
Author: Matt Connew <mconnew@microsoft.com>
Date:   Mon Jun 1 15:25:35 2015 -0700

    Enable Kerberos and Ntlm authentication for HTTP
    The HTTP transport was relying on TokenProvider's to provide the ICredentials for HttpClient. The code for
    Kerberos is complex and platform specific requiring P/Invokes which may not be available, such as in .Net Native.
    The TokenProvider logic was throwing an exception preventing Kerberos from being used when the absence of a
    Kerberos TokenProvide only blocks usage with message security. TokenProviders are only needed if using message
    security. For transport authentication, they are not needed so we can remove them and allow HttpClient to manage
    the authentication.

[33mcommit f228f55e28d16563579f5a12ec6929039854f472[m
Merge: 208d3cf 0221ae0
Author: Matt Connew <mconnew@microsoft.com>
Date:   Tue Jun 2 11:16:31 2015 -0700

    Merge pull request #58 from StephenBonikowsky/ScenarioTestReorg
    
    Reorganizing the scenario tests as described in Issue #50.

[33mcommit 208d3cfc665ca7b5d3ec5bef210e559e165e0ca5[m
Merge: cd0d452 187de82
Author: Sajay Antony <sajaya@microsoft.com>
Date:   Tue Jun 2 10:37:07 2015 -0700

    Merge pull request #61 from roncain/lock-json
    
    Add project.lock.json files for product and unit tests

[33mcommit 187de82405f554a05d96863061e8a7eeff9064af[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue Jun 2 10:00:26 2015 -0700

    Add project.lock.json files for product and unit tests
    
    The project.lock.json file must be added to the repo
    to ensure building against the correct versions.
    This matches how CoreFx handles them.

[33mcommit 0221ae0b94b0bfecea910432bf55dcc95c0ae47e[m
Author: Stephen Bonikowsky <stebon@microsoft.com>
Date:   Mon Jun 1 14:05:17 2015 -0700

    Reorganizing the scenario tests as described in Issue #50.

[33mcommit cd0d452ab09a4823d13e6d6d9bd9211826dbfced[m
Merge: af86fe9 1fca15b
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Fri May 29 03:11:35 2015 +0800

    Merge pull request #52 from roncain/csproj-cleanup
    
    Cleanup System.Private.ServiceModel.csproj

[33mcommit 1fca15bc708301ca8ab129912edd53ffe20f61a6[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Thu May 28 11:55:18 2015 -0700

    Cleanup System.Private.ServiceModel.csproj
    
    Added AssemblyVersion 4.0.0.0 to be consistent with CoreFx.
    Removed CodeAnalysisRuleSet to eliminate a warning building mirrored sources in TFS.
    Removed unnecessary source control properties.

[33mcommit af86fe95fe607c9f2609059ac918edc95b0141c7[m
Merge: 6b46d68 cf69703
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed May 27 13:06:13 2015 -0700

    Merge pull request #41 from iamjasonp/scenario-tests
    
    Merge working scenario feature branch to master

[33mcommit cf697036fc1e6c192370c965be354607ce69dc03[m
Merge: b9e3bb5 db8bed2
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Thu May 28 02:34:47 2015 +0800

    Import more tests and fixup issues from merge conflicts

[33mcommit b9e3bb57c3559f2ccf7353d7b3a789592feef9c4[m
Author: Jason Pang <jasonpa@microsoft.com>
Date:   Fri May 15 18:49:42 2015 +0800

    Create/port more scenario tests
    
    * Port over scenario tests and rationalize names
    * Remove packages.config references from ServiceModel.Scenarios.Common.csproj
    * Change exception messages in TypedProxyTests to state "timed out" rather than "failed".
    * Removing/replacing instances of Assert.True where it's no longer needed
      for exception catching
    * Modified a test to not use try { } catch { } Assert.True pattern

[33mcommit d45a3e80cb05ce0f1c419084e91b6c4819299e3a[m
Author: Hong Dai <hongdai@microsoft.com>
Date:   Fri May 15 19:14:08 2015 +0000

    WCF self-hosted E2E test service setup scripts
    
    * Setup script
    * Clean up self-hosted WCF Test Service E2E
    * Support more than one parameter to build.cmd for OuterLoop Tests

[33mcommit 6b46d68065ce08208dd1f8df3a5ec3ab821775a6[m
Merge: 876af67 7daf799
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue May 26 06:10:57 2015 -0700

    Merge pull request #34 from dVakulen/typo-fixes
    
    Typo fixes in Comments / Summaries

[33mcommit 7daf79957a65f42b966710f32d5efc0389241a96[m
Author: Dmitry Vakylenko <kremdima@mail.ru>
Date:   Sat May 23 17:57:08 2015 +0300

    Typo fixes in Comments / Summaries

[33mcommit 876af6763551129c7505e74bb2b3889899ec7cdb[m
Merge: 5651770 b34cca9
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Sat May 23 17:49:02 2015 +0800

    Merge pull request #33 from dVakulen/typo-patch-1
    
    Fix typo in comment in DuplexChannelFactory.cs

[33mcommit b34cca9bddfe949827b69636dab239e0699cbf86[m
Author: Dmitry <777Eternal777@users.noreply.github.com>
Date:   Fri May 22 20:18:02 2015 +0300

    Fix typo

[33mcommit 5651770101c7268447aad4b018f09a5bd8f46957[m
Merge: 177d29e 91ca200
Author: Jason Pang <iamjasonp@users.noreply.github.com>
Date:   Thu May 21 04:59:49 2015 +0800

    Merge pull request #24 from roncain/readmeupdate
    
    Update README

[33mcommit 91ca200c576e2784ef111eec2a33c954d188d818[m
Author: Ron Cain <roncain@microsoft.com>
Date:   Wed May 20 10:13:50 2015 -0700

    Update README

[33mcommit 177d29e79bad40f3d6c6c922738513656faaa026[m
Merge: 9615d08 fbe53b3
Author: Ron Cain <roncain@microsoft.com>
Date:   Tue May 19 05:49:25 2015 -0700

    Merge pull request #16 from iamjasonp/add-licence
    
    Add LICENSE file to WCF repo

[33mcommit fbe53b3c15fe6a8a0672cff7c7174ee27ea2d9fd[m
Author: dotnet-bot <dotnet-bot@microsoft.com>
Date:   Mon May 18 18:17:25 2015 +0800

    Add LICENSE file to WCF repo

[33mcommit db8bed2b13aa50bc9b477f18430e3ffa44bbab4b[m
Author: dotnet-bot <dotnet-bot@microsoft.com>
Date:   Thu May 14 01:49:39 2015 +0800

    Initial commit - scenario working branch

[33mcommit 9615d0849b776c08517e64c859dca1f3275db451[m
Author: dotnet-bot <dotnet-bot@microsoft.com>
Date:   Wed May 13 10:47:42 2015 -0700

    Add .gitmirrorall file
    
    [tfs-changeset: 1470020]

[33mcommit e5b2119ac81abfe63d851bf5df796c80938d778c[m
Author: dotnet-bot <dotnet-bot@microsoft.com>
Date:   Thu May 14 01:31:17 2015 +0800

    Initial commit
    
    [OperationContract]
    public string HelloWcf()
    {
        return "Hello Open World!";
    }
