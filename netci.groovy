// Import the utility functionality.

import jobs.generation.Utilities;

def project = GithubProject

// Globals

// Map of os -> osGroup.
def osGroupMap = ['Ubuntu':'Linux',
                  'Ubuntu14.04':'Linux',
                  'Ubuntu15.10':'Linux',
                  'Debian8.2':'Linux',
                  'OSX':'OSX',
                  'Windows_NT':'Windows_NT',
                  'CentOS7.1': 'Linux',
                  'OpenSUSE13.2': 'Linux',
                  'RHEL7.2': 'Linux']
// Map of os -> nuget runtime
def targetNugetRuntimeMap = ['OSX' : 'osx.10.10-x64',
                             'Ubuntu' : 'ubuntu.14.04-x64',
                             'Ubuntu15.10' : 'ubuntu.14.04-x64',
                             'Debian8.2' : 'ubuntu.14.04-x64',
                             'CentOS7.1' : 'centos.7-x64',
                             'OpenSUSE13.2' : 'ubuntu.14.04-x64',
                             'RHEL7.2': 'rhel.7-x64']
def branchList = ['master', 'pr', 'rc2']
def osShortName = ['Windows 10': 'win10',
                   'Windows 7' : 'win7',
                   'Windows_NT' : 'windows_nt',
                   'Ubuntu14.04' : 'ubuntu14.04',
                   'OSX' : 'osx',
                   'Windows Nano' : 'winnano',
                   'Ubuntu15.10' : 'ubuntu15.10',
                   'CentOS7.1' : 'centos7.1',
                   'OpenSUSE13.2' : 'opensuse13.2',
                   'RHEL7.2' : 'rhel7.2']                  

def static getFullBranchName(def branch) {
    def branchMap = ['master':'*/master',
        'rc2':'*/release/1.0.0-rc2',
        'pr':'*/master']
    def fullBranchName = branchMap.get(branch, null)
    assert fullBranchName != null : "Could not find a full branch name for ${branch}"
    return branchMap[branch]
}

def static getJobName(def name, def branchName) {
    def baseName = name
    if (branchName == 'rc2') {
        baseName += "_rc2"
    }
    return baseName
}

def configurationGroupList = ['Debug', 'Release']

def branch = GithubBranchName

// **************************
// Utilities shared for WCF Core builds
// **************************
class WcfUtilities
{
    def wcfPRServiceCount = 0 
    
