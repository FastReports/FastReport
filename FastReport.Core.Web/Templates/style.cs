using System.Drawing;

namespace FastReport.Web
{
    partial class WebReport
    {
        string template_style() => $" :root{{ {GetStyleVars()} }}";
    }
}