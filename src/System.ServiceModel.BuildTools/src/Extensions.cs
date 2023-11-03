// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.CodeAnalysis;

namespace System.ServiceModel.BuildTools
{
    internal static class SymbolExtensions
    {
        public static AttributeData? HasAttribute(this ISymbol symbol, INamedTypeSymbol attributeTypeSymbol)
        {
            foreach (var attribute in symbol.GetAttributes())
            {
                if (SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, attributeTypeSymbol))
                {
                    return attribute;
                }
            }

            return null;
        }
    }
}
