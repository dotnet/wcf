// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime.Serialization;

namespace System.ServiceModel
{
    [Serializable]
    public class MsmqPoisonMessageException : PoisonMessageException
    {
        private long _messageLookupId = 0;

        public MsmqPoisonMessageException() { }
        public MsmqPoisonMessageException(string message) : base(message) { }
        public MsmqPoisonMessageException(string message, Exception innerException) : base(message, innerException) { }
        public MsmqPoisonMessageException(long messageLookupId) : this(messageLookupId, null) { }
        public MsmqPoisonMessageException(long messageLookupId, Exception innerException)
            : base(SR.MsmqPoisonMessage, innerException)
        {
            _messageLookupId = messageLookupId;
        }

        public long MessageLookupId
        {
            get { return _messageLookupId; }
        }

        protected MsmqPoisonMessageException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _messageLookupId = (long)info.GetValue("messageLookupId", typeof(long));
        }

#pragma warning disable SYSLIB0051 // Type or member is obsolete: legacy formatter-based serialization support.
        [Obsolete]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("messageLookupId", _messageLookupId);
        }
#pragma warning restore SYSLIB0051
    }
}
