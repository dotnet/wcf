// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

using System.Threading.Tasks;

namespace System.ServiceModel
{
    public abstract partial class ChannelFactory : System.IAsyncDisposable
    {
        ValueTask IAsyncDisposable.DisposeAsync() { return default; }
    }
    public abstract partial class ClientBase<TChannel> : System.IAsyncDisposable
    {
        public Task CloseAsync() { return default; }
        ValueTask IAsyncDisposable.DisposeAsync() { return default; }
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
