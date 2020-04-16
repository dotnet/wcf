// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Xml;

namespace System.ServiceModel.Security
{
    public abstract class TrustVersion
    {
        private readonly XmlDictionaryString _trustNamespace;
        private readonly XmlDictionaryString _prefix;

        internal TrustVersion(XmlDictionaryString ns, XmlDictionaryString prefix)
        {
            _trustNamespace = ns;
            _prefix = prefix;
        }

        public XmlDictionaryString Namespace
        {
            get
            {
                return _trustNamespace;
            }
        }

        public XmlDictionaryString Prefix
        {
            get
            {
                return _prefix;
            }
        }

        public static TrustVersion Default
        {
            get { return WSTrustFeb2005; }
        }

        public static TrustVersion WSTrustFeb2005
        {
            get { return WSTrustVersionFeb2005.Instance; }
        }

        public static TrustVersion WSTrust13
        {
            get { return WSTrustVersion13.Instance; }
        }

        internal class WSTrustVersionFeb2005 : TrustVersion
        {
            private static readonly WSTrustVersionFeb2005 s_instance = new WSTrustVersionFeb2005();

            protected WSTrustVersionFeb2005()
                : base(XD.TrustFeb2005Dictionary.Namespace, XD.TrustFeb2005Dictionary.Prefix)
            {
            }

            public static TrustVersion Instance
            {
                get
                {
                    return s_instance;
                }
            }
        }

        internal class WSTrustVersion13 : TrustVersion
        {
            private static readonly WSTrustVersion13 s_instance = new WSTrustVersion13();

            protected WSTrustVersion13()
                : base(DXD.TrustDec2005Dictionary.Namespace, DXD.TrustDec2005Dictionary.Prefix)
            {
            }

            public static TrustVersion Instance
            {
                get
                {
                    return s_instance;
                }
            }
        }
    }
}
