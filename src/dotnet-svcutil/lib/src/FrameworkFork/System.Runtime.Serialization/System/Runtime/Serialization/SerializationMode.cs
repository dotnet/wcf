// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.Runtime.Serialization
{
    internal enum SerializationMode
    {
        SharedContract,
#if NET_NATIVE
        SharedType
#endif
    }
}
