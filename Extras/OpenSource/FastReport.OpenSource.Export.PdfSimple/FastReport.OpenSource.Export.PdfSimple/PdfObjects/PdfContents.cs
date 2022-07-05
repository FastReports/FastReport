using FastReport.Export.PdfSimple.PdfCore;
using System;
using System.Text;

namespace FastReport.Export.PdfSimple.PdfObjects
{
    /// <summary>
    /// The contents stream of page of pdf file
    /// </summary>
    public class PdfContents : PdfStream
    {
        #region Private Fields

        private int realPrecision = 2;
        private StringBuilder sb = new StringBuilder();

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Gets or sets length of content stream
        /// </summary>
        public int Length
        {
            get { return sb.Length; }
            set { sb.Length = value; }
        }

        /// <summary>
        /// Percision for real values
        /// </summary>
        public int RealPrecision
        {
            get { return realPrecision; }
            set { realPrecision = value; }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Append text
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public PdfContents Append(string text)
        {
            sb.Append(text);
            return this;
        }

        /// <summary>
        /// Convert real value to text and append
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public PdfContents Append(float value)
        {
            sb.Append(ExportUtils.FloatToString(value, realPrecision));
            return this;
        }

        /// <summary>
        /// Convert real value to text and append
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public PdfContents Append(double value)
        {
            sb.Append(ExportUtils.FloatToString(value, realPrecision));
            return this;
        }

        /// <summary>
        /// Convert int value to text and append
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public PdfContents Append(int value)
        {
            sb.Append(value);
            return this;
        }

        /// <summary>
        /// Convert long value to text and append
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public PdfContents Append(long value)
        {
            sb.Append(value);
            return this;
        }

        /// <summary>
        /// Append new line
        /// </summary>
        /// <returns></returns>
        public PdfContents AppendLine()
        {
            sb.AppendLine();
            return this;
        }

        /// <summary>
        /// Clear the stream
        /// </summary>
        public void ClearContent()
        {
            sb.Length = 0;
        }

        /// <inheritdoc/>
        public override void Write(PdfWriter writer)
        {
            Stream = Encoding.ASCII.GetBytes(sb.ToString());
            base.Write(writer);
        }

        #endregion Public Methods
    }
}