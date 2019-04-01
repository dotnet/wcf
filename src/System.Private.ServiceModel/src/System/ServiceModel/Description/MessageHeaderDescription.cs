// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ComponentModel;

namespace System.ServiceModel.Description
{
    public class MessageHeaderDescription : MessagePartDescription
    {
        private bool _relay;
        private bool _isUnknownHeader;

        public MessageHeaderDescription(string name, string ns)
            : base(name, ns)
        {
        }

        internal MessageHeaderDescription(MessageHeaderDescription other)
            : base(other)
        {
            MustUnderstand = other.MustUnderstand;
            Relay = other.Relay;
            Actor = other.Actor;
            TypedHeader = other.TypedHeader;
            IsUnknownHeaderCollection = other.IsUnknownHeaderCollection;
        }

        internal override MessagePartDescription Clone()
        {
            return new MessageHeaderDescription(this);
        }

        [DefaultValue(null)]
        public string Actor { get; set; }

        [DefaultValue(false)]
        public bool MustUnderstand { get; set; }

        [DefaultValue(false)]
        public bool Relay
        {
            get { return _relay; }
            set { _relay = value; }
        }

        [DefaultValue(false)]
        public bool TypedHeader { get; set; }

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
