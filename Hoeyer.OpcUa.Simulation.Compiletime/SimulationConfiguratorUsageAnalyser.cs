using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Hoeyer.OpcUa.Simulation.SourceGeneration.Constants;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Hoeyer.OpcUa.Simulation.SourceGeneration;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class SimulationConfiguratorUsageAnalyser : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
    [
        SimulationRules.MustBeActionSimulation,
        SimulationRules.MustBeFunctionSimulation,
        SimulationRules.TEntityMustBeAnEntity,
        SimulationRules.TypeHierarchyMustContainOnlyOneActionSimulator,
        SimulationRules.TArgsMustBeAnnotatedWithOpcEntityMethodArgs
    ];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeConfigurationMethodSymbol, SyntaxKind.ClassDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeConfigurationMethodSymbol, SyntaxKind.RecordDeclaration);
    }

    private static void AnalyzeConfigurationMethodSymbol(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not ClassDeclarationSyntax classDeclarationSyntax) return;

        INamedTypeSymbol? symbol = context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax);
        if (symbol is not INamedTypeSymbol
            {
                IsAbstract: false,
                TypeKind: TypeKind.Class
            } simulationImplementor)
            return;

        if (simulationImplementor.Interfaces.Length < 1) return;

        AnalyzeActionSimulationConfigurationUsage(
            simulationImplementor,
            context,
            l =>
                Diagnostic.Create(SimulationRules.TypeHierarchyMustContainOnlyOneActionSimulator, l)
        );

        AnalyzeFunctionSimulationConfigurationUsage(
            simulationImplementor,
            context,
            l =>
                Diagnostic.Create(SimulationRules.TypeHierarchyMustContainOnlyOneFunctionSimulator, l)
        );
    }

    private static void AnalyzeActionSimulationConfigurationUsage(
        INamedTypeSymbol simulationImplementor,
        SyntaxNodeAnalysisContext context,
        Func<Location, Diagnostic> onMultipleInterfaceImplementations)
    {
        INamedTypeSymbol? actionConfigType = context.Compilation.GetTypeByMetadataName(WellKnown.FullyQualifiedInterface
            .IActionSimulationConfigurator
            .WithoutGlobalPrefix);

        if (actionConfigType is null) return;

        (Location? location, AttributeData? argsAttrData) = AnalyseSimulationInterfaceUsage(simulationImplementor,
            actionConfigType, context,
            onMultipleInterfaceImplementations);
        if (argsAttrData == null) return;

        ITypeSymbol simulatedInterface = argsAttrData.AttributeClass!.TypeArguments[1];
        TypedConstant methodName = argsAttrData.ConstructorArguments[0];
        IMethodSymbol? simulatedMethod = simulatedInterface.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(m =>
            string.Equals(m.Name, (string)methodName.Value!, StringComparison.Ordinal));

        if (simulatedMethod!.ReturnType is INamedTypeSymbol { Arity: 1 })
        {
            context.ReportDiagnostic(Diagnostic.Create(SimulationRules.MustBeFunctionSimulation, location));
        }
    }

    private static void AnalyzeFunctionSimulationConfigurationUsage(
        INamedTypeSymbol simulationImplementor,
        SyntaxNodeAnalysisContext context,
        Func<Location, Diagnostic> onMultipleInterfaceImplementations)
    {
        INamedTypeSymbol? actionConfigType = context.Compilation.GetTypeByMetadataName(WellKnown.FullyQualifiedInterface
            .IFunctionSimulationConfigurator
            .WithoutGlobalPrefix);

        if (actionConfigType is null) return;

        (Location? location, AttributeData? argsAttrData) = AnalyseSimulationInterfaceUsage(
            simulationImplementor,
            actionConfigType,
            context,
            onMultipleInterfaceImplementations);

        if (argsAttrData == null) return;

        ITypeSymbol simulatedInterface = argsAttrData.AttributeClass!.TypeArguments[1];
        TypedConstant methodName = argsAttrData.ConstructorArguments[0];
        IMethodSymbol? simulatedMethod = simulatedInterface
            .GetMembers()
            .OfType<IMethodSymbol>()
            .FirstOrDefault(m => string.Equals(m.Name, (string)methodName.Value!, StringComparison.Ordinal));

        if (simulatedMethod!.ReturnType is not INamedTypeSymbol { Arity: 1 })
            context.ReportDiagnostic(Diagnostic.Create(SimulationRules.MustBeActionSimulation, location));
    }


    private static (Location? location, AttributeData? argsAttrData) AnalyseSimulationInterfaceUsage(
        INamedTypeSymbol simulationImplementor,
        INamedTypeSymbol simulationInterface,
        SyntaxNodeAnalysisContext context,
        Func<Location, Diagnostic> onMultipleInterfaceImplementations)
    {
        List<INamedTypeSymbol> actionInterfaces = simulationImplementor.AllInterfaces
            .Where(iface =>
                iface.Arity == 2 &&
                iface.OriginalDefinition.Equals(simulationInterface, SymbolEqualityComparer.Default))
            .ToList();


        if (actionInterfaces.Count == 0) return default;

        var implementorSyntax =
            simulationImplementor.DeclaringSyntaxReferences.FirstOrDefault()!.GetSyntax() as TypeDeclarationSyntax;
        if (implementorSyntax!.BaseList is null) return default;

        Location location = implementorSyntax
            .BaseList
            .Types
            .First(baseType =>
            {
                ITypeSymbol? actualType = context.SemanticModel.GetTypeInfo(baseType.Type).Type;
                return actualType is INamedTypeSymbol named &&
                       SymbolEqualityComparer.Default.Equals(named.OriginalDefinition, simulationInterface);
            }).GetLocation();

        if (actionInterfaces.Count > 1) context.ReportDiagnostic(onMultipleInterfaceImplementations.Invoke(location));

        INamedTypeSymbol simulatorInterface = actionInterfaces[0];
        ITypeSymbol argsType = simulatorInterface.TypeArguments[1];
        AttributeData? argsAttrData = argsType.GetOpcArgsAttribute();
        if (argsAttrData is null)
        {
            context.ReportDiagnostic(Diagnostic.Create(SimulationRules.TArgsMustBeAnnotatedWithOpcEntityMethodArgs,
                location));
        }

        ITypeSymbol tEntity = simulatorInterface.TypeArguments[0];
        if (tEntity is not INamedTypeSymbol entitySymbol || !entitySymbol.IsAnnotatedAsOpcUaEntity())
        {
            context.ReportDiagnostic(Diagnostic.Create(SimulationRules.TEntityMustBeAnEntity, location));
        }

        return (location, argsAttrData);
    }
}