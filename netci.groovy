// Import the utility functionality.

import jobs.generation.Utilities;
import jobs.generation.JobReport;

// The input project name (e.g. dotnet/corefx)
def project = GithubProject
// The input branch name (e.g. master)
def branch = GithubBranchName

// Globals

// Map of os -> osGroup.
def osGroupMap = ['Ubuntu':'Linux',
                  'Ubuntu14.04':'Linux',
                  'Ubuntu16.04':'Linux',
                  'Debian8.4':'Linux',
                  'OSX':'OSX',
                  'Windows_NT':'Windows_NT',
                  'CentOS7.1': 'Linux',
                  'OpenSUSE13.2': 'Linux',
                  'RHEL7.2': 'Linux']

// Map of os -> nuget runtime
def targetNugetRuntimeMap = ['OSX' : 'osx.10.10-x64',
                             'Ubuntu' : 'ubuntu.14.04-x64',
                             'Ubuntu14.04' : 'ubuntu.14.04-x64',
                             'Ubuntu16.04' : 'ubuntu.16.04-x64',
                             'Fedora23' : 'fedora.23-x64',
                             'Debian8.4' : 'debian.8-x64',
                             'CentOS7.1' : 'centos.7-x64',
                             'OpenSUSE13.2' : 'opensuse.13.2-x64',
                             'RHEL7.2': 'rhel.7-x64']

def osShortName = ['Windows 10': 'win10',
                   'Windows 7' : 'win7',
                   'Windows_NT' : 'windows_nt',
                   'Ubuntu14.04' : 'ubuntu14.04',
                   'OSX' : 'osx',
                   'Windows Nano 2016' : 'winnano16',
                   'Ubuntu16.04' : 'ubuntu16.04',
                   'CentOS7.1' : 'centos7.1',
                   'Debian8.4' : 'debian8.4',
                   'OpenSUSE13.2' : 'opensuse13.2',
                   'Fedora23' : 'fedora23',
                   'RHEL7.2' : 'rhel7.2']

def configurationGroupList = ['Debug', 'Release']

// **************************
// Utilities shared for WCF Core builds
// **************************
class WcfUtilities
{
    def wcfPRServiceCount = 0 
    
    // Outerloop jobs for WCF Core require an external server reference
    // Jenkins provides the correct parameters to the scripts to get the correct PR ID or branch
    def addWcfOuterloopTestServiceSync(def job, String os, String branch, boolean isPR) { 

        def operation = isPR ? "pr" : "branch"
        def currentWcfPRService = ++wcfPRServiceCount 

        // workaround after branchifying - each branch independently runs this file hence our serial
        // numbers will overlap on different branches
        if (branch.toLowerCase() == "release/1.0.0") {
            currentWcfPRService = wcfPRServiceCount + 100
        }

        job.with { 
            parameters {
                stringParam('WcfServiceUri', "wcfcoresrv2.cloudapp.net/WcfService${currentWcfPRService}", 'Wcf OuterLoop Test Service Uri')
                stringParam('WcfPRServiceUri', "http://wcfcoresrv2.cloudapp.net/PRServiceMaster/pr.ashx", 'Wcf OuterLoop Test PR Service Uri')
                stringParam('WcfPRServiceId', "${currentWcfPRService}", 'Wcf OuterLoop Test PR Service Id')
            }
        }
        if (os.toLowerCase().contains("windows")) {
            job.with { 
                steps {
                    batchFile(".\\src\\System.Private.ServiceModel\\tools\\scripts\\sync-pr.cmd %WcfPRServiceId% ${operation} %WcfPRServiceUri%")
                }           
            }
        } 
        else {
            job.with { 
                steps {
                   shell("HOME=\$WORKSPACE/tempHome ./src/System.Private.ServiceModel/tools/scripts/sync-pr.sh \$WcfPRServiceId ${operation} \$WcfPRServiceUri")
                }
            }
        }
    }
}

wcfUtilities = new WcfUtilities()

// **************************
// Define the code coverage jobs
// **************************

