using FastReport.Utils;

namespace FastReport
{
    partial class Report
    {
        #region Private Methods

        private void ClearPreparedPages()
        {
            if (preparedPages != null)
                preparedPages.Clear();
        }

        /// <summary>
        /// Does nothing
        /// </summary>
        /// <param name="password"></param>
        /// <returns>password</returns>
        private string ShowPaswordForm(string password)
        {
            return password;
        }

        /// <summary>
        /// Does nothing
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="report"></param>
        partial void SerializeDesign(FRWriter writer, Report report);

        /// <summary>
        /// Does nothing
        /// </summary>
        partial void InitDesign();

        /// <summary>
        /// Does nothing
        /// </summary>
        partial void ClearDesign();

        /// <summary>
        /// Does nothing
        /// </summary>
        partial void DisposeDesign();

        #endregion Private Methods
    }
}