// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Description
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Runtime;
    using System.Runtime.Diagnostics;
    using System.Security;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Diagnostics;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Xml;
    using WsdlNS = System.Web.Services.Description;
    using XsdNS = Microsoft.Xml.Schema;

    public partial class MetadataExchangeClient
    {
        private ChannelFactory<IMetadataExchange> _factory;
        private ICredentials _webRequestCredentials;

        private TimeSpan _resolveTimeout = TimeSpan.FromMinutes(1);
        private int _maximumResolvedReferences = 10;
        private bool _resolveMetadataReferences = true;
        private long _maxMessageSize;
        private XmlDictionaryReaderQuotas _readerQuotas;

        private EndpointAddress _ctorEndpointAddress = null;
        private Uri _ctorUri = null;

        private object _thisLock = new object();

        internal const string MetadataExchangeClientKey = "MetadataExchangeClientKey";

        public MetadataExchangeClient()
        {
            _factory = new ChannelFactory<IMetadataExchange>("*");
            _maxMessageSize = GetMaxMessageSize(_factory.Endpoint.Binding);
        }
        public MetadataExchangeClient(Uri address, MetadataExchangeClientMode mode)
        {
            Validate(address, mode);

            if (mode == MetadataExchangeClientMode.HttpGet)
            {
                _ctorUri = address;
            }
            else
            {
                _ctorEndpointAddress = new EndpointAddress(address);
            }

            CreateChannelFactory(address.Scheme);
        }
        public MetadataExchangeClient(EndpointAddress address)
        {
            if (address == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("address");
            }

            _ctorEndpointAddress = address;

            CreateChannelFactory(address.Uri.Scheme);
        }
        public MetadataExchangeClient(string endpointConfigurationName)
        {
            if (endpointConfigurationName == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("endpointConfigurationName");
            }
            _factory = new ChannelFactory<IMetadataExchange>(endpointConfigurationName);
            _maxMessageSize = GetMaxMessageSize(_factory.Endpoint.Binding);
        }
        public MetadataExchangeClient(Binding mexBinding)
        {
            if (mexBinding == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("mexBinding");
            }
            _factory = new ChannelFactory<IMetadataExchange>(mexBinding);
            _maxMessageSize = GetMaxMessageSize(_factory.Endpoint.Binding);
        }

        //Configuration for credentials
        public ClientCredentials SoapCredentials
        {
            get { return _factory.Credentials; }
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }
                _factory.Endpoint.Behaviors.RemoveAll<ClientCredentials>();
                _factory.Endpoint.Behaviors.Add(value);
            }
        }
        public ICredentials HttpCredentials
        {
            get { return _webRequestCredentials; }
            set { _webRequestCredentials = value; }
        }

        // Configuration options for the entire MetadataResolver
        public TimeSpan OperationTimeout
        {
            get { return _resolveTimeout; }
            set
            {
                if (value < TimeSpan.Zero)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SRServiceModel.SFxTimeoutOutOfRange0));
                }

                if (TimeoutHelper.IsTooLarge(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, SRServiceModel.SFxTimeoutOutOfRangeTooBig));
                }

                _resolveTimeout = value;
            }
        }
        public int MaximumResolvedReferences
        {
            get { return _maximumResolvedReferences; }
            set
            {
                if (value < 1)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", string.Format(SRServiceModel.SFxMaximumResolvedReferencesOutOfRange, value)));
                }
                _maximumResolvedReferences = value;
            }
        }
        public bool ResolveMetadataReferences
        {
            get { return _resolveMetadataReferences; }
            set { _resolveMetadataReferences = value; }
        }

        internal object ThisLock
        {
            get { return _thisLock; }
        }

        internal long MaxMessageSize
        {
            get { return _maxMessageSize; }
            set { _maxMessageSize = value; }
        }

        internal XmlDictionaryReaderQuotas ReaderQuotas
        {
            get
            {
                if (_readerQuotas == null)
                {
                    if (_factory != null)
                    {
                        BindingElementCollection bindingElementCollection = _factory.Endpoint.Binding.CreateBindingElements();
                        if (bindingElementCollection != null)
                        {
                            MessageEncodingBindingElement bindingElement = bindingElementCollection.Find<MessageEncodingBindingElement>();
                            if (bindingElement != null)
                            {
                                _readerQuotas = bindingElement.GetIndividualProperty<XmlDictionaryReaderQuotas>();
                            }
                        }
                    }
                    _readerQuotas = _readerQuotas ?? EncoderDefaults.ReaderQuotas;
                }
                return _readerQuotas;
            }
        }

        private bool IsHttpOrHttps(Uri address)
        {
            // TODO
            // return address.Scheme == Uri.UriSchemeHttp || address.Scheme == Uri.UriSchemeHttps;
            return address.Scheme == "http" || address.Scheme == "https";
        }

        private void CreateChannelFactory(string scheme)
        {
            Binding mexBinding = null;
            if (MetadataExchangeBindings.TryGetBindingForScheme(scheme, out mexBinding))
            {
                _factory = new ChannelFactory<IMetadataExchange>(mexBinding);
            }
            else
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("scheme", string.Format(SRServiceModel.SFxMetadataExchangeClientCouldNotCreateChannelFactoryBadScheme, scheme));
            }
            _maxMessageSize = GetMaxMessageSize(_factory.Endpoint.Binding);
        }

        private void Validate(Uri address, MetadataExchangeClientMode mode)
        {
            if (address == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("address");
            }

            if (!address.IsAbsoluteUri)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("address", string.Format(SRServiceModel.SFxCannotGetMetadataFromRelativeAddress, address));
            }

            if (mode == MetadataExchangeClientMode.HttpGet && !IsHttpOrHttps(address))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("address", string.Format(SRServiceModel.SFxCannotHttpGetMetadataFromAddress, address));
            }

            MetadataExchangeClientModeHelper.Validate(mode);
        }

        public IAsyncResult BeginGetMetadata(AsyncCallback callback, object asyncState)
        {
            if (_ctorUri != null)
                return BeginGetMetadata(_ctorUri, MetadataExchangeClientMode.HttpGet, callback, asyncState);
            if (_ctorEndpointAddress != null)
                return BeginGetMetadata(_ctorEndpointAddress, callback, asyncState);
            else
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.SFxMetadataExchangeClientNoMetadataAddress));
        }

        public IAsyncResult BeginGetMetadata(Uri address, MetadataExchangeClientMode mode, AsyncCallback callback, object asyncState)
        {
            Validate(address, mode);

            if (mode == MetadataExchangeClientMode.HttpGet)
            {
                return this.BeginGetMetadata(new MetadataLocationRetriever(address, this), callback, asyncState);
            }
            else
            {
                return this.BeginGetMetadata(new MetadataReferenceRetriever(new EndpointAddress(address), this), callback, asyncState);
            }
        }

        public IAsyncResult BeginGetMetadata(EndpointAddress address, AsyncCallback callback, object asyncState)
        {
            if (address == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("address");
            }
            return this.BeginGetMetadata(new MetadataReferenceRetriever(address, this), callback, asyncState);
        }

        private IAsyncResult BeginGetMetadata(MetadataRetriever retriever, AsyncCallback callback, object asyncState)
        {
            ResolveCallState state = new ResolveCallState(_maximumResolvedReferences, _resolveMetadataReferences, new TimeoutHelper(this.OperationTimeout), this);
            state.StackedRetrievers.Push(retriever);
            return new AsyncMetadataResolver(state, callback, asyncState);
        }

        public MetadataSet EndGetMetadata(IAsyncResult result)
        {
            return AsyncMetadataResolver.End(result);
        }

        public Task<MetadataSet> GetMetadataAsync()
        {
            if (_ctorUri != null)
                return GetMetadataAsync(_ctorUri, MetadataExchangeClientMode.HttpGet);
            if (_ctorEndpointAddress != null)
                return GetMetadataAsync(_ctorEndpointAddress);
            else
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.SFxMetadataExchangeClientNoMetadataAddress));
        }

        public Task<MetadataSet> GetMetadataAsync(Uri address, MetadataExchangeClientMode mode)
        {
            Validate(address, mode);

            MetadataRetriever retriever = (mode == MetadataExchangeClientMode.HttpGet)
                                                ? (MetadataRetriever)new MetadataLocationRetriever(address, this)
                                                : (MetadataRetriever)new MetadataReferenceRetriever(new EndpointAddress(address), this);

            return Task.Factory.FromAsync<MetadataRetriever, MetadataSet>(this.BeginGetMetadata, this.EndGetMetadata, retriever, /* state */ null);
        }

        public Task<MetadataSet> GetMetadataAsync(EndpointAddress address)
        {
            if (address == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("address");
            }

            return Task.Factory.FromAsync<MetadataRetriever, MetadataSet>(this.BeginGetMetadata, this.EndGetMetadata, new MetadataReferenceRetriever(address, this), /* state */ null);
        }

        public Task<MetadataSet> GetMetadataAsync(EndpointAddress address, Uri via)
        {
            if (address == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("address");
            }

            if (via == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("via");
            }

            return Task.Factory.FromAsync<MetadataRetriever, MetadataSet>(this.BeginGetMetadata, this.EndGetMetadata, new MetadataReferenceRetriever(address, via, this), /* state */ null);
        }

        public MetadataSet GetMetadata()
        {
            if (_ctorUri != null)
                return GetMetadata(_ctorUri, MetadataExchangeClientMode.HttpGet);
            if (_ctorEndpointAddress != null)
                return GetMetadata(_ctorEndpointAddress);
            else
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.SFxMetadataExchangeClientNoMetadataAddress));
        }

        public MetadataSet GetMetadata(Uri address, MetadataExchangeClientMode mode)
        {
            Validate(address, mode);

            MetadataRetriever retriever;
            if (mode == MetadataExchangeClientMode.HttpGet)
            {
                retriever = new MetadataLocationRetriever(address, this);
            }
            else
            {
                retriever = new MetadataReferenceRetriever(new EndpointAddress(address), this);
            }
            return GetMetadata(retriever);
        }

        public MetadataSet GetMetadata(EndpointAddress address)
        {
            if (address == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("address");
            }

            MetadataReferenceRetriever retriever = new MetadataReferenceRetriever(address, this);
            return GetMetadata(retriever);
        }

        public MetadataSet GetMetadata(EndpointAddress address, Uri via)
        {
            if (address == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("address");
            }

            if (via == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("via");
            }

            MetadataReferenceRetriever retriever = new MetadataReferenceRetriever(address, via, this);
            return GetMetadata(retriever);
        }

        private MetadataSet GetMetadata(MetadataRetriever retriever)
        {
            ResolveCallState resolveCallState = new ResolveCallState(_maximumResolvedReferences, _resolveMetadataReferences, new TimeoutHelper(this.OperationTimeout), this);
            resolveCallState.StackedRetrievers.Push(retriever);
            this.ResolveNext(resolveCallState);

            return resolveCallState.MetadataSet;
        }

        private void ResolveNext(ResolveCallState resolveCallState)
        {
            if (resolveCallState.StackedRetrievers.Count > 0)
            {
                MetadataRetriever retriever = resolveCallState.StackedRetrievers.Pop();

                if (resolveCallState.HasBeenUsed(retriever))
                {
                    this.ResolveNext(resolveCallState);
                }
                else
                {
                    if (resolveCallState.ResolvedMaxResolvedReferences)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.SFxResolvedMaxResolvedReferences));
                    }

                    resolveCallState.LogUse(retriever);
                    resolveCallState.HandleSection(retriever.Retrieve(resolveCallState.TimeoutHelper));
                    this.ResolveNext(resolveCallState);
                }
            }
        }

        protected internal virtual ChannelFactory<IMetadataExchange> GetChannelFactory(EndpointAddress metadataAddress, string dialect, string identifier)
        {
            return _factory;
        }

        private static long GetMaxMessageSize(Binding mexBinding)
        {
            BindingElementCollection bindingElementCollection = mexBinding.CreateBindingElements();
            TransportBindingElement bindingElement = bindingElementCollection.Find<TransportBindingElement>();
            if (bindingElement == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.SFxBindingDoesNotHaveATransportBindingElement));
            }
            return bindingElement.MaxReceivedMessageSize;
        }

        protected internal virtual HttpWebRequest GetWebRequest(Uri location, string dialect, string identifier)
        {
            ServicePointManager.CheckCertificateRevocationList = true;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(location);
            request.Method = "GET";
            request.Credentials = this.HttpCredentials;

            return request;
        }

        internal static void TraceSendRequest(Uri address)
        {
            TraceSendRequest(TraceCode.MetadataExchangeClientSendRequest, SRServiceModel.TraceCodeMetadataExchangeClientSendRequest,
                address.ToString(), MetadataExchangeClientMode.HttpGet.ToString());
        }

        internal static void TraceSendRequest(EndpointAddress address)
        {
            TraceSendRequest(TraceCode.MetadataExchangeClientSendRequest, SRServiceModel.TraceCodeMetadataExchangeClientSendRequest,
                address.ToString(), MetadataExchangeClientMode.MetadataExchange.ToString());
        }

        private static void TraceSendRequest(int traceCode, string traceDescription, string address, string mode)
        {
        }

        internal static void TraceReceiveReply(string sourceUrl, Type metadataType)
        {
        }

        private class ResolveCallState
        {
            private Dictionary<MetadataRetriever, MetadataRetriever> _usedRetrievers;   // to prevent looping when chasing MetadataReferences
            private MetadataSet _metadataSet;
            private int _maxResolvedReferences;
            private bool _resolveMetadataReferences;
            private Stack<MetadataRetriever> _stackedRetrievers;
            private MetadataExchangeClient _resolver;
            private TimeoutHelper _timeoutHelper;

            internal ResolveCallState(int maxResolvedReferences, bool resolveMetadataReferences,
                TimeoutHelper timeoutHelper, MetadataExchangeClient resolver)
            {
                _maxResolvedReferences = maxResolvedReferences;
                _resolveMetadataReferences = resolveMetadataReferences;
                _resolver = resolver;
                _timeoutHelper = timeoutHelper;
                _metadataSet = new MetadataSet();
                _usedRetrievers = new Dictionary<MetadataRetriever, MetadataRetriever>();
                _stackedRetrievers = new Stack<MetadataRetriever>();
            }

            internal MetadataSet MetadataSet
            {
                get { return _metadataSet; }
            }

            internal Stack<MetadataRetriever> StackedRetrievers
            {
                get { return _stackedRetrievers; }
            }

            internal bool ResolvedMaxResolvedReferences
            {
                get { return _usedRetrievers.Count == _maxResolvedReferences; }
            }

            internal TimeoutHelper TimeoutHelper
            {
                get { return _timeoutHelper; }
            }

            internal void HandleSection(MetadataSection section)
            {
                if (section.Metadata is MetadataSet)
                {
                    foreach (MetadataSection innerSection in ((MetadataSet)section.Metadata).MetadataSections)
                    {
                        innerSection.SourceUrl = section.SourceUrl;
                        this.HandleSection(innerSection);
                    }
                }
                else if (section.Metadata is MetadataReference)
                {
                    if (_resolveMetadataReferences)
                    {
                        EndpointAddress address = ((MetadataReference)section.Metadata).Address;
                        MetadataRetriever retriever = new MetadataReferenceRetriever(address, _resolver, section.Dialect, section.Identifier);
                        _stackedRetrievers.Push(retriever);
                    }
                    else
                    {
                        _metadataSet.MetadataSections.Add(section);
                    }
                }
                else if (section.Metadata is MetadataLocation)
                {
                    if (_resolveMetadataReferences)
                    {
                        string location = ((MetadataLocation)section.Metadata).Location;
                        MetadataRetriever retriever = new MetadataLocationRetriever(this.CreateUri(section.SourceUrl, location), _resolver, section.Dialect, section.Identifier);
                        _stackedRetrievers.Push(retriever);
                    }
                    else
                    {
                        _metadataSet.MetadataSections.Add(section);
                    }
                }
                else if (section.Metadata is WsdlNS.ServiceDescription)
                {
                    if (_resolveMetadataReferences)
                    {
                        this.HandleWsdlImports(section);
                    }
                    _metadataSet.MetadataSections.Add(section);
                }
                else if (section.Metadata is XsdNS.XmlSchema)
                {
                    if (_resolveMetadataReferences)
                    {
                        this.HandleSchemaImports(section);
                    }
                    _metadataSet.MetadataSections.Add(section);
                }
                else
                {
                    _metadataSet.MetadataSections.Add(section);
                }
            }

            private void HandleSchemaImports(MetadataSection section)
            {
                XsdNS.XmlSchema schema = (XsdNS.XmlSchema)section.Metadata;
                foreach (XsdNS.XmlSchemaExternal external in schema.Includes)
                {
                    if (!String.IsNullOrEmpty(external.SchemaLocation))
                    {
                        EnqueueRetrieverIfShouldResolve(
                            new MetadataLocationRetriever(
                                this.CreateUri(section.SourceUrl, external.SchemaLocation),
                                _resolver));
                    }
                }
            }

            private void HandleWsdlImports(MetadataSection section)
            {
                WsdlNS.ServiceDescription wsdl = (WsdlNS.ServiceDescription)section.Metadata;
                foreach (WsdlNS.Import import in wsdl.Imports)
                {
                    if (!String.IsNullOrEmpty(import.Location))
                    {
                        EnqueueRetrieverIfShouldResolve(new MetadataLocationRetriever(this.CreateUri(section.SourceUrl, import.Location), _resolver));
                    }
                }

                foreach (XsdNS.XmlSchema schema in wsdl.Types.Schemas)
                {
                    MetadataSection schemaSection = new MetadataSection(null, null, schema);
                    schemaSection.SourceUrl = section.SourceUrl;
                    this.HandleSchemaImports(schemaSection);
                }
            }

            private Uri CreateUri(string baseUri, string relativeUri)
            {
                return new Uri(new Uri(baseUri), relativeUri);
            }

            private void EnqueueRetrieverIfShouldResolve(MetadataRetriever retriever)
            {
                if (_resolveMetadataReferences)
                {
                    _stackedRetrievers.Push(retriever);
                }
            }

            internal bool HasBeenUsed(MetadataRetriever retriever)
            {
                return _usedRetrievers.ContainsKey(retriever);
            }

            internal void LogUse(MetadataRetriever retriever)
            {
                _usedRetrievers.Add(retriever, retriever);
            }
        }

        internal abstract class MetadataRetriever
        {
            protected MetadataExchangeClient resolver;
            protected string dialect;
            protected string identifier;

            public MetadataRetriever(MetadataExchangeClient resolver, string dialect, string identifier)
            {
                this.resolver = resolver;
                this.dialect = dialect;
                this.identifier = identifier;
            }

            internal MetadataSection Retrieve(TimeoutHelper timeoutHelper)
            {
                try
                {
                    XmlReader reader = this.DownloadMetadata(timeoutHelper);
                    {
                        return MetadataRetriever.CreateMetadataSection(reader, this.SourceUrl);
                    }
                }
#pragma warning disable 56500 // covered by FxCOP
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                        throw;
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                        string.Format(SRServiceModel.SFxBadMetadataReference, this.SourceUrl), e));
                }
            }

            internal abstract IAsyncResult BeginRetrieve(TimeoutHelper timeoutHelper, AsyncCallback callback, object state);
            internal abstract MetadataSection EndRetrieve(IAsyncResult result);

            static internal MetadataSection CreateMetadataSection(XmlReader reader, string sourceUrl)
            {
                MetadataSection section = null;
                Type metadataType = null;

                if (CanReadMetadataSet(reader))
                {
                    MetadataSet newSet = MetadataSet.ReadFrom(reader);
                    section = new MetadataSection(MetadataSection.MetadataExchangeDialect, null, newSet);
                    metadataType = typeof(MetadataSet);
                }
                else if (WsdlNS.ServiceDescription.CanRead(reader))
                {
                    WsdlNS.ServiceDescription wsdl = WsdlNS.ServiceDescription.Read(reader);
                    section = MetadataSection.CreateFromServiceDescription(wsdl);
                    metadataType = typeof(WsdlNS.ServiceDescription);
                }
                else if (CanReadSchema(reader))
                {
                    XsdNS.XmlSchema schema = XsdNS.XmlSchema.Read(reader, null);
                    section = MetadataSection.CreateFromSchema(schema);
                    metadataType = typeof(XsdNS.XmlSchema);
                }
                else
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(reader);
                    section = new MetadataSection(null, null, doc.DocumentElement);
                    metadataType = typeof(XmlElement);
                }

                section.SourceUrl = sourceUrl;

                TraceReceiveReply(sourceUrl, metadataType);

                return section;
            }

            protected abstract XmlReader DownloadMetadata(TimeoutHelper timeoutHelper);

            protected abstract string SourceUrl { get; }

            private static bool CanReadSchema(XmlReader reader)
            {
                return reader.LocalName == MetadataStrings.XmlSchema.Schema
                    && reader.NamespaceURI == XsdNS.XmlSchema.Namespace;
            }

            private static bool CanReadMetadataSet(XmlReader reader)
            {
                return reader.LocalName == MetadataStrings.MetadataExchangeStrings.Metadata
                    && reader.NamespaceURI == MetadataStrings.MetadataExchangeStrings.Namespace;
            }
        }

        private class MetadataLocationRetriever : MetadataRetriever
        {
            private Uri _location;
            private Uri _responseLocation;

            internal MetadataLocationRetriever(Uri location, MetadataExchangeClient resolver)
                : this(location, resolver, null, null)
            {
            }

            internal MetadataLocationRetriever(Uri location, MetadataExchangeClient resolver, string dialect, string identifier)
                : base(resolver, dialect, identifier)
            {
                ValidateLocation(location);
                _location = location;
                _responseLocation = location;
            }

            internal static void ValidateLocation(Uri location)
            {
                if (location == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("location");
                }

                // TODO
                //if (location.Scheme != Uri.UriSchemeHttp && location.Scheme != Uri.UriSchemeHttps)
                if (location.Scheme != "http" && location.Scheme != "https")
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("location", string.Format(SRServiceModel.SFxCannotGetMetadataFromLocation, location.ToString()));
                }
            }

            public override bool Equals(object obj)
            {
                return obj is MetadataLocationRetriever && ((MetadataLocationRetriever)obj)._location == _location;
            }

            public override int GetHashCode()
            {
                return _location.GetHashCode();
            }

            protected override XmlReader DownloadMetadata(TimeoutHelper timeoutHelper)
            {
                HttpWebResponse response;
                HttpWebRequest request;

                try
                {
                    request = this.resolver.GetWebRequest(_location, this.dialect, this.identifier);
                }
#pragma warning disable 56500 // covered by FxCOP
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                        throw;

                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                        string.Format(SRServiceModel.SFxMetadataExchangeClientCouldNotCreateWebRequest, _location, this.dialect, this.identifier), e));
                }

                TraceSendRequest(_location);

                Task<WebResponse> task = request.GetResponseAsync();
                task.Wait();
                response = task.Result as HttpWebResponse;
                return MetadataLocationRetriever.GetXmlReader(response, this.resolver.MaxMessageSize, this.resolver.ReaderQuotas);
            }

            internal static XmlReader GetXmlReader(HttpWebResponse response, long maxMessageSize, XmlDictionaryReaderQuotas readerQuotas)
            {
                // the response stream is not owned by us, the XmlReader will own the stream so we need to pass a copy to it.
                MemoryStream stream = new MemoryStream();
                response.GetResponseStream().CopyTo(stream);
                stream.Position = 0;

                readerQuotas = readerQuotas ?? EncoderDefaults.ReaderQuotas;
                XmlReader reader = XmlDictionaryReader.CreateTextReader(
                    new MaxMessageSizeStream(stream, maxMessageSize),
                    EncodingHelper.GetDictionaryReaderEncoding(response.ContentType),
                    readerQuotas,
                    null);

                reader.Read();
                reader.MoveToContent();

                return reader;
            }

            internal override IAsyncResult BeginRetrieve(TimeoutHelper timeoutHelper, AsyncCallback callback, object state)
            {
                AsyncMetadataLocationRetriever result;
                try
                {
                    HttpWebRequest request;
                    try
                    {
                        request = this.resolver.GetWebRequest(_location, this.dialect, this.identifier);
                    }
#pragma warning disable 56500 // covered by FxCOP
                    catch (Exception e)
                    {
                        if (Fx.IsFatal(e))
                            throw;
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                            string.Format(SRServiceModel.SFxMetadataExchangeClientCouldNotCreateWebRequest, _location, this.dialect, this.identifier), e));
                    }

                    TraceSendRequest(_location);
                    result = new AsyncMetadataLocationRetriever(request, this.resolver.MaxMessageSize, this.resolver.ReaderQuotas, timeoutHelper, callback, state);
                }
