// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.IO;
using Microsoft.Xml;
using System.Threading.Tasks;

namespace Microsoft.Xml.Resolvers {

    // 
    // XmlPreloadedResolver is an XmlResolver that which can be pre-loaded with data.
    // By default it contains well-known DTDs for XHTML 1.0 and RSS 0.91. 
    // Custom mappings of URIs to data can be added with the Add method.
    //
    public partial class XmlPreloadedResolver : XmlResolver {

        public override Task<Object> GetEntityAsync(Uri absoluteUri,
                                             string role,
                                             Type ofObjectToReturn) {

            if (absoluteUri == null) {
                throw new ArgumentNullException("absoluteUri");
            }

            PreloadedData data;
            if (!mappings.TryGetValue(absoluteUri, out data)) {
                if (fallbackResolver != null) {
                    return fallbackResolver.GetEntityAsync(absoluteUri, role, ofObjectToReturn);
                }
                throw new XmlException(ResXml.GetString(ResXml.Xml_CannotResolveUrl, absoluteUri.ToString()));
            }

            if (ofObjectToReturn == null || ofObjectToReturn == typeof(Stream) || ofObjectToReturn == typeof(Object)) {
                return Task.FromResult<Object>(data.AsStream());
            }
            else if (ofObjectToReturn == typeof(TextReader)) {
                return Task.FromResult<Object>(data.AsTextReader());
            }
            else {
                throw new XmlException(ResXml.GetString(ResXml.Xml_UnsupportedClass));
            }
        }
    }
}
