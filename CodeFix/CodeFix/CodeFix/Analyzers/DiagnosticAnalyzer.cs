using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeFix
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ReturnTaskRequiresAsyncAnalyzer : DiagnosticAnalyzer
    {
        #region Constats

        public const string DiagnosticId = nameof(ReturnTaskRequiresAsyncAnalyzer);

        private const string Title = "Method which returns Task(<T>) whose name not ends with Async";
        private const string MessageFormat = "Method which returns '{0}' whose name '{1}' doesn't end with Async";
        private const string Category = "Naming";

        #endregion

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Method);
        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            var methodSymbol = (IMethodSymbol)context.Symbol;
            var returnType = methodSymbol.ReturnType;
            if (returnType.ContainingAssembly.Name != "mscorlib")
            {
                return;
            }

            if (returnType.Name.StartsWith("Task") && !methodSymbol.Name.EndsWith("Async"))
            {
                // For all such symbols, produce a diagnostic.
                var diagnostic = Diagnostic.Create(Rule, methodSymbol.Locations[0], methodSymbol.ReturnType.Name, methodSymbol.Name);

                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
