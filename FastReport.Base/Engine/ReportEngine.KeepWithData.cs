using System.Collections.Generic;

namespace FastReport.Engine
{
    public partial class ReportEngine
    {
        #region Private Methods

        private void EnumHeaders(BandBase band, List<BandBase> list)
        {
            if (band != null)
            {
                list.Add(band);
                EnumHeaders(band.Parent as BandBase, list);
            }
        }

        // get a list of the footers that must be kept with the dataBand row
        private List<HeaderFooterBandBase> GetAllFooters(DataBand dataBand)
        {
            // get all parent databands/groups
            List<BandBase> list = new List<BandBase>();
            EnumHeaders(dataBand, list);

            // add report summary if required
            ReportSummaryBand summaryBand = (dataBand.Page as ReportPage).ReportSummary;
            if (dataBand.KeepSummary && summaryBand != null)
                list.Add(summaryBand);

            // make footers list
            List<HeaderFooterBandBase> footers = new List<HeaderFooterBandBase>();
            foreach (BandBase band in list)
            {
                HeaderFooterBandBase footer = null;
                if (band is DataBand)
                    footer = (band as DataBand).Footer;
                else if (band is GroupHeaderBand)
                    footer = (band as GroupHeaderBand).GroupFooter;
                else if (band is ReportSummaryBand)
                    footer = band as ReportSummaryBand;

                if (footer != null)
                    footers.Add(footer);

                // skip non-last data rows. Keep the dataBand to allow
                // calling this method from the beginning of RunDataBand
                if (band != dataBand && !band.IsLastRow)
                    break;
            }

            // remove all footers at the end which have no KeepWithData flag
            for (int i = footers.Count - 1; i >= 0; i--)
            {
                if (!footers[i].KeepWithData)
                    footers.RemoveAt(i);
                else
                    break;
            }

            return footers;
        }

        private bool NeedKeepFirstRow(DataBand dataBand)
        {
            return dataBand.Header != null && dataBand.Header.KeepWithData;
        }

        private bool NeedKeepFirstRow(GroupHeaderBand groupBand)
        {
            if (groupBand == null)
                return false;
            if (groupBand.KeepWithData)
                return true;
            DataBand dataBand = groupBand.GroupDataBand;
            if (dataBand.Header != null && dataBand.Header.KeepWithData)
                return true;

            if (groupBand.IsFirstRow)
                return NeedKeepFirstRow(groupBand.Parent as GroupHeaderBand);
            return false;
        }

        private bool NeedKeepLastRow(DataBand dataBand)
        {
            List<HeaderFooterBandBase> footers = GetAllFooters(dataBand);
            return footers.Count > 0;
        }

        private float GetFootersHeight(DataBand dataBand)
        {
            List<HeaderFooterBandBase> footers = GetAllFooters(dataBand);

            float height = 0;
            foreach (HeaderFooterBandBase band in footers)
            {
                // skip bands with RepeatOnEveryPage flag: its height is already
                // included in the FreeSpace property
                if (!band.RepeatOnEveryPage)
                    height += GetBandHeightWithChildren(band);
            }

            return height;
        }

        private void CheckKeepFooter(DataBand dataBand)
        {
            if (FreeSpace < GetFootersHeight(dataBand))
                EndColumn();
            else
                EndKeep();
        }

        #endregion Private Methods
    }
}
