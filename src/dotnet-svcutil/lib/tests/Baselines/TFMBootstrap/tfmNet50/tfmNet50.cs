using System;
using Microsoft.Tools.ServiceModel.Svcutil;
using System.Linq;
using System.Text.RegularExpressions;

namespace tfmNet50
{
    class Program
    {
        static int Main(string[] args)
        {
            var re = new Regex(@"'[^\""]*'|[^\""^\s]+|""[^\""]*""");
            string optstring = @"""$testCasesPath$/wsdl/Simple.wsdl"" -tf net5.0 -r ""{Newtonsoft.Json,*}"" -bd $resultPath$/TestResults/TFMBootstrap/tfmNet50 -nl -tc global -v minimal -d ../tfmNet50 -n ""*,tfmNet50_NS""";
            string[] opts = re.Matches(optstring).Cast<Match>().Select(m => m.Value).ToArray();
            return Tool.Main(opts);
        }
    }
}