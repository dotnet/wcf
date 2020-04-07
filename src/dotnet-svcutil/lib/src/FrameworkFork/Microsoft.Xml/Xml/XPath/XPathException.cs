// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Xml.XPath {

    using System;
    using System.IO;
    using System.Resources;
    using System.Diagnostics;
    // using System.Security.Permissions;

    // Represents the exception that is thrown when there is error processing an
    // XPath expression.
    // [Serializable],
    public class XPathException : System.Exception {
        // we need to keep this members for V1 serialization compatibility
        string   res;
        string[] args;

        // message != null for V1 & V2 exceptions deserialized in Whidbey
        // message == null for created V2 exceptions; the exception message is stored in Exception._message
        string   message = null;

        public XPathException() : this (string.Empty, (Exception) null) {}

        public XPathException(string message) : this (message, (Exception) null) {}

        public XPathException(string message, Exception innerException) : 
            this(ResXml.Xml_UserException, new string[] { message }, innerException) {
        }

        internal static XPathException Create(string res) {
            return new XPathException(res, (string[])null);
        }

        internal static XPathException Create(string res, string arg) {
            return new XPathException(res, new string[] { arg });
        }
            
        internal static XPathException Create(string res, string arg, string arg2) {
            return new XPathException(res, new string[] { arg, arg2 });
        }
            
        internal static XPathException Create(string res, string arg, Exception innerException) {
            return new XPathException(res, new string[] { arg }, innerException);
        }
            
        private XPathException(string res, string[] args) :
            this(res, args, null) {
        }

        private XPathException(string res, string[] args, Exception inner) :
            base(CreateMessage(res, args), inner) {
            HResult = HResults.XmlXPath;
            this.res = res;
            this.args = args;
        }

        private static string CreateMessage(string res, string[] args) { 
            try {
                string message = ResXml.GetString(res, args);
                if (message == null)
                    message = "UNKNOWN("+res+")";
                return message;
            }
            catch ( MissingManifestResourceException ) {
                return "UNKNOWN("+res+")";
            }
        }

        public override string Message {
            get {
                return (message == null) ? base.Message : message;
            }
        }
    }
}
