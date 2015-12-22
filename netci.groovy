// Import the utility functionality.

import jobs.generation.Utilities;

def project = 'dotnet/wcf'

// Utility to move into dotnet-ci eventually

// **************************
// Define the basic inner loop builds for PR 
// **************************

// Create the test only builds for Linux
// The test only build utilizes the artifacts from other jobs
// as well as an upstream job in order to execute the runs on Linux.
// Create one for PR and one for Regular
[true, false].each { isPR ->
    ['Debug', 'Release'].each { configuration ->
        def configurationJobName = configuration.toLowerCase()
        def jobName = "linux_${configurationJobName}_tst"
        
        def linuxTestJob = job(Utilities.getFullJobName(project, jobName, isPR)) {
            label('ubuntu')
            
            parameters {
                stringParam('WCF_LINUX_BUILD_NUMBER', '', 'Build number to copy WCF Linux build artifacts from')
            }
            steps {
                // Copy artifacts from all of the required upstream jobs
            
                copyArtifacts('dotnet_coreclr/release_ubuntu') {
                    excludePatterns('**/testResults.xml', '**/*.ni.dll')
                    buildSelector {
                        latestSuccessful(true)
                    }
                    targetDirectory('coreclr')
                }
                
                copyArtifacts('dotnet_coreclr/release_windows_nt') {
                    includePatterns('bin/Product/Linux*/**')
                    excludePatterns('**/testResults.xml', '**/*.ni.dll')
                    buildSelector {
                        latestSuccessful(true)
                    }
                    targetDirectory('coreclr')
                }
                
                copyArtifacts('dotnet_corefx/nativecomp_ubuntu_debug') {
                    includePatterns('bin/**')
                    buildSelector {
                        latestSuccessful(true)
                    }
                    targetDirectory('corefx')
                }
                
                copyArtifacts('dotnet_corefx/ubuntu_debug_bld') {
                    excludePatterns('**/testResults.xml', '**/*.ni.dll')
                    buildSelector {
                        latestSuccessful(true)
                    }
                    targetDirectory('corefx')
                }
                
                // The input WCF build is specified by parameter.  See below for the flow job
                // that triggers the Linux build, then passes that build result to this build.
                // Upstream job is the PR test job.  Note we need the fully qualified job name
                // in order to copy artifacts.
                def linuxBuildName = Utilities.getFolderName(project) + '/' + 
                    Utilities.getFullJobName(project, "linux_${configurationJobName}_bld", isPR)
                
                copyArtifacts(linuxBuildName) {
                    includePatterns('bin/build.pack')
                    buildSelector {
                        buildNumber('${WCF_LINUX_BUILD_NUMBER}')
                    }
                }
            
                // Unpack
                shell("unpacker ./bin/build.pack ./bin")
                shell("""
./run-test.sh --coreclr-bins \${WORKSPACE}/coreclr/bin/Product/Linux.x64.Release \\
--mscorlib-bins \${WORKSPACE}/coreclr/bin/Product/Linux.x64.Release \\
--corefx-bins \${WORKSPACE}/corefx/bin/Linux.AnyCPU.Debug/ \\
--corefx-native-bins \${WORKSPACE}/corefx/bin/Linux.x64.Debug/Native \\
--wcf-bins \${WORKSPACE}/bin/Linux.AnyCPU.${configuration} \\
--wcf-tests \${WORKSPACE}/bin/tests/Linux.AnyCPU.${configuration}""")
            }
        }
        
        // Finish off the job with the usual options
        if (isPR) {
            Utilities.addPRTestSCM(linuxTestJob, project)
            Utilities.addStandardPRParameters(linuxTestJob, project)
        }
        else {
            Utilities.addScm(linuxTestJob, project)
            Utilities.addStandardNonPRParameters(linuxTestJob)
        }
        
        Utilities.addStandardOptions(linuxTestJob)
        Utilities.addXUnitDotNETResults(linuxTestJob, 'bin/tests/**/testResults.xml')
    }
}

// Loop over the options and build up the innerloop build matrix.
// When we go to create the Linux build, in addition to creating the regular job,
// we should create a flow job that launches the build on Windows, followed by the
// test on Linux, passing the build parameter to the linux test job.  Then, instead
// of adding the PR/commit triggers to the build job, we should add it to the flow job.

