// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using System.Security.Cryptography.X509Certificates;

using Infrastructure.Common;

public static class ServiceUtilHelper
{
    private static X509Certificate2 s_rootCert;
    private static X509Certificate2 s_clientCert;
    private static object s_rootCertLock = new object();
    private static object s_clientCertLock = new object();
    private static string s_serviceHostName = string.Empty;

    public static string LocalCertThumbprint { get; set; }

    //Install Root certificate, this is required for all https test
    public static void EnsureRootCertificateInstalled()
    {
        lock (s_rootCertLock)
        {
            if (s_rootCert == null)
            {
                BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
                ChannelFactory<IUtil> factory = null;
                IUtil serviceProxy = null;
                try
                {
                    factory = new ChannelFactory<IUtil>(basicHttpBinding, new EndpointAddress(ServiceUtil_Address));
                    serviceProxy = factory.CreateChannel();
                    s_rootCert = new X509Certificate2(serviceProxy.GetRootCert(false));

                    if (s_rootCert == null)
                    {
                        //throw
                        throw new Exception("Failed to obtain root cert from the server");
                    }

                    BridgeClientCertificateManager.InstallCertificateToRootStore(s_rootCert);
                }
                finally
                {
                    CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
                }
            }
        }
    }

    //Install client certificate
    public static void EnsureLocalClientCertificateInstalled()
    {
        EnsureRootCertificateInstalled();
        lock (s_clientCertLock)
        {
            if (s_clientCert == null)
            {
                BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
                ChannelFactory<IUtil> factory = null;
                IUtil serviceProxy = null;
                try
                {
                    factory = new ChannelFactory<IUtil>(basicHttpBinding, new EndpointAddress(ServiceUtil_Address));
                    serviceProxy = factory.CreateChannel();
                    s_clientCert = new X509Certificate2(serviceProxy.GetClientCert(false), "test", X509KeyStorageFlags.PersistKeySet);
                    if (s_clientCert == null)
                    {
                        //throw
                        throw new Exception("Failed to obtain client cert from the server");
                    }

                    BridgeClientCertificateManager.AddToStoreIfNeeded(StoreName.My, StoreLocation.CurrentUser, s_clientCert);
                    LocalCertThumbprint = s_clientCert.Thumbprint;
                }
                finally
                {
                    CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
                }
            }
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

    public static string ServiceHostName
    {
        get
        {
            //Get the host name from server
            if (string.IsNullOrEmpty(s_serviceHostName))
            {
                BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
                ChannelFactory<IUtil> factory = null;
                IUtil serviceProxy = null;
                try
                {
                    factory = new ChannelFactory<IUtil>(basicHttpBinding, new EndpointAddress(ServiceUtil_Address));
                    serviceProxy = factory.CreateChannel();
                    s_serviceHostName = serviceProxy.GetFQDN();
                }
                finally
                {
                    CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
                }
            }

            return s_serviceHostName;
        }
    }

    private static bool IISHosted
    {
        get
        {
            // We assume the self hosted service does not have test service base addess, only the host name passed
            // This will satisfy all current requirements
            if (TestProperties.GetProperty(TestProperties.ServiceUri_PropertyName).Contains("/"))
            {
                return true;
            };

            return false;
        }
    }

    private static Uri BuildBaseUri(string protocol)
    {
        var builder = new UriBuilder();
        builder.Host = TestProperties.GetProperty(TestProperties.ServiceUri_PropertyName);
        builder.Scheme = protocol;

        if (!IISHosted)
        {
            switch (protocol)
            {
                case "http":
                    builder.Port = int.Parse(TestProperties.GetProperty(TestProperties.BridgeHttpPort_PropertyName));
                    break;
                case "ws":
                    builder.Port = int.Parse(TestProperties.GetProperty(TestProperties.BridgeWebSocketPort_PropertyName));
                    builder.Scheme = "http";
                    break;
                case "https":
                    builder.Port = int.Parse(TestProperties.GetProperty(TestProperties.BridgeHttpsPort_PropertyName));
                    break;
                case "wss":
                    builder.Port = int.Parse(TestProperties.GetProperty(TestProperties.BridgeSecureWebSocketPort_PropertyName));
                    builder.Scheme = "https";
                    break;
                case "net.tcp":
                    builder.Port = int.Parse(TestProperties.GetProperty(TestProperties.BridgeTcpPort_PropertyName));
                    break;
                default:
                    break;
            }
        }

        return builder.Uri;
    }
    public static string GetEndpointAddress(string endpoint, string protocol = "http")
    {
        return string.Format(@"{0}/{1}", BuildBaseUri(protocol), endpoint);
    }

    public static string ServiceUtil_Address
    {
        get { return GetEndpointAddress("Util.svc//Util"); }
    }
}

