//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
namespace Microsoft.Tools.ServiceModel.SvcUtil
{
    partial class Options
    {
        internal static class Cmd
        {
            internal const string Async = "async";
            internal const string Target = "target";
            internal const string Help = "help";
            internal const string NoLogo = "noLogo";
            internal const string Out = "out";
            internal const string Directory = "directory";
            internal const string Language = "language";
            internal const string Config = "config";
            internal const string NoConfig = "noConfig";
            internal const string MergeConfig = "mergeConfig";
            internal const string ToolConfig = "svcutilConfig";
            internal const string Reference = "reference";
            internal const string ServiceName = "serviceName";
            internal const string Nostdlib = "noStdLib";
            internal const string ExcludeType = "excludeType";
            internal const string CollectionType = "collectionType";
            internal const string Serializable = "serializable";
            internal const string Serializer = "serializer";
            internal const string Namespace = "namespace";
            internal const string Internal = "internal";
            internal const string MessageContract = "messageContract";
            internal const string ImportXmlTypes = "importXmlTypes";
            internal const string Validate = "validate";
            internal const string EnableDataBinding = "enableDataBinding";
            internal const string DataContractOnly = "dataContractOnly";
            internal const string TargetClientVersion = "targetClientVersion";
            internal const string UseSerializerForFaults = "useSerializerForFaults";
            internal const string Wrapped = "wrapped";
            internal const string ServiceContract = "serviceContract";
            internal const string SyncOnly = "syncOnly";
#if DEBUG
            internal const string Debug = "debug";
#endif
        }

        internal static class Abbr
        {
            internal const string Async = "a";
            internal const string Target = "t";
            internal const string Help = "?";
            internal const string Out = "o";
            internal const string Directory = "d";
            internal const string Language = "l";
            internal const string Reference = "r";
            internal const string ExcludeType = "et";
            internal const string CollectionType = "ct";
            internal const string Serializable = "s";
            internal const string Serializer = "ser";
            internal const string Internal = "i";
            internal const string MessageContract = "mc";
            internal const string Namespace = "n";
            internal const string ImportXmlTypes = "ixt";
            internal const string Validate = "v";
            internal const string EnableDataBinding = "edb";
            internal const string DataContractOnly = "dconly";
            internal const string TargetClientVersion = "tcv";
            internal const string UseSerializerForFaults = "fault";
            internal const string Wrapped = "wr";
            internal const string ServiceContract = "sc";
        }

        internal static class Targets
        {
            public const string Metadata = "metadata";
            public const string Code = "code";
            public const string XmlSerializer = "xmlSerializer";
            static readonly string[] targets = new string[] { Metadata, Code, XmlSerializer };
            public static readonly string SupportedTargets = string.Join(", ", targets);
        }

