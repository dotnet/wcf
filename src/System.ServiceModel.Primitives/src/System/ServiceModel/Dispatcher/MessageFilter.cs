// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel.Channels;
using System.Runtime.Serialization;
using System.Runtime;

namespace System.ServiceModel.Dispatcher
{
    /// <summary>
    /// Abstract base class for different classes of message filters
    /// </summary>
    [DataContract]
    //[KnownType(typeof(XPathMessageFilter))]
    //[KnownType(typeof(ActionMessageFilter))]
    //[KnownType(typeof(MatchAllMessageFilter))]
    //[KnownType(typeof(MatchNoneMessageFilter))]
    //[KnownType(typeof(EndpointAddressMessageFilter))]
    public abstract class MessageFilter
    {
        protected MessageFilter()
        {
        }

        protected internal virtual IMessageFilterTable<FilterData> CreateFilterTable<FilterData>()
        {
            return null;
        }

        /// <summary>
        /// Tests whether the filter matches the given message.
        /// </summary>                
        public abstract bool Match(MessageBuffer buffer);

        /// <summary>
        /// Tests whether the filter matches the given message without examining its body.
        /// Note: since this method never probes the message body, it should NOT close the message
        /// If the filter probes the message body, then the filter must THROW an Exception. The filter should not return false
        /// This is deliberate - we don't want to produce false positives. 
        /// </summary>
        public abstract bool Match(Message message);
    }
}
