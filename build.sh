#!/usr/bin/env bash

usage()
{
    echo "Usage: $0 [BuildArch] [BuildType] [clean] [verbose] [platform]"
    echo "The following arguments affect native builds only:"
    echo "BuildArch can be: x64, arm"
    echo "BuildType can be: Debug, Release"
    echo "clean - optional argument to force a clean build."
    echo "verbose - optional argument to enable verbose build output."
    echo "platform can be: Windows, Linux, OSX, FreeBSD"

    exit 1
}

setup_dirs()
{
    echo Setting up directories for build

    mkdir -p "$__BinDir"
    mkdir -p "$__IntermediatesDir"
}

# Performs "clean build" type actions (deleting and remaking directories)

clean()
{
    echo "Cleaning previous output for the selected configuration"
    rm -rf "$__BinDir"
    rm -rf "$__IntermediatesDir"
    setup_dirs
}

# Check the system to ensure the right pre-reqs are in place

check_managed_prereqs()
{
    __monoversion=$(mono --version | grep "version 4.[1-9]")

    if [ $? -ne 0 ]; then
        # if built from tarball, mono only identifies itself as 4.0.1
        __monoversion=$(mono --version | egrep "version 4.0.[1-9]+(.[0-9]+)?")
        if [ $? -ne 0 ]; then
            echo "Mono 4.0.1.44 or later is required to build WCF. Please see https://github.com/dotnet/wcf/blob/master/Documentation/building/unix-instructions.md for more details."
            exit 1
        else
            echo "WARNING: Mono 4.0.1.44 or later is required to build WCF. Unable to assess if current version is supported."
        fi
    fi

    if [ ! -e "$__referenceassemblyroot/.NETPortable" ]; then
        echo "PCL reference assemblies not found. Please see https://github.com/dotnet/wcf/blob/master/Documentation/building/unix-instructions.md for more details."
        exit 1
    fi
}

# Prepare the system for building

prepare_managed_build()
{
    # Pull NuGet.exe down if we don't have it already
    if [ ! -e "$__nugetpath" ]; then
        which curl wget > /dev/null 2> /dev/null
        if [ $? -ne 0 -a $? -ne 1 ]; then
            echo "cURL or wget is required to build WCF. Please see https://github.com/dotnet/WCF/blob/master/Documentation/building/unix-instructions.md for more details."
            exit 1
        fi
        echo "Restoring NuGet.exe..."

        # curl has HTTPS CA trust-issues less often than wget, so lets try that first.
        which curl > /dev/null 2> /dev/null
        if [ $? -ne 0 ]; then
           mkdir -p $__packageroot
           wget -q -O $__nugetpath https://api.nuget.org/downloads/nuget.exe
        else
           curl -sSL --create-dirs -o $__nugetpath https://api.nuget.org/downloads/nuget.exe
        fi

        if [ $? -ne 0 ]; then
            echo "Failed to restore NuGet.exe."
            exit 1
        fi
    fi

    # Grab the MSBuild package if we don't have it already
    if [ ! -e "$__msbuildpath" ]; then
        echo "Restoring MSBuild..."
        mono "$__nugetpath" install $__msbuildpackageid -Version $__msbuildpackageversion -source "https://www.myget.org/F/dotnet-buildtools/" -OutputDirectory "$__packageroot"
        if [ $? -ne 0 ]; then
            echo "Failed to restore MSBuild."
            exit 1
        fi
    fi
}

build_managed_wcf()
{
    __buildproj=$__scriptpath/build.proj
    __buildlog=$__scriptpath/msbuild.log

    MONO29679=1 ReferenceAssemblyRoot=$__referenceassemblyroot mono $__msbuildpath "$__buildproj" /nologo /verbosity:minimal "/fileloggerparameters:Verbosity=normal;LogFile=$__buildlog" /t:Build /p:OSGroup=$__BuildOS /p:COMPUTERNAME=$(hostname) /p:USERNAME=$(id -un) /p:TestNugetRuntimeId=$__TestNugetRuntimeId /p:ToolNugetRuntimeId=$__TestNugetRuntimeId $__UnprocessedBuildArgs
    BUILDERRORLEVEL=$?

    echo

    # Pull the build summary from the log file
    tail -n 4 "$__buildlog"
    echo Build Exit Code = $BUILDERRORLEVEL
}

