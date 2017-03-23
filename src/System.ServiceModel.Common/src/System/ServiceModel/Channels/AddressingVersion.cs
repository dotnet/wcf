// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Xml;

namespace System.ServiceModel.Channels
{
    public sealed class AddressingVersion
    {
        private string _ns;
        private XmlDictionaryString _dictionaryNs;
        private string _toStringFormat;
        private string _anonymous;
        private XmlDictionaryString _dictionaryAnonymous;
        private Uri _anonymousUri;
        private Uri _noneUri;
        private string _faultAction;
        private string _defaultFaultAction;
        private const string AddressingNoneToStringFormat = "AddressingNone ({0})";
        private const string Addressing10ToStringFormat = "Addressing10 ({0})";

        private static AddressingVersion s_none = new AddressingVersion(AddressingNoneStrings.Namespace, XD.AddressingNoneDictionary.Namespace,
            AddressingNoneToStringFormat, null, null, null, null, null);

        private static AddressingVersion s_addressing10 = new AddressingVersion(Addressing10Strings.Namespace,
            XD.Addressing10Dictionary.Namespace, Addressing10ToStringFormat, Addressing10Strings.Anonymous, XD.Addressing10Dictionary.Anonymous, Addressing10Strings.NoneAddress,
            Addressing10Strings.FaultAction, Addressing10Strings.DefaultFaultAction);

        static AddressingVersion s_addressing200408 = new AddressingVersion(Addressing200408Strings.Namespace,
            XD.Addressing200408Dictionary.Namespace, SR.Addressing200408ToStringFormat, Addressing200408Strings.Anonymous, XD.Addressing200408Dictionary.Anonymous, null,
            Addressing200408Strings.FaultAction, Addressing200408Strings.DefaultFaultAction);

        private AddressingVersion(string ns, XmlDictionaryString dictionaryNs, string toStringFormat,
            string anonymous, XmlDictionaryString dictionaryAnonymous, string none, string faultAction, string defaultFaultAction)
        {
            _ns = ns;
            _dictionaryNs = dictionaryNs;
            _toStringFormat = toStringFormat;
            _anonymous = anonymous;
            _dictionaryAnonymous = dictionaryAnonymous;

            if (anonymous != null)
            {
                _anonymousUri = new Uri(anonymous);
            }

            if (none != null)
            {
                _noneUri = new Uri(none);
            }

            _faultAction = faultAction;
            _defaultFaultAction = defaultFaultAction;
        }

        // WSAddressingAugust2004 is used by AddressHeader.GetComparableReader() as it does
        // not write the IsReferenceParameter attribute, and that's good for a consistent comparable form
        internal static AddressingVersion WSAddressingAugust2004
        {
            get { return s_addressing200408; }
        }

        public static AddressingVersion WSAddressing10
        {
            get { return s_addressing10; }
        }

        public static AddressingVersion None
        {
            get { return s_none; }
        }

        internal string Namespace
        {
            get { return _ns; }
        }

        internal XmlDictionaryString DictionaryNamespace
        {
            get { return _dictionaryNs; }
        }

        internal string Anonymous
        {
            get { return _anonymous; }
        }

        internal XmlDictionaryString DictionaryAnonymous
        {
            get { return _dictionaryAnonymous; }
        }

        internal Uri AnonymousUri
        {
            get { return _anonymousUri; }
        }

        internal Uri NoneUri
        {
            get { return _noneUri; }
        }

        internal string FaultAction   // the action for addressing faults
        {
            get { return _faultAction; }
        }

        internal string DefaultFaultAction  // a default string that can be used for non-addressing faults
        {
            get { return _defaultFaultAction; }
        }

        public override string ToString()
        {
            return string.Format(_toStringFormat, Namespace);
        }
    }
}
