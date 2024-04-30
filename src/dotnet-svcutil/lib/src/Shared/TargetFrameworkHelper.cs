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
             {new Version("2.0"), new Version("2.0") }
         });

        internal static SortedDictionary<Version, List<ProjectDependency>> NetCoreVersionReferenceTable = new SortedDictionary<Version, List<ProjectDependency>>
        {
            {new Version("1.0"), new List<ProjectDependency> {
                ProjectDependency.FromPackage("System.ServiceModel.Duplex", "4.0.*"  ),
                ProjectDependency.FromPackage("System.ServiceModel.Http", "4.1.*"    ),
                ProjectDependency.FromPackage("System.ServiceModel.NetTcp", "4.1.*"  ),
                ProjectDependency.FromPackage("System.ServiceModel.Security", "4.0.*"),
                ProjectDependency.FromPackage("System.Xml.XmlSerializer", "4.0.*"    ),
            } },
            {new Version("1.1"), new List<ProjectDependency> {
                ProjectDependency.FromPackage("System.ServiceModel.Duplex", "4.3.*"  ),
                ProjectDependency.FromPackage("System.ServiceModel.Http", "4.3.*"    ),
                ProjectDependency.FromPackage("System.ServiceModel.NetTcp", "4.3.*"  ),
                ProjectDependency.FromPackage("System.ServiceModel.Security", "4.3.*"),
                ProjectDependency.FromPackage("System.Xml.XmlSerializer", "4.3.*"    ),
            } },
            {new Version("2.0"), new List<ProjectDependency> {
                ProjectDependency.FromPackage("System.ServiceModel.Duplex", "4.4.*"  ),
                ProjectDependency.FromPackage("System.ServiceModel.Http", "4.4.*"    ),
                ProjectDependency.FromPackage("System.ServiceModel.NetTcp", "4.4.*"  ),
                ProjectDependency.FromPackage("System.ServiceModel.Security", "4.4.*"),
            } },
            {new Version("2.1"), new List<ProjectDependency> {
                ProjectDependency.FromPackage("System.ServiceModel.Duplex", "4.6.*"  ),
                ProjectDependency.FromPackage("System.ServiceModel.Http", "4.6.*"    ),
                ProjectDependency.FromPackage("System.ServiceModel.NetTcp", "4.6.*"  ),
                ProjectDependency.FromPackage("System.ServiceModel.Security", "4.6.*"),
            } },
            {new Version("3.1"), new List<ProjectDependency> {
                ProjectDependency.FromPackage("System.ServiceModel.Duplex", "4.7.*"  ),
                ProjectDependency.FromPackage("System.ServiceModel.Http", "4.7.*"    ),
                ProjectDependency.FromPackage("System.ServiceModel.NetTcp", "4.7.*"  ),
                ProjectDependency.FromPackage("System.ServiceModel.Security", "4.7.*"),
             } },
            {new Version("5.0"), new List<ProjectDependency> {
                ProjectDependency.FromPackage("System.ServiceModel.Duplex", "4.8.*"  ),
                ProjectDependency.FromPackage("System.ServiceModel.Http", "4.8.*"    ),
                ProjectDependency.FromPackage("System.ServiceModel.NetTcp", "4.8.*"  ),
                ProjectDependency.FromPackage("System.ServiceModel.Security", "4.8.*"),
                ProjectDependency.FromPackage("System.ServiceModel.Federation", "4.8.*")
            } },
            {new Version("6.0"), new List<ProjectDependency> {
                ProjectDependency.FromPackage("System.ServiceModel.Duplex", "6.0.*"  ),
                ProjectDependency.FromPackage("System.ServiceModel.Http", "6.0.*"    ),
                ProjectDependency.FromPackage("System.ServiceModel.NetTcp", "6.0.*"  ),
                ProjectDependency.FromPackage("System.ServiceModel.Security", "6.0.*"),
                ProjectDependency.FromPackage("System.ServiceModel.Federation", "6.0.*"),
                ProjectDependency.FromPackage("System.ServiceModel.NetNamedPipe", "6.0.*")
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

        public static IEnumerable<ProjectDependency> GetWcfProjectReferences(string targetFramework)
        {
            IEnumerable<ProjectDependency> dependencies = null;

            if (IsSupportedFramework(targetFramework, out var frameworkInfo))
            {
                if (frameworkInfo.IsDnx)
                {
                    if (NetCoreVersionReferenceTable.ContainsKey(frameworkInfo.Version))
                    {
                        dependencies = NetCoreVersionReferenceTable[frameworkInfo.Version];
                    }
                    else
                    {
                        dependencies = NetCoreVersionReferenceTable.Last().Value;
                    }
                }
                else
                {
                    dependencies = FullFrameworkReferences;
                }
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

            if (fxInfo != null)
            {
                return fxInfo.FullName;
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
                        netCoreVersion = frameworkInfo.Name == FrameworkInfo.Netstandard ?
                           TargetFrameworkHelper.NetStandardToNetCoreVersionMap[frameworkInfo.Version] :
                           frameworkInfo.Version;
                    }
                    else
                    {
                        // target framework is not the minumum standard supported netcore version but it is a known shipped netcore version.
                        if (TargetFrameworkHelper.NetCoreVersionReferenceTable.ContainsKey(frameworkInfo.Version))
                        {
                            netCoreVersion = frameworkInfo.Version;
                        }
                        else
                        {
                            // target framework is not known to the tool, use the latest known netcore version.
                            netCoreVersion = TargetFrameworkHelper.NetCoreVersionReferenceTable.Keys.LastOrDefault();
                        }
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

            var tfx = fullFrameworkName.Split('-');
            if (tfx.Length > 1)
            {
                fullFrameworkName = tfx[0];
            }

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

