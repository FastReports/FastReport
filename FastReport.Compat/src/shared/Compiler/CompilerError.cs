#if NETSTANDARD2_0 || NETSTANDARD2_1 || NETCOREAPP
using System;
using System.Collections.Generic;
using System.Text;

namespace FastReport.Code.CodeDom.Compiler
{
    public class CompilerError
    {
        public int Line { get; set; }
        public int Column { get; set; }
        public string ErrorText { get; set; }
        public string ErrorNumber { get; set; }

    }
}

#endif