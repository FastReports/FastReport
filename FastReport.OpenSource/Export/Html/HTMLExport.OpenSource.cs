using FastReport.Utils;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;

namespace FastReport.Export.Html
{

    public partial class HTMLExport
    {
        private void ExportHTMLPageBegin(object data)
        {
            HTMLData d = (HTMLData)data;
            ExportHTMLPageLayeredBegin(d);
        }

        private void ExportHTMLPageEnd(object data)
        {
            HTMLData d = (HTMLData)data;
            ExportHTMLPageLayeredEnd(d);
        }

        private bool HasExtendedExport(ReportComponentBase obj)
        {
            return false;
        }

        partial void ExtendExport(FastString Page, ReportComponentBase obj, FastString text);

        private class ExportIEMStyle
        {

        }
        partial void SetExportableAdvMatrix(Base c);
       

        private string GetHrefAdvMatrixButton(ReportComponentBase obj, string href)
        {
            return string.Empty;
        }
       
        /// <inheritdoc/>
        protected override void ExportBand(BandBase band)
        {
            if (ExportMode == ExportType.Export)
                base.ExportBand(band);

            ExportBandLayers(band);
        }
    }
}