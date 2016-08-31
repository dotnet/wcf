// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.IdentityModel.Selectors;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Security;
using System.Text;

namespace WcfService
{
    public class WsHttpTransSecUserNameTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            WsHttpTranSecUserNameTestServiceHost serviceHost = new WsHttpTranSecUserNameTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }
    public class WsHttpTranSecUserNameTestServiceHost : TestServiceHostBase<IWcfCustomUserNameService>
    {
        protected override string Address { get { return "wshttp-transec-username"; } }

        protected override Binding GetBinding()
        {
            var securityBindingElement = SecurityBindingElement.CreateUserNameOverTransportBindingElement();
            securityBindingElement.IncludeTimestamp = false;
            securityBindingElement.SecurityHeaderLayout = SecurityHeaderLayout.Lax;

            CustomBinding binding = new CustomBinding(
                            new TextMessageEncodingBindingElement(MessageVersion.Soap11, Encoding.UTF8),
                            securityBindingElement,
                            new HttpsTransportBindingElement());
            return binding;
        }

        public WsHttpTranSecUserNameTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }

        protected override void ApplyConfiguration()
        {
            base.ApplyConfiguration();

            ServiceCredentials serviceCredentials = new ServiceCredentials();
            serviceCredentials.UserNameAuthentication.UserNamePasswordValidationMode = UserNamePasswordValidationMode.Custom;
            serviceCredentials.UserNameAuthentication.CustomUserNamePasswordValidator = new CustomUserNameValidator();
            this.Description.Behaviors.Add(serviceCredentials);
        }

        //private class CustomUserNameValidator : UserNamePasswordValidator
        //{
        //    // This method validates users. It allows in two users, test1 and test2 
        //    // with passwords 1tset and 2tset respectively.
        //    // This code is for illustration purposes only and 
        //    // must not be used in a production environment because it is not secure.	
        //    public override void Validate(string userName, string password)
        //    {
        //        if (null == userName || null == password)
        //        {
        //            throw new ArgumentNullException();
        //        }

        //        if (!(userName == "someUser" && password == "somePassword"))
        //        {
        //            // This throws an informative fault to the client.
        //            throw new FaultException("Unknown Username or Incorrect Password");
        //            // When you do not want to throw an infomative fault to the client,
        //            // throw the following exception.
        //            // throw new SecurityTokenException("Unknown Username or Incorrect Password");
        //        }
        //    }
        //}
    }
}
