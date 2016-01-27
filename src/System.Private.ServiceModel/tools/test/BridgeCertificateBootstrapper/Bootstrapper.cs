// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Infrastructure.Common;

public static class BridgeCertificateBootstrapper
{
    public static int Main(string[] args)
    {
        DisplayBanner(); 

        if (args.Length < 1)
        {
            DisplayUsage();
            return -1;
        }

        var filename = args[0];

        byte[] certificateBytes; 
        if (GetCertificate(out certificateBytes))
        {
            File.WriteAllBytes(filename, certificateBytes);
            return 0;
        }

        Console.WriteLine();
        return -1; 
    }

    public static bool GetCertificate(out byte[] certificateBytes)
    {
        string endpoint = Endpoints.Https_DefaultBinding_Address;
        string bridgePort = TestProperties.GetProperty(TestProperties.BridgePort_PropertyName);
        
        certificateBytes = default(byte[]);

        Uri uri;
        try
        {
            if (Uri.TryCreate(endpoint, UriKind.RelativeOrAbsolute, out uri))
            {
                string certAsPemUri = string.Format(
                    "http://{0}:{1}/resource/WcfService.CertificateResources.CertificateAuthorityResource?exportAsPem=true", uri.Host, bridgePort);

                Console.WriteLine("  Bridge hostname is: '{0}'", uri.Host);
                Console.WriteLine("  Retrieving certificate from:");
                
                Console.WriteLine(certAsPemUri);
                Console.WriteLine();

                HttpClient client = new HttpClient();
                var response = client.GetAsync(certAsPemUri).GetAwaiter().GetResult();

                certificateBytes = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();

                Console.WriteLine("   ... success");
                return true; 
            }
            else
            {
                Console.WriteLine("  The Bridge uri specified: '{0}' is an invalid uri", endpoint);
                certificateBytes = null;
                return false; 
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("  There was an exception thrown while accessing the Bridge at '{0}'", endpoint);
            Console.WriteLine();
            Console.WriteLine(ex);
        }

        return false; 
    }

    public static void DisplayBanner()
    {
        Console.WriteLine("  WCF for .NET Core - Linux certificate test bootstrapper tool");
        Console.WriteLine("  https://github.com/dotnet/wcf");
        Console.WriteLine();
        Console.WriteLine("  Makes a call to the Bridge to bootstrap the bridge and allow the certificate ");
        Console.WriteLine("  authority certificate to be acquired.");
        Console.WriteLine();
    }

    public static void DisplayUsage()
    {
        Console.WriteLine("  Required parameter missing.");
        Console.WriteLine("  BridgeCertificateBootstrapper [outputfile]");
        Console.WriteLine("  outputfile should be the desired output file for the root CA cert, e.g., ~/tmp/wcf-ca.crt");
    }
}
