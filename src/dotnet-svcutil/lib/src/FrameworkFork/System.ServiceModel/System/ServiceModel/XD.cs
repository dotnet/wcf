// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// NOTE: this file was generated from 'xd.xml'

using Microsoft.Xml;

namespace System.ServiceModel
{
    // Static Xml Dictionary
    internal static class XD
    {
        static public ServiceModelDictionary Dictionary { get { return ServiceModelDictionary.CurrentVersion; } }

        private static ActivityIdFlowDictionary s_activityIdFlowDictionary;
        private static AddressingDictionary s_addressingDictionary;
        private static Addressing10Dictionary s_addressing10Dictionary;
        private static Addressing200408Dictionary s_addressing200408Dictionary;
        private static AddressingNoneDictionary s_addressingNoneDictionary;
        private static DotNetSecurityDictionary s_dotNetSecurityDictionary;
        private static MessageDictionary s_messageDictionary;
        private static Message11Dictionary s_message11Dictionary;
        private static Message12Dictionary s_message12Dictionary;
        private static PolicyDictionary s_policyDictionary;
        private static SecureConversationFeb2005Dictionary s_secureConversationFeb2005Dictionary;
        private static SecurityAlgorithmDictionary s_securityAlgorithmDictionary;
        private static SecurityJan2004Dictionary s_securityJan2004Dictionary;
        private static SecurityXXX2005Dictionary s_securityXXX2005Dictionary;
        private static TrustFeb2005Dictionary s_trustFeb2005Dictionary;
        private static UtilityDictionary s_utilityDictionary;
        private static WsrmFeb2005Dictionary s_wsrmFeb2005Dictionary;
        private static XmlEncryptionDictionary s_xmlEncryptionDictionary;
        private static XmlSignatureDictionary s_xmlSignatureDictionary;

        static public ActivityIdFlowDictionary ActivityIdFlowDictionary
        {
            get
            {
                if (s_activityIdFlowDictionary == null)
                    s_activityIdFlowDictionary = new ActivityIdFlowDictionary(Dictionary);
                return s_activityIdFlowDictionary;
            }
        }

        static public AddressingDictionary AddressingDictionary
        {
            get
            {
                if (s_addressingDictionary == null)
                    s_addressingDictionary = new AddressingDictionary(Dictionary);
                return s_addressingDictionary;
            }
        }

        static public Addressing10Dictionary Addressing10Dictionary
        {
            get
            {
                if (s_addressing10Dictionary == null)
                    s_addressing10Dictionary = new Addressing10Dictionary(Dictionary);
                return s_addressing10Dictionary;
            }
        }

        static public Addressing200408Dictionary Addressing200408Dictionary
        {
            get
            {
                if (s_addressing200408Dictionary == null)
                    s_addressing200408Dictionary = new Addressing200408Dictionary(Dictionary);
                return s_addressing200408Dictionary;
            }
        }

        static public AddressingNoneDictionary AddressingNoneDictionary
        {
            get
            {
                if (s_addressingNoneDictionary == null)
                    s_addressingNoneDictionary = new AddressingNoneDictionary(Dictionary);
                return s_addressingNoneDictionary;
            }
        }

        static public DotNetSecurityDictionary DotNetSecurityDictionary
        {
            get
            {
                if (s_dotNetSecurityDictionary == null)
                    s_dotNetSecurityDictionary = new DotNetSecurityDictionary(Dictionary);
                return s_dotNetSecurityDictionary;
            }
        }

        static public MessageDictionary MessageDictionary
        {
            get
            {
                if (s_messageDictionary == null)
                    s_messageDictionary = new MessageDictionary(Dictionary);
                return s_messageDictionary;
            }
        }

        static public Message11Dictionary Message11Dictionary
        {
            get
            {
                if (s_message11Dictionary == null)
                    s_message11Dictionary = new Message11Dictionary(Dictionary);
                return s_message11Dictionary;
            }
        }

        static public Message12Dictionary Message12Dictionary
        {
            get
            {
                if (s_message12Dictionary == null)
                    s_message12Dictionary = new Message12Dictionary(Dictionary);
                return s_message12Dictionary;
            }
        }
        static public PolicyDictionary PolicyDictionary
        {
            get
            {
                if (s_policyDictionary == null)
                    s_policyDictionary = new PolicyDictionary(Dictionary);
                return s_policyDictionary;
            }
        }


        static public SecureConversationFeb2005Dictionary SecureConversationFeb2005Dictionary
        {
            get
            {
                if (s_secureConversationFeb2005Dictionary == null)
                    s_secureConversationFeb2005Dictionary = new SecureConversationFeb2005Dictionary(Dictionary);
                return s_secureConversationFeb2005Dictionary;
            }
        }

        static public SecurityAlgorithmDictionary SecurityAlgorithmDictionary
        {
            get
            {
                if (s_securityAlgorithmDictionary == null)
                    s_securityAlgorithmDictionary = new SecurityAlgorithmDictionary(Dictionary);
                return s_securityAlgorithmDictionary;
            }
        }

        static public SecurityJan2004Dictionary SecurityJan2004Dictionary
        {
            get
            {
                if (s_securityJan2004Dictionary == null)
                    s_securityJan2004Dictionary = new SecurityJan2004Dictionary(Dictionary);
                return s_securityJan2004Dictionary;
            }
        }

        static public SecurityXXX2005Dictionary SecurityXXX2005Dictionary
        {
            get
            {
                if (s_securityXXX2005Dictionary == null)
                    s_securityXXX2005Dictionary = new SecurityXXX2005Dictionary(Dictionary);
                return s_securityXXX2005Dictionary;
            }
        }

        static public TrustFeb2005Dictionary TrustFeb2005Dictionary
        {
            get
            {
                if (s_trustFeb2005Dictionary == null)
                    s_trustFeb2005Dictionary = new TrustFeb2005Dictionary(Dictionary);
                return s_trustFeb2005Dictionary;
            }
        }

        static public UtilityDictionary UtilityDictionary
        {
            get
            {
                if (s_utilityDictionary == null)
                    s_utilityDictionary = new UtilityDictionary(Dictionary);
                return s_utilityDictionary;
            }
        }

        static public WsrmFeb2005Dictionary WsrmFeb2005Dictionary
        {
            get
            {
                if (s_wsrmFeb2005Dictionary == null)
                    s_wsrmFeb2005Dictionary = new WsrmFeb2005Dictionary(Dictionary);
                return s_wsrmFeb2005Dictionary;
            }
        }

        static public XmlEncryptionDictionary XmlEncryptionDictionary
        {
            get
            {
                if (s_xmlEncryptionDictionary == null)
                    s_xmlEncryptionDictionary = new XmlEncryptionDictionary(Dictionary);
                return s_xmlEncryptionDictionary;
            }
        }

        static public XmlSignatureDictionary XmlSignatureDictionary
        {
            get
            {
                if (s_xmlSignatureDictionary == null)
                    s_xmlSignatureDictionary = new XmlSignatureDictionary(Dictionary);
                return s_xmlSignatureDictionary;
            }
        }
    }

    internal class ActivityIdFlowDictionary
    {
        public XmlDictionaryString ActivityId;
        public XmlDictionaryString ActivityIdNamespace;

        public ActivityIdFlowDictionary(ServiceModelDictionary dictionary)
        {
            this.ActivityId = dictionary.CreateString(ServiceModelStringsVersion1.String425, 425);
            this.ActivityIdNamespace = dictionary.CreateString(ServiceModelStringsVersion1.String426, 426);
        }
    }

    internal class AddressingDictionary
    {
        public XmlDictionaryString Action;
        public XmlDictionaryString To;
        public XmlDictionaryString RelatesTo;
        public XmlDictionaryString MessageId;
        public XmlDictionaryString Address;
        public XmlDictionaryString ReplyTo;
        public XmlDictionaryString Empty;
        public XmlDictionaryString From;
        public XmlDictionaryString FaultTo;
        public XmlDictionaryString EndpointReference;
        public XmlDictionaryString PortType;
        public XmlDictionaryString ServiceName;
        public XmlDictionaryString PortName;
        public XmlDictionaryString ReferenceProperties;
        public XmlDictionaryString RelationshipType;
        public XmlDictionaryString Reply;
        public XmlDictionaryString Prefix;
        public XmlDictionaryString IdentityExtensionNamespace;
        public XmlDictionaryString Identity;
        public XmlDictionaryString Spn;
        public XmlDictionaryString Upn;
        public XmlDictionaryString Rsa;
        public XmlDictionaryString Dns;
        public XmlDictionaryString X509v3Certificate;
        public XmlDictionaryString ReferenceParameters;
        public XmlDictionaryString IsReferenceParameter;

        public AddressingDictionary(ServiceModelDictionary dictionary)
        {
            this.Action = dictionary.CreateString(ServiceModelStringsVersion1.String5, 5);
            this.To = dictionary.CreateString(ServiceModelStringsVersion1.String6, 6);
            this.RelatesTo = dictionary.CreateString(ServiceModelStringsVersion1.String9, 9);
            this.MessageId = dictionary.CreateString(ServiceModelStringsVersion1.String13, 13);
            this.Address = dictionary.CreateString(ServiceModelStringsVersion1.String21, 21);
            this.ReplyTo = dictionary.CreateString(ServiceModelStringsVersion1.String22, 22);
            this.Empty = dictionary.CreateString(ServiceModelStringsVersion1.String81, 81);
            this.From = dictionary.CreateString(ServiceModelStringsVersion1.String82, 82);
            this.FaultTo = dictionary.CreateString(ServiceModelStringsVersion1.String83, 83);
            this.EndpointReference = dictionary.CreateString(ServiceModelStringsVersion1.String84, 84);
            this.PortType = dictionary.CreateString(ServiceModelStringsVersion1.String85, 85);
            this.ServiceName = dictionary.CreateString(ServiceModelStringsVersion1.String86, 86);
            this.PortName = dictionary.CreateString(ServiceModelStringsVersion1.String87, 87);
            this.ReferenceProperties = dictionary.CreateString(ServiceModelStringsVersion1.String88, 88);
            this.RelationshipType = dictionary.CreateString(ServiceModelStringsVersion1.String89, 89);
            this.Reply = dictionary.CreateString(ServiceModelStringsVersion1.String90, 90);
            this.Prefix = dictionary.CreateString(ServiceModelStringsVersion1.String91, 91);
            this.IdentityExtensionNamespace = dictionary.CreateString(ServiceModelStringsVersion1.String92, 92);
            this.Identity = dictionary.CreateString(ServiceModelStringsVersion1.String93, 93);
            this.Spn = dictionary.CreateString(ServiceModelStringsVersion1.String94, 94);
            this.Upn = dictionary.CreateString(ServiceModelStringsVersion1.String95, 95);
            this.Rsa = dictionary.CreateString(ServiceModelStringsVersion1.String96, 96);
            this.Dns = dictionary.CreateString(ServiceModelStringsVersion1.String97, 97);
            this.X509v3Certificate = dictionary.CreateString(ServiceModelStringsVersion1.String98, 98);
            this.ReferenceParameters = dictionary.CreateString(ServiceModelStringsVersion1.String100, 100);
            this.IsReferenceParameter = dictionary.CreateString(ServiceModelStringsVersion1.String101, 101);
        }
    }

    internal class Addressing10Dictionary
    {
        public XmlDictionaryString Namespace;
        public XmlDictionaryString Anonymous;
        public XmlDictionaryString FaultAction;
        public XmlDictionaryString ReplyRelationship;
        public XmlDictionaryString NoneAddress;
        public XmlDictionaryString Metadata;

