using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

namespace FastReport.Utils
{
#if !COMMUNITY
    /// <summary>
    /// Class for handling Exports visibility in the Preview control.
    /// </summary>
    public partial class ExportsOptions
    {
        private static ExportsOptions instance = null;

        /// <summary>
        /// Gets an instance of ExportOptions.
        /// </summary>
        /// <returns>An ExportOptions instance.</returns>
        public static ExportsOptions GetInstance()
        {
            if (instance == null)
                instance = new ExportsOptions();

            return instance;
        }

        private ExportsTreeNode exportsMenu;

        /// <summary>
        /// All exports available in the Preview control.
        /// </summary>
        public ExportsTreeNode ExportsMenu
        {
            get
            {
                if (exportsMenu == null)
                {
                    exportsMenu = new ExportsTreeNode() { Name = "Exports" };
                    CreateDefaultExports();
                }

                return exportsMenu;
            }
        }

        /// <summary>
        /// Occurs once right before restore exports state.
        /// </summary>
        /// <remarks>
        /// Use this event to configure the default exports state or add your own exports.
        /// </remarks>
        public event EventHandler BeforeRestoreState;

        /// <summary>
        /// Occurs once right after restore exports state.
        /// </summary>
        /// <remarks>
        /// You may use this event to disable some exports, for example:
        /// <code>Config.PreviewSettings.Exports &amp;= ~PreviewExports.PDFExport;</code>
        /// Doing so before state is restored may not take an effect.
        /// </remarks>
        public event EventHandler AfterRestoreState;

        internal void SaveState()
        {
            SaveOptions();
        }

        private bool isRestored;

        internal void RestoreState()
        {
            if (!isRestored)
            {
                BeforeRestoreState?.Invoke(this, EventArgs.Empty);
                RestoreOptions();
                AfterRestoreState?.Invoke(this, EventArgs.Empty);
                isRestored = true;
            }
        }

        private void EnumNodes(List<ExportsTreeNode> list, ExportsTreeNode root)
        {
            list.Add(root);
            foreach (var node in root.Nodes)
                EnumNodes(list, node);
        }

        private List<ExportsTreeNode> EnumNodes(ExportsTreeNode root)
        {
            var list = new List<ExportsTreeNode>();
            EnumNodes(list, root);
            return list;
        }

        private ExportsTreeNode FindItem(ExportsTreeNode root, string name, Type exportType)
        {
            foreach (var node in EnumNodes(root))
            {
                if (exportType != null && node.ExportType == exportType ||
                    !string.IsNullOrEmpty(name) && node.Name == name)
                {
                    return node;
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
            var node = FindItem(ExportsMenu, name, null);
            if (node != null)
            {
                foreach (var n in EnumNodes(node))
                    n.Enabled = enabled;
            }
        }

        /// <summary>
        /// Sets Export visibility.
        /// </summary>
        /// <param name="exportType">Export type.</param>
        /// <param name="enabled">Visibility state.</param>
        public void SetExportEnabled(Type exportType, bool enabled)
        {
            var node = FindItem(ExportsMenu, null, exportType);
            if (node != null)
                node.Enabled = enabled;
        }

        private ExportsOptions()
        {
        }


        /// <summary>
        /// Exports menu node.
        /// </summary>
        public partial class ExportsTreeNode
        {
            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets child nodes.
            /// </summary>
            public ExportsTreeNodeCollection Nodes { get; }

            /// <summary>
            /// Gets the parent node.
            /// </summary>
            public ExportsTreeNode Parent { get; private set; }

            /// <summary>
            /// Gets the root node.
            /// </summary>
            public ExportsTreeNode Root
            {
                get
                {
                    var parent = this;
                    while (parent.Parent != null)
                    {
                        parent = parent.Parent;
                    }
                    return parent;
                }
            }

            /// <summary>
            /// Gets or sets the type of the export.
            /// </summary>
            public Type ExportType { get; set; }

            /// <summary>
            /// Gets or sets the display text.
            /// </summary>
            public string Text { get; set; }

            /// <summary>
            /// Gets or sets the image index.
            /// </summary>
            public int ImageIndex { get; set; } = -1;

            /// <summary>
            /// Gets or sets the image.
            /// </summary>
            public Bitmap Image { get; set; }

            /// <summary>
            /// Gets or sets the tag.
            /// </summary>
            public ObjectInfo Tag { get; private set; }

            /// <summary>
            /// Gets or sets a value that indicates whether the node is enabled.
            /// </summary>
            public bool Enabled { get; set; } = true;

            internal ExportsTreeNode()
            {
                Nodes = new ExportsTreeNodeCollection(this);
            }

            /// <summary>
            /// Adds a category.
            /// </summary>
            /// <param name="name">The category key name.</param>
            /// <param name="imageIndex">The image index.</param>
            /// <returns>The category node.</returns>
            public ExportsTreeNode AddCategory(string name, int imageIndex = -1)
            {
                var node = new ExportsTreeNode() { Name = name, ImageIndex = imageIndex };
                Nodes.Add(node);
                return node;
            }

            /// <summary>
            /// Adds a category.
            /// </summary>
            /// <param name="name">The category key name.</param>
            /// <param name="text">The category display text.</param>
            /// <param name="image">The image.</param>
            /// <returns>The category node.</returns>
            public ExportsTreeNode AddCategory(string name, string text, Bitmap image)
            {
                var node = new ExportsTreeNode() { Name = name, Text = text, Image = image };
                Nodes.Add(node);
                return node;
            }

            /// <summary>
            /// Adds an export and registers it.
            /// </summary>
            /// <param name="type">The export type.</param>
            /// <param name="text">The display text.</param>
            /// <param name="imageIndex">The image index.</param>
            /// <returns>Returns this object to allow method chaining.</returns>
            public ExportsTreeNode AddExport(Type type, string text, int imageIndex = -1)
            {
                var node = new ExportsTreeNode() { Name = type.Name, ExportType = type, Text = text, ImageIndex = imageIndex };
                node.Tag = RegisteredObjects.AddExport(node.ExportType, node.ToString(), node.ImageIndex);
                Nodes.Add(node);
                return this;
            }

            /// <summary>
            /// Adds an export and registers it.
            /// </summary>
            /// <param name="type">The export type.</param>
            /// <param name="text">The display text.</param>
            /// <param name="image">The image.</param>
            /// <returns>Returns this object to allow method chaining.</returns>
            public ExportsTreeNode AddExport(Type type, string text, Bitmap image)
            {
                var node = new ExportsTreeNode() { Name = type.Name, ExportType = type, Text = text, Image = image };
                node.Tag = RegisteredObjects.AddExport(node.ExportType, text, -1);
                Nodes.Add(node);
                return this;
            }


            /// <summary>
            /// Represents a collection of nodes.
            /// </summary>
            public class ExportsTreeNodeCollection : Collection<ExportsTreeNode>
            {
                private ExportsTreeNode Owner { get; }

                internal ExportsTreeNodeCollection(ExportsTreeNode owner)
                {
                    Owner = owner;
                }

                /// <inheritdoc/>
                protected override void InsertItem(int index, ExportsTreeNode item)
                {
                    item.Parent?.Nodes.Remove(item);
                    base.InsertItem(index, item);
                    item.Parent = Owner;
                }

                /// <inheritdoc/>
                protected override void RemoveItem(int index)
                {
                    var item = Items[index];
                    item.Parent = null;
                    base.RemoveItem(index);
                }
            }
        }
    }
#endif
}
