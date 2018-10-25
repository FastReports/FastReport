using FastReport.Utils;
using System.Drawing;

namespace FastReport
{
    partial class StyleBase
    {
        #region Private Methods

        private Font GetDefaultFontInternal()
        {
            return DrawUtils.DefaultFont;
        }

        #endregion Private Methods
    }
}