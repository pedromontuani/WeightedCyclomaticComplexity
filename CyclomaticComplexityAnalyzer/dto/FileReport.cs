namespace CyclomaticComplexityAnalyzer.dto;

public class FileReport(string filePath, int cyclomaticComplexity, int modifiedCyclomaticComplexity)
{
    public string filePath { get; set; } = filePath;
    public int cyclomaticComplexity { get; set; } = cyclomaticComplexity;
    public int modifiedCyclomaticComplexity { get; set; } = modifiedCyclomaticComplexity;
}