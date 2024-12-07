using Hoeyer.Machines.OpcUa.Configuration;

namespace Hoeyer.Machines.OpcUa.ResourceLoading;

public record TemplateFileLoad(CodeTemplateInformation TemplateInformation, TypeContext TypeContext)
{
    public TypeContext TypeContext { get; } = TypeContext;
    public CodeTemplateInformation TemplateInformation { get; } = TemplateInformation;
}