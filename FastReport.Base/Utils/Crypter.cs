using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace FastReport.Utils
{
  /// <summary>
  /// Contains methods used to crypt/decrypt a data.
  /// </summary>
  public static class Crypter
  {
    private static string FDefaultPassword = typeof(Crypter).FullName;
    
    /// <summary>
    /// Sets the password that is used to crypt connection strings stored in a report.
    /// </summary>
    /// <remarks>
    /// See the <see cref="FastReport.Data.DataConnectionBase.ConnectionString"/> property for more details.
    /// </remarks>
    public static string DefaultPassword
    {
      set { FDefaultPassword = value; }
    }
    
    /// <summary>
    /// Crypts a stream using specified password.
    /// </summary>
    /// <param name="dest">The destination stream that will receive the crypted data.</param>
    /// <param name="password">The password.</param>
    /// <returns>The stream that you need to write to.</returns>
    /// <remarks>
    /// Pass the stream you need to write to, to the <b>dest</b> parameter. Write your data to the 
    /// stream that this method returns. When you close this stream, the <b>dest</b> stream will be
    /// closed too and contains the crypted data.
    /// </remarks>
    public static Stream Encrypt(Stream dest, string password)
    {
      ICryptoTransform encryptor = null;
#if DOTNET_4
      using (PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, Encoding.UTF8.GetBytes("Salt")))
#else
      PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, Encoding.UTF8.GetBytes("Salt"));
#endif
      {
        RijndaelManaged rm = new RijndaelManaged();
        rm.Padding = PaddingMode.ISO10126;
        encryptor = rm.CreateEncryptor(pdb.GetBytes(16), pdb.GetBytes(16));
      }
      // write "rij" signature
      dest.Write(new byte[] { 114, 105, 106 }, 0, 3);
      return new CryptoStream(dest, encryptor, CryptoStreamMode.Write);
    }

    /// <summary>
    /// Decrypts a stream using specified password.
    /// </summary>
    /// <param name="source">Stream that contains crypted data.</param>
    /// <param name="password">The password.</param>
    /// <returns>The stream that contains decrypted data.</returns>
    /// <remarks>
    /// You should read from the stream that this method returns.
    /// </remarks>
    public static Stream Decrypt(Stream source, string password)
    {
      ICryptoTransform decryptor = null;
#if DOTNET_4
      using (PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, Encoding.UTF8.GetBytes("Salt")))
#else
      PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, Encoding.UTF8.GetBytes("Salt"));
