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
                case ToolMode.XmlSerializerGeneration:
                    GenerateSerializer(inputModule);
                    break;
                case ToolMode.DataContractImport:
                case ToolMode.ProxyGeneration:
                case ToolMode.ServiceContractGeneration:
                case ToolMode.DataContractExport:
                case ToolMode.Validate:
                case ToolMode.MetadataFromAssembly:
                case ToolMode.WSMetadataExchange:
                    throw new ArgumentException(SR.ErrInvalidTarget);
            }

            return ToolExitCodes.Success;
        }

        ToolExitCodes GenerateSerializer(InputModule inputModule)
        {
            ToolConsole.WriteLine(SR.Format(SR.GeneratingSerializer));
            XmlSerializerGenerator generator = new XmlSerializerGenerator(options);
            generator.GenerateCode(inputModule.Assemblies);
            return ToolExitCodes.Success;
        }
    }
}
