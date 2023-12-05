// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Threading.Tasks;
using Binding.UDS.IntegrationTests;
using Binding.UDS.IntegrationTests.ServiceContract;
using CoreWCF.Configuration;
using Infrastructure.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

public partial class Binding_UDSBindingTests : ConditionalWcfTest
{
    // Simple echo of a string using NetTcpBinding on both client and server with SecurityMode=None
    [WcfFact]
    [OuterLoop]
    public static void SecurityModeNone_Echo_RoundTrips_String()
    {

        string testString = new string('a', 3000);
        IHost host = ServiceHelper.CreateWebHostBuilder<StartUpForUDS>(UDS.GetUDSFilePath());
        using (host)
        {
            System.ServiceModel.ChannelFactory<IEchoService> factory = null;
            IEchoService serviceProxy = null;
            host.Start();
            try
            {
                System.ServiceModel.UnixDomainSocketBinding binding = new UnixDomainSocketBinding(UnixDomainSocketSecurityMode.None);
                var uriBuilder = new UriBuilder()
                {
                    Scheme = "net.uds",
                    Path = UDS.GetUDSFilePath()
                };
                factory = new System.ServiceModel.ChannelFactory<IEchoService>(binding,
                    new System.ServiceModel.EndpointAddress(uriBuilder.ToString()));
                serviceProxy = factory.CreateChannel();
                ((IChannel)serviceProxy).Open();
                string result = serviceProxy.Echo(testString);
                Assert.Equal(testString, result);
                ((IChannel)serviceProxy).Close();
                factory.Close();
            }
            finally
            {
                ServiceHelper.CloseServiceModelObjects((IChannel)serviceProxy, factory);
            }
        }
    }

    [WcfFact]
    [OuterLoop]
    [Condition(nameof(Windows_Authentication_Available),
              nameof(WindowsOrSelfHosted))]
    public void WindowsAuth()
    {
        string testString = new string('a', 3000);
        IHost host = ServiceHelper.CreateWebHostBuilder<StartupForWindowsAuth>(UDS.GetUDSFilePath());
        using (host)
        {
            System.ServiceModel.ChannelFactory<IEchoService> factory = null;
            IEchoService channel = null;
            host.Start();
            try
            {
                System.ServiceModel.UnixDomainSocketBinding binding = new UnixDomainSocketBinding(System.ServiceModel.UnixDomainSocketSecurityMode.Transport);
                binding.Security.Transport.ClientCredentialType = System.ServiceModel.UnixDomainSocketClientCredentialType.Windows;

                var uriBuilder = new UriBuilder()
                {
                    Scheme = "net.uds",
                    Path = UDS.GetUDSFilePath()
                };
                factory = new System.ServiceModel.ChannelFactory<IEchoService>(binding,
                    new System.ServiceModel.EndpointAddress(uriBuilder.ToString()));
                channel = factory.CreateChannel();
                ((IChannel)channel).Open();
                string result = channel.Echo(testString);
                Assert.Equal(testString, result);
                ((IChannel)channel).Close();
                factory.Close();
            }
            finally
            {
                ServiceHelper.CloseServiceModelObjects((IChannel)channel, factory);
            }
        }
    }

    [WcfFact]
    [Issue(2870, OS = OSID.OSX)]
    [Condition(nameof(SSL_Available))]
    [OuterLoop]
    private void BasicCertAsTransport()
    {
        string testString = new string('a', 3000);
        IHost host = ServiceHelper.CreateWebHostBuilder<StartupForUnixDomainSocketTransportCertificate>(UDS.GetUDSFilePath());
        using (host)
        {
            host.Start();
            System.ServiceModel.UnixDomainSocketBinding binding = new System.ServiceModel.UnixDomainSocketBinding(System.ServiceModel.UnixDomainSocketSecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = System.ServiceModel.UnixDomainSocketClientCredentialType.Certificate;
            var uriBuilder = new UriBuilder()
            {
                Scheme = "net.uds",
                Path = UDS.GetUDSFilePath()
            };
            var cert = ServiceHelper.GetServiceCertificate();
            var identity = new X509CertificateEndpointIdentity(cert);
            var factory = new System.ServiceModel.ChannelFactory<IEchoService>(binding,
                new System.ServiceModel.EndpointAddress(new Uri(uriBuilder.ToString()), identity));

            factory.Credentials.ServiceCertificate.SslCertificateAuthentication = new System.ServiceModel.Security.X509ServiceCertificateAuthentication
            {
                CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.None,
                RevocationMode = X509RevocationMode.NoCheck
            };

            ClientCredentials clientCredentials = (ClientCredentials)factory.Endpoint.EndpointBehaviors[typeof(ClientCredentials)];
            clientCredentials.ClientCertificate.Certificate = cert; // this is a fake cert and we are not doing client cert validation
            var channel = factory.CreateChannel();
            try
            {
                ((IChannel)channel).Open();
                string result = channel.Echo(testString);
                Assert.Equal(testString, result);
                ((IChannel)channel).Close();
                factory.Close();
            }
            finally
            {
                ServiceHelper.CloseServiceModelObjects((IChannel)channel, factory);
            }
        }
    }

    [WcfFact]
    [OuterLoop]
    [Condition(nameof(IsNotWindows))]
    public void BasicIdentityOnlyAuthLinux()
    {
        string testString = new string('a', 3000);
        IHost host = ServiceHelper.CreateWebHostBuilder<StartupForUnixDomainSocketTransportIdentity>(UDS.GetUDSFilePath());
        using (host)
        {
            System.ServiceModel.ChannelFactory<IEchoService> factory = null;
            IEchoService channel = null;
            host.Start();
            try
            {
                System.ServiceModel.UnixDomainSocketBinding binding = new UnixDomainSocketBinding(UnixDomainSocketSecurityMode.TransportCredentialOnly);
                binding.Security.Transport.ClientCredentialType = System.ServiceModel.UnixDomainSocketClientCredentialType.PosixIdentity;

                factory = new System.ServiceModel.ChannelFactory<IEchoService>(binding,
                    new System.ServiceModel.EndpointAddress(new Uri("net.uds://" + UDS.GetUDSFilePath())));
                channel = factory.CreateChannel();
                ((IChannel)channel).Open();
                string result = channel.Echo(testString);
                Assert.Equal(testString, result);
                ((IChannel)channel).Close();
                factory.Close();
            }
            finally
            {
                ServiceHelper.CloseServiceModelObjects((IChannel)channel, factory);
            }
        }
    }

    public class UDS
    {
        public static string GetUDSFilePath()
        {
            return Path.Combine(Path.GetTempPath(), "unix1.txt");
        }
    }

    public class StartUpForUDS : UDS
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddServiceModelServices();
        }

