// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ServiceModel.Channels;

namespace WcfService.TestResources.CustomBindings
{
    class CustomTextMessageEncoderFactory : MessageEncoderFactory
    {
        private System.ServiceModel.Channels.MessageEncoder _encoder;
        private MessageVersion _version;
        private string _mediaType;
        private string _charSet;

        internal CustomTextMessageEncoderFactory(string mediaType, string charSet, MessageVersion version)
        {
            _version = version;
            _mediaType = mediaType;
            _charSet = charSet;
            _encoder = new CustomTextMessageEncoder(this);
        }

        public override System.ServiceModel.Channels.MessageEncoder Encoder
        {
            get
            {
                return _encoder;
            }
        }

        public override MessageVersion MessageVersion
        {
            get
            {
                return _version;
            }
        }

        internal string MediaType
        {
            get
            {
                return _mediaType;
            }
        }

        internal string CharSet
        {
            get
            {
                return _charSet;
            }
        }
    }
}