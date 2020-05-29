using System;
using System.Collections.Generic;

namespace FastReport.Utils
{
#if !COMMUNITY
    /// <summary>
    /// Class for handling Exports visibility in the Preview control.
    /// </summary>
    public partial class ExportsOptions
    {
        private static ExportsOptions instance = null;

        private List<ExportsTreeNode> menuNodes;

        /// <summary>
        /// All exports available in the Preview control.
        /// </summary>
        public List<ExportsTreeNode> ExportsMenu
        {
            get { return menuNodes; }
        }

        private ExportsOptions()
        {
            menuNodes = new List<ExportsTreeNode>();
        }

        /// <summary>
        /// Gets an instance of ExportOptions.
        /// </summary>
        /// <returns></returns>
        public static ExportsOptions GetInstance()
        {
            if (instance == null)
            {
                return instance = new ExportsOptions();
            }
            else
            {
                return instance;
            }
        }

        /// <summary>
        /// Exports menu node.
        /// </summary>
        public partial class ExportsTreeNode
        {
            private const string EXPORT_ITEM_PREFIX = "Export,";
            private const string ITEM_POSTFIX = ",Name";
            private const string EXPORT_ITEM_POSTFIX = ",File";
            private const string CATEGORY_PREFIX = "Export,ExportGroups,";

            private string name;
            private List<ExportsTreeNode> nodes = new List<ExportsTreeNode>();
            private Type exportType = null;
            private int imageIndex = -1;
            private ObjectInfo tag = null;
            private bool enabled = true;
            private bool isExport;

            /// <summary>
            /// Gets the name.
            /// </summary>
            public string Name
            {
                get { return name; }
            }

            /// <summary>
            /// Gets nodes.
            /// </summary>
            public List<ExportsTreeNode> Nodes
            {
                get { return nodes; }
            }
            
            /// <summary>
            /// Gets type of the export.
            /// </summary>
            public Type ExportType
            {
                get { return exportType; }
            }
            
            /// <summary>
            /// Gets index of the image.
            /// </summary>
            public int ImageIndex
            {
                get { return imageIndex; }
            }
            
            /// <summary>
            /// Gets or sets the tag.
            /// </summary>
            public ObjectInfo Tag
            {
                get { return tag; }
                set { tag = value; }
            }

            /// <summary>
            /// Gets or sets a value that indicates is node enabled.
            /// </summary>
            public bool Enabled
            {
                get { return enabled; }
                set { enabled = value; }
            }
            
            /// <summary>
            /// Gets true if node is export, otherwise false.
            /// </summary>
            public bool IsExport
            {
                get { return isExport; }
            }

            public ExportsTreeNode(string name, bool isExport)
            {
                this.name = name;
                this.isExport = isExport;
            }

            public ExportsTreeNode(string name, Type exportType, bool isExport)
            {
                this.name = name;
                this.exportType = exportType;
                this.isExport = isExport;
            }

            public ExportsTreeNode(string name, int imageIndex, bool isExport)
            {
                this.name = name;
                this.imageIndex = imageIndex;
                this.isExport = isExport;
            }

            public ExportsTreeNode(string name, Type exportType, int imageIndex, bool isExport)
            {
                this.name = name;
                this.exportType = exportType;
                this.imageIndex = imageIndex;
                this.isExport = isExport;
            }

            public ExportsTreeNode(string name, Type exportType, int imageIndex, bool enabled, bool isExport)
            {
                this.name = name;
                this.exportType = exportType;
                this.imageIndex = imageIndex;
                this.enabled = enabled;
                this.isExport = isExport;
            }
        }

        /// <summary>
        /// Saves current visible exports in config file.
        /// </summary>
        public void SaveExportOptions()
        {
            SaveOptions();
        }

        /// <summary>
        /// Restores visible exports from config file.
        /// </summary>
        public void RestoreExportOptions()
        {
            RestoreOptions();
        }

        /// <summary>
        /// 
        /// </summary>
        public void RegisterExports()
        {
            Queue<ExportsTreeNode> queue = new Queue<ExportsTreeNode>(menuNodes);

            while (queue.Count != 0)
            {
                ExportsTreeNode node = queue.Dequeue();
                if (node.ExportType != null)
                {
                    RegisteredObjects.AddExport(node.ExportType, node.ToString(), node.ImageIndex);
                }
                List<ObjectInfo> list = new List<ObjectInfo>();
                RegisteredObjects.Objects.EnumItems(list);
                node.Tag = list[list.Count - 1];
                foreach (ExportsTreeNode nextNode in node.Nodes)
                {
                    queue.Enqueue(nextNode);
                }
            }
        }

        private ExportsTreeNode FindItem(string name, Type exportType)
        {
            Queue<ExportsTreeNode> queue = new Queue<ExportsTreeNode>(menuNodes);

            while (queue.Count != 0)
            {
                ExportsTreeNode node = queue.Dequeue();

                if (exportType != null && node.ExportType == exportType ||
                    !string.IsNullOrEmpty(name) && node.Name == name)
                {
                    return node;
                }

                foreach (ExportsTreeNode nextNode in node.Nodes)
                {
                    queue.Enqueue(nextNode);
                }
            }

            return null;
        }

        /// <summary>
        /// Sets Export category visibility.
        /// </summary>
        /// <param name="name">Export category name.</param>
        /// <param name="enabled">Visibility state.</param>
        public void SetExportCategoryEnabled(string name, bool enabled)
        {
            FindItem(name, null).Enabled = enabled;
        }

        /// <summary>
        /// Sets Export visibility.
        /// </summary>
        /// <param name="exportType">Export type.</param>
        /// <param name="enabled">Visibility state.</param>
        public void SetExportEnabled(Type exportType, bool enabled)
        {
            ExportsTreeNode node = FindItem(null, exportType);
            if (node != null)
            {
                node.Enabled = enabled;
            }
        }
    }
#endif
}
