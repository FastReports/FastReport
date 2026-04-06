using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web;

namespace FastReport.Web
{
    partial class WebReport
    {
        string template_modalcontainer()
        {
            var templateModalContainer = $@"
<div class=""modalcontainers"" id=""modalcontainers"">
 {(false ? @"<script type=""module"" src=""./_content/FastReport.Web/js/webreport-export-script.bundle.js""></script>" : "")}
    <div class=""modalcontainer-overlay"">

        <div class=""content-modalcontainer""></div>
    </div>
</div>";
            return templateModalContainer;
        }
    }

}
