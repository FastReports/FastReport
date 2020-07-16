using System;
using System.Drawing;
using System.Text;
using FastReport.Dialog;
using System.Windows.Forms;

namespace FastReport.Web
{
    public partial class Dialog
    {
        private WebReport WebReport {
            get;
        }

        private int CurrentForm {
            get;
            set;
        }


        public Dialog(WebReport webReport)
        {
            WebReport = webReport;
        }


        private void CheckDialogs()
        {
            Report report = this.WebReport.Report;
            while (CurrentForm < report.Pages.Count && !(report.Pages[CurrentForm] is DialogPage && report.Pages[CurrentForm].Visible == true))
                CurrentForm++;

            if (CurrentForm < report.Pages.Count)
            {
                WebReport.Mode = WebReportMode.Dialog;
            }
            else
            {
                if(WebReport.Mode == WebReportMode.Dialog)    // 
                {
                    report.PreparePhase2();
                    WebReport.ReportPrepared = true;
                }
                WebReport.Mode = WebReportMode.Preview;
            }
        }

        internal void ProcessDialogs(StringBuilder sb)
        {
            Report report = this.WebReport.Report;

            if (CurrentForm < report.Pages.Count)
            {
                DialogPage dialog = report.Pages[CurrentForm] as DialogPage;
                if (!dialog.ActiveInWeb)
                {
                    dialog.ActiveInWeb = true;
                    dialog.OnLoad(EventArgs.Empty);
                    dialog.OnShown(EventArgs.Empty);
                }
                GetDialogHtml(sb, dialog);
            }
        }

        internal void SetDialogs(string dialogN, string controlName, string eventName, string data)
        {
            SetUpDialogs(dialogN, controlName, eventName, data);

            CheckDialogs();
        }


        internal void SetUpDialogs(string dialogN, string controlName, string eventName, string data)
        {
            if (!string.IsNullOrEmpty(dialogN))
            {
                int dialogIndex = Convert.ToInt16(dialogN);
                if (dialogIndex >= 0 && dialogIndex < WebReport.Report.Pages.Count)
                {
                    DialogPage dialog = WebReport.Report.Pages[dialogIndex] as DialogPage;                    

                    DialogControl control = dialog.FindObject(controlName) as DialogControl;
                    if (control != null)
                    {
                        if (eventName == ONCHANGE)
                        {
                            if (!string.IsNullOrEmpty(data))
                            {
                                if (control is TextBoxControl)
                                    TextBoxChange(control as TextBoxControl, data);
                                else if (control is ComboBoxControl)
                                    ComboBoxChange(control as ComboBoxControl, Convert.ToInt16(data));
                                else if (control is ListBoxControl)
                                    ListBoxChange(control as ListBoxControl, Convert.ToInt16(data));
                                else if (control is CheckedListBoxControl)
                                    CheckedListBoxChange(control as CheckedListBoxControl, data);
                                else if (control is DateTimePickerControl)
                                    DateTimePickerChange(control as DateTimePickerControl, data);
                                else if (control is MonthCalendarControl)
                                    MonthCalendarChange(control as MonthCalendarControl, data);
                            }
                        }
                        else if (eventName == ONCLICK)
                        {
                            if (control is ButtonControl)
                                ButtonClick(control as ButtonControl);
                            else if (control is CheckBoxControl)
                                CheckBoxClick(control as CheckBoxControl, data);
                            else if (control is RadioButtonControl)
                                RadioButtonClick(control as RadioButtonControl, data);
                        }
                    }
                }
            }
        }

        private void ControlFilterRefresh(DataFilterBaseControl control)
        {            
            control.FilterData();            
            if (control.DetailControl != null)
            {
                control.DetailControl.ResetFilter();
                control.DetailControl.FillData(control);
            }
        }

        private string GetDialogID()
        {
            return String.Concat(WebReport.ID, "Dialog");
        }

        private string GetControlID(DialogControl control)
        {
            return WebReport.ID + control.Name;
        }


