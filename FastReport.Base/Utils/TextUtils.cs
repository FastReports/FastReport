using System.Collections;

namespace FastReport.Utils
{
    /// <summary>
    /// Contains text utility methods.
    /// </summary>
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

        /// <summary>
        /// Determines if substring of the text with given start and len parameters is a whole word.
        /// </summary>
        /// <param name="str">The text.</param>
        /// <param name="start">Start position.</param>
        /// <param name="len">Length.</param>
        /// <returns>true if substring is a whole word.</returns>
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