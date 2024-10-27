// See https://aka.ms/new-console-template for more information

using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace CyclomaticComplexityAnalyzer;

class Program
{
    static async Task Main(string[] args)
    {
        InitializeMsBuild();
        
        if(args.Length == 0)
        {
            Console.WriteLine("Forneca o caminho do projeto C# como argumento.");
            return;
        }

        string projectPath = args[0];
        
        var project = await OpenProject(projectPath);
        
        var cc = await CyclomaticComplexityAnalyzer.CalculateCyclomaticComplexity(project);
        var wcc = await WeightedCyclomaticComplexityAnalyzer.CalculateCognitiveComplexity(project);
        
        int totalComplexity = cc.Sum(c => c.complexity);
        Console.WriteLine($"Total Cyclomatic Complexity: {totalComplexity}");
        totalComplexity = wcc.Sum(c => c.complexity);
        Console.WriteLine($"Total Weighted Cyclomatic Complexity: {totalComplexity}");

        var report = new Report(cc, wcc);
        
        report.GenerateReport();

        // int totalComplexity = complexity.Sum();
        // Console.WriteLine($"Total Cyclomatic Complexity: {totalComplexity}");
    }
    
    private static void InitializeMsBuild()
    {
        MSBuildLocator.RegisterDefaults();
        
        if(!MSBuildLocator.IsRegistered)
        {
            var msBuildPath = Environment.GetEnvironmentVariable("MSBUILD_PATH");
            
            if (String.IsNullOrEmpty(msBuildPath))
            {
                Console.WriteLine("Forneca o caminho do MSBuild como variável de ambiente MSBUILD_PATH.");
                return;
            }
            
            MSBuildLocator.RegisterMSBuildPath(msBuildPath);
        }
    }
    
    private static async Task<Project> OpenProject(string projectPath)
    {
        var workspace = MSBuildWorkspace.Create(new Dictionary<string, string> { { "AlwaysCompileMarkupFilesInSeparateDomain", "true" }, { "CheckForSystemRuntimeDependency", "true" } });
        var project = await workspace.OpenProjectAsync(projectPath).ConfigureAwait(false);

        return project;
    }
}