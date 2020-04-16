// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Description
{
    using System.Globalization;
    using System.ServiceModel.Channels;
    using System.ServiceModel;
    using System.Collections.Generic;

    public static class MetadataExchangeBindings
    {
        private static Binding s_httpBinding;
        private static Binding s_httpGetBinding;
        private static Binding s_httpsBinding;
        private static Binding s_httpsGetBinding;
        private static Binding s_tcpBinding;
        private static Binding s_pipeBinding;

        internal static Binding Http
        {
            get
            {
                // don't need to lock because no guarantee of instance identity
                if (s_httpBinding == null)
                {
                    s_httpBinding = CreateHttpBinding();
                }

                return s_httpBinding;
            }
        }

        internal static Binding HttpGet
        {
            get
            {
                // don't need to lock because no guarantee of instance identity
                if (s_httpGetBinding == null)
                {
                    s_httpGetBinding = CreateHttpGetBinding();
                }

                return s_httpGetBinding;
            }
        }

        internal static Binding Https
        {
            get
            {
                // don't need to lock because no guarantee of instance identity
                if (s_httpsBinding == null)
                {
                    s_httpsBinding = CreateHttpsBinding();
                }

                return s_httpsBinding;
            }
        }

        internal static Binding HttpsGet
        {
            get
            {
                // don't need to lock because no guarantee of instance identity
                if (s_httpsGetBinding == null)
                {
                    s_httpsGetBinding = CreateHttpsGetBinding();
                }

                return s_httpsGetBinding;
            }
        }

        internal static Binding Tcp
        {
            get
            {
                // don't need to lock because no guarantee of instance identity
                if (s_tcpBinding == null)
                {
                    s_tcpBinding = CreateTcpBinding();
                }

                return s_tcpBinding;
            }
        }

        internal static Binding NamedPipe
        {
            get
            {
                // don't need to lock because no guarantee of instance identity
                if (s_pipeBinding == null)
                {
                    s_pipeBinding = CreateNamedPipeBinding();
                }

                return s_pipeBinding;
            }
        }

        public static Binding CreateMexHttpBinding()
        {
            return MetadataExchangeBindings.CreateHttpBinding();
        }

        public static Binding CreateMexHttpsBinding()
        {
            return MetadataExchangeBindings.CreateHttpsBinding();
        }

        public static Binding CreateMexTcpBinding()
        {
            return MetadataExchangeBindings.CreateTcpBinding();
        }

        public static Binding CreateMexNamedPipeBinding()
        {
            return MetadataExchangeBindings.CreateNamedPipeBinding();
        }

        internal static Binding GetBindingForScheme(string scheme)
        {
            Binding binding = null;
            TryGetBindingForScheme(scheme, out binding);
            return binding;
        }

        internal static bool TryGetBindingForScheme(string scheme, out Binding binding)
        {
            if (String.Compare(scheme, "http", StringComparison.OrdinalIgnoreCase) == 0)
            {
                binding = Http;
            }
            else if (String.Compare(scheme, "https", StringComparison.OrdinalIgnoreCase) == 0)
            {
                binding = Https;
            }
            else if (String.Compare(scheme, "net.tcp", StringComparison.OrdinalIgnoreCase) == 0)
            {
                binding = Tcp;
            }
            else if (String.Compare(scheme, "net.pipe", StringComparison.OrdinalIgnoreCase) == 0)
            {
                binding = NamedPipe;
            }
            else
            {
                binding = null;
            }

            return binding != null;
        }

        private static WSHttpBinding CreateHttpBinding()
        {
            WSHttpBinding binding = new WSHttpBinding(SecurityMode.None, reliableSessionEnabled: false);
            binding.Name = MetadataStrings.MetadataExchangeStrings.HttpBindingName;
            binding.Namespace = MetadataStrings.MetadataExchangeStrings.BindingNamespace;
            return binding;
        }

        private static WSHttpBinding CreateHttpsBinding()
        {
            WSHttpBinding binding = new WSHttpBinding(
                new WSHttpSecurity(SecurityMode.Transport, new HttpTransportSecurity(), new NonDualMessageSecurityOverHttp()), reliableSessionEnabled: false);
            binding.Name = MetadataStrings.MetadataExchangeStrings.HttpsBindingName;
            binding.Namespace = MetadataStrings.MetadataExchangeStrings.BindingNamespace;

            return binding;
        }

        private static CustomBinding CreateHttpGetBinding()
        {
            return CreateGetBinding(new HttpTransportBindingElement());
        }

        private static CustomBinding CreateHttpsGetBinding()
        {
            return CreateGetBinding(new HttpsTransportBindingElement());
        }

        private static CustomBinding CreateGetBinding(HttpTransportBindingElement httpTransport)
        {
            TextMessageEncodingBindingElement textEncoding = new TextMessageEncodingBindingElement();
            textEncoding.MessageVersion = MessageVersion.None;
            httpTransport.Method = "GET";
            httpTransport.InheritBaseAddressSettings = true;
            return new CustomBinding(textEncoding, httpTransport);
        }

        private static CustomBinding CreateTcpBinding()
        {
            CustomBinding binding = new CustomBinding(MetadataStrings.MetadataExchangeStrings.TcpBindingName, MetadataStrings.MetadataExchangeStrings.BindingNamespace);
            TcpTransportBindingElement tcpTransport = new TcpTransportBindingElement();
            binding.Elements.Add(tcpTransport);
            return binding;
        }

        private static CustomBinding CreateNamedPipeBinding()
        {
            throw new NotImplementedException();
        }

        internal static bool IsSchemeSupported(string scheme)
        {
            Binding binding;
            return TryGetBindingForScheme(scheme, out binding);
        }
    }
}
