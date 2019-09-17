// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.Tools.ServiceModel.SvcUtil.XmlSerializer.IntegrationTests
{
    public static class SvcutilTests
    {
        [Fact]
        public static void SvcutilTest1()
        {
            string outputFile = Path.GetTempFileName() + ".cs";
            Assert.False(File.Exists(outputFile));
            string testassemblypath = Assembly.GetExecutingAssembly().Location;
            string smassemblypath = testassemblypath.Replace("dotnet-svcutil.xmlserializer.IntegrationTests", "System.ServiceModel.Primitives").Replace("Microsoft.Tools.ServiceModel.SvcUtil.XmlSerializer.IntegrationTests.dll", "System.ServiceModel.Primitives.dll").Replace("netcoreapp", "netstandard");
            if (File.Exists(smassemblypath))
            {
                Tool.Main(new string[] { Assembly.GetExecutingAssembly().Location, $"--out:{outputFile}", $"--smreference:{smassemblypath}", $"--quiet"});
                Assert.True(File.Exists(outputFile));
                File.Delete(outputFile);
            }
        }
    }
}
