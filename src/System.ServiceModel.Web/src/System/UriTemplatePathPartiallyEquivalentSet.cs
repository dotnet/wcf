// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
namespace System
{
    using System.Collections.Generic;

    // This class was named UriTemplatePathEquivalentSet in the Orcas bits, where it used to
    //  represent a set of uri templates, which are equivalent in thier path. The introduction
    //  of terminal defaults, caused it to be no longer true; now it is representing a set
    //  of templates, which are equivalent in their path till a certian point, which is stored
    //  in the segmentsCount field. To highlight that fact the class was renamed as
    //  UriTemplatePathPartiallyEquivalentSet.
    class UriTemplatePathPartiallyEquivalentSet
    {
        List<KeyValuePair<UriTemplate, object>> kvps;
        int segmentsCount;

        public UriTemplatePathPartiallyEquivalentSet(int segmentsCount)
        {
            this.segmentsCount = segmentsCount;
            this.kvps = new List<KeyValuePair<UriTemplate, object>>();
        }
        public List<KeyValuePair<UriTemplate, object>> Items
        {
            get
            {
                return this.kvps;
            }
        }

        public int SegmentsCount
        {
            get
            {
                return this.segmentsCount;
            }
        }
    }
}
