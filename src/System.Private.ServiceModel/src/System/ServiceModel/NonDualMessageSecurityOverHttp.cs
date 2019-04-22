// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel
{
    public sealed class NonDualMessageSecurityOverHttp : MessageSecurityOverHttp
    {
        internal const bool DefaultEstablishSecurityContext = true;

        public NonDualMessageSecurityOverHttp()
            : base()
        {
            EstablishSecurityContext = DefaultEstablishSecurityContext;
        }

        public bool EstablishSecurityContext { get; set; }

        protected override bool IsSecureConversationEnabled()
        {
            return EstablishSecurityContext;
        }
    }
}
