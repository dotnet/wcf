// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Security.Cryptography.X509Certificates;
using Infrastructure.Common;
using Xunit;

public class SetupValidationTests : ConditionalWcfTest
{
#if FULLXUNIT_NOTSUPPORTED
    [Fact]
#endif
    [WcfFact]
    [Condition(nameof(Root_Certificate_Installed))]
    [Issue(1347, OS = OSID.AnyUnix)]
    [OuterLoop]
    public static void Root_Certificate_Correctly_Installed()
    {
#if FULLXUNIT_NOTSUPPORTED
        bool root_Certificate_Installed = Root_Certificate_Installed();
        if (!root_Certificate_Installed)
        {
            Console.WriteLine("---- Test SKIPPED --------------");
            Console.WriteLine("Attempting to run the test in ToF, a ConditionalFact evaluated as FALSE.");
            Console.WriteLine("Root_Certificate_Installed evaluated as {0}", root_Certificate_Installed);
            return;
        }
#endif

        // *** SETUP *** \\
        InvalidOperationException exception = null;

        // *** EXECUTE *** \\
        try
        {
            ValidateCertificate(ServiceUtilHelper.RootCertificate, StoreName.Root, ServiceUtilHelper.PlatformSpecificRootStoreLocation);
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

#if FULLXUNIT_NOTSUPPORTED
    [Fact]
#endif
    [WcfFact]
    [Condition(nameof(Client_Certificate_Installed))]
    [OuterLoop]
    public static void Client_Certificate_Correctly_Installed()
    {
#if FULLXUNIT_NOTSUPPORTED
        bool client_Certificate_Installed = Client_Certificate_Installed();
        if (!client_Certificate_Installed)
        {
            Console.WriteLine("---- Test SKIPPED --------------");
            Console.WriteLine("Attempting to run the test in ToF, a ConditionalFact evaluated as FALSE.");
            Console.WriteLine("Client_Certificate_Installed evaluated as {0}", client_Certificate_Installed);
            return;
        }
#endif
        // *** SETUP *** \\
        InvalidOperationException exception = null;

        // *** EXECUTE *** \\
        try
        {
            ValidateCertificate(ServiceUtilHelper.ClientCertificate, StoreName.My, StoreLocation.CurrentUser);
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

#if FULLXUNIT_NOTSUPPORTED
    [Fact]
#endif
    [WcfFact]
    [Condition(nameof(Peer_Certificate_Installed))]
    [OuterLoop]
    public static void Peer_Certificate_Correctly_Installed()
    {
#if FULLXUNIT_NOTSUPPORTED
        bool peer_Certificate_Installed = Peer_Certificate_Installed();
        if (!peer_Certificate_Installed)
        {
            Console.WriteLine("---- Test SKIPPED --------------");
            Console.WriteLine("Attempting to run the test in ToF, a ConditionalFact evaluated as FALSE.");
            Console.WriteLine("Peer_Certificate_Installed evaluated as {0}", peer_Certificate_Installed);
            return;
        }
#endif
        // *** SETUP *** \\
        InvalidOperationException exception = null;

        // *** EXECUTE *** \\
        try
        {
            ValidateCertificate(ServiceUtilHelper.PeerCertificate, StoreName.TrustedPeople, StoreLocation.CurrentUser);
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

    private static void ValidateCertificate(X509Certificate2 certificate, StoreName storeName, StoreLocation storeLocation)
    {
        Assert.True(certificate != null, "Certificate is null");

        DateTime now = DateTime.Now;
        Assert.True(now > certificate.NotBefore,
                   String.Format("The current date {{0}} is earlier than NotBefore ({1})",
                                 now,
                                 certificate.NotBefore));

        Assert.True(now < certificate.NotAfter,
           String.Format("The current date {{0}} is later than NotAfter ({1})",
                         now,
                         certificate.NotAfter));

        using (X509Store store = new X509Store(storeName, storeLocation))
        {
            store.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection certificates = store.Certificates.Find(X509FindType.FindByThumbprint, certificate.Thumbprint, validOnly: true);
            Assert.True(certificates.Count == 1,
                        String.Format("Did not find valid certificate with thumbprint {0} in StoreName '{1}', StoreLocation '{2}'",
                                      certificate.Thumbprint,
                                      storeName,
                                      storeLocation));
        }

        using (X509Store store = new X509Store(StoreName.Disallowed, storeLocation))
        {
            store.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection certificates = store.Certificates.Find(X509FindType.FindByThumbprint, certificate.Thumbprint, validOnly: false);
            Assert.True(certificates.Count == 0, "Certificate was found in Disallowed store.");
        }
    }
}
