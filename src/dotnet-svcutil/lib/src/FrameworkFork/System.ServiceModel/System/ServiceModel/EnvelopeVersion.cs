// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Xml;

namespace System.ServiceModel
{
    public sealed class EnvelopeVersion
    {
        private string _ultimateDestinationActor;
        private string[] _ultimateDestinationActorValues;
        private string _nextDestinationActorValue;
        private string _ns;
        private XmlDictionaryString _dictionaryNs;
        private string _actor;
        private XmlDictionaryString _dictionaryActor;
        private string _toStringFormat;
        private string[] _mustUnderstandActorValues;
        private string _senderFaultName;
        private string _receiverFaultName;
        private const string Soap11ToStringFormat = "Soap11 ({0})";
        private const string Soap12ToStringFormat = "Soap12 ({0})";
        private const string EnvelopeNoneToStringFormat = "EnvelopeNone ({0})";

        private static EnvelopeVersion s_soap11 =
            new EnvelopeVersion(
                "",
                "http://schemas.xmlsoap.org/soap/actor/next",
                Message11Strings.Namespace,
                XD.Message11Dictionary.Namespace,
                Message11Strings.Actor,
                XD.Message11Dictionary.Actor,
                Soap11ToStringFormat,
                "Client",
                "Server");

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

        private static EnvelopeVersion s_none = new EnvelopeVersion(
                null,
                null,
                MessageStrings.Namespace,
                XD.MessageDictionary.Namespace,
                null,
                null,
                EnvelopeNoneToStringFormat,
                "Sender",
                "Receiver");

        private EnvelopeVersion(string ultimateReceiverActor, string nextDestinationActorValue,
            string ns, XmlDictionaryString dictionaryNs, string actor, XmlDictionaryString dictionaryActor,
            string toStringFormat, string senderFaultName, string receiverFaultName)
        {
            _toStringFormat = toStringFormat;
            _ultimateDestinationActor = ultimateReceiverActor;
            _nextDestinationActorValue = nextDestinationActorValue;
            _ns = ns;
            _dictionaryNs = dictionaryNs;
            _actor = actor;
            _dictionaryActor = dictionaryActor;
            _senderFaultName = senderFaultName;
            _receiverFaultName = receiverFaultName;

            if (ultimateReceiverActor != null)
            {
                if (ultimateReceiverActor.Length == 0)
                {
                    _mustUnderstandActorValues = new string[] { "", nextDestinationActorValue };
                    _ultimateDestinationActorValues = new string[] { "", nextDestinationActorValue };
                }
                else
                {
                    _mustUnderstandActorValues = new string[] { "", ultimateReceiverActor, nextDestinationActorValue };
                    _ultimateDestinationActorValues = new string[] { "", ultimateReceiverActor, nextDestinationActorValue };
                }
            }
        }

        internal string Actor
        {
            get { return _actor; }
        }

        internal XmlDictionaryString DictionaryActor
        {
            get { return _dictionaryActor; }
        }

        internal string Namespace
        {
            get { return _ns; }
        }

        internal XmlDictionaryString DictionaryNamespace
        {
            get { return _dictionaryNs; }
        }

        public string NextDestinationActorValue
        {
            get { return _nextDestinationActorValue; }
        }

        public static EnvelopeVersion None
        {
            get { return s_none; }
        }

        public static EnvelopeVersion Soap11
        {
            get { return s_soap11; }
        }

        public static EnvelopeVersion Soap12
        {
            get { return s_soap12; }
        }

        internal string ReceiverFaultName
        {
            get { return _receiverFaultName; }
        }

        internal string SenderFaultName
        {
            get { return _senderFaultName; }
        }

        internal string[] MustUnderstandActorValues
        {
            get { return _mustUnderstandActorValues; }
        }

        internal string UltimateDestinationActor
        {
            get { return _ultimateDestinationActor; }
        }

        public string[] GetUltimateDestinationActorValues()
        {
            return (string[])_ultimateDestinationActorValues.Clone();
        }

        internal string[] UltimateDestinationActorValues
        {
            get { return _ultimateDestinationActorValues; }
        }

        internal bool IsUltimateDestinationActor(string actor)
        {
            return actor.Length == 0 || actor == _ultimateDestinationActor || actor == _nextDestinationActorValue;
        }

        public override string ToString()
        {
            return string.Format(_toStringFormat, Namespace);
        }
    }
}
