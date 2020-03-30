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
    [Serializable]
    [KnownType(typeof(FaultCodeData))]
    [KnownType(typeof(FaultCodeData[]))]
    [KnownType(typeof(FaultReasonData))]
    [KnownType(typeof(FaultReasonData[]))]
    public class FaultException : CommunicationException
    {
        internal const string Namespace = "http://schemas.xmlsoap.org/Microsoft/WindowsCommunicationFoundation/2005/08/Faults/";
        private MessageFault _fault;

        public FaultException()
            : base(SR.SFxFaultReason)
        {
            Code = FaultException.DefaultCode;
            Reason = FaultException.DefaultReason;
        }

        public FaultException(string reason)
            : base(reason)
        {
            Code = FaultException.DefaultCode;
            Reason = FaultException.CreateReason(reason);
        }

        public FaultException(FaultReason reason)
            : base(FaultException.GetSafeReasonText(reason))
        {
            Code = FaultException.DefaultCode;
            Reason = FaultException.EnsureReason(reason);
        }

        public FaultException(string reason, FaultCode code)
            : base(reason)
        {
            Code = FaultException.EnsureCode(code);
            Reason = FaultException.CreateReason(reason);
        }

        public FaultException(FaultReason reason, FaultCode code)
            : base(FaultException.GetSafeReasonText(reason))
        {
            Code = FaultException.EnsureCode(code);
            Reason = FaultException.EnsureReason(reason);
        }

        public FaultException(string reason, FaultCode code, string action)
            : base(reason)
        {
            Code = FaultException.EnsureCode(code);
            Reason = FaultException.CreateReason(reason);
            Action = action;
        }

        internal FaultException(string reason, FaultCode code, string action, Exception innerException)
            : base(reason, innerException)
        {
            Code = FaultException.EnsureCode(code);
            Reason = FaultException.CreateReason(reason);
            Action = action;
        }

        public FaultException(FaultReason reason, FaultCode code, string action)
            : base(FaultException.GetSafeReasonText(reason))
        {
            Code = FaultException.EnsureCode(code);
            Reason = FaultException.EnsureReason(reason);
            Action = action;
        }

        internal FaultException(FaultReason reason, FaultCode code, string action, Exception innerException)
            : base(FaultException.GetSafeReasonText(reason), innerException)
        {
            Code = FaultException.EnsureCode(code);
            Reason = FaultException.EnsureReason(reason);
            Action = action;
        }

        public FaultException(MessageFault fault)
            : base(FaultException.GetSafeReasonText(GetReason(fault)))
        {
            if (fault == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(fault));
            }

            Code = FaultException.EnsureCode(fault.Code);
            Reason = FaultException.EnsureReason(fault.Reason);
            _fault = fault;
        }

        public FaultException(MessageFault fault, string action)
            : base(FaultException.GetSafeReasonText(GetReason(fault)))
        {
            if (fault == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(fault));
            }

            Code = fault.Code;
            Reason = fault.Reason;
            _fault = fault;
            Action = action;
        }

        protected FaultException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            throw new PlatformNotSupportedException();
        }

        public string Action { get; }

        public FaultCode Code { get; }

        private static FaultReason DefaultReason
        {
            get { return new FaultReason(SR.SFxFaultReason); }
        }

        private static FaultCode DefaultCode
        {
            get { return new FaultCode("Sender"); }
        }

        public override string Message
        {
            get { return FaultException.GetSafeReasonText(Reason); }
        }

        public FaultReason Reason { get; }

        internal MessageFault Fault
        {
            get { return _fault; }
        }

        internal void AddFaultCodeObjectData(SerializationInfo info, string key, FaultCode code)
        {
            info.AddValue(key, FaultCodeData.GetObjectData(code));
        }

        internal void AddFaultReasonObjectData(SerializationInfo info, string key, FaultReason reason)
        {
            info.AddValue(key, FaultReasonData.GetObjectData(reason));
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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(messageFault));
            }

            if (faultDetailTypes == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(faultDetailTypes));
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
                return MessageFault.CreateFault(Code, Reason);
            }
        }

        private static FaultReason CreateReason(string reason)
        {
            return (reason != null) ? new FaultReason(reason) : DefaultReason;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            AddFaultCodeObjectData(info, "code", Code);
            AddFaultReasonObjectData(info, "reason", Reason);
            info.AddValue("messageFault", this._fault);
            info.AddValue("action", this.Action);
        }

        private static FaultReason GetReason(MessageFault fault)
        {
            if (fault == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(fault));
            }
            return fault.Reason;
        }

        internal static string GetSafeReasonText(MessageFault messageFault)
        {
            if (messageFault == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(messageFault));
            }

            return GetSafeReasonText(messageFault.Reason);
        }

        internal static string GetSafeReasonText(FaultReason reason)
        {
            if (reason == null)
            {
                return SR.SFxUnknownFaultNullReason0;
            }

            try
            {
                return reason.GetMatchingTranslation(System.Globalization.CultureInfo.CurrentCulture).Text;
            }
            catch (ArgumentException)
            {
                if (reason.Translations.Count == 0)
                {
                    return SR.SFxUnknownFaultZeroReasons0;
                }
                else
                {
                    return SR.Format(SR.SFxUnknownFaultNoMatchingTranslation1, reason.Translations[0].Text);
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

        [Serializable]
        internal class FaultCodeData
        {
            private string name;
            private string ns;

            internal static FaultCode Construct(FaultCodeData[] nodes)
            {
                FaultCode code = null;

                for (int i = nodes.Length - 1; i >= 0; i--)
                {
                    code = new FaultCode(nodes[i].name, nodes[i].ns, code);
                }

                return code;
            }

            internal static FaultCodeData[] GetObjectData(FaultCode code)
            {
                FaultCodeData[] array = new FaultCodeData[FaultCodeData.GetDepth(code)];

                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = new FaultCodeData();
                    array[i].name = code.Name;
                    array[i].ns = code.Namespace;
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

        [Serializable]
        internal class FaultReasonData
        {
            private string xmlLang;
            private string text;

            internal static FaultReason Construct(FaultReasonData[] nodes)
            {
                FaultReasonText[] reasons = new FaultReasonText[nodes.Length];

                for (int i = 0; i < nodes.Length; i++)
                {
                    reasons[i] = new FaultReasonText(nodes[i].text, nodes[i].xmlLang);
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
                    array[i].xmlLang = translations[i].XmlLang;
                    array[i].text = translations[i].Text;
                }

                return array;
            }
        }
    }

    [Serializable]
    public class FaultException<TDetail> : FaultException
    {
        public FaultException(TDetail detail)
            : base()
        {
            Detail = detail;
        }

        public FaultException(TDetail detail, string reason)
            : base(reason)
        {
            Detail = detail;
        }

        public FaultException(TDetail detail, FaultReason reason)
            : base(reason)
        {
            Detail = detail;
        }

        public FaultException(TDetail detail, string reason, FaultCode code)
            : base(reason, code)
        {
            Detail = detail;
        }

        public FaultException(TDetail detail, FaultReason reason, FaultCode code)
            : base(reason, code)
        {
            Detail = detail;
        }

        public FaultException(TDetail detail, string reason, FaultCode code, string action)
            : base(reason, code, action)
        {
            Detail = detail;
        }

        public FaultException(TDetail detail, FaultReason reason, FaultCode code, string action)
            : base(reason, code, action)
        {
            Detail = detail;
        }

        protected FaultException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            throw new PlatformNotSupportedException();
        }

        public TDetail Detail { get; }

        public override MessageFault CreateMessageFault()
        {
            return MessageFault.CreateFault(Code, Reason, Detail);
        }

        public override string ToString()
        {
            return SR.Format(SR.SFxFaultExceptionToString3, GetType(), Message, Detail != null ? Detail.ToString() : String.Empty);
        }
    }
}
