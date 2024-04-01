// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using System.Net;

namespace WcfService
{
    public class ServiceHostHelper
    {
        public static bool Ping(string url)
        {
            Console.WriteLine("Ping service host...");
            for (int i = 0; i < 10; i++)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                try
                {
                    var response = request.GetResponse();
                    return true;
                }
                catch (WebException wex)
                {
                    Console.WriteLine("Ping failed... retrying after 3 seconds. " + wex.Message);
                    Task.Delay(3000).Wait();
                }
            }

            Console.WriteLine("Ping failed... Exiting..");
            return false;
        }
    }
}
#endif
