// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// DevNote: See how there are no using statements needed here? 
//          That's part of the plan. Let's keep it that way.

namespace Microsoft.Tools.ServiceModel.SvcUtil
{
    internal class ToolRuntime
    {
        private Options _options;

        internal ToolRuntime(Options options)
        {
            _options = options;
        }

        internal ToolExitCodes Run()
        {
            if (!_options.NoLogo)
                ToolConsole.WriteHeader();

            if (_options.GetToolMode() == ToolMode.DisplayHelp)
            {
                ToolConsole.WriteHelpText();
                return ToolExitCodes.Success;
            }
            else
            {
                InputModule inputModule = InputModule.LoadInputs(_options);

                Tool.Assert(_options.GetToolMode().HasValue, System.SR.Format(System.SR.AmbiguousToolUseage, Options.Cmd.Target, Options.Cmd.Validate));
                ToolMode toolMode = _options.GetToolMode().Value;

                return ExecuteToolMode(toolMode, inputModule);
            }
        }

        private ToolExitCodes ExecuteToolMode(ToolMode toolMode, InputModule inputModule)
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
                    throw new System.ArgumentException(System.SR.ErrInvalidTarget);
            }

            return ToolExitCodes.Success;
        }

        private ToolExitCodes GenerateSerializer(InputModule inputModule)
        {
            ToolConsole.WriteLine(System.SR.Format(System.SR.GeneratingSerializer));
            XmlSerializerGenerator generator = new XmlSerializerGenerator(_options);
            generator.GenerateCode(inputModule.Assemblies);
            return ToolExitCodes.Success;
        }
    }
}
