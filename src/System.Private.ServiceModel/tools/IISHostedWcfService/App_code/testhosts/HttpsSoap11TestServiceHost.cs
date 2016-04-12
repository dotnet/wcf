//  Copyright (c) Microsoft Corporation.  All Rights Reserved.
using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.Text;

namespace WcfService
{
    public class HttpsSoap11TestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            HttpsSoap11TestServiceHost serviceHost = new HttpsSoap11TestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }
    public class HttpsSoap11TestServiceHost : TestServiceHostBase<IWcfService>
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
