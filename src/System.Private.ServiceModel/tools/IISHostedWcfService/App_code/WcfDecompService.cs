// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF;
using CoreWCF.Channels;
#else
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;
#endif
using System.Net;

namespace WcfService
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class WcfDecompService : IWcfDecompService
    {    
        public bool IsDecompressionEnabled()
        {
            MessageProperties properties = new MessageProperties(OperationContext.Current.IncomingMessageProperties);
            var property = (HttpRequestMessageProperty)properties[HttpRequestMessageProperty.Name];
            WebHeaderCollection collection = property.Headers;
            string acceptEncodingValue = collection.Get("Accept-Encoding");
            bool result = false;
            if (!string.IsNullOrEmpty(acceptEncodingValue))
            {
                result = acceptEncodingValue.IndexOf("gzip", StringComparison.OrdinalIgnoreCase) >= 0 || acceptEncodingValue.IndexOf("deflate", StringComparison.OrdinalIgnoreCase) >= 0;
            }

            return result;
        }
    }
}
