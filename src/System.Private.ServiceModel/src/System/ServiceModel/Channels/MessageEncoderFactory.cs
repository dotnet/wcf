// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.ServiceModel.Channels
{
    public abstract class MessageEncoderFactory
    {
        protected MessageEncoderFactory()
        {
        }

        public abstract MessageEncoder Encoder
        {
            get;
        }

        public abstract MessageVersion MessageVersion
        {
            get;
        }

        public virtual MessageEncoder CreateSessionEncoder()
        {
            return Encoder;
        }
    }
}