['Debug', 'Release'].each { configuration ->
    ['Linux', 'Windows_NT'].each { os ->
        // Calculate job name
        def osJobName = os.toLowerCase()
        if (osJobName == 'windows_nt') {
            osJobName = 'windows'
        }
        def configurationJobName = configuration.toLowerCase()
        def jobName = "${osJobName}_${configurationJobName}"
        // The flow job name will be free of the suffix below
        def flowJobName = jobName
        
        // If Linux, append _bld to the end.
        if (os == 'Linux') {
            jobName += '_bld'
        }
        
        // Create the new job
        def newCommitJob = job(Utilities.getFullJobName(project, jobName, false)) {
            label('windows')
            steps {
                // Use inline replacement
                batchFile("build.cmd /p:Configuration=${configuration} /p:OSGroup=${os}")
                // Pack up the results for max efficiency
                batchFile("C:\\Packer\\Packer.exe .\\bin\\build.pack .\\bin")
            }
        }

        // Add commit job options
        Utilities.addScm(newCommitJob, project)
        Utilities.addStandardNonPRParameters(newCommitJob)
        
        // Don't add the push trigger if on Linux, since we'll run it through the
        // flow job defined below
        if (os != 'Linux') {
            Utilities.addGithubPushTrigger(newCommitJob)
        }
        
        // Create the new PR job
        
        def newPRJob = job(Utilities.getFullJobName(project, jobName, true)) {
            label('windows')
            steps {
                // Use inline replacement
                batchFile("build.cmd /p:Configuration=${configuration} /p:OSGroup=${os}")
                // Pack up the results for max efficiency
                batchFile("C:\\Packer\\Packer.exe .\\bin\\build.pack .\\bin")
            }
        }
        
        // Add a PR trigger
        if (os != 'Linux') {
            Utilities.addGithubPRTrigger(newPRJob, "${os} ${configuration} Build")
        }
        Utilities.addPRTestSCM(newPRJob, project)
        Utilities.addStandardPRParameters(newPRJob, project)
        
        // Add common options:
        
        [newPRJob, newCommitJob].each { newJob ->
            Utilities.addStandardOptions(newJob)
            
            if (os != 'Linux') {
                Utilities.addXUnitDotNETResults(newJob, 'bin/tests/**/testResults.xml')
                Utilities.addArchival(newJob, "bin/${os}.AnyCPU.${configuration}/**,bin/build.pack")
            } else {
                // Include the tests on Linux, since they'll be moved to another
                // machine for execution
                Utilities.addArchival(newJob, "bin/${os}.AnyCPU.${configuration}/**,bin/build.pack")
            }
        }
    }
}

// Add flow jobs for Linux bld/tst

[true, false].each { isPR ->
    ['Debug', 'Release'].each { configuration ->
        def configurationJobName = configuration.toLowerCase()
        def jobName = "linux_${configurationJobName}"
        
        def linuxFlowJob = buildFlowJob(Utilities.getFullJobName(project, jobName, isPR)) {
            def buildJobName = Utilities.getFolderName(project) + '/' + Utilities.getFullJobName(project, jobName + '_bld', isPR)
            def testJobName = Utilities.getFolderName(project) + '/' + Utilities.getFullJobName(project, jobName + '_tst', isPR)
            
            buildFlow("""
// Build the Linux _bld job
def linuxBuildJob = build(params, \"${buildJobName}\")
// Pass this to the test job.  Include the parameters
build(params + [WCF_LINUX_BUILD_NUMBER: linuxBuildJob.build.number], 
    \"${testJobName}\")
            """)

            // Needs a workspace
            configure {
                def buildNeedsWorkspace = it / 'buildNeedsWorkspace'
                buildNeedsWorkspace.setValue('true')
            }
        }
        
        if (isPR) {
            Utilities.addPRTestSCM(linuxFlowJob, project)
            Utilities.addStandardPRParameters(linuxFlowJob, project)
            Utilities.addGithubPRTrigger(linuxFlowJob, "Linux ${configuration} Build and Test")
        }
        else {
            Utilities.addScm(linuxFlowJob, project)
            Utilities.addStandardNonPRParameters(linuxFlowJob)
            Utilities.addGithubPushTrigger(linuxFlowJob)
        }
        
        Utilities.addStandardOptions(linuxFlowJob)
    }
}

// **************************
// Define the code coverage jobs
// **************************

// Define build string
def codeCoverageBuildString = '''build.cmd /p:Coverage=true /p:WithCategories=OuterLoop'''

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
            batchFile("build.cmd /p:Configuration=${configuration} /p:WithCategories=OuterLoop")
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
            batchFile("build.cmd /p:Configuration=${configuration} /p:WithCategories=OuterLoop")
        }
    }
    
    // Add a PR trigger
    Utilities.addGithubPRTrigger(newPRJob, "Outerloop Windows ${configuration} Build", '@dotnet-bot test outerloop please')
    Utilities.addPRTestSCM(newPRJob, project)
    Utilities.addStandardPRParameters(newPRJob, project)
    
    // Add common options   
    [newPRJob, newRollingJob].each { newJob ->
        Utilities.addStandardOptions(newJob)
        Utilities.addXUnitDotNETResults(newJob, 'bin/tests/**/testResults.xml')
    }
}