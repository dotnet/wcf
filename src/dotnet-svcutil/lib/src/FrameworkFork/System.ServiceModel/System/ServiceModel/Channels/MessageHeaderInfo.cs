// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.Xml;
using System.ServiceModel;
using System.Diagnostics;

namespace System.ServiceModel.Channels
{
    public abstract class MessageHeaderInfo
    {
        public abstract string Actor { get; }
        public abstract bool IsReferenceParameter { get; }
        public abstract string Name { get; }
        public abstract string Namespace { get; }
        public abstract bool MustUnderstand { get; }
        public abstract bool Relay { get; }
    }
}
