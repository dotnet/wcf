// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.CodeDom;
using Microsoft.CodeDom.Compiler;
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
                    _codeProvider.GenerateCodeFromCompileUnit(codeCompileUnit, writer, codeGenOptions);
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
