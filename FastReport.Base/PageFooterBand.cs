using System;
using System.Collections;
using System.ComponentModel;
using FastReport.Utils;

namespace FastReport
{
    /// <summary>
    /// Represents a page footer band.
    /// </summary>
    public class PageFooterBand : BandBase
    {
        #region Properties
        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false)]
        public new bool StartNewPage
        {
            get { return base.StartNewPage; }
            set { base.StartNewPage = value; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false)]
        public new bool PrintOnBottom
        {
            get { return base.PrintOnBottom; }
            set { base.PrintOnBottom = value; }
        }
        #endregion

        /// <inheritdoc/>
        public override void InitializeComponent()
        {
            base.InitializeComponent();
            // SubreportObject on a pagefooter will produce StackOverflow exception. Set PrintOnParent flag to avoid this
            foreach (ReportComponentBase obj in Objects)
            {
                if (obj is SubreportObject)
                    (obj as SubreportObject).PrintOnParent = true;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageFooterBand"/> class with default settings.
        /// </summary>
        public PageFooterBand()
        {
            FlagUseStartNewPage = false;
        }
    }
}