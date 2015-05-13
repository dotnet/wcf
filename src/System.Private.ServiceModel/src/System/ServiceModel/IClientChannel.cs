// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel.Channels;
using System.Collections.Generic;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.Net.Security;
using System.ServiceModel.Security.Tokens;

namespace System.ServiceModel
{
    public interface IClientChannel : IContextChannel, IDisposable
    {
        bool AllowInitializationUI { get; set; }
        bool DidInteractiveInitialization { get; }
        Uri Via { get; }

        event EventHandler<UnknownMessageReceivedEventArgs> UnknownMessageReceived;

        void DisplayInitializationUI();
        IAsyncResult BeginDisplayInitializationUI(AsyncCallback callback, object state);
        void EndDisplayInitializationUI(IAsyncResult result);
    }
}
