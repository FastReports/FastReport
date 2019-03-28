using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Reflection;
using System.Text;


namespace Microsoft.CSharp
{
    internal class CSharpCodeProvider : CodeDomProvider
    {


        public override CompilerResults CompileAssemblyFromSource(CompilerParameters cp, string code)
        {
            CodeAnalysis.SyntaxTree codeTree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(code);
            CSharpCompilationOptions options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel: OptimizationLevel.Release,
                generalDiagnosticOption: ReportDiagnostic.Default,
                reportSuppressedDiagnostics: true);

            List<MetadataReference> references = new List<MetadataReference>();


            foreach (string reference in cp.ReferencedAssemblies)
                references.Add(GetReference(reference));

            AddExtraAssemblies(cp.ReferencedAssemblies, references);


            Compilation compilation = CSharpCompilation.Create(
                "_" + Guid.NewGuid().ToString("D"), new SyntaxTree[] { codeTree },
                references: references, options: options
                );
            


            using (MemoryStream ms = new MemoryStream())
            {
                CodeAnalysis.Emit.EmitResult results = compilation.Emit(ms);
                if (results.Success)
                {
                    return new CompilerResults()
                    {
                        CompiledAssembly = Assembly.Load(ms.ToArray())
                    };
                }
                else
                {
                    CompilerResults result = new CompilerResults();
                    foreach (Diagnostic d in results.Diagnostics)
                    {
                        if (d.Severity == DiagnosticSeverity.Error)
                        {
                            result.Errors.Add(new CompilerError()
                            {
                                ErrorText = d.GetMessage(),
                                ErrorNumber = d.Id,
                                Line = d.Location.GetLineSpan().StartLinePosition.Line,
                                Column = d.Location.GetLineSpan().StartLinePosition.Character,

                            });
                        }
                    }
                    return result;
                }
            }

        }

        public override void Dispose()
        {
            
        }

    }
}