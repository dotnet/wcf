// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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

