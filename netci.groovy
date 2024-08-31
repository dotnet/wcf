// Import the utility functionality.

import jobs.generation.Utilities;
import jobs.generation.JobReport;

// The input project name (e.g. dotnet/corefx)
def project = GithubProject
// The input branch name (e.g. master)
def branch = GithubBranchName

// Globals

// Map of osName -> osGroup.
def osGroupMap = ['Ubuntu':'Linux',
                  'Ubuntu14.04':'Linux',
                  'Ubuntu16.04':'Linux',
                  'Debian8.4':'Linux',
                  'OSX10.12':'OSX',
                  'Windows_NT':'Windows_NT',
                  'CentOS7.1': 'Linux',
                  'RHEL7.2': 'Linux']

// Map of osName -> nuget runtime
def targetNugetRuntimeMap = ['OSX10.12' : 'osx.10.12-x64',
                             'Ubuntu' : 'ubuntu.14.04-x64',
                             'Ubuntu14.04' : 'ubuntu.14.04-x64',
                             'Ubuntu16.04' : 'ubuntu.16.04-x64',
                             'Debian8.4' : 'debian.8-x64',
                             'CentOS7.1' : 'centos.7-x64',
                             'RHEL7.2': 'rhel.7-x64']

def osShortName = ['Windows 10': 'win10',
                   'Windows 7' : 'win7',
                   'Windows_NT' : 'windows_nt',
                   'Ubuntu14.04' : 'ubuntu14.04',
                   'OSX10.12' : 'osx',
                   'Windows Nano 2016' : 'winnano16',
                   'Ubuntu16.04' : 'ubuntu16.04',
                   'CentOS7.1' : 'centos7.1',
                   'Debian8.4' : 'debian8.4',
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
        // Strictly speaking this file only needs to specify the starting point for the serial numbers of the branch it is in
        // We are showing the starting points of other branches for the sake of clarity
        if (branch.toLowerCase() == "release/1.0.0") {
            currentWcfPRService = wcfPRServiceCount + 100
        } else if (branch.toLowerCase() == "release/1.1.0") {
            currentWcfPRService = wcfPRServiceCount + 150
        } else if (branch.toLowerCase() == "release/2.0.0") {
            currentWcfPRService = wcfPRServiceCount + 200
        } else if (branch.toLowerCase() == "release/2.1.0") {
            currentWcfPRService = wcfPRServiceCount + 250
        } else if (branch.toLowerCase() == "release/uwp6.0") {
            currentWcfPRService = wcfPRServiceCount + 300
        } else if (branch.toLowerCase() == "release/uwp6.1") {
            currentWcfPRService = wcfPRServiceCount + 350
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
    
    wcfUtilities.addWcfOuterloopTestServiceSync(newJob, os, branch, isPR)
    
    newJob.with {
        steps {
            batchFile("build.cmd -coverage -outerloop -${configurationGroup} -- /p:ShouldGenerateNuSpec=false /p:OSGroup=${osGroupMap[os]} /p:ServiceUri=%WcfServiceUri%")
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
        def targetGroup = "netcoreapp"
        
        newJob.with {
            steps {
                batchFile("build.cmd -framework:${targetGroup} -${configurationGroup} -os:${osGroupMap[os]}")
                batchFile("build-tests.cmd -framework:${targetGroup} -${configurationGroup} -os:${osGroupMap[os]} -outerloop -- /p:IsCIBuild=true")
            }
        }

        // Set affinity for elevated machines
        Utilities.setMachineAffinity(newJob, os, 'latest-or-auto-elevated')

        // Set up standard options.
        Utilities.standardJobSetup(newJob, project, isPR, "*/${branch}")
        // Add the unit test results
        Utilities.addXUnitDotNETResults(newJob, 'bin/**/testResults.xml')
        
        // Set up appropriate triggers. PR on demand, otherwise on change pushed
        if (isPR) {
            // Set PR trigger.
            Utilities.addGithubPRTriggerForBranch(newJob, branch, "OuterLoop Selfhost ${os} ${configurationGroup}", "(?i).*test\\W+(all\\W+outerloop|outerloop\\W+selfhost\\W+${os}).*", false /*triggerOnPhraseOnly*/)
        } 
        else {
            // Set a push trigger
            Utilities.addGithubPushTrigger(newJob)
        }
    } 
} 

// **************************
// Define outerloop testing for OSes that can build and run.  Run locally on each machine.
// Subset runs on every PR, the ones that don't run per PR can be requested via a magic phrase
// **************************

def supportedFullCycleOuterloopPlatforms = ['Windows_NT', 'Ubuntu14.04', 'Ubuntu16.04', 'Debian8.4', 'CentOS7.1', 'RHEL7.2', 'OSX10.12']
[true, false].each { isPR ->
    configurationGroupList.each { configurationGroup ->
        supportedFullCycleOuterloopPlatforms.each { os ->
            def newJobName = "outerloop_${os.toLowerCase()}_${configurationGroup.toLowerCase()}"
            def newJob = job(Utilities.getFullJobName(project, newJobName, isPR))
            def targetGroup = "netcoreapp"
            
            wcfUtilities.addWcfOuterloopTestServiceSync(newJob, os, branch, isPR)
            
            if (osGroupMap[os] == 'Windows_NT') {
                newJob.with {
                    steps {
                        batchFile("build.cmd -framework:${targetGroup} -${configurationGroup} -os:${osGroupMap[os]}")
                        batchFile("build-tests.cmd -framework:${targetGroup} -${configurationGroup} -os:${osGroupMap[os]} -outerloop -- /p:ServiceUri=%WcfServiceUri% /p:SSL_Available=true /p:Root_Certificate_Installed=true /p:Client_Certificate_Installed=true /p:Peer_Certificate_Installed=true /p:IsCIBuild=true")
                    }
                }
            } 
            else {
                newJob.with {
                    steps {
                        shell("HOME=\$WORKSPACE/tempHome ./build.sh -${configurationGroup.toLowerCase()}")
                        shell("HOME=\$WORKSPACE/tempHome ./build-tests.sh -${configurationGroup.toLowerCase()} -outerloop -testWithLocalLibraries -- /p:OSGroup=${osGroupMap[os]} /p:ServiceUri=\$WcfServiceUri /p:SSL_Available=true /p:IsCIBuild=true")
                    }
                }
            }

            // Set the affinity.  OS name matches the machine affinity.
            if (os == 'Windows_NT' || os == 'OSX10.12') {
                // Set affinity for elevated machines on Windows
                Utilities.setMachineAffinity(newJob, os, 'latest-or-auto-elevated')
            } 
            else if (osGroupMap[os] == 'Linux') {
                Utilities.setMachineAffinity(newJob, os, "outer-latest-or-auto")
            } 
            else {
                Utilities.setMachineAffinity(newJob, os, 'latest-or-auto')
            }

            // Set up standard options.
            Utilities.standardJobSetup(newJob, project, isPR, "*/${branch}")
            // Add the unit test results
            Utilities.addXUnitDotNETResults(newJob, 'bin/**/testResults.xml')
            // Add archival for the built data.
            Utilities.addArchival(newJob, "msbuild.log", '', doNotFailIfNothingArchived=true, archiveOnlyIfSuccessful=false)

            // Our outerloops rely on us calling WcfPRServiceUri to sync server code, after which the client 
            // will test against WcfServiceUri.
            // The current design limitation means that if we allow concurrent builds, it becomes possible to pave over 
            // the server endpoint with mismatched code while another test is running.
            // Due to this design limitation, we have to disable concurrent builds for outerloops 
            newJob.concurrentBuild(false)

            // Set up appropriate triggers. PR on demand, otherwise daily
            if (isPR) {
                // Set PR trigger.
                if ( os == 'Windows_NT' || os == 'Ubuntu14.04' || os == 'CentOS7.1' || os == 'OSX10.12' ) {
                    Utilities.addGithubPRTriggerForBranch(newJob, branch, "OuterLoop ${os} ${configurationGroup}", "(?i).*test\\W+(all\\W+outerloop|outerloop\\W+${os}).*", false /*triggerOnPhraseOnly*/)
                } 
                else {                  
                    Utilities.addGithubPRTriggerForBranch(newJob, branch, "OuterLoop ${os} ${configurationGroup}", "(?i).*test\\W+(all\\W+outerloop|outerloop\\W+${os}).*", true /*triggerOnPhraseOnly*/)
                } 
            } 
            else {
                // Set a periodic trigger, runs daily regardless of whether a change happened
                Utilities.addPeriodicTrigger(newJob, '@daily', true /*alwaysRuns*/)

                // Set a push trigger
                Utilities.addGithubPushTrigger(newJob)
            }
        }
    } 
} 

// **************************
// Define innerloop testing for OSes that can build and run.  Run locally on each machine.
// Subset runs on every PR, the ones that don't run per PR can be requested via a magic phrase
// **************************

def supportedFullCycleInnerloopPlatforms = ['Windows_NT', 'Ubuntu14.04', 'Ubuntu16.04', 'Debian8.4', 'CentOS7.1', 'RHEL7.2', 'OSX10.12']
[true, false].each { isPR ->
    configurationGroupList.each { configurationGroup ->
        supportedFullCycleInnerloopPlatforms.each { os -> 
            def newJobName = "${os.toLowerCase()}_${configurationGroup.toLowerCase()}"
            def targetGroup = "netcoreapp"
            
            def newJob = job(Utilities.getFullJobName(project, newJobName, isPR))
            
            if (osGroupMap[os] == 'Windows_NT')
            {
                newJob.with {
                    steps {
                        batchFile("call \"C:\\Program Files (x86)\\Microsoft Visual Studio 14.0\\VC\\vcvarsall.bat\" x86 && build.cmd -${configurationGroup} -os:${osGroupMap[os]} -framework:${targetGroup}")
                        batchFile("call \"C:\\Program Files (x86)\\Microsoft Visual Studio 14.0\\VC\\vcvarsall.bat\" x86 && build-tests.cmd -${configurationGroup} -os:${osGroupMap[os]} -framework:${targetGroup} -- /p:IsCIBuild=true")
                        batchFile("C:\\Packer\\Packer.exe .\\bin\\build.pack .\\bin")
                    }
                }
            } 
            else {
                newJob.with {
                    steps {
                    def useServerGC = (configurationGroup == 'Release' && isPR) ? 'useServerGC' : ''
                        shell("HOME=\$WORKSPACE/tempHome ./build.sh -${configurationGroup.toLowerCase()} -framework:${targetGroup} -os:${osGroupMap[os]}")
                        shell("HOME=\$WORKSPACE/tempHome ./build-tests.sh -${configurationGroup.toLowerCase()} -framework:${targetGroup} -os:${osGroupMap[os]} -- ${useServerGC} /p:IsCIBuild=true")
                    }
                }
            }
            
            // Set the affinity  
            Utilities.setMachineAffinity(newJob, os, 'latest-or-auto')
            // Set up standard options
            Utilities.standardJobSetup(newJob, project, isPR, "*/${branch}")
            // Add the unit test results
            Utilities.addXUnitDotNETResults(newJob, 'bin/**/testResults.xml')

            def archiveContents = "msbuild.log"

            // Add archival for the built data
            if (os.contains('Windows')) {
                archiveContents += ",bin/build.pack"
            } 
            else {
                archiveContents != ",bin/build.tar.gz"
            }
            
            Utilities.addArchival(newJob, archiveContents, '', doNotFailIfNothingArchived=true, archiveOnlyIfSuccessful=false)

            // Set up triggers
            if (isPR) {
                // Set PR trigger
                if ( os == 'Windows_NT' || os == 'Ubuntu14.04' || os == 'CentOS7.1' || os == 'OSX10.12' ) {
                    Utilities.addGithubPRTriggerForBranch(newJob, branch, "InnerLoop ${os} ${configurationGroup}", "(?i).*test\\W+(all\\W+innerloop|innerloop\\W+${os}).*", false /*triggerOnPhraseOnly*/)
                } 
                else {
                    Utilities.addGithubPRTriggerForBranch(newJob, branch, "InnerLoop ${os} ${configurationGroup}", "(?i).*test\\W+(all\\W+innerloop|innerloop\\W+${os}).*", true /*triggerOnPhraseOnly*/)
                }
            } 
            else {
                // Set a push trigger
                Utilities.addGithubPushTrigger(newJob)
            }
        }
    }
}

JobReport.Report.generateJobReport(out)

Utilities.createHelperJob(this, project, branch,
    "Welcome to the ${project} repository",  // This is prepended to the help message
    "Have a nice day!")  // This is appended to the help message.  You might put known issues here.

