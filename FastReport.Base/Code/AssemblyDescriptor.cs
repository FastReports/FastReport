using System;
#if NETSTANDARD || NETCOREAPP
using FastReport.Code.CodeDom.Compiler;
#else
using System.CodeDom.Compiler;
#endif
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;
using FastReport.Data;
using FastReport.Engine;
using FastReport.Utils;

namespace FastReport.Code
{
    internal partial class AssemblyDescriptor
    {
        private static readonly ConcurrentDictionary<string, Assembly> FAssemblyCache;
        private readonly FastString scriptText;
        private readonly List<SourcePosition> sourcePositions;
        private int insertLine;
        private int insertPos;
        private bool needCompile;
        private const string shaKey = "FastReportCode";
        private readonly static object compileLocker;
        private readonly string currentFolder;

        private const int RECOMPILE_COUNT = 1;

        public Assembly Assembly { get; private set; }

        public object Instance { get; private set; }

        public Report Report { get; }

        public Hashtable Expressions { get; }

        private void InsertItem(string text, string objName)
        {
            string[] lines = text.Split(new char[] { '\r' });
            scriptText.Insert(insertPos, text);
            SourcePosition pos = new SourcePosition(objName, insertLine, insertLine + lines.Length - 2);
            sourcePositions.Add(pos);
            insertLine += lines.Length - 1;
            insertPos += text.Length;
        }

