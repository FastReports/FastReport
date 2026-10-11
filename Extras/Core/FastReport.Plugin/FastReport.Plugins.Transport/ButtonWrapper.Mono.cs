using System;
using System.Drawing;
using System.Windows.Forms;

namespace FastReport.Plugins.Transport
{
    internal static class DotnetExt
    {
        public static void InsertSubItem(this ToolStripMenuItem parent, int index, IMenuItem item)
            => parent.DropDownItems.Insert(index, (item as ButtonWrapper)?._item);
    }

    public class ButtonWrapper : IMenuItem
    {
        internal ToolStripDropDownItem _item;

        public ButtonWrapper(ToolStripDropDownItem item) => _item = item;
        public ButtonWrapper()
        {
            _item = new ToolStripMenuItem();
        }

        public string Name { get => _item.Name; set => _item.Name = value; }
        public string Text { get => _item.Text; set => _item.Text = value; }
        public object Tag { get => _item.Tag; set => _item.Tag = value; }
        public int ImageIndex { get => _item.ImageIndex; set => _item.ImageIndex = value; }

        public event EventHandler Click
        {
            add => _item.Click += value;
            remove => _item.Click -= value;
        }

        public static implicit operator ButtonWrapper(ToolStripMenuItem item) => new ButtonWrapper(item);


        public static implicit operator ButtonWrapper(ToolStripDropDownButton item) => new ButtonWrapper(item);

        public static implicit operator ButtonWrapper(ToolStripDropDownItem item) => new ButtonWrapper(item);
        public void AddSubItem(IMenuItem item)
        {
            var name = _item.GetType().Name;

            if (_item is ToolStripDropDownButton parent && (item as ButtonWrapper)?._item is ToolStripMenuItem child)
                parent.DropDownItems.Add(child);
            else
                _item.DropDownItems.Add((item as ButtonWrapper)?._item);
        }

        public static ButtonWrapper CreateFromSender(object obj)
        {
            if (obj != null && obj is ToolStripDropDownItem item)
                return item;
            return null;
        }

        public void SetFont(Font font)
        {
            _item.Font = font;
        }

        public void SetImage(Image image)
        {
            _item.Image = image;
        }
    }
}
