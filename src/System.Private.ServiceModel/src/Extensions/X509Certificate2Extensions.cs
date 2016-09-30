// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Cryptography.X509Certificates;

namespace System.ServiceModel
{
    internal static class X509Certificate2Extensions
    {
        // dotnet/wcf#1574 - This is a workaround for dotnet/corefx#12214
        // The copy constructor was removed in .NET Core, this is the workaround to allow us to make copies of certificates

        internal static X509Certificate2 CloneCertificateInternal(this X509Certificate2 certificateToClone)
        {
#if TARGETS_WINDOWS
            return new X509Certificate2(certificateToClone.Handle);
#else
            X509Certificate2Collection collection = new X509Certificate2Collection(certificateToClone);
            X509Certificate2Collection copyCollection = collection.Find(X509FindType.FindByThumbprint, certificateToClone.Thumbprint, false);
            return copyCollection[0];
#endif
        }

        // dotnet/wcf#1574 - This is a workaround for dotnet/corefx#12214
        internal static X509Certificate2 CloneCertificateInternal(this X509Certificate certificateToClone)
        {
#if TARGETS_WINDOWS
            return new X509Certificate2(certificateToClone.Handle);
#else
            byte[] certificateBytes = certificateToClone.Export(X509ContentType.Pfx, string.Empty);
            X509Certificate2 certificateToClone2 = new X509Certificate2(certificateBytes, string.Empty);

            X509Certificate2Collection collection = new X509Certificate2Collection(certificateToClone2);
            X509Certificate2Collection copyCollection = collection.Find(X509FindType.FindByThumbprint, certificateToClone2.Thumbprint, false);
            return copyCollection[0];
#endif
        }
    }
}