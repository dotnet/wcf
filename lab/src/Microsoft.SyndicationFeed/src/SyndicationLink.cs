using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.SyndicationFeed
{
    class SyndicationLink : ISyndicationLink
    {
        public Uri Uri { get; set; }

        public string Title { get; set; }

        public string MediaType { get; set; }

        public string RelationshipType { get; set; }

        public long Length { get; set; }
    }
}
