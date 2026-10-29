using System;

namespace FastReport.Web
{
    partial class WebReport
    {
        string template_body(bool renderBody)
        {
            if (!renderBody)
                return $@"
<div class=""fr-report-body"">
    <script type=""module"" class=""webreport-script"" data-config='{GetScriptProps()}' src=""{template_STATIC_ASSET_PATH("js/webreport-script.bundle.min.js")}"" defer></script>
</div>";
                    
            return $@"
<div class=""fr-report-body"">
    {template_outline()}

    <div class=""fr-report"">
        {ReportBody()}
    </div>
 <script type=""module"" class=""webreport-script"" data-config='{GetScriptProps()}' src=""{template_STATIC_ASSET_PATH("js/webreport-script.bundle.min.js")}"" defer></script>
";
        }
    }
}
