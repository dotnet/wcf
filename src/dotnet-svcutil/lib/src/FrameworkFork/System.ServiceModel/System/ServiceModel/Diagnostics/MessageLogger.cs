// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;

namespace System.ServiceModel.Diagnostics
{
    [Flags]
    internal enum MessageLoggingSource : int
    {
        None = 0,
        TransportReceive = 2,
        TransportSend = 4,
        Transport = TransportReceive | TransportSend,
        ServiceLevelReceiveDatagram = 16,
        ServiceLevelSendDatagram = 32,
        ServiceLevelReceiveRequest = 64,
        ServiceLevelSendRequest = 128,
        ServiceLevelReceiveReply = 256,
        ServiceLevelSendReply = 512,
        ServiceLevelReceive = ServiceLevelReceiveReply | ServiceLevelReceiveRequest | ServiceLevelReceiveDatagram,
        ServiceLevelSend = ServiceLevelSendReply | ServiceLevelSendRequest | ServiceLevelSendDatagram,
        ServiceLevelService = ServiceLevelSendReply | ServiceLevelReceiveRequest | ServiceLevelReceiveDatagram,
        ServiceLevelProxy = ServiceLevelReceiveReply | ServiceLevelSendRequest | ServiceLevelSendDatagram,
        ServiceLevel = ServiceLevelReceive | ServiceLevelSend,
        Malformed = 1024,
        LastChance = 2048,
        All = int.MaxValue
    }

    internal static class MessageLogger
    {
        /// <summary>
        /// Property that guards the calls into the methods 
        /// and this disables logging. 
        /// </summary>
        public static bool LoggingEnabled { get { return false; } }

        internal static void LogMessage(ref Channels.Message message, int arg1)
        {
            throw NotImplemented.ByDesign;
        }

        public static bool ShouldLogMalformed { get; set; }

        internal static void LogMessage(Stream stream, MessageLoggingSource messageLoggingSource)
        {
            throw NotImplemented.ByDesign;
        }

        public static bool LogMessagesAtTransportLevel { get; set; }

        internal static void LogMessage(ref Channels.Message message, MessageLoggingSource messageLoggingSource)
        {
            throw NotImplemented.ByDesign;
        }

        public static bool LogMessagesAtServiceLevel { get; set; }

        internal static void LogMessage(ArraySegment<byte> arraySegment, MessageLoggingSource messageLoggingSource)
        {
            throw NotImplemented.ByDesign;
        }

        internal static void LogMessage(ref Channels.Message message, Microsoft.Xml.XmlDictionaryReader xmlDictionaryReader, MessageLoggingSource messageLoggingSource)
        {
            throw NotImplemented.ByDesign;
        }
    }
}

