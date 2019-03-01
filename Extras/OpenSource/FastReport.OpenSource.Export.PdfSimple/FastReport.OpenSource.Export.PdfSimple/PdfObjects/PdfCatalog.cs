using FastReport.Export.PdfSimple.PdfCore;

namespace FastReport.Export.PdfSimple.PdfObjects
{
    /// <summary>
    /// The catalog element for the pdf document
    /// </summary>
    public class PdfCatalog : PdfDictionary
    {
        #region Public Properties

        /// <summary>
        /// Sets pages of pdf document
        /// </summary>
        public PdfIndirectObject Pages
        {
            set
            {
                this["Pages"] = value;
            }
        }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Initialize a new catalog
        /// </summary>
        public PdfCatalog()
        {
            this["Type"] = new PdfName("Catalog");
            this["Version"] = new PdfName("1.5");
            PdfDictionary markInfo = new PdfDictionary();
            markInfo["Marked"] = new PdfBoolean(true);
            this["MarkInfo"] = markInfo;
        }

        #endregion Public Constructors
    }
}