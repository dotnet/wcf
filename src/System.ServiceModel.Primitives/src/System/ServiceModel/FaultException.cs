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

        public FaultException()
            : base(SRP.SFxFaultReason)
        {
            Code = DefaultCode;
            Reason = DefaultReason;
        }

        public FaultException(string reason)
            : base(reason)
        {
            Code = DefaultCode;
            Reason = CreateReason(reason);
        }

        public FaultException(FaultReason reason)
            : base(GetSafeReasonText(reason))
        {
            Code = DefaultCode;
            Reason = EnsureReason(reason);
        }

        public FaultException(string reason, FaultCode code)
            : base(reason)
        {
            Code = EnsureCode(code);
            Reason = CreateReason(reason);
        }

        public FaultException(FaultReason reason, FaultCode code)
            : base(GetSafeReasonText(reason))
        {
            Code = EnsureCode(code);
            Reason = EnsureReason(reason);
        }

        public FaultException(string reason, FaultCode code, string action)
            : base(reason)
        {
            Code = EnsureCode(code);
            Reason = CreateReason(reason);
            Action = action;
        }

        internal FaultException(string reason, FaultCode code, string action, Exception innerException)
            : base(reason, innerException)
        {
            Code = EnsureCode(code);
            Reason = CreateReason(reason);
            Action = action;
        }

        public FaultException(FaultReason reason, FaultCode code, string action)
            : base(GetSafeReasonText(reason))
        {
            Code = EnsureCode(code);
            Reason = EnsureReason(reason);
            Action = action;
        }

        internal FaultException(FaultReason reason, FaultCode code, string action, Exception innerException)
            : base(GetSafeReasonText(reason), innerException)
        {
            Code = EnsureCode(code);
            Reason = EnsureReason(reason);
            Action = action;
        }

        public FaultException(MessageFault fault)
            : base(GetSafeReasonText(GetReason(fault)))
        {
            if (fault == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(fault));
            }

            Code = EnsureCode(fault.Code);
            Reason = EnsureReason(fault.Reason);
            Fault = fault;
        }

        public FaultException(MessageFault fault, string action)
            : base(GetSafeReasonText(GetReason(fault)))
        {
            if (fault == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(fault));
            }

            Code = fault.Code;
            Reason = fault.Reason;
            Fault = fault;
            Action = action;
        }

#pragma warning disable SYSLIB0051
        protected FaultException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Code = ReconstructFaultCode(info, "code");
            Reason = ReconstructFaultReason(info, "reason");
            Fault = (MessageFault)info.GetValue("messageFault", typeof(MessageFault));
            Action = info.GetString("action");
        }
#pragma warning restore SYSLIB0051

        public string Action { get; }

        public FaultCode Code { get; }

        private static FaultReason DefaultReason
        {
            get { return new FaultReason(SRP.SFxFaultReason); }
        }

        private static FaultCode DefaultCode
        {
            get { return new FaultCode("Sender"); }
        }

        public override string Message
        {
            get { return GetSafeReasonText(Reason); }
        }

        public FaultReason Reason { get; }

        internal MessageFault Fault { get; }

        internal void AddFaultCodeObjectData(SerializationInfo info, string key, FaultCode code)
        {
            info.AddValue(key, FaultCodeData.GetObjectData(code));
        }

        internal void AddFaultReasonObjectData(SerializationInfo info, string key, FaultReason reason)
        {
            info.AddValue(key, FaultReasonData.GetObjectData(reason));
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
            if (Fault != null)
            {
                return Fault;
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

        [System.Obsolete]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            AddFaultCodeObjectData(info, "code", Code);
            AddFaultReasonObjectData(info, "reason", Reason);
            info.AddValue("messageFault", Fault);
            info.AddValue("action", Action);
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
                return SRP.SFxUnknownFaultNullReason0;
            }

            try
            {
                return reason.GetMatchingTranslation(Globalization.CultureInfo.CurrentCulture).Text;
            }
            catch (ArgumentException)
            {
                if (reason.Translations.Count == 0)
                {
                    return SRP.SFxUnknownFaultZeroReasons0;
                }
                else
                {
                    return SRP.Format(SRP.SFxUnknownFaultNoMatchingTranslation1, reason.Translations[0].Text);
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

        internal FaultCode ReconstructFaultCode(SerializationInfo info, string key)
        {
            FaultCodeData[] data = (FaultCodeData[])info.GetValue(key, typeof(FaultCodeData[]));
            return FaultCodeData.Construct(data);
        }

        internal FaultReason ReconstructFaultReason(SerializationInfo info, string key)
        {
            FaultReasonData[] data = (FaultReasonData[])info.GetValue(key, typeof(FaultReasonData[]));
            return FaultReasonData.Construct(data);
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
                FaultCodeData[] array = new FaultCodeData[GetDepth(code)];

                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = new FaultCodeData();
                    array[i].name = code.Name;
                    array[i].ns = code.Namespace;
                    code = code.SubCode;
                }

                Fx.Assert(code == null, "FaultException.FaultCodeData.GetObjectData: (code != null)");
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
    [KnownType("GetKnownTypes")]
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

#pragma warning disable SYSLIB0051
        protected FaultException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Detail = (TDetail)info.GetValue("detail", typeof(TDetail));
        }
#pragma warning restore SYSLIB0051

        public TDetail Detail { get; }

        public override MessageFault CreateMessageFault()
        {
            return MessageFault.CreateFault(Code, Reason, Detail);
        }

        [System.Obsolete]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("detail", Detail);
        }

        public override string ToString()
        {
            return SRP.Format(SRP.SFxFaultExceptionToString3, GetType(), Message, Detail != null ? Detail.ToString() : string.Empty);
        }

        private static Type[] s_knownTypes = new Type[] { typeof(TDetail) };
        internal static IEnumerable<Type> GetKnownTypes()
        {
            return s_knownTypes;
        }
    }
}
