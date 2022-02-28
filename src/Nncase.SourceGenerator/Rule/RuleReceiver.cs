﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Nncase.SourceGenerator.Rule;


internal class RuleCandidate
{
    public ClassDeclarationSyntax classDeclaration;
    public INamedTypeSymbol classSymobl;
    public IMethodSymbol methodSymbol;
    public RuleCandidate(ClassDeclarationSyntax class_declaration, INamedTypeSymbol class_symobl, IMethodSymbol method_symbol)
    {
        classDeclaration = class_declaration;
        classSymobl = class_symobl;
        methodSymbol = method_symbol;
    }
}

internal class RuleReceiver : ISyntaxContextReceiver
{
    public readonly List<Diagnostic> Diagnostics = new();
    public readonly List<RuleCandidate> Candidates = new();

    public INamedTypeSymbol? ExprSymobl;
    public INamedTypeSymbol? IMatchResultSymobl;

    public void OnVisitSyntaxNode(GeneratorSyntaxContext ctx)
    {
        ExprSymobl ??= ctx.SemanticModel.Compilation.GetTypeByMetadataName("Nncase.IR.Expr");
        IMatchResultSymobl ??= ctx.SemanticModel.Compilation.GetTypeByMetadataName("Nncase.PatternMatch.IMatchResult");


        var compilation = ctx.SemanticModel.Compilation;
        if (ctx.Node is ClassDeclarationSyntax classDeclaration)
        {
            var classSymbol = ctx.SemanticModel.GetDeclaredSymbol(classDeclaration);
            if (classSymbol!.GetAttributes().Any(attr => attr.AttributeClass is { Name: "RuleGeneratorAttribute" }))
            {
                // 0. check inherit from base class;
                if (classSymbol.BaseType is not { IsGenericType: true, Name: "RewriteRule" })
                    Diagnostics.Add(Diagnostic.Create(RecriverUtil.ClassNotFromBaseClassError, Location.None, classSymbol.ToDisplayString(), "RewriteRule"));

                // 1. check is Partial
                if (!classDeclaration.Modifiers.Any(tok => tok.IsKind(SyntaxKind.PartialKeyword)))
                    Diagnostics.Add(Diagnostic.Create(RecriverUtil.ClassNotPartialError, Location.None, classSymbol.ToDisplayString()));

                // 2. find the candidate method!
                var methods = classSymbol.GetMembers().OfType<IMethodSymbol>().Where(m =>
                  m.Name == "GetReplace"
                  && m.ReturnType.IsInheritFrom(ExprSymobl!)
                  && (m.Parameters.All(
                      p => SymbolEqualityComparer.Default.Equals(p.Type, IMatchResultSymobl)
                           || p.Type.IsInheritFrom(ExprSymobl)
                      ))).ToArray();
                if (methods.Length == 0)
                    return;

                // 3. if have Override method, skip
                if (methods.Any(m =>
                    m.IsOverride
                    && m.Parameters.Length == 1
                    && SymbolEqualityComparer.Default.Equals(m.Parameters[0].Type, IMatchResultSymobl)))
                    return;

                // 4. if have more than one valid method 
                if (methods.Length != 1)
                    Diagnostics.Add(Diagnostic.Create(RecriverUtil.ClassMoreMethodError, Location.None, classSymbol.ToDisplayString()));

                // 5. add to the Candidates
                var method = methods[0];
                Candidates.Add(new(classDeclaration, classSymbol, method));
                Console.WriteLine($"RuleGenerator Receive {classSymbol} For RewriteRule");
            }
        }
    }
}
