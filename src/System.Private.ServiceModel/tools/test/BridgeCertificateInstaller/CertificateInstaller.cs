// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Infrastructure.Common;

public static class BridgeCertificateInstaller
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
            var directory = Path.GetDirectoryName(filename);
            try
            {
                Directory.CreateDirectory(directory);
            }
            catch (Exception ex)
            {
                Console.WriteLine("  An exception while creating the directory '{0}.'{1}  {2}",
                    directory,
                    Environment.NewLine,
                    ex.ToString());

                return -1;
            }

            try
            {
                File.WriteAllBytes(filename, certificateBytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine("  An exception while writing the file '{0}.'{1}  {2}",
                    filename,
                    Environment.NewLine,
                    ex.ToString());

                return -1;
            }

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

                if (response.IsSuccessStatusCode)
                {
                    certificateBytes = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
                    Console.WriteLine("    ... read {0} bytes from Bridge", certificateBytes.Length);
                    return true;
                }
                else
                {
                    Console.WriteLine("  Received an unexpected response from Bridge:{0}  HTTP Status code {1}: '{2}'",
                        Environment.NewLine,
                        response.StatusCode,
                        response.ReasonPhrase);
                    return false;
                }
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
        Console.WriteLine("  WCF for .NET Core - Linux test certificate installer tool");
        Console.WriteLine("  https://github.com/dotnet/wcf");
        Console.WriteLine();
        Console.WriteLine("  Makes a call to the Bridge to bootstrap the bridge and allow the certificate ");
        Console.WriteLine("  authority certificate to be acquired.");
        Console.WriteLine();
    }

    public static void DisplayUsage()
    {
        Console.WriteLine("  Required parameter missing.");
        Console.WriteLine("  BridgeCertificateInstaller [outputfile]");
        Console.WriteLine("  outputfile should be the desired output file for the root CA cert, e.g., ~/tmp/wcf-ca.crt");
    }
}