    // Outerloop jobs for WCF Core require an external server reference
    // This should be run 
    def addWcfOuterloopTestServiceSync(def job, String os, String branch, boolean isPR) { 

        // Exclude rc2 branch, since that branch will not have the sync scripts in
        if (branch.toLowerCase().contains("rc2")) {
            return 
        }

        wcfPRServiceCount++

        def operation = isPR ? "pr" : "branch"

        job.with { 
            parameters {
                stringParam('WcfServiceUri', "wcfcoresrv2.cloudapp.net/WcfService${wcfPRServiceCount}", 'Wcf OuterLoop Test Service Uri')
                stringParam('WcfPRServiceUri', "http://wcfcoresrv2.cloudapp.net/PRServiceMaster/pr.ashx", 'Wcf OuterLoop Test PR Service Uri')
                stringParam('WcfPRServiceId', "${wcfPRServiceCount}", 'Wcf OuterLoop Test PR Service Id')
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

branchList.each { branchName -> 
    def isPR = (branchName == 'pr')
    def os = "Windows_NT"
    def configurationGroup = "Debug"
    def newJobName = "code_coverage_${os.toLowerCase()}_${configurationGroup.toLowerCase()}"
    
    // Create the new rolling job
    def newJob = job(getJobName(Utilities.getFullJobName(project, newJobName, isPR), branchName))
    
    wcfUtilities.addWcfOuterloopTestServiceSync(newJob, os, branchName, isPR)
    
    newJob.with {
        steps {
            batchFile("build.cmd /p:ShouldCreatePackage=false /p:ShouldGenerateNuSpec=false /p:OSGroup=${osGroupMap[os]} /p:ConfigurationGroup=${configurationGroup} /p:Coverage=true /p:WithCategories=\"InnerLoop;OuterLoop\" /p:ServiceUri=%WcfServiceUri%")
        }
    }

    // Set affinity for elevated machines
    Utilities.setMachineAffinity(newJob, os, 'latest-or-auto-elevated')
    // Set up standard options.
    Utilities.standardJobSetup(newJob, project, isPR, getFullBranchName(branchName))
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
        Utilities.addGithubPRTrigger(newJob, "Code Coverage Windows_NT ${configurationGroup}", '(?i).*test\\W+code\\W*coverage.*')
    } 
    else {
        Utilities.addPeriodicTrigger(newJob, '@daily')
    }
}

// **************************
// Define outerloop testing on Windows_NT for seflhosted service
// Note: This outerloop run can run concurrently unlike other ones due to the run being entirely self-contained 
// **************************

['master', 'pr'].each { branchName ->   // only master and pr for now for branches to test 
    configurationGroupList.each { configurationGroup ->
        def os = 'Windows_NT'
        def isPR = (branchName == 'pr')
        def newJobName = "outerloop_selfhost_${os.toLowerCase()}_${configurationGroup.toLowerCase()}"
        def newJob = job(getJobName(Utilities.getFullJobName(project, newJobName, isPR), branchName))
        
        newJob.with {
            steps {
                batchFile("build.cmd /p:ConfigurationGroup=${configurationGroup} /p:OSGroup=${osGroupMap[os]} /p:WithCategories=OuterLoop")
            }
        }

        // Set affinity for elevated machines
        Utilities.setMachineAffinity(newJob, os, 'latest-or-auto-elevated')

        // Set up standard options.
        Utilities.standardJobSetup(newJob, project, isPR, getFullBranchName(branchName))
        // Add the unit test results
        Utilities.addXUnitDotNETResults(newJob, 'bin/tests/**/testResults.xml')
        
        // Set up appropriate triggers. PR on demand, otherwise daily
        if (isPR) {
            // Set PR trigger.
            Utilities.addGithubPRTrigger(newJob, "OuterLoop Selfhost ${os} ${configurationGroup}", "(?i).*test\\W+(all\\W+outerloop|outerloop\\W+selfhost\\W+${os}).*")
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
branchList.each { branchName ->
    configurationGroupList.each { configurationGroup ->
        supportedFullCycleOuterloopPlatforms.each { os ->
            def isPR = (branchName == 'pr')
            def newJobName = "outerloop_${os.toLowerCase()}_${configurationGroup.toLowerCase()}"
            def newJob = job(getJobName(Utilities.getFullJobName(project, newJobName, isPR), branchName))
            
            wcfUtilities.addWcfOuterloopTestServiceSync(newJob, os, branchName, isPR)
            
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
            Utilities.standardJobSetup(newJob, project, isPR, getFullBranchName(branchName))
            // Add the unit test results
            Utilities.addXUnitDotNETResults(newJob, 'bin/tests/**/testResults.xml')
            
            // Our outerloops rely on us calling WcfPRServiceUri to sync server code, after which the client 
            // will test against WcfServiceUri.
            // The current design limitation means that if we allow concurrent builds, it becomes possible to pave over 
            // the server endpoint with mismatched code while another test is running.
            // Due to this design limitation, we have to disable concurrent builds for outerloops 
            newJob.concurrentBuild(false)

            // Skip outerloop testing on rc2 branch on non-WinNT platforms
            // we are incapable of running outerloops in CI due to the dependency on the Bridge
            if (branchName == 'rc2' && os != 'Windows_NT') {
                newJob.disabled(true)
            }

            // Set up appropriate triggers. PR on demand, otherwise daily
            if (isPR) {
                // Set PR trigger.
                Utilities.addGithubPRTrigger(newJob, "OuterLoop ${os} ${configurationGroup}", "(?i).*test\\W+(all\\W+outerloop|outerloop\\W+${os}).*")
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
branchList.each { branchName ->
    configurationGroupList.each { configurationGroup ->
        supportedFullCycleInnerloopPlatforms.each { os -> 
            def isPR = (branchName == 'pr')
            def newJobName = "${os.toLowerCase()}_${configurationGroup.toLowerCase()}"
            
            def newJob = job(getJobName(Utilities.getFullJobName(project, newJobName, isPR), branchName)) 
            
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
            Utilities.standardJobSetup(newJob, project, isPR, getFullBranchName(branchName))
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
                Utilities.addGithubPRTrigger(newJob, "Innerloop ${os} ${configurationGroup}")
            } 
            else {
                // Set a push trigger
                Utilities.addGithubPushTrigger(newJob)
            }
        }
    }
}
