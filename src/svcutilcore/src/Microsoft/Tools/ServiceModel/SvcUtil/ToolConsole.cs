//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
namespace Microsoft.Tools.ServiceModel.SvcUtil
{
    using System;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.IO;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.CodeDom.Compiler;
    using System.ServiceModel;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Reflection;
    using System.Globalization;

    static class ToolConsole
    {

#if DEBUG
        static bool debug = false;
#endif

#if DEBUG
        internal static void SetOptions(bool debug)
        {
            ToolConsole.debug = debug;
        }
#endif

        internal static void WriteLine(string str, int indent)
        {
            ToolStringBuilder toolStringBuilder = new ToolStringBuilder(indent);
            toolStringBuilder.WriteParagraph(str);
        }

        internal static void WriteLine(string str)
        {
            Console.WriteLine(str);
        }

        internal static void WriteLine()
        {
            Console.WriteLine();
        }

        internal static void WriteError(string errMsg)
        {
            WriteError(errMsg, SR.Format(SR.Error));
        }

        static void WriteError(Exception e)
        {
            WriteError(e, SR.Format(SR.Error));
        }

        internal static void WriteUnexpectedError(Exception e)
        {
            WriteError(SR.Format(SR.ErrUnexpectedError));
            WriteError(e);
        }

        internal static void WriteInvalidDataContractError(InvalidDataContractException e)
        {
            WriteError(e);

            ToolConsole.WriteLine();
            ToolConsole.WriteLine(SR.Format(SR.HintConsiderUseXmlSerializer, Options.Cmd.DataContractOnly,
                Options.Cmd.ImportXmlTypes));
        }

        internal static void WriteUnexpectedError(string errMsg)
        {
            WriteError(SR.Format(SR.ErrUnexpectedError));
            if (!string.IsNullOrEmpty(errMsg))
                WriteError(errMsg);
        }

        internal static void WriteToolError(ToolArgumentException ae)
        {
            WriteError(ae);

            ToolMexException me = ae as ToolMexException;
            if (me != null)
            {
                string serviceUri = me.ServiceUri.AbsoluteUri;

                ToolConsole.WriteLine();
                ToolConsole.WriteError(SR.Format(SR.WrnWSMExFailed, serviceUri), string.Empty);
                ToolConsole.WriteError(me.WSMexException, "    ");

                if (me.HttpGetException != null)
                {
                    ToolConsole.WriteLine();
                    ToolConsole.WriteError(SR.Format(SR.WrnHttpGetFailed, serviceUri), string.Empty);
                    ToolConsole.WriteError(me.HttpGetException, "    ");
                }
            }

            ToolConsole.WriteLine(SR.Format(SR.MoreHelp, Options.Abbr.Help));
        }

        internal static void WriteToolError(ToolRuntimeException re)
        {
            WriteError(re);
        }

        internal static void WriteWarning(string message)
        {
            Console.Error.Write(SR.Format(SR.Warning));
            Console.Error.WriteLine(message);
            Console.Error.WriteLine();
        }

        static void WriteError(string errMsg, string prefix)
        {
            Console.Error.Write(prefix);
            Console.Error.WriteLine(errMsg);
            Console.Error.WriteLine();
        }

        internal static void WriteError(Exception e, string prefix)
        {

#if DEBUG
            if (debug)
            {
                ToolConsole.WriteLine();
                WriteError(e.ToString(), prefix);
                return;
            }
#endif

            WriteError(e.Message, prefix);

            while (e.InnerException != null)
            {
                if (e.Message != e.InnerException.Message)
                {
                    WriteError(e.InnerException.Message, "    ");
                }
                e = e.InnerException;
            }

        }

        internal static void WriteHeader()
        {
            // Using CommonResStrings.WcfTrademarkForCmdLine for the trademark: the proper resource for command line tools.
            ToolConsole.WriteLine(SR.Format(SR.Logo, SR.WcfTrademarkForCmdLine, ThisAssembly.InformationalVersion, SR.CopyrightForCmdLine));
        }

