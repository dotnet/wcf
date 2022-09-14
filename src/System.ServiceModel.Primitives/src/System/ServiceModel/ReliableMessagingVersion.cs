// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Xml;

namespace System.ServiceModel
{
    public abstract class ReliableMessagingVersion
    {
        // Do not initialize directly, this constructor is for derived classes.
        internal ReliableMessagingVersion(string ns, XmlDictionaryString dictionaryNs)
        {
            Namespace = ns;
            DictionaryNamespace = dictionaryNs;
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

        internal XmlDictionaryString DictionaryNamespace { get; }

        internal string Namespace { get; }

        internal static bool IsDefined(ReliableMessagingVersion reliableMessagingVersion)
        {
            return (reliableMessagingVersion == WSReliableMessaging11)
                || (reliableMessagingVersion == WSReliableMessagingFebruary2005);
        }
    }

    class WSReliableMessaging11Version : ReliableMessagingVersion
    {
        WSReliableMessaging11Version()
            : base(Wsrm11Strings.Namespace, DXD.Wsrm11Dictionary.Namespace)
        {
        }

        internal static ReliableMessagingVersion Instance { get; } = new WSReliableMessaging11Version();

        public override string ToString()
        {
            return "WSReliableMessaging11";
        }
    }

    class WSReliableMessagingFebruary2005Version : ReliableMessagingVersion
    {
        WSReliableMessagingFebruary2005Version()
            : base(WsrmFeb2005Strings.Namespace, XD.WsrmFeb2005Dictionary.Namespace)
        {
        }

        internal static ReliableMessagingVersion Instance { get; } = new WSReliableMessagingFebruary2005Version();

        public override string ToString()
        {
            return "WSReliableMessagingFebruary2005";
        }
    }
}
