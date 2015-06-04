Intro to WCF
================

Windows Communication Foundation (WCF) has been a part of the full .NET framework for years.  This WCF repository contains source code for all the client libraries originally available for the Windows Store but made compatible with the new [.NET Core Runtime](https://github.com/dotnet/coreclr).

Project Goals and Priorities
============================

Microsoft published WCF to GitHub, with the following goals:

- Establish a high-quality open source .NET implementation of the WCF client libraries.
- Work with the open source community to manage and extend the available libraries.

Reporting Issues
================
To report an issue or just to open the discussion for a proposed feature, open a [New Issue](https://github.com/dotnet/wcf/issues/new). Please assign labels to associate it with the appropriate library if you know it. Example: "System.ServiceModel.Http".

Contributing
============

Please read [Contributing](https://github.com/dotnet/corefx/blob/master/Documentation/contributing.md) to .NET Core before making your first contribution.

Building the repository
=======================

The WCF repository can be built from a regular, non-admin command prompt using build.cmd. This build produces a single System.Private.ServiceModel assembly that implements each of these individual client libraries:
 - System.ServiceModel.Primitives
 - System.ServiceModel.Http
 - System.ServiceModel.NetTcp
 - System.ServiceModel.Duplex
 - System.ServiceModel.Security

Microsoft uses this repository to create and publish separate NuGet packages for each library. 

The repository is a work in progress, and not all of the libraries are complete yet.