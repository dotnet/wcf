// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security.Tokens;
//Issue #31 in progress
//using HexBinary = System.Runtime.Remoting.Metadata.W3cXsd2001.SoapHexBinary;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Runtime.Serialization;

using KeyIdentifierEntry = System.ServiceModel.Security.WSSecurityTokenSerializer.KeyIdentifierEntry;
using KeyIdentifierClauseEntry = System.ServiceModel.Security.WSSecurityTokenSerializer.KeyIdentifierClauseEntry;
using TokenEntry = System.ServiceModel.Security.WSSecurityTokenSerializer.TokenEntry;
using StrEntry = System.ServiceModel.Security.WSSecurityTokenSerializer.StrEntry;

namespace System.ServiceModel.Security
{
    class WSSecurityXXX2005 : WSSecurityJan2004
    {
        public WSSecurityXXX2005(WSSecurityTokenSerializer tokenSerializer)
            : this(tokenSerializer, new SamlSerializer())
        {
        }

        public WSSecurityXXX2005(WSSecurityTokenSerializer tokenSerializer, SamlSerializer samlSerializer)
            : base(tokenSerializer, samlSerializer)
        {
        }

        public override void PopulateTokenEntries(IList<TokenEntry> tokenEntryList)
        {
            PopulateJan2004TokenEntries(tokenEntryList);
            tokenEntryList.Add(new WSSecurityXXX2005.WrappedKeyTokenEntry(this.WSSecurityTokenSerializer));
            tokenEntryList.Add(new WSSecurityXXX2005.SamlTokenEntry(this.WSSecurityTokenSerializer, this.SamlSerializer));
        }

        new class SamlTokenEntry : WSSecurityJan2004.SamlTokenEntry
        {
            public SamlTokenEntry(WSSecurityTokenSerializer tokenSerializer, SamlSerializer samlSerializer)
                : base(tokenSerializer, samlSerializer)
            {
            }

            public override string TokenTypeUri { get { return SecurityXXX2005Strings.SamlTokenType; } }
        }

        new class WrappedKeyTokenEntry : WSSecurityJan2004.WrappedKeyTokenEntry
        {
            public WrappedKeyTokenEntry(WSSecurityTokenSerializer tokenSerializer)
                : base(tokenSerializer)
            {
            }

            public override string TokenTypeUri { get { return SecurityXXX2005Strings.EncryptedKeyTokenType; } }
        }
    }
}

