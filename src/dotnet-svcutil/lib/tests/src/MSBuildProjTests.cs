// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Tools.ServiceModel.Svcutil;
using Xunit;

namespace SvcutilTest
{
    public class MSBuildProjTests
    {
        private readonly string _testDir = ClassFixture.FindRepositoryRoot() + "/artifacts/TestOutput";
        public string TestDir
        {
            get
            {
                if (!Directory.Exists(_testDir))
                {
                    Directory.CreateDirectory(_testDir);
                }

                return _testDir;
            }
        }

        [Fact]
        public async Task ParseAsync_ValidProjectWithPackageReference_ShouldParseSuccessfully()
        {
            var projectText = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFrameworks>net6.0</TargetFrameworks>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include=""xunit"" Version=""2.4.1"" />
  </ItemGroup>
</Project>";

            var build = await MSBuildProj.ParseAsync(projectText, TestDir + "/MSBuildProjTests/Sample", null, CancellationToken.None);

            Assert.NotNull(build);
            Assert.NotEmpty(build.Dependencies);
            Assert.Contains(build.Dependencies, x => x.Name == "xunit" && x.Version == "2.4.1");
        }

        [Fact]
        public async Task ParseAsync_ProjectWithUpdatePackageReference_ShouldSkipUpdatePackageAndParseRestOfProjectSuccessfully()
        {
            var projectText = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFrameworks>netcoreapp2.1;netcoreapp3.0</TargetFrameworks>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include=""xunit"" Version=""2.4.1"" />
    <PackageReference Update=""NuGet.Versioning"" Version=""3.5.0"" />
  </ItemGroup>
</Project>";

            var build = await MSBuildProj.ParseAsync(projectText, TestDir + "/MSBuildProjTests/Sample", null, CancellationToken.None);

            Assert.NotNull(build);
            Assert.NotEmpty(build.Dependencies);
            Assert.Contains(build.Dependencies, x => x.Name == "xunit" && x.Version == "2.4.1");
        }

        [Fact]
        public async Task ParseAsync_ValidProjectWithDotNetCliToolReference_ShouldParseSuccessfully()
        {
            var projectText = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFrameworks>netcoreapp2.1;netcoreapp3.0</TargetFrameworks>
  </PropertyGroup>
  <ItemGroup>
    <DotNetCliToolReference Include=""xunit"" Version=""2.4.1"" />
  </ItemGroup>
</Project>";

            var build = await MSBuildProj.ParseAsync(projectText, TestDir + "/MSBuildProjTests/Sample", null, CancellationToken.None);

            Assert.NotNull(build);
            Assert.NotEmpty(build.Dependencies);
            Assert.Contains(build.Dependencies, x => x.Name == "xunit" && x.Version == "2.4.1");
        }

        [Fact]
        public async Task ParseAsync_ProjectWithUpdateDotNetCliToolReference_ShouldSkipUpdateReferencendParseRestOfProjectSuccessfully()
        {
            var projectText = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFrameworks>netcoreapp2.1;netcoreapp3.0</TargetFrameworks>
  </PropertyGroup>
  <ItemGroup>
    <DotNetCliToolReference Include=""xunit"" Version=""2.4.1"" />
    <DotNetCliToolReference Update=""NuGet.Versioning"" Version=""3.5.0"" />
  </ItemGroup>
</Project>";

            var build = await MSBuildProj.ParseAsync(projectText, TestDir + "/MSBuildProjTests/Sample", null, CancellationToken.None);

            Assert.NotNull(build);
            Assert.NotEmpty(build.Dependencies);
            Assert.Contains(build.Dependencies, x => x.Name == "xunit" && x.Version == "2.4.1");
        }
    }
}
