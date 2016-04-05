<%@ WebHandler Language="C#" Class="PullRequestHandler" %>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

// This script assumes that the repo is set up as follows: 

//[remote "origin"]
//	url = https://github.com/dotnet/wcf
//	fetch = +refs/heads/*:refs/remotes/origin/*
//	fetch = +refs/pull/*/head:refs/remotes/origin/pr/*      <-- allows us to run git checkout pr/{id} 

public class PullRequestHandler : IHttpHandler
{
    private const string _gitExecutablePath = @"C:\Program Files (x86)\Git\bin\git.exe";
    private const string _gitRepoPath = @"N:\git\wcf-test";

    private const int _gitSanityExecutionTimeoutMilliseconds = 1000;
    private const int _gitExecutionTimeoutMilliseconds = 60000;

    public void ProcessRequest(HttpContext context)
    {
        string sha = context.Request.QueryString["sha"];
        string pr = context.Request.QueryString["pr"];

        context.Response.Write(string.Format("SHA = {0}<br/>", sha ?? "Unspecified"));
        context.Response.Write(string.Format("PR = {0}<br/>", pr ?? "Unspecified"));

        StringBuilder result = new StringBuilder();

        bool success = CheckPrerequisites(result);

        if (!success)
        {
            context.Response.Write(result);
            return;
        }

        // Check inputs to the script
        if (!string.IsNullOrEmpty(pr) && VerifyPrId(pr))
        {
            success = SyncToPr(pr, result);
        }
        else if (!string.IsNullOrEmpty(sha) && VerifySha(sha))
        {
            success = SyncToSha(sha, result);
        }
        else
        {
            context.Response.Write(string.Format("Invalid SHA or PR ID specified<br/>"));
            success = false;
        }

        if (context.Request.IsLocal)
        {
            // Don't allow error results to leak to non-local machines
            context.Response.Write(result);
        }

        if (!success)
        {
            context.Response.StatusCode = 400;
        }
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

        try
        {
            string[] gitCommands = new string[]
            {
                "status"
            };

            RunGitCommands(gitCommands, executionResult);
        }
        catch (Exception ex)
        {
            executionResult.AppendFormat("Exception caught while attempting to start '{0}'", _gitExecutablePath);
            executionResult.Append(ex.ToString());
            return false;
        }

        return true;
    }

    private bool SyncToPr(string pr, StringBuilder executionResult)
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

    private bool SyncToSha(string sha, StringBuilder executionResult)
    {
        string[] gitCommands = new string[]
        {
            "clean -fdx",
            "checkout master",
            "fetch --all --prune",
            "merge --ff-only origin/master",
            string.Format("checkout {0}", sha)
        };

        return RunGitCommands(gitCommands, executionResult);
    }

    private void CleanupMergedBranches(StringBuilder executionResult)
    {
        RunGitCommands(new string[] { "checkout master" }, executionResult);

        StringBuilder branches = new StringBuilder();
        RunGitCommands(new string[] { "branch" }, branches);

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

        RunGitCommands(commands.ToArray(), executionResult);
    }

    private bool RunGitCommands(string[] gitCommands, StringBuilder executionResult)
    {
        ProcessStartInfo psi = new ProcessStartInfo()
        {
            CreateNoWindow = true,
            FileName = _gitExecutablePath,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            WorkingDirectory = _gitRepoPath
        };

        bool success = false;

        foreach (string gitCommand in gitCommands)
        {
            psi.Arguments = gitCommand;

            Process p = Process.Start(psi);
            if (!p.WaitForExit(_gitExecutionTimeoutMilliseconds))
            {
                executionResult.Append(string.Format("Git executable '{0}' took more than '{1}' ms to execute", _gitExecutablePath, _gitSanityExecutionTimeoutMilliseconds));
            }

            success = p.ExitCode == 0;

            executionResult.Append(p.StandardOutput.ReadToEnd());
            executionResult.Append(p.StandardError.ReadToEnd());

            if (!success)
            {
                executionResult.Append(string.Format("Git executable '{0}' exited with code '{1}', expected '{2}'", _gitExecutablePath, p.ExitCode, "0"));
                return success;
            }
        }

        return success;
    }

    private bool VerifyPrId(string pr)
    {
        if (!string.IsNullOrWhiteSpace(pr))
        {
            return Regex.IsMatch(pr, "^[0-9]{1,}$");
        }
        return false;
    }

    private bool VerifySha(string sha)
    {
        if (!string.IsNullOrWhiteSpace(sha))
        {
            return Regex.IsMatch(sha, "^[a-f0-9]{7,}$");
        }
        return false;
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}


