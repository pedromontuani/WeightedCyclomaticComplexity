using CyclomaticComplexityAnalyzer.utils;

namespace CyclomaticComplexityAnalyzer.dto;

public class FileReport(string filePath, int cyclomaticComplexity, int weightedCyclomaticComplexity)
{
    public string filePath { get; set; } = FilesManager.GetFileName(filePath);
    public int cyclomaticComplexity { get; set; } = cyclomaticComplexity;
    public int weightedCyclomaticComplexity { get; set; } = weightedCyclomaticComplexity;

    public string ratingCssClass { get; set; } = "high";
}