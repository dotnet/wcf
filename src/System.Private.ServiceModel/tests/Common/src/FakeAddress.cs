// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net;

public static class FakeAddress
{
    public static string HttpAddress = "http://" + IPAddress.None + ":0";
    
    public static string TcpAddress = "net.tcp://" + IPAddress.None + ":0";
}
