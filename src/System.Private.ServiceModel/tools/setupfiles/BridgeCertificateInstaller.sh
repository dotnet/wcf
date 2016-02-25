#!/usr/bin/env bash

show_banner()
{
    echo ""
    echo "WCF Core Bridge CA Certificate Installer"
}

show_usage() 
{
    echo "    Usage: $0 --bridge-host [bridge-host] --cert-file [certificate-file]"
    echo "    bridge-host - hostname where Bridge is running" 
    echo "    certificate-file - Full file path where the retrieved certificate should be saved"
    echo ""
}

run_installer() 
{
    __cafile=$1

    echo "Using certificate installer at '$__installer_path'" 
    
    echo "Running installer and making a call to '$BridgeHost'"
    # echo $__corerun_exe $__installer_path $__cafile
   
    # Need to make a call as the original user as we need to write to the cert store for the current 
    # user, not as root
    echo "Making a call to the Bridge as user '$SUDO_USER'"
    sudo -E -u $SUDO_USER $__corerun_exe $__installer_path $__cafile
    
    return $?
}

install_root_cert()
{
    __cafile=$1

    cp -f "$__cafile" /usr/local/share/ca-certificates

    if [ $? -ne 0 ]; then 
        return $? 
    fi 

    echo "Updating root certificates with cert from '$__cafile'"
    $__update_ca_certificates_exec

    return $?
}

# Main execution

# Parse arguments
while [[ $# > 0 ]]
do
    opt="$1"
    case $opt in
        -h|--help)
        show_usage
        ;;
        --bridge-host)
        __bridge_host=$2
        ;;
        --cert-file)
        __cafile=$2
        ;;
        *)
        ;;
    esac
    shift
done

show_banner

__scriptpath=$(cd "$(dirname "$0")"; pwd -P)
__binpath=$__scriptpath/../../../../bin
__testspath=$__binpath/tests

# Check prerequisities
if [ `id -u` -ne 0 ]; then
    show_usage
    echo "ERROR: This script must be run under sudo or as a superuser" 
    exit 1
fi

__update_ca_certificates_exec=`which update-ca-certificates`
if [ $? -ne 0 -o ! -f "$__update_ca_certificates_exec" ]; then 
    echo "ERROR: Could not find 'update-ca-certificates', which is needed to update certificates" 
    exit 1
fi

# Locate any CoreRun.exe to be able to execute BridgeCertificateInstaller.exe
# Because BridgeCertificateInstaller itself is a test project, we are guaranteed
# it will contain CoreRun.exe even if no other tests have been built.
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

__installer_path=$(find $__binpath -iname 'BridgeCertificateInstaller.exe' | head -1)

if [ "$__installer_path" == "" ]; then
    echo "Could not find 'BridgeCertificateInstaller.exe' under path '$__binpath'"
    exit 1
fi

# Check parameters

if [ -z "$__cafile" ]; then 
    show_usage
    exit 1
else 
    echo "Certificate Authority will be written to '$__cafile'"
fi 

if [ -n "$__bridge_host" ]; then 
    echo "Certificate will be obtained from BridgeHost '$__bridge_host'"
    if [ -n "$BridgeHost" ]; then 
        echo "WARNING: This will replace the BridgeHost specified in the environment, '$BridgeHost', for the duration of this script"
    fi 
    
    # set BridgeHost as an external env var so that this can be picked up by the Bridge
    # this is effectively local to this run as scripts usually run inside a new shell
    export BridgeHost=$__bridge_host
fi 

if [ -z "$BridgeHost" ]; then
    echo "WARNING: \$BridgeHost environment variable was not specified in the environment"
    echo "         This may be caused by running under sudo; run 'sudo -E' if you want to keep the user environment"
    echo ""
    echo "         Either set the BridgeHost environment variable, or run '$0' with a --bridge-host parameter"
    echo ""
    echo "         The default BridgeHost is 'localhost'."
    echo "         If the Bridge is running on a port other than the default, also set the 'BridgePort' variable"
    echo "         so that the installer can make the call to the Bridge correctly"
fi

# Run certificate installer 
run_installer $__cafile

# Install certificate
if [ $? -eq 0 ]; then 
    if [ -e "$__cafile" ]; then 
        install_root_cert "$__cafile"
    else 
        echo "ERROR: Certificate authority file not written to '$__cafile' as expected"
    fi  
fi 

exit 0
