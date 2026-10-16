// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Channels
{
    using System;
    using System.Collections.Specialized;
    using Microsoft.CodeDom.Compiler;
    using Microsoft.CodeDom;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.ServiceModel;
    using System.Runtime.Serialization;
    using System.Xml;
    using System.Xml.Schema;
    using Microsoft.Tools.ServiceModel.Svcutil.XmlSerializer;

    public class XmlSerializerImportOptions
    {
        private CodeCompileUnit _codeCompileUnit;
        private CodeDomProvider _codeProvider;
        private string _clrNamespace;
        private CodeGenerationOptions _codeGenerationOptions = CodeGenerationOptions.GenerateProperties | CodeGenerationOptions.GenerateOrder;
        private StringCollection _schemaImporterExtensions;

        public XmlSerializerImportOptions()
            : this(new CodeCompileUnit())
        {
        }

        public XmlSerializerImportOptions(CodeCompileUnit codeCompileUnit)
        {
            _codeCompileUnit = codeCompileUnit;
        }

        public CodeCompileUnit CodeCompileUnit
        {
            get
            {
                if (_codeCompileUnit == null)
                    _codeCompileUnit = new CodeCompileUnit();
                return _codeCompileUnit;
            }
        }

        public CodeDomProvider CodeProvider
        {
            get
            {
                if (_codeProvider == null)
                    _codeProvider = CodeDomProvider.CreateProvider("C#");
                return _codeProvider;
            }
            set { _codeProvider = value; }
        }

        public string ClrNamespace
        {
            get { return _clrNamespace; }
            set { _clrNamespace = value; }
        }

        public CodeGenerationOptions CodeGenerationOptions
        {
            get { return _codeGenerationOptions; }
            set { _codeGenerationOptions = value; }
        }

        public StringCollection SchemaImporterExtensions
        {
            get
            {
                if (_schemaImporterExtensions == null)
                {
                    _schemaImporterExtensions = new StringCollection();
                }

                return _schemaImporterExtensions;
            }
        }
    }
}

