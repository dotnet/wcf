// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
 
<%@ WebHandler Language="C#" Class="PullRequestHandler" %>

// This script is called to prepare a local repo for testing
// ASSUMES: 
//   - Git is installed on the system to "C:\Program Files\Git\cmd\git.exe"
//   - IIS web site points to this script
//   - This script is committed to ./src/System.Private.ServiceModel/tools/PRService
//   - 
//   - The Git repo is set up as follows: 
//     [remote "origin"]
//         url = https://github.com/dotnet/wcf
//         fetch = +refs/heads/*:refs/remotes/origin/*
//         fetch = +refs/pull/*/head:refs/remotes/origin/pr/*      <-- allows us to run git checkout pr/{id} 
//
// We also support checking out branches, but we limit the the branches we can check out
// NOTE: Not locking here, we assume we only have one request hitting each id= at the same time. We could introduce some
//       kind of locking here across the whole request, but decided not to as this isn't a heavily hit service. If we find 
//       that there's a need, then we will revisit 

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web;

public class PullRequestHandler : IHttpHandler
{
    private string _gitExecutablePath;
    private string _gitRepositoriesBasePath;
    private TimeSpan _gitExecutionTimeout;

    private bool _configurationRead = false;

    private const string _gitExecutablePathConfigurationKeyString = "GitExecutablePath";
    private const string _gitRepositoriesBasePathConfigurationKeyString = "GitRepositoriesBasePath";
    private const string _gitExecutionTimeoutConfigurationKeyString = "GitExecutionTimeout";

    private const string _gitRepoPathTemplate = @"{0}\wcf{1}";

    // Paths (relative to the repo root) whose content drives certificate generation.
    // Whenever any of these change between PR syncs we re-run the certificate
    // generator so that wcfcoresrv23 serves certs minted from the PR's code
    // (e.g. AIA / OCSP fields). See dotnet/wcf#2870.
    private static readonly string[] _certSourcePaths = new[]
    {
        "src/System.Private.ServiceModel/tools/CertificateGenerator",
        "src/System.Private.ServiceModel/tools/IISHostedWcfService"
    };

    private const string _certSourceHashMarkerName = ".cert-source-hash";
    private const string _wcfTestDir = @"C:\WCFTest";
    private static readonly TimeSpan _certRefreshTimeout = TimeSpan.FromMinutes(4);

