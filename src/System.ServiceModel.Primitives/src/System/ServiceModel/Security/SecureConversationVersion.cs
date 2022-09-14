// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Xml;

namespace System.ServiceModel.Security
{
    public abstract class SecureConversationVersion
    {
        private readonly XmlDictionaryString _prefix;

        internal SecureConversationVersion(XmlDictionaryString ns, XmlDictionaryString prefix)
        {
            Namespace = ns;
            _prefix = prefix;
        }

        public XmlDictionaryString Namespace { get; }

        public XmlDictionaryString Prefix
        {
            get
            {
                return _prefix;
            }
        }

        public static SecureConversationVersion Default
        {
            get { return WSSecureConversationFeb2005; }
        }

        public static SecureConversationVersion WSSecureConversationFeb2005
        {
            get { return WSSecureConversationVersionFeb2005.Instance; }
        }

        public static SecureConversationVersion WSSecureConversation13
        {
            get { return WSSecureConversationVersion13.Instance; }
        }

        internal class WSSecureConversationVersionFeb2005 : SecureConversationVersion
        {
            private static readonly WSSecureConversationVersionFeb2005 s_instance = new WSSecureConversationVersionFeb2005();

            protected WSSecureConversationVersionFeb2005()
                : base(XD.SecureConversationFeb2005Dictionary.Namespace, XD.SecureConversationFeb2005Dictionary.Prefix)
            {
            }

            public static SecureConversationVersion Instance
            {
                get
                {
                    return s_instance;
                }
            }
        }

        internal class WSSecureConversationVersion13 : SecureConversationVersion
        {
            private static readonly WSSecureConversationVersion13 s_instance = new WSSecureConversationVersion13();

            protected WSSecureConversationVersion13()
                : base(DXD.SecureConversationDec2005Dictionary.Namespace, DXD.SecureConversationDec2005Dictionary.Prefix)
            {
            }

            public static SecureConversationVersion Instance
            {
                get
                {
                    return s_instance;
                }
            }
        }
    }
}
