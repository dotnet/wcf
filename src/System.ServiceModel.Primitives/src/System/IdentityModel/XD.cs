// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


// NOTE: this file was generated from 'xd.xml'

using System.ServiceModel;
using System.Xml;

namespace System.IdentityModel
{
    // Static Xml Dictionary
    internal static class XD
    {
        static public IdentityModelDictionary Dictionary { get { return IdentityModelDictionary.CurrentVersion; } }

        private static ExclusiveC14NDictionary s_exclusiveC14NDictionary;
        private static SamlDictionary s_samlDictionary;
        private static SecureConversationDec2005Dictionary s_secureConversationDec2005Dictionary;
        private static SecureConversationFeb2005Dictionary s_secureConversationFeb2005Dictionary;
        private static SecurityAlgorithmDictionary s_securityAlgorithmDictionary;
        private static SecurityAlgorithmDec2005Dictionary s_securityAlgorithmDec2005Dictionary;
        private static SecurityJan2004Dictionary s_securityJan2004Dictionary;
        private static SecurityXXX2005Dictionary s_securityXXX2005Dictionary;
        private static TrustDec2005Dictionary s_trustDec2005Dictionary;
        private static TrustFeb2005Dictionary s_trustFeb2005Dictionary;
        private static UtilityDictionary s_utilityDictionary;
        private static XmlEncryptionDictionary s_xmlEncryptionDictionary;
        private static XmlSignatureDictionary s_xmlSignatureDictionary;

        static public ExclusiveC14NDictionary ExclusiveC14NDictionary
        {
            get
            {
                if (s_exclusiveC14NDictionary == null)
                {
                    s_exclusiveC14NDictionary = new ExclusiveC14NDictionary(Dictionary);
                }

                return s_exclusiveC14NDictionary;
            }
        }

        static public SamlDictionary SamlDictionary
        {
            get
            {
                if (s_samlDictionary == null)
                {
                    s_samlDictionary = new SamlDictionary(Dictionary);
                }

                return s_samlDictionary;
            }
        }

        static public SecureConversationDec2005Dictionary SecureConversationDec2005Dictionary
        {
            get
            {
                if (s_secureConversationDec2005Dictionary == null)
                {
                    s_secureConversationDec2005Dictionary = new SecureConversationDec2005Dictionary(Dictionary);
                }

                return s_secureConversationDec2005Dictionary;
            }
        }

        static public SecureConversationFeb2005Dictionary SecureConversationFeb2005Dictionary
        {
            get
            {
                if (s_secureConversationFeb2005Dictionary == null)
                {
                    s_secureConversationFeb2005Dictionary = new SecureConversationFeb2005Dictionary(Dictionary);
                }

                return s_secureConversationFeb2005Dictionary;
            }
        }

        static public SecurityAlgorithmDictionary SecurityAlgorithmDictionary
        {
            get
            {
                if (s_securityAlgorithmDictionary == null)
                {
                    s_securityAlgorithmDictionary = new SecurityAlgorithmDictionary(Dictionary);
                }

                return s_securityAlgorithmDictionary;
            }
        }

        static public SecurityAlgorithmDec2005Dictionary SecurityAlgorithmDec2005Dictionary
        {
            get
            {
                if (s_securityAlgorithmDec2005Dictionary == null)
                {
                    s_securityAlgorithmDec2005Dictionary = new SecurityAlgorithmDec2005Dictionary(Dictionary);
                }

                return s_securityAlgorithmDec2005Dictionary;
            }
        }

        static public SecurityJan2004Dictionary SecurityJan2004Dictionary
        {
            get
            {
                if (s_securityJan2004Dictionary == null)
                {
                    s_securityJan2004Dictionary = new SecurityJan2004Dictionary(Dictionary);
                }

                return s_securityJan2004Dictionary;
            }
        }

        static public SecurityXXX2005Dictionary SecurityXXX2005Dictionary
        {
            get
            {
                if (s_securityXXX2005Dictionary == null)
                {
                    s_securityXXX2005Dictionary = new SecurityXXX2005Dictionary(Dictionary);
                }

                return s_securityXXX2005Dictionary;
            }
        }

        static public TrustDec2005Dictionary TrustDec2005Dictionary
        {
            get
            {
                if (s_trustDec2005Dictionary == null)
                {
                    s_trustDec2005Dictionary = new TrustDec2005Dictionary(Dictionary);
                }

                return s_trustDec2005Dictionary;
            }
        }

        static public TrustFeb2005Dictionary TrustFeb2005Dictionary
        {
            get
            {
                if (s_trustFeb2005Dictionary == null)
                {
                    s_trustFeb2005Dictionary = new TrustFeb2005Dictionary(Dictionary);
                }

                return s_trustFeb2005Dictionary;
            }
        }

        static public UtilityDictionary UtilityDictionary
        {
            get
            {
                if (s_utilityDictionary == null)
                {
                    s_utilityDictionary = new UtilityDictionary(Dictionary);
                }

                return s_utilityDictionary;
            }
        }

        static public XmlEncryptionDictionary XmlEncryptionDictionary
        {
            get
            {
                if (s_xmlEncryptionDictionary == null)
                {
                    s_xmlEncryptionDictionary = new XmlEncryptionDictionary(Dictionary);
                }

                return s_xmlEncryptionDictionary;
            }
        }

        static public XmlSignatureDictionary XmlSignatureDictionary
        {
            get
            {
                if (s_xmlSignatureDictionary == null)
                {
                    s_xmlSignatureDictionary = new XmlSignatureDictionary(Dictionary);
                }

                return s_xmlSignatureDictionary;
            }
        }
    }

    internal class ExclusiveC14NDictionary
    {
        public XmlDictionaryString Namespace;
        public XmlDictionaryString PrefixList;
        public XmlDictionaryString InclusiveNamespaces;
        public XmlDictionaryString Prefix;

        public ExclusiveC14NDictionary(IdentityModelDictionary dictionary)
        {
            Namespace = dictionary.CreateString(IdentityModelStringsVersion1.String20, 20);
            PrefixList = dictionary.CreateString(IdentityModelStringsVersion1.String21, 21);
            InclusiveNamespaces = dictionary.CreateString(IdentityModelStringsVersion1.String22, 22);
            Prefix = dictionary.CreateString(IdentityModelStringsVersion1.String23, 23);
        }