        internal static void WriteHelpText()
        {
            HelpGenerator.WriteUsage();
            ToolConsole.WriteLine();
            ToolConsole.WriteLine();
            HelpGenerator.WriteCommonOptionsHelp();
            ToolConsole.WriteLine();
            ToolConsole.WriteLine();
            HelpGenerator.WriteXmlSerializerTypeGenerationHelp();
            ToolConsole.WriteLine();
            ToolConsole.WriteLine();
            HelpGenerator.WriteExamples();
            ToolConsole.WriteLine();
            ToolConsole.WriteLine();
        }

        static class HelpGenerator
        {
            static ToolStringBuilder exampleBuilder = new ToolStringBuilder(4);
            static HelpGenerator() { } // beforefeildInit

            internal static void WriteUsage()
            {
                ToolConsole.WriteLine(SR.Format(SR.HelpUsage1));
                ToolConsole.WriteLine();
                ToolConsole.WriteLine(SR.Format(SR.HelpUsage6));
            }

            internal static void WriteXmlSerializerTypeGenerationHelp()
            {
                HelpCategory helpCategory = new HelpCategory(SR.Format(SR.HelpXmlSerializerTypeGenerationCategory));

                helpCategory.Description = SR.Format(SR.HelpXmlSerializerTypeGenerationDescription, ThisAssembly.Title);
                helpCategory.Syntax = SR.Format(SR.HelpXmlSerializerTypeGenerationSyntax, ThisAssembly.Title, Options.Abbr.Target, Options.Targets.XmlSerializer, SR.Format(SR.HelpInputAssemblyPath));

                helpCategory.Inputs = new ArgumentInfo[1];

                helpCategory.Inputs[0] = ArgumentInfo.CreateInputHelpInfo(SR.Format(SR.HelpInputAssemblyPath));
                helpCategory.Inputs[0].HelpText = SR.Format(SR.HelpXmlSerializerTypeGenerationSyntaxInput1);

                helpCategory.Options = new ArgumentInfo[3];

                helpCategory.Options[0] = ArgumentInfo.CreateParameterHelpInfo(Options.Cmd.Reference, SR.Format(SR.ParametersReference));
                helpCategory.Options[0].HelpText = SR.Format(SR.HelpXmlSerializerTypeGenerationSyntaxInput2, Options.Abbr.Reference);

                helpCategory.Options[1] = ArgumentInfo.CreateParameterHelpInfo(Options.Cmd.ExcludeType, SR.Format(SR.ParametersExcludeType));
                helpCategory.Options[1].HelpText = SR.Format(SR.HelpXmlSerializerTypeGenerationSyntaxInput3, Options.Cmd.DataContractOnly, Options.Abbr.ExcludeType);

                helpCategory.Options[2] = ArgumentInfo.CreateParameterHelpInfo(Options.Cmd.Out, SR.Format(SR.ParametersOut));
                helpCategory.Options[2].HelpText = SR.Format(SR.HelpXmlSerializerTypeGenerationSyntaxInput4, Options.Abbr.Out);

                helpCategory.WriteHelp();
            }

            internal static void WriteMetadataDownloadHelp()
            {
                HelpCategory helpCategory = new HelpCategory(SR.Format(SR.HelpMetadataDownloadCategory));

                helpCategory.Description = SR.Format(SR.HelpMetadataDownloadDescription, ThisAssembly.Title, Options.Abbr.Target, Options.Targets.Metadata, Options.Cmd.ToolConfig);
                helpCategory.Syntax = SR.Format(SR.HelpMetadataDownloadSyntax, ThisAssembly.Title, Options.Abbr.Target, Options.Targets.Metadata, SR.Format(SR.HelpInputUrl), SR.Format(SR.HelpInputEpr));

                helpCategory.Inputs = new ArgumentInfo[2];

                helpCategory.Inputs[0] = ArgumentInfo.CreateInputHelpInfo(SR.Format(SR.HelpInputUrl));
                helpCategory.Inputs[0].HelpText = SR.Format(SR.HelpMetadataDownloadSyntaxInput1);

                helpCategory.Inputs[1] = ArgumentInfo.CreateInputHelpInfo(SR.Format(SR.HelpInputEpr));
                helpCategory.Inputs[1].HelpText = SR.Format(SR.HelpMetadataDownloadSyntaxInput2);

                helpCategory.WriteHelp();
            }

