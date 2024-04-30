// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using NuGet.ProjectModel;
using Microsoft.Extensions.DependencyModel;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class MSBuildProj : IDisposable
    {
        private bool _isSaved;
        private bool _ownsDirectory;
        private readonly ProjectPropertyResolver _propertyResolver;
        private XNamespace _msbuildNS;

        private MSBuildProj()
        {
            _propertyResolver = new ProjectPropertyResolver();
        }

        #region Properties

        // Values in this collection have the effect of properties that are passed to MSBuild in the command line which become global properties.
        public Dictionary<string, string> GlobalProperties { get; } = new Dictionary<string, string>();

        // Netcore projects can specify TargetFramework and/or TargetFrameworks, TargetFramework is always used.
        // When only TargetFrameworks is specified, the build system builds the project specifying TargetFramework for each entry.
        private string _targetFramework;
        public string TargetFramework
        {
            get { return _targetFramework; }
            set { UpdateTargetFramework(value); }
        }

        private List<string> _targetFrameworks = new List<string>();
        public IEnumerable<string> TargetFrameworks { get { return _targetFrameworks; } }

        private string _runtimeIdentifier;
        public string RuntimeIdentifier
        {
            get { return _runtimeIdentifier; }
            set { SetRuntimeIdentifier(value); }
        }

        private SortedSet<ProjectDependency> _dependencies = new SortedSet<ProjectDependency>();
        public IEnumerable<ProjectDependency> Dependencies { get { return _dependencies; } }

        public SortedDictionary<string, string> _resolvedProperties = new SortedDictionary<string, string>();
        public IEnumerable<KeyValuePair<string, string>> ResolvedProperties { get { return this._resolvedProperties; } }

        public string FileName { get; private set; }

        public string DirectoryPath { get; private set; }

        public string FullPath { get { return Path.Combine(DirectoryPath, FileName); } }

        public string SdkVersion { get; private set; }

        private XElement ProjectNode { get; set; }

        private XElement _projectReferenceGroup;
        private XElement ProjectReferceGroup
        {
            get
            {
                if (_projectReferenceGroup == null)
                {
                    IEnumerable<XElement> refItems = this.ProjectNode.Elements("ProjectReference");
                    if (refItems == null || refItems.Count() == 0)
                    {
                        // add ref subgroup
                        _projectReferenceGroup = new XElement("ItemGroup");
                        this.ProjectNode.Add(_projectReferenceGroup);
                    }
                    else
                    {
                        _projectReferenceGroup = refItems.FirstOrDefault().Parent;
                    }
                }
                return _projectReferenceGroup;
            }
        }

        private XElement _referenceGroup;
        private XElement ReferenceGroup
        {
            get
            {
                if (_referenceGroup == null)
                {
                    IEnumerable<XElement> refItems = this.ProjectNode.Elements("Reference");
                    if (refItems == null || refItems.Count() == 0)
                    {
                        // add ref subgroup
                        _referenceGroup = new XElement("ItemGroup");
                        this.ProjectNode.Add(_referenceGroup);
                    }
                    else
                    {
                        _referenceGroup = refItems.FirstOrDefault().Parent;
                    }
                }
                return _referenceGroup;
            }
        }
        #endregion

        private XElement _packageReferenceGroup;
        private XElement PacakgeReferenceGroup
        {
            get
            {
                if (_packageReferenceGroup == null)
                {
                    IEnumerable<XElement> refItems = this.ProjectNode.Elements("PackageReference");
                    if (refItems == null || refItems.Count() == 0)
                    {
                        // add ref subgroup
                        _packageReferenceGroup = new XElement("ItemGroup");
                        this.ProjectNode.Add(_packageReferenceGroup);
                    }
                    else
                    {
                        _packageReferenceGroup = refItems.FirstOrDefault().Parent;
                    }

                    FrameworkInfo netfxInfo = null;
                    FrameworkInfo dnxInfo = null;
                    if (this.TargetFrameworks.Count() > 1 && this.TargetFrameworks.Any(t => TargetFrameworkHelper.IsSupportedFramework(t, out netfxInfo) && !netfxInfo.IsDnx))
                    {
                        var tfx = this.TargetFrameworks.FirstOrDefault(t => TargetFrameworkHelper.IsSupportedFramework(t, out dnxInfo) && dnxInfo.IsDnx);
                        if (!string.IsNullOrEmpty(tfx) && dnxInfo.Version.Major >= 6)
                        {
                            _packageReferenceGroup.Add(new XAttribute("Condition", $"'$(TargetFramework)' != '{netfxInfo.FullName}'"));
                        }
                    }
                }

                return _packageReferenceGroup;
            }
        }

        #region Parsing/Settings Methods
        public static async Task<MSBuildProj> FromPathAsync(string filePath, ILogger logger, CancellationToken cancellationToken)
        {
            var project = await ParseAsync(File.ReadAllText(filePath), filePath, logger, cancellationToken).ConfigureAwait(false);
            project._isSaved = true;
            return project;
        }

        internal static async Task<MSBuildProj> FromPathAsync(string filePath, ILogger logger, string tfMoniker, CancellationToken cancellationToken)
        {
            var project = await ParseAsync(File.ReadAllText(filePath), filePath, logger, cancellationToken, tfMoniker).ConfigureAwait(false);
            project._isSaved = true;
            return project;
        }

        public static async Task<MSBuildProj> ParseAsync(string projectText, string projectFullPath, ILogger logger, CancellationToken cancellationToken, string tfMoniker = "")
        {
            using (var safeLogger = await SafeLogger.WriteStartOperationAsync(logger, $"Parsing project {Path.GetFileName(projectFullPath)}").ConfigureAwait(false))
            {
                projectFullPath = Path.GetFullPath(projectFullPath);

                MSBuildProj msbuildProj = new MSBuildProj
                {
                    FileName = Path.GetFileName(projectFullPath),
                    DirectoryPath = Path.GetDirectoryName(projectFullPath)
                };

                XDocument projDefinition = XDocument.Parse(projectText);

                var msbuildNS = XNamespace.None;
                if (projDefinition.Root != null && projDefinition.Root.Name != null)
                {
                    msbuildNS = projDefinition.Root.Name.Namespace;
                }

                msbuildProj._msbuildNS = msbuildNS;
                msbuildProj.ProjectNode = projDefinition.Element(msbuildNS + "Project");
                if (msbuildProj.ProjectNode == null)
                {
                    throw new Exception(Shared.Resources.ErrorInvalidProjectFormat);
                }

                // The user project can declare TargetFramework and/or TargetFrameworks property. If both are provided, TargetFramework wins.
                // When TargetFrameworks is provided, the project is built for every entry specified in the TargetFramework property.

                IEnumerable<XElement> targetFrameworkElements = GetSubGroupValues(msbuildProj.ProjectNode, msbuildNS, "PropertyGroup", "TargetFramework");
                if (targetFrameworkElements.Count() > 0)
                {
                    // If property is specified more than once, MSBuild will resolve it by overwriting it with the last value.
                    var targetFramework = targetFrameworkElements.Last().Value.Trim().ToLowerInvariant();
                    if (!string.IsNullOrWhiteSpace(targetFramework))
                    {
                        if(TargetFrameworkHelper.IsSupportedFramework(targetFramework, out FrameworkInfo fxInfo))
                        {
                            msbuildProj._targetFrameworks.Add(targetFramework);
                        }
                    }
                }

                if (msbuildProj._targetFrameworks.Count == 0)
                {
                    // TargetFramework was not provided, check TargetFrameworks property.
                    IEnumerable<XElement> targetFrameworksElements = GetSubGroupValues(msbuildProj.ProjectNode, msbuildNS, "PropertyGroup", "TargetFrameworks");
                    if (targetFrameworksElements.Count() > 0)
                    {
                        var targetFrameworks = targetFrameworksElements.Last().Value;
                        foreach (var targetFx in targetFrameworks.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()))
                        {
                            if (!string.IsNullOrWhiteSpace(targetFx))
                            {
                                msbuildProj._targetFrameworks.Add(targetFx);
                            }
                        }
                    }
                }

                msbuildProj._targetFrameworks.Sort();
                msbuildProj._targetFramework = TargetFrameworkHelper.GetBestFitTargetFramework(msbuildProj._targetFrameworks);

                if(string.IsNullOrEmpty(msbuildProj._targetFramework))
                {
                    if(!string.IsNullOrEmpty(tfMoniker) && FrameworkInfo.TryParse(tfMoniker, out FrameworkInfo fxInfo))
                    {
                        msbuildProj._targetFramework = fxInfo.FullName;
                    }
                    else
                    {
                        msbuildProj._targetFramework = string.Concat("net", TargetFrameworkHelper.NetCoreVersionReferenceTable.LastOrDefault().Key.ToString());
                    }
                    
                    msbuildProj._targetFrameworks.Add(msbuildProj._targetFramework);
                }

                // Ensure target framework is valid.
                FrameworkInfo frameworkInfo = TargetFrameworkHelper.GetValidFrameworkInfo(msbuildProj.TargetFramework);

                IEnumerable<XElement> runtimeIdentifierElements = GetSubGroupValues(msbuildProj.ProjectNode, msbuildNS, "PropertyGroup", "RuntimeIdentifier");
                if (runtimeIdentifierElements.Count() > 0)
                {
                    msbuildProj.RuntimeIdentifier = runtimeIdentifierElements.Last().Value.Trim();
                }

                IEnumerable<XElement> packageReferenceElements = GetSubGroupValues(msbuildProj.ProjectNode, msbuildNS, "ItemGroup", "PackageReference");
                foreach (XElement reference in packageReferenceElements)
                {
                    if(!TryGetItemIdentity(reference, out var packageName))
                        continue;

                    string version = GetItemValue(reference, "Version");
                    if (!ProjectDependency.IsValidVersion(version))
                    {
                        version = "";
                    }

                    ProjectDependency packageDep = ProjectDependency.FromPackage(packageName, version);

                    msbuildProj._dependencies.Add(packageDep);
                }

                IEnumerable<XElement> toolReferenceElements = GetSubGroupValues(msbuildProj.ProjectNode, msbuildNS, "ItemGroup", "DotNetCliToolReference");
                foreach (XElement reference in toolReferenceElements)
                {
                    if (!TryGetItemIdentity(reference, out var packageName))
                        continue;

                    string version = GetItemValue(reference, "Version");

                    ProjectDependency packageDep = ProjectDependency.FromCliTool(packageName, version);

                    msbuildProj._dependencies.Add(packageDep);
                }

                IEnumerable<XElement> projectReferenceElements = GetSubGroupValues(msbuildProj.ProjectNode, msbuildNS, "ItemGroup", "ProjectReference");
                foreach (XElement reference in projectReferenceElements)
                {
                    string projectPath = GetItemValue(reference, "Include", throwIfMissing: true);

                    ProjectDependency projectDep = ProjectDependency.FromProject(projectPath);

                    msbuildProj._dependencies.Add(projectDep);
                }

                IEnumerable<XElement> binReferenceElements = GetSubGroupValues(msbuildProj.ProjectNode, msbuildNS, "ItemGroup", "Reference");
                foreach (XElement reference in binReferenceElements)
                {
                    //Find hint path or path
                    string binReference = GetItemIdentity(reference);

                    if (!Path.IsPathRooted(binReference))
                    {
                        string fullPath = null;
                        bool fullPathFound = true;

                        XElement hintPath = reference.Element("HintPath");
                        XElement path = reference.Element("Path");
                        if (path != null)
                        {
                            fullPath = path.Value;
                        }
                        else if (hintPath != null)
                        {
                            fullPath = hintPath.Value;
                        }
                        else
                        {
                            try
                            {
                                fullPath = new FileInfo(Path.Combine(msbuildProj.DirectoryPath, binReference)).FullName;
                            }
                            catch
                            {
                            }

                            if (fullPath == null || !File.Exists(fullPath))
                            {
                                fullPathFound = false;

                                // If we're only targeting .NET Core or .NET Standard projects we throw if we can't find the full path to the assembly.
                                if (!TargetFrameworkHelper.ContainsFullFrameworkTarget(msbuildProj._targetFrameworks))
                                {
                                    throw new Exception(string.Format(CultureInfo.CurrentCulture, Shared.Resources.ErrorProjectReferenceMissingFilePathFormat, binReference));
                                }
                            }
                        }

                        if (fullPathFound)
                        {
                            if (System.IO.Directory.Exists(fullPath)) // IsDir?
                            {
                                fullPath = Path.Combine(fullPath, binReference);
                            }
                            else if (Directory.Exists(Path.Combine(msbuildProj.DirectoryPath, fullPath)))
                            {
                                fullPath = Path.Combine(msbuildProj.DirectoryPath, fullPath, binReference);
                            }

                            binReference = fullPath;
                        }
                    }

                    ProjectDependency projectDep = ProjectDependency.FromAssembly(binReference);

                    msbuildProj._dependencies.Add(projectDep);
                }

                // ensure we have a working directory for the ProcessRunner (ProjectPropertyResolver)..
                if (!Directory.Exists(msbuildProj.DirectoryPath))
                {
                    Directory.CreateDirectory(msbuildProj.DirectoryPath);
                    msbuildProj._ownsDirectory = true;
                }

                var sdkVersion = await ProjectPropertyResolver.GetSdkVersionAsync(msbuildProj.DirectoryPath, logger, cancellationToken).ConfigureAwait(false);
                msbuildProj.SdkVersion = sdkVersion ?? string.Empty;

                return msbuildProj;
            }
        }

        public static async Task<MSBuildProj> DotNetNewAsync(string fullPath, ILogger logger, CancellationToken cancellationToken, string optional = "")
        {
            cancellationToken.ThrowIfCancellationRequested();

            MSBuildProj project = null;
            bool ownsDir = false;

            if (fullPath == null)
            {
                throw new ArgumentNullException(nameof(fullPath));
            }
            fullPath = Path.GetFullPath(fullPath);

            string projectName = Path.GetFileNameWithoutExtension(fullPath);
            string projectExtension = Path.GetExtension(fullPath);
            string projectDir = Path.GetDirectoryName(fullPath);

            if (string.IsNullOrEmpty(projectName) || string.CompareOrdinal(projectExtension.ToLowerInvariant(), ".csproj") != 0)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Shared.Resources.ErrorFullPathNotAProjectFilePathFormat, fullPath));
            }

            if (File.Exists(fullPath))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Shared.Resources.ErrorProjectFileAlreadyExistsFormat, fullPath));
            }

            // ensure we have a working directory for the ProcessRunner (ProjectPropertyResolver).
            if (!Directory.Exists(projectDir))
            {
                Directory.CreateDirectory(projectDir);
                ownsDir = true;
            }

            var sdkVersion = await ProjectPropertyResolver.GetSdkVersionAsync(projectDir, logger, cancellationToken).ConfigureAwait(false);
            var dotnetNewParams = $"new console {GetNoRestoreParam(sdkVersion)} --force --type project --language C# --output . --name {projectName} {optional}";
            await ProcessRunner.RunAsync("dotnet", dotnetNewParams, projectDir, logger, cancellationToken).ConfigureAwait(false);

            project = await ParseAsync(File.ReadAllText(fullPath), fullPath, logger, cancellationToken).ConfigureAwait(false);
            project._ownsDirectory = ownsDir;

            project.SdkVersion = sdkVersion ?? string.Empty;
            project._isSaved = true;

            return project;
        }

        private static IEnumerable<XElement> GetGroupValues(XElement projectElement, string group, bool createOnMissing = false)
        {
            // XElement.Elements() always returns a collection, no need to check for null.
            IEnumerable<XElement> groups = projectElement.Elements(group);

            if (createOnMissing && groups.Count() == 0)
            {
                // add a property group
                XElement propertyGroup = new XElement(group);
                projectElement.Add(propertyGroup);
                return new XElement[] { propertyGroup };
            }

            return groups;
        }

        //Used for both references and properties
        private static IEnumerable<XElement> GetSubGroupValues(XElement projectElement, XNamespace msbuildNS, string group, string subGroupName)
        {
            IEnumerable<XElement> groups = GetGroupValues(projectElement, group);
            IEnumerable<XElement> subGroupValues = groups.Elements(msbuildNS + subGroupName);
            return subGroupValues;
        }

        private static string GetItemValue(XElement reference, string itemName, bool throwIfMissing = false)
        {
            // XElement.Attributes() alwasy returns a collection, no need to check for null.
            var itemAttribue = reference.Attributes().FirstOrDefault(item => item.Name == itemName);
            if (itemAttribue != null)
            {
                return itemAttribue.Value;
            }

            XElement itemNameElement = null;
            itemNameElement = reference.Elements().FirstOrDefault(item => item.Name == itemName);
            if (itemNameElement != null)
            {
                return itemNameElement.Value;
            }

            if (throwIfMissing)
            {
                throw new Exception(Shared.Resources.ErrorInvalidProjectFormat);
            }
            return null;
        }

        public static bool TryGetItemIdentity(XElement itemName, out string itemIdentity)
        {
            var itemAttribute = itemName.Attributes().FirstOrDefault(item => item.Name == "Include");

            if (itemAttribute != null)
            {
                itemIdentity = itemAttribute.Value;
                return true;
            }

            itemIdentity = default;
            return false;
        }

        private static string GetItemIdentity(XElement itemName)
        {
            var itemAttribute = itemName.Attributes().FirstOrDefault(item => item.Name == "Include");
            if (itemAttribute != null)
            {
                return itemAttribute.Value;
            }

            throw new Exception(Shared.Resources.ErrorInvalidProjectFormat);
        }

        public bool AddDependency(ProjectDependency dependency, bool copyInternalAssets = false)
        {
            // a nuget package can contain multiple assemblies, we need to filter package references so we don't add dups.
            bool addDependency = !_dependencies.Any(d =>
            {
                switch (d.DependencyType)
                {
                    case ProjectDependencyType.Package:
                    case ProjectDependencyType.Tool:
                        return d.Name == dependency.Name;
                    default:
                        if (d.FullPath == null && dependency.FullPath == null)
                        {
                            goto case ProjectDependencyType.Package;
                        }
                        return d.FullPath == dependency.FullPath;
                }
            });

            if (addDependency)
            {
                switch (dependency.DependencyType)
                {
                    case ProjectDependencyType.Project:
                        this.ProjectReferceGroup.Add(new XElement("ProjectReference", new XAttribute("Include", dependency.FullPath)));
                        break;
                    case ProjectDependencyType.Binary:
                        FrameworkInfo netfxInfo = null;
                        FrameworkInfo dnxInfo = null;
                        string dnxStr = this.TargetFrameworks.FirstOrDefault(t => TargetFrameworkHelper.IsSupportedFramework(t, out dnxInfo) && dnxInfo.IsDnx);
                        if (this.TargetFrameworks.Count() > 1 && dependency.Name.Equals(TargetFrameworkHelper.FullFrameworkReferences.FirstOrDefault().Name)
                            && !string.IsNullOrWhiteSpace(dnxStr) && dnxInfo.Version.Major >= 6)
                        {
                            if (this.TargetFrameworks.Any(t => TargetFrameworkHelper.IsSupportedFramework(t, out netfxInfo) && !netfxInfo.IsDnx))
                            {
                                this.ReferenceGroup.Add(new XElement("Reference", new XAttribute("Condition", $"'$(TargetFramework)' == '{netfxInfo.FullName}'"), new XAttribute("Include", dependency.AssemblyName), new XElement("HintPath", dependency.FullPath)));
                            }
                        }
                        else
                        {
                            this.ReferenceGroup.Add(new XElement("Reference", new XAttribute("Include", dependency.AssemblyName), new XElement("HintPath", dependency.FullPath)));
                        }
                        break;
                    case ProjectDependencyType.Package:
                        this.PacakgeReferenceGroup.Add(new XElement("PackageReference", new XAttribute("Include", dependency.Name), new XAttribute("Version", dependency.Version)));
                        break;
                    case ProjectDependencyType.Tool:
                        this.ReferenceGroup.Add(new XElement("DotNetCliToolReference", new XAttribute("Include", dependency.Name), new XAttribute("Version", dependency.Version)));
                        break;
                }

                if(copyInternalAssets && dependency.AssemblyName == "dotnet-svcutil-lib")
                {
                    switch(dependency.DependencyType)
                    {
                        case ProjectDependencyType.Binary:
                            this.ReferenceGroup.Add(new XElement("Content", new XAttribute("CopyToOutputDirectory", "always"), new XAttribute("Include", Path.Combine(dependency.FullPath.Substring(0, dependency.FullPath.LastIndexOf(Path.DirectorySeparatorChar)), "internalAssets\\**")), new XAttribute("Link", "internalAssets/%(RecursiveDir)%(Filename)%(Extension)")));
                            break;
                        case ProjectDependencyType.Package:
                            string path = $"$(NuGetPackageRoot){dependency.Name}\\{dependency.Version}\\content\\internalAssets\\**";
                            this.PacakgeReferenceGroup.Add(new XElement("Content", new XAttribute("CopyToOutputDirectory", "always"), new XAttribute("Include", path), new XAttribute("Link", "internalAssets/%(RecursiveDir)%(Filename)%(Extension)")));
                            break;
                    }
                }

                _dependencies.Add(dependency);
                _isSaved = false;
            }

            return addDependency;
        }

        // Sets the property value in a PropertyGroup. Returns true if the value was changed, and false if it was already set to that value.
        private bool SetPropertyValue(string propertyName, string value)
        {
            XElement element = new XElement(propertyName, null);

            IEnumerable<XElement> existingElements = GetSubGroupValues(this.ProjectNode, _msbuildNS, "PropertyGroup", propertyName);
            if (existingElements.Count() > 0)
            {
                element = existingElements.Last();
            }
            else
            {
                IEnumerable<XElement> propertyGroupItems = GetGroupValues(this.ProjectNode, "PropertyGroup", createOnMissing: true);
                XElement propertyGroup = propertyGroupItems.First();
                propertyGroup.Add(element);
            }

            if (element.Value != value)
            {
                element.SetValue(value);
                return true;
            }

            return false;
        }

        private void SetRuntimeIdentifier(string runtimeIdentifier)
        {
            if (this.RuntimeIdentifier != runtimeIdentifier && !string.IsNullOrWhiteSpace(runtimeIdentifier))
            {
                if (SetPropertyValue("RuntimeIdentifier", runtimeIdentifier))
                {
                    _runtimeIdentifier = runtimeIdentifier;
                    _isSaved = false;
                }
            }
        }

        public void ClearWarningsAsErrors()
        {
            // Add an empty WarningsAsErrors element to clear the list, and treat them as warnings.
            SetPropertyValue("WarningsAsErrors", string.Empty);
        }

        private void UpdateTargetFramework(string targetFramework)
        {
            if (_targetFramework != targetFramework && !string.IsNullOrWhiteSpace(targetFramework))
            {
                // validate framework
                TargetFrameworkHelper.GetValidFrameworkInfo(targetFramework);

                if (!_targetFrameworks.Contains(targetFramework))
                {
                    // replace values (if existing).
                    if (_targetFramework != null && _targetFrameworks.Contains(_targetFramework))
                    {
                        _targetFrameworks.Remove(_targetFramework);
                    }

                    _targetFrameworks.Add(targetFramework);
                }

                IEnumerable<XElement> targetFrameworkElements = GetSubGroupValues(this.ProjectNode, _msbuildNS, "PropertyGroup", "TargetFramework");
                if (targetFrameworkElements.Count() > 0)
                {
                    var targetFrameworkNode = targetFrameworkElements.Last();
                    targetFrameworkNode.SetValue(targetFramework);
                }

                // TargetFramework was not provided, check TargetFrameworks property.
                IEnumerable<XElement> targetFrameworksElements = GetSubGroupValues(this.ProjectNode, _msbuildNS, "PropertyGroup", "TargetFrameworks");
                if (targetFrameworksElements.Count() > 0)
                {
                    var targetFrameworksNode = targetFrameworksElements.Last();
                    targetFrameworksNode.SetValue(_targetFrameworks.Aggregate((tfs, tf) => $"{tfs};{tf}"));
                }

                _targetFramework = targetFramework;
                _isSaved = false;
            }
        }
        #endregion

        #region Operation Methods
        public async Task SaveAsync(bool force, ILogger logger, CancellationToken cancellationToken)
        {
            _isSaved &= !force;
            await SaveAsync(logger, cancellationToken).ConfigureAwait(false);
        }

        public async Task SaveAsync(ILogger logger, CancellationToken cancellationToken)
        {
            ThrowOnDisposed();

            cancellationToken.ThrowIfCancellationRequested();

            if (!_isSaved)
            {
                using (await SafeLogger.WriteStartOperationAsync(logger, $"Saving project file: \"{this.FullPath}\"").ConfigureAwait(false))
                {
                    if (!Directory.Exists(this.DirectoryPath))
                    {
                        Directory.CreateDirectory(this.DirectoryPath);
                        _ownsDirectory = true;
                    }

                    using (StreamWriter writer = File.CreateText(this.FullPath))
                    {
                        await AsyncHelper.RunAsync(() => ProjectNode.Save(writer), cancellationToken).ConfigureAwait(false);
                    }

                    _isSaved = true;
                }
            }
        }

        public async Task<ProcessRunner.ProcessResult> RestoreAsync(ILogger logger, CancellationToken cancellationToken)
        {
            ThrowOnDisposed();

            if (!_isSaved)
            {
                await this.SaveAsync(logger, cancellationToken).ConfigureAwait(false);
            }

            var restoreParams = "restore --ignore-failed-sources" + (string.IsNullOrWhiteSpace(this.RuntimeIdentifier) ? "" : (" -r " + this.RuntimeIdentifier));
            // Restore no-dependencies first to workaround NuGet issue https://github.com/NuGet/Home/issues/4979
            await ProcessRunner.TryRunAsync("dotnet", restoreParams + " --no-dependencies", this.DirectoryPath, logger, cancellationToken).ConfigureAwait(false);
            var result = await ProcessRunner.TryRunAsync("dotnet", restoreParams, this.DirectoryPath, logger, cancellationToken).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Builds the project and optionally restores it before building. If restore is false the project is not saved automatically.
        /// </summary>
        /// <returns></returns>
        public async Task<ProcessRunner.ProcessResult> BuildAsync(bool restore, ILogger logger, CancellationToken cancellationToken)
        {
            if (restore)
            {
                await this.RestoreAsync(logger, cancellationToken).ConfigureAwait(false);
            }
            return await BuildAsync(logger, cancellationToken).ConfigureAwait(false);
        }

        public async Task<ProcessRunner.ProcessResult> BuildAsync(ILogger logger, CancellationToken cancellationToken)
        {
            ThrowOnDisposed();

            string buildParams = $"build {GetNoRestoreParam(this.SdkVersion)}";
            return await ProcessRunner.RunAsync("dotnet", buildParams, this.DirectoryPath, logger, cancellationToken).ConfigureAwait(false);
        }
        #endregion

        #region Helper Methods
        public async Task<IEnumerable<ProjectDependency>> ResolveProjectReferencesAsync(IEnumerable<ProjectDependency> excludeDependencies, ILogger logger, CancellationToken cancellationToken)
        {
            ThrowOnDisposed();

            IEnumerable<ProjectDependency> dependencies = null;

            if (excludeDependencies == null)
            {
                excludeDependencies = new List<ProjectDependency>();
            }

            using (var safeLogger = await SafeLogger.WriteStartOperationAsync(logger, "Resolving project references ...").ConfigureAwait(false))
            {
                if (_targetFrameworks.Count == 1 && TargetFrameworkHelper.IsSupportedFramework(this.TargetFramework, out var frameworkInfo) && frameworkInfo.IsDnx)
                {
                    await this.RestoreAsync(logger, cancellationToken).ConfigureAwait(false);

                    var packageReferences = await ResolvePackageReferencesAsync(logger, cancellationToken).ConfigureAwait(false);
                    var assemblyReferences = await ResolveAssemblyReferencesAsync(logger, cancellationToken).ConfigureAwait(false);
                    dependencies = packageReferences.Union(assemblyReferences).Except(excludeDependencies);
                }
                else
                {
                    await safeLogger.WriteWarningAsync(Shared.Resources.WarningMultiFxOrNoSupportedDnxVersion, logToUI: true).ConfigureAwait(false);
                    dependencies = new List<ProjectDependency>();
                }

                await safeLogger.WriteMessageAsync($"Resolved project reference count: {dependencies.Count()}", logToUI: false).ConfigureAwait(false);
            }

            return dependencies;
        }

        private async Task<List<ProjectDependency>> ResolvePackageReferencesAsync(ILogger logger, CancellationToken cancellationToken)
        {
            ThrowOnDisposed();

            cancellationToken.ThrowIfCancellationRequested();

            var packageDependencies = new List<ProjectDependency>();

            using (var safeLogger = await SafeLogger.WriteStartOperationAsync(logger, "Resolving package references ...").ConfigureAwait(false))
            {
                await AsyncHelper.RunAsync(async () =>
                {
                    try
                    {
                        var assetsFile = new FileInfo(Path.Combine(this.DirectoryPath, "obj", "project.assets.json")).FullName;
                        if (File.Exists(assetsFile))
                        {
                            LockFile lockFile = LockFileUtilities.GetLockFile(assetsFile, logger as NuGet.Common.ILogger);

                            if (lockFile != null)
                            {
                                if (lockFile.Targets.Count == 1)
                                {
                                    foreach (var lib in lockFile.Targets[0].Libraries)
                                    {
                                        bool isPackage = StringComparer.OrdinalIgnoreCase.Compare(lib.Type, "package") == 0;

                                        if (isPackage)
                                        {
                                            foreach (var compiletimeAssembly in lib.CompileTimeAssemblies)
                                            {
                                                if (Path.GetExtension(compiletimeAssembly.Path) == ".dll")
                                                {
                                                    var dependency = ProjectDependency.FromPackage(Path.GetFileNameWithoutExtension(compiletimeAssembly.Path), lib.Name, lib.Version.ToNormalizedString());
                                                    var itemIdx = packageDependencies.IndexOf(dependency);

                                                    if (itemIdx == -1)
                                                    {
                                                        packageDependencies.Add(dependency);
                                                    }
                                                    else if (dependency.IsFramework)
                                                    {
                                                        // packages can be described individually and/or as part of a platform metapackage in the lock file; for instance: Microsoft.CSharp is a package that is part of Microsoft.NetCore.
                                                        packageDependencies[itemIdx] = dependency;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    packageDependencies.Sort();
                                }
                                else
                                {
                                    await safeLogger.WriteWarningAsync(Shared.Resources.WarningMultiFxOrNoSupportedDnxVersion, logToUI: true).ConfigureAwait(false);
                                }
                            }
                            else
                            {
                                await safeLogger.WriteWarningAsync(Shared.Resources.WarningCannotResolveProjectReferences, logToUI: true).ConfigureAwait(false);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (Utils.IsFatalOrUnexpected(ex)) throw;
                        await safeLogger.WriteWarningAsync(ex.Message, logToUI: false).ConfigureAwait(false);
                    }
                }, cancellationToken).ConfigureAwait(false);

                await safeLogger.WriteMessageAsync($"Package reference count: {packageDependencies.Count}", logToUI: false).ConfigureAwait(false);
            }

            return packageDependencies;
        }

        private async Task<List<ProjectDependency>> ResolveAssemblyReferencesAsync(ILogger logger, CancellationToken cancellationToken)
        {
            ThrowOnDisposed();

            cancellationToken.ThrowIfCancellationRequested();

            var assemblyDependencies = new List<ProjectDependency>();

            using (var safeLogger = await SafeLogger.WriteStartOperationAsync(logger, $"Resolving assembly references for {this.TargetFramework} target framework ...").ConfigureAwait(false))
            {
                await ResolveProperyValuesAsync(new string[] { "OutputPath", "TargetPath" }, logger, cancellationToken).ConfigureAwait(false);

                var outputPath = this._resolvedProperties["OutputPath"];
                if (!Path.IsPathRooted(outputPath))
                {
                    outputPath = Path.Combine(this.DirectoryPath, outputPath.Trim(new char[] { '\"' }));
                }

                var depsFile = this.GlobalProperties.TryGetValue("Configuration", out var activeConfiguration) && !string.IsNullOrWhiteSpace(activeConfiguration) ?
                    Path.Combine(outputPath, $"{Path.GetFileNameWithoutExtension(this.FileName)}.deps.json") :
                    await ResolveDepsFilePathFromBuildConfigAsync(outputPath, logger, cancellationToken).ConfigureAwait(false);

                if (File.Exists(depsFile))
                {
                    await AsyncHelper.RunAsync(async () =>
                    {
                        try
                        {
                            DependencyContext depContext = null;
                            using (var stream = File.OpenRead(depsFile))
                            {
                                depContext = new DependencyContextJsonReader().Read(stream);
                            }

                            var targetLib = Path.GetFileName(this._resolvedProperties["TargetPath"].Trim('\"'));
                            if (string.IsNullOrEmpty(targetLib))
                            {
                                targetLib = $"{Path.ChangeExtension(this.FileName, ".dll")}";
                            }

                            foreach (var rtLib in depContext.RuntimeLibraries.Where(l => l.NativeLibraryGroups.Count == 0))
                            {
                                ProjectDependency dependency = null;
                                switch (rtLib.Type)
                                {
                                    case "project":
                                    case "reference":
                                        foreach (var assemblyGroup in rtLib.RuntimeAssemblyGroups)
                                        {
                                            foreach (var assetPath in assemblyGroup.AssetPaths)
                                            {
                                                if (!Path.GetFileName(assetPath).Equals(targetLib, RuntimeEnvironmentHelper.FileStringComparison))
                                                {
                                                    dependency = ProjectDependency.FromAssembly(Path.Combine(outputPath, assetPath));
                                                    if (File.Exists(dependency.FullPath) && !assemblyDependencies.Contains(dependency))
                                                    {
                                                        assemblyDependencies.Add(dependency);
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    //case "package":
                                    default:
                                        break;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            if (Utils.IsFatalOrUnexpected(ex)) throw;
                            await safeLogger.WriteWarningAsync(ex.Message, logToUI: false).ConfigureAwait(false);
                        }
                    }, cancellationToken).ConfigureAwait(false);

                    assemblyDependencies.Sort();
                }
                else
                {
                    await safeLogger.WriteWarningAsync("Deps file not found (project not built), unable to resolve assembly/project dependencies!", logToUI: false).ConfigureAwait(false);
                }

                await safeLogger.WriteMessageAsync($"Assembly reference count: {assemblyDependencies.Count}", logToUI: false).ConfigureAwait(false);
            }

            return assemblyDependencies;
        }

        public async Task<IEnumerable<KeyValuePair<string, string>>> ResolveProperyValuesAsync(IEnumerable<string> propertyNames, ILogger logger, CancellationToken cancellationToken)
        {
            ThrowOnDisposed();

            cancellationToken.ThrowIfCancellationRequested();

            if (propertyNames == null)
            {
                throw new ArgumentNullException(nameof(propertyNames));
            }

            if (!this.GlobalProperties.Any(p => p.Key == "TargetFramework"))
            {
                this.GlobalProperties["TargetFramework"] = this.TargetFrameworks.FirstOrDefault();
            }

            if (!this.GlobalProperties.Any(p => p.Key == "SdkVersion"))
            {
                this.GlobalProperties["SdkVersion"] = this.SdkVersion;
            }

            if (!propertyNames.All(p => this._resolvedProperties.Keys.Contains(p)))
            {
                var propertyTable = this._resolvedProperties.Where(p => propertyNames.Contains(p.Key));

                if (propertyTable.Count() != propertyNames.Count())
                {
                    propertyTable = await _propertyResolver.EvaluateProjectPropertiesAsync(this.FullPath, this.TargetFrameworks.FirstOrDefault(), propertyNames, this.GlobalProperties, logger, cancellationToken).ConfigureAwait(false);

                    foreach (var entry in propertyTable)
                    {
                        this._resolvedProperties[entry.Key] = entry.Value;
                    }
                }
            }

            return this._resolvedProperties.Where(p => propertyNames.Contains(p.Key));
        }

        private async Task<string> ResolveDepsFilePathFromBuildConfigAsync(string outputPath, ILogger logger, CancellationToken cancellationToken)
        {
            // Since we are resolving the deps file path it means the passed in outputPath is built using the default build 
            // configuration (debug/release). We need to resolve the configuration by looking at the most recent build in the
            // output path. The output should look something like 'bin\Debug\netcoreapp1.0\HelloSvcutil.deps.json'

            using (var safeLogger = await SafeLogger.WriteStartOperationAsync(logger, $"Resolving deps.json file ...").ConfigureAwait(false))
            {
                var fileName = $"{Path.GetFileNameWithoutExtension(this.FileName)}.deps.json";
                var depsFile = string.Empty;

                // find the most recent deps.json files under the project's bin folder built for the project's target framework.
                var binFolder = await PathHelper.TryFindFolderAsync("bin", outputPath, logger, cancellationToken).ConfigureAwait(false);

                if (Directory.Exists(binFolder))
                {
                    var depsFiles = Directory.GetFiles(binFolder, "*", SearchOption.AllDirectories)
                        .Where(d => Path.GetFileName(d).Equals(fileName, RuntimeEnvironmentHelper.FileStringComparison))
                        .Where(f => PathHelper.GetFolderName(Path.GetDirectoryName(f)) == this.TargetFrameworks.FirstOrDefault())
                        .Select(f => new FileInfo(f))
                        .OrderByDescending(f => f.CreationTimeUtc);

                    depsFile = depsFiles.FirstOrDefault()?.FullName;
                }

                await safeLogger.WriteMessageAsync($"deps file: {depsFile}", logToUI: false).ConfigureAwait(false);
                return depsFile;
            }
        }

        public override string ToString()
        {
            return this.FullPath;
        }

        private void ThrowOnDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(MSBuildProj));
            }
        }

        private static string GetNoRestoreParam(string sdkVersion)
        {
            if (string.IsNullOrEmpty(sdkVersion) || sdkVersion.StartsWith("1", StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }
            return "--no-restore";
        }
        #endregion

        #region IDisposable Support
        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    try
                    {
                        if (_ownsDirectory && Directory.Exists(this.DirectoryPath) && !DebugUtils.KeepTemporaryDirs)
                        {
                            try { Directory.Delete(this.DirectoryPath, recursive: true); } catch { }
                        }
                    }
                    catch
                    {
                    }
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
