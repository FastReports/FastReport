using System;
using FastReport.Dialog;
using static FastReport.Web.Constants;

namespace FastReport.Web
{
    public partial class Dialog
    {

        /// <summary>
        /// Gets or sets date time format in javascript type="date"
        /// </summary>
        public string DateTimePickerFormat {
            get;
            set;
        } = DEFAULT_DATE_TIME_PICKER_FORMAT;


        private void DateTimePickerChange(DateTimePickerControl dp, string value)
        {
            dp.Value = DateTime.Parse(value);
            dp.OnValueChanged(null);
        }

        private string GetDateTimePickerHtml(DateTimePickerControl control)
        {
            control.FillData();
            ControlFilterRefresh(control);
            string id = GetControlID(control);

            return string.Format("<input style=\"{0}\" type=\"date\" name=\"{1}\" value=\"{2}\" onchange=\"{3}\" id=\"{4}\"/>",
                // style
                GetDateTimePickerStyle(control),
                // name
                control.Name,
                // value
                control.Value.ToString(DateTimePickerFormat),
                // onclick
                GetEvent(ONCHANGE, control, SILENT_RELOAD, $"document.getElementById('{id}').value"),
                // title
                id
                );
        }

        private string GetDateTimePickerStyle(DateTimePickerControl control)
        {
            return $"{GetControlPosition(control)} {GetControlFont(control.Font)}";
        }        

    }
}