            internal static void WriteMetadataExportHelp()
            {
                HelpCategory helpCategory = new HelpCategory(SR.Format(SR.HelpMetadataExportCategory));

                helpCategory.Description = SR.Format(SR.HelpMetadataExportDescription, ThisAssembly.Title, Options.Cmd.ServiceName, Options.Cmd.DataContractOnly);
                helpCategory.Syntax = SR.Format(SR.HelpMetadataExportSyntax, ThisAssembly.Title, Options.Abbr.Target, Options.Targets.Metadata,
                    Options.Cmd.ServiceName, SR.Format(SR.ParametersServiceName), Options.Cmd.DataContractOnly, SR.Format(SR.HelpInputAssemblyPath));

                helpCategory.Inputs = new ArgumentInfo[1];

                helpCategory.Inputs[0] = ArgumentInfo.CreateInputHelpInfo(SR.Format(SR.HelpInputAssemblyPath));
                helpCategory.Inputs[0].HelpText = SR.Format(SR.HelpMetadataExportSyntaxInput1);

                helpCategory.Options = new ArgumentInfo[4];

                helpCategory.Options[0] = ArgumentInfo.CreateParameterHelpInfo(Options.Cmd.ServiceName, SR.Format(SR.ParametersServiceName));
                helpCategory.Options[0].HelpText = SR.Format(SR.HelpServiceNameExport, Options.Abbr.Reference);

                helpCategory.Options[1] = ArgumentInfo.CreateParameterHelpInfo(Options.Cmd.Reference, SR.Format(SR.ParametersReference));
                helpCategory.Options[1].HelpText = SR.Format(SR.HelpReferenceOther, Options.Abbr.Reference);

                helpCategory.Options[2] = ArgumentInfo.CreateFlagHelpInfo(Options.Cmd.DataContractOnly);
                helpCategory.Options[2].HelpText = SR.Format(SR.HelpDataContractOnly, Options.Abbr.DataContractOnly);

                helpCategory.Options[3] = ArgumentInfo.CreateParameterHelpInfo(Options.Cmd.ExcludeType, SR.Format(SR.ParametersExcludeType));
                helpCategory.Options[3].HelpText = SR.Format(SR.HelpExcludeTypeExport, Options.Abbr.ExcludeType, Options.Abbr.DataContractOnly);

                helpCategory.WriteHelp();
            }

            internal static void WriteValidationHelp()
            {
                HelpCategory helpCategory = new HelpCategory(SR.Format(SR.HelpValidationCategory));

                helpCategory.Description = SR.Format(SR.HelpValidationDescription, Options.Cmd.ServiceName);
                helpCategory.Syntax = SR.Format(SR.HelpValidationSyntax, ThisAssembly.Title, Options.Cmd.Validate,
                    Options.Cmd.ServiceName, SR.Format(SR.ParametersServiceName), SR.Format(SR.HelpInputAssemblyPath));

                helpCategory.Inputs = new ArgumentInfo[1];

                helpCategory.Inputs[0] = ArgumentInfo.CreateInputHelpInfo(SR.Format(SR.HelpInputAssemblyPath));
                helpCategory.Inputs[0].HelpText = SR.Format(SR.HelpValidationSyntaxInput1);

                helpCategory.Options = new ArgumentInfo[5];

                helpCategory.Options[0] = ArgumentInfo.CreateFlagHelpInfo(Options.Cmd.Validate);
                helpCategory.Options[0].HelpText = SR.Format(SR.HelpValidate, Options.Cmd.ServiceName, Options.Abbr.Validate);

                helpCategory.Options[1] = ArgumentInfo.CreateParameterHelpInfo(Options.Cmd.ServiceName, SR.Format(SR.ParametersServiceName));
                helpCategory.Options[1].HelpText = SR.Format(SR.HelpServiceNameValidate, Options.Abbr.Reference);

                helpCategory.Options[2] = ArgumentInfo.CreateParameterHelpInfo(Options.Cmd.Reference, SR.Format(SR.ParametersReference));
                helpCategory.Options[2].HelpText = SR.Format(SR.HelpReferenceOther, Options.Abbr.Reference);

                helpCategory.Options[3] = ArgumentInfo.CreateFlagHelpInfo(Options.Cmd.DataContractOnly);
                helpCategory.Options[3].HelpText = SR.Format(SR.HelpDataContractOnly, Options.Abbr.DataContractOnly);

                helpCategory.Options[4] = ArgumentInfo.CreateParameterHelpInfo(Options.Cmd.ExcludeType, SR.Format(SR.ParametersExcludeType));
                helpCategory.Options[4].HelpText = SR.Format(SR.HelpValidationExcludeTypeExport, Options.Abbr.ExcludeType, Options.Abbr.DataContractOnly);

                helpCategory.WriteHelp();
            }

