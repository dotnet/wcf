// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace System.ServiceModel.BuildTools
{
    public sealed partial class OperationInvokerGenerator
    {
        private sealed class Parser
        {
            private readonly Compilation _compilation;
            private readonly OperationInvokerSourceGenerationContext _context;
            private readonly INamedTypeSymbol _sSMOperationContractSymbol;
            private readonly INamedTypeSymbol _sSMServiceContractSymbol;

            public Parser(Compilation compilation, in OperationInvokerSourceGenerationContext context)
            {
                _compilation = compilation;
                _context = context;
                _sSMOperationContractSymbol = _compilation.GetTypeByMetadataName("System.ServiceModel.OperationContractAttribute")!;
                _sSMServiceContractSymbol = _compilation.GetTypeByMetadataName("System.ServiceModel.ServiceContractAttribute")!;
            }

            public SourceGenerationSpec GetGenerationSpec(ImmutableArray<MethodDeclarationSyntax> methodDeclarationSyntaxes)
            {
                var methodSymbols = (from methodDeclarationSyntax in methodDeclarationSyntaxes
                    let semanticModel = _compilation.GetSemanticModel(methodDeclarationSyntax.SyntaxTree)
                    let symbol = semanticModel.GetDeclaredSymbol(methodDeclarationSyntax)
                    where symbol is not null
                    let methodSymbol = symbol
                    select methodSymbol).ToImmutableArray();

                var methods = (from method in methodSymbols
                    where method.HasAttribute(_sSMOperationContractSymbol) is not null
                    let @interface = method.ContainingSymbol
                    where @interface.HasAttribute(_sSMServiceContractSymbol) is not null
                    select method).ToImmutableArray();

                var builder = ImmutableArray.CreateBuilder<OperationContractSpec>();

                foreach (var value in methods)
                {
                    builder.Add(new OperationContractSpec(value));
                }

                ImmutableArray<OperationContractSpec> operationContractSpecs = builder.ToImmutable();

                if (operationContractSpecs.IsEmpty)
                {
                    return SourceGenerationSpec.None;
                }

                return new SourceGenerationSpec(operationContractSpecs);
            }

            internal static bool IsSyntaxTargetForGeneration(SyntaxNode node) => node is MethodDeclarationSyntax methodDeclarationSyntax
                && methodDeclarationSyntax.AttributeLists.Count > 0
                && methodDeclarationSyntax.Ancestors().Any(static ancestor => ancestor.IsKind(SyntaxKind.InterfaceDeclaration) && ((InterfaceDeclarationSyntax)ancestor).AttributeLists.Count > 0);

            internal static MethodDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
            {
                var methodDeclarationSyntax = (MethodDeclarationSyntax)context.Node;
                foreach (var attributeList in methodDeclarationSyntax.AttributeLists)
                {
                    foreach (var attribute in attributeList.Attributes)
                    {
                        var symbolInfo = context.SemanticModel.GetSymbolInfo(attribute);
                        ISymbol? attributeSymbol = symbolInfo.Symbol as IMethodSymbol;
                        // If the symbol is null, let's try to get it from the CandidateSymbols property
                        // NOTE: we do not need this fallback towards CandidateSymbols in the CoreWCF similar implementation
                        if (attributeSymbol == null)
                        {
                            foreach (var symbol in symbolInfo.CandidateSymbols)
                            {
                                if (symbol is IMethodSymbol methodSymbol)
                                {
                                    attributeSymbol = methodSymbol;
                                    break;
                                }
                            }
                        }
                        if (attributeSymbol == null)
                        {
                            continue;
                        }

                        var attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                        var fullName = attributeContainingTypeSymbol.ToDisplayString();

                        if (fullName == "System.ServiceModel.OperationContractAttribute")
                        {
                            return methodDeclarationSyntax;
                        }
                    }
                }

                return null;
            }
        }
    }
}
