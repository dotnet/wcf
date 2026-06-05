#!/usr/bin/env bash

show_banner()
{
    echo ""
    echo "WCF Core CA Certificate Installer"
}

show_usage() 
{
    echo "    Usage: $0 --service-host [service-host] --cert-file [certificate-file]"
    echo "    service-host - hostname where service is running" 
    echo "    certificate-file - Full file path where the retrieved certificate should be saved"
    echo ""
}

acquire_certificate() 
{
    __cafile=$1
    
    echo "Obtaining certificate from '$ServiceUri'"
   
    # Need to make a call as the original user as we need to write to the cert store for the current 
    # user, not as root
    echo "Making a call to '${__service_host}/TestHost.svc/RootCert' as user '$SUDO_USER'"
    sudo -E -u $SUDO_USER $__curl_exe -o $__cafile "http://${__service_host}/TestHost.svc/RootCert?asPem=true" 
    
    return $?
}

install_root_cert()
{
    __cafile=$1
    chmod a+r "$__cafile"

    # Find the command for updating the OS certificate bundle
    echo "Updating root certificates with cert from '$__cafile'"

    case ${__os} in 
        "darwin")
            # macOS: install into the System keychain (admin domain) and grant full trust.
            # `add-trusted-cert -d` targets the admin domain (System.keychain). It is
            # non-interactive only when invoked as root, which is always the case here
            # (helix runs this script under `sudo -E`). We deliberately omit `-p ssl`
            # so the cert is trusted for ALL policies; specifying a policy narrows trust
            # to that policy only and has produced inconsistent SslStream chain validation
            # results in CI (dotnet/wcf#2870).
            $__update_os_certbundle_exec -v add-trusted-cert -d -r trustRoot -k /Library/Keychains/System.keychain ${__cafile}
            __add_rc=$?
            echo "[InstallRootCertificate] add-trusted-cert exit=${__add_rc}"

            # Force trustd to drop its in-memory cache and re-read the trust settings.
            # Without this, SecTrustEvaluate (used by .NET's X509Chain on macOS) can keep
            # returning the previous "untrusted" verdict for the lifetime of the helix VM,
            # even though `security verify-cert` already reports the cert as trusted.
            killall -HUP trustd 2>/dev/null || true

            # Pre-warm trustd's CRL cache.
            #
            # On macOS the CRL distribution-point fetcher in trustd is asynchronous and
            # batched: the first SecTrust evaluation that needs revocation typically
            # returns RevocationStatusUnknown because the CRL has not yet been fetched
            # and parsed. SecTrust with kSecRevocationRequirePositiveResponse then
            # rejects the chain.
            #
            # Mitigation: synchronously fetch the CRL ourselves before any test runs so
            # the bytes are at least present on the local network path and (more
            # importantly) prime trustd by issuing one synchronous verify against a
            # representative leaf so the CRL gets fetched and cached. We tolerate any
            # failure here -- this is best-effort priming, not a hard install gate.
            __crl_url="http://${__service_host}/TestHost.svc/Crl"
            __crl_file="/tmp/wcf-test.crl"
            __leaf_file="/tmp/wcf-test-leaf.pem"
            echo "[InstallRootCertificate] --- pre-warming CRL cache from ${__crl_url} ---"
            sudo -E -u $SUDO_USER $__curl_exe -fsS -o "${__crl_file}" "${__crl_url}" && \
                echo "[InstallRootCertificate] fetched CRL ($(wc -c < "${__crl_file}") bytes)" || \
                echo "[InstallRootCertificate] CRL fetch failed (continuing)"
            # Pull a representative server leaf cert (the same TestHost endpoint exposes /MachineCert).
            sudo -E -u $SUDO_USER $__curl_exe -fsS -o "${__leaf_file}" "http://${__service_host}/TestHost.svc/MachineCert?asPem=true" && \
                echo "[InstallRootCertificate] fetched leaf ($(wc -c < "${__leaf_file}") bytes)" || \
                echo "[InstallRootCertificate] leaf fetch failed (continuing)"
            if [ -s "${__leaf_file}" ]; then
                # -L = fetch + cache CRL synchronously via trustd; -R offline=0 ensures network is allowed.
                $__update_os_certbundle_exec verify-cert -L -c "${__leaf_file}" -p ssl 2>&1 | head -20 || true
                $__update_os_certbundle_exec verify-cert -L -c "${__leaf_file}" -p basic 2>&1 | head -20 || true
            fi

            # Diagnostics: dump cert details + verify the trust setting actually took effect.
            echo "[InstallRootCertificate] --- downloaded cert details ---"
            $__update_os_certbundle_exec -v find-certificate -a -p /Library/Keychains/System.keychain | head -20 || true
            __subject=$(openssl x509 -in "${__cafile}" -noout -subject -issuer -fingerprint -sha1 2>&1 || echo "openssl missing")
            echo "[InstallRootCertificate] cert: ${__subject}"
            echo "[InstallRootCertificate] --- verify-cert (ssl policy) ---"
            $__update_os_certbundle_exec verify-cert -c "${__cafile}" -p ssl 2>&1 || true
            echo "[InstallRootCertificate] --- admin trust settings dump ---"
            $__update_os_certbundle_exec dump-trust-settings -d 2>&1 | head -40 || true
            ;;
        "centos" | "rhel" | "fedora")
            cp -f "${__cafile}" /etc/pki/ca-trust/source/anchors

            if [ $? -ne 0 ]; then
                return $?
            fi

            $__update_os_certbundle_exec
            ;;
        "opensuse" | "sles")
            cp -f "${__cafile}" /etc/pki/trust/anchors

            if [ $? -ne 0 ]; then
                return $?
            fi

            $__update_os_certbundle_exec
            ;;
        *)
            cp -f "${__cafile}" /usr/local/share/ca-certificates
        
            if [ $? -ne 0 ]; then 
                return $? 
            fi

            $__update_os_certbundle_exec
            ;;
    esac 

    if [ $? -ne 0 ]; then
        return $?
    fi

    # Recalculate OpenSSL hashes; redirect stdout to /dev/null but allow stderr to show on console
    echo "Recalculating OpenSSL certificate hashes using c_rehash"
    $__c_rehash_exec > /dev/null 

    return $?
}

