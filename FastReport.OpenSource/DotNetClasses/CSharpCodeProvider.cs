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
        static Dictionary<string, string> cache = new Dictionary<string, string>();

        public override CompilerResults CompileAssemblyFromSource(CompilerParameters cp, string code)
        {
            CodeAnalysis.SyntaxTree codeTree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(code);
            CSharpCompilationOptions options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel: OptimizationLevel.Release,
                generalDiagnosticOption: ReportDiagnostic.Error,
                reportSuppressedDiagnostics: true);

            List<MetadataReference> references = new List<MetadataReference>();


            foreach (string reference in cp.ReferencedAssemblies)
                references.Add(GetReference(reference));

            if (!cp.ReferencedAssemblies.Contains("netstandard"))
                references.Add(GetReference("netstandard"));

            if (!cp.ReferencedAssemblies.Contains("System.Runtime"))
                references.Add(GetReference("System.Runtime"));

            if (!cp.ReferencedAssemblies.Contains("System.ComponentModel.Primitives"))
                references.Add(GetReference("System.ComponentModel.Primitives"));


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
                        result.Errors.Add(new CompilerError()
                        {
                            ErrorText = d.GetMessage(),
                            ErrorNumber = d.Id,
                            Line = d.Location.GetLineSpan().StartLinePosition.Line,
                            Column = d.Location.GetLineSpan().StartLinePosition.Character
                        });
                    }
                    return result;
                }
            }

        }

        public override void Dispose()
        {
            
        }

        internal MetadataReference GetReference(string refDll)
        {
            string reference = refDll;
            try
            {
                if (cache.ContainsKey(refDll))
                    return MetadataReference.CreateFromFile(cache[refDll]);
                MetadataReference result;
                foreach (AssemblyName name in Assembly.GetExecutingAssembly().GetReferencedAssemblies())
                {
                    if (name.Name == reference
                        || reference.ToLower().EndsWith(".dll")
                        && name.Name == reference.Substring(0, reference.Length - 4))
                    {
                        result = MetadataReference.CreateFromFile(
                            Assembly.Load(name).Location);
                        cache[refDll] = reference;
                        return result;
                    }
                }

                result = MetadataReference.CreateFromFile(reference);
                cache[refDll] = reference;
                return result;
            }
            catch
            {
                string ass = reference;
                if (reference.ToLower().EndsWith(".dll"))
                    ass = reference.Substring(0, reference.Length - 4);
                cache[refDll] = Assembly.Load(new AssemblyName(ass)).Location;
                return MetadataReference.CreateFromFile(cache[refDll]);
            }

        }
    }
}