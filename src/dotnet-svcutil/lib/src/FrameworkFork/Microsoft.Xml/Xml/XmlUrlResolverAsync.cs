// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// Not needed in dotnet-svcutil scenario. 
// using System.Runtime.Versioning;
// using System.Threading.Tasks;
// 
// namespace Microsoft.Xml {
// 				using System;
// 				
//     public partial class XmlUrlResolver : XmlResolver {
// 
//         // Maps a URI to an Object containing the actual resource.
//         // [ResourceConsumption(ResourceScope.Machine)]
//         // [ResourceExposure(ResourceScope.Machine)]
//         public override async Task<Object> GetEntityAsync(Uri absoluteUri, string role, Type ofObjectToReturn) {
//             if (ofObjectToReturn == null || ofObjectToReturn == typeof(System.IO.Stream) || ofObjectToReturn == typeof(System.Object)) {
//                 return await DownloadManager.GetStreamAsync(absoluteUri, _credentials, _proxy, _cachePolicy).ConfigureAwait(false);
//             }
//             else {
//                 throw new XmlException(Res.Xml_UnsupportedClass, string.Empty);
//             }
//         }
//     }
// }
