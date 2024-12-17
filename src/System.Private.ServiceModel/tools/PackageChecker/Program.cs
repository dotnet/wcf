using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Packaging.Core;
using NuGet.Packaging;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using System;
using NuGet.Versioning;
using NuGet.Frameworks;

namespace PackageChecker
{
    internal class Program
    {
        /// <summary>
        /// This app validates a NuGet package is formed correctly. Currently it only validates that all types in the
        /// netstandard2.0 reference assembly are type forwarded for .NET Framework.
        /// </summary>
        /// <param name="package">The NuGet package to validate</param>
        /// <param name="verbose">Turns on verbose output</param>
        private static async Task<int> Main(FileInfo package, bool verbose)
        {
            if (!package.Exists)
            {
                Console.Error.WriteLine($"Unable to find package {package.FullName}");
                return 1;
            }

            var packageReader = new PackageArchiveReader(File.OpenRead(package.FullName));
            var supportedFrameworks = GetSupportedFrameworks(packageReader);
            if (!supportedFrameworks.Any())
            {
                Console.Error.WriteLine("No supported .NET TFMs found in the package.");
                return 1;
            }

            var dependencies = await DownloadNugetPackageDependenciesAsync(packageReader, package.DirectoryName, verbose);
            Console.WriteLine("Downloaded all dependencies");
            foreach (var tfm in supportedFrameworks)
            {
                if (IsNetStandard20(tfm))
                {
                    Console.WriteLine("Checking type forwards from netstandard2.0 to NetFx");
                    CheckNetstandardToNetFxTypeForwards(packageReader, dependencies[tfm], verbose);
                }
                else
                {
                    Console.WriteLine($"Validating reference assembly matches implementation for tfm {tfm.GetShortFolderName()}");
                    CheckReferenceAssemblyMatchesImplementation(packageReader, tfm, dependencies.ContainsKey(tfm) ? dependencies[tfm] : new (), verbose);
                }
            }

            return 0;
        }

        private static bool CheckReferenceAssemblyMatchesImplementation(PackageReaderBase packageReader, NuGetFramework tfm, List<string> dependencies, bool verbose)
        {
            FrameworkSpecificGroup libsFsg = packageReader.GetLibItems().Where(fsg => fsg.TargetFramework.Equals(tfm)).SingleOrDefault();
            FrameworkSpecificGroup refsFsg = packageReader.GetItems(PackagingConstants.Folders.Ref).Where(fsg => fsg.TargetFramework.Equals(tfm)).SingleOrDefault();

            if (libsFsg is null)
            {
                Console.Error.WriteLine($"Something went wrong. Package was detected as supporting {tfm} but has no lib/{tfm.GetShortFolderName()} folder.");
                return false;
            }

            if (refsFsg is null)
            {
                if (libsFsg.Items.Any())
                {
                    Console.WriteLine($"No reference assemblies found for {tfm.GetShortFolderName()}. If this package isn't using reference assemblies, this is normal.");
                }
                return true;
            }

            var runtimeFolder = RuntimeEnvironment.GetRuntimeDirectory();
            if (verbose) { Console.WriteLine($"Using .NET runtime assemblies from {runtimeFolder}"); }
            var runtimeAssemblies = new DirectoryInfo(runtimeFolder).EnumerateFiles("*.dll").Select(fi => fi.FullName);
            if (dependencies != null && dependencies.Any())
            {
                runtimeAssemblies = runtimeAssemblies.Concat(dependencies);
            }

            var assemblyResolver = new PathAssemblyResolver(runtimeAssemblies);
            var context = new MetadataLoadContext(assemblyResolver);

            if (verbose) { Console.WriteLine($"Loading type and member info from {tfm.GetShortFolderName()} reference assemblies"); }
            List<ExportedType> referenceTypes = new();
            foreach (var refAssemblyItem in refsFsg.Items.Where(item => item.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)))
            {
                if (verbose) { Console.WriteLine($"Loading types from assembly {refAssemblyItem}"); }
                MemoryStream memoryStream = new MemoryStream();
                using (Stream assemblyStream = packageReader.GetStream(refAssemblyItem))
                {
                    assemblyStream.CopyTo(memoryStream);
                }
                memoryStream.Position = 0;
                Assembly assembly;
                try
                {
                    assembly = context.LoadFromStream(memoryStream);
                }
                catch (BadImageFormatException bife)
                {
                    Console.Error.WriteLine($"Error loading assembly {refAssemblyItem}");
                    Console.Error.WriteLine(bife.Message);
                    continue;
                }
                var assemblyExportedTypes = assembly.GetExportedTypes();
                foreach (var exportedType in assemblyExportedTypes)
                {
                    if (exportedType.IsPublic || exportedType.IsNestedFamily)
                    {
                        referenceTypes.Add(new ExportedType(exportedType));
                    }
                }
            }

