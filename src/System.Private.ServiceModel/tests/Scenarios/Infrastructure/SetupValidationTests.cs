// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using Infrastructure.Common;
using System;
using Xunit;

public class SetupValidationTests : ConditionalWcfTest
{
    [ConditionalFact(nameof(Root_Certificate_Installed))]
    [OuterLoop]
    public static void Root_Certificate_Correctly_Installed()
    {
        // *** SETUP *** \\
        InvalidOperationException exception = null;

        // *** EXECUTE *** \\
        try
        {
            ServiceUtilHelper.EnsureRootCertificateInstalled();
        }
        catch (InvalidOperationException e)
        {
            exception = e;
        }

        // *** VALIDATE *** \\
        // Validate rather than allowing an exception to propagate
        // to be clear the exception was anticipated. 
        Assert.True(exception == null, exception == null ? String.Empty : exception.ToString());
    }

    [OuterLoop]
    [ConditionalFact(nameof(Client_Certificate_Installed))]
    public static void Client_Certificate_Correctly_Installed()
    {
        // *** SETUP *** \\
        InvalidOperationException exception = null;

        // *** EXECUTE *** \\
        try
        {
            ServiceUtilHelper.EnsureClientCertificateInstalled();
        }
        catch (InvalidOperationException e)
        {
            exception = e;
        }

        // *** VALIDATE *** \\
        // Validate rather than allowing an exception to propagate
        // to be clear the exception was anticipated. 
        Assert.True(exception == null, exception == null ? String.Empty : exception.ToString());
    }
}