[true, false].each { isPR -> 
    def os = "Windows_NT"
    def configurationGroup = "Debug"
    def newJobName = "code_coverage_${os.toLowerCase()}_${configurationGroup.toLowerCase()}"
    
    // Create the new rolling job
    def newJob = job(Utilities.getFullJobName(project, newJobName, isPR))
    
    wcfUtilities.addWcfOuterloopTestServiceSync(newJob, os, "*/${branch}", isPR)
    
    newJob.with {
        steps {
            batchFile("build.cmd /p:ShouldCreatePackage=false /p:ShouldGenerateNuSpec=false /p:OSGroup=${osGroupMap[os]} /p:ConfigurationGroup=${configurationGroup} /p:Coverage=true /p:WithCategories=\"InnerLoop;OuterLoop\" /p:ServiceUri=%WcfServiceUri%")
        }
    }

    // Set affinity for elevated machines
    Utilities.setMachineAffinity(newJob, os, 'latest-or-auto-elevated')
    // Set up standard options.
    Utilities.standardJobSetup(newJob, project, isPR, "*/${branch}")
    // Add code coverage report
    Utilities.addHtmlPublisher(newJob, 'bin/tests/coverage', 'Code Coverage Report', 'index.htm')
    // Archive results
    Utilities.addArchival(newJob, '**/coverage/*,msbuild.log')
    
    // Our outerloops rely on us calling WcfPRServiceUri to sync server code, after which the client 
    // will test against WcfServiceUri.
    // The current design limitation means that if we allow concurrent builds, it becomes possible to pave over 
    // the server endpoint with mismatched code while another test is running.
    // Due to this design limitation, we have to disable concurrent builds for outerloops 
    newJob.concurrentBuild(false)

    // Set triggers
    if (isPR)
    {
        Utilities.addGithubPRTriggerForBranch(newJob, branch, "Code Coverage Windows_NT ${configurationGroup}", '(?i).*test\\W+code\\W*coverage.*')
    } 
    else {
        Utilities.addPeriodicTrigger(newJob, '@daily')
    }
}

// **************************
// Define outerloop testing on Windows_NT for seflhosted service
// Note: This outerloop run can run concurrently unlike other ones due to the run being entirely self-contained 
// **************************

[true, false].each { isPR -> 
    configurationGroupList.each { configurationGroup ->
        def os = 'Windows_NT'
        def newJobName = "outerloop_selfhost_${os.toLowerCase()}_${configurationGroup.toLowerCase()}"
        def newJob = job(Utilities.getFullJobName(project, newJobName, isPR))
        
        newJob.with {
            steps {
                batchFile("build.cmd /p:ConfigurationGroup=${configurationGroup} /p:OSGroup=${osGroupMap[os]} /p:WithCategories=OuterLoop")
            }
        }

        // Set affinity for elevated machines
        Utilities.setMachineAffinity(newJob, os, 'latest-or-auto-elevated')

        // Set up standard options.
        Utilities.standardJobSetup(newJob, project, isPR, "*/${branch}")
        // Add the unit test results
        Utilities.addXUnitDotNETResults(newJob, 'bin/tests/**/testResults.xml')
        
        // Set up appropriate triggers. PR on demand, otherwise daily
        if (isPR) {
            // Set PR trigger.
            Utilities.addGithubPRTriggerForBranch(newJob, branch, "OuterLoop Selfhost ${os} ${configurationGroup}", "(?i).*test\\W+(all\\W+outerloop|outerloop\\W+selfhost\\W+${os}).*", false /*triggerOnPhraseOnly*/)
        } 
        else {
            // Set a periodic trigger
            Utilities.addPeriodicTrigger(newJob, '@daily')
        }
    } 
} 

// **************************
// Define outerloop testing for OSes that can build and run.  Run locally on each machine.
// **************************

