// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Security.Tokens;
using System.Collections.ObjectModel;
using System.IdentityModel.Policy;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Xml;
using System.Runtime.CompilerServices;

namespace System.ServiceModel.Security
{
    internal class SecurityStandardsManager
    {
#pragma warning disable 0649 // Remove this once we do real implementation, this prevents "field is never assigned to" warning
        private static SecurityStandardsManager s_instance;
        private readonly MessageSecurityVersion _messageSecurityVersion;
        private readonly TrustDriver _trustDriver;
#pragma warning restore 0649


        [MethodImpl(MethodImplOptions.NoInlining)]
        public SecurityStandardsManager()
            : this(WSSecurityTokenSerializer.DefaultInstance)
        {
        }

        public SecurityStandardsManager(SecurityTokenSerializer tokenSerializer)
            : this(MessageSecurityVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11, tokenSerializer)
        {
        }

        public SecurityStandardsManager(MessageSecurityVersion messageSecurityVersion, SecurityTokenSerializer tokenSerializer)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        public static SecurityStandardsManager DefaultInstance
        {
            get
            {
                if (s_instance == null)
                    s_instance = new SecurityStandardsManager();
                return s_instance;
            }
        }

        public SecurityVersion SecurityVersion
        {
            get { return _messageSecurityVersion == null ? null : _messageSecurityVersion.SecurityVersion; }
        }

        public MessageSecurityVersion MessageSecurityVersion
        {
            get { return _messageSecurityVersion; }
        }

        internal TrustDriver TrustDriver
        {
            get { return _trustDriver; }
        }
    }
}
