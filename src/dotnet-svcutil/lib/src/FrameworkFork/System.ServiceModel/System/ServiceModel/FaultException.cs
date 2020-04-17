// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Runtime;
using System.Runtime.Serialization;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace System.ServiceModel
{
    [KnownType(typeof(FaultException.FaultCodeData))]
    [KnownType(typeof(FaultException.FaultCodeData[]))]
    [KnownType(typeof(FaultException.FaultReasonData))]
    [KnownType(typeof(FaultException.FaultReasonData[]))]
    public class FaultException : CommunicationException
    {
        internal const string Namespace = "http://schemas.xmlsoap.org/Microsoft/WindowsCommunicationFoundation/2005/08/Faults/";

        private string _action;
        private FaultCode _code;
        private FaultReason _reason;
        private MessageFault _fault;

        public FaultException()
            : base(SRServiceModel.SFxFaultReason)
        {
            _code = FaultException.DefaultCode;
            _reason = FaultException.DefaultReason;
        }

        public FaultException(string reason)
            : base(reason)
        {
            _code = FaultException.DefaultCode;
            _reason = FaultException.CreateReason(reason);
        }

        public FaultException(FaultReason reason)
            : base(FaultException.GetSafeReasonText(reason))
        {
            _code = FaultException.DefaultCode;
            _reason = FaultException.EnsureReason(reason);
        }

        public FaultException(string reason, FaultCode code)
            : base(reason)
        {
            _code = FaultException.EnsureCode(code);
            _reason = FaultException.CreateReason(reason);
        }

        public FaultException(FaultReason reason, FaultCode code)
            : base(FaultException.GetSafeReasonText(reason))
        {
            _code = FaultException.EnsureCode(code);
            _reason = FaultException.EnsureReason(reason);
        }

        public FaultException(string reason, FaultCode code, string action)
            : base(reason)
        {
            _code = FaultException.EnsureCode(code);
            _reason = FaultException.CreateReason(reason);
            _action = action;
        }

        internal FaultException(string reason, FaultCode code, string action, Exception innerException)
            : base(reason, innerException)
        {
            _code = FaultException.EnsureCode(code);
            _reason = FaultException.CreateReason(reason);
            _action = action;
        }

        public FaultException(FaultReason reason, FaultCode code, string action)
            : base(FaultException.GetSafeReasonText(reason))
        {
            _code = FaultException.EnsureCode(code);
            _reason = FaultException.EnsureReason(reason);
            _action = action;
        }

        internal FaultException(FaultReason reason, FaultCode code, string action, Exception innerException)
            : base(FaultException.GetSafeReasonText(reason), innerException)
        {
            _code = FaultException.EnsureCode(code);
            _reason = FaultException.EnsureReason(reason);
            _action = action;
        }

        public FaultException(MessageFault fault)
            : base(FaultException.GetSafeReasonText(GetReason(fault)))
        {
            if (fault == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("fault");

            _code = FaultException.EnsureCode(fault.Code);
            _reason = FaultException.EnsureReason(fault.Reason);
            _fault = fault;
        }

        public FaultException(MessageFault fault, string action)
            : base(FaultException.GetSafeReasonText(GetReason(fault)))
        {
            if (fault == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("fault");

            _code = fault.Code;
            _reason = fault.Reason;
            _fault = fault;
            _action = action;
        }

        public string Action
        {
            get { return _action; }
        }

        public FaultCode Code
        {
            get { return _code; }
        }

        private static FaultReason DefaultReason
        {
            get { return new FaultReason(SRServiceModel.SFxFaultReason); }
        }

        private static FaultCode DefaultCode
        {
            get { return new FaultCode("Sender"); }
        }

        public override string Message
        {
            get { return FaultException.GetSafeReasonText(this.Reason); }
        }

        public FaultReason Reason
        {
            get { return _reason; }
        }

        internal MessageFault Fault
        {
            get { return _fault; }
        }

        private static FaultCode CreateCode(string code)
        {
            return (code != null) ? new FaultCode(code) : DefaultCode;
        }

        public static FaultException CreateFault(MessageFault messageFault, params Type[] faultDetailTypes)
        {
            return CreateFault(messageFault, null, faultDetailTypes);
        }

        public static FaultException CreateFault(MessageFault messageFault, string action, params Type[] faultDetailTypes)
        {
            if (messageFault == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("messageFault");
            }

            if (faultDetailTypes == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("faultDetailTypes");
            }
            DataContractSerializerFaultFormatter faultFormatter = new DataContractSerializerFaultFormatter(faultDetailTypes);
            return faultFormatter.Deserialize(messageFault, action);
        }

        public virtual MessageFault CreateMessageFault()
        {
            if (_fault != null)
            {
                return _fault;
            }
            else
            {
                return MessageFault.CreateFault(_code, _reason);
            }
        }

        private static FaultReason CreateReason(string reason)
        {
            return (reason != null) ? new FaultReason(reason) : DefaultReason;
        }

        private static FaultReason GetReason(MessageFault fault)
        {
            if (fault == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("fault");
            }
            return fault.Reason;
        }

        internal static string GetSafeReasonText(MessageFault messageFault)
        {
            if (messageFault == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("messageFault");

            return GetSafeReasonText(messageFault.Reason);
        }

        internal static string GetSafeReasonText(FaultReason reason)
        {
            if (reason == null)
                return SRServiceModel.SFxUnknownFaultNullReason0;

            try
            {
                return reason.GetMatchingTranslation(System.Globalization.CultureInfo.CurrentCulture).Text;
            }
            catch (ArgumentException)
            {
                if (reason.Translations.Count == 0)
                {
                    return SRServiceModel.SFxUnknownFaultZeroReasons0;
                }
                else
                {
                    return string.Format(SRServiceModel.SFxUnknownFaultNoMatchingTranslation1, reason.Translations[0].Text);
                }
            }
        }

        private static FaultCode EnsureCode(FaultCode code)
        {
            return (code != null) ? code : DefaultCode;
        }

        private static FaultReason EnsureReason(FaultReason reason)
        {
            return (reason != null) ? reason : DefaultReason;
        }

        internal class FaultCodeData
        {
            private string _name;
            private string _ns;

            internal static FaultCode Construct(FaultCodeData[] nodes)
            {
                FaultCode code = null;

                for (int i = nodes.Length - 1; i >= 0; i--)
                {
                    code = new FaultCode(nodes[i]._name, nodes[i]._ns, code);
                }

                return code;
            }

            internal static FaultCodeData[] GetObjectData(FaultCode code)
            {
                FaultCodeData[] array = new FaultCodeData[FaultCodeData.GetDepth(code)];

                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = new FaultCodeData();
                    array[i]._name = code.Name;
                    array[i]._ns = code.Namespace;
                    code = code.SubCode;
                }

                if (code != null)
                {
                    Fx.Assert("FaultException.FaultCodeData.GetObjectData: (code != null)");
                }
                return array;
            }

            private static int GetDepth(FaultCode code)
            {
                int depth = 0;

                while (code != null)
                {
                    depth++;
                    code = code.SubCode;
                }

                return depth;
            }
        }

        internal class FaultReasonData
        {
            private string _xmlLang;
            private string _text;

            internal static FaultReason Construct(FaultReasonData[] nodes)
            {
                FaultReasonText[] reasons = new FaultReasonText[nodes.Length];

                for (int i = 0; i < nodes.Length; i++)
                {
                    reasons[i] = new FaultReasonText(nodes[i]._text, nodes[i]._xmlLang);
                }

                return new FaultReason(reasons);
            }

            internal static FaultReasonData[] GetObjectData(FaultReason reason)
            {
                SynchronizedReadOnlyCollection<FaultReasonText> translations = reason.Translations;
                FaultReasonData[] array = new FaultReasonData[translations.Count];

                for (int i = 0; i < translations.Count; i++)
                {
                    array[i] = new FaultReasonData();
                    array[i]._xmlLang = translations[i].XmlLang;
                    array[i]._text = translations[i].Text;
                }

                return array;
            }
        }
    }

    public class FaultException<TDetail> : FaultException
    {
        private TDetail _detail;

        public FaultException(TDetail detail)
            : base()
        {
            _detail = detail;
        }

        public FaultException(TDetail detail, string reason)
            : base(reason)
        {
            _detail = detail;
        }

        public FaultException(TDetail detail, FaultReason reason)
            : base(reason)
        {
            _detail = detail;
        }

        public FaultException(TDetail detail, string reason, FaultCode code)
            : base(reason, code)
        {
            _detail = detail;
        }

        public FaultException(TDetail detail, FaultReason reason, FaultCode code)
            : base(reason, code)
        {
            _detail = detail;
        }

        public FaultException(TDetail detail, string reason, FaultCode code, string action)
            : base(reason, code, action)
        {
            _detail = detail;
        }

        public FaultException(TDetail detail, FaultReason reason, FaultCode code, string action)
            : base(reason, code, action)
        {
            _detail = detail;
        }

        public TDetail Detail
        {
            get { return _detail; }
        }

        public override MessageFault CreateMessageFault()
        {
            return MessageFault.CreateFault(this.Code, this.Reason, _detail);
        }

        public override string ToString()
        {
            return string.Format(SRServiceModel.SFxFaultExceptionToString3, this.GetType(), this.Message, _detail != null ? _detail.ToString() : String.Empty);
        }
    }
}
