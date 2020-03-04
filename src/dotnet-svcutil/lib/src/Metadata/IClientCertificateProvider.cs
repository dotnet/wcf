// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Tools.ServiceModel.Svcutil.Metadata
{
    public interface IClientCertificateProvider : ICloneable
    {
        X509Certificate GetCertificate(Uri serviceUri);
    }
}
