using FastReport.Export.PdfSimple.PdfObjects;

namespace FastReport.Export.PdfSimple.PdfCore
{
    /// <summary>
    /// The boolean object of pdf can be true or false
    /// </summary>
    public class PdfBoolean : PdfObjectBase
    {
        #region Private Fields

        private bool value;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// The value of boolean
        /// </summary>
        public bool Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Initialize a new object and set the value
        /// </summary>
        /// <param name="value">true or false</param>
        public PdfBoolean(bool value)
        {
            this.value = value;
        }

        /// <summary>
        /// Initialize a new object
        /// </summary>
        public PdfBoolean()
        {
        }

        #endregion Public Constructors

        #region Public Methods

        /// <inheritdoc/>
        public override void Write(PdfWriter writer)
        {
            if (Value)
            {
                writer.Write("true");
            }
            else
            {
                writer.Write("false");
            }
        }

        #endregion Public Methods
    }
}