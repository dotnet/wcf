// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.ServiceModel.Channels
{
    internal interface ICompressedMessageEncoder
    {
        bool CompressionEnabled { get; }

        void SetSessionContentType(string contentType);

        void AddCompressedMessageProperties(Message message, string supportedCompressionTypes);
    }
}
