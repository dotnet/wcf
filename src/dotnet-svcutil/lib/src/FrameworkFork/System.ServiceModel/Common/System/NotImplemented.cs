// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// Not needed in dotnet-svcutil scenario. Use System.Runtime.Serialization
// namespace System
// {
//     //
//     // This silly looking class enables one to throw a NotImplementedException using the following
//     // idiom:
//     //
//     //     throw NotImplemented.ByDesign;
//     //
//     // Used by methods whose intended implementation is to throw a NotImplementedException (typically
//     // virtual methods in public abstract classes that intended to be subclassed by third parties.)
//     //
//     // This makes it distinguishable both from human eyes and CCI from NYI's that truly represent undone work.
//     //
//     internal static class NotImplemented
//     {
//         internal static Exception ByDesign
//         {
//             get
//             {
//                 return new NotImplementedException();
//             }
//         }
// 
//         internal static Exception ByDesignWithMessage(String message)
//         {
//             return new NotImplementedException(message);
//         }
//     }
// }

