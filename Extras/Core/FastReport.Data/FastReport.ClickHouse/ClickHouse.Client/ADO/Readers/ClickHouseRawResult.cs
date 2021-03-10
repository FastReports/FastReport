using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClickHouse.Client.ADO
{
    public class ClickHouseRawResult : IDisposable
    {
        private readonly HttpResponseMessage response;

        internal ClickHouseRawResult(HttpResponseMessage response)
        {
            this.response = response;
        }

        public Task<Stream> ReadAsStreamAsync() => response.Content.ReadAsStreamAsync();

        public Task<byte[]> ReadAsByteArrayAsync() => response.Content.ReadAsByteArrayAsync();

        public Task<string> ReadAsStringAsync() => response.Content.ReadAsStringAsync();

        public Task CopyToAsync(Stream stream) => response.Content.CopyToAsync(stream);

        public void Dispose() => response?.Dispose();
    }
}
