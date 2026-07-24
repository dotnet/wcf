// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime.Versioning;
using System.ServiceModel.Channels;

namespace System.ServiceModel.MsmqIntegration
{
    // Carries MSMQ message metadata (label, priority, correlation, …)
    // alongside the message body. Mirrors the netfx public type so
    // applications porting from .NET Framework can keep their usage
    // unchanged. The enums (AcknowledgeTypes, Acknowledgment,
    // MessageType, MessagePriority) live in this same namespace; their
    // values match the MQMSG_* / MQMSG_CLASS_* native constants
    // exactly so they round-trip through the underlying MSMQ ABI
    // without conversion loss.
    [SupportedOSPlatform("windows")]
    public sealed class MsmqIntegrationMessageProperty
    {
        public const string Name = "MsmqIntegrationMessageProperty";

        public object Body { get; set; }
        public AcknowledgeTypes? AcknowledgeType { get; set; }
        public Acknowledgment? Acknowledgment { get; internal set; }
        public Uri AdministrationQueue { get; set; }
        public int? AppSpecific { get; set; }
        public DateTime? ArrivedTime { get; internal set; }
        public bool? Authenticated { get; internal set; }
        public int? BodyType { get; set; }
        public string CorrelationId { get; set; }
        public Uri DestinationQueue { get; internal set; }
        public byte[] Extension { get; set; }
        public string Id { get; internal set; }
        public string Label { get; set; }
        public MessageType? MessageType { get; internal set; }
        public MessagePriority? Priority { get; set; }
        public Uri ResponseQueue { get; set; }
        public byte[] SenderId { get; internal set; }
        public DateTime? SentTime { get; internal set; }
        public TimeSpan? TimeToReachQueue { get; set; }

        public static MsmqIntegrationMessageProperty Get(System.ServiceModel.Channels.Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            return message.Properties.TryGetValue(Name, out object value) ? value as MsmqIntegrationMessageProperty : null;
        }

        // Copies the user-settable MSMQ properties onto the outgoing
        // native message. Each property maps to a PROPID_M_* slot whose
        // VT_* matches the value's natural width:
        //
        //   AcknowledgeType        VT_UI1   PROPID_M_ACKNOWLEDGE
        //   AdministrationQueue    VT_LPWSTR + VT_UI4 (length)
        //   AppSpecific            VT_UI4   PROPID_M_APPSPECIFIC
        //   BodyType               VT_UI4   PROPID_M_BODY_TYPE
        //   CorrelationId          VT_VECTOR|VT_UI1 (20 bytes)
        //   Extension              VT_VECTOR|VT_UI1 + VT_UI4 (length)
        //   Label                  VT_LPWSTR + VT_UI4 (length)
        //   Priority               VT_UI1   PROPID_M_PRIORITY
        //   ResponseQueue          VT_LPWSTR + VT_UI4 (length)
        //   TimeToReachQueue       VT_UI4   PROPID_M_TIME_TO_REACH_QUEUE
        internal void ApplyTo(NativeMsmqMessage message)
        {
            if (AcknowledgeType.HasValue)
            {
                message.SetByte(UnsafeNativeMethods.PROPID_M_ACKNOWLEDGE, (byte)(int)AcknowledgeType.Value);
            }
            if (AdministrationQueue != null)
            {
                message.SetWideString(
                    UnsafeNativeMethods.PROPID_M_ADMIN_QUEUE,
                    MsmqUri.UriToFormatNameByScheme(AdministrationQueue),
                    UnsafeNativeMethods.PROPID_M_ADMIN_QUEUE_LEN);
            }
            if (AppSpecific.HasValue)
            {
                message.SetUInt32(UnsafeNativeMethods.PROPID_M_APPSPECIFIC, unchecked((uint)AppSpecific.Value));
            }
            if (BodyType.HasValue)
            {
                message.SetUInt32(UnsafeNativeMethods.PROPID_M_BODY_TYPE, unchecked((uint)BodyType.Value));
            }
            if (!string.IsNullOrEmpty(CorrelationId))
            {
                message.SetByteVector(
                    UnsafeNativeMethods.PROPID_M_CORRELATIONID,
                    NativeMsmqMessage.ParseMessageId(CorrelationId),
                    lengthPropId: 0);
            }
            if (Extension != null)
            {
                message.SetByteVector(
                    UnsafeNativeMethods.PROPID_M_EXTENSION,
                    Extension,
                    UnsafeNativeMethods.PROPID_M_EXTENSION_LEN);
            }
            if (!string.IsNullOrEmpty(Label))
            {
                message.SetWideString(
                    UnsafeNativeMethods.PROPID_M_LABEL,
                    Label,
                    UnsafeNativeMethods.PROPID_M_LABEL_LEN);
            }
            if (Priority.HasValue)
            {
                message.SetByte(UnsafeNativeMethods.PROPID_M_PRIORITY, (byte)(int)Priority.Value);
            }
            if (ResponseQueue != null)
            {
                message.SetWideString(
                    UnsafeNativeMethods.PROPID_M_RESP_QUEUE,
                    MsmqUri.UriToFormatNameByScheme(ResponseQueue),
                    UnsafeNativeMethods.PROPID_M_RESP_QUEUE_LEN);
            }
            if (TimeToReachQueue.HasValue)
            {
                message.SetTimeToReachQueue(TimeToReachQueue.Value);
            }
        }
    }
}
