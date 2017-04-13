This document describes how to get started with WCF for .NET Core in containers. It will guide you through creating a sample WCF client & server apps, creating a Windows and Linux Docker containers and share additional details on how to install client certificates in Windows containers.

Prerequisites:
- Windows Server 2016 or Windows 10 with "Containers" feature enabled
- Latest [Docker](https://www.docker.com) version installed
- If running Docker inside a VM, make sure you [enabled nested virtualization for the VM](https://blogs.technet.microsoft.com/virtualization/2015/10/13/windows-insider-preview-nested-virtualization/) 
- For getting started with Windows containers please refer to this document: https://docs.microsoft.com/en-us/virtualization/windowscontainers/quick-start/quick-start-windows-10
- For running a sample (and similar simple service apps) in a container you can follow a blog post by Jeffrey T. Fritz at https://blogs.msdn.microsoft.com/webdev/2017/02/20/lets-try-wcf-self-hosted-services-in-a-container/


### 1. Sample app
These are the steps for creating a sample WCF service and client. If you already have a sample WCF service app, and a corresponding .NET Core WCF client project then you may skip the sample app creation and proceed to step 2. 
For the purpose of this demo we’ll go ahead and just create a WCF service app from a default project template. Among other things this will generate a service and a data contract that look like the one below (IService1.cs):

~~~
using System;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace HelloWcfService
{
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        string GetData(int value);
        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);
    }

    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";
        [DataMember]
        public bool BoolValue { get { return boolValue; } set { boolValue = value; } }
        [DataMember]
        public string StringValue  { get { return stringValue; } set { stringValue = value; } }
    }
}
~~~

For further simplicity let’s skip the “standard” path of service reference generation and simply make a copy of it for our future WCF core client. Next, we’re going to write our WCF client code (program.cs):

~~~
using HelloWcfService;
using System;
using System.ServiceModel;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var url = Environment.GetEnvironmentVariable("url");
            var helloEndpoint = new EndpointAddress(url);
            var binding = new BasicHttpBinding();
            var factory = new ChannelFactory<IService1>(binding, helloEndpoint);
            var client = factory.CreateChannel();
            var data = client.GetData(5);
            Console.WriteLine(data);
        }
    }
}
~~~

Finally, we’ll need a project file. As .NET Core evolves the project file may look different depending on the version of .NET core SDK you’re going to use in the container. One way to generate a project file compatible with a particular version of SDK (or Visual Studio) is to run the following (make sure the directory you’re running this in doesn’t already have a project file):

~~~
mkdir wcfClient
cd wcfClient
dotnet new console
~~~

and use the newly created project file as a template for your WCF client project. The project that we’re providing here is created by Visual Studio 2017. The last step is to add references to WCF .NET Core project like the one below. Again, depending on the SDK version, the format may change (wcfClient.csproj):

~~~
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp1.1</TargetFramework>
  </PropertyGroup>
<ItemGroup>
    <PackageReference Include="System.ServiceModel.Http">
      <Version>4.3.0</Version>
    </PackageReference>
    <PackageReference Include="System.ServiceModel.NetTcp">
      <Version>4.3.0</Version>
    </PackageReference>
    <PackageReference Include="System.ServiceModel.Primitives">
      <Version>4.3.0</Version>
    </PackageReference>
    <PackageReference Include="System.ServiceModel.Duplex">
      <Version>4.3.0</Version>
    </PackageReference>
    <PackageReference Include="System.ServiceModel.Security">
      <Version>4.3.0</Version>
    </PackageReference>
  </ItemGroup>
</Project>
~~~

#### To sum it up: you have a  default WCF template (or your own) service running and a folder called “wcfClient” with the client app that has 3 files: wcfClient.csproj, program.cs, and IService1.cs.

### 2. Container base image
.NET Core Runtime Docker images are available for both [released versions](https://github.com/dotnet/dotnet-docker) and for [nightly builds](https://hub.docker.com/r/microsoft/dotnet-nightly/). One can get images that have only the .NET Core runtime or both the runtime and the SDK. If you're using your own custom WCF client app with a dependency on a certain .NET Core version then please take a look at the available Docker images and pick the right one for your app. We’re going to use Nano Server and Linux SDK images for the latest 1.1.1 nightly build (which are compatible with the project file that we created above).

### 3. Dockerfile
Our goal here is to have a dockerfile that restores necessary packages and builds the app at the time when we build the container. Should you pick up a different base layer and rebuild your container – the app will automatically use the new binaries. Having a container with a shared .NET Core framework helps to save space and to speed up launching new containers with the same base image.

#### Dockerfile for Linux
~~~
FROM microsoft/dotnet-nightly:1.1.1-sdk
COPY wcfClient wcfClient
RUN dotnet restore wcfClient/wcfClient.csproj
RUN dotnet build wcfClient/wcfClient.csproj
CMD dotnet run -p wcfClient/wcfClient.csproj
~~~

#### Dockerfile for Windows Nano Server
~~~
FROM microsoft/dotnet-nightly:1.1.1-sdk-nanoserver
COPY wcfClient wcfClient
RUN dotnet restore wcfClient\wcfClient.csproj
RUN dotnet build wcfClient\wcfClient.csproj
CMD dotnet run -p wcfClient\wcfClient.csproj 
~~~
### 4. Build and run the container
As you noticed our dockerfile copies files from “wcfClient” folder (with our sample client app created in step 1) on the host to “wcfClient” folder inside the container. So we're just going to place the dockerfile above to “wcfClient” parent folder and run the following commands. Since our sample app reads an environment variable named "url" to create an endpoint we pass it via --env parameter:

~~~
docker build -t wcf_client_in_container .
docker run --env url=http://server_name/app_name/Service1.svc -it --rm wcf_client_in_container
~~~

### 5. Windows containers and client certificates
WCF .NET Core supports client certificate authentication and it is fully supported inside containers. Both Windows Nano and Windows Core Server provide powershell cmdlets to install client certificates. You can use the following commands to install the certificates during the container build. For example if your certificate authority public key is in cert_authority_public.cer and client certificate is client_cert.pfx (protected by pwd “password”) then the dockerfile will have the following powershell commands:
~~~
$pass = ConvertTo-SecureString -AsPlainText -Force password 
Import-pfxcertificate -certStoreLocation Cert:LocalMachine\My -Password $pass -FilePath client_cert.pfx
Import-Certificate -certStoreLocation Cert:LocalMachine\Root -FilePath cert_authority_public.cer
~~~

