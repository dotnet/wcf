// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;

namespace Microsoft.Tools.ServiceModel.Svcutil.Metadata
{
    public interface IServerCertificateValidationProvider : ICloneable
    {
        void BeforeServerCertificateValidation(Uri serviceUri);

        void AfterServerCertificateValidation(Uri serviceUri);
    }
}
