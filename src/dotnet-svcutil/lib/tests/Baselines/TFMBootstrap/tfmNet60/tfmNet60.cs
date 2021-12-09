using System;
using Microsoft.Tools.ServiceModel.Svcutil;
using System.Linq;
using System.Text.RegularExpressions;

namespace tfmNet60
{
    class Program
    {
        static int Main(string[] args)
        {
            var re = new Regex(@"'[^\""]*'|[^\""^\s]+|""[^\""]*""");
            string optstring = @"""$testCasesPath$/wsdl/Simple.wsdl"" -tf net6.0 -r ""{Newtonsoft.Json,*}"" -bd $resultPath$/TestResults/TFMBootstrap/tfmNet60 -nl -tc global -v minimal -d ../tfmNet60 -n ""*,tfmNet60_NS""";
            string[] opts = re.Matches(optstring).Cast<Match>().Select(m => m.Value).ToArray();
            return Tool.Main(opts);
        }
    }
}