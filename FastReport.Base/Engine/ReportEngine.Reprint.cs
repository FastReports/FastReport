using System.Collections.Generic;

namespace FastReport.Engine
{
    public partial class ReportEngine
    {
        #region Fields

        private List<BandBase> reprintHeaders;
        private List<BandBase> keepReprintHeaders;
        private List<BandBase> reprintFooters;
        private List<BandBase> keepReprintFooters;

        #endregion Fields

        #region Private Methods

        private void InitReprint()
        {
            reprintHeaders = new List<BandBase>();
            keepReprintHeaders = new List<BandBase>();
            reprintFooters = new List<BandBase>();
            keepReprintFooters = new List<BandBase>();
        }

        private void ShowReprintHeaders()
        {
            float saveOriginX = originX;

            foreach (BandBase band in reprintHeaders)
            {
                band.Repeated = true;
                originX = band.ReprintOffset;
                ShowBand(band);
                band.Repeated = false;
            }

            originX = saveOriginX;
        }

        private void ShowReprintFooters()
        {
            ShowReprintFooters(true);
        }

        private void ShowReprintFooters(bool repeated)
        {
            float saveOriginX = originX;

            // show footers in reverse order
            for (int i = reprintFooters.Count - 1; i >= 0; i--)
            {
                BandBase band = reprintFooters[i];
                band.Repeated = repeated;
                band.FlagCheckFreeSpace = false;
                originX = band.ReprintOffset;
                ShowBand(band);
                band.Repeated = false;
                band.FlagCheckFreeSpace = true;
            }

            originX = saveOriginX;
        }

        private void AddReprint(BandBase band)
        {
            // save current offset and use it later when reprinting a band.
            // it is required when printing subreports
            band.ReprintOffset = originX;

            if (keeping)
            {
                if (band is DataHeaderBand || band is GroupHeaderBand)
                    keepReprintHeaders.Add(band);
                else
                    keepReprintFooters.Add(band);
            }
            else
            {
                if (band is DataHeaderBand || band is GroupHeaderBand)
                    reprintHeaders.Add(band);
                else
                    reprintFooters.Add(band);
            }
        }

        private void RemoveReprint(BandBase band)
        {
            if (keepReprintHeaders.Contains(band))
                keepReprintHeaders.Remove(band);
            if (reprintHeaders.Contains(band))
                reprintHeaders.Remove(band);
            if (keepReprintFooters.Contains(band))
                keepReprintFooters.Remove(band);
            if (reprintFooters.Contains(band))
                reprintFooters.Remove(band);
        }

        private void StartKeepReprint()
        {
            keepReprintHeaders.Clear();
            keepReprintFooters.Clear();
        }

        private void EndKeepReprint()
        {
            foreach (BandBase band in keepReprintHeaders)
            {
                reprintHeaders.Add(band);
            }
            foreach (BandBase band in keepReprintFooters)
            {
                reprintFooters.Add(band);
            }
            keepReprintHeaders.Clear();
            keepReprintFooters.Clear();
        }

        private float GetFootersHeight()
        {
            float result = 0;
            bool saveRepeated = false;

            foreach (BandBase band in reprintFooters)
            {
                saveRepeated = band.Repeated;
                band.Repeated = true;
                result += GetBandHeightWithChildren(band);
                band.Repeated = saveRepeated;
            }
            foreach (BandBase band in keepReprintFooters)
            {
                saveRepeated = band.Repeated;
                band.Repeated = true;
                result += GetBandHeightWithChildren(band);
                band.Repeated = saveRepeated;
            }

            return result;
        }

        #endregion Private Methods
    }
}
