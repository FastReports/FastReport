using FastReport.Web;
using System;

namespace FastReport.Web.Toolbar
{
    /// <summary>
    /// Input field for the toolbar
    /// </summary>
    public class ToolbarInput : ToolbarElement
    {
        /// <summary>
        /// Type of input, which is specified in the type tag
        /// </summary>
        public string InputType { get; set; }

        /// <summary>
        /// Styles that are specified in the style tag for input
        /// </summary>
        public string InputCustomStyle { get; set; }

        /// <summary>
        /// Standard value for input
        /// </summary>
        public string InputDefaultValue { get; set; }

        /// <summary>
        /// Action to be triggered when user changes input value
        /// 
        /// Can be either ElementScript or ElementChangeAction
        /// </summary>
        public IChangeAction OnChangeAction { get; set; }

        internal override string Render(string template_FR)
        {
            if (!Enabled) return default;

            var action = OnChangeAction is ElementScript elementScript
                ? elementScript.Script
                : $"{template_FR}.customMethodInvoke('{ID}', this.value)";

            return
                $@"<div class=""fr-toolbar-item fr-toolbar-notbutton {template_FR}-toolbar-item {template_FR}-toolbar-notbutton {ElementClasses}"" style = ""{ElementCustomStyle}"">
                        <input style=""{InputCustomStyle}"" type=""{InputType}"" value=""{InputDefaultValue}"" title=""{Title}"" onchange=""{action}"">
                      </div>";
        }
    }
}