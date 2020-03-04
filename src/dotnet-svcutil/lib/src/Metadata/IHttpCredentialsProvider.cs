// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Net;

namespace Microsoft.Tools.ServiceModel.Svcutil.Metadata
{
    public interface IHttpCredentialsProvider : ICloneable
    {
        NetworkCredential GetCredentials(Uri serviceUri, WebException webException);
    }
}
