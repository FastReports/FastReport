using FastReport.Export.PdfSimple.PdfObjects;
using System.Collections;
using System.Collections.Generic;

namespace FastReport.Export.PdfSimple.PdfCore
{
    /// <summary>
    /// The dictionary object of pdf
    /// </summary>
    public class PdfDictionary : PdfObjectBase, IDictionary<PdfName, PdfObjectBase>
    {
        #region Private Fields

        private readonly Dictionary<PdfName, PdfObjectBase> dictionary;

        #endregion Private Fields

        #region Public Indexers

        /// <inheritdoc/>
        public PdfObjectBase this[PdfName key]
        {
            get { return dictionary[key]; }
            set { dictionary[key] = value; }
        }

        /// <inheritdoc/>
        public PdfObjectBase this[string key]
        {
            get { return dictionary[new PdfName(key)]; }
            set { dictionary[new PdfName(key)] = value; }
        }

        #endregion Public Indexers

        #region Public Properties

        /// <inheritdoc/>
        public int Count
        {
            get
            {
                return dictionary.Count;
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

        /// <inheritdoc/>
        public ICollection<PdfName> Keys
        {
            get
            {
                return dictionary.Keys;
            }
        }

        /// <inheritdoc/>
        public ICollection<PdfObjectBase> Values
        {
            get
            {
                return dictionary.Values;
            }
        }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Initialize a new instance
        /// </summary>
        public PdfDictionary()
        {
            dictionary = new Dictionary<PdfName, PdfObjectBase>();
        }

        #endregion Public Constructors

        #region Public Methods

        /// <inheritdoc/>
        public void Add(PdfName key, PdfObjectBase value)
        {
            dictionary.Add(key, value);
        }

        /// <inheritdoc/>
        public void Add(KeyValuePair<PdfName, PdfObjectBase> item)
        {
            (dictionary as ICollection<KeyValuePair<PdfName, PdfObjectBase>>).Add(item);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            dictionary.Clear();
        }

        /// <inheritdoc/>
        public bool Contains(KeyValuePair<PdfName, PdfObjectBase> item)
        {
            return (dictionary as ICollection<KeyValuePair<PdfName, PdfObjectBase>>).Contains(item);
        }

        /// <inheritdoc/>
        public bool ContainsKey(PdfName key)
        {
            return dictionary.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<PdfName, PdfObjectBase>[] array, int arrayIndex)
        {
            (dictionary as ICollection<KeyValuePair<PdfName, PdfObjectBase>>).CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<PdfName, PdfObjectBase>> GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (dictionary as IEnumerable).GetEnumerator();
        }

        /// <inheritdoc/>
        public bool Remove(PdfName key)
        {
            return dictionary.Remove(key);
        }

        /// <inheritdoc/>
        public bool Remove(KeyValuePair<PdfName, PdfObjectBase> item)
        {
            return (dictionary as ICollection<KeyValuePair<PdfName, PdfObjectBase>>).Remove(item);
        }

        /// <inheritdoc/>
        public bool TryGetValue(PdfName key, out PdfObjectBase value)
        {
            return dictionary.TryGetValue(key, out value);
        }

        /// <inheritdoc/>
        public override void Write(PdfWriter writer)
        {
            writer.Write("<< ");
            foreach (KeyValuePair<PdfName, PdfObjectBase> keyValue in dictionary)
            {
                keyValue.Key.Write(writer);
                writer.Write(' ');
                keyValue.Value.Write(writer);
                writer.Write(' ');
            }
            writer.Write(">>");
        }

        #endregion Public Methods
    }
}