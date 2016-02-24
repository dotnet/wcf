// Import the utility functionality.

import jobs.generation.Utilities;

def project = 'dotnet/wcf'

// Utility to move into dotnet-ci eventually

// **************************
// Define the basic inner loop builds for PR 
// **************************

// Loop over the options and build up the innerloop build matrix.

['Debug', 'Release'].each { configuration ->
    ['Linux', 'Windows_NT'].each { os ->
        // Calculate job name
        def osJobName = os.toLowerCase()
        if (osJobName == 'windows_nt') {
            osJobName = 'windows'
        }
        def configurationJobName = configuration.toLowerCase()
        def jobName = "${osJobName}_${configurationJobName}"
        
        def osAffinityName = os; 
        if (osAffinityName == 'Linux') {
            // our Linux runs should only ever run on Ubuntu14.04; we don't run on other flavours yet
            osAffinityName = 'Ubuntu14.04'
        }
        
        // **************************
        // Create the new commit job
        // **************************
        def newCommitJob

        if (osJobName == 'linux') {
            // Jobs run as a service in unix, which means that HOME variable is not set, and it is required for restoring packages
            // so we set it first, and then call build.sh
            newCommitJob = job(Utilities.getFullJobName(project, jobName, false)) {
                steps {
                    shell("HOME=\$WORKSPACE/tempHome ./build.sh /p:ShouldCreatePackage=false /p:ShouldGenerateNuSpec=false /p:OSGroup=${os} /p:Configuration=${os}_${configuration}")
                }
            }
        } else {
            // On other platforms, we run the build under Windows and then pack the results
            newCommitJob = job(Utilities.getFullJobName(project, jobName, false)) {
                steps {
                    // Use inline replacement
                    batchFile("build.cmd /p:Configuration=${os}_${configuration} /p:OSGroup=${os}")
                    // Pack up the results for max efficiency
                    batchFile("C:\\Packer\\Packer.exe .\\bin\\build.pack .\\bin")
                }
            }
        }
        
        Utilities.setMachineAffinity(newCommitJob, osAffinityName, 'latest-or-auto')

        // Add commit job options
        Utilities.addScm(newCommitJob, project)
        Utilities.addStandardNonPRParameters(newCommitJob)
        Utilities.addGithubPushTrigger(newCommitJob)
        
        // **************************
        // Create the new PR job
        // **************************
        def newPRJob

        if (osJobName == 'linux') {
            newPRJob = job(Utilities.getFullJobName(project, jobName, true)) {
                steps {
                    shell("HOME=\$WORKSPACE/tempHome ./build.sh /p:ShouldCreatePackage=false /p:ShouldGenerateNuSpec=false /p:OSGroup=${os} /p:Configuration=${os}_${configuration}")
                }
            }
        } else {
            newPRJob = job(Utilities.getFullJobName(project, jobName, true)) {
                steps {
                    // Use inline replacement
                    batchFile("build.cmd /p:Configuration=${os}_${configuration} /p:OSGroup=${os}")
                    // Pack up the results for max efficiency
                    batchFile("C:\\Packer\\Packer.exe .\\bin\\build.pack .\\bin")
                }
            }
        }
        
        Utilities.setMachineAffinity(newPRJob, osAffinityName, 'latest-or-auto')
        
        // Add a PR job options
        Utilities.addGithubPRTrigger(newPRJob, "Innerloop ${os} ${configuration} Build and Test")
        Utilities.addPRTestSCM(newPRJob, project)
        Utilities.addStandardPRParameters(newPRJob, project)
        
        // Add common options:
        
        [newPRJob, newCommitJob].each { newJob ->
            Utilities.addStandardOptions(newJob)
            
            Utilities.addXUnitDotNETResults(newJob, 'bin/tests/**/testResults.xml')
            
            if (os != 'Linux') {
                // We do not do the pack step on non-Linux builds
                Utilities.addArchival(newJob, "bin/${os}.AnyCPU.${configuration}/**,bin/build.pack")
            }
        }
    }
}

// **************************
// Define the code coverage jobs
// **************************

// Define build string
def codeCoverageBuildString = '''build.cmd /p:ShouldCreatePackage=false /p:ShouldGenerateNuSpec=false /p:OSGroup=Windows_NT /p:Configuration=Windows_NT_Debug /p:Coverage=true /p:WithCategories=\"InnerLoop;OuterLoop\"'''

// Generate a rolling (12 hr job) and a PR job that can be run on demand

def rollingCCJob = job(Utilities.getFullJobName(project, 'code_coverage_windows', false)) {
  label('windows-elevated')
  steps {
    batchFile(codeCoverageBuildString)
  }
}

