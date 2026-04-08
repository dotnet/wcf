// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

using System.Collections.ObjectModel;

namespace System.ServiceModel
{
    public abstract partial class ChannelFactory : System.IAsyncDisposable
    {
        System.Threading.Tasks.ValueTask IAsyncDisposable.DisposeAsync() { return default; }
    }
    public abstract partial class ClientBase<TChannel> : System.IAsyncDisposable
    {
        public System.Threading.Tasks.Task CloseAsync() { return default; }
        System.Threading.Tasks.ValueTask IAsyncDisposable.DisposeAsync() { return default; }
    }
    public enum TransactionFlowOption
    {
        NotAllowed = 0,
        Allowed = 1,
        Mandatory = 2,
    }
    [System.AttributeUsageAttribute(System.AttributeTargets.Method)]
    public sealed partial class TransactionFlowAttribute : System.Attribute, System.ServiceModel.Description.IOperationBehavior
    {
        public TransactionFlowAttribute(System.ServiceModel.TransactionFlowOption transactions) { }
        public System.ServiceModel.TransactionFlowOption Transactions { get { return default; } }
        void System.ServiceModel.Description.IOperationBehavior.AddBindingParameters(System.ServiceModel.Description.OperationDescription description, System.ServiceModel.Channels.BindingParameterCollection parameters) { }
        void System.ServiceModel.Description.IOperationBehavior.ApplyClientBehavior(System.ServiceModel.Description.OperationDescription description, System.ServiceModel.Dispatcher.ClientOperation proxy) { }
        void System.ServiceModel.Description.IOperationBehavior.ApplyDispatchBehavior(System.ServiceModel.Description.OperationDescription description, System.ServiceModel.Dispatcher.DispatchOperation dispatch) { }
        void System.ServiceModel.Description.IOperationBehavior.Validate(System.ServiceModel.Description.OperationDescription description) { }
    }
    public abstract partial class TransactionProtocol
    {
        public static System.ServiceModel.TransactionProtocol Default { get { return default; } }
        public static System.ServiceModel.TransactionProtocol OleTransactions { get { return default; } }
        public static System.ServiceModel.TransactionProtocol WSAtomicTransaction11 { get { return default; } }
        public static System.ServiceModel.TransactionProtocol WSAtomicTransactionOctober2004 { get { return default; } }
    }
}
namespace System.ServiceModel.Channels
{
    public abstract partial class MessageEncoder
    {
        public virtual System.Threading.Tasks.ValueTask<Message> ReadMessageAsync(System.IO.Stream stream, int maxSizeOfHeaders, string contentType) => default;
        public virtual System.Threading.Tasks.ValueTask<Message> ReadMessageAsync(System.ArraySegment<byte> buffer, System.ServiceModel.Channels.BufferManager bufferManager, string contentType) => default;
        public virtual Threading.Tasks.ValueTask WriteMessageAsync(System.ServiceModel.Channels.Message message, System.IO.Stream stream) => default;
        public virtual System.Threading.Tasks.ValueTask<System.ArraySegment<byte>> WriteMessageAsync(System.ServiceModel.Channels.Message message, int maxMessageSize, System.ServiceModel.Channels.BufferManager bufferManager, int messageOffset) => default;
    }
    public interface ITransportCompressionSupport
    {
        bool IsCompressionFormatSupported(CompressionFormat compressionFormat);
    }
    public sealed partial class TransactionFlowBindingElement : System.ServiceModel.Channels.BindingElement
    {
        public TransactionFlowBindingElement() { }
        public TransactionFlowBindingElement(System.ServiceModel.TransactionProtocol transactionProtocol) { }
        public bool AllowWildcardAction { get { return default; } set { } }
        public System.ServiceModel.TransactionProtocol TransactionProtocol { get { return default; } set { } }
        public override System.ServiceModel.Channels.IChannelFactory<TChannel> BuildChannelFactory<TChannel>(System.ServiceModel.Channels.BindingContext context) { return default; }
        public override bool CanBuildChannelFactory<TChannel>(System.ServiceModel.Channels.BindingContext context) { return default; }
        public override System.ServiceModel.Channels.BindingElement Clone() { return default; }
        public override T GetProperty<T>(System.ServiceModel.Channels.BindingContext context) { return default; }
        public bool ShouldSerializeTransactionProtocol() { return default; }
    }
    public sealed partial class TransactionMessageProperty
    {
        internal TransactionMessageProperty() { }
        public System.Transactions.Transaction Transaction { get { return default; } }
        public static void Set(System.Transactions.Transaction transaction, System.ServiceModel.Channels.Message message) { }
    }
}
namespace System.ServiceModel.Description
{
    public partial class DataContractSerializerOperationBehavior : System.ServiceModel.Description.IOperationBehavior
    {
        public System.Runtime.Serialization.ISerializationSurrogateProvider SerializationSurrogateProvider { get { return default; } set { } }
    }
}
namespace System.IdentityModel.Selectors
{
    public abstract partial class SecurityTokenProvider
    {
        public System.Threading.Tasks.Task<System.IdentityModel.Tokens.SecurityToken> GetTokenAsync(System.TimeSpan timeout) { return default; }
        public System.Threading.Tasks.Task<System.IdentityModel.Tokens.SecurityToken> RenewTokenAsync(System.TimeSpan timeout, System.IdentityModel.Tokens.SecurityToken tokenToBeRenewed) { return default; }
        public System.Threading.Tasks.Task CancelTokenAsync(System.TimeSpan timeout, System.IdentityModel.Tokens.SecurityToken token) { return default; }
        protected virtual System.Threading.Tasks.Task<System.IdentityModel.Tokens.SecurityToken> GetTokenCoreAsync(System.TimeSpan timeout) { return default; }
        protected virtual System.Threading.Tasks.Task<System.IdentityModel.Tokens.SecurityToken> RenewTokenCoreAsync(System.TimeSpan timeout, System.IdentityModel.Tokens.SecurityToken tokenToBeRenewed) { return default; }
        protected virtual System.Threading.Tasks.Task CancelTokenCoreAsync(System.TimeSpan timeout, System.IdentityModel.Tokens.SecurityToken token) { return default; }
    }
}
namespace System.ServiceModel.Dispatcher
{
    public partial class ChannelDispatcher
    {
        public Collection<IErrorHandler> ErrorHandlers { get { return default; } }
    }
    public interface IErrorHandler
    {
        void ProvideFault(Exception error, System.ServiceModel.Channels.MessageVersion version, ref System.ServiceModel.Channels.Message fault);
        bool HandleError(Exception error);
    }
}
