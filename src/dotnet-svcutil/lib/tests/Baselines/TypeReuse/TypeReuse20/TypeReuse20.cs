using System;
using Microsoft.Tools.ServiceModel.Svcutil;
using System.Linq;
using System.Text.RegularExpressions;

namespace TypeReuse20
{
    class Program
    {
        static int Main(string[] args)
        {
            var re = new Regex(@"'[^\""]*'|[^\""^\s]+|""[^\""]*""");
            string optstring = @"../TypeReuseSvc.wsdl -nl -v minimal -d $resultPath$/TestResults/TypeReuse/TypeReuse20/ServiceReference -n ""*,TypeReuse20_NS"" -bd $resultPath$/TestResults/TypeReuse/TypeReuse20";
            string[] opts = re.Matches(optstring).Cast<Match>().Select(m => m.Value).ToArray();
            return Tool.Main(opts);
        }
    }
}