        public ExclusiveC14NDictionary(IXmlDictionary dictionary)
        {
            Namespace = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String20);
            PrefixList = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String21);
            InclusiveNamespaces = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String22);
            Prefix = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String23);
        }

        private XmlDictionaryString LookupDictionaryString(IXmlDictionary dictionary, string value)
        {
            XmlDictionaryString expectedValue;
            if (!dictionary.TryLookup(value, out expectedValue))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SRP.Format(SRP.XDCannotFindValueInDictionaryString, value));
            }

            return expectedValue;
        }
    }

    internal class SamlDictionary
    {
        public XmlDictionaryString Access;
        public XmlDictionaryString AccessDecision;
        public XmlDictionaryString Action;
        public XmlDictionaryString Advice;
        public XmlDictionaryString Assertion;
        public XmlDictionaryString AssertionId;
        public XmlDictionaryString AssertionIdReference;
        public XmlDictionaryString Attribute;
        public XmlDictionaryString AttributeName;
        public XmlDictionaryString AttributeNamespace;
        public XmlDictionaryString AttributeStatement;
        public XmlDictionaryString AttributeValue;
        public XmlDictionaryString Audience;
        public XmlDictionaryString AudienceRestrictionCondition;
        public XmlDictionaryString AuthenticationInstant;
        public XmlDictionaryString AuthenticationMethod;
        public XmlDictionaryString AuthenticationStatement;
        public XmlDictionaryString AuthorityBinding;
        public XmlDictionaryString AuthorityKind;
        public XmlDictionaryString AuthorizationDecisionStatement;
        public XmlDictionaryString Binding;
        public XmlDictionaryString Condition;
        public XmlDictionaryString Conditions;
        public XmlDictionaryString Decision;
        public XmlDictionaryString DoNotCacheCondition;
        public XmlDictionaryString Evidence;
        public XmlDictionaryString IssueInstant;
        public XmlDictionaryString Issuer;
        public XmlDictionaryString Location;
        public XmlDictionaryString MajorVersion;
        public XmlDictionaryString MinorVersion;
        public XmlDictionaryString Namespace;
        public XmlDictionaryString NameIdentifier;
        public XmlDictionaryString NameIdentifierFormat;
        public XmlDictionaryString NameIdentifierNameQualifier;
        public XmlDictionaryString ActionNamespaceAttribute;
        public XmlDictionaryString NotBefore;
        public XmlDictionaryString NotOnOrAfter;
        public XmlDictionaryString PreferredPrefix;
        public XmlDictionaryString Statement;
        public XmlDictionaryString Subject;
        public XmlDictionaryString SubjectConfirmation;
        public XmlDictionaryString SubjectConfirmationData;
        public XmlDictionaryString SubjectConfirmationMethod;
        public XmlDictionaryString HolderOfKey;
        public XmlDictionaryString SenderVouches;
        public XmlDictionaryString SubjectLocality;
        public XmlDictionaryString SubjectLocalityDNSAddress;
        public XmlDictionaryString SubjectLocalityIPAddress;
        public XmlDictionaryString SubjectStatement;
        public XmlDictionaryString UnspecifiedAuthenticationMethod;
        public XmlDictionaryString NamespaceAttributePrefix;
        public XmlDictionaryString Resource;
        public XmlDictionaryString UserName;
        public XmlDictionaryString UserNameNamespace;
        public XmlDictionaryString EmailName;
        public XmlDictionaryString EmailNamespace;

        public SamlDictionary(IdentityModelDictionary dictionary)
        {
            Access = dictionary.CreateString(IdentityModelStringsVersion1.String24, 24);
            AccessDecision = dictionary.CreateString(IdentityModelStringsVersion1.String25, 25);
            Action = dictionary.CreateString(IdentityModelStringsVersion1.String26, 26);
            Advice = dictionary.CreateString(IdentityModelStringsVersion1.String27, 27);
            Assertion = dictionary.CreateString(IdentityModelStringsVersion1.String28, 28);
            AssertionId = dictionary.CreateString(IdentityModelStringsVersion1.String29, 29);
            AssertionIdReference = dictionary.CreateString(IdentityModelStringsVersion1.String30, 30);
            Attribute = dictionary.CreateString(IdentityModelStringsVersion1.String31, 31);
            AttributeName = dictionary.CreateString(IdentityModelStringsVersion1.String32, 32);
            AttributeNamespace = dictionary.CreateString(IdentityModelStringsVersion1.String33, 33);
            AttributeStatement = dictionary.CreateString(IdentityModelStringsVersion1.String34, 34);
            AttributeValue = dictionary.CreateString(IdentityModelStringsVersion1.String35, 35);
            Audience = dictionary.CreateString(IdentityModelStringsVersion1.String36, 36);
            AudienceRestrictionCondition = dictionary.CreateString(IdentityModelStringsVersion1.String37, 37);
            AuthenticationInstant = dictionary.CreateString(IdentityModelStringsVersion1.String38, 38);
            AuthenticationMethod = dictionary.CreateString(IdentityModelStringsVersion1.String39, 39);
            AuthenticationStatement = dictionary.CreateString(IdentityModelStringsVersion1.String40, 40);
            AuthorityBinding = dictionary.CreateString(IdentityModelStringsVersion1.String41, 41);
            AuthorityKind = dictionary.CreateString(IdentityModelStringsVersion1.String42, 42);
            AuthorizationDecisionStatement = dictionary.CreateString(IdentityModelStringsVersion1.String43, 43);
            Binding = dictionary.CreateString(IdentityModelStringsVersion1.String44, 44);
            Condition = dictionary.CreateString(IdentityModelStringsVersion1.String45, 45);
            Conditions = dictionary.CreateString(IdentityModelStringsVersion1.String46, 46);
            Decision = dictionary.CreateString(IdentityModelStringsVersion1.String47, 47);
            DoNotCacheCondition = dictionary.CreateString(IdentityModelStringsVersion1.String48, 48);
            Evidence = dictionary.CreateString(IdentityModelStringsVersion1.String49, 49);
            IssueInstant = dictionary.CreateString(IdentityModelStringsVersion1.String50, 50);
            Issuer = dictionary.CreateString(IdentityModelStringsVersion1.String51, 51);
            Location = dictionary.CreateString(IdentityModelStringsVersion1.String52, 52);
            MajorVersion = dictionary.CreateString(IdentityModelStringsVersion1.String53, 53);
            MinorVersion = dictionary.CreateString(IdentityModelStringsVersion1.String54, 54);
            Namespace = dictionary.CreateString(IdentityModelStringsVersion1.String55, 55);
            NameIdentifier = dictionary.CreateString(IdentityModelStringsVersion1.String56, 56);
            NameIdentifierFormat = dictionary.CreateString(IdentityModelStringsVersion1.String57, 57);
            NameIdentifierNameQualifier = dictionary.CreateString(IdentityModelStringsVersion1.String58, 58);
            ActionNamespaceAttribute = dictionary.CreateString(IdentityModelStringsVersion1.String59, 59);
            NotBefore = dictionary.CreateString(IdentityModelStringsVersion1.String60, 60);
            NotOnOrAfter = dictionary.CreateString(IdentityModelStringsVersion1.String61, 61);
            PreferredPrefix = dictionary.CreateString(IdentityModelStringsVersion1.String62, 62);
            Statement = dictionary.CreateString(IdentityModelStringsVersion1.String63, 63);
            Subject = dictionary.CreateString(IdentityModelStringsVersion1.String64, 64);
            SubjectConfirmation = dictionary.CreateString(IdentityModelStringsVersion1.String65, 65);
            SubjectConfirmationData = dictionary.CreateString(IdentityModelStringsVersion1.String66, 66);
            SubjectConfirmationMethod = dictionary.CreateString(IdentityModelStringsVersion1.String67, 67);
            HolderOfKey = dictionary.CreateString(IdentityModelStringsVersion1.String68, 68);
            SenderVouches = dictionary.CreateString(IdentityModelStringsVersion1.String69, 69);
            SubjectLocality = dictionary.CreateString(IdentityModelStringsVersion1.String70, 70);
            SubjectLocalityDNSAddress = dictionary.CreateString(IdentityModelStringsVersion1.String71, 71);
            SubjectLocalityIPAddress = dictionary.CreateString(IdentityModelStringsVersion1.String72, 72);
            SubjectStatement = dictionary.CreateString(IdentityModelStringsVersion1.String73, 73);
            UnspecifiedAuthenticationMethod = dictionary.CreateString(IdentityModelStringsVersion1.String74, 74);
            NamespaceAttributePrefix = dictionary.CreateString(IdentityModelStringsVersion1.String75, 75);
            Resource = dictionary.CreateString(IdentityModelStringsVersion1.String76, 76);
            UserName = dictionary.CreateString(IdentityModelStringsVersion1.String77, 77);
            UserNameNamespace = dictionary.CreateString(IdentityModelStringsVersion1.String78, 78);
            EmailName = dictionary.CreateString(IdentityModelStringsVersion1.String79, 79);
            EmailNamespace = dictionary.CreateString(IdentityModelStringsVersion1.String80, 80);
        }

        public SamlDictionary(IXmlDictionary dictionary)
        {
            Access = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String24);
            AccessDecision = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String25);
            Action = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String26);
            Advice = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String27);
            Assertion = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String28);
            AssertionId = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String29);
            AssertionIdReference = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String30);
            Attribute = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String31);
            AttributeName = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String32);
            AttributeNamespace = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String33);
            AttributeStatement = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String34);
            AttributeValue = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String35);
            Audience = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String36);
            AudienceRestrictionCondition = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String37);
            AuthenticationInstant = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String38);
            AuthenticationMethod = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String39);
            AuthenticationStatement = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String40);
            AuthorityBinding = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String41);
            AuthorityKind = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String42);
            AuthorizationDecisionStatement = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String43);
            Binding = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String44);
            Condition = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String45);
            Conditions = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String46);
            Decision = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String47);
            DoNotCacheCondition = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String48);
            Evidence = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String49);
            IssueInstant = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String50);
            Issuer = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String51);
            Location = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String52);
            MajorVersion = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String53);
            MinorVersion = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String54);
            Namespace = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String55);
            NameIdentifier = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String56);
            NameIdentifierFormat = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String57);
            NameIdentifierNameQualifier = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String58);
            ActionNamespaceAttribute = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String59);
            NotBefore = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String60);
            NotOnOrAfter = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String61);
            PreferredPrefix = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String62);
            Statement = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String63);
            Subject = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String64);
            SubjectConfirmation = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String65);
            SubjectConfirmationData = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String66);
            SubjectConfirmationMethod = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String67);
            HolderOfKey = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String68);
            SenderVouches = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String69);
            SubjectLocality = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String70);
            SubjectLocalityDNSAddress = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String71);
            SubjectLocalityIPAddress = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String72);
            SubjectStatement = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String73);
            UnspecifiedAuthenticationMethod = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String74);
            NamespaceAttributePrefix = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String75);
            Resource = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String76);
            UserName = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String77);
            UserNameNamespace = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String78);
            EmailName = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String79);
            EmailNamespace = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String80);
        }

        private XmlDictionaryString LookupDictionaryString(IXmlDictionary dictionary, string value)
        {
            XmlDictionaryString expectedValue;
            if (!dictionary.TryLookup(value, out expectedValue))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SRP.Format(SRP.XDCannotFindValueInDictionaryString, value));
            }

            return expectedValue;
        }
    }

    internal class SecureConversationDictionary
    {
        public XmlDictionaryString Namespace;
        public XmlDictionaryString DerivedKeyToken;
        public XmlDictionaryString Nonce;
        public XmlDictionaryString Length;
        public XmlDictionaryString SecurityContextToken;
        public XmlDictionaryString AlgorithmAttribute;
        public XmlDictionaryString Generation;
        public XmlDictionaryString Label;
        public XmlDictionaryString Offset;
        public XmlDictionaryString Properties;
        public XmlDictionaryString Identifier;
        public XmlDictionaryString Cookie;
        public XmlDictionaryString RenewNeededFaultCode;
        public XmlDictionaryString BadContextTokenFaultCode;
        public XmlDictionaryString Prefix;
        public XmlDictionaryString DerivedKeyTokenType;
        public XmlDictionaryString SecurityContextTokenType;
        public XmlDictionaryString SecurityContextTokenReferenceValueType;
        public XmlDictionaryString RequestSecurityContextIssuance;
        public XmlDictionaryString RequestSecurityContextIssuanceResponse;
        public XmlDictionaryString RequestSecurityContextRenew;
        public XmlDictionaryString RequestSecurityContextRenewResponse;
        public XmlDictionaryString RequestSecurityContextClose;
        public XmlDictionaryString RequestSecurityContextCloseResponse;
        public XmlDictionaryString Instance;

        public SecureConversationDictionary()
        {
        }

        public SecureConversationDictionary(IdentityModelDictionary dictionary)
        {
        }

        public SecureConversationDictionary(IXmlDictionary dictionary)
        {
        }

        private XmlDictionaryString LookupDictionaryString(IXmlDictionary dictionary, string value)
        {
            XmlDictionaryString expectedValue;
            if (!dictionary.TryLookup(value, out expectedValue))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SRP.Format(SRP.XDCannotFindValueInDictionaryString, value));
            }

            return expectedValue;
        }
    }

    internal class SecureConversationDec2005Dictionary : SecureConversationDictionary
    {
        public SecureConversationDec2005Dictionary(IdentityModelDictionary dictionary) : base(dictionary)
        {
            SecurityContextToken = dictionary.CreateString(IdentityModelStringsVersion1.String175, 175);
            AlgorithmAttribute = dictionary.CreateString(IdentityModelStringsVersion1.String0, 0);
            Generation = dictionary.CreateString(IdentityModelStringsVersion1.String176, 176);
            Label = dictionary.CreateString(IdentityModelStringsVersion1.String177, 177);
            Offset = dictionary.CreateString(IdentityModelStringsVersion1.String178, 178);
            Properties = dictionary.CreateString(IdentityModelStringsVersion1.String179, 179);
            Identifier = dictionary.CreateString(IdentityModelStringsVersion1.String180, 180);
            Cookie = dictionary.CreateString(IdentityModelStringsVersion1.String181, 181);
            RenewNeededFaultCode = dictionary.CreateString(IdentityModelStringsVersion1.String182, 182);
            BadContextTokenFaultCode = dictionary.CreateString(IdentityModelStringsVersion1.String183, 183);
            Prefix = dictionary.CreateString(IdentityModelStringsVersion1.String268, 268);
            DerivedKeyTokenType = dictionary.CreateString(IdentityModelStringsVersion1.String269, 269);
            SecurityContextTokenType = dictionary.CreateString(IdentityModelStringsVersion1.String270, 270);
            SecurityContextTokenReferenceValueType = dictionary.CreateString(IdentityModelStringsVersion1.String270, 270);
            RequestSecurityContextIssuance = dictionary.CreateString(IdentityModelStringsVersion1.String271, 271);
            RequestSecurityContextIssuanceResponse = dictionary.CreateString(IdentityModelStringsVersion1.String272, 272);
            RequestSecurityContextRenew = dictionary.CreateString(IdentityModelStringsVersion1.String273, 273);
            RequestSecurityContextRenewResponse = dictionary.CreateString(IdentityModelStringsVersion1.String274, 274);
            RequestSecurityContextClose = dictionary.CreateString(IdentityModelStringsVersion1.String275, 275);
            RequestSecurityContextCloseResponse = dictionary.CreateString(IdentityModelStringsVersion1.String276, 276);
            Namespace = dictionary.CreateString(IdentityModelStringsVersion1.String277, 277);
            DerivedKeyToken = dictionary.CreateString(IdentityModelStringsVersion1.String173, 173);
            Nonce = dictionary.CreateString(IdentityModelStringsVersion1.String120, 120);
            Length = dictionary.CreateString(IdentityModelStringsVersion1.String174, 174);
            Instance = dictionary.CreateString(IdentityModelStringsVersion1.String278, 278);
        }

        public SecureConversationDec2005Dictionary(IXmlDictionary dictionary) : base(dictionary)
        {
            SecurityContextToken = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String175);
            AlgorithmAttribute = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String0);
            Generation = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String176);
            Label = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String177);
            Offset = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String178);
            Properties = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String179);
            Identifier = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String180);
            Cookie = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String181);
            RenewNeededFaultCode = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String182);
            BadContextTokenFaultCode = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String183);
            Prefix = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String268);
            DerivedKeyTokenType = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String269);
            SecurityContextTokenType = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String270);
            SecurityContextTokenReferenceValueType = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String270);
            RequestSecurityContextIssuance = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String271);
            RequestSecurityContextIssuanceResponse = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String272);
            RequestSecurityContextRenew = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String273);
            RequestSecurityContextRenewResponse = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String274);
            RequestSecurityContextClose = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String275);
            RequestSecurityContextCloseResponse = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String276);
            Namespace = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String277);
            DerivedKeyToken = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String173);
            Nonce = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String120);
            Length = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String174);
            Instance = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String278);
        }

        private XmlDictionaryString LookupDictionaryString(IXmlDictionary dictionary, string value)
        {
            XmlDictionaryString expectedValue;
            if (!dictionary.TryLookup(value, out expectedValue))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SRP.Format(SRP.XDCannotFindValueInDictionaryString, value));
            }

            return expectedValue;
        }
    }

    internal class SecureConversationFeb2005Dictionary : SecureConversationDictionary
    {
        public SecureConversationFeb2005Dictionary(IdentityModelDictionary dictionary) : base(dictionary)
        {
            Namespace = dictionary.CreateString(IdentityModelStringsVersion1.String172, 172);
            DerivedKeyToken = dictionary.CreateString(IdentityModelStringsVersion1.String173, 173);
            Nonce = dictionary.CreateString(IdentityModelStringsVersion1.String120, 120);
            Length = dictionary.CreateString(IdentityModelStringsVersion1.String174, 174);
            SecurityContextToken = dictionary.CreateString(IdentityModelStringsVersion1.String175, 175);
            AlgorithmAttribute = dictionary.CreateString(IdentityModelStringsVersion1.String0, 0);
            Generation = dictionary.CreateString(IdentityModelStringsVersion1.String176, 176);
            Label = dictionary.CreateString(IdentityModelStringsVersion1.String177, 177);
            Offset = dictionary.CreateString(IdentityModelStringsVersion1.String178, 178);
            Properties = dictionary.CreateString(IdentityModelStringsVersion1.String179, 179);
            Identifier = dictionary.CreateString(IdentityModelStringsVersion1.String180, 180);
            Cookie = dictionary.CreateString(IdentityModelStringsVersion1.String181, 181);
            RenewNeededFaultCode = dictionary.CreateString(IdentityModelStringsVersion1.String182, 182);
            BadContextTokenFaultCode = dictionary.CreateString(IdentityModelStringsVersion1.String183, 183);
            Prefix = dictionary.CreateString(IdentityModelStringsVersion1.String184, 184);
            DerivedKeyTokenType = dictionary.CreateString(IdentityModelStringsVersion1.String185, 185);
            SecurityContextTokenType = dictionary.CreateString(IdentityModelStringsVersion1.String186, 186);
            SecurityContextTokenReferenceValueType = dictionary.CreateString(IdentityModelStringsVersion1.String186, 186);
            RequestSecurityContextIssuance = dictionary.CreateString(IdentityModelStringsVersion1.String187, 187);
            RequestSecurityContextIssuanceResponse = dictionary.CreateString(IdentityModelStringsVersion1.String188, 188);
            RequestSecurityContextRenew = dictionary.CreateString(IdentityModelStringsVersion1.String189, 189);
            RequestSecurityContextRenewResponse = dictionary.CreateString(IdentityModelStringsVersion1.String190, 190);
            RequestSecurityContextClose = dictionary.CreateString(IdentityModelStringsVersion1.String191, 191);
            RequestSecurityContextCloseResponse = dictionary.CreateString(IdentityModelStringsVersion1.String192, 192);
        }

        public SecureConversationFeb2005Dictionary(IXmlDictionary dictionary) : base(dictionary)
        {
            Namespace = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String172);
            DerivedKeyToken = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String173);
            Nonce = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String120);
            Length = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String174);
            SecurityContextToken = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String175);
            AlgorithmAttribute = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String0);
            Generation = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String176);
            Label = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String177);
            Offset = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String178);
            Properties = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String179);
            Identifier = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String180);
            Cookie = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String181);
            RenewNeededFaultCode = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String182);
            BadContextTokenFaultCode = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String183);
            Prefix = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String184);
            DerivedKeyTokenType = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String185);
            SecurityContextTokenType = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String186);
            SecurityContextTokenReferenceValueType = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String186);
            RequestSecurityContextIssuance = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String187);
            RequestSecurityContextIssuanceResponse = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String188);
            RequestSecurityContextRenew = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String189);
            RequestSecurityContextRenewResponse = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String190);
            RequestSecurityContextClose = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String191);
            RequestSecurityContextCloseResponse = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String192);
        }

        private XmlDictionaryString LookupDictionaryString(IXmlDictionary dictionary, string value)
        {
            XmlDictionaryString expectedValue;
            if (!dictionary.TryLookup(value, out expectedValue))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SRP.Format(SRP.XDCannotFindValueInDictionaryString, value));
            }

            return expectedValue;
        }
    }

    internal class SecurityAlgorithmDictionary
    {
        public XmlDictionaryString Aes128Encryption;
        public XmlDictionaryString Aes128KeyWrap;
        public XmlDictionaryString Aes192Encryption;
        public XmlDictionaryString Aes192KeyWrap;
        public XmlDictionaryString Aes256Encryption;
        public XmlDictionaryString Aes256KeyWrap;
        public XmlDictionaryString DesEncryption;
        public XmlDictionaryString DsaSha1Signature;
        public XmlDictionaryString ExclusiveC14n;
        public XmlDictionaryString ExclusiveC14nWithComments;
        public XmlDictionaryString HmacSha1Signature;
        public XmlDictionaryString HmacSha256Signature;
        public XmlDictionaryString Psha1KeyDerivation;
        public XmlDictionaryString Ripemd160Digest;
        public XmlDictionaryString RsaOaepKeyWrap;
        public XmlDictionaryString RsaSha1Signature;
        public XmlDictionaryString RsaSha256Signature;
        public XmlDictionaryString RsaV15KeyWrap;
        public XmlDictionaryString Sha1Digest;
        public XmlDictionaryString Sha256Digest;
        public XmlDictionaryString Sha512Digest;
        public XmlDictionaryString TripleDesEncryption;
        public XmlDictionaryString TripleDesKeyWrap;
        public XmlDictionaryString TlsSspiKeyWrap;
        public XmlDictionaryString WindowsSspiKeyWrap;

        public SecurityAlgorithmDictionary(IdentityModelDictionary dictionary)
        {
            Aes128Encryption = dictionary.CreateString(IdentityModelStringsVersion1.String95, 95);
            Aes128KeyWrap = dictionary.CreateString(IdentityModelStringsVersion1.String96, 96);
            Aes192Encryption = dictionary.CreateString(IdentityModelStringsVersion1.String97, 97);
            Aes192KeyWrap = dictionary.CreateString(IdentityModelStringsVersion1.String98, 98);
            Aes256Encryption = dictionary.CreateString(IdentityModelStringsVersion1.String99, 99);
            Aes256KeyWrap = dictionary.CreateString(IdentityModelStringsVersion1.String100, 100);
            DesEncryption = dictionary.CreateString(IdentityModelStringsVersion1.String101, 101);
            DsaSha1Signature = dictionary.CreateString(IdentityModelStringsVersion1.String102, 102);
            ExclusiveC14n = dictionary.CreateString(IdentityModelStringsVersion1.String20, 20);
            ExclusiveC14nWithComments = dictionary.CreateString(IdentityModelStringsVersion1.String103, 103);
            HmacSha1Signature = dictionary.CreateString(IdentityModelStringsVersion1.String104, 104);
            HmacSha256Signature = dictionary.CreateString(IdentityModelStringsVersion1.String105, 105);
            Psha1KeyDerivation = dictionary.CreateString(IdentityModelStringsVersion1.String106, 106);
            Ripemd160Digest = dictionary.CreateString(IdentityModelStringsVersion1.String107, 107);
            RsaOaepKeyWrap = dictionary.CreateString(IdentityModelStringsVersion1.String108, 108);
            RsaSha1Signature = dictionary.CreateString(IdentityModelStringsVersion1.String109, 109);
            RsaSha256Signature = dictionary.CreateString(IdentityModelStringsVersion1.String110, 110);
            RsaV15KeyWrap = dictionary.CreateString(IdentityModelStringsVersion1.String111, 111);
            Sha1Digest = dictionary.CreateString(IdentityModelStringsVersion1.String112, 112);
            Sha256Digest = dictionary.CreateString(IdentityModelStringsVersion1.String113, 113);
            Sha512Digest = dictionary.CreateString(IdentityModelStringsVersion1.String114, 114);
            TripleDesEncryption = dictionary.CreateString(IdentityModelStringsVersion1.String115, 115);
            TripleDesKeyWrap = dictionary.CreateString(IdentityModelStringsVersion1.String116, 116);
            TlsSspiKeyWrap = dictionary.CreateString(IdentityModelStringsVersion1.String117, 117);
            WindowsSspiKeyWrap = dictionary.CreateString(IdentityModelStringsVersion1.String118, 118);
        }

        public SecurityAlgorithmDictionary(IXmlDictionary dictionary)
        {
            Aes128Encryption = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String95);
            Aes128KeyWrap = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String96);
            Aes192Encryption = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String97);
            Aes192KeyWrap = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String98);
            Aes256Encryption = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String99);
            Aes256KeyWrap = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String100);
            DesEncryption = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String101);
            DsaSha1Signature = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String102);
            ExclusiveC14n = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String20);
            ExclusiveC14nWithComments = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String103);
            HmacSha1Signature = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String104);
            HmacSha256Signature = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String105);
            Psha1KeyDerivation = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String106);
            Ripemd160Digest = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String107);
            RsaOaepKeyWrap = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String108);
            RsaSha1Signature = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String109);
            RsaSha256Signature = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String110);
            RsaV15KeyWrap = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String111);
            Sha1Digest = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String112);
            Sha256Digest = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String113);
            Sha512Digest = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String114);
            TripleDesEncryption = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String115);
            TripleDesKeyWrap = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String116);
            TlsSspiKeyWrap = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String117);
            WindowsSspiKeyWrap = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String118);
        }

        private XmlDictionaryString LookupDictionaryString(IXmlDictionary dictionary, string value)
        {
            XmlDictionaryString expectedValue;
            if (!dictionary.TryLookup(value, out expectedValue))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SRP.Format(SRP.XDCannotFindValueInDictionaryString, value));
            }

            return expectedValue;
        }
    }

    internal class SecurityAlgorithmDec2005Dictionary
    {
        public XmlDictionaryString Psha1KeyDerivationDec2005;

        public SecurityAlgorithmDec2005Dictionary(IdentityModelDictionary dictionary)
        {
            Psha1KeyDerivationDec2005 = dictionary.CreateString(IdentityModelStringsVersion1.String267, 267);
        }

        public SecurityAlgorithmDec2005Dictionary(IXmlDictionary dictionary)
        {
            Psha1KeyDerivationDec2005 = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String267);
        }

        private XmlDictionaryString LookupDictionaryString(IXmlDictionary dictionary, string value)
        {
            XmlDictionaryString expectedValue;
            if (!dictionary.TryLookup(value, out expectedValue))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SRP.Format(SRP.XDCannotFindValueInDictionaryString, value));
            }

            return expectedValue;
        }
    }

    internal class SecurityJan2004Dictionary
    {
        public XmlDictionaryString Prefix;
        public XmlDictionaryString NonceElement;
        public XmlDictionaryString PasswordElement;
        public XmlDictionaryString PasswordTextName;
        public XmlDictionaryString UserNameElement;
        public XmlDictionaryString UserNameTokenElement;
        public XmlDictionaryString BinarySecurityToken;
        public XmlDictionaryString EncodingType;
        public XmlDictionaryString Reference;
        public XmlDictionaryString URI;
        public XmlDictionaryString KeyIdentifier;
        public XmlDictionaryString EncodingTypeValueBase64Binary;
        public XmlDictionaryString EncodingTypeValueHexBinary;
        public XmlDictionaryString EncodingTypeValueText;
        public XmlDictionaryString X509SKIValueType;
        public XmlDictionaryString KerberosTokenTypeGSS;
        public XmlDictionaryString KerberosTokenType1510;
        public XmlDictionaryString SamlAssertionIdValueType;
        public XmlDictionaryString SamlAssertion;
        public XmlDictionaryString SamlUri;
        public XmlDictionaryString RelAssertionValueType;
        public XmlDictionaryString FailedAuthenticationFaultCode;
        public XmlDictionaryString InvalidSecurityTokenFaultCode;
        public XmlDictionaryString InvalidSecurityFaultCode;
        public XmlDictionaryString SecurityTokenReference;
        public XmlDictionaryString Namespace;
        public XmlDictionaryString Security;
        public XmlDictionaryString ValueType;
        public XmlDictionaryString TypeAttribute;
        public XmlDictionaryString KerberosHashValueType;

        public SecurityJan2004Dictionary(IdentityModelDictionary dictionary)
        {
            Prefix = dictionary.CreateString(IdentityModelStringsVersion1.String119, 119);
            NonceElement = dictionary.CreateString(IdentityModelStringsVersion1.String120, 120);
            PasswordElement = dictionary.CreateString(IdentityModelStringsVersion1.String121, 121);
            PasswordTextName = dictionary.CreateString(IdentityModelStringsVersion1.String122, 122);
            UserNameElement = dictionary.CreateString(IdentityModelStringsVersion1.String123, 123);
            UserNameTokenElement = dictionary.CreateString(IdentityModelStringsVersion1.String124, 124);
            BinarySecurityToken = dictionary.CreateString(IdentityModelStringsVersion1.String125, 125);
            EncodingType = dictionary.CreateString(IdentityModelStringsVersion1.String126, 126);
            Reference = dictionary.CreateString(IdentityModelStringsVersion1.String2, 2);
            URI = dictionary.CreateString(IdentityModelStringsVersion1.String1, 1);
            KeyIdentifier = dictionary.CreateString(IdentityModelStringsVersion1.String127, 127);
            EncodingTypeValueBase64Binary = dictionary.CreateString(IdentityModelStringsVersion1.String128, 128);
            EncodingTypeValueHexBinary = dictionary.CreateString(IdentityModelStringsVersion1.String129, 129);
            EncodingTypeValueText = dictionary.CreateString(IdentityModelStringsVersion1.String130, 130);
            X509SKIValueType = dictionary.CreateString(IdentityModelStringsVersion1.String131, 131);
            KerberosTokenTypeGSS = dictionary.CreateString(IdentityModelStringsVersion1.String132, 132);
            KerberosTokenType1510 = dictionary.CreateString(IdentityModelStringsVersion1.String133, 133);
            SamlAssertionIdValueType = dictionary.CreateString(IdentityModelStringsVersion1.String134, 134);
            SamlAssertion = dictionary.CreateString(IdentityModelStringsVersion1.String28, 28);
            SamlUri = dictionary.CreateString(IdentityModelStringsVersion1.String55, 55);
            RelAssertionValueType = dictionary.CreateString(IdentityModelStringsVersion1.String135, 135);
            FailedAuthenticationFaultCode = dictionary.CreateString(IdentityModelStringsVersion1.String136, 136);
            InvalidSecurityTokenFaultCode = dictionary.CreateString(IdentityModelStringsVersion1.String137, 137);
            InvalidSecurityFaultCode = dictionary.CreateString(IdentityModelStringsVersion1.String138, 138);
            SecurityTokenReference = dictionary.CreateString(IdentityModelStringsVersion1.String139, 139);
            Namespace = dictionary.CreateString(IdentityModelStringsVersion1.String140, 140);
            Security = dictionary.CreateString(IdentityModelStringsVersion1.String141, 141);
            ValueType = dictionary.CreateString(IdentityModelStringsVersion1.String142, 142);
            TypeAttribute = dictionary.CreateString(IdentityModelStringsVersion1.String83, 83);
            KerberosHashValueType = dictionary.CreateString(IdentityModelStringsVersion1.String143, 143);
        }

        public SecurityJan2004Dictionary(IXmlDictionary dictionary)
        {
            Prefix = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String119);
            NonceElement = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String120);
            PasswordElement = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String121);
            PasswordTextName = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String122);
            UserNameElement = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String123);
            UserNameTokenElement = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String124);
            BinarySecurityToken = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String125);
            EncodingType = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String126);
            Reference = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String2);
            URI = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String1);
            KeyIdentifier = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String127);
            EncodingTypeValueBase64Binary = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String128);
            EncodingTypeValueHexBinary = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String129);
            EncodingTypeValueText = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String130);
            X509SKIValueType = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String131);
            KerberosTokenTypeGSS = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String132);
            KerberosTokenType1510 = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String133);
            SamlAssertionIdValueType = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String134);
            SamlAssertion = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String28);
            SamlUri = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String55);
            RelAssertionValueType = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String135);
            FailedAuthenticationFaultCode = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String136);
            InvalidSecurityTokenFaultCode = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String137);
            InvalidSecurityFaultCode = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String138);
            SecurityTokenReference = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String139);
            Namespace = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String140);
            Security = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String141);
            ValueType = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String142);
            TypeAttribute = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String83);
            KerberosHashValueType = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String143);
        }

        private XmlDictionaryString LookupDictionaryString(IXmlDictionary dictionary, string value)
        {
            XmlDictionaryString expectedValue;
            if (!dictionary.TryLookup(value, out expectedValue))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SRP.Format(SRP.XDCannotFindValueInDictionaryString, value));
            }

            return expectedValue;
        }
    }

    internal class SecurityXXX2005Dictionary
    {
        public XmlDictionaryString Prefix;
        public XmlDictionaryString SignatureConfirmation;
        public XmlDictionaryString ValueAttribute;
        public XmlDictionaryString TokenTypeAttribute;
        public XmlDictionaryString ThumbprintSha1ValueType;
        public XmlDictionaryString EncryptedKeyTokenType;
        public XmlDictionaryString EncryptedKeyHashValueType;
        public XmlDictionaryString SamlTokenType;
        public XmlDictionaryString Saml20TokenType;
        public XmlDictionaryString Saml11AssertionValueType;
        public XmlDictionaryString EncryptedHeader;
        public XmlDictionaryString Namespace;

        public SecurityXXX2005Dictionary(IdentityModelDictionary dictionary)
        {
            Prefix = dictionary.CreateString(IdentityModelStringsVersion1.String144, 144);
            SignatureConfirmation = dictionary.CreateString(IdentityModelStringsVersion1.String145, 145);
            ValueAttribute = dictionary.CreateString(IdentityModelStringsVersion1.String146, 146);
            TokenTypeAttribute = dictionary.CreateString(IdentityModelStringsVersion1.String147, 147);
            ThumbprintSha1ValueType = dictionary.CreateString(IdentityModelStringsVersion1.String148, 148);
            EncryptedKeyTokenType = dictionary.CreateString(IdentityModelStringsVersion1.String149, 149);
            EncryptedKeyHashValueType = dictionary.CreateString(IdentityModelStringsVersion1.String150, 150);
            SamlTokenType = dictionary.CreateString(IdentityModelStringsVersion1.String151, 151);
            Saml20TokenType = dictionary.CreateString(IdentityModelStringsVersion1.String152, 152);
            Saml11AssertionValueType = dictionary.CreateString(IdentityModelStringsVersion1.String153, 153);
            EncryptedHeader = dictionary.CreateString(IdentityModelStringsVersion1.String154, 154);
            Namespace = dictionary.CreateString(IdentityModelStringsVersion1.String155, 155);
        }

        public SecurityXXX2005Dictionary(IXmlDictionary dictionary)
        {
            Prefix = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String144);
            SignatureConfirmation = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String145);
            ValueAttribute = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String146);
            TokenTypeAttribute = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String147);
            ThumbprintSha1ValueType = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String148);
            EncryptedKeyTokenType = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String149);
            EncryptedKeyHashValueType = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String150);
            SamlTokenType = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String151);
            Saml20TokenType = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String152);
            Saml11AssertionValueType = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String153);
            EncryptedHeader = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String154);
            Namespace = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String155);
        }

        private XmlDictionaryString LookupDictionaryString(IXmlDictionary dictionary, string value)
        {
            XmlDictionaryString expectedValue;
            if (!dictionary.TryLookup(value, out expectedValue))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SRP.Format(SRP.XDCannotFindValueInDictionaryString, value));
            }

            return expectedValue;
        }
    }

    internal class TrustDictionary
    {
        public XmlDictionaryString RequestSecurityTokenResponseCollection;
        public XmlDictionaryString Namespace;
        public XmlDictionaryString BinarySecretClauseType;
        public XmlDictionaryString CombinedHashLabel;
        public XmlDictionaryString RequestSecurityTokenResponse;
        public XmlDictionaryString TokenType;
        public XmlDictionaryString KeySize;
        public XmlDictionaryString RequestedTokenReference;
        public XmlDictionaryString AppliesTo;
        public XmlDictionaryString Authenticator;
        public XmlDictionaryString CombinedHash;
        public XmlDictionaryString BinaryExchange;
        public XmlDictionaryString Lifetime;
        public XmlDictionaryString RequestedSecurityToken;
        public XmlDictionaryString Entropy;
        public XmlDictionaryString RequestedProofToken;
        public XmlDictionaryString ComputedKey;
        public XmlDictionaryString RequestSecurityToken;
        public XmlDictionaryString RequestType;
        public XmlDictionaryString Context;
        public XmlDictionaryString BinarySecret;
        public XmlDictionaryString Type;
        public XmlDictionaryString SpnegoValueTypeUri;
        public XmlDictionaryString TlsnegoValueTypeUri;
        public XmlDictionaryString Prefix;
        public XmlDictionaryString RequestSecurityTokenIssuance;
        public XmlDictionaryString RequestSecurityTokenIssuanceResponse;
        public XmlDictionaryString RequestTypeIssue;
        public XmlDictionaryString SymmetricKeyBinarySecret;
        public XmlDictionaryString Psha1ComputedKeyUri;
        public XmlDictionaryString NonceBinarySecret;
        public XmlDictionaryString RenewTarget;
        public XmlDictionaryString CloseTarget;
        public XmlDictionaryString RequestedTokenClosed;
        public XmlDictionaryString RequestedAttachedReference;
        public XmlDictionaryString RequestedUnattachedReference;
        public XmlDictionaryString IssuedTokensHeader;
        public XmlDictionaryString RequestTypeRenew;
        public XmlDictionaryString RequestTypeClose;
        public XmlDictionaryString KeyType;
        public XmlDictionaryString SymmetricKeyType;
        public XmlDictionaryString PublicKeyType;
        public XmlDictionaryString Claims;
        public XmlDictionaryString InvalidRequestFaultCode;
        public XmlDictionaryString FailedAuthenticationFaultCode;
        public XmlDictionaryString UseKey;
        public XmlDictionaryString SignWith;
        public XmlDictionaryString EncryptWith;
        public XmlDictionaryString EncryptionAlgorithm;
        public XmlDictionaryString CanonicalizationAlgorithm;
        public XmlDictionaryString ComputedKeyAlgorithm;
        public XmlDictionaryString AsymmetricKeyBinarySecret;
        public XmlDictionaryString RequestSecurityTokenCollectionIssuanceFinalResponse;
        public XmlDictionaryString RequestSecurityTokenRenewal;
        public XmlDictionaryString RequestSecurityTokenRenewalResponse;
        public XmlDictionaryString RequestSecurityTokenCollectionRenewalFinalResponse;
        public XmlDictionaryString RequestSecurityTokenCancellation;
        public XmlDictionaryString RequestSecurityTokenCancellationResponse;
        public XmlDictionaryString RequestSecurityTokenCollectionCancellationFinalResponse;
        public XmlDictionaryString KeyWrapAlgorithm;
        public XmlDictionaryString BearerKeyType;
        public XmlDictionaryString SecondaryParameters;
        public XmlDictionaryString Dialect;
        public XmlDictionaryString DialectType;

        public TrustDictionary()
        {
        }

        public TrustDictionary(IdentityModelDictionary dictionary)
        {
        }

        public TrustDictionary(IXmlDictionary dictionary)
        {
        }

        private XmlDictionaryString LookupDictionaryString(IXmlDictionary dictionary, string value)
        {
            XmlDictionaryString expectedValue;
            if (!dictionary.TryLookup(value, out expectedValue))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SRP.Format(SRP.XDCannotFindValueInDictionaryString, value));
            }

            return expectedValue;
        }
    }

    internal class TrustDec2005Dictionary : TrustDictionary
    {
        public TrustDec2005Dictionary(IdentityModelDictionary dictionary) : base(dictionary)
        {
            CombinedHashLabel = dictionary.CreateString(IdentityModelStringsVersion1.String196, 196);
            RequestSecurityTokenResponse = dictionary.CreateString(IdentityModelStringsVersion1.String197, 197);
            TokenType = dictionary.CreateString(IdentityModelStringsVersion1.String147, 147);
            KeySize = dictionary.CreateString(IdentityModelStringsVersion1.String198, 198);
            RequestedTokenReference = dictionary.CreateString(IdentityModelStringsVersion1.String199, 199);
            AppliesTo = dictionary.CreateString(IdentityModelStringsVersion1.String200, 200);
            Authenticator = dictionary.CreateString(IdentityModelStringsVersion1.String201, 201);
            CombinedHash = dictionary.CreateString(IdentityModelStringsVersion1.String202, 202);
            BinaryExchange = dictionary.CreateString(IdentityModelStringsVersion1.String203, 203);
            Lifetime = dictionary.CreateString(IdentityModelStringsVersion1.String204, 204);
            RequestedSecurityToken = dictionary.CreateString(IdentityModelStringsVersion1.String205, 205);
            Entropy = dictionary.CreateString(IdentityModelStringsVersion1.String206, 206);
            RequestedProofToken = dictionary.CreateString(IdentityModelStringsVersion1.String207, 207);
            ComputedKey = dictionary.CreateString(IdentityModelStringsVersion1.String208, 208);
            RequestSecurityToken = dictionary.CreateString(IdentityModelStringsVersion1.String209, 209);
            RequestType = dictionary.CreateString(IdentityModelStringsVersion1.String210, 210);
            Context = dictionary.CreateString(IdentityModelStringsVersion1.String211, 211);
            BinarySecret = dictionary.CreateString(IdentityModelStringsVersion1.String212, 212);
            Type = dictionary.CreateString(IdentityModelStringsVersion1.String83, 83);
            SpnegoValueTypeUri = dictionary.CreateString(IdentityModelStringsVersion1.String240, 240);
            TlsnegoValueTypeUri = dictionary.CreateString(IdentityModelStringsVersion1.String241, 241);
            Prefix = dictionary.CreateString(IdentityModelStringsVersion1.String242, 242);
            RequestSecurityTokenIssuance = dictionary.CreateString(IdentityModelStringsVersion1.String243, 243);
            RequestSecurityTokenIssuanceResponse = dictionary.CreateString(IdentityModelStringsVersion1.String244, 244);
            RequestTypeIssue = dictionary.CreateString(IdentityModelStringsVersion1.String245, 245);
            AsymmetricKeyBinarySecret = dictionary.CreateString(IdentityModelStringsVersion1.String246, 246);
            SymmetricKeyBinarySecret = dictionary.CreateString(IdentityModelStringsVersion1.String247, 247);
            NonceBinarySecret = dictionary.CreateString(IdentityModelStringsVersion1.String248, 248);
            Psha1ComputedKeyUri = dictionary.CreateString(IdentityModelStringsVersion1.String249, 249);
            KeyType = dictionary.CreateString(IdentityModelStringsVersion1.String230, 230);
            SymmetricKeyType = dictionary.CreateString(IdentityModelStringsVersion1.String247, 247);
            PublicKeyType = dictionary.CreateString(IdentityModelStringsVersion1.String250, 250);
            Claims = dictionary.CreateString(IdentityModelStringsVersion1.String232, 232);
            InvalidRequestFaultCode = dictionary.CreateString(IdentityModelStringsVersion1.String233, 233);
            FailedAuthenticationFaultCode = dictionary.CreateString(IdentityModelStringsVersion1.String136, 136);
            UseKey = dictionary.CreateString(IdentityModelStringsVersion1.String234, 234);
            SignWith = dictionary.CreateString(IdentityModelStringsVersion1.String235, 235);
            EncryptWith = dictionary.CreateString(IdentityModelStringsVersion1.String236, 236);
            EncryptionAlgorithm = dictionary.CreateString(IdentityModelStringsVersion1.String237, 237);
            CanonicalizationAlgorithm = dictionary.CreateString(IdentityModelStringsVersion1.String238, 238);
            ComputedKeyAlgorithm = dictionary.CreateString(IdentityModelStringsVersion1.String239, 239);
            RequestSecurityTokenResponseCollection = dictionary.CreateString(IdentityModelStringsVersion1.String193, 193);
            Namespace = dictionary.CreateString(IdentityModelStringsVersion1.String251, 251);
            BinarySecretClauseType = dictionary.CreateString(IdentityModelStringsVersion1.String252, 252);
            RequestSecurityTokenCollectionIssuanceFinalResponse = dictionary.CreateString(IdentityModelStringsVersion1.String253, 253);
            RequestSecurityTokenRenewal = dictionary.CreateString(IdentityModelStringsVersion1.String254, 254);
            RequestSecurityTokenRenewalResponse = dictionary.CreateString(IdentityModelStringsVersion1.String255, 255);
            RequestSecurityTokenCollectionRenewalFinalResponse = dictionary.CreateString(IdentityModelStringsVersion1.String256, 256);
            RequestSecurityTokenCancellation = dictionary.CreateString(IdentityModelStringsVersion1.String257, 257);
            RequestSecurityTokenCancellationResponse = dictionary.CreateString(IdentityModelStringsVersion1.String258, 258);
            RequestSecurityTokenCollectionCancellationFinalResponse = dictionary.CreateString(IdentityModelStringsVersion1.String259, 259);
            RequestTypeRenew = dictionary.CreateString(IdentityModelStringsVersion1.String260, 260);
            RequestTypeClose = dictionary.CreateString(IdentityModelStringsVersion1.String261, 261);
            RenewTarget = dictionary.CreateString(IdentityModelStringsVersion1.String222, 222);
            CloseTarget = dictionary.CreateString(IdentityModelStringsVersion1.String223, 223);
            RequestedTokenClosed = dictionary.CreateString(IdentityModelStringsVersion1.String224, 224);
            RequestedAttachedReference = dictionary.CreateString(IdentityModelStringsVersion1.String225, 225);
            RequestedUnattachedReference = dictionary.CreateString(IdentityModelStringsVersion1.String226, 226);
            IssuedTokensHeader = dictionary.CreateString(IdentityModelStringsVersion1.String227, 227);
            KeyWrapAlgorithm = dictionary.CreateString(IdentityModelStringsVersion1.String262, 262);
            BearerKeyType = dictionary.CreateString(IdentityModelStringsVersion1.String263, 263);
            SecondaryParameters = dictionary.CreateString(IdentityModelStringsVersion1.String264, 264);
            Dialect = dictionary.CreateString(IdentityModelStringsVersion1.String265, 265);
            DialectType = dictionary.CreateString(IdentityModelStringsVersion1.String266, 266);
        }

        public TrustDec2005Dictionary(IXmlDictionary dictionary) : base(dictionary)
        {
            CombinedHashLabel = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String196);
            RequestSecurityTokenResponse = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String197);
            TokenType = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String147);
            KeySize = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String198);
            RequestedTokenReference = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String199);
            AppliesTo = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String200);
            Authenticator = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String201);
            CombinedHash = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String202);
            BinaryExchange = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String203);
            Lifetime = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String204);
            RequestedSecurityToken = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String205);
            Entropy = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String206);
            RequestedProofToken = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String207);
            ComputedKey = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String208);
            RequestSecurityToken = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String209);
            RequestType = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String210);
            Context = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String211);
            BinarySecret = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String212);
            Type = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String83);
            SpnegoValueTypeUri = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String240);
            TlsnegoValueTypeUri = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String241);
            Prefix = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String242);
            RequestSecurityTokenIssuance = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String243);
            RequestSecurityTokenIssuanceResponse = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String244);
            RequestTypeIssue = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String245);
            AsymmetricKeyBinarySecret = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String246);
            SymmetricKeyBinarySecret = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String247);
            NonceBinarySecret = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String248);
            Psha1ComputedKeyUri = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String249);
            KeyType = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String230);
            SymmetricKeyType = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String247);
            PublicKeyType = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String250);
            Claims = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String232);
            InvalidRequestFaultCode = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String233);
            FailedAuthenticationFaultCode = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String136);
            UseKey = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String234);
            SignWith = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String235);
            EncryptWith = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String236);
            EncryptionAlgorithm = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String237);
            CanonicalizationAlgorithm = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String238);
            ComputedKeyAlgorithm = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String239);
            RequestSecurityTokenResponseCollection = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String193);
            Namespace = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String251);
            BinarySecretClauseType = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String252);
            RequestSecurityTokenCollectionIssuanceFinalResponse = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String253);
            RequestSecurityTokenRenewal = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String254);
            RequestSecurityTokenRenewalResponse = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String255);
            RequestSecurityTokenCollectionRenewalFinalResponse = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String256);
            RequestSecurityTokenCancellation = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String257);
            RequestSecurityTokenCancellationResponse = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String258);
            RequestSecurityTokenCollectionCancellationFinalResponse = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String259);
            RequestTypeRenew = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String260);
            RequestTypeClose = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String261);
            RenewTarget = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String222);
            CloseTarget = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String223);
            RequestedTokenClosed = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String224);
            RequestedAttachedReference = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String225);
            RequestedUnattachedReference = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String226);
            IssuedTokensHeader = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String227);
            KeyWrapAlgorithm = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String262);
            BearerKeyType = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String263);
            SecondaryParameters = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String264);
            Dialect = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String265);
            DialectType = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String266);
        }

        private XmlDictionaryString LookupDictionaryString(IXmlDictionary dictionary, string value)
        {
            XmlDictionaryString expectedValue;
            if (!dictionary.TryLookup(value, out expectedValue))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SRP.Format(SRP.XDCannotFindValueInDictionaryString, value));
            }

            return expectedValue;
        }
    }

    internal class TrustFeb2005Dictionary : TrustDictionary
    {
        public TrustFeb2005Dictionary(IdentityModelDictionary dictionary) : base(dictionary)
        {
            RequestSecurityTokenResponseCollection = dictionary.CreateString(IdentityModelStringsVersion1.String193, 193);
            Namespace = dictionary.CreateString(IdentityModelStringsVersion1.String194, 194);
            BinarySecretClauseType = dictionary.CreateString(IdentityModelStringsVersion1.String195, 195);
            CombinedHashLabel = dictionary.CreateString(IdentityModelStringsVersion1.String196, 196);
            RequestSecurityTokenResponse = dictionary.CreateString(IdentityModelStringsVersion1.String197, 197);
            TokenType = dictionary.CreateString(IdentityModelStringsVersion1.String147, 147);
            KeySize = dictionary.CreateString(IdentityModelStringsVersion1.String198, 198);
            RequestedTokenReference = dictionary.CreateString(IdentityModelStringsVersion1.String199, 199);
            AppliesTo = dictionary.CreateString(IdentityModelStringsVersion1.String200, 200);
            Authenticator = dictionary.CreateString(IdentityModelStringsVersion1.String201, 201);
            CombinedHash = dictionary.CreateString(IdentityModelStringsVersion1.String202, 202);
            BinaryExchange = dictionary.CreateString(IdentityModelStringsVersion1.String203, 203);
            Lifetime = dictionary.CreateString(IdentityModelStringsVersion1.String204, 204);
            RequestedSecurityToken = dictionary.CreateString(IdentityModelStringsVersion1.String205, 205);
            Entropy = dictionary.CreateString(IdentityModelStringsVersion1.String206, 206);
            RequestedProofToken = dictionary.CreateString(IdentityModelStringsVersion1.String207, 207);
            ComputedKey = dictionary.CreateString(IdentityModelStringsVersion1.String208, 208);
            RequestSecurityToken = dictionary.CreateString(IdentityModelStringsVersion1.String209, 209);
            RequestType = dictionary.CreateString(IdentityModelStringsVersion1.String210, 210);
            Context = dictionary.CreateString(IdentityModelStringsVersion1.String211, 211);
            BinarySecret = dictionary.CreateString(IdentityModelStringsVersion1.String212, 212);
            Type = dictionary.CreateString(IdentityModelStringsVersion1.String83, 83);
            SpnegoValueTypeUri = dictionary.CreateString(IdentityModelStringsVersion1.String213, 213);
            TlsnegoValueTypeUri = dictionary.CreateString(IdentityModelStringsVersion1.String214, 214);
            Prefix = dictionary.CreateString(IdentityModelStringsVersion1.String215, 215);
            RequestSecurityTokenIssuance = dictionary.CreateString(IdentityModelStringsVersion1.String216, 216);
            RequestSecurityTokenIssuanceResponse = dictionary.CreateString(IdentityModelStringsVersion1.String217, 217);
            RequestTypeIssue = dictionary.CreateString(IdentityModelStringsVersion1.String218, 218);
            SymmetricKeyBinarySecret = dictionary.CreateString(IdentityModelStringsVersion1.String219, 219);
            Psha1ComputedKeyUri = dictionary.CreateString(IdentityModelStringsVersion1.String220, 220);
            NonceBinarySecret = dictionary.CreateString(IdentityModelStringsVersion1.String221, 221);
            RenewTarget = dictionary.CreateString(IdentityModelStringsVersion1.String222, 222);
            CloseTarget = dictionary.CreateString(IdentityModelStringsVersion1.String223, 223);
            RequestedTokenClosed = dictionary.CreateString(IdentityModelStringsVersion1.String224, 224);
            RequestedAttachedReference = dictionary.CreateString(IdentityModelStringsVersion1.String225, 225);
            RequestedUnattachedReference = dictionary.CreateString(IdentityModelStringsVersion1.String226, 226);
            IssuedTokensHeader = dictionary.CreateString(IdentityModelStringsVersion1.String227, 227);
            RequestTypeRenew = dictionary.CreateString(IdentityModelStringsVersion1.String228, 228);
            RequestTypeClose = dictionary.CreateString(IdentityModelStringsVersion1.String229, 229);
            KeyType = dictionary.CreateString(IdentityModelStringsVersion1.String230, 230);
            SymmetricKeyType = dictionary.CreateString(IdentityModelStringsVersion1.String219, 219);
            PublicKeyType = dictionary.CreateString(IdentityModelStringsVersion1.String231, 231);
            Claims = dictionary.CreateString(IdentityModelStringsVersion1.String232, 232);
            InvalidRequestFaultCode = dictionary.CreateString(IdentityModelStringsVersion1.String233, 233);
            FailedAuthenticationFaultCode = dictionary.CreateString(IdentityModelStringsVersion1.String136, 136);
            UseKey = dictionary.CreateString(IdentityModelStringsVersion1.String234, 234);
            SignWith = dictionary.CreateString(IdentityModelStringsVersion1.String235, 235);
            EncryptWith = dictionary.CreateString(IdentityModelStringsVersion1.String236, 236);
            EncryptionAlgorithm = dictionary.CreateString(IdentityModelStringsVersion1.String237, 237);
            CanonicalizationAlgorithm = dictionary.CreateString(IdentityModelStringsVersion1.String238, 238);
            ComputedKeyAlgorithm = dictionary.CreateString(IdentityModelStringsVersion1.String239, 239);
        }

        public TrustFeb2005Dictionary(IXmlDictionary dictionary) : base(dictionary)
        {
            RequestSecurityTokenResponseCollection = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String193);
            Namespace = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String194);
            BinarySecretClauseType = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String195);
            CombinedHashLabel = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String196);
            RequestSecurityTokenResponse = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String197);
            TokenType = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String147);
            KeySize = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String198);
            RequestedTokenReference = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String199);
            AppliesTo = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String200);
            Authenticator = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String201);
            CombinedHash = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String202);
            BinaryExchange = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String203);
            Lifetime = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String204);
            RequestedSecurityToken = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String205);
            Entropy = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String206);
            RequestedProofToken = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String207);
            ComputedKey = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String208);
            RequestSecurityToken = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String209);
            RequestType = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String210);
            Context = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String211);
            BinarySecret = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String212);
            Type = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String83);
            SpnegoValueTypeUri = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String213);
            TlsnegoValueTypeUri = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String214);
            Prefix = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String215);
            RequestSecurityTokenIssuance = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String216);
            RequestSecurityTokenIssuanceResponse = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String217);
            RequestTypeIssue = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String218);
            SymmetricKeyBinarySecret = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String219);
            Psha1ComputedKeyUri = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String220);
            NonceBinarySecret = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String221);
            RenewTarget = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String222);
            CloseTarget = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String223);
            RequestedTokenClosed = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String224);
            RequestedAttachedReference = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String225);
            RequestedUnattachedReference = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String226);
            IssuedTokensHeader = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String227);
            RequestTypeRenew = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String228);
            RequestTypeClose = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String229);
            KeyType = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String230);
            SymmetricKeyType = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String219);
            PublicKeyType = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String231);
            Claims = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String232);
            InvalidRequestFaultCode = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String233);
            FailedAuthenticationFaultCode = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String136);
            UseKey = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String234);
            SignWith = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String235);
            EncryptWith = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String236);
            EncryptionAlgorithm = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String237);
            CanonicalizationAlgorithm = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String238);
            ComputedKeyAlgorithm = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String239);
        }

        private XmlDictionaryString LookupDictionaryString(IXmlDictionary dictionary, string value)
        {
            XmlDictionaryString expectedValue;
            if (!dictionary.TryLookup(value, out expectedValue))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SRP.Format(SRP.XDCannotFindValueInDictionaryString, value));
            }

            return expectedValue;
        }
    }

    internal class UtilityDictionary
    {
        public XmlDictionaryString IdAttribute;
        public XmlDictionaryString Namespace;
        public XmlDictionaryString Timestamp;
        public XmlDictionaryString CreatedElement;
        public XmlDictionaryString ExpiresElement;
        public XmlDictionaryString Prefix;

        public UtilityDictionary(IdentityModelDictionary dictionary)
        {
            IdAttribute = dictionary.CreateString(IdentityModelStringsVersion1.String3, 3);
            Namespace = dictionary.CreateString(IdentityModelStringsVersion1.String16, 16);
            Timestamp = dictionary.CreateString(IdentityModelStringsVersion1.String17, 17);
            CreatedElement = dictionary.CreateString(IdentityModelStringsVersion1.String18, 18);
            ExpiresElement = dictionary.CreateString(IdentityModelStringsVersion1.String19, 19);
            Prefix = dictionary.CreateString(IdentityModelStringsVersion1.String81, 81);
        }

        public UtilityDictionary(IXmlDictionary dictionary)
        {
            IdAttribute = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String3);
            Namespace = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String16);
            Timestamp = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String17);
            CreatedElement = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String18);
            ExpiresElement = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String19);
            Prefix = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String81);
        }

        private XmlDictionaryString LookupDictionaryString(IXmlDictionary dictionary, string value)
        {
            XmlDictionaryString expectedValue;
            if (!dictionary.TryLookup(value, out expectedValue))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SRP.Format(SRP.XDCannotFindValueInDictionaryString, value));
            }

            return expectedValue;
        }
    }

    internal class XmlEncryptionDictionary
    {
        public XmlDictionaryString Namespace;
        public XmlDictionaryString DataReference;
        public XmlDictionaryString EncryptedData;
        public XmlDictionaryString EncryptionMethod;
        public XmlDictionaryString CipherData;
        public XmlDictionaryString CipherValue;
        public XmlDictionaryString ReferenceList;
        public XmlDictionaryString Encoding;
        public XmlDictionaryString MimeType;
        public XmlDictionaryString Type;
        public XmlDictionaryString Id;
        public XmlDictionaryString CarriedKeyName;
        public XmlDictionaryString Recipient;
        public XmlDictionaryString EncryptedKey;
        public XmlDictionaryString URI;
        public XmlDictionaryString KeyReference;
        public XmlDictionaryString Prefix;
        public XmlDictionaryString ElementType;
        public XmlDictionaryString ContentType;
        public XmlDictionaryString AlgorithmAttribute;

        public XmlEncryptionDictionary(IdentityModelDictionary dictionary)
        {
            Namespace = dictionary.CreateString(IdentityModelStringsVersion1.String156, 156);
            DataReference = dictionary.CreateString(IdentityModelStringsVersion1.String157, 157);
            EncryptedData = dictionary.CreateString(IdentityModelStringsVersion1.String158, 158);
            EncryptionMethod = dictionary.CreateString(IdentityModelStringsVersion1.String159, 159);
            CipherData = dictionary.CreateString(IdentityModelStringsVersion1.String160, 160);
            CipherValue = dictionary.CreateString(IdentityModelStringsVersion1.String161, 161);
            ReferenceList = dictionary.CreateString(IdentityModelStringsVersion1.String162, 162);
            Encoding = dictionary.CreateString(IdentityModelStringsVersion1.String163, 163);
            MimeType = dictionary.CreateString(IdentityModelStringsVersion1.String164, 164);
            Type = dictionary.CreateString(IdentityModelStringsVersion1.String83, 83);
            Id = dictionary.CreateString(IdentityModelStringsVersion1.String3, 3);
            CarriedKeyName = dictionary.CreateString(IdentityModelStringsVersion1.String165, 165);
            Recipient = dictionary.CreateString(IdentityModelStringsVersion1.String166, 166);
            EncryptedKey = dictionary.CreateString(IdentityModelStringsVersion1.String167, 167);
            URI = dictionary.CreateString(IdentityModelStringsVersion1.String1, 1);
            KeyReference = dictionary.CreateString(IdentityModelStringsVersion1.String168, 168);
            Prefix = dictionary.CreateString(IdentityModelStringsVersion1.String169, 169);
            ElementType = dictionary.CreateString(IdentityModelStringsVersion1.String170, 170);
            ContentType = dictionary.CreateString(IdentityModelStringsVersion1.String171, 171);
            AlgorithmAttribute = dictionary.CreateString(IdentityModelStringsVersion1.String0, 0);
        }

        public XmlEncryptionDictionary(IXmlDictionary dictionary)
        {
            Namespace = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String156);
            DataReference = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String157);
            EncryptedData = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String158);
            EncryptionMethod = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String159);
            CipherData = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String160);
            CipherValue = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String161);
            ReferenceList = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String162);
            Encoding = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String163);
            MimeType = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String164);
            Type = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String83);
            Id = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String3);
            CarriedKeyName = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String165);
            Recipient = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String166);
            EncryptedKey = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String167);
            URI = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String1);
            KeyReference = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String168);
            Prefix = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String169);
            ElementType = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String170);
            ContentType = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String171);
            AlgorithmAttribute = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String0);
        }

        private XmlDictionaryString LookupDictionaryString(IXmlDictionary dictionary, string value)
        {
            XmlDictionaryString expectedValue;
            if (!dictionary.TryLookup(value, out expectedValue))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SRP.Format(SRP.XDCannotFindValueInDictionaryString, value));
            }

            return expectedValue;
        }
    }

    internal class XmlSignatureDictionary
    {
        public XmlDictionaryString Algorithm;
        public XmlDictionaryString URI;
        public XmlDictionaryString Reference;
        public XmlDictionaryString Transforms;
        public XmlDictionaryString Transform;
        public XmlDictionaryString DigestMethod;
        public XmlDictionaryString DigestValue;
        public XmlDictionaryString Namespace;
        public XmlDictionaryString EnvelopedSignature;
        public XmlDictionaryString KeyInfo;
        public XmlDictionaryString Signature;
        public XmlDictionaryString SignedInfo;
        public XmlDictionaryString CanonicalizationMethod;
        public XmlDictionaryString SignatureMethod;
        public XmlDictionaryString SignatureValue;
        public XmlDictionaryString KeyName;
        public XmlDictionaryString Type;
        public XmlDictionaryString MgmtData;
        public XmlDictionaryString Prefix;
        public XmlDictionaryString KeyValue;
        public XmlDictionaryString RsaKeyValue;
        public XmlDictionaryString Modulus;
        public XmlDictionaryString Exponent;
        public XmlDictionaryString X509Data;
        public XmlDictionaryString X509IssuerSerial;
        public XmlDictionaryString X509IssuerName;
        public XmlDictionaryString X509SerialNumber;
        public XmlDictionaryString X509Certificate;

        public XmlSignatureDictionary(IdentityModelDictionary dictionary)
        {
            Algorithm = dictionary.CreateString(IdentityModelStringsVersion1.String0, 0);
            URI = dictionary.CreateString(IdentityModelStringsVersion1.String1, 1);
            Reference = dictionary.CreateString(IdentityModelStringsVersion1.String2, 2);
            Transforms = dictionary.CreateString(IdentityModelStringsVersion1.String4, 4);
            Transform = dictionary.CreateString(IdentityModelStringsVersion1.String5, 5);
            DigestMethod = dictionary.CreateString(IdentityModelStringsVersion1.String6, 6);
            DigestValue = dictionary.CreateString(IdentityModelStringsVersion1.String7, 7);
            Namespace = dictionary.CreateString(IdentityModelStringsVersion1.String8, 8);
            EnvelopedSignature = dictionary.CreateString(IdentityModelStringsVersion1.String9, 9);
            KeyInfo = dictionary.CreateString(IdentityModelStringsVersion1.String10, 10);
            Signature = dictionary.CreateString(IdentityModelStringsVersion1.String11, 11);
            SignedInfo = dictionary.CreateString(IdentityModelStringsVersion1.String12, 12);
            CanonicalizationMethod = dictionary.CreateString(IdentityModelStringsVersion1.String13, 13);
            SignatureMethod = dictionary.CreateString(IdentityModelStringsVersion1.String14, 14);
            SignatureValue = dictionary.CreateString(IdentityModelStringsVersion1.String15, 15);
            KeyName = dictionary.CreateString(IdentityModelStringsVersion1.String82, 82);
            Type = dictionary.CreateString(IdentityModelStringsVersion1.String83, 83);
            MgmtData = dictionary.CreateString(IdentityModelStringsVersion1.String84, 84);
            Prefix = dictionary.CreateString(IdentityModelStringsVersion1.String85, 85);
            KeyValue = dictionary.CreateString(IdentityModelStringsVersion1.String86, 86);
            RsaKeyValue = dictionary.CreateString(IdentityModelStringsVersion1.String87, 87);
            Modulus = dictionary.CreateString(IdentityModelStringsVersion1.String88, 88);
            Exponent = dictionary.CreateString(IdentityModelStringsVersion1.String89, 89);
            X509Data = dictionary.CreateString(IdentityModelStringsVersion1.String90, 90);
            X509IssuerSerial = dictionary.CreateString(IdentityModelStringsVersion1.String91, 91);
            X509IssuerName = dictionary.CreateString(IdentityModelStringsVersion1.String92, 92);
            X509SerialNumber = dictionary.CreateString(IdentityModelStringsVersion1.String93, 93);
            X509Certificate = dictionary.CreateString(IdentityModelStringsVersion1.String94, 94);
        }

        public XmlSignatureDictionary(IXmlDictionary dictionary)
        {
            Algorithm = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String0);
            URI = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String1);
            Reference = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String2);
            Transforms = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String4);
            Transform = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String5);
            DigestMethod = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String6);
            DigestValue = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String7);
            Namespace = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String8);
            EnvelopedSignature = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String9);
            KeyInfo = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String10);
            Signature = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String11);
            SignedInfo = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String12);
            CanonicalizationMethod = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String13);
            SignatureMethod = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String14);
            SignatureValue = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String15);
            KeyName = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String82);
            Type = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String83);
            MgmtData = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String84);
            Prefix = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String85);
            KeyValue = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String86);
            RsaKeyValue = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String87);
            Modulus = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String88);
            Exponent = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String89);
            X509Data = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String90);
            X509IssuerSerial = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String91);
            X509IssuerName = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String92);
            X509SerialNumber = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String93);
            X509Certificate = LookupDictionaryString(dictionary, IdentityModelStringsVersion1.String94);
        }

        private XmlDictionaryString LookupDictionaryString(IXmlDictionary dictionary, string value)
        {
            XmlDictionaryString expectedValue;
            if (!dictionary.TryLookup(value, out expectedValue))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SRP.Format(SRP.XDCannotFindValueInDictionaryString, value));
            }

            return expectedValue;
        }
    }


    internal static class ExclusiveC14NStrings
    {
        // Main dictionary strings
        public const string Namespace = IdentityModelStringsVersion1.String20;
        public const string PrefixList = IdentityModelStringsVersion1.String21;
        public const string InclusiveNamespaces = IdentityModelStringsVersion1.String22;
        public const string Prefix = IdentityModelStringsVersion1.String23;
    }

    internal static class SamlStrings
    {
        // Main dictionary strings
        public const string Access = IdentityModelStringsVersion1.String24;
        public const string AccessDecision = IdentityModelStringsVersion1.String25;
        public const string Action = IdentityModelStringsVersion1.String26;
        public const string Advice = IdentityModelStringsVersion1.String27;
        public const string Assertion = IdentityModelStringsVersion1.String28;
        public const string AssertionId = IdentityModelStringsVersion1.String29;
        public const string AssertionIdReference = IdentityModelStringsVersion1.String30;
        public const string Attribute = IdentityModelStringsVersion1.String31;
        public const string AttributeName = IdentityModelStringsVersion1.String32;
        public const string AttributeNamespace = IdentityModelStringsVersion1.String33;
        public const string AttributeStatement = IdentityModelStringsVersion1.String34;
        public const string AttributeValue = IdentityModelStringsVersion1.String35;
        public const string Audience = IdentityModelStringsVersion1.String36;
        public const string AudienceRestrictionCondition = IdentityModelStringsVersion1.String37;
        public const string AuthenticationInstant = IdentityModelStringsVersion1.String38;
        public const string AuthenticationMethod = IdentityModelStringsVersion1.String39;
        public const string AuthenticationStatement = IdentityModelStringsVersion1.String40;
        public const string AuthorityBinding = IdentityModelStringsVersion1.String41;
        public const string AuthorityKind = IdentityModelStringsVersion1.String42;
        public const string AuthorizationDecisionStatement = IdentityModelStringsVersion1.String43;
        public const string Binding = IdentityModelStringsVersion1.String44;
        public const string Condition = IdentityModelStringsVersion1.String45;
        public const string Conditions = IdentityModelStringsVersion1.String46;
        public const string Decision = IdentityModelStringsVersion1.String47;
        public const string DoNotCacheCondition = IdentityModelStringsVersion1.String48;
        public const string Evidence = IdentityModelStringsVersion1.String49;
        public const string IssueInstant = IdentityModelStringsVersion1.String50;
        public const string Issuer = IdentityModelStringsVersion1.String51;
        public const string Location = IdentityModelStringsVersion1.String52;
        public const string MajorVersion = IdentityModelStringsVersion1.String53;
        public const string MinorVersion = IdentityModelStringsVersion1.String54;
        public const string Namespace = IdentityModelStringsVersion1.String55;
        public const string NameIdentifier = IdentityModelStringsVersion1.String56;
        public const string NameIdentifierFormat = IdentityModelStringsVersion1.String57;
        public const string NameIdentifierNameQualifier = IdentityModelStringsVersion1.String58;
        public const string ActionNamespaceAttribute = IdentityModelStringsVersion1.String59;
        public const string NotBefore = IdentityModelStringsVersion1.String60;
        public const string NotOnOrAfter = IdentityModelStringsVersion1.String61;
        public const string PreferredPrefix = IdentityModelStringsVersion1.String62;
        public const string Statement = IdentityModelStringsVersion1.String63;
        public const string Subject = IdentityModelStringsVersion1.String64;
        public const string SubjectConfirmation = IdentityModelStringsVersion1.String65;
        public const string SubjectConfirmationData = IdentityModelStringsVersion1.String66;
        public const string SubjectConfirmationMethod = IdentityModelStringsVersion1.String67;
        public const string HolderOfKey = IdentityModelStringsVersion1.String68;
        public const string SenderVouches = IdentityModelStringsVersion1.String69;
        public const string SubjectLocality = IdentityModelStringsVersion1.String70;
        public const string SubjectLocalityDNSAddress = IdentityModelStringsVersion1.String71;
        public const string SubjectLocalityIPAddress = IdentityModelStringsVersion1.String72;
        public const string SubjectStatement = IdentityModelStringsVersion1.String73;
        public const string UnspecifiedAuthenticationMethod = IdentityModelStringsVersion1.String74;
        public const string NamespaceAttributePrefix = IdentityModelStringsVersion1.String75;
        public const string Resource = IdentityModelStringsVersion1.String76;
        public const string UserName = IdentityModelStringsVersion1.String77;
        public const string UserNameNamespace = IdentityModelStringsVersion1.String78;
        public const string EmailName = IdentityModelStringsVersion1.String79;
        public const string EmailNamespace = IdentityModelStringsVersion1.String80;
    }

    internal static class SecureConversationStrings
    {
    }

    internal static class SecureConversationDec2005Strings
    {
        // Main dictionary strings
        public const string SecurityContextToken = IdentityModelStringsVersion1.String175;
        public const string AlgorithmAttribute = IdentityModelStringsVersion1.String0;
        public const string Generation = IdentityModelStringsVersion1.String176;
        public const string Label = IdentityModelStringsVersion1.String177;
        public const string Offset = IdentityModelStringsVersion1.String178;
        public const string Properties = IdentityModelStringsVersion1.String179;
        public const string Identifier = IdentityModelStringsVersion1.String180;
        public const string Cookie = IdentityModelStringsVersion1.String181;
        public const string RenewNeededFaultCode = IdentityModelStringsVersion1.String182;
        public const string BadContextTokenFaultCode = IdentityModelStringsVersion1.String183;
        public const string Prefix = IdentityModelStringsVersion1.String268;
        public const string DerivedKeyTokenType = IdentityModelStringsVersion1.String269;
        public const string SecurityContextTokenType = IdentityModelStringsVersion1.String270;
        public const string SecurityContextTokenReferenceValueType = IdentityModelStringsVersion1.String270;
        public const string RequestSecurityContextIssuance = IdentityModelStringsVersion1.String271;
        public const string RequestSecurityContextIssuanceResponse = IdentityModelStringsVersion1.String272;
        public const string RequestSecurityContextRenew = IdentityModelStringsVersion1.String273;
        public const string RequestSecurityContextRenewResponse = IdentityModelStringsVersion1.String274;
        public const string RequestSecurityContextClose = IdentityModelStringsVersion1.String275;
        public const string RequestSecurityContextCloseResponse = IdentityModelStringsVersion1.String276;
        public const string Namespace = IdentityModelStringsVersion1.String277;
        public const string DerivedKeyToken = IdentityModelStringsVersion1.String173;
        public const string Nonce = IdentityModelStringsVersion1.String120;
        public const string Length = IdentityModelStringsVersion1.String174;
        public const string Instance = IdentityModelStringsVersion1.String278;
    }

    internal static class SecureConversationFeb2005Strings
    {
        // Main dictionary strings
        public const string Namespace = IdentityModelStringsVersion1.String172;
        public const string DerivedKeyToken = IdentityModelStringsVersion1.String173;
        public const string Nonce = IdentityModelStringsVersion1.String120;
        public const string Length = IdentityModelStringsVersion1.String174;
        public const string SecurityContextToken = IdentityModelStringsVersion1.String175;
        public const string AlgorithmAttribute = IdentityModelStringsVersion1.String0;
        public const string Generation = IdentityModelStringsVersion1.String176;
        public const string Label = IdentityModelStringsVersion1.String177;
        public const string Offset = IdentityModelStringsVersion1.String178;
        public const string Properties = IdentityModelStringsVersion1.String179;
        public const string Identifier = IdentityModelStringsVersion1.String180;
        public const string Cookie = IdentityModelStringsVersion1.String181;
        public const string RenewNeededFaultCode = IdentityModelStringsVersion1.String182;
        public const string BadContextTokenFaultCode = IdentityModelStringsVersion1.String183;
        public const string Prefix = IdentityModelStringsVersion1.String184;
        public const string DerivedKeyTokenType = IdentityModelStringsVersion1.String185;
        public const string SecurityContextTokenType = IdentityModelStringsVersion1.String186;
        public const string SecurityContextTokenReferenceValueType = IdentityModelStringsVersion1.String186;
        public const string RequestSecurityContextIssuance = IdentityModelStringsVersion1.String187;
        public const string RequestSecurityContextIssuanceResponse = IdentityModelStringsVersion1.String188;
        public const string RequestSecurityContextRenew = IdentityModelStringsVersion1.String189;
        public const string RequestSecurityContextRenewResponse = IdentityModelStringsVersion1.String190;
        public const string RequestSecurityContextClose = IdentityModelStringsVersion1.String191;
        public const string RequestSecurityContextCloseResponse = IdentityModelStringsVersion1.String192;
    }

    internal static class SecurityAlgorithmStrings
    {
        // Main dictionary strings
        public const string Aes128Encryption = IdentityModelStringsVersion1.String95;
        public const string Aes128KeyWrap = IdentityModelStringsVersion1.String96;
        public const string Aes192Encryption = IdentityModelStringsVersion1.String97;
        public const string Aes192KeyWrap = IdentityModelStringsVersion1.String98;
        public const string Aes256Encryption = IdentityModelStringsVersion1.String99;
        public const string Aes256KeyWrap = IdentityModelStringsVersion1.String100;
        public const string DesEncryption = IdentityModelStringsVersion1.String101;
        public const string DsaSha1Signature = IdentityModelStringsVersion1.String102;
        public const string ExclusiveC14n = IdentityModelStringsVersion1.String20;
        public const string ExclusiveC14nWithComments = IdentityModelStringsVersion1.String103;
        public const string HmacSha1Signature = IdentityModelStringsVersion1.String104;
        public const string HmacSha256Signature = IdentityModelStringsVersion1.String105;
        public const string Psha1KeyDerivation = IdentityModelStringsVersion1.String106;
        public const string Ripemd160Digest = IdentityModelStringsVersion1.String107;
        public const string RsaOaepKeyWrap = IdentityModelStringsVersion1.String108;
        public const string RsaSha1Signature = IdentityModelStringsVersion1.String109;
        public const string RsaSha256Signature = IdentityModelStringsVersion1.String110;
        public const string RsaV15KeyWrap = IdentityModelStringsVersion1.String111;
        public const string Sha1Digest = IdentityModelStringsVersion1.String112;
        public const string Sha256Digest = IdentityModelStringsVersion1.String113;
        public const string Sha512Digest = IdentityModelStringsVersion1.String114;
        public const string TripleDesEncryption = IdentityModelStringsVersion1.String115;
        public const string TripleDesKeyWrap = IdentityModelStringsVersion1.String116;
        public const string TlsSspiKeyWrap = IdentityModelStringsVersion1.String117;
        public const string WindowsSspiKeyWrap = IdentityModelStringsVersion1.String118;
        // String constants
        public const string StrTransform = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#STR-Transform";
    }

    internal static class SecurityAlgorithmDec2005Strings
    {
        // Main dictionary strings
        public const string Psha1KeyDerivationDec2005 = IdentityModelStringsVersion1.String267;
    }

    internal static class SecurityJan2004Strings
    {
        // Main dictionary strings
        public const string Prefix = IdentityModelStringsVersion1.String119;
        public const string NonceElement = IdentityModelStringsVersion1.String120;
        public const string PasswordElement = IdentityModelStringsVersion1.String121;
        public const string PasswordTextName = IdentityModelStringsVersion1.String122;
        public const string UserNameElement = IdentityModelStringsVersion1.String123;
        public const string UserNameTokenElement = IdentityModelStringsVersion1.String124;
        public const string BinarySecurityToken = IdentityModelStringsVersion1.String125;
        public const string EncodingType = IdentityModelStringsVersion1.String126;
        public const string Reference = IdentityModelStringsVersion1.String2;
        public const string URI = IdentityModelStringsVersion1.String1;
        public const string KeyIdentifier = IdentityModelStringsVersion1.String127;
        public const string EncodingTypeValueBase64Binary = IdentityModelStringsVersion1.String128;
        public const string EncodingTypeValueHexBinary = IdentityModelStringsVersion1.String129;
        public const string EncodingTypeValueText = IdentityModelStringsVersion1.String130;
        public const string X509SKIValueType = IdentityModelStringsVersion1.String131;
        public const string KerberosTokenTypeGSS = IdentityModelStringsVersion1.String132;
        public const string KerberosTokenType1510 = IdentityModelStringsVersion1.String133;
        public const string SamlAssertionIdValueType = IdentityModelStringsVersion1.String134;
        public const string SamlAssertion = IdentityModelStringsVersion1.String28;
        public const string SamlUri = IdentityModelStringsVersion1.String55;
        public const string RelAssertionValueType = IdentityModelStringsVersion1.String135;
        public const string FailedAuthenticationFaultCode = IdentityModelStringsVersion1.String136;
        public const string InvalidSecurityTokenFaultCode = IdentityModelStringsVersion1.String137;
        public const string InvalidSecurityFaultCode = IdentityModelStringsVersion1.String138;
        public const string SecurityTokenReference = IdentityModelStringsVersion1.String139;
        public const string Namespace = IdentityModelStringsVersion1.String140;
        public const string Security = IdentityModelStringsVersion1.String141;
        public const string ValueType = IdentityModelStringsVersion1.String142;
        public const string TypeAttribute = IdentityModelStringsVersion1.String83;
        public const string KerberosHashValueType = IdentityModelStringsVersion1.String143;
        // String constants
        public const string SecurityProfileNamespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0";
        public const string X509TokenProfileNamespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-x509-token-profile-1.0";
        public const string UPTokenProfileNamespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0";
        public const string SamlTokenProfileNamespace = "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.0";
        public const string KerberosTokenProfileNamespace = "http://www.docs.oasis-open.org/wss/2004/07/oasis-000000-wss-kerberos-token-profile-1.0";
        public const string UPTokenType = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#UsernameToken";
        public const string X509TokenType = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-x509-token-profile-1.0#X509v3";
        public const string UPTokenPasswordTextValue = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText";
    }

    internal static class SecurityXXX2005Strings
    {
        // Main dictionary strings
        public const string Prefix = IdentityModelStringsVersion1.String144;
        public const string SignatureConfirmation = IdentityModelStringsVersion1.String145;
        public const string ValueAttribute = IdentityModelStringsVersion1.String146;
        public const string TokenTypeAttribute = IdentityModelStringsVersion1.String147;
        public const string ThumbprintSha1ValueType = IdentityModelStringsVersion1.String148;
        public const string EncryptedKeyTokenType = IdentityModelStringsVersion1.String149;
        public const string EncryptedKeyHashValueType = IdentityModelStringsVersion1.String150;
        public const string SamlTokenType = IdentityModelStringsVersion1.String151;
        public const string Saml20TokenType = IdentityModelStringsVersion1.String152;
        public const string Saml11AssertionValueType = IdentityModelStringsVersion1.String153;
        public const string EncryptedHeader = IdentityModelStringsVersion1.String154;
        public const string Namespace = IdentityModelStringsVersion1.String155;
        // String constants
        public const string SecurityProfileNamespace = "http://docs.oasis-open.org/wss/oasis-wss-soap-message-security-1.1";
        public const string SamlTokenProfileNamespace = "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1";
        public const string X509TokenProfileNamespace = "http://docs.oasis-open.org/wss/2004/xx/oasis-2004xx-wss-x509-token-profile-1.1";
    }

    internal static class TrustStrings
    {
    }

    internal static class TrustDec2005Strings
    {
        // Main dictionary strings
        public const string CombinedHashLabel = IdentityModelStringsVersion1.String196;
        public const string RequestSecurityTokenResponse = IdentityModelStringsVersion1.String197;
        public const string TokenType = IdentityModelStringsVersion1.String147;
        public const string KeySize = IdentityModelStringsVersion1.String198;
        public const string RequestedTokenReference = IdentityModelStringsVersion1.String199;
        public const string AppliesTo = IdentityModelStringsVersion1.String200;
        public const string Authenticator = IdentityModelStringsVersion1.String201;
        public const string CombinedHash = IdentityModelStringsVersion1.String202;
        public const string BinaryExchange = IdentityModelStringsVersion1.String203;
        public const string Lifetime = IdentityModelStringsVersion1.String204;
        public const string RequestedSecurityToken = IdentityModelStringsVersion1.String205;
        public const string Entropy = IdentityModelStringsVersion1.String206;
        public const string RequestedProofToken = IdentityModelStringsVersion1.String207;
        public const string ComputedKey = IdentityModelStringsVersion1.String208;
        public const string RequestSecurityToken = IdentityModelStringsVersion1.String209;
        public const string RequestType = IdentityModelStringsVersion1.String210;
        public const string Context = IdentityModelStringsVersion1.String211;
        public const string BinarySecret = IdentityModelStringsVersion1.String212;
        public const string Type = IdentityModelStringsVersion1.String83;
        public const string SpnegoValueTypeUri = IdentityModelStringsVersion1.String240;
        public const string TlsnegoValueTypeUri = IdentityModelStringsVersion1.String241;
        public const string Prefix = IdentityModelStringsVersion1.String242;
        public const string RequestSecurityTokenIssuance = IdentityModelStringsVersion1.String243;
        public const string RequestSecurityTokenIssuanceResponse = IdentityModelStringsVersion1.String244;
        public const string RequestTypeIssue = IdentityModelStringsVersion1.String245;
        public const string AsymmetricKeyBinarySecret = IdentityModelStringsVersion1.String246;
        public const string SymmetricKeyBinarySecret = IdentityModelStringsVersion1.String247;
        public const string NonceBinarySecret = IdentityModelStringsVersion1.String248;
        public const string Psha1ComputedKeyUri = IdentityModelStringsVersion1.String249;
        public const string KeyType = IdentityModelStringsVersion1.String230;
        public const string SymmetricKeyType = IdentityModelStringsVersion1.String247;
        public const string PublicKeyType = IdentityModelStringsVersion1.String250;
        public const string Claims = IdentityModelStringsVersion1.String232;
        public const string InvalidRequestFaultCode = IdentityModelStringsVersion1.String233;
        public const string FailedAuthenticationFaultCode = IdentityModelStringsVersion1.String136;
        public const string UseKey = IdentityModelStringsVersion1.String234;
        public const string SignWith = IdentityModelStringsVersion1.String235;
        public const string EncryptWith = IdentityModelStringsVersion1.String236;
        public const string EncryptionAlgorithm = IdentityModelStringsVersion1.String237;
        public const string CanonicalizationAlgorithm = IdentityModelStringsVersion1.String238;
        public const string ComputedKeyAlgorithm = IdentityModelStringsVersion1.String239;
        public const string RequestSecurityTokenResponseCollection = IdentityModelStringsVersion1.String193;
        public const string Namespace = IdentityModelStringsVersion1.String251;
        public const string BinarySecretClauseType = IdentityModelStringsVersion1.String252;
        public const string RequestSecurityTokenCollectionIssuanceFinalResponse = IdentityModelStringsVersion1.String253;
        public const string RequestSecurityTokenRenewal = IdentityModelStringsVersion1.String254;
        public const string RequestSecurityTokenRenewalResponse = IdentityModelStringsVersion1.String255;
        public const string RequestSecurityTokenCollectionRenewalFinalResponse = IdentityModelStringsVersion1.String256;
        public const string RequestSecurityTokenCancellation = IdentityModelStringsVersion1.String257;
        public const string RequestSecurityTokenCancellationResponse = IdentityModelStringsVersion1.String258;
        public const string RequestSecurityTokenCollectionCancellationFinalResponse = IdentityModelStringsVersion1.String259;
        public const string RequestTypeRenew = IdentityModelStringsVersion1.String260;
        public const string RequestTypeClose = IdentityModelStringsVersion1.String261;
        public const string RenewTarget = IdentityModelStringsVersion1.String222;
        public const string CloseTarget = IdentityModelStringsVersion1.String223;
        public const string RequestedTokenClosed = IdentityModelStringsVersion1.String224;
        public const string RequestedAttachedReference = IdentityModelStringsVersion1.String225;
        public const string RequestedUnattachedReference = IdentityModelStringsVersion1.String226;
        public const string IssuedTokensHeader = IdentityModelStringsVersion1.String227;
        public const string KeyWrapAlgorithm = IdentityModelStringsVersion1.String262;
        public const string BearerKeyType = IdentityModelStringsVersion1.String263;
        public const string SecondaryParameters = IdentityModelStringsVersion1.String264;
        public const string Dialect = IdentityModelStringsVersion1.String265;
        public const string DialectType = IdentityModelStringsVersion1.String266;
    }

    internal static class TrustFeb2005Strings
    {
        // Main dictionary strings
        public const string RequestSecurityTokenResponseCollection = IdentityModelStringsVersion1.String193;
        public const string Namespace = IdentityModelStringsVersion1.String194;
        public const string BinarySecretClauseType = IdentityModelStringsVersion1.String195;
        public const string CombinedHashLabel = IdentityModelStringsVersion1.String196;
        public const string RequestSecurityTokenResponse = IdentityModelStringsVersion1.String197;
        public const string TokenType = IdentityModelStringsVersion1.String147;
        public const string KeySize = IdentityModelStringsVersion1.String198;
        public const string RequestedTokenReference = IdentityModelStringsVersion1.String199;
        public const string AppliesTo = IdentityModelStringsVersion1.String200;
        public const string Authenticator = IdentityModelStringsVersion1.String201;
        public const string CombinedHash = IdentityModelStringsVersion1.String202;
        public const string BinaryExchange = IdentityModelStringsVersion1.String203;
        public const string Lifetime = IdentityModelStringsVersion1.String204;
        public const string RequestedSecurityToken = IdentityModelStringsVersion1.String205;
        public const string Entropy = IdentityModelStringsVersion1.String206;
        public const string RequestedProofToken = IdentityModelStringsVersion1.String207;
        public const string ComputedKey = IdentityModelStringsVersion1.String208;
        public const string RequestSecurityToken = IdentityModelStringsVersion1.String209;
        public const string RequestType = IdentityModelStringsVersion1.String210;
        public const string Context = IdentityModelStringsVersion1.String211;
        public const string BinarySecret = IdentityModelStringsVersion1.String212;
        public const string Type = IdentityModelStringsVersion1.String83;
        public const string SpnegoValueTypeUri = IdentityModelStringsVersion1.String213;
        public const string TlsnegoValueTypeUri = IdentityModelStringsVersion1.String214;
        public const string Prefix = IdentityModelStringsVersion1.String215;
        public const string RequestSecurityTokenIssuance = IdentityModelStringsVersion1.String216;
        public const string RequestSecurityTokenIssuanceResponse = IdentityModelStringsVersion1.String217;
        public const string RequestTypeIssue = IdentityModelStringsVersion1.String218;
        public const string SymmetricKeyBinarySecret = IdentityModelStringsVersion1.String219;
        public const string Psha1ComputedKeyUri = IdentityModelStringsVersion1.String220;
        public const string NonceBinarySecret = IdentityModelStringsVersion1.String221;
        public const string RenewTarget = IdentityModelStringsVersion1.String222;
        public const string CloseTarget = IdentityModelStringsVersion1.String223;
        public const string RequestedTokenClosed = IdentityModelStringsVersion1.String224;
        public const string RequestedAttachedReference = IdentityModelStringsVersion1.String225;
        public const string RequestedUnattachedReference = IdentityModelStringsVersion1.String226;
        public const string IssuedTokensHeader = IdentityModelStringsVersion1.String227;
        public const string RequestTypeRenew = IdentityModelStringsVersion1.String228;
        public const string RequestTypeClose = IdentityModelStringsVersion1.String229;
        public const string KeyType = IdentityModelStringsVersion1.String230;
        public const string SymmetricKeyType = IdentityModelStringsVersion1.String219;
        public const string PublicKeyType = IdentityModelStringsVersion1.String231;
        public const string Claims = IdentityModelStringsVersion1.String232;
        public const string InvalidRequestFaultCode = IdentityModelStringsVersion1.String233;
        public const string FailedAuthenticationFaultCode = IdentityModelStringsVersion1.String136;
        public const string UseKey = IdentityModelStringsVersion1.String234;
        public const string SignWith = IdentityModelStringsVersion1.String235;
        public const string EncryptWith = IdentityModelStringsVersion1.String236;
        public const string EncryptionAlgorithm = IdentityModelStringsVersion1.String237;
        public const string CanonicalizationAlgorithm = IdentityModelStringsVersion1.String238;
        public const string ComputedKeyAlgorithm = IdentityModelStringsVersion1.String239;
    }

    internal static class UtilityStrings
    {
        // Main dictionary strings
        public const string IdAttribute = IdentityModelStringsVersion1.String3;
        public const string Namespace = IdentityModelStringsVersion1.String16;
        public const string Timestamp = IdentityModelStringsVersion1.String17;
        public const string CreatedElement = IdentityModelStringsVersion1.String18;
        public const string ExpiresElement = IdentityModelStringsVersion1.String19;
        public const string Prefix = IdentityModelStringsVersion1.String81;
    }

    internal static class XmlEncryptionStrings
    {
        // Main dictionary strings
        public const string Namespace = IdentityModelStringsVersion1.String156;
        public const string DataReference = IdentityModelStringsVersion1.String157;
        public const string EncryptedData = IdentityModelStringsVersion1.String158;
        public const string EncryptionMethod = IdentityModelStringsVersion1.String159;
        public const string CipherData = IdentityModelStringsVersion1.String160;
        public const string CipherValue = IdentityModelStringsVersion1.String161;
        public const string ReferenceList = IdentityModelStringsVersion1.String162;
        public const string Encoding = IdentityModelStringsVersion1.String163;
        public const string MimeType = IdentityModelStringsVersion1.String164;
        public const string Type = IdentityModelStringsVersion1.String83;
        public const string Id = IdentityModelStringsVersion1.String3;
        public const string CarriedKeyName = IdentityModelStringsVersion1.String165;
        public const string Recipient = IdentityModelStringsVersion1.String166;
        public const string EncryptedKey = IdentityModelStringsVersion1.String167;
        public const string URI = IdentityModelStringsVersion1.String1;
        public const string KeyReference = IdentityModelStringsVersion1.String168;
        public const string Prefix = IdentityModelStringsVersion1.String169;
        public const string ElementType = IdentityModelStringsVersion1.String170;
        public const string ContentType = IdentityModelStringsVersion1.String171;
        public const string AlgorithmAttribute = IdentityModelStringsVersion1.String0;
    }

    internal static class XmlSignatureStrings
    {
        // Main dictionary strings
        public const string Algorithm = IdentityModelStringsVersion1.String0;
        public const string URI = IdentityModelStringsVersion1.String1;
        public const string Reference = IdentityModelStringsVersion1.String2;
        public const string Transforms = IdentityModelStringsVersion1.String4;
        public const string Transform = IdentityModelStringsVersion1.String5;
        public const string DigestMethod = IdentityModelStringsVersion1.String6;
        public const string DigestValue = IdentityModelStringsVersion1.String7;
        public const string Namespace = IdentityModelStringsVersion1.String8;
        public const string EnvelopedSignature = IdentityModelStringsVersion1.String9;
        public const string KeyInfo = IdentityModelStringsVersion1.String10;
        public const string Signature = IdentityModelStringsVersion1.String11;
        public const string SignedInfo = IdentityModelStringsVersion1.String12;
        public const string CanonicalizationMethod = IdentityModelStringsVersion1.String13;
        public const string SignatureMethod = IdentityModelStringsVersion1.String14;
        public const string SignatureValue = IdentityModelStringsVersion1.String15;
        public const string KeyName = IdentityModelStringsVersion1.String82;
        public const string Type = IdentityModelStringsVersion1.String83;
        public const string MgmtData = IdentityModelStringsVersion1.String84;
        public const string Prefix = IdentityModelStringsVersion1.String85;
        public const string KeyValue = IdentityModelStringsVersion1.String86;
        public const string RsaKeyValue = IdentityModelStringsVersion1.String87;
        public const string Modulus = IdentityModelStringsVersion1.String88;
        public const string Exponent = IdentityModelStringsVersion1.String89;
        public const string X509Data = IdentityModelStringsVersion1.String90;
        public const string X509IssuerSerial = IdentityModelStringsVersion1.String91;
        public const string X509IssuerName = IdentityModelStringsVersion1.String92;
        public const string X509SerialNumber = IdentityModelStringsVersion1.String93;
        public const string X509Certificate = IdentityModelStringsVersion1.String94;
        // String constants
        public const string SecurityJan2004Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";
        public const string SecurityJan2004Prefix = "o";
        public const string X509Ski = "X509SKI";
        public const string TransformationParameters = "TransformationParameters";
    }
}
