// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel
{
    using System.ComponentModel;
    using Microsoft.Xml;

    //TODO: [TypeConverter(typeof(ReliableMessagingVersionConverter))]
    public abstract class ReliableMessagingVersion
    {
        private XmlDictionaryString _dictionaryNs;
        private string _ns;

        // Do not initialize directly, this constructor is for derived classes.
        internal ReliableMessagingVersion(string ns, XmlDictionaryString dictionaryNs)
        {
            _ns = ns;
            _dictionaryNs = dictionaryNs;
        }

        public static ReliableMessagingVersion Default
        {
            get { return System.ServiceModel.Channels.ReliableSessionDefaults.ReliableMessagingVersion; }
        }

        public static ReliableMessagingVersion WSReliableMessaging11
        {
            get { return WSReliableMessaging11Version.Instance; }
        }

        public static ReliableMessagingVersion WSReliableMessagingFebruary2005
        {
            get { return WSReliableMessagingFebruary2005Version.Instance; }
        }

        internal XmlDictionaryString DictionaryNamespace
        {
            get { return _dictionaryNs; }
        }

        internal string Namespace
        {
            get { return _ns; }
        }

        internal static bool IsDefined(ReliableMessagingVersion reliableMessagingVersion)
        {
            return (reliableMessagingVersion == WSReliableMessaging11)
                || (reliableMessagingVersion == WSReliableMessagingFebruary2005);
        }
    }

    internal class WSReliableMessaging11Version : ReliableMessagingVersion
    {
        private static ReliableMessagingVersion s_instance = new WSReliableMessaging11Version();

        private WSReliableMessaging11Version()
            : base(Wsrm11Strings.Namespace, DXD.Wsrm11Dictionary.Namespace)
        {
        }

        internal static ReliableMessagingVersion Instance
        {
            get { return s_instance; }
        }

        public override string ToString()
        {
            return "WSReliableMessaging11";
        }
    }

    internal class WSReliableMessagingFebruary2005Version : ReliableMessagingVersion
    {
        private WSReliableMessagingFebruary2005Version()
            : base(WsrmFeb2005Strings.Namespace, XD.WsrmFeb2005Dictionary.Namespace)
        {
        }

        private static ReliableMessagingVersion s_instance = new WSReliableMessagingFebruary2005Version();

        internal static ReliableMessagingVersion Instance
        {
            get { return s_instance; }
        }

        public override string ToString()
        {
            return "WSReliableMessagingFebruary2005";
        }
    }
}
