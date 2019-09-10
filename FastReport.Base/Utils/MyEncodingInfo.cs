using System;
using System.Collections.Generic;
using System.Text;

namespace FastReport.Utils
{
    internal class MyEncodingInfo
    {
        #region Private Fields

        private int codePage;
        private string displayName;
        private string name;

        #endregion Private Fields

        #region Public Properties

        public int CodePage
        {
            get
            {
                return codePage;
            }
            set
            {
                codePage = value;
            }
        }

        public string DisplayName
        {
            get { return displayName; }
            set
            {
                displayName = value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        #endregion Public Properties

        #region Public Constructors

        public MyEncodingInfo(Encoding encoding)
        {
            DisplayName = encoding.EncodingName;
            Name = encoding.WebName;
            CodePage = encoding.CodePage;
        }

        public MyEncodingInfo(EncodingInfo info)
        {
            DisplayName = info.DisplayName;
            Name = info.Name;
            CodePage = info.CodePage;
        }

        #endregion Public Constructors

        #region Public Methods

        public static IEnumerable<MyEncodingInfo> GetEncodings()
        {
            List<MyEncodingInfo> encodings = new List<MyEncodingInfo>();

            foreach (EncodingInfo info in Encoding.GetEncodings())
            {
                encodings.Add(new MyEncodingInfo(info));
            }

            encodings.Sort(new Comparison<MyEncodingInfo>(compareEncoding)); ;

            return encodings;
        }

        public override string ToString()
        {
            return DisplayName;
        }

        #endregion Public Methods

        #region Private Methods

        private static int compareEncoding(MyEncodingInfo x, MyEncodingInfo y)
        {
            if (x != null && y != null)
            {
                return String.Compare(x.DisplayName, y.DisplayName);
            }
            return 0;
        }

        #endregion Private Methods
    }
}