def supportedFullCycleOuterloopPlatforms = ['Windows_NT', 'Ubuntu14.04', 'CentOS7.1', 'OSX']
[true, false].each { isPR ->
    configurationGroupList.each { configurationGroup ->
        supportedFullCycleOuterloopPlatforms.each { os ->
            def newJobName = "outerloop_${os.toLowerCase()}_${configurationGroup.toLowerCase()}"
            def newJob = job(Utilities.getFullJobName(project, newJobName, isPR))
            
            wcfUtilities.addWcfOuterloopTestServiceSync(newJob, os, "*/${branch}", isPR)
            
            if (osGroupMap[os] == 'Windows_NT') {
                newJob.with {
                    steps {
                        batchFile("build.cmd /p:ConfigurationGroup=${configurationGroup} /p:OSGroup=${osGroupMap[os]} /p:WithCategories=OuterLoop /p:ServiceUri=%WcfServiceUri%")
                    }
                }
            } 
            else {
                newJob.with {
                    steps {
                        shell("HOME=\$WORKSPACE/tempHome ./build.sh /p:ConfigurationGroup=${configurationGroup} /p:OSGroup=${osGroupMap[os]} /p:WithCategories=OuterLoop /p:TestWithLocalLibraries=true /p:ServiceUri=\$WcfServiceUri")
                    }
                }
            }

            // Set the affinity.  OS name matches the machine affinity.
            if (os == 'Windows_NT') {
                // Set affinity for elevated machines on Windows
                Utilities.setMachineAffinity(newJob, os, 'latest-or-auto-elevated')
            } 
            else if (os == 'Ubuntu14.04' || os == 'CentOS7.1') {
                Utilities.setMachineAffinity(newJob, os, "outer-latest-or-auto")
            } 
            else {
                Utilities.setMachineAffinity(newJob, os, 'latest-or-auto')
            }

            // Set up standard options.
            Utilities.standardJobSetup(newJob, project, isPR, "*/${branch}")
            // Add the unit test results
            Utilities.addXUnitDotNETResults(newJob, 'bin/tests/**/testResults.xml')
            
            // Our outerloops rely on us calling WcfPRServiceUri to sync server code, after which the client 
            // will test against WcfServiceUri.
            // The current design limitation means that if we allow concurrent builds, it becomes possible to pave over 
            // the server endpoint with mismatched code while another test is running.
            // Due to this design limitation, we have to disable concurrent builds for outerloops 
            newJob.concurrentBuild(false)

            // Set up appropriate triggers. PR on demand, otherwise daily
            if (isPR) {
                // Set PR trigger.
                Utilities.addGithubPRTriggerForBranch(newJob, branch, "OuterLoop ${os} ${configurationGroup}", "(?i).*test\\W+(all\\W+outerloop|outerloop\\W+${os}).*", false /*triggerOnPhraseOnly*/)
            } 
            else {
                // Set a periodic trigger
                Utilities.addPeriodicTrigger(newJob, '@daily')
            }
        }
    } 
} 

// **************************
// Define innerloop testing for OSes that can build and run.  Run locally on each machine.
// **************************

def supportedFullCycleInnerloopPlatforms = ['Windows_NT', 'Ubuntu14.04', 'CentOS7.1', 'OSX']
[true, false].each { isPR ->
    configurationGroupList.each { configurationGroup ->
        supportedFullCycleInnerloopPlatforms.each { os -> 
            def newJobName = "${os.toLowerCase()}_${configurationGroup.toLowerCase()}"
            
            def newJob = job(Utilities.getFullJobName(project, newJobName, isPR))
            
            if (osGroupMap[os] == 'Windows_NT')
            {
                newJob.with {
                    steps {
                        batchFile("call \"C:\\Program Files (x86)\\Microsoft Visual Studio 14.0\\VC\\vcvarsall.bat\" x86 && build.cmd /p:ConfigurationGroup=${configurationGroup} /p:OSGroup=${osGroupMap[os]}")
                        batchFile("C:\\Packer\\Packer.exe .\\bin\\build.pack .\\bin")
                    }
                }
            } 
            else {
                newJob.with {
                    steps {
                        shell("HOME=\$WORKSPACE/tempHome ./build.sh /p:ShouldCreatePackage=false /p:ShouldGenerateNuSpec=false /p:OSGroup=${osGroupMap[os]} /p:ConfigurationGroup=${configurationGroup}")
                    }
                }
            }
            
            // Set the affinity.  All of these run on Windows currently.
            Utilities.setMachineAffinity(newJob, os, 'latest-or-auto')
            // Set up standard options.
            Utilities.standardJobSetup(newJob, project, isPR, "*/${branch}")
            // Add the unit test results
            Utilities.addXUnitDotNETResults(newJob, 'bin/tests/**/testResults.xml')
            // Add archival for the built data
            if (osGroupMap[os] == 'Windows_NT') {
                Utilities.addArchival(newJob, "bin/build.pack,bin/${osGroupMap[os]}.AnyCPU.${configurationGroup}/**,bin/ref/**,bin/packages/**,msbuild.log")
            } 
            else {
                Utilities.addArchival(newJob, "bin/${osGroupMap[os]}.AnyCPU.${configurationGroup}/**,bin/ref/**,bin/packages/**,msbuild.log")
            }
            
            // Set up triggers
            if (isPR) {
                // Set PR trigger.
                Utilities.addGithubPRTriggerForBranch(newJob, branch, "Innerloop ${os} ${configurationGroup}")
            } 
            else {
                // Set a push trigger
                Utilities.addGithubPushTrigger(newJob)
            }
        }
    }
}

JobReport.Report.generateJobReport(out)
