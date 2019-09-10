//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Tools.ServiceModel.Svcutil.Metadata
{
    public interface IClientCertificateProvider : ICloneable
    {
        X509Certificate GetCertificate(Uri serviceUri);
    }
}