        public Addressing10Dictionary(ServiceModelDictionary dictionary)
        {
            this.Namespace = dictionary.CreateString(ServiceModelStringsVersion1.String3, 3);
            this.Anonymous = dictionary.CreateString(ServiceModelStringsVersion1.String10, 10);
            this.FaultAction = dictionary.CreateString(ServiceModelStringsVersion1.String99, 99);
            this.ReplyRelationship = dictionary.CreateString(ServiceModelStringsVersion1.String102, 102);
            this.NoneAddress = dictionary.CreateString(ServiceModelStringsVersion1.String103, 103);
            this.Metadata = dictionary.CreateString(ServiceModelStringsVersion1.String104, 104);
        }
    }

    internal class Addressing200408Dictionary
    {
        public XmlDictionaryString Namespace;
        public XmlDictionaryString Anonymous;
        public XmlDictionaryString FaultAction;

        public Addressing200408Dictionary(ServiceModelDictionary dictionary)
        {
            this.Namespace = dictionary.CreateString(ServiceModelStringsVersion1.String105, 105);
            this.Anonymous = dictionary.CreateString(ServiceModelStringsVersion1.String106, 106);
            this.FaultAction = dictionary.CreateString(ServiceModelStringsVersion1.String107, 107);
        }
    }

    internal class AddressingNoneDictionary
    {
        public XmlDictionaryString Namespace;

        public AddressingNoneDictionary(ServiceModelDictionary dictionary)
        {
            this.Namespace = dictionary.CreateString(ServiceModelStringsVersion1.String439, 439);
        }
    }

    internal class MessageDictionary
    {
        public XmlDictionaryString MustUnderstand;
        public XmlDictionaryString Envelope;
        public XmlDictionaryString Header;
        public XmlDictionaryString Body;
        public XmlDictionaryString Prefix;
        public XmlDictionaryString Fault;
        public XmlDictionaryString MustUnderstandFault;
        public XmlDictionaryString Namespace;

        public MessageDictionary(ServiceModelDictionary dictionary)
        {
            this.MustUnderstand = dictionary.CreateString(ServiceModelStringsVersion1.String0, 0);
            this.Envelope = dictionary.CreateString(ServiceModelStringsVersion1.String1, 1);
            this.Header = dictionary.CreateString(ServiceModelStringsVersion1.String4, 4);
            this.Body = dictionary.CreateString(ServiceModelStringsVersion1.String7, 7);
            this.Prefix = dictionary.CreateString(ServiceModelStringsVersion1.String66, 66);
            this.Fault = dictionary.CreateString(ServiceModelStringsVersion1.String67, 67);
            this.MustUnderstandFault = dictionary.CreateString(ServiceModelStringsVersion1.String68, 68);
            this.Namespace = dictionary.CreateString(ServiceModelStringsVersion1.String440, 440);
        }
    }

    internal class Message12Dictionary
    {
        public XmlDictionaryString Namespace;
        public XmlDictionaryString Role;
        public XmlDictionaryString Relay;
        public XmlDictionaryString FaultCode;
        public XmlDictionaryString FaultReason;
        public XmlDictionaryString FaultText;
        public XmlDictionaryString FaultNode;
        public XmlDictionaryString FaultRole;
        public XmlDictionaryString FaultDetail;
        public XmlDictionaryString FaultValue;
        public XmlDictionaryString FaultSubcode;
        public XmlDictionaryString NotUnderstood;
        public XmlDictionaryString QName;

        public Message12Dictionary(ServiceModelDictionary dictionary)
        {
            this.Namespace = dictionary.CreateString(ServiceModelStringsVersion1.String2, 2);
            this.Role = dictionary.CreateString(ServiceModelStringsVersion1.String69, 69);
            this.Relay = dictionary.CreateString(ServiceModelStringsVersion1.String70, 70);
            this.FaultCode = dictionary.CreateString(ServiceModelStringsVersion1.String71, 71);
            this.FaultReason = dictionary.CreateString(ServiceModelStringsVersion1.String72, 72);
            this.FaultText = dictionary.CreateString(ServiceModelStringsVersion1.String73, 73);
            this.FaultNode = dictionary.CreateString(ServiceModelStringsVersion1.String74, 74);
            this.FaultRole = dictionary.CreateString(ServiceModelStringsVersion1.String75, 75);
            this.FaultDetail = dictionary.CreateString(ServiceModelStringsVersion1.String76, 76);
            this.FaultValue = dictionary.CreateString(ServiceModelStringsVersion1.String77, 77);
            this.FaultSubcode = dictionary.CreateString(ServiceModelStringsVersion1.String78, 78);
            this.NotUnderstood = dictionary.CreateString(ServiceModelStringsVersion1.String79, 79);
            this.QName = dictionary.CreateString(ServiceModelStringsVersion1.String80, 80);
        }
    }

    internal class PolicyDictionary
    {
        public XmlDictionaryString Namespace;

        public PolicyDictionary(ServiceModelDictionary dictionary)
        {
            this.Namespace = dictionary.CreateString(ServiceModelStringsVersion1.String428, 428);
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
        public XmlDictionaryString Prefix;
        public XmlDictionaryString DerivedKeyTokenType;
        public XmlDictionaryString SecurityContextTokenType;
        public XmlDictionaryString SecurityContextTokenReferenceValueType;
        public XmlDictionaryString RequestSecurityContextIssuance;
        public XmlDictionaryString RequestSecurityContextIssuanceResponse;
        public XmlDictionaryString RenewNeededFaultCode;
        public XmlDictionaryString BadContextTokenFaultCode;

        public SecureConversationDictionary()
        {
        }

        public SecureConversationDictionary(ServiceModelDictionary dictionary)
        {
        }
    }

    internal class SecureConversationFeb2005Dictionary : SecureConversationDictionary
    {
        public XmlDictionaryString RequestSecurityContextRenew;
        public XmlDictionaryString RequestSecurityContextRenewResponse;
        public XmlDictionaryString RequestSecurityContextClose;
        public XmlDictionaryString RequestSecurityContextCloseResponse;

        public SecureConversationFeb2005Dictionary(ServiceModelDictionary dictionary)
            : base(dictionary)
        {
            this.Namespace = dictionary.CreateString(ServiceModelStringsVersion1.String38, 38);
            this.DerivedKeyToken = dictionary.CreateString(ServiceModelStringsVersion1.String39, 39);
            this.Nonce = dictionary.CreateString(ServiceModelStringsVersion1.String40, 40);
            this.Length = dictionary.CreateString(ServiceModelStringsVersion1.String56, 56);
            this.SecurityContextToken = dictionary.CreateString(ServiceModelStringsVersion1.String115, 115);
            this.AlgorithmAttribute = dictionary.CreateString(ServiceModelStringsVersion1.String8, 8);
            this.Generation = dictionary.CreateString(ServiceModelStringsVersion1.String116, 116);
            this.Label = dictionary.CreateString(ServiceModelStringsVersion1.String117, 117);
            this.Offset = dictionary.CreateString(ServiceModelStringsVersion1.String118, 118);
            this.Properties = dictionary.CreateString(ServiceModelStringsVersion1.String119, 119);
            this.Identifier = dictionary.CreateString(ServiceModelStringsVersion1.String15, 15);
            this.Cookie = dictionary.CreateString(ServiceModelStringsVersion1.String120, 120);
            this.RenewNeededFaultCode = dictionary.CreateString(ServiceModelStringsVersion1.String127, 127);
            this.BadContextTokenFaultCode = dictionary.CreateString(ServiceModelStringsVersion1.String128, 128);
            this.Prefix = dictionary.CreateString(ServiceModelStringsVersion1.String129, 129);
            this.DerivedKeyTokenType = dictionary.CreateString(ServiceModelStringsVersion1.String130, 130);
            this.SecurityContextTokenType = dictionary.CreateString(ServiceModelStringsVersion1.String131, 131);
            this.SecurityContextTokenReferenceValueType = dictionary.CreateString(ServiceModelStringsVersion1.String131, 131);
            this.RequestSecurityContextIssuance = dictionary.CreateString(ServiceModelStringsVersion1.String132, 132);
            this.RequestSecurityContextIssuanceResponse = dictionary.CreateString(ServiceModelStringsVersion1.String133, 133);
            this.RequestSecurityContextRenew = dictionary.CreateString(ServiceModelStringsVersion1.String134, 134);
            this.RequestSecurityContextRenewResponse = dictionary.CreateString(ServiceModelStringsVersion1.String135, 135);
            this.RequestSecurityContextClose = dictionary.CreateString(ServiceModelStringsVersion1.String136, 136);
            this.RequestSecurityContextCloseResponse = dictionary.CreateString(ServiceModelStringsVersion1.String137, 137);
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

        public SecurityAlgorithmDictionary(ServiceModelDictionary dictionary)
        {
            this.Aes128Encryption = dictionary.CreateString(ServiceModelStringsVersion1.String138, 138);
            this.Aes128KeyWrap = dictionary.CreateString(ServiceModelStringsVersion1.String139, 139);
            this.Aes192Encryption = dictionary.CreateString(ServiceModelStringsVersion1.String140, 140);
            this.Aes192KeyWrap = dictionary.CreateString(ServiceModelStringsVersion1.String141, 141);
            this.Aes256Encryption = dictionary.CreateString(ServiceModelStringsVersion1.String142, 142);
            this.Aes256KeyWrap = dictionary.CreateString(ServiceModelStringsVersion1.String143, 143);
            this.DesEncryption = dictionary.CreateString(ServiceModelStringsVersion1.String144, 144);
            this.DsaSha1Signature = dictionary.CreateString(ServiceModelStringsVersion1.String145, 145);
            this.ExclusiveC14n = dictionary.CreateString(ServiceModelStringsVersion1.String111, 111);
            this.ExclusiveC14nWithComments = dictionary.CreateString(ServiceModelStringsVersion1.String146, 146);
            this.HmacSha1Signature = dictionary.CreateString(ServiceModelStringsVersion1.String147, 147);
            this.HmacSha256Signature = dictionary.CreateString(ServiceModelStringsVersion1.String148, 148);
            this.Psha1KeyDerivation = dictionary.CreateString(ServiceModelStringsVersion1.String149, 149);
            this.Ripemd160Digest = dictionary.CreateString(ServiceModelStringsVersion1.String150, 150);
            this.RsaOaepKeyWrap = dictionary.CreateString(ServiceModelStringsVersion1.String151, 151);
            this.RsaSha1Signature = dictionary.CreateString(ServiceModelStringsVersion1.String152, 152);
            this.RsaSha256Signature = dictionary.CreateString(ServiceModelStringsVersion1.String153, 153);
            this.RsaV15KeyWrap = dictionary.CreateString(ServiceModelStringsVersion1.String154, 154);
            this.Sha1Digest = dictionary.CreateString(ServiceModelStringsVersion1.String155, 155);
            this.Sha256Digest = dictionary.CreateString(ServiceModelStringsVersion1.String156, 156);
            this.Sha512Digest = dictionary.CreateString(ServiceModelStringsVersion1.String157, 157);
            this.TripleDesEncryption = dictionary.CreateString(ServiceModelStringsVersion1.String158, 158);
            this.TripleDesKeyWrap = dictionary.CreateString(ServiceModelStringsVersion1.String159, 159);
            this.TlsSspiKeyWrap = dictionary.CreateString(ServiceModelStringsVersion1.String160, 160);
            this.WindowsSspiKeyWrap = dictionary.CreateString(ServiceModelStringsVersion1.String161, 161);
        }
    }

    internal class SecurityJan2004Dictionary
    {
        public XmlDictionaryString SecurityTokenReference;
        public XmlDictionaryString Namespace;
        public XmlDictionaryString Security;
        public XmlDictionaryString ValueType;
        public XmlDictionaryString TypeAttribute;
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
        public XmlDictionaryString KerberosHashValueType;

