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
using SecurityToken = System.IdentityModel.Tokens.SecurityToken;

namespace System.ServiceModel.Federation
{
    /// <summary>
    /// <see cref="WSTrustChannelSecurityTokenProvider"/> has been designed to work with the <see cref="WSFederationHttpBinding"/> to send a WsTrust message to obtain a SecurityToken from an STS. The SecurityToken is
    /// added as an IssuedToken on the outbound WCF message.
    /// </summary>
    public static class WSTrustUtilities
    {
        /// <summary>
        /// Gets the WsTrustVersion for the current MessageSecurityVersion.
        /// </summary>        /// <summary>
        /// Get a proof token from a WsTrust request/response pair based on section 4.4.3 of the WS-Trust 1.3 spec.
        /// How the proof token is retrieved depends on whether the requestor or issuer provide key material:
        /// Requestor   |   Issuer                  | Results
        /// -------------------------------------------------
        /// Entropy     | No key material           | No proof token returned, requestor entropy used
        /// Entropy     | Entropy                   | Computed key algorithm returned and key computed based on request and response entropy
        /// Entropy     | Rejects requestor entropy | Proof token in response used as key
        /// No entropy  | Issues key                | Proof token in response used as key
        /// No entropy  | No key material           | No proof token
        /// </summary>
        /// <param name="request">The WS-Trust request (RST).</param>
        /// <param name="response">The WS-Trust response (RSTR).</param>
        /// <returns>The proof token or null if there is no proof token.</returns>
        internal static BinarySecretSecurityToken GetProofToken(WsTrustRequest request, RequestSecurityTokenResponse response, WsSerializationContext serializationContext, SecurityAlgorithmSuite algorithmSuite)
        {
            // According to the WS-Trust 1.3 spec, symmetric is the default key type
            string keyType = response.KeyType ?? request.KeyType ?? serializationContext.TrustKeyTypes.Symmetric;

            // Encrypted keys and encrypted entropy are not supported, currently, as they should
            // only be needed by unsupported message security scenarios.
            if (response.RequestedProofToken?.EncryptedKey != null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new NotSupportedException("Encrypted keys for proof tokens are not supported."), EventLevel.Error);

            // Bearer scenarios have no proof token
            if (string.Equals(keyType, serializationContext.TrustKeyTypes.Bearer, StringComparison.Ordinal))
            {
                if (response.RequestedProofToken != null || response.Entropy != null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new InvalidOperationException("Bearer key scenarios should not include a proof token or issuer entropy in the response."), EventLevel.Error);

                return null;
            }

            // If the response includes a proof token, use it as the security token's proof.
            // This scenario will occur if the request does not include entropy or if the issuer rejects the requestor's entropy.
            if (response.RequestedProofToken?.BinarySecret != null)
            {
                // Confirm that a computed key algorithm isn't also specified
                if (!string.IsNullOrEmpty(response.RequestedProofToken.ComputedKeyAlgorithm) || response.Entropy != null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new InvalidOperationException("An RSTR containing a proof token should not also have a computed key algorithm or issuer entropy."), EventLevel.Error);

                return new BinarySecretSecurityToken(response.RequestedProofToken.BinarySecret.Data);
            }
            // If the response includes a computed key algorithm, compute the proof token based on requestor and issuer entropy.
            // This scenario will occur if the requestor and issuer both provide key material.
            else if (response.RequestedProofToken?.ComputedKeyAlgorithm != null)
            {
                if (!string.Equals(keyType, serializationContext.TrustKeyTypes.Symmetric, StringComparison.Ordinal))
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new InvalidOperationException("Computed key proof tokens are only supported with symmetric key types."), EventLevel.Error);

                if (string.Equals(response.RequestedProofToken.ComputedKeyAlgorithm, serializationContext.TrustKeyTypes.PSHA1, StringComparison.Ordinal))
                {
                    // Confirm that no encrypted entropy was provided as that is currently not supported.
                    // If we wish to support it in the future, most of the work will be in the WSTrust serializer;
                    // this code would just have to use protected key's .Secret property to get the key material.
                    if (response.Entropy?.ProtectedKey != null || request.Entropy?.ProtectedKey != null)
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelper( new NotSupportedException("Protected key entropy is not supported."), EventLevel.Error);

                    // Get issuer and requestor entropy
                    byte[] issuerEntropy = response.Entropy?.BinarySecret?.Data;
                    if (issuerEntropy == null)
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new InvalidOperationException("Computed key proof tokens require issuer to supply key material via entropy."), EventLevel.Error);

                    byte[] requestorEntropy = request.Entropy?.BinarySecret?.Data;
                    if (requestorEntropy == null)
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new InvalidOperationException("Computed key proof tokens require requestor to supply key material via entropy."), EventLevel.Error);

                    // Get key size
                    int keySizeInBits = response.KeySizeInBits ?? 0; // RSTR key size has precedence
                    if (keySizeInBits == 0)
                        keySizeInBits = request.KeySizeInBits ?? 0; // Followed by RST

                    if (keySizeInBits == 0)
                        keySizeInBits = algorithmSuite?.DefaultSymmetricKeyLength ?? 0; // Symmetric keys should default to a length corresponding to the algorithm in use

                    if (keySizeInBits == 0)
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new InvalidOperationException("No key size provided."), EventLevel.Error);

                    return new BinarySecretSecurityToken(Psha1KeyGenerator.ComputeCombinedKey(issuerEntropy, requestorEntropy, keySizeInBits));
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new NotSupportedException("Only PSHA1 computed keys are supported."), EventLevel.Error);
                }
            }
            // If the response does not have a proof token or computed key value, but the request proposed entropy,
            // then the requestor's entropy is used as the proof token.
            else if (request.Entropy != null)
            {
                if (request.Entropy.ProtectedKey != null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new NotSupportedException("Protected key entropy is not supported."), EventLevel.Error);

                if (request.Entropy.BinarySecret != null)
                    return new BinarySecretSecurityToken(request.Entropy.BinarySecret.Data);
            }

            // If we get here, then no key material has been supplied (by either issuer or requestor), so there is no proof token.
            return null;
        }

        internal static GenericXmlSecurityKeyIdentifierClause GetSecurityKeyIdentifierForTokenReference(SecurityTokenReference securityTokenReference)
        {
            if (securityTokenReference == null)
                return null;

            return new GenericXmlSecurityKeyIdentifierClause(WsSecuritySerializer.CreateXmlElement(securityTokenReference));
        }

        internal static SecurityToken CreateGenericXmlSecurityToken(WsTrustRequest request, WsTrustResponse trustResponse, WsSerializationContext serializationContext, SecurityAlgorithmSuite algorithmSuite)
        {
            // Create GenericXmlSecurityToken
            // Assumes that token is first and Saml2SecurityToken.
            RequestSecurityTokenResponse response = trustResponse.RequestSecurityTokenResponseCollection[0];

            // Get attached and unattached references
            GenericXmlSecurityKeyIdentifierClause internalSecurityKeyIdentifierClause = null;
            if (response.AttachedReference != null)
                internalSecurityKeyIdentifierClause = GetSecurityKeyIdentifierForTokenReference(response.AttachedReference);

            GenericXmlSecurityKeyIdentifierClause externalSecurityKeyIdentifierClause = null;
            if (response.UnattachedReference != null)
                externalSecurityKeyIdentifierClause = GetSecurityKeyIdentifierForTokenReference(response.UnattachedReference);

            // Get proof token
            IdentityModel.Tokens.SecurityToken proofToken = GetProofToken(request, response, serializationContext, algorithmSuite);

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
