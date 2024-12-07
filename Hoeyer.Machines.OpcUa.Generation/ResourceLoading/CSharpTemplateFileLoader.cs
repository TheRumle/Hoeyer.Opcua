using System;
using System.Collections.Generic;
using System.Text;
using Hoeyer.Machines.OpcUa.Configuration;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hoeyer.Machines.OpcUa.ResourceLoading;

/// <summary>
/// Finds .cst files, replaces class names based on template formatting
/// </summary>
internal class CSharpTemplateFileLoader : IResourceLoader
{
    private readonly AssociativeEmbeddedResourceLoader<TypeContext> _loader;
    private readonly TemplateFileLoad _fileLoad;

    public CSharpTemplateFileLoader(TemplateFileLoad fileLoad, Action<Diagnostic>? errorReport = null)
    {
        _fileLoad = fileLoad; 
        _loader = new AssociativeEmbeddedResourceLoader<TypeContext>([(_fileLoad.TemplateInformation.TemplateClassName, _fileLoad.TypeContext)],
            new ResourceMatcher(".cst"), GetGeneratedFileNameFor, errorReport);
    }

    /// <inheritdoc />
    public IEnumerable<LoadableType> LoadResources()
    {
        var loadedTemplates = _loader.LoadResources();
        foreach (var loadableType in loadedTemplates)
        {
            yield return new LoadableType(
                FileName: loadableType.FileName,
                TypeDefinition: AdjustTemplateCode(loadableType.TypeDefinition)
            );
        }
    }

    private string AdjustTemplateCode(string typeCode)
    {
        StringBuilder builder = new(typeCode);
        builder.Replace("CLASS_NAME", _fileLoad.TypeContext.Node.Identifier.Text);
        builder.Replace("NAMESPACE_OF_CLASS", _fileLoad.TypeContext.GetNamespace.ToDisplayString());
        return builder.ToString();
    }

    private string GetGeneratedFileNameFor(TypeContext context)
    {
        return context.Node.Identifier.Text + _fileLoad.TemplateInformation.TemplateClassName + ".g.cs";
    }
}