        public SecurityJan2004Dictionary(ServiceModelDictionary dictionary)
        {
            this.SecurityTokenReference = dictionary.CreateString(ServiceModelStringsVersion1.String30, 30);
            this.Namespace = dictionary.CreateString(ServiceModelStringsVersion1.String36, 36);
            this.Security = dictionary.CreateString(ServiceModelStringsVersion1.String52, 52);
            this.ValueType = dictionary.CreateString(ServiceModelStringsVersion1.String58, 58);
            this.TypeAttribute = dictionary.CreateString(ServiceModelStringsVersion1.String59, 59);
            this.Prefix = dictionary.CreateString(ServiceModelStringsVersion1.String164, 164);
            this.NonceElement = dictionary.CreateString(ServiceModelStringsVersion1.String40, 40);
            this.PasswordElement = dictionary.CreateString(ServiceModelStringsVersion1.String165, 165);
            this.PasswordTextName = dictionary.CreateString(ServiceModelStringsVersion1.String166, 166);
            this.UserNameElement = dictionary.CreateString(ServiceModelStringsVersion1.String167, 167);
            this.UserNameTokenElement = dictionary.CreateString(ServiceModelStringsVersion1.String168, 168);
            this.BinarySecurityToken = dictionary.CreateString(ServiceModelStringsVersion1.String169, 169);
            this.EncodingType = dictionary.CreateString(ServiceModelStringsVersion1.String170, 170);
            this.Reference = dictionary.CreateString(ServiceModelStringsVersion1.String12, 12);
            this.URI = dictionary.CreateString(ServiceModelStringsVersion1.String11, 11);
            this.KeyIdentifier = dictionary.CreateString(ServiceModelStringsVersion1.String171, 171);
            this.EncodingTypeValueBase64Binary = dictionary.CreateString(ServiceModelStringsVersion1.String172, 172);
            this.EncodingTypeValueHexBinary = dictionary.CreateString(ServiceModelStringsVersion1.String173, 173);
            this.EncodingTypeValueText = dictionary.CreateString(ServiceModelStringsVersion1.String174, 174);
            this.X509SKIValueType = dictionary.CreateString(ServiceModelStringsVersion1.String175, 175);
            this.KerberosTokenTypeGSS = dictionary.CreateString(ServiceModelStringsVersion1.String176, 176);
            this.KerberosTokenType1510 = dictionary.CreateString(ServiceModelStringsVersion1.String177, 177);
            this.SamlAssertionIdValueType = dictionary.CreateString(ServiceModelStringsVersion1.String178, 178);
            this.SamlAssertion = dictionary.CreateString(ServiceModelStringsVersion1.String179, 179);
            this.SamlUri = dictionary.CreateString(ServiceModelStringsVersion1.String180, 180);
            this.RelAssertionValueType = dictionary.CreateString(ServiceModelStringsVersion1.String181, 181);
            this.FailedAuthenticationFaultCode = dictionary.CreateString(ServiceModelStringsVersion1.String182, 182);
            this.InvalidSecurityTokenFaultCode = dictionary.CreateString(ServiceModelStringsVersion1.String183, 183);
            this.InvalidSecurityFaultCode = dictionary.CreateString(ServiceModelStringsVersion1.String184, 184);
            this.KerberosHashValueType = dictionary.CreateString(ServiceModelStringsVersion1.String427, 427);
        }
    }

    internal class SecurityXXX2005Dictionary
    {
        public XmlDictionaryString EncryptedHeader;
        public XmlDictionaryString Namespace;
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

        public SecurityXXX2005Dictionary(ServiceModelDictionary dictionary)
        {
            this.EncryptedHeader = dictionary.CreateString(ServiceModelStringsVersion1.String60, 60);
            this.Namespace = dictionary.CreateString(ServiceModelStringsVersion1.String61, 61);
            this.Prefix = dictionary.CreateString(ServiceModelStringsVersion1.String185, 185);
            this.SignatureConfirmation = dictionary.CreateString(ServiceModelStringsVersion1.String186, 186);
            this.ValueAttribute = dictionary.CreateString(ServiceModelStringsVersion1.String77, 77);
            this.TokenTypeAttribute = dictionary.CreateString(ServiceModelStringsVersion1.String187, 187);
            this.ThumbprintSha1ValueType = dictionary.CreateString(ServiceModelStringsVersion1.String188, 188);
            this.EncryptedKeyTokenType = dictionary.CreateString(ServiceModelStringsVersion1.String189, 189);
            this.EncryptedKeyHashValueType = dictionary.CreateString(ServiceModelStringsVersion1.String190, 190);
            this.SamlTokenType = dictionary.CreateString(ServiceModelStringsVersion1.String191, 191);
            this.Saml20TokenType = dictionary.CreateString(ServiceModelStringsVersion1.String192, 192);
            this.Saml11AssertionValueType = dictionary.CreateString(ServiceModelStringsVersion1.String193, 193);
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
        public XmlDictionaryString Psha1ComputedKeyUri;
        public XmlDictionaryString SymmetricKeyBinarySecret;
        public XmlDictionaryString NonceBinarySecret;
        public XmlDictionaryString KeyType;
        public XmlDictionaryString SymmetricKeyType;
        public XmlDictionaryString PublicKeyType;
        public XmlDictionaryString Claims;
        public XmlDictionaryString InvalidRequestFaultCode;
        public XmlDictionaryString FailedAuthenticationFaultCode;
        public XmlDictionaryString RequestFailedFaultCode;
        public XmlDictionaryString SignWith;
        public XmlDictionaryString EncryptWith;
        public XmlDictionaryString EncryptionAlgorithm;
        public XmlDictionaryString CanonicalizationAlgorithm;
        public XmlDictionaryString ComputedKeyAlgorithm;
        public XmlDictionaryString UseKey;
        public XmlDictionaryString RenewTarget;
        public XmlDictionaryString CloseTarget;
        public XmlDictionaryString RequestedTokenClosed;
        public XmlDictionaryString RequestedAttachedReference;
        public XmlDictionaryString RequestedUnattachedReference;
        public XmlDictionaryString IssuedTokensHeader;
        public XmlDictionaryString RequestTypeRenew;
        public XmlDictionaryString RequestTypeClose;

        public TrustDictionary()
        {
        }

        public TrustDictionary(ServiceModelDictionary dictionary)
        {
        }
    }

    internal class TrustFeb2005Dictionary : TrustDictionary
    {
        public TrustFeb2005Dictionary(ServiceModelDictionary dictionary)
            : base(dictionary)
        {
            this.RequestSecurityTokenResponseCollection = dictionary.CreateString(ServiceModelStringsVersion1.String62, 62);
            this.Namespace = dictionary.CreateString(ServiceModelStringsVersion1.String63, 63);
            this.BinarySecretClauseType = dictionary.CreateString(ServiceModelStringsVersion1.String64, 64);
            this.CombinedHashLabel = dictionary.CreateString(ServiceModelStringsVersion1.String194, 194);
            this.RequestSecurityTokenResponse = dictionary.CreateString(ServiceModelStringsVersion1.String195, 195);
            this.TokenType = dictionary.CreateString(ServiceModelStringsVersion1.String187, 187);
            this.KeySize = dictionary.CreateString(ServiceModelStringsVersion1.String196, 196);
            this.RequestedTokenReference = dictionary.CreateString(ServiceModelStringsVersion1.String197, 197);
            this.AppliesTo = dictionary.CreateString(ServiceModelStringsVersion1.String198, 198);
            this.Authenticator = dictionary.CreateString(ServiceModelStringsVersion1.String199, 199);
            this.CombinedHash = dictionary.CreateString(ServiceModelStringsVersion1.String200, 200);
            this.BinaryExchange = dictionary.CreateString(ServiceModelStringsVersion1.String201, 201);
            this.Lifetime = dictionary.CreateString(ServiceModelStringsVersion1.String202, 202);
            this.RequestedSecurityToken = dictionary.CreateString(ServiceModelStringsVersion1.String203, 203);
            this.Entropy = dictionary.CreateString(ServiceModelStringsVersion1.String204, 204);
            this.RequestedProofToken = dictionary.CreateString(ServiceModelStringsVersion1.String205, 205);
            this.ComputedKey = dictionary.CreateString(ServiceModelStringsVersion1.String206, 206);
            this.RequestSecurityToken = dictionary.CreateString(ServiceModelStringsVersion1.String207, 207);
            this.RequestType = dictionary.CreateString(ServiceModelStringsVersion1.String208, 208);
            this.Context = dictionary.CreateString(ServiceModelStringsVersion1.String209, 209);
            this.BinarySecret = dictionary.CreateString(ServiceModelStringsVersion1.String210, 210);
            this.Type = dictionary.CreateString(ServiceModelStringsVersion1.String59, 59);
            this.SpnegoValueTypeUri = dictionary.CreateString(ServiceModelStringsVersion1.String233, 233);
            this.TlsnegoValueTypeUri = dictionary.CreateString(ServiceModelStringsVersion1.String234, 234);
            this.Prefix = dictionary.CreateString(ServiceModelStringsVersion1.String235, 235);
            this.RequestSecurityTokenIssuance = dictionary.CreateString(ServiceModelStringsVersion1.String236, 236);
            this.RequestSecurityTokenIssuanceResponse = dictionary.CreateString(ServiceModelStringsVersion1.String237, 237);
            this.RequestTypeIssue = dictionary.CreateString(ServiceModelStringsVersion1.String238, 238);
            this.SymmetricKeyBinarySecret = dictionary.CreateString(ServiceModelStringsVersion1.String239, 239);
            this.Psha1ComputedKeyUri = dictionary.CreateString(ServiceModelStringsVersion1.String240, 240);
            this.NonceBinarySecret = dictionary.CreateString(ServiceModelStringsVersion1.String241, 241);
            this.RenewTarget = dictionary.CreateString(ServiceModelStringsVersion1.String242, 242);
            this.CloseTarget = dictionary.CreateString(ServiceModelStringsVersion1.String243, 243);
            this.RequestedTokenClosed = dictionary.CreateString(ServiceModelStringsVersion1.String244, 244);
            this.RequestedAttachedReference = dictionary.CreateString(ServiceModelStringsVersion1.String245, 245);
            this.RequestedUnattachedReference = dictionary.CreateString(ServiceModelStringsVersion1.String246, 246);
            this.IssuedTokensHeader = dictionary.CreateString(ServiceModelStringsVersion1.String247, 247);
            this.RequestTypeRenew = dictionary.CreateString(ServiceModelStringsVersion1.String248, 248);
            this.RequestTypeClose = dictionary.CreateString(ServiceModelStringsVersion1.String249, 249);
            this.KeyType = dictionary.CreateString(ServiceModelStringsVersion1.String221, 221);
            this.SymmetricKeyType = dictionary.CreateString(ServiceModelStringsVersion1.String239, 239);
            this.PublicKeyType = dictionary.CreateString(ServiceModelStringsVersion1.String250, 250);
            this.Claims = dictionary.CreateString(ServiceModelStringsVersion1.String224, 224);
            this.InvalidRequestFaultCode = dictionary.CreateString(ServiceModelStringsVersion1.String225, 225);
            this.FailedAuthenticationFaultCode = dictionary.CreateString(ServiceModelStringsVersion1.String182, 182);
            this.UseKey = dictionary.CreateString(ServiceModelStringsVersion1.String232, 232);
            this.SignWith = dictionary.CreateString(ServiceModelStringsVersion1.String227, 227);
            this.EncryptWith = dictionary.CreateString(ServiceModelStringsVersion1.String228, 228);
            this.EncryptionAlgorithm = dictionary.CreateString(ServiceModelStringsVersion1.String229, 229);
            this.CanonicalizationAlgorithm = dictionary.CreateString(ServiceModelStringsVersion1.String230, 230);
            this.ComputedKeyAlgorithm = dictionary.CreateString(ServiceModelStringsVersion1.String231, 231);
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
        public XmlDictionaryString UniqueEndpointHeaderName;
        public XmlDictionaryString UniqueEndpointHeaderNamespace;

        public UtilityDictionary(ServiceModelDictionary dictionary)
        {
            this.IdAttribute = dictionary.CreateString(ServiceModelStringsVersion1.String14, 14);
            this.Namespace = dictionary.CreateString(ServiceModelStringsVersion1.String51, 51);
            this.Timestamp = dictionary.CreateString(ServiceModelStringsVersion1.String53, 53);
            this.CreatedElement = dictionary.CreateString(ServiceModelStringsVersion1.String54, 54);
            this.ExpiresElement = dictionary.CreateString(ServiceModelStringsVersion1.String55, 55);
            this.Prefix = dictionary.CreateString(ServiceModelStringsVersion1.String305, 305);
            this.UniqueEndpointHeaderName = dictionary.CreateString(ServiceModelStringsVersion1.String306, 306);
            this.UniqueEndpointHeaderNamespace = dictionary.CreateString(ServiceModelStringsVersion1.String307, 307);
        }
    }

