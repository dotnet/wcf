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
        public static ReadOnlyDictionary<Version, Version> NetStandardToNetCoreVersionMap { get; } = new ReadOnlyDictionary<Version, Version>(new SortedDictionary<Version, Version>
         {
            // Service Model requires netstandard1.3 so it is the minimum version that will work.
             {new Version("1.3"), new Version("1.0") },
             {new Version("1.4"), new Version("1.0") },
             {new Version("1.5"), new Version("1.0") },
             {new Version("1.6"), new Version("1.0") },
             {new Version("1.6.1"), new Version("1.1") },
             {new Version("2.0"), new Version("2.0") },
             {new Version("2.1"), new Version("3.1") }
         });

        public static ReadOnlyDictionary<Version, Version> NetCoreToWCFPackageReferenceVersionMap { get; } = new ReadOnlyDictionary<Version, Version>(new SortedDictionary<Version, Version>
         {
             {new Version("1.0"), new Version("2.0") },
             {new Version("1.1"), new Version("2.0") },
             {new Version("2.0"), new Version("2.0") },
             {new Version("2.1"), new Version("2.0") },
             {new Version("2.2"), new Version("2.0") },
             {new Version("3.0"), new Version("2.0") },
             {new Version("3.1"), new Version("2.0") },
             {new Version("5.0"), new Version("2.0") },
             {new Version("6.0"), new Version("6.0") },
             {new Version("7.0"), new Version("6.0") },
             {new Version("8.0"), new Version("8.0") }
         });

        internal static SortedDictionary<Version, List<ProjectDependency>> NetCoreVersionReferenceTable = new SortedDictionary<Version, List<ProjectDependency>>
        {
            {new Version("2.0"), new List<ProjectDependency> {
                ProjectDependency.FromPackage("System.ServiceModel.Duplex", "4.10.*"  ),
                ProjectDependency.FromPackage("System.ServiceModel.Http", "4.10.*"    ),
                ProjectDependency.FromPackage("System.ServiceModel.NetTcp", "4.10.*"  ),
                ProjectDependency.FromPackage("System.ServiceModel.Security", "4.10.*"),
                ProjectDependency.FromPackage("System.ServiceModel.Federation", "4.10.*")
            } },
            {new Version("6.0"), new List<ProjectDependency> {
                ProjectDependency.FromPackage("System.ServiceModel.Http", "6.*"),
                ProjectDependency.FromPackage("System.ServiceModel.NetTcp", "6.*"),
                ProjectDependency.FromPackage("System.ServiceModel.Primitives", "6.*")
            } },
            {new Version("8.0"), new List<ProjectDependency> {
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

        public static Version MinSupportedNetFxVersion { get; } = new Version("4.5");
        public static Version MinSupportedNetStandardVersion { get; } = NetStandardToNetCoreVersionMap.Keys.First();
        public static Version MinSupportedNetCoreAppVersion { get; } = NetStandardToNetCoreVersionMap.Values.First();

        public static IEnumerable<ProjectDependency> GetWcfProjectReferences(IEnumerable<string> targetFrameworks)
        {
            IEnumerable<ProjectDependency> dependencies = null;
            Version version = GetLowestNetCoreVersion(targetFrameworks);
            if (version != null)
            {
                dependencies = NetCoreVersionReferenceTable[NetCoreToWCFPackageReferenceVersionMap[version]];
            }
            else
            {
                dependencies = FullFrameworkReferences;
            }

            return dependencies;
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
                targetFramework = targetFrameworks.FirstOrDefault((framework) => IsSupportedFramework(framework, out fxInfo) && fxInfo.IsKnownDnx);
                if (targetFramework == null)
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
            }

            return targetFramework;
        }

        public static Version GetLowestNetCoreVersion(IEnumerable<string> targetFrameworks)
        {
            Version targetVersion = null;

            foreach (string targetFramework in targetFrameworks)
            {
                if (TargetFrameworkHelper.IsSupportedFramework(targetFramework, out var frameworkInfo) && frameworkInfo.IsDnx)
                {
                    Version netCoreVersion;
                    if (frameworkInfo.IsKnownDnx)
                    {
                        if (frameworkInfo.Name == FrameworkInfo.Netstandard)
                        {
                            if (!NetStandardToNetCoreVersionMap.TryGetValue(frameworkInfo.Version, out netCoreVersion))
                            {
                                netCoreVersion = NetStandardToNetCoreVersionMap.LastOrDefault().Value;
                            }
                        }
                        else
                        {
                            if (NetCoreToWCFPackageReferenceVersionMap.TryGetValue(frameworkInfo.Version, out Version version))
                            {
                                netCoreVersion = frameworkInfo.Version;
                            }
                            else
                            {
                                netCoreVersion = NetCoreToWCFPackageReferenceVersionMap.Keys.LastOrDefault();
                            }
                        }
                    }
                    else
                    {
                        // target framework is not known to the tool, use the latest known netcore version.
                        netCoreVersion = NetCoreToWCFPackageReferenceVersionMap.Keys.LastOrDefault();
                    }

                    if (targetVersion == null || targetVersion > netCoreVersion)
                    {
                        targetVersion = netCoreVersion;
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

