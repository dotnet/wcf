// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Globalization;
using System.Runtime;

namespace System.ServiceModel.Channels
{
    public sealed class MessageVersion
    {
        private static MessageVersion s_soap12Addressing200408;
        private const string MessageVersionToStringFormat = "{0} {1}";

        static MessageVersion()
        {
            None = new MessageVersion(EnvelopeVersion.None, AddressingVersion.None);
            Soap11 = new MessageVersion(EnvelopeVersion.Soap11, AddressingVersion.None);
            Soap12 = new MessageVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
            Soap11WSAddressing10 = new MessageVersion(EnvelopeVersion.Soap11, AddressingVersion.WSAddressing10);
            Soap12WSAddressing10 = new MessageVersion(EnvelopeVersion.Soap12, AddressingVersion.WSAddressing10);
            Soap11WSAddressingAugust2004 = new MessageVersion(EnvelopeVersion.Soap11, AddressingVersion.WSAddressingAugust2004);
            s_soap12Addressing200408 = new MessageVersion(EnvelopeVersion.Soap12, AddressingVersion.WSAddressingAugust2004);
        }

        private MessageVersion(EnvelopeVersion envelopeVersion, AddressingVersion addressingVersion)
        {
            Envelope = envelopeVersion;
            Addressing = addressingVersion;
        }

        public static MessageVersion CreateVersion(EnvelopeVersion envelopeVersion)
        {
            return CreateVersion(envelopeVersion, AddressingVersion.WSAddressing10);
        }

        public static MessageVersion CreateVersion(EnvelopeVersion envelopeVersion, AddressingVersion addressingVersion)
        {
            if (envelopeVersion == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(envelopeVersion));
            }

            if (addressingVersion == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(addressingVersion));
            }

            if (envelopeVersion == EnvelopeVersion.Soap12)
            {
                if (addressingVersion == AddressingVersion.WSAddressing10)
                {
                    return Soap12WSAddressing10;
                }
                else if (addressingVersion == AddressingVersion.WSAddressingAugust2004)
                {
                    return s_soap12Addressing200408;
                }
                else if (addressingVersion == AddressingVersion.None)
                {
                    return Soap12;
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("addressingVersion",
                        SRP.Format(SRP.AddressingVersionNotSupported, addressingVersion));
                }
            }
            else if (envelopeVersion == EnvelopeVersion.Soap11)
            {
                if (addressingVersion == AddressingVersion.WSAddressing10)
                {
                    return Soap11WSAddressing10;
                }
                else if (addressingVersion == AddressingVersion.WSAddressingAugust2004)
                {
                    return Soap11WSAddressingAugust2004;
                }
                else if (addressingVersion == AddressingVersion.None)
                {
                    return Soap11;
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("addressingVersion",
                        SRP.Format(SRP.AddressingVersionNotSupported, addressingVersion));
                }
            }
            else if (envelopeVersion == EnvelopeVersion.None)
            {
                if (addressingVersion == AddressingVersion.None)
                {
                    return None;
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("addressingVersion",
                        SRP.Format(SRP.AddressingVersionNotSupported, addressingVersion));
                }
            }
            else
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("envelopeVersion",
                    SRP.Format(SRP.EnvelopeVersionNotSupported, envelopeVersion));
            }
        }

        public AddressingVersion Addressing { get; }

        public static MessageVersion Default
        {
            get { return Soap12WSAddressing10; }
        }

        public EnvelopeVersion Envelope { get; }

        public override bool Equals(object obj)
        {
            return this == obj;
        }

        public override int GetHashCode()
        {
            int code = 0;
            if (Envelope == EnvelopeVersion.Soap11)
            {
                code += 1;
            }

            if (Addressing == AddressingVersion.WSAddressingAugust2004)
            {
                code += 2;
            }

            return code;
        }

        public static MessageVersion None { get; private set; }

        public static MessageVersion Soap12WSAddressing10 { get; private set; }

        public static MessageVersion Soap11WSAddressing10 { get; private set; }

        public static MessageVersion Soap12WSAddressingAugust2004
        {
            get { return s_soap12Addressing200408; }
        }

        public static MessageVersion Soap11WSAddressingAugust2004 { get; private set; }

        public static MessageVersion Soap11 { get; private set; }

        public static MessageVersion Soap12 { get; private set; }

        public override string ToString()
        {
            return string.Format(MessageVersionToStringFormat, Envelope.ToString(), Addressing.ToString());
        }

        internal bool IsMatch(MessageVersion messageVersion)
        {
            if (messageVersion == null)
            {
                Fx.Assert("Invalid (null) messageVersion value");
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(messageVersion));
            }
            if (Addressing == null)
            {
                Fx.Assert("Invalid (null) addressing value");
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "MessageVersion.Addressing cannot be null")));
            }

            if (Envelope != messageVersion.Envelope)
            {
                return false;
            }

            if (Addressing.Namespace != messageVersion.Addressing.Namespace)
            {
                return false;
            }

            return true;
        }
    }
}
