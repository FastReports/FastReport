using FastReport.Data;
using FastReport.Engine;
using FastReport.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FastReport.Code
{
    /// <summary>
    /// Represents an assembly with report script code.
    /// </summary>
    public abstract class AssemblyDescriptor : IDisposable
    {
        private readonly List<SourcePosition> _sourcePositions;
        private int _insertLine;
        private int _insertPos;

        /// <summary>
        /// Gets a script text.
        /// </summary>
        protected StringBuilder ScriptText { get; }

        /// <summary>
        /// Indicates that a compilation is required.
        /// </summary>
        protected bool NeedCompile { get; private set; }

        /// <summary>
        /// Gets a reference to parent Report object.
        /// </summary>
        public Report Report { get; }

        internal Hashtable Expressions { get; }

        /// <summary>
        /// Gets or sets an instance of report script class.
        /// </summary>
        public object Instance { get; protected set; }

        /// <summary>
        /// Inits a field of report script class instance.
        /// </summary>
        /// <param name="name">Name of a field.</param>
        /// <param name="c">Value of a field.</param>
        protected abstract void InitField(string name, object c);

        /// <summary>
        /// Creates a new expression descriptor.
        /// </summary>
        /// <param name="methodName">Name of a method used to calculate an expression.</param>
        /// <returns>A new instance of descriptor.</returns>
        protected abstract ExpressionDescriptor CreateExpressionDescriptor(string methodName);

        private void InsertItem(string text, string objName)
        {
            string[] lines = text.Split('\r');
            ScriptText.Insert(_insertPos, text);
            
            var pos = new SourcePosition(objName, _insertLine, _insertLine + lines.Length - 2);
            _sourcePositions.Add(pos);
            _insertLine += lines.Length - 1;
            _insertPos += text.Length;
        }

        private void InitFields()
        {
            InitField("Report", Report);
            InitField("Engine", Report.Engine);

            foreach (Base c in Report.AllObjects)
            {
                if (!String.IsNullOrEmpty(c.Name))
                    InitField(c.Name, c);
            }
        }

        private void AddExpression(string expression, Base source, bool forceSimpleItems)
        {
            if (expression.Trim() == "" || Expressions.ContainsKey(expression))
                return;

            string expr = expression;

            if (expr.StartsWith("[") && expr.EndsWith("]"))
                expr = expr.Substring(1, expr.Length - 2);

            // skip simple items. Report.Calc does this.
            if (!forceSimpleItems)
            {
                if (DataHelper.IsSimpleColumn(Report.Dictionary, expr) ||
                    DataHelper.IsValidParameter(Report.Dictionary, expr) ||
                    DataHelper.IsValidTotal(Report.Dictionary, expr))
                    return;
            }

            // generate an error if a column that has been disabled is used
            Column column = DataHelper.GetColumn(Report.Dictionary, expr);
            
            if (column != null && !column.Enabled)
                expr += "Disable";

            // handle complex expressions, relations
            Expressions.Add(expression, CreateExpressionDescriptor("CalcExpression"));

            if (DataHelper.IsValidColumn(Report.Dictionary, expr))
            {
                expr = "[" + expr + "]";
            }
            else
            {
                expr = expression;
            }

            string expressionCode = ReplaceDataItems(Report, expr);
            InsertItem(Report.CodeHelper.AddExpression(expression, expressionCode), source == null ? "" : source.Name);
            NeedCompile = true;
        }

        /// <summary>
        /// Gets a name of report object in which an error occured.
        /// </summary>
        /// <param name="errorLine">Line number of script code with an error.</param>
        /// <returns>An object name, if any, or an empty string.</returns>
        protected string GetErrorObjectName(int errorLine)
        {
            foreach (var pos in _sourcePositions)
            {
                if (errorLine >= pos.start && errorLine <= pos.end)
                    return pos.sourceObject;
            }

            return "";
        }

        /// <summary>
        /// Gets actual line of script code.
        /// </summary>
        /// <param name="errorLine">Line number of script code with an error.</param>
        /// <returns>Actual line of script code.</returns>
        protected int GetScriptLine(int errorLine)
        {
            int start = _sourcePositions[0].start;
            int end = _sourcePositions[_sourcePositions.Count - 1].end;
            
            if (errorLine >= start && errorLine <= end)
                return -1;
            
            if (errorLine > end)
                return errorLine - (end - start + 1);
            
            return errorLine;
        }

        /// <summary>
        /// Replaces data items in an expression with calls to Calc.
        /// </summary>
        /// <param name="report">The report instance.</param>
        /// <param name="expression">The expression.</param>
        /// <returns>A new expression.</returns>
        public static string ReplaceDataItems(Report report, string expression)
        {
            var args = new FindTextArgs();
            args.Text = new FastString(expression);
            args.OpenBracket = "[";
            args.CloseBracket = "]";

            while (args.StartIndex < args.Text.Length)
            {
                expression = CodeUtils.GetExpression(args, true);
                if (expression == null)
                    break;

                if (DataHelper.IsValidColumn(report.Dictionary, expression))
                {
                    Type type = DataHelper.GetColumnType(report.Dictionary, expression);
                    expression = report.CodeHelper.ReplaceColumnName(expression, type);
                }
                else if (DataHelper.IsValidParameter(report.Dictionary, expression))
                {
                    expression = report.CodeHelper.ReplaceParameterName(DataHelper.GetParameter(report.Dictionary, expression));
                }
                else if (DataHelper.IsValidTotal(report.Dictionary, expression))
                {
                    expression = report.CodeHelper.ReplaceTotalName(expression);
                }
                else
                {
                    expression = "[" + ReplaceDataItems(report, expression) + "]";
                }

                args.Text = args.Text.Remove(args.StartIndex, args.EndIndex - args.StartIndex);
                args.Text = args.Text.Insert(args.StartIndex, expression);
                args.StartIndex += expression.Length;
            }

            return args.Text.ToString();
        }

        /// <summary>
        /// Replaces a text with an eroor message.
        /// </summary>
        /// <param name="error">Error message.</param>
        /// <param name="text">Text object with text to replace.</param>
        /// <returns>A new text.</returns>
        protected string ReplaceExpression(string error, TextObjectBase text)
        {
            string result = text.Text;
            string[] parts = error.Split('\"');

            if (parts.Length == 3)
            {
                string[] expressions = text.GetExpressions();

                foreach (string expr in expressions)
                {
                    if (expr.Contains(parts[1]))
                    {
                        if (!DataHelper.IsValidColumn(Report.Dictionary, expr))
                        {
                            string replaceString = text.Brackets[0] + expr + text.Brackets[2];

                            if (Config.CompilerSettings.ExceptionBehaviour == CompilerExceptionBehaviour.ShowExceptionMessage ||
                                Config.CompilerSettings.ExceptionBehaviour == CompilerExceptionBehaviour.ReplaceExpressionWithPlaceholder)
                            {
                                result = result.Replace(replaceString, Config.CompilerSettings.Placeholder);
                            }
                            else if (Config.CompilerSettings.ExceptionBehaviour == CompilerExceptionBehaviour.ReplaceExpressionWithExceptionMessage)
                            {
                                result = result.Replace(replaceString, error);
                            }
                        }
                    }
                }
            }

            return result;
        }

        internal void AddObjects()
        {
            InsertItem(Report.CodeHelper.AddField(typeof(Report), "Report") +
                Report.CodeHelper.AddField(typeof(ReportEngine), "Engine"), "Report");

            // add all report objects
            var objects = new SortedList<string, Base>();

            foreach (Base c in Report.AllObjects)
            {
                if (!String.IsNullOrEmpty(c.Name) && !objects.ContainsKey(c.Name))
                    objects.Add(c.Name, c);
            }

            foreach (Base c in objects.Values)
            {
                InsertItem(Report.CodeHelper.AddField(c.GetType(), c.Name), c.Name);
            }

            // add custom script
            string processedCode = "";
            
            foreach (Base c in objects.Values)
            {
                string customCode = c.GetCustomScript();
                
                // avoid custom script duplicates
                if (!String.IsNullOrEmpty(customCode) && processedCode.IndexOf(customCode) == -1)
                {
                    InsertItem(customCode, c.Name);
                    processedCode += customCode;
                    NeedCompile = true;
                }
            }
        }

        internal void AddSingleExpression(string expression)
        {
            InsertItem(Report.CodeHelper.BeginCalcExpression(), "");
            AddExpression(expression, null, true);
            InsertItem(Report.CodeHelper.EndCalcExpression(), "");
            NeedCompile = true;
        }

        internal void AddExpressions()
        {
            // speed up the case: lot of report objects (> 1000) and lot of data columns in the dictionary (> 10000). 
            Report.Dictionary.CacheAllObjects = true;

            InsertItem(Report.CodeHelper.BeginCalcExpression(), "");

            ObjectCollection allObjects = Report.AllObjects;
            
            foreach (Base c in Report.Dictionary.AllObjects)
            {
                allObjects.Add(c);
            }

            foreach (Base c in allObjects)
            {
                string[] expressions = c.GetExpressions();
                
                if (expressions != null)
                {
                    foreach (string expr in expressions)
                    {
                        AddExpression(expr, c, false);
                    }
                }
            }

            InsertItem(Report.CodeHelper.EndCalcExpression(), "");
            Report.Dictionary.CacheAllObjects = false;
        }

        internal void AddFunctions()
        {
            var list = new List<FunctionInfo>();
            RegisteredObjects.Functions.EnumItems(list);

            foreach (var info in list)
            {
                if (info.Function != null)
                {
                    InsertItem(Report.CodeHelper.GetMethodSignatureAndBody(info.Function), "Function");
                }
            }
        }

        internal string GenerateReportClass(string className)
        {
            InsertItem(Report.CodeHelper.GenerateInitializeMethod(), "");
            return Report.CodeHelper.ReplaceClassName(ScriptText.ToString(), className);
        }

        /// <summary>
        /// Compiles an assembly.
        /// </summary>
        public abstract void Compile();

        /// <summary>
        /// Compiles an assembly, async way.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task result.</returns>
        public abstract Task CompileAsync(CancellationToken token = default);

        /// <summary>
        /// Inits the instance of the report script.
        /// </summary>
        /// <param name="instance">An instance.</param>
        public void InitInstance(object instance)
        {
            Instance = instance;
            InitFields();
        }

        internal bool ContainsExpression(string expr)
        {
            return Expressions.ContainsKey(expr);
        }

        internal object CalcExpression(string expr, Variant value)
        {
            var expressionDescriptor = Expressions[expr] as ExpressionDescriptor;

            if (expressionDescriptor != null)
            {
                return expressionDescriptor.Invoke(new object[] { expr, value });
            }
            else
            {
                return null;
            }
        }

        internal object InvokeMethod(string name, object[] parms)
        {
            if (String.IsNullOrEmpty(name))
                return null;

            string exprName = "method_" + name;
            
            if (!ContainsExpression(exprName))
            {
                Expressions.Add(exprName, CreateExpressionDescriptor(name));
            }

            try
            {
                return (Expressions[exprName] as ExpressionDescriptor).Invoke(parms);
            }
            catch (TargetInvocationException ex)
            {
                throw (ex.InnerException); // ex now stores the original success
            }
        }

        /// <summary>
        /// Disposes any unmanaged resources used during the compilation.
        /// </summary>
        public virtual void Dispose()
        {
        }

        /// <summary>
        /// Initializes a new instance of the assembly descriptor.
        /// </summary>
        /// <param name="report">The report instance.</param>
        /// <param name="scriptText">The script text.</param>
        /// <exception cref="CompilerException"></exception>
        public AssemblyDescriptor(Report report, string scriptText)
        {
            Report = report;
            ScriptText = new StringBuilder(scriptText);
            Expressions = new Hashtable();
            _sourcePositions = new List<SourcePosition>();
            _insertPos = Report.CodeHelper.GetPositionToInsertOwnItems(scriptText);
            
            if (_insertPos == -1)
            {
                throw new CompilerException(Res.Get("Messages,ClassError"), null);
            }
            else
            {
                string[] lines = scriptText.Substring(0, _insertPos).Split('\r');
                _insertLine = lines.Length;
                
                if (scriptText != Report.CodeHelper.EmptyScript())
                    NeedCompile = true;
            }
        }


        private class SourcePosition
        {
            public readonly string sourceObject;
            public readonly int start;
            public readonly int end;

            public SourcePosition(string obj, int start, int end)
            {
                sourceObject = obj;
                this.start = start;
                this.end = end;
            }
        }
    }
}
