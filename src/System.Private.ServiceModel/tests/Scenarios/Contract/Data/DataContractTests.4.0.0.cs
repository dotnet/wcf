// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using Infrastructure.Common;
using Xunit;

public static partial class DataContractTests
{
#if FULLXUNIT_NOTSUPPORTED
    [Fact]
#endif
    [WcfFact]
    [OuterLoop]
    public static void CustomBinding_DefaultSettings_Echo_RoundTrips_DataContract()
    {
        // Verifies a typed proxy can call a service operation echoing a DataContract object synchronously
        StringBuilder errorBuilder = new StringBuilder();
        try
        {
            CustomBinding customBinding = new CustomBinding();
            customBinding.Elements.Add(new TextMessageEncodingBindingElement());
            customBinding.Elements.Add(new HttpTransportBindingElement());

            // Note the service interface used.  It was manually generated with svcutil.
            ChannelFactory<IWcfService> factory = new ChannelFactory<IWcfService>(customBinding, new EndpointAddress(Endpoints.DefaultCustomHttp_Address));
            IWcfService serviceProxy = factory.CreateChannel();

            CompositeType request = new CompositeType() { StringValue = "myString", BoolValue = true };
            CompositeType response = serviceProxy.GetDataUsingDataContract(request);

            Assert.True(response != null, "GetDataUsingDataContract(request) returned null");

            string expectedStringValue = request.StringValue + "Suffix";
            if (!string.Equals(response.StringValue, expectedStringValue))
            {
                errorBuilder.AppendLine(string.Format("Expected CompositeType.StringValue \"{0}\", actual was \"{1}\"",
                                                        expectedStringValue, response.StringValue));
            }
            if (response.BoolValue != request.BoolValue)
            {
                errorBuilder.AppendLine(string.Format("Expected CompositeType.BoolValue \"{0}\", actual was \"{1}\"",
                                                        request.BoolValue, response.BoolValue));
            }

            factory.Close();
        }
        catch (Exception ex)
        {
            errorBuilder.AppendLine(String.Format("Unexpected exception was caught: {0}", ex.ToString()));
        }

        Assert.True(errorBuilder.Length == 0, errorBuilder.ToString());
    }
}
