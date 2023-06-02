// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.IO;
using System.Net.Security;
using System.Runtime;
using System.ServiceModel.Security;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal class UnixPosixIdentitySecurityUpgradeProvider : StreamSecurityUpgradeProvider
    {
        internal const string UnixPosixUpgradeString = "application/unixposix";

        public UnixPosixIdentitySecurityUpgradeProvider(UnixPosixIdentityBindingElement bindingElement, BindingContext context)
            : base(context.Binding)
        {

        }

        internal IdentityVerifier IdentityVerifier { get; private set; }

        public override StreamUpgradeInitiator CreateUpgradeInitiator(EndpointAddress remoteAddress, Uri via)
        {
            this.ThrowIfDisposedOrNotOpen();
            return new UnixPosixIdentitySecurityUpgradeInitiator();
        }

        protected override void OnAbort()
        {
        }

        protected override void OnClose(TimeSpan timeout)
        {
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return Task.CompletedTask.ToApm(callback, state);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            result.ToApmEnd();
        }

        protected internal override Task OnCloseAsync(TimeSpan timeout)
        {
            return Task.CompletedTask;
        }

        protected override void OnOpen(TimeSpan timeout)
        {
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return Task.CompletedTask.ToApm(callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            result.ToApmEnd();
        }

        protected internal override Task OnOpenAsync(TimeSpan timeout)
        {
            return Task.CompletedTask;
        }

        protected override void OnOpened()
        {
            base.OnOpened();

            if (IdentityVerifier == null)
            {
                IdentityVerifier = IdentityVerifier.CreateDefault();
            }
        }

        private class UnixPosixIdentitySecurityUpgradeInitiator : StreamSecurityUpgradeInitiator
        {
            private string _upgradeString = UnixPosixUpgradeString;
            public UnixPosixIdentitySecurityUpgradeInitiator()
            {
            }

            public override SecurityMessageProperty GetRemoteSecurity() 
            {
                return null;;
            }

            public override string GetNextUpgrade()
            {
                string localUpgradeString = _upgradeString;
                _upgradeString = null;
                return localUpgradeString;
            }
           
            public override Task<Stream> InitiateUpgradeAsync(Stream stream)
            {
                if (stream == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(stream));
                }

                return Task.FromResult(stream);
            }
        }
    }
}
