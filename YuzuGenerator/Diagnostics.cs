using Microsoft.CodeAnalysis;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace SourceGenerator;

internal class Diagnostics
{
	public static void ReportExceptionDiagnostic(
		SourceProductionContext context,
		Exception exception,
		Func<Exception, Diagnostic> diagnosticFactory
	) {
		try {
			var diagnostic = diagnosticFactory(exception);
			context.ReportDiagnostic(diagnostic);
			var exceptionInfo = "#error " + exception.ToString().Replace("\n", "\n//");
			context.AddSource($"error_{diagnostic.Descriptor.Id}_{Guid.NewGuid()}", exceptionInfo);
		} catch {
		}
	}

	public static Diagnostic CreateExceptionDiagnostic(
		Exception exception, ImmutableArray<Location>? locations
	) {
		return Diagnostic.Create(
			UnhandledException, locations?.First(), locations?.Skip(1), exception?.GetType(), exception?.Message
		);
	}

	public static readonly DiagnosticDescriptor UnhandledException =
		new (
			"YZ1001",
			"Unhandled exception occurred",
			"YuzuGenerator caused an exception {0}: {1}",
			Category,
			DiagnosticSeverity.Error,
			true
		);

	private const string Category = "Usage";
}

