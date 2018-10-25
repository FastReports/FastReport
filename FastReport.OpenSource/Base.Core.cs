using System.Collections.Generic;

namespace FastReport
{
    partial class Base
    {
        /// <summary>
        /// Does nothing
        /// </summary>
        /// <param name="macroValues"></param>
        /// <param name="text"></param>
        private string ExtractDefaultMacrosInternal(Dictionary<string, object> macroValues, string text)
        {
            return text;
        }
    }
}
