// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Runtime;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    public abstract class StreamUpgradeInitiator
    {
        protected StreamUpgradeInitiator()
        {
        }

        public abstract string GetNextUpgrade();

        public abstract Stream InitiateUpgrade(Stream stream);

        internal abstract Task<Stream> InitiateUpgradeAsync(Stream stream);

        internal abstract void Open(TimeSpan timeout);

        internal abstract Task OpenAsync(TimeSpan timeout);

        internal abstract void Close(TimeSpan timeout);

        internal abstract Task CloseAsync(TimeSpan timeout);
    }
}
