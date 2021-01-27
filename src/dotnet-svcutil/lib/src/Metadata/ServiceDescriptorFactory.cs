// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Tools.ServiceModel.Svcutil.Metadata
{
    /// <summary>
    /// This class is intended for caching service descriptors that have already successfully resolved the service metadata.
    /// This is useful when authentication is required, like in UI-based scenarios. Once metadocs are downloaded they are cached in a service descriptor.
    /// Observe that it keeps a cache of all created descriptor instances regardless of state; it is intended for short-lived sessions.
    /// </summary>
    public class ServiceDescriptorFactory
    {
        private IHttpCredentialsProvider _userCredentialsProvider;
        private IClientCertificateProvider _clientCertificateProvider;
        private IServerCertificateValidationProvider _serverCertificateValidationProvider;

        private object _cacheLock = new object();
        private Dictionary<int, ServiceDescriptor> _cache = new Dictionary<int, ServiceDescriptor>();

        public ServiceDescriptorFactory(
            IHttpCredentialsProvider userCredentialsProvider,
            IClientCertificateProvider clientCertificateProvider,
            IServerCertificateValidationProvider serverCertificateValidationProvider)
        {
            _userCredentialsProvider = userCredentialsProvider;
            _clientCertificateProvider = clientCertificateProvider;
            _serverCertificateValidationProvider = serverCertificateValidationProvider;
        }


        /// <summary>
        /// Creates a ServiceDescriptor instance or retrieve an existing one that has imported its metadata already.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        private ServiceDescriptor Get(MetadataDocumentLoader metadaDocLoader)
        {
            ServiceDescriptor serviceDescriptor;
            var key = metadaDocLoader.GetHashCode();

            lock (_cacheLock)
            {
                if (_cache.ContainsKey(key) && _cache[key].MetadataImported)
                {
                    serviceDescriptor = _cache[key];
                }
                else
                {
                    serviceDescriptor = CreateServiceDescriptor(metadaDocLoader);
                    _cache[key] = serviceDescriptor;
                }
            }

            return serviceDescriptor;
        }

        protected virtual ServiceDescriptor CreateServiceDescriptor(MetadataDocumentLoader metadaDocLoader)
        {
            return new ServiceDescriptor(metadaDocLoader);
        }

        protected virtual MetadataDocumentLoader CreateMetadataDocumentLoader(string uri, IHttpCredentialsProvider httpCredentialsProvider, IClientCertificateProvider clientCertificatesProvider, IServerCertificateValidationProvider serverCertificateValidationProvider)
        {
            return new MetadataDocumentLoader(uri, httpCredentialsProvider, clientCertificatesProvider, serverCertificateValidationProvider);
        }

        protected virtual MetadataDocumentLoader CreateMetadataDocumentLoader(IEnumerable<string> metadataFiles, bool resolveExternalDocuments, IHttpCredentialsProvider httpCredentialsProvider, IClientCertificateProvider clientCertificatesProvider, IServerCertificateValidationProvider serverCertificateValidationProvider)
        {
            return new MetadataDocumentLoader(metadataFiles, resolveExternalDocuments, httpCredentialsProvider, clientCertificatesProvider, serverCertificateValidationProvider);
        }

        public ServiceDescriptor Get(string metadataUri)
        {
            // validate the uri, this would throw if invalid.
            var metadaDocLoader = CreateMetadataDocumentLoader(metadataUri, _userCredentialsProvider, _clientCertificateProvider, _serverCertificateValidationProvider);
            return Get(metadaDocLoader);
        }

        public ServiceDescriptor Get(IEnumerable<string> metadataFiles)
        {
            // validate the uris, this would throw if invalid.
            var metadaDocLoader = CreateMetadataDocumentLoader(metadataFiles, false, _userCredentialsProvider, _clientCertificateProvider, _serverCertificateValidationProvider);
            return Get(metadaDocLoader);
        }

        /// <summary>
        /// Purges the cache.  Careful use of this method is required:
        /// This method removes any service descriptor that has not already downloaded its metadata successfully,
        /// so those currently downloading metadata (if any) will be removed from the cache.
        /// </summary>
        public void Purge()
        {
            lock (_cacheLock)
            {
                var removeKeys = _cache.Keys.Where((k) => !_cache[k].MetadataImported).ToList();
                removeKeys.ForEach((k) => _cache.Remove(k));
            }
        }
    }
}
