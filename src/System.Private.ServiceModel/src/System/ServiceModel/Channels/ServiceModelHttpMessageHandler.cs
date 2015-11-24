// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    public partial class ServiceModelHttpMessageHandler : DelegatingHandler
    {
        public DecompressionMethods AutomaticDecompression
        {
            get { return _innerHandler.AutomaticDecompression; }
            set { _innerHandler.AutomaticDecompression = value; }
        }

        public CookieContainer CookieContainer
        {
            get { return _innerHandler.CookieContainer; }
            set { _innerHandler.CookieContainer = value; }
        }

        public int MaxAutomaticRedirections
        {
            get { return _innerHandler.MaxAutomaticRedirections; }
            set { _innerHandler.MaxAutomaticRedirections = value; }
        }

        public bool PreAuthenticate
        {
            get { return _innerHandler.PreAuthenticate; }
            set { _innerHandler.PreAuthenticate = value; }
        }

        public IWebProxy Proxy
        {
            get { return _innerHandler.Proxy; }
            set { _innerHandler.Proxy = value; }
        }
    }
}
