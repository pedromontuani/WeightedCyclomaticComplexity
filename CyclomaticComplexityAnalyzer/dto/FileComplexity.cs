namespace CyclomaticComplexityAnalyzer.dto;

public class FileComplexity(string filePath, int complexity)
{
    public string filePath { get; set; } = filePath;
    public int complexity { get; set; } = complexity;
}