__scriptpath=$(cd "$(dirname "$0")"; pwd -P)
__nativeroot=$__scriptpath/src/Native
__packageroot=$__scriptpath/packages
__sourceroot=$__scriptpath/src
__nugetpath=$__packageroot/NuGet.exe
__nugetconfig=$__sourceroot/NuGet.Config
__rootbinpath="$__scriptpath/bin"
__msbuildpackageid="Microsoft.Build.Mono.Debug"
__msbuildpackageversion="14.1.0.0-prerelease"
__msbuildpath=$__packageroot/$__msbuildpackageid.$__msbuildpackageversion/lib/MSBuild.exe
__BuildArch=x64
__buildmanaged=false
__buildnative=false
__TestNugetRuntimeId=win7-x64

# Use uname to determine what the OS is.
OSName=$(uname -s)
case $OSName in
    Linux)
        __HostOS=Linux
        __TestNugetRuntimeId=ubuntu.14.04-x64
        ;;

    Darwin)
        __HostOS=OSX
        __TestNugetRuntimeId=osx.10.10-x64
        ;;

    FreeBSD)
        __HostOS=FreeBSD
        __TestNugetRuntimeId=osx.10.10-x64
        ;;

    *)
        echo "Unsupported OS $OSName detected, configuring as if for Linux"
        __HostOS=Linux
        __TestNugetRuntimeId=ubuntu.14.04-x64
        ;;
esac
__BuildOS=$__HostOS
__BuildType=Debug
__CMakeArgs=DEBUG

case $__HostOS in
    FreeBSD)
        __monoroot=/usr/local
        ;;
    OSX)
        __monoroot=/Library/Frameworks/Mono.framework/Versions/Current
        ;;
    *)
        __monoroot=/usr
        ;;
esac

__referenceassemblyroot=$__monoroot/lib/mono/xbuild-frameworks
BUILDERRORLEVEL=0

# Set the various build properties here so that CMake and MSBuild can pick them up
__UnprocessedBuildArgs=
__CleanBuild=false
__VerboseBuild=false

for i in "$@"
    do
        lowerI="$(echo $i | awk '{print tolower($0)}')"
        case $lowerI in
        -?|-h|--help)
            usage
            exit 1
            ;;
        x64)
            __BuildArch=x64
            __MSBuildBuildArch=x64
            ;;
        arm)
            __BuildArch=arm
            __MSBuildBuildArch=arm
            ;;
        debug)
            __BuildType=Debug
            ;;
        release)
            __BuildType=Release
            ;;
        clean)
            __CleanBuild=1
            ;;
        verbose)
            __VerboseBuild=1
            ;;
        windows)
            __BuildOS=Windows_NT
            ;;
        linux)
            __BuildOS=Linux
            __TestNugetRuntimeId=ubuntu.14.04-x64
            ;;
        osx)
            __BuildOS=OSX
            __TestNugetRuntimeId=osx.10.10-x64
            ;;
        freebsd)
            __BuildOS=FreeBSD
            ;;
        *)
          __UnprocessedBuildArgs="$__UnprocessedBuildArgs $i"
    esac
done

# Set the remaining variables based upon the determined build configuration
__IntermediatesDir="$__rootbinpath/obj/$__BuildOS.$__BuildArch.$__BuildType/Native"
__BinDir="$__rootbinpath/$__BuildOS.$__BuildArch.$__BuildType/Native"

# Make the directories necessary for build if they don't exist

setup_dirs

# Check prereqs.

check_managed_prereqs

# Prepare the system

prepare_managed_build

# Build the wcf managed components.

build_managed_wcf

# Build complete

# If managed build failed, exit with the status code of the managed build
if [ $BUILDERRORLEVEL != 0 ]; then
    exit $BUILDERRORLEVEL
fi

exit $BUILDERRORLEVEL
