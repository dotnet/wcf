// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using Infrastructure.Common;
using System.Net.Http;
using System.Threading.Tasks;

public static class ServiceUtilHelper
{
    private const string ClientCertificateSubject = "WCF Client Certificate";
    private const string CertificateIssuer = "DO_NOT_TRUST_WcfBridgeRootCA";

    private const string TestHostUtilitiesService = "TestHost.svc";
    private const string ClientCertificateResource = "ClientCert";
    private const string MachineCertificateResource = "MachineCert";
    private const string CrlResource = "Crl";
    private const string PeerCertificateResource = "PeerCert";
    private const string RootCertificateResource = "RootCert";
    private const string FqdnResource = "Fqdn";
    private const string PingResource = "Ping";
    private const string StateResource = "State";

    private static object s_certLock = new object();
    private static string s_serviceHostName = string.Empty;
    private static bool s_rootCertAvailabilityChecked = false;
    private static bool s_clientCertAvailabilityChecked = false;
    private static bool s_peerCertAvailabilityChecked = false;
    private static bool s_osXPeerCertAvailabilityChecked = false;
    private static X509Certificate2 s_rootCertificate = null;
    private static X509Certificate2 s_clientCertificate = null;
    private static X509Certificate2 s_peerCertificate = null;
    private static string s_rootCertInstallErrorMessage = null;
    private static string s_clientCertInstallErrorMessage = null;
    private static string s_peerCertInstallErrorMessage = null;
    private static string s_osXKeychainCertInstallErrorMessage = null;

    public static X509Certificate2 RootCertificate
    {
        get
        {
            // When running under VS, the Condition checks are run in a different process than the test run
            // which means the test running process won't have the certificate installed by the condition check code.
            EnsureRootCertificateInstalled();
            ThrowIfRootCertificateInstallationError();
            return s_rootCertificate;
        }
    }

    public static X509Certificate2 ClientCertificate
    {
        get
        {
            // When running under VS, the Condition checks are run in a different process than the test run
            // which means the test running process won't have the certificate installed by the condition check code.
            EnsureClientCertificateInstalled();
            ThrowIfClientCertificateInstallationError();
            return s_clientCertificate;
        }
    }

    public static X509Certificate2 PeerCertificate
    {
        get
        {
            // When running under VS, the Condition checks are run in a different process than the test run
            // which means the test running process won't have the certificate installed by the condition check code.
            EnsurePeerCertificateInstalled();
            ThrowIfPeerCertificateInstallationError();
            return s_peerCertificate;
        }
    }

    public static StoreLocation PlatformSpecificRootStoreLocation
    {
        get { return CertificateManager.PlatformSpecificRootStoreLocation; }
    }

    public static string OSXCustomKeychainPassword => CertificateManager.OSXCustomKeychainPassword;

