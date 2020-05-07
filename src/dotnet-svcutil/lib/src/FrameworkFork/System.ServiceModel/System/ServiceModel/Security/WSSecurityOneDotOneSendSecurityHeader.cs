// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Diagnostics;
using System.IO;
using System.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.ServiceModel.Security.Tokens;
using Microsoft.Xml;
using System.ServiceModel.Diagnostics;

using ISignatureValueSecurityElement = System.IdentityModel.ISignatureValueSecurityElement;

namespace System.ServiceModel.Security
{
    internal sealed class WSSecurityOneDotOneSendSecurityHeader : WSSecurityOneDotZeroSendSecurityHeader
    {
        public WSSecurityOneDotOneSendSecurityHeader(Message message, string actor, bool mustUnderstand, bool relay,
            SecurityStandardsManager standardsManager,
            SecurityAlgorithmSuite algorithmSuite,
            MessageDirection direction)
            : base(message, actor, mustUnderstand, relay, standardsManager, algorithmSuite, direction)
        {
        }
    }
}