        private void InitField(string name, object c)
        {
            FieldInfo info = Instance.GetType().GetField(name,
              BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            info.SetValue(Instance, c);
        }

        private void InitFields()
        {
            InitField("Report", Report);
            InitField("Engine", Report.Engine);
            ObjectCollection allObjects = Report.AllObjects;
            foreach (Base c in allObjects)
            {
                if (!String.IsNullOrEmpty(c.Name))
                    InitField(c.Name, c);
            }
        }

        private string GetErrorObjectName(int errorLine)
        {
            foreach (SourcePosition pos in sourcePositions)
            {
                if (errorLine >= pos.start && errorLine <= pos.end)
                {
                    return pos.sourceObject;
                }
            }
            return "";
        }

        private int GetScriptLine(int errorLine)
        {
            int start = sourcePositions[0].start;
            int end = sourcePositions[sourcePositions.Count - 1].end;
            if (errorLine >= start && errorLine <= end)
                return -1;
            if (errorLine > end)
                return errorLine - (end - start + 1);
            return errorLine;
        }

        private string ReplaceDataItems(string expression)
        {
            FindTextArgs args = new FindTextArgs();
            args.Text = new FastString(expression);
            args.OpenBracket = "[";
            args.CloseBracket = "]";

            while (args.StartIndex < args.Text.Length)
            {
                expression = CodeUtils.GetExpression(args, true);
                if (expression == "")
                    break;

                if (DataHelper.IsValidColumn(Report.Dictionary, expression))
                {
                    Type type = DataHelper.GetColumnType(Report.Dictionary, expression);
                    expression = Report.CodeHelper.ReplaceColumnName(expression, type);
                }
                else if (DataHelper.IsValidParameter(Report.Dictionary, expression))
                {
                    expression = Report.CodeHelper.ReplaceParameterName(DataHelper.GetParameter(Report.Dictionary, expression));
                }
                else if (DataHelper.IsValidTotal(Report.Dictionary, expression))
                {
                    expression = Report.CodeHelper.ReplaceTotalName(expression);
                }
                else
                {
                    expression = "[" + ReplaceDataItems(expression) + "]";
                }

                args.Text = args.Text.Remove(args.StartIndex, args.EndIndex - args.StartIndex);
                args.Text = args.Text.Insert(args.StartIndex, expression);
                args.StartIndex += expression.Length;
            }
            return args.Text.ToString();
        }

        private bool ContansAssembly(IList assemblies, string assembly)
        {
            string asmName = Path.GetFileName(assembly);
            foreach (string a in assemblies)
            {
                string asmName1 = Path.GetFileName(a);
                if (String.Compare(asmName, asmName1, true) == 0)
                    return true;
            }
            return false;
        }

        private void AddFastReportAssemblies(IList assemblies)
        {
            List<ObjectInfo> list = new List<ObjectInfo>();
            RegisteredObjects.Objects.EnumItems(list);

            foreach (ObjectInfo info in list)
            {
                string aLocation = "";
                if (info.Object != null)
                    aLocation = info.Object.Assembly.Location;
                else if (info.Function != null)
                    aLocation = info.Function.DeclaringType.Assembly.Location;

                if (aLocation != "" && !ContansAssembly(assemblies, aLocation))
                    assemblies.Add(aLocation);
            }
        }

        private void AddReferencedAssemblies(IList assemblies, string defaultPath)
        {
            foreach (string s in Report.ReferencedAssemblies)
            {
#if NETSTANDARD
                // replace 
                if (s == "System.Windows.Forms.dll")
                {
                    AddReferencedAssembly(assemblies, defaultPath, "FastReport.Compat");
                    continue;
                }
#endif
                // fix for old reports with "System.Windows.Forms.DataVisualization" in referenced assemblies 
                if (s.IndexOf("System.Windows.Forms.DataVisualization") != -1)
                {
                    AddReferencedAssembly(assemblies, defaultPath, "FastReport.DataVisualization");
                    continue;
                }

                AddReferencedAssembly(assemblies, defaultPath, s);
            }
        }

        private void AddReferencedAssembly(IList assemblies, string defaultPath, string assemblyName)
        {
            string location = GetFullAssemblyReference(assemblyName, defaultPath);
            if (location != "" && !ContansAssembly(assemblies, location))
                assemblies.Add(location);
        }

        private string GetFullAssemblyReference(string relativeReference, string defaultPath)
        {
            if (relativeReference == null || relativeReference.Trim() == "")
                return "";

            // Strip off any trailing ".dll" ".exe" if present.
            string dllName = relativeReference;
            if (string.Compare(relativeReference.Substring(relativeReference.Length - 4), ".dll", true) == 0 ||
              string.Compare(relativeReference.Substring(relativeReference.Length - 4), ".exe", true) == 0)
                dllName = relativeReference.Substring(0, relativeReference.Length - 4);

            // See if the required assembly is already present in our current AppDomain
            foreach (Assembly currAssembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (string.Compare(currAssembly.GetName().Name, dllName, true) == 0)
                {
                    // Found it, return the location as the full reference.
                    return currAssembly.Location;
                }
            }

            // See if the required assembly is present in the ReferencedAssemblies but not yet loaded
            foreach (AssemblyName assemblyName in System.Reflection.Assembly.GetExecutingAssembly().GetReferencedAssemblies())
            {
                if (string.Compare(assemblyName.Name, dllName, true) == 0)
                {
                    // Found it, try to load assembly and return the location as the full reference.
                    try
                    {
                        return System.Reflection.Assembly.ReflectionOnlyLoad(assemblyName.FullName).Location;
                    }
                    catch { }
                }
            }

            // See if the required assembly is present locally
            string path = Path.Combine(defaultPath, relativeReference);
            if (File.Exists(path))
                return path;

            return relativeReference;
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

            // handle complex expressions, relations
            ExpressionDescriptor descriptor = new ExpressionDescriptor(this);
            Expressions.Add(expression, descriptor);
            descriptor.MethodName = "CalcExpression";

            if (DataHelper.IsValidColumn(Report.Dictionary, expr))
                expr = "[" + expr + "]";
            else
                expr = expression;
            string expressionCode = ReplaceDataItems(expr);
            InsertItem(Report.CodeHelper.AddExpression(expression, expressionCode), source == null ? "" : source.Name);
            needCompile = true;
        }

        public void AddObjects()
        {
            ObjectCollection allObjects = Report.AllObjects;
            SortedList<string, Base> objects = new SortedList<string, Base>();

            // add all report objects
            InsertItem(Report.CodeHelper.AddField(typeof(Report), "Report") +
              Report.CodeHelper.AddField(typeof(ReportEngine), "Engine"), "Report");
            foreach (Base c in allObjects)
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
                    needCompile = true;
                }
            }
        }

