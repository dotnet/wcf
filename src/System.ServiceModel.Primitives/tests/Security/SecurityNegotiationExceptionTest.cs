// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using Infrastructure.Common;
using TestTypes;
using Xunit;

public static class SecurityNegotiationExceptionTest
{
    [WcfFact]
    public static void ValidateConstructors()
    {
        string exceptionMessage = "This is the exception message.";
        Exception innerException = new Exception(exceptionMessage);
        SecurityNegotiationException exception1 = new SecurityNegotiationException();

        SecurityNegotiationException exception2 = new SecurityNegotiationException(exceptionMessage);
        Assert.Equal(exceptionMessage, exception2.Message);

        SecurityNegotiationException exception3 = new SecurityNegotiationException(exceptionMessage, innerException);
        Assert.Equal(exceptionMessage, exception3.InnerException.Message);
    }
}
