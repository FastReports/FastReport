using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FastReport.Utils
{
    public class TextUtils 
    {
        private static Hashtable delimiters = InitDelimiters(@";.,:'""{}[]()?!@#$%^&*-+=|\/<>".ToCharArray());
        private static Hashtable InitDelimiters(char[] delims)
        {
            Hashtable delimiters = new Hashtable();
            for (int i = 0; i <= 0x20; i++)
                delimiters[(char)i] = (char)i;
            if (delims != null)
                for (int i = 0; i < delims.Length; i++)
                    delimiters[delims[i]] = delims[i];

            return delimiters;
        }

        public static bool IsWholeWord(string str, int start, int len)
        {
            bool isDelim = start == 0 || start > 0 && (IsDelimiter(str, start - 1) || IsDelimiter(str, start));
            isDelim = isDelim && (start + len < str.Length - 1) && (start + len > 0) && (IsDelimiter(str, start + len) || IsDelimiter(str, start + len - 1));
            return isDelim;
        }

        private static bool IsDelimiter(string str, int index)
        {
            return delimiters.ContainsKey(str[index]);
        }
    }
}