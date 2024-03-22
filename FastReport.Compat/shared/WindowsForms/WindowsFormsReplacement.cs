using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.Drawing;

#pragma warning disable 1591 // disable missing xml comments warning
#pragma warning disable FR0000 // Field must be texted in lowerCamelCase.

namespace System.Windows.Forms
{
    #region Enums
    [Flags]
    public enum Keys
    {
        Modifiers = -65536,
        None = 0,
        LButton = 1,
        RButton = 2,
        Cancel = 3,
        MButton = 4,
        XButton1 = 5,
        XButton2 = 6,
        Back = 8,
        Tab = 9,
        LineFeed = 10,
        Clear = 12,
        Return = 13,
        Enter = 13,
        ShiftKey = 16,
        ControlKey = 17,
        Menu = 18,
        Pause = 19,
        Capital = 20,
        CapsLock = 20,
        KanaMode = 21,
        HanguelMode = 21,
        HangulMode = 21,
        JunjaMode = 23,
        FinalMode = 24,
        HanjaMode = 25,
        KanjiMode = 25,
        Escape = 27,
        IMEConvert = 28,
        IMENonconvert = 29,
        IMEAccept = 30,
        IMEAceept = 30,
        IMEModeChange = 31,
        Space = 32,
        Prior = 33,
        PageUp = 33,
        Next = 34,
        PageDown = 34,
        End = 35,
        Home = 36,
        Left = 37,
        Up = 38,
        Right = 39,
        Down = 40,
        Select = 41,
        Print = 42,
        Execute = 43,
        Snapshot = 44,
        PrintScreen = 44,
        Insert = 45,
        Delete = 46,
        Help = 47,
        D0 = 48,
        D1 = 49,
        D2 = 50,
        D3 = 51,
        D4 = 52,
        D5 = 53,
        D6 = 54,
        D7 = 55,
        D8 = 56,
        D9 = 57,
        A = 65,
        B = 66,
        C = 67,
        D = 68,
        E = 69,
        F = 70,
        G = 71,
        H = 72,
        I = 73,
        J = 74,
        K = 75,
        L = 76,
        M = 77,
        N = 78,
        O = 79,
        P = 80,
        Q = 81,
        R = 82,
        S = 83,
        T = 84,
        U = 85,
        V = 86,
        W = 87,
        X = 88,
        Y = 89,
        Z = 90,
        LWin = 91,
        RWin = 92,
        Apps = 93,
        Sleep = 95,
        NumPad0 = 96,
        NumPad1 = 97,
        NumPad2 = 98,
        NumPad3 = 99,
        NumPad4 = 100,
        NumPad5 = 101,
        NumPad6 = 102,
        NumPad7 = 103,
        NumPad8 = 104,
        NumPad9 = 105,
        Multiply = 106,
        Add = 107,
        Separator = 108,
        Subtract = 109,
        Decimal = 110,
        Divide = 111,
        F1 = 112,
        F2 = 113,
        F3 = 114,
        F4 = 115,
        F5 = 116,
        F6 = 117,
        F7 = 118,
        F8 = 119,
        F9 = 120,
        F10 = 121,
        F11 = 122,
        F12 = 123,
        F13 = 124,
        F14 = 125,
        F15 = 126,
        F16 = 127,
        F17 = 128,
        F18 = 129,
        F19 = 130,
        F20 = 131,
        F21 = 132,
        F22 = 133,
        F23 = 134,
        F24 = 135,
        NumLock = 144,
        Scroll = 145,
        LShiftKey = 160,
        RShiftKey = 161,
        LControlKey = 162,
        RControlKey = 163,
        LMenu = 164,
        RMenu = 165,
        BrowserBack = 166,
        BrowserForward = 167,
        BrowserRefresh = 168,
        BrowserStop = 169,
        BrowserSearch = 170,
        BrowserFavorites = 171,
        BrowserHome = 172,
        VolumeMute = 173,
        VolumeDown = 174,
        VolumeUp = 175,
        MediaNextTrack = 176,
        MediaPreviousTrack = 177,
        MediaStop = 178,
        MediaPlayPause = 179,
        LaunchMail = 180,
        SelectMedia = 181,
        LaunchApplication1 = 182,
        LaunchApplication2 = 183,
        OemSemicolon = 186,
        Oem1 = 186,
        Oemplus = 187,
        Oemcomma = 188,
        OemMinus = 189,
        OemPeriod = 190,
        OemQuestion = 191,
        Oem2 = 191,
        Oemtilde = 192,
        Oem3 = 192,
        OemOpenBrackets = 219,
        Oem4 = 219,
        OemPipe = 220,
        Oem5 = 220,
        OemCloseBrackets = 221,
        Oem6 = 221,
        OemQuotes = 222,
        Oem7 = 222,
        Oem8 = 223,
        OemBackslash = 226,
        Oem102 = 226,
        ProcessKey = 229,
        Packet = 231,
        Attn = 246,
        Crsel = 247,
        Exsel = 248,
        EraseEof = 249,
        Play = 250,
        Zoom = 251,
        NoName = 252,
        Pa1 = 253,
        OemClear = 254,
        KeyCode = 65535,
        Shift = 65536,
        Control = 131072,
        Alt = 262144
    }

