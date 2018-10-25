using System;

namespace System.CodeDom.Compiler
{
    internal abstract class CodeDomProvider : IDisposable
    {
        public abstract void Dispose();
        public abstract CompilerResults CompileAssemblyFromSource(CompilerParameters cp, string v);
    }
}