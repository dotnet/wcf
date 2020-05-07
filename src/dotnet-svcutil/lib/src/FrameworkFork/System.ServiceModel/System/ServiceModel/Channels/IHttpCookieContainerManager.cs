// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net;

namespace System.ServiceModel.Channels
{
    /// <summary>
    /// Defines the interface used to provide access to an optional instance of CookieContainer that can be used to manage a collection of cookies.
    /// This interface was ported from Silverlight to allow this type to be part of the portable assembly.
    /// </summary>
    public interface IHttpCookieContainerManager
    {
        /// <summary>
        /// Gets or sets the CookieContainer object to be used. "Get" returns null if no CookieContainer object is to be used.
        /// </summary>
        CookieContainer CookieContainer { get; set; }
    }
}