    [Flags]
    public enum MouseButtons
    {
        None = 0,
        Left = 1048576,
        Right = 2097152,
        Middle = 4194304,
        XButton1 = 8388608,
        XButton2 = 16777216
    }

    [Flags]
    public enum AnchorStyles
    {
        None = 0,
        Top = 1,
        Bottom = 2,
        Left = 4,
        Right = 8
    }

    public enum DockStyle
    {
        None,
        Top,
        Bottom,
        Left,
        Right,
        Fill
    }

    public enum PictureBoxSizeMode
    {
        Normal,
        StretchImage,
        AutoSize,
        CenterImage,
        Zoom
    }

    public enum RightToLeft
    {
        No,
        Yes,
        Inherit
    }

    public enum TextImageRelation
    {
        Overlay,
        ImageBeforeText,
        TextBeforeImage,
        ImageAboveText,
        TextAboveImage
    };

    public enum DialogResult
    {
        None,
        OK,
        Cancel,
        Abort,
        Retry,
        Ignore,
        Yes,
        No
    }

    public enum Appearance
    {
        Normal,
        Button
    }

    public enum CheckState
    {
        Unchecked,
        Checked,
        Indeterminate
    }

    public enum DrawMode
    {
        Normal,
        OwnerDrawFixed,
        OwnerDrawVariable
    }

    public enum SelectionMode
    {
        None,
        One,
        MultiSimple,
        MultiExtended
    }

    public enum ComboBoxStyle
    {
        Simple,
        DropDown,
        DropDownList
    }

    public enum LeftRightAlignment
    {
        Left,
        Right
    }

    public enum Day
    {
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
        Sunday,
        Default
    }

    public enum CharacterCasing
    {
        Normal,
        Upper,
        Lower
    }

    public enum ScrollBars
    {
        None,
        Horizontal,
        Vertical,
        Both
    }

    public enum HorizontalAlignment
    {
        Left,
        Right,
        Center
    }

    public enum DateTimePickerFormat
    {
        Long,
        Short,
        Time,
        Custom
    }

    public enum FormBorderStyle
    {
        None,
        FixedSingle,
        Fixed3D,
        FixedDialog,
        Sizable,
        FixedToolWindow,
        SizableToolWindow
    }

    public enum FormStartPosition
    {
        Manual,
        CenterScreen,
        WindowsDefaultLocation,
        WindowsDefaultBounds,
        CenterParent
    }

    public enum CloseReason
    {
        None,
        WindowsShutDown,
        MdiFormClosing,
        UserClosing,
        TaskManagerClosing,
        FormOwnerClosing,
        ApplicationExitCall
    }

    public enum BorderStyle
    {
        Fixed3D,
        FixedSingle,
        None
    }

    public enum ControlStyles
    {
        ContainerControl = 0x00000001,
        UserPaint = 0x00000002,
        Opaque = 0x00000004,
        ResizeRedraw = 0x00000010,
        FixedWidth = 0x00000020,
        FixedHeight = 0x00000040,
        StandardClick = 0x00000100,
        Selectable = 0x00000200,
        UserMouse = 0x00000400,
        SupportsTransparentBackColor = 0x00000800,
        StandardDoubleClick = 0x00001000,
        AllPaintingInWmPaint = 0x00002000,
        CacheText = 0x00004000,
        EnableNotifyMessage = 0x00008000,
        DoubleBuffer = 0x00010000,
        OptimizedDoubleBuffer = 0x00020000,
        UseTextForAccessibility = 0x00040000,
    }

