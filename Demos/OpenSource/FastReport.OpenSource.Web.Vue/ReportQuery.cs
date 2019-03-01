using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FastReport.OpenSource.Web.Vue
{
    public class ReportQuery
    {
        // Format of resulting report: png, pdf, html
        public string Format { get; set; }
        // Enable Inline preview in browser (generates "inline" or "attachment")
        public bool Inline { get; set; }
        // Value of "Parameter" variable in report
        public string Parameter { get; set; }
    }
}
