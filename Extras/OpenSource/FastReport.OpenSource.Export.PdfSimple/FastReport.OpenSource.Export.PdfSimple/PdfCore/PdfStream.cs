using FastReport.Export.PdfSimple.PdfObjects;
using System.IO;
using System.IO.Compression;

namespace FastReport.Export.PdfSimple.PdfCore
{
    /// <summary>
    /// Pdf stream dictionary
    /// </summary>
    public class PdfStream : PdfDictionary
    {
        #region Private Fields

        private const int BASE = 65521;
        private const int NMAX = 5552;
        private bool compress = true;
        private byte[] stream;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Gets or sets compress for this stream
        /// </summary>
        public bool Compress { get { return compress; } set { compress = value; } }

        /// <summary>
        /// The stream of this object
        /// </summary>
        public byte[] Stream
        {
            set
            {
                this.stream = value;
            }
        }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Initialize a new object
        /// </summary>
        public PdfStream()
        {
        }

        #endregion Public Constructors

        #region Public Methods

        /// <inheritdocs/>
        public override void Write(PdfWriter writer)
        {
            if (stream == null)
                stream = new byte[0];

            byte[] byteStream = stream;

            if (compress)
            {
                using (MemoryStream fromMS = new MemoryStream(byteStream))
                {
                    using (MemoryStream toMS = new MemoryStream())
                    {
                        ZLibDeflate(fromMS, toMS);
                        byteStream = toMS.ToArray();
                    }
                }
                this["Filter"] = new PdfName("FlateDecode");
            }
            this["Length"] = new PdfNumeric(byteStream.Length);
            base.Write(writer);
            writer.WriteLn();
            writer.WriteLn("stream");
            writer.WriteBytesLn(byteStream);
            writer.WriteLn("endstream");
        }

        #endregion Public Methods

        #region Private Methods

        private long Adler32(long adler, byte[] buf, int index, int len)
        {
            if (buf == null) { return 1L; }

            long s1 = adler & 0xffff;
            long s2 = (adler >> 16) & 0xffff;
            int k;

            while (len > 0)
            {
                k = len < NMAX ? len : NMAX;
                len -= k;
                while (k >= 16)
                {
                    s1 += buf[index++] & 0xff; s2 += s1;
                    s1 += buf[index++] & 0xff; s2 += s1;
                    s1 += buf[index++] & 0xff; s2 += s1;
                    s1 += buf[index++] & 0xff; s2 += s1;
                    s1 += buf[index++] & 0xff; s2 += s1;
                    s1 += buf[index++] & 0xff; s2 += s1;
                    s1 += buf[index++] & 0xff; s2 += s1;
                    s1 += buf[index++] & 0xff; s2 += s1;
                    s1 += buf[index++] & 0xff; s2 += s1;
                    s1 += buf[index++] & 0xff; s2 += s1;
                    s1 += buf[index++] & 0xff; s2 += s1;
                    s1 += buf[index++] & 0xff; s2 += s1;
                    s1 += buf[index++] & 0xff; s2 += s1;
                    s1 += buf[index++] & 0xff; s2 += s1;
                    s1 += buf[index++] & 0xff; s2 += s1;
                    s1 += buf[index++] & 0xff; s2 += s1;
                    k -= 16;
                }
                if (k != 0)
                {
                    do
                    {
                        s1 += buf[index++] & 0xff;
                        s2 += s1;
                    }
                    while (--k != 0);
                }
                s1 %= BASE;
                s2 %= BASE;
            }
            return (s2 << 16) | s1;
        }

        private void ZLibDeflate(Stream src, Stream dst)
        {
            dst.WriteByte(0x78);
            dst.WriteByte(0xDA);
            src.Position = 0;
            long adler = 1L;
            using (DeflateStream compressor = new DeflateStream(dst, CompressionMode.Compress, true))
            {
                int bufflength = 2048;
                byte[] buff = new byte[bufflength];
                int i;
                while ((i = src.Read(buff, 0, bufflength)) > 0)
                {
                    adler = Adler32(adler, buff, 0, i);
                    compressor.Write(buff, 0, i);
                }
            }
            dst.WriteByte((byte)(adler >> 24 & 0xFF));
            dst.WriteByte((byte)(adler >> 16 & 0xFF));
            dst.WriteByte((byte)(adler >> 8 & 0xFF));
            dst.WriteByte((byte)(adler & 0xFF));
        }

        #endregion Private Methods
    }
}