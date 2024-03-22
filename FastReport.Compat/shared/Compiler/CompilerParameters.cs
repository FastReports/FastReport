#if NETSTANDARD2_0 || NETSTANDARD2_1 || NETCOREAPP
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace FastReport.Code.CodeDom.Compiler
{
    public class CompilerParameters
    {
        public bool GenerateInMemory { get; set; }
        public StringCollection ReferencedAssemblies { get; } = new StringCollection();
        public TempFileCollection TempFiles { get; set; } = new TempFileCollection("", false);
    }
}

#endif