using FastReport.Export.PdfSimple.PdfObjects;

namespace FastReport.Export.PdfSimple.PdfCore
{
    /// <summary>
    /// the direct object for pdf like 12 0 obj endobj
    /// </summary>
    public class PdfDirectObject
    {
        #region Private Fields

        private PdfObjectBase contentForWriting;
        private long number;
        private long offset;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// have to write this object
        /// </summary>
        public bool HaveToWrite
        {
            get
            {
                return contentForWriting != null;
            }
        }

        /// <summary>
        /// offset of object in stream
        /// </summary>
        public long Offset
        {
            get { return offset; }
        }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// initialize a new instance and set object number
        /// </summary>
        /// <param name="number"></param>
        public PdfDirectObject(long number)
        {
            this.number = number;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Prepare objectBase to writing later
        /// </summary>
        /// <param name="objectBase"></param>
        public void Prepare(PdfObjectBase objectBase)
        {
            contentForWriting = objectBase;
        }

        /// <summary>
        /// write this object to writer
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="content"></param>
        /// <param name="position"></param>
        public void Write(PdfWriter writer, PdfObjectBase content, long position)
        {
            offset = position;
            writer.Write(number);
            writer.WriteLn(" 0 obj");
            WriteInternal(content, writer);
            writer.WriteLn("");
            writer.WriteLn("endobj");
        }

        /// <summary>
        /// Write prepared object
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="position"></param>
        public void Write(PdfWriter writer, long position)
        {
            if (contentForWriting != null)
            {
                Write(writer, contentForWriting, position);
                contentForWriting = null;
            }
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// function that write the content
        /// </summary>
        /// <param name="content"></param>
        /// <param name="writer"></param>
        protected virtual void WriteInternal(PdfObjectBase content, PdfWriter writer)
        {
            if (content != null)
                content.Write(writer);
        }

        #endregion Protected Methods
    }
}