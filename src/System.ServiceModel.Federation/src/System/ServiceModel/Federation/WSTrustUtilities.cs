// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Tracing;
using System.IdentityModel.Tokens;
using System.ServiceModel.Security;
using System.ServiceModel.Security.Tokens;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.WsSecurity;
using Microsoft.IdentityModel.Protocols.WsTrust;
using WCFSecurityToken = System.IdentityModel.Tokens.SecurityToken;

namespace System.ServiceModel.Federation
{
    /// <summary>
    /// WSTrustUtilities are shared between <see cref="WSTrustChannelSecurityTokenProvider"/> with <see cref="WSFederationHttpBinding"/>
    /// and <see cref="WSTrustChannelFactory"/> and <see cref="WSTrustChannel"/> to send a WsTrust message to obtain a SecurityToken from a STS.
    /// The SecurityToken can be added as an IssuedToken on the outbound WCF message OR be used to call a WebAPI.
    /// </summary>
    internal static class WSTrustUtilities
    {
        /// <summary>
        /// Get a proof token from a WsTrust request/response pair based on section 4.4.3 of the WS-Trust 1.3 spec.
        /// How the proof token is retrieved depends on whether the requester or issuer provide key material:
        /// Requester   |   Issuer                  | Results
        /// -------------------------------------------------
        /// Entropy     | No key material           | No proof token returned, requester entropy used
        /// Entropy     | Entropy                   | Computed key algorithm returned and key computed based on request and response entropy
        /// Entropy     | Rejects requester entropy | Proof token in response used as key
        /// No entropy  | Issues key                | Proof token in response used as key
        /// No entropy  | No key material           | No proof token
        /// </summary>
        /// <param name="request">The WS-Trust request (RST).</param>
        /// <param name="response">The WS-Trust response (RSTR).</param>
        /// <param name="serializationContext">The serialization context.</param>
        /// <param name="algorithmSuite">The algorithm suite.</param>
        /// <returns>The proof token or null if there is no proof token.</returns>
        internal static BinarySecretSecurityToken GetProofToken(WsTrustRequest request, Microsoft.IdentityModel.Protocols.WsTrust.RequestSecurityTokenResponse response, WsSerializationContext serializationContext, SecurityAlgorithmSuite algorithmSuite)
        {
            // According to the WS-Trust 1.3 spec, symmetric is the default key type
            string keyType = response.KeyType ?? request.KeyType ?? serializationContext.TrustKeyTypes.Symmetric;

            // Encrypted keys and encrypted entropy are not supported, currently, as they should
            // only be needed by unsupported message security scenarios.
            if (response.RequestedProofToken?.EncryptedKey != null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new NotSupportedException(SR.GetResourceString(SR.EncryptedKeysForProofTokensNotSupported)), EventLevel.Error);

            // Bearer scenarios have no proof token
            if (string.Equals(keyType, serializationContext.TrustKeyTypes.Bearer, StringComparison.Ordinal))
            {
                if (response.RequestedProofToken != null || response.Entropy != null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new InvalidOperationException(SR.GetResourceString(SR.BearerKeyShouldNotIincludeAProofToken)), EventLevel.Error);

                return null;
            }

            // If the response includes a proof token, use it as the security token's proof.
            // This scenario will occur if the request does not include entropy or if the issuer rejects the requester's entropy.
            if (response.RequestedProofToken?.BinarySecret != null)
            {
                // Confirm that a computed key algorithm isn't also specified
                if (!string.IsNullOrEmpty(response.RequestedProofToken.ComputedKeyAlgorithm) || response.Entropy != null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new InvalidOperationException(SR.GetResourceString(SR.RSTRProofTokenShouldNotHaveAComputedKeyAlgorithmOrIssuerEntropy)), EventLevel.Error);

                return new BinarySecretSecurityToken(response.RequestedProofToken.BinarySecret.Data);
            }
            // If the response includes a computed key algorithm, compute the proof token based on requester and issuer entropy.
            // This scenario will occur if the requester and issuer both provide key material.
            else if (response.RequestedProofToken?.ComputedKeyAlgorithm != null)
            {
                if (!string.Equals(keyType, serializationContext.TrustKeyTypes.Symmetric, StringComparison.Ordinal))
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new InvalidOperationException(SR.GetResourceString(SR.ComputedKeyProofTokensAreOnlySupportedWithSymmetricKeyTypes)), EventLevel.Error);

                if (string.Equals(response.RequestedProofToken.ComputedKeyAlgorithm, serializationContext.TrustKeyTypes.PSHA1, StringComparison.Ordinal))
                {
                    // Confirm that no encrypted entropy was provided as that is currently not supported.
                    // If we wish to support it in the future, most of the work will be in the WSTrust serializer;
                    // this code would just have to use protected key's .Secret property to get the key material.
                    if (response.Entropy?.ProtectedKey != null || request.Entropy?.ProtectedKey != null)
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelper( new NotSupportedException(SR.GetResourceString(SR.ProtectedKeyEntropyIsNotSupported)), EventLevel.Error);

                    // Get issuer and requester entropy
                    byte[] issuerEntropy = response.Entropy?.BinarySecret?.Data;
                    if (issuerEntropy == null)
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new InvalidOperationException(SR.GetResourceString(SR.ComputedKeyProofTokensRequireIssuerToSupplyKeyMaterialViaEntropy)), EventLevel.Error);

                    byte[] requestorEntropy = request.Entropy?.BinarySecret?.Data;
                    if (requestorEntropy == null)
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new InvalidOperationException(SR.GetResourceString(SR.ComputedKeyProofTokensRequireRequesterToSupplyKeyMaterialViaEntropy)), EventLevel.Error);

