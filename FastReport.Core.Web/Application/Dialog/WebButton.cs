using FastReport.Dialog;
using System.Windows.Forms;
using static FastReport.Web.Constants;

namespace FastReport.Web
{
    public partial class Dialog
    {

        private void ButtonClick(ButtonControl button)
        {
            if (button.DialogResult == DialogResult.OK)
            {
                FormClose(button, CloseReason.None);
                CurrentForm++;
            }
            else if (button.DialogResult == DialogResult.Cancel)
            {
                FormClose(button, CloseReason.UserClosing);
                WebReport.Canceled = true;
            }
            button.OnClick(null);
        }

        private void FormClose(ButtonControl button, CloseReason reason)
        {
            DialogPage dialog = button.Report.Pages[CurrentForm] as DialogPage;
            dialog.Form.DialogResult = button.DialogResult;
            FormClosingEventArgs closingArgs = new FormClosingEventArgs(reason, false);
            dialog.OnFormClosing(closingArgs);
            FormClosedEventArgs closedArgs = new FormClosedEventArgs(reason);
            dialog.OnFormClosed(closedArgs);
            dialog.ActiveInWeb = false;
        }

        private string GetButtonHtml(ButtonControl control)
        {
            return $"<input style=\"{GetButtonStyle(control)}\" type=\"button\" name=\"{control.Name}\" value=\"{control.Text}\" onclick=\"{GetEvent(ONCLICK, control, REALOAD)}\" title=\"{control.Text}\"/>";
        }

        private string GetButtonStyle(ButtonControl control)
        {
            return $"{GetControlPosition(control)} {GetControlFont(control.Font)} {GetControlAlign(control)} padding:0;margin:0;";
        }

    }
}
