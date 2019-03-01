using FastReport.Export.PdfSimple.PdfObjects;

namespace FastReport.Export.PdfSimple.PdfCore
{
    /// <summary>
    /// The inderect object for pdf like "12 0 R"
    /// </summary>
    public class PdfIndirectObject : PdfObjectBase
    {
        #region Private Fields

        private long number;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// The number of direct object
        /// </summary>
        public long Number { get { return number; } set { number = value; } }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Initialize a new object with number
        /// </summary>
        /// <param name="number"></param>
        public PdfIndirectObject(long number)
        {
            this.number = number;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <inheritdoc />
        public override void Write(PdfWriter writer)
        {
            writer.Write(number);
            writer.Write(" 0 R");
        }

        #endregion Public Methods
    }
}