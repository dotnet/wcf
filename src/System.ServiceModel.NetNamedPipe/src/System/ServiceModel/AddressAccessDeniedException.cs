// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime.CompilerServices;
using System.ServiceModel;

// AddressAccessDeniedException lived in this package through 8.x. As of
// 10.x the type's home is System.ServiceModel.Primitives so other
// transports (notably MSMQ) can throw it without taking a coupling on
// NetNamedPipe. The TypeForwardedTo keeps existing consumers that
// reference our NetNamedPipe surface seeing the same type identity.
[assembly: TypeForwardedTo(typeof(AddressAccessDeniedException))]

