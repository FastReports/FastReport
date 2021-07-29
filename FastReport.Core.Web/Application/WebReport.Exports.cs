using System;
using System.Collections.Generic;
using System.Text;

namespace FastReport.Web
{
    partial class WebReport
    {
        /// <summary>
        /// Switch a visibility of prepared report export in toolbar
        /// </summary>
        public bool ShowPreparedReport { get => Toolbar.Exports.ShowPreparedReport; set => Toolbar.Exports.ShowPreparedReport = value; }
#if !OPENSOURCE
        /// <summary>
        /// Switches a visibility of PDF (Adobe Acrobat) export in toolbar.
        /// </summary>
        public bool ShowPdfExport { get => Toolbar.Exports.ShowPdfExport; set => Toolbar.Exports.ShowPdfExport = value; }
        /// <summary>
        /// Switches a visibility of Excel 2007 export in toolbar.
        /// </summary>
        public bool ShowExcel2007Export { get => Toolbar.Exports.ShowExcel2007Export; set => Toolbar.Exports.ShowExcel2007Export = value; }
        /// <summary>
        /// Switches a visibility of Word 2007 export in toolbar.
        /// </summary>
        public bool ShowWord2007Export { get => Toolbar.Exports.ShowWord2007Export; set => Toolbar.Exports.ShowWord2007Export = value; }
        /// <summary>
        /// Switches a visibility of PowerPoint 2007 export in toolbar.
        /// </summary>
        public bool ShowPowerPoint2007Export { get => Toolbar.Exports.ShowPowerPoint2007Export; set => Toolbar.Exports.ShowPowerPoint2007Export = value; }
        /// <summary>
        /// Switch a visibility of text (plain text) export in toolbar
        /// </summary>
        public bool ShowTextExport { get => Toolbar.Exports.ShowTextExport; set => Toolbar.Exports.ShowTextExport = value; }
        /// <summary>
        /// Switches a visibility of RTF export in toolbar.
        /// </summary>
        public bool ShowRtfExport { get => Toolbar.Exports.ShowRtfExport; set => Toolbar.Exports.ShowRtfExport = value; }
        /// <summary>
        /// Switches a visibility of XPS export in toolbar.
        /// </summary>
        public bool ShowXpsExport { get => Toolbar.Exports.ShowXpsExport; set => Toolbar.Exports.ShowXpsExport = value; }
        /// <summary>
        /// Switches a visibility of Open Office Spreadsheet (ODS) export in toolbar.
        /// </summary>
        public bool ShowOdsExport { get => Toolbar.Exports.ShowOdsExport; set => Toolbar.Exports.ShowOdsExport = value; }
        /// <summary>
        /// Switches a visibility of Open Office Text (ODT) export in toolbar
        /// </summary>
        public bool ShowOdtExport { get => Toolbar.Exports.ShowOdtExport; set => Toolbar.Exports.ShowOdtExport = value; }
        /// <summary>
        /// Switches a visibility of XML (Excel) export in toolbar.
        /// </summary>
        public bool ShowXmlExcelExport { get => Toolbar.Exports.ShowXmlExcelExport; set => Toolbar.Exports.ShowXmlExcelExport = value; }
        /// <summary>
        /// Switches a visibility of DBF export in toolbar.
        /// </summary>
        public bool ShowDbfExport { get => Toolbar.Exports.ShowDbfExport; set => Toolbar.Exports.ShowDbfExport = value; }
        /// <summary>
        /// Switches visibility the CSV (comma separated values) export in toolbar.
        /// </summary>
        public bool ShowCsvExport { get => Toolbar.Exports.ShowCsvExport; set => Toolbar.Exports.ShowCsvExport = value; }
        /// <summary>
        /// Switches visibility the MHT export in toolbar.
        /// </summary>
        public bool ShowMhtExport { get => Toolbar.Exports.ShowMhtExport; set => Toolbar.Exports.ShowMhtExport = value; }
#endif
    }
}
