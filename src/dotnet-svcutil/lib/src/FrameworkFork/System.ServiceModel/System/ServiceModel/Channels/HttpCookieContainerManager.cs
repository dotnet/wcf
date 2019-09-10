// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net;

namespace System.ServiceModel.Channels
{
    /// <summary>
    /// Implementation of the IHttpCookieContainerManager
    /// </summary>
    internal class HttpCookieContainerManager : IHttpCookieContainerManager
    {
        private CookieContainer _cookieContainer;

        // We need this flag to avoid overriding the CookieConatiner if the user has already initialized it.
        public bool IsInitialized { get; private set; }

        public CookieContainer CookieContainer
        {
            get
            {
                return _cookieContainer;
            }

            set
            {
                this.IsInitialized = true;
                _cookieContainer = value;
            }
        }
    }
}
