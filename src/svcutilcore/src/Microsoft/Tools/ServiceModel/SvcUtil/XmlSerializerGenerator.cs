//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
namespace Microsoft.Tools.ServiceModel.SvcUtil
{
    using System;
    using System.ServiceModel.Description;
    using System.Reflection;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Globalization;
    using System.Text;
    using System.Xml.Serialization;
    using System.CodeDom.Compiler;
    using System.ServiceModel;

    class XmlSerializerGenerator : OutputModule
    {
        const string sourceExtension = ".cs";
        readonly ExportModule.IsTypeExcludedDelegate isTypeExcluded;

        string outFile;

        internal XmlSerializerGenerator(Options options)
            : base(options)
        {
            this.isTypeExcluded = options.IsTypeExcluded;
            outFile = options.OutputFileArg;
        }

        internal void GenerateCode(List<Assembly> assemblies)
        {
            if (!string.IsNullOrEmpty(outFile) && assemblies.Count > 1)
            {
                ToolConsole.WriteWarning(SR.Format(SR.WrnOptionConflictsWithInput, Options.Cmd.Out));
                outFile = null;
            }

            foreach (Assembly assembly in assemblies)
            {
                GenerateCode(assembly);
            }
        }

        void GenerateCode(Assembly assembly)
        {
            List<XmlMapping> mappings = new List<XmlMapping>();
            List<Type> types = CollectXmlSerializerTypes(assembly, mappings);

            if (types.Count == 0)
            {
                ToolConsole.WriteWarning(SR.Format(SR.WrnNoServiceContractTypes, assembly.GetName().CodeBase));
                return;
            }
            if (mappings.Count == 0)
            {
                ToolConsole.WriteWarning(SR.Format(SR.WrnNoXmlSerializerOperationBehavior, assembly.GetName().CodeBase));
                return;
            }

            bool success = false;
            bool toDeleteFile = true;

            string codePath = Path.GetTempFileName();

            try
            {
                if (File.Exists(codePath))
                {
                    File.Delete(codePath);
                }

                using (FileStream fs = File.Create(codePath))
                {
                    MethodInfo method = typeof(System.Xml.Serialization.XmlSerializer).GetMethod("GenerateSerializer", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    if (method == null)
                    {
                        throw new PlatformNotSupportedException();
                    }
                    else
                    {
                        success = (bool)method.Invoke(null, new object[] { types, mappings, fs });
                    }
                }
            }
            finally
            {
                if (!success && toDeleteFile && File.Exists(codePath))
                {
                    File.Delete(codePath);
                }
            }

            return;
        }

        List<Type> CollectXmlSerializerTypes(Assembly assembly, List<XmlMapping> mappings)
        {
            List<Type> types = new List<Type>();

            ExportModule.ContractLoader contractLoader = new ExportModule.ContractLoader(new Assembly[] { assembly }, this.isTypeExcluded);
            contractLoader.ContractLoadErrorCallback = delegate(Type contractType, string errorMessage)
                    {
                        ToolConsole.WriteWarning(SR.Format(SR.WrnUnableToLoadContractForSGen, contractType, errorMessage));
                    };

            foreach (ContractDescription contract in contractLoader.GetContracts())
            {
                types.Add(contract.ContractType);
                foreach (OperationDescription operation in contract.Operations)
                {
                    XmlSerializerOperationBehavior behavior = operation.Behaviors.Find<XmlSerializerOperationBehavior>();
                    if (behavior != null)
                    {
                        foreach (XmlMapping map in behavior.GetXmlMappings())
                        {
                            mappings.Add(map);
                        }
                    }
                }
            }
            return types;
        }
    }
}
