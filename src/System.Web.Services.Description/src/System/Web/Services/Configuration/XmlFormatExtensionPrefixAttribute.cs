// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Web.Services.Configuration
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class XmlFormatExtensionPrefixAttribute : Attribute {
        private string _prefix;
        private string _ns;

        public XmlFormatExtensionPrefixAttribute() {
        }

        public XmlFormatExtensionPrefixAttribute(string prefix, string ns) {
            _prefix = prefix;
            _ns = ns;
        }

        public string Prefix {
            get { return _prefix == null ? string.Empty : _prefix; }
            set { _prefix = value; }
        }

        public string Namespace {
            get { return _ns == null ? string.Empty : _ns; }
            set { _ns = value; }
        }
    }
}
