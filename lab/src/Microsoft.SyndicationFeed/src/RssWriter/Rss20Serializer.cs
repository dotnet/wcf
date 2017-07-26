// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Microsoft.SyndicationFeed
{
    public class Rss20Serializer : ISyndicationFeedSerializer
    {
        public virtual string Serialize(ISyndicationContent content)
        {
            string xml = content.RawContent;
            return xml;
        }

        public virtual string Serialize(ISyndicationCategory category)
        {

            if (string.IsNullOrEmpty(category.Name))
            {
                throw new FormatException("Category does not contain a name");
            }

            string result = ConstructCategory(category);

            throw new NotImplementedException();
        }

        public virtual string Serialize(ISyndicationImage image)
        {
            throw new NotImplementedException();
        }

        public virtual string Serialize(ISyndicationItem item)
        {
            throw new NotImplementedException();
        }

        public virtual string Serialize(ISyndicationPerson person)
        {
            throw new NotImplementedException();
        }

        public virtual string Serialize(ISyndicationLink link)
        {
            throw new NotImplementedException();
        }

        private string ConstructCategory(ISyndicationCategory category)
        {
            throw new NotImplementedException();
        }



    }
}
