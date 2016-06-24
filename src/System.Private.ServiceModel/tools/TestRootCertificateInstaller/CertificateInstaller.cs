// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Threading.Tasks;
using Infrastructure.Common;

public static class TestRootCertificateInstaller
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
        certificateBytes = default(byte[]);
        string address = ServiceUtilHelper.ServiceUtil_Address;
        Console.WriteLine("  Retrieving certificate from:" + address);
        ChannelFactory<IUtil> factory = null;
        IUtil serviceProxy = null;
        BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
        try
        {
            factory = new ChannelFactory<IUtil>(basicHttpBinding, new EndpointAddress(address));
            serviceProxy = factory.CreateChannel();
            certificateBytes = serviceProxy.GetRootCert(true);
            Console.WriteLine("    ... read {0} bytes from Bridge", certificateBytes.Length);
            return true;
        }
        finally
        {
            CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    private static void CloseCommunicationObjects(params ICommunicationObject[] objects)
    {
        foreach (ICommunicationObject comObj in objects)
        {
            try
            {
                if (comObj == null)
                {
                    continue;
                }
                // Only want to call Close if it is in the Opened state
                if (comObj.State == CommunicationState.Opened)
                {
                    comObj.Close();
                }
                // Anything not closed by this point should be aborted
                if (comObj.State != CommunicationState.Closed)
                {
                    comObj.Abort();
                }
            }
            catch (TimeoutException)
            {
                comObj.Abort();
            }
            catch (CommunicationException)
            {
                comObj.Abort();
            }
        }
    }

    public static void DisplayBanner()
    {
        Console.WriteLine("  WCF for .NET Core - Linux test certificate installer tool");
        Console.WriteLine("  https://github.com/dotnet/wcf");
        Console.WriteLine();
        Console.WriteLine("  Makes a call to the service to bootstrap the service and allow the root certificate ");
        Console.WriteLine("  authority certificate to be acquired.");
        Console.WriteLine();
    }

    public static void DisplayUsage()
    {
        Console.WriteLine("  Required parameter missing.");
        Console.WriteLine("  TestRootCertificateInstaller [outputfile]");
        Console.WriteLine("  outputfile should be the desired output file for the root CA cert, e.g., ~/tmp/wcf-ca.crt");
    }
}