            context.Dispose();

            context = new MetadataLoadContext(assemblyResolver);
            if (verbose) { Console.WriteLine($"Loading type and member info from {tfm} implementation assemblies"); }
            List<ExportedType> implementationTypes = new();
            foreach (var libAssemblyItem in libsFsg.Items.Where(item => item.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)))
            {
                if (verbose) { Console.WriteLine($"Loading types from assembly {libAssemblyItem}"); }
                MemoryStream memoryStream = new MemoryStream();
                using (Stream assemblyStream = packageReader.GetStream(libAssemblyItem))
                {
                    assemblyStream.CopyTo(memoryStream);
                }
                memoryStream.Position = 0;
                Assembly assembly;
                try
                {
                    assembly = context.LoadFromStream(memoryStream);
                }
                catch (BadImageFormatException bife)
                {
                    Console.Error.WriteLine($"Error loading assembly {libAssemblyItem}");
                    Console.Error.WriteLine(bife.Message);
                    continue;
                }

                var assemblyExportedTypes = assembly.GetExportedTypes();
                foreach (var exportedType in assemblyExportedTypes)
                {
                    if (exportedType.IsPublic || exportedType.IsNestedFamily)
                    {
                        implementationTypes.Add(new ExportedType(exportedType));
                    }
                }
            }

            context.Dispose();

            bool hasError = false;
            Dictionary<string, ExportedType> implementedTypes = new();
            implementationTypes.ForEach(t =>
            {
                if (implementedTypes.ContainsKey(t.TypeName))
                {
                    Console.Error.WriteLine($"Type {t.TypeName} is duplicated in the implementation assemblies");
                    hasError = true;
                }
                else
                {
                    implementedTypes[t.TypeName] = t;
                }
            });

            foreach (var referenceType in referenceTypes)
            {
                if (!implementedTypes.ContainsKey(referenceType.TypeName))
                {
                    Console.Error.WriteLine($"Type {referenceType.TypeName} not found in implementation assemblies");
                    hasError = true;
                }
                else
                {
                    var candidateType = implementedTypes[referenceType.TypeName];
                    if (referenceType.IsMatchingImplmentation(candidateType))
                    {
                        if (verbose) { Console.WriteLine($"Type {referenceType.TypeName} matches implementation"); }
                    }
                    else
                    {
                        Console.Error.WriteLine($"Type {referenceType.TypeName} doesn't match implementation");
                        hasError = true;
                    }
                }
            }

