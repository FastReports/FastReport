using FastReport.Export.PdfSimple.PdfObjects;

namespace FastReport.Export.PdfSimple.PdfCore
{
    /// <summary>
    /// The base object for all pdf object
    /// </summary>
    public abstract class PdfObjectBase
    {
        #region Public Methods

        /// <summary>
        /// Write this object to the stream
        /// </summary>
        /// <param name="writer">text stream</param>
        public abstract void Write(PdfWriter writer);

        #endregion Public Methods
    }
}