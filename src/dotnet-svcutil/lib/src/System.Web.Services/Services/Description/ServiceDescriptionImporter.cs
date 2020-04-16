// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Web.Services.Description
{
    using System.Web.Services;
    using System.Web.Services.Protocols;
    using Microsoft.Xml.Serialization;
    using Microsoft.Xml.Schema;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Collections.Generic;
    using System;
    using System.Reflection;
    using Microsoft.CodeDom.Compiler;
    using System.Web.Services.Configuration;
    using Microsoft.Xml;
    using Microsoft.CodeDom;
    using System.Globalization;
    using System.Runtime.InteropServices;

    /// <include file='doc\ServiceDescriptionImporter.uex' path='docs/doc[@for="ServiceDescriptionImportStyle"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public enum ServiceDescriptionImportStyle
    {
        /// <include file='doc\ServiceDescriptionImporter.uex' path='docs/doc[@for="ServiceDescriptionImportStyle.Client"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [XmlEnum("client")]
        Client,
        /// <include file='doc\ServiceDescriptionImporter.uex' path='docs/doc[@for="ServiceDescriptionImportStyle.Server"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [XmlEnum("server")]
        Server,
        /// <include file='doc\ServiceDescriptionImporter.uex' path='docs/doc[@for="ServiceDescriptionImportStyle.ServerInterface"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [XmlEnum("serverInterface")]
        ServerInterface,
    }
}
