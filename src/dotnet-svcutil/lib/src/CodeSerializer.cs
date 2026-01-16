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
            bool isVisualBasic = string.Equals(_codeProvider.FileExtension, "vb", StringComparison.OrdinalIgnoreCase);

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
                string condition = conditionStart >= 0 && conditionStart <= markerLine.Length
                    ? markerLine.Substring(conditionStart).Trim().Trim('"')
                    : string.Empty;

                // The marker is emitted as a #region/#endregion pair around the member.
                // Convert that pair into #if/#endif (or VB equivalents) in the final generated text.
                int endRegionLineStart = FindMatchingRegionEndLineStart(text, markerLineEnd, isVisualBasic);
                if (endRegionLineStart < 0)
                {
                    return text;
                }

                int endRegionLineEnd = text.IndexOf('\n', endRegionLineStart);
                endRegionLineEnd = endRegionLineEnd < 0 ? text.Length : endRegionLineEnd;

                string ifStart = isVisualBasic ? $"#If {condition} Then" : $"#if {condition}";
                string ifEnd = isVisualBasic ? "#End If" : "#endif";

                string ifStartLine = indent + ifStart;

                // Replace the #region line with #if.
                int oldStartLen = markerLineEnd - markerLineStart;
                text = text.Remove(markerLineStart, oldStartLen).Insert(markerLineStart, ifStartLine);

                // Adjust end-region indices after start-line replacement.
                int delta = ifStartLine.Length - oldStartLen;
                endRegionLineStart += delta;
                endRegionLineEnd += delta;

                string endRegionLine = text.Substring(endRegionLineStart, endRegionLineEnd - endRegionLineStart);
                string endIndent = GetLeadingWhitespace(endRegionLine);
                string ifEndLine = endIndent + ifEnd;

                // Replace the #endregion line with #endif.
                int oldEndLen = endRegionLineEnd - endRegionLineStart;
                text = text.Remove(endRegionLineStart, oldEndLen).Insert(endRegionLineStart, ifEndLine);
            }
        }

        private static int FindMatchingRegionEndLineStart(string text, int searchStart, bool isVisualBasic)
        {
            if (string.IsNullOrEmpty(text))
            {
                return -1;
            }

            int i = searchStart;
            if (i < text.Length && text[i] == '\n')
            {
                i++;
            }

            int depth = 1;
            while (i < text.Length)
            {
                int lineEnd = text.IndexOf('\n', i);
                if (lineEnd < 0)
                {
                    lineEnd = text.Length;
                }

                string line = text.Substring(i, lineEnd - i);
                string trimmed = line.TrimStart();

                if (IsRegionStart(trimmed, isVisualBasic))
                {
                    depth++;
                }
                else if (IsRegionEnd(trimmed, isVisualBasic))
                {
                    depth--;
                    if (depth == 0)
                    {
                        return i;
                    }
                }

                i = lineEnd + 1;
            }

            return -1;
        }

        private static bool IsRegionStart(string trimmedLine, bool isVisualBasic)
        {
            if (string.IsNullOrEmpty(trimmedLine))
            {
                return false;
            }

            return isVisualBasic
                ? trimmedLine.StartsWith("#Region", StringComparison.OrdinalIgnoreCase)
                : trimmedLine.StartsWith("#region", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsRegionEnd(string trimmedLine, bool isVisualBasic)
        {
            if (string.IsNullOrEmpty(trimmedLine))
            {
                return false;
            }

            return isVisualBasic
                ? trimmedLine.StartsWith("#End Region", StringComparison.OrdinalIgnoreCase)
                : trimmedLine.StartsWith("#endregion", StringComparison.OrdinalIgnoreCase);
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
