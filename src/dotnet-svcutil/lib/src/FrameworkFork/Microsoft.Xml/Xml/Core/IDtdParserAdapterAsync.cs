// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// Not needed in dotnet-svcutil scenario. 
// 
// using System;
// using System.Text;
// #if !SILVERLIGHT
// using Microsoft.Xml.Schema;
// #endif
// 
// #if SILVERLIGHT
// using BufferBuilder=Microsoft.Xml.BufferBuilder;
// #else
// using BufferBuilder=System.Text.StringBuilder;
// #endif
// 
// using System.Threading.Tasks;
// 
// namespace Microsoft.Xml {
// 				using System;
// 				
// 
//     internal partial interface IDtdParserAdapter {
// 
//         Task< int > ReadDataAsync();
// 
//         Task< int > ParseNumericCharRefAsync( BufferBuilder internalSubsetBuilder );
//         Task< int > ParseNamedCharRefAsync( bool expand, BufferBuilder internalSubsetBuilder );
//         Task ParsePIAsync( BufferBuilder sb );
//         Task ParseCommentAsync( BufferBuilder sb );
// 
//         Task< Tuple<int,bool> > PushEntityAsync( IDtdEntityInfo entity);
// 
//         Task< bool > PushExternalSubsetAsync( string systemId, string publicId );
// 
//     }
// 
// }
