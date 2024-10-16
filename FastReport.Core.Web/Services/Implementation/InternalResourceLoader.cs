using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FastReport.Web.Services
{
    /// <summary>
    /// Internal implementation of <see cref="IResourceLoader"/>.
    /// Loads requested resources from embedded resources of the current assembly
    /// </summary>
    internal sealed class InternalResourceLoader : IResourceLoader
    {

        static readonly string AssemblyName;
        static readonly Assembly _assembly;

        static InternalResourceLoader()
        {
            _assembly = Assembly.GetExecutingAssembly();
            AssemblyName = _assembly.GetName().Name;
        }


        readonly ConcurrentDictionary<string, string> cache1 = new ConcurrentDictionary<string, string>();
        readonly ConcurrentDictionary<string, byte[]> cache2 = new ConcurrentDictionary<string, byte[]>();


        public string GetContent(string name)
        {
            if (cache1.TryGetValue(name, out string value))
                return value;

            var fullname = $"{AssemblyName}.Resources.{name}";
            var resourceStream = _assembly.GetManifestResourceStream(fullname);
            if (resourceStream == null)
                return null;

            using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
            {
                var res = reader.ReadToEnd();
                cache1[name] = res;
                return res;
            }
        }

        public async ValueTask<string> GetContentAsync(string name)
        {
            if (cache1.TryGetValue(name, out string value))
                return value;

            var fullname = $"{AssemblyName}.Resources.{name}";
            var resourceStream = _assembly.GetManifestResourceStream(fullname);
            if (resourceStream == null)
                return null;

            using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
            {
                var res = await reader.ReadToEndAsync();
                cache1[name] = res;
                return res;
            }
        }

        public byte[] GetBytes(string name)
        {
            if (cache2.TryGetValue(name, out byte[] value))
                return value;

            var fullname = $"{AssemblyName}.Resources.{name}";
            var resourceStream = _assembly.GetManifestResourceStream(fullname);
            if (resourceStream == null)
                return null;

            var buffer = new byte[resourceStream.Length];
            resourceStream.Read(buffer, 0, buffer.Length);
            
            cache2[name] = buffer;
            return buffer;
        }

        public async ValueTask<byte[]> GetBytesAsync(string name, CancellationToken cancellationToken = default)
        {
            if (cache2.TryGetValue(name, out byte[] value))
                return value;

            var fullname = $"{AssemblyName}.Resources.{name}";
            var resourceStream = _assembly.GetManifestResourceStream(fullname);
            if (resourceStream == null)
                return null;

            var buffer = new byte[resourceStream.Length];
            await resourceStream.ReadAsync(buffer, cancellationToken);
            cache2[name] = buffer;
            return buffer;
        }
    }
}
