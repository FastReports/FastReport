using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastReport.Functions
{
    internal class NumToLettersRu : NumToLettersBase
    {
        #region Private Fields
        private static char[] letters =
        {
          'а', 'б', 'в', 'г', 'д', 'е', 'ё', 'ж', 'з', 'и', 'й', 'к', 'л', 'м', 'н', 'о', 'п',
          'р', 'с', 'т', 'у', 'ф', 'х', 'ц', 'ч', 'ш', 'щ', 'ъ', 'ы', 'ь', 'э', 'ю', 'я'
        };

        private static char[] upperLetters =
        {
          'А', 'Б', 'В', 'Г', 'Д', 'Е', 'Ё', 'Ж', 'З', 'И', 'Й', 'К', 'Л', 'М', 'Н', 'О', 'П',
          'Р', 'С', 'Т', 'У', 'Ф', 'Х', 'Ц', 'Ч', 'Ш', 'Щ', 'Ъ', 'Ы', 'Ь', 'Э', 'Ю', 'Я'
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