            internal static void WriteCommonOptionsHelp()
            {
                HelpCategory helpCategory = new HelpCategory(SR.Format(SR.HelpCommonOptionsCategory));
                var options = new List<ArgumentInfo>();
                ArgumentInfo option;
                option = ArgumentInfo.CreateParameterHelpInfo(Options.Cmd.Target, SR.Format(SR.ParametersOutputType));
                option.HelpText = SR.Format(SR.HelpTargetOutputType, Options.Targets.Code, Options.Targets.Metadata, Options.Targets.XmlSerializer);
                options.Add(option);

                option = ArgumentInfo.CreateParameterHelpInfo(Options.Cmd.Directory, SR.Format(SR.ParametersDirectory));
                option.HelpText = SR.Format(SR.HelpDirectory, Options.Abbr.Directory);
                options.Add(option);

                option = ArgumentInfo.CreateFlagHelpInfo(Options.Cmd.NoLogo);
                option.HelpText = SR.Format(SR.HelpNologo);
                options.Add(option);

                option = ArgumentInfo.CreateFlagHelpInfo(Options.Cmd.Help);
                option.HelpText = SR.Format(SR.HelpHelp, Options.Abbr.Help);
                options.Add(option);

                helpCategory.Options = options.ToArray();
                helpCategory.WriteHelp();
            }

