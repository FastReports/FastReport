#if NETSTANDARD2_0 || NETSTANDARD2_1 || NETCOREAPP
using System;
using System.Collections.Generic;
using System.Text;

namespace System.CodeDom.Compiler
{
    internal class CodeGenerator
    {
        internal static bool IsValidLanguageIndependentIdentifier(string value)
        {
            //TODO o_0 what????
            return true;
        }
    }
}
#endif