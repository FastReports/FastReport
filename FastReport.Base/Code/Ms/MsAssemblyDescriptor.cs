using System;

#if NETSTANDARD || NETCOREAPP
using FastReport.Code.CodeDom.Compiler;
using FastReport.Code.CSharp;
using FastReport.Code.VisualBasic;
#else
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using Microsoft.VisualBasic;
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
using FastReport.Utils;
#if SKIA
using HMACSHA1 = FastReport.Utils.DetravHMACSHA1;
#endif

namespace FastReport.Code.Ms
{
    partial class MsAssemblyDescriptor : AssemblyDescriptor
    {
        private static readonly ConcurrentDictionary<string, Assembly> _assemblyCache;
        private const string shaKey = "FastReportCode";
        private readonly static object _compileLocker;
        private readonly string _currentFolder;

        public Assembly Assembly { get; private set; }

        protected override ExpressionDescriptor CreateExpressionDescriptor(string methodName)
        {
            return new MsExpressionDescriptor(this, methodName);
        }

        protected override void InitField(string name, object c)
        {
            FieldInfo info = Instance.GetType().GetField(name,
              BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            info.SetValue(Instance, c);
        }

        private static bool ContainsAssembly(StringCollection assemblies, string assembly)
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

        private static void AddFastReportAssemblies(StringCollection assemblies)
        {
            foreach (Assembly assembly in RegisteredObjects.Assemblies)
            {
                string aLocation = assembly.Location;
#if CROSSPLATFORM || COREWIN
                if (string.IsNullOrEmpty(aLocation))
                {
                    // try fix SFA in FastReport.Compat
                    string fixedReference = CodeDomProvider.TryFixAssemblyReference(assembly);
                    if (!string.IsNullOrEmpty(fixedReference))
                        aLocation = fixedReference;
                }
#endif
                if (!ContainsAssembly(assemblies, aLocation))
                    assemblies.Add(aLocation);
            }
        }

        private void AddReferencedAssemblies(StringCollection assemblies, string defaultPath)
        {
            for (int i = 0; i < Report.ReferencedAssemblies.Length; i++)
            {
                string s = Report.ReferencedAssemblies[i];

#if CROSSPLATFORM
                if (s == "System.Windows.Forms.dll")
                    s = "FastReport.Compat";
#endif
                // fix for old reports with "System.Windows.Forms.DataVisualization" in referenced assemblies 
                if (s.IndexOf("System.Windows.Forms.DataVisualization") != -1)
                    s = "FastReport.DataVisualization";
#if (SKIA && !AVALONIA)
                if (s.IndexOf("FastReport.Compat") != -1)
                    s = "FastReport.Compat.Skia";
                if (s.IndexOf("FastReport.DataVisualization") != -1)
                    s = "FastReport.DataVisualization.Skia";
#endif

                AddReferencedAssembly(assemblies, defaultPath, s);
            }

#if SKIA
            AddReferencedAssembly(assemblies, defaultPath, "FastReport.SkiaDrawing");
#endif

            // these two required for "dynamic" type support
            AddReferencedAssembly(assemblies, defaultPath, "System.Core");
            AddReferencedAssembly(assemblies, defaultPath, "Microsoft.CSharp");
        }

        private void AddReferencedAssembly(StringCollection assemblies, string defaultPath, string assemblyName)
        {
            string location = GetFullAssemblyReference(assemblyName, defaultPath);
            if (location != "" && !ContainsAssembly(assemblies, location))
                assemblies.Add(location);
        }

        private string GetFullAssemblyReference(string relativeReference, string defaultPath)
        {
            // in .NET Core we get the AssemblyReference in FR.Compat
#if !(CROSSPLATFORM || COREWIN)
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
            foreach (AssemblyName assemblyName in Assembly.GetExecutingAssembly().GetReferencedAssemblies())
            {
                if (string.Compare(assemblyName.Name, dllName, true) == 0)
                {
                    // Found it, try to load assembly and return the location as the full reference.
                    try
                    {
                        return Assembly.ReflectionOnlyLoad(assemblyName.FullName).Location;
                    }
                    catch { }
                }
            }

            // See if the required assembly is present locally
            string path = Path.Combine(defaultPath, relativeReference);
            if (File.Exists(path))
                return path;

            path = Path.Combine(defaultPath, relativeReference + ".dll");
            if (File.Exists(path))
                return path;

            path = Path.Combine(defaultPath, relativeReference + ".exe");
            if (File.Exists(path))
                return path;
#endif
            return relativeReference;
        }

        public override void Compile()
        {
            if (NeedCompile)
            {
                lock (_compileLocker)
                {
                    if (NeedCompile)
                        InternalCompile();
                }
            }
        }

        private void InternalCompile()
        {
            CompilerParameters cp = GetCompilerParameters();

            if (Config.WebMode &&
                Config.EnableScriptSecurity &&
                Config.ScriptSecurityProps.AddStubClasses)
                AddStubClasses();

            CompilerResults cr;
            bool exception = !TryInternalCompile(cp, out cr);
            for (int i = 0; exception && i < Config.CompilerSettings.RecompileCount; i++)
            {
                exception = !TryRecompile(cp, ref cr);
            }

            if (cr != null)
            {
                var ex = HandleCompileErrors(cr);

                if (exception && ex != null)
                    throw ex;
            }
        }

        private CompilerParameters GetCompilerParameters()
        {
            // configure compiler options
            CompilerParameters cp = new CompilerParameters();
            AddFastReportAssemblies(cp.ReferencedAssemblies);   // 2
            AddReferencedAssemblies(cp.ReferencedAssemblies, _currentFolder);    // 9
            ReviewReferencedAssemblies(cp.ReferencedAssemblies);
            cp.GenerateInMemory = true;
            // sometimes the system temp folder is not accessible...
            if (Config.TempFolder != null)
                cp.TempFiles = new TempFileCollection(Config.TempFolder, false);
            return cp;
        }

        private string GetAssemblyHash(CompilerParameters cp)
        {
            var assemblyHashSB = new StringBuilder();

            foreach (string a in cp.ReferencedAssemblies)
            {
                assemblyHashSB.Append(a);
            }

            assemblyHashSB.Append(ScriptText);
            byte[] hash;

            using (HMACSHA1 hMACSHA1 = new HMACSHA1(Encoding.ASCII.GetBytes(shaKey)))
            {
                hash = hMACSHA1.ComputeHash(Encoding.Unicode.GetBytes(assemblyHashSB.ToString()));
            }

            return Convert.ToBase64String(hash);
        }

        private CodeDomProvider GetCodeProvider()
        {
            return Report.ScriptLanguage == Language.CSharp ?
                new CSharpCodeProvider() : new VBCodeProvider();
        }

        /// <summary>
        /// Returns true, if compilation is successful
        /// </summary>
        private bool TryInternalCompile(CompilerParameters cp, out CompilerResults cr)
        {
            // find assembly in cache
            string assemblyHash = GetAssemblyHash(cp);
            Assembly cachedAssembly;
            if (_assemblyCache.TryGetValue(assemblyHash, out cachedAssembly))
            {
                Assembly = cachedAssembly;
                var reportScript = Assembly.CreateInstance("FastReport.ReportScript");
                InitInstance(reportScript);
                cr = null;
                return true;
            }

            // compile report scripts
            using (var provider = GetCodeProvider())
            {
                string script = ScriptText.ToString();
                ScriptSecurityEventArgs ssea = new ScriptSecurityEventArgs(Report, script, Report.ReferencedAssemblies);
                Config.OnScriptCompile(ssea);

#if CROSSPLATFORM || COREWIN
                provider.BeforeEmitCompilation += Config.OnBeforeScriptCompilation;

                // in .NET Core we use cultureInfo to represent errors
                cr = provider.CompileAssemblyFromSource(cp, script, Config.CompilerSettings.CultureInfo);
#else
                cr = provider.CompileAssemblyFromSource(cp, script);
#endif
                Assembly = null;
                Instance = null;

                if (cr.Errors.Count != 0)   // Compile errors
                    return false;

                _assemblyCache.TryAdd(assemblyHash, cr.CompiledAssembly);

                Assembly = cr.CompiledAssembly;
                var reportScript = Assembly.CreateInstance("FastReport.ReportScript");
                InitInstance(reportScript);
                return true;
            }
        }

        private CompilerException HandleCompileErrors(CompilerResults cr)
        {
            Regex regex;

            if (Config.WebMode && Config.EnableScriptSecurity)
            {
                for (int i = 0; i < cr.Errors.Count;)
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
                        if (string.Equals(res, message))
                            message = "Please don't use the type " + typeName;
                        else
                            message = message.Replace("{typeName}", typeName); //$"Please don't use the type {typeName}";

                        ce.ErrorText = message;

                    }
                    else if (ce.ErrorNumber == "CS0117") // user using a forbidden method
                    {
                        const string pattern = "[\"'](\\S+)[\"']";
                        regex = new Regex(pattern, RegexOptions.Compiled);
                        MatchCollection mathes = regex.Matches(ce.ErrorText);
                        if (mathes.Count > 1)
                        {
                            string methodName = mathes[1].Value;

                            const string res = "Web,ScriptSecurity,ForbiddenMethod";
                            string message = Res.TryGet(res);
                            if (string.Equals(res, message))
                                message = "Please don't use the method " + methodName;
                            else
                                message = message.Replace("{methodName}", methodName); //$"Please don't use the method {methodName}";

                            ce.ErrorText = message;
                        }
                    }

                    i++;
                }
            }

