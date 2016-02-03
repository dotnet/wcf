#!/usr/bin/env bash

show_usage() 
{
    echo ""
    echo "WCF Core Bridge CA Certificate Bootstrapper" 
    echo "    Usage: $0 [bridge-host] [certificate-file]"
    echo ""
    echo "    bridge-host - hostname where Bridge is running" 
    echo "    certificate-file - Full file path where the retrieved certificate should be saved"
    echo ""
}

run_bootstrapper() 
{
    __bridge_host=$1
    __outfile=$2

     # Locate any CoreRun.exe to be able to execute BridgeCertificateBootstrapper.exe
     # Because BridgeCertificateBootstrapper itself is a test project, we are guaranteed
     # it will contain CoreRun.exe even if no other tests have been built.
    __scriptpath=$(cd "$(dirname "$0")"; pwd -P)
    __binpath=$__scriptpath/../../../../bin
    __testspath=$__binpath/tests
    __corerun_path=$(dirname "$(find $__testspath -iname 'ScenarioTests.Common.dll' | head -1)")

    if [ $? -ne 0 ]; then
        echo "Could not find 'ScenarioTests.Common.dll' under path '$__binpath'" 
        exit 1
    fi

    __corerun_exe="$__corerun_path/corerun"
       
    if [ ! -e "$__corerun_exe" ]; then 
        echo "Could not find corerun in path $__corerun_path"
        exit 1
    fi

    __bootstrapper_path=$(find $__binpath -iname 'BridgeCertificateBootstrapper.exe' | head -1)
   
    if [ "$__bootstrapper_path" == "" ]; then 
        echo "Could not find 'BridgeCertificateBootstrapper.exe' under path '$__binpath'" 
        exit 1
    fi 

    echo "Found bootstrapper at '$__bootstrapper_path'" 
    
    export BridgeHost=$__bridge_host

    if [ -z "${BridgeHost:+x}" ]; then 
        echo "WARNING: No \$BridgeHost environment variable specified." 
        echo "         The default BridgeHost is 'localhost'."
        echo "         Please set the 'BridgeHost' environment variable using the command 'export BridgeHost=<hostname>'" 
        echo "         If the Bridge is running on a port other than the default, also set the 'BridgePort' variable"
        echo "         so that the bootstrapper can make the call to the Bridge correctly."
    fi 

    echo "Running bootstrapper and making a call to '$BridgeHost'"
    # echo $__corerun_exe $__bootstrapper_path $__outfile
   
    $__corerun_exe $__bootstrapper_path $__outfile
    
    return 0
}

install_root_cert()
{
    __cafile=$1

    cp -f "$__cafile" /usr/local/share/ca-certificates

    if [ $? -ne 0 ]; then 
        return $? 
    fi 

    echo "Updating root certificates with cert from '$__cafile'"
    update-ca-certificates

    return $?
}

# Main execution

if [ -z "$1" -o -z "$2" ]; then 
    show_usage
    exit 1
else 
    __bridge_host=$1
    __cafile=$2
    echo "Certificate Authority will be written to '$__cafile'"
fi 

if [ `id -u` -ne 0 ]; then
    echo "ERROR: This script must be run under sudo or as a superuser" 
    exit 1
fi

run_bootstrapper $__bridge_host $__cafile

if [ $? -eq 0 ]; then 
    if [ -e "$__cafile" ]; then 
        install_root_cert "$__cafile"
    else 
        echo "ERROR: Certificate authority file not written to '$__cafile' as expected"
    fi  
fi 

