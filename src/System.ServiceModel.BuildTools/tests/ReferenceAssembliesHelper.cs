// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Testing;

namespace System.ServiceModel.BuildTools.Tests;

internal static class ReferenceAssembliesHelper
{
    public static readonly Lazy<ReferenceAssemblies> Default = new(() =>
    {
        var packages = new[]
        {
            new PackageIdentity("System.Security.Cryptography.Xml", "6.0.1"),
            new PackageIdentity("Microsoft.Extensions.ObjectPool", "6.0.16"),
        }.ToImmutableArray();
        return ReferenceAssemblies.Default.AddPackages(packages);
    });
}
