using System.Collections.Concurrent;
using CyclomaticComplexityAnalyzer.dto;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;

namespace CyclomaticComplexityAnalyzer;

public static class WeightedCyclomaticComplexityAnalyzer
{
    private static readonly Type[] DecisionNodesTypes = new[] {typeof(IfStatementSyntax), typeof(SwitchStatementSyntax), typeof(ConditionalExpressionSyntax), typeof(CatchClauseSyntax)};
    private static readonly Type[] LoopNodesTypes = new[] {typeof(ForStatementSyntax), typeof(WhileStatementSyntax), typeof(DoStatementSyntax), typeof(ForEachStatementSyntax)};
    
    public static async Task<FileComplexity[]> CalculateCognitiveComplexity(Project msProject)
    {
        var filesComplexity = new ConcurrentBag<FileComplexity>();

        await Parallel.ForEachAsync(msProject.Documents, async (document, _) =>
        {
            var syntaxTree = await document.GetSyntaxTreeAsync(_);
            var root = await syntaxTree.GetRootAsync(_);

            var methodDeclarations = root.DescendantNodes().OfType<MethodDeclarationSyntax>();

            foreach (var method in methodDeclarations)
            {
                var complexity = new FileComplexity(document.FilePath ?? "", CalculateMethodWeightedComplexity(method));
                filesComplexity.Add(complexity);
            }
        });
        

        return filesComplexity.ToArray();
    }

    
    private static int CalculateMethodWeightedComplexity(MethodDeclarationSyntax method)
    {
        int complexity = 1;
        int nestedLoopCount = 0;

        var descendants = method.DescendantNodes();

        foreach (var node in descendants)
        {
            var nestingLevel = node.Ancestors().Count(n => IsDecisionNode(n) || IsLoopNode(n));
            
            if (IsDecisionNode(node))
            {
                complexity += 1 + nestingLevel;
            }
            
            if (IsLoopNode(node))
            {
                if (node.Ancestors().Where(IsLoopNode).Any())
                {
                    nestedLoopCount++;
                }
                else
                {
                    nestedLoopCount = 0;
                }
                
                complexity += 1 * Math.Max(nestedLoopCount, 1) + nestingLevel;
            }
        }

        return complexity;
    }

    private static bool IsDecisionNode(SyntaxNode node)
    {
        return DecisionNodesTypes.Contains(node.GetType());
    }

    private static bool IsLoopNode(SyntaxNode node)
    {
        return LoopNodesTypes.Contains(node.GetType());
    }
}