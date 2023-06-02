// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.IO;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    public abstract class StreamUpgradeInitiator
    {
        protected StreamUpgradeInitiator() { }
        public abstract string GetNextUpgrade();
        public abstract Task<Stream> InitiateUpgradeAsync(Stream stream);
        internal virtual ValueTask OpenAsync(TimeSpan timeout) => default;
        internal virtual ValueTask CloseAsync(TimeSpan timeout) => default;
    }
}
