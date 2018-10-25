using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace FastReport.Utils
{
  internal static class Compressor
  {
    public static Stream Decompress(Stream source, bool bidiStream)
    {
      int byte1 = source.ReadByte();
      int byte2 = source.ReadByte();
      source.Position -= 2;
      bool result = byte1 == 0x1F && byte2 == 0x8B;
      if (result)
      {
        if (bidiStream)
        {
          // create bidirectional stream
          Stream stream = new MemoryStream();
          using (GZipStream gzip = new GZipStream(source, CompressionMode.Decompress))
          {
            byte[] buffer = new byte[4096];
            while (true)
            {
              int bytesRead = gzip.Read(buffer, 0, 4096);
              if (bytesRead == 0) break;
              stream.Write(buffer, 0, bytesRead);
            }
          }
          stream.Position = 0;
          return stream;
        }
        else
          return new GZipStream(source, CompressionMode.Decompress);
      }  
      return null;
    }

    public static Stream Compress(Stream dest)
    {
      return new GZipStream(dest, CompressionMode.Compress, true);
    }

    public static byte[] Compress(byte[] buffer)
    {
      using (MemoryStream dest = new MemoryStream())
      {
        using (Stream gzipStream = new GZipStream(dest, CompressionMode.Compress, true))
        {
          gzipStream.Write(buffer, 0, buffer.Length);
        }

        return dest.ToArray();
      }
    }

    public static string Compress(string source)
    {
      UTF8Encoding encoding = new UTF8Encoding();
      byte[] srcBytes = encoding.GetBytes(source);
      byte[] compressedBytes = Compress(srcBytes);
      return Convert.ToBase64String(compressedBytes);
    }

    public static byte[] Decompress(byte[] buffer)
    {
      using (MemoryStream ms = new MemoryStream(buffer))
      {
        if (IsStreamCompressed(ms))
        {
          using (MemoryStream uncompressedStream = Compressor.Decompress(ms, true) as MemoryStream)
          {
            return uncompressedStream.ToArray();
          }
        }
        else
        {
          return buffer;
        }
      }
    }

    public static string Decompress(string source)
    {
      byte[] srcBytes = Convert.FromBase64String(source);
      byte[] decompressedBytes = Compressor.Decompress(srcBytes);
      UTF8Encoding encoding = new UTF8Encoding();
      return encoding.GetString(decompressedBytes);
    }

    public static bool IsStreamCompressed(Stream stream)
    {
      int byte1 = stream.ReadByte();
      int byte2 = stream.ReadByte();
      stream.Position -= 2;
      return byte1 == 0x1F && byte2 == 0x8B;
    }
  }
}