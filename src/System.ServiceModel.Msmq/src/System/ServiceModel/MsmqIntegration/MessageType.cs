// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.ServiceModel.MsmqIntegration
{
    // Mirrors System.Messaging.MessageType. Indicates whether an
    // incoming message is a normal application payload, a response to
    // a previous send, a system report, or an acknowledgment message
    // emitted by MSMQ in response to a requested AcknowledgeTypes flag.
    public enum MessageType
    {
        Normal = 1,
        Response = 2,
        Report = 3,
        Acknowledgment = 6,
    }
}
