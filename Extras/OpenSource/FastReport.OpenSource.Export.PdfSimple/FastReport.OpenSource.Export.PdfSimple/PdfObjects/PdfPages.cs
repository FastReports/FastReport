using FastReport.Export.PdfSimple.PdfCore;

namespace FastReport.Export.PdfSimple.PdfObjects
{
    /// <summary>
    /// The pages object of catalog
    /// </summary>
    public class PdfPages : PdfDictionary
    {
        #region Private Fields

        private PdfArray kids;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Gets pages array
        /// </summary>
        public PdfArray Kids { get { return kids; } }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Initialize a new instance
        /// </summary>
        public PdfPages()
        {
            this["Type"] = new PdfName("Pages");
            this["Kids"] = kids = new PdfArray();
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Write this object to stream
        /// </summary>
        /// <param name="writer"></param>
        public override void Write(PdfWriter writer)
        {
            this["Count"] = new PdfNumeric(kids.Count);
            base.Write(writer);
        }

        #endregion Public Methods
    }
}