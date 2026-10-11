using FastReport.DevComponents.DotNetBar;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace FastReport.Plugins.Transport
{
    internal static class DotnetExt
    {
        public static void InsertSubItem(this ButtonItem parent, int index, IMenuItem item)
            => parent.SubItems.Insert(index, (item as ButtonWrapper)?._item);
    }

    public class ButtonWrapper : IMenuItem
    {
        internal ButtonItem _item;

        public ButtonWrapper(ButtonItem item) => _item = item;
        public ButtonWrapper()
        {
            _item = new ButtonItem();
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

        public static implicit operator ButtonWrapper(ButtonItem item) => new ButtonWrapper(item);

        public static implicit operator ButtonItem(ButtonWrapper item) => item._item;

        public void AddSubItem(IMenuItem item)
            => _item.SubItems.Add((item as ButtonWrapper)?._item);

        public static ButtonWrapper CreateFromSender(object obj)
        {
            if (obj != null && obj is ButtonItem item)
                return item;
            return null;
        }

        public void SetFont(Font font) { } 
        public void SetImage(Image image) { }
    }
}
