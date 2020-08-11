using System;
using Microsoft.Tools.ServiceModel.Svcutil;
using System.Linq;
using System.Text.RegularExpressions;

namespace UpdateServiceRefOptionsExtraOptionsWarn
{
    class Program
    {
        static int Main(string[] args)
        {
            var re = new Regex(@"'[^\""]*'|[^\""^\s]+|""[^\""]*""");
            string optstring = @"-u -edb -nb -elm -v minimal";
            string[] opts = re.Matches(optstring).Cast<Match>().Select(m => m.Value).ToArray();
            return Tool.Main(opts);
        }
    }
}