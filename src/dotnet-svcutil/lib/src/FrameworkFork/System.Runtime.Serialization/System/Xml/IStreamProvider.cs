// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.IO;

namespace Microsoft.Xml
{
    public interface IStreamProvider
    {
        Stream GetStream();
        void ReleaseStream(Stream stream);
    }
}
