using FastReport.Data.JsonConnection.JsonParser;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FastReport
{
    /// <summary>
    /// Allows working with JsonObject
    /// </summary>
    public abstract class JsonBase
    {
        #region Private Fields

        private static NumberFormatInfo format;

        #endregion Private Fields

        #region Public Indexers

        /// <summary>
        /// Returns child object for JsonArray
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual object this[int index]
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        /// <summary>
        /// Returns child object for JsonObject
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual object this[string key]
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        #endregion Public Indexers

        #region Public Properties

        /// <summary>
        /// Returns count of child object
        /// </summary>
        public virtual int Count
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Returns true if this object is JsonArray
        /// </summary>
        public virtual bool IsArray
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Returns true if this object is JsonObject
        /// </summary>
        public virtual bool IsObject
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Returns list of JsonObject keys
        /// </summary>
        public virtual IEnumerable<string> Keys
        {
            get
            {
                yield break;
            }
        }

        #endregion Public Properties

        #region Public Constructors

        static JsonBase()
        {
            format = new NumberFormatInfo();
            format.NumberGroupSeparator = String.Empty;
            format.NumberDecimalSeparator = ".";
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Pars json text string and return a new JsonBase Object
        /// </summary>
        /// <param name="jsonText"></param>
        /// <returns></returns>
        public static JsonBase FromString(string jsonText)
        {
            using (JsonTextReader reader = new JsonTextReader(jsonText))
            {
                return FromTextReader(reader);
            }
        }

        /// <summary>
        /// returns true
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual bool ContainsKey(string key)
        {
            return false;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            WriteTo(sb, 0);
            return sb.ToString();
        }

        /// <summary>
        /// Serialize this object to sb
        /// </summary>
        /// 
        /// <param name="sb"></param>
        /// <param name="indent">indent in space, 0 = without indent</param>
        public abstract void WriteTo(StringBuilder sb, int indent);

        #endregion Public Methods

        #region Internal Methods

        internal string ReadString(string key)
        {
            object result = this[key];
            if (result != null)
            {
                return result.ToString();
            }
            return null;
        }

        internal void WriteValue(StringBuilder sb, object item, int indent)
        {
            if (item is JsonBase)
            {
                if (indent > 0)
                {
                    (item as JsonBase).WriteTo(sb, indent + 2);
                }
                else
                {
                    (item as JsonBase).WriteTo(sb, 0);
                }
            }
            else if (item is bool)
            {
                if (item.Equals(true))
                {
                    sb.Append("true");
                }
                else
                {
                    sb.Append("false");
                }
            }
            else if (IsNumber(sb, item))
            {
                sb.Append(((IConvertible)item).ToString(format));
            }
            else if (item == null)
            {
                sb.Append("null");
            }
            else
            {
                sb.Append('"');

                foreach (char c in item.ToString())
                {
                    switch (c)
                    {
                        case '"': sb.Append("\\\""); break;
                        case '\\': sb.Append("\\\\"); break;
                        case '/': sb.Append("\\/"); break;
                        case '\b': sb.Append("\\b"); break;
                        case '\f': sb.Append("\\f"); break;
                        case '\n': sb.Append("\\n"); break;
                        case '\r': sb.Append("\\r"); break;
                        case '\t': sb.Append("\\t"); break;
                        default: sb.Append(c); break;
                    }
                }
                sb.Append('"');
            }
        }

        #endregion Internal Methods

        #region Private Methods

        private static JsonBase FromTextReader(JsonTextReader reader)
        {
            reader.SkipWhiteSpace();
            if (reader.IsNotEOF)
                switch (reader.Char)
                {
                    case '{':
                        return ReadObject(reader);

                    case '[':
                        return ReadArray(reader);

                    default:
                        throw reader.ThrowFormat('{', '[');
                }
            throw reader.ThrowEOF('{', '[');
        }

        private static JsonArray ReadArray(JsonTextReader reader)
        {
            if (reader.Char != '[')
                throw reader.ThrowFormat('[');

            JsonArray result = new JsonArray();

            reader.ReadNext();
            reader.SkipWhiteSpace();

            if (reader.IsEOF)
                throw reader.ThrowEOF(']');
            else if (reader.Char != ']')
            {
                while (true)
                {
                    result.Add(ReadValue(reader));
                    reader.SkipWhiteSpace();

                    if (reader.IsEOF)
                        throw reader.ThrowEOF(']');
                    else if (reader.Char == ',')
                    {
                        reader.ReadNext();
                        reader.SkipWhiteSpace();
                        continue;
                    }
                    else if (reader.Char == ']')
                        break;
                    else reader.ThrowFormat(',', ']');
                }
            }

            reader.ReadNext();

            return result;
        }

        private static JsonObject ReadObject(JsonTextReader reader)
        {
            if (reader.Char != '{')
                throw reader.ThrowFormat('{');

            JsonObject result = new JsonObject();

            reader.ReadNext();
            reader.SkipWhiteSpace();

            if (reader.IsEOF)
                throw reader.ThrowEOF('}');
            else if (reader.Char != '}')
            {
                while (true)
                {
                    string key = reader.Dedublicate(ReadValueString(reader));

                    reader.SkipWhiteSpace();

                    if (reader.IsEOF)
                        throw reader.ThrowEOF(':');
                    else if (reader.Char != ':')
                        reader.ThrowFormat(':');

                    reader.ReadNext();
                    reader.SkipWhiteSpace();

                    result[key] = ReadValue(reader);
                    reader.SkipWhiteSpace();

                    if (reader.IsEOF)
                        throw reader.ThrowEOF('}');
                    else if (reader.Char == ',')
                    {
                        reader.ReadNext();
                        reader.SkipWhiteSpace();
                        continue;
                    }
                    else if (reader.Char == '}')
                        break;
                    else reader.ThrowFormat(',', '}');
                }
            }

            reader.ReadNext();

            return result;
        }

        private static object ReadValue(JsonTextReader reader)
        {
            if (reader.IsEOF)
                throw reader.ThrowEOF('"', '[', '{', 'n', 't', 'f', '-', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
            switch (reader.Char)
            {
                case '"':
                    return ReadValueString(reader);

                case '[':
                    return ReadArray(reader);

                case '{':
                    return ReadObject(reader);

                case 'n':
                    return ReadValue(reader, "null", null);

                case 't':
                    return ReadValue(reader, "true", true);

                case 'f':
                    return ReadValue(reader, "false", false);

                case '-':
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return ReadValueNumber(reader);

                default:
                    throw reader.ThrowFormat('"', '[', '{', 'n', 't', 'f', '-', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
            }
        }

        private static object ReadValue(JsonTextReader reader, string str, object result)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (reader.IsEOF)
                    throw reader.ThrowEOF(str[i]);
                else if (reader.Char != str[i])
                    throw reader.ThrowFormat(str[i]);
                reader.ReadNext();
            }
            return result;
        }

        private static double ReadValueNumber(JsonTextReader reader)
        {
            int startPos = reader.Position;
            if (reader.IsEOF)
                throw reader.ThrowEOF('-', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9');

            if (reader.Char == '-')
            {
                reader.ReadNext();
                if (reader.IsEOF)
                    throw reader.ThrowEOF('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
            }

            if (reader.Char < '0' || '9' < reader.Char)
                throw reader.ThrowFormat('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');

            while (true)
            {
                reader.ReadNext();
                if (reader.IsEOF)
                    throw reader.ThrowEOF('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');

                if (reader.Char < '0' || '9' < reader.Char)
                    break;
            }

            if (reader.Char == '.')
            {
                reader.ReadNext();
                if (reader.IsEOF)
                    throw reader.ThrowEOF('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');

                if (reader.Char < '0' || '9' < reader.Char)
                    throw reader.ThrowFormat('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');

                while (true)
                {
                    reader.ReadNext();
                    if (reader.IsEOF)
                        throw reader.ThrowEOF('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');

                    if (reader.Char < '0' || '9' < reader.Char)
                        break;
                }
            }

            if (reader.Char == 'e' || reader.Char == 'E')
            {
                reader.ReadNext();
                if (reader.IsEOF)
                    throw reader.ThrowEOF('+', '-', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9');

                bool signed = false;
                if (reader.Char == '+')
                {
                    reader.ReadNext();
                    signed = true;
                    if (reader.IsEOF)
                        throw reader.ThrowEOF('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
                }
                else if (reader.Char == '-')
                {
                    reader.ReadNext();
                    signed = true;
                    if (reader.IsEOF)
                        throw reader.ThrowEOF('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
                }

                if (reader.Char < '0' || '9' < reader.Char)
                {
                    if (signed)
                        throw reader.ThrowFormat('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
                    else
                        throw reader.ThrowFormat('-', '+', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
                }

                while (true)
                {
                    reader.ReadNext();
                    if (reader.IsEOF)
                        throw reader.ThrowEOF('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');

                    if (reader.Char < '0' || '9' < reader.Char)
                        break;
                }
            }

            string value = reader.Substring(startPos, reader.Position - startPos);

            return double.Parse(value, format);
        }

        private static string ReadValueString(JsonTextReader reader)
        {
            if (reader.IsEOF)
                throw reader.ThrowEOF('"');
            else if (reader.Char != '"')
                throw reader.ThrowFormat('"');

            reader.ReadNext();

            StringBuilder sb = new StringBuilder();
            while (true)
            {
                if (reader.IsEOF)
                    throw reader.ThrowEOF('"');

                if (reader.Char == '"')
                    break;
                else if (reader.Char == '\\')
                {
                    reader.ReadNext();
                    if (reader.IsEOF)
                        throw reader.ThrowEOF('"', '\\', '/', 'b', 'f', 'n', 'r', 't', 'u');
                    switch (reader.Char)
                    {
                        case '"': sb.Append('"'); break;
                        case '\\': sb.Append('\\'); break;
                        case '/': sb.Append('/'); break;
                        case 'b': sb.Append('\b'); break;
                        case 'f': sb.Append('\f'); break;
                        case 'n': sb.Append('\n'); break;
                        case 'r': sb.Append('\r'); break;
                        case 't': sb.Append('\t'); break;
                        case 'u':
                            int number = 0;
                            for (int i = 0; i < 4; i++)
                            {
                                reader.ReadNext();
                                if (reader.IsEOF)
                                    throw reader.ThrowEOF('0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'A', 'B', 'C', 'D', 'E', 'F');
                                if ('0' <= reader.Char && reader.Char <= '9')
                                {
                                    number = number * 0x10 + (int)(reader.Char - '0');
                                }
                                else if ('a' <= reader.Char && reader.Char <= 'f')
                                {
                                    number = number * 0x10 + 10 + (int)(reader.Char - 'a');
                                }
                                else if ('A' <= reader.Char && reader.Char <= 'F')
                                {
                                    number = number * 0x10 + 10 + (int)(reader.Char - 'A');
                                }
                                else
                                {
                                    throw reader.ThrowFormat('0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'A', 'B', 'C', 'D', 'E', 'F');
                                }
                            }

                            sb.Append((char)number);

                            break;

                        default:
                            throw reader.ThrowFormat('"', '\\', '/', 'b', 'f', 'n', 'r', 't', 'u');
                    }
                }
                else
                {
                    sb.Append(reader.Char);
                }

                reader.ReadNext();
            }

            reader.ReadNext();

            return sb.ToString();
        }

        private bool IsNumber(StringBuilder sb, object item)
        {
            return item is float
                || item is double
                || item is sbyte
                || item is byte
                || item is short
                || item is ushort
                || item is int
                || item is uint
                || item is long
                || item is ulong
                || item is decimal;
        }

        #endregion Private Methods
    }
}