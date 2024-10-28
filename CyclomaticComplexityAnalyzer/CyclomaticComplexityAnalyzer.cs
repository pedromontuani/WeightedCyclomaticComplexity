using System.Collections.Concurrent;
using CyclomaticComplexityAnalyzer.dto;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;

namespace CyclomaticComplexityAnalyzer;

public class CyclomaticComplexityAnalyzer : Complexity
{

    public static async Task<FileComplexity[]> CalculateCyclomaticComplexity(Project msProject)
    {
        var filesComplexity = new ConcurrentBag<FileComplexity>();
        
        await Parallel.ForEachAsync(msProject.Documents, async (document, _) =>
        {
            var syntaxTree = await document.GetSyntaxTreeAsync(_);
            var root = await syntaxTree.GetRootAsync(_);
            var methodDeclarations = root.DescendantNodes()
                .OfType<MethodDeclarationSyntax>();
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
        var complexity = 1;
        var descendants = method.DescendantNodes();
        
        var branchingNodes = descendants
            .Where(IsBranchingNode).ToList();
        
        complexity += branchingNodes.Count;

        return complexity;
    }
    
}