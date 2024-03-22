#if NETSTANDARD2_0 || NETSTANDARD2_1 || NETCOREAPP
using Microsoft.CodeAnalysis;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace FastReport.Code.CodeDom.Compiler
{
    public class CompilationEventArgs : EventArgs
    {
        public CompilationEventArgs(Compilation compilation)
        {
            Compilation = compilation;
        }

        public Compilation Compilation { get; }

    }
}
#endif