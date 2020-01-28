#if NETSTANDARD2_0 || NETSTANDARD2_1 || NETCOREAPP
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace System.CodeDom.Compiler
{
    internal abstract class CodeDomProvider : IDisposable
    {
        static Dictionary<string, string> cache = new Dictionary<string, string>();

        public abstract void Dispose();
        public abstract CompilerResults CompileAssemblyFromSource(CompilerParameters cp, string v);

        protected void AddExtraAssemblies(Collections.Specialized.StringCollection referencedAssemblies, List<MetadataReference> references)
        {

            string[] assemblies = new[] {
                "netstandard",
                "System.Runtime",
                "System.ComponentModel.Primitives",
                "System.Drawing.Common"
            };

            foreach(string assembly in assemblies)
            {
                if (!referencedAssemblies.Contains(assembly))
                    references.Add(GetReference(assembly));
            }
        }


        internal MetadataReference GetReference(string refDll)
        {
            string reference = refDll;
            try
            {
                if (cache.ContainsKey(refDll))
                    return MetadataReference.CreateFromFile(cache[refDll]);
                MetadataReference result;
                foreach (AssemblyName name in Assembly.GetExecutingAssembly().GetReferencedAssemblies())
                {
                    if (name.Name == reference
                        || reference.ToLower().EndsWith(".dll")
                        && name.Name == reference.Substring(0, reference.Length - 4))
                    {
                        result = MetadataReference.CreateFromFile(
                            Assembly.Load(name).Location);
                        cache[refDll] = reference;
                        return result;
                    }
                }

                result = MetadataReference.CreateFromFile(reference);
                cache[refDll] = reference;
                return result;
            }
            catch
            {
                string ass = reference;
                if (reference.ToLower().EndsWith(".dll"))
                    ass = reference.Substring(0, reference.Length - 4);
                cache[refDll] = Assembly.Load(new AssemblyName(ass)).Location;
                return MetadataReference.CreateFromFile(cache[refDll]);
            }

        }
    }
}
#endif