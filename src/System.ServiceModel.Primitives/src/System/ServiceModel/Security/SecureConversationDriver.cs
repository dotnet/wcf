// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Xml;

namespace System.ServiceModel.Security
{
    internal abstract class SecureConversationDriver
    {
        public virtual XmlDictionaryString CloseAction
        {
            get
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.SecureConversationDriverVersionDoesNotSupportSession));
            }
        }

        public virtual XmlDictionaryString CloseResponseAction
        {
            get
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.SecureConversationDriverVersionDoesNotSupportSession));
            }
        }

        public virtual bool IsSessionSupported
        {
            get
            {
                return false;
            }
        }

        public abstract XmlDictionaryString IssueAction { get; }

        public abstract XmlDictionaryString IssueResponseAction { get; }

        public virtual XmlDictionaryString RenewAction
        {
            get
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.SecureConversationDriverVersionDoesNotSupportSession));
            }
        }

        public virtual XmlDictionaryString RenewResponseAction
        {
            get
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.SecureConversationDriverVersionDoesNotSupportSession));
            }
        }

        public abstract XmlDictionaryString Namespace { get; }

        public abstract XmlDictionaryString RenewNeededFaultCode { get; }

        public abstract XmlDictionaryString BadContextTokenFaultCode { get; }

        public abstract string TokenTypeUri { get; }

        public abstract UniqueId GetSecurityContextTokenId(XmlDictionaryReader reader);
        public abstract bool IsAtSecurityContextToken(XmlDictionaryReader reader);
    }
}
