//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace System.ServiceModel.Channels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.Xml;
    using System.ServiceModel.Description;
    using System.Net.Security;

    static class ContextBindingElementPolicy
    {
        const string EncryptAndSignName = "EncryptAndSign";
        const string HttpNamespace = "http://schemas.xmlsoap.org/soap/http";
        const string HttpUseCookieName = "HttpUseCookie";
        const string IncludeContextName = "IncludeContext";
        const string NoneName = "None";
        const string ProtectionLevelName = "ProtectionLevel";
        const string SignName = "Sign";
        const string WscNamespace = "http://schemas.microsoft.com/ws/2006/05/context";
        static XmlDocument document;

        static XmlDocument Document
        {
            get
            {
                if (document == null)
                {
                    document = new XmlDocument();
                }

                return document;
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

        static bool ContainOnlyWhitespaceChild(XmlElement parent)
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
