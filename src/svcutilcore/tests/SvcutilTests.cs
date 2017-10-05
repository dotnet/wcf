using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.Tools.ServiceModel.SvcUtil.Tests
{
    public static class SvcutilTests
    {
        [Fact]
        public static void SvcutilTest1()
        {
            string outputFile = Path.GetTempFileName() + ".cs";
            Assert.False(File.Exists(outputFile));

            Tool.Main(new string[] { "/t:xmlserializer", Assembly.GetExecutingAssembly().Location, $"/out:{outputFile}"});
            Assert.True(File.Exists(outputFile));

            File.Delete(outputFile);
        }
    }
}
