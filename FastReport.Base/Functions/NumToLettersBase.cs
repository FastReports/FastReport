using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastReport.Functions
{
    internal abstract class NumToLettersBase
    {
        #region Protected Methods
        protected string Str(int value, char[] letters)
        {
            if (value < 0) return "";

            int n = value;
            StringBuilder r = new StringBuilder();

            //if (minus)
            //    r.Insert(0, GetMinus() + " ");
            int letter;
            while (n >= letters.Length)
            {
                letter = n % letters.Length;
                r.Insert(0, letters[letter]);
                n /= letters.Length;
                if (n < letters.Length) --n;
            }
            r.Insert(0, letters[n]);

            return r.ToString();
        }
        #endregion

        #region Public Methods
        public abstract string ConvertNumber(int value, bool isUpper);
        #endregion
    }
}
