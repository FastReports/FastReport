using System.Reflection;

namespace FastReport.Code
{
    internal class ExpressionDescriptor
    {
        private string methodName;
        private MethodInfo methodInfo;
        private readonly AssemblyDescriptor assembly;

        public string MethodName
        {
            get { return methodName; }
            set { methodName = value; }
        }

#pragma warning disable 618
        public object Invoke(object[] parameters)
        {
            if (assembly == null || assembly.Instance == null)
                return null;
            if (methodInfo == null)
                methodInfo = assembly.Instance.GetType().GetMethod(MethodName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (methodInfo == null)
                return null;
            return methodInfo.Invoke(assembly.Instance, parameters);
        }
#pragma warning restore 618

        public ExpressionDescriptor(AssemblyDescriptor assembly)
        {
            this.assembly = assembly;
        }
    }
}