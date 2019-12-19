using System;
using System.Collections;
using System.Collections.Generic;

namespace FastReport.Utils
{
    partial class ExportsOptions
    {
        private delegate List<ExportsTreeNode> MakeDefaultExportsMenuDelegate();

        private static ExportsOptions instance = null;

        private List<ExportsTreeNode> menuNodes;
        private MakeDefaultExportsMenuDelegate defaultMenuDelegate;

        public List<ExportsTreeNode> ExportsMenu { get { return menuNodes; } }

        private ExportsOptions()
        {
            defaultMenuDelegate = this.DefaultExports;
            menuNodes = new List<ExportsTreeNode>();
        }

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

        public class ExportsTreeNode
        {
            private const string EXPORT_ITEM_PREFIX = "Export,";
            private const string EXPORT_ITEM_POSTFIX = ",File";
            private const string EXPORT_CATEGORY_PREFIX = "Export,ExportGroups,";

            private string name;
            private List<ExportsTreeNode> nodes = new List<ExportsTreeNode>();
            private Type exportType = null;
            private int imageIndex = -1;
            private ObjectInfo tag = null;
            private bool enabled = true;

            public string Name { get { return name; } }
            public List<ExportsTreeNode> Nodes { get { return nodes; } }
            public Type ExportType { get { return exportType; } }
            public int ImageIndex { get { return imageIndex; } }
            public ObjectInfo Tag { get { return tag; } set { tag = value; } }
            public bool Enabled { get { return enabled; }
                set { tag.Enabled = enabled = value; } }

            public override string ToString()
            {
                return Res.Get(exportType == null ? EXPORT_CATEGORY_PREFIX + name :
                    EXPORT_ITEM_PREFIX + name + EXPORT_ITEM_POSTFIX);
            }

            public ExportsTreeNode(string name)
            {
                this.name = name;
            }

            public ExportsTreeNode(string name, Type exportType)
            {
                this.name = name;
                this.exportType = exportType;
            }

            public ExportsTreeNode(string name, int imageIndex)
            {
                this.name = name;
                this.imageIndex = imageIndex;
            }

            public ExportsTreeNode(string name, Type exportType, int imageIndex)
            {
                this.name = name;
                this.exportType = exportType;
                this.imageIndex = imageIndex;
            }

            public ExportsTreeNode(string name, Type exportType, int imageIndex, bool enabled)
            {
                this.name = name;
                this.exportType = exportType;
                this.imageIndex = imageIndex;
                this.enabled = enabled;
            }
        }

        private void SaveMenuTree(XmlItem xi, List<ExportsTreeNode> nodes)
        {
            xi.Items.Clear();

            foreach (ExportsTreeNode node in nodes)
            {
                XmlItem newItem = new XmlItem();
                newItem.Name = node.Name;
                if (node.ExportType != null)
                {
                    newItem.SetProp("ExportType", node.ExportType.FullName);
                }
                if (node.ImageIndex != -1)
                {
                    newItem.SetProp("Icon", node.ImageIndex.ToString());
                }
                newItem.SetProp("Enabled", node.Enabled.ToString());
                xi.Items.Add(newItem);
                if (node.Nodes.Count != 0)
                {
                    SaveMenuTree(newItem, node.Nodes);
                }
            }
        }

        public void SaveExportOptions()
        {
            XmlItem options = Config.Root.FindItem("ExportOptions");

            if (options == null)
            {
                options = new XmlItem();
                options.Name = "ExportOptions";
                Config.Root.AddItem(options);
            }
            SaveMenuTree(options, menuNodes);
        }

        private void RestoreMenuTree(XmlItem xi, List<ExportsTreeNode> nodes)
        {
            foreach (XmlItem item in xi.Items)
            {
                Type exportType = null;
                string typeProp = item.GetProp("ExportType");
                if (!string.IsNullOrEmpty(typeProp))
                {
                    exportType = Type.GetType(typeProp);
                }
                string imageIndexProp = item.GetProp("Icon");
                int imageIndex = -1;
                if (!string.IsNullOrEmpty(imageIndexProp))
                {
                    int.TryParse(imageIndexProp, out imageIndex);
                }
                string enabledProp = item.GetProp("Enabled");
                bool enabled = true;
                if (!string.IsNullOrEmpty(imageIndexProp))
                {
                    int.TryParse(imageIndexProp, out imageIndex);
                }
                ExportsTreeNode currentNode = new ExportsTreeNode(item.Name, exportType, imageIndex, enabled);
                nodes.Add(currentNode);
                if (item.Items.Count > 0)
                {
                    RestoreMenuTree(item, currentNode.Nodes);
                }
            }
        }

        public List<ExportsTreeNode> MakeDefaultExportsMenu()
        {
            return defaultMenuDelegate();
        }

        private void RestoreDefault()
        {
            menuNodes = MakeDefaultExportsMenu();
        }

        public void RestoreExportOptions()
        {
            XmlItem options = Config.Root.FindItem("ExportOptions");

            if (options != null && options.Items.Count != 0)
            {
                RestoreMenuTree(options, menuNodes);
            }
            else
            {
                RestoreDefault();
            }
        }

        private void RegisterObject(ExportsTreeNode node, bool registerCategories)
        {
            if (node.ExportType == null && registerCategories)
            {
                RegisteredObjects.AddExportCategory(node.Name, node.ToString(), node.ImageIndex);
            }
            else if (node.ExportType != null && !registerCategories)
            {
                RegisteredObjects.AddExport(node.ExportType, node.ToString(), node.ImageIndex);
            }
            List<ObjectInfo> list = new List<ObjectInfo>();
            RegisteredObjects.Objects.EnumItems(list);
            node.Tag = list[list.Count - 1];
        }

        // if registeredCategories has been set to true - method would register categories only
        // else - exports only
        private void RegisterObjects(List<ExportsTreeNode> nodes, bool registerCategories)
        {
            foreach (ExportsTreeNode node in nodes)
            {
                RegisterObject(node, registerCategories);
                if (node.ExportType == null)
                {
                    RegisterObjects(node.Nodes, registerCategories);
                }
            }
        }

        public void RegisterCategories()
        {
            RegisterObjects(ExportsMenu, true);
        }

        public void RegisterExports()
        {
            RegisterObjects(ExportsMenu, false);
        }

        private ExportsTreeNode FindItem(string name, Type exportType, List<ExportsTreeNode> menu)
        {
            ExportsTreeNode res = null;

            foreach (ExportsTreeNode node in menu)
            {
                if (!string.IsNullOrEmpty(name) && node.Name == name)
                {
                    res =  node;
                }
                if (exportType != null && node.ExportType == exportType)
                {
                    res = node;
                }
                if (node.ExportType == null)
                {
                    res = FindItem(name, exportType, node.Nodes);
                }
            }

            return res;
        }

        public void SetExportCategoryEnabled(string name, bool enabled)
        {
            FindItem(name, null, menuNodes).Enabled = enabled;
        }

        public void SetExportEnabled(Type exportType, bool enabled)
        {
            FindItem(null, exportType, menuNodes).Enabled = enabled;
        }
    }
}
