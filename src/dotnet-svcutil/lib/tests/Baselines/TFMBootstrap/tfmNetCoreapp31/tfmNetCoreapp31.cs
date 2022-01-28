using System;
using Microsoft.Tools.ServiceModel.Svcutil;
using System.Linq;
using System.Text.RegularExpressions;

namespace tfmNetCoreapp31
{
    class Program
    {
        static int Main(string[] args)
        {
            var re = new Regex(@"'[^\""]*'|[^\""^\s]+|""[^\""]*""");
            string optstring = @"""$testCasesPath$/wsdl/Simple.wsdl"" -tf netcoreapp3.1 -r ""{Newtonsoft.Json,*}"" -bd $resultPath$/TestResults/TFMBootstrap/tfmNetCoreapp31 -nl -tc global -v minimal -d ../tfmNetCoreapp31 -n ""*,tfmNetCoreapp31_NS""";
            string[] opts = re.Matches(optstring).Cast<Match>().Select(m => m.Value).ToArray();
            return Tool.Main(opts);
        }
    }
}