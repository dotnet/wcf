// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF;
using CoreWCF.Channels;
using CoreWCF.Web;
#else
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
#endif

namespace WcfService
{
    // ---- Contract --------------------------------------------------------
    // Shared between server (CoreWCF on .NET, full .NET FX on net4xx via IIS)
    // and client (this repo's new System.ServiceModel.Web). The attribute
    // parameter names match across CoreWCF.Web.WebGet/WebInvoke and
    // System.ServiceModel.Web.WebGet/WebInvoke, so a single contract works
    // for both.
    [ServiceContract]
    public interface IWcfWebHttpService
    {
        [OperationContract]
        [WebGet(UriTemplate = "EchoWithGet?message={message}",
                BodyStyle = WebMessageBodyStyle.Bare,
                ResponseFormat = WebMessageFormat.Xml)]
        string EchoWithGet(string message);

        [OperationContract]
        [WebGet(UriTemplate = "EchoWithGetJson?message={message}",
                BodyStyle = WebMessageBodyStyle.Bare,
                ResponseFormat = WebMessageFormat.Json)]
        string EchoWithGetJson(string message);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "EchoWithPost",
                   BodyStyle = WebMessageBodyStyle.Bare,
                   ResponseFormat = WebMessageFormat.Xml,
                   RequestFormat = WebMessageFormat.Xml)]
        string EchoWithPost(string message);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "EchoWithPostJson",
                   BodyStyle = WebMessageBodyStyle.Bare,
                   ResponseFormat = WebMessageFormat.Json,
                   RequestFormat = WebMessageFormat.Json)]
        string EchoWithPostJson(string message);

        [OperationContract]
        [WebGet(UriTemplate = "EchoWithGetPath/{message}",
                BodyStyle = WebMessageBodyStyle.Bare,
                ResponseFormat = WebMessageFormat.Xml)]
        string EchoWithGetPath(string message);
    }

    // ---- Implementation --------------------------------------------------
    public class WcfWebHttpService : IWcfWebHttpService
    {
        public string EchoWithGet(string message)      => message;
        public string EchoWithGetJson(string message)  => message;
        public string EchoWithPost(string message)     => message;
        public string EchoWithPostJson(string message) => message;
        public string EchoWithGetPath(string message)  => message;
    }

    // ---- Test Host -------------------------------------------------------
    //
    // Registers a CoreWCF (or IIS .NET FX) REST endpoint at:
    //   HTTP  -> http://<base>:8081/WebHttp.svc/<UriTemplate>
    //
    // On the CoreWCF (#if NET) path, TestDefinitionHelper.StartHosts() already
    // calls services.AddServiceModelWebServices(), so the WebHttpServiceBehavior
    // in CoreWCF.WebHttp will automatically install a WebHttpBehavior on this
    // endpoint when it sees the WebMessageEncodingBindingElement that the
    // WebHttpBinding produces.
    //
    // On the .NET FX path we need to install WebHttpBehavior manually since
    // TestServiceHostBase does not.
    [TestServiceDefinition(BasePath = "WebHttp.svc", Schema = ServiceSchema.HTTP)]
    public class WebHttpTestServiceHost : TestServiceHostBase<IWcfWebHttpService>
    {
        protected override string Address => "";

        protected override Binding GetBinding() => new WebHttpBinding();

        public WebHttpTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WcfWebHttpService), baseAddresses) { }

#if !NET
        protected override void ApplyConfiguration()
        {
            base.ApplyConfiguration();
            foreach (var ep in Description.Endpoints)
            {
                if (ep.Binding is WebHttpBinding && ep.Behaviors.Find<System.ServiceModel.Description.WebHttpBehavior>() == null)
                {
                    ep.Behaviors.Add(new System.ServiceModel.Description.WebHttpBehavior());
                }
            }
        }
#endif
    }
}
