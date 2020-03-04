// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.ServiceModel.Channels
{
    internal interface ICompressedMessageEncoder
    {
        bool CompressionEnabled { get; }

        void SetSessionContentType(string contentType);

        void AddCompressedMessageProperties(Message message, string supportedCompressionTypes);
    }
}
