// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Net;

namespace System.ServiceModel.Web
{
    internal interface IWebFaultException
    {
        HttpStatusCode StatusCode { get; }

        Type DetailType { get; }

        object DetailObject { get; }

        Type[] KnownTypes { get; }
    }
}
