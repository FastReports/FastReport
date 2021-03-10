using System.Drawing;

namespace FastReport.Utils
{
    partial class ObjectInfo
    {
        #region Private Methods

        /// <summary>
        /// Does nothing
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="image"></param>
        /// <param name="imageIndex"></param>
        /// <param name="text"></param>
        /// <param name="flags"></param>
        /// <param name="multiInsert"></param>
        partial void UpdateDesign(object obj, Bitmap image, int imageIndex, string text, int flags, bool multiInsert);

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="image"></param>
        /// <param name="imageIndex"></param>
        /// <param name="ButtonIndex"></param>
        /// <param name="text"></param>
        /// <param name="flags"></param>
        /// <param name="multiInsert"></param>
        partial void UpdateDesign(object obj, Bitmap image, int imageIndex, int ButtonIndex, string text, int flags, bool multiInsert);

        #endregion Private Methods
    }
}
