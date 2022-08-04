FROM mcr.microsoft.com/dotnet/sdk:6.0.300-focal-amd64

CMD /wcf/src/System.Private.ServiceModel/tools/scripts/SetClientEnv-Linux.sh && /wcf/src/System.Private.ServiceModel/tools/scripts/InstallRootCertificate.sh --cert-file /tmp/wcfrootca.crt && /wcf/eng/common/cibuild.sh -configuration Release --prepareMachine --ci --test --integrationTest

