//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

// DevNote: See how there are no using statements needed here? 
//          That's part of the plan. Let's keep it that way.

using System;

namespace Microsoft.Tools.ServiceModel.SvcUtil
{
    class ToolRuntime
    {
        Options options;

        internal ToolRuntime(Options options)
        {
            this.options = options;
        }

        internal ToolExitCodes Run()
        {
            if (!options.NoLogo)
                ToolConsole.WriteHeader();

            if (options.GetToolMode() == ToolMode.DisplayHelp)
            {
                ToolConsole.WriteHelpText();
                return ToolExitCodes.Success;
            }
            else
            {
                InputModule inputModule = InputModule.LoadInputs(options);

                Tool.Assert(options.GetToolMode().HasValue, SR.Format(SR.AmbiguousToolUseage, Options.Cmd.Target, Options.Cmd.Validate));
                ToolMode toolMode = options.GetToolMode().Value;

                return ExecuteToolMode(toolMode, inputModule);
            }
        }

        ToolExitCodes ExecuteToolMode(ToolMode toolMode, InputModule inputModule)
        {
            switch (toolMode)
            {
                //case ToolMode.DataContractImport:
                //    this.ImportDataContracts(inputModule);
                //    break;

                //case ToolMode.ProxyGeneration:
                //case ToolMode.ServiceContractGeneration:
                //    this.ImportServiceContracts(inputModule);
                //    break;

                //case ToolMode.DataContractExport:
                //    this.ExportDataContracts(inputModule);
                //    break;

                //case ToolMode.Validate:
                //    return this.ValidateServices(inputModule);

                //case ToolMode.MetadataFromAssembly:
                //    return this.ExportServiceContracts(inputModule);

                //case ToolMode.WSMetadataExchange:
                //    this.ExportDownloadedMetadata(inputModule);
                //    break;

                case ToolMode.XmlSerializerGeneration:
                    this.GenerateSerializer(inputModule);
                    break;
            }
            return ToolExitCodes.Success;
        }

        //void ImportServiceContracts(InputModule inputModule)
        //{
        //    ImportModule importModule = new ImportModule(options, inputModule.InputConfiguration, inputModule.MetadataDocuments);
        //    importModule.ImportServiceContracts();

        //    ToolConsole.WriteLine(SR.Format(SR.GeneratingFiles));

        //    CodeGenOutputModule outputModule = new CodeGenOutputModule(options, inputModule.MetadataDocuments);
        //    outputModule.Save(importModule.CodeCompileUnit, importModule.Configuration, importModule.ConfigWasGenerated);
        //}

        //void ExportDownloadedMetadata(InputModule inputModule)
        //{
        //    ToolConsole.WriteLine(SR.Format(SR.SavingDownloadedMetadata));

        //    MetadataOutputModule outputModule = new MetadataOutputModule(options);
        //    outputModule.SaveMetadata(inputModule.MetadataDocuments);
        //}

        //void ImportDataContracts(InputModule inputModule)
        //{
        //    ImportModule importModule = new ImportModule(options, null, inputModule.MetadataDocuments);
        //    importModule.ImportDataContracts();

        //    ToolConsole.WriteLine(SR.Format(SR.GeneratingFiles));

        //    CodeGenOutputModule outputModule = new CodeGenOutputModule(options, inputModule.MetadataDocuments);
        //    outputModule.Save(importModule.CodeCompileUnit,
        //        null /* configuration */, false /* configWasGenerated */);
        //}

        //void ExportDataContracts(InputModule inputModule)
        //{
        //    ExportModule exportModule = new ExportModule(options);
        //    exportModule.ExportDataContracts(inputModule.Assemblies);

        //    MetadataOutputModule outputModule = new MetadataOutputModule(options);
        //    outputModule.SaveMetadata(exportModule.GetGeneratedMetadata());
        //}

        //ToolExitCodes ValidateServices(InputModule inputModule)
        //{
        //    ExportModule exportModule = new ExportModule(options);
        //    return exportModule.ValidateTypes(inputModule.Assemblies);
        //}

        //ToolExitCodes ExportServiceContracts(InputModule inputModule)
        //{
        //    ToolExitCodes exitCode;
        //    ExportModule exportModule = new ExportModule(options);
        //    ToolConsole.WriteLine(SR.Format(SR.GeneratingMetadata));
            
        //    exitCode = exportModule.ExportMetadata(inputModule.Assemblies);

        //    if (!exportModule.MetadataWasGenerated)
        //    {
        //        ToolConsole.WriteWarning(SR.Format(SR.NoMetadataWasGenerated, Options.Cmd.ServiceName, Options.Cmd.DataContractOnly));
        //    }
        //    else
        //    {
        //        MetadataOutputModule outputModule = new MetadataOutputModule(options);
        //        outputModule.SaveMetadata(exportModule.GetGeneratedMetadata());
        //    }
        //    return exitCode;
        //}

        ToolExitCodes GenerateSerializer(InputModule inputModule)
        {
            ToolConsole.WriteLine(SR.Format(SR.GeneratingSerializer));
            XmlSerializerGenerator generator = new XmlSerializerGenerator(options);
            generator.GenerateCode(inputModule.Assemblies);
            return ToolExitCodes.Success;
        }
    }
}
