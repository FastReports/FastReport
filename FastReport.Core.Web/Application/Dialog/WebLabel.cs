using FastReport.Dialog;

namespace FastReport.Web
{
    public partial class Dialog
    {
        private string GetLabelHtml(LabelControl control)
        {
            return $"<div style=\"{GetLabelStyle(control)}\">{control.Text}</div>";
        }

        private string GetLabelStyle(LabelControl control)
        {
            return $"{GetControlPosition(control)} {GetControlFont(control.Font)} {GetControlAlign(control)}";
        }

    }
}