            return !hasError;
        }

        private static bool CheckNetstandardToNetFxTypeForwards(PackageReaderBase packageReader, List<string> dependencies, bool verbose)
        {
            FrameworkSpecificGroup netfxLibsFsg = packageReader.GetLibItems().Where(fsg => fsg.TargetFramework.Framework.Equals(".NETFramework")).SingleOrDefault();
            FrameworkSpecificGroup netstandardFsg = packageReader.GetReferenceItems().Where(fsg => IsNetStandard20(fsg.TargetFramework)).SingleOrDefault();
            if (netstandardFsg is null)
            {
                netstandardFsg = packageReader.GetLibItems().Where(fsg => IsNetStandard20(fsg.TargetFramework)).SingleOrDefault();
            }

            if (netfxLibsFsg is null)
            {
                Console.Error.WriteLine("No lib assemblies found for netfx. No type forwards to check.");
                Console.Error.WriteLine("If netfx isn't intended to be supported, this is expected.");
                return false;
            }

            if (netstandardFsg is null)
            {
                Console.Error.WriteLine("No reference or lib assemblies found for netstandard2.0. No type forwards to check.");
                Console.Error.WriteLine("If netstandard2.0 isn't intended to be supported, this is expected.");
                return false;
            }

            var runtimeFolder = @"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\";
            if (verbose) { Console.WriteLine($"Using netfx runtime assemblies from {runtimeFolder}"); }
            var runtimeAssemblies = new DirectoryInfo(runtimeFolder).EnumerateFiles("*.dll").Select(fi => fi.FullName);
            if (dependencies != null && dependencies.Any())
            {
                runtimeAssemblies = runtimeAssemblies.Concat(dependencies);
            }
            var assemblyResolver = new PathAssemblyResolver(runtimeAssemblies);
            var context = new MetadataLoadContext(assemblyResolver);
            if (verbose) { Console.WriteLine("Loading type and member info from netstandard reference assemblies"); }
            List<ExportedType> netStandardTypes = new();
            var memoryStream = new MemoryStream();
            foreach (var refAssemblyItem in netstandardFsg.Items.Where(item => item.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)))
            {
                if (verbose) { Console.WriteLine($"Loading types from assembly {refAssemblyItem}"); }
                memoryStream.SetLength(0);
                using (Stream assemblyStream = packageReader.GetStream(refAssemblyItem))
                {
                    assemblyStream.CopyTo(memoryStream);
                }
                memoryStream.Position = 0;
                Assembly assembly;
                try
                {
                    assembly = context.LoadFromStream(memoryStream);
                }
                catch (BadImageFormatException bife)
                {
                    Console.Error.WriteLine($"Error loading assembly {refAssemblyItem}");
                    Console.Error.WriteLine(bife.Message);
                    continue;
                }

                var assemblyExportedTypes = assembly.GetExportedTypes();
                foreach (var exportedType in assemblyExportedTypes)
                {
                    netStandardTypes.Add(new ExportedType(exportedType));
                }
            }

            context.Dispose();

            context = new MetadataLoadContext(assemblyResolver);
            var forwardedTypes = new HashSet<string>();
            foreach (var typeForwardingAssemblyItem in netfxLibsFsg.Items.Where(item => item.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine($"Checking assembly {typeForwardingAssemblyItem}");
                memoryStream.SetLength(0);
                using (Stream assemblyStream = packageReader.GetStream(typeForwardingAssemblyItem))
                {
                    assemblyStream.CopyTo(memoryStream);
                }
                memoryStream.Position = 0;
                Assembly assembly;
                try
                {
                    assembly = context.LoadFromStream(memoryStream);
                }
                catch (BadImageFormatException bife)
                {
                    Console.Error.WriteLine($"Error loading assembly {typeForwardingAssemblyItem}");
                    Console.Error.WriteLine(bife.Message);
                    continue;
                }

                foreach (var forwardedType in assembly.GetForwardedTypes())
                {
                    if (verbose) { Console.WriteLine($"Found forwarded type: {forwardedType.FullName}"); }
                    forwardedTypes.Add(forwardedType.FullName!);
                }
            }

            context.Dispose();

            List<string> missingForwards = new();
            foreach (var exportedType in netStandardTypes)
            {
                if (!forwardedTypes.Contains(exportedType.TypeName))
                {
                    if (verbose) { Console.WriteLine($"Type {exportedType.TypeName} hasn't been forwarded"); }
                    missingForwards.Add(exportedType.TypeName);
                }
                else if (verbose) { Console.WriteLine($"Type {exportedType.TypeName} has been forwarded"); }
            }

            if (verbose) { Console.WriteLine(); }
            if (missingForwards.Count > 0)
            {
                Console.WriteLine($"There are {missingForwards.Count} missing type forwards. Add the following type forwards:");
                foreach (var missingForward in missingForwards)
                {
                    Console.WriteLine($"[assembly: TypeForwardedTo(typeof({missingForward}))]");
                }

                Console.WriteLine();
                return false;
            }
            else
            {
                return true;
            }
        }

        private static async Task<Dictionary<NuGetFramework, List<string>>> DownloadNugetPackageDependenciesAsync(PackageReaderBase packageReader, string packageLocation, bool verbose)
        {
            Dictionary<NuGetFramework, List<string>> dependencies = new();

            // Get dependencies
            var dependencyGroups = packageReader.GetPackageDependencies();
            foreach (var group in dependencyGroups)
            {
                var tfm = group.TargetFramework;
                var dependencyPaths = new List<string>();
                if (verbose) { Console.WriteLine($"Getting dependencies for tfm: {tfm.GetFrameworkString()}"); }
                foreach (var dependency in group.Packages)
                {
                    List<string> implementationPath = await DownloadNugetPackageAndGetImplementationPathsAsync(dependency, group.TargetFramework, packageLocation, verbose);
                    if (implementationPath == null)
                    {
                        Console.Error.WriteLine($"Failed to download {dependency.Id} {dependency.VersionRange} for {tfm}, may fail later");
                        continue;
                    }

                    dependencyPaths.AddRange(implementationPath);
                }

                if (verbose)
                {
                    Console.WriteLine($"Found {dependencyPaths.Count} dependencies for {tfm.GetFrameworkString()}");
                    foreach (var path in dependencyPaths)
                    {
                        Console.WriteLine($"  {path}");
                    }

                    Console.WriteLine();
                }

                dependencies[tfm] = dependencyPaths;
            }

            return dependencies;
        }

        private static async Task<List<string>> DownloadNugetPackageAndGetImplementationPathsAsync(PackageDependency dependency, NuGetFramework tfm, string packageLocation, bool verbose)
        {
            // TODO: Wire up proper logging
            // TODO: Wire up proper cancellation token

            bool isServiceModelDependency = dependency.Id.StartsWith("System.ServiceModel.");

            // Define the package sources
            List<PackageSource> packageSources;
            if (isServiceModelDependency)
            {
                packageSources = new List<PackageSource>
                {
                    new PackageSource(packageLocation)
                };
            }
            else
            {
                packageSources = new List<PackageSource>
                {
                    new PackageSource("https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-eng/nuget/v3/index.json"),
                    new PackageSource("https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-public/nuget/v3/index.json")
                };
            }

            // Create the source repositories
            var sourceRepositories = new List<SourceRepository>();
            foreach (var source in packageSources)
            {
                sourceRepositories.Add(Repository.Factory.GetCoreV3(source));
            }

            var logger = NullLogger.Instance;
            var cacheContext = new SourceCacheContext();
            var downloadContext = new PackageDownloadContext(cacheContext);
            string packagesDownloadFolder;
            if (isServiceModelDependency)
            {
                packagesDownloadFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(packagesDownloadFolder);
                if (verbose) { Console.WriteLine($"Using temporary packages folder {packagesDownloadFolder} to install {dependency.Id}"); }
            }
            else
            {
                packagesDownloadFolder = SettingsUtility.GetGlobalPackagesFolder(Settings.LoadDefaultSettings(null));
            }

            // Loop through the repositories to find and download the package
            foreach (var repository in sourceRepositories)
            {
                NuGetVersion matchedVersion;
                if (isServiceModelDependency)
                {
                    matchedVersion = dependency.VersionRange.MinVersion;
                }
                else
                {
                    var findPackageByIdResource = await repository.GetResourceAsync<FindPackageByIdResource>();
                    // Find all versions that match the version range
                    var allVersions = await findPackageByIdResource.GetAllVersionsAsync(dependency.Id, cacheContext, logger, default);
                    if (allVersions == null)
                    {
                        if (verbose) { Console.WriteLine($"Didn't find package {dependency.Id} in repository {repository.PackageSource.SourceUri}"); }
                        continue;
                    }
                    matchedVersion = allVersions.Where(dependency.VersionRange.Satisfies).OrderByDescending(v => v).FirstOrDefault();
                    if (matchedVersion == null)
                    {
                        if (verbose) { Console.WriteLine($"No version found that satisfies the {dependency.Id} package version range {dependency.VersionRange.ToNormalizedString()}."); }
                        continue;
                    }
                }

                // Get the DownloadResource and download the package
                var downloadResource = await repository.GetResourceAsync<DownloadResource>();
                var downloadResult = await downloadResource.GetDownloadResourceResultAsync(
                    new PackageIdentity(dependency.Id, matchedVersion),
                    downloadContext,
                    packagesDownloadFolder,
                    logger,
                    CancellationToken.None);

                if (downloadResult.Status == DownloadResourceResultStatus.Available || downloadResult.Status == DownloadResourceResultStatus.AvailableWithoutStream)
                {
                    var packageTfmToUse = GetNearestTargetFramework(downloadResult.PackageReader, tfm);
                    if (packageTfmToUse == default)
                    {
                        Console.WriteLine($"No target framework found that's compatible with {tfm} for {dependency.Id} {matchedVersion}. Found {string.Join(" ,", GetSupportedFrameworks(downloadResult.PackageReader).Select(t => t.ToString()))}");
                        return null; // Returning instead of continuing as we found the package with the right version but no compatible TFM. We won't find the right version elsewhere.
                    }

                    var packageExtractPath = Path.Combine(packagesDownloadFolder, dependency.Id, matchedVersion.ToNormalizedString());
                    var libItems = downloadResult.PackageReader.GetLibItems().Where(fsg => fsg.TargetFramework.Equals(packageTfmToUse)).FirstOrDefault();
                    if (libItems == null)
                    {
                        Console.WriteLine($"No lib items found for {packageTfmToUse} in {dependency.Id} {matchedVersion}");
                        return null;
                    }

                    if (isServiceModelDependency)
                    {
                        // GetDownloadResourceResultAsync doesn't extract the package if it's local, so we need to do it ourselves
                        Directory.CreateDirectory(packageExtractPath);
                        foreach(var file in libItems.Items.Where(i => i.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)))
                        {
                            var targetPath = Path.Combine(packageExtractPath, file);
                            var directoryPath = Path.GetDirectoryName(targetPath);
                            if (!string.IsNullOrEmpty(directoryPath))
                            {
                                Directory.CreateDirectory(directoryPath);
                            }

                            using (var fileStream = downloadResult.PackageReader.GetStream(file))
                            using (var outStream = new FileStream(targetPath, FileMode.Create, FileAccess.Write, FileShare.None))
                            {
                                await fileStream.CopyToAsync(outStream);
                            }
                        }
                    }

                    return libItems.Items.Where(i => i.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)).Select(i => Path.Combine(packageExtractPath, i)).ToList();
                }
            }

            return null;
        }

        public static NuGetFramework GetNearestTargetFramework(PackageReaderBase packageReader, NuGetFramework targetFramework)
        {
            var supportedFrameworks = GetSupportedFrameworks(packageReader);
            FrameworkReducer frameworkReducer = new FrameworkReducer();
            NuGetFramework mostCompatibleFramework = frameworkReducer.GetNearest(targetFramework, supportedFrameworks);
            return mostCompatibleFramework;
        }

        public static IEnumerable<NuGetFramework> GetSupportedFrameworks(PackageReaderBase packageReader)
        {
            var frameworks = new HashSet<NuGetFramework>();

            // Get the frameworks from the lib folder
            foreach (var file in packageReader.GetLibItems())
            {
                frameworks.Add(file.TargetFramework);
            }

            // Get the frameworks from the dependency groups
            foreach (var group in packageReader.NuspecReader.GetDependencyGroups())
            {
                frameworks.Add(group.TargetFramework);
            }

            return frameworks;
        }

        public static bool IsNetStandard20(NuGetFramework framework)
        {
            return framework.Framework.Equals(".NETStandard", StringComparison.OrdinalIgnoreCase) &&
                   framework.Version == new Version(2, 0);
        }
    }
}