                    // Get key size
                    int keySizeInBits = response.KeySizeInBits ?? 0; // RSTR key size has precedence
                    if (keySizeInBits == 0)
                        keySizeInBits = request.KeySizeInBits ?? 0; // Followed by RST

                    if (keySizeInBits == 0)
                        keySizeInBits = algorithmSuite?.DefaultSymmetricKeyLength ?? 0; // Symmetric keys should default to a length corresponding to the algorithm in use

                    if (keySizeInBits == 0)
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new InvalidOperationException(SR.GetResourceString(SR.NoKeySizeProvided)), EventLevel.Error);

                    return new BinarySecretSecurityToken(Psha1KeyGenerator.ComputeCombinedKey(issuerEntropy, requestorEntropy, keySizeInBits));
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new NotSupportedException(SR.GetResourceString(SR.OnlyPSHA1ComputedKeysAreSupported)), EventLevel.Error);
                }
            }
            // If the response does not have a proof token or computed key value, but the request proposed entropy,
            // then the requester's entropy is used as the proof token.
            else if (request.Entropy != null)
            {
                if (request.Entropy.ProtectedKey != null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new NotSupportedException(SR.GetResourceString(SR.ProtectedKeyEntropyIsNotSupported)), EventLevel.Error);

                if (request.Entropy.BinarySecret != null)
                    return new BinarySecretSecurityToken(request.Entropy.BinarySecret.Data);
            }

            // If we get here, then no key material has been supplied (by either issuer or requester), so there is no proof token.
            return null;
        }

        internal static GenericXmlSecurityKeyIdentifierClause GetSecurityKeyIdentifierForTokenReference(SecurityTokenReference securityTokenReference)
        {
            if (securityTokenReference == null)
                return null;

            return new GenericXmlSecurityKeyIdentifierClause(WsSecuritySerializer.CreateXmlElement(securityTokenReference));
        }

        internal static WCFSecurityToken CreateGenericXmlSecurityToken(WsTrustRequest request, WsTrustResponse trustResponse, WsSerializationContext serializationContext, SecurityAlgorithmSuite algorithmSuite)
        {
            // Create GenericXmlSecurityToken
            // Assumes that token is first and Saml2SecurityToken.
            Microsoft.IdentityModel.Protocols.WsTrust.RequestSecurityTokenResponse response = trustResponse.RequestSecurityTokenResponseCollection[0];

            // Get attached and unattached references
            GenericXmlSecurityKeyIdentifierClause internalSecurityKeyIdentifierClause = null;
            if (response.AttachedReference != null)
                internalSecurityKeyIdentifierClause = GetSecurityKeyIdentifierForTokenReference(response.AttachedReference);

            GenericXmlSecurityKeyIdentifierClause externalSecurityKeyIdentifierClause = null;
            if (response.UnattachedReference != null)
                externalSecurityKeyIdentifierClause = GetSecurityKeyIdentifierForTokenReference(response.UnattachedReference);

            // Get proof token
            WCFSecurityToken proofToken = GetProofToken(request, response, serializationContext, algorithmSuite);

            // Get lifetime
            DateTime created = response.Lifetime?.Created ?? DateTime.UtcNow;
            DateTime expires = response.Lifetime?.Expires ?? created.AddDays(1);

            return new GenericXmlSecurityToken(response.RequestedSecurityToken.TokenElement,
                                               proofToken,
                                               created,
                                               expires,
                                               internalSecurityKeyIdentifierClause,
                                               externalSecurityKeyIdentifierClause,
                                               null);
        }
    }
}