    [TypeConverter(typeof(PaddingConverter))]
    public struct Padding
    {
        private int FLeft;
        private int FTop;
        private int FRight;
        private int FBottom;

        public int Left { get => FLeft; set => FLeft = value; }
        public int Top { get => FTop; set => FTop = value; }
        public int Right { get => FRight; set => FRight = value; }
        public int Bottom { get => FBottom; set => FBottom = value; }

        public static readonly Padding Empty = new Padding(0, 0, 0, 0);

        public Padding(int left, int top, int right, int bottom)
        {
            FLeft = left;
            FTop = top;
            FRight = right;
            FBottom = bottom;
        }

        public int Horizontal
        {
            get { return Left + Right; }
        }

        public int Vertical
        {
            get { return Top + Bottom; }
        }

        public static bool operator ==(Padding p1, Padding p2)
        {
            return p1.Left == p2.Left && p1.Top == p2.Top && p1.Right == p2.Right && p1.Bottom == p2.Bottom;
        }

        public static bool operator !=(Padding p1, Padding p2)
        {
            return !(p1 == p2);
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + Left;
            hash = hash * 31 + Top;
            hash = hash * 31 + Right;
            hash = hash * 31 + Bottom;
            return hash;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
    }

    internal class PaddingConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                string[] values = (value as string).Split(new char[] { ',' });
                int[] val = new int[values.Length];
                if (values.Length != 4)
                    throw new Exception("Padding: need 4 values to parse from string");
                for (int i = 0; i < values.Length; i++)
                {
                    val[i] = (int)Converter.FromString(typeof(int), values[i]);
                }
                return new Padding(val[0], val[1], val[2], val[3]);
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value == null)
                    return "";
                Padding p = (Padding)value;
                StringBuilder builder = new StringBuilder();
                builder.Append(Converter.ToString(p.Left)).Append(",");
                builder.Append(Converter.ToString(p.Top)).Append(",");
                builder.Append(Converter.ToString(p.Right)).Append(",");
                builder.Append(Converter.ToString(p.Bottom));
                return builder.ToString();
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    internal class Converter
    {
        /// <summary>
        /// Converts an object to a string.
        /// </summary>
        /// <param name="value">The object to convert.</param>
        /// <returns>The string that contains the converted value.</returns>
        internal static string ToString(object value)
        {
            return TypeDescriptor.GetConverter(value).ConvertToInvariantString(value);
        }

        /// <summary>
        /// Converts a string value to the specified data type.
        /// </summary>
        /// <param name="type">The data type to convert to.</param>
        /// <param name="value">The string to convert from.</param>
        /// <returns>The object of type specified in the <b>type</b> parameter that contains 
        /// a converted value.</returns>
        internal static object FromString(Type type, string value)
        {
            return TypeDescriptor.GetConverter(type).ConvertFromInvariantString(value);
        }
    }
    #endregion


    public class KeyEventArgs : EventArgs
    {
        public Keys KeyCode
        {
            get
            {
                Keys keys = KeyData & Keys.KeyCode;
                if (!Enum.IsDefined(typeof(Keys), (int)keys))
                {
                    return Keys.None;
                }

                return keys;
            }
        }

        public readonly Keys KeyData;

        public bool Control => (KeyData & Keys.Control) == Keys.Control;

        public KeyEventArgs(Keys keyData)
        {
            KeyData = keyData;
        }

        public KeyEventArgs() : this(default)
        {
        }
    }

    public class KeyPressEventArgs : EventArgs
    {
    }

    public class MouseEventArgs : EventArgs
    {
        public readonly int X, Y;
        public readonly MouseButtons Button;
        public readonly int Clicks;
        public readonly int Delta;
        public Point Location => new Point(X, Y);
        public MouseEventArgs(MouseButtons button, int clicks, int x, int y, int delta) {
            Button = button;
            X = x;
            Y = y;
            Clicks = clicks;
            Delta = delta;
        }
    }

    public class PaintEventArgs : EventArgs
    {
        public Graphics Graphics;
    }

    public class InvalidateEventArgs : EventArgs
    {
        public Rectangle Rect;
        public InvalidateEventArgs(Rectangle r) { Rect = r; }
    }

    public class DateRangeEventArgs : EventArgs
    {
    }