    public void ProcessRequest(HttpContext context)
    {
        StringBuilder result = new StringBuilder();

        // Read configuration
        bool success = ReadConfiguration(result);
        if (!success)
        {
            // ReadConfiguration failed, this is a server issue 
            context.Response.StatusCode = 500;
            context.Response.Write(HttpUtility.HtmlEncode(result.ToString()));
            return;
        }

        // Check inputs 
        // We expect one of the following to be specified: 
        // - A numerical repo ID
        // - A branch name OR a PR number (not both)
        //
        // We will use Git to validate and verify that the branch exists if specified

        success = false;

        string repoIdString = context.Request.QueryString["id"];
        string branchString = context.Request.QueryString["branch"];
        string prString = context.Request.QueryString["pr"];

        uint repoId;

        if (string.IsNullOrWhiteSpace(repoIdString) || !uint.TryParse(repoIdString, out repoId))
        {
            context.Response.StatusCode = 400;
            context.Response.StatusDescription = string.Format("The client ID specified, '{0}', is invalid. Please specify a valid 'id' in the request query string.", repoIdString ?? "unspecified");
            context.Response.Write(string.Format("The client ID specified, '{0}', is invalid. Please specify a valid 'id' in the request query string <br/>", repoIdString ?? "unspecified"));
            return;
        }

        if (string.IsNullOrWhiteSpace(prString) && string.IsNullOrWhiteSpace(branchString))
        {
            context.Response.StatusCode = 400;
            context.Response.StatusDescription = "No PR or branch specified. Specify either a 'pr' or 'branch' in the request query string.";
            context.Response.Write("No PR or branch specified. Specify either a 'pr' or 'branch' in the request query string <br/>");
            return;
        }

        if (!string.IsNullOrWhiteSpace(prString) && !string.IsNullOrWhiteSpace(branchString))
        {
            context.Response.StatusCode = 400;
            context.Response.StatusDescription = "Both 'pr' and 'branch' specified; please specify only one of them to sync to.";
            context.Response.Write("Both 'pr' and 'branch' specified; please specify only one of them to sync to <br/>");
            return;
        }

        context.Response.Write(string.Format("pr = {0} <br/>", HttpUtility.HtmlEncode(prString) ?? "unspecified"));
        context.Response.Write(string.Format("branch = {0} <br/>", HttpUtility.HtmlEncode(branchString) ?? "unspecified"));

        // Check for prerequisites needed before this script can execute
        string gitRepoPath = GetRepoPath(repoId);
        success = CheckPrerequisites(gitRepoPath, result);

        if (!success)
        {
            // CheckPrerequisites failed, this is a server issue
            context.Response.StatusCode = 500;
            context.Response.Write(HttpUtility.HtmlEncode(result.ToString()));
            return;
        }

        // Cleanup all branches except main from the repo - this shouldn't fail, and if it does, there's something wrong with the repo
        success = CleanupBranches(gitRepoPath, result);

        if (!success)
        {
            // CleanupBranches failed, this is a server issue
            context.Response.StatusCode = 500;
            context.Response.Write(HttpUtility.HtmlEncode(result.ToString()));
            return;
        }

        // Do stuff
        success = false;

        uint pr;
        // At this point we are guaranteed to have either a 'pr' or 'branch' but not both
        if (!string.IsNullOrEmpty(prString))
        {
            if (uint.TryParse(prString, out pr))
            {
                success = SyncToPr(pr, gitRepoPath, result);
            }
        }
        else
        {
            // Validate input here, because we don't want someone passing in an arbitrary branch
            // Obtain a list of valid remote branches and allow sync to one of these only

            string[] branches;
            success = GetBranches(gitRepoPath, true, out branches, result);

            if (success)
            {
                success = false;
                for (int i = 0; i < branches.Length; i++)
                {
                    // git branches are case sensitive 
                    if (string.Equals(branchString, branches[i]))
                    {
                        success = SyncToBranch(branchString, gitRepoPath, result);
                        break;
                    }
                }
            }
        }

        if (!success)
        {
            // it's most likely that the client supplied a bad PR ID, but there could potentially 
            // be other errors in SyncToPr or SyncToBranch. For now, return 400 in this path
            context.Response.StatusCode = 400;
            context.Response.StatusDescription = string.Format("Invalid 'pr', '{0}' or 'branch', '{1}' specified. Please specify a valid 'pr' or 'branch'", prString, branchString);
            context.Response.Write(string.Format("Invalid 'pr', '{0}' or 'branch', '{1}' specified. Please specify a valid 'pr' or 'branch' <br/>", HttpUtility.HtmlEncode(prString), HttpUtility.HtmlEncode(branchString)));
        }
        else
        {
            // Sync succeeded. If the PR changed certificate-related sources, re-mint server
            // certs so that AIA / OCSP / CRL artifacts on wcfcoresrv23 match the PR's code.
            // Failure here is logged but does NOT fail the sync request - tests will then
            // run against the previously-installed certs and surface the problem themselves.
            try
            {
                RefreshCertsIfNeeded(gitRepoPath, repoId, result);
            }
            catch (Exception ex)
            {
                result.AppendFormat("Cert refresh raised an exception (non-fatal): {0}<br/>", ex.Message);
            }
        }

        context.Response.Write(HttpUtility.HtmlEncode(result.ToString()));
    }