        public void AddSingleExpression(string expression)
        {
            InsertItem(Report.CodeHelper.BeginCalcExpression(), "");
            AddExpression(expression, null, true);
            InsertItem(Report.CodeHelper.EndCalcExpression(), "");
            needCompile = true;
        }

        public void AddExpressions()
        {
            InsertItem(Report.CodeHelper.BeginCalcExpression(), "");

            ObjectCollection allObjects = Report.AllObjects;
            ObjectCollection l = Report.Dictionary.AllObjects;
            foreach (Base c in l)
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
        }

        public void AddFunctions()
        {
            List<ObjectInfo> list = new List<ObjectInfo>();
            RegisteredObjects.Objects.EnumItems(list);

            foreach (ObjectInfo info in list)
            {
                if (info.Function != null)
                {
                    InsertItem(Report.CodeHelper.GetMethodSignatureAndBody(info.Function), "Function");
                }
            }
        }

        public string GenerateReportClass(string className)
        {
            InsertItem(Report.CodeHelper.GenerateInitializeMethod(), "");
            return Report.CodeHelper.ReplaceClassName(scriptText.ToString(), className);
        }

        public void Compile()
        {
            if (needCompile)
            {
                lock(compileLocker)
                {
                    if (needCompile)
                        InternalCompile();
                }
            }    
        }

        private void InternalCompile()
        {          
            // configure compiler options
            CompilerParameters cp = new CompilerParameters();
            AddFastReportAssemblies(cp.ReferencedAssemblies);
            AddReferencedAssemblies(cp.ReferencedAssemblies, currentFolder);
            ReviewReferencedAssemblies(cp.ReferencedAssemblies);
            cp.GenerateInMemory = true;
            // sometimes the system temp folder is not accessible...
            if (Config.TempFolder != null)
                cp.TempFiles = new TempFileCollection(Config.TempFolder, false);

            if (Config.WebMode &&
                Config.EnableScriptSecurity &&
                Config.ScriptSecurityProps.AddStubClasses)
                AddStubClasses();

            string errors = string.Empty;
            CompilerResults cr;
            bool exception = !InternalCompile(cp, out cr);
            for (int i = 0; exception && i < RECOMPILE_COUNT ; i++)
            {
                exception = !HandleCompileErrors(cp, cr, out errors);
            }

            if (exception)
                throw new CompilerException(errors);
        }
        
        private string GetAssemblyHash(CompilerParameters cp)
        {
            StringBuilder assemblyHashSB = new StringBuilder();
            foreach (string a in cp.ReferencedAssemblies)
                assemblyHashSB.Append(a);
            assemblyHashSB.Append(scriptText.ToString());
            byte[] hash = null;
            using (HMACSHA1 hMACSHA1 = new HMACSHA1(Encoding.ASCII.GetBytes(shaKey)))
            {
                hash = hMACSHA1.ComputeHash(Encoding.Unicode.GetBytes(assemblyHashSB.ToString()));
            }

            return Convert.ToBase64String(hash);
        }

        /// <summary>
        /// Returns true, if compilation is successful
        /// </summary>
        private bool InternalCompile(CompilerParameters cp, out CompilerResults cr)
        {

            // find assembly in cache
            string assemblyHash = GetAssemblyHash(cp);
            Assembly cachedAssembly = null;
            if (FAssemblyCache.TryGetValue(assemblyHash, out cachedAssembly))
            {
                Assembly = cachedAssembly;
                InitInstance(Assembly.CreateInstance("FastReport.ReportScript"));
                cr = null;
                return true;
            }

            // compile report scripts
            using (CodeDomProvider provider = Report.CodeHelper.GetCodeProvider())
            {
                ScriptSecurityEventArgs ssea = new ScriptSecurityEventArgs(Report, scriptText.ToString(), Report.ReferencedAssemblies);
                Config.OnScriptCompile(ssea);

                cr = provider.CompileAssemblyFromSource(cp, scriptText.ToString());
                Assembly = null;
                Instance = null;

                if (cr.Errors.Count != 0)   // Compile errors
                    return false;

                FAssemblyCache.TryAdd(assemblyHash, cr.CompiledAssembly);

                Assembly = cr.CompiledAssembly;
                InitInstance(Assembly.CreateInstance("FastReport.ReportScript"));
                return true;
            }
        }

