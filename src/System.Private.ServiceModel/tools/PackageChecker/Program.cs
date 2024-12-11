using System.IO.Compression;
using System.Reflection;
using System.Runtime.InteropServices;

internal class Program
{
    /// <summary>
    /// This app validates a NuGet package is formed correctly. Currently it only validates that all types in the
    /// netstandard2.0 reference assembly are type forwarded for .NET Framework.
    /// </summary>
    /// <param name="package">The NuGet package to validate</param>
    /// <param name="verbose">Turns on verbose output</param>
    private static int Main(FileInfo package, bool verbose)
    {
        if (!package.Exists)
        {
            Console.Error.WriteLine($"Unable to find package {package.FullName}");
            return 1;
        }

        Stream nugetPackageStream;
        try
        {
            nugetPackageStream = File.OpenRead(package.FullName);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"Unable to open package {package.FullName}");
            Console.Error.WriteLine(e.Message);
            return 1;
        }

        var zipArchive = new ZipArchive(nugetPackageStream, ZipArchiveMode.Read);
        var netFxLibEntries = zipArchive.Entries.Where(e => e.FullName.StartsWith("lib/net4"));
        if (!netFxLibEntries.Any())
        {
            Console.Error.WriteLine("No assemblies found in a lib/net4?? folder. No type forwards to check.");
            Console.Error.WriteLine("If netfx isn't intended to be supported, this is expected.");
            return 1;
        }

        var netstandardRefEntries = zipArchive.Entries.Where(e => e.FullName.StartsWith("ref/netstandard2.0/"));
        if (!netstandardRefEntries.Any())
        {
            Console.Error.WriteLine("No assemblies in the ref/netstandard2.0 folder, checking lib/netstandard2.0");
            netstandardRefEntries = zipArchive.Entries.Where(e => e.FullName.StartsWith("lib/netstandard2.0/"));
            if (!netstandardRefEntries.Any())
            {
                Console.Error.WriteLine("No assemblies in the lib/netstandard2.0 folder");
                return 1;
            }
        }

        var runtimeFolder = @"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\";
        if (verbose) { Console.WriteLine($"Using netfx runtime assemblies from {runtimeFolder}"); }
        var runtimeAssemblies = new DirectoryInfo(runtimeFolder).EnumerateFiles("*.dll").Select(fi => fi.FullName);
        var assemblyResolver = new PathAssemblyResolver(runtimeAssemblies);
        var context = new MetadataLoadContext(assemblyResolver);
        var forwardedTypes = new HashSet<string>();
        foreach(var typeForwardingAssemblyEntry in netFxLibEntries.Where(entry => entry.FullName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)))
        {
            Console.WriteLine($"Checking assembly {typeForwardingAssemblyEntry.FullName}");
            var assemblyStream = typeForwardingAssemblyEntry.Open();
            var memoryStream = new MemoryStream();
            assemblyStream.CopyTo(memoryStream);
            memoryStream.Position = 0;
            Assembly assembly;
            try
            {
                assembly = context.LoadFromStream(memoryStream);
            }
            catch (System.BadImageFormatException bife)
            {
                Console.Error.WriteLine($"Error loading assembly {typeForwardingAssemblyEntry.FullName}");
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

        context = new MetadataLoadContext(assemblyResolver);
        List<string> missingForwards = new();
        foreach(var refAssemblyEntry in netstandardRefEntries.Where(entry => entry.FullName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)))
        {
            if (verbose) { Console.WriteLine($"Checking types from assembly {refAssemblyEntry.FullName} have been forwarded"); }
            var assemblyStream = refAssemblyEntry.Open();
            var memoryStream = new MemoryStream();
            assemblyStream.CopyTo(memoryStream);
            memoryStream.Position = 0;
            Assembly assembly;
            try
            {
                assembly = context.LoadFromStream(memoryStream);
            }
            catch (System.BadImageFormatException bife)
            {
                Console.Error.WriteLine($"Error loading assembly {refAssemblyEntry.FullName}");
                Console.Error.WriteLine(bife.Message);
                continue;
            }

            var exportedTypes = assembly.GetExportedTypes();
            foreach(var exportedType in exportedTypes)
            {
                if (!forwardedTypes.Contains(exportedType.FullName))
                {
                    if (verbose) { Console.WriteLine($"Type {exportedType.FullName} hasn't been forwarded"); }
                    missingForwards.Add(exportedType.FullName);
                }
                else if(verbose) { Console.WriteLine($"Type {exportedType.FullName} has been forwarded"); }
            }
        }
        context.Dispose();

        if (verbose) { Console.WriteLine(); }
        if (missingForwards.Count > 0)
        {
            Console.WriteLine($"There are {missingForwards.Count} missing type forwards:");
            foreach (var missingForward in missingForwards)
            {
                Console.WriteLine($"[assembly: TypeForwardedTo(typeof({missingForward}))]");
            }
        }
        else
        {
            Console.WriteLine("All netstandard2.0 types have been type forwarded by the netfx implementation");
        }

        return 0;
    }
}
