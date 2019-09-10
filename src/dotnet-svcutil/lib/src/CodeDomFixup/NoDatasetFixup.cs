#if disabled // TODO (miguell): none of these types exists in System.Data in DNX

//-----------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (C) Microsoft Corporation. All Rights Reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;

    internal class NoDatasetFixup : MetadataFixup
    {
        static readonly string[] extensionNames = new string[]
                {
                    typeof(System.Data.Design.TypedDataSetSchemaImporterExtensionFx35).AssemblyQualifiedName,
                    typeof(System.Data.Design.TypedDataSetSchemaImporterExtension).AssemblyQualifiedName,
                    typeof(System.Data.DataSetSchemaImporterExtension).AssemblyQualifiedName,
                };

        public NoDatasetFixup(WsdlImporter importer)
            : base(importer) { }

        public override void Fixup()
        {
            XmlSerializerImportOptions options = null;
            if (TryGetOptions(out options))
            {
                CollectionHelpers.MapList<string>(
                    options.WebReferenceOptions.SchemaImporterExtensions,
                    delegate(string extensionName) { return !IsDatasetExtension(extensionName); },
                    null
                );
            }
        }

        private bool TryGetOptions(out XmlSerializerImportOptions options)
        {
            options = null;

            object o = null;
            if (importer.State.TryGetValue(typeof(XmlSerializerImportOptions), out o))
            {
                options = o as XmlSerializerImportOptions;
            }
            return options != null;
        }

        static bool IsDatasetExtension(string extensionName)
        {
            for (int i = 0; i < extensionNames.Length; i++)
            {
                if (extensionNames[i] == extensionName)
                    return true;
            }
            return false;
        }
    }
}
#endif