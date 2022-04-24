# dotnet-svcutil -- command-line tool for generating a web service reference compatible with .NET Core and .NET Standard projects.

## How to build

Execute following commandline under repository's root directory:
`eng\common\cibuild.cmd -preparemachine -configuration Release -projects dotnet-svcutil.sln /p:Test=false /p:Sign=false`

Alternatively, run `build.cmd`  which located at same directory of this readme file.

The built package is placed at:
`[RepoRootDir]/artifacts/artifacts/packages/[Release/Debug]/[Shipping/NonShipping]/[dotnet-svcutil.*.nupkg]`

## How to run test

Execute following commandline under repository's root directory:
`eng\common\cibuild.cmd -preparemachine -configuration Release -projects dotnet-svcutil.sln /p:Test=True /p:Sign=false`

Test result summary could be found at:
`[RepoRootDir]/artifacts/artifacts/TestResults/[Release/Debug]/dotnet-svcutil-lib.Tests*.xml`

Test result details could be found at:
`[RepoRootDir]/artifacts/artifacts/TestOutput/TestResults/`, each test variation has a corresponding folder generated.

## How to install and uninstall the tool

Uninstall: 
`dotnet tool uninstall --global dotnet-svcutil`

Install:
`dotnet tool install --global --add-source [RepoRootDir]/artifacts/packages/[Release/Debug]/[Shipping/NonShipping] dotnet-svcutil --version [tool-version]`

## Using dotnet-svcutil

Use it to generate web service proxy code from running services or static metadata documents.
                                              
### USAGE
| Input | Description| 
|:--------|:----------|
\<metadata document path\>|The path to a metadata document (wsdl or xsd). Standard command-line wildcards can be used in the file path.
\<url\>|The URL to a service endpoint that provides metadata or to a metadata document hosted online.
\<epr\>|The path to an XML file that contains a WS-Addressing EndpointReference for a service  endpoint that supports WS-Metadata Exchange.

Options:
| Option Input| Description| 
|:--------|:----------|
 --outputDir \<directory\>|Directory to create files in. Default: A directory called ServiceReference inside the current directory. (Short Form: -d)
 --noLogo|Suppress the copyright and banner message. (Short Form: -nl)
 --verbosity \<verbosity level\>|Determines the amount of information displayed by the tool. Valid values are 'Silent, Minimal, Normal, Verbose, Debug'. (Short Form: -v)
 --help|Display command syntax and options for the tool. (Short Form: -h)
 --projectFile \<project file\>|The project file to add the client to (if any). (Short Form: -pf).
 --outputFile \<file\>|The filename for the generated code. Default: derived from the WSDL definition name, WSDL service name or targetNamespace of one of the schemas.  (Short Form: -o)
 --namespace \<string,string\>|A mapping from a WSDL or XML Schema targetNamespace to a CLR namespace. Using the '*' for the targetNamespace maps all targetNamespaces without an explicit mapping to that CLR namespace. Default: derived from the target namespace of the schema document for Data Contracts. The default namespace is used for all other generated types. (Short Form: -n)
 --messageContract|Generate Message Contract types. (Short Form: -mc)
 --enableDataBinding|Implement the System.ComponentModel.INotifyPropertyChanged interface on all Data Contract types to enable data binding. (Short Form: -edb)
 --internal|Generate classes that are marked as internal. Default: generate public classes. (Short Form: -i)
 --reference \<package\>|Package or project references to reuse types from. When generating clients, use this option to specify references in the user's project that might  contain types representing the metadata being imported. The reference can be specified as a nuget package name and version or the path to a .Net Core project. (Short Form: -r)
 --noTypeReuse|Disable reusing types from project references. References provided with the --reference option will only be considered for resolving collection types  specified with the --collectionType option. (Short Form: -ntr)
 --collectionType \<type\>|A fully-qualified or assembly-qualified name of the type to use as a collection data type when code is generated from schemas. (Short Form: -ct)
 --excludeType \<type\>|A fully-qualified or assembly-qualified type name to exclude from referenced contract types. (Short Form: -et)
 --noStdLib|Do not reference standard libraries. By default System.Runtime (mscorlib) and System.ServiceModel (WCF) libraries are referenced. (Short Form: -nsl)
 --serializer Auto|Automatically select the serializer. This tries to use the Data Contract serializer and uses the XmlSerializer if that fails. (Short Form: -ser)
 --serializer DataContractSerializer|Generate data types that use the Data Contract Serializer for serialization and deserialization
 --serializer XmlSerializer|Generate data types that use the XmlSerializer for serialization and deserialization
 --sync|Generate synchronous methods for operations in addition to async. (Short Form: -syn)
 --wrapped|Generated code will not unwrap "parameters" member of document-wrapped-literal messages. (Short Form: -wr)
 --update \<web service reference\>|Updates an existing web service reference. If the project contains more than one web service reference, the name of the web service reference to be updated is required. (Short Form: -u)
 --runtimeIdentifier \<runtime identifier\>|The runtime identifier used for building reference projects (if any). (Short Form: -ri)
 --targetFramework \<framework\>|The target framework used for building reference projects. (Short Form: -tf).


### EXAMPLES
- Generate client code from a running service or online metadata documents.
`dotnet-svcutil http://example.com/service.svc?wsdl`

- Generate client code from a running service using settings that emulate the default "WCF Connected Service"
    behavior in Visual Studio 
``dotnet-svcutil http://example.com/service.svc?wsdl -edb -n "*,MyApplication.ServiceReference1" -ct MyLibrary.MyCollection`1 -r "{{MyLibrary,1.0.0}}"``

- Update existing web service reference named ServiceReference. The command need to be executed under directory
    which contains the project file.
`dotnet-svcutil -u ServiceReference`
