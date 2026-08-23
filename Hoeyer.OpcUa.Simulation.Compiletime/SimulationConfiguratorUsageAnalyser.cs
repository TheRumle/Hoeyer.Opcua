using System.Collections.Immutable;
using Hoeyer.OpcUa.Simulation.SourceGeneration.Constants;
using Hoeyer.OpcUa.Simulation.SourceGeneration.Models;
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
        SimulationRules.TArgsMustBeAnnotatedWithOpcEntityMethodArgs,
        SimulationRules.ReturnTypeMustMatchReturnTypeOfSimulatedMethod
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
        if (context.Node is not TypeDeclarationSyntax classDeclarationSyntax)
        {
            return;
        }

        var symbol = context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax);
        if (symbol is not
            {
                IsAbstract: false,
                TypeKind: TypeKind.Class,
                Interfaces.Length: > 0
            } simulationImplementor)
        {
            return;
        }

        AnalyzeActionSimulationConfigurationUsage(
            simulationImplementor,
            context
        );

        AnalyzeFunctionSimulationConfigurationUsage(
            simulationImplementor,
            context
        );
    }

    private static void AnalyzeActionSimulationConfigurationUsage(
        INamedTypeSymbol implementor,
        SyntaxNodeAnalysisContext context)
    {
        var node = (TypeDeclarationSyntax)context.Node;
        var wanted = WellKnown.FullyQualifiedInterface.IActionSimulationConfigurator;

        var usageStructures =
            GetImplementedConfiguratorInterfaces(wanted, implementor, context)
                .Where(@interface => @interface is not null)
                .Select(iFace => CreateConfigurationSimulationUsage(context, node, iFace))
                .Where(e => e.SimulatedMethod != null!);

        foreach (var (simulatedMethod, configuratorInterface, _) in usageStructures)
        {
            var reporter = new InterfaceUsageReporter(context, node, configuratorInterface);
            if (simulatedMethod.ReturnType is INamedTypeSymbol { Arity: 1 })
            {
                reporter.ReportDiagnostic(SimulationRules.MustBeFunctionSimulation);
            }
        }
    }


    private static void AnalyzeFunctionSimulationConfigurationUsage(
        INamedTypeSymbol implementor,
        SyntaxNodeAnalysisContext context)
    {
        var node = (TypeDeclarationSyntax)context.Node;
        var wanted = WellKnown.FullyQualifiedInterface.IFunctionSimulationConfigurator;

        var usageStructures =
            GetImplementedConfiguratorInterfaces(wanted, implementor, context)
                .Where(@interface => @interface is not null)
                .Select(iFace => CreateConfigurationSimulationUsage(context, node, iFace))
                .Where(e => e.SimulatedMethod != null!);

        foreach (var (simulatedMethod, configuratorInterface, inputArgInfo) in usageStructures)
        {
            var actualReturn = configuratorInterface.TypeArguments[2];
            var reporter = new InterfaceUsageReporter(context, node, configuratorInterface);
            AnalyzeFunctionReturnTypes(simulatedMethod, reporter, inputArgInfo, actualReturn);
        }
    }

    private static void AnalyzeFunctionReturnTypes(IMethodSymbol simulatedMethod, InterfaceUsageReporter reporter,
        InputArgumentStructureInfo inputArgInfo, ITypeSymbol actualReturnType)
    {
        //Is always a task
        if (simulatedMethod!.ReturnType is not INamedTypeSymbol { Arity: 1 } taskReturn)
        {
            reporter.ReportDiagnostic(SimulationRules.MustBeActionSimulation);
            return;
        }

        var expectedReturnType = taskReturn.TypeArguments[0];
        if (!SymbolEqualityComparer.Default.Equals(expectedReturnType, actualReturnType))
        {
            reporter.ReportDiagnostic(SimulationRules.ReturnTypeMustMatchReturnTypeOfSimulatedMethod,
                actualReturnType.Name,
                inputArgInfo.SimulatedInterface.Name, simulatedMethod.Name,
                simulatedMethod.ReturnType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
        }
    }


    private static IMethodSymbol? GetSimulatedMethod(InputArgumentStructureInfo opcArgsAttribute,
        InterfaceUsageReporter reporter)
    {
        if (opcArgsAttribute.ArgsAttrData == null || opcArgsAttribute.SimulatedInterface == null!)
        {
            return null;
        }

        var targetMethod = opcArgsAttribute
            .SimulatedInterface
            .GetMembers()
            .OfType<IMethodSymbol>()
            .FirstOrDefault(m => m.Name.Equals(opcArgsAttribute.TargetedMethod));

        if (targetMethod is null)
        {
            var interfaceName = opcArgsAttribute.SimulatedInterface.Name;
            reporter.ReportDiagnostic(SimulationRules.MethodTargetedByTArgsDoesNotExistOnTheTargetedInterface,
                interfaceName,
                opcArgsAttribute.TargetedMethod ?? "NULL");
        }

        return targetMethod;
    }

    private static InputArgumentStructureInfo GetInputArgumentStructureInfo(INamedTypeSymbol configuratorInterface,
        InterfaceUsageReporter reporter)
    {
        var argsType = configuratorInterface.TypeArguments[1];
        var argsAttrData = argsType.GetOpcArgsAttribute();
        if (argsAttrData is null)
        {
            reporter.ReportDiagnostic(SimulationRules.TArgsMustBeAnnotatedWithOpcEntityMethodArgs);
            return default;
        }

        var simulatedInterface = argsAttrData.AttributeClass?.TypeArguments[1];
        var methodName = argsAttrData.ConstructorArguments[0].Value as string;
        return new InputArgumentStructureInfo(argsAttrData, simulatedInterface, methodName);
    }


    private static IEnumerable<INamedTypeSymbol> GetImplementedConfiguratorInterfaces(
        FullyQualifiedTypeName wanted,
        INamedTypeSymbol implementor,
        SyntaxNodeAnalysisContext context)
    {
        var simulationInterface = context.Compilation.GetTypeByMetadataName(wanted.WithoutGlobalPrefix);
        if (simulationInterface is null)
        {
            return [];
        }

        return implementor
            .AllInterfaces
            .Where(iface =>
                iface is not null &&
                iface.OriginalDefinition.Equals(simulationInterface, SymbolEqualityComparer.Default));
    }


    private static SimulationConfigurationUsage CreateConfigurationSimulationUsage(SyntaxNodeAnalysisContext context,
        TypeDeclarationSyntax node, INamedTypeSymbol iFace)
    {
        var reporter = new InterfaceUsageReporter(context, node, iFace);
        var opcArgsAttribute = GetInputArgumentStructureInfo(iFace, reporter);
        var simulatedMethod = GetSimulatedMethod(opcArgsAttribute, reporter);
        return new SimulationConfigurationUsage(simulatedMethod!, iFace, opcArgsAttribute);
    }


    private sealed record SimulationConfigurationUsage(
        IMethodSymbol SimulatedMethod,
        INamedTypeSymbol ConfiguratorInterface,
        InputArgumentStructureInfo InterfaceInformation)
    {
        public IMethodSymbol SimulatedMethod { get; } = SimulatedMethod;
        public INamedTypeSymbol ConfiguratorInterface { get; } = ConfiguratorInterface;
        public InputArgumentStructureInfo InterfaceInformation { get; } = InterfaceInformation;
    }

    private record struct InputArgumentStructureInfo(
        AttributeData? ArgsAttrData,
        ITypeSymbol SimulatedInterface,
        string? TargetedMethod);
}