# Main execution

# Detect OS
# This is a pretty coarse detection, and we make the assumption that OSes will behave
# a certain way across versions. Hence the detection logic and placement of files will
# be assumed to be for the OS at the time of the writing of this script. We'll solve 
# the  later if there are additonal issues when we version up

if [[ "$(uname -a)" =~ ^.*(Darwin).*$ ]]; then
    # OSX
    __os="darwin"
elif [ -f /etc/os-release ]; then
    # ubuntu, debian, centos, rhel, fedora, opensuse (among others) are detected here 
    __os=`cat /etc/os-release | grep "^ID=" | sed 's/ID=//g' | sed 's/["]//g' | awk '{print $1}'`
else 
    echo "WARNING: Could not detect OS, defaulting to 'ubuntu'"
    __os="ubuntu"
fi 

# Parse arguments
while [[ $# > 0 ]]
do
    opt="$1"
    case $opt in
        -h|--help)
        show_usage
        ;;
        --service-host)
        __service_host=$2
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
echo "    Detected OS as: ${__os}"

__scriptpath=$(cd "$(dirname "$0")"; pwd -P)
__repopath=$__scriptpath/../../../..
__binpath=$__repopath/bin

readonly __scriptpath
readonly __repopath
readonly __binpath
readonly __os

# Check prerequisities
if [ `id -u` -ne 0 ]; then
    show_usage
    echo "ERROR: This script must be run under sudo or as a superuser" 
    exit 1
fi

# OpenSSL rehash - applicable on all platforms

__c_rehash_exec=`command -v c_rehash`
if [ $? -ne 0 -o ! -f "$__c_rehash_exec" ]; then 
    echo "WARNING: Could not find 'c_rehash'. Is OpenSSL installed properly?" 
fi

# Find the command for updating the OS certificate bundle
case ${__os} in 
    "darwin")
        __update_os_certbundle_cmd="security"
        ;;
    "centos" | "rhel" | "fedora") 
        __update_os_certbundle_cmd="update-ca-trust"
        ;;
    "ubuntu" | "debian" | "opensuse" | "sles")
        __update_os_certbundle_cmd="update-ca-certificates"
        ;;
    *)
        echo "ERROR: Could not detect OS certificate bundle update tool" 
        exit 1
        ;;
esac 

__update_os_certbundle_exec=`command -v ${__update_os_certbundle_cmd}`
if [ $? -ne 0 -o ! -f "$__update_os_certbundle_exec" ]; then 
    echo "ERROR: Could not find '${__update_os_certbundle_cmd}', which is needed to update certificates on '${__os}'" 
    exit 1
fi

__curl_exe=`command -v curl`

if [ ! -e "$__curl_exe" ]; then
    echo "Could not find cURL"
    echo "cURL is required to fetch the root certificates from the service"
    exit 1
fi

# Check parameters

if [ -z "$__cafile" ]; then 
    show_usage
    exit 1
else 
    echo "Certificate Authority will be written to '$__cafile'"
fi 

if [ -n "$__service_host" ]; then 
    echo "Certificate will be obtained from ServiceHost '$__service_host'"
    if [ -n "$ServiceUri" ]; then 
        echo "WARNING: This will replace the ServiceUri specified in the environment, '$ServiceUri', for the duration of this script"
    fi 
    
    # set ServiceUri as an external env var so that this can be picked up by the certificate installer
    # this is effectively local to this run as scripts usually run inside a new shell
    export ServiceUri=$__service_host
fi 

if [ -z "$ServiceUri" ]; then
    echo "WARNING: \$ServiceUri environment variable was not specified in the environment"
    echo "         This may be caused by running under sudo; run 'sudo -E' if you want to keep the user environment"
    echo ""
    echo "         Either set the ServiceUri environment variable, or run '$0' with a --service-host parameter"
    echo ""
    echo "         The default ServiceUri is 'localhost'."
    echo "         If the test service is running on a port other than the default, also set the 'BridgePort' variable"
    echo "         so that the installer can make the call to the service correctly"
fi

acquire_certificate $__cafile

# Install certificate
if [ $? -eq 0 ]; then 
    if [ -e "$__cafile" ]; then 
        install_root_cert "$__cafile"
    else 
        echo "ERROR: cURL returned an error - certificate authority file not written to '$__cafile' as expected"
        exit 1
    fi  
fi 

exit 0
