using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace FastReport.Data.JsonConnection.JsonParser
{
    internal class JsonArray : JsonBase, IEnumerable<object>
    {
        #region Private Fields

        private List<object> array = new List<object>();

        #endregion Private Fields

        #region Public Indexers

        public override object this[int index]
        {
            get
            {
                if (index >= 0 && index < Count)
                    return array[index];
                return null;
            }

            set
            {
                array[index] = value;
            }
        }

        #endregion Public Indexers

        #region Public Properties

        public override int Count
        {
            get
            {
                return array.Count;
            }
        }

        public override bool IsArray
        {
            get
            {
                return true;
            }
        }

        #endregion Public Properties

        #region Public Methods

        public void Add(object obj)
        {
            array.Add(obj);
        }

        public IEnumerator<object> GetEnumerator()
        {
            return array.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return array.GetEnumerator();
        }

        public void Insert(int index, object obj)
        {
            array.Insert(index, obj);
        }

        public void Remove(int index)
        {
            array.RemoveAt(index);
        }

        public override void WriteTo(StringBuilder sb, int indent)
        {
            sb.Append("[");

            bool notFirst = false;
            foreach (object item in array)
            {
                if (notFirst)
                    sb.Append(",");
                if (indent > 0)
                {
                    sb.AppendLine();
                    for (int i = 0; i < indent; i++)
                        sb.Append(' ');
                }
                WriteValue(sb, item, indent);
                notFirst = true;
            }
            if (indent > 0 && notFirst)
            {
                sb.AppendLine();
                for (int i = 2; i < indent; i++)
                    sb.Append(' ');
            }
            sb.Append("]");
        }

        #endregion Public Methods
    }
}