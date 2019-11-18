// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.Text;
using Infrastructure.Common;
using Xunit;

public static class ClientBaseTest
{
    [WcfFact]
    public static void ClientBaseCloseMethodClosesCorrectly()
    {
        // *** SETUP *** \\
        BasicHttpBinding binding = new BasicHttpBinding();
        MyClientBase client = new MyClientBase(binding, new EndpointAddress("http://myendpoint"));

        // *** VALIDATE *** \\
        Assert.Equal(CommunicationState.Created, client.State);
        client.Open();
        Assert.Equal(CommunicationState.Opened, client.State);
        client.Close();
        Assert.Equal(CommunicationState.Closed, client.State);
    }

    public class MyClientBase : ClientBase<ITestService>
    {
        public MyClientBase(Binding binding, EndpointAddress endpointAddress)
            : base(binding, endpointAddress)
        {
        }

        public ITestService Proxy
        {
            get { return base.Channel; }
        }
    }

    [ServiceContract]
    public interface ITestService
    {
        [OperationContract]
        string Echo(string message);
    }
}
