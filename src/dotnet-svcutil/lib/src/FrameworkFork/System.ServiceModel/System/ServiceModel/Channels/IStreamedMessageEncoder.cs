// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.IO;

namespace System.ServiceModel.Channels
{
    internal interface IStreamedMessageEncoder
    {
        Stream GetResponseMessageStream(Message message);
    }
}
