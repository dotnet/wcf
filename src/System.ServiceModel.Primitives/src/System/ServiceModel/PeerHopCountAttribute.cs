// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net.Security;
using System.ServiceModel.Channels;

namespace System.ServiceModel
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class PeerHopCountAttribute : MessageHeaderAttribute
    {
        public PeerHopCountAttribute()
            : base()
        {
            base.Name = PeerStrings.HopCountElementName;
            base.Namespace = PeerStrings.HopCountElementNamespace;
            base.ProtectionLevel = ProtectionLevel.None;
            base.MustUnderstand = false;
        }
        public new bool MustUnderstand => base.MustUnderstand;

        public new bool Relay => base.Relay;

        public new string Actor => base.Actor;

        public new string Namespace => base.Namespace;

        public new string Name => base.Name;

        public new ProtectionLevel ProtectionLevel => base.ProtectionLevel;
    }
}
