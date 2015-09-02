// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens;
using System.ServiceModel;
using System.Xml;

namespace System.IdentityModel.Selectors
{
    public abstract class SecurityTokenResolver
    {
        private class SimpleTokenResolver : SecurityTokenResolver
        {
            private ReadOnlyCollection<SecurityToken> _tokens;
            private bool _canMatchLocalId;

            public SimpleTokenResolver(ReadOnlyCollection<SecurityToken> tokens, bool canMatchLocalId)
            {
                if (tokens == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("tokens");

                _tokens = tokens;
                _canMatchLocalId = canMatchLocalId;
            }
        }
    }
}
