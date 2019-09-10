#if disabled
//-----------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (C) Microsoft Corporation. All Rights Reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.ServiceModel;
using System.ServiceModel.Description;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class FaultSerializerFixup : MetadataFixup
    {
        public FaultSerializerFixup(WsdlImporter importer)
            : base(importer) { }

        public override void Fixup()
        {
            FaultImportOptions options = null;
            if (!TryGetOptions(out options))
            {
                importer.State.Add(typeof(FaultImportOptions), SetFaultFormatToMessageFormat());
            }
        }

        bool TryGetOptions(out FaultImportOptions options)
        {
            options = null;

            object o = null;
            if (importer.State.TryGetValue(typeof(FaultImportOptions), out o))
            {
                options = o as FaultImportOptions;
            }
            return options != null;
        }

        static FaultImportOptions SetFaultFormatToMessageFormat()
        {
            FaultImportOptions faultImportOptions = new FaultImportOptions();
            faultImportOptions.UseMessageFormat = true;
            return faultImportOptions;
        }
    }
}
#endif