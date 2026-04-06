using System;

namespace FastReport.Web.Toolbar
{
    /// <summary>
    /// Button for the toolbar
    /// </summary>
    public class ToolbarButton : ToolbarElement
    {
        /// <summary>
        /// The image of the button that appears in the toolbar
        /// </summary>
        public ToolbarElementImage Image { get; set; } = new ToolbarElementImage();

        /// <summary>
        /// Action that is triggered when the button is clicked
        /// </summary>
        public IClickAction OnClickAction { get; set; }

        internal override string Render(string template_FR)
        {
            if (!Enabled) return default;

            var action = OnClickAction is ElementScript scriptButton
                ? scriptButton.Script
                : $"{template_FR}.customMethodInvoke('{ID}', this.value)";

            return $@"<div class=""fr-toolbar-item fr-toolbar-pointer fr-toolbar-item fr-pointer {ElementClasses}"" style=""{ElementCustomStyle}"" onclick=""{action}"">
                    <img src=""{Image.RenderedImage}"" title=""{Title}"" class=""fr-toolbar-image"">
                    </div>";
        }
    }
}