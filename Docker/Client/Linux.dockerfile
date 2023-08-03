# Linux Docker image - Host WCF Client
FROM mcr.microsoft.com/dotnet/sdk:6.0.300-focal-amd64

# Run commands:
# Setup environment variables
# Install Root certificate
# Run WCF Tests
CMD /wcf/src/System.Private.ServiceModel/tools/scripts/SetClientEnv-Linux.sh && /wcf/src/System.Private.ServiceModel/tools/scripts/InstallRootCertificate.sh --cert-file /tmp/wcfrootca.crt && /wcf/eng/common/cibuild.sh -configuration Release --prepareMachine --ci --test --integrationTest