            internal static void WriteCodeGenerationHelp()
            {
                HelpCategory helpCategory = new HelpCategory(SR.Format(SR.HelpCodeGenerationCategory));

                helpCategory.Description = SR.Format(SR.HelpCodeGenerationDescription, ThisAssembly.Title);
                helpCategory.Syntax = SR.Format(SR.HelpCodeGenerationSyntax, ThisAssembly.Title, Options.Abbr.Target, Options.Targets.Code,
                    SR.Format(SR.HelpInputMetadataDocumentPath), SR.Format(SR.HelpInputUrl), SR.Format(SR.HelpInputEpr));

                helpCategory.Inputs = new ArgumentInfo[3];

                helpCategory.Inputs[0] = ArgumentInfo.CreateInputHelpInfo(SR.Format(SR.HelpInputMetadataDocumentPath));
                helpCategory.Inputs[0].HelpText = SR.Format(SR.HelpCodeGenerationSyntaxInput1);

                helpCategory.Inputs[1] = ArgumentInfo.CreateInputHelpInfo(SR.Format(SR.HelpInputUrl));
                helpCategory.Inputs[1].HelpText = SR.Format(SR.HelpCodeGenerationSyntaxInput2);

                helpCategory.Inputs[2] = ArgumentInfo.CreateInputHelpInfo(SR.Format(SR.HelpInputEpr));
                helpCategory.Inputs[2].HelpText = SR.Format(SR.HelpCodeGenerationSyntaxInput3);

                helpCategory.Options = new ArgumentInfo[26];

                helpCategory.Options[0] = ArgumentInfo.CreateParameterHelpInfo(Options.Cmd.Out, SR.Format(SR.ParametersOut));
                helpCategory.Options[0].HelpText = SR.Format(SR.HelpOut, Options.Abbr.Out);

                helpCategory.Options[1] = ArgumentInfo.CreateParameterHelpInfo(Options.Cmd.Config, SR.Format(SR.ParametersConfig));
                helpCategory.Options[1].HelpText = SR.Format(SR.HelpConfig);

                helpCategory.Options[2] = ArgumentInfo.CreateFlagHelpInfo(Options.Cmd.MergeConfig);
                helpCategory.Options[2].HelpText = SR.Format(SR.HelpMergeConfig);

                helpCategory.Options[3] = ArgumentInfo.CreateFlagHelpInfo(Options.Cmd.NoConfig);
                helpCategory.Options[3].HelpText = SR.Format(SR.HelpNoconfig);

                helpCategory.Options[4] = ArgumentInfo.CreateFlagHelpInfo(Options.Cmd.DataContractOnly);
                helpCategory.Options[4].HelpText = SR.Format(SR.HelpCodeGenerationDataContractOnly, Options.Abbr.DataContractOnly);

                helpCategory.Options[5] = ArgumentInfo.CreateParameterHelpInfo(Options.Cmd.Language, SR.Format(SR.ParametersLanguage));
                helpCategory.Options[5].BeginGroup = true;
                helpCategory.Options[5].HelpText = SR.Format(SR.HelpLanguage, Options.Abbr.Language);

                helpCategory.Options[6] = ArgumentInfo.CreateParameterHelpInfo(Options.Cmd.Namespace, SR.Format(SR.ParametersNamespace));
                helpCategory.Options[6].HelpText = SR.Format(SR.HelpNamespace, Options.Abbr.Namespace);

                helpCategory.Options[7] = ArgumentInfo.CreateFlagHelpInfo(Options.Cmd.MessageContract);
                helpCategory.Options[7].BeginGroup = true;
                helpCategory.Options[7].HelpText = SR.Format(SR.HelpMessageContract, Options.Abbr.MessageContract);

                helpCategory.Options[8] = ArgumentInfo.CreateFlagHelpInfo(Options.Cmd.EnableDataBinding);
                helpCategory.Options[8].HelpText = SR.Format(SR.HelpEnableDataBinding, Options.Abbr.EnableDataBinding);

                helpCategory.Options[9] = ArgumentInfo.CreateFlagHelpInfo(Options.Cmd.Serializable);
                helpCategory.Options[9].HelpText = SR.Format(SR.HelpSerializable, Options.Abbr.Serializable);

                helpCategory.Options[10] = ArgumentInfo.CreateFlagHelpInfo(Options.Cmd.Async);
                helpCategory.Options[10].HelpText = SR.Format(SR.HelpAsync, Options.Abbr.Async);

                helpCategory.Options[11] = ArgumentInfo.CreateFlagHelpInfo(Options.Cmd.Internal);
                helpCategory.Options[11].HelpText = SR.Format(SR.HelpInternal, Options.Abbr.Internal);

                helpCategory.Options[12] = ArgumentInfo.CreateParameterHelpInfo(Options.Cmd.Reference, SR.Format(SR.ParametersReference));
                helpCategory.Options[12].BeginGroup = true;
                helpCategory.Options[12].HelpText = SR.Format(SR.HelpReferenceCodeGeneration, Options.Abbr.Reference);

                helpCategory.Options[13] = ArgumentInfo.CreateParameterHelpInfo(Options.Cmd.CollectionType, SR.Format(SR.ParametersCollectionType));
                helpCategory.Options[13].HelpText = SR.Format(SR.HelpCollectionType, Options.Abbr.CollectionType);

                helpCategory.Options[14] = ArgumentInfo.CreateParameterHelpInfo(Options.Cmd.ExcludeType, SR.Format(SR.ParametersExcludeType));
                helpCategory.Options[14].HelpText = SR.Format(SR.HelpExcludeTypeCodeGeneration, Options.Abbr.ExcludeType);

                helpCategory.Options[15] = ArgumentInfo.CreateFlagHelpInfo(Options.Cmd.Nostdlib);
                helpCategory.Options[15].HelpText = SR.Format(SR.HelpNostdlib);

                helpCategory.Options[16] = ArgumentInfo.CreateParameterHelpInfo(Options.Cmd.Serializer, SerializerMode.Auto.ToString());
                helpCategory.Options[16].BeginGroup = true;
                helpCategory.Options[16].HelpText = SR.Format(SR.HelpAutoSerializer, Options.Abbr.Serializer);

                helpCategory.Options[17] = ArgumentInfo.CreateParameterHelpInfo(Options.Cmd.Serializer, SerializerMode.DataContractSerializer.ToString());
                helpCategory.Options[17].HelpText = SR.Format(SR.HelpDataContractSerializer);

                helpCategory.Options[18] = ArgumentInfo.CreateParameterHelpInfo(Options.Cmd.Serializer, SerializerMode.XmlSerializer.ToString());
                helpCategory.Options[18].HelpText = SR.Format(SR.HelpXmlSerializer);

                helpCategory.Options[19] = ArgumentInfo.CreateFlagHelpInfo(Options.Cmd.ImportXmlTypes);
                helpCategory.Options[19].HelpText = SR.Format(SR.HelpImportXmlType, Options.Abbr.ImportXmlTypes);

                helpCategory.Options[20] = ArgumentInfo.CreateFlagHelpInfo(Options.Cmd.UseSerializerForFaults);
                helpCategory.Options[20].HelpText = SR.Format(SR.HelpUseSerializerForFaults, Options.Abbr.UseSerializerForFaults);

                helpCategory.Options[21] = ArgumentInfo.CreateParameterHelpInfo(Options.Cmd.TargetClientVersion, TargetClientVersionMode.Version30.ToString());
                helpCategory.Options[21].BeginGroup = true;
                helpCategory.Options[21].HelpText = SR.Format(SR.HelpVersion30TargetClientVersion, Options.Abbr.TargetClientVersion);

                helpCategory.Options[22] = ArgumentInfo.CreateParameterHelpInfo(Options.Cmd.TargetClientVersion, TargetClientVersionMode.Version35.ToString());
                helpCategory.Options[22].HelpText = SR.Format(SR.HelpVersion35TargetClientVersion, Options.Abbr.TargetClientVersion);

                helpCategory.Options[23] = ArgumentInfo.CreateFlagHelpInfo(Options.Cmd.Wrapped);
                helpCategory.Options[23].HelpText = SR.Format(SR.HelpWrapped);

                helpCategory.Options[24] = ArgumentInfo.CreateFlagHelpInfo(Options.Cmd.ServiceContract);
                helpCategory.Options[24].HelpText = SR.Format(SR.HelpCodeGenerationServiceContract, Options.Abbr.ServiceContract);

                helpCategory.Options[25] = ArgumentInfo.CreateFlagHelpInfo(Options.Cmd.SyncOnly);
                helpCategory.Options[25].HelpText = SR.Format(SR.HelpSyncOnly);

                helpCategory.WriteHelp();
            }

