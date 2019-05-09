// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Tools.ServiceModel.SvcUtil.XmlSerializer
{
    internal partial class Options
    {
        internal static class Cmd
        {
            internal const string Target = "target";
            internal const string Help = "help";
            internal const string NoLogo = "noLogo";
            internal const string Out = "out";
            internal const string Directory = "directory";
            internal const string Reference = "reference";
            internal const string SMReference = "smreference";
            internal const string Nostdlib = "noStdLib";
            internal const string ExcludeType = "excludeType";
            internal const string CollectionType = "collectionType";
            internal const string Namespace = "namespace";
            internal const string Quiet = "quiet";
#if DEBUG
            internal const string Debug = "debug";
#endif
        }

        internal static class Abbr
        {
            internal const string Target = "t";
            internal const string Help = "?";
            internal const string Out = "o";
            internal const string Directory = "d";
            internal const string Reference = "r";
            internal const string SMReference = "sr";
            internal const string ExcludeType = "et";
            internal const string CollectionType = "ct";
            internal const string Namespace = "n";
            internal const string Quiet = "q";
        }

        internal static class Targets
        {
            public const string Metadata = "metadata";
            public const string Code = "code";
            public const string XmlSerializer = "xmlSerializer";
            private static readonly string[] s_targets = new string[] { Metadata, Code, XmlSerializer };
            public static readonly string SupportedTargets = string.Join(", ", s_targets);
        }

        internal static class Switches
        {
            public static readonly CommandSwitch Directory = new CommandSwitch(Options.Cmd.Directory, Abbr.Directory, SwitchType.SingletonValue);
            public static readonly CommandSwitch Help = new CommandSwitch(Options.Cmd.Help, Abbr.Help, SwitchType.Flag);
            public static readonly CommandSwitch NoLogo = new CommandSwitch(Options.Cmd.NoLogo, Options.Cmd.NoLogo, SwitchType.Flag);
            public static readonly CommandSwitch Out = new CommandSwitch(Options.Cmd.Out, Abbr.Out, SwitchType.SingletonValue);
            public static readonly CommandSwitch Reference = new CommandSwitch(Options.Cmd.Reference, Abbr.Reference, SwitchType.ValueList);
            public static readonly CommandSwitch SMReference = new CommandSwitch(Options.Cmd.SMReference, Abbr.SMReference, SwitchType.ValueList);
            public static readonly CommandSwitch Nostdlib = new CommandSwitch(Options.Cmd.Nostdlib, Options.Cmd.Nostdlib, SwitchType.Flag);
            public static readonly CommandSwitch ExcludeType = new CommandSwitch(Options.Cmd.ExcludeType, Abbr.ExcludeType, SwitchType.ValueList);
            public static readonly CommandSwitch Namespace = new CommandSwitch(Options.Cmd.Namespace, Abbr.Namespace, SwitchType.ValueList);
            public static readonly CommandSwitch Quiet = new CommandSwitch(Options.Cmd.Quiet, Abbr.Quiet, SwitchType.Flag);
#if DEBUG
            public static readonly CommandSwitch Debug = new CommandSwitch(Options.Cmd.Debug, Options.Cmd.Debug, SwitchType.Flag);
#endif
            public static readonly CommandSwitch[] All = new CommandSwitch[] { Directory, Help, NoLogo, Out,
                                                                        Nostdlib, ExcludeType, Namespace, Reference, SMReference, Quiet,
#if DEBUG
                                                                        Debug,
#endif
                                                                        };
        }

    }
}
