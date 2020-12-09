using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using FastReport.Utils;

namespace FastReport.Export.Html
{

    /// <summary>
    /// Represents the HTML export format enum
    /// </summary>
    public enum HTMLExportFormat
    {
        /// <summary>
        /// Represents the message-HTML type
        /// </summary>
        MessageHTML,
        /// <summary>
        /// Represents the HTML type
        /// </summary>
        HTML
    }

    /// <summary>
    /// Specifies the image format in HTML export.
    /// </summary>
    public enum ImageFormat
    {
        /// <summary>
        /// Specifies the .bmp format.
        /// </summary>
        Bmp,

        /// <summary>
        /// Specifies the .png format.
        /// </summary>
        Png,

        /// <summary>
        /// Specifies the .jpg format.
        /// </summary>
        Jpeg,

        /// <summary>
        /// Specifies the .gif format.
        /// </summary>
        Gif
    }

    /// <summary>
    /// Specifies the units of HTML sizes.
    /// </summary>
    public enum HtmlSizeUnits
    {
        /// <summary>
        /// Specifies the pixel units.
        /// </summary>
        Pixel,
        /// <summary>
        /// Specifies the percent units.
        /// </summary>
        Percent
    }

    public partial class HTMLExport : ExportBase
    {
        private string Px(double pixel)
        {
            return String.Join(String.Empty, new String[] { Convert.ToString(Math.Round(pixel, 2), numberFormat), "px;" });
        }

        private string SizeValue(double value, double maxvalue, HtmlSizeUnits units)
        {
            FastString sb = new FastString(6);
            if (units == HtmlSizeUnits.Pixel)
                sb.Append(Px(value));
            else if (units == HtmlSizeUnits.Percent)
                sb.Append(((int)Math.Round((value * 100 / maxvalue))).ToString()).Append("%");
            else
                sb.Append(value.ToString());
            return sb.ToString();
        }

        private void WriteMimePart(Stream stream, string mimetype, string charset, string filename)
        {
            FastString sb = new FastString();
            sb.Append("--").AppendLine(boundary);
            sb.Append("Content-Type: ").Append(mimetype).Append(";");
            if (charset != String.Empty)
                sb.Append(" charset=\"").Append(charset).AppendLine("\"");
            else
                sb.AppendLine();
            string body;
            byte[] buff = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(buff, 0, buff.Length);
            if (mimetype == "text/html")
            {
                sb.AppendLine("Content-Transfer-Encoding: quoted-printable");
                body = ExportUtils.QuotedPrintable(buff);
            }
            else
            {
                sb.AppendLine("Content-Transfer-Encoding: base64");
                body = System.Convert.ToBase64String(buff, Base64FormattingOptions.InsertLineBreaks);
            }
            sb.Append("Content-Location: ").AppendLine(ExportUtils.HtmlURL(filename));
            sb.AppendLine();
            sb.AppendLine(body);
            sb.AppendLine();
            Stream.Write(Encoding.ASCII.GetBytes(sb.ToString()), 0, sb.Length);
            sb.Clear();
        }

        private void WriteMHTHeader(Stream Stream, string FileName)
        {
            FastString sb = new FastString(256);
            string s = "=?utf-8?B?" + System.Convert.ToBase64String(Encoding.UTF8.GetBytes(FileName)) + "?=";
            sb.Append("From: ").AppendLine(s);
            sb.Append("Subject: ").AppendLine(s);
            sb.Append("Date: ").AppendLine(ExportUtils.GetRFCDate(SystemFake.DateTime.Now));
            sb.AppendLine("MIME-Version: 1.0");
            sb.Append("Content-Type: multipart/related; type=\"text/html\"; boundary=\"").Append(boundary).AppendLine("\"");
            sb.AppendLine();
            sb.AppendLine("This is a multi-part message in MIME format.");
            sb.AppendLine();
            ExportUtils.Write(Stream, sb.ToString());
            sb.Clear();
        }

    }

    /// <summary>
    /// For internal use only.
    /// </summary>
    public class HTMLPageData
    {
        private string cssText;
        private string pageText;
        private List<Stream> pictures;
        private List<string> guids;
        private ManualResetEvent pageEvent;
        private int pageNumber;
        private float width;
        private float height;

        /// <summary>
        /// For internal use only.
        /// </summary>
        public float Width
        {
            get { return width; }
            set { width = value; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        public float Height
        {
            get { return height; }
            set { height = value; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        public string CSSText
        {
            get { return cssText; }
            set { cssText = value; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        public string PageText
        {
            get { return pageText; }
            set { pageText = value; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        public List<Stream> Pictures
        {
            get { return pictures; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        public List<string> Guids
        {
            get { return guids; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        public ManualResetEvent PageEvent
        {
            get { return pageEvent; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        public int PageNumber
        {
            get { return pageNumber; }
            set { pageNumber = value; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        public HTMLPageData()
        {
            pictures = new List<Stream>();
            guids = new List<string>();
            pageEvent = new ManualResetEvent(false);
        }
    }

}