    public static string OSXCustomKeychainFilePath => CertificateManager.OSXCustomKeychainFilePath;

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
                            rootCertificate = CertificateManager.RootCertificateFromThumprint(thumbprint, validOnly: false);
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
        X509Certificate2 rootCertificate = new X509Certificate2(GetResourceFromServiceAsByteArray(RootCertificateResource));
        return CertificateManager.InstallCertificateToRootStore(rootCertificate);
    }

    public static async Task<X509Certificate2> GetServiceMacineCertFromServerAsync()
    {
        return new X509Certificate2(await GetResourceFromServiceAsByteArrayAsync(MachineCertificateResource));
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
                            clientCertificate = CertificateManager.ClientCertificateFromThumprint(thumbprint, validOnly: false);
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

    // Tries to ensure that the peer trust certificate is installed into
    // a local OSX keychain.  It obtains the certificate from the service
    // utility endpoint and either installs it or verifies that a matching
    // one is already installed.  InvalidOperationException will be thrown
    // if an error occurred attempting to install the certificate. This
    // method may be called multiple times but will attempt the installation
    // once only.
    public static void EnsureOSXKeychainCertificateInstalled()
    {
        if (!s_osXPeerCertAvailabilityChecked)
        {
            lock (s_certLock)
            {
                if (!s_osXPeerCertAvailabilityChecked)
                {
                    X509Certificate2 peerCertificate = null;
                    string thumbprint = null;

                    try
                    {
                        // Once only, we interrogate the service utility endpoint
                        // for the server certificate and install it locally if it
                        // is not already in the store.
                        peerCertificate = InstallOSXPeerCertificateFromServer();

                        // If we had a certificate from the service endpoint, verify it was installed
                        // by retrieving it from the store by thumbprint.
                        if (peerCertificate != null)
                        {
                            thumbprint = peerCertificate.Thumbprint;
                            peerCertificate = CertificateManager.OSXLocalKeychainCertificateFromThumprint(thumbprint, validOnly: false);
                            if (peerCertificate == null)
                            {
                                s_clientCertInstallErrorMessage =
                                    $"Failed to find a server certificate matching thumbprint '{thumbprint}'";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        s_osXKeychainCertInstallErrorMessage = ex.ToString();
                    }

                    s_peerCertificate = peerCertificate;
                    s_osXPeerCertAvailabilityChecked = true;
                }
            }
        }

        // If the installation failed, throw an exception everytime
        // this method is called.
        ThrowIfOSXKeychainCertificateInstallationError();
    }

    // Tries to ensure that the peer trust certificate is installed into
    // the TrustedPeople store.  It obtains the certificate from the service
    // utility endpoint and either installs it or verifies that a matching
    // one is already installed.  InvalidOperationException will be thrown
    // if an error occurred attempting to install the certificate.  This
    // method may be called multiple times but will attempt the installation
    // once only.
    public static void EnsurePeerCertificateInstalled()
    {
        if (!s_peerCertAvailabilityChecked)
        {
            lock (s_certLock)
            {
                if (!s_peerCertAvailabilityChecked)
                {
                    X509Certificate2 peerCertificate = null;
                    string thumbprint = null;

                    try
                    {
                        // Once only, we interrogate the service utility endpoint
                        // for the server certificate and install it locally if it
                        // is not already in the store.
                        peerCertificate = InstallPeerCertificateFromServer();

                        // If we had a certificate from the service endpoint, verify it was installed
                        // by retrieving it from the store by thumbprint.
                        if (peerCertificate != null)
                        {
                            thumbprint = peerCertificate.Thumbprint;
                            peerCertificate = CertificateManager.PeerCertificateFromThumprint(thumbprint, validOnly: false);
                            if (peerCertificate != null)
                            {
                                System.Console.WriteLine(String.Format("Using peer trust certificate:{0}{1}",
                                                        Environment.NewLine, peerCertificate));
                            }
                            else
                            {
                                s_clientCertInstallErrorMessage =
                                    String.Format("Failed to find a server certificate matching thumbprint '{0}'", thumbprint);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        s_peerCertInstallErrorMessage = ex.ToString();
                    }

                    s_peerCertificate = peerCertificate;
                    s_peerCertAvailabilityChecked = true;
                }
            }
        }

        // If the installation failed, throw an exception everytime
        // this method is called.
        ThrowIfPeerCertificateInstallationError();
    }

    // Acquires a client certificate from the service utility endpoint and
    // attempts to install it into the store.  All failures are
    // propagated back to the caller.
    private static X509Certificate2 InstallClientCertificateFromServer()
    {
        X509KeyStorageFlags storageFlags = X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.UserKeySet;
        if((OSHelper.Current & OSID.OSX) == OSHelper.Current)
        {
            // The PersistKeySet flag causes an exception on OSX when constructing an X509Certificate2 object.
            // The UserKeySet flag is meaningless on OSX. The Exportable flag means the private certificate
            // can be retrieved from the X509Certificate2 instance which is needed to be able to store the
            // certificate in the user keychain.
            storageFlags = X509KeyStorageFlags.Exportable;
        }

        X509Certificate2 clientCertificate = new X509Certificate2(GetResourceFromServiceAsByteArray(ClientCertificateResource), "test", storageFlags);
        return CertificateManager.InstallCertificateToMyStore(clientCertificate);
    }

    // Acquires the peer trust certificate from the service utility endpoint and
    // attempts to install it into the our custom keychain store.  All failures are
    // propagated back to the caller.
    private static X509Certificate2 InstallOSXPeerCertificateFromServer()
    {
        X509Certificate2 peerCertificate = new X509Certificate2(GetResourceFromServiceAsByteArray(PeerCertificateResource), "test", X509KeyStorageFlags.DefaultKeySet);
        return CertificateManager.InstallCertificateToOSXKeychainStore(peerCertificate);
    }

    // Acquires the peer trust certificate from the service utility endpoint and
    // attempts to install it into the TrustedPeople store.  All failures are
    // propagated back to the caller.
    private static X509Certificate2 InstallPeerCertificateFromServer()
    {
        X509Certificate2 peerCertificate = new X509Certificate2(GetResourceFromServiceAsByteArray(PeerCertificateResource), "test", X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.UserKeySet);
        return CertificateManager.InstallCertificateToTrustedPeopleStore(peerCertificate);
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

    private static void ThrowIfPeerCertificateInstallationError()
    {
        if (s_peerCertInstallErrorMessage != null)
        {
            throw new InvalidOperationException(
                String.Format("The peer certificate could not be installed: {0}", s_peerCertInstallErrorMessage));
        }
    }

    private static void ThrowIfOSXKeychainCertificateInstallationError()
    {
        if(s_osXKeychainCertInstallErrorMessage != null)
        {
            throw new InvalidOperationException(
                String.Format("The OSX local Keychain certificate could not be installed: {0}", s_osXKeychainCertInstallErrorMessage));
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
                s_serviceHostName = GetResourceFromServiceAsString(FqdnResource);
            }

            return s_serviceHostName;
        }
    }

    public static bool IISHosted
    {
        get
        {
            // We assume the self hosted service does not have test service base address, only the host name passed
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
        try
        {
            builder.Host = TestProperties.GetProperty(TestProperties.ServiceUri_PropertyName);
            builder.Scheme = protocol;

            if (!IISHosted)
            {
                switch (protocol)
                {
                    case "http":
                        builder.Port = int.Parse(TestProperties.GetProperty(TestProperties.ServiceHttpPort_PropertyName));
                        break;
                    case "ws":
                        builder.Port = int.Parse(TestProperties.GetProperty(TestProperties.ServiceWebSocketPort_PropertyName));
                        builder.Scheme = "http";
                        break;
                    case "https":
                        builder.Port = int.Parse(TestProperties.GetProperty(TestProperties.ServiceHttpsPort_PropertyName));
                        break;
                    case "wss":
                        builder.Port = int.Parse(TestProperties.GetProperty(TestProperties.ServiceSecureWebSocketPort_PropertyName));
                        builder.Scheme = "https";
                        break;
                    case "net.tcp":
                        builder.Port = int.Parse(TestProperties.GetProperty(TestProperties.ServiceTcpPort_PropertyName));
                        break;
                    case "net.pipe":
                        // No port number used with named pipes, so do nothing
                        break;
                    default:
                        break;
                }
            }

            return builder.Uri;
        }
        catch (UriFormatException ufe)
        {
            throw new UriFormatException($"UriBuilder didn't like parsing {builder.ToString()}", ufe);
        }
    }

    public static string GetEndpointAddress(string endpoint, string protocol = "http")
    {
        return string.Format(@"{0}{1}", BuildBaseUri(protocol), endpoint);
    }

    private static string GetResourceAddress(string resource, string protocol = "http")
    {
        var baseUri = BuildBaseUri(protocol);
        return new Uri(baseUri, $"{TestHostUtilitiesService}/{resource}").ToString();
    }

    public static string GetResourceFromServiceAsString(string resource)
    {
        string requestUri = GetResourceAddress(resource);
        Console.WriteLine(String.Format("Invoking {0} ...", requestUri));

        using (HttpClient httpClient = new HttpClient())
        {
            HttpResponseMessage response = httpClient.GetAsync(requestUri).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
            else
            {
                throw new Exception($"Got Status code {response.StatusCode} from {requestUri}.");
            }
        }
    }

    public static byte[] GetResourceFromServiceAsByteArray(string resource)
    {
        string requestUri = GetResourceAddress(resource);
        Console.WriteLine(String.Format("Invoking {0} ...", requestUri));

        using (HttpClient httpClient = new HttpClient())
        {
            HttpResponseMessage response = httpClient.GetAsync(requestUri).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
            }
            else
            {
                throw new Exception($"Got Status code {response.StatusCode} from {requestUri}.");
            }
        }
    }

    public static async Task<byte[]> GetResourceFromServiceAsByteArrayAsync(string resource)
    {
        string requestUri = GetResourceAddress(resource);
        Console.WriteLine(String.Format("Invoking {0} ...", requestUri));

        using (HttpClient httpClient = new HttpClient())
        {
            HttpResponseMessage response = await httpClient.GetAsync(requestUri);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
            else
            {
                throw new Exception($"Got Status code {response.StatusCode} from {requestUri}.");
            }
        }
    }
}
