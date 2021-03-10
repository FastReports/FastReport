using System.Collections.Specialized;
#if !NETFRAMEWORK
using FastReport.Code.CodeDom.Compiler;
#else
using System.CodeDom.Compiler;
#endif

namespace FastReport.Code
{
    partial class AssemblyDescriptor
    {
        partial void ErrorMsg(CompilerError ce, int number);

        partial void ErrorMsg(string str, CompilerError ce);

        partial void ErrorMsg(string str);

        partial void ReviewReferencedAssemblies(StringCollection referencedAssemblies);
    }
}
