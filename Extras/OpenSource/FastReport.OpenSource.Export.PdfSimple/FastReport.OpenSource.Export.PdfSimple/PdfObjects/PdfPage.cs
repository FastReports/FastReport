using FastReport.Export.PdfSimple.PdfCore;
using System.Drawing;

namespace FastReport.Export.PdfSimple.PdfObjects
{
    /// <summary>
    /// The page in pdf document
    /// </summary>
    public class PdfPage : PdfDictionary
    {
        #region Private Fields

        private PdfDictionary xObject;

        #endregion Private Fields

        #region Public Properties

        public RectangleF MediaBox
        {
            set
            {
                PdfArray mediaBox = new PdfArray();
                mediaBox.Add(new PdfNumeric(value.Left));
                mediaBox.Add(new PdfNumeric(value.Top));
                mediaBox.Add(new PdfNumeric(value.Right));
                mediaBox.Add(new PdfNumeric(value.Bottom));
                this["MediaBox"] = mediaBox;
            }
        }

        /// <summary>
        /// Sets indirect link the pages
        /// </summary>
        public PdfIndirectObject Parent
        {
            set
            {
                this["Parent"] = value;
            }
        }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Initialize a new instance
        /// </summary>
        public PdfPage()
        {
            this["Type"] = new PdfName("Page");
            PdfDictionary resources;
            this["Resources"] = resources = new PdfDictionary();
            resources["XObject"] = xObject = new PdfDictionary();
            resources["ProcSet"] = new PdfArray(new PdfName[] { new PdfName("PDF"), new PdfName("Text"), new PdfName("ImageC") });
        }

        #endregion Public Constructors

        #region Internal Methods

        internal string AddImage(PdfIndirectObject imageLink)
        {
            string result = "Im" + xObject.Count;
            xObject[result] = imageLink;
            return "/" + result;
        }

        #endregion Internal Methods
    }
}