#pragma warning disable 56500 // covered by FxCOP
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                        throw;
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                        string.Format(SRServiceModel.SFxBadMetadataReference, this.SourceUrl), e));
                }
                return result;
            }

            internal override MetadataSection EndRetrieve(IAsyncResult result)
            {
                try
                {
                    return AsyncMetadataLocationRetriever.End(result);
                }
#pragma warning disable 56500 // covered by FxCOP
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                        throw;
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                        string.Format(SRServiceModel.SFxBadMetadataReference, this.SourceUrl), e));
                }
            }

            protected override string SourceUrl
            {
                get { return _responseLocation.ToString(); }
            }

            private class AsyncMetadataLocationRetriever : AsyncResult
            {
                private MetadataSection _section;
                private long _maxMessageSize;
                private XmlDictionaryReaderQuotas _readerQuotas;

                internal AsyncMetadataLocationRetriever(WebRequest request, long maxMessageSize, XmlDictionaryReaderQuotas readerQuotas, TimeoutHelper timeoutHelper, AsyncCallback callback, object state)
                    : base(callback, state)
                {
                    _maxMessageSize = maxMessageSize;
                    _readerQuotas = readerQuotas;
                    IAsyncResult result = request.BeginGetResponse(Fx.ThunkCallback(new AsyncCallback(this.GetResponseCallback)), request);
                }

                private static void RetrieveTimeout(object state, bool timedOut)
                {
                    if (timedOut)
                    {
                        HttpWebRequest request = state as HttpWebRequest;
                        if (request != null)
                        {
                            request.Abort();
                        }
                    }
                }

                internal static MetadataSection End(IAsyncResult result)
                {
                    AsyncMetadataLocationRetriever retrieverResult = AsyncResult.End<AsyncMetadataLocationRetriever>(result);
                    return retrieverResult._section;
                }

                internal void GetResponseCallback(IAsyncResult result)
                {
                    if (result.CompletedSynchronously)
                        return;

                    Exception exception = null;
                    try
                    {
                        HandleResult(result);
                    }
#pragma warning disable 56500 // covered by FxCOP
                    catch (Exception e)
                    {
                        if (Fx.IsFatal(e))
                            throw;
                        exception = e;
                    }
                    this.Complete(false, exception);
                }

                private void HandleResult(IAsyncResult result)
                {
                    HttpWebRequest request = (HttpWebRequest)result.AsyncState;

                    using (XmlReader reader =
                        MetadataLocationRetriever.GetXmlReader((HttpWebResponse)request.EndGetResponse(result), _maxMessageSize, _readerQuotas))
                    {
                        // TODO (test): section = MetadataRetriever.CreateMetadataSection(reader, request.Address.ToString());
                        _section = MetadataRetriever.CreateMetadataSection(reader, request.RequestUri.ToString());
                    }
                }
            }
        }

        private class MetadataReferenceRetriever : MetadataRetriever
        {
            private EndpointAddress _address;
            private Uri _via;

            public MetadataReferenceRetriever(EndpointAddress address, MetadataExchangeClient resolver)
                : this(address, null, resolver, null, null)
            {
            }

            public MetadataReferenceRetriever(EndpointAddress address, Uri via, MetadataExchangeClient resolver)
                : this(address, via, resolver, null, null)
            {
            }

            public MetadataReferenceRetriever(EndpointAddress address, MetadataExchangeClient resolver, string dialect, string identifier)
                : this(address, null, resolver, dialect, identifier)
            {
            }

            private MetadataReferenceRetriever(EndpointAddress address, Uri via, MetadataExchangeClient resolver, string dialect, string identifier)
                : base(resolver, dialect, identifier)
            {
                if (address == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("address");
                }

                _address = address;
                _via = via;
            }

            protected override string SourceUrl
            {
                get { return _address.Uri.ToString(); }
            }

            internal override IAsyncResult BeginRetrieve(TimeoutHelper timeoutHelper, AsyncCallback callback, object state)
            {
                try
                {
                    IMetadataExchange metadataClient;
                    MessageVersion messageVersion;
                    lock (this.resolver.ThisLock)
                    {
                        ChannelFactory<IMetadataExchange> channelFactory;
                        try
                        {
                            channelFactory = this.resolver.GetChannelFactory(_address, this.dialect, this.identifier);
                        }
#pragma warning disable 56500 // covered by FxCOP
                        catch (Exception e)
                        {
                            if (Fx.IsFatal(e))
                                throw;
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                                string.Format(SRServiceModel.SFxMetadataExchangeClientCouldNotCreateChannelFactory, _address, this.dialect, this.identifier), e));
                        }
                        metadataClient = CreateChannel(channelFactory);
                        messageVersion = channelFactory.Endpoint.Binding.MessageVersion;
                    }
                    TraceSendRequest(_address);
                    return new AsyncMetadataReferenceRetriever(metadataClient, messageVersion, timeoutHelper, callback, state);
                }
