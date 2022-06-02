using System.Collections.Generic;

namespace FastReport.Export
{
    partial class ExportBase
    {
        #region Private Methods

        /// <summary>
        /// Does nothing
        /// </summary>
        /// <param name="int0"></param>
        partial void ShowPerformance(int int0);

        protected ReportPage GetOverlayPage(ReportPage page)
        {
            return page;
        }

        private int GetPagesCount(List<int> pages)
        {
            return pages.Count;
        }

        internal const bool HAVE_TO_WORK_WITH_OVERLAY = false;

        #endregion Private Methods
    }
}