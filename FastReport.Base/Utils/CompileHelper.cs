using System;
#if NETSTANDARD || NETCOREAPP
using FastReport.Code.CodeDom.Compiler;
using FastReport.Code.CSharp;
#else
using System.CodeDom.Compiler;
using Microsoft.CSharp;
#endif
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace FastReport.Utils
{
    /// <summary>
    /// Class helper for compile source code with path of assemblies 
    /// </summary>
    public static class CompileHelper
    {
        /// <summary>
        /// Generate a assembly in memory with some source code and several path for additional assemblies
        /// </summary>
        /// <param name="sourceCode"></param>
        /// <param name="assemblyPaths"></param>
        /// <returns></returns>
        public static Assembly GenerateAssemblyInMemory(string sourceCode, params string[] assemblyPaths)
        {
            using (CSharpCodeProvider compiler = new CSharpCodeProvider())
            {
                CompilerParameters parameters = new CompilerParameters();
                parameters.GenerateInMemory = true;

                foreach (string asm in assemblyPaths)
                {
                    parameters.ReferencedAssemblies.Add(asm);
                }

#if NETSTANDARD2_0 || NETSTANDARD2_1

                var mscorPath = compiler.GetReference("System.Private.CoreLib.dll").Display;
                parameters.ReferencedAssemblies.Add(mscorPath);
#endif

                CompilerResults results = compiler.CompileAssemblyFromSource(parameters, sourceCode);
                return results.CompiledAssembly;
            }
        }
    }
}
