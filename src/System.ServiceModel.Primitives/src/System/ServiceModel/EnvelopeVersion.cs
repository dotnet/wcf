// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Xml;

namespace System.ServiceModel
{
    public sealed class EnvelopeVersion
    {
        private string _ns;
        private string _toStringFormat;
        private string _receiverFaultName;
        private const string Soap11ToStringFormat = "Soap11 ({0})";
        private const string Soap12ToStringFormat = "Soap12 ({0})";
        private const string EnvelopeNoneToStringFormat = "EnvelopeNone ({0})";
        private static EnvelopeVersion s_soap12 =
            new EnvelopeVersion(
                "http://www.w3.org/2003/05/soap-envelope/role/ultimateReceiver",
                "http://www.w3.org/2003/05/soap-envelope/role/next",
                Message12Strings.Namespace,
                XD.Message12Dictionary.Namespace,
                Message12Strings.Role,
                XD.Message12Dictionary.Role,
                Soap12ToStringFormat,
                "Sender",
                "Receiver");

        private EnvelopeVersion(string ultimateReceiverActor, string nextDestinationActorValue,
            string ns, XmlDictionaryString dictionaryNs, string actor, XmlDictionaryString dictionaryActor,
            string toStringFormat, string senderFaultName, string receiverFaultName)
        {
            _toStringFormat = toStringFormat;
            UltimateDestinationActor = ultimateReceiverActor;
            NextDestinationActorValue = nextDestinationActorValue;
            _ns = ns;
            DictionaryNamespace = dictionaryNs;
            Actor = actor;
            DictionaryActor = dictionaryActor;
            SenderFaultName = senderFaultName;
            _receiverFaultName = receiverFaultName;

            if (ultimateReceiverActor != null)
            {
                if (ultimateReceiverActor.Length == 0)
                {
                    MustUnderstandActorValues = new string[] { "", nextDestinationActorValue };
                    UltimateDestinationActorValues = new string[] { "", nextDestinationActorValue };
                }
                else
                {
                    MustUnderstandActorValues = new string[] { "", ultimateReceiverActor, nextDestinationActorValue };
                    UltimateDestinationActorValues = new string[] { "", ultimateReceiverActor, nextDestinationActorValue };
                }
            }
        }

        internal string Actor { get; }

        internal XmlDictionaryString DictionaryActor { get; }

        internal string Namespace
        {
            get { return _ns; }
        }

        internal XmlDictionaryString DictionaryNamespace { get; }

        public string NextDestinationActorValue { get; }

        public static EnvelopeVersion None { get; } = new EnvelopeVersion(
                null,
                null,
                MessageStrings.Namespace,
                XD.MessageDictionary.Namespace,
                null,
                null,
                EnvelopeNoneToStringFormat,
                "Sender",
                "Receiver");

        public static EnvelopeVersion Soap11 { get; } = new EnvelopeVersion(
                "",
                "http://schemas.xmlsoap.org/soap/actor/next",
                Message11Strings.Namespace,
                XD.Message11Dictionary.Namespace,
                Message11Strings.Actor,
                XD.Message11Dictionary.Actor,
                Soap11ToStringFormat,
                "Client",
                "Server");

        public static EnvelopeVersion Soap12
        {
            get { return s_soap12; }
        }

        internal string ReceiverFaultName
        {
            get { return _receiverFaultName; }
        }

        internal string SenderFaultName { get; }

        internal string[] MustUnderstandActorValues { get; }

        internal string UltimateDestinationActor { get; }

        public string[] GetUltimateDestinationActorValues()
        {
            return (string[])UltimateDestinationActorValues.Clone();
        }

        internal string[] UltimateDestinationActorValues { get; }

        internal bool IsUltimateDestinationActor(string actor)
        {
            return actor.Length == 0 || actor == UltimateDestinationActor || actor == NextDestinationActorValue;
        }

        public override string ToString()
        {
            return string.Format(_toStringFormat, Namespace);
        }
    }
}
