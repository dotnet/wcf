// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Xml;
using System.ServiceModel.Security;

namespace System.ServiceModel.Channels
{
    public sealed class AddressingVersion
    {
        private string _toStringFormat;
        private Uri _noneUri;
        private string _defaultFaultAction;
        private const string AddressingNoneToStringFormat = "AddressingNone ({0})";
        private const string Addressing10ToStringFormat = "Addressing10 ({0})";
        private const string Addressing200408ToStringFormat = "Addressing200408 ({0})";
        private static AddressingVersion s_addressing10 = new AddressingVersion(Addressing10Strings.Namespace,
            XD.Addressing10Dictionary.Namespace, Addressing10ToStringFormat, Addressing10SignedMessageParts,
            Addressing10Strings.Anonymous, XD.Addressing10Dictionary.Anonymous, Addressing10Strings.NoneAddress,
            Addressing10Strings.FaultAction, Addressing10Strings.DefaultFaultAction);
        private static MessagePartSpecification s_addressing10SignedMessageParts;
        private static MessagePartSpecification s_addressing200408SignedMessageParts;

        private AddressingVersion(string ns, XmlDictionaryString dictionaryNs, string toStringFormat,
            MessagePartSpecification signedMessageParts, string anonymous, XmlDictionaryString dictionaryAnonymous, string none, string faultAction, string defaultFaultAction)
        {
            Namespace = ns;
            DictionaryNamespace = dictionaryNs;
            _toStringFormat = toStringFormat;
            SignedMessageParts = signedMessageParts;
            Anonymous = anonymous;
            DictionaryAnonymous = dictionaryAnonymous;

            if (anonymous != null)
            {
                AnonymousUri = new Uri(anonymous);
            }

            if (none != null)
            {
                _noneUri = new Uri(none);
            }

            FaultAction = faultAction;
            _defaultFaultAction = defaultFaultAction;
        }


        public static AddressingVersion WSAddressingAugust2004 { get; } = new AddressingVersion(Addressing200408Strings.Namespace,
            XD.Addressing200408Dictionary.Namespace, Addressing200408ToStringFormat, Addressing200408SignedMessageParts,
            Addressing200408Strings.Anonymous, XD.Addressing200408Dictionary.Anonymous, null,
            Addressing200408Strings.FaultAction, Addressing200408Strings.DefaultFaultAction);

        public static AddressingVersion WSAddressing10
        {
            get { return s_addressing10; }
        }

        public static AddressingVersion None { get; } = new AddressingVersion(AddressingNoneStrings.Namespace, XD.AddressingNoneDictionary.Namespace,
            AddressingNoneToStringFormat, new MessagePartSpecification(), null, null, null, null, null);

        internal string Namespace { get; }

        private static MessagePartSpecification Addressing10SignedMessageParts
        {
            get
            {
                if (s_addressing10SignedMessageParts == null)
                {
                    MessagePartSpecification s = new MessagePartSpecification(
                        new XmlQualifiedName(AddressingStrings.To, Addressing10Strings.Namespace),
                        new XmlQualifiedName(AddressingStrings.From, Addressing10Strings.Namespace),
                        new XmlQualifiedName(AddressingStrings.FaultTo, Addressing10Strings.Namespace),
                        new XmlQualifiedName(AddressingStrings.ReplyTo, Addressing10Strings.Namespace),
                        new XmlQualifiedName(AddressingStrings.MessageId, Addressing10Strings.Namespace),
                        new XmlQualifiedName(AddressingStrings.RelatesTo, Addressing10Strings.Namespace),
                        new XmlQualifiedName(AddressingStrings.Action, Addressing10Strings.Namespace)
                        );
                    s.MakeReadOnly();
                    s_addressing10SignedMessageParts = s;
                }

                return s_addressing10SignedMessageParts;
            }
        }

        private static MessagePartSpecification Addressing200408SignedMessageParts
        {
            get
            {
                if (s_addressing200408SignedMessageParts == null)
                {
                    MessagePartSpecification s = new MessagePartSpecification(
                        new XmlQualifiedName(AddressingStrings.To, Addressing200408Strings.Namespace),
                        new XmlQualifiedName(AddressingStrings.From, Addressing200408Strings.Namespace),
                        new XmlQualifiedName(AddressingStrings.FaultTo, Addressing200408Strings.Namespace),
                        new XmlQualifiedName(AddressingStrings.ReplyTo, Addressing200408Strings.Namespace),
                        new XmlQualifiedName(AddressingStrings.MessageId, Addressing200408Strings.Namespace),
                        new XmlQualifiedName(AddressingStrings.RelatesTo, Addressing200408Strings.Namespace),
                        new XmlQualifiedName(AddressingStrings.Action, Addressing200408Strings.Namespace)
                        );
                    s.MakeReadOnly();
                    s_addressing200408SignedMessageParts = s;
                }

                return s_addressing200408SignedMessageParts;
            }
        }

        internal XmlDictionaryString DictionaryNamespace { get; }

        internal string Anonymous { get; }

        internal XmlDictionaryString DictionaryAnonymous { get; }

        internal Uri AnonymousUri { get; }

        internal Uri NoneUri
        {
            get { return _noneUri; }
        }

        internal string FaultAction   // the action for addressing faults
        { get; }

        internal string DefaultFaultAction  // a default string that can be used for non-addressing faults
        {
            get { return _defaultFaultAction; }
        }

        internal MessagePartSpecification SignedMessageParts { get; }

        public override string ToString()
        {
            return string.Format(_toStringFormat, Namespace);
        }
    }
}
