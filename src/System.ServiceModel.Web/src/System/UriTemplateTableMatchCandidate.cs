// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System
{
    internal struct UriTemplateTableMatchCandidate
    {
        public UriTemplateTableMatchCandidate(UriTemplate template, int segmentsCount, object data)
        {
            Template = template;
            SegmentsCount = segmentsCount;
            Data = data;
        }

        public object Data { get; }

        public int SegmentsCount { get; }

        public UriTemplate Template { get; }
    }
}
