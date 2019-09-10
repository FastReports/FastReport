using System;
using System.Collections.Generic;
using System.Text;

namespace FastReport.Data.JsonConnection.JsonParser
{
    internal class JsonTextReader : IDisposable
    {
        #region Private Fields

        private string jsonText;
        private Dictionary<string, string> pool = new Dictionary<string, string>();
        private int position;
        private bool stringOptimization;

        #endregion Private Fields

        #region Public Properties

        public char Char
        {
            get
            {
                return jsonText[position];
            }
        }

        public bool IsEOF
        {
            get
            {
                return position >= jsonText.Length;
            }
        }

        public bool IsNotEOF
        {
            get
            {
                return position < jsonText.Length;
            }
        }

        public string JsonText
        {
            get { return jsonText; }
            set { jsonText = value; }
        }

        public int Position
        {
            get { return position; }
            set { position = value; }
        }

        #endregion Public Properties

        #region Public Constructors

        public JsonTextReader(string jsonText)
        {
            stringOptimization = Utils.Config.IsStringOptimization;

            this.jsonText = jsonText;
            position = 0;
        }

        #endregion Public Constructors

        #region Public Methods

        public string Dedublicate(string value)
        {
            if (stringOptimization)
            {
                string result;
                if (pool.TryGetValue(value, out result))
                    return result;
                return pool[value] = value;
            }
            return value;
        }

        public void Dispose()
        {
            JsonText = null;
            pool.Clear();
            pool = null;
        }

        public void ReadNext()
        {
            position++;
        }

        public void SkipWhiteSpace()
        {
            while (IsNotEOF && char.IsWhiteSpace(Char))
                position++;
        }

        public Exception ThrowEOF(params char[] args)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Unexpected end of input json, wait for");
            if (args.Length > 0)
            {
                sb.Append(" ").Append(args[0]);
            }
            for (int i = 1; i < args.Length; i++)
            {
                sb.Append(", ").Append(args[i]);
            }
            return new FormatException(sb.ToString());
        }

        public Exception ThrowEOF(string args)
        {
            return new FormatException("Unexpected end of input json, wait for " + args);
        }

        public Exception ThrowFormat(params char[] args)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Json text at position ").Append(Position).Append(", unexpected symbol ").Append(Char).Append(", wait for");
            if (args.Length > 0)
            {
                sb.Append(" ").Append(args[0]);
            }
            for (int i = 1; i < args.Length; i++)
            {
                sb.Append(", ").Append(args[i]);
            }
            return new FormatException(sb.ToString());
        }

        #endregion Public Methods

        #region Internal Methods

        internal string Substring(int startPos, int len)
        {
            return JsonText.Substring(startPos, len);
        }

        #endregion Internal Methods
    }
}