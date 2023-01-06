// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel.Description;

namespace System.ServiceModel
{
    public class FaultCode
    {
        private string _ns;
        private EnvelopeVersion _version;

        public FaultCode(string name)
            : this(name, "", null)
        {
        }

        public FaultCode(string name, FaultCode subCode)
            : this(name, "", subCode)
        {
        }

        public FaultCode(string name, string ns)
            : this(name, ns, null)
        {
        }

        public FaultCode(string name, string ns, FaultCode subCode)
        {
            if (name == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(name)));
            }

            if (name.Length == 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(name)));
            }

            if (!string.IsNullOrEmpty(ns))
            {
                NamingHelper.CheckUriParameter(ns, "ns");
            }

            Name = name;
            _ns = ns;
            SubCode = subCode;

            if (ns == Message12Strings.Namespace)
            {
                _version = EnvelopeVersion.Soap12;
            }
            else if (ns == Message11Strings.Namespace)
            {
                _version = EnvelopeVersion.Soap11;
            }
            else if (ns == MessageStrings.Namespace)
            {
                _version = EnvelopeVersion.None;
            }
            else
            {
                _version = null;
            }
        }

        public bool IsPredefinedFault
        {
            get
            {
                return _ns.Length == 0 || _version != null;
            }
        }

        public bool IsSenderFault
        {
            get
            {
                if (IsPredefinedFault)
                {
                    return Name == (_version ?? EnvelopeVersion.Soap12).SenderFaultName;
                }

                return false;
            }
        }

        public bool IsReceiverFault
        {
            get
            {
                if (IsPredefinedFault)
                {
                    return Name == (_version ?? EnvelopeVersion.Soap12).ReceiverFaultName;
                }

                return false;
            }
        }

        public string Namespace
        {
            get
            {
                return _ns;
            }
        }

        public string Name { get; }

        public FaultCode SubCode { get; }

        public static FaultCode CreateSenderFaultCode(FaultCode subCode)
        {
            return new FaultCode("Sender", subCode);
        }

        public static FaultCode CreateSenderFaultCode(string name, string ns)
        {
            return CreateSenderFaultCode(new FaultCode(name, ns));
        }

        public static FaultCode CreateReceiverFaultCode(FaultCode subCode)
        {
            if (subCode == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(subCode)));
            }

            return new FaultCode("Receiver", subCode);
        }

        public static FaultCode CreateReceiverFaultCode(string name, string ns)
        {
            return CreateReceiverFaultCode(new FaultCode(name, ns));
        }
    }
}
