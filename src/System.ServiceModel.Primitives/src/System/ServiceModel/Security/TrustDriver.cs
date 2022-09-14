// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;

using System.Xml;
using System.Collections.ObjectModel;
using System.IdentityModel.Policy;

namespace System.ServiceModel.Security
{
    internal abstract class TrustDriver
    {
        // issued tokens control        
        public virtual bool IsIssuedTokensSupported
        {
            get
            {
                return false;
            }
        }

        // issued tokens feature        
        public virtual string IssuedTokensHeaderName
        {
            get
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.TrustDriverVersionDoesNotSupportIssuedTokens));
            }
        }

        // issued tokens feature        
        public virtual string IssuedTokensHeaderNamespace
        {
            get
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.TrustDriverVersionDoesNotSupportIssuedTokens));
            }
        }

        // session control
        public virtual bool IsSessionSupported
        {
            get
            {
                return false;
            }
        }

        public abstract XmlDictionaryString RequestSecurityTokenAction { get; }

        public abstract XmlDictionaryString RequestSecurityTokenResponseAction { get; }

        public abstract XmlDictionaryString RequestSecurityTokenResponseFinalAction { get; }

        // session feature
        public virtual string RequestTypeClose
        {
            get
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.TrustDriverVersionDoesNotSupportSession));
            }
        }

        public abstract string RequestTypeIssue { get; }

        // session feature
        public virtual string RequestTypeRenew
        {
            get
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.TrustDriverVersionDoesNotSupportSession));
            }
        }

        public abstract string ComputedKeyAlgorithm { get; }

        public abstract SecurityStandardsManager StandardsManager { get; }

        public abstract XmlDictionaryString Namespace { get; }

        // RST specific method
        public abstract RequestSecurityToken CreateRequestSecurityToken(XmlReader reader);

        // RSTR specific method
        public abstract RequestSecurityTokenResponse CreateRequestSecurityTokenResponse(XmlReader reader);

        // RSTRC specific method
        public abstract RequestSecurityTokenResponseCollection CreateRequestSecurityTokenResponseCollection(XmlReader xmlReader);

        public abstract bool IsAtRequestSecurityTokenResponse(XmlReader reader);

        public abstract bool IsAtRequestSecurityTokenResponseCollection(XmlReader reader);

        public abstract bool IsRequestedSecurityTokenElement(string name, string nameSpace);

        public abstract bool IsRequestedProofTokenElement(string name, string nameSpace);

        public abstract T GetAppliesTo<T>(RequestSecurityToken rst, XmlObjectSerializer serializer);

        public abstract T GetAppliesTo<T>(RequestSecurityTokenResponse rstr, XmlObjectSerializer serializer);

        public abstract void GetAppliesToQName(RequestSecurityToken rst, out string localName, out string namespaceUri);

        public abstract void GetAppliesToQName(RequestSecurityTokenResponse rstr, out string localName, out string namespaceUri);

        public abstract bool IsAppliesTo(string localName, string namespaceUri);

        // RSTR specific method
        public abstract byte[] GetAuthenticator(RequestSecurityTokenResponse rstr);

        // RST specific method
        public abstract BinaryNegotiation GetBinaryNegotiation(RequestSecurityToken rst);

        // RSTR specific method
        public abstract BinaryNegotiation GetBinaryNegotiation(RequestSecurityTokenResponse rstr);

        // RST specific method
        public abstract SecurityToken GetEntropy(RequestSecurityToken rst, SecurityTokenResolver resolver);

        // RSTR specific method
        public abstract SecurityToken GetEntropy(RequestSecurityTokenResponse rstr, SecurityTokenResolver resolver);

        // RSTR specific method
        public abstract GenericXmlSecurityToken GetIssuedToken(RequestSecurityTokenResponse rstr, SecurityTokenResolver resolver, IList<SecurityTokenAuthenticator> allowedAuthenticators, SecurityKeyEntropyMode keyEntropyMode, byte[] requestorEntropy,
            string expectedTokenType, ReadOnlyCollection<IAuthorizationPolicy> authorizationPolicies, int defaultKeySize, bool isBearerKeyType);

        // RST specific method
        public abstract void WriteRequestSecurityToken(RequestSecurityToken rst, XmlWriter w);

        // RSTR specific method
        public abstract void WriteRequestSecurityTokenResponse(RequestSecurityTokenResponse rstr, XmlWriter w);

        // RSTR Collection method
        public abstract void WriteRequestSecurityTokenResponseCollection(RequestSecurityTokenResponseCollection rstrCollection, XmlWriter writer);
    }
}
