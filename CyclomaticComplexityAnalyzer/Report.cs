using CyclomaticComplexityAnalyzer.dto;
using CyclomaticComplexityAnalyzer.utils;
using JinianNet.JNTemplate;

namespace CyclomaticComplexityAnalyzer;

public class Report
{
    private const string IndexTemplatePath = "resources/index.html";
    private const string OutDir = "report";
    private readonly ITemplate _indexTemplate = Engine.LoadTemplate(IndexTemplatePath);
    
    private int totalCyclomaticComplexity;
    private int totalWeightedCyclomaticComplexity;
    
    private readonly Dictionary<string, FileReport> _reports = new ();
    
    public Report(FileComplexity[] cyclomatic, FileComplexity[] modifiedCyclomatic)
    {
        foreach (var fileComplexity in cyclomatic)
        {
            _reports[fileComplexity.filePath] = new FileReport(fileComplexity.filePath, fileComplexity.complexity, 0);
        }
        
        foreach (var fileComplexity in modifiedCyclomatic)
        {
            if (_reports.TryGetValue(fileComplexity.filePath, out var report))
            {
                report.weightedCyclomaticComplexity = fileComplexity.complexity;
            }
            else
            {
                _reports[fileComplexity.filePath] = new FileReport(fileComplexity.filePath, 0, fileComplexity.complexity);
            }
        }
        
        foreach (var report in _reports.Values)
        {
            totalCyclomaticComplexity += report.cyclomaticComplexity;
            totalWeightedCyclomaticComplexity += report.weightedCyclomaticComplexity;
        }
    }

    public void GenerateReport()
    {
        var reports = _reports.Values.ToList();
        
        _indexTemplate.Set("reports", reports);
        _indexTemplate.Set("ratingCssClass", "");
        _indexTemplate.Set("result", "");
        _indexTemplate.Set("ratingCssClass", "high");
        _indexTemplate.Set("totalCyclomaticComplexity", totalCyclomaticComplexity);
        _indexTemplate.Set("totalWeightedCyclomaticComplexity", totalWeightedCyclomaticComplexity);
        
        FilesManager.SaveFile(OutDir + "/index.html", _indexTemplate.Render());
    }

}