// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel;

namespace System.ServiceModel.Description
{
    public class MessageBodyDescription
    {
        private XmlName _wrapperName;
        private string _wrapperNs;
        private MessagePartDescriptionCollection _parts;
        private MessagePartDescription _returnValue;

        public MessageBodyDescription()
        {
            _parts = new MessagePartDescriptionCollection();
        }

        internal MessageBodyDescription(MessageBodyDescription other)
        {
            this.WrapperName = other.WrapperName;
            this.WrapperNamespace = other.WrapperNamespace;
            _parts = new MessagePartDescriptionCollection();
            foreach (MessagePartDescription mpd in other.Parts)
            {
                this.Parts.Add(mpd.Clone());
            }
            if (other.ReturnValue != null)
            {
                this.ReturnValue = other.ReturnValue.Clone();
            }
        }

        internal MessageBodyDescription Clone()
        {
            return new MessageBodyDescription(this);
        }

        public MessagePartDescriptionCollection Parts
        {
            get { return _parts; }
        }

        [DefaultValue(null)]
        public MessagePartDescription ReturnValue
        {
            get { return _returnValue; }
            set { _returnValue = value; }
        }

        [DefaultValue(null)]
        public string WrapperName
        {
            get { return _wrapperName == null ? null : _wrapperName.EncodedName; }
            set { _wrapperName = new XmlName(value, true /*isEncoded*/); }
        }

        [DefaultValue(null)]
        public string WrapperNamespace
        {
            get { return _wrapperNs; }
            set { _wrapperNs = value; }
        }
    }
}
