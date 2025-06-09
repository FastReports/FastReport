using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FastReport.Code
{
    /// <summary>
    /// Provides a set of methods working with the report code compilation.
    /// </summary>
    public abstract class CodeProvider
    {
        /// <summary>
        /// Gets a report instance.
        /// </summary>
        public Report Report { get; }

        internal List<AssemblyDescriptor> Assemblies { get; }

        /// <summary>
        /// Creates a new instance of assembly descriptor specific to this provider.
        /// </summary>
        /// <param name="scriptText">Script text.</param>
        /// <returns>Assembly descriptor instance..</returns>
        public abstract AssemblyDescriptor CreateAssemblyDescriptor(string scriptText);

        /// <summary>
        /// Clears the inner state.
        /// </summary>
        public virtual void Clear()
        {
            Assemblies.Clear();
        }

        /// <summary>
        /// Compiles the report's script.
        /// </summary>
        /// <inheritdoc/>
        public virtual void Compile()
        {
            using (var descriptor = CreateAssemblyDescriptor(Report.ScriptText))
            {
                Assemblies.Clear();
                Assemblies.Add(descriptor);
                descriptor.AddObjects();
                descriptor.AddExpressions();
                descriptor.AddFunctions();
                descriptor.Compile();
            }
        }

        /// <summary>
        /// Compiles the report's script async way.
        /// </summary>
        public virtual async Task CompileAsync(CancellationToken token)
        {
            using (var descriptor = CreateAssemblyDescriptor(Report.ScriptText))
            {
                Assemblies.Clear();
                Assemblies.Add(descriptor);
                descriptor.AddObjects();
                descriptor.AddExpressions();
                descriptor.AddFunctions();
                await descriptor.CompileAsync(token);
            }
        }

        /// <summary>
        /// Initializes the report's fields.
        /// </summary>
        public virtual void InternalInit()
        {
            var descriptor = CreateAssemblyDescriptor(Report.CodeHelper.EmptyScript());
            Assemblies.Clear();
            Assemblies.Add(descriptor);
            descriptor.InitInstance(Report);
        }

        /// <summary>
        /// Generates the file (.cs or .vb) that contains the report source code.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public virtual void GenerateReportAssembly(string fileName)
        {
            // create the class name
            string className = "";
            const string punctuation = " ~`!@#$%^&*()-=+[]{},.<>/?;:'\"\\|";

            foreach (char c in Path.GetFileNameWithoutExtension(fileName))
            {
                if (!punctuation.Contains(c.ToString()))
                    className += c;
            }

            var descriptor = CreateAssemblyDescriptor(Report.ScriptText);
            descriptor.AddObjects();
            descriptor.AddExpressions();
            descriptor.AddFunctions();

            string reportClassText = descriptor.GenerateReportClass(className);
            File.WriteAllText(fileName, reportClassText, Encoding.UTF8);
        }

        /// <summary>
        /// Tries to calculate an expression.
        /// </summary>
        /// <param name="expression">The expression to calc.</param>
        /// <param name="value">The value parameter.</param>
        /// <param name="result">The calculation result.</param>
        /// <returns><b>true</b> if the expression was calculated.</returns>
        protected virtual bool TryCalcExpression(string expression, Variant value, out object result)
        {
            result = null;

            // try to calculate the expression
            foreach (var d in Assemblies)
            {
                if (d.ContainsExpression(expression))
                {
                    result = d.CalcExpression(expression, value);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns an expression value.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="value">The value of currently printing object.</param>
        /// <returns>Returns the result of calculation.</returns>
        public virtual object CalcExpression(string expression, Variant value)
        {
            if (TryCalcExpression(expression, value, out var result))
                return result;

            // expression not found. Probably it was added after the start of the report.
            // Compile new assembly containing this expression.
            using (var descriptor = CreateAssemblyDescriptor(Report.CodeHelper.EmptyScript()))
            {
                Assemblies.Add(descriptor);
                descriptor.AddObjects();
                descriptor.AddSingleExpression(expression);
                descriptor.AddFunctions();
                descriptor.Compile();
                return descriptor.CalcExpression(expression, value);
            }
        }

        /// <summary>
        /// Returns an expression value.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="value">The value of currently printing object.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>Returns the result of calculation.</returns>
        public virtual async Task<object> CalcExpressionAsync(string expression, Variant value, CancellationToken token)
        {
            if (TryCalcExpression(expression, value, out var result))
                return result;

            // expression not found. Probably it was added after the start of the report.
            // Compile new assembly containing this expression.
            using (var descriptor = CreateAssemblyDescriptor(Report.CodeHelper.EmptyScript()))
            {
                Assemblies.Add(descriptor);
                descriptor.AddObjects();
                descriptor.AddSingleExpression(expression);
                descriptor.AddFunctions();
                await descriptor.CompileAsync(token);
                return descriptor.CalcExpression(expression, value);
            }
        }

        /// <summary>
        /// Invokes the script method with given name.
        /// </summary>
        /// <param name="name">The name of the script method.</param>
        /// <param name="parms">The method parameters.</param>
        /// <returns>Returns the result of invokation.</returns>
        public virtual object InvokeMethod(string name, object[] parms)
        {
            if (Assemblies.Count > 0)
                return Assemblies[0].InvokeMethod(name, parms);

            return null;
        }

        /// <summary>
        /// Initializes a new instance of CodeProvider class.
        /// </summary>
        public CodeProvider(Report report)
        {
            Report = report;
            Assemblies = new List<AssemblyDescriptor>();
        }

        private static Type _defaultProvider = typeof(FastReport.Code.Ms.MsCodeProvider);

        /// <summary>
        /// Gets or sets a default code provider class.
        /// </summary>
        /// <remarks>
        /// Default value is MsCodeProvider. Assigning a null to this property resets it to default value.
        /// </remarks>
        public static Type DefaultProvider
        {
            get => _defaultProvider;
            set
            {
                value ??= typeof(FastReport.Code.Ms.MsCodeProvider);

                if (!typeof(CodeProvider).IsAssignableFrom(value))
                {
                    throw new ArgumentException("value must be of CodeProvider type");
                }

                _defaultProvider = value;
            }
        }

        internal static CodeProvider GetCodeProvider(Report report)
        {
            return Activator.CreateInstance(DefaultProvider, report) as CodeProvider;
        }
    }
}