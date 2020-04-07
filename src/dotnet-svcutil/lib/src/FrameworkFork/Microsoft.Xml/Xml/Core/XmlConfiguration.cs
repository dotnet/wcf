// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
using System;
// using System.Configuration;
using System.Globalization;
using Microsoft.Xml;

namespace Microsoft.Xml.XmlConfiguration {
    internal static class XmlConfigurationString {
        internal const string XmlReaderSectionName = "xmlReader";
        internal const string XsltSectionName = "xslt";

        internal const string ProhibitDefaultResolverName = "prohibitDefaultResolver";
        internal const string LimitXPathComplexityName = "limitXPathComplexity";
        internal const string EnableMemberAccessForXslCompiledTransformName = "enableMemberAccessForXslCompiledTransform";

        internal const string XmlConfigurationSectionName = "system.xml";

        internal static string XmlReaderSectionPath = string.Format(CultureInfo.InvariantCulture, @"{0}/{1}", XmlConfigurationSectionName, XmlReaderSectionName);
        internal static string XsltSectionPath = string.Format(CultureInfo.InvariantCulture, @"{0}/{1}", XmlConfigurationSectionName, XsltSectionName);
    }

    public sealed class XmlReaderSection {
        internal static XmlResolver CreateDefaultResolver() {
            return new XmlUrlResolver();
        }
    }
}
