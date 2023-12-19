#if NETSTANDARD2_0 || NETSTANDARD2_1 || NETCOREAPP
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using FastReport.Code.CodeDom.Compiler;
using System.Threading;

namespace FastReport.Code.VisualBasic
{
    public class VBCodeProvider : CodeDomProvider
    {
        
        public override void Dispose()
        {

        }

        protected override Compilation CreateCompilation(SyntaxTree codeTree, ICollection<MetadataReference> references)
        {
            VisualBasicCompilationOptions options = new VisualBasicCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                true,
                embedVbCoreRuntime: true,
                optimizationLevel: OptimizationLevel.Release,
                generalDiagnosticOption: ReportDiagnostic.Default);

            Compilation compilation = VisualBasicCompilation.Create(
                "_" + Guid.NewGuid().ToString("D"), new SyntaxTree[] { codeTree },
                references: references, options: options
                );
            return compilation;
        }

        protected override SyntaxTree ParseTree(string text, CancellationToken ct = default)
            => VisualBasicSyntaxTree.ParseText(text,
                cancellationToken: ct);
    }
}
#endif