// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Net;

public static class FakeAddress
{
    public static string HttpAddress = "http://" + IPAddress.None + ":0";
    
    public static string HttpsAddress = "https://" + IPAddress.None + ":0";

    public static string TcpAddress = "net.tcp://" + IPAddress.None + ":0";
}