        /// <summary>
        /// Handle compile errors
        /// </summary>
        /// <returns>Returns <"true"> if all errors were handled</returns>
        private bool HandleCompileErrors(CompilerParameters cp, CompilerResults cr, out string errors)
        {
            errors = string.Empty;
            List<string> additionalAssemblies = new List<string>(4);
            Regex regex;
            
            if (Config.WebMode && Config.EnableScriptSecurity)
            {
                for (int i=0; i < cr.Errors.Count; )
                {
                    CompilerError ce = cr.Errors[i];
                    if (ce.ErrorNumber == "CS1685") // duplicate class
                    {
                        cr.Errors.Remove(ce);
                        continue;
                    }
                    else if (ce.ErrorNumber == "CS0436") // user using a forbidden type 
                    {
                        const string pattern = "[\"'](\\S+)[\"']";
                        regex = new Regex(pattern, RegexOptions.Compiled);
                        string typeName = regex.Match(ce.ErrorText).Value;

                        const string res = "Web,ScriptSecurity,ForbiddenType";
                        string message = Res.TryGet(res);
                        if(string.Equals(res, message))
                            message = "Please, don't use the type " + typeName;
                        else
                            message = message.Replace("{typeName}", typeName); //$"Please, don't use the type {typeName}";

                        ce.ErrorText = message;
                        
                    }
                    else if (ce.ErrorNumber == "CS0117") // user using a forbidden method
                    {
                        const string pattern = "[\"'](\\S+)[\"']";
                        regex = new Regex(pattern, RegexOptions.Compiled);
                        MatchCollection mathes = regex.Matches(ce.ErrorText);
                        if(mathes.Count > 1)
                        {
                            string methodName = mathes[1].Value;

                            const string res = "Web,ScriptSecurity,ForbiddenMethod";
                            string message = Res.TryGet(res);
                            if (string.Equals(res, message))
                                message = "Please, don't use the method " + methodName;
                            else 
                                message = message.Replace("{methodName}", methodName); //$"Please, don't use the method {methodName}";

                            ce.ErrorText = message;
                        }
                    }

                    i++;
                }
            }

            foreach (CompilerError ce in cr.Errors)
            {
                if (ce.ErrorNumber == "CS0012") // missing reference on assembly
                {
                    // try to add reference
                    try
                    {
                        // in .Net Core compiler will return other quotes
#if NETSTANDARD || NETCOREAPP
                        const string quotes = "\'";
#else
                        const string quotes = "\"";
#endif
                        const string pattern = quotes + @"(\S{1,}),";
                        regex = new Regex(pattern, RegexOptions.Compiled);
                        string assemblyName = regex.Match(ce.ErrorText).Groups[1].Value;   // Groups[1] include string without quotes and , symbols
                        if (!additionalAssemblies.Contains(assemblyName))
                            additionalAssemblies.Add(assemblyName);
                        continue;
                    }
                    catch { }
                }

                int line = GetScriptLine(ce.Line);
                // error is inside own items
                if (line == -1)
                { 
                    string errObjName = GetErrorObjectName(ce.Line);

                    // handle division by zero errors
                    if (ce.ErrorNumber == "CS0020")
                    {
                        TextObjectBase text = Report.FindObject(errObjName) as TextObjectBase;
                        text.CanGrow = true;
                        text.FillColor = Color.Red;
                        text.Text = "DIVISION BY ZERO!";
                        if (cr.Errors.Count == 1) // there are only division by zero errors, exception does't needed
                            return true;
                    }
                    else
                    {
                        errors += String.Format("({0}): " + Res.Get("Messages,Error") + " {1}: {2}", new object[] { errObjName, ce.ErrorNumber, ce.ErrorText }) + "\r\n";
                        ErrorMsg(errObjName, ce);
                    }
                }
                else
                {
                    errors += String.Format("({0},{1}): " + Res.Get("Messages,Error") + " {2}: {3}", new object[] { line, ce.Column, ce.ErrorNumber, ce.ErrorText }) + "\r\n";
                    ErrorMsg(ce, line);
                }
            }

            if(additionalAssemblies.Count > 0)  // need recompile
                return ReCompile(cp, cr, additionalAssemblies);

            return false;
        }

