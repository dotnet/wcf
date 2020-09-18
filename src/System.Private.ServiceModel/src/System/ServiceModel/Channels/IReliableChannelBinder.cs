// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal delegate void BinderExceptionHandler(IReliableChannelBinder sender, Exception exception);

    internal interface IReliableChannelBinder
    {
        IChannel Channel { get; }
        bool Connected { get; }
        TimeSpan DefaultSendTimeout { get; }
        bool HasSession { get; }
        EndpointAddress LocalAddress { get; }
        EndpointAddress RemoteAddress { get; }
        CommunicationState State { get; }

        event BinderExceptionHandler Faulted;
        event BinderExceptionHandler OnException;

        void Abort();

        Task CloseAsync(TimeSpan timeout);
        Task CloseAsync(TimeSpan timeout, MaskingMode maskingMode);
        Task OpenAsync(TimeSpan timeout);
        Task SendAsync(Message message, TimeSpan timeout);
        Task SendAsync(Message message, TimeSpan timeout, MaskingMode maskingMode);

        Task<(bool success, RequestContext requestContext)> TryReceiveAsync(TimeSpan timeout);
        Task<(bool success, RequestContext requestContext)> TryReceiveAsync(TimeSpan timeout, MaskingMode maskingMode);

        ISession GetInnerSession();
        void HandleException(Exception e);
        bool IsHandleable(Exception e);
        void SetMaskingMode(RequestContext context, MaskingMode maskingMode);
        RequestContext WrapRequestContext(RequestContext context);
    }

    internal interface IClientReliableChannelBinder : IReliableChannelBinder
    {
        Uri Via { get; }
        event EventHandler ConnectionLost;

        Task<bool> EnsureChannelForRequestAsync();

        Task<Message> RequestAsync(Message message, TimeSpan timeout);
        Task<Message> RequestAsync(Message message, TimeSpan timeout, MaskingMode maskingMode);
    }
}
