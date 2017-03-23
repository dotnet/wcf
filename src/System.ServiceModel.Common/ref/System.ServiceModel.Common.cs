// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace System.Collections.Generic 
{
      public partial class SynchronizedCollection<T> : IList<T>, IList
      {
           public SynchronizedCollection() { }
           public SynchronizedCollection(object syncRoot) { }
           public SynchronizedCollection(object syncRoot, IEnumerable<T> list) { }
           public SynchronizedCollection(object syncRoot, params T[] list) { }
           public int Count { get { return default(int); } }
           protected List<T> Items { get { return default(List<T>); } }
           public object SyncRoot { get { return default(object); } }
           public T this[int index] { get {return default(T); } set { } }
           public void Add(T item) { }
           public void Clear() { }
           public void CopyTo(T[] array, int index) {}
           public bool Contains(T item) { return default(bool); }
           public IEnumerator<T> GetEnumerator() { return default(IEnumerator<T>); }
           public int IndexOf(T item) { return default(int); }
           public void Insert(int index, T item) { }
           public bool Remove(T item) { return default(bool); }
           public void RemoveAt(int index) { }
           protected virtual void ClearItems() { }
           protected virtual void InsertItem(int index, T item) { }
           protected virtual void RemoveItem(int index) { }
           protected virtual void SetItem(int index, T item) { }
           bool ICollection<T>.IsReadOnly { get { return default(bool); } }
           IEnumerator IEnumerable.GetEnumerator() { return default(IEnumerator); }
           bool ICollection.IsSynchronized { get { return default(bool); } }
           object ICollection.SyncRoot { get { return default(object); } }
           void ICollection.CopyTo(Array array, int index) {}
           object IList.this[int index] { get { return default(object); } set { }}
           bool IList.IsReadOnly { get { return default(bool); } }
           bool IList.IsFixedSize { get { return default(bool); } }
           int IList.Add(object value) { return default(int); }
           bool IList.Contains(object value) { return default(bool); }
           int IList.IndexOf(object value) { return default(int); }
           void IList.Insert(int index, object value) { }
           void IList.Remove(object value) { }
      }
}
namespace System.ServiceModel
{
    public partial class CommunicationException : System.Exception
    {
        public CommunicationException() { }
        public CommunicationException(string message) { }
        public CommunicationException(string message, System.Exception innerException) { }
    }
    public enum CommunicationState
    {
        Closed = 4,
        Closing = 3,
        Created = 0,
        Faulted = 5,
        Opened = 2,
        Opening = 1,
    }
    [System.AttributeUsageAttribute((System.AttributeTargets)(1092), Inherited = false, AllowMultiple = false)]
    public sealed partial class DataContractFormatAttribute : System.Attribute
    {
        public DataContractFormatAttribute() { }
        public System.ServiceModel.OperationFormatStyle Style { get { return default(System.ServiceModel.OperationFormatStyle); } set { } }
    }
    public partial class DnsEndpointIdentity : System.ServiceModel.EndpointIdentity
    {
        public DnsEndpointIdentity(string dnsName) { }
    }
    public partial class EndpointAddress
    {
        public EndpointAddress(string uri) { }
        public EndpointAddress(System.Uri uri, params System.ServiceModel.Channels.AddressHeader[] addressHeaders) { }
        public EndpointAddress(System.Uri uri, System.ServiceModel.EndpointIdentity identity, params System.ServiceModel.Channels.AddressHeader[] addressHeaders) { }
        public static System.Uri AnonymousUri { get { return default(System.Uri); } }
        public System.ServiceModel.Channels.AddressHeaderCollection Headers { get { return default(System.ServiceModel.Channels.AddressHeaderCollection); } }
        public System.ServiceModel.EndpointIdentity Identity { get { return default(System.ServiceModel.EndpointIdentity); } }
        public bool IsAnonymous { get { return default(bool); } }
        public bool IsNone { get { return default(bool); } }
        public static System.Uri NoneUri { get { return default(System.Uri); } }
        public System.Uri Uri { get { return default(System.Uri); } }
        public void ApplyTo(System.ServiceModel.Channels.Message message) { }
        public override bool Equals(object obj) { return default(bool); }
        public override int GetHashCode() { return default(int); }
        public static bool operator ==(System.ServiceModel.EndpointAddress address1, System.ServiceModel.EndpointAddress address2) { return default(bool); }
        public static bool operator !=(System.ServiceModel.EndpointAddress address1, System.ServiceModel.EndpointAddress address2) { return default(bool); }
        public static System.ServiceModel.EndpointAddress ReadFrom(System.ServiceModel.Channels.AddressingVersion addressingVersion, System.Xml.XmlDictionaryReader reader) { return default(System.ServiceModel.EndpointAddress); }
        public override string ToString() { return default(string); }
        public void WriteContentsTo(System.ServiceModel.Channels.AddressingVersion addressingVersion, System.Xml.XmlDictionaryWriter writer) { }
    }
    public partial class EndpointAddressBuilder
    {
        public EndpointAddressBuilder() { }
        public EndpointAddressBuilder(System.ServiceModel.EndpointAddress address) { }
        public System.Collections.ObjectModel.Collection<System.ServiceModel.Channels.AddressHeader> Headers { get { return default(System.Collections.ObjectModel.Collection<System.ServiceModel.Channels.AddressHeader>); } }
        public System.ServiceModel.EndpointIdentity Identity { get { return default(System.ServiceModel.EndpointIdentity); } set { } }
        public System.Uri Uri { get { return default(System.Uri); } set { } }
        public System.ServiceModel.EndpointAddress ToEndpointAddress() { return default(System.ServiceModel.EndpointAddress); }
    }
    public abstract partial class EndpointIdentity
    {
        protected EndpointIdentity() { }
        public override bool Equals(object obj) { return default(bool); }
        public override int GetHashCode() { return default(int); }
        public override string ToString() { return default(string); }
    }
    public sealed partial class EnvelopeVersion
    {
        internal EnvelopeVersion() { }
        public string NextDestinationActorValue { get { return default(string); } }
        public static System.ServiceModel.EnvelopeVersion None { get { return default(System.ServiceModel.EnvelopeVersion); } }
        public static System.ServiceModel.EnvelopeVersion Soap11 { get { return default(System.ServiceModel.EnvelopeVersion); } }
        public static System.ServiceModel.EnvelopeVersion Soap12 { get { return default(System.ServiceModel.EnvelopeVersion); } }
        public string[] GetUltimateDestinationActorValues() { return default(string[]); }
        public override string ToString() { return default(string); }
    }
    public sealed partial class ExtensionCollection<T> : System.Collections.Generic.SynchronizedCollection<System.ServiceModel.IExtension<T>>, System.ServiceModel.IExtensionCollection<T> where T : System.ServiceModel.IExtensibleObject<T>
    {
        public ExtensionCollection(T owner) { }
        public ExtensionCollection(T owner, object syncRoot) : base(syncRoot) { }
        protected override void ClearItems() { }
        public E Find<E>() { return default(E); }
        public System.Collections.ObjectModel.Collection<E> FindAll<E>() { return default(System.Collections.ObjectModel.Collection<E>); }
        protected override void InsertItem(int index, System.ServiceModel.IExtension<T> item) { }
        protected override void RemoveItem(int index) { }
        protected override void SetItem(int index, System.ServiceModel.IExtension<T> item) { }
    }
    [System.AttributeUsageAttribute((System.AttributeTargets)(64), AllowMultiple = true, Inherited = false)]
    public sealed partial class FaultContractAttribute : System.Attribute
    {
        public FaultContractAttribute(System.Type detailType) { }
        public string Action { get { return default(string); } set { } }
        public System.Type DetailType { get { return default(System.Type); } }
        public string Name { get { return default(string); } set { } }
        public string Namespace { get { return default(string); } set { } }
    }
    public partial interface ICommunicationObject
    {
        System.ServiceModel.CommunicationState State { get; }
        event System.EventHandler Closed;
        event System.EventHandler Closing;
        event System.EventHandler Faulted;
        event System.EventHandler Opened;
        event System.EventHandler Opening;
        void Abort();
        System.IAsyncResult BeginClose(System.AsyncCallback callback, object state);
        System.IAsyncResult BeginClose(System.TimeSpan timeout, System.AsyncCallback callback, object state);
        System.IAsyncResult BeginOpen(System.AsyncCallback callback, object state);
        System.IAsyncResult BeginOpen(System.TimeSpan timeout, System.AsyncCallback callback, object state);
        void Close();
        void Close(System.TimeSpan timeout);
        void EndClose(System.IAsyncResult result);
        void EndOpen(System.IAsyncResult result);
        void Open();
        void Open(System.TimeSpan timeout);
    }
    public partial interface IContextChannel : System.ServiceModel.Channels.IChannel, System.ServiceModel.ICommunicationObject, System.ServiceModel.IExtensibleObject<System.ServiceModel.IContextChannel>
    {
        bool AllowOutputBatching { get; set; }
        System.ServiceModel.Channels.IInputSession InputSession { get; }
        System.ServiceModel.EndpointAddress LocalAddress { get; }
        System.TimeSpan OperationTimeout { get; set; }
        System.ServiceModel.Channels.IOutputSession OutputSession { get; }
        System.ServiceModel.EndpointAddress RemoteAddress { get; }
        string SessionId { get; }
    }
    public partial interface IExtensibleObject<T> where T : System.ServiceModel.IExtensibleObject<T>
    {
        System.ServiceModel.IExtensionCollection<T> Extensions { get; }
    }
    public partial interface IExtension<T> where T : System.ServiceModel.IExtensibleObject<T>
    {
        void Attach(T owner);
        void Detach(T owner);
    }
    public partial interface IExtensionCollection<T> : System.Collections.Generic.ICollection<System.ServiceModel.IExtension<T>>, System.Collections.Generic.IEnumerable<System.ServiceModel.IExtension<T>>, System.Collections.IEnumerable where T : System.ServiceModel.IExtensibleObject<T>
    {
        E Find<E>();
        System.Collections.ObjectModel.Collection<E> FindAll<E>();
    }
    [System.AttributeUsageAttribute((System.AttributeTargets)(384), AllowMultiple = false, Inherited = false)]
    public partial class MessageHeaderAttribute : MessageContractMemberAttribute
    {
        public bool MustUnderstand { get { return default(bool); } set { } } 
    }
    [System.AttributeUsageAttribute((System.AttributeTargets)(384), AllowMultiple = false, Inherited = false)]
    public sealed partial class MessageHeaderArrayAttribute : MessageHeaderAttribute
    {
        public MessageHeaderArrayAttribute() { }
    }
    [System.AttributeUsageAttribute((System.AttributeTargets)(384), Inherited = false)]
    public partial class MessageBodyMemberAttribute : System.ServiceModel.MessageContractMemberAttribute
    {
        public MessageBodyMemberAttribute() { }
        public int Order { get { return default(int); } set { } }
    }
    public partial class MessageHeaderException : System.ServiceModel.ProtocolException
    {
        public MessageHeaderException(string message) : base(default(string)) { }
        public MessageHeaderException(string message, bool isDuplicate) : base(default(string)) { }
        public MessageHeaderException(string message, System.Exception innerException) : base(default(string)) { }
        public MessageHeaderException(string message, string headerName, string ns) : base(default(string)) { }
        public MessageHeaderException(string message, string headerName, string ns, bool isDuplicate) : base(default(string)) { }
        public MessageHeaderException(string message, string headerName, string ns, bool isDuplicate, System.Exception innerException) : base(default(string)) { }
        public MessageHeaderException(string message, string headerName, string ns, System.Exception innerException) : base(default(string)) { }
        public string HeaderName { get { return default(string); } }
        public string HeaderNamespace { get { return default(string); } }
        public bool IsDuplicate { get { return default(bool); } }
    }
    [System.AttributeUsageAttribute((System.AttributeTargets)(12), AllowMultiple = false)]
    public sealed partial class MessageContractAttribute : System.Attribute
    {
        public MessageContractAttribute() { }
        public bool IsWrapped { get { return default(bool); } set { } }
        public string WrapperName { get { return default(string); } set { } }
        public string WrapperNamespace { get { return default(string); } set { } }
    }
    public abstract partial class MessageContractMemberAttribute : System.Attribute
    {
        protected MessageContractMemberAttribute() { }
        public string Name { get { return default(string); } set { } }
        public string Namespace { get { return default(string); } set { } }
    }
    [System.AttributeUsageAttribute((System.AttributeTargets)(384), AllowMultiple = false)]
    public sealed partial class MessagePropertyAttribute : System.Attribute
    {
        public MessagePropertyAttribute() { }
        public string Name { get { return default(string); } set { } }
    }
    public partial class MessageHeader<T>
    {
        public MessageHeader() { }
        public MessageHeader(T content) { }
        public MessageHeader(T content, bool mustUnderstand, string actor, bool relay) { }
        public string Actor { get { return default(string); } set { } }
        public T Content { get { return default(T); } set { } }
        public bool MustUnderstand { get { return default(bool); } set { } }
        public bool Relay { get { return default(bool); } set { } }
        public System.ServiceModel.Channels.MessageHeader GetUntypedHeader(string name, string ns) { return default(System.ServiceModel.Channels.MessageHeader); }
    }
    [System.AttributeUsageAttribute((System.AttributeTargets)(10240), Inherited = false)]
    public sealed partial class MessageParameterAttribute : System.Attribute
    {
        public MessageParameterAttribute() { }
        public string Name { get { return default(string); } set { } }
    }
    public sealed partial class OperationContext : System.ServiceModel.IExtensibleObject<System.ServiceModel.OperationContext>
    {
        public OperationContext(System.ServiceModel.IContextChannel channel) { }
        public static System.ServiceModel.OperationContext Current { get { return default(System.ServiceModel.OperationContext); } set { } }
        public System.ServiceModel.IExtensionCollection<System.ServiceModel.OperationContext> Extensions { get { return default(System.ServiceModel.IExtensionCollection<System.ServiceModel.OperationContext>); } }
        public System.ServiceModel.Channels.MessageHeaders IncomingMessageHeaders { get { return default(System.ServiceModel.Channels.MessageHeaders); } }
        public System.ServiceModel.Channels.MessageProperties IncomingMessageProperties { get { return default(System.ServiceModel.Channels.MessageProperties); } }
        public System.ServiceModel.Channels.MessageVersion IncomingMessageVersion { get { return default(System.ServiceModel.Channels.MessageVersion); } }
        public bool IsUserContext { get { return default(bool); } }
        public System.ServiceModel.Channels.MessageHeaders OutgoingMessageHeaders { get { return default(System.ServiceModel.Channels.MessageHeaders); } }
        public System.ServiceModel.Channels.MessageProperties OutgoingMessageProperties { get { return default(System.ServiceModel.Channels.MessageProperties); } }
        public System.ServiceModel.Channels.RequestContext RequestContext { get { return default(System.ServiceModel.Channels.RequestContext); } set { } }
        public event System.EventHandler OperationCompleted { add { } remove { } }
    }
    public sealed partial class OperationContextScope : System.IDisposable
    {
        public OperationContextScope(System.ServiceModel.IContextChannel channel) { }
        public OperationContextScope(System.ServiceModel.OperationContext context) { }
        public void Dispose() { }
    }
    [System.AttributeUsageAttribute((System.AttributeTargets)(64))]
    public sealed partial class OperationContractAttribute : System.Attribute
    {
        public OperationContractAttribute() { }
        public string Action { get { return default(string); } set { } }
        public bool AsyncPattern { get { return default(bool); } set { } }
        public bool IsOneWay { get { return default(bool); } set { } }
        public string Name { get { return default(string); } set { } }
        public string ReplyAction { get { return default(string); } set { } }
    }
    public enum OperationFormatStyle
    {
        Document = 0,
        Rpc = 1,
    }
    public partial class ProtocolException : System.ServiceModel.CommunicationException
    {
        public ProtocolException(string message) { }
        public ProtocolException(string message, System.Exception innerException) { }
    }
    public partial class QuotaExceededException : System.Exception
    {
        public QuotaExceededException(string message) { }
        public QuotaExceededException(string message, System.Exception innerException) { }
    }
    [System.AttributeUsageAttribute((System.AttributeTargets)(1028), Inherited = false, AllowMultiple = false)]
    public sealed partial class ServiceContractAttribute : System.Attribute
    {
        public ServiceContractAttribute() { }
        public System.Type CallbackContract { get { return default(System.Type); } set { } }
        public string ConfigurationName { get { return default(string); } set { } }
        public string Name { get { return default(string); } set { } }
        public string Namespace { get { return default(string); } set { } }
    }
    [System.AttributeUsageAttribute((System.AttributeTargets)(1092), Inherited = true, AllowMultiple = true)]
    public sealed partial class ServiceKnownTypeAttribute : System.Attribute
    {
        public ServiceKnownTypeAttribute(string methodName) { }
        public ServiceKnownTypeAttribute(string methodName, System.Type declaringType) { }
        public ServiceKnownTypeAttribute(System.Type type) { }
        public System.Type DeclaringType { get { return default(System.Type); } }
        public string MethodName { get { return default(string); } }
        public System.Type Type { get { return default(System.Type); } }
    }
    public partial class SpnEndpointIdentity : System.ServiceModel.EndpointIdentity
    {
        public SpnEndpointIdentity(string spnName) { }
        public static System.TimeSpan SpnLookupTime { get { return default(System.TimeSpan); } set { } }
    }
    public partial class UpnEndpointIdentity : System.ServiceModel.EndpointIdentity
    {
        public UpnEndpointIdentity(string upnName) { }
    }
    public partial class X509CertificateEndpointIdentity : System.ServiceModel.EndpointIdentity
    {
        public X509CertificateEndpointIdentity(System.Security.Cryptography.X509Certificates.X509Certificate2 certificate) { }
        public System.Security.Cryptography.X509Certificates.X509Certificate2Collection Certificates { get { return default(System.Security.Cryptography.X509Certificates.X509Certificate2Collection); } }
    }
    [System.AttributeUsageAttribute((System.AttributeTargets)(1092), Inherited = false, AllowMultiple = false)]
    public sealed partial class XmlSerializerFormatAttribute : System.Attribute
    {
        public XmlSerializerFormatAttribute() { }
        public System.ServiceModel.OperationFormatStyle Style { get { return default(System.ServiceModel.OperationFormatStyle); } set { } }
        public bool SupportFaults { get { return default(bool); } set { } }
    }
}
namespace System.ServiceModel.Channels
{
    public abstract partial class AddressHeader
    {
        protected AddressHeader() { }
        public abstract string Name { get; }
        public abstract string Namespace { get; }
        public static System.ServiceModel.Channels.AddressHeader CreateAddressHeader(string name, string ns, object value) { return default(System.ServiceModel.Channels.AddressHeader); }
        public static System.ServiceModel.Channels.AddressHeader CreateAddressHeader(string name, string ns, object value, System.Runtime.Serialization.XmlObjectSerializer serializer) { return default(System.ServiceModel.Channels.AddressHeader); }
        public override bool Equals(object obj) { return default(bool); }
        public virtual System.Xml.XmlDictionaryReader GetAddressHeaderReader() { return default(System.Xml.XmlDictionaryReader); }
        public override int GetHashCode() { return default(int); }
        public T GetValue<T>() { return default(T); }
        public T GetValue<T>(System.Runtime.Serialization.XmlObjectSerializer serializer) { return default(T); }
        protected abstract void OnWriteAddressHeaderContents(System.Xml.XmlDictionaryWriter writer);
        protected virtual void OnWriteStartAddressHeader(System.Xml.XmlDictionaryWriter writer) { }
        public System.ServiceModel.Channels.MessageHeader ToMessageHeader() { return default(System.ServiceModel.Channels.MessageHeader); }
        public void WriteAddressHeader(System.Xml.XmlDictionaryWriter writer) { }
        public void WriteAddressHeader(System.Xml.XmlWriter writer) { }
        public void WriteAddressHeaderContents(System.Xml.XmlDictionaryWriter writer) { }
        public void WriteStartAddressHeader(System.Xml.XmlDictionaryWriter writer) { }
    }
    public sealed partial class AddressHeaderCollection : System.Collections.ObjectModel.ReadOnlyCollection<System.ServiceModel.Channels.AddressHeader>
    {
        public AddressHeaderCollection() : base(default(System.Collections.Generic.IList<System.ServiceModel.Channels.AddressHeader>)) { }
        public AddressHeaderCollection(System.Collections.Generic.IEnumerable<System.ServiceModel.Channels.AddressHeader> addressHeaders) : base(default(System.Collections.Generic.IList<System.ServiceModel.Channels.AddressHeader>)) { }
        public void AddHeadersTo(System.ServiceModel.Channels.Message message) { }
        public System.ServiceModel.Channels.AddressHeader[] FindAll(string name, string ns) { return default(System.ServiceModel.Channels.AddressHeader[]); }
        public System.ServiceModel.Channels.AddressHeader FindHeader(string name, string ns) { return default(System.ServiceModel.Channels.AddressHeader); }
    }
    public sealed partial class AddressingVersion
    {
        internal AddressingVersion() { }
        public static System.ServiceModel.Channels.AddressingVersion None { get { return default(System.ServiceModel.Channels.AddressingVersion); } }
        public static System.ServiceModel.Channels.AddressingVersion WSAddressing10 { get { return default(System.ServiceModel.Channels.AddressingVersion); } }
        public override string ToString() { return default(string); }
    }
    public abstract partial class BodyWriter
    {
        protected BodyWriter(bool isBuffered) { }
        public bool IsBuffered { get { return default(bool); } }
        public System.ServiceModel.Channels.BodyWriter CreateBufferedCopy(int maxBufferSize) { return default(System.ServiceModel.Channels.BodyWriter); }
        protected virtual System.ServiceModel.Channels.BodyWriter OnCreateBufferedCopy(int maxBufferSize) { return default(System.ServiceModel.Channels.BodyWriter); }
        protected abstract void OnWriteBodyContents(System.Xml.XmlDictionaryWriter writer);
        public void WriteBodyContents(System.Xml.XmlDictionaryWriter writer) { }
    }
    public abstract partial class BufferManager
    {
        protected BufferManager() { }
        public abstract void Clear();
        public static System.ServiceModel.Channels.BufferManager CreateBufferManager(long maxBufferPoolSize, int maxBufferSize) { return default(System.ServiceModel.Channels.BufferManager); }
        public abstract void ReturnBuffer(byte[] buffer);
        public abstract byte[] TakeBuffer(int bufferSize);
    }
    public partial interface IChannel : System.ServiceModel.ICommunicationObject
    {
        T GetProperty<T>() where T : class;
    }
    public partial interface IInputSession : System.ServiceModel.Channels.ISession
    {
    }
    public partial interface IMessageProperty
    {
        System.ServiceModel.Channels.IMessageProperty CreateCopy();
    }
    public partial interface IOutputSession : System.ServiceModel.Channels.ISession
    {
    }
    public partial interface ISession
    {
        string Id { get; }
    }
    public abstract partial class Message : System.IDisposable
    {
        protected Message() { }
        public abstract System.ServiceModel.Channels.MessageHeaders Headers { get; }
        protected bool IsDisposed { get { return default(bool); } }
        public virtual bool IsEmpty { get { return default(bool); } }
        public virtual bool IsFault { get { return default(bool); } }
        public abstract System.ServiceModel.Channels.MessageProperties Properties { get; }
        public System.ServiceModel.Channels.MessageState State { get { return default(System.ServiceModel.Channels.MessageState); } }
        public abstract System.ServiceModel.Channels.MessageVersion Version { get; }
        public void Close() { }
        public System.ServiceModel.Channels.MessageBuffer CreateBufferedCopy(int maxBufferSize) { return default(System.ServiceModel.Channels.MessageBuffer); }
        public static System.ServiceModel.Channels.Message CreateMessage(System.ServiceModel.Channels.MessageVersion version, string action) { return default(System.ServiceModel.Channels.Message); }
        public static System.ServiceModel.Channels.Message CreateMessage(System.ServiceModel.Channels.MessageVersion version, string action, object body) { return default(System.ServiceModel.Channels.Message); }
        public static System.ServiceModel.Channels.Message CreateMessage(System.ServiceModel.Channels.MessageVersion version, string action, object body, System.Runtime.Serialization.XmlObjectSerializer serializer) { return default(System.ServiceModel.Channels.Message); }
        public static System.ServiceModel.Channels.Message CreateMessage(System.ServiceModel.Channels.MessageVersion version, string action, System.ServiceModel.Channels.BodyWriter body) { return default(System.ServiceModel.Channels.Message); }
        public static System.ServiceModel.Channels.Message CreateMessage(System.ServiceModel.Channels.MessageVersion version, string action, System.Xml.XmlDictionaryReader body) { return default(System.ServiceModel.Channels.Message); }
        public static System.ServiceModel.Channels.Message CreateMessage(System.ServiceModel.Channels.MessageVersion version, string action, System.Xml.XmlReader body) { return default(System.ServiceModel.Channels.Message); }
        public static System.ServiceModel.Channels.Message CreateMessage(System.Xml.XmlDictionaryReader envelopeReader, int maxSizeOfHeaders, System.ServiceModel.Channels.MessageVersion version) { return default(System.ServiceModel.Channels.Message); }
        public static System.ServiceModel.Channels.Message CreateMessage(System.Xml.XmlReader envelopeReader, int maxSizeOfHeaders, System.ServiceModel.Channels.MessageVersion version) { return default(System.ServiceModel.Channels.Message); }
        public T GetBody<T>() { return default(T); }
        public T GetBody<T>(System.Runtime.Serialization.XmlObjectSerializer serializer) { return default(T); }
        public string GetBodyAttribute(string localName, string ns) { return default(string); }
        public System.Xml.XmlDictionaryReader GetReaderAtBodyContents() { return default(System.Xml.XmlDictionaryReader); }
        protected virtual void OnBodyToString(System.Xml.XmlDictionaryWriter writer) { }
        protected virtual void OnClose() { }
        protected virtual System.ServiceModel.Channels.MessageBuffer OnCreateBufferedCopy(int maxBufferSize) { return default(System.ServiceModel.Channels.MessageBuffer); }
        protected virtual T OnGetBody<T>(System.Xml.XmlDictionaryReader reader) { return default(T); }
        protected virtual string OnGetBodyAttribute(string localName, string ns) { return default(string); }
        protected virtual System.Xml.XmlDictionaryReader OnGetReaderAtBodyContents() { return default(System.Xml.XmlDictionaryReader); }
        protected abstract void OnWriteBodyContents(System.Xml.XmlDictionaryWriter writer);
        protected virtual void OnWriteMessage(System.Xml.XmlDictionaryWriter writer) { }
        protected virtual void OnWriteStartBody(System.Xml.XmlDictionaryWriter writer) { }
        protected virtual void OnWriteStartEnvelope(System.Xml.XmlDictionaryWriter writer) { }
        protected virtual void OnWriteStartHeaders(System.Xml.XmlDictionaryWriter writer) { }
        void System.IDisposable.Dispose() { }
        public override string ToString() { return default(string); }
        public void WriteBody(System.Xml.XmlDictionaryWriter writer) { }
        public void WriteBody(System.Xml.XmlWriter writer) { }
        public void WriteBodyContents(System.Xml.XmlDictionaryWriter writer) { }
        public void WriteMessage(System.Xml.XmlDictionaryWriter writer) { }
        public void WriteMessage(System.Xml.XmlWriter writer) { }
        public void WriteStartBody(System.Xml.XmlDictionaryWriter writer) { }
        public void WriteStartBody(System.Xml.XmlWriter writer) { }
        public void WriteStartEnvelope(System.Xml.XmlDictionaryWriter writer) { }
    }
    public abstract partial class MessageBuffer : System.IDisposable
    {
        protected MessageBuffer() { }
        public abstract int BufferSize { get; }
        public virtual string MessageContentType { get { return default(string); } }
        public abstract void Close();
        public abstract System.ServiceModel.Channels.Message CreateMessage();
        void System.IDisposable.Dispose() { }
        public virtual void WriteMessage(System.IO.Stream stream) { }
    }
    public abstract partial class MessageEncoder
    {
        protected MessageEncoder() { }
        public abstract string ContentType { get; }
        public abstract string MediaType { get; }
        public abstract System.ServiceModel.Channels.MessageVersion MessageVersion { get; }
        public virtual T GetProperty<T>() where T : class { return default(T); }
        public virtual bool IsContentTypeSupported(string contentType) { return default(bool); }
        public System.ServiceModel.Channels.Message ReadMessage(System.ArraySegment<byte> buffer, System.ServiceModel.Channels.BufferManager bufferManager) { return default(System.ServiceModel.Channels.Message); }
        public abstract System.ServiceModel.Channels.Message ReadMessage(System.ArraySegment<byte> buffer, System.ServiceModel.Channels.BufferManager bufferManager, string contentType);
        public System.ServiceModel.Channels.Message ReadMessage(System.IO.Stream stream, int maxSizeOfHeaders) { return default(System.ServiceModel.Channels.Message); }
        public abstract System.ServiceModel.Channels.Message ReadMessage(System.IO.Stream stream, int maxSizeOfHeaders, string contentType);
        public override string ToString() { return default(string); }
        public System.ArraySegment<byte> WriteMessage(System.ServiceModel.Channels.Message message, int maxMessageSize, System.ServiceModel.Channels.BufferManager bufferManager) { return default(System.ArraySegment<byte>); }
        public abstract System.ArraySegment<byte> WriteMessage(System.ServiceModel.Channels.Message message, int maxMessageSize, System.ServiceModel.Channels.BufferManager bufferManager, int messageOffset);
        public abstract void WriteMessage(System.ServiceModel.Channels.Message message, System.IO.Stream stream);
    }
    public abstract partial class MessageHeader : System.ServiceModel.Channels.MessageHeaderInfo
    {
        protected MessageHeader() { }
        public override string Actor { get { return default(string); } }
        public override bool IsReferenceParameter { get { return default(bool); } }
        public override bool MustUnderstand { get { return default(bool); } }
        public override bool Relay { get { return default(bool); } }
        public static System.ServiceModel.Channels.MessageHeader CreateHeader(string name, string ns, object value) { return default(System.ServiceModel.Channels.MessageHeader); }
        public static System.ServiceModel.Channels.MessageHeader CreateHeader(string name, string ns, object value, bool mustUnderstand) { return default(System.ServiceModel.Channels.MessageHeader); }
        public static System.ServiceModel.Channels.MessageHeader CreateHeader(string name, string ns, object value, bool mustUnderstand, string actor) { return default(System.ServiceModel.Channels.MessageHeader); }
        public static System.ServiceModel.Channels.MessageHeader CreateHeader(string name, string ns, object value, bool mustUnderstand, string actor, bool relay) { return default(System.ServiceModel.Channels.MessageHeader); }
        public static System.ServiceModel.Channels.MessageHeader CreateHeader(string name, string ns, object value, System.Runtime.Serialization.XmlObjectSerializer serializer) { return default(System.ServiceModel.Channels.MessageHeader); }
        public static System.ServiceModel.Channels.MessageHeader CreateHeader(string name, string ns, object value, System.Runtime.Serialization.XmlObjectSerializer serializer, bool mustUnderstand) { return default(System.ServiceModel.Channels.MessageHeader); }
        public static System.ServiceModel.Channels.MessageHeader CreateHeader(string name, string ns, object value, System.Runtime.Serialization.XmlObjectSerializer serializer, bool mustUnderstand, string actor) { return default(System.ServiceModel.Channels.MessageHeader); }
        public static System.ServiceModel.Channels.MessageHeader CreateHeader(string name, string ns, object value, System.Runtime.Serialization.XmlObjectSerializer serializer, bool mustUnderstand, string actor, bool relay) { return default(System.ServiceModel.Channels.MessageHeader); }
        public virtual bool IsMessageVersionSupported(System.ServiceModel.Channels.MessageVersion messageVersion) { return default(bool); }
        protected abstract void OnWriteHeaderContents(System.Xml.XmlDictionaryWriter writer, System.ServiceModel.Channels.MessageVersion messageVersion);
        protected virtual void OnWriteStartHeader(System.Xml.XmlDictionaryWriter writer, System.ServiceModel.Channels.MessageVersion messageVersion) { }
        public override string ToString() { return default(string); }
        public void WriteHeader(System.Xml.XmlDictionaryWriter writer, System.ServiceModel.Channels.MessageVersion messageVersion) { }
        public void WriteHeader(System.Xml.XmlWriter writer, System.ServiceModel.Channels.MessageVersion messageVersion) { }
        protected void WriteHeaderAttributes(System.Xml.XmlDictionaryWriter writer, System.ServiceModel.Channels.MessageVersion messageVersion) { }
        public void WriteHeaderContents(System.Xml.XmlDictionaryWriter writer, System.ServiceModel.Channels.MessageVersion messageVersion) { }
        public void WriteStartHeader(System.Xml.XmlDictionaryWriter writer, System.ServiceModel.Channels.MessageVersion messageVersion) { }
    }
    public abstract partial class MessageHeaderInfo
    {
        protected MessageHeaderInfo() { }
        public abstract string Actor { get; }
        public abstract bool IsReferenceParameter { get; }
        public abstract bool MustUnderstand { get; }
        public abstract string Name { get; }
        public abstract string Namespace { get; }
        public abstract bool Relay { get; }
    }
    public sealed partial class MessageHeaders : System.Collections.Generic.IEnumerable<System.ServiceModel.Channels.MessageHeaderInfo>, System.Collections.IEnumerable
    {
        public MessageHeaders(System.ServiceModel.Channels.MessageHeaders collection) { }
        public MessageHeaders(System.ServiceModel.Channels.MessageVersion version) { }
        public MessageHeaders(System.ServiceModel.Channels.MessageVersion version, int initialSize) { }
        public string Action { get { return default(string); } set { } }
        public int Count { get { return default(int); } }
        public System.ServiceModel.EndpointAddress FaultTo { get { return default(System.ServiceModel.EndpointAddress); } set { } }
        public System.ServiceModel.EndpointAddress From { get { return default(System.ServiceModel.EndpointAddress); } set { } }
        public System.ServiceModel.Channels.MessageHeaderInfo this[int index] { get { return default(System.ServiceModel.Channels.MessageHeaderInfo); } }
        public System.Xml.UniqueId MessageId { get { return default(System.Xml.UniqueId); } set { } }
        public System.ServiceModel.Channels.MessageVersion MessageVersion { get { return default(System.ServiceModel.Channels.MessageVersion); } }
        public System.ServiceModel.Channels.UnderstoodHeaders UnderstoodHeaders { get { return default(System.ServiceModel.Channels.UnderstoodHeaders); } }
        public System.Xml.UniqueId RelatesTo { get { return default(System.Xml.UniqueId); } set { } }
        public System.ServiceModel.EndpointAddress ReplyTo { get { return default(System.ServiceModel.EndpointAddress); } set { } }
        public System.Uri To { get { return default(System.Uri); } set { } }
        public void Add(System.ServiceModel.Channels.MessageHeader header) { }
        public void Clear() { }
        public void CopyHeaderFrom(System.ServiceModel.Channels.Message message, int headerIndex) { }
        public void CopyHeaderFrom(System.ServiceModel.Channels.MessageHeaders collection, int headerIndex) { }
        public void CopyHeadersFrom(System.ServiceModel.Channels.Message message) { }
        public void CopyHeadersFrom(System.ServiceModel.Channels.MessageHeaders collection) { }
        public void CopyTo(System.ServiceModel.Channels.MessageHeaderInfo[] array, int index) { }
        public int FindHeader(string name, string ns) { return default(int); }
        public int FindHeader(string name, string ns, params string[] actors) { return default(int); }
        public System.Collections.Generic.IEnumerator<System.ServiceModel.Channels.MessageHeaderInfo> GetEnumerator() { return default(System.Collections.Generic.IEnumerator<System.ServiceModel.Channels.MessageHeaderInfo>); }
        public T GetHeader<T>(int index) { return default(T); }
        public T GetHeader<T>(int index, System.Runtime.Serialization.XmlObjectSerializer serializer) { return default(T); }
        public T GetHeader<T>(string name, string ns) { return default(T); }
        public T GetHeader<T>(string name, string ns, System.Runtime.Serialization.XmlObjectSerializer serializer) { return default(T); }
        public T GetHeader<T>(string name, string ns, params string[] actors) { return default(T); }
        public System.Xml.XmlDictionaryReader GetReaderAtHeader(int headerIndex) { return default(System.Xml.XmlDictionaryReader); }
        public bool HaveMandatoryHeadersBeenUnderstood() { return default(bool); }
        public bool HaveMandatoryHeadersBeenUnderstood(params string[] actors) { return default(bool); }
        public void Insert(int headerIndex, System.ServiceModel.Channels.MessageHeader header) { }
        public void RemoveAll(string name, string ns) { }
        public void RemoveAt(int headerIndex) { }
        public void SetAction(System.Xml.XmlDictionaryString action) { }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return default(System.Collections.IEnumerator); }
        public void WriteHeader(int headerIndex, System.Xml.XmlDictionaryWriter writer) { }
        public void WriteHeader(int headerIndex, System.Xml.XmlWriter writer) { }
        public void WriteHeaderContents(int headerIndex, System.Xml.XmlDictionaryWriter writer) { }
        public void WriteHeaderContents(int headerIndex, System.Xml.XmlWriter writer) { }
        public void WriteStartHeader(int headerIndex, System.Xml.XmlDictionaryWriter writer) { }
        public void WriteStartHeader(int headerIndex, System.Xml.XmlWriter writer) { }
    }
    public sealed partial class MessageProperties : System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<string, object>>, System.Collections.Generic.IDictionary<string, object>, System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, object>>, System.Collections.IEnumerable, System.IDisposable
    {
        public MessageProperties() { }
        public MessageProperties(System.ServiceModel.Channels.MessageProperties properties) { }
        public bool AllowOutputBatching { get { return default(bool); } set { } }
        public int Count { get { return default(int); } }
        public System.ServiceModel.Channels.MessageEncoder Encoder { get { return default(System.ServiceModel.Channels.MessageEncoder); } set { } }
        public bool IsFixedSize { get { return default(bool); } }
        public object this[string name] { get { return default(object); } set { } }
        public System.Collections.Generic.ICollection<string> Keys { get { return default(System.Collections.Generic.ICollection<string>); } }
        bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<System.String, System.Object>>.IsReadOnly { get { return default(bool); } }
        public System.Collections.Generic.ICollection<object> Values { get { return default(System.Collections.Generic.ICollection<object>); } }
        public System.Uri Via { get { return default(System.Uri); } set { } }
        public void Add(string name, object property) { }
        public void Clear() { }
        public bool ContainsKey(string name) { return default(bool); }
        public void CopyProperties(System.ServiceModel.Channels.MessageProperties properties) { }
        public void Dispose() { }
        public bool Remove(string name) { return default(bool); }
        void System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<System.String, System.Object>>.Add(System.Collections.Generic.KeyValuePair<string, object> pair) { }
        bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<System.String, System.Object>>.Contains(System.Collections.Generic.KeyValuePair<string, object> pair) { return default(bool); }
        void System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<System.String, System.Object>>.CopyTo(System.Collections.Generic.KeyValuePair<string, object>[] array, int index) { }
        bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<System.String, System.Object>>.Remove(System.Collections.Generic.KeyValuePair<string, object> pair) { return default(bool); }
        System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<string, object>> System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<System.String, System.Object>>.GetEnumerator() { return default(System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<string, object>>); }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return default(System.Collections.IEnumerator); }
        public bool TryGetValue(string name, out object value) { value = default(object); return default(bool); }
    }
    public enum MessageState
    {
        Closed = 4,
        Copied = 3,
        Created = 0,
        Read = 1,
        Written = 2,
    }
    public sealed partial class MessageVersion
    {
        internal MessageVersion() { }
        public System.ServiceModel.Channels.AddressingVersion Addressing { get { return default(System.ServiceModel.Channels.AddressingVersion); } }
        public static System.ServiceModel.Channels.MessageVersion Default { get { return default(System.ServiceModel.Channels.MessageVersion); } }
        public System.ServiceModel.EnvelopeVersion Envelope { get { return default(System.ServiceModel.EnvelopeVersion); } }
        public static System.ServiceModel.Channels.MessageVersion None { get { return default(System.ServiceModel.Channels.MessageVersion); } }
        public static System.ServiceModel.Channels.MessageVersion Soap11 { get { return default(System.ServiceModel.Channels.MessageVersion); } }
        public static System.ServiceModel.Channels.MessageVersion Soap12WSAddressing10 { get { return default(System.ServiceModel.Channels.MessageVersion); } }
        public static System.ServiceModel.Channels.MessageVersion CreateVersion(System.ServiceModel.EnvelopeVersion envelopeVersion) { return default(System.ServiceModel.Channels.MessageVersion); }
        public static System.ServiceModel.Channels.MessageVersion CreateVersion(System.ServiceModel.EnvelopeVersion envelopeVersion, System.ServiceModel.Channels.AddressingVersion addressingVersion) { return default(System.ServiceModel.Channels.MessageVersion); }
        public override bool Equals(object obj) { return default(bool); }
        public override int GetHashCode() { return default(int); }
        public override string ToString() { return default(string); }
    }
    public abstract partial class RequestContext : System.IDisposable
    {
        protected RequestContext() { }
        public abstract System.ServiceModel.Channels.Message RequestMessage { get; }
        public abstract void Abort();
        public abstract System.IAsyncResult BeginReply(System.ServiceModel.Channels.Message message, System.AsyncCallback callback, object state);
        public abstract System.IAsyncResult BeginReply(System.ServiceModel.Channels.Message message, System.TimeSpan timeout, System.AsyncCallback callback, object state);
        public abstract void Close();
        public abstract void Close(System.TimeSpan timeout);
        protected virtual void Dispose(bool disposing) { }
        public abstract void EndReply(System.IAsyncResult result);
        public abstract void Reply(System.ServiceModel.Channels.Message message);
        public abstract void Reply(System.ServiceModel.Channels.Message message, System.TimeSpan timeout);
        void System.IDisposable.Dispose() { }
    }
    public sealed partial class UnderstoodHeaders : System.Collections.Generic.IEnumerable<System.ServiceModel.Channels.MessageHeaderInfo>
    {
        internal UnderstoodHeaders() { }
        public void Add(System.ServiceModel.Channels.MessageHeaderInfo headerInfo) { }
        public bool Contains(System.ServiceModel.Channels.MessageHeaderInfo headerInfo) { return default(bool); }
        public System.Collections.Generic.IEnumerator<System.ServiceModel.Channels.MessageHeaderInfo> GetEnumerator() { return default(System.Collections.Generic.IEnumerator<System.ServiceModel.Channels.MessageHeaderInfo>); }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return default(System.Collections.IEnumerator); }
        public void Remove(System.ServiceModel.Channels.MessageHeaderInfo headerInfo) { }
    }
}
