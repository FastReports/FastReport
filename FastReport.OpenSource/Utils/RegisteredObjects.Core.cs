using System.Drawing;

namespace FastReport.Utils
{
    partial class ObjectInfo
    {
        #region Private Methods

        partial void UpdateDesign(Bitmap image, int imageIndex, int buttonIndex = -1);

        /// <summary>
        /// Does nothing.
        /// </summary>
        partial void UpdateDesign(int flags, bool multiInsert, Bitmap image, int imageIndex, int buttonIndex = -1);

        #endregion Private Methods
    }
}
