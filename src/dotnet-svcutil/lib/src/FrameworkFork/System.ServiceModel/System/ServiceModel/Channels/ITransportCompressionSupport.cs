// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.ServiceModel.Channels
{
    public interface ITransportCompressionSupport
    {
        bool IsCompressionFormatSupported(CompressionFormat compressionFormat);
    }
}
