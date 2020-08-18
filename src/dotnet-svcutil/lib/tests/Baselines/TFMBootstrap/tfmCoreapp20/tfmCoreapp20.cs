using System;
using Microsoft.Tools.ServiceModel.Svcutil;
using System.Linq;
using System.Text.RegularExpressions;

namespace tfmCoreapp20
{
    class Program
    {
        static int Main(string[] args)
        {
            var re = new Regex(@"'[^\""]*'|[^\""^\s]+|""[^\""]*""");
            string optstring = @"""$testCasesPath$\wsdl\simple.wsdl"" -tf netcoreapp2.0 -r ""{Newtonsoft.Json,*}"" -bd $resultPath$\TestResults\TFMBootstrap\tfmCoreapp20 -nl -tc global -v minimal -d ..\tfmCoreapp20 -n ""*,tfmCoreapp20_NS""";
            string[] opts = re.Matches(optstring).Cast<Match>().Select(m => m.Value).ToArray();
            return Tool.Main(opts);
        }
    }
}