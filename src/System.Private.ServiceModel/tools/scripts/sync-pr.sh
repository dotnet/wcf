#!/usr/bin/env bash

# This notifies a the test web service, specified on the command line,
# that a test is about to start
# This script is meant to be called from a GitHub Pull Request.

show_banner()
{
    echo ""
    echo "WCF Test Service Pull Request sync script"
    echo ""
}

show_usage() 
{
    echo "    A URL must be specified for the pull request synchronization URL"
    echo "    Usage: $0 [id] [branch|pr] [sync-url] [branch-name | pr-number (optional)]"
    echo "    id          - ID of the repo on the PR service"
    echo "    branch|pr   - Sync to branch or PR (choose one)"
    echo "    sync-url    - URL on remote server for PR synchronization"
    echo "    branch-name - branch name to sync to"
    echo "    pr-number   - PR to sync to"
    echo ""
    echo "    If branch-name or pr-number are left blank, then the script will use"
    echo "    \$GIT_BRANCH or \$ghprbPullId by default depending on the operation mode"
    echo ""
    echo "    Example:  $0 1 branch http://wcfci-sync-server/PRService/pr.ashx"
    echo "    Example:  $0 2 branch http://wcfci-sync-server/PRService/pr.ashx master"
    echo "    Example:  $0 3 pr http://wcfci-sync-server/PRService/pr.ashx"
    echo "    Example:  $0 4 pr http://wcfci-sync-server/PRService/pr.ashx 404"
    echo ""
}

run_request() 
{
    __repo_id=$1
    __operation_mode=$2
    __sync_url=$3
    __branch_or_pullid=$4

    __request_uri=${__sync_url}?id=${__repo_id}\&${__operation_mode}=${__branch_or_pullid}

    echo "    Making a call to '${__request_uri}'" 

    which curl wget > /dev/null 2> /dev/null
    if [ $? -ne 0 -a $? -ne 1 ]; then
        echo "    cURL or wget is required to make this request. Please see https://github.com/dotnet/wcf/blob/master/Documentation/building/unix-instructions.md for more details."
        exit 1
    fi

    # curl has HTTPS CA trust-issues less often than wget, so lets try that first.
    which curl > /dev/null 2> /dev/null
    if [ $? -ne 0 ]; then
       wget -q -O- ${__request_uri}
    else
       output=`curl -sw "\n%{http_code}" ${__request_uri}`
       echo $output
       retCode=`echo ${output} | grep "200\$"`
    fi

    return $?

}

# Main execution

show_banner

# Check parameters

# We expect at least three parameters, with $4 optional
if [ -z "$3" ]; then
    show_usage
    exit 1
fi 

__repo_id=$1

if [ "$2" == "branch" ] || [ "$2" == "pr" ]; then 
    __operation_mode=$2
else 
    show_usage
    exit 1
fi 

__sync_url=$3

if [ -z "$4" ]; then 
    if [ "$__operation_mode" == "branch" ]; then 
        if [ -z "$GIT_BRANCH" ]; then 
            show_usage
            echo "    This script should only be called only from the context of a GitHub Pull Request"
            echo "    'GIT_BRANCH' environment variable not set and no branch specified"
        
            exit 1 
        else
            __pr_or_branch=$GIT_BRANCH
        fi 
    elif [ "$__operation_mode" == "pr" ]; then 
        if [ -z "$ghprbPullId" ]; then 
            show_usage
            echo "    This script should only be called only from the context of a GitHub Pull Request"
            echo "    'ghprbPullId' environment variable not set and no PR ID specified"
        
            exit 1
        else 
            __pr_or_branch=$ghprbPullId
        fi  
    fi
else  
    __pr_or_branch=$4
    show_usage 
    echo "    WARNING: This script should usually only be called only from the context of a GitHub Pull Request"
    echo "    Branch or PR ID overridden '${__pr_or_branch}'"
fi

# Run 
run_request $__repo_id $__operation_mode $__sync_url $__pr_or_branch

__exit_code=$?

if [ $__exit_code -ne 0 ]; then
    echo "Server error when syncing to Pull Request"
fi 

exit $__exit_code 