    public class MeasureItemEventArgs : EventArgs
    {
    }

    public class DrawItemEventArgs : EventArgs
    {
    }

    public class ItemCheckEventArgs : EventArgs
    {
        public ItemCheckEventArgs()
        {

        }

        public ItemCheckEventArgs(int index, CheckState newCheckValue, CheckState currentValue)
        {
            Index = index;
            NewValue = newCheckValue;
            CurrentValue = currentValue;
        }

        public int Index { get; }

        public CheckState NewValue { get; set; }

        public CheckState CurrentValue { get; }
    }

    public class FormClosingEventArgs : CancelEventArgs
    {
        private readonly CloseReason FCloseReason;

        public FormClosingEventArgs(CloseReason closeReason, bool cancel)
          : base(cancel)
        {
            FCloseReason = closeReason;
        }

        public CloseReason CloseReason
        {
            get { return FCloseReason; }
        }
    }

    public class FormClosedEventArgs : EventArgs
    {
        private readonly CloseReason FCloseReason;

        public FormClosedEventArgs(CloseReason closeReason)
        {
            FCloseReason = closeReason;
        }

        public CloseReason CloseReason
        {
            get { return FCloseReason; }
        }
    }

    public delegate void FormClosingEventHandler(object sender, FormClosingEventArgs e);
    public delegate void FormClosedEventHandler(object sender, FormClosedEventArgs e);
    public delegate void KeyEventHandler(object sender, KeyEventArgs e);
    public delegate void KeyPressEventHandler(object sender, KeyPressEventArgs e);
    public delegate void MouseEventHandler(object sender, MouseEventArgs e);
    public delegate void PaintEventHandler(object sender, PaintEventArgs e);
    public delegate void MeasureItemEventHandler(object sender, MeasureItemEventArgs e);
    public delegate void DrawItemEventHandler(object sender, DrawItemEventArgs e);
    public delegate void ItemCheckEventHandler(object sender, ItemCheckEventArgs e);
    public delegate void DateRangeEventHandler(object sender, DateRangeEventArgs e);

    public sealed class Cursor
    {
    }

    public static class Cursors
    {
        public static Cursor AppStarting = new Cursor();
        public static Cursor PanSW = new Cursor();
        public static Cursor PanSouth = new Cursor();
        public static Cursor PanSE = new Cursor();
        public static Cursor PanNW = new Cursor();
        public static Cursor PanNorth = new Cursor();
        public static Cursor PanNE = new Cursor();
        public static Cursor PanEast = new Cursor();
        public static Cursor NoMoveVert = new Cursor();
        public static Cursor NoMoveHoriz = new Cursor();
        public static Cursor NoMove2D = new Cursor();
        public static Cursor VSplit = new Cursor();
        public static Cursor HSplit = new Cursor();
        public static Cursor Help = new Cursor();
        public static Cursor WaitCursor = new Cursor();
        public static Cursor UpArrow = new Cursor();
        public static Cursor SizeWE = new Cursor();
        public static Cursor SizeNWSE = new Cursor();
        public static Cursor SizeNS = new Cursor();
        public static Cursor SizeNESW = new Cursor();
        public static Cursor SizeAll = new Cursor();
        public static Cursor No = new Cursor();
        public static Cursor IBeam = new Cursor();
        public static Cursor Default = new Cursor();
        public static Cursor Cross = new Cursor();
        public static Cursor Arrow = new Cursor();
        public static Cursor PanWest = new Cursor();
        public static Cursor Hand = new Cursor();
    }

    public class Control : Component
    {
        public Control Parent {
            get { return _parent; }
            set {
                _parent = value;
                _parent?.Controls.Add(this);
            }
        }
        public List<Control> Controls { get; } = new List<Control>();

        private Control _parent;
        public Cursor Cursor;
        public bool Enabled = true;                                                         //
        public Font Font;                                           //
        public RightToLeft RightToLeft;
        public int TabIndex;
        public bool TabStop;
        public virtual string Text { get; set; } = "";                                                            //
        public DockStyle Dock;
        public AnchorStyles Anchor;
        public bool Visible = true;                                                         //
        public int Left;                                                                    //
        public int Top;                                                                     //
        public int Width;                                                                   //
        public int Height;                                                                  //
        public static Keys ModifierKeys;
        public Rectangle ClientRectangle { get; }