#pragma warning disable 56500 // covered by FxCOP
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                        throw;
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                        string.Format(SRServiceModel.SFxBadMetadataReference, this.SourceUrl), e));
                }
            }

            private IMetadataExchange CreateChannel(ChannelFactory<IMetadataExchange> channelFactory)
            {
                if (_via != null)
                {
                    return channelFactory.CreateChannel(_address, _via);
                }
                else
                {
                    return channelFactory.CreateChannel(_address);
                }
            }

            private static Message CreateGetMessage(MessageVersion messageVersion)
            {
                return Message.CreateMessage(messageVersion, MetadataStrings.WSTransfer.GetAction);
            }

            protected override XmlReader DownloadMetadata(TimeoutHelper timeoutHelper)
            {
                IMetadataExchange metadataClient;
                MessageVersion messageVersion;

                lock (this.resolver.ThisLock)
                {
                    ChannelFactory<IMetadataExchange> channelFactory;
                    try
                    {
                        channelFactory = this.resolver.GetChannelFactory(_address, this.dialect, this.identifier);
                    }
#pragma warning disable 56500 // covered by FxCOP
                    catch (Exception e)
                    {
                        if (Fx.IsFatal(e))
                            throw;
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                            string.Format(SRServiceModel.SFxMetadataExchangeClientCouldNotCreateChannelFactory, _address, this.dialect, this.identifier), e));
                    }

                    metadataClient = CreateChannel(channelFactory);
                    messageVersion = channelFactory.Endpoint.Binding.MessageVersion;
                }
                Message response;

                TraceSendRequest(_address);

                try
                {
                    using (Message getMessage = CreateGetMessage(messageVersion))
                    {
                        ((IClientChannel)metadataClient).OperationTimeout = timeoutHelper.RemainingTime();
                        response = metadataClient.Get(getMessage);
                    }

                    ((IClientChannel)metadataClient).Close();
                }
                finally
                {
                    ((IClientChannel)metadataClient).Abort();
                }

                if (response.IsFault)
                {
                    MessageFault fault = MessageFault.CreateFault(response, 64 * 1024);
                    StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
                    XmlWriter xmlWriter = XmlWriter.Create(stringWriter);
                    fault.WriteTo(xmlWriter, response.Version.Envelope);
                    xmlWriter.Flush();
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(stringWriter.ToString()));
                }

                return response.GetReaderAtBodyContents();
            }

            internal override MetadataSection EndRetrieve(IAsyncResult result)
            {
                try
                {
                    return AsyncMetadataReferenceRetriever.End(result);
                }
#pragma warning disable 56500 // covered by FxCOP
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                        throw;
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                        string.Format(SRServiceModel.SFxBadMetadataReference, this.SourceUrl), e));
                }
            }

            public override bool Equals(object obj)
            {
                return obj is MetadataReferenceRetriever && ((MetadataReferenceRetriever)obj)._address == _address;
            }

            public override int GetHashCode()
            {
                return _address.GetHashCode();
            }

            private class AsyncMetadataReferenceRetriever : AsyncResult
            {
                private MetadataSection _section;
                private Message _message;
                internal AsyncMetadataReferenceRetriever(IMetadataExchange metadataClient, MessageVersion messageVersion, TimeoutHelper timeoutHelper, AsyncCallback callback, object state)
                    : base(callback, state)
                {
                    _message = MetadataReferenceRetriever.CreateGetMessage(messageVersion);
                    ((IClientChannel)metadataClient).OperationTimeout = timeoutHelper.RemainingTime();
                    IAsyncResult result = metadataClient.BeginGet(_message, Fx.ThunkCallback(new AsyncCallback(this.RequestCallback)), metadataClient);

                    if (result.CompletedSynchronously)
                    {
                        HandleResult(result);

                        this.Complete(true);
                    }
                }

                internal static MetadataSection End(IAsyncResult result)
                {
                    AsyncMetadataReferenceRetriever retrieverResult = AsyncResult.End<AsyncMetadataReferenceRetriever>(result);
                    return retrieverResult._section;
                }

                internal void RequestCallback(IAsyncResult result)
                {
                    if (result.CompletedSynchronously)
                        return;

                    Exception exception = null;
                    try
                    {
                        HandleResult(result);
                    }
#pragma warning disable 56500 // covered by FxCOP
                    catch (Exception e)
                    {
                        if (Fx.IsFatal(e))
                            throw;
                        exception = e;
                    }
                    this.Complete(false, exception);
                }

                private void HandleResult(IAsyncResult result)
                {
                    IMetadataExchange metadataClient = (IMetadataExchange)result.AsyncState;
                    Message response = metadataClient.EndGet(result);

                    using (_message)
                    {
                        if (response.IsFault)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.SFxBadMetadataReference,
                                ((IClientChannel)metadataClient).RemoteAddress.Uri.ToString())));
                        }
                        else
                        {
                            using (XmlReader reader = response.GetReaderAtBodyContents())
                            {
                                _section = MetadataRetriever.CreateMetadataSection(reader, ((IClientChannel)metadataClient).RemoteAddress.Uri.ToString());
                            }
                        }
                    }
                }
            }
        }

        private class AsyncMetadataResolver : AsyncResult
        {
            private ResolveCallState _resolveCallState;

            internal AsyncMetadataResolver(ResolveCallState resolveCallState, AsyncCallback callerCallback, object callerAsyncState)
                : base(callerCallback, callerAsyncState)
            {
                if (resolveCallState == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("resolveCallState");
                }

                _resolveCallState = resolveCallState;


                Exception exception = null;
                bool doneResolving = false;
                try
                {
                    doneResolving = this.ResolveNext();
                }
#pragma warning disable 56500 // covered by FxCOP
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                        throw;
                    exception = e;
                    doneResolving = true;
                }

                if (doneResolving)
                {
                    this.Complete(true, exception);
                }
            }

            private bool ResolveNext()
            {
                bool doneResolving = false;
                if (_resolveCallState.StackedRetrievers.Count > 0)
                {
                    MetadataRetriever retriever = _resolveCallState.StackedRetrievers.Pop();

                    if (_resolveCallState.HasBeenUsed(retriever))
                    {
                        doneResolving = this.ResolveNext();
                    }
                    else
                    {
                        if (_resolveCallState.ResolvedMaxResolvedReferences)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.SFxResolvedMaxResolvedReferences));
                        }
                        else
                        {
                            _resolveCallState.LogUse(retriever);
                            IAsyncResult result = retriever.BeginRetrieve(_resolveCallState.TimeoutHelper, Fx.ThunkCallback(new AsyncCallback(this.RetrieveCallback)), retriever);

                            if (result.CompletedSynchronously)
                            {
                                doneResolving = HandleResult(result);
                            }
                        }
                    }
                }
                else
                {
                    doneResolving = true;
                }

                return doneResolving;
            }

            internal static MetadataSet End(IAsyncResult result)
            {
                AsyncMetadataResolver resolverResult = AsyncResult.End<AsyncMetadataResolver>(result);
                return resolverResult._resolveCallState.MetadataSet;
            }

            internal void RetrieveCallback(IAsyncResult result)
            {
                if (result.CompletedSynchronously)
                    return;

                Exception exception = null;
                bool doneResolving = false;
                try
                {
                    doneResolving = HandleResult(result);
                }
#pragma warning disable 56500 // covered by FxCOP
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                        throw;
                    exception = e;
                    doneResolving = true;
                }

                if (doneResolving)
                {
                    this.Complete(false, exception);
                }
            }

            private bool HandleResult(IAsyncResult result)
            {
                MetadataRetriever retriever = (MetadataRetriever)result.AsyncState;
                MetadataSection section = retriever.EndRetrieve(result);
                _resolveCallState.HandleSection(section);
                return this.ResolveNext();
            }
        }
    }

    internal class EncodingHelper
    {
        internal const string ApplicationBase = "application";

        internal static Encoding GetRfcEncoding(string contentTypeStr)
        {
            return new ASCIIEncoding();
        }

        internal static Encoding GetDictionaryReaderEncoding(string contentTypeStr)
        {
            if (String.IsNullOrEmpty(contentTypeStr))
                return TextEncoderDefaults.Encoding;

            Encoding encoding = GetRfcEncoding(contentTypeStr);

            if (encoding == null)
                return TextEncoderDefaults.Encoding;

            string charSet = encoding.WebName;
            Encoding[] supportedEncodings = TextEncoderDefaults.SupportedEncodings;
            for (int i = 0; i < supportedEncodings.Length; i++)
            {
                if (charSet == supportedEncodings[i].WebName)
                    return encoding;
            }

            return TextEncoderDefaults.Encoding;
        }
    }

    public enum MetadataExchangeClientMode
    {
        MetadataExchange,
        HttpGet,
    }

    internal static class MetadataExchangeClientModeHelper
    {
        static public bool IsDefined(MetadataExchangeClientMode x)
        {
            return
                x == MetadataExchangeClientMode.MetadataExchange ||
                x == MetadataExchangeClientMode.HttpGet ||
                false;
        }

        public static void Validate(MetadataExchangeClientMode value)
        {
            if (!IsDefined(value))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidEnumArgumentException("value", (int)value,
                    typeof(MetadataExchangeClientMode)));
            }
        }
    }
}