#endif
      {
        RijndaelManaged rm = new RijndaelManaged();
        rm.Padding = PaddingMode.ISO10126;
        decryptor = rm.CreateDecryptor(pdb.GetBytes(16), pdb.GetBytes(16));
      }
      // check "rij" signature
      int byte1 = source.ReadByte();
      int byte2 = source.ReadByte();
      int byte3 = source.ReadByte();
      if (byte1 == 114 && byte2 == 105 && byte3 == 106)
        return new CryptoStream(source, decryptor, CryptoStreamMode.Read);
      source.Position -= 3;
      return null;
    }
    
    /// <summary>
    /// Checks if the stream contains a crypt signature.
    /// </summary>
    /// <param name="stream">Stream to check.</param>
    /// <returns><b>true</b> if stream is crypted.</returns>
    public static bool IsStreamEncrypted(Stream stream)
    {
      // check "rij" signature
      int byte1 = stream.ReadByte();
      int byte2 = stream.ReadByte();
      int byte3 = stream.ReadByte();
      stream.Position -= 3;
      return byte1 == 114 && byte2 == 105 && byte3 == 106;
    }
    
    /// <summary>
    /// Encrypts the string using the default password.
    /// </summary>
    /// <param name="data">String to encrypt.</param>
    /// <returns>The encrypted string.</returns>
    /// <remarks>
    /// The password used to encrypt a string can be set via <see cref="DefaultPassword"/> property.
    /// You also may use the <see cref="EncryptString(string, string)"/> method if you want to
    /// specify another password.
    /// </remarks>
    public static string EncryptString(string data)
    {
      return EncryptString(data, FDefaultPassword);
    }

    /// <summary>
    /// Encrypts the string using specified password.
    /// </summary>
    /// <param name="data">String to encrypt.</param>
    /// <param name="password">The password.</param>
    /// <returns>The encrypted string.</returns>
    public static string EncryptString(string data, string password)
    {
      if (String.IsNullOrEmpty(data) || String.IsNullOrEmpty(password))
        return data;

      using (MemoryStream stream = new MemoryStream())
      {
        using (Stream cryptedStream = Encrypt(stream, password))
        {
          byte[] bytes = Encoding.UTF8.GetBytes(data);
          cryptedStream.Write(bytes, 0, bytes.Length);
        }  
          
        return "rij" + Convert.ToBase64String(stream.ToArray());
      }
    }

    /// <summary>
    /// Decrypts the string using the default password.
    /// </summary>
    /// <param name="data">String to decrypt.</param>
    /// <returns>The decrypted string.</returns>
    /// <remarks>
    /// The password used to decrypt a string can be set via <see cref="DefaultPassword"/> property.
    /// You also may use the <see cref="DecryptString(string, string)"/> method if you want to
    /// specify another password.
    /// </remarks>
    public static string DecryptString(string data)
    {
      return DecryptString(data, FDefaultPassword);
    }

    /// <summary>
    /// Decrypts the string using specified password.
    /// </summary>
    /// <param name="data">String to decrypt.</param>
    /// <param name="password">The password.</param>
    /// <returns>The decrypted string.</returns>
    public static string DecryptString(string data, string password)
    {
      if (String.IsNullOrEmpty(data) || String.IsNullOrEmpty(password) || !data.StartsWith("rij"))
        return data;

      data = data.Substring(3);
      using (Stream stream = Converter.FromString(typeof(Stream), data) as Stream)
      {
        using (Stream decryptedStream = Decrypt(stream, password))
        {
          byte[] bytes = new byte[data.Length];
          int bytesRead = decryptedStream.Read(bytes, 0, bytes.Length);
          return Encoding.UTF8.GetString(bytes, 0, bytesRead);
        }
      }
    }

    /// <summary>
    /// Computes hash of specified stream. Initial position in stream will be saved.
    /// </summary>
    /// <param name="input">Initial stream</param>
    /// <returns></returns>
    public static string ComputeHash(Stream input)
    {
        byte[] buff = new byte[input.Length];
        input.Read(buff, 0, buff.Length);
        return ComputeHash(buff);
    }

    /// <summary>
    /// Computes hash of specified array. 
    /// </summary>
    /// <param name="input">Initial array</param>
    /// <returns></returns>
    public static string ComputeHash(byte[] input)
    {
        byte[] hash = new Murmur3().ComputeHash(input);        
        return BitConverter.ToString(hash).Replace("-", String.Empty);
    }

    /// <summary>
    /// Computes hash of specified array. 
    /// </summary>
    /// <param name="input">Initial array</param>
    /// <returns></returns>
    public static string ComputeHash(string input)
    {
        return ComputeHash(Encoding.UTF8.GetBytes(input));
    }
  }

    /// <summary>
    /// MurmurHash is a non-cryptographic hash function suitable for general hash-based lookup. 
    /// It was created by Austin Appleby in 2008 and is currently hosted on Github along with its test suite named 'SMHasher'. 
    /// It also exists in a number of variants, all of which have been released into the public domain. 
    /// The name comes from two basic operations, multiply (MU) and rotate (R), used in its inner loop.
    /// https://en.wikipedia.org/wiki/MurmurHash
    /// Implementation of Murmur3 Hash by Adam Horvath 
    /// http://blog.teamleadnet.com/2012/08/murmurhash3-ultra-fast-hash-algorithm.html
    /// </summary>
    [CLSCompliantAttribute(true)]
    public class Murmur3
    {
        // 128 bit output, 64 bit platform version
        /// <summary>
        /// READ_SIZE
        /// </summary>
        [CLSCompliantAttribute(false)]
        public static ulong READ_SIZE = 16;
        private static ulong C1 = 0x87c37b91114253d5L;
        private static ulong C2 = 0x4cf5ad432745937fL;

        private ulong length;
        private uint seed = 0; // if want to start with a seed, create a constructor
        ulong h1;
        ulong h2;

        private void MixBody(ulong k1, ulong k2)
        {
			unchecked 
			{ 
	            h1 ^= MixKey1(k1);
				h1 = (h1 << 27) | (h1 >> 37);
	            h1 += h2;
	            h1 = h1* 5 + 0x52dce729;
	            h2 ^= MixKey2(k2);
				h2 = (h2 << 31) | (h2 >> 33);
	            h2 += h1;
	            h2 = h2* 5 + 0x38495ab5;
			}
        }

        private static ulong MixKey1(ulong k1)
        {
			unchecked 
			{ 
	            k1 *= C1;
	            k1 = (k1 << 31) | (k1 >> 33);
	            k1 *= C2;
			}
            return k1;
        }

        private static ulong MixKey2(ulong k2)
        {
			unchecked 
			{ 
	            k2 *= C2;
	            k2 = (k2 << 33) | (k2 >> 31);
	            k2 *= C1;
			}
            return k2;
        }

        private static ulong MixFinal(ulong k)
        {
			unchecked 
			{ 
	            // avalanche bits
	            k ^= k >> 33;
	            k *= 0xff51afd7ed558ccdL;
	            k ^= k >> 33;
	            k *= 0xc4ceb9fe1a85ec53L;
	            k ^= k >> 33;
			}
            return k;
        }

        /// <summary>
        /// ComputeHash function
        /// </summary>
        /// <param name="bb"></param>
        /// <returns></returns>
        public byte[] ComputeHash(byte[] bb)
        {
            ProcessBytes(bb);
            return Hash;
        }

        private void ProcessBytes(byte[] bb)
        {
            h1 = seed;
            this.length = 0L;
            int pos = 0;
            int npos = 0;
            ulong remaining = (ulong)bb.Length;
            // read 128 bits, 16 bytes, 2 longs in eacy cycle
			while (remaining >= READ_SIZE) unchecked
            {
                npos = pos;
                ulong k1 = (uint)(bb[npos++] | bb[npos++] << 8 | bb[npos++] << 16 | bb[npos++] << 24);
                pos += 8;
                npos = pos;
                ulong k2 = (uint)(bb[npos++] | bb[npos++] << 8 | bb[npos++] << 16 | bb[npos++] << 24);
                pos += 8;
                length += READ_SIZE;
                remaining -= READ_SIZE;
                MixBody(k1, k2);
            }
            // if the input MOD 16 != 0
            if (remaining > 0)
                ProcessBytesRemaining(bb, remaining, pos);
        }

        private void ProcessBytesRemaining(byte[] bb, ulong remaining, int pos)
        {
            ulong k1 = 0;
            ulong k2 = 0;
            length += remaining;
            // little endian (x86) processing
            unchecked
            {
                switch (remaining)
                {
                    case 15:
                        k2 ^= (ulong)bb[pos + 14] << 48; // fall through
                        goto case 14;
                    case 14:
                        k2 ^= (ulong)bb[pos + 13] << 40; // fall through
                        goto case 13;
                    case 13:
                        k2 ^= (ulong)bb[pos + 12] << 32; // fall through
                        goto case 12;
                    case 12:
                        k2 ^= (ulong)bb[pos + 11] << 24; // fall through
                        goto case 11;
                    case 11:
                        k2 ^= (ulong)bb[pos + 10] << 16; // fall through
                        goto case 10;
                    case 10:
                        k2 ^= (ulong)bb[pos + 9] << 8; // fall through
                        goto case 9;
                    case 9:
                        k2 ^= (ulong)bb[pos + 8]; // fall through
                        goto case 8;
                    case 8:
                        int npos = pos;
                        k1 ^= (uint)(bb[npos++] | bb[npos++] << 8 | bb[npos++] << 16 | bb[npos++] << 24);
                        break;
                    case 7:
                        k1 ^= (ulong)bb[pos + 6] << 48; // fall through
                        goto case 6;
                    case 6:
                        k1 ^= (ulong)bb[pos + 5] << 40; // fall through
                        goto case 5;
                    case 5:
                        k1 ^= (ulong)bb[pos + 4] << 32; // fall through
                        goto case 4;
                    case 4:
                        k1 ^= (ulong)bb[pos + 3] << 24; // fall through
                        goto case 3;
                    case 3:
                        k1 ^= (ulong)bb[pos + 2] << 16; // fall through
                        goto case 2;
                    case 2:
                        k1 ^= (ulong)bb[pos + 1] << 8; // fall through
                        goto case 1;
                    case 1:
                        k1 ^= (ulong)bb[pos]; // fall through
                        break;
                    default:
                        throw new Exception("Something went wrong with remaining bytes calculation.");
                }
                h1 ^= MixKey1(k1);
                h2 ^= MixKey2(k2);
            }
        }

        /// <summary>
        /// Gets the Hash
        /// </summary>
        public byte[] Hash
        {
            get
            {
				unchecked
				{
					h1 ^= length;
					h2 ^= length;
					h1 += h2;
					h2 += h1;
					h1 = Murmur3.MixFinal(h1);
					h2 = Murmur3.MixFinal(h2);
					h1 += h2;
					h2 += h1;
				}
                byte[] hash = new byte[Murmur3.READ_SIZE];
                Array.Copy(BitConverter.GetBytes(h1), 0, hash, 0, 8);
                Array.Copy(BitConverter.GetBytes(h2), 0, hash, 8, 8);
                return hash;
            }
        }
    }
}
