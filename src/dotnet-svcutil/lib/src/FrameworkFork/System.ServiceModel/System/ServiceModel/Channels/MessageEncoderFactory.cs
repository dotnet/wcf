// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

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