        public Point Location
        {
            get => new Point(Left, Top);
            set
            {
                Left = Location.X;
                Top = Location.Y;
            }
        }

        public Size Size
        {
            get => new Size(Width, Height);
            set
            {
                Width = value.Width;
                Height = value.Height;
            }
        }

        public Padding Padding { get; set; }

        /// <summary>
        /// Gets the rectangle that represents the display area of the control.
        /// </summary>
        public virtual Rectangle DisplayRectangle { get; }

        public readonly IntPtr Handle = IntPtr.Zero;

        public event EventHandler Click;                                                    //
        public event EventHandler DoubleClick;
        public event EventHandler Enter;
        public event EventHandler Leave;
        public event KeyEventHandler KeyDown;
        public event KeyPressEventHandler KeyPress;
        public event KeyEventHandler KeyUp;
        public event MouseEventHandler MouseDown;
        public event MouseEventHandler MouseMove;
        public event MouseEventHandler MouseUp;
        public event EventHandler MouseEnter;
        public event EventHandler MouseLeave;
        public event EventHandler Resize;
        public event EventHandler TextChanged;                                              //
        public event PaintEventHandler Paint;
        public event EventHandler LostFocus;

        public Control() : base()
        {
            // Compute our default size.
            Size defaultSize = DefaultSize;
            Width = defaultSize.Width;
            Height = defaultSize.Height;
        }

        public Control(Control parent, string text) : this()
        {
            Parent = parent;
            Text = text;
        }

        public void BringToFront() { }
        public void Focus() { }
        public void Hide() { }
        public void Show() { }
        public virtual void Refresh() { }
        public void Update() { }
        public virtual void Invalidate(bool b) { }
        public void Invalidate(Rectangle r) { }
        public void Invalidate() { }
        public void SetStyle(ControlStyles style, bool fl) { }
        public Form FindForm() { return null; }
        public void PerformLayout() { }
        protected void UpdateStyles() { }

        protected virtual System.Drawing.Size DefaultSize { get; set; }
        public virtual Image BackgroundImage { get; set; }
        public virtual Color BackColor { get; set; }
        public virtual Color ForeColor { get; set; }


        protected virtual void OnPaint(PaintEventArgs e) { }
        protected virtual void OnPaintBackground(PaintEventArgs pevent) { }
        protected virtual void OnSystemColorsChanged(EventArgs e) { }
        protected virtual void OnLocationChanged(EventArgs e) { }
        protected virtual void OnRightToLeftChanged(EventArgs e) { }
        protected virtual void OnResize(EventArgs e) { }
        protected virtual void OnGotFocus(EventArgs e) { }
        protected virtual void OnLostFocus(EventArgs e) { }
        protected virtual void OnCursorChanged(EventArgs e) { }
        protected virtual void OnMouseDown(MouseEventArgs e) { }
        protected virtual void OnMouseLeave(EventArgs e) { }
        protected virtual void OnMouseUp(MouseEventArgs e) { }
        protected virtual void OnMouseMove(MouseEventArgs e) { }
        protected virtual void OnDoubleClick(EventArgs e) { }
        protected virtual void OnMouseDoubleClick(MouseEventArgs e) { }
        protected virtual void OnInvalidated(InvalidateEventArgs e) { }
        protected virtual void OnBackColorChanged(EventArgs eventArgs) { }
    }

    public class GroupBox : Control
    {
        protected override Size DefaultSize {
            get {
                return new Size(200, 100);
            }
        }

    }

    public class ToolTip : Control
    {
        public bool Active;
        public int AutoPopDelay;
        public int InitialDelay;
        public int ReshowDelay;
        public bool ShowAlways;

        public void SetToolTip(Control c, string newToolTipText) { }
        public string GetToolTip(Control c) { return ""; }
    }

    public class ButtonBase : Control
    {
        public bool AutoSize;
        public Image Image;
        public ContentAlignment ImageAlign;
        public ContentAlignment TextAlign = ContentAlignment.MiddleCenter;                  //
        public TextImageRelation TextImageRelation;

        protected override Size DefaultSize {
            get {
                return new Size(75, 23);
            }
        }
    }

    public class PictureBox : Control
    {
        private Image _image;
        private BorderStyle _borderStyle = BorderStyle.None;
        private PictureBoxSizeMode _sizeMode = PictureBoxSizeMode.Normal;

