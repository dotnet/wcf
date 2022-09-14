// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Xml;

namespace System.IdentityModel
{
    internal class DictionaryManager
    {
        private IXmlDictionary _parentDictionary;

        public DictionaryManager()
        {
            SamlDictionary = XD.SamlDictionary;
            XmlSignatureDictionary = XD.XmlSignatureDictionary;
            UtilityDictionary = XD.UtilityDictionary;
            ExclusiveC14NDictionary = XD.ExclusiveC14NDictionary;
            SecurityAlgorithmDictionary = XD.SecurityAlgorithmDictionary;
            _parentDictionary = XD.Dictionary;
            SecurityJan2004Dictionary = XD.SecurityJan2004Dictionary;
            SecurityJanXXX2005Dictionary = XD.SecurityXXX2005Dictionary;
            SecureConversationFeb2005Dictionary = XD.SecureConversationFeb2005Dictionary;
            TrustFeb2005Dictionary = XD.TrustFeb2005Dictionary;
            XmlEncryptionDictionary = XD.XmlEncryptionDictionary;

            // These 3 are factored into a separate dictionary in ServiceModel under DXD. 
            SecureConversationDec2005Dictionary = XD.SecureConversationDec2005Dictionary;
            SecurityAlgorithmDec2005Dictionary = XD.SecurityAlgorithmDec2005Dictionary;
            TrustDec2005Dictionary = XD.TrustDec2005Dictionary;
        }

        public DictionaryManager(IXmlDictionary parentDictionary)
        {
            SamlDictionary = new SamlDictionary(parentDictionary);
            XmlSignatureDictionary = new XmlSignatureDictionary(parentDictionary);
            UtilityDictionary = new UtilityDictionary(parentDictionary);
            ExclusiveC14NDictionary = new ExclusiveC14NDictionary(parentDictionary);
            SecurityAlgorithmDictionary = new SecurityAlgorithmDictionary(parentDictionary);
            SecurityJan2004Dictionary = new SecurityJan2004Dictionary(parentDictionary);
            SecurityJanXXX2005Dictionary = new SecurityXXX2005Dictionary(parentDictionary);
            SecureConversationFeb2005Dictionary = new SecureConversationFeb2005Dictionary(parentDictionary);
            TrustFeb2005Dictionary = new TrustFeb2005Dictionary(parentDictionary);
            XmlEncryptionDictionary = new XmlEncryptionDictionary(parentDictionary);
            _parentDictionary = parentDictionary;

            // These 3 are factored into a separate dictionary in ServiceModel under DXD. 
            // ServiceModel should set these separately using the property setters.
            SecureConversationDec2005Dictionary = XD.SecureConversationDec2005Dictionary;
            SecurityAlgorithmDec2005Dictionary = XD.SecurityAlgorithmDec2005Dictionary;
            TrustDec2005Dictionary = XD.TrustDec2005Dictionary;
        }

        public SamlDictionary SamlDictionary { get; set; }

        public XmlSignatureDictionary XmlSignatureDictionary { get; set; }

        public UtilityDictionary UtilityDictionary { get; set; }

        public ExclusiveC14NDictionary ExclusiveC14NDictionary { get; set; }

        public SecurityAlgorithmDec2005Dictionary SecurityAlgorithmDec2005Dictionary { get; set; }

        public SecurityAlgorithmDictionary SecurityAlgorithmDictionary { get; set; }

        public SecurityJan2004Dictionary SecurityJan2004Dictionary { get; set; }

        public SecurityXXX2005Dictionary SecurityJanXXX2005Dictionary { get; set; }

        public SecureConversationDec2005Dictionary SecureConversationDec2005Dictionary { get; set; }

        public SecureConversationFeb2005Dictionary SecureConversationFeb2005Dictionary { get; set; }

        public TrustDec2005Dictionary TrustDec2005Dictionary { get; set; }

        public TrustFeb2005Dictionary TrustFeb2005Dictionary { get; set; }

        public XmlEncryptionDictionary XmlEncryptionDictionary { get; set; }

        public IXmlDictionary ParentDictionary
        {
            get { return _parentDictionary; }
            set { _parentDictionary = value; }
        }
    }
}
