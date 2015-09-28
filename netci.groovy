// Import the utility functionality.

import jobs.generation.Utilities;

def project = 'dotnet/wcf'

// **************************
// Define the basic inner loop builds for PR 
// **************************

// Loop over the options and build up the innerloop build matrix

['Debug', 'Release'].each { configuration ->
    ['Linux', 'Windows_NT'].each { os ->
        // Calculate job name
        def osJobName = os.toLowerCase()
        if (osJobName == 'windows_nt') {
            osJobName = 'windows'
        }
        def configurationJobName = configuration.toLowerCase()
        def jobName = "${osJobName}_${configurationJobName}"
        
        // Create the new job
        def newCommitJob = job(Utilities.getFullJobName(project, jobName, false)) {
            label('windows')
            steps {
                // Use inline replacement
                batchFile("build.cmd /p:Configuration=${configuration} /p:OSGroup=${os}")
            }
        }

        // Add commit job options
        Utilities.addScm(newCommitJob, project)
        Utilities.addStandardNonPRParameters(newCommitJob)
        Utilities.addGithubPushTrigger(newCommitJob)
        
        // Create the new PR job
        
        def newPRJob = job(Utilities.getFullJobName(project, jobName, true)) {
            label('windows')
            steps {
                // Use inline replacement
                batchFile("build.cmd /p:Configuration=${configuration} /p:OSGroup=${os}")
            }
        }
        
        // Add a PR trigger
        Utilities.addGithubPRTrigger(newPRJob, "${os} ${configuration} Build")
        Utilities.addPRTestSCM(newPRJob, project)
        Utilities.addStandardPRParameters(newPRJob, project)
        
        // Add common options:
        
        [newPRJob, newCommitJob].each { newJob ->
            Utilities.addStandardOptions(newJob)
            
            if (os != 'Linux') {
                Utilities.addXUnitDotNETResults(newJob, 'bin/tests/**/testResults.xml')
                Utilities.addArchival(newJob, "bin/${os}.AnyCPU.${configuration}/**")
            } else {
                // Include the tests on Linux, since they'll be moved to another
                // machine for execution
                Utilities.addArchival(newJob, "bin/${os}.AnyCPU.${configuration}/**,bin/tests/**")
            }
        }
    }
}