    internal class WsrmFeb2005Dictionary
    {
        public XmlDictionaryString Identifier;
        public XmlDictionaryString Namespace;
        public XmlDictionaryString SequenceAcknowledgement;
        public XmlDictionaryString AcknowledgementRange;
        public XmlDictionaryString Upper;
        public XmlDictionaryString Lower;
        public XmlDictionaryString BufferRemaining;
        public XmlDictionaryString NETNamespace;
        public XmlDictionaryString SequenceAcknowledgementAction;
        public XmlDictionaryString Sequence;
        public XmlDictionaryString MessageNumber;
        public XmlDictionaryString AckRequested;
        public XmlDictionaryString AckRequestedAction;
        public XmlDictionaryString AcksTo;
        public XmlDictionaryString Accept;
        public XmlDictionaryString CreateSequence;
        public XmlDictionaryString CreateSequenceAction;
        public XmlDictionaryString CreateSequenceRefused;
        public XmlDictionaryString CreateSequenceResponse;
        public XmlDictionaryString CreateSequenceResponseAction;
        public XmlDictionaryString Expires;
        public XmlDictionaryString FaultCode;
        public XmlDictionaryString InvalidAcknowledgement;
        public XmlDictionaryString LastMessage;
        public XmlDictionaryString LastMessageAction;
        public XmlDictionaryString LastMessageNumberExceeded;
        public XmlDictionaryString MessageNumberRollover;
        public XmlDictionaryString Nack;
        public XmlDictionaryString NETPrefix;
        public XmlDictionaryString Offer;
        public XmlDictionaryString Prefix;
        public XmlDictionaryString SequenceFault;
        public XmlDictionaryString SequenceTerminated;
        public XmlDictionaryString TerminateSequence;
        public XmlDictionaryString TerminateSequenceAction;
        public XmlDictionaryString UnknownSequence;
        public XmlDictionaryString ConnectionLimitReached;

