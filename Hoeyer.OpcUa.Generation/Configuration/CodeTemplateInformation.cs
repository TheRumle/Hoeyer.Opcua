namespace Hoeyer.Machines.OpcUa.Configuration;

public record CodeTemplateInformation(string TemplateClassName, string TemplateClassResourcePath)
{
    public string TemplateClassResourcePath { get; } = TemplateClassResourcePath;
    public string TemplateClassName { get; } = TemplateClassName;
}