﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.Text;

namespace WcfProjectNService
{
    public class HttpsSoap11TestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            HttpsSoap11TestServiceHost serviceHost = new HttpsSoap11TestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }
    public class HttpsSoap11TestServiceHost : TestServiceHostBase<IWcfProjectNService>
    {
        protected override string Address { get { return "https-soap11"; } }

        protected override Binding GetBinding()
        {
            return new CustomBinding(new TextMessageEncodingBindingElement(MessageVersion.Soap11, Encoding.UTF8), new HttpsTransportBindingElement());
        }

        public HttpsSoap11TestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }
}
