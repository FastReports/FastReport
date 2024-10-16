using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
#if NETCOREAPP
using System.Net.Http;
#endif
using System.Threading;
using System.Threading.Tasks;

namespace FastReport.Utils
{
    public static partial class ImageHelper
    {
        internal static async Task<byte[]> LoadAsync(string fileName, CancellationToken cancellationToken)
        {
            if (!String.IsNullOrEmpty(fileName))
#if NETCOREAPP
                return await File.ReadAllBytesAsync(fileName, cancellationToken);
#else
                return File.ReadAllBytes(fileName);
#endif
            return null;
        }

        internal static async Task<byte[]> LoadURLAsync(Uri url, CancellationToken cancellationToken)
        {
#if NETCOREAPP
            using (var httpClient = new HttpClient())
            {
                return await httpClient.GetByteArrayAsync(url, cancellationToken);
            }
#else
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0xc0 | 0x300 | 0xc00);
            using (var web = new WebClient())
            {
                return await web.DownloadDataTaskAsync(url);
            }
#endif
        }
    }
}
