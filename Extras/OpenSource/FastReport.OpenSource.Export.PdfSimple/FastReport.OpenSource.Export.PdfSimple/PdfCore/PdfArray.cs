using FastReport.Export.PdfSimple.PdfObjects;
using System.Collections;
using System.Collections.Generic;

namespace FastReport.Export.PdfSimple.PdfCore
{
    /// <summary>
    /// The array for pdf
    /// </summary>
    public class PdfArray : PdfObjectBase, ICollection<PdfObjectBase>
    {
        #region Private Fields

        private readonly List<PdfObjectBase> objects;

        #endregion Private Fields

        #region Public Indexers

        /// <summary>
        /// Gets or sets array value
        /// </summary>
        /// <param name="index">index of element</param>
        /// <returns>value or throw</returns>
        public PdfObjectBase this[int index]
        {
            get
            {
                return objects[index];
            }
            set
            {
                objects[index] = value;
            }
        }

        #endregion Public Indexers

        #region Public Properties

        /// <inheritdoc/>
        public int Count
        {
            get
            {
                return objects.Count;
            }
        }

        /// <inheritdoc/>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Initialize a new instance
        /// </summary>
        public PdfArray()
        {
            objects = new List<PdfObjectBase>();
        }

        /// <summary>
        /// Initialize a new instance, and set value of array
        /// </summary>
        public PdfArray(IEnumerable<PdfObjectBase> baseCollection)

        {
            objects = new List<PdfObjectBase>(baseCollection);
        }

        #endregion Public Constructors

        #region Public Methods

        /// <inheritdoc/>
        public void Add(PdfObjectBase item)
        {
            objects.Add(item);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            objects.Clear();
        }

        /// <inheritdoc/>
        public bool Contains(PdfObjectBase item)
        {
            return objects.Contains(item);
        }

        /// <inheritdoc/>
        public void CopyTo(PdfObjectBase[] array, int arrayIndex)
        {
            objects.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        public IEnumerator<PdfObjectBase> GetEnumerator()
        {
            return objects.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (objects as IEnumerable).GetEnumerator();
        }

        /// <inheritdoc/>
        public bool Remove(PdfObjectBase item)
        {
            return objects.Remove(item);
        }

        /// <inheritdoc/>
        public override void Write(PdfWriter writer)
        {
            writer.Write("[");
            writer.Write(' ');
            foreach (PdfObjectBase obj in objects)
            {
                obj.Write(writer);
                writer.Write(' ');
            }
            writer.Write("]");
        }

        #endregion Public Methods
    }
}