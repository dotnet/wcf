//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;

namespace Microsoft.Tools.ServiceModel.Svcutil.Metadata
{
    public interface IServerCertificateValidationProvider : ICloneable
    {
        void BeforeServerCertificateValidation(Uri serviceUri);

        void AfterServerCertificateValidation(Uri serviceUri);
    }
}
