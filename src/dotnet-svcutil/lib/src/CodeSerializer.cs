// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceModel.Description;
using System.Text;
using Microsoft.Xml;
using Microsoft.Xml.Schema;
using WsdlNS = System.Web.Services.Description;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class CodeSerializer
    {
        private static readonly string s_defaultFileName = "ServiceReferences";
        private static readonly Encoding s_ouputEncoding = new System.Text.UTF8Encoding(true);

        private readonly CodeDomProvider _codeProvider;
        private readonly string _outputFilePath;

        internal CodeSerializer(CommandProcessorOptions options, IEnumerable<MetadataSection> inputMetadata)
        {
            string extension = GetOutputFileExtension(options);
            string outputFilename = GetOutputFileName(options, inputMetadata);
            _outputFilePath = OutputPathHelper.BuildFilePath(s_defaultFileName, options.OutputDir.FullName, outputFilename, extension, CommandProcessorOptions.Switches.OutputFile.Name);
            _codeProvider = options.CodeProvider;
        }

        public string Save(CodeCompileUnit codeCompileUnit)
        {
            bool codeGenerated = CompileUnitHasTypes(codeCompileUnit);

            if (!codeGenerated)
            {
                throw new ToolRuntimeException(SR.NoCodeWasGenerated);
            }

            try
            {
                return SaveCode(codeCompileUnit);
            }
            catch (Exception e)
            {
                if (e is ToolRuntimeException || Utils.IsFatalOrUnexpected(e))
                    throw;

                throw new ToolRuntimeException(SR.ErrCannotWriteFile, e);
            }
        }

        public string SaveCode(CodeCompileUnit codeCompileUnit)
        {
            string filePath = null;
            OutputPathHelper.CreateDirectoryIfNeeded(_outputFilePath);

            CodeGeneratorOptions codeGenOptions = new CodeGeneratorOptions();
            codeGenOptions.BracingStyle = "C";

            using (TextWriter writer = CreateOutputFile())
            {
                try
                {
                    string generated;
                    using (StringWriter buffer = new StringWriter(CultureInfo.InvariantCulture))
                    {
                        _codeProvider.GenerateCodeFromCompileUnit(codeCompileUnit, buffer, codeGenOptions);
                        generated = buffer.ToString();
                    }

                    generated = WrapCloseAsyncWithDirectivesIfNeeded(generated);

                    writer.Write(generated);
                    writer.Flush();
                }
                catch (Exception e)
                {
                    if (Utils.IsFatalOrUnexpected(e)) throw;

                    try
                    {
                        if (File.Exists(_outputFilePath))
                        {
                            File.Delete(_outputFilePath);
                        }
                    }
                    catch (System.UnauthorizedAccessException)
                    {
                    }

                    throw new ToolRuntimeException(SR.ErrCodegenError, e);
                }
                filePath = _outputFilePath.Contains(" ") ? string.Format(CultureInfo.InvariantCulture, "\"{0}\"", _outputFilePath) : _outputFilePath;
            }

            return filePath;
        }

        private string WrapCloseAsyncWithDirectivesIfNeeded(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            const string markerPrefix = "SVCUTIL_CLOSEASYNC_WRAP:";

            while (true)
            {
                int markerIndex = text.IndexOf(markerPrefix, StringComparison.Ordinal);
                if (markerIndex < 0)
                {
                    return text;
                }

                int markerLineStart = text.LastIndexOf('\n', markerIndex);
                markerLineStart = markerLineStart < 0 ? 0 : markerLineStart + 1;
                int markerLineEnd = text.IndexOf('\n', markerIndex);
                markerLineEnd = markerLineEnd < 0 ? text.Length : markerLineEnd;

                string markerLine = text.Substring(markerLineStart, markerLineEnd - markerLineStart);
                string indent = GetLeadingWhitespace(markerLine);
                int conditionStart = markerLine.IndexOf(markerPrefix, StringComparison.Ordinal) + markerPrefix.Length;
                string condition = conditionStart >= 0 && conditionStart <= markerLine.Length ? markerLine.Substring(conditionStart).Trim() : string.Empty;

                bool isVisualBasic = string.Equals(_codeProvider.FileExtension, "vb", StringComparison.OrdinalIgnoreCase);
                string ifStart = isVisualBasic ? $"#If {condition} Then" : $"#if {condition}";
                string ifEnd = isVisualBasic ? "#End If" : "#endif";

                string ifStartLine = indent + ifStart;
                string ifEndLine = indent + ifEnd;

                // Find the beginning of the member (we'll insert #if before the marker comment)
                int insertIfPos = markerLineStart;

                // Find the end of the member
                int memberEndPos = isVisualBasic
                    ? FindVisualBasicMemberEnd(text, markerLineEnd)
                    : FindCSharpMemberEnd(text, markerLineEnd);

                if (memberEndPos <= 0)
                {
                    return text;
                }

                // Remove the marker line (and its trailing newline if present)
                int removeStart = markerLineStart;
                int removeEnd = markerLineEnd;
                if (removeEnd < text.Length && text[removeEnd] == '\n')
                {
                    removeEnd++;
                }
                text = text.Remove(removeStart, removeEnd - removeStart);

                // Adjust positions after removal
                int removedLength = removeEnd - removeStart;
                memberEndPos -= removedLength;

                // Insert #if
                text = text.Insert(insertIfPos, ifStartLine + Environment.NewLine);
                memberEndPos += ifStartLine.Length + Environment.NewLine.Length;

                // Insert #endif after the member
                string endifInsertion;
                if (memberEndPos > 0 && text[memberEndPos - 1] == '\n')
                {
                    // We are already at the start of the next line.
                    endifInsertion = ifEndLine + Environment.NewLine;
                }
                else
                {
                    // No newline after the member (EOF). Ensure #endif starts on its own line.
                    endifInsertion = Environment.NewLine + ifEndLine + Environment.NewLine;
                }

                text = text.Insert(memberEndPos, endifInsertion);
            }
        }

        private static string GetLeadingWhitespace(string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                return string.Empty;
            }

            int i = 0;
            while (i < line.Length)
            {
                char ch = line[i];
                if (ch != ' ' && ch != '\t')
                {
                    break;
                }
                i++;
            }

            return i == 0 ? string.Empty : line.Substring(0, i);
        }

        private static int FindCSharpMemberEnd(string text, int searchStart)
        {
            int openBrace = text.IndexOf('{', searchStart);
            if (openBrace < 0)
            {
                return -1;
            }

            int depth = 0;
            for (int i = openBrace; i < text.Length; i++)
            {
                char ch = text[i];
                if (ch == '{')
                {
                    depth++;
                }
                else if (ch == '}')
                {
                    depth--;
                    if (depth == 0)
                    {
                        // Return position right after the closing brace line
                        int lineEnd = text.IndexOf('\n', i);
                        return lineEnd < 0 ? text.Length : Math.Min(text.Length, lineEnd + 1);
                    }
                }
            }

            return -1;
        }

        private static int FindVisualBasicMemberEnd(string text, int searchStart)
        {
            int i = searchStart;
            while (i < text.Length)
            {
                int lineEnd = text.IndexOf('\n', i);
                if (lineEnd < 0)
                {
                    lineEnd = text.Length;
                }

                string line = text.Substring(i, lineEnd - i).TrimStart();
                if (line.StartsWith("End Function", StringComparison.OrdinalIgnoreCase) ||
                    line.StartsWith("End Sub", StringComparison.OrdinalIgnoreCase))
                {
                    return lineEnd < text.Length ? lineEnd + 1 : text.Length;
                }

                i = lineEnd + 1;
            }

            return -1;
        }

        private StreamWriter CreateOutputFile()
        {
            OutputPathHelper.CreateDirectoryIfNeeded(_outputFilePath);

            try
            {
                return new StreamWriter(new FileStream(_outputFilePath, FileMode.Create, FileAccess.Write), s_ouputEncoding);
            }
            catch (Exception e)
            {
                if (Utils.IsFatalOrUnexpected(e)) throw;
                throw new ToolRuntimeException(string.Format(SR.ErrCannotCreateFileFormat, _outputFilePath), e);
            }
        }

        private static bool CompileUnitHasTypes(CodeCompileUnit codeCompileUnit)
        {
            foreach (CodeNamespace ns in codeCompileUnit.Namespaces)
            {
                if (ns.Types.Count != 0)
                    return true;
            }
            return false;
        }

        internal static string GetOutputFileExtension(CommandProcessorOptions options)
        {
            string fileExtension = options.CodeProvider.FileExtension ?? string.Empty;
            if (fileExtension.Length > 0 && fileExtension[0] != '.')
            {
                fileExtension = "." + fileExtension;
            }
            return fileExtension;
        }

        internal static string GetOutputFileName(CommandProcessorOptions options, IEnumerable<MetadataSection> metadataSections)
        {
            string fileName = options.OutputFile?.FullName;

            if (string.IsNullOrWhiteSpace(fileName))
            {
                var wsdlDocuments = metadataSections.Where(s => s.Metadata is WsdlNS.ServiceDescription).Cast<WsdlNS.ServiceDescription>();

                foreach (WsdlNS.ServiceDescription wsdl in wsdlDocuments)
                {
                    if (!string.IsNullOrEmpty(wsdl.Name))
                    {
                        fileName = XmlConvert.DecodeName(wsdl.Name);
                        if (!string.IsNullOrWhiteSpace(fileName) && fileName.IndexOfAny(Path.GetInvalidFileNameChars()) == -1)
                        {
                            break;
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(fileName))
                {
                    foreach (WsdlNS.ServiceDescription wsdl in wsdlDocuments)
                    {
                        if (wsdl.Services.Count > 0 && !string.IsNullOrEmpty(wsdl.Services[0].Name))
                        {
                            fileName = XmlConvert.DecodeName(wsdl.Services[0].Name);
                            if (!string.IsNullOrWhiteSpace(fileName) && fileName.IndexOfAny(Path.GetInvalidFileNameChars()) == -1)
                            {
                                break;
                            }
                        }
                    }

                    if (string.IsNullOrWhiteSpace(fileName))
                    {
                        var schemaDocuments = metadataSections.Where(s => s.Metadata is XmlSchema).Cast<XmlSchema>();

                        foreach (XmlSchema schema in schemaDocuments)
                        {
                            if (!string.IsNullOrEmpty(schema.TargetNamespace))
                            {
                                fileName = OutputPathHelper.FilenameFromUri(schema.TargetNamespace);
                                if (!string.IsNullOrWhiteSpace(fileName) && fileName.IndexOfAny(Path.GetInvalidFileNameChars()) == -1)
                                {
                                    break;
                                }
                            }
                        }

                        if (string.IsNullOrWhiteSpace(fileName))
                        {
                            fileName = CodeSerializer.s_defaultFileName;
                        }
                    }
                }
            }

            return fileName;
        }
    }
}
