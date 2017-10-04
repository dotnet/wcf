using System;
using System.Collections.Generic;
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
            Tool.Main(new string[] { "/t:xmlserializer", Assembly.GetExecutingAssembly().Location});
        }
    }
}
