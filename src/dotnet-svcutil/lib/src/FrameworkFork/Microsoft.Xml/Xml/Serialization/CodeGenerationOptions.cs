// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.


namespace Microsoft.Xml.Serialization {
				using System;
				using Microsoft.Xml;
    using System.ComponentModel;

    /// <include file='doc\CodeGenerationOptions.uex' path='docs/doc[@for="CodeGenerationOptions"]/*' />
    /// <devdoc>
    ///    Specifies varoius flavours of XmlCodeExporter generated code.
    /// </devdoc>
    [Flags]
    public enum CodeGenerationOptions
    {
        /// <include file='doc\CodeGenerationOptions.uex' path='docs/doc[@for="CodeGenerationOptions.None"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Default: use clr primitives for xsd primitives, generate fields and arrays.
        ///    </para>
        /// </devdoc>
        [XmlIgnore]
        None = 0,
        /// <include file='doc\CodeGenerationOptions.uex' path='docs/doc[@for="CodeGenerationOptions.GenerateProperties"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Generate propertyes instead of fields.
        ///    </para>
        /// </devdoc>
        [XmlEnum("properties")]
        GenerateProperties = 0x1,

        /// <include file='doc\CodeGenerationOptions.uex' path='docs/doc[@for="CodeGenerationOptions.GenerateNewAsync"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Generate new RAD asynchronous pattern. The feature allows customers to use an event-based model for invoking Web services asynchronously.
        ///    </para>
        /// </devdoc>
        [XmlEnum("newAsync")]
        GenerateNewAsync = 0x2,

        /// <include file='doc\CodeGenerationOptions.uex' path='docs/doc[@for="CodeGenerationOptions.GenerateOldAsync"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Generate old asynchronous pattern: BeginXXX/EndXXX.
        ///    </para>
        /// </devdoc>
        [XmlEnum("oldAsync")]
        GenerateOldAsync = 0x4,


        /// <include file='doc\CodeGenerationOptions.uex' path='docs/doc[@for="CodeGenerationOptions.GenerateOrder"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Generate OM using explicit ordering feature.
        ///    </para>
        /// </devdoc>
        [XmlEnum("order")]
        GenerateOrder = 0x08,
        
        /// <include file='doc\CodeGenerationOptions.uex' path='docs/doc[@for="CodeGenerationOptions.EnableDataBinding"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Generate OM INotifyPropertyChanged interface to enable data binding.
        ///    </para>
        /// </devdoc>
        [XmlEnum("enableDataBinding")]
        EnableDataBinding = 0x10,
    }
}
