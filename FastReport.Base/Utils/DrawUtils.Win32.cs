using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FastReport.Utils
{
    partial class DrawUtils
    {
#if NETSTANDARD2_0 || NETSTANDARD2_1
        static IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, DrawingOptions drawingOptions)
        {
            return IntPtr.Zero;
        }
#else
        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, DrawingOptions drawingOptions);
#endif

        const int WM_PRINT = 0x317;

        [Flags]
        enum DrawingOptions
        {
            PRF_CHECKVISIBLE = 0x01,
            PRF_NONCLIENT = 0x02,
            PRF_CLIENT = 0x04,
            PRF_ERASEBKGND = 0x08,
            PRF_CHILDREN = 0x10,
            PRF_OWNED = 0x20
        }

        public static Bitmap DrawToBitmap(Control control, bool children)
        {
            Bitmap bitmap = new Bitmap(control.Width, control.Height);
            using (Graphics gr = Graphics.FromImage(bitmap))
            {
                IntPtr hdc = gr.GetHdc();
                DrawingOptions options = DrawingOptions.PRF_ERASEBKGND |
                  DrawingOptions.PRF_CLIENT | DrawingOptions.PRF_NONCLIENT;
                if (children)
                    options |= DrawingOptions.PRF_CHILDREN;
                SendMessage(control.Handle, WM_PRINT, hdc, options);
                gr.ReleaseHdc(hdc);
            }
            return bitmap;
        }
    }
}