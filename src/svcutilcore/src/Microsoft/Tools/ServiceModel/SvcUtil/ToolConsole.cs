// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Tools.ServiceModel.SvcUtil.XmlSerializer
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.CodeDom.Compiler;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Reflection;
    using System.Globalization;

    internal static class ToolConsole
    {
#if DEBUG
        private static bool s_debug = false;
#endif

#if DEBUG
        internal static void SetOptions(bool debug)
        {
            ToolConsole.s_debug = debug;
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

        private static void WriteError(Exception e)
        {
            WriteError(e, SR.Format(SR.Error));
        }

        internal static void WriteUnexpectedError(Exception e)
        {
            WriteError(SR.Format(SR.ErrUnexpectedError));
            WriteError(e);
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

        private static void WriteError(string errMsg, string prefix)
        {
            Console.Error.Write(prefix);
            Console.Error.WriteLine(errMsg);
            Console.Error.WriteLine();
        }

        internal static void WriteError(Exception e, string prefix)
        {
#if DEBUG
            if (s_debug)
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
            ToolConsole.WriteLine(SR.Format(SR.Logo, ThisAssembly.InformationalVersion, SR.WcfTrademarkForCmdLine, SR.CopyrightForCmdLine));
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

        private static class HelpGenerator
        {
            private static ToolStringBuilder s_exampleBuilder = new ToolStringBuilder(4);
            static HelpGenerator() { } // beforefieldInit

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
                helpCategory.Syntax = SR.Format(SR.HelpXmlSerializerTypeGenerationSyntax, ThisAssembly.Title, SR.Format(SR.HelpInputAssemblyPath));

                helpCategory.Inputs = new ArgumentInfo[1];

                helpCategory.Inputs[0] = ArgumentInfo.CreateInputHelpInfo(SR.Format(SR.HelpInputAssemblyPath));
                helpCategory.Inputs[0].HelpText = SR.Format(SR.HelpXmlSerializerTypeGenerationSyntaxInput1);

                helpCategory.Options = new ArgumentInfo[3];

                helpCategory.Options[0] = ArgumentInfo.CreateParameterHelpInfo(Options.Cmd.Reference, SR.Format(SR.ParametersReference));
                helpCategory.Options[0].HelpText = SR.Format(SR.HelpXmlSerializerTypeGenerationSyntaxInput2, Options.Abbr.Reference);

                helpCategory.Options[1] = ArgumentInfo.CreateParameterHelpInfo(Options.Cmd.ExcludeType, SR.Format(SR.ParametersExcludeType));
                helpCategory.Options[1].HelpText = SR.Format(SR.HelpXmlSerializerTypeGenerationSyntaxInput3, Options.Abbr.ExcludeType);

                helpCategory.Options[2] = ArgumentInfo.CreateParameterHelpInfo(Options.Cmd.Out, SR.Format(SR.ParametersOut));
                helpCategory.Options[2].HelpText = SR.Format(SR.HelpXmlSerializerTypeGenerationSyntaxInput4, Options.Abbr.Out);

                helpCategory.WriteHelp();
            }

            internal static void WriteCommonOptionsHelp()
            {
                HelpCategory helpCategory = new HelpCategory(SR.Format(SR.HelpCommonOptionsCategory));
                var options = new List<ArgumentInfo>();
                ArgumentInfo option;

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
                helpCategory.Syntax = SR.Format(SR.HelpCodeGenerationAbbreviationSyntax, ThisAssembly.Title, Options.Abbr.Target, Options.Targets.Code,
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

                helpCategory.Options[1] = ArgumentInfo.CreateParameterHelpInfo(Options.Cmd.Namespace, SR.Format(SR.ParametersNamespace));
                helpCategory.Options[1].HelpText = SR.Format(SR.HelpNamespace, Options.Abbr.Namespace);

                helpCategory.Options[2] = ArgumentInfo.CreateParameterHelpInfo(Options.Cmd.Reference, SR.Format(SR.ParametersReference));
                helpCategory.Options[2].BeginGroup = true;
                helpCategory.Options[2].HelpText = SR.Format(SR.HelpReferenceCodeGeneration, Options.Abbr.Reference);

                helpCategory.Options[3] = ArgumentInfo.CreateParameterHelpInfo(Options.Cmd.CollectionType, SR.Format(SR.ParametersCollectionType));
                helpCategory.Options[3].HelpText = SR.Format(SR.HelpCollectionType, Options.Abbr.CollectionType);

                helpCategory.Options[4] = ArgumentInfo.CreateParameterHelpInfo(Options.Cmd.ExcludeType, SR.Format(SR.ParametersExcludeType));
                helpCategory.Options[4].HelpText = SR.Format(SR.HelpExcludeTypeCodeGeneration, Options.Abbr.ExcludeType);

                helpCategory.Options[5] = ArgumentInfo.CreateFlagHelpInfo(Options.Cmd.Nostdlib);
                helpCategory.Options[5].HelpText = SR.Format(SR.HelpNostdlib);

                helpCategory.WriteHelp();
            }

            internal static void WriteExamples()
            {
                HelpCategory helpCategory = new HelpCategory(SR.Format(SR.HelpExamples));
                helpCategory.WriteHelp();

                WriteExample(SR.HelpExamples1, SR.HelpExamples2);
            }

            private static void WriteExample(string syntax, string explanation)
            {
                ToolConsole.WriteLine(string.Format(CultureInfo.InvariantCulture, " {0}", syntax));
                s_exampleBuilder.WriteParagraph(string.Format(CultureInfo.InvariantCulture, "    {0}", explanation));
                ToolConsole.WriteLine();
            }

            private class ArgumentInfo
            {
                private const string argHelpPrefix = " ";
                private const string argHelpSeperator = " - ";

                internal static ArgumentInfo CreateInputHelpInfo(string input)
                {
                    ArgumentInfo argInfo = new ArgumentInfo();
                    argInfo._name = input;
                    return argInfo;
                }

                internal static ArgumentInfo CreateFlagHelpInfo(string option)
                {
                    ArgumentInfo argInfo = new ArgumentInfo();
                    argInfo._name = $"--{option}";
                    return argInfo;
                }

                internal static ArgumentInfo CreateParameterHelpInfo(string option, string optionUse)
                {
                    ArgumentInfo argInfo = new ArgumentInfo();
                    argInfo._name = $"--{option}:{optionUse}";
                    return argInfo;
                }

                private bool _beginGroup;
                private string _name;
                private string _helpText;

                public bool BeginGroup
                {
                    get { return _beginGroup; }
                    set { _beginGroup = value; }
                }

                public string Name
                {
                    get { return _name; }
                }
                public string HelpText
                {
                    set { _helpText = value; }
                }

                private string GenerateHelp(string pattern)
                {
                    return string.Format(CultureInfo.InvariantCulture, pattern, _name, _helpText);
                }

                private static int CalculateMaxNameLength(ArgumentInfo[] arguments)
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
                    int maxArgumentLength = CalculateMaxNameLength(arguments);
                    int helpTextIndent = argHelpPrefix.Length + maxArgumentLength + argHelpSeperator.Length;
                    string helpPattern = argHelpPrefix + "{0, -" + maxArgumentLength + "}" + argHelpSeperator + "{1}";

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

            private class HelpCategory
            {
                static HelpCategory()
                {
                    try
                    {
#pragma warning disable CA1416 // Validate platform compatibility
                        bool junk = Console.CursorVisible;
#pragma warning restore CA1416 // Validate platform compatibility
                        if (Console.WindowWidth > 75)
                            s_nameMidpoint = Console.WindowWidth / 3;
                        else
                            s_nameMidpoint = 25;
                    }
                    catch
                    {
                        s_nameMidpoint = 25;
                    }
                }

                private static ToolStringBuilder s_categoryBuilder = new ToolStringBuilder(4);
                private static int s_nameMidpoint;

                private int _nameStart;
                private string _name;
                private string _description = null;
                private string _syntax = null;
                private ArgumentInfo[] _options;
                private ArgumentInfo[] _inputs;

                public HelpCategory(string name)
                {
                    Tool.Assert(name != null, "Name should never be null");
                    _name = name;
                    _nameStart = s_nameMidpoint - (name.Length / 2);
                }

                public string Description
                {
                    set { _description = value; }
                }

                public string Syntax
                {
                    set { _syntax = value; }
                }

                public ArgumentInfo[] Options
                {
                    get { return _options; }
                    set { _options = value; }
                }

                public ArgumentInfo[] Inputs
                {
                    get { return _inputs; }
                    set { _inputs = value; }
                }

                public void WriteHelp()
                {
                    int start = s_nameMidpoint;
                    ToolConsole.WriteLine(new string(' ', _nameStart) + _name);
                    ToolConsole.WriteLine();

                    if (_description != null)
                    {
                        s_categoryBuilder.WriteParagraph(_description);
                        ToolConsole.WriteLine();
                    }

                    if (_syntax != null)
                    {
                        s_categoryBuilder.WriteParagraph(_syntax);
                        ToolConsole.WriteLine();
                    }
                    if (_inputs != null)
                    {
                        ArgumentInfo.WriteArguments(_inputs);
                        ToolConsole.WriteLine();
                    }

                    if (_options != null)
                    {
                        ToolConsole.WriteLine(SR.Format(SR.HelpOptions));
                        ToolConsole.WriteLine();
                        ArgumentInfo.WriteArguments(_options);
                        ToolConsole.WriteLine();
                    }
                }
            }
        }

        private class ToolStringBuilder
        {
            private int _indentLength;
            private int _cursorLeft;
            private int _lineWidth;
            private StringBuilder _stringBuilder;

            public ToolStringBuilder(int indentLength)
            {
                _indentLength = indentLength;
            }

            private void Reset()
            {
                _stringBuilder = new StringBuilder();
                _cursorLeft = GetConsoleCursorLeft();
                _lineWidth = GetBufferWidth();
            }

            public void WriteParagraph(string text)
            {
                this.Reset();
                this.AppendParagraph(text);
                ToolConsole.WriteLine(_stringBuilder.ToString());
                _stringBuilder = null;
            }

            private void AppendParagraph(string text)
            {
                Tool.Assert(_stringBuilder != null, "stringBuilder cannot be null");

                int index = 0;
                while (index < text.Length)
                {
                    this.AppendWord(text, ref index);
                    this.AppendWhitespace(text, ref index);
                }
            }

            private void AppendWord(string text, ref int index)
            {
                // If we're at the beginning of a new line we should indent.
                if ((_cursorLeft == 0) && (index != 0))
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
                    _stringBuilder.Append(text, index, wordLength);
                    _cursorLeft += wordLength;
                }
                else
                {
                    AppendWithOverflow(text, ref index, ref wordLength);
                }

                index += wordLength;
            }

            private void AppendWithOverflow(string test, ref int start, ref int wordLength)
            {
                do
                {
                    _stringBuilder.Append(test, start, this.BufferWidth);
                    start += this.BufferWidth;
                    wordLength -= this.BufferWidth;
                    this.AppendLineBreak();

                    if (wordLength > 0)
                        this.AppendIndent();
                } while (wordLength > this.BufferWidth);

                if (wordLength > 0)
                {
                    _stringBuilder.Append(test, start, wordLength);
                    _cursorLeft += wordLength;
                }
            }

            private void AppendWhitespace(string text, ref int index)
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
                    else if (_cursorLeft == 0 && index != 0)
                    {
                        AppendIndent();
                        index++;
                    }
                    else
                    {
                        _stringBuilder.Append(text[index]);
                        index++;
                        _cursorLeft++;
                    }
                }
            }

            private void AppendIndent()
            {
                _stringBuilder.Append(' ', _indentLength);
                _cursorLeft += _indentLength;
            }

            private void AppendLineBreak()
            {
                if (BufferWidth != 0)
                    _stringBuilder.AppendLine();
                _cursorLeft = 0;
            }

            private int BufferWidth
            {
                get
                {
                    return _lineWidth - _cursorLeft;
                }
            }

            private int HangingLineWidth
            {
                get
                {
                    return _lineWidth - _indentLength;
                }
            }

            private static int FindWordLength(string text, int index)
            {
                for (int end = index; end < text.Length; end++)
                {
                    if (char.IsWhiteSpace(text[end]))
                        return end - index;
                }
                return text.Length - index;
            }

            private static bool AtNewLine(string text, int index)
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

            private static int GetConsoleCursorLeft()
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

            private static int GetBufferWidth()
            {
                try
                {
#pragma warning disable CA1416 // Validate platform compatibility
                    bool junk = Console.CursorVisible;
#pragma warning restore CA1416 // Validate platform compatibility
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
