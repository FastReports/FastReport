using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

/// <summary>
/// Originally sourced from https://stackoverflow.com/questions/16673714/how-to-compress-http-request-on-the-fly-and-without-loading-compressed-buffer-in
/// </summary>
namespace ClickHouse.Client.Utility
{
    public class CompressedContent : HttpContent
    {
        private readonly HttpContent originalContent;
        private readonly DecompressionMethods compressionMethod;

        public CompressedContent(HttpContent content, DecompressionMethods compressionMethod)
        {
            originalContent = content ?? throw new ArgumentNullException("content");
            this.compressionMethod = compressionMethod;

            if (this.compressionMethod != DecompressionMethods.GZip && this.compressionMethod != DecompressionMethods.Deflate)
            {
                throw new ArgumentException(string.Format($"Compression '{compressionMethod}' is not supported. Valid types: GZip, Deflate"), nameof(compressionMethod));
            }

            foreach (var header in originalContent.Headers)
            {
                Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            Headers.ContentEncoding.Add(this.compressionMethod.ToString().ToLowerInvariant());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                originalContent?.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = -1;
            return false;
        }

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            using Stream compressedStream = compressionMethod switch
            {
                DecompressionMethods.GZip => new GZipStream(stream, CompressionLevel.Fastest, leaveOpen: true),
                DecompressionMethods.Deflate => new DeflateStream(stream, CompressionMode.Compress, leaveOpen: true),
                _ => throw new ArgumentOutOfRangeException(nameof(compressionMethod))
            };

            await originalContent.CopyToAsync(compressedStream);
        }
    }
}
