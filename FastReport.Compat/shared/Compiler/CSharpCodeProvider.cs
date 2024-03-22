#if NETSTANDARD2_0 || NETSTANDARD2_1 || NETCOREAPP
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.CSharp;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Reflection;
using System.Text;
using FastReport.Code.CodeDom.Compiler;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Threading;

namespace FastReport.Code.CSharp
{
    public class CSharpCodeProvider : CodeDomProvider
    {

        protected override SyntaxTree ParseTree(string text, CancellationToken ct = default)
            => CSharpSyntaxTree.ParseText(text,
                cancellationToken: ct);


        private static CSharpCompilationOptions GetCompilationOptions()
        {
            CSharpCompilationOptions options = new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel: OptimizationLevel.Release,
                generalDiagnosticOption: ReportDiagnostic.Default,
                reportSuppressedDiagnostics: true);
            return options;
        }

        protected override Compilation CreateCompilation(SyntaxTree codeTree, ICollection<MetadataReference> references)
        {
            CSharpCompilationOptions options = GetCompilationOptions();
            Compilation compilation = CSharpCompilation.Create(
                "_" + Guid.NewGuid().ToString("D"), new SyntaxTree[] { codeTree },
                references: references, options: options
                );
            return compilation;
        }


        public override void Dispose()
        {
            
        }
    }
}
#endif