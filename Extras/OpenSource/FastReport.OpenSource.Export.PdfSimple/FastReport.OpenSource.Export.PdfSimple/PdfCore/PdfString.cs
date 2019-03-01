using FastReport.Export.PdfSimple.PdfObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastReport.Export.PdfSimple.PdfCore
{
    /// <summary>
    /// The string object of pdf
    /// </summary>
    public class PdfString : PdfObjectBase
    {
        #region Private Fields

        private List<string> appendText;
        private bool isHex;
        private string text;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Set the presentation of this string, hex or ansii
        /// </summary>
        public bool IsHex
        {
            get { return isHex; }
            set { isHex = value; }
        }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Initialize a new instance and set the ansii text
        /// </summary>
        /// <param name="text"></param>
        public PdfString(string text)
        {
            this.text = text;
        }

        /// <summary>
        /// Initialize a new instance and set the text in ansii or hex
        /// </summary>
        /// <param name="text"></param>
        /// <param name="isHex"></param>
        public PdfString(string text, bool isHex)
        {
            this.text = text;
            this.isHex = isHex;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Append str to this text
        /// </summary>
        /// <param name="str">text for append</param>
        /// <returns>return this instance</returns>
        public PdfString Append(string str)
        {
            if (this.text == null)
            {
                this.text = str;
            }
            else
            {
                if (appendText == null)
                    appendText = new List<string>();
                appendText.Add(str);
            }
            return this;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (appendText != null)
            {
                StringBuilder sb = new StringBuilder();
                if (text != null)
                    sb.Append(text);
                foreach (string str in appendText)
                {
                    if (str != null)
                        sb.Append(str);
                }
                return sb.ToString();
            }
            if (text == null)
                return "";
            return text;
        }

        /// <inheritdoc/>
        public override void Write(PdfWriter writer)
        {
            if (isHex)
            {
                writer.Write("<");
                WriteHex(writer, text);
                if (appendText != null)
                {
                    foreach (string str in appendText)
                    {
                        WriteHex(writer, str);
                    }
                }
                writer.Write(">");
            }
            else
            {
                writer.Write("(");
                WriteText(writer, text);
                if (appendText != null)
                {
                    foreach (string str in appendText)
                    {
                        WriteText(writer, str);
                    }
                }
                writer.Write(")");
            }
        }

        #endregion Public Methods

        #region Private Methods

        private char[] StringToPdfUnicode(string s)
        {
            char[] result = new char[s.Length * 2 + 2];
            result[0] = (char)254;
            result[1] = (char)255;
            int i = 2;

            foreach (char c in s)
            {
                result[i] = (char)(c >> 8);
                result[i + 1] = (char)(c & 0xFF);
                i += 2;
            }
            return result;
        }

        private void WriteHex(PdfWriter writer, string text)
        {
            if (String.IsNullOrEmpty(text))
                return;
            char[] chars = StringToPdfUnicode(text);
            foreach (char c in chars)
            {
                writer.Write(((byte)c).ToString("X2"));
            }
        }

        private void WriteText(PdfWriter writer, string text)
        {
            if (String.IsNullOrEmpty(text))
                return;
            char[] chars = StringToPdfUnicode(text);
            foreach (char c in chars)
            {
                if (c < 127 )
                {
                    switch (c)
                    {
                        case '\n': writer.Write("\\n"); break;
                        case '\r': writer.Write("\\r"); break;
                        case '\t': writer.Write("\\t"); break;
                        case '\b': writer.Write("\\b"); break;
                        case '\f': writer.Write("\\f"); break;
                        case '(': writer.Write("\\("); break;
                        case ')': writer.Write("\\)"); break;
                        case '\\': writer.Write("\\\\"); break;
                        default: writer.Write(c); break;
                    }
                }
                else
                {
                    writer.Write("\\");
                    writer.Write((int)c);
                }
            }
        }

        #endregion Private Methods
    }
}