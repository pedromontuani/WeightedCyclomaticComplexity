using System.Collections.Concurrent;
using CyclomaticComplexityAnalyzer.dto;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;

namespace CyclomaticComplexityAnalyzer;

public class WeightedCyclomaticComplexityAnalyzer : Complexity
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

            var complexity = new FileComplexity(document.FilePath ?? "", 0);

            foreach (var method in methodDeclarations)
            {
                complexity.complexity += CalculateMethodWeightedComplexity(method);
            }

            if (complexity.complexity > 0)
            {
                filesComplexity.Add(complexity);
            }
        });
        

        return filesComplexity.ToArray();
    }

    
    private static int CalculateMethodWeightedComplexity(MethodDeclarationSyntax method)
    {
        int complexity = 1;
        var descendants = method.DescendantNodes();

        foreach (var node in descendants)
        {
            var nestingLevel = node.Ancestors()
                .Count(IsBranchingNode);
            
            if (IsBranchingNode(node))
            {
                complexity += 1 + nestingLevel;
            }
            
        }

        return complexity;
    }
}