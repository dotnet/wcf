// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;

namespace System.ServiceModel.Channels
{
    public sealed class UnderstoodHeaders : IEnumerable<MessageHeaderInfo>
    {
        private MessageHeaders _messageHeaders;
        private bool _modified;

        internal UnderstoodHeaders(MessageHeaders messageHeaders, bool modified)
        {
            _messageHeaders = messageHeaders;
            _modified = modified;
        }

        internal bool Modified
        {
            get { return _modified; }
            set { _modified = value; }
        }

        public void Add(MessageHeaderInfo headerInfo)
        {
            _messageHeaders.AddUnderstood(headerInfo);
            _modified = true;
        }

        public bool Contains(MessageHeaderInfo headerInfo)
        {
            return _messageHeaders.IsUnderstood(headerInfo);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public IEnumerator<MessageHeaderInfo> GetEnumerator()
        {
            return _messageHeaders.GetUnderstoodEnumerator();
        }

        public void Remove(MessageHeaderInfo headerInfo)
        {
            _messageHeaders.RemoveUnderstood(headerInfo);
            _modified = true;
        }
    }
}
