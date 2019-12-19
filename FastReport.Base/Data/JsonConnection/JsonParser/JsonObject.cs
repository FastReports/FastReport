using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace FastReport.Data.JsonConnection.JsonParser
{
    internal class JsonObject : JsonBase, IEnumerable<KeyValuePair<string, object>>
    {
        #region Private Fields

        private Dictionary<string, object> dict = new Dictionary<string, object>();

        #endregion Private Fields

        #region Public Indexers

        public override object this[string key]
        {
            get
            {
                object result;
                if (dict.TryGetValue(key, out result))
                    return result;
                return null;
            }
            set
            {
                dict[key] = value;
            }
        }

        #endregion Public Indexers

        #region Public Properties

        public override int Count
        {
            get
            {
                return dict.Count;
            }
        }

        public override bool IsObject
        {
            get
            {
                return true;
            }
        }

        public override IEnumerable<string> Keys
        {
            get
            {
                return dict.Keys;
            }
        }

        #endregion Public Properties

        #region Public Methods

        public override bool ContainsKey(string key)
        {
            return dict.ContainsKey(key);
        }

        public bool DeleteKey(string key)
        {
            return dict.Remove(key);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return dict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dict.GetEnumerator();
        }

        public override void WriteTo(StringBuilder sb, int indent)
        {
            sb.Append("{");
            bool notFirst = false;
            foreach (KeyValuePair<string, object> kv in dict)
            {
                if (notFirst)
                    sb.Append(",");
                if (indent > 0)
                {
                    sb.AppendLine();
                    for (int i = 0; i < indent; i++)
                        sb.Append(' ');
                }

                WriteValue(sb, kv.Key, indent);
                if (indent > 0)
                    sb.Append(": ");
                else
                    sb.Append(":");
                WriteValue(sb, kv.Value, indent);
                notFirst = true;
            }
            if (indent > 0 && notFirst)
            {
                sb.AppendLine();
                for (int i = 2; i < indent; i++)
                    sb.Append(' ');
            }
            sb.Append("}");
        }

        #endregion Public Methods
    }
}