            internal static void WriteExamples()
            {
                HelpCategory helpCategory = new HelpCategory(SR.Format(SR.HelpExamples));
                helpCategory.WriteHelp();

                WriteExample(SR.Format(SR.HelpExamples18), SR.Format(SR.HelpExamples19));
            }

            static void WriteExample(string syntax, string explanation)
            {
                ToolConsole.WriteLine(string.Format(CultureInfo.InvariantCulture, " {0}", syntax));
                exampleBuilder.WriteParagraph(string.Format(CultureInfo.InvariantCulture, "    {0}", explanation));
                ToolConsole.WriteLine();
            }

            class ArgumentInfo
            {
                const string argHelpPrefix = " ";
                const string argHelpSeperator = " - ";

                internal static ArgumentInfo CreateInputHelpInfo(string input)
                {
                    ArgumentInfo argInfo = new ArgumentInfo();
                    argInfo.name = input;
                    return argInfo;
                }

                internal static ArgumentInfo CreateFlagHelpInfo(string option)
                {
                    ArgumentInfo argInfo = new ArgumentInfo();
                    argInfo.name = String.Format(CultureInfo.InvariantCulture, "/{0}", option);
                    return argInfo;
                }

                internal static ArgumentInfo CreateParameterHelpInfo(string option, string optionUse)
                {
                    ArgumentInfo argInfo = new ArgumentInfo();
                    argInfo.name = String.Format(CultureInfo.InvariantCulture, "/{0}:{1}", option, optionUse);
                    return argInfo;
                }

                bool beginGroup;
                string name;
                string helpText;