    // Check prerequisites for whether or not we can run
    private bool CheckPrerequisites(string gitRepoPath, StringBuilder executionResult)
    {
        if (!File.Exists(_gitExecutablePath))
        {
            executionResult.AppendFormat("Could not find or insufficient permissions to access the git executable at path '{0}'", _gitExecutablePath);
            return false;
        }

        if (!Directory.Exists(gitRepoPath))
        {
            executionResult.AppendFormat("Could not find or insufficient permissions to access the git repo at path '{0}'", gitRepoPath);
            return false;
        }

        // Do a sanity execute of the git executable 
        bool success = false;

        try
        {
            string[] gitCommands = new string[]
            {
                "status"
            };

            success = RunGitCommands(gitCommands, gitRepoPath, executionResult);
        }
        catch (Exception ex)
        {
            executionResult.AppendFormat("Exception caught while attempting to start '{0}'", _gitExecutablePath);
            executionResult.Append(ex.ToString());
        }

        return success;
    }

    private string GetRepoPath(uint repoId)
    {
        // Note: assumes configuration is read. 
        return string.Format(_gitRepoPathTemplate, _gitRepositoriesBasePath, repoId);
    }

    private bool SyncToPr(uint pr, string gitRepoPath, StringBuilder executionResult)
    {
        string[] gitCommands = new string[]
        {
            string.Format("fetch origin pull/{0}/head:pr/{0}", pr),
            string.Format("checkout -f pr/{0}", pr)
        };

        return RunGitCommands(gitCommands, gitRepoPath, executionResult);
    }

    private bool SyncToBranch(string branch, string gitRepoPath, StringBuilder executionResult)
    {
        string[] gitCommands = new string[]
        {
            string.Format("checkout -f {0}", branch)
        };

        return RunGitCommands(gitCommands, gitRepoPath, executionResult);
    }

    // Gets a list of branches 
    // - that are available in the repo (remoteBranches == false)
    // - that are available in defined remotes (remoteBranches == true)
    private bool GetBranches(string gitRepoPath, bool remoteBranches, out string[] branches, StringBuilder executionResult)
    {
        bool success = false;
        string[] tempBranches;
        branches = new string[0];

        StringBuilder branchesString = new StringBuilder();

        if (remoteBranches)
        {
            success = RunGitCommands(new string[] { "branch --remote --list" }, gitRepoPath, branchesString);
        }
        else
        {
            success = RunGitCommands(new string[] { "branch --list" }, gitRepoPath, branchesString);
        }

        executionResult.Append(branchesString.ToString());

        if (success)
        {
            tempBranches = branchesString.ToString().Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> branchesList = new List<string>();

            // Parse for anything that implies a detached HEAD - those are not branches to clean up
            for (int i = 0; i < tempBranches.Length; i++)
            {
                if (!tempBranches[i].Contains("HEAD detached"))
                {
                    branchesList.Add(tempBranches[i].Trim('*', ' '));
                }
            }

            branches = branchesList.ToArray();
        }

        return success;
    }

    // Cleans up branches that aren't currently used except "main" from the repo
    // If not done, we'll end up with dozens of branches that aren't used after a while
    // because we don't nuke and re-create the repo after each PR
    private bool CleanupBranches(string gitRepoPath, StringBuilder executionResult)
    {
        bool success = false;

        string[] gitCommands = new string[]
        {
            "fetch --all --prune",
            "checkout -f origin/main"
        };

        success = RunGitCommands(gitCommands, gitRepoPath, executionResult);

        string[] branchesList = null;
        if (success)
        {
            success = GetBranches(gitRepoPath, false, out branchesList, executionResult);
        }

        if (success)
        {
            List<string> commands = new List<string>();

            // we won't get branchesList == null because of the success check
            for (int i = 0; i < branchesList.Length; i++)
            {
                // delete all branches - GetBranches will prevent deletion of the detached head at origin/main
                commands.Add(string.Format("branch -D {0}", branchesList[i]));
            }

            // Execute a checkout to put the branch back to a usable state at the main branch
            // We do this to prevent successive origin/main checkouts from checking out to the same commit

            commands.Add("checkout -b main --track origin/main");
            success = RunGitCommands(commands.ToArray(), gitRepoPath, executionResult);
        }

        return success;
    }

