// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

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
