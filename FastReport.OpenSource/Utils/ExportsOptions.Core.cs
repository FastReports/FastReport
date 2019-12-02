using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FastReport.Export.Image;

namespace FastReport.Utils
{
    partial class ExportsOptions
    {
        private List<ExportsTreeNode> DefaultExports()
        {
            List<ExportsTreeNode> defaultMenu = new List<ExportsTreeNode>();

            defaultMenu.Add(new ExportsTreeNode("Image", typeof(ImageExport), 103));

            return defaultMenu;
        }
    }
}
