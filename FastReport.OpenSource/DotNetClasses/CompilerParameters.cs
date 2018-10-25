using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace System.CodeDom.Compiler
{
    internal class CompilerParameters
    {
        public bool GenerateInMemory { get; internal set; }
        public StringCollection ReferencedAssemblies { get; internal set; } = new StringCollection();
        public TempFileCollection TempFiles { get; internal set; } = new TempFileCollection("", false);
    }
}
