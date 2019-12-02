// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading.Tasks;
using Infrastructure.Common;
using Xunit;

public static class OperationContractAttributeTest
{
    [WcfFact]
    public static void Default_Ctor_Initializes_Correctly()
    {
        OperationContractAttribute oca = new OperationContractAttribute();

        Assert.True(oca.IsInitiating, "IsInitiating should have been true");
        Assert.True(!oca.IsTerminating, "IsTerminating should have been false");
    }

    [WcfFact]
    public static void IsInitiating_Method_Requires_Proper_SessionMode()
    {
        var channelFactory = CreateChannelFactoryHelper<INonInitiatingNonTerminatingService>("net.tcp://localhost/dummy.svc");
        // The default ServiceContract's SessionMode.Allowed is not compatible with an OperationContract that has IsInitiating or IsTerminating
        var exception = Assert.Throws<InvalidOperationException>(() =>
        {
            channelFactory.CreateChannel();
        });
        Assert.Contains("INonInitiatingNonTerminatingService", exception.Message);
    }

    [WcfFact]
    public static void IsInitiating_And_IsTerminating_Method_Requires_Proper_SessionMode()
    {
        var channelFactory = CreateChannelFactoryHelper<IInitiatingTerminatingService>("net.tcp://localhost/dummy.svc");
        // We no longer ignore IsTerminating = true in our contract invariant checks 
        // so they will now fail if the ServiceContract's SessionMode isn't SessionMode.Required
        var exception = Assert.Throws<InvalidOperationException>(() =>
        {
            channelFactory.CreateChannel();
        });
        Assert.Contains("IInitiatingTerminatingService", exception.Message);
    }

    static ChannelFactory<T> CreateChannelFactoryHelper<T>(string url)
    {
        var helloEndpoint = new EndpointAddress(url);
        var binding = new NetTcpBinding();
        binding.Security = new NetTcpSecurity();
        binding.Security.Mode = SecurityMode.None;
        return new ChannelFactory<T>(binding, helloEndpoint);
    }
    [ServiceContract]
    public interface INonInitiatingNonTerminatingService
    {
        [OperationContract(IsInitiating = false, IsTerminating = false)]
        int MethodA(int a);
    }

    [ServiceContract]
    public interface IInitiatingTerminatingService
    {
        [OperationContract(IsInitiating = true, IsTerminating = true)]
        int MethodA(int a);
    }
}
