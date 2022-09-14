// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace System.ServiceModel.Security
{
    internal class SecuritySessionSecurityTokenAuthenticator
    {
    }

    internal class SessionActionFilter : HeaderFilter
    {
        private SecurityStandardsManager _standardsManager;
        private string[] _actions;

        public SessionActionFilter(SecurityStandardsManager standardsManager, params string[] actions)
        {
            _actions = actions;
            _standardsManager = standardsManager;
        }

        public override bool Match(Message message)
        {
            for (int i = 0; i < _actions.Length; ++i)
            {
                if (message.Headers.Action == _actions[i])
                {
                    return _standardsManager.DoesMessageContainSecurityHeader(message);
                }
            }

            return false;
        }
    }
}
