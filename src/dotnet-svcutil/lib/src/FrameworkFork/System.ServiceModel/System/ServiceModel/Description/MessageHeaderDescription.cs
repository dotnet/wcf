// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.ServiceModel.Description
{
    public class MessageHeaderDescription : MessagePartDescription
    {
        private bool _mustUnderstand;
        private bool _relay;
        private string _actor;
        private bool _typedHeader;
        private bool _isUnknownHeader;

        public MessageHeaderDescription(string name, string ns)
            : base(name, ns)
        {
        }

        internal MessageHeaderDescription(MessageHeaderDescription other)
            : base(other)
        {
            this.MustUnderstand = other.MustUnderstand;
            this.Relay = other.Relay;
            this.Actor = other.Actor;
            this.TypedHeader = other.TypedHeader;
            this.IsUnknownHeaderCollection = other.IsUnknownHeaderCollection;
        }

        internal override MessagePartDescription Clone()
        {
            return new MessageHeaderDescription(this);
        }

        [DefaultValue(null)]
        public string Actor
        {
            get { return _actor; }
            set { _actor = value; }
        }

        [DefaultValue(false)]
        public bool MustUnderstand
        {
            get { return _mustUnderstand; }
            set { _mustUnderstand = value; }
        }

        [DefaultValue(false)]
        public bool Relay
        {
            get { return _relay; }
            set { _relay = value; }
        }

        [DefaultValue(false)]
        public bool TypedHeader
        {
            get { return _typedHeader; }
            set { _typedHeader = value; }
        }

        internal bool IsUnknownHeaderCollection
        {
            get
            {
                return _isUnknownHeader;
            }
            set
            {
                _isUnknownHeader = value;
            }
        }
    }
}
