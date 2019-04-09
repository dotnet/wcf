// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.



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
