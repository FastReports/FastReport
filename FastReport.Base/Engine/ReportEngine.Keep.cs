using FastReport.Utils;

namespace FastReport.Engine
{
    public partial class ReportEngine
    {
        #region Fields

        private bool keeping;
        private int keepPosition;
        private XmlItem keepOutline;
        private int keepBookmarks;
        private float keepCurX;
        private float keepCurY;
        private float keepDeltaY;

        #endregion Fields

        #region Properties
        /// <summary>
        /// Returns true of keeping is enabled
        /// </summary>
        public bool IsKeeping
        {
            get { return keeping; }
        }
        /// <summary>
        /// Returns keeping position
        /// </summary>
        public float KeepCurY
        {
            get { return keepCurY; }
        }
        #endregion Properties

        #region Private Methods

        private void StartKeep(BandBase band)
        {
            // do not keep the first row on a page, avoid empty first page
            if (keeping || (band != null && band.AbsRowNo == 1 && !band.FirstRowStartsNewPage))
                return;
            keeping = true;

            keepPosition = PreparedPages.CurPosition;
            keepOutline = PreparedPages.Outline.CurPosition;
            keepBookmarks = PreparedPages.Bookmarks.CurPosition;
            keepCurY = CurY;
            Report.Dictionary.Totals.StartKeep();
            StartKeepReprint();
        }

        private void CutObjects()
        {
            keepCurX = CurX;
            keepDeltaY = CurY - keepCurY;
            PreparedPages.CutObjects(keepPosition);
            CurY = keepCurY;
        }

        private void PasteObjects()
        {
            PreparedPages.PasteObjects(CurX - keepCurX, CurY - keepCurY);
            PreparedPages.Outline.Shift(keepOutline, CurY);
            PreparedPages.Bookmarks.Shift(keepBookmarks, CurY);
            EndKeep();
            CurY += keepDeltaY;
        }

        #endregion Private Methods

        #region Public Methods

        /// <summary>
        /// Starts the keep mechanism.
        /// </summary>
        /// <remarks>
        /// Use this method along with the <see cref="EndKeep"/> method if you want to keep
        /// several bands together. Call <b>StartKeep</b> method before printing the first band
        /// you want to keep, then call the <b>EndKeep</b> method after printing the last band you want to keep.
        /// </remarks>
        public void StartKeep()
        {
            StartKeep(null);
        }

        /// <summary>
        /// Ends the keep mechanism.
        /// </summary>
        /// <remarks>
        /// Use this method along with the <see cref="StartKeep()"/> method if you want to keep
        /// several bands together. Call <b>StartKeep</b> method before printing the first band
        /// you want to keep, then call the <b>EndKeep</b> method after printing the last band you want to keep.
        /// </remarks>
        public void EndKeep()
        {
            if (keeping)
            {
                Report.Dictionary.Totals.EndKeep();
                EndKeepReprint();
                keeping = false;
            }
        }

        #endregion Public Methods
    }
}
