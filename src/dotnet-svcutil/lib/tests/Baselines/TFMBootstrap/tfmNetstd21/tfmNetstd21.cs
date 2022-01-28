using System;
using Microsoft.Tools.ServiceModel.Svcutil;
using System.Linq;
using System.Text.RegularExpressions;

namespace tfmNetstd21
{
    class Program
    {
        static int Main(string[] args)
        {
            var re = new Regex(@"'[^\""]*'|[^\""^\s]+|""[^\""]*""");
            string optstring = @"""$testCasesPath$/wsdl/Simple.wsdl"" -tf netstandard2.1 -r ""{Newtonsoft.Json,*}"" -bd $resultPath$/TestResults/TFMBootstrap/tfmNetstd21 -nl -tc global -v minimal -d ../tfmNetstd21 -n ""*,tfmNetstd21_NS""";
            string[] opts = re.Matches(optstring).Cast<Match>().Select(m => m.Value).ToArray();
            return Tool.Main(opts);
        }
    }
}