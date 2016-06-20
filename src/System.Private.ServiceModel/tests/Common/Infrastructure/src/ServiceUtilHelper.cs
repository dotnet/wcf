// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using System.Security.Cryptography.X509Certificates;

using Infrastructure.Common;

public static class ServiceUtilHelper
{
    private const string ClientCertificateSubject = "WCF Client Certificate";
    private const string CertificateIssuer = "DO_NOT_TRUST_WcfBridgeRootCA";

    private static object s_certLock = new object();
    private static string s_serviceHostName = string.Empty;
    private static bool s_rootCertAvailabilityChecked = false;
    private static bool s_clientCertAvailabilityChecked = false;
    private static X509Certificate2 s_rootCertificate = null;
    private static X509Certificate2 s_clientCertificate = null;
    private static string s_rootCertInstallErrorMessage = null;
    private static string s_clientCertInstallErrorMessage = null;

    public static X509Certificate2 RootCertificate
    {
        get
        {
            ThrowIfRootCertificateInstallationError();
            return s_rootCertificate;
        }
    }

    public static X509Certificate2 ClientCertificate
    {
        get
        {
            ThrowIfClientCertificateInstallationError();
            return s_clientCertificate;
        }
    }

    // Tries to ensure that the root certificate is installed into
    // the root store.  It obtains the root certificate from the service
    // utility endpoint and either installs it or verifies that a matching
    // one is already installed.  InvalidOperationException will be thrown
    // if an error occurred attempting to install the certificate.  This
    // method may be called multiple times but will attempt the installation
    // once only.
    public static void EnsureRootCertificateInstalled()
    {
        if (!s_rootCertAvailabilityChecked)
        {
            lock (s_certLock)
            {
                if (!s_rootCertAvailabilityChecked)
                {
                    X509Certificate2 rootCertificate = null;
                    string thumbprint = null;

                    try
                    {
                        // Once only, we interrogate the service utility endpoint
                        // for the root certificate and install it locally if it
                        // is not already in the store.
                        rootCertificate = InstallRootCertificateFromServer();

                        // If we had a certificate from the service endpoint, verify it was installed
                        // by retrieving it from the store by thumbprint.
                        if (rootCertificate != null)
                        {
                            thumbprint = rootCertificate.Thumbprint;
                            rootCertificate = CertificateManager.RootCertificateFromThumprint(thumbprint);
                            if (rootCertificate != null)
                            {
                                System.Console.WriteLine(String.Format("Using root certificate:{0}{1}",
                                                        Environment.NewLine, rootCertificate));
                            }
                            else
                            {
                                s_rootCertInstallErrorMessage =
                                    String.Format("Failed to find a root certificate matching thumbprint '{0}'", thumbprint);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        s_rootCertInstallErrorMessage = ex.ToString();
                    }

                    s_rootCertificate = rootCertificate;
                    s_rootCertAvailabilityChecked = true;
                }
            }
        }

        // If the installation failed, throw an exception everytime
        // this method is called.
        ThrowIfRootCertificateInstallationError();
    }

    // Acquires a root certificate from the service utility endpoint and
    // attempts to install it into the root store.  It returns the
    // certificate in the store, which may include one that was already
    // installed.  Exceptions are propagated to the caller.
    private static X509Certificate2 InstallRootCertificateFromServer()
    {
        X509Certificate2 rootCertificate = null;
        BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
        ChannelFactory<IUtil> factory = null;
        IUtil serviceProxy = null;
        try
        {
            factory = new ChannelFactory<IUtil>(basicHttpBinding, new EndpointAddress(ServiceUtil_Address));
            serviceProxy = factory.CreateChannel();
            rootCertificate = new X509Certificate2(serviceProxy.GetRootCert(false));
            rootCertificate = CertificateManager.InstallCertificateToRootStore(rootCertificate);
        }
        finally
        {
            CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }

        return rootCertificate;
    }

    // Tries to ensure that the client certificate is installed into
    // the local store.  It obtains the client certificate from the service
    // utility endpoint and either installs it or verifies that a matching
    // one is already installed.  InvalidOperationException will be thrown
    // if an error occurred attempting to install the certificate.  This
    // method may be called multiple times but will attempt the installation
    // once only.
    public static void EnsureClientCertificateInstalled()
    {
        if (!s_clientCertAvailabilityChecked)
        {
            lock (s_certLock)
            {
                if (!s_clientCertAvailabilityChecked)
                {
                    X509Certificate2 clientCertificate = null;
                    string thumbprint = null;

                    // To be valid, the client certificate also requires the root certificate
                    // to be installed.  But even if the root certificate installation fails,
                    // it is still possible to verify or install the client certificate for
                    // scenarios that don't require chain validation.
                    try
                    {
                        EnsureRootCertificateInstalled();
                    }
                    catch
                    {
                        // Exceptions installing the root certificate are captured and
                        // will be reported if it is requested.  But allow the attempt
                        // to install the client certificate to succeed or fail independently.
                    }

                    try
                    {
                        // Once only, we interrogate the service utility endpoint
                        // for the client certificate and install it locally if it
                        // is not already in the store.
                        clientCertificate = InstallClientCertificateFromServer();

                        // If we had a certificate from the service endpoint, verify it was installed
                        // by retrieving it from the store by thumbprint.
                        if (clientCertificate != null)
                        {
                            thumbprint = clientCertificate.Thumbprint;
                            clientCertificate = CertificateManager.ClientCertificateFromThumprint(thumbprint);
                            if (clientCertificate != null)
                            {
                                System.Console.WriteLine(String.Format("Using client certificate:{0}{1}",
                                                        Environment.NewLine, clientCertificate));
                            }
                            else
                            {
                                s_clientCertInstallErrorMessage =
                                    String.Format("Failed to find a client certificate matching thumbprint '{0}'", thumbprint);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        s_clientCertInstallErrorMessage = ex.ToString();
                    }

                    s_clientCertificate = clientCertificate;
                    s_clientCertAvailabilityChecked = true;
                }
            }
        }

        // If the installation failed, throw an exception everytime
        // this method is called.
        ThrowIfClientCertificateInstallationError();
    }

    // Acquires a client certificate from the service utility endpoint and
    // attempts to install it into the store.  All failures are
    // propagated back to the caller.
    private static X509Certificate2 InstallClientCertificateFromServer()
    {
        X509Certificate2 clientCertificate = null;
        BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
        ChannelFactory<IUtil> factory = null;
        IUtil serviceProxy = null;
        try
        {
            factory = new ChannelFactory<IUtil>(basicHttpBinding, new EndpointAddress(ServiceUtil_Address));
            serviceProxy = factory.CreateChannel();
            byte[] certdata = serviceProxy.GetClientCert(false);
            clientCertificate = new X509Certificate2(certdata, "test", X509KeyStorageFlags.PersistKeySet);
            clientCertificate = CertificateManager.InstallCertificateToMyStore(clientCertificate);
        }
        finally
        {
            CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }

        return clientCertificate;
    }

    private static void ThrowIfRootCertificateInstallationError()
    {
        if (s_rootCertInstallErrorMessage != null)
        {
            throw new InvalidOperationException(
                String.Format("The root certificate could not be installed: {0}", s_rootCertInstallErrorMessage));
        }
    }

    private static void ThrowIfClientCertificateInstallationError()
    {
        if (s_clientCertInstallErrorMessage != null)
        {
            throw new InvalidOperationException(
                String.Format("The client certificate could not be installed: {0}", s_clientCertInstallErrorMessage));
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

    public static bool IISHosted
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
