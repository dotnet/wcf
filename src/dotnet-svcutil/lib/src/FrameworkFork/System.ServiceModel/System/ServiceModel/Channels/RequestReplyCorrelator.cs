// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime;
using System.ServiceModel.Diagnostics;
using Microsoft.Xml;

namespace System.ServiceModel.Channels
{
    internal class RequestReplyCorrelator : IRequestReplyCorrelator
    {
        private Dictionary<Key, object> _states;

        internal RequestReplyCorrelator()
        {
            _states = new Dictionary<Key, object>();
        }

        void IRequestReplyCorrelator.Add<T>(Message request, T state)
        {
            UniqueId messageId = request.Headers.MessageId;
            Type stateType = typeof(T);
            Key key = new Key(messageId, stateType);

            // add the correlator key to the request, this will be needed for cleaning up the correlator table in case of 
            // channel aborting or faulting while there are pending requests
            ICorrelatorKey value = state as ICorrelatorKey;
            if (value != null)
            {
                value.RequestCorrelatorKey = key;
            }

            lock (_states)
            {
                _states.Add(key, state);
            }
        }

        T IRequestReplyCorrelator.Find<T>(Message reply, bool remove)
        {
            UniqueId relatesTo = GetRelatesTo(reply);
            Type stateType = typeof(T);
            Key key = new Key(relatesTo, stateType);
            T value;

            lock (_states)
            {
                value = (T)_states[key];

                if (remove)
                    _states.Remove(key);
            }

            return value;
        }

        // This method is used to remove the request from the correlator table when the
        // reply is lost. This will avoid leaking the correlator table in cases where the 
        // channel faults or aborts while there are pending requests.
        internal void RemoveRequest(ICorrelatorKey request)
        {
            Fx.Assert(request != null, "request cannot be null");
            if (request.RequestCorrelatorKey != null)
            {
                lock (_states)
                {
                    _states.Remove(request.RequestCorrelatorKey);
                }
            }
        }

        private UniqueId GetRelatesTo(Message reply)
        {
            UniqueId relatesTo = reply.Headers.RelatesTo;
            if (relatesTo == null)
                throw TraceUtility.ThrowHelperError(new ArgumentException(SRServiceModel.SuppliedMessageIsNotAReplyItHasNoRelatesTo0), reply);
            return relatesTo;
        }

        internal static bool AddressReply(Message reply, Message request)
        {
            ReplyToInfo info = RequestReplyCorrelator.ExtractReplyToInfo(request);
            return RequestReplyCorrelator.AddressReply(reply, info);
        }

        internal static bool AddressReply(Message reply, ReplyToInfo info)
        {
            EndpointAddress destination = null;

            if (info.HasFaultTo && (reply.IsFault))
            {
                destination = info.FaultTo;
            }
            else if (info.HasReplyTo)
            {
                destination = info.ReplyTo;
            }

            if (destination != null)
            {
                destination.ApplyTo(reply);
                return !destination.IsNone;
            }
            else
            {
                return true;
            }
        }

        internal static ReplyToInfo ExtractReplyToInfo(Message message)
        {
            return new ReplyToInfo(message);
        }

        internal static void PrepareRequest(Message request)
        {
            MessageHeaders requestHeaders = request.Headers;

            if (requestHeaders.MessageId == null)
            {
                requestHeaders.MessageId = new UniqueId();
            }

            request.Properties.AllowOutputBatching = false;
            if (TraceUtility.PropagateUserActivity || TraceUtility.ShouldPropagateActivity)
            {
                TraceUtility.AddAmbientActivityToMessage(request);
            }
        }

        internal static void PrepareReply(Message reply, UniqueId messageId)
        {
            if (object.ReferenceEquals(messageId, null))
                throw TraceUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.MissingMessageID), reply);

            MessageHeaders replyHeaders = reply.Headers;

            if (object.ReferenceEquals(replyHeaders.RelatesTo, null))
            {
                replyHeaders.RelatesTo = messageId;
            }

            if (TraceUtility.PropagateUserActivity || TraceUtility.ShouldPropagateActivity)
            {
                TraceUtility.AddAmbientActivityToMessage(reply);
            }
        }

        internal static void PrepareReply(Message reply, Message request)
        {
            UniqueId messageId = request.Headers.MessageId;

            if (messageId != null)
            {
                MessageHeaders replyHeaders = reply.Headers;

                if (object.ReferenceEquals(replyHeaders.RelatesTo, null))
                {
                    replyHeaders.RelatesTo = messageId;
                }
            }

            if (TraceUtility.PropagateUserActivity || TraceUtility.ShouldPropagateActivity)
            {
                TraceUtility.AddAmbientActivityToMessage(reply);
            }
        }

        internal struct ReplyToInfo
        {
            private readonly EndpointAddress _faultTo;
            private readonly EndpointAddress _from;
            private readonly EndpointAddress _replyTo;

            internal ReplyToInfo(Message message)
            {
                _faultTo = message.Headers.FaultTo;
                _replyTo = message.Headers.ReplyTo;
                _from = null;
            }

            internal EndpointAddress FaultTo
            {
                get { return _faultTo; }
            }

            internal EndpointAddress From
            {
                get { return _from; }
            }

            internal bool HasFaultTo
            {
                get { return !IsTrivial(this.FaultTo); }
            }

            internal bool HasFrom
            {
                get { return !IsTrivial(this.From); }
            }

            internal bool HasReplyTo
            {
                get { return !IsTrivial(this.ReplyTo); }
            }

            internal EndpointAddress ReplyTo
            {
                get { return _replyTo; }
            }

            private bool IsTrivial(EndpointAddress address)
            {
                // Note: even if address.IsAnonymous, it may have identity, reference parameters, etc.
                return (address == null) || (address == EndpointAddress.AnonymousAddress);
            }
        }

        internal class Key
        {
            internal UniqueId MessageId;
            internal Type StateType;

            internal Key(UniqueId messageId, Type stateType)
            {
                MessageId = messageId;
                StateType = stateType;
            }

            public override bool Equals(object obj)
            {
                Key other = obj as Key;
                if (other == null)
                    return false;
                return other.MessageId == this.MessageId && other.StateType == this.StateType;
            }

            [SuppressMessage(FxCop.Category.Usage, "CA2303:FlagTypeGetHashCode", Justification = "The hashcode is not used for identity purposes for embedded types.")]
            public override int GetHashCode()
            {
                return MessageId.GetHashCode() ^ StateType.GetHashCode();
            }

            public override string ToString()
            {
                return typeof(Key).ToString() + ": {" + MessageId + ", " + StateType.ToString() + "}";
            }
        }
    }
}
