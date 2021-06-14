// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Web.Services.Configuration
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class XmlFormatExtensionAttribute : Attribute {
        private Type[] _types;
        private string _name;
        private string _ns;

        public XmlFormatExtensionAttribute() {
        }

        public XmlFormatExtensionAttribute(string elementName, string ns, Type extensionPoint1) : this(elementName, ns, new Type[] { extensionPoint1 }) {
        }

        public XmlFormatExtensionAttribute(string elementName, string ns, Type extensionPoint1, Type extensionPoint2) : this(elementName, ns, new Type[] { extensionPoint1, extensionPoint2 }) {
        }

        public XmlFormatExtensionAttribute(string elementName, string ns, Type extensionPoint1, Type extensionPoint2, Type extensionPoint3) : this(elementName, ns, new Type[] { extensionPoint1, extensionPoint2, extensionPoint3 }) {
        }

        public XmlFormatExtensionAttribute(string elementName, string ns, Type extensionPoint1, Type extensionPoint2, Type extensionPoint3, Type extensionPoint4) : this(elementName, ns, new Type[] { extensionPoint1, extensionPoint2, extensionPoint3, extensionPoint4 }) {
        }

        public XmlFormatExtensionAttribute(string elementName, string ns, Type[] extensionPoints) {
            _name = elementName;
            _ns = ns;
            _types = extensionPoints;
        }

        public Type[] ExtensionPoints {
            get { return _types == null ? new Type[0] : _types; }
            set { _types = value; }
        }

        public string Namespace {
            get { return _ns == null ? string.Empty : _ns; }
            set { _ns = value; }
        }

        public string ElementName {
            get { return _name == null ? string.Empty : _name; }
            set { _name = value; }
        }
    }
}
