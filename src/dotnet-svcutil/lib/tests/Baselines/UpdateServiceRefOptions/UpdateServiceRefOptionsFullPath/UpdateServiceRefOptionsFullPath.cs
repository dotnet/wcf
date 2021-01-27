using System;
using Microsoft.Tools.ServiceModel.Svcutil;
using System.Linq;
using System.Text.RegularExpressions;

namespace UpdateServiceRefOptionsFullPath
{
    class Program
    {
        static int Main(string[] args)
        {
            var re = new Regex(@"'[^\""]*'|[^\""^\s]+|""[^\""]*""");
            string optstring = @"-u $resultPath$/TestResults/UpdateServiceRefOptions/UpdateServiceRefOptionsFullPath/UpdateServiceRefOptionsFullPath/dotnet-svcutil.params.json -v minimal";
            string[] opts = re.Matches(optstring).Cast<Match>().Select(m => m.Value).ToArray();
            return Tool.Main(opts);
        }
    }
}