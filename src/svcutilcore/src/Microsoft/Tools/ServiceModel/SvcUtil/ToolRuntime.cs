// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// DevNote: See how there are no using statements needed here? 
//          That's part of the plan. Let's keep it that way.

namespace Microsoft.Tools.ServiceModel.SvcUtil.XmlSerializer
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
            else if(!_options.Quiet)
            {
                ToolConsole.WriteWarning(System.SR.WrnToolIsUsedDirectly);
                return ToolExitCodes.InputError;
            }
            else
            {
                InputModule inputModule = InputModule.LoadInputs(_options);

                Tool.Assert(_options.GetToolMode().HasValue, System.SR.Format(System.SR.AmbiguousToolUseage));
                return ExecuteToolMode(inputModule);
            }
        }

        private ToolExitCodes ExecuteToolMode(InputModule inputModule)
        {
            GenerateSerializer(inputModule);
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