def prCCJob = job(Utilities.getFullJobName(project, 'code_coverage_windows', true)) {
  label('windows-elevated')
  steps {
    batchFile(codeCoverageBuildString)
  }
}

// For both jobs, archive the coverage info and publish an HTML report
[rollingCCJob, prCCJob].each { newJob ->
    Utilities.addHtmlPublisher(newJob, 'bin/tests/coverage', 'Code Coverage Report', 'index.htm')
    Utilities.addArchival(newJob, '**/coverage/*,msbuild.log')
}

Utilities.addScm(rollingCCJob, project)
Utilities.addStandardOptions(rollingCCJob)
Utilities.addStandardNonPRParameters(rollingCCJob)
Utilities.addPeriodicTrigger(rollingCCJob, '@daily')
             
Utilities.addPRTestSCM(prCCJob, project)
Utilities.addStandardOptions(prCCJob)
Utilities.addStandardPRParameters(prCCJob, project)
Utilities.addGithubPRTrigger(prCCJob, 'Code Coverage Windows Debug', '@dotnet-bot test code coverage please')

// **************************
// Outerloop.  Rolling every 4 hours for debug and release
// **************************

['Debug', 'Release'].each { configuration ->
    def configurationJobName = configuration.toLowerCase()
    def jobName = "outerloop_windows_${configurationJobName}"
    
    // Create the new rolling job
    def newRollingJob = job(Utilities.getFullJobName(project, jobName, false)) {
        label('windows-elevated')
        steps {
            batchFile("build.cmd /p:Configuration=Windows_NT_${configuration} /p:WithCategories=OuterLoop")
        }
    }

    // Add commit job options
    Utilities.addScm(newRollingJob, project)
    Utilities.addStandardNonPRParameters(newRollingJob)
    Utilities.addPeriodicTrigger(newRollingJob, 'H H/4 * * *')
    
    // Create the new PR job for on demand execution.  No automatic PR trigger.
    // Triggered with '@dotnet-bot test outerloop please'
    
    def newPRJob = job(Utilities.getFullJobName(project, jobName, true)) {
        label('windows-elevated')
        steps {
            batchFile("build.cmd /p:Configuration=Windows_NT_${configuration} /p:WithCategories=OuterLoop")
        }
    }
    
    // Add a PR trigger
    Utilities.addGithubPRTrigger(newPRJob, "Outerloop Windows ${configuration} Build and Test", '@dotnet-bot test outerloop please')
    Utilities.addPRTestSCM(newPRJob, project)
    Utilities.addStandardPRParameters(newPRJob, project)
    
    // Add common options   
    [newPRJob, newRollingJob].each { newJob ->
        Utilities.addStandardOptions(newJob)
        Utilities.addXUnitDotNETResults(newJob, 'bin/tests/**/testResults.xml')
    }
}

// **************************
// Outerloop and Innerloop against the latest dependencies on Windows. Rolling daily for debug and release
// **************************

['Debug', 'Release'].each { configuration ->
    def configurationJobName = configuration.toLowerCase()
    def jobName = "latest_dependencies_windows_${configurationJobName}"
    def latestDepBuildString = '''build.cmd /p:Configuration=Windows_NT_${configuration}  /p:FloatingTestRuntimeDependencies=true /p:WithCategories=\"InnerLoop;OuterLoop\"'''

    // Create the new rolling job
    def newLatestDepRollingJob = job(Utilities.getFullJobName(project, jobName, false)) {
        label('windows-elevated')
        steps {
            batchFile(latestDepBuildString)
        }
    }

    // Add commit job options
    Utilities.addScm(newLatestDepRollingJob, project)
    Utilities.addStandardNonPRParameters(newLatestDepRollingJob)
    Utilities.addPeriodicTrigger(newLatestDepRollingJob, '@daily')

    // Create the new PR job for on demand execution.  No automatic PR trigger.
    // Triggered with '@dotnet-bot test latest dependencies please'

    def newLatestDepPRJob = job(Utilities.getFullJobName(project, jobName, true)) {
        label('windows-elevated')
        steps {
            batchFile(latestDepBuildString)
        }
    }

    // Add a PR trigger
    Utilities.addGithubPRTrigger(newLatestDepPRJob, "Latest dependencies Windows ${configuration} Build and Test", '@dotnet-bot test latest dependencies please')
    Utilities.addPRTestSCM(newLatestDepPRJob, project)
    Utilities.addStandardPRParameters(newLatestDepPRJob, project)

    // Add common options
    [newLatestDepPRJob, newLatestDepRollingJob].each { newJob ->
        Utilities.addStandardOptions(newJob)
        Utilities.addXUnitDotNETResults(newJob, 'bin/tests/**/testResults.xml')
    }
}