        public WsrmFeb2005Dictionary(ServiceModelDictionary dictionary)
        {
            this.Identifier = dictionary.CreateString(ServiceModelStringsVersion1.String15, 15);
            this.Namespace = dictionary.CreateString(ServiceModelStringsVersion1.String16, 16);
            this.SequenceAcknowledgement = dictionary.CreateString(ServiceModelStringsVersion1.String23, 23);
            this.AcknowledgementRange = dictionary.CreateString(ServiceModelStringsVersion1.String24, 24);
            this.Upper = dictionary.CreateString(ServiceModelStringsVersion1.String25, 25);
            this.Lower = dictionary.CreateString(ServiceModelStringsVersion1.String26, 26);
            this.BufferRemaining = dictionary.CreateString(ServiceModelStringsVersion1.String27, 27);
            this.NETNamespace = dictionary.CreateString(ServiceModelStringsVersion1.String28, 28);
            this.SequenceAcknowledgementAction = dictionary.CreateString(ServiceModelStringsVersion1.String29, 29);
            this.Sequence = dictionary.CreateString(ServiceModelStringsVersion1.String31, 31);
            this.MessageNumber = dictionary.CreateString(ServiceModelStringsVersion1.String32, 32);
            this.AckRequested = dictionary.CreateString(ServiceModelStringsVersion1.String328, 328);
            this.AckRequestedAction = dictionary.CreateString(ServiceModelStringsVersion1.String329, 329);
            this.AcksTo = dictionary.CreateString(ServiceModelStringsVersion1.String330, 330);
            this.Accept = dictionary.CreateString(ServiceModelStringsVersion1.String331, 331);
            this.CreateSequence = dictionary.CreateString(ServiceModelStringsVersion1.String332, 332);
            this.CreateSequenceAction = dictionary.CreateString(ServiceModelStringsVersion1.String333, 333);
            this.CreateSequenceRefused = dictionary.CreateString(ServiceModelStringsVersion1.String334, 334);
            this.CreateSequenceResponse = dictionary.CreateString(ServiceModelStringsVersion1.String335, 335);
            this.CreateSequenceResponseAction = dictionary.CreateString(ServiceModelStringsVersion1.String336, 336);
            this.Expires = dictionary.CreateString(ServiceModelStringsVersion1.String55, 55);
            this.FaultCode = dictionary.CreateString(ServiceModelStringsVersion1.String337, 337);
            this.InvalidAcknowledgement = dictionary.CreateString(ServiceModelStringsVersion1.String338, 338);
            this.LastMessage = dictionary.CreateString(ServiceModelStringsVersion1.String339, 339);
            this.LastMessageAction = dictionary.CreateString(ServiceModelStringsVersion1.String340, 340);
            this.LastMessageNumberExceeded = dictionary.CreateString(ServiceModelStringsVersion1.String341, 341);
            this.MessageNumberRollover = dictionary.CreateString(ServiceModelStringsVersion1.String342, 342);
            this.Nack = dictionary.CreateString(ServiceModelStringsVersion1.String343, 343);
            this.NETPrefix = dictionary.CreateString(ServiceModelStringsVersion1.String344, 344);
            this.Offer = dictionary.CreateString(ServiceModelStringsVersion1.String345, 345);
            this.Prefix = dictionary.CreateString(ServiceModelStringsVersion1.String346, 346);
            this.SequenceFault = dictionary.CreateString(ServiceModelStringsVersion1.String347, 347);
            this.SequenceTerminated = dictionary.CreateString(ServiceModelStringsVersion1.String348, 348);
            this.TerminateSequence = dictionary.CreateString(ServiceModelStringsVersion1.String349, 349);
            this.TerminateSequenceAction = dictionary.CreateString(ServiceModelStringsVersion1.String350, 350);
            this.UnknownSequence = dictionary.CreateString(ServiceModelStringsVersion1.String351, 351);
            this.ConnectionLimitReached = dictionary.CreateString(ServiceModelStringsVersion1.String480, 480);
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

        public XmlEncryptionDictionary(ServiceModelDictionary dictionary)
        {
            this.Namespace = dictionary.CreateString(ServiceModelStringsVersion1.String37, 37);
            this.DataReference = dictionary.CreateString(ServiceModelStringsVersion1.String46, 46);
            this.EncryptedData = dictionary.CreateString(ServiceModelStringsVersion1.String47, 47);
            this.EncryptionMethod = dictionary.CreateString(ServiceModelStringsVersion1.String48, 48);
            this.CipherData = dictionary.CreateString(ServiceModelStringsVersion1.String49, 49);
            this.CipherValue = dictionary.CreateString(ServiceModelStringsVersion1.String50, 50);
            this.ReferenceList = dictionary.CreateString(ServiceModelStringsVersion1.String57, 57);
            this.Encoding = dictionary.CreateString(ServiceModelStringsVersion1.String308, 308);
            this.MimeType = dictionary.CreateString(ServiceModelStringsVersion1.String309, 309);
            this.Type = dictionary.CreateString(ServiceModelStringsVersion1.String59, 59);
            this.Id = dictionary.CreateString(ServiceModelStringsVersion1.String14, 14);
            this.CarriedKeyName = dictionary.CreateString(ServiceModelStringsVersion1.String310, 310);
            this.Recipient = dictionary.CreateString(ServiceModelStringsVersion1.String311, 311);
            this.EncryptedKey = dictionary.CreateString(ServiceModelStringsVersion1.String312, 312);
            this.URI = dictionary.CreateString(ServiceModelStringsVersion1.String11, 11);
            this.KeyReference = dictionary.CreateString(ServiceModelStringsVersion1.String313, 313);
            this.Prefix = dictionary.CreateString(ServiceModelStringsVersion1.String314, 314);
            this.ElementType = dictionary.CreateString(ServiceModelStringsVersion1.String315, 315);
            this.ContentType = dictionary.CreateString(ServiceModelStringsVersion1.String316, 316);
            this.AlgorithmAttribute = dictionary.CreateString(ServiceModelStringsVersion1.String8, 8);
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

        public XmlSignatureDictionary(ServiceModelDictionary dictionary)
        {
            this.Algorithm = dictionary.CreateString(ServiceModelStringsVersion1.String8, 8);
            this.URI = dictionary.CreateString(ServiceModelStringsVersion1.String11, 11);
            this.Reference = dictionary.CreateString(ServiceModelStringsVersion1.String12, 12);
            this.Transforms = dictionary.CreateString(ServiceModelStringsVersion1.String17, 17);
            this.Transform = dictionary.CreateString(ServiceModelStringsVersion1.String18, 18);
            this.DigestMethod = dictionary.CreateString(ServiceModelStringsVersion1.String19, 19);
            this.DigestValue = dictionary.CreateString(ServiceModelStringsVersion1.String20, 20);
            this.Namespace = dictionary.CreateString(ServiceModelStringsVersion1.String33, 33);
            this.EnvelopedSignature = dictionary.CreateString(ServiceModelStringsVersion1.String34, 34);
            this.KeyInfo = dictionary.CreateString(ServiceModelStringsVersion1.String35, 35);
            this.Signature = dictionary.CreateString(ServiceModelStringsVersion1.String41, 41);
            this.SignedInfo = dictionary.CreateString(ServiceModelStringsVersion1.String42, 42);
            this.CanonicalizationMethod = dictionary.CreateString(ServiceModelStringsVersion1.String43, 43);
            this.SignatureMethod = dictionary.CreateString(ServiceModelStringsVersion1.String44, 44);
            this.SignatureValue = dictionary.CreateString(ServiceModelStringsVersion1.String45, 45);
            this.KeyName = dictionary.CreateString(ServiceModelStringsVersion1.String317, 317);
            this.Type = dictionary.CreateString(ServiceModelStringsVersion1.String59, 59);
            this.MgmtData = dictionary.CreateString(ServiceModelStringsVersion1.String318, 318);
            this.Prefix = dictionary.CreateString(ServiceModelStringsVersion1.String81, 81);
            this.KeyValue = dictionary.CreateString(ServiceModelStringsVersion1.String319, 319);
            this.RsaKeyValue = dictionary.CreateString(ServiceModelStringsVersion1.String320, 320);
            this.Modulus = dictionary.CreateString(ServiceModelStringsVersion1.String321, 321);
            this.Exponent = dictionary.CreateString(ServiceModelStringsVersion1.String322, 322);
            this.X509Data = dictionary.CreateString(ServiceModelStringsVersion1.String323, 323);
            this.X509IssuerSerial = dictionary.CreateString(ServiceModelStringsVersion1.String324, 324);
            this.X509IssuerName = dictionary.CreateString(ServiceModelStringsVersion1.String325, 325);
            this.X509SerialNumber = dictionary.CreateString(ServiceModelStringsVersion1.String326, 326);
            this.X509Certificate = dictionary.CreateString(ServiceModelStringsVersion1.String327, 327);
        }
    }

    internal class Message11Dictionary
    {
        public XmlDictionaryString Namespace;
        public XmlDictionaryString Actor;
        public XmlDictionaryString FaultCode;
        public XmlDictionaryString FaultString;
        public XmlDictionaryString FaultActor;
        public XmlDictionaryString FaultDetail;
        public XmlDictionaryString FaultNamespace;

        public Message11Dictionary(ServiceModelDictionary dictionary)
        {
            this.Namespace = dictionary.CreateString(ServiceModelStringsVersion1.String481, 481);
            this.Actor = dictionary.CreateString(ServiceModelStringsVersion1.String482, 482);
            this.FaultCode = dictionary.CreateString(ServiceModelStringsVersion1.String483, 483);
            this.FaultString = dictionary.CreateString(ServiceModelStringsVersion1.String484, 484);
            this.FaultActor = dictionary.CreateString(ServiceModelStringsVersion1.String485, 485);
            this.FaultDetail = dictionary.CreateString(ServiceModelStringsVersion1.String486, 486);
            this.FaultNamespace = dictionary.CreateString(ServiceModelStringsVersion1.String81, 81);
        }
    }

    internal static class ActivityIdFlowStrings
    {
        // Main dictionary strings
        public const string ActivityId = ServiceModelStringsVersion1.String425;
        public const string ActivityIdNamespace = ServiceModelStringsVersion1.String426;
    }

    internal static class AddressingStrings
    {
        // Main dictionary strings
        public const string Action = ServiceModelStringsVersion1.String5;
        public const string To = ServiceModelStringsVersion1.String6;
        public const string RelatesTo = ServiceModelStringsVersion1.String9;
        public const string MessageId = ServiceModelStringsVersion1.String13;
        public const string Address = ServiceModelStringsVersion1.String21;
        public const string ReplyTo = ServiceModelStringsVersion1.String22;
        public const string Empty = ServiceModelStringsVersion1.String81;
        public const string From = ServiceModelStringsVersion1.String82;
        public const string FaultTo = ServiceModelStringsVersion1.String83;
        public const string EndpointReference = ServiceModelStringsVersion1.String84;
        public const string PortType = ServiceModelStringsVersion1.String85;
        public const string ServiceName = ServiceModelStringsVersion1.String86;
        public const string PortName = ServiceModelStringsVersion1.String87;
        public const string ReferenceProperties = ServiceModelStringsVersion1.String88;
        public const string RelationshipType = ServiceModelStringsVersion1.String89;
        public const string Reply = ServiceModelStringsVersion1.String90;
        public const string Prefix = ServiceModelStringsVersion1.String91;
        public const string IdentityExtensionNamespace = ServiceModelStringsVersion1.String92;
        public const string Identity = ServiceModelStringsVersion1.String93;
        public const string Spn = ServiceModelStringsVersion1.String94;
        public const string Upn = ServiceModelStringsVersion1.String95;
        public const string Rsa = ServiceModelStringsVersion1.String96;
        public const string Dns = ServiceModelStringsVersion1.String97;
        public const string X509v3Certificate = ServiceModelStringsVersion1.String98;
        public const string ReferenceParameters = ServiceModelStringsVersion1.String100;
        public const string IsReferenceParameter = ServiceModelStringsVersion1.String101;
        // String constants
        public const string EndpointUnavailable = "EndpointUnavailable";
        public const string ActionNotSupported = "ActionNotSupported";
        public const string EndpointReferenceType = "EndpointReferenceType";
        public const string Request = "Request";
        public const string DestinationUnreachable = "DestinationUnreachable";
        public const string AnonymousUri = "http://schemas.microsoft.com/2005/12/ServiceModel/Addressing/Anonymous";
        public const string NoneUri = "http://schemas.microsoft.com/2005/12/ServiceModel/Addressing/None";
        public const string IndigoNamespace = "http://schemas.microsoft.com/serviceModel/2004/05/addressing";
        public const string ChannelTerminated = "ChannelTerminated";
    }

    internal static class Addressing10Strings
    {
        // Main dictionary strings
        public const string Namespace = ServiceModelStringsVersion1.String3;
        public const string Anonymous = ServiceModelStringsVersion1.String10;
        public const string FaultAction = ServiceModelStringsVersion1.String99;
        public const string ReplyRelationship = ServiceModelStringsVersion1.String102;
        public const string NoneAddress = ServiceModelStringsVersion1.String103;
        public const string Metadata = ServiceModelStringsVersion1.String104;
        // String constants
        public const string MessageAddressingHeaderRequired = "MessageAddressingHeaderRequired";
        public const string InvalidAddressingHeader = "InvalidAddressingHeader";
        public const string InvalidCardinality = "InvalidCardinality";
        public const string ActionMismatch = "ActionMismatch";
        public const string ProblemHeaderQName = "ProblemHeaderQName";
        public const string FaultDetail = "FaultDetail";
        public const string DefaultFaultAction = "http://www.w3.org/2005/08/addressing/soap/fault";
    }

    internal static class Addressing200408Strings
    {
        // Main dictionary strings
        public const string Namespace = ServiceModelStringsVersion1.String105;
        public const string Anonymous = ServiceModelStringsVersion1.String106;
        public const string FaultAction = ServiceModelStringsVersion1.String107;
        // String constants
        public const string InvalidMessageInformationHeader = "InvalidMessageInformationHeader";
        public const string MessageInformationHeaderRequired = "MessageInformationHeaderRequired";
        public const string DefaultFaultAction = "http://schemas.xmlsoap.org/ws/2004/08/addressing/fault";
    }

    internal static class AddressingNoneStrings
    {
        // Main dictionary strings
        public const string Namespace = ServiceModelStringsVersion1.String439;
    }

    internal static class AtomicTransactionExternalStrings
    {
        // Main dictionary strings
        public const string Prefix = ServiceModelStringsVersion1.String383;
        public const string Prepare = ServiceModelStringsVersion1.String387;
        public const string Prepared = ServiceModelStringsVersion1.String388;
        public const string ReadOnly = ServiceModelStringsVersion1.String389;
        public const string Commit = ServiceModelStringsVersion1.String390;
        public const string Rollback = ServiceModelStringsVersion1.String391;
        public const string Committed = ServiceModelStringsVersion1.String392;
        public const string Aborted = ServiceModelStringsVersion1.String393;
        public const string Replay = ServiceModelStringsVersion1.String394;
        public const string CompletionCoordinatorPortType = ServiceModelStringsVersion1.String404;
        public const string CompletionParticipantPortType = ServiceModelStringsVersion1.String405;
        public const string CoordinatorPortType = ServiceModelStringsVersion1.String406;
        public const string ParticipantPortType = ServiceModelStringsVersion1.String407;
        public const string InconsistentInternalState = ServiceModelStringsVersion1.String408;
    }

    internal static class AtomicTransactionExternal10Strings
    {
        // Main dictionary strings
        public const string Namespace = ServiceModelStringsVersion1.String382;
        public const string CompletionUri = ServiceModelStringsVersion1.String384;
        public const string Durable2PCUri = ServiceModelStringsVersion1.String385;
        public const string Volatile2PCUri = ServiceModelStringsVersion1.String386;
        public const string CommitAction = ServiceModelStringsVersion1.String395;
        public const string RollbackAction = ServiceModelStringsVersion1.String396;
        public const string CommittedAction = ServiceModelStringsVersion1.String397;
        public const string AbortedAction = ServiceModelStringsVersion1.String398;
        public const string PrepareAction = ServiceModelStringsVersion1.String399;
        public const string PreparedAction = ServiceModelStringsVersion1.String400;
        public const string ReadOnlyAction = ServiceModelStringsVersion1.String401;
        public const string ReplayAction = ServiceModelStringsVersion1.String402;
        public const string FaultAction = ServiceModelStringsVersion1.String403;
    }

    internal static class CoordinationExternalStrings
    {
        // Main dictionary strings
        public const string Prefix = ServiceModelStringsVersion1.String357;
        public const string CreateCoordinationContext = ServiceModelStringsVersion1.String358;
        public const string CreateCoordinationContextResponse = ServiceModelStringsVersion1.String359;
        public const string CoordinationContext = ServiceModelStringsVersion1.String360;
        public const string CurrentContext = ServiceModelStringsVersion1.String361;
        public const string CoordinationType = ServiceModelStringsVersion1.String362;
        public const string RegistrationService = ServiceModelStringsVersion1.String363;
        public const string Register = ServiceModelStringsVersion1.String364;
        public const string RegisterResponse = ServiceModelStringsVersion1.String365;
        public const string Protocol = ServiceModelStringsVersion1.String366;
        public const string CoordinatorProtocolService = ServiceModelStringsVersion1.String367;
        public const string ParticipantProtocolService = ServiceModelStringsVersion1.String368;
        public const string Expires = ServiceModelStringsVersion1.String55;
        public const string Identifier = ServiceModelStringsVersion1.String15;
        public const string ActivationCoordinatorPortType = ServiceModelStringsVersion1.String374;
        public const string RegistrationCoordinatorPortType = ServiceModelStringsVersion1.String375;
        public const string InvalidState = ServiceModelStringsVersion1.String376;
        public const string InvalidProtocol = ServiceModelStringsVersion1.String377;
        public const string InvalidParameters = ServiceModelStringsVersion1.String378;
        public const string NoActivity = ServiceModelStringsVersion1.String379;
        public const string ContextRefused = ServiceModelStringsVersion1.String380;
        public const string AlreadyRegistered = ServiceModelStringsVersion1.String381;
    }

    internal static class CoordinationExternal10Strings
    {
        // Main dictionary strings
        public const string Namespace = ServiceModelStringsVersion1.String356;
        public const string CreateCoordinationContextAction = ServiceModelStringsVersion1.String369;
        public const string CreateCoordinationContextResponseAction = ServiceModelStringsVersion1.String370;
        public const string RegisterAction = ServiceModelStringsVersion1.String371;
        public const string RegisterResponseAction = ServiceModelStringsVersion1.String372;
        public const string FaultAction = ServiceModelStringsVersion1.String373;
    }

    internal static class DotNetAddressingStrings
    {
        // Main dictionary strings
        public const string Namespace = ServiceModelStringsVersion1.String108;
        public const string RedirectTo = ServiceModelStringsVersion1.String109;
        public const string Via = ServiceModelStringsVersion1.String110;
    }

    internal static class DotNetAtomicTransactionExternalStrings
    {
        // Main dictionary strings
        public const string Namespace = ServiceModelStringsVersion1.String65;
        public const string Prefix = ServiceModelStringsVersion1.String409;
        public const string Enlistment = ServiceModelStringsVersion1.String410;
        public const string Protocol = ServiceModelStringsVersion1.String411;
        public const string LocalTransactionId = ServiceModelStringsVersion1.String412;
        public const string IsolationLevel = ServiceModelStringsVersion1.String413;
        public const string IsolationFlags = ServiceModelStringsVersion1.String414;
        public const string Description = ServiceModelStringsVersion1.String415;
        public const string Loopback = ServiceModelStringsVersion1.String416;
        public const string RegisterInfo = ServiceModelStringsVersion1.String417;
        public const string ContextId = ServiceModelStringsVersion1.String418;
        public const string TokenId = ServiceModelStringsVersion1.String419;
        public const string AccessDenied = ServiceModelStringsVersion1.String420;
        public const string InvalidPolicy = ServiceModelStringsVersion1.String421;
        public const string CoordinatorRegistrationFailed = ServiceModelStringsVersion1.String422;
        public const string TooManyEnlistments = ServiceModelStringsVersion1.String423;
        public const string Disabled = ServiceModelStringsVersion1.String424;
    }

    internal static class DotNetOneWayStrings
    {
        // Main dictionary strings
        public const string Namespace = ServiceModelStringsVersion1.String437;
        public const string HeaderName = ServiceModelStringsVersion1.String438;
    }

    internal static class DotNetSecurityStrings
    {
        // Main dictionary strings
        public const string Namespace = ServiceModelStringsVersion1.String162;
        public const string Prefix = ServiceModelStringsVersion1.String163;
        // String constants
        public const string KeyRenewalNeededFault = "ExpiredSecurityContextTokenKey";
        public const string SecuritySessionAbortedFault = "SecuritySessionAborted";
        public const string SecurityServerTooBusyFault = "ServerTooBusy";
        public const string SecuritySessionFaultAction = "http://schemas.microsoft.com/ws/2006/05/security/SecureConversationFault";
        public const string SecureConversationCancelNotAllowedFault = "SecureConversationCancellationNotAllowed";
    }
    internal class DotNetSecurityDictionary
    {
        public XmlDictionaryString Namespace;
        public XmlDictionaryString Prefix;

        public DotNetSecurityDictionary(ServiceModelDictionary dictionary)
        {
            this.Namespace = dictionary.CreateString(ServiceModelStringsVersion1.String162, 162);
            this.Prefix = dictionary.CreateString(ServiceModelStringsVersion1.String163, 163);
        }
    }

    internal static class ExclusiveC14NStrings
    {
        // Main dictionary strings
        public const string Namespace = ServiceModelStringsVersion1.String111;
        public const string PrefixList = ServiceModelStringsVersion1.String112;
        public const string InclusiveNamespaces = ServiceModelStringsVersion1.String113;
        public const string Prefix = ServiceModelStringsVersion1.String114;
    }

    internal static class MessageStrings
    {
        // Main dictionary strings
        public const string MustUnderstand = ServiceModelStringsVersion1.String0;
        public const string Envelope = ServiceModelStringsVersion1.String1;
        public const string Header = ServiceModelStringsVersion1.String4;
        public const string Body = ServiceModelStringsVersion1.String7;
        public const string Prefix = ServiceModelStringsVersion1.String66;
        public const string Fault = ServiceModelStringsVersion1.String67;
        public const string MustUnderstandFault = ServiceModelStringsVersion1.String68;
        public const string Namespace = ServiceModelStringsVersion1.String440;
    }

    internal static class Message11Strings
    {
        // Text dictionary strings
        public const string Namespace = ServiceModelStringsVersion1.String481;
        public const string Actor = ServiceModelStringsVersion1.String482;
        public const string FaultCode = ServiceModelStringsVersion1.String483;
        public const string FaultString = ServiceModelStringsVersion1.String484;
        public const string FaultActor = ServiceModelStringsVersion1.String485;
        public const string FaultDetail = ServiceModelStringsVersion1.String486;
        public const string FaultNamespace = ServiceModelStringsVersion1.String81;
    }

    internal static class Message12Strings
    {
        // Main dictionary strings
        public const string Namespace = ServiceModelStringsVersion1.String2;
        public const string Role = ServiceModelStringsVersion1.String69;
        public const string Relay = ServiceModelStringsVersion1.String70;
        public const string FaultCode = ServiceModelStringsVersion1.String71;
        public const string FaultReason = ServiceModelStringsVersion1.String72;
        public const string FaultText = ServiceModelStringsVersion1.String73;
        public const string FaultNode = ServiceModelStringsVersion1.String74;
        public const string FaultRole = ServiceModelStringsVersion1.String75;
        public const string FaultDetail = ServiceModelStringsVersion1.String76;
        public const string FaultValue = ServiceModelStringsVersion1.String77;
        public const string FaultSubcode = ServiceModelStringsVersion1.String78;
        public const string NotUnderstood = ServiceModelStringsVersion1.String79;
        public const string QName = ServiceModelStringsVersion1.String80;
    }

    internal static class OleTxTransactionExternalStrings
    {
        // Main dictionary strings
        public const string Namespace = ServiceModelStringsVersion1.String352;
        public const string Prefix = ServiceModelStringsVersion1.String353;
        public const string OleTxTransaction = ServiceModelStringsVersion1.String354;
        public const string PropagationToken = ServiceModelStringsVersion1.String355;
    }

    internal static class PeerWireStringsStrings
    {
        // Main dictionary strings
        public const string FloodAction = ServiceModelStringsVersion1.String429;
        public const string LinkUtilityAction = ServiceModelStringsVersion1.String430;
        public const string HopCount = ServiceModelStringsVersion1.String431;
        public const string HopCountNamespace = ServiceModelStringsVersion1.String432;
        public const string PeerVia = ServiceModelStringsVersion1.String433;
        public const string Namespace = ServiceModelStringsVersion1.String434;
        public const string Demuxer = ServiceModelStringsVersion1.String435;
        public const string PeerTo = ServiceModelStringsVersion1.String436;
    }

    internal static class PolicyStrings
    {
        // Main dictionary strings
        public const string Namespace = ServiceModelStringsVersion1.String428;
    }

    internal static class SamlStrings
    {
        // Main dictionary strings
        public const string Access = ServiceModelStringsVersion1.String251;
        public const string AccessDecision = ServiceModelStringsVersion1.String252;
        public const string Action = ServiceModelStringsVersion1.String5;
        public const string Advice = ServiceModelStringsVersion1.String253;
        public const string Assertion = ServiceModelStringsVersion1.String179;
        public const string AssertionId = ServiceModelStringsVersion1.String254;
        public const string AssertionIdReference = ServiceModelStringsVersion1.String255;
        public const string Attribute = ServiceModelStringsVersion1.String256;
        public const string AttributeName = ServiceModelStringsVersion1.String257;
        public const string AttributeNamespace = ServiceModelStringsVersion1.String258;
        public const string AttributeStatement = ServiceModelStringsVersion1.String259;
        public const string AttributeValue = ServiceModelStringsVersion1.String260;
        public const string Audience = ServiceModelStringsVersion1.String261;
        public const string AudienceRestrictionCondition = ServiceModelStringsVersion1.String262;
        public const string AuthenticationInstant = ServiceModelStringsVersion1.String263;
        public const string AuthenticationMethod = ServiceModelStringsVersion1.String264;
        public const string AuthenticationStatement = ServiceModelStringsVersion1.String265;
        public const string AuthorityBinding = ServiceModelStringsVersion1.String266;
        public const string AuthorityKind = ServiceModelStringsVersion1.String267;
        public const string AuthorizationDecisionStatement = ServiceModelStringsVersion1.String268;
        public const string Binding = ServiceModelStringsVersion1.String269;
        public const string Condition = ServiceModelStringsVersion1.String270;
        public const string Conditions = ServiceModelStringsVersion1.String271;
        public const string Decision = ServiceModelStringsVersion1.String272;
        public const string DoNotCacheCondition = ServiceModelStringsVersion1.String273;
        public const string Evidence = ServiceModelStringsVersion1.String274;
        public const string IssueInstant = ServiceModelStringsVersion1.String275;
        public const string Issuer = ServiceModelStringsVersion1.String276;
        public const string Location = ServiceModelStringsVersion1.String277;
        public const string MajorVersion = ServiceModelStringsVersion1.String278;
        public const string MinorVersion = ServiceModelStringsVersion1.String279;
        public const string Namespace = ServiceModelStringsVersion1.String180;
        public const string NameIdentifier = ServiceModelStringsVersion1.String280;
        public const string NameIdentifierFormat = ServiceModelStringsVersion1.String281;
        public const string NameIdentifierNameQualifier = ServiceModelStringsVersion1.String282;
        public const string ActionNamespaceAttribute = ServiceModelStringsVersion1.String283;
        public const string NotBefore = ServiceModelStringsVersion1.String284;
        public const string NotOnOrAfter = ServiceModelStringsVersion1.String285;
        public const string PreferredPrefix = ServiceModelStringsVersion1.String286;
        public const string Statement = ServiceModelStringsVersion1.String287;
        public const string Subject = ServiceModelStringsVersion1.String288;
        public const string SubjectConfirmation = ServiceModelStringsVersion1.String289;
        public const string SubjectConfirmationData = ServiceModelStringsVersion1.String290;
        public const string SubjectConfirmationMethod = ServiceModelStringsVersion1.String291;
        public const string HolderOfKey = ServiceModelStringsVersion1.String292;
        public const string SenderVouches = ServiceModelStringsVersion1.String293;
        public const string SubjectLocality = ServiceModelStringsVersion1.String294;
        public const string SubjectLocalityDNSAddress = ServiceModelStringsVersion1.String295;
        public const string SubjectLocalityIPAddress = ServiceModelStringsVersion1.String296;
        public const string SubjectStatement = ServiceModelStringsVersion1.String297;
        public const string UnspecifiedAuthenticationMethod = ServiceModelStringsVersion1.String298;
        public const string NamespaceAttributePrefix = ServiceModelStringsVersion1.String299;
        public const string Resource = ServiceModelStringsVersion1.String300;
        public const string UserName = ServiceModelStringsVersion1.String301;
        public const string UserNameNamespace = ServiceModelStringsVersion1.String302;
        public const string EmailName = ServiceModelStringsVersion1.String303;
        public const string EmailNamespace = ServiceModelStringsVersion1.String304;
    }

    internal static class SecureConversationStrings
    {
    }

    internal static class SecureConversationApr2004Strings
    {
        // Main dictionary strings
        public const string SecurityContextToken = ServiceModelStringsVersion1.String115;
        public const string DerivedKeyToken = ServiceModelStringsVersion1.String39;
        public const string AlgorithmAttribute = ServiceModelStringsVersion1.String8;
        public const string Generation = ServiceModelStringsVersion1.String116;
        public const string Label = ServiceModelStringsVersion1.String117;
        public const string Length = ServiceModelStringsVersion1.String56;
        public const string Nonce = ServiceModelStringsVersion1.String40;
        public const string Offset = ServiceModelStringsVersion1.String118;
        public const string Properties = ServiceModelStringsVersion1.String119;
        public const string Identifier = ServiceModelStringsVersion1.String15;
        public const string Cookie = ServiceModelStringsVersion1.String120;
        public const string Prefix = ServiceModelStringsVersion1.String121;
        public const string Namespace = ServiceModelStringsVersion1.String122;
        public const string DerivedKeyTokenType = ServiceModelStringsVersion1.String123;
        public const string SecurityContextTokenType = ServiceModelStringsVersion1.String124;
        public const string SecurityContextTokenReferenceValueType = ServiceModelStringsVersion1.String124;
        public const string RequestSecurityContextIssuance = ServiceModelStringsVersion1.String125;
        public const string RequestSecurityContextIssuanceResponse = ServiceModelStringsVersion1.String126;
        public const string RenewNeededFaultCode = ServiceModelStringsVersion1.String127;
        public const string BadContextTokenFaultCode = ServiceModelStringsVersion1.String128;
    }

    internal static class SecureConversationFeb2005Strings
    {
        // Main dictionary strings
        public const string Namespace = ServiceModelStringsVersion1.String38;
        public const string DerivedKeyToken = ServiceModelStringsVersion1.String39;
        public const string Nonce = ServiceModelStringsVersion1.String40;
        public const string Length = ServiceModelStringsVersion1.String56;
        public const string SecurityContextToken = ServiceModelStringsVersion1.String115;
        public const string AlgorithmAttribute = ServiceModelStringsVersion1.String8;
        public const string Generation = ServiceModelStringsVersion1.String116;
        public const string Label = ServiceModelStringsVersion1.String117;
        public const string Offset = ServiceModelStringsVersion1.String118;
        public const string Properties = ServiceModelStringsVersion1.String119;
        public const string Identifier = ServiceModelStringsVersion1.String15;
        public const string Cookie = ServiceModelStringsVersion1.String120;
        public const string RenewNeededFaultCode = ServiceModelStringsVersion1.String127;
        public const string BadContextTokenFaultCode = ServiceModelStringsVersion1.String128;
        public const string Prefix = ServiceModelStringsVersion1.String129;
        public const string DerivedKeyTokenType = ServiceModelStringsVersion1.String130;
        public const string SecurityContextTokenType = ServiceModelStringsVersion1.String131;
        public const string SecurityContextTokenReferenceValueType = ServiceModelStringsVersion1.String131;
        public const string RequestSecurityContextIssuance = ServiceModelStringsVersion1.String132;
        public const string RequestSecurityContextIssuanceResponse = ServiceModelStringsVersion1.String133;
        public const string RequestSecurityContextRenew = ServiceModelStringsVersion1.String134;
        public const string RequestSecurityContextRenewResponse = ServiceModelStringsVersion1.String135;
        public const string RequestSecurityContextClose = ServiceModelStringsVersion1.String136;
        public const string RequestSecurityContextCloseResponse = ServiceModelStringsVersion1.String137;
    }

    internal static class SecurityAlgorithmStrings
    {
        // Main dictionary strings
        public const string Aes128Encryption = ServiceModelStringsVersion1.String138;
        public const string Aes128KeyWrap = ServiceModelStringsVersion1.String139;
        public const string Aes192Encryption = ServiceModelStringsVersion1.String140;
        public const string Aes192KeyWrap = ServiceModelStringsVersion1.String141;
        public const string Aes256Encryption = ServiceModelStringsVersion1.String142;
        public const string Aes256KeyWrap = ServiceModelStringsVersion1.String143;
        public const string DesEncryption = ServiceModelStringsVersion1.String144;
        public const string DsaSha1Signature = ServiceModelStringsVersion1.String145;
        public const string ExclusiveC14n = ServiceModelStringsVersion1.String111;
        public const string ExclusiveC14nWithComments = ServiceModelStringsVersion1.String146;
        public const string HmacSha1Signature = ServiceModelStringsVersion1.String147;
        public const string HmacSha256Signature = ServiceModelStringsVersion1.String148;
        public const string Psha1KeyDerivation = ServiceModelStringsVersion1.String149;
        public const string Ripemd160Digest = ServiceModelStringsVersion1.String150;
        public const string RsaOaepKeyWrap = ServiceModelStringsVersion1.String151;
        public const string RsaSha1Signature = ServiceModelStringsVersion1.String152;
        public const string RsaSha256Signature = ServiceModelStringsVersion1.String153;
        public const string RsaV15KeyWrap = ServiceModelStringsVersion1.String154;
        public const string Sha1Digest = ServiceModelStringsVersion1.String155;
        public const string Sha256Digest = ServiceModelStringsVersion1.String156;
        public const string Sha512Digest = ServiceModelStringsVersion1.String157;
        public const string TripleDesEncryption = ServiceModelStringsVersion1.String158;
        public const string TripleDesKeyWrap = ServiceModelStringsVersion1.String159;
        public const string TlsSspiKeyWrap = ServiceModelStringsVersion1.String160;
        public const string WindowsSspiKeyWrap = ServiceModelStringsVersion1.String161;
        // String constants
        public const string StrTransform = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#STR-Transform";
    }

    internal static class SecurityJan2004Strings
    {
        // Main dictionary strings
        public const string SecurityTokenReference = ServiceModelStringsVersion1.String30;
        public const string Namespace = ServiceModelStringsVersion1.String36;
        public const string Security = ServiceModelStringsVersion1.String52;
        public const string ValueType = ServiceModelStringsVersion1.String58;
        public const string TypeAttribute = ServiceModelStringsVersion1.String59;
        public const string Prefix = ServiceModelStringsVersion1.String164;
        public const string NonceElement = ServiceModelStringsVersion1.String40;
        public const string PasswordElement = ServiceModelStringsVersion1.String165;
        public const string PasswordTextName = ServiceModelStringsVersion1.String166;
        public const string UserNameElement = ServiceModelStringsVersion1.String167;
        public const string UserNameTokenElement = ServiceModelStringsVersion1.String168;
        public const string BinarySecurityToken = ServiceModelStringsVersion1.String169;
        public const string EncodingType = ServiceModelStringsVersion1.String170;
        public const string Reference = ServiceModelStringsVersion1.String12;
        public const string URI = ServiceModelStringsVersion1.String11;
        public const string KeyIdentifier = ServiceModelStringsVersion1.String171;
        public const string EncodingTypeValueBase64Binary = ServiceModelStringsVersion1.String172;
        public const string EncodingTypeValueHexBinary = ServiceModelStringsVersion1.String173;
        public const string EncodingTypeValueText = ServiceModelStringsVersion1.String174;
        public const string X509SKIValueType = ServiceModelStringsVersion1.String175;
        public const string KerberosTokenTypeGSS = ServiceModelStringsVersion1.String176;
        public const string KerberosTokenType1510 = ServiceModelStringsVersion1.String177;
        public const string SamlAssertionIdValueType = ServiceModelStringsVersion1.String178;
        public const string SamlAssertion = ServiceModelStringsVersion1.String179;
        public const string SamlUri = ServiceModelStringsVersion1.String180;
        public const string RelAssertionValueType = ServiceModelStringsVersion1.String181;
        public const string FailedAuthenticationFaultCode = ServiceModelStringsVersion1.String182;
        public const string InvalidSecurityTokenFaultCode = ServiceModelStringsVersion1.String183;
        public const string InvalidSecurityFaultCode = ServiceModelStringsVersion1.String184;
        public const string KerberosHashValueType = ServiceModelStringsVersion1.String427;
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
        public const string EncryptedHeader = ServiceModelStringsVersion1.String60;
        public const string Namespace = ServiceModelStringsVersion1.String61;
        public const string Prefix = ServiceModelStringsVersion1.String185;
        public const string SignatureConfirmation = ServiceModelStringsVersion1.String186;
        public const string ValueAttribute = ServiceModelStringsVersion1.String77;
        public const string TokenTypeAttribute = ServiceModelStringsVersion1.String187;
        public const string ThumbprintSha1ValueType = ServiceModelStringsVersion1.String188;
        public const string EncryptedKeyTokenType = ServiceModelStringsVersion1.String189;
        public const string EncryptedKeyHashValueType = ServiceModelStringsVersion1.String190;
        public const string SamlTokenType = ServiceModelStringsVersion1.String191;
        public const string Saml20TokenType = ServiceModelStringsVersion1.String192;
        public const string Saml11AssertionValueType = ServiceModelStringsVersion1.String193;
        // String constants
        public const string SecurityProfileNamespace = "http://docs.oasis-open.org/wss/oasis-wss-soap-message-security-1.1";
        public const string SamlTokenProfileNamespace = "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1";
        public const string X509TokenProfileNamespace = "http://docs.oasis-open.org/wss/2004/xx/oasis-2004xx-wss-x509-token-profile-1.1";
    }

    internal static class SerializationStrings
    {
        // Main dictionary strings
        public const string XmlSchemaInstanceNamespace = ServiceModelStringsVersion1.String441;
        public const string XmlSchemaNamespace = ServiceModelStringsVersion1.String442;
        public const string Nil = ServiceModelStringsVersion1.String443;
        public const string Type = ServiceModelStringsVersion1.String444;
        public const string Char = ServiceModelStringsVersion1.String445;
        public const string Boolean = ServiceModelStringsVersion1.String446;
        public const string Byte = ServiceModelStringsVersion1.String447;
        public const string UnsignedByte = ServiceModelStringsVersion1.String448;
        public const string Short = ServiceModelStringsVersion1.String449;
        public const string UnsignedShort = ServiceModelStringsVersion1.String450;
        public const string Int = ServiceModelStringsVersion1.String451;
        public const string UnsignedInt = ServiceModelStringsVersion1.String452;
        public const string Long = ServiceModelStringsVersion1.String453;
        public const string UnsignedLong = ServiceModelStringsVersion1.String454;
        public const string Float = ServiceModelStringsVersion1.String455;
        public const string Double = ServiceModelStringsVersion1.String456;
        public const string Decimal = ServiceModelStringsVersion1.String457;
        public const string DateTime = ServiceModelStringsVersion1.String458;
        public const string String = ServiceModelStringsVersion1.String459;
        public const string Base64Binary = ServiceModelStringsVersion1.String460;
        public const string AnyType = ServiceModelStringsVersion1.String461;
        public const string Duration = ServiceModelStringsVersion1.String462;
        public const string Guid = ServiceModelStringsVersion1.String463;
        public const string AnyURI = ServiceModelStringsVersion1.String464;
        public const string QName = ServiceModelStringsVersion1.String465;
        public const string Time = ServiceModelStringsVersion1.String466;
        public const string Date = ServiceModelStringsVersion1.String467;
        public const string HexBinary = ServiceModelStringsVersion1.String468;
        public const string GYearMonth = ServiceModelStringsVersion1.String469;
        public const string GYear = ServiceModelStringsVersion1.String470;
        public const string GMonthDay = ServiceModelStringsVersion1.String471;
        public const string GDay = ServiceModelStringsVersion1.String472;
        public const string GMonth = ServiceModelStringsVersion1.String473;
        public const string Integer = ServiceModelStringsVersion1.String474;
        public const string PositiveInteger = ServiceModelStringsVersion1.String475;
        public const string NegativeInteger = ServiceModelStringsVersion1.String476;
        public const string NonPositiveInteger = ServiceModelStringsVersion1.String477;
        public const string NonNegativeInteger = ServiceModelStringsVersion1.String478;
        public const string NormalizedString = ServiceModelStringsVersion1.String479;
    }

    internal static class TrustStrings
    {
    }

    internal static class TrustApr2004Strings
    {
        // Main dictionary strings
        public const string CombinedHashLabel = ServiceModelStringsVersion1.String194;
        public const string RequestSecurityTokenResponse = ServiceModelStringsVersion1.String195;
        public const string TokenType = ServiceModelStringsVersion1.String187;
        public const string KeySize = ServiceModelStringsVersion1.String196;
        public const string RequestedTokenReference = ServiceModelStringsVersion1.String197;
        public const string AppliesTo = ServiceModelStringsVersion1.String198;
        public const string Authenticator = ServiceModelStringsVersion1.String199;
        public const string CombinedHash = ServiceModelStringsVersion1.String200;
        public const string BinaryExchange = ServiceModelStringsVersion1.String201;
        public const string Lifetime = ServiceModelStringsVersion1.String202;
        public const string RequestedSecurityToken = ServiceModelStringsVersion1.String203;
        public const string Entropy = ServiceModelStringsVersion1.String204;
        public const string RequestedProofToken = ServiceModelStringsVersion1.String205;
        public const string ComputedKey = ServiceModelStringsVersion1.String206;
        public const string RequestSecurityToken = ServiceModelStringsVersion1.String207;
        public const string RequestType = ServiceModelStringsVersion1.String208;
        public const string RequestSecurityTokenResponseCollection = ServiceModelStringsVersion1.String62;
        public const string Context = ServiceModelStringsVersion1.String209;
        public const string BinarySecret = ServiceModelStringsVersion1.String210;
        public const string Type = ServiceModelStringsVersion1.String59;
        public const string SpnegoValueTypeUri = ServiceModelStringsVersion1.String211;
        public const string TlsnegoValueTypeUri = ServiceModelStringsVersion1.String212;
        public const string Prefix = ServiceModelStringsVersion1.String213;
        public const string Namespace = ServiceModelStringsVersion1.String214;
        public const string RequestSecurityTokenIssuance = ServiceModelStringsVersion1.String215;
        public const string RequestSecurityTokenIssuanceResponse = ServiceModelStringsVersion1.String216;
        public const string RequestTypeIssue = ServiceModelStringsVersion1.String217;
        public const string Psha1ComputedKeyUri = ServiceModelStringsVersion1.String218;
        public const string SymmetricKeyBinarySecret = ServiceModelStringsVersion1.String219;
        public const string NonceBinarySecret = ServiceModelStringsVersion1.String220;
        public const string KeyType = ServiceModelStringsVersion1.String221;
        public const string SymmetricKeyType = ServiceModelStringsVersion1.String222;
        public const string PublicKeyType = ServiceModelStringsVersion1.String223;
        public const string Claims = ServiceModelStringsVersion1.String224;
        public const string InvalidRequestFaultCode = ServiceModelStringsVersion1.String225;
        public const string FailedAuthenticationFaultCode = ServiceModelStringsVersion1.String182;
        public const string RequestFailedFaultCode = ServiceModelStringsVersion1.String226;
        public const string SignWith = ServiceModelStringsVersion1.String227;
        public const string EncryptWith = ServiceModelStringsVersion1.String228;
        public const string EncryptionAlgorithm = ServiceModelStringsVersion1.String229;
        public const string CanonicalizationAlgorithm = ServiceModelStringsVersion1.String230;
        public const string ComputedKeyAlgorithm = ServiceModelStringsVersion1.String231;
        public const string UseKey = ServiceModelStringsVersion1.String232;
    }

    internal static class TrustFeb2005Strings
    {
        // Main dictionary strings
        public const string RequestSecurityTokenResponseCollection = ServiceModelStringsVersion1.String62;
        public const string Namespace = ServiceModelStringsVersion1.String63;
        public const string BinarySecretClauseType = ServiceModelStringsVersion1.String64;
        public const string CombinedHashLabel = ServiceModelStringsVersion1.String194;
        public const string RequestSecurityTokenResponse = ServiceModelStringsVersion1.String195;
        public const string TokenType = ServiceModelStringsVersion1.String187;
        public const string KeySize = ServiceModelStringsVersion1.String196;
        public const string RequestedTokenReference = ServiceModelStringsVersion1.String197;
        public const string AppliesTo = ServiceModelStringsVersion1.String198;
        public const string Authenticator = ServiceModelStringsVersion1.String199;
        public const string CombinedHash = ServiceModelStringsVersion1.String200;
        public const string BinaryExchange = ServiceModelStringsVersion1.String201;
        public const string Lifetime = ServiceModelStringsVersion1.String202;
        public const string RequestedSecurityToken = ServiceModelStringsVersion1.String203;
        public const string Entropy = ServiceModelStringsVersion1.String204;
        public const string RequestedProofToken = ServiceModelStringsVersion1.String205;
        public const string ComputedKey = ServiceModelStringsVersion1.String206;
        public const string RequestSecurityToken = ServiceModelStringsVersion1.String207;
        public const string RequestType = ServiceModelStringsVersion1.String208;
        public const string Context = ServiceModelStringsVersion1.String209;
        public const string BinarySecret = ServiceModelStringsVersion1.String210;
        public const string Type = ServiceModelStringsVersion1.String59;
        public const string SpnegoValueTypeUri = ServiceModelStringsVersion1.String233;
        public const string TlsnegoValueTypeUri = ServiceModelStringsVersion1.String234;
        public const string Prefix = ServiceModelStringsVersion1.String235;
        public const string RequestSecurityTokenIssuance = ServiceModelStringsVersion1.String236;
        public const string RequestSecurityTokenIssuanceResponse = ServiceModelStringsVersion1.String237;
        public const string RequestTypeIssue = ServiceModelStringsVersion1.String238;
        public const string SymmetricKeyBinarySecret = ServiceModelStringsVersion1.String239;
        public const string Psha1ComputedKeyUri = ServiceModelStringsVersion1.String240;
        public const string NonceBinarySecret = ServiceModelStringsVersion1.String241;
        public const string RenewTarget = ServiceModelStringsVersion1.String242;
        public const string CloseTarget = ServiceModelStringsVersion1.String243;
        public const string RequestedTokenClosed = ServiceModelStringsVersion1.String244;
        public const string RequestedAttachedReference = ServiceModelStringsVersion1.String245;
        public const string RequestedUnattachedReference = ServiceModelStringsVersion1.String246;
        public const string IssuedTokensHeader = ServiceModelStringsVersion1.String247;
        public const string RequestTypeRenew = ServiceModelStringsVersion1.String248;
        public const string RequestTypeClose = ServiceModelStringsVersion1.String249;
        public const string KeyType = ServiceModelStringsVersion1.String221;
        public const string SymmetricKeyType = ServiceModelStringsVersion1.String239;
        public const string PublicKeyType = ServiceModelStringsVersion1.String250;
        public const string Claims = ServiceModelStringsVersion1.String224;
        public const string InvalidRequestFaultCode = ServiceModelStringsVersion1.String225;
        public const string FailedAuthenticationFaultCode = ServiceModelStringsVersion1.String182;
        public const string UseKey = ServiceModelStringsVersion1.String232;
        public const string SignWith = ServiceModelStringsVersion1.String227;
        public const string EncryptWith = ServiceModelStringsVersion1.String228;
        public const string EncryptionAlgorithm = ServiceModelStringsVersion1.String229;
        public const string CanonicalizationAlgorithm = ServiceModelStringsVersion1.String230;
        public const string ComputedKeyAlgorithm = ServiceModelStringsVersion1.String231;
    }

    internal static class UtilityStrings
    {
        // Main dictionary strings
        public const string IdAttribute = ServiceModelStringsVersion1.String14;
        public const string Namespace = ServiceModelStringsVersion1.String51;
        public const string Timestamp = ServiceModelStringsVersion1.String53;
        public const string CreatedElement = ServiceModelStringsVersion1.String54;
        public const string ExpiresElement = ServiceModelStringsVersion1.String55;
        public const string Prefix = ServiceModelStringsVersion1.String305;
        public const string UniqueEndpointHeaderName = ServiceModelStringsVersion1.String306;
        public const string UniqueEndpointHeaderNamespace = ServiceModelStringsVersion1.String307;
    }

    internal static class WsrmFeb2005Strings
    {
        // Main dictionary strings
        public const string Identifier = ServiceModelStringsVersion1.String15;
        public const string Namespace = ServiceModelStringsVersion1.String16;
        public const string SequenceAcknowledgement = ServiceModelStringsVersion1.String23;
        public const string AcknowledgementRange = ServiceModelStringsVersion1.String24;
        public const string Upper = ServiceModelStringsVersion1.String25;
        public const string Lower = ServiceModelStringsVersion1.String26;
        public const string BufferRemaining = ServiceModelStringsVersion1.String27;
        public const string NETNamespace = ServiceModelStringsVersion1.String28;
        public const string SequenceAcknowledgementAction = ServiceModelStringsVersion1.String29;
        public const string Sequence = ServiceModelStringsVersion1.String31;
        public const string MessageNumber = ServiceModelStringsVersion1.String32;
        public const string AckRequested = ServiceModelStringsVersion1.String328;
        public const string AckRequestedAction = ServiceModelStringsVersion1.String329;
        public const string AcksTo = ServiceModelStringsVersion1.String330;
        public const string Accept = ServiceModelStringsVersion1.String331;
        public const string CreateSequence = ServiceModelStringsVersion1.String332;
        public const string CreateSequenceAction = ServiceModelStringsVersion1.String333;
        public const string CreateSequenceRefused = ServiceModelStringsVersion1.String334;
        public const string CreateSequenceResponse = ServiceModelStringsVersion1.String335;
        public const string CreateSequenceResponseAction = ServiceModelStringsVersion1.String336;
        public const string Expires = ServiceModelStringsVersion1.String55;
        public const string FaultCode = ServiceModelStringsVersion1.String337;
        public const string InvalidAcknowledgement = ServiceModelStringsVersion1.String338;
        public const string LastMessage = ServiceModelStringsVersion1.String339;
        public const string LastMessageAction = ServiceModelStringsVersion1.String340;
        public const string LastMessageNumberExceeded = ServiceModelStringsVersion1.String341;
        public const string MessageNumberRollover = ServiceModelStringsVersion1.String342;
        public const string Nack = ServiceModelStringsVersion1.String343;
        public const string NETPrefix = ServiceModelStringsVersion1.String344;
        public const string Offer = ServiceModelStringsVersion1.String345;
        public const string Prefix = ServiceModelStringsVersion1.String346;
        public const string SequenceFault = ServiceModelStringsVersion1.String347;
        public const string SequenceTerminated = ServiceModelStringsVersion1.String348;
        public const string TerminateSequence = ServiceModelStringsVersion1.String349;
        public const string TerminateSequenceAction = ServiceModelStringsVersion1.String350;
        public const string UnknownSequence = ServiceModelStringsVersion1.String351;
        public const string ConnectionLimitReached = ServiceModelStringsVersion1.String480;
    }

    internal static class XmlEncryptionStrings
    {
        // Main dictionary strings
        public const string Namespace = ServiceModelStringsVersion1.String37;
        public const string DataReference = ServiceModelStringsVersion1.String46;
        public const string EncryptedData = ServiceModelStringsVersion1.String47;
        public const string EncryptionMethod = ServiceModelStringsVersion1.String48;
        public const string CipherData = ServiceModelStringsVersion1.String49;
        public const string CipherValue = ServiceModelStringsVersion1.String50;
        public const string ReferenceList = ServiceModelStringsVersion1.String57;
        public const string Encoding = ServiceModelStringsVersion1.String308;
        public const string MimeType = ServiceModelStringsVersion1.String309;
        public const string Type = ServiceModelStringsVersion1.String59;
        public const string Id = ServiceModelStringsVersion1.String14;
        public const string CarriedKeyName = ServiceModelStringsVersion1.String310;
        public const string Recipient = ServiceModelStringsVersion1.String311;
        public const string EncryptedKey = ServiceModelStringsVersion1.String312;
        public const string URI = ServiceModelStringsVersion1.String11;
        public const string KeyReference = ServiceModelStringsVersion1.String313;
        public const string Prefix = ServiceModelStringsVersion1.String314;
        public const string ElementType = ServiceModelStringsVersion1.String315;
        public const string ContentType = ServiceModelStringsVersion1.String316;
        public const string AlgorithmAttribute = ServiceModelStringsVersion1.String8;
    }

    internal static class XmlSignatureStrings
    {
        // Main dictionary strings
        public const string Algorithm = ServiceModelStringsVersion1.String8;
        public const string URI = ServiceModelStringsVersion1.String11;
        public const string Reference = ServiceModelStringsVersion1.String12;
        public const string Transforms = ServiceModelStringsVersion1.String17;
        public const string Transform = ServiceModelStringsVersion1.String18;
        public const string DigestMethod = ServiceModelStringsVersion1.String19;
        public const string DigestValue = ServiceModelStringsVersion1.String20;
        public const string Namespace = ServiceModelStringsVersion1.String33;
        public const string EnvelopedSignature = ServiceModelStringsVersion1.String34;
        public const string KeyInfo = ServiceModelStringsVersion1.String35;
        public const string Signature = ServiceModelStringsVersion1.String41;
        public const string SignedInfo = ServiceModelStringsVersion1.String42;
        public const string CanonicalizationMethod = ServiceModelStringsVersion1.String43;
        public const string SignatureMethod = ServiceModelStringsVersion1.String44;
        public const string SignatureValue = ServiceModelStringsVersion1.String45;
        public const string KeyName = ServiceModelStringsVersion1.String317;
        public const string Type = ServiceModelStringsVersion1.String59;
        public const string MgmtData = ServiceModelStringsVersion1.String318;
        public const string Prefix = ServiceModelStringsVersion1.String81;
        public const string KeyValue = ServiceModelStringsVersion1.String319;
        public const string RsaKeyValue = ServiceModelStringsVersion1.String320;
        public const string Modulus = ServiceModelStringsVersion1.String321;
        public const string Exponent = ServiceModelStringsVersion1.String322;
        public const string X509Data = ServiceModelStringsVersion1.String323;
        public const string X509IssuerSerial = ServiceModelStringsVersion1.String324;
        public const string X509IssuerName = ServiceModelStringsVersion1.String325;
        public const string X509SerialNumber = ServiceModelStringsVersion1.String326;
        public const string X509Certificate = ServiceModelStringsVersion1.String327;
        // String constants
        public const string X509Ski = "X509SKI";
        public const string TransformationParameters = "TransformationParameters";
    }
}
