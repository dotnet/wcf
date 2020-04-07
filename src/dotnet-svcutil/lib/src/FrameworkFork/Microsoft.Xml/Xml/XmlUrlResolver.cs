// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.Threading;
// using System.Security.Permissions;
using System.Net;
//using System.Net.Cache;
using System.Runtime.Versioning;

namespace Microsoft.Xml {
				using System;
				

    // Resolves external XML resources named by a Uniform Resource Identifier (URI).
    public partial class XmlUrlResolver : XmlResolver {
        //private static object s_DownloadManager;
        private ICredentials _credentials;
        private IWebProxy _proxy;

        // Construction

        // Creates a new instance of the XmlUrlResolver class.
        public XmlUrlResolver() {
        }

        public override ICredentials Credentials {
            set { _credentials = value; }
        }

        public IWebProxy Proxy {
            set { _proxy = value; }
        }

        // Resource resolution

        // Maps a URI to an Object containing the actual resource.
        // [ResourceConsumption(ResourceScope.Machine)]
        // [ResourceExposure(ResourceScope.Machine)]
        public override Object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn) {
            throw new NotImplementedException();
        }

        // [PermissionSetAttribute(SecurityAction.InheritanceDemand, Name = "FullTrust")]
        // [ResourceConsumption(ResourceScope.Machine)]
        // [ResourceExposure(ResourceScope.Machine)]
        public override Uri ResolveUri(Uri baseUri, string relativeUri){
            return base.ResolveUri(baseUri, relativeUri);
        }
    }
}