    private bool RunGitCommands(string[] gitCommands, string gitRepoPath, StringBuilder executionResult)
    {
        if (gitCommands == null || gitCommands.Length == 0)
        {
            return true;
        }

        ProcessStartInfo psi = new ProcessStartInfo()
        {
            CreateNoWindow = true,
            FileName = _gitExecutablePath,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            WorkingDirectory = gitRepoPath
        };

        bool success = true;

        foreach (string gitCommand in gitCommands)
        {
            success = false;
            psi.Arguments = gitCommand;


            Process p = Process.Start(psi);
            if (!p.WaitForExit((int)_gitExecutionTimeout.TotalMilliseconds))
            {
                executionResult.Append(string.Format("Git executable '{0}' took more than '{1}' to execute <br/>", _gitExecutablePath, _gitExecutionTimeout));
                executionResult.Append(string.Format("Git command was: '{0}' ", gitCommand));

                // return early if !WaitForExit, as p.ExitCode will not be usable in this case (it throws an exception) 
                return success;
            }

            success = p.ExitCode == 0;
            executionResult.Append(p.StandardOutput.ReadToEnd());
            executionResult.Append(p.StandardError.ReadToEnd());

            if (!success)
            {
                executionResult.Append(string.Format("{0}Command: '{1}' ", Environment.NewLine, gitCommand));
                executionResult.Append(string.Format("Git executable '{0}' exited with code '{1}', expected '{2}' <br/>", _gitExecutablePath, p.ExitCode, "0"));

                return success;
            }
        }

        return success;
    }

    // Re-mint the server certificates if any of `_certSourcePaths` differ from the
    // versions used on the last successful refresh. Idempotent: subsequent syncs of
    // the same PR are no-ops once the cert source tree hash matches the marker.
    // Returns true if no refresh was needed or refresh succeeded; false on failure.
    private bool RefreshCertsIfNeeded(string gitRepoPath, uint repoId, StringBuilder result)
    {
        StringBuilder hashBuilder = new StringBuilder();
        foreach (string p in _certSourcePaths)
        {
            StringBuilder revOut = new StringBuilder();
            if (!RunGitCommands(new string[] { string.Format("rev-parse HEAD:{0}", p) }, gitRepoPath, revOut))
            {
                result.AppendFormat("Skipping cert refresh: could not resolve tree hash for '{0}'.<br/>", p);
                return true;
            }
            hashBuilder.Append(revOut.ToString().Trim()).Append(';');
        }
        string currentHash = hashBuilder.ToString();
        string markerPath = Path.Combine(gitRepoPath, _certSourceHashMarkerName);

        string previousHash = null;
        try
        {
            if (File.Exists(markerPath)) previousHash = File.ReadAllText(markerPath).Trim();
        }
        catch (Exception ex)
        {
            result.AppendFormat("Could not read cert source hash marker (will refresh): {0}<br/>", ex.Message);
        }

        if (string.Equals(currentHash, previousHash, StringComparison.Ordinal))
        {
            result.Append("Cert source unchanged since last refresh; skipping cert regeneration.<br/>");
            return true;
        }

        result.AppendFormat("Cert source changed (was '{0}', now '{1}'); refreshing server certificates...<br/>",
            previousHash ?? "<none>", currentHash);

        if (!InvokeCertRefresh(gitRepoPath, repoId, result))
        {
            result.Append("Cert refresh FAILED. Marker not updated; will retry on next sync.<br/>");
            return false;
        }

        try
        {
            File.WriteAllText(markerPath, currentHash);
            result.Append("Cert refresh complete; marker updated.<br/>");
        }
        catch (Exception ex)
        {
            result.AppendFormat("Cert refresh succeeded but marker write failed: {0}<br/>", ex.Message);
        }

        return true;
    }

