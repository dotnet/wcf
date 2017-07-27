// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Microsoft.SyndicationFeed
{
    public class Rss20Formatter : ISyndicationFeedFormatter
    {
        XmlWriterSettings _settings;

        public Rss20Formatter(XmlWriterSettings settings)
        {
            _settings = settings?.Clone() ?? new XmlWriterSettings();

            _settings.Async = false;
            _settings.CloseOutput = false;
            _settings.OmitXmlDeclaration = true;
        }

        public virtual string Format(ISyndicationContent content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            using (XmlWriter writer = CreateXmlWriter(out StringBuilder sb))
            {
                Write(content, writer);

                return sb.ToString();
            }
        }

        public virtual string Format(ISyndicationCategory category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            using (XmlWriter writer = CreateXmlWriter(out StringBuilder sb))
            {
                Write(category, writer);

                return sb.ToString();
            }
        }

        public virtual string Format(ISyndicationImage image)
        {
            throw new NotImplementedException();
        }

        public virtual string Format(ISyndicationItem item)
        {
            throw new NotImplementedException();
        }

        public virtual string Format(ISyndicationPerson person)
        {
            throw new NotImplementedException();
        }

        public virtual string Format(ISyndicationLink link)
        {
            throw new NotImplementedException();
        }

        public string FormatValue<T>(T value)
        {
            throw new NotImplementedException();
        }
        
        private XmlWriter CreateXmlWriter(out StringBuilder sb)
        {
            sb = new StringBuilder();
            return XmlWriter.Create(sb, _settings.Clone());
        }

        private void Write(ISyndicationContent content, XmlWriter writer)
        {
            throw new NotImplementedException();
        }

        private void Write(ISyndicationCategory category, XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}