using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace FastReport.Web
{
    public class ReportTabCollection : Collection<ReportTab>
    {
    }

    public class ReportTab
    {
        public string Name { get; set; } = null;
        public Report Report { get; set; } = null;
        public bool ReportPrepared { get; set; } = false;
        public int CurrentPageIndex { get; set; } = 0;
        public bool Closeable { get; set; } = true;
        public bool NeedParent { get; set; } = false;

        //public ReportTab Clone()
        //{
        //    return new ReportTab()
        //    {
        //        Name = Name,
        //        Report = Report,
        //        ReportPrepared = ReportPrepared,
        //        CurrentPageIndex = CurrentPageIndex,
        //        Closeable = Closeable,
        //    };
        //}
    }
}
