using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Html;
using System.Threading.Tasks;
using System.Linq;
using FastReport.Web.Application;
using System.Drawing;

namespace FastReport.Web
{

    public partial class WebReport
    {

        public HtmlString RenderSync()
        {
            return Task.Run(() => Render()).Result;
        }

        public async Task<HtmlString> Render()
        {
            if (Report == null)
                throw new Exception("Report is null");

            return Render(false);
        }


        internal HtmlString Render(bool renderBody)
        {
            switch (Mode)
            {
#if DIALOGS
                case WebReportMode.Dialog:
#endif
                case WebReportMode.Preview:
                    return new HtmlString(template_render(renderBody));
#if DESIGNER
                case WebReportMode.Designer:
                    return RenderDesigner();
#endif
                default:
                    throw new Exception($"Unknown mode: {Mode}");
            }
        }
    }
}
