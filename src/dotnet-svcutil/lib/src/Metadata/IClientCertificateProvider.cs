// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Tools.ServiceModel.Svcutil.Metadata
{
    public interface IClientCertificateProvider : ICloneable
    {
        X509Certificate GetCertificate(Uri serviceUri);
    }
}
