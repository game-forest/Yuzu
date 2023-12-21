using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SourceGenerator
{
	[Generator]
	public class GenerateStaticYuzuSerializeItemIfMethod : IIncrementalGenerator
	{
		private const string SerializeItemIfAttributeString = "Yuzu.YuzuSerializeItemIf";

		public void Initialize(IncrementalGeneratorInitializationContext context)
		{
			var provider1 = context.SyntaxProvider.ForAttributeWithMetadataName(
				SerializeItemIfAttributeString,
				(syntaxNode, ct) => {
					return syntaxNode is MethodDeclarationSyntax mds;
				},
				(context, ct) => {
					return context.SemanticModel.GetDeclaredSymbol(context.TargetNode);
				}
			);
			context.RegisterSourceOutput(
				provider1,
				(context, mdSymbol) => {
					if (mdSymbol.IsStatic) {
						return;
					}
					var containingTypes = new List<INamedTypeSymbol>();
					var ct = mdSymbol.ContainingType;
					while (ct != null)
					{
						containingTypes.Add(ct);
						ct = ct.ContainingType;
					}
					var cns = containingTypes.Last().ContainingNamespace;
					int indentLevel = 0;
					var sb = new StringBuilder();
					P("// <auto-generated/>");
					P("using System;");
					P($"namespace {cns.ToDisplayString()};");
					P("");
					string className = "";
					for (int i = containingTypes.Count - 1; i >= 0; i--)
					{
						ct = containingTypes[i];
						var accessibiliy = ct.DeclaredAccessibility;
						className = ct.ToDisplayString(new SymbolDisplayFormat(
							genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters
							| SymbolDisplayGenericsOptions.IncludeVariance
							| SymbolDisplayGenericsOptions.IncludeTypeConstraints
						));
						var typeKind = ct.TypeKind.ToString().ToLower();
						if (typeKind == "structure") typeKind = "struct";
						var recordString = ct.IsRecord ? "record " : "";
						var staticString = ct.IsStatic ? "static " : "";
						var readonlyString = ct.IsReadOnly ? "readonly " : "";
						P(
							$"{accessibiliy.ToString().ToLower()} " +
							$"{staticString}{readonlyString}partial {recordString}{typeKind} {className}"
						);
						P("{");
						indentLevel++;
					}
					P($"[{SerializeItemIfAttributeString}]");
					P($"public static bool Yuzu_{mdSymbol.Name}_Static(object collection, int index, object item)");
					P("{");
					indentLevel++;
					P($"return (({className})collection).{mdSymbol.Name}(index, item);");
					indentLevel--;
					P("}");
					for (int i = containingTypes.Count - 1; i >= 0; i--)
					{
						indentLevel--;
						P("}");
					}
					var prefix = string.Join("_", containingTypes.Select(s => s.Name).Reverse());
					context.AddSource($"{prefix}_{mdSymbol.Name}.g.cs", sb.ToString());
					void P(string s)
					{
						if (indentLevel > 0 || !string.IsNullOrEmpty(s))
						{
							sb.Append('\t', indentLevel);
							sb.Append(s);
						}
						sb.Append("\n");
					}
				}
			);
		}
	}
}
