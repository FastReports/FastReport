using System;
using System.Drawing;

namespace FastReport.Plugins.Transport
{
    public interface IMenuItem
    {
        string Text { get; set; }
        object Tag { get; set; }
        int ImageIndex { get; set; }
        event EventHandler Click;
        void AddSubItem(IMenuItem item);
        void SetFont(Font font);
        void SetImage(Image image);
    }
}