        public BorderStyle BorderStyle {
            get => _borderStyle;
            set {
                if (_borderStyle != value)
                {
                    _borderStyle = value;
                }
            }
        }

        public Image Image {
            get => _image;
            set => _image = value;
        }

        public PictureBoxSizeMode SizeMode {
            get => _sizeMode;
            set {
                if (_sizeMode != value)
                {
                    _sizeMode = value;
                }
            }
        }


        protected override Size DefaultSize => new Size(100, 50);


    }

    public class Button : ButtonBase
    {
        public DialogResult DialogResult = DialogResult.None;                               //
    }

    public class CheckBox : ButtonBase
    {
        public Appearance Appearance;
        public ContentAlignment CheckAlign;
        public bool Checked = false;                                                        //
        public CheckState CheckState;
        public bool ThreeState;
        public object Tag;

        public event EventHandler CheckedChanged;

        protected override Size DefaultSize {
            get {
                return new Size(104, 24);
            }
        }
    }

    public abstract class ListControl : Control
    {

        public event EventHandler SelectedIndexChanged;                                     //
        public event MeasureItemEventHandler MeasureItem;
        public event DrawItemEventHandler DrawItem;

        //public bool Sorted;
        public DrawMode DrawMode;
        public int ItemHeight;

        public abstract int SelectedIndex { get; set; }

        public string GetItemText(object item)
        {
            // if we did not do any work then return the old ItemText
            return Convert.ToString(item, CultureInfo.CurrentCulture);
        }
    }

    public partial class ListBox : ListControl
    {
        public bool IntegralHeight;
        public int ColumnWidth;
        public bool MultiColumn;
        public SelectionMode SelectionMode = SelectionMode.One;
        public bool UseTabStops;
        bool sorted = false;
        ObjectCollection itemsCollection;
        SelectedIndexCollection selectedIndices;
        SelectedObjectCollection selectedItems;

        protected override Size DefaultSize {
            get {
                return new Size(120, 96);
            }
        }

        public bool Sorted
        {
            get
            {
                return sorted;
            }
            set
            {
                if (sorted != value)
                {
                    sorted = value;
                }
            }
        }
        public ObjectCollection Items
        {
            get
            {
                if (itemsCollection == null)
                {
                    itemsCollection = new ObjectCollection(this);
                }
                return itemsCollection;
            }
        }

        public override int SelectedIndex
        {
            get
            {
                if (itemsCollection != null && SelectedItems.Count > 0)
                {
                    return Items.IndexOfIdentifier(SelectedItems.GetObjectAt(0));
                }

                return -1;
            }
            set
            {
                int itemCount = (itemsCollection is null) ? 0 : itemsCollection.Count;

                if (value < -1 || value >= itemCount)
                {
                    throw new ArgumentOutOfRangeException();
                }

                if (SelectionMode == SelectionMode.One && value != -1)
                {
                    // Single select an individual value.
                    int currentIndex = SelectedIndex;

                    if (currentIndex != value)
                    {
                        if (currentIndex != -1)
                        {
                            SelectedItems.SetSelected(currentIndex, false);
                        }
                        SelectedItems.SetSelected(value, true);
                    }
                }
                else if (value == -1)
                {
                    if (SelectedIndex != -1)
                    {
                        ClearSelected();
                        // ClearSelected raises OnSelectedIndexChanged for us
                    }
                }
                else
                {
                    if (!SelectedItems.GetSelected(value))
                    {
                        SelectedItems.SetSelected(value, true);
                    }
                }
            }
        }

        public object SelectedItem
        {
            get
            {
                if (SelectedItems.Count > 0)
                {
                    return SelectedItems[0];
                }

                return null;
            }
            set
            {
                if (itemsCollection != null)
                {
                    if (value != null)
                    {
                        int index = itemsCollection.IndexOf(value);
                        if (index != -1)
                        {
                            SelectedIndex = index;
                        }
                    }
                    else
                    {
                        SelectedIndex = -1;
                    }
                }
            }
        }

        public SelectedObjectCollection SelectedItems
        {
            get
            {
                if (selectedItems is null)
                {
                    selectedItems = new SelectedObjectCollection(this);
                }
                return selectedItems;
            }
        }

