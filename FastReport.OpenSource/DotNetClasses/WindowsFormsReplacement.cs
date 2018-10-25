using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using FastReport.Utils;
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
    #endregion


    public class KeyEventArgs : EventArgs
    {
    }

    public class KeyPressEventArgs : EventArgs
    {
    }

    public class MouseEventArgs : EventArgs
    {
    }

    public class PaintEventArgs : EventArgs
    {
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
    }

    public class FormClosingEventArgs : CancelEventArgs
    {
        private CloseReason FCloseReason;

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
        private CloseReason FCloseReason;

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

    public class Cursor
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
        public Control Parent;                                                              //

        public Color BackColor;
        public Cursor Cursor;
        public bool Enabled = true;                                                         //
        public Font Font = DrawUtils.DefaultFont;                                           //
        public Color ForeColor;
        public RightToLeft RightToLeft;
        public int TabIndex;
        public bool TabStop;
        public string Text = "";                                                            //
        public DockStyle Dock;
        public AnchorStyles Anchor;
        public bool Visible = true;                                                         //
        public int Left;                                                                    //
        public int Top;                                                                     //
        public int Width;                                                                   //
        public int Height;                                                                  //

        public IntPtr Handle = IntPtr.Zero;

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

        public void BringToFront() { }
        public void Focus() { }
        public void Hide() { }
        public void Show() { }
    }

    public class ButtonBase : Control
    {
        public bool AutoSize;
        public Image Image;
        public ContentAlignment ImageAlign;
        public ContentAlignment TextAlign = ContentAlignment.MiddleCenter;                  //
        public TextImageRelation TextImageRelation;
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

        public event EventHandler CheckedChanged;
    }

    public class ListControl : Control
    {
        public class ObjectCollection : List<object> { }

        public event EventHandler SelectedIndexChanged;                                     //
        public event MeasureItemEventHandler MeasureItem;
        public event DrawItemEventHandler DrawItem;

        public bool Sorted;
        public DrawMode DrawMode;
        public int ItemHeight;
        public ObjectCollection Items = new ObjectCollection();                             //
        public int SelectedIndex;                                                           //
        public object SelectedItem;
    }

    public class ListBox : ListControl
    {
        public class SelectedIndexCollection : List<int> { }
        public class SelectedObjectCollection : List<object> { }

        public bool IntegralHeight;
        public int ColumnWidth;
        public bool MultiColumn;
        public SelectionMode SelectionMode;
        public bool UseTabStops;
        public SelectedIndexCollection SelectedIndices = new SelectedIndexCollection();
        public SelectedObjectCollection SelectedItems = new SelectedObjectCollection();
    }

    public class ComboBox : ListControl
    {
        public ComboBoxStyle DropDownStyle;
        public int DropDownWidth;
        public int DropDownHeight;
        public int MaxDropDownItems;
    }

    public class CheckedListBox : ListBox
    {
        public class CheckedIndexCollection : List<int> { }
        public class CheckedItemCollection : List<object> { }

        public event ItemCheckEventHandler ItemCheck;

        public bool CheckOnClick;
        public CheckedIndexCollection CheckedIndices = new CheckedIndexCollection();        //
        public CheckedItemCollection CheckedItems = new CheckedItemCollection();            //

        public void SetItemChecked(int index, bool check)
        {
            if (check)
            {
                if (!CheckedIndices.Contains(index)) CheckedIndices.Add(index);
            }
            else
            {
                CheckedIndices.Remove(index);
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
    }

    public class Label : Control
    {
        public bool AutoSize;
        public ContentAlignment TextAlign = ContentAlignment.TopLeft;                       //
    }

    public class SelectionRange
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
        public event EventHandler CheckedChanged;

        public ContentAlignment CheckAlign;
        public bool Checked = false;                                                        //
    }

    public class TextBox : Control
    {
        public bool AcceptsReturn;
        public bool AcceptsTab;
        public CharacterCasing CharacterCasing;
        public int MaxLength;
        public bool Multiline = false;                                                      //
        public bool ReadOnly;
        public ScrollBars ScrollBars;
        public HorizontalAlignment TextAlign = HorizontalAlignment.Left;                    //
        public bool UseSystemPasswordChar;
        public bool WordWrap;
    }

    public class Form : Control
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

        public DialogResult ShowDialog()
        {
            return DialogResult.OK;
        }
    }

    public class Timer : Component
    {
        public event EventHandler Tick;
        public int Interval;

        public void Start() { if (Tick != null) Tick(this, EventArgs.Empty); }
        public void Stop() { }
    }

    public class MessageBox
    {
        public static DialogResult Show(string text)
        {
            return DialogResult.None;
        }
    }


    internal sealed class Application
    {
        public static void DoEvents()
        {

        }
    }
}



#pragma warning restore FR0000 // Field must be texted in lowerCamelCase.
#pragma warning restore 1591