// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Channels
{
    using System;
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
    using Microsoft.Xml;
    using Microsoft.Xml.Schema;
    using Microsoft.Xml.Serialization;
    using WsdlNS = System.Web.Services.Description;

    public class XmlSerializerImportOptions
    {
        private CodeCompileUnit _codeCompileUnit;
        private CodeDomProvider _codeProvider;
        private string _clrNamespace;
        private WsdlNS.WebReferenceOptions _webReferenceOptions;
        private static CodeGenerationOptions s_defaultCodeGenerationOptions = CodeGenerationOptions.GenerateProperties | CodeGenerationOptions.GenerateOrder;

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

        public WsdlNS.WebReferenceOptions WebReferenceOptions
        {
            get
            {
                if (_webReferenceOptions == null)
                {
                    _webReferenceOptions = new WsdlNS.WebReferenceOptions();
                    _webReferenceOptions.CodeGenerationOptions = s_defaultCodeGenerationOptions;
                }
                return _webReferenceOptions;
            }
            set { _webReferenceOptions = value; }
        }
    }
}