        public SelectedIndexCollection SelectedIndices
        {
            get
            {
                if (selectedIndices is null)
                {
                    selectedIndices = new SelectedIndexCollection(this);
                }
                return selectedIndices;
            }
        }

        public void ClearSelected()
        {
            int itemCount = (itemsCollection is null) ? 0 : itemsCollection.Count;
            for (int x = 0; x < itemCount; x++)
            {
                if (SelectedItems.GetSelected(x))
                {
                    SelectedItems.SetSelected(x, false);
                }
            }
        }

        public void SetSelected(int index, bool value)
        {
            SelectedItems.SetSelected(index, value);
            SelectedItems.Dirty();
        }
    }

    public partial class ComboBox : ListControl
    {
        private int selectedIndex = -1;
        private ObjectCollection _itemsCollection;
        public ComboBoxStyle DropDownStyle;
        public int DropDownWidth;
        public int DropDownHeight;
        public int MaxDropDownItems;
        private bool sorted;
        public object Tag;

        protected override Size DefaultSize {
            get {
                return new Size(121,
                    21);    // Approximate value, may be incorrect
            }
        }

        public bool Sorted
        {
            get
            {
                return sorted;
            }
            set
            {
                if (sorted != value)
                {
                    sorted = value;
                    SelectedIndex = -1;
                }
            }
        }
        public ObjectCollection Items
        {
            get
            {
                if (_itemsCollection == null)
                {
                    _itemsCollection = new ObjectCollection(this);
                }
                return _itemsCollection;
            }
        }

        public override int SelectedIndex
        {
            get
            {
                return selectedIndex;
            }
            set
            {
                if (SelectedIndex != value)
                {
                    selectedIndex = value;

                    UpdateText();
                }
            }
        }

        public object SelectedItem 
        {
            get
            {
                int index = SelectedIndex;
                return (index == -1) ? null : Items[index];
            }
            set
            {
                int x = -1;

                if (_itemsCollection != null)
                {
                    //
                    if (value != null)
                    {
                        x = _itemsCollection.IndexOf(value);
                    }
                    else
                    {
                        SelectedIndex = -1;
                    }
                }

                if (x != -1)
                {
                    SelectedIndex = x;
                }
            }
        }

        private void UpdateText()
        {
            string s = null;

            if (SelectedIndex != -1)
            {
                object item = Items[SelectedIndex];
                if (item != null)
                {
                    s = item.ToString();
                }
            }

            Text = s;
        }
    }

    public class CheckedListBox : ListBox
    {
        public class CheckedIndexCollection : List<int> { }
        public class CheckedItemCollection : List<object> { }

        public event ItemCheckEventHandler ItemCheck;

        public bool CheckOnClick;
        public CheckedIndexCollection CheckedIndices = new CheckedIndexCollection();        //
        public CheckedItemCollection CheckedItems { get; } = new CheckedItemCollection();
        //

        public void SetItemChecked(int index, bool check)
        {
            if (check)
            {
                if (!CheckedIndices.Contains(index))
                {
                    CheckedIndices.Add(index);
                    CheckedItems.Add(Items[index]);
                }
            }
            else
            {
                CheckedIndices.Remove(index);
                CheckedItems.Remove(Items[index]);
            }
        }
    }

    public class Panel : ScrollableControl
    {
        private BorderStyle borderStyle;

        /// <summary>Initializes a new instance of the <see cref="T:System.Windows.Forms.Panel" /> class.</summary>
        public Panel()
        {
            this.TabStop = false;
        }

