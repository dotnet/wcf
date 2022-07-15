FROM mcr.microsoft.com/dotnet/sdk:6.0.300-focal-amd64

CMD /wcf/src/System.Private.ServiceModel/tools/scripts/SetClientEnv-Linux.sh && /wcf/eng/common/cibuild.sh -configuration Release -prepareMachine -t --integrationTest
