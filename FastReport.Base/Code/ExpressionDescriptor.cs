namespace FastReport.Code
{
    /// <summary>
    /// Represents a descriptor used to calculate an expression.
    /// </summary>
    public abstract class ExpressionDescriptor
    {
        /// <summary>
        /// A reference to assembly descriptor.
        /// </summary>
        public AssemblyDescriptor Assembly { get; }

        /// <summary>
        /// The name of a method used to calculate an expression.
        /// </summary>
        public string MethodName { get; }

        /// <summary>
        /// Invokes a method used to calculate an expression.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public abstract object Invoke(object[] parameters);

        /// <summary>
        /// Initializes a new instance of expression descriptor class.
        /// </summary>
        /// <param name="assembly">The parent assembly descriptor.</param>
        /// <param name="methodName">The name of method used to calculate an expression.</param>
        public ExpressionDescriptor(AssemblyDescriptor assembly, string methodName)
        {
            Assembly = assembly;
            MethodName = methodName;
        }
    }
}