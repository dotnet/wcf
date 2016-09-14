// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel;
using System.Xml;

namespace System.ServiceModel.Security
{
    public interface ISecureConversationSession : ISecuritySession
    {
        void WriteSessionTokenIdentifier(XmlDictionaryWriter writer);
        bool TryReadSessionTokenIdentifier(XmlReader reader);
    }
}
