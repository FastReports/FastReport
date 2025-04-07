#if NETSTANDARD2_0 || NETSTANDARD2_1 || NETCOREAPP
using Microsoft.CodeAnalysis;
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Linq;
using System.Reflection.Metadata;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Emit;
using System.Globalization;
using System.Collections.Concurrent;
#if NETCOREAPP
using System.Runtime.Loader;
#endif

namespace FastReport.Code.CodeDom.Compiler
{
    public abstract class CodeDomProvider : IDisposable
    {
        private static readonly ConcurrentDictionary<string, MetadataReference> cache = new ConcurrentDictionary<string, MetadataReference>();


        /// <summary>
        /// Throws before compilation emit
        /// </summary>
        public event EventHandler<CompilationEventArgs> BeforeEmitCompilation;

        /// <summary>
        /// Manual resolve MetadataReference
        /// </summary>
        [Obsolete("Use AssemblyLoadResolver")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Func<AssemblyName, MetadataReference> ResolveMetadataReference { get; set; }

        /// <summary>
        /// Manual resolve MetadataReference
        /// </summary>
        public static IAssemblyLoadResolver AssemblyLoadResolver { get; set; }

        /// <summary>
        /// For developers only
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static event EventHandler<string> Log;

#if NETCOREAPP
        /// <summary>
        /// If these assemblies were not found when 'trimmed', then skip them
        /// </summary>
        private static readonly HashSet<string> SkippedAssemblies = new HashSet<string> {
                    "System",

                    "System.Core",

                    "System.Drawing",

                    //"System.Drawing.Primitives",

                    "System.Data",

                    "System.Xml",

                    "System.Private.CoreLib",
                };
#endif

        private static readonly string[] _additionalAssemblies = new[] {
                "mscorlib",
                "netstandard",
                "System.Core",
                "System.Collections.Concurrent",
                "System.Collections",
                "System.Collections.NonGeneric",
                "System.Collections.Specialized",
                "System.ComponentModel",
                "System.ComponentModel.Primitives",
                "System.Data.Common",
#if !SKIA
                "System.Drawing.Common",
#endif
                "System.Globalization",
                "System.IO",
                "System.Linq",
                "System.Linq.Expressions",
                "System.Linq.Parallel",
                "System.Linq.Queryable",
                "System.Numerics",
                "System.Runtime",
                "System.Text.Encoding",
                "System.Text.RegularExpressions"
            };

        [Conditional("DEBUG")]  // Comment for use log messages in Release configuration
        protected static void DebugMessage(string message)
        {
            Debug.WriteLine(message);
            Console.WriteLine(message);
            
            Log?.Invoke(null, message);
        }

        protected void AddReferences(CompilerParameters cp, List<MetadataReference> references)
        {
            foreach (string reference in cp.ReferencedAssemblies)
            {
                DebugMessage($"TRY ADD '{reference}'");
#if NETCOREAPP
                try
                {
#endif
                    var metadata = GetReference(reference);
                    references.Add(metadata);
#if NETCOREAPP
                }
                catch (FileNotFoundException)
                {
                    DebugMessage($"{reference} FileNotFound");

                    string assemblyName = GetCorrectAssemblyName(reference);
                    if (SkippedAssemblies.Contains(assemblyName))
                    {
                        DebugMessage($"{reference} FileNotFound. SKIPPED");
                        continue;
                    }
                    else
                    {
                        throw;
                    }
                }
#endif

                DebugMessage($"{reference} ADDED");
            }
            DebugMessage("AFTER ADDING ReferencedAssemblies");

            AddExtraAssemblies(cp.ReferencedAssemblies, references);
        }

        protected async ValueTask AddReferencesAsync(CompilerParameters cp, List<MetadataReference> references, CancellationToken cancellationToken)
        {
            foreach (string reference in cp.ReferencedAssemblies)
            {
                DebugMessage($"TRY ADD '{reference}'");
#if NETCOREAPP
                try
                {
#endif
                var metadata = await GetReferenceAsync(reference, cancellationToken);
                references.Add(metadata);
#if NETCOREAPP
                }
                catch (FileNotFoundException)
                {
                    DebugMessage($"{reference} FileNotFound");

                    string assemblyName = GetCorrectAssemblyName(reference);
                    if (SkippedAssemblies.Contains(assemblyName))
                    {
                        DebugMessage($"{reference} FileNotFound. SKIPPED");
                        continue;
                    }
                    else
                    {
                        throw;
                    }
                }
#endif

                DebugMessage($"{reference} ADDED");
            }
            DebugMessage("AFTER ADDING ReferencedAssemblies");

            await AddExtraAssembliesAsync(cp.ReferencedAssemblies, references, cancellationToken);
        }


        protected void AddExtraAssemblies(StringCollection referencedAssemblies, List<MetadataReference> references)
        {
            DebugMessage("Add Extra Assemblies...");
            foreach(string assembly in _additionalAssemblies)
            {
                if (!referencedAssemblies.Contains(assembly))
                {
#if NETCOREAPP
                    try
                    {
#endif
                        var metadata = GetReference(assembly);
                        references.Add(metadata);
#if NETCOREAPP
                    }
                    // If user run 'dotnet publish' with Trimmed - dotnet cut some extra assemblies.
                    // We skip this error, because some assemblies in 'assemblies' array may not be needed
                    catch (FileNotFoundException)
                    {
                        DebugMessage($"{assembly} FILENOTFOUND. SKIPPED");
                        continue;
                    }
#endif
                }
            }
        }

        protected async ValueTask AddExtraAssembliesAsync(StringCollection referencedAssemblies, List<MetadataReference> references, CancellationToken ct)
        {
            DebugMessage("Add Extra Assemblies...");
            foreach (string assembly in _additionalAssemblies)
            {
                if (!referencedAssemblies.Contains(assembly))
                {
#if NETCOREAPP
                    try
                    {
#endif
                        var metadata = await GetReferenceAsync(assembly, ct);
                        references.Add(metadata);
#if NETCOREAPP
                    }
                    // If user run 'dotnet publish' with Trimmed - dotnet cut some extra assemblies.
                    // We skip this error, because some assemblies in 'assemblies' array may not be needed
                    catch (FileNotFoundException)
                    {
                        DebugMessage($"{assembly} FILENOTFOUND. SKIPPED");
                        continue;
                    }
#endif
                }
            }
        }


        private void OnBeforeEmitCompilation(Compilation compilation)
        {
            if (BeforeEmitCompilation != null)
            {
                var eventArgs = new CompilationEventArgs(compilation);
                BeforeEmitCompilation(this, eventArgs);
            }
        }

        public MetadataReference GetReference(string refDll)
        {
            if (cache.TryGetValue(refDll, out var metadataReference))
                return metadataReference;

            MetadataReference result;
            string reference = GetCorrectAssemblyName(refDll);

            try
            {
                if (!refDll.Contains(Path.DirectorySeparatorChar))
                {
#if NETCOREAPP
                    // try find in AssemblyLoadContext
                    foreach (AssemblyLoadContext assemblyLoadContext in AssemblyLoadContext.All)
                    {
                        foreach (Assembly loadedAssembly in assemblyLoadContext.Assemblies)
                        {
                            if (loadedAssembly.GetName().Name == reference)
                            {
                                DebugMessage($"FIND {reference} IN AssemblyLoadContext");

                                result = ProcessAssembly(loadedAssembly);

                                AddToCache(refDll, result);
                                return result;
                            }
                        }
                    }
#else
                    foreach (Assembly currAssembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        if (string.Compare(currAssembly.GetName().Name, reference, true) == 0)
                        {
                            DebugMessage("FIND IN AppDomain");

                            // Found it, return the location as the full reference.
                            result = ProcessAssembly(currAssembly);
                            AddToCache(refDll, result);
                            return result;
                        }
                    }
#endif
                    // try find in ReferencedAssemblies
                    foreach (AssemblyName name in Assembly.GetExecutingAssembly().GetReferencedAssemblies())
                    {
                        if (name.Name == reference)
                        {
                            DebugMessage($"FIND {reference} IN ReferencedAssemblies");
#if NETCOREAPP
                            // try load Assembly in runtime (for user script with custom assembly)
                            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(name);
#else
                            var assembly = Assembly.Load(name);
#endif
                            result = ProcessAssembly(assembly);

                            AddToCache(refDll, result);
                            return result;
                        }
                    }
                }
                

                result = MetadataReference.CreateFromFile(refDll);
#if NETCOREAPP
                try
                {
                    // try load Assembly in runtime (for user script with custom assembly)
                    AssemblyLoadContext.Default.LoadFromAssemblyPath(refDll);
                }
                catch(ArgumentException) {
                    var fullpath = Path.Combine(Environment.CurrentDirectory, refDll);
                    try
                    {
                        AssemblyLoadContext.Default.LoadFromAssemblyPath(fullpath);
                    }
                    catch { }
                }
                catch { }
#endif
                AddToCache(refDll, result);

                return result;
            }
            catch
            {
                DebugMessage("IN AssemblyName");
                var assemblyName = new AssemblyName(reference);

                result = UserResolveMetadataReference(assemblyName);
                if(result != null)
                {
                    DebugMessage($"MetadataReference for assembly {reference} resolved by user");
                    AddToCache(refDll, result);
                    return result;
                }

#if NETCOREAPP
                // try load Assembly in runtime (for user script with custom assembly)
                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(assemblyName);
#else
                var assembly = Assembly.Load(assemblyName);
#endif
                DebugMessage("After LoadFromAssemblyName");

                result = ProcessAssembly(assembly);

                AddToCache(refDll, result);
                return result;
            }
        }

        private static async ValueTask<MetadataReference> GetReferenceAsync(string refDll, CancellationToken cancellationToken)
        {
            DebugMessage($"GetReferenceAsync: {refDll}");

            if (cache.TryGetValue(refDll, out var metadataReference))
                return metadataReference;

            MetadataReference result;
            string reference = GetCorrectAssemblyName(refDll);

            try
            {
                if (!refDll.Contains(Path.DirectorySeparatorChar))
                {
#if NETCOREAPP
                    // try find in AssemblyLoadContext
                    foreach (AssemblyLoadContext assemblyLoadContext in AssemblyLoadContext.All)
                    {
                        foreach (Assembly loadedAssembly in assemblyLoadContext.Assemblies)
                        {
                            if (loadedAssembly.GetName().Name == reference)
                            {
                                DebugMessage($"FIND {reference} IN AssemblyLoadContext");

                                result = await ProcessAssemblyAsync(loadedAssembly, cancellationToken);

                                AddToCache(refDll, result);
                                return result;
                            }
                        }
                    }
#else
                    foreach (Assembly currAssembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        if (string.Compare(currAssembly.GetName().Name, reference, true) == 0)
                        {
                            DebugMessage("FIND IN AppDomain");

                            // Found it, return the location as the full reference.
                            result = await ProcessAssemblyAsync(currAssembly, cancellationToken);
                            AddToCache(refDll, result);
                            return result;
                        }
                    }
#endif
                    // try find in ReferencedAssemblies
                    foreach (AssemblyName name in Assembly.GetExecutingAssembly().GetReferencedAssemblies())
                    {
                        if (name.Name == reference)
                        {
                            DebugMessage($"FIND {reference} IN ReferencedAssemblies");
#if NETCOREAPP
                            // try load Assembly in runtime (for user script with custom assembly)
                            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(name);
#else
                            var assembly = Assembly.Load(name);
#endif
                            result = await ProcessAssemblyAsync(assembly, cancellationToken);

                            AddToCache(refDll, result);
                            return result;
                        }
                    }
                }


                result = MetadataReference.CreateFromFile(refDll);
#if NETCOREAPP
                try
                {
                    // try load Assembly in runtime (for user script with custom assembly)
                    AssemblyLoadContext.Default.LoadFromAssemblyPath(refDll);
                }
                catch(ArgumentException) {
                    var fullpath = Path.Combine(Environment.CurrentDirectory, refDll);
                    try
                    {
                        AssemblyLoadContext.Default.LoadFromAssemblyPath(fullpath);
                    }
                    catch { }
                }
                catch { }
#endif
                AddToCache(refDll, result);

                return result;
            }
            catch
            {
                DebugMessage("IN AssemblyName");
                var assemblyName = new AssemblyName(reference);

                result = await UserResolveMetadataReferenceAsync(assemblyName, cancellationToken);
                if (result != null)
                {
                    DebugMessage($"MetadataReference for assembly {reference} resolved by user");
                    AddToCache(refDll, result);
                    return result;
                }

#if NETCOREAPP
                // try load Assembly in runtime (for user script with custom assembly)
                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(assemblyName);
#else
                var assembly = Assembly.Load(assemblyName);
#endif
                DebugMessage("After LoadFromAssemblyName");

                result = await ProcessAssemblyAsync(assembly, cancellationToken);

                AddToCache(refDll, result);
                return result;
            }
        }


        private static MetadataReference UserResolveMetadataReference(AssemblyName assembly)
        {
            if(AssemblyLoadResolver != null)
            {
                return AssemblyLoadResolver.LoadManagedLibrary(assembly);
            }

            return ResolveMetadataReference?.Invoke(assembly);
        }

        private static async ValueTask<MetadataReference> UserResolveMetadataReferenceAsync(AssemblyName assemblyName, CancellationToken ct)
        {
            if (AssemblyLoadResolver != null)
            {
                return await AssemblyLoadResolver.LoadManagedLibraryAsync(assemblyName, ct);
            }

            return null;
        }

        private static MetadataReference ProcessAssembly(Assembly assembly)
        {
            MetadataReference result;
            DebugMessage($"Location: {assembly.Location}");

#if NETCOREAPP
            // In SFA location is empty
            // In WASM location is empty
            // In Android DEBUG location is correct (not empty)
            // In Android RELEASE (AOT) location is not empty but incorrect
            if (SpecialCondition(assembly))
            {
                DebugMessage("SpecialCondition is true");
                result = GetMetadataReferenceSpecialized(assembly);
                return result;
            }
#endif
            result = MetadataReference.CreateFromFile(assembly.Location);

            return result;
        }

        private static async ValueTask<MetadataReference> ProcessAssemblyAsync(Assembly assembly, CancellationToken ct)
        {
            MetadataReference result;
            DebugMessage($"Location: {assembly.Location}");


#if NETCOREAPP
            // In SFA location is empty
            // In WASM location is empty
            // In Android DEBUG location is correct (not empty)
            // In Android RELEASE (AOT) location is not empty but incorrect
            if (SpecialCondition(assembly))
            {
                DebugMessage("SpecialCondition is true");
                result = await GetMetadataReferenceSpecializedAsync(assembly, ct);
                return result;
            }
#endif
            result = MetadataReference.CreateFromFile(assembly.Location);

            return result;
        }


#if NETCOREAPP

        private static bool SpecialCondition(Assembly assembly)
        {
            string location = assembly.Location;

            DebugMessage($"assemblyName Name {assembly.GetName().Name}");

            bool result = string.IsNullOrEmpty(location)
#if NET6_0_OR_GREATER   // ANDROID_BUILD || IOS_BUILD
                || location.StartsWith(assembly.GetName().Name)
#endif
                ;
            return result;
        }


        private static MetadataReference GetMetadataReferenceSpecialized(Assembly assembly)
        {
            MetadataReference result;
            try
            {
                result = GetMetadataReferenceInSingleFileApp(assembly);
            }
            catch (NotImplementedException)
            {
                DebugMessage("Not implemented assembly load from SFA");
                // try load from external source
                result = UserResolveMetadataReference(assembly.GetName());

                if(result == null)
                    throw;
            }
            return result;
        }

        private static async ValueTask<MetadataReference> GetMetadataReferenceSpecializedAsync(Assembly assembly, CancellationToken ct)
        {
            MetadataReference result;
            try
            {
                result = GetMetadataReferenceInSingleFileApp(assembly);
            }
            catch
            {
                DebugMessage("Not implemented assembly load from SFA");
                // try load from external source
                result = await UserResolveMetadataReferenceAsync(assembly.GetName(), ct);

                if (result == null)
                    throw;
            }
            return result;
        }


        private static unsafe MetadataReference GetMetadataReferenceInSingleFileApp(Assembly assembly)
        {
            DebugMessage($"TRY IN UNSAFE METHOD {assembly.GetName().Name}");
            assembly.TryGetRawMetadata(out byte* blob, out int length);
            var moduleMetadata = ModuleMetadata.CreateFromMetadata((IntPtr)blob, length);
            var assemblyMetadata = AssemblyMetadata.Create(moduleMetadata);
            return assemblyMetadata.GetReference();
        }

#endif

        public static string TryFixAssemblyReference(Assembly assembly)
        {
            string assemblyName = null;
#if NETCOREAPP
            try
            {
                assemblyName = assembly.GetName().Name;
                if (!cache.ContainsKey(assemblyName))
                {
                    MetadataReference metadataReference = GetMetadataReferenceSpecialized(assembly);
                    AddToCache(assemblyName, metadataReference);
                }
                return assemblyName;
            }
            catch (Exception ex)
            {
                DebugMessage(ex.ToString());
            }

            // sometimes we don't know the assembly's location, but it's not a SFA
            // in this case just return the assemblyName to handle it during compilation
#endif
            return assemblyName;
        }

        public static async ValueTask<string> TryFixAssemblyReferenceAsync(Assembly assembly, CancellationToken ct)
        {
            string assemblyName = null;
#if NETCOREAPP
            try
            {
                assemblyName = assembly.GetName().Name;
                if (!cache.ContainsKey(assemblyName))
                {
                    MetadataReference metadataReference = await GetMetadataReferenceSpecializedAsync(assembly, ct);
                    AddToCache(assemblyName, metadataReference);
                }
                return assemblyName;
            }
            catch (Exception ex)
            {
                DebugMessage(ex.ToString());
            }

            // sometimes we don't know the assembly's location, but it's not a SFA
            // in this case just return the assemblyName to handle it during compilation
#endif
            return assemblyName;
        }


        private static void AddToCache(string refDll, MetadataReference metadata)
        {
            cache.TryAdd(refDll, metadata);
        }

        private static string GetCorrectAssemblyName(string reference)
        {
            string assemblyName = reference.EndsWith(".dll") || reference.EndsWith(".exe") ?
                reference.Substring(0, reference.Length - 4) : reference;
            return assemblyName;
        }

        // backward compatibility
        public CompilerResults CompileAssemblyFromSource(CompilerParameters cp, string code)
        {
            return CompileAssemblyFromSource(cp, code, null);
        }

        public CompilerResults CompileAssemblyFromSource(CompilerParameters cp, string code, CultureInfo cultureInfo = null)
        {
            DebugMessage(typeof(SyntaxTree).Assembly.FullName);
            DebugMessage($"threadId: {Environment.CurrentManagedThreadId}");

#if NET6_0_OR_GREATER
            DebugMessage($"rid: {RuntimeInformation.RuntimeIdentifier} " +
                            $"arch: {RuntimeInformation.ProcessArchitecture} " +
                            $"os-arch: {RuntimeInformation.OSArchitecture} " +
                            $"os: {RuntimeInformation.OSDescription}");
#endif

            DebugMessage("FR.Compat: " +
#if NETSTANDARD
                "NETSTANDARD"
#elif NETCOREAPP
                "NETCOREAPP"
#endif
                );

            SyntaxTree codeTree = ParseTree(code);

            List<MetadataReference> references = new List<MetadataReference>();

            AddReferences(cp, references);

            DebugMessage($"References count: {references.Count}");
            //foreach (var reference in references)
            //    DebugMessage($"{reference.Display}");

            Compilation compilation = CreateCompilation(codeTree, references);


            OnBeforeEmitCompilation(compilation);

            return Emit(compilation, cultureInfo);
        }


        public async Task<CompilerResults> CompileAssemblyFromSourceAsync(CompilerParameters cp, string code, CultureInfo cultureInfo = null, CancellationToken cancellationToken = default)
        {
            DebugMessage(typeof(SyntaxTree).Assembly.FullName);
            DebugMessage($"threadId: {Environment.CurrentManagedThreadId}");

#if NET6_0_OR_GREATER
            DebugMessage($"rid: {RuntimeInformation.RuntimeIdentifier} " +
                $"arch: {RuntimeInformation.ProcessArchitecture} " +
                $"os-arch: {RuntimeInformation.OSArchitecture} " +
                $"os: {RuntimeInformation.OSDescription}");
#endif

            DebugMessage("FR.Compat: " +
#if NETSTANDARD
                "NETSTANDARD"
#elif NETCOREAPP
                "NETCOREAPP"
#endif
                );

            SyntaxTree codeTree = ParseTree(code, cancellationToken);

            List<MetadataReference> references = new List<MetadataReference>();

            await AddReferencesAsync(cp, references, cancellationToken);

            DebugMessage($"References count: {references.Count}");
            //foreach (var reference in references)
            //    DebugMessage($"{reference.Display}");

            Compilation compilation = CreateCompilation(codeTree, references);
            OnBeforeEmitCompilation(compilation);

            return await EmitAsync(compilation, cultureInfo, cancellationToken);
        }

        protected abstract Compilation CreateCompilation(SyntaxTree codeTree, ICollection<MetadataReference> references);

        protected abstract SyntaxTree ParseTree(string text, CancellationToken ct = default);

        public abstract void Dispose();


        private static CompilerResults Emit(Compilation compilation, CultureInfo cultureInfo, CancellationToken ct = default)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                DebugMessage("Emit...");
                //DebugMessage(code);
                DebugMessage($"threadId: {Environment.CurrentManagedThreadId}");                
                EmitResult results = compilation.Emit(ms,
                    cancellationToken: ct);
                return HandleEmitResult(results, ms, cultureInfo);
            }
        }

