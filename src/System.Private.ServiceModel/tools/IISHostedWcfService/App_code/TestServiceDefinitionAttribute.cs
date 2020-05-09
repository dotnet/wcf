// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace WcfService
{
    public class TestServiceDefinitionAttribute : Attribute
    {
        public string BasePath { get; set; }
        public ServiceSchema Schema { get; set; }
    }

    public enum ServiceSchema
    {
        HTTP,
        HTTPS,
        NETTCP,
        WS,
        WSS
    }
}
