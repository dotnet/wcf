// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.ServiceModel.Channels;

namespace System.ServiceModel
{
    // Wraps MSMQ native error codes (MQ_ERROR_*) returned from the
    // underlying queue. Maps them to standard WCF exception types via
    // the Normalized property so callers see meaningful exceptions
    // (EndpointNotFoundException, TimeoutException, ...) instead of a
    // raw integer error.
    [Serializable]
    public class MsmqException : ExternalException
    {
        [NonSerialized] private bool? _faultSender;
        [NonSerialized] private bool? _faultReceiver;
        [NonSerialized] private Type _outerExceptionType;

        public MsmqException() { }
        public MsmqException(string message) : base(message) { }
        public MsmqException(string message, int error) : base(message, error) { }
        public MsmqException(string message, Exception inner) : base(message, inner) { }
#pragma warning disable SYSLIB0051 // legacy formatter-based serialization support
        protected MsmqException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#pragma warning restore SYSLIB0051

        internal bool FaultSender
        {
            get
            {
                TuneBehavior();
                return _faultSender.Value;
            }
        }

        internal bool FaultReceiver
        {
            get
            {
                TuneBehavior();
                return _faultReceiver.Value;
            }
        }

        // Returns the most appropriate WCF exception for this MSMQ error,
        // wrapping `this` as the inner exception. Used by the channel
        // factory to surface a standard exception (e.g.
        // EndpointNotFoundException for QueueNotFound) instead of leaking
        // the raw integer error code to callers.
        internal Exception Normalized
        {
            get
            {
                TuneBehavior();
                if (_outerExceptionType == null)
                {
                    return this;
                }
                return (Exception)Activator.CreateInstance(_outerExceptionType, new object[] { Message, this });
            }
        }

        // Exposed internally to make the WCF-style exception type
        // available to channel code without taking a runtime call on
        // every send. Tests reach the property by reflection.
        internal Type NormalizedType
        {
            get
            {
                TuneBehavior();
                return _outerExceptionType;
            }
        }

        private void TuneBehavior()
        {
            if (_faultSender.HasValue && _faultReceiver.HasValue)
            {
                return;
            }

            // Mirrors the (int -> behavior) table from the .NET Framework
            // reference source. Codes are the native MQ_ERROR_* values.
            switch (unchecked((uint)ErrorCode))
            {
                // ----- Configuration errors -----
                case MsmqErrorCodes.AccessDenied:
                    _faultSender = true; _faultReceiver = true; _outerExceptionType = typeof(AddressAccessDeniedException); break;
                case MsmqErrorCodes.QueueDeleted:
                case MsmqErrorCodes.QueueNotFound:
                    _faultSender = true; _faultReceiver = true; _outerExceptionType = typeof(EndpointNotFoundException); break;
                case MsmqErrorCodes.IllegalFormatName:
                case MsmqErrorCodes.IllegalQueuePathName:
                    _faultSender = false; _faultReceiver = false; _outerExceptionType = typeof(ArgumentException); break;
                case MsmqErrorCodes.UnsupportedFormatNameOperation:
                    _faultSender = true; _faultReceiver = true; _outerExceptionType = typeof(ArgumentException); break;
                case MsmqErrorCodes.SharingViolation:
                    _faultSender = true; _faultReceiver = true; _outerExceptionType = typeof(AddressAccessDeniedException); break;

                // ----- Transient errors -----
                case MsmqErrorCodes.IOTimeout:
                    _faultSender = false; _faultReceiver = false; _outerExceptionType = typeof(TimeoutException); break;
                case MsmqErrorCodes.QueueNotAvailable:
                case MsmqErrorCodes.RemoteMachineNotAvailable:
                case MsmqErrorCodes.ServiceNotAvailable:
                    _faultSender = false; _faultReceiver = true; _outerExceptionType = typeof(EndpointNotFoundException); break;
                case MsmqErrorCodes.InsufficientResources:
                case MsmqErrorCodes.MessageStorageFailed:
                    _faultSender = true; _faultReceiver = true; _outerExceptionType = typeof(CommunicationException); break;
                case MsmqErrorCodes.TransactionEnlist:
                    _faultSender = false; _faultReceiver = true; _outerExceptionType = typeof(CommunicationException); break;
                case MsmqErrorCodes.TransactionImport:
                    _faultSender = true; _faultReceiver = true; _outerExceptionType = typeof(CommunicationException); break;
                case MsmqErrorCodes.TransactionUsage:
                    _faultSender = true; _faultReceiver = true; _outerExceptionType = typeof(InvalidOperationException); break;
                case MsmqErrorCodes.StaleHandle:
                    _faultSender = false; _faultReceiver = false; _outerExceptionType = typeof(InvalidOperationException); break;

                default:
                    _faultSender = true; _faultReceiver = true; _outerExceptionType = null; break;
            }
        }
    }

    // MQ_ERROR_* native MSMQ error codes (a subset that's normalized by
    // MsmqException). Values are exact matches for the native winerror
    // codes used by mqrt.dll, and also for MSMQ.Messaging's
    // MessageQueueErrorCode enum members of the same name.
    internal static class MsmqErrorCodes
    {
        internal const uint AccessDenied                  = 0xC00E0025;
        internal const uint QueueNotFound                 = 0xC00E0003;
        internal const uint QueueDeleted                  = 0xC00E005A;
        internal const uint IllegalFormatName             = 0xC00E001E;
        internal const uint IllegalQueuePathName          = 0xC00E0014;
        internal const uint UnsupportedFormatNameOperation = 0xC00E0020;
        internal const uint SharingViolation              = 0xC00E0009;
        internal const uint IOTimeout                     = 0xC00E001B;
        internal const uint QueueNotAvailable             = 0xC00E0026;
        internal const uint RemoteMachineNotAvailable     = 0xC00E000E;
        internal const uint ServiceNotAvailable           = 0xC00E000B;
        internal const uint InsufficientResources         = 0xC00E0027;
        internal const uint MessageStorageFailed          = 0xC00E002A;
        internal const uint TransactionEnlist             = 0xC00E0058;
        internal const uint TransactionImport             = 0xC00E0051;
        internal const uint TransactionUsage              = 0xC00E0050;
        internal const uint StaleHandle                   = 0xC00E0006;
    }
}
