using FastReport.Export.PdfSimple.PdfCore;

namespace FastReport.Export.PdfSimple.PdfObjects
{
    /// <summary>
    /// XObject Image for pdf
    /// </summary>
    public class PdfImage : PdfStream
    {
        #region Public Properties

        /// <summary>
        /// Sets height of pdf image
        /// </summary>
        public int Height
        {
            set
            {
                this["Height"] = new PdfNumeric(value);
            }
        }

        /// <summary>
        /// Sets width of pdf image
        /// </summary>
        public int Width
        {
            set
            {
                this["Width"] = new PdfNumeric(value);
            }
        }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Initialize a new instance
        /// </summary>
        public PdfImage()
        {
            Compress = false;
            this["Type"] = new PdfName("XObject");
            this["Subtype"] = new PdfName("Image");
            this["ColorSpace"] = new PdfName("DeviceRGB");
            this["BitsPerComponent"] = new PdfNumeric(8);
            this["Filter"] = new PdfName("DCTDecode");
            this["Interpolate"] = new PdfBoolean(true);
        }

        #endregion Public Constructors
    }
}