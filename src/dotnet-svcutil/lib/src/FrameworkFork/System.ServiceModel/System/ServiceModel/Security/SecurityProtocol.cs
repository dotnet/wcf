// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IdentityModel.Policy;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Runtime;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Diagnostics;
using System.ServiceModel.Security.Tokens;

namespace System.ServiceModel.Security
{
    // See SecurityProtocolFactory for contracts on subclasses etc

    // SecureOutgoingMessage and VerifyIncomingMessage take message as
    // ref parameters (instead of taking a message and returning a
    // message) to reduce the likelihood that a caller will forget to
    // do the rest of the processing with the modified message object.
    // Especially, on the sender-side, not sending the modified
    // message will result in sending it with an unencrypted body.
    // Correspondingly, the async versions have out parameters instead
    // of simple return values.
    internal abstract class SecurityProtocol : ISecurityCommunicationObject
    {
        private WrapperSecurityCommunicationObject _communicationObject;

        public TimeSpan DefaultCloseTimeout
        {
            get { throw ExceptionHelper.PlatformNotSupported(); }
        }

        public TimeSpan DefaultOpenTimeout
        {
            get { throw ExceptionHelper.PlatformNotSupported(); }
        }

        public void OnAbort() { throw ExceptionHelper.PlatformNotSupported(); }
        public IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state) { throw ExceptionHelper.PlatformNotSupported(); }
        public IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state) { throw ExceptionHelper.PlatformNotSupported(); }
        public void OnClose(TimeSpan timeout) { throw ExceptionHelper.PlatformNotSupported(); }
        public void OnClosed() { throw ExceptionHelper.PlatformNotSupported(); }
        public void OnClosing() { throw ExceptionHelper.PlatformNotSupported(); }
        public void OnEndClose(IAsyncResult result) { throw ExceptionHelper.PlatformNotSupported(); }
        public void OnEndOpen(IAsyncResult result) { throw ExceptionHelper.PlatformNotSupported(); }
        public void OnFaulted() { throw ExceptionHelper.PlatformNotSupported(); }
        public void OnOpen(TimeSpan timeout) { throw ExceptionHelper.PlatformNotSupported(); }
        public void OnOpened() { throw ExceptionHelper.PlatformNotSupported(); }
        public void OnOpening() { throw ExceptionHelper.PlatformNotSupported(); }

        public void Close(bool aborted, TimeSpan timeout)
        {
            if (aborted)
            {
                _communicationObject.Abort();
            }
            else
            {
                _communicationObject.Close(timeout);
            }
        }
    }
}
