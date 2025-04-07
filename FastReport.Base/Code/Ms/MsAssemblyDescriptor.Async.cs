using System;
#if NETSTANDARD || NETCOREAPP
using FastReport.Code.CodeDom.Compiler;
#else
using System.CodeDom.Compiler;
#endif
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using FastReport.Utils;


namespace FastReport.Code.Ms
{
    partial class MsAssemblyDescriptor : IDisposable
    {
        private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1);


        private static async Task AddFastReportAssemblies(StringCollection assemblies, CancellationToken token)
        {
            foreach (Assembly assembly in RegisteredObjects.Assemblies)
            {
                string aLocation = assembly.Location;
#if CROSSPLATFORM || COREWIN
                if (string.IsNullOrEmpty(aLocation))
                {
                    // try fix SFA in FastReport.Compat
                    string fixedReference = await CodeDomProvider.TryFixAssemblyReferenceAsync(assembly, token);
                    if (!string.IsNullOrEmpty(fixedReference))
                        aLocation = fixedReference;
                }
#endif
                if (!ContainsAssembly(assemblies, aLocation))
                    assemblies.Add(aLocation);
            }
        }

        public override async Task CompileAsync(CancellationToken token = default)
        {
            if (NeedCompile)
            {
                await semaphoreSlim.WaitAsync(token);

                if (NeedCompile)
                {
                    try
                    {
                        await InternalCompileAsync(token);
                    }
                    finally
                    {
                        semaphoreSlim.Release();
                    }
                }
            }
        }


        private async Task InternalCompileAsync(CancellationToken cancellationToken)
        {
            CompilerParameters cp = await GetCompilerParametersAsync(cancellationToken);

            if (Config.WebMode &&
                Config.EnableScriptSecurity &&
                Config.ScriptSecurityProps.AddStubClasses)
                AddStubClasses();

            CompilerResults cr = await InternalCompileAsync(cp, cancellationToken);
            bool success = CheckCompileResult(cr);
            for (int i = 0; !success && i < Config.CompilerSettings.RecompileCount; i++)
            {
                cr = await TryRecompileAsync(cp, cr, cancellationToken);
                success = CheckCompileResult(cr);
            }

            if (cr != null)
            {
                var ex = HandleCompileErrors(cr);

                if (!success && ex != null)
                    throw ex;
            }
        }

        private static bool CheckCompileResult(CompilerResults result)
        {
            // if result == null => was found in cache
            return result == null || result.Errors.Count == 0;
        }


        private async Task<CompilerParameters> GetCompilerParametersAsync(CancellationToken ct)
        {
            // configure compiler options
            CompilerParameters cp = new CompilerParameters();
            await AddFastReportAssemblies(cp.ReferencedAssemblies, ct);   // 2
            AddReferencedAssemblies(cp.ReferencedAssemblies, _currentFolder);    // 9
            ReviewReferencedAssemblies(cp.ReferencedAssemblies);
            cp.GenerateInMemory = true;
            // sometimes the system temp folder is not accessible...
            if (Config.TempFolder != null)
                cp.TempFiles = new TempFileCollection(Config.TempFolder, false);
            return cp;
        }

        /// <summary>
        /// Returns true, if compilation is successful
        /// </summary>
        private async Task<CompilerResults> InternalCompileAsync(CompilerParameters cp, CancellationToken cancellationToken)
        {
            CompilerResults cr;
            // find assembly in cache
            string assemblyHash = GetAssemblyHash(cp);
            Assembly cachedAssembly;
            if (_assemblyCache.TryGetValue(assemblyHash, out cachedAssembly))
            {
                Assembly = cachedAssembly;
                var reportScript = Assembly.CreateInstance("FastReport.ReportScript");
                InitInstance(reportScript);
                cr = null;
                return cr;    // return true;
            }

            // compile report scripts
            using (var provider = GetCodeProvider())
            {
                string script = ScriptText.ToString();
                ScriptSecurityEventArgs ssea = new ScriptSecurityEventArgs(Report, script, Report.ReferencedAssemblies);
                Config.OnScriptCompile(ssea);

#if CROSSPLATFORM || COREWIN
                provider.BeforeEmitCompilation += Config.OnBeforeScriptCompilation;

                cr = await provider.CompileAssemblyFromSourceAsync(cp, script, Config.CompilerSettings.CultureInfo, cancellationToken);
#else
                cr = provider.CompileAssemblyFromSource(cp, script);
#endif
                Assembly = null;
                Instance = null;

                if (cr.Errors.Count != 0)   // Compile errors
                    return cr;  // return false;

                _assemblyCache.TryAdd(assemblyHash, cr.CompiledAssembly);

                Assembly = cr.CompiledAssembly;
                var reportScript = Assembly.CreateInstance("FastReport.ReportScript");
                InitInstance(reportScript);
                return cr;
            }
        }


        /// <summary>
        /// Returns true if recompilation is successful
        /// </summary>
        private async Task<CompilerResults> TryRecompileAsync(CompilerParameters cp, CompilerResults oldResult, CancellationToken ct)
        {
            List<string> additionalAssemblies = new List<string>(4);

            foreach (CompilerError ce in oldResult.Errors)
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

                return await InternalCompileAsync(cp, ct);
            }

            return oldResult;
        }

        public override void Dispose()
        {
            semaphoreSlim.Dispose();
        }
    }
}
