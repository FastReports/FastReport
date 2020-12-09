using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastReport.Functions
{
    internal class NumToLettersEn : NumToLettersBase
    {
        #region Private Fields
        private static char[] letters =
        {
          'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
          'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
        };

        private static char[] upperLetters =
        {
          'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
          'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
        };
        #endregion

        #region Public Methods
        public override string ConvertNumber(int value, bool isUpper)
        {
            return Str(value, (isUpper) ? (upperLetters) : (letters));
        }
        #endregion
    }
}
