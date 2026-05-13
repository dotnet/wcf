// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Text;
using Infrastructure.Common;
using Xunit;

// Regression tests for dotnet/wcf#5883.
//
// .NET Framework's WSSecurityOneDotZeroSendSecurityHeader.CompletePrimarySignatureCore
// gates the To-header reference block on `signatureKey is AsymmetricSecurityKey` -- only
// asymmetric primary signatures (e.g. an X.509 client cert) reference the <a:To> header.
// Symmetric primary signatures driven by an SCT-derived KeyedHashAlgorithm (e.g. an
// outgoing message on an established secure session, including the RST/SCT/Renew
// exchange) skip the reference entirely.
//
// In the .NET (Core) port the equivalent of `signatureKey is AsymmetricSecurityKey` is
// `_signedXml.SigningKey != null`: asymmetric setup leaves _signingKey null and stores
// the AsymmetricAlgorithm on _signedXml.SigningKey, while symmetric setup populates
// _signingKey (a KeyedHashAlgorithm) and leaves _signedXml.SigningKey null. The pre-fix
// code OR-ed both fields together, so symmetric primary signatures wrongly attached a
// To-header reference and .NET Framework 4.8 servers rejected the resulting renew
// message with "The security protocol cannot verify the incoming message."
//
// These tests pin the gate by driving CompletePrimarySignatureCore via reflection on
// both an asymmetric (must reference To) and a symmetric (must not reference To) setup.
public static class WSSecurityOneDotZeroSendSecurityHeaderTests
{
    private const string ToHeaderId = "_to_1";
    private const string TimestampId = "_ts_0";
    private const string ToCanonicalXml =
        "<a:To xmlns:a=\"http://www.w3.org/2005/08/addressing\" " +
        "xmlns:u=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\" " +
        "u:Id=\"" + ToHeaderId + "\">net.tcp://localhost/test</a:To>";

    [WcfFact]
    public static void CompletePrimarySignatureCore_AsymmetricKey_SignsToHeader()
    {
        // Mirrors the initial SCT Issue request, where the primary signature is keyed by
        // the X.509 client certificate (AsymmetricSecurityKey). The SignedInfo must
        // reference both the timestamp and the To header.
        SignatureFixture fx = SignatureFixture.Create(SigningKeyKind.Asymmetric);

        object result = fx.CompletePrimarySignatureCore(isPrimarySignature: true);

        Reference[] refs = SignatureFixture.GetReferences(result);

        Assert.Equal(2, refs.Length);
        Assert.Contains(refs, r => r.Uri == "#" + TimestampId);
        Assert.Contains(refs, r => r.Uri == "#" + ToHeaderId);
    }

    [WcfFact]
    public static void CompletePrimarySignatureCore_SymmetricKey_DoesNotSignToHeader()
    {
        // Mirrors the RST/SCT/Renew request from the bug report: the primary signature on
        // the renew message is keyed by the SCT-derived symmetric key. The SignedInfo
        // must reference only the timestamp -- no To header -- matching what .NET
        // Framework 4.8 servers accept. Pre-fix this branch attached a To reference and
        // the FX server rejected the message; subsequent renewals could not complete.
        SignatureFixture fx = SignatureFixture.Create(SigningKeyKind.Symmetric);

        object result = fx.CompletePrimarySignatureCore(isPrimarySignature: true);

        Reference[] refs = SignatureFixture.GetReferences(result);

        Assert.Single(refs);
        Assert.Equal("#" + TimestampId, refs[0].Uri);
    }

    private enum SigningKeyKind
    {
        Asymmetric,
        Symmetric,
    }

    // Reflection harness that builds a WSSecurityOneDotZeroSendSecurityHeader, pre-loads
    // the internal state CompletePrimarySignatureCore expects, and invokes the method.
    private sealed class SignatureFixture
    {
        private static readonly Assembly s_primitivesAssembly =
            typeof(MessageSecurityException).Assembly;
        private static readonly Type s_headerType =
            s_primitivesAssembly.GetType(
                "System.ServiceModel.Security.WSSecurityOneDotZeroSendSecurityHeader",
                throwOnError: true);
        private static readonly Type s_standardsManagerType =
            s_primitivesAssembly.GetType(
                "System.ServiceModel.Security.SecurityStandardsManager",
                throwOnError: true);
        private static readonly Type s_securityTimestampType =
            s_primitivesAssembly.GetType(
                "System.ServiceModel.Security.SecurityTimestamp",
                throwOnError: true);

        private readonly object _instance;

        private SignatureFixture(object instance) => _instance = instance;

