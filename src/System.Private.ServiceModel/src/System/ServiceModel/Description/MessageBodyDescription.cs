// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ComponentModel;

namespace System.ServiceModel.Description
{
    public class MessageBodyDescription
    {
        private XmlName _wrapperName;
        private MessagePartDescription _returnValue;

        public MessageBodyDescription()
        {
            Parts = new MessagePartDescriptionCollection();
        }

        internal MessageBodyDescription(MessageBodyDescription other)
        {
            WrapperName = other.WrapperName;
            WrapperNamespace = other.WrapperNamespace;
            Parts = new MessagePartDescriptionCollection();
            foreach (MessagePartDescription mpd in other.Parts)
            {
                Parts.Add(mpd.Clone());
            }
            if (other.ReturnValue != null)
            {
                ReturnValue = other.ReturnValue.Clone();
            }
        }

        internal MessageBodyDescription Clone()
        {
            return new MessageBodyDescription(this);
        }

        public MessagePartDescriptionCollection Parts { get; }

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
        public string WrapperNamespace { get; set; }
    }
}
