using System;
using Microsoft.Tools.ServiceModel.Svcutil;
using System.Linq;
using System.Text.RegularExpressions;

namespace tfmNetstd20
{
    class Program
    {
        static int Main(string[] args)
        {
            var re = new Regex(@"'[^\""]*'|[^\""^\s]+|""[^\""]*""");
            string optstring = @"""$testCasesPath$/wsdl/Simple.wsdl"" -tf netstandard2.0 -r ""{Newtonsoft.Json,*}"" -bd $resultPath$/TestResults/TFMBootstrap/tfmNetstd20 -nl -tc global -v minimal -d ../tfmNetstd20 -n ""*,tfmNetstd20_NS""";
            string[] opts = re.Matches(optstring).Cast<Match>().Select(m => m.Value).ToArray();
            return Tool.Main(opts);
        }
    }
}