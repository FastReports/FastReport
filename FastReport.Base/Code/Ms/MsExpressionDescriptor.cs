using System.Reflection;

namespace FastReport.Code.Ms
{
    internal class MsExpressionDescriptor : ExpressionDescriptor
    {
        private MethodInfo methodInfo;

        public override object Invoke(object[] parameters)
        {
            if (Assembly == null || Assembly.Instance == null)
                return null;

            if (methodInfo == null)
            {
                methodInfo = Assembly.Instance.GetType().GetMethod(MethodName,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            }

            if (methodInfo == null)
                return null;

#if NETCOREAPP
            return methodInfo.Invoke(Assembly.Instance, parameters);
#else
#pragma warning disable 618
            var restrictions = Assembly.Report.ScriptRestrictions;
            if (restrictions != null)
                restrictions.Deny();
            try
            {
                return methodInfo.Invoke(Assembly.Instance, parameters);
            }
            finally
            {
                if (restrictions != null)
                    System.Security.CodeAccessPermission.RevertDeny();
            }
#pragma warning restore 618
#endif
        }

        public MsExpressionDescriptor(MsAssemblyDescriptor assembly, string methodName) : base(assembly, methodName)
        {
        }
    }
}