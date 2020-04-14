// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.ServiceModel
{
    using System.ServiceModel.Channels;

    public sealed class NonDualMessageSecurityOverHttp : MessageSecurityOverHttp
    {
        internal const bool DefaultEstablishSecurityContext = true;

        private bool _establishSecurityContext;

        public NonDualMessageSecurityOverHttp()
            : base()
        {
            _establishSecurityContext = DefaultEstablishSecurityContext;
        }

        public bool EstablishSecurityContext
        {
            get
            {
                return _establishSecurityContext;
            }
            set
            {
                _establishSecurityContext = value;
            }
        }

        protected override bool IsSecureConversationEnabled()
        {
            return _establishSecurityContext;
        }
    }
}