        public void Configure(IHost host)
        {
            CoreWCF.UnixDomainSocketBinding serverBinding = new CoreWCF.UnixDomainSocketBinding(CoreWCF.UnixDomainSocketSecurityMode.None);
            host.UseServiceModel(builder =>
            {
                builder.AddService<EchoService>();
                builder.AddServiceEndpoint<EchoService, IEchoService>(serverBinding, "net.uds://" + GetUDSFilePath());
            });
        }
    }

    public class StartupForWindowsAuth : UDS
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddServiceModelServices();
        }

        public void Configure(IHost host)
        {
            host.UseServiceModel(builder =>
            {
                builder.AddService<EchoService>();
                var udsBinding = new CoreWCF.UnixDomainSocketBinding
                {
                    Security = new CoreWCF.UnixDomainSocketSecurity
                    {
                        Mode = CoreWCF.UnixDomainSocketSecurityMode.Transport,
                        Transport = new CoreWCF.UnixDomainSocketTransportSecurity
                        {
                            ClientCredentialType = CoreWCF.UnixDomainSocketClientCredentialType.Windows,
                        },
                    },
                };

                builder.AddServiceEndpoint<EchoService, IEchoService>(udsBinding, "net.uds://" + GetUDSFilePath());
            });
        }
    }

    public class StartupForUnixDomainSocketTransportCertificate : UDS
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddServiceModelServices();
        }

        public void Configure(IHost host)
        {
            host.UseServiceModel(builder =>
            {
                builder.AddService<EchoService>();
                var udsBinding = new CoreWCF.UnixDomainSocketBinding
                {
                    Security = new CoreWCF.UnixDomainSocketSecurity
                    {
                        Mode = CoreWCF.UnixDomainSocketSecurityMode.Transport,
                        Transport = new CoreWCF.UnixDomainSocketTransportSecurity
                        {
                            ClientCredentialType = CoreWCF.UnixDomainSocketClientCredentialType.Certificate,
                        },
                    },
                };

                builder.AddServiceEndpoint<EchoService, IEchoService>(udsBinding, "net.uds://" + GetUDSFilePath());
                Action<CoreWCF.ServiceHostBase> serviceHost =  host => ChangeHostBehavior(host);
                builder.ConfigureServiceHostBase<EchoService>(serviceHost);
            });
        }

        public void ChangeHostBehavior(CoreWCF.ServiceHostBase host)
        {
            var srvCredentials = host.Credentials;
            //provide the certificate, here we are getting the default asp.net core default certificate, not recommended for prod workload.
            srvCredentials.ServiceCertificate.Certificate = ServiceHelper.GetServiceCertificate();
            srvCredentials.ClientCertificate.Authentication.CertificateValidationMode = CoreWCF.Security.X509CertificateValidationMode.None;
        }
    }

    public class StartupForUnixDomainSocketTransportIdentity : UDS
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddServiceModelServices();
        }

        public void Configure(IHost host)
        {
            host.UseServiceModel(builder =>
            {
                builder.AddService<EchoService>();
                var udsBinding = new CoreWCF.UnixDomainSocketBinding
                {
                    Security = new CoreWCF.UnixDomainSocketSecurity
                    {
                        Mode = CoreWCF.UnixDomainSocketSecurityMode.TransportCredentialOnly,
                        Transport = new CoreWCF.UnixDomainSocketTransportSecurity
                        {
                            ClientCredentialType = CoreWCF.UnixDomainSocketClientCredentialType.PosixIdentity,
                        },
                    },
                };

                builder.AddServiceEndpoint<EchoService, IEchoService>(udsBinding, "net.uds://" + GetUDSFilePath());
            });
        }
    }
}
