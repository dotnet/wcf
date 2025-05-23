// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using Infrastructure.Common;
using Xunit;

public partial class DataContractTests
{
    [WcfFact]
    [OuterLoop]
    public static void CustomBinding_DefaultSettings_Echo_RoundTrips_DataContract()
    {
        // Verifies a typed proxy can call a service operation echoing a DataContract object synchronously
        CustomBinding customBinding = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        EndpointAddress endpointAddress = null;
        CompositeType request = null;
        CompositeType response = null;

        try
        {
            // *** SETUP *** \\
            customBinding = new CustomBinding();
            customBinding.Elements.Add(new TextMessageEncodingBindingElement());
            customBinding.Elements.Add(new HttpTransportBindingElement());

            // Note the service interface used.  It was manually generated with svcutil.
            endpointAddress = new EndpointAddress(Endpoints.DefaultCustomHttp_Address);
            factory = new ChannelFactory<IWcfService>(customBinding, endpointAddress);
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            request = new CompositeType() { StringValue = "myString", BoolValue = true };
            response = serviceProxy.GetDataUsingDataContract(request);

            // *** VALIDATE *** \\
            Assert.True(response != null, "GetDataUsingDataContract(request) returned null");
            string expectedStringValue = request.StringValue + "Suffix";
            Assert.True(String.Equals(response.StringValue, expectedStringValue), String.Format("Expected CompositeType.StringValue \"{0}\", actual was \"{1}\"",
                                                        expectedStringValue, response.StringValue));
            Assert.True(response.BoolValue == request.BoolValue, String.Format("Expected CompositeType.BoolValue \"{0}\", actual was \"{1}\"",
                                                        request.BoolValue, response.BoolValue));
            // *** CLEANUP *** \\
            factory.Close();
            ((ICommunicationObject)serviceProxy).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }
}