            var errors = new List<CompilerException.Info>();
            var errorMsg = "";
            
            foreach (CompilerError ce in cr.Errors)
            {
                int line = GetScriptLine(ce.Line);
                // error is inside own items
                if (line == -1)
                {
                    string errObjName = GetErrorObjectName(ce.Line);

                    if (Config.CompilerSettings.ExceptionBehaviour != CompilerExceptionBehaviour.Default)
                    {
                        // handle errors when name does not exist in the current context
                        if (ce.ErrorNumber == "CS0103")
                        {
                            TextObjectBase text = Report.FindObject(errObjName) as TextObjectBase;
                            text.Text = ReplaceExpression(ce.ErrorText, text);
                            if (Config.CompilerSettings.ExceptionBehaviour == CompilerExceptionBehaviour.ShowExceptionMessage)
                                System.Windows.Forms.MessageBox.Show(ce.ErrorText);
                            continue;
                        }
                    }

                    // handle division by zero errors
                    if (ce.ErrorNumber == "CS0020")
                    {
                        TextObjectBase text = Report.FindObject(errObjName) as TextObjectBase;
                        text.CanGrow = true;
                        text.FillColor = Color.Red;
                        text.Text = "DIVISION BY ZERO!";
                        continue;
                    }
                    else
                    {
                        var msg = $"({errObjName}): {Res.Get("Messages,Error")} {ce.ErrorNumber}: {ce.ErrorText}";
                        errors.Add(new CompilerException.Info(0, 0, errObjName, msg));
                        errorMsg += msg + "\r\n";
                    }
                }
                else
                {
                    var msg = $"{Res.Get("Messages,Error")} {ce.ErrorNumber}: {ce.ErrorText}";
                    errors.Add(new CompilerException.Info(line, ce.Column, null, msg));
                    errorMsg += $"({line},{ce.Column}): {msg}\r\n";
                }
            }