        internal static class Switches
        {
            public static readonly CommandSwitch Async = new CommandSwitch(Options.Cmd.Async, Abbr.Async, SwitchType.Flag);
            public static readonly CommandSwitch Directory = new CommandSwitch(Options.Cmd.Directory, Abbr.Directory, SwitchType.SingletonValue);
            public static readonly CommandSwitch Target = new CommandSwitch(Options.Cmd.Target, Abbr.Target, SwitchType.SingletonValue);
            public static readonly CommandSwitch Help = new CommandSwitch(Options.Cmd.Help, Abbr.Help, SwitchType.Flag);
            public static readonly CommandSwitch NoLogo = new CommandSwitch(Options.Cmd.NoLogo, Options.Cmd.NoLogo, SwitchType.Flag);
            public static readonly CommandSwitch NoConfig = new CommandSwitch(Options.Cmd.NoConfig, Options.Cmd.NoConfig, SwitchType.Flag);
            public static readonly CommandSwitch MergeConfig = new CommandSwitch(Options.Cmd.MergeConfig, Options.Cmd.MergeConfig, SwitchType.Flag);
            public static readonly CommandSwitch Out = new CommandSwitch(Options.Cmd.Out, Abbr.Out, SwitchType.SingletonValue);
            public static readonly CommandSwitch Language = new CommandSwitch(Options.Cmd.Language, Abbr.Language, SwitchType.SingletonValue);
            public static readonly CommandSwitch Config = new CommandSwitch(Options.Cmd.Config, Options.Cmd.Config, SwitchType.SingletonValue);
            public static readonly CommandSwitch ToolConfig = new CommandSwitch(Options.Cmd.ToolConfig, Options.Cmd.ToolConfig, SwitchType.SingletonValue);
            public static readonly CommandSwitch Reference = new CommandSwitch(Options.Cmd.Reference, Abbr.Reference, SwitchType.ValueList);
            public static readonly CommandSwitch ServiceName = new CommandSwitch(Options.Cmd.ServiceName, Cmd.ServiceName, SwitchType.SingletonValue);
            public static readonly CommandSwitch Nostdlib = new CommandSwitch(Options.Cmd.Nostdlib, Options.Cmd.Nostdlib, SwitchType.Flag);
            public static readonly CommandSwitch ExcludeType = new CommandSwitch(Options.Cmd.ExcludeType, Abbr.ExcludeType, SwitchType.ValueList);
            public static readonly CommandSwitch CollectionType = new CommandSwitch(Options.Cmd.CollectionType, Abbr.CollectionType, SwitchType.ValueList);
            public static readonly CommandSwitch Serializable = new CommandSwitch(Options.Cmd.Serializable, Abbr.Serializable, SwitchType.Flag);
            public static readonly CommandSwitch Serializer = new CommandSwitch(Options.Cmd.Serializer, Abbr.Serializer, SwitchType.SingletonValue);
            public static readonly CommandSwitch Namespace = new CommandSwitch(Options.Cmd.Namespace, Abbr.Namespace, SwitchType.ValueList);
            public static readonly CommandSwitch Internal = new CommandSwitch(Options.Cmd.Internal, Abbr.Internal, SwitchType.Flag);
            public static readonly CommandSwitch MessageContract = new CommandSwitch(Options.Cmd.MessageContract, Abbr.MessageContract, SwitchType.Flag);
            public static readonly CommandSwitch ImportXmlTypes = new CommandSwitch(Options.Cmd.ImportXmlTypes, Abbr.ImportXmlTypes, SwitchType.Flag);
            public static readonly CommandSwitch Validate = new CommandSwitch(Options.Cmd.Validate, Abbr.Validate, SwitchType.Flag);
#if DEBUG
            public static readonly CommandSwitch Debug = new CommandSwitch(Options.Cmd.Debug, Options.Cmd.Debug, SwitchType.Flag);
#endif
            public static readonly CommandSwitch EnableDataBinding = new CommandSwitch(Options.Cmd.EnableDataBinding, Abbr.EnableDataBinding, SwitchType.Flag);
            public static readonly CommandSwitch DataContractOnly = new CommandSwitch(Options.Cmd.DataContractOnly, Abbr.DataContractOnly, SwitchType.Flag);
            public static readonly CommandSwitch TargetClientVersion = new CommandSwitch(Options.Cmd.TargetClientVersion, Abbr.TargetClientVersion, SwitchType.SingletonValue);
            public static readonly CommandSwitch UseSerializerForFaults = new CommandSwitch(Options.Cmd.UseSerializerForFaults, Abbr.UseSerializerForFaults, SwitchType.Flag);
            public static readonly CommandSwitch Wrapped = new CommandSwitch(Options.Cmd.Wrapped, Abbr.Wrapped, SwitchType.Flag);
            public static readonly CommandSwitch ServiceContract = new CommandSwitch(Options.Cmd.ServiceContract, Abbr.ServiceContract, SwitchType.Flag);
            public static readonly CommandSwitch SyncOnly = new CommandSwitch(Options.Cmd.SyncOnly, Options.Cmd.SyncOnly, SwitchType.Flag);

            public static readonly CommandSwitch[] All = new CommandSwitch[] { Async, Directory, Target, Help, NoLogo, NoConfig, MergeConfig, Out, Language, Config,
                                                                        ToolConfig, Reference, ServiceName, Nostdlib, ExcludeType, CollectionType, Serializable,
                                                                        Serializer, Namespace, Internal, MessageContract, ImportXmlTypes, Validate, 
#if DEBUG
                                                                        Debug,
#endif
                                                                        EnableDataBinding, DataContractOnly, TargetClientVersion, UseSerializerForFaults, Wrapped,
                                                                        ServiceContract, SyncOnly };

        }

        static readonly string SupportedSerializers = string.Join(", ", System.Enum.GetNames(typeof(SerializerMode)));
        static readonly string SupportedTargetClientVersions = string.Join(", ", System.Enum.GetNames(typeof(TargetClientVersionMode)));
    }

    enum SerializerMode
    {
        Default,
        Auto,
        DataContractSerializer,
        XmlSerializer
    }

    enum TargetClientVersionMode
    {
        Version30,
        Version35
    }
}
