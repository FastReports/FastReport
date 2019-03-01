using FastReport.Export.PdfSimple.PdfCore;

namespace FastReport.Export.PdfSimple.PdfObjects
{
    /// <summary>
    /// The mask of image of pdf
    /// </summary>
    public class PdfMask : PdfImage
    {
        #region Public Constructors

        /// <summary>
        /// Initialize a new instance
        /// </summary>
        public PdfMask()
            : base()
        {
            Compress = true;
            this["ColorSpace"] = new PdfName("DeviceGray");
            this["Interpolate"] = new PdfBoolean(false);
        }

        #endregion Public Constructors
    }
}