// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !NET
using System;
#endif

namespace WcfService
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class TestServiceDefinitionAttribute : Attribute
    {
        public string BasePath { get; set; }
        public ServiceSchema Schema { get; set; }
    }

    [Flags]
    public enum ServiceSchema
    {
        HTTP = 1,
        HTTPS = 2,
        NETTCP = 4,
        WS = 8,
        WSS = 16,
        NETPIPE = 32,
    }
}
