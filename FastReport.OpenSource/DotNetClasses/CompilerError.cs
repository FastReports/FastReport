using System;
using System.Collections.Generic;
using System.Text;

namespace System.CodeDom.Compiler
{
    internal class CompilerError
    {
        public int Line { get; internal set; }
        public object Column { get; internal set; }
        public object ErrorText { get; internal set; }
        public object ErrorNumber { get; internal set; }
    }
}
