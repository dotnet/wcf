// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Net;

namespace Microsoft.Tools.ServiceModel.Svcutil.Metadata
{
    public interface IHttpCredentialsProvider : ICloneable
    {
        NetworkCredential GetCredentials(Uri serviceUri, WebException webException);
    }
}