                public bool BeginGroup
                {
                    get { return beginGroup; }
                    set { beginGroup = value; }
                }

                public string Name
                {
                    get { return name; }
                }
                public string HelpText
                {
                    set { helpText = value; }
                }

                string GenerateHelp(string pattern)
                {
                    return string.Format(CultureInfo.InvariantCulture, pattern, name, helpText);
                }

                static int CalculateMaxNameLength(ArgumentInfo[] arguments)
                {
                    int maxNameLength = 0;
                    foreach (ArgumentInfo argument in arguments)
                    {
                        if (argument.Name.Length > maxNameLength)
                        {
                            maxNameLength = argument.Name.Length;
                        }
                    }
                    return maxNameLength;
                }

                public static void WriteArguments(ArgumentInfo[] arguments)
                {
                    int maxArgumentnLength = CalculateMaxNameLength(arguments);
                    int helpTextIndent = argHelpPrefix.Length + maxArgumentnLength + argHelpSeperator.Length;
                    string helpPattern = argHelpPrefix + "{0, -" + maxArgumentnLength + "}" + argHelpSeperator + "{1}";

                    ToolStringBuilder builder = new ToolStringBuilder(helpTextIndent);

                    foreach (ArgumentInfo argument in arguments)
                    {
                        if (argument.BeginGroup)
                            ToolConsole.WriteLine();

                        string optionHelp = argument.GenerateHelp(helpPattern);
                        builder.WriteParagraph(optionHelp);
                    }
                }

            }

            class HelpCategory
            {
                static HelpCategory()
                {
                    try
                    {
                        bool junk = Console.CursorVisible;
                        if (Console.WindowWidth > 75)
                            nameMidpoint = Console.WindowWidth / 3;
                        else
                            nameMidpoint = 25;
                    }
                    catch
                    {
                        nameMidpoint = 25;
                    }
                }

                static ToolStringBuilder categoryBuilder = new ToolStringBuilder(4);
                static int nameMidpoint;

                int nameStart;
                string name;
                string description = null;
                string syntax = null;
                ArgumentInfo[] options;
                ArgumentInfo[] inputs;

                public HelpCategory(string name)
                {
                    Tool.Assert(name != null, "Name should never be null");
                    this.name = name;
                    this.nameStart = nameMidpoint - (name.Length / 2);
                }

                public string Description
                {
                    set { description = value; }
                }

                public string Syntax
                {
                    set { syntax = value; }
                }

                public ArgumentInfo[] Options
                {
                    get { return options; }
                    set { options = value; }
                }

                public ArgumentInfo[] Inputs
                {
                    get { return inputs; }
                    set { inputs = value; }
                }

                public void WriteHelp()
                {
                    int start = nameMidpoint;
                    ToolConsole.WriteLine(new string(' ', nameStart) + this.name);
                    ToolConsole.WriteLine();

                    if (this.description != null)
                    {
                        categoryBuilder.WriteParagraph(this.description);
                        ToolConsole.WriteLine();
                    }

                    if (this.syntax != null)
                    {
                        categoryBuilder.WriteParagraph(this.syntax);
                        ToolConsole.WriteLine();
                    }
                    if (this.inputs != null)
                    {
                        ArgumentInfo.WriteArguments(this.inputs);
                        ToolConsole.WriteLine();
                    }

                    if (this.options != null)
                    {
                        ToolConsole.WriteLine(SR.Format(SR.HelpOptions));
                        ToolConsole.WriteLine();
                        ArgumentInfo.WriteArguments(this.options);
                        ToolConsole.WriteLine();
                    }
                }



            }
        }

        class ToolStringBuilder
        {

            int indentLength;
            int cursorLeft;
            int lineWidth;
            StringBuilder stringBuilder;

            public ToolStringBuilder(int indentLength)
            {
                this.indentLength = indentLength;
            }

            void Reset()
            {
                this.stringBuilder = new StringBuilder();
                this.cursorLeft = GetConsoleCursorLeft();
                this.lineWidth = GetBufferWidth();
            }

            public void WriteParagraph(string text)
            {
                this.Reset();
                this.AppendParagraph(text);
                ToolConsole.WriteLine(this.stringBuilder.ToString());
                this.stringBuilder = null;
            }

