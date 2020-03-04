// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Security.Tokens;
using Microsoft.Xml;

namespace System.ServiceModel.Security
{
    internal class WSSecurityOneDotOneReceiveSecurityHeader : WSSecurityOneDotZeroReceiveSecurityHeader
    {
        public WSSecurityOneDotOneReceiveSecurityHeader(Message message, string actor, bool mustUnderstand, bool relay,
            SecurityStandardsManager standardsManager,
            SecurityAlgorithmSuite algorithmSuite,
            int headerIndex, MessageDirection direction)
            : base(message, actor, mustUnderstand, relay, standardsManager, algorithmSuite, headerIndex, direction)
        {
        }
    }
}
