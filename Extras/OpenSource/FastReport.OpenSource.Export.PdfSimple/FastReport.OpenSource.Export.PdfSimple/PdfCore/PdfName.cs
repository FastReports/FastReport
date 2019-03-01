using FastReport.Export.PdfSimple.PdfObjects;
using System;

namespace FastReport.Export.PdfSimple.PdfCore
{
    /// <summary>
    /// The name object for pdf like "/Type"
    /// </summary>
    public sealed class PdfName : PdfObjectBase, IEquatable<PdfName>
    {
        #region Private Fields

        private string name;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name
        {
            get { return name; }
            set
            {
                if (value == null)
                    name = "";
                else
                    name = value;
            }
        }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Initialize a new instance and set the name, e.g Type, Object etc.
        /// </summary>
        /// <param name="name"></param>
        public PdfName(string name)
        {
            this.Name = name;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <inheritdoc/>
        public bool Equals(PdfName other)
        {
            if (other == null)
                return false;
            return String.Equals(Name, other.Name);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return Equals(obj as PdfName);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        /// <inheritdoc/>
        public override void Write(PdfWriter writer)
        {
            if (String.IsNullOrEmpty(name))
                return;
            writer.Write("/");
            foreach (char c in name)
            {
                if ('a' <= c && c <= 'z' || 'A' <= c && c <= 'Z' || '0' <= c && c <= '9')
                {
                    writer.Write(c);
                }
                else
                {
                    writer.Write("#");
                    writer.Write(((byte)c).ToString("X2"));
                }
            }
        }

        #endregion Public Methods
    }
}