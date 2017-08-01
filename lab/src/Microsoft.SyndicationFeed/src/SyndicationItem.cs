// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.SyndicationFeed
{
    public class SyndicationItem : ISyndicationItem
    {
        private ICollection<ISyndicationCategory> _categories;
        private ICollection<ISyndicationPerson> _contributors;
        private ICollection<ISyndicationLink> _links;

        public SyndicationItem()
        {
            _categories = new List<ISyndicationCategory>();
            _contributors = new List<ISyndicationPerson>();
            _links = new List<ISyndicationLink>();
        }

        public SyndicationItem(ISyndicationItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            Id = item.Id;
            Title = item.Title;
            Description = item.Description;
            LastUpdated = item.LastUpdated;
            Published = item.Published;

            // Copy collections only if needed
            _categories = item.Categories as ICollection<ISyndicationCategory> ?? item.Categories.ToList();
            _contributors = item.Contributors as ICollection<ISyndicationPerson> ?? item.Contributors.ToList();
            _links = item.Links as ICollection<ISyndicationLink> ?? item.Links.ToList();
        }

        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public IEnumerable<ISyndicationCategory> Categories 
        {
            get 
            {
                return _categories;
            }
        }

        public IEnumerable<ISyndicationPerson> Contributors 
        {
            get
            {
                return _contributors;
            }
        }

        public IEnumerable<ISyndicationLink> Links 
        {
            get {
                return _links;
            }
        }

        public DateTimeOffset LastUpdated { get; set; }

        public DateTimeOffset Published { get; set; }

        public void AddCategory(ISyndicationCategory category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            if (_categories.IsReadOnly)
            {
                _categories = _categories.ToList();
            }

            _categories.Add(category);
        }

        public void AddContributor(ISyndicationPerson person)
        {
            if (person == null)
            {
                throw new ArgumentNullException(nameof(person));
            }

            if (_contributors.IsReadOnly)
            {
                _contributors = _contributors.ToList();
            }

            _contributors.Add(person);
        }

        public void AddLink(ISyndicationLink link)
        {
            if (link == null)
            {
                throw new ArgumentNullException(nameof(link));
            }

            if (_links.IsReadOnly)
            {
                _links = _links.ToList();
            }

            _links.Add(link);
        }
    }
}