        private static async Task<CompilerResults> EmitAsync(Compilation compilation, CultureInfo cultureInfo, CancellationToken ct = default)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                DebugMessage("Emit...");
                //DebugMessage(code);
                // we use Task.Run because in Wasm Emit throws an exception in current context
                EmitResult results = await Task.Run(() => compilation.Emit(ms,
                            cancellationToken: ct), ct);
                return HandleEmitResult(results, ms, cultureInfo);
            }
        }

        private static CompilerResults HandleEmitResult(EmitResult results, MemoryStream ms, CultureInfo cultureInfo)
        {
            if (results.Success)
            {
#if DEBUG
                foreach (Diagnostic d in results.Diagnostics)
                    if (d.Severity > DiagnosticSeverity.Hidden)
                        DebugMessage($"Compiler {d.Severity}: {d.GetMessage()}. Line: {d.Location}");
#endif

                ms.Position = 0;
                Assembly compiledAssembly = null;

#if NETCOREAPP
                foreach (var assemblyLoadContext in AssemblyLoadContext.All)
                {
                    if (assemblyLoadContext.Assemblies.Any(assembly => assembly == typeof(CodeDomProvider).Assembly))
                    {
                        compiledAssembly = assemblyLoadContext.LoadFromStream(ms);
                    }
                }

                if (compiledAssembly == null)
                    compiledAssembly = AssemblyLoadContext.Default.LoadFromStream(ms);
#else
                compiledAssembly = Assembly.Load(ms.ToArray());
#endif
                return new CompilerResults(compiledAssembly);
            }
            else
            {
                DebugMessage($"results not success, {ms.Length}");
                CompilerResults result = new CompilerResults();
                foreach (Diagnostic d in results.Diagnostics)
                {
                    if (d.Severity == DiagnosticSeverity.Error)
                    {
                        DebugMessage(d.GetMessage());
                        var position = d.Location.GetLineSpan().StartLinePosition;
                        result.Errors.Add(new CompilerError()
                        {
                            ErrorText = d.GetMessage(cultureInfo),
                            ErrorNumber = d.Id,
                            Line = position.Line,
                            Column = position.Character,
                        });
                    }
                }
                return result;
            }
        }

    }
}
#endif