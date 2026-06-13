// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
namespace System
{
    class UriTemplateTrieLocation
    {
        public UriTemplateTrieIntraNodeLocation locationWithin;
        public UriTemplateTrieNode node;
        public UriTemplateTrieLocation(UriTemplateTrieNode n, UriTemplateTrieIntraNodeLocation i)
        {
            this.node = n;
            this.locationWithin = i;
        }
    }

}
