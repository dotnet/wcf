// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Net;

namespace System.ServiceModel.Channels
{
    /// <summary>
    /// Implementation of the IHttpCookieContainerManager
    /// </summary>
    internal class HttpCookieContainerManager : IHttpCookieContainerManager
    {
        private CookieContainer _cookieContainer;

        // We need this flag to avoid overriding the CookieContainer if the user has already initialized it.
        public bool IsInitialized { get; private set; }

        public CookieContainer CookieContainer
        {
            get
            {
                return _cookieContainer;
            }

            set
            {
                IsInitialized = true;
                _cookieContainer = value;
            }
        }
    }
}
