// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Text;

namespace System.Private.ServiceModel.Tests.Tools
{
    internal class CommandLineArgs
    {
        private const string HelpTextFormatString = "{0}  {1, -15}{2}";

        private const string VerboseArgumentString = "--verbose";
        public bool Verbose { get; private set; }

        private const string HelpArgumentString1 = "-?";
        private const string HelpArgumentString2 = "--help";
        public bool Help { get; private set; }

        private const string PreviewArgumentString = "--preview";
        public bool Preview { get; private set; }

        private string _invalidArgument = string.Empty;

        public CommandLineArgs(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (string.Compare(HelpArgumentString1, args[i], StringComparison.OrdinalIgnoreCase) == 0 ||
                    string.Compare(HelpArgumentString2, args[i], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    Help = true;
                }
                else if (string.Compare(PreviewArgumentString, args[i], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    Preview = true;
                }
                else if (string.Compare(VerboseArgumentString, args[i], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    Verbose = true;
                }
                else
                {
                    _invalidArgument = args[i];
                    Help = true;
                    break;
                }
            }
        }

        public string HelpText
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                if (!string.IsNullOrWhiteSpace(_invalidArgument))
                {
                    builder.AppendFormat("{0}  Invalid argument specified: {1}", Environment.NewLine, _invalidArgument);
                    builder.AppendFormat(Environment.NewLine);
                }

                builder.AppendFormat(HelpTextFormatString, Environment.NewLine, HelpArgumentString1, string.Empty);
                builder.AppendFormat(HelpTextFormatString, Environment.NewLine, HelpArgumentString2, "Displays this help text");
                builder.AppendFormat(HelpTextFormatString, Environment.NewLine, PreviewArgumentString, "Preview mode - does not actually delete certs");
                builder.AppendFormat(HelpTextFormatString, Environment.NewLine, VerboseArgumentString, "Verbose mode - display all exception messages that are otherwise suppressed");
                builder.AppendFormat(Environment.NewLine);
                return builder.ToString();
            }
        }
    }
}
