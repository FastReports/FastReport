using FastReport.Export.PdfSimple.PdfCore;
using System;

namespace FastReport.Export.PdfSimple.PdfObjects
{
    /// <summary>
    /// The info dictionary of pdf file
    /// </summary>
    public class PdfInfo : PdfDictionary
    {
        #region Public Properties

        /// <summary>
        /// Author of the document.
        /// </summary>
        public string Author
        {
            set
            {
                PdfName key = new PdfName("Author");
                if (String.IsNullOrEmpty(value))
                {
                    this.Remove(key);
                }
                else
                {
                    this[key] = new PdfString(value);
                }
            }
        }

        /// <summary>
        /// Keywords of the document.
        /// </summary>
        public string Keywords
        {
            set
            {
                PdfName key = new PdfName("Keywords");
                if (String.IsNullOrEmpty(value))
                {
                    this.Remove(key);
                }
                else
                {
                    this[key] = new PdfString(value);
                }
            }
        }

        /// <summary>
        /// Subject of the document.
        /// </summary>
        public string Subject
        {
            set
            {
                PdfName key = new PdfName("Subject");
                if (String.IsNullOrEmpty(value))
                {
                    this.Remove(key);
                }
                else
                {
                    this[key] = new PdfString(value);
                }
            }
        }

        /// <summary>
        /// Title of the document.
        /// </summary>
        public string Title
        {
            set
            {
                PdfName key = new PdfName("Title");
                if (String.IsNullOrEmpty(value))
                {
                    this.Remove(key);
                }
                else
                {
                    this[key] = new PdfString(value);
                }
            }
        }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Initialize a new instance
        /// </summary>
        public PdfInfo()
        {
            this["Creator"] = new PdfString("FastReport.NET", true);
            this["Producer"] = new PdfString("FastReport.NET", true);
        }

        #endregion Public Constructors

        #region Public Methods

        /// <inheritdoc/>
        public override void Write(PdfWriter writer)
        {
            base.Write(writer);
        }

        #endregion Public Methods
    }
}