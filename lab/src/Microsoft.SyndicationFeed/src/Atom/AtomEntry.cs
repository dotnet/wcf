// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.SyndicationFeed.Atom
{
    public class AtomEntry : SyndicationItem, IAtomEntry
    {
        public AtomEntry()
        {
        }

        public AtomEntry(IAtomEntry item) 
            : base(item)
        {
            ContentType = item.ContentType;
            Summary = item.Summary;
            Rights = item.Rights;
        }

        public string ContentType { get; set; }

        public string Summary { get; set; }

        public string Rights { get; set; }
    }
}
