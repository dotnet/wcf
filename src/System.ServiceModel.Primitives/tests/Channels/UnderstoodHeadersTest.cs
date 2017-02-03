// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Infrastructure.Common;
using System;
using System.Collections;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Xunit;

public static class UnderstoodHeadersTest
{
    [WcfFact]
    public static void UnderstoodHeaders_Add_Contains_Remove_GetEnumerator()
    {
        // Create Message
        Message requestMessage = Message.CreateMessage(
            MessageVersion.Default,
            "http://tempuri.org/AnyService",
            new CustomBodyWriter("Any Message"));

        // Create additional message headers.
        MessageHeader customHeaderAlpha = MessageHeader.CreateHeader("AlphaHeader", "http://tempuri.org/AlphaHeaderNamespace", "AlphaObjectValue");
        MessageHeader customHeaderBravo = MessageHeader.CreateHeader("BravoHeader", "http://tempuri.org/BravoHeaderNamespace", "BravoObjectValue");

        // Add the new message headers to the Message
        requestMessage.Headers.Add(customHeaderAlpha);
        requestMessage.Headers.Add(customHeaderBravo);

        // Use the "UnderstoodHeaders.Contains" API to list which MessageHeaders are set as "UnderstoodHeaders"
        foreach (MessageHeaderInfo mhi in requestMessage.Headers)
        {
            // The MessageHeader with name "Action" is created with the Message object by default and should be MustUnderstand.
            if (String.Equals(mhi.Name, "Action")) Assert.True(mhi.MustUnderstand);
            // The two newly added headers should not have MustUnderstand set to true.
            if (String.Equals(mhi.Name, "AlphaHeader")) Assert.False(mhi.MustUnderstand);
            if (String.Equals(mhi.Name, "BravoHeader")) Assert.False(mhi.MustUnderstand);
        }

        // Use "UnderstoodHeaders.Add" API.
        requestMessage.Headers.UnderstoodHeaders.Add(customHeaderAlpha);
        requestMessage.Headers.UnderstoodHeaders.Add(customHeaderBravo);
        int numUnderstoodHeaders = requestMessage.Headers.UnderstoodHeaders.Count<MessageHeaderInfo>();
        Assert.Equal(3, numUnderstoodHeaders);
        
        // Use "UnderstoodHeaders.GetEnumerator" API.
        IEnumerator iEnum = requestMessage.Headers.UnderstoodHeaders.GetEnumerator();
        iEnum.Reset();
        while (iEnum.MoveNext())
        {
            // Verify we can iterate through the collection.
            MessageHeaderInfo mhi = (MessageHeaderInfo)iEnum.Current;
        }

        // Use "UnderstoodHeaders.IEnumerable.GetEnumerator"
        IEnumerator iEnum2 = ((IEnumerable)requestMessage.Headers.UnderstoodHeaders).GetEnumerator();
        iEnum2.Reset();
        while (iEnum2.MoveNext())
        {
            // Verify we can iterate through the collection.
            MessageHeaderInfo mhi = (MessageHeaderInfo)iEnum2.Current;
        }

        // Use "UnderstoodHeaders.Remove" API.
        foreach (MessageHeaderInfo mhi in requestMessage.Headers)
        {
            requestMessage.Headers.UnderstoodHeaders.Remove(mhi);
        }

        // Verify all MessageHeaders were removed from UnderstoodHeaders
        numUnderstoodHeaders = requestMessage.Headers.UnderstoodHeaders.Count<MessageHeaderInfo>();
        Assert.Equal(0, numUnderstoodHeaders);
    }
}