        private void GetComponentHtml(StringBuilder sb, DialogComponentCollection collection)
        {
            foreach (DialogControl control in collection)
            {
                if (control.Visible)
                {
                    // button
                    if (control is ButtonControl)
                        sb.Append(GetButtonHtml(control as ButtonControl));
                    // label
                    else if (control is LabelControl)
                        sb.Append(GetLabelHtml(control as LabelControl));
                    // textbox
                    else if (control is TextBoxControl)
                        sb.Append(GetTextBoxHtml(control as TextBoxControl));
                    // checkbox
                    else if (control is CheckBoxControl)
                        sb.Append(GetCheckBoxHtml(control as CheckBoxControl));
                    // radio button
                    else if (control is RadioButtonControl)
                        sb.Append(GetRadioButtonHtml(control as RadioButtonControl));
                    // combo box
                    else if (control is ComboBoxControl)
                        sb.Append(GetComboBoxHtml(control as ComboBoxControl));
                    // list box
                    else if (control is ListBoxControl)
                        sb.Append(GetListBoxHtml(control as ListBoxControl));
                    // checked list box
                    else if (control is CheckedListBoxControl)
                        sb.Append(GetCheckedListBoxHtml(control as CheckedListBoxControl));
                    // datetime
                    else if (control is DateTimePickerControl)
                        sb.Append(GetDateTimePickerHtml(control as DateTimePickerControl));
                    // monthcalendar
                    else if (control is MonthCalendarControl)
                        sb.Append(GetMonthCalendarHtml(control as MonthCalendarControl));
                    else if (control is GroupBoxControl)
                        sb.Append(GetGroupBoxHtml(control as GroupBoxControl));
                    else if (control is PictureBoxControl)
                        sb.Append(GetPictureBoxHtml(control as PictureBoxControl));
                }
            }
        }

        private string Zoom(float value)
        {
            return $"{value*WebReport.Zoom:0.##}";
        }
        
        private void GetDialogHtml(StringBuilder sb, DialogPage dialog)
        {
            string s = String.Format("<div style=\"min-width:{0}px! important;min-height:{1}px !important\">",
                Zoom(dialog.Width),
                Zoom(dialog.Height)
                );
            sb.Append(s);

            sb.Append($"<div id=\"{GetDialogID()}\" style=\"position:relative;\" title=\"{dialog.Text}\">"); 

            GetComponentHtml(sb, dialog.Controls);
            sb.Append("</div></div>");
        }

        
        private string GetEvent(string eventName, DialogControl control, string func, string data = null)
        {
            data = string.IsNullOrEmpty(data) ? "'" : $"&data=' + {data}";

            string HandlerURL = $"'&dialog={CurrentForm}&control={control.Name}&event={eventName}{data}";

            return $"fr{WebReport.ID}.{func}({HandlerURL})";
        }

        private string GetControlFont(Font font)
        {
            string fontStyle = (((font.Style & FontStyle.Bold) > 0) ? "font-weight:bold;" : String.Empty) +
    (((font.Style & FontStyle.Italic) > 0) ? "font-style:italic;" : "font-style:normal;");
            if ((font.Style & FontStyle.Underline) > 0 && (font.Style & FontStyle.Strikeout) > 0)
                fontStyle += "text-decoration:underline|line-through;";
            else if ((font.Style & FontStyle.Underline) > 0)
                fontStyle += "text-decoration:underline;";
            else if ((font.Style & FontStyle.Strikeout) > 0)
                fontStyle += "text-decoration:line-through;";

            return $"font-size:{Zoom(font.Size)}pt;font-family:{font.FontFamily.Name};{fontStyle};display:inline-block;";
        }

        private string GetControlPosition(DialogControl control)
        {            
            return string.Format("position:absolute;left:{0}px;top:{1}px;width:{2}px;height:{3}px;padding:0px;margin:0px;",
                Zoom(control.Left),
                Zoom(control.Top),
                Zoom(control.Width),
                Zoom(control.Height));
        }

        private string GetControlAlign(DialogControl control)
        {
            if (control is LabelControl)
                return GetAlign((control as LabelControl).TextAlign);
            else if (control is ButtonControl)
                return GetAlign((control as ButtonControl).TextAlign);
            else if (control is TextBoxControl)
                return GetEditAlign((control as TextBoxControl).TextAlign);
            else
                return ""; 
        }

        private string GetEditAlign(HorizontalAlignment align)
        {
            if (align == HorizontalAlignment.Left)
                return "text-align:left;";
            else if (align == HorizontalAlignment.Center)
                return "text-align:center;";
            else if (align == HorizontalAlignment.Right)
                return "text-align:right;";
            else
                return "";
        }

        private string GetAlign(ContentAlignment align)
        {            
            if (align == ContentAlignment.TopLeft)
                return "text-align:left;vertical-align:top;";
            else if (align == ContentAlignment.TopCenter)
                return "text-align:center;vertical-align:top;";
            else if (align == ContentAlignment.TopRight)
                return "text-align:right;vertical-align:top;";
            else if (align == ContentAlignment.BottomLeft)
                return "text-align:left;vertical-align:bottom;";
            else if (align == ContentAlignment.BottomCenter)
                return "text-align:center;vertical-align:bottom;";
            else if (align == ContentAlignment.TopRight)
                return "text-align:right;vertical-align:bottom;";
            else if (align == ContentAlignment.MiddleLeft)
                return "text-align:left;vertical-align:middle;";
            else if (align == ContentAlignment.MiddleCenter)
                return "text-align:center;vertical-align:middle;";
            else if (align == ContentAlignment.MiddleRight)
                return "text-align:right;vertical-align:middle;";
            else
                return "";
        }
    }
}
