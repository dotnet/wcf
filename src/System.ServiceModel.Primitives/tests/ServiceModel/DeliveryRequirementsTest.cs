// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using Infrastructure.Common;
using Xunit;

public static class DeliveryRequirementsTest
{
    [WcfFact]
    public static void OrderedRequired_OrderedSession_NoThrow()
    {
        var binding = new WSHttpBinding(SecurityMode.None, true);
        binding.ReliableSession.Ordered = true;
        EndpointAddress endpointAddress = new EndpointAddress(FakeAddress.HttpAddress);
        var factory = new ChannelFactory<IReliableOrderedRequired>(binding, endpointAddress);
        factory.Open();
    }

    [WcfFact]
    public static void OrderedRequired_UnorderedSession_Throws()
    {
        var binding = new WSHttpBinding(SecurityMode.None, true);
        binding.ReliableSession.Ordered = false;
        EndpointAddress endpointAddress = new EndpointAddress(FakeAddress.HttpAddress);
        var factory = new ChannelFactory<IReliableOrderedRequired>(binding, endpointAddress);
        Assert.Throws<InvalidOperationException>(() => factory.Open());
    }

    [WcfFact]
    public static void OrderedNotRequired_OrderedSession_NoThrow()
    {
        var binding = new WSHttpBinding(SecurityMode.None, true);
        binding.ReliableSession.Ordered = true;
        EndpointAddress endpointAddress = new EndpointAddress(FakeAddress.HttpAddress);
        var factory = new ChannelFactory<IReliableOrderedNotRequired>(binding, endpointAddress);
        factory.Open();
    }

    [WcfFact]
    public static void OrderedNotRequired_UnorderedSession_NoThrow()
    {
        var binding = new WSHttpBinding(SecurityMode.None, true);
        binding.ReliableSession.Ordered = true;
        EndpointAddress endpointAddress = new EndpointAddress(FakeAddress.HttpAddress);
        var factory = new ChannelFactory<IReliableOrderedNotRequired>(binding, endpointAddress);
        factory.Open();
    }

    [WcfFact]
    public static void QueuedDeliveryRequired_Throws()
    {
        var binding = new WSHttpBinding(SecurityMode.None, true);
        binding.ReliableSession.Ordered = false;
        EndpointAddress endpointAddress = new EndpointAddress(FakeAddress.HttpAddress);
        var factory = new ChannelFactory<IReliableOrderedRequired>(binding, endpointAddress);
        Assert.Throws<InvalidOperationException>(() => factory.Open());
    }

    [WcfFact]
    public static void BindingDeliveryCapabilitiesMissing_Throws()
    {
        var binding = new BasicHttpBinding();
        EndpointAddress endpointAddress = new EndpointAddress(FakeAddress.HttpAddress);
        var factory = new ChannelFactory<IReliableOrderedRequired>(binding, endpointAddress);
        Assert.Throws<InvalidOperationException>(() => factory.Open());
    }

    [DeliveryRequirements(QueuedDeliveryRequirements = QueuedDeliveryRequirementsMode.Allowed, RequireOrderedDelivery = true)]
    [ServiceContract]
    public interface IReliableOrderedRequired
    {
        [OperationContract]
        void DoWork();
    }

    [DeliveryRequirements(QueuedDeliveryRequirements = QueuedDeliveryRequirementsMode.Allowed, RequireOrderedDelivery = false)]
    [ServiceContract]
    public interface IReliableOrderedNotRequired
    {
        [OperationContract]
        void DoWork();
    }

    [DeliveryRequirements(QueuedDeliveryRequirements = QueuedDeliveryRequirementsMode.Required, RequireOrderedDelivery = false)]
    [ServiceContract]
    public interface IReliableQueuedDeliveryRequired
    {
        [OperationContract]
        void DoWork();
    }
}
