// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net;
using System.Net.Sockets;
using System.Runtime;
using System.Threading.Tasks;
#if FEATURE_NETNATIVE
using System.Collections.Generic;

using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using RTSocketError = Windows.Networking.Sockets.SocketError;
#endif

namespace System.ServiceModel.Channels
{
    internal static class DnsCache
    {
        private const int mruWatermark = 64;
        private static MruCache<string, DnsCacheEntry> s_resolveCache = new MruCache<string, DnsCacheEntry>(mruWatermark);
        private static readonly TimeSpan s_cacheTimeout = TimeSpan.FromSeconds(2);

        // Double-checked locking pattern requires volatile for read/write synchronization
        private static volatile string s_machineName;

        private static object ThisLock
        {
            get
            {
                return s_resolveCache;
            }
        }

        public static string MachineName
        {
            get
            {
                if (s_machineName == null)
                {
                    lock (ThisLock)
                    {
                        if (s_machineName == null)
                        {
                            try
                            {
#if FEATURE_NETNATIVE
                                var hostNamesList = NetworkInformation.GetHostNames();
                                foreach (var entry in hostNamesList)
                                {
                                    if (entry.Type == HostNameType.DomainName)
                                    {
                                        s_machineName = entry.CanonicalName;
                                        break;
                                    }
                                }
#else
                                s_machineName = Dns.GetHostEntryAsync(String.Empty).GetAwaiter().GetResult().HostName;
#endif
                            }
                            catch (SocketException)
                            {
                                throw;
                            }
                        }
                    }
                }

                return s_machineName;
            }
        }

        public static async Task<IPAddress[]> ResolveAsync(Uri uri)
        {
            string hostName = uri.DnsSafeHost;
            IPAddress[] hostAddresses = null;
            DateTime now = DateTime.UtcNow;

            lock (ThisLock)
            {
                DnsCacheEntry cacheEntry;
                if (s_resolveCache.TryGetValue(hostName, out cacheEntry))
                {
                    if (now.Subtract(cacheEntry.TimeStamp) > s_cacheTimeout)
                    {
                        s_resolveCache.Remove(hostName);
                        cacheEntry = null;
                    }
                    else
                    {
                        if (cacheEntry.AddressList == null)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                                new EndpointNotFoundException(string.Format(SRServiceModel.DnsResolveFailed, hostName)));
                        }
                        hostAddresses = cacheEntry.AddressList;
                    }
                }
            }

            if (hostAddresses == null)
            {
                SocketException dnsException = null;
                try
                {
                    hostAddresses = await LookupHostName(hostName);
                }
                catch (SocketException e)
                {
                    dnsException = e;
                }

                lock (ThisLock)
                {
                    // MruCache doesn't have a this[] operator, so we first remove (just in case it exists already)
                    s_resolveCache.Remove(hostName);
                    s_resolveCache.Add(hostName, new DnsCacheEntry(hostAddresses, now));
                }

                if (dnsException != null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new EndpointNotFoundException(string.Format(SRServiceModel.DnsResolveFailed, hostName), dnsException));
                }
            }

            return hostAddresses;
        }

#if FEATURE_NETNATIVE
        internal static async Task<IPAddress[]> LookupHostName(string hostName)
        {
            try
            {
                IReadOnlyList<EndpointPair> data = await DatagramSocket.GetEndpointPairsAsync(new HostName(hostName), "0").AsTask();
                List<IPAddress> addresses = new List<IPAddress>(data.Count);
                if (data != null && data.Count > 0)
                {
                    foreach (EndpointPair item in data)
                    {
                        if (item != null && item.RemoteHostName != null &&
                                      (item.RemoteHostName.Type == HostNameType.Ipv4 || item.RemoteHostName.Type == HostNameType.Ipv6))
                        {
                            IPAddress address;
                            if(IPAddress.TryParse(item.RemoteHostName.CanonicalName, out address))
                            {
                                addresses.Add(address);
                            }
                        }
                    }
                }
                return addresses.ToArray();
            }
            catch (Exception exception)
            {
                if (RTSocketError.GetStatus(exception.HResult) != SocketErrorStatus.Unknown)
                {
                    throw new SocketException(exception.HResult & 0x0000FFFF);
                }
                throw;
            }
        }
#else
        internal static async Task<IPAddress[]> LookupHostName(string hostName)
        {
            return (await Dns.GetHostEntryAsync(hostName)).AddressList;
        }
#endif

        internal class DnsCacheEntry
        {
            private DateTime _timeStamp;
            private IPAddress[] _addressList;

            public DnsCacheEntry(IPAddress[] addressList, DateTime timeStamp)
            {
                _timeStamp = timeStamp;
                _addressList = addressList;
            }

            public IPAddress[] AddressList
            {
                get
                {
                    return _addressList;
                }
            }

            public DateTime TimeStamp
            {
                get
                {
                    return _timeStamp;
                }
            }
        }
    }
}