    // Runs the certificate generator out of the freshly-synced PR repo, reconfigures
    // the HTTPS binding to use the new cert, grants the WCF service app pool access
    // to the new private key, and bounces IIS so the change takes effect. Mirrors
    // the cert install block of SetupWcfIISHostedService.cmd (lines 189-209) but
    // operates unconditionally on the current PR's source.
    //
    // NOTE: invoking CertificateGenerator.exe and iisreset requires that the
    // PRServiceMaster IIS app pool runs with sufficient privileges (admin / LocalSystem).
    // This is a one-time deploy concern; the script logic itself is idempotent.
    private bool InvokeCertRefresh(string gitRepoPath, uint repoId, StringBuilder result)
    {
        string scriptsDir = Path.Combine(gitRepoPath, @"src\System.Private.ServiceModel\tools\scripts");
        string buildCertUtil = Path.Combine(scriptsDir, "BuildCertUtil.cmd");
        string certGenExe = Path.Combine(gitRepoPath, @"artifacts\bin\CertificateGenerator\Release\net10.0\CertificateGenerator.exe");
        string configHttpsPort = Path.Combine(scriptsDir, "ConfigHttpsPort.ps1");
        string privateKeyPerms = Path.Combine(scriptsDir, "CertificatePrivateKeyPermissions.ps1");
        string serviceName = "WcfService" + repoId.ToString();

        try
        {
            if (!Directory.Exists(_wcfTestDir)) Directory.CreateDirectory(_wcfTestDir);
        }
        catch (Exception ex)
        {
            result.AppendFormat("Could not create '{0}': {1}<br/>", _wcfTestDir, ex.Message);
            return false;
        }

        // 1) Build CertificateGenerator from the synced PR source.
        if (!RunProcess("cmd.exe", "/c \"" + buildCertUtil + "\"", gitRepoPath, result)) return false;

        // 2) Write CertificateGenerator.exe.config (same pattern as SetupWcfIISHostedService.cmd).
        try
        {
            string config =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<configuration>" +
                  "<appSettings>" +
                    "<add key=\"testserverbase\" value=\"" + serviceName + "\"/>" +
                    "<add key=\"CertExpirationInDay\" value=\"99\"/>" +
                    "<add key=\"CrlFileLocation\" value=\"" + Path.Combine(_wcfTestDir, "test.crl") + "\"/>" +
                  "</appSettings>" +
                  "<startup>" +
                    "<supportedRuntime version=\"v4.0\" sku=\".NETFramework,Version=v4.5\"/>" +
                  "</startup>" +
                "</configuration>";
            File.WriteAllText(certGenExe + ".config", config);
        }
        catch (Exception ex)
        {
            result.AppendFormat("Failed to write CertificateGenerator config: {0}<br/>", ex.Message);
            return false;
        }

        // 3) Run CertificateGenerator (mints root + leaf certs, writes test.crl and test.ocsp).
        if (!RunProcess(certGenExe, "", _wcfTestDir, result)) return false;

        // 4) Bind the new cert to the HTTPS port.
        if (!RunProcess("powershell.exe",
            "-NoProfile -ExecutionPolicy unrestricted -File \"" + configHttpsPort + "\"",
            gitRepoPath, result)) return false;

        // 5) Grant the WCF service app pool access to the new cert's private key.
        if (!RunProcess("powershell.exe",
            "-NoProfile -ExecutionPolicy unrestricted -File \"" + privateKeyPerms + "\" \"IIS APPPOOL\\" + serviceName + "\"",
            gitRepoPath, result)) return false;

        // 6) Bounce IIS so app pools pick up the new cert.
        if (!RunProcess("iisreset.exe", "", gitRepoPath, result)) return false;

        return true;
    }

