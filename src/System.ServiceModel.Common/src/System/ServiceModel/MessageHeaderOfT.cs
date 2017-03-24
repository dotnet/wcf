// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.Generic;
using System.Runtime;
using System.ServiceModel.Channels;
using System.Threading;

namespace System.ServiceModel
{
    public class MessageHeader<T>
    {
        private string _actor;
        private bool _mustUnderstand;
        private bool _relay;
        private T _content;

        public MessageHeader()
        {
        }

        public MessageHeader(T content)
            : this(content, false, "", false)
        {
        }

        public MessageHeader(T content, bool mustUnderstand, string actor, bool relay)
        {
            _content = content;
            _mustUnderstand = mustUnderstand;
            _actor = actor;
            _relay = relay;
        }

        public string Actor
        {
            get { return _actor; }
            set { _actor = value; }
        }

        public T Content
        {
            get { return _content; }
            set { _content = value; }
        }

        public bool MustUnderstand
        {
            get { return _mustUnderstand; }
            set { _mustUnderstand = value; }
        }

        public bool Relay
        {
            get { return _relay; }
            set { _relay = value; }
        }

        public MessageHeader GetUntypedHeader(string name, string ns)
        {
            return MessageHeader.CreateHeader(name, ns, _content, _mustUnderstand, _actor, _relay);
        }
    }
}