            if (errors.Count > 0) 
            { 
                return new CompilerException(errorMsg, errors.ToArray());
            }

            return null;
        }

        /// <summary>
        /// Returns true if recompilation is successful
        /// </summary>
        private bool TryRecompile(CompilerParameters cp, ref CompilerResults cr)
        {
            List<string> additionalAssemblies = new List<string>(4);

            foreach (CompilerError ce in cr.Errors)
            {
                if (ce.ErrorNumber == "CS0012") // missing reference on assembly
                {
                    // try to add reference
                    try
                    {
                        // in .Net Core compiler will return other quotes
#if CROSSPLATFORM || COREWIN
                        const string quotes = "\'";
#else
                        const string quotes = "\"";
#endif
                        const string pattern = quotes + @"(\S{1,}),";
                        Regex regex = new Regex(pattern, RegexOptions.Compiled);
                        string assemblyName = regex.Match(ce.ErrorText).Groups[1].Value;   // Groups[1] include string without quotes and , symbols
                        if (!additionalAssemblies.Contains(assemblyName))
                            additionalAssemblies.Add(assemblyName);
                        continue;
                    }
                    catch { }
                }
            }

            if (additionalAssemblies.Count > 0)  // need recompile
            {
                // try to load missing assemblies
                foreach (string assemblyName in additionalAssemblies)
                {
                    AddReferencedAssembly(cp.ReferencedAssemblies, _currentFolder, assemblyName);
                }

                return TryInternalCompile(cp, out cr);
            }

            return false;
        }

        public MsAssemblyDescriptor(Report report, string scriptText) : base(report, scriptText)
        {
            // set the current folder
            _currentFolder = Config.ApplicationFolder;
            if (Config.WebMode)
            {
                try
                {
                    string bin_directory = Path.Combine(_currentFolder, "Bin");
                    if (Directory.Exists(bin_directory))
                        _currentFolder = bin_directory;
                }
                catch
                {
                }
            }
        }

        static MsAssemblyDescriptor()
        {
            _assemblyCache = new ConcurrentDictionary<string, Assembly>();
            _compileLocker = new object();
        }
    }
}
