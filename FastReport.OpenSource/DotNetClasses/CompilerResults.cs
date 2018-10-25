using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace System.CodeDom.Compiler
{
    internal class CompilerResults
    {
        public List<CompilerError> Errors { get; internal set; } = new List<CompilerError>();
        public Assembly CompiledAssembly { get; internal set; } = null;
    }
}
