//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    static class HelpGenerator
    {
        private static StringBuilder HelpBuilder;

        internal static string GenerateHelpText()
        {
            HelpBuilder = new StringBuilder();
            HelpGenerator.WriteUsage();
            HelpBuilder.AppendLine();
            HelpBuilder.AppendLine();
            HelpGenerator.WriteCodeGenerationHelp();
            HelpBuilder.AppendLine();
            HelpBuilder.AppendLine();
            HelpGenerator.WriteExamples();
            HelpBuilder.AppendLine();
            HelpBuilder.AppendLine();
            return HelpBuilder.ToString();
        }

        private static void WriteUsage()
        {
            HelpBuilder.AppendLine(SR.GetString(SR.HelpUsage1));
            HelpBuilder.AppendLine();
            HelpBuilder.AppendLine(SR.GetString(SR.HelpUsage2));
        }

        private static void WriteCodeGenerationHelp()
        {
            HelpCategory helpCategory = new HelpCategory(SR.GetString(SR.HelpUsageCategory))
            {
                Inputs = new ArgumentInfo[] {
                    ArgumentInfo.CreateInputHelpInfo(SR.GetString(SR.HelpInputMetadataDocumentPath), SR.GetString(SR.HelpCodeGenerationSyntaxInput1)),
                    ArgumentInfo.CreateInputHelpInfo(SR.GetString(SR.HelpInputUrl),                  SR.GetString(SR.HelpCodeGenerationSyntaxInput2)),
                    ArgumentInfo.CreateInputHelpInfo(SR.GetString(SR.HelpInputEpr),                  SR.GetString(SR.HelpCodeGenerationSyntaxInput3))
                },

                Options = new ArgumentInfo[] {
#if VB_SUPPORT
                    ArgumentInfo.CreateParameterHelpInfo(Options.Switches.Language.Name,                          SR.ParametersLanguage,                           SR.GetString(SR.HelpLanguage, Options.Switches.Language.Abbreviation), true);
#endif
                    ArgumentInfo.CreateParameterHelpInfo(CommandProcessorOptions.Switches.OutputDirectory.Name,   SR.ParametersDirectory,                           SR.GetString(SR.HelpDirectoryFormat, CommandProcessorOptions.Switches.OutputDirectory.Abbreviation)),
                    ArgumentInfo.CreateFlagHelpInfo(     CommandProcessorOptions.Switches.NoLogo.Name,                                                              SR.GetString(SR.HelpNologoFormat, CommandProcessorOptions.Switches.NoLogo.Abbreviation)),
                    ArgumentInfo.CreateParameterHelpInfo(CommandProcessorOptions.Switches.Verbosity.Name,         SR.ParametersVerbosity,                           SR.GetString(SR.HelpVerbosityFormat, string.Join(", ", System.Enum.GetNames(typeof(Verbosity))), CommandProcessorOptions.Switches.Verbosity.Abbreviation)),
                    ArgumentInfo.CreateFlagHelpInfo(     CommandProcessorOptions.Switches.Help.Name,                                                                SR.GetString(SR.HelpHelpFormat, CommandProcessorOptions.Switches.Help.Abbreviation)),
                    ArgumentInfo.CreateParameterHelpInfo(CommandProcessorOptions.Switches.ProjectFile.Name,       SR.ParametersProjectFile,                         SR.GetString(SR.HelpProjectFileFormat, CommandProcessorOptions.Switches.ProjectFile.Abbreviation)),
                    ArgumentInfo.CreateParameterHelpInfo(CommandProcessorOptions.Switches.OutputFile.Name,        SR.ParametersOut,                                 SR.GetString(SR.HelpOutFormat, CommandProcessorOptions.Switches.OutputFile.Abbreviation)),
                    ArgumentInfo.CreateParameterHelpInfo(CommandProcessorOptions.Switches.Namespace.Name,         SR.ParametersNamespace,                           SR.GetString(SR.HelpNamespaceFormat, CommandProcessorOptions.Switches.Namespace.Abbreviation), true),
                    ArgumentInfo.CreateFlagHelpInfo(     CommandProcessorOptions.Switches.MessageContract.Name,                                                     SR.GetString(SR.HelpMessageContractFormat, CommandProcessorOptions.Switches.MessageContract.Abbreviation), true),
                    ArgumentInfo.CreateFlagHelpInfo(     CommandProcessorOptions.Switches.EnableDataBinding.Name,                                                   SR.GetString(SR.HelpEnableDataBindingFormat, CommandProcessorOptions.Switches.EnableDataBinding.Abbreviation)),
                    ArgumentInfo.CreateFlagHelpInfo(     CommandProcessorOptions.Switches.Internal.Name,                                                            SR.GetString(SR.HelpInternalFormat, CommandProcessorOptions.Switches.Internal.Abbreviation)),
                    ArgumentInfo.CreateParameterHelpInfo(CommandProcessorOptions.Switches.Reference.Name,         SR.ParametersReference,                           SR.GetString(SR.HelpReferenceCodeGenerationFormat, CommandProcessorOptions.Switches.Reference.Abbreviation), true),
                    ArgumentInfo.CreateFlagHelpInfo(     CommandProcessorOptions.Switches.NoTypeReuse.Name,                                                         SR.GetString(SR.HelpNoTypeReuseFormat, CommandProcessorOptions.Switches.NoTypeReuse.Abbreviation, CommandProcessorOptions.Switches.Reference.Name, CommandProcessorOptions.Switches.CollectionType.Name)),
                    ArgumentInfo.CreateParameterHelpInfo(CommandProcessorOptions.Switches.CollectionType.Name,    SR.ParametersCollectionType,                      SR.GetString(SR.HelpCollectionTypeFormat, CommandProcessorOptions.Switches.CollectionType.Abbreviation)),
                    ArgumentInfo.CreateParameterHelpInfo(CommandProcessorOptions.Switches.ExcludeType.Name,       SR.ParametersExcludeType,                         SR.GetString(SR.HelpExcludeTypeCodeGenerationFormat, CommandProcessorOptions.Switches.ExcludeType.Abbreviation)),
                    ArgumentInfo.CreateFlagHelpInfo(     CommandProcessorOptions.Switches.NoStdlib.Name,                                                            SR.GetString(SR.HelpNostdlibFormat, CommandProcessorOptions.Switches.NoStdlib.Abbreviation)),
                    ArgumentInfo.CreateParameterHelpInfo(CommandProcessorOptions.Switches.Serializer.Name,        SerializerMode.Auto.ToString(),                   SR.GetString(SR.HelpAutoSerializerFormat, CommandProcessorOptions.Switches.Serializer.Abbreviation), true),
                    ArgumentInfo.CreateParameterHelpInfo(CommandProcessorOptions.Switches.Serializer.Name,        SerializerMode.DataContractSerializer.ToString(), SR.GetString(SR.HelpDataContractSerializer)),
                    ArgumentInfo.CreateParameterHelpInfo(CommandProcessorOptions.Switches.Serializer.Name,        SerializerMode.XmlSerializer.ToString(),          SR.GetString(SR.HelpXmlSerializer)),
                    ArgumentInfo.CreateFlagHelpInfo(     CommandProcessorOptions.Switches.Sync.Name,                                                                SR.GetString(SR.HelpSyncFormat, CommandProcessorOptions.Switches.Sync.Abbreviation)),
                    ArgumentInfo.CreateFlagHelpInfo(     CommandProcessorOptions.Switches.Wrapped.Name,                                                             SR.GetString(SR.HelpWrappedFormat, CommandProcessorOptions.Switches.Wrapped.Abbreviation)),
                    ArgumentInfo.CreateParameterHelpInfo(CommandProcessorOptions.Switches.Update.Name,            SR.ParametersWebServiceReferenceName,             SR.GetString(SR.HelpUpdateWebServiceReferenceFormat, CommandProcessorOptions.Switches.Update.Abbreviation)),
                    ArgumentInfo.CreateParameterHelpInfo(CommandProcessorOptions.Switches.RuntimeIdentifier.Name, SR.ParametersRuntimeIdentifier,                   SR.GetString(SR.HelpRuntimeIdentifierFormat, CommandProcessorOptions.Switches.RuntimeIdentifier.Abbreviation)),
                    ArgumentInfo.CreateParameterHelpInfo(CommandProcessorOptions.Switches.TargetFramework.Name,   SR.ParametersTargetFramework,                     SR.GetString(SR.HelpTargetFrameworkFormat, CommandProcessorOptions.Switches.TargetFramework.Abbreviation)),
                }
            };

            helpCategory.WriteHelp();
        }

        private static void WriteExamples()
        {
            HelpCategory helpCategory = new HelpCategory(SR.GetString(SR.HelpExamples));
            helpCategory.WriteHelp();

            WriteExample(SR.GetString(SR.HelpExamples2), SR.GetString(SR.HelpExamples3));
#if VB_SUPPORT // language
                WriteExample(SR.GetString(SR.HelpExamples6), SR.GetString(SR.HelpExamples7));
#endif
            WriteExample(SR.GetString(SR.HelpExamples8), SR.GetString(SR.HelpExamples9));
        }

        private static void WriteExample(string syntax, string explanation)
        {
            ParagraphHelper paragraphHelper = new ParagraphHelper();
            HelpBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, " {0}", syntax));
            HelpBuilder.AppendLine(paragraphHelper.AddIndentation(string.Format(CultureInfo.InvariantCulture, "    {0}", explanation), indentLength: 4));
            HelpBuilder.AppendLine();
        }

        class ArgumentInfo
        {
            const string argHelpPrefix = " ";
            const string argHelpSeperator = " - ";

            internal static ArgumentInfo CreateInputHelpInfo(string input, string helpText, bool beginGroup = false)
            {
                return new ArgumentInfo()
                {
                    name = input,
                    helpText = helpText,
                    beginGroup = beginGroup
                };
            }

            internal static ArgumentInfo CreateFlagHelpInfo(string option, string helpText, bool beginGroup = false)
            {
                return new ArgumentInfo()
                {
                    name = String.Format(CultureInfo.InvariantCulture, "{0}{1}", CommandSwitch.FullSwitchIndicator, option),
                    helpText = helpText,
                    beginGroup = beginGroup
                };
            }

            internal static ArgumentInfo CreateParameterHelpInfo(string option, string optionUse, string helpText, bool beginGroup = false)
            {
                return new ArgumentInfo()
                {
                    name = String.Format(CultureInfo.InvariantCulture, "{0}{1} {2}", CommandSwitch.FullSwitchIndicator, option, optionUse),
                    helpText = helpText,
                    beginGroup = beginGroup
                };
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

                ParagraphHelper paragraphHelper = new ParagraphHelper();

                foreach (ArgumentInfo argument in arguments)
                {
                    if (argument.BeginGroup)
                        HelpBuilder.AppendLine();

                    string optionHelp = argument.GenerateHelp(helpPattern);
                    HelpBuilder.AppendLine(paragraphHelper.AddIndentation(optionHelp, helpTextIndent));
                }
            }
        }

        class HelpCategory
        {
            static HelpCategory()
            {
                try
                {
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

            static int nameMidpoint;

            int nameStart;
            string name;
            string description = null;
            string syntax = null;
            ArgumentInfo[] options;
            ArgumentInfo[] inputs;

            public HelpCategory(string name)
            {
                Debug.Assert(!string.IsNullOrEmpty(name), "Help category name should have a valid value!");
                if (name == null)
                {
                    name = string.Empty;
                }
                this.name = name;
                this.nameStart = nameMidpoint - (name.Length / 2);
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
                HelpBuilder.AppendLine(new string(' ', nameStart) + this.name);
                HelpBuilder.AppendLine();

                if (this.inputs != null)
                {
                    ArgumentInfo.WriteArguments(this.inputs);
                    HelpBuilder.AppendLine();
                }

                if (this.options != null)
                {
                    HelpBuilder.AppendLine(SR.GetString(SR.HelpOptions));
                    HelpBuilder.AppendLine();
                    ArgumentInfo.WriteArguments(this.options);
                    HelpBuilder.AppendLine();
                }
            }
        }
    }

    // Helper class to insert whitespace into a string so multiple lines will line up correctly in the console window.
    class ParagraphHelper
    {
        int indentLength;
        int cursorLeft;
        int lineWidth;
        StringBuilder stringBuilder;

        public string AddIndentation(string text, int indentLength)
        {
            this.indentLength = indentLength;
            this.Reset();
            this.AppendParagraph(text);
            return this.stringBuilder.ToString();
        }

        void Reset()
        {
            this.stringBuilder = new StringBuilder();
            this.cursorLeft = GetConsoleCursorLeft();
            this.lineWidth = GetBufferWidth();
        }

        void AppendParagraph(string text)
        {
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
                int bufferWidth = Console.BufferWidth;

                if (bufferWidth > 0)
                {
                    return Console.BufferWidth;
                }
                else
                {
                    return int.MaxValue;
                }
            }
            catch
            {
                return int.MaxValue;
            }
        }
    }
}