            void AppendParagraph(string text)
            {
                Tool.Assert(this.stringBuilder != null, "stringBuilder cannot be null");

                int index = 0;
                while (index < text.Length)
                {
                    this.AppendWord(text, ref index);
                    this.AppendWhitespace(text, ref index);
                }
            }

            void AppendWord(string text, ref int index)
            {
                // If we're at the beginning of a new line we should indent.
                if ((this.cursorLeft == 0) && (index != 0))
                    AppendIndent();

                int wordLength = FindWordLength(text, index);

                // Now that we know how long the string is we can:
                //   1. print it on the current line if we have enough space
                //   2. print it on the next line if we don't have space 
                //      on the current line and it will fit on the next line
                //   3. print whatever will fit on the current line 
                //      and overflow to the next line.
                if (wordLength < this.HangingLineWidth)
                {
                    if (wordLength > this.BufferWidth)
                    {
                        this.AppendLineBreak();
                        this.AppendIndent();
                    }
                    this.stringBuilder.Append(text, index, wordLength);
                    this.cursorLeft += wordLength;
                }
                else
                {
                    AppendWithOverflow(text, ref index, ref wordLength);
                }

                index += wordLength;
            }

            void AppendWithOverflow(string test, ref int start, ref int wordLength)
            {
                do
                {
                    this.stringBuilder.Append(test, start, this.BufferWidth);
                    start += this.BufferWidth;
                    wordLength -= this.BufferWidth;
                    this.AppendLineBreak();

                    if (wordLength > 0)
                        this.AppendIndent();

                } while (wordLength > this.BufferWidth);

                if (wordLength > 0)
                {
                    this.stringBuilder.Append(test, start, wordLength);
                    this.cursorLeft += wordLength;
                }
            }

            void AppendWhitespace(string text, ref int index)
            {
                while ((index < text.Length) && char.IsWhiteSpace(text[index]))
                {
                    if (BufferWidth == 0)
                    {
                        this.AppendLineBreak();
                    }

                    // For each whitespace character:
                    //   1. If we're at a newline character we insert 
                    //      a new line and reset the cursor.
                    //   2. If the whitespace character is at the beginning of a new 
                    //      line, we insert an indent instead of the whitespace
                    //   3. Insert the whitespace 
                    if (AtNewLine(text, index))
                    {
                        this.AppendLineBreak();
                        index += Environment.NewLine.Length;
                    }
                    else if (this.cursorLeft == 0 && index != 0)
                    {
                        AppendIndent();
                        index++;
                    }
                    else
                    {
                        this.stringBuilder.Append(text[index]);
                        index++;
                        cursorLeft++;
                    }
                }
            }

            void AppendIndent()
            {
                this.stringBuilder.Append(' ', this.indentLength);
                this.cursorLeft += this.indentLength;
            }

            void AppendLineBreak()
            {
                if (BufferWidth != 0)
                    this.stringBuilder.AppendLine();
                this.cursorLeft = 0;
            }

            int BufferWidth
            {
                get
                {
                    return this.lineWidth - this.cursorLeft;
                }
            }

            int HangingLineWidth
            {
                get
                {
                    return this.lineWidth - this.indentLength;
                }
            }

            static int FindWordLength(string text, int index)
            {
                for (int end = index; end < text.Length; end++)
                {
                    if (char.IsWhiteSpace(text[end]))
                        return end - index;
                }
                return text.Length - index;
            }

            static bool AtNewLine(string text, int index)
            {

                if ((index + Environment.NewLine.Length) > text.Length)
                {
                    return false;
                }

                for (int i = 0; i < Environment.NewLine.Length; i++)
                {
                    if (Environment.NewLine[i] != text[index + i])
                    {
                        return false;
                    }
                }

                return true;
            }

            static int GetConsoleCursorLeft()
            {
                try
                {
                    return Console.CursorLeft;
                }
                catch
                {
                    return 0;
                }
            }

            static int GetBufferWidth()
            {
                try
                {
                    bool junk = Console.CursorVisible;
                    return Console.BufferWidth;
                }
                catch
                {
                    return int.MaxValue;
                }
            }
        }

    }
}
