// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;

namespace WcfService
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using (ServiceHost host = new ServiceHost(
                typeof(WcfService)))
            {
                host.Open();
                Console.WriteLine("Service is ready!");
                Console.ReadKey();
            }
        }
    }
}
