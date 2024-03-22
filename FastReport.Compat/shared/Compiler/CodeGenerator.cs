#if NETSTANDARD2_0 || NETSTANDARD2_1 || NETCOREAPP
using System;
using System.Collections.Generic;
using System.Text;

namespace FastReport.Code.CodeDom.Compiler
{
    public class CodeGenerator
    {
        public static bool IsValidLanguageIndependentIdentifier(string value)
        {
            //TODO o_0 what????
            return true;
        }
    }
}
#endif