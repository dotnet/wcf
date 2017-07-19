// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.SyndicationFeed
{
    public class Atom10FeedFormatter : ISyndicationFeedFormatter
    {
        public ISyndicationCategory ParseCategory(string value)
        {
            throw new NotImplementedException();
        }

        public ISyndicationImage ParseImage(string value)
        {
            throw new NotImplementedException();
        }

        public ISyndicationItem ParseItem(string value)
        {
            throw new NotImplementedException();
        }

        public ISyndicationLink ParseLink(string value)
        {
            throw new NotImplementedException();
        }

        public ISyndicationPerson ParsePerson(string value)
        {
            throw new NotImplementedException();
        }

        public bool TryParseValue<T>(string value, out T result)
        {
            throw new NotImplementedException();
        }
    }
}
