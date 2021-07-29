using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Html;
using System.Threading.Tasks;
using System.Linq;
using FastReport.Web.Controllers;
using FastReport.Web.Application;
using System.Drawing;

namespace FastReport.Web
{
    public class ToolbarSettings
    {
        public static ToolbarSettings Default => new ToolbarSettings();

        public bool Show { get; set; } = true;

#if DIALOGS
        public bool ShowOnDialogPage { get; set; } = true;
#endif

        public bool ShowBottomToolbar { get; set; } = false;

        public ExportMenuSettings Exports { get; set; } = new ExportMenuSettings();

        public bool ShowPrevButton { get; set; } = true;
        public bool ShowNextButton { get; set; } = true;
        public bool ShowFirstButton { get; set; } = true;
        public bool ShowLastButton { get; set; } = true;

        public bool ShowRefreshButton { get; set; } = true;
        public bool ShowZoomButton { get; set; } = true;

        public bool ShowPrint { get; set; } = true;
        public bool PrintInHtml { get; set; } = true;
#if !OPENSOURCE
        public bool PrintInPdf { get; set; } = true;
#endif


        public Color Color { get; set; } = Color.LightGray;


        public ToolbarSettings()
        {

        }
    }
}
