// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.Generic;
using System.Xml;
using static System.IdentityModel.Selectors.SecurityTokenSerializer;

namespace System.ServiceModel.Security
{
    internal class WSSecureConversationFeb2005 : WSSecureConversation
    {
        public WSSecureConversationFeb2005(WSSecurityTokenSerializer tokenSerializer, SecurityStateEncoder securityStateEncoder, IEnumerable<Type> knownTypes,
            int maxKeyDerivationOffset, int maxKeyDerivationLabelLength, int maxKeyDerivationNonceLength)
            : base(tokenSerializer, maxKeyDerivationOffset, maxKeyDerivationLabelLength, maxKeyDerivationNonceLength)
        {
            if (securityStateEncoder != null)
            {
                throw ExceptionHelper.PlatformNotSupported();
            }
        }

        public override SecureConversationDictionary SerializerDictionary
        {
            get { return XD.SecureConversationFeb2005Dictionary; }
        }

        public class DriverFeb2005 : Driver
        {
            public DriverFeb2005()
            {
            }

            protected override SecureConversationDictionary DriverDictionary
            {
                get { return XD.SecureConversationFeb2005Dictionary; }
            }

            public override XmlDictionaryString CloseAction
            {
                get { return XD.SecureConversationFeb2005Dictionary.RequestSecurityContextClose; }
            }

            public override XmlDictionaryString CloseResponseAction
            {
                get { return XD.SecureConversationFeb2005Dictionary.RequestSecurityContextCloseResponse; }
            }

            public override bool IsSessionSupported
            {
                get { return true; }
            }

            public override XmlDictionaryString RenewAction
            {
                get { return XD.SecureConversationFeb2005Dictionary.RequestSecurityContextRenew; }
            }

            public override XmlDictionaryString RenewResponseAction
            {
                get { return XD.SecureConversationFeb2005Dictionary.RequestSecurityContextRenewResponse; }
            }

            public override XmlDictionaryString Namespace
            {
                get { return XD.SecureConversationFeb2005Dictionary.Namespace; }
            }

            public override string TokenTypeUri
            {
                get { return XD.SecureConversationFeb2005Dictionary.SecurityContextTokenType.Value; }
            }
        }
    }
}
