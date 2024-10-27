using JinianNet.JNTemplate;

namespace CyclomaticComplexityAnalyzer.dto;

public class Report(FileReport[] reports)
{
    private const string IndexTemplatePath = "resources/index.html";

    private readonly ITemplate _indexTemplate = Engine.LoadTemplate(IndexTemplatePath);

}