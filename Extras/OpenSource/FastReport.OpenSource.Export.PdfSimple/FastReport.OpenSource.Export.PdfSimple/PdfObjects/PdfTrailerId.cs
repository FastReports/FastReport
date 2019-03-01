using FastReport.Export.PdfSimple.PdfCore;
using System;

namespace FastReport.Export.PdfSimple.PdfObjects
{
    /// <summary>
    /// Id object of trailer of pdf file
    /// </summary>
    public class PdfTrailerId : PdfArray
    {
        #region Public Constructors

        /// <summary>
        /// Initialize a new ID with random value
        /// </summary>
        public PdfTrailerId()
        {
            string fileID = Guid.NewGuid().ToString().Replace("-", "");
            Add(new PdfGUID(fileID));
            Add(new PdfGUID(fileID));
        }

        #endregion Public Constructors

        #region Private Classes

        private class PdfGUID : PdfObjectBase
        {
            #region Private Fields

            private readonly string guid;

            #endregion Private Fields

            #region Public Constructors

            public PdfGUID(string guid)
            {
                this.guid = guid;
            }

            #endregion Public Constructors

            #region Public Methods

            public override void Write(PdfWriter writer)
            {
                writer.Write("<");
                writer.Write(guid);
                writer.Write(">");
            }

            #endregion Public Methods
        }

        #endregion Private Classes
    }
}