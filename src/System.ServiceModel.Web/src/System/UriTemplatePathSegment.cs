// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Specialized;
using System.Diagnostics;
using System.Runtime;
using System.Text;

namespace System
{
    // This represents a Path segment, which can either be a Literal, a Variable or a Compound
    [DebuggerDisplay("Segment={originalSegment} Nature={nature}")]
    internal abstract class UriTemplatePathSegment
    {
        protected UriTemplatePathSegment(string originalSegment, UriTemplatePartType nature,
            bool endsWithSlash)
        {
            OriginalSegment = originalSegment;
            Nature = nature;
            EndsWithSlash = endsWithSlash;
        }

        public bool EndsWithSlash { get; }

        public UriTemplatePartType Nature { get; }

        public string OriginalSegment { get; }

        public static UriTemplatePathSegment CreateFromUriTemplate(string segment, UriTemplate template)
        {
            // Identifying the type of segment - Literal|Compound|Variable
            switch (UriTemplateHelpers.IdentifyPartType(segment))
            {
                case UriTemplatePartType.Literal:
                    return UriTemplateLiteralPathSegment.CreateFromUriTemplate(segment, template);

                case UriTemplatePartType.Compound:
                    return UriTemplateCompoundPathSegment.CreateFromUriTemplate(segment, template);

                case UriTemplatePartType.Variable:
                    if (segment.EndsWith("/", StringComparison.Ordinal))
                    {
                        string varName = template.AddPathVariable(UriTemplatePartType.Variable,
                            segment.Substring(1, segment.Length - 3));
                        return new UriTemplateVariablePathSegment(segment, true, varName);
                    }
                    else
                    {
                        string varName = template.AddPathVariable(UriTemplatePartType.Variable,
                            segment.Substring(1, segment.Length - 2));
                        return new UriTemplateVariablePathSegment(segment, false, varName);
                    }

                default:
                    Fx.Assert("Invalid value from IdentifyStringNature");
                    return null;
            }
        }

        public abstract void Bind(string[] values, ref int valueIndex, StringBuilder path);

        public abstract bool IsEquivalentTo(UriTemplatePathSegment other, bool ignoreTrailingSlash);
        public bool IsMatch(UriTemplateLiteralPathSegment segment)
        {
            return IsMatch(segment, false);
        }

        public abstract bool IsMatch(UriTemplateLiteralPathSegment segment, bool ignoreTrailingSlash);
        public abstract void Lookup(string segment, NameValueCollection boundParameters);
    }
}
