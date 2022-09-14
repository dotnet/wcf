// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Net;
using System.Net.Sockets;
using System.Runtime;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal static class DnsCache
    {
        private const int MruWatermark = 64;
        private static MruCache<string, DnsCacheEntry> s_resolveCache = new MruCache<string, DnsCacheEntry>(MruWatermark);
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
                                s_machineName = Dns.GetHostEntry(string.Empty).HostName;
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
                                new EndpointNotFoundException(SR.Format(SR.DnsResolveFailed, hostName)));
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
                        new EndpointNotFoundException(SR.Format(SR.DnsResolveFailed, hostName), dnsException));
                }
            }

            return hostAddresses;
        }

        internal static async Task<IPAddress[]> LookupHostName(string hostName)
        {
            return (await Dns.GetHostEntryAsync(hostName)).AddressList;
        }

        internal class DnsCacheEntry
        {
            private IPAddress[] _addressList;

            public DnsCacheEntry(IPAddress[] addressList, DateTime timeStamp)
            {
                TimeStamp = timeStamp;
                _addressList = addressList;
            }

            public IPAddress[] AddressList
            {
                get
                {
                    return _addressList;
                }
            }

            public DateTime TimeStamp { get; }
        }
    }
}
