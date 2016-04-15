 // Copyright (c) Microsoft. All rights reserved.
 // Licensed under the MIT license. See LICENSE file in the project root for full license information.
 
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web;
using System.IO;
using System.Text;

public class PullRequestHandler : IHttpHandler
{
    private const string _gitExecutablePath = @"C:\Program Files\Git\cmd\git.exe";
    private static string _gitRepoPath;
    private const int _gitExecutionTimeoutMilliseconds = 100000;

    public void ProcessRequest(HttpContext context)
    {
        // Check for prerequisites needed before this script can execute

        _gitRepoPath = context.Request.PhysicalApplicationPath + "..\\..\\..\\..";

        StringBuilder result = new StringBuilder();
        bool success = CheckPrerequisites(result);

        if (!success)
        {
            // CheckPrerequisites failed, this is a server issue
            context.Response.StatusCode = 500;
            context.Response.Write(HttpUtility.HtmlEncode(result));
            return;
        }

        success = CleanupMergedBranches(result);

        if (!success)
        {
            // CleanupMergedBranches failed, this is a server issue
            context.Response.StatusCode = 500;
            context.Response.Write(HttpUtility.HtmlEncode(result));
            return;
        }

        // Parameter verification

        success = false;

        string branchString = context.Request.QueryString["branch"];
        string prString = context.Request.QueryString["pr"];

        if (string.IsNullOrWhiteSpace(prString) && string.IsNullOrWhiteSpace(branchString))
        {
            context.Response.StatusCode = 400;
            context.Response.Write("No PR or branch specified");
            return;
        }

        if (!string.IsNullOrWhiteSpace(prString) && !string.IsNullOrWhiteSpace(branchString))
        {
            context.Response.StatusCode = 400;
            context.Response.Write("PR and branch specified; please specify only one of them to sync to <br/>");
            return;
        }

        context.Response.Write(string.Format("PR = {0}<br/>", HttpUtility.HtmlEncode(prString) ?? "Unspecified"));
        context.Response.Write(string.Format("branch = {0}<br/>", HttpUtility.HtmlEncode(branchString) ?? "Unspecified"));

        // Do stuff

        uint pr;
        if (!string.IsNullOrEmpty(prString) && uint.TryParse(prString, out pr))
        {
            success = SyncToPr(pr, result);
        }
        else if (!string.IsNullOrEmpty(branchString))
        {
            // We only want to support limited branches as we only use this for PR purposes
            // This should be opened up later once we decide we need to (e.g., for testing release branches), but for now, 
            // whitelist what we need
            // for example, we could use the output from "git branch -r" to figure out what branches are valid

            switch (branchString)
            {
                case "master":
                case "origin/master":
                case "origin/infrastructure":
                    success = SyncToBranch(branchString, result);
                    break;
                default:
                    context.Response.Write(string.Format("Branch name specified, '{0}' is not supported by this service<br/>", HttpUtility.HtmlEncode(branchString)));
                    break;
            }
        }

        if (!success)
        {
            // it's most likely that the client supplied a bad PR ID, but there could potentially 
            // be other errors in SyncToPr or SyncToBranch. For now, return 400 in this path
            context.Response.StatusCode = 400;
            context.Response.Write(string.Format("Invalid PR ID, '{0}' or branch, '{1}' specified <br/>", HttpUtility.HtmlEncode(prString), HttpUtility.HtmlEncode(branchString)));
        }

        context.Response.Write(HttpUtility.HtmlEncode(result));
    }

    // Check prerequisites for whether or not we can run
    private bool CheckPrerequisites(StringBuilder executionResult)
    {
        if (!File.Exists(_gitExecutablePath))
        {
            executionResult.AppendFormat("Could not find or insufficient permissions to access the git executable at path '{0}'", _gitExecutablePath);
            return false;
        }

        if (!Directory.Exists(_gitRepoPath))
        {
            executionResult.AppendFormat("Could not find or insufficient permissions to access the git repo at path '{0}'", _gitRepoPath);
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

            success = RunGitCommands(gitCommands, executionResult);
        }
        catch (Exception ex)
        {
            executionResult.AppendFormat("Exception caught while attempting to start '{0}'", _gitExecutablePath);
            executionResult.Append(ex.ToString());
            return success;
        }

        return success;
    }

    private bool SyncToPr(uint pr, StringBuilder executionResult)
    {
        string[] gitCommands = new string[]
        {
            "clean -fdx",
            "checkout master",
            "fetch --all --prune",
            "merge --ff-only origin/master",
            string.Format("fetch origin pull/{0}/head:pr/{0}", pr),
            string.Format("checkout pr/{0}", pr)
        };

        return RunGitCommands(gitCommands, executionResult);
    }

    private bool SyncToBranch(string branch, StringBuilder executionResult)
    {
        string[] gitCommands = new string[]
        {
            "clean -fdx",
            "checkout master",
            "fetch --all --prune",
            "merge --ff-only origin/master",
            string.Format("checkout {0}", branch)
        };

        return RunGitCommands(gitCommands, executionResult);
    }

    // Cleans up branches that aren't currently used except "master" from the repo
    // If not done, we'll end up with dozens of branches that aren't used after a while
    // because we don't nuke and re-create the repo after each PR
    private bool CleanupMergedBranches(StringBuilder executionResult)
    {
        bool success = false;

        success = RunGitCommands(new string[] { "checkout master" }, executionResult);

        StringBuilder branches = new StringBuilder();
        if (success)
        {
            success = RunGitCommands(new string[] { "branch" }, branches);
        }

        if (success)
        {
            var branchesList =
                branches.ToString().Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            List<string> commands = new List<string>();

            for (int i = 0; i < branchesList.Length; i++)
            {
                // Don't try to delete "master" branch, since we're checked out to it
                if (!branchesList[i].EndsWith("master"))
                {
                    commands.Add(string.Format("branch -D {0}", branchesList[i]));
                }
            }

            success = RunGitCommands(commands.ToArray(), executionResult);
        }

        return success;
    }

    private bool RunGitCommands(string[] gitCommands, StringBuilder executionResult)
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
            WorkingDirectory = _gitRepoPath
        };

        bool success = true;

        foreach (string gitCommand in gitCommands)
        {
            success = false;
            psi.Arguments = gitCommand;

            Process p = Process.Start(psi);
            if (!p.WaitForExit(_gitExecutionTimeoutMilliseconds))
            {
                executionResult.Append(string.Format("Git executable '{0}' took more than '{1}' ms to execute <br/>", _gitExecutablePath, _gitExecutionTimeoutMilliseconds));
                executionResult.Append(string.Format("Git command was: '{0}' ", gitCommand));

                // return early if !WaitForExit, as p.ExitCode will not be usable in this case (it throws an exception) 
                return success;
            }

            success = p.ExitCode == 0;

            executionResult.Append(p.StandardOutput.ReadToEnd());
            executionResult.Append(p.StandardError.ReadToEnd());

            if (!success)
            {
                executionResult.Append(string.Format("Git executable '{0}' exited with code '{1}', expected '{2}' <br/>", _gitExecutablePath, p.ExitCode, "0"));
                executionResult.Append(string.Format("Git command was: '{0}' ", gitCommand));

                return success;
            }
        }

        return success;
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}
