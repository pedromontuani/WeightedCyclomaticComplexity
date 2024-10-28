using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CyclomaticComplexityAnalyzer;

public class Complexity
{
    private static readonly Type[] BranchingNodesTypes = new[]
    {
        typeof(IfStatementSyntax),
        typeof(WhileStatementSyntax),
        typeof(ForStatementSyntax),
        typeof(SwitchStatementSyntax),
        typeof(ConditionalExpressionSyntax),
        typeof(CatchClauseSyntax)
    };

    protected static bool IsBranchingNode(SyntaxNode node)
    {
        return BranchingNodesTypes.Contains(node.GetType());
    }
}