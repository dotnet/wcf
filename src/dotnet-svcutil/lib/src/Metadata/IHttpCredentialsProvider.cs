//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Net;

namespace Microsoft.Tools.ServiceModel.Svcutil.Metadata
{
    public interface IHttpCredentialsProvider : ICloneable
    {
        NetworkCredential GetCredentials(Uri serviceUri, WebException webException);
    }
}
