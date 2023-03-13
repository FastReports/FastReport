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
    /// Loads necessary WebReport resources such as toolbar images, scripts and other
    /// </summary>
    public interface IResourceLoader
    {
        /// <summary>
        /// Returns the requested UTF8 string representation resource.
        /// </summary>
        /// <param name="name">Requested resource name</param>
        /// <returns>A UTF8 string representation resource. If the resource is not found - returns null</returns>
        string GetContent(string name);

        /// <summary>
        /// Asynchronously returns the requested UTF8 string representation resource.
        /// </summary>
        /// <param name="name">Requested resource name</param>
        /// <returns>A UTF8 string representation resource. If the resource is not found - returns null</returns>
        ValueTask<string> GetContentAsync(string name);

        /// <summary>
        /// Returns the requested resource as byte array.
        /// </summary>
        /// <param name="name">Requested resource name</param>
        /// <returns>Byte array of requested resource. If the resource is not found - returns null</returns>
        byte[] GetBytes(string name);

        /// <summary>
        /// Asynchronously returns the requested resource as byte array.
        /// </summary>
        /// <param name="name">Requested resource name</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Byte array of requested resource. If the resource is not found - returns null</returns>
        ValueTask<byte[]> GetBytesAsync(string name, CancellationToken cancellationToken = default);

    }
}