        public BorderStyle BorderStyle
        {
            get => this.borderStyle;
            set => this.borderStyle = value;
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(200,
                    100);
            }
        }
    }


    public class DateTimePicker : Control
    {
        public event EventHandler ValueChanged;

        public bool Checked;
        public string CustomFormat;
        public LeftRightAlignment DropDownAlign;
        public DateTimePickerFormat Format;
        public DateTime MaxDate;
        public DateTime MinDate;
        public bool ShowCheckBox;
        public bool ShowUpDown;
        public DateTime Value = DateTime.Now;                                               //

        protected override Size DefaultSize {
            get {
                return new Size(200, 
                    20);    // Approximate value, may be incorrect
            }
        }
    }

    public class Label : Control
    {
        public bool AutoSize;
        public ContentAlignment TextAlign = ContentAlignment.TopLeft;                       //

        protected override Size DefaultSize {
            get {
                return new Size(100, 
                    23);    // Approximate value, may be incorrect
            }
        }
    }

    public sealed class SelectionRange
    {
    }

    public class MonthCalendar : Control
    {
        public event DateRangeEventHandler DateChanged;

        public Size CalendarDimensions;
        public Day FirstDayOfWeek;
        public DateTime MaxDate;
        public int MaxSelectionCount;
        public DateTime MinDate;
        public bool ShowToday;
        public bool ShowTodayCircle;
        public bool ShowWeekNumbers;
        public DateTime TodayDate;
        public DateTime[] AnnuallyBoldedDates;
        public DateTime[] BoldedDates;
        public DateTime[] MonthlyBoldedDates;
        public DateTime SelectionEnd;
        public SelectionRange SelectionRange;
        public DateTime SelectionStart = DateTime.Now;                                      //
    }

    public class RadioButton : ButtonBase
    {
        private bool isChecked = false; 
        public event EventHandler CheckedChanged;

        public ContentAlignment CheckAlign;
        public bool Checked {
            get { return isChecked; }
            set 
            {
                if(isChecked != value)
                {
                    isChecked = value;
                    if (value && Parent != null)
                    {
                        var controls = this.Parent.Controls;
                        foreach (var control in controls)
                        {
                            if (control is RadioButton && control != this)
                                (control as RadioButton).isChecked = false;
                        }
                    }
                }
            }
        }                                                      //

        protected override Size DefaultSize {
            get {
                return new Size(104, 24);
            }
        }
    }

    public class TextBox : Control
    {
        public bool AcceptsReturn;
        public bool AcceptsTab;
        public CharacterCasing CharacterCasing;
        public int MaxLength = 32767;
        public bool Multiline = false;                                                      //
        public bool ReadOnly;
        public ScrollBars ScrollBars;
        public HorizontalAlignment TextAlign = HorizontalAlignment.Left;                    //
        public bool UseSystemPasswordChar;
        public bool WordWrap;
        public BorderStyle BorderStyle;

        public void SelectAll() { }

        protected override Size DefaultSize {
            get {
                return new Size(100, 
                    20);    // Approximate value, may be incorrect
            }
        }


    }

	public enum AutoScaleMode
    {
        None = 0,
        Font = 1,
        Dpi = 2,
        Inherit = 3
    }

    public class Form : ScrollableControl
    {
        public event EventHandler Load;
        public event FormClosedEventHandler FormClosed;
        public event FormClosingEventHandler FormClosing;
        public event EventHandler Shown;

        public Button AcceptButton;
        public Button CancelButton;
        public FormBorderStyle FormBorderStyle;
        public Size ClientSize;
        public FormStartPosition StartPosition;
        public bool ShowIcon;
        public bool ShowInTaskbar;
        public bool MinimizeBox;
        public bool MaximizeBox;
        public DialogResult DialogResult;
        public SizeF AutoScaleDimensions;
        public AutoScaleMode AutoScaleMode;

        public DialogResult ShowDialog()
        {
            return DialogResult.OK;
        }

        protected override Size DefaultSize {
            get {
                return new Size(300, 300);
            }
        }

        public void SuspendLayout()
        {
        }

        public void ResumeLayout()
        {
        }

        public int Dpi()
        {
            return 96;
        }

        public float FontDpiMultiplier()
        {
            return 1f;
        }

    }

    public class BaseForm : Form
    {
        public event EventHandler DpiChanged;
        public virtual void UpdateDpiDependencies()
        {
        }
    }


    public class Timer : Component
    {
        public event EventHandler Tick;
        public int Interval;
        public bool Enabled;

        public void Start() { if (Tick != null) Tick(this, EventArgs.Empty); }
        public void Stop() { }
    }

    public sealed class MessageBox
    {
        public static DialogResult Show(string text)
        {
            return DialogResult.None;
        }
    }


    public sealed class Application
    {
        public static void DoEvents()
        {

        }
    }

    public class SystemInformation
    {
        public static bool HighContrast;
    }

    public sealed class ControlPaint
    {
        public static void DrawFocusRectangle(Graphics g, Rectangle r) { }
    }

    public class ScrollableControl : Control
    {
        public Control ActiveControl;

    }
}

#pragma warning restore FR0000 // Field must be texted in lowerCamelCase.
#pragma warning restore 1591