    // Runs an external process with the cert-refresh timeout, capturing stdout/stderr.
    private bool RunProcess(string fileName, string arguments, string workingDirectory, StringBuilder result)
    {
        ProcessStartInfo psi = new ProcessStartInfo()
        {
            CreateNoWindow = true,
            FileName = fileName,
            Arguments = arguments,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            WorkingDirectory = workingDirectory
        };

        result.AppendFormat("Run: {0} {1}<br/>", fileName, arguments);

        try
        {
            using (Process p = Process.Start(psi))
            {
                if (!p.WaitForExit((int)_certRefreshTimeout.TotalMilliseconds))
                {
                    result.AppendFormat("Process '{0}' exceeded timeout '{1}'.<br/>", fileName, _certRefreshTimeout);
                    try { p.Kill(); } catch { }
                    return false;
                }

                string stdout = p.StandardOutput.ReadToEnd();
                string stderr = p.StandardError.ReadToEnd();
                if (!string.IsNullOrEmpty(stdout)) result.AppendFormat("stdout: {0}<br/>", stdout);
                if (!string.IsNullOrEmpty(stderr)) result.AppendFormat("stderr: {0}<br/>", stderr);

                if (p.ExitCode != 0)
                {
                    result.AppendFormat("Process '{0}' exited with code {1}.<br/>", fileName, p.ExitCode);
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            result.AppendFormat("Failed to launch '{0}': {1}<br/>", fileName, ex.Message);
            return false;
        }

        return true;
    }

    // Read configuration
    // Precedence is: 
    // 1. Environment variable
    // 2. AppSettings in web.config
    // 
    // returns: successful read of configuration or not
    // errorOutput: passed back to the caller to provide a verbose explanation of what went wrong
    private bool ReadConfiguration(StringBuilder errorOutput)
    {
        if (_configurationRead)
        {
            // No locking needed, reading this in twice, even on multiple concurrent threads, doesn't do harm
            return true;
        }

        NameValueCollection appSettings = ConfigurationManager.AppSettings;

        string gitExecutablePath = Environment.GetEnvironmentVariable(_gitExecutablePathConfigurationKeyString);
        if (string.IsNullOrWhiteSpace(gitExecutablePath))
        {
            gitExecutablePath = appSettings[_gitExecutablePathConfigurationKeyString];
        }
        if (string.IsNullOrWhiteSpace(gitExecutablePath))
        {
            errorOutput.AppendFormat("Invalid or missing {0}: '{1}'", _gitExecutablePathConfigurationKeyString, gitExecutablePath ?? "unspecified");
            return false;
        }

        _gitExecutablePath = gitExecutablePath;

        string gitRepositoriesBasePath = Environment.GetEnvironmentVariable(_gitRepositoriesBasePathConfigurationKeyString);
        if (string.IsNullOrWhiteSpace(gitRepositoriesBasePath))
        {
            gitRepositoriesBasePath = appSettings[_gitRepositoriesBasePathConfigurationKeyString];
        }
        if (string.IsNullOrWhiteSpace(gitRepositoriesBasePath))
        {
            errorOutput.AppendFormat("Invalid or missing {0}: '{1}'", _gitRepositoriesBasePathConfigurationKeyString, gitRepositoriesBasePath ?? "unspecified");
            return false;
        }

        _gitRepositoriesBasePath = gitRepositoriesBasePath;

        string gitExecutionTimeoutString = Environment.GetEnvironmentVariable(_gitExecutionTimeoutConfigurationKeyString);
        if (string.IsNullOrWhiteSpace(gitExecutionTimeoutString))
        {
            gitExecutionTimeoutString = appSettings[_gitExecutionTimeoutConfigurationKeyString];
        }
        if (!string.IsNullOrWhiteSpace(gitRepositoriesBasePath) && !TimeSpan.TryParse(gitExecutionTimeoutString, out _gitExecutionTimeout))
        {
            errorOutput.AppendFormat("Invalid or missing {0}: '{1}'", _gitExecutionTimeoutConfigurationKeyString, gitRepositoriesBasePath ?? "unspecified");
            return false;
        }

        _configurationRead = true;
        return true;
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}

