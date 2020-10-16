// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Channels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.Xml;
    using System.ServiceModel.Description;
    using System.Net.Security;

    internal static class ContextBindingElementPolicy
    {
        private const string EncryptAndSignName = "EncryptAndSign";
        private const string HttpNamespace = "http://schemas.xmlsoap.org/soap/http";
        private const string HttpUseCookieName = "HttpUseCookie";
        private const string IncludeContextName = "IncludeContext";
        private const string NoneName = "None";
        private const string ProtectionLevelName = "ProtectionLevel";
        private const string SignName = "Sign";
        private const string WscNamespace = "http://schemas.microsoft.com/ws/2006/05/context";
        private static XmlDocument s_document;

        private static XmlDocument Document
        {
            get
            {
                if (s_document == null)
                {
                    s_document = new XmlDocument();
                }

                return s_document;
            }
        }

        public static bool TryGetHttpUseCookieAssertion(ICollection<XmlElement> assertions, out XmlElement httpUseCookieAssertion)
        {
            if (assertions == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("assertions");
            }

            httpUseCookieAssertion = null;

            foreach (XmlElement assertion in assertions)
            {
                if (assertion.LocalName == HttpUseCookieName
                    && assertion.NamespaceURI == HttpNamespace
                    && assertion.ChildNodes.Count == 0)
                {
                    httpUseCookieAssertion = assertion;
                    break;
                }
            }

            return httpUseCookieAssertion != null;
        }

        private static bool ContainOnlyWhitespaceChild(XmlElement parent)
        {
            if (parent.ChildNodes.Count == 0)
            {
                return true;
            }

            foreach (XmlNode node in parent.ChildNodes)
            {
                if (!(node is XmlWhitespace))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool TryImportRequireContextAssertion(PolicyAssertionCollection assertions, out ContextBindingElement bindingElement)
        {
            if (assertions == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("assertions");
            }

            bindingElement = null;

            foreach (XmlElement assertion in assertions)
            {
                if (assertion.LocalName == IncludeContextName
                    && assertion.NamespaceURI == WscNamespace
                    && ContainOnlyWhitespaceChild(assertion))
                {
                    string protectionLevelAttribute = assertion.GetAttribute(ProtectionLevelName);
                    if (EncryptAndSignName.Equals(protectionLevelAttribute, StringComparison.Ordinal))
                    {
                        bindingElement = new ContextBindingElement(ProtectionLevel.EncryptAndSign);
                    }
                    else if (SignName.Equals(protectionLevelAttribute, StringComparison.Ordinal))
                    {
                        bindingElement = new ContextBindingElement(ProtectionLevel.Sign);
                    }
                    else if (NoneName.Equals(protectionLevelAttribute, StringComparison.Ordinal))
                    {
                        bindingElement = new ContextBindingElement(ProtectionLevel.None);
                    }

                    if (bindingElement != null)
                    {
                        assertions.Remove(assertion);
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
