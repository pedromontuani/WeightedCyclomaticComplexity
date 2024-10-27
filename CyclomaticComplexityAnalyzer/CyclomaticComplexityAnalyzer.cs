using System.Collections.Concurrent;
using CyclomaticComplexityAnalyzer.dto;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;

namespace CyclomaticComplexityAnalyzer;

public static class CyclomaticComplexityAnalyzer
{
    private static readonly Type[] BranchingNodesTypes = new[] {typeof(IfStatementSyntax), typeof(WhileStatementSyntax), typeof(ForStatementSyntax), typeof(SwitchStatementSyntax), typeof(ConditionalExpressionSyntax), typeof(CatchClauseSyntax)};

    public static async Task<FileComplexity[]> CalculateCyclomaticComplexity(Project msProject)
    {
        var filesComplexity = new ConcurrentBag<FileComplexity>();
        
        await Parallel.ForEachAsync(msProject.Documents, async (document, _) =>
        {
            var syntaxTree = await document.GetSyntaxTreeAsync(_);
            var root = await syntaxTree.GetRootAsync(_);

            var methodDeclarations = root.DescendantNodes().OfType<MethodDeclarationSyntax>();
            
            var complexity = new FileComplexity(document.FilePath ?? "", 0);

            foreach (var method in methodDeclarations)
            {
                complexity.complexity += CalculateMethodCyclomaticComplexity(method);
            }
            
            if (complexity.complexity > 0)
            {
                filesComplexity.Add(complexity);
            }
        });

        return filesComplexity.ToArray();
    }

    private static int CalculateMethodCyclomaticComplexity(MethodDeclarationSyntax method)
    {
        var complexity = 1; // Start with 1 for the method itself.

        var descendants = method.DescendantNodes();
        
        var branchingNodes = descendants.Where(node => BranchingNodesTypes.Contains(node.GetType())).ToList();
        complexity += branchingNodes.Count;

        return complexity;
    }
    
}