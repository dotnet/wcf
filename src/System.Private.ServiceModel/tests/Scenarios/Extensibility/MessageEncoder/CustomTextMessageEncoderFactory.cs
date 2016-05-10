// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Extensibility.MessageEncoder.Tests
{
    internal class CustomTextMessageEncoderFactory : MessageEncoderFactory
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