        public static SignatureFixture Create(SigningKeyKind keyKind)
        {
            object standardsManager = s_standardsManagerType
                .GetProperty("DefaultInstance", BindingFlags.Public | BindingFlags.Static)
                .GetValue(null);

            // Version.Addressing must not be AddressingVersion.None for the gated branch
            // to be considered at all, so we pin SOAP 1.2 + WS-Addressing 1.0.
            Message message = Message.CreateMessage(
                MessageVersion.Soap12WSAddressing10, "urn:test/action");
            message.Headers.To = new Uri("net.tcp://localhost/test");

            object instance = Activator.CreateInstance(
                s_headerType,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                binder: null,
                args: new object[]
                {
                    message, "", false, false,
                    standardsManager,
                    SecurityAlgorithmSuite.Default,
                    MessageDirection.Output,
                },
                culture: null);

            // RequireMessageProtection = false (transport-mode shape; matches the
            // TransportWithMessageCredential scenario from the bug report).
            s_headerType.BaseType.BaseType
                .GetProperty("RequireMessageProtection")
                .SetValue(instance, false);

            // ShouldSignToHeader has a private setter -- force it on so the gate is the
            // only thing keeping the To reference out for the symmetric case.
            s_headerType.BaseType
                .GetProperty("ShouldSignToHeader", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(instance, true);

            // GetSignatureStream populates _toHeaderStream during canonicalization and
            // leaves it at Position 0 (see line 216 of the production file). Reproduce
            // the post-canonicalization shape so AddReference sees a real stream.
            MemoryStream toStream = new MemoryStream();
            byte[] toBytes = Encoding.UTF8.GetBytes(ToCanonicalXml);
            toStream.Write(toBytes, 0, toBytes.Length);
            toStream.Position = 0;

            SetPrivateField(instance, "_toHeaderStream", toStream);
            SetPrivateField(instance, "_toHeaderId", ToHeaderId);

            // Configure the signing key according to the scenario. The production code
            // gates on `_signedXml.SigningKey != null`, matching NetFx's
            // `signatureKey is AsymmetricSecurityKey`.
            SignedXml signedXml = new SignedXml();
            signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigExcC14NTransformUrl;

            if (keyKind == SigningKeyKind.Asymmetric)
            {
                signedXml.SignedInfo.SignatureMethod = SignedXml.XmlDsigRSASHA1Url;
                signedXml.SigningKey = RSA.Create(2048);
            }
            else
            {
                signedXml.SignedInfo.SignatureMethod = SignedXml.XmlDsigHMACSHA1Url;
                SetPrivateField(instance, "_signingKey", new HMACSHA1(new byte[32]));
            }

            SetPrivateField(instance, "_signedXml", signedXml);

            // Add a Timestamp -- one reference is required, otherwise
            // CompletePrimarySignatureCore throws NoPartsOfMessageMatchedPartsToSign for
            // the symmetric path (which does not add the To-header reference).
            object timestamp = Activator.CreateInstance(
                s_securityTimestampType,
                new object[]
                {
                    DateTime.UtcNow,
                    DateTime.UtcNow.AddMinutes(5),
                    TimestampId,
                });
            s_headerType.BaseType
                .GetMethod("AddTimestamp", new[] { s_securityTimestampType })
                .Invoke(instance, new[] { timestamp });

            return new SignatureFixture(instance);
        }

        public object CompletePrimarySignatureCore(bool isPrimarySignature)
        {
            MethodInfo method = s_headerType.GetMethod(
                "CompletePrimarySignatureCore",
                BindingFlags.Instance | BindingFlags.NonPublic);

            try
            {
                return method.Invoke(_instance, new object[] { null, null, null, null, isPrimarySignature });
            }
            catch (TargetInvocationException tie) when (tie.InnerException != null)
            {
                // Surface the real exception so xunit reports it directly.
                throw tie.InnerException;
            }
        }

        public static Reference[] GetReferences(object signatureValueElement)
        {
            // CompletePrimarySignatureCore returns a WSSecurityOneDotZeroSendSecurityHeader.SignatureValue,
            // a nested internal type that wraps the produced Signature.
            Type sigValType = s_headerType.GetNestedType(
                "SignatureValue", BindingFlags.NonPublic);
            FieldInfo sigField = sigValType.GetField(
                "_signature", BindingFlags.Instance | BindingFlags.NonPublic);
            Signature signature = (Signature)sigField.GetValue(signatureValueElement);
            return signature.SignedInfo.References.Cast<Reference>().ToArray();
        }

        private static void SetPrivateField(object instance, string name, object value)
        {
            FieldInfo field = s_headerType.GetField(
                name, BindingFlags.Instance | BindingFlags.NonPublic)
                ?? throw new InvalidOperationException(
                    $"Field {name} not found on {s_headerType.FullName}.");
            field.SetValue(instance, value);
        }
    }
}