        /// <summary>
        /// Returns true, if recompilation is successful
        /// </summary>
        private bool ReCompile(CompilerParameters cp, CompilerResults cr, IList additionalAssemblies)
        {
            // try to load missing assemblies
            foreach (string assemblyName in additionalAssemblies)
            {
                AddReferencedAssembly(cp.ReferencedAssemblies, currentFolder, assemblyName);
            }

            return InternalCompile(cp, out cr);
        }

        public void InitInstance(object instance)
        {
            this.Instance = instance;
            InitFields();
        }

        public bool ContainsExpression(string expr)
        {
            return Expressions.ContainsKey(expr);
        }

        public object CalcExpression(string expr, Variant value)
        {
            FastReport.Code.ExpressionDescriptor expressionDescriptor = Expressions[expr] as FastReport.Code.ExpressionDescriptor;
            if (expressionDescriptor != null)
                return expressionDescriptor.Invoke(new object[] { expr, value });
            else
                return null;
        }

        public void InvokeEvent(string name, object[] parms)
        {
            if (String.IsNullOrEmpty(name))
                return;

            string exprName = "event_" + name;
            if (!ContainsExpression(exprName))
            {
                ExpressionDescriptor descriptor = new ExpressionDescriptor(this);
                Expressions.Add(exprName, descriptor);
                descriptor.MethodName = name;
            }
            try
            {
                (Expressions[exprName] as ExpressionDescriptor).Invoke(parms);
            }
            catch (TargetInvocationException ex)
            {
                throw (ex.InnerException); // ex now stores the original exception
            }
        }

        public AssemblyDescriptor(Report report, string scriptText)
        {
            this.Report = report;
            this.scriptText = new FastString(scriptText);
            Expressions = new Hashtable();
            sourcePositions = new List<SourcePosition>();
            insertPos = Report.CodeHelper.GetPositionToInsertOwnItems(scriptText);
            if (insertPos == -1)
            {
                string msg = Res.Get("Messages,ClassError");
                ErrorMsg(msg);
                throw new CompilerException(msg);
            }
            else
            {
                string[] lines = scriptText.Substring(0, insertPos).Split(new char[] { '\r' });
                insertLine = lines.Length;
                if (scriptText != Report.CodeHelper.EmptyScript())
                    needCompile = true;
            }

            // set the current folder
            currentFolder = Config.ApplicationFolder;
            if (Config.WebMode)
            {
                try
                {
                    string bin_directory = Path.Combine(currentFolder, "Bin");
                    if (Directory.Exists(bin_directory))
                        currentFolder = bin_directory;
                }
                catch
                {
                }
            }
            // Commented by Samuray
            //Directory.SetCurrentDirectory(currentFolder);
        }

        static AssemblyDescriptor()
        {
            FAssemblyCache = new ConcurrentDictionary<string, Assembly>();

            compileLocker = new object();
        }

        private class SourcePosition
        {
            public string sourceObject;
            public int start;
            public int end;

            public SourcePosition(string obj, int start, int end)
            {
                sourceObject = obj;
                this.start = start;
                this.end = end;
            }
        }
    }
}
