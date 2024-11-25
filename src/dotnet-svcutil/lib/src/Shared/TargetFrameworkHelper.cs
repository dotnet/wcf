// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class TargetFrameworkHelper
    {
        private static readonly Version s_minEOFNetVersion = new Version("5.0");
        private static readonly Version s_maxEOFNetVersion = new Version("7.0");
        internal static readonly List<string> s_supportedVersions = new List<string>() { "8.0", "9.0", "10.0" };
        
        public static Version MinSupportedNetFxVersion { get; } = new Version("4.5");
        public static Version MinSupportedNetStandardVersion { get; } = new Version("1.3");
        public static Version MinSupportedNetCoreAppVersion { get; } = new Version("1.0");

        internal static SortedDictionary<string, List<ProjectDependency>> NetCoreVersionReferenceTable = new SortedDictionary<string, List<ProjectDependency>>
        {
            {"Legacy", new List<ProjectDependency> {
                ProjectDependency.FromPackage("System.ServiceModel.Duplex", "4.10.*"  ),
                ProjectDependency.FromPackage("System.ServiceModel.Http", "4.10.*"    ),
                ProjectDependency.FromPackage("System.ServiceModel.NetTcp", "4.10.*"  ),
                ProjectDependency.FromPackage("System.ServiceModel.Security", "4.10.*"),
                ProjectDependency.FromPackage("System.ServiceModel.Federation", "4.10.*")
            } },
            {"Current", new List<ProjectDependency> {
                ProjectDependency.FromPackage("System.ServiceModel.Http", "8.*"),
                ProjectDependency.FromPackage("System.ServiceModel.NetTcp", "8.*"),
                ProjectDependency.FromPackage("System.ServiceModel.Primitives", "8.*")
            } }
        };

        internal static List<ProjectDependency> FullFrameworkReferences = new List<ProjectDependency>()
        {
            ProjectDependency.FromAssembly("System.ServiceModel"),
        };

        internal static List<ProjectDependency> ServiceModelPackages = new List<ProjectDependency>()
        {
            ProjectDependency.FromPackage("System.ServiceModel.Duplex", "*"),
            ProjectDependency.FromPackage("System.ServiceModel.Http", "*" ),
            ProjectDependency.FromPackage("System.ServiceModel.NetTcp", "*"),
            ProjectDependency.FromPackage("System.ServiceModel.Primitives", "*"),
            ProjectDependency.FromPackage("System.ServiceModel", "System.ServiceModel.Primitives", "*"),
            ProjectDependency.FromPackage("System.Private.ServiceModel", "*"),
            ProjectDependency.FromPackage("System.ServiceModel.Security", "*"),
            ProjectDependency.FromPackage("System.Xml.XmlSerializer", "*"),
            ProjectDependency.FromPackage("System.ServiceModel.Federation", "*"),
            ProjectDependency.FromPackage("System.ServiceModel.NetNamedPipe", "*"),
            ProjectDependency.FromPackage("System.ServiceModel.NetFramingBase", "*")
        };

        public static IEnumerable<ProjectDependency> GetWcfProjectReferences(IEnumerable<string> targetFrameworks)
        {
            // If there is no .NET Core or .NET Standard target framework, return FullFrameworkReferences
            if (!targetFrameworks.Any(targetFramework => IsSupportedFramework(targetFramework, out var frameworkInfo) && frameworkInfo.IsDnx))
            {
                return FullFrameworkReferences;
            }

            // Determine the lowest .NET Core version in the target frameworks
            Version netCoreVersion = GetLowestNetCoreVersion(targetFrameworks);

            // Return the appropriate WCF reference based on the target framework version
            // Behavior table based on framework combinations:
            // netstandard2.0       :  add WCF reference 4.10.*
            // netstandard2.0;net8.0:  add WCF reference 8.*
            // netstandard2.0;net6.0:  add WCF reference 4.10.*
            // net6.0;net8.0        :  add WCF reference 4.10.*
            if (netCoreVersion != null && s_supportedVersions.Contains(netCoreVersion.ToString()))
            {
                return NetCoreVersionReferenceTable["Current"];
            }
            // Otherwise, return the legacy WCF reference
            return NetCoreVersionReferenceTable["Legacy"];
        }

        /// <summary>
        /// From the specified framework collection, find the the framework that would work best for Svcutil (if any).
        /// </summary>
        public static string GetBestFitTargetFramework(IEnumerable<string> targetFrameworks)
        {
            string targetFramework = string.Empty;
            FrameworkInfo fxInfo = null;

            if (targetFrameworks != null)
            {
                targetFramework = targetFrameworks.FirstOrDefault((framework) => IsSupportedFramework(framework, out fxInfo) && fxInfo.IsDnx);
                if (targetFramework == null)
                {
                    targetFramework = targetFrameworks.FirstOrDefault((framework) => IsSupportedFramework(framework, out fxInfo));
                    if (targetFramework == null)
                    {
                        targetFramework = targetFrameworks.FirstOrDefault() ?? string.Empty;
                    }
                }
            }

            return targetFramework;
        }

        public static Version GetLowestNetCoreVersion(IEnumerable<string> targetFrameworks)
        {
            Version targetVersion = null;

            foreach (string targetFramework in targetFrameworks)
            {
                if (TargetFrameworkHelper.IsSupportedFramework(targetFramework, out var frameworkInfo) && frameworkInfo.IsDnx && frameworkInfo.Name != FrameworkInfo.Netstandard)
                {
                    if (targetVersion == null || targetVersion > frameworkInfo.Version)
                    {
                        targetVersion = frameworkInfo.Version;
                    }
                }
            }

            return targetVersion;
        }

        public static FrameworkInfo GetValidFrameworkInfo(string targetFramework)
        {
            if (!IsSupportedFramework(targetFramework, out FrameworkInfo frameworkInfo))
            {
                throw new Exception(string.Format(CultureInfo.CurrentCulture, Shared.Resources.ErrorNotSupportedTargetFrameworkFormat,
                                                  targetFramework, MinSupportedNetCoreAppVersion, MinSupportedNetStandardVersion, MinSupportedNetFxVersion));
            }
            return frameworkInfo;
        }

        public static bool IsSupportedFramework(string fullFrameworkName, out FrameworkInfo frameworkInfo)
        {
            bool isSupported = false;

            if (FrameworkInfo.TryParse(fullFrameworkName, out frameworkInfo))
            {
                isSupported = (frameworkInfo.Name == FrameworkInfo.Netstandard && frameworkInfo.Version >= MinSupportedNetStandardVersion) ||
                              (frameworkInfo.Name == FrameworkInfo.Netcoreapp && frameworkInfo.Version >= MinSupportedNetCoreAppVersion) ||
                              (frameworkInfo.Name == FrameworkInfo.Netfx && frameworkInfo.Version >= MinSupportedNetFxVersion);
            }

            return isSupported;
        }

        public static bool IsEndofLifeFramework(string fullFrameworkName)
        {
            if (FrameworkInfo.TryParse(fullFrameworkName, out FrameworkInfo frameworkInfo))
            {
                // Return true if .NETCoreApp or .NET version is within the end-of-life range
                return frameworkInfo.Name.Equals(FrameworkInfo.Netcoreapp, StringComparison.OrdinalIgnoreCase) ||
                       (frameworkInfo.Name.Equals(FrameworkInfo.Netfx, StringComparison.OrdinalIgnoreCase) &&
                        frameworkInfo.Version >= s_minEOFNetVersion &&
                        frameworkInfo.Version <= s_maxEOFNetVersion);
            }

            // Return false if parsing fails or no conditions matched
            return false;
        }

        public static bool ContainsFullFrameworkTarget(IEnumerable<string> targetFrameworks)
        {
            foreach (string targetFramework in targetFrameworks)
            {
                if (TargetFrameworkHelper.IsSupportedFramework(targetFramework, out var frameworkInfo) && !frameworkInfo.IsDnx)
                {
                    return true;
                }
            }

            return false;
        }
    }
}

