# Using pre-release WCF packages in applications

This GitHub project builds the WCF packages used to develop client and mid-tier NET Core WCF applications.  Microsoft publishes the packages built by this repo to NuGet.org (example: [System.ServiceModel.Primitives](https://www.nuget.org/packages/System.ServiceModel.Primitives)). 

Microsoft also publishes pre-release versions of these packages each night that show work-in-progress.  And you can use this repo to build the WCF packages yourself, capable of being used in Windows, Linux, OSX, etc.

This document describes how you can create WCF applications that use Microsoft's nightly package releases or your own personally built WCF packages.

## Usage Scenarios

### Identifying alternate NuGet package sources

When you build a NET Core WCF application, by default the packages you use come from NuGet.org.  If you look at one of the packages (example: [System.ServiceModel.Primitives](https://www.nuget.org/packages/System.ServiceModel.Primitives)) you will see the different versions available.  Some are "stable" packages corresponding to an official release (example: [4.1.0](https://www.nuget.org/packages/System.ServiceModel.Primitives/4.1.0)) and some are "pre-release" packages, corresponding to early looks at a pending release (example: [4.3.0-preview1-24610-02](https://www.nuget.org/packages/System.ServiceModel.Primitives/4.3.0-preview1-24610-02)).

NuGet.org is an example of a "package source." To make other package sources available to your application, you just need to identify their location in a NuGet.config file.  Where this file lives and how it is handled is described at [Configurating NuGet behavior](https://docs.nuget.org/ndocs/consume-packages/configuring-nuget-behavior).  You can modify this text file directly or in Visual Studio use the Tools | NuGet Package Manager | Package Manager Settings menu.

Package sources can be either a NuGet "feed" such as [https://api.nuget.org/v3/index.json](https://api.nuget.org/v3/index.json) or just a file directory path containing the *.nupkg files.  Here is the relevant section from my NuGet.config for the 2 examples in this article:
```
<packageSources>
  <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
  <add key="nuget.org" value="https://www.nuget.org/api/v2/" />
  <add key="DotNetMyGet" value="https://dotnet.myget.org/f/dotnet-core" />
  <add key="WcfRepo" value="c:\git\wcf\bin\packages\Debug" />
</packageSources>
```

### Build a WCF application using nightly pre-release packages

Each night, Microsoft builds and publishes packages representing the latest code in their respective GitHub repo's.  You can see these at the [dotnet.myget.org gallery](https://dotnet.myget.org/gallery).

To be able to use packages from this package source, just add `https://dotnet.myget.org/f/dotnet-core` as a package source to your NuGet.config file.  Each package source also requires a name, but you can choose any unique name you want (example: I used `DotNetMyGet` and `WcfRepo` above).

Once you've added your new package source(s), you can use the Package Manager in Visual Studio to browse any or all of your named package sources and choose which packages to install.

Naturally, these nightly packages represent work in progress, and there are no guarantees they are fully functional.  Internally, Microsoft runs the unit and functional tests in each repro against these nightly packages and publishes the results.  For example, scroll down the main page at https://github.com/dotnet/wcf to see the results of the prior night's test runs.

If you have an existing WCF application you want to test against the nightly build packages, edit that project's `project.json` file so that all 'System.ServiceModel' packages specify exactly the versions you want, like this:

```
{
  "version": "1.0.0-*",
  "buildOptions": {
    "emitEntryPoint": true
  },
  "dependencies": {
    "Microsoft.NETCore.App": {
      "type": "platform",
      "version": "1.0.0"
    }
  },
  "frameworks": {
    "netcoreapp1.0": {
      "imports": "dnxcore50",
      "dependencies": {
        "System.ServiceModel.Duplex": "4.4.0-beta-24708-0",
        "System.ServiceModel.Http": "4.4.0-beta-24708-0",
        "System.ServiceModel.NetTcp": "4.4.0-beta-24708-0",
        "System.ServiceModel.Security": "4.4.0-beta-24708-0",
        "System.Xml.XmlSerializer": "4.4.0-beta-24708-02"
      }
    }
  }
}

```

Note that in VS 2017 RC, package references will migrate from project.json files directly into the project's .csproj file.  To make this identical change to a WCF application created using VS 2017 RC, the changes in the .csproj file look like this:

```
  <ItemGroup>
    <PackageReference Include="Microsoft.NETCore.App">
      <Version>1.0.1</Version>
    </PackageReference>
    <PackageReference Include="Microsoft.NET.Sdk">
      <Version>1.0.0-alpha-20161104-2</Version>
      <PrivateAssets>All</PrivateAssets>
    </PackageReference>
    <PackageReference Include="System.ServiceModel.Duplex">
      <Version>4.4.0-beta-24707-0</Version>
    </PackageReference>
    <PackageReference Include="System.ServiceModel.Http">
      <Version>4.4.0-beta-24708-0</Version>
    </PackageReference>
    <PackageReference Include="System.ServiceModel.NetTcp">
      <Version>4.4.0-beta-24708-0</Version>
    </PackageReference>
    <PackageReference Include="System.ServiceModel.Primitives">
      <Version>4.4.0-beta-24708-0</Version>
    </PackageReference>
    <PackageReference Include="System.ServiceModel.Security">
      <Version>4.4.0-beta-24708-0</Version>
    </PackageReference>
    <PackageReference Include="System.Xml.XmlSerializer">
      <Version>4.3.0-preview1-24530-04</Version>
    </PackageReference>
  </ItemGroup>

```

### Build a WCF application using packages you build

You can also consume WCF packages you build yourself using a clone or fork of https://github.com/dotnet/wcf.  To do this:

1. Build the packages using 'build.cmd" in a CMD window.
2. Edit NuGet.config to declare the location of the generated .nupkg files (again, use VS Package Manager Settings or just directly edit the NuGet.config file)

For example, I just created a new package source to point directly to the folder where build.cmd placed the WCF built packages: `c:\git\wcf\bin\packages\Debug`.

You need to update your `project.json` (VS 2015) or your corresponding .csproj file (VS 2017 RC) as described above to name the specific package versions you want. In this case, the .nupkg files under `c:\git\wcf\bin\packages\Debug` are named with the version numbers, like this:

```
System.Private.ServiceModel.4.4.0-beta-24709-0.nupkg
System.ServiceModel.Duplex.4.4.0-beta-24709-0.nupkg
System.ServiceModel.Http.4.4.0-beta-24709-0.nupkg
System.ServiceModel.NetTcp.4.4.0-beta-24709-0.nupkg
System.ServiceModel.Primitives.4.4.0-beta-24709-0.nupkg
System.ServiceModel.Security.4.4.0-beta-24709-0.nupkg
```




