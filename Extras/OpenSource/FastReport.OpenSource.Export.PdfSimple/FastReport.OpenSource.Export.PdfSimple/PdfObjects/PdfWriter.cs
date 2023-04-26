using FastReport.Export.PdfSimple.PdfCore;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace FastReport.Export.PdfSimple.PdfObjects
{
    /// <summary>
    /// Writer for pdf document
    /// </summary>
    public class PdfWriter
    {
        #region Internal Fields

        internal const float PDF_PAGE_DIVIDER = 2.8346400000000003f;
        internal const float PDF_DIVIDER = 0.75f;

        #endregion Internal Fields

        #region Private Fields

        // mm to point
        private Stream baseStream;
        private List<PdfDirectObject> objects = new List<PdfDirectObject>();
        private string pdfVersion = "%PDF-1.5";
        private PdfDictionary trailer;
        private long xNumber = 1;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initialize a new pdf document writer
        /// </summary>
        /// <param name="stream"></param>
        public PdfWriter(Stream stream)
        {
            baseStream = stream;
            trailer = new PdfDictionary();
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Begin the pdf document
        /// </summary>
        public void Begin()
        {
            WriteLn(pdfVersion);
            byte[] signature = { 0x25, 0xE2, 0xE3, 0xCF, 0xD3, 0x0D, 0x0A };
            baseStream.Write(signature, 0, signature.Length);
        }

        /// <summary>
        /// Finish the pdf document
        /// </summary>
        public void Finish()
        {
            foreach (PdfDirectObject obj in objects)
            {
                if (obj.HaveToWrite)
                    obj.Write(this, baseStream.Position);
            }

            long xrefPos = baseStream.Position;
            WriteLn("xref");
            Write("0 "); WriteLn(objects.Count + 1);
            WriteLn("0000000000 65535 f");
            foreach (PdfDirectObject obj in objects)
            {
                Write(PrepXRefPos(obj.Offset));
                WriteLn(" 00000 n");
            }

            trailer["Size"] = new PdfNumeric(objects.Count + 1);
            trailer["ID"] = new PdfTrailerId();

            WriteLn("trailer");
            trailer.Write(this);
            WriteLn();
            WriteLn("startxref");
            WriteLn(xrefPos);
            WriteLn("%%EOF");
        }

        /// <summary>
        /// Prepare this object for writing
        /// </summary>
        /// <param name="objectBase"></param>
        /// <returns></returns>
        public PdfIndirectObject Prepare(PdfObjectBase objectBase)
        {
            PdfIndirectObject result;
            PdfDirectObject pdfObject;
            ProcessObject(objectBase, out pdfObject, out result);
            pdfObject.Prepare(objectBase);
            return result;
        }

        /// <summary>
        /// Write pdf object to stream;
        /// </summary>
        /// <param name="objectBase"></param>
        /// <returns></returns>
        public PdfCore.PdfIndirectObject Write(PdfObjectBase objectBase)
        {
            PdfIndirectObject result;
            PdfDirectObject pdfObject;
            ProcessObject(objectBase, out pdfObject, out result);
            pdfObject.Write(this, objectBase, baseStream.Position);
            return result;
        }

        /// <summary>
        /// Write string to pdf file
        /// </summary>
        /// <param name="value"></param>
        public void Write(string value)
        {
            byte[] arr = StringToByteArray(value);
            baseStream.Write(arr, 0, arr.Length);
        }

        /// <summary>
        /// Write long value to pdf file
        /// </summary>
        /// <param name="value"></param>
        public void Write(long value)
        {
            Write(value.ToString(CultureInfo.CurrentCulture.NumberFormat));
        }

        /// <summary>
        /// Write int value to pdf file
        /// </summary>
        /// <param name="value"></param>
        public void Write(int value)
        {
            Write(value.ToString(CultureInfo.CurrentCulture.NumberFormat));
        }

        /// <summary>
        /// Write char value to pdf file
        /// </summary>
        /// <param name="value"></param>
        public void Write(char value)
        {
            baseStream.WriteByte((byte)value);
        }

        /// <summary>
        /// Write raw byte to pdf file
        /// </summary>
        /// <param name="byteRaw"></param>
        public void WriteByte(byte byteRaw)
        {
            baseStream.WriteByte(byteRaw);
        }

        /// <summary>
        /// Write raw byte array to pdf file
        /// </summary>
        /// <param name="stream"></param>
        public void WriteBytes(byte[] stream)
        {
            baseStream.Write(stream, 0, stream.Length);
        }

        /// <summary>
        /// Write raw byte array to pdf file and then break a line
        /// </summary>
        /// <param name="stream"></param>
        public void WriteBytesLn(byte[] stream)
        {
            WriteBytes(stream);
            WriteLn();
        }

        /// <summary>
        /// Write long value to pdf file and break a line
        /// </summary>
        /// <param name="value"></param>
        public void WriteLn(long value)
        {
            Write(value);
            WriteLn();
        }

        /// <summary>
        /// Write int value to pdf file and break a line
        /// </summary>
        /// <param name="value"></param>
        public void WriteLn(int value)
        {
            Write(value);
            WriteLn();
        }

        /// <summary>
        /// Write string value to pdf file and break a line
        /// </summary>
        /// <param name="value"></param>
        public void WriteLn(string value)
        {
            Write(value);
            WriteLn();
        }

        /// <summary>
        /// Break a line in pdf file
        /// </summary>
        public void WriteLn()
        {
            baseStream.WriteByte(0x0d);
            baseStream.WriteByte(0x0a);
        }

        #endregion Public Methods

        #region Private Methods

        private string PrepXRefPos(long p)
        {
            string pos = p.ToString();
            return new string('0', 10 - pos.Length) + pos;
        }

        private void ProcessObject(PdfObjectBase objectBase, out PdfDirectObject pdfDirectObject, out PdfIndirectObject pdfIndirectObject)
        {
            pdfDirectObject = new PdfDirectObject(xNumber);
            pdfIndirectObject = new PdfIndirectObject(xNumber);
            xNumber++;
            objects.Add(pdfDirectObject);

            if (objectBase is PdfCatalog)
            {
                trailer["Root"] = pdfIndirectObject;
            }
            else if (objectBase is PdfInfo)
            {
                trailer["Info"] = pdfIndirectObject;
            }
        }

        private byte[] StringToByteArray(string source)
        {
            byte[] result = new byte[source.Length];
            for (int i = 0; i < source.Length; i++)
                result[i] = (byte)source[i];
            return result;
        }

        #endregion Private Methods
    }
}