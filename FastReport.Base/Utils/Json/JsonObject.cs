using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace FastReport.Utils.Json
{
    /// <summary>
    /// Represents JSON object.
    /// </summary>
    public class JsonObject : JsonBase, IEnumerable<KeyValuePair<string, object>>
    {
        #region Private Fields

        private readonly Dictionary<string, object> dict = new Dictionary<string, object>();

        #endregion Private Fields

        #region Public Indexers

        /// <summary>
        /// Gets or sets an object with specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The object.</returns>
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

        /// <summary>
        /// Gets the number of keys.
        /// </summary>
        public override int Count
        {
            get
            {
                return dict.Count;
            }
        }

        /// <summary>
        /// Returns true.
        /// </summary>
        public override bool IsObject
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the list of keys.
        /// </summary>
        public override IEnumerable<string> Keys
        {
            get
            {
                return dict.Keys;
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Determines whether the dictionary contains the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>true if the dictionary contains a specified key.</returns>
        public override bool ContainsKey(string key)
        {
            return dict.ContainsKey(key);
        }

        /// <summary>
        /// Deletes a specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>true if the key was found and removed succesfully.</returns>
        public bool DeleteKey(string key)
        {
            return dict.Remove(key);
        }

        /// <summary>
        /// Gets enumerator for this object.
        /// </summary>
        /// <returns>An enumerator.</returns>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return dict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dict.GetEnumerator();
        }

        /// <summary>
        /// Writes the content to specified string builder.
        /// </summary>
        /// <param name="sb">The string builder instance.</param>
        /// <param name="indent">The indent.</param>
        public override void WriteTo(StringBuilder sb, int indent)
        {
            sb.Append('{');
            bool notFirst = false;
            foreach (KeyValuePair<string, object> kv in dict)
            {
                if (notFirst)
                    sb.Append(',');
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
                    sb.Append(':');
                WriteValue(sb, kv.Value, indent);
                notFirst = true;
            }
            if (indent > 0 && notFirst)
            {
                sb.AppendLine();
                for (int i = 2; i < indent; i++)
                    sb.Append(' ');
            }
            sb.Append('}');
        }

        #endregion Public Methods
    }
}