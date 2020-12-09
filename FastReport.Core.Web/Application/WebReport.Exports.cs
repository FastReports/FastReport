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
        public bool ShowPreparedReport { get; set; } = true;
#if !OPENSOURCE
        /// <summary>
        /// Switches a visibility of PDF (Adobe Acrobat) export in toolbar.
        /// </summary>
        public bool ShowPdfExport { get; set; } = true;
        /// <summary>
        /// Switches a visibility of Excel 2007 export in toolbar.
        /// </summary>
        public bool ShowExcel2007Export { get; set; } = true;
        /// <summary>
        /// Switches a visibility of Word 2007 export in toolbar.
        /// </summary>
        public bool ShowWord2007Export { get; set; } = false;
        /// <summary>
        /// Switches a visibility of PowerPoint 2007 export in toolbar.
        /// </summary>
        public bool ShowPowerPoint2007Export { get; set; } = false;
        /// <summary>
        /// Switch a visibility of text (plain text) export in toolbar
        /// </summary>
        public bool ShowTextExport { get; set; } = true;
        /// <summary>
        /// Switches a visibility of RTF export in toolbar.
        /// </summary>
        public bool ShowRtfExport { get; set; } = true;
        /// <summary>
        /// Switches a visibility of XPS export in toolbar.
        /// </summary>
        public bool ShowXpsExport { get; set; } = true;
        /// <summary>
        /// Switches a visibility of Open Office Spreadsheet (ODS) export in toolbar.
        /// </summary>
        public bool ShowOdsExport { get; set; } = true;
        /// <summary>
        /// Switches a visibility of Open Office Text (ODT) export in toolbar
        /// </summary>
        public bool ShowOdtExport { get; set; } = true;
        /// <summary>
        /// Switches a visibility of XML (Excel) export in toolbar.
        /// </summary>
        public bool ShowXmlExcelExport { get; set; } = true;
        /// <summary>
        /// Switches a visibility of DBF export in toolbar.
        /// </summary>
        public bool ShowDbfExport { get; set; } = false;
        /// <summary>
        /// Switches visibility the CSV (comma separated values) export in toolbar.
        /// </summary>
        public bool ShowCsvExport { get; set; } = true;
        /// <summary>
        /// Switches visibility the MHT export in toolbar.
        /// </summary>
        public bool ShowMhtExport { get; set; } = false;
#endif
    }
}
