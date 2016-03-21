#!/usr/bin/env bash

show_banner()
{
    echo ""
    echo "WCF Remote Github Repo sync script"
    echo ""
}

show_usage() 
{
    echo "    A URL must be specified for the pull request synchronization URL"
    echo "    Usage: $0 [sync-url] [pr-number (optional)]"
    echo "    sync-url  - URL on remote server for PR synchronization"
    echo "    pr-number - PR to sync to"
    echo ""
    echo "    Example:  $0 http://wcfci-sync-server/repo/sync.ashx 404"
    echo ""
}

run_request() 
{
    __sync_url=$1
    __ghprbPullId=$2

    __request_uri=${__sync_url}?pr=${__ghprbPullId}

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

if [ -z "$1" ]; then 
    show_usage
    exit 1
fi 

__sync_url=$1

if [ -z "$2" ]; then 
    if [ -z "$ghprbPullId" ]; then 
        show_usage
        echo "    This script should only be called only from the context of a GitHub Pull Request"
        echo "    'ghprbPullId' environment variable not set"
    
        exit 1
    else 
        __pr_id=$ghprbPullId
    fi  
else  
    __pr_id=$2
    show_usage 
    echo "    WARNING: This script should usually only be called only from the context of a GitHub Pull Request"
    echo "    PR ID overridden '${__pr_id}'"
fi

# Run 
run_request $__sync_url $__pr_id

__exit_code=$?

if [ $__exit_code -ne 0 ]; then
    echo "Server error when syncing to Pull Request"
fi 

exit $__exit_code 
