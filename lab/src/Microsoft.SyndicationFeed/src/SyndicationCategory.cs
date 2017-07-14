using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.SyndicationFeed
{
    class SyndicationCategory : ISyndicationCategory
    {
        public string Name { get; set; }

        public string Label { get; set; }

        public string Scheme { get; set; }
    }
}
