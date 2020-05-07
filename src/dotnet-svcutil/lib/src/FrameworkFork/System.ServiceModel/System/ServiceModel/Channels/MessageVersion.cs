// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Runtime;

namespace System.ServiceModel.Channels
{
    public sealed class MessageVersion
    {
        private EnvelopeVersion _envelope;
        private AddressingVersion _addressing;
        private static MessageVersion s_none;
        private static MessageVersion s_soap11;
        private static MessageVersion s_soap12;
        private static MessageVersion s_soap11Addressing10;
        private static MessageVersion s_soap12Addressing10;
        private const string MessageVersionToStringFormat = "{0} {1}";

        static MessageVersion()
        {
            s_none = new MessageVersion(EnvelopeVersion.None, AddressingVersion.None);
            s_soap11 = new MessageVersion(EnvelopeVersion.Soap11, AddressingVersion.None);
            s_soap12 = new MessageVersion(EnvelopeVersion.Soap12, AddressingVersion.None);
            s_soap11Addressing10 = new MessageVersion(EnvelopeVersion.Soap11, AddressingVersion.WSAddressing10);
            s_soap12Addressing10 = new MessageVersion(EnvelopeVersion.Soap12, AddressingVersion.WSAddressing10);
        }

        private MessageVersion(EnvelopeVersion envelopeVersion, AddressingVersion addressingVersion)
        {
            _envelope = envelopeVersion;
            _addressing = addressingVersion;
        }

        public static MessageVersion CreateVersion(EnvelopeVersion envelopeVersion)
        {
            return CreateVersion(envelopeVersion, AddressingVersion.WSAddressing10);
        }

        public static MessageVersion CreateVersion(EnvelopeVersion envelopeVersion, AddressingVersion addressingVersion)
        {
            if (envelopeVersion == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("envelopeVersion");
            }

            if (addressingVersion == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("addressingVersion");
            }

            if (envelopeVersion == EnvelopeVersion.Soap12)
            {
                if (addressingVersion == AddressingVersion.WSAddressing10)
                {
                    return s_soap12Addressing10;
                }
                else if (addressingVersion == AddressingVersion.None)
                {
                    return s_soap12;
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("addressingVersion",
                        string.Format(SRServiceModel.AddressingVersionNotSupported, addressingVersion));
                }
            }
            else if (envelopeVersion == EnvelopeVersion.Soap11)
            {
                if (addressingVersion == AddressingVersion.WSAddressing10)
                {
                    return s_soap11Addressing10;
                }
                else if (addressingVersion == AddressingVersion.None)
                {
                    return s_soap11;
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("addressingVersion",
                        string.Format(SRServiceModel.AddressingVersionNotSupported, addressingVersion));
                }
            }
            else if (envelopeVersion == EnvelopeVersion.None)
            {
                if (addressingVersion == AddressingVersion.None)
                {
                    return s_none;
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("addressingVersion",
                        string.Format(SRServiceModel.AddressingVersionNotSupported, addressingVersion));
                }
            }
            else
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("envelopeVersion",
                    string.Format(SRServiceModel.EnvelopeVersionNotSupported, envelopeVersion));
            }
        }

        public AddressingVersion Addressing
        {
            get { return _addressing; }
        }

        public static MessageVersion Default
        {
            get { return s_soap12Addressing10; }
        }

        public EnvelopeVersion Envelope
        {
            get { return _envelope; }
        }

        public override bool Equals(object obj)
        {
            return this == obj;
        }

        public override int GetHashCode()
        {
            int code = 0;
            if (this.Envelope == EnvelopeVersion.Soap11)
                code += 1;
            return code;
        }

        public static MessageVersion None
        {
            get { return s_none; }
        }

        public static MessageVersion Soap12WSAddressing10
        {
            get { return s_soap12Addressing10; }
        }

        public static MessageVersion Soap11WSAddressing10
        {
            get { return s_soap11Addressing10; }
        }

        public static MessageVersion Soap11
        {
            get { return s_soap11; }
        }

        public static MessageVersion Soap12
        {
            get { return s_soap12; }
        }

        public override string ToString()
        {
            return string.Format(MessageVersionToStringFormat, _envelope.ToString(), _addressing.ToString());
        }

        internal bool IsMatch(MessageVersion messageVersion)
        {
            if (messageVersion == null)
            {
                Fx.Assert("Invalid (null) messageVersion value");
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("messageVersion");
            }
            if (_addressing == null)
            {
                Fx.Assert("Invalid (null) addressing value");
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "MessageVersion.Addressing cannot be null")));
            }

            if (_envelope != messageVersion.Envelope)
                return false;
            if (_addressing.Namespace != messageVersion.Addressing.Namespace)
                return false;
            return true;
        }
    }
}
