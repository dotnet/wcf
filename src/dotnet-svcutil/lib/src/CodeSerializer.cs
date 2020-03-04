// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

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
        static readonly string DefaultFileName = "ServiceReferences";
        static readonly Encoding ouputEncoding = new System.Text.UTF8Encoding(true);

        readonly CodeDomProvider codeProvider;
        readonly string outputFilePath;

        internal CodeSerializer(CommandProcessorOptions options, IEnumerable<MetadataSection> inputMetadata)
        {
            string extension = GetOutputFileExtension(options);
            string outputFilename = GetOutputFileName(options, inputMetadata);
            this.outputFilePath = OutputPathHelper.BuildFilePath(DefaultFileName, options.OutputDir.FullName, outputFilename, extension, CommandProcessorOptions.Switches.OutputFile.Name);
            this.codeProvider = options.CodeProvider;
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

                throw new ToolRuntimeException(SR.GetString(SR.ErrCannotWriteFile), e);
            }
        }

        public string SaveCode(CodeCompileUnit codeCompileUnit)
        {
            string filePath = null;
            OutputPathHelper.CreateDirectoryIfNeeded(outputFilePath);

            CodeGeneratorOptions codeGenOptions = new CodeGeneratorOptions();
            codeGenOptions.BracingStyle = "C";

            using (TextWriter writer = CreateOutputFile())
            {
                try
                {
                    this.codeProvider.GenerateCodeFromCompileUnit(codeCompileUnit, writer, codeGenOptions);
                    writer.Flush();
                }
                catch (Exception e)
                {
                    if (Utils.IsFatalOrUnexpected(e)) throw;

                    try
                    {
                        if (File.Exists(this.outputFilePath))
                        {
                            File.Delete(this.outputFilePath);
                        }
                    }
                    catch (System.UnauthorizedAccessException)
                    {
                    }

                    throw new ToolRuntimeException(SR.GetString(SR.ErrCodegenError), e);
                }
                filePath = this.outputFilePath.Contains(" ") ? string.Format(CultureInfo.InvariantCulture, "\"{0}\"", this.outputFilePath) : this.outputFilePath;
            }

            return filePath;
        }

        StreamWriter CreateOutputFile()
        {
            OutputPathHelper.CreateDirectoryIfNeeded(this.outputFilePath);

            try
            {
                return new StreamWriter(new FileStream(this.outputFilePath, FileMode.Create, FileAccess.Write), ouputEncoding);
            }
            catch (Exception e)
            {
                if (Utils.IsFatalOrUnexpected(e)) throw;
                throw new ToolRuntimeException(SR.GetString(SR.ErrCannotCreateFileFormat, this.outputFilePath), e);
            }
        }

        static bool CompileUnitHasTypes(CodeCompileUnit codeCompileUnit)
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
                            fileName = CodeSerializer.DefaultFileName;
                        }
                    }
                }
            }

            return fileName;
        }
    }
}
