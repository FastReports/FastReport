using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using FastReport.Barcode.QRCode;
using FastReport.Utils;
using System.Drawing.Drawing2D;

namespace FastReport.Barcode
{
    /// <summary>
    /// Specifies the QR code error correction level.
    /// </summary>
    public enum QRCodeErrorCorrection
    {
        /// <summary>
        /// L = ~7% correction.
        /// </summary>
        L,

        /// <summary>
        /// M = ~15% correction.
        /// </summary>
        M,

        /// <summary>
        /// Q = ~25% correction.
        /// </summary>
        Q,

        /// <summary>
        /// H = ~30% correction.
        /// </summary>
        H
    }

    /// <summary>
    /// Specifies the QR Code encoding.
    /// </summary>
    public enum QRCodeEncoding
    {
        /// <summary>
        /// UTF-8 encoding.
        /// </summary>
        UTF8,
        /// <summary>
        /// ISO 8859-1 encoding.
        /// </summary>
        ISO8859_1,
        /// <summary>
        /// Shift_JIS encoding.
        /// </summary>
        Shift_JIS,
        /// <summary>
        /// Windows-1251 encoding.
        /// </summary>
        Windows_1251,
        /// <summary>
        /// cp866 encoding.
        /// </summary>
        cp866
    }

    /// <summary>
    /// Generates the 2D QR code barcode.
    /// </summary>
    public class BarcodeQR : Barcode2DBase
    {
        #region Fields
        private QRCodeErrorCorrection errorCorrection;
        private QRCodeEncoding encoding;
        private bool quietZone;
        private ByteMatrix matrix;
        private const int PixelSize = 4;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the error correction.
        /// </summary>
        [DefaultValue(QRCodeErrorCorrection.L)]
        public QRCodeErrorCorrection ErrorCorrection
        {
            get { return errorCorrection; }
            set { errorCorrection = value; }
        }

        /// <summary>
        /// Gets or sets the encoding used for text conversion.
        /// </summary>
        [DefaultValue(QRCodeEncoding.UTF8)]
        public QRCodeEncoding Encoding
        {
            get { return encoding; }
            set { encoding = value; }
        }

        /// <summary>
        /// Gets or sets the value indicating that quiet zone must be shown.
        /// </summary>
        [DefaultValue(true)]
        public bool QuietZone
        {
            get { return quietZone; }
            set { quietZone = value; }
        }
        #endregion

        #region Private Methods
        private ErrorCorrectionLevel GetErrorCorrectionLevel()
        {
            switch (errorCorrection)
            {
                case QRCodeErrorCorrection.L:
                    return ErrorCorrectionLevel.L;

                case QRCodeErrorCorrection.M:
                    return ErrorCorrectionLevel.M;

                case QRCodeErrorCorrection.Q:
                    return ErrorCorrectionLevel.Q;

                case QRCodeErrorCorrection.H:
                    return ErrorCorrectionLevel.H;
            }

            return ErrorCorrectionLevel.L;
        }

        private string GetEncoding()
        {
            switch (encoding)
            {
                case QRCodeEncoding.UTF8:
                    return "UTF-8";

                case QRCodeEncoding.ISO8859_1:
                    return "ISO-8859-1";

                case QRCodeEncoding.Shift_JIS:
                    return "Shift_JIS";

                case QRCodeEncoding.Windows_1251:
                    return "Windows-1251";

                case QRCodeEncoding.cp866:
                    return "cp866";
            }

            return "";
        }
        #endregion

        #region Public Methods
        /// <inheritdoc/>
        public override void Assign(BarcodeBase source)
        {
            base.Assign(source);
            BarcodeQR src = source as BarcodeQR;

            ErrorCorrection = src.ErrorCorrection;
            Encoding = src.Encoding;
            QuietZone = src.QuietZone;
        }

        internal override void Serialize(FastReport.Utils.FRWriter writer, string prefix, BarcodeBase diff)
        {
            base.Serialize(writer, prefix, diff);
            BarcodeQR c = diff as BarcodeQR;

            if (c == null || ErrorCorrection != c.ErrorCorrection)
                writer.WriteValue(prefix + "ErrorCorrection", ErrorCorrection);
            if (c == null || Encoding != c.Encoding)
                writer.WriteValue(prefix + "Encoding", Encoding);
            if (c == null || QuietZone != c.QuietZone)
                writer.WriteBool(prefix + "QuietZone", QuietZone);
        }

        internal override void Initialize(string text, bool showText, int angle, float zoom)
        {
            base.Initialize(text, showText, angle, zoom);
            matrix = QRCodeWriter.encode(base.text, 0, 0, GetErrorCorrectionLevel(), GetEncoding(), QuietZone);
        }

        internal override SizeF CalcBounds()
        {
            int textAdd = showText ? 18 : 0;
            return new SizeF(matrix.Width * PixelSize, matrix.Height * PixelSize + textAdd);
        }

        internal override void Draw2DBarcode(IGraphicsRenderer g, float kx, float ky)
        {
            Brush light = Brushes.White;
            Brush dark = new SolidBrush(Color);
            GraphicsPath path = new GraphicsPath();

            for (int y = 0; y < matrix.Height; y++)
            {
                for (int x = 0; x < matrix.Width; x++)
                {
                    if (matrix.get_Renamed(x, y) == 0)
                    {
                        g.PathAddRectangle(path, new RectangleF(
                        x * PixelSize * kx,
                        y * PixelSize * ky,
                        PixelSize * kx,
                        PixelSize * ky
                        ));
                    }
                }
            }
            if (path.PointCount > 0)
            {
                g.FillPath(dark, path);
                if(text.StartsWith("SPC"))
                {
                    ErrorCorrection = QRCodeErrorCorrection.M;
                }
            }

            dark.Dispose();
            path.Dispose();
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="BarcodeQR"/> class with default settings.
        /// </summary>
        public BarcodeQR()
        {
            Encoding = QRCodeEncoding.UTF8;
            QuietZone = true;
        }
    }
}
