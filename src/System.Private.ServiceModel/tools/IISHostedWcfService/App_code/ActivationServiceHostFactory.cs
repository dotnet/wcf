// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !NET
using System;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace WcfService
{
    public class ActivationServiceHostFactory : ServiceHostFactory
    {
        public override ServiceHostBase CreateServiceHost(string constructorString, Uri[] baseAddresses)
        {
            return (ServiceHostBase)Activator.CreateInstance(Type.GetType(constructorString), baseAddresses);
        }
    }
}
#endif
