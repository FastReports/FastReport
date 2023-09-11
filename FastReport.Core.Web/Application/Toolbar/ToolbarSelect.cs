using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastReport.Web.Toolbar
{
    /// <summary>
    /// Element that opens a drop-down list when you hover over it
    /// </summary>
    public class ToolbarSelect : ToolbarElement
    {
        /// <summary>
        /// The image of the button that appears in the toolbar
        /// </summary>
        public ToolbarElementImage Image { get; set; } = new ToolbarElementImage();

        /// <summary>
        /// Contains items that will be displayed in the drop-down list
        /// 
        /// List of ToolbarSelectItems
        /// </summary>
        public List<ToolbarSelectItem> Items { get; set; } = new List<ToolbarSelectItem>();

        internal override string Render(string template_FR)
        {
            if (!Enabled) return default;

            var sb = new StringBuilder();

            sb.Append(
                $@"<div class=""fr-toolbar-item {template_FR}-toolbar-item {ElementClasses}"" style = ""{ElementCustomStyle}"">
                            <img src=""{Image.RenderedImage}"" title=""{Title}"" class=""{template_FR}-toolbar-image"">
                         <div class=""fr-toolbar-dropdown-content {template_FR}-toolbar-dropdown-content"">");

            foreach (var item in Items.Where(item => item.Enabled))
                sb.Append(item.Render(template_FR));

            sb.Append("</div></div>");
            return sb.ToString();
        }
    }

    /// <summary>
    /// The element that appears in the drop-down list
    /// </summary> 
    public class ToolbarSelectItem
    {
        public ToolbarSelectItem() 
        { 
            Name = ID.ToString();
        }

        internal Guid ID { get; } = Guid.NewGuid();

        /// <summary>
        /// Name of toolbar item required to interact with the items list
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The inscription displayed in the drop-down list
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Defines the visibility of the item in the drop-down list
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Action that is triggered when the item is clicked
        /// </summary>
        public IClickAction OnClickAction { get; set; }

        internal string Render(string template_FR)
        {
            var action = OnClickAction is ElementScript scriptButton
                ? scriptButton.Script
                : $"{template_FR}.customMethodInvoke('{ID}', this.value)";

            return $@"<a onclick=""{action}"">{Title}</a>";
        }
    }
}