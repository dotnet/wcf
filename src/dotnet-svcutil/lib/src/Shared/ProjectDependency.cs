// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using NuGet.Versioning;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal enum ProjectDependencyType
    {
        Unknown,
        Project,
        Binary,
        Package,
        Tool
    }

    internal class ProjectDependency : IComparable<ProjectDependency>
    {
        private const string DefaultVersion = "*";
        public const string NetCoreAppPackageID = "Microsoft.NETCore.App";
        public const string NetStandardLibraryPackageID = "NETStandard.Library";

        private static readonly IReadOnlyList<string> s_binaryExtensions = new List<string> { ".exe", ".dll" }.AsReadOnly();
        private static readonly IReadOnlyList<string> s_projectExtensions = new List<string> { ".csproj", ".vbproj", ".fsproj", ".vcxproj" }.AsReadOnly();
        private static readonly IReadOnlyList<string> s_exeExtensions = new List<string> { ".exe" }.AsReadOnly();

        /// <summary>
        /// The dependency name.  This can be a package or assembly name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Package Version.
        /// </summary>
        public string Version { get; private set; }

        /// <summary>
        /// Assembly full path.
        /// </summary>
        public string FullPath { get; private set; }

        /// <summary>
        /// Assembly name.
        /// </summary>
        public string AssemblyName { get; private set; }

        /// <summary>
        /// Dependency type: package, project or binary.
        /// </summary>
        public ProjectDependencyType DependencyType { get; private set; }

        /// <summary>
        /// If true, this represents a framework dependency.
        /// </summary>
        public bool IsFramework { get; private set; }

        /// <summary>
        /// The package identity, used for describing the dependency passed to Svcutil.
        /// </summary>
        public string ReferenceIdentity { get; private set; }

        #region instance contruction methods
        // this ctr is private to ensure the integrity of the data which can be done only by the provided static instance creation (FromXXX) methods.
        private ProjectDependency(string filePath = "", string packageName = "", string packageVersion = "", ProjectDependencyType dependencyType = ProjectDependencyType.Unknown)
        {
            bool fileHasKnownExtension = false;

            // basic param check.
            if (!string.IsNullOrWhiteSpace(filePath) && Path.GetInvalidPathChars().Intersect(filePath).Count() > 0)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Shared.Resources.ErrorInvalidDependencyValue, filePath, nameof(filePath)));
            }

            if (!string.IsNullOrWhiteSpace(packageName) && Path.GetInvalidFileNameChars().Intersect(packageName).Count() > 0)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Shared.Resources.ErrorInvalidDependencyValue, packageName, nameof(packageName)));
            }

            if (string.IsNullOrWhiteSpace(packageVersion))
            {
                packageVersion = DefaultVersion;
            }
            else if (!IsValidVersion(packageVersion))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Shared.Resources.ErrorInvalidDependencyValue, packageVersion, nameof(packageVersion)));
            }

            if (dependencyType == ProjectDependencyType.Unknown)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Shared.Resources.ErrorInvalidDependencyValue, dependencyType, nameof(dependencyType)));
            }

            // dependency type check.
            if (dependencyType == ProjectDependencyType.Package || dependencyType == ProjectDependencyType.Tool)
            {
                if (string.IsNullOrWhiteSpace(packageName))
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Shared.Resources.ErrorInvalidDependencyValue, packageName, nameof(packageName)));
                }

                // filePath cannot be a project.
                if (s_projectExtensions.Any((ext) => String.Compare(Path.GetExtension(AssemblyName), ext, StringComparison.OrdinalIgnoreCase) == 0))
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Shared.Resources.ErrorInvalidDependencyValue, packageName, nameof(packageName)));
                }

                // even when it is a Pacakge type, it may contain a valid binary, let's check.
                fileHasKnownExtension = string.IsNullOrWhiteSpace(filePath) ? false :
                     (s_binaryExtensions.Any((ext) => String.Compare(Path.GetExtension(filePath), ext, StringComparison.OrdinalIgnoreCase) == 0));

                // check that package name is not confused with executable name.
                if (s_exeExtensions.Any((ext) => String.Compare(Path.GetExtension(packageName), ext, StringComparison.OrdinalIgnoreCase) == 0))
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Shared.Resources.ErrorInvalidDependencyValue, packageName, nameof(packageName)));
                }
            }
            else // Binary or Project.
            {
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Shared.Resources.ErrorInvalidDependencyValue, filePath, nameof(filePath)));
                }

                if (dependencyType == ProjectDependencyType.Binary)
                {
                    // Can be a full path to an assembly or just the assembly name (with or w/o extension).
                    fileHasKnownExtension = s_binaryExtensions.Any((ext) => String.Compare(Path.GetExtension(filePath), ext, StringComparison.OrdinalIgnoreCase) == 0);
                }
                else if (dependencyType == ProjectDependencyType.Project)
                {
                    fileHasKnownExtension = s_projectExtensions.Any((ext) => String.Compare(Path.GetExtension(filePath), ext, StringComparison.OrdinalIgnoreCase) == 0);
                    if (!fileHasKnownExtension)
                    {
                        throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Shared.Resources.ErrorInvalidDependencyValue, filePath, nameof(filePath)));
                    }
                }
            }

            this.FullPath = fileHasKnownExtension ? Path.GetFullPath(filePath) : filePath;
            this.DependencyType = dependencyType;
            this.Version = packageVersion;

            this.AssemblyName = string.IsNullOrWhiteSpace(filePath) ? packageName :
                fileHasKnownExtension ? Path.GetFileNameWithoutExtension(filePath) : Path.GetFileName(filePath);

            this.Name = string.IsNullOrWhiteSpace(packageName) ? this.AssemblyName : packageName;

            this.ReferenceIdentity = this.DependencyType == ProjectDependencyType.Package || this.DependencyType == ProjectDependencyType.Tool || !string.IsNullOrWhiteSpace(packageName) ?
                string.Format(CultureInfo.InvariantCulture, "{0}, {{{1}, {2}}}", this.AssemblyName, this.Name, this.Version) :
                this.FullPath;

            this.IsFramework = this.Name == NetCoreAppPackageID || this.Name == NetStandardLibraryPackageID;
        }

        public static ProjectDependency Parse(string dependencySpec)
        {
            // Parse text of the form below where assemblyName can be a fileName, fileName.Extension of fileFullPath.
            // spec form:
            //   [assemblyName, ]{packageName[, version]]}
            //   assemblyName[, {packageName[, version]]}]
            // examples:
            //   Microsoft.CSharp, {NetCoreApp, 2.0.0} 
            //   Microsoft.CSharp, {NetCoreApp} 
            //   Microsoft.CSharp
            //   {NetCoreApp, 2.0.0} 
            //   NetCoreApp, 2.0.0 
            //   Microsoft.CSharp.dll
            //   C:\Fx\Microsoft.CSharp.dll, {NetCoreApp, 2.0.0} 

            ProjectDependency dependency = null;

            if (string.IsNullOrWhiteSpace(dependencySpec))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Shared.Resources.ErrorInvalidDependencyValue, dependencySpec, nameof(dependencySpec)));
            }

            string processedSpec = dependencySpec.Trim('\"').Trim();
            string assemblyName = string.Empty;
            bool hasParenthesis = false;

            int openingIdx = processedSpec.IndexOf('{');
            if (openingIdx != -1)
            {
                int closingIdx = processedSpec.IndexOf('}');
                if (closingIdx != processedSpec.Length - 1 || processedSpec.IndexOf('{', openingIdx + 1) != -1 || processedSpec.IndexOf('}', closingIdx + 1) != -1)
                {
                    // error:parenthesis in wrong place, mismatching or extras.
                    processedSpec = null;
                }
                else
                {
                    // parse full spec: assemblyName, {packageName, version}
                    assemblyName = processedSpec.Substring(0, openingIdx).Trim();
                    processedSpec = processedSpec.Substring(openingIdx + 1, closingIdx - openingIdx - 1).Trim();

                    if (!string.IsNullOrWhiteSpace(assemblyName))
                    {
                        if (assemblyName[assemblyName.Length - 1] == ',')
                        {
                            assemblyName = assemblyName.Substring(0, assemblyName.Length - 1).Trim();
                        }
                        else
                        {
                            processedSpec = null;
                        }
                    }
                    hasParenthesis = true;
                }
            }

            if (processedSpec != null)
            {
                // parse package name or file spec: '{packageName, version}' or 'c:\path\file.ext'
                var values = processedSpec.Split(new char[] { ',' });

                if (values.Length == 1 || values.Length == 2)
                {
                    string name = values[0].Trim();
                    string version = values.Length > 1 ? values[1].Trim() : string.Empty;
                    ProjectDependencyType dependencyType;

                    if (string.IsNullOrWhiteSpace(assemblyName))
                    {
                        // name may refer to a file path or a package name, let's check.
                        dependencyType = GetDependencyType(name);
                        if (dependencyType != ProjectDependencyType.Package)
                        {
                            // we are dealing with a file dependency (binary or project)
                            if (hasParenthesis)
                            {
                                // error, file treated as package: '{c:\temp\lib.dll, }'
                                dependencyType = ProjectDependencyType.Unknown;
                            }
                            else
                            {
                                assemblyName = name;
                                name = string.Empty;
                            }
                        }
                    }
                    else
                    {
                        // we have an assembly spec but it may be a full spec 'assembly, {name, version}', let's check.
                        if (string.IsNullOrWhiteSpace(name))
                        {
                            dependencyType = GetDependencyType(assemblyName);
                            if (dependencyType == ProjectDependencyType.Package)
                            {
                                name = assemblyName;
                            }
                        }
                        else
                        {
                            dependencyType = ProjectDependencyType.Package;
                        }
                    }

                    if (dependencyType != ProjectDependencyType.Unknown)
                    {
                        dependency = new ProjectDependency(assemblyName, name, version, dependencyType);
                    }
                }
            }

            if (dependency == null)
            {
                throw new FormatException(string.Format(CultureInfo.CurrentCulture, Shared.Resources.ErrorDepenencySpecFormat, dependencySpec));
            }

            return dependency;
        }

        public static ProjectDependency FromAssembly(string assemblyPath)
        {
            return new ProjectDependency(assemblyPath, null, null, ProjectDependencyType.Binary);
        }

        public static ProjectDependency FromProject(string projectPath)
        {
            return new ProjectDependency(projectPath, null, null, ProjectDependencyType.Project);
        }

        public static ProjectDependency FromPackage(string packageName, string version)
        {
            return ProjectDependency.FromPackage(null, packageName, version);
        }

        public static ProjectDependency FromPackage(string assemblyName, string packageName, string version)
        {
            return new ProjectDependency(assemblyName, packageName, version, ProjectDependencyType.Package);
        }

        public static ProjectDependency FromCliTool(string packageName, string version)
        {
            return new ProjectDependency(null, packageName, version, ProjectDependencyType.Tool);
        }
        #endregion

        #region helper methods
        private static ProjectDependencyType GetDependencyType(string path)
        {
            ProjectDependencyType dependencyType;

            if (s_projectExtensions.Any((ext) => String.Compare(Path.GetExtension(path), ext, StringComparison.OrdinalIgnoreCase) == 0))
            {
                // project dependency: <projectpath>,<version> : C:\Source\MyProject\project.csproj
                dependencyType = ProjectDependencyType.Project;
            }
            else if (s_binaryExtensions.Any((ext) => String.Compare(Path.GetExtension(path), ext, StringComparison.OrdinalIgnoreCase) == 0))
            {
                // assembly dependency: <assemblypath> : C:\assemblies\assemblyX.dll
                dependencyType = ProjectDependencyType.Binary;
            }
            else
            {
                // package dependency: <packagename>,<version> : EntityFramework.Core,7.0.0.0
                // Note: It might be that it is a path that does not exist; it is ok to treat it as a package.
                dependencyType = ProjectDependencyType.Package;
            }

            return dependencyType;
        }

        internal static bool IsValidVersion(string version)
        {
            if (version == "*-*")
            {
                return true;
            }
            // https://semver.org/spec/v2.0.0.html

            bool? isValid = null;

            if (!string.IsNullOrWhiteSpace(version))
            {
                version = version.Trim();

                int starIdx = version.IndexOf('*');
                if (starIdx != -1)
                {
                    // check if there's another star
                    if (version.IndexOf('*', starIdx + 1) != -1)
                    {
                        isValid = false;
                    }
                    else if (version.Length == 1)
                    {
                        isValid = true;
                    }
                    else
                    {
                        version = version.Substring(0, starIdx - 1);
                    }
                }

                if (!isValid.HasValue)
                {
                    isValid = NuGetVersion.TryParse(version, out NuGetVersion nugetVersion);
                }
            }

            return isValid == true;
        }
        #endregion

        #region overrides
        public bool Equals(ProjectDependency other)
        {
            return this.CompareTo(other) == 0;
        }

        public override bool Equals(object obj)
        {
            return this.CompareTo(obj as ProjectDependency) == 0;
        }

        public override int GetHashCode()
        {
            return this.Name.ToUpperInvariant().GetHashCode();
        }

        public override string ToString()
        {
            return this.ReferenceIdentity;
        }
        #endregion

        #region IComparable
        public int CompareTo(ProjectDependency other)
        {
            if (other == null) return 1;
            if (other.DependencyType == ProjectDependencyType.Binary)
            {
                return this.DependencyType == other.DependencyType ?
                StringComparer.CurrentCultureIgnoreCase.Compare(this.FullPath, other.FullPath) :
                this.DependencyType > other.DependencyType ? 1 : -1;
            }

            return this.DependencyType == other.DependencyType ?
            StringComparer.CurrentCultureIgnoreCase.Compare(this.AssemblyName, other.AssemblyName) :
            this.DependencyType > other.DependencyType ? 1 : -1;
        }
        #endregion

        public ProjectDependency Clone()
        {
            return new ProjectDependency(this.FullPath, this.Name, this.Version, this.DependencyType);
        }

        internal static List<ProjectDependency> IgnorableDependencies = new List<ProjectDependency>
        {
            // Packages that fail to restore.
            ProjectDependency.FromPackage("System.Runtime.InteropServices.PInvoke","*"),

            // Private assets in VS templates.
            ProjectDependency.FromPackage("Microsoft.EntityFrameworkCore.Design", "*"),
            ProjectDependency.FromPackage("Microsoft.EntityFrameworkCore.SqlServer.Design","*"),
            ProjectDependency.FromPackage("Microsoft.EntityFrameworkCore.Tools", "*"),
            ProjectDependency.FromPackage("Microsoft.VisualStudio.Web.CodeGeneration.Design","*"),
            
            // Package resolved by Microsoft.VisualStudio.Web.CodeGeneration.Design.
            ProjectDependency.FromPackage("dotnet-aspnet-codegenerator-design","*"),
        };

        internal static void RemoveRedundantReferences(IList<ProjectDependency> dependencies)
        {
            // Target framework and ServiceModel assemblies are redundant because the first are available at runtime and
            // the second are not used as types come from the private code.
            for (int idx = dependencies.Count - 1; idx >= 0; idx--)
            {
                var dependency = dependencies[idx];
                if (dependency.IsFramework || TargetFrameworkHelper.ServiceModelPackages.Any(s => s.Name == dependency.Name))
                {
                    dependencies.RemoveAt(idx);
                }
            }
        }
    }
}
