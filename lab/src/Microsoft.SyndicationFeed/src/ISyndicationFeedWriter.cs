// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;

namespace Microsoft.SyndicationFeed
{
    public interface ISyndicationFeedWriter
    {
        Task Write(ISyndicationContent content);

        Task Write(ISyndicationCategory category);

        Task Write(ISyndicationImage image);

        Task Write(ISyndicationItem item);

        Task Write(ISyndicationPerson person);

        Task Write(ISyndicationLink link);

        Task WriteValue<T>(string name, T value);

        Task WriteElement(string content);
    }
}
