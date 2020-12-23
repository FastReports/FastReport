using System;
using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel;
using FastReport.Utils;
using System.Windows.Forms;
using System.Drawing.Design;

namespace FastReport
{
    /// <summary>
    /// The automatic shift mode.
    /// </summary>
    public enum ShiftMode
    {
        /// <summary>
        /// Do not shift the object.
        /// </summary>
        Never,

        /// <summary>
        /// Shift the object up or down if any object above it shrinks or grows.
        /// </summary>
        Always,

        /// <summary>
        /// Shift the object up or down if any object above it shrinks or grows. 
        /// Objects must have overlapped x-coordinates.
        /// </summary>
        WhenOverlapped
    }

    /// <summary>
    /// Specifies where to print an object.
    /// </summary>
    [Flags]
    public enum PrintOn
    {
        /// <summary>
        /// Do not print the object.
        /// </summary>
        None = 0,

        /// <summary>
        /// Print the object on the first page. If this flag is not set, the object will not
        /// be printed on the first page.
        /// </summary>
        FirstPage = 1,

        /// <summary>
        /// Print the object on the last page. If this flag is not set, the object will not
        /// be printed on the last page. You should set the report's double pass option to make
        /// it work correctly.
        /// </summary>
        LastPage = 2,

        /// <summary>
        /// Print the object on odd pages only.
        /// </summary>
        OddPages = 4,

        /// <summary>
        /// Print the object on even pages only.
        /// </summary>
        EvenPages = 8,

        /// <summary>
        /// Print the object on band with "Repeat on Every Page" flag when that band is repeated. 
        /// </summary>
        RepeatedBand = 16,

        /// <summary>
        /// Print the object if the report has single page only.
        /// </summary>
        SinglePage = 32
    }


    /// <summary>
    /// Specifies the style properties to use when style is applied.
    /// </summary>
    public enum StylePriority
    {
        /// <summary>
        /// Use the fill property of the style.
        /// </summary>
        UseFill,

        /// <summary>
        /// Use all style properties.
        /// </summary>
        UseAll
    }

    /// <summary>
    /// Base class for all report objects.
    /// </summary>
    public abstract partial class ReportComponentBase : ComponentBase
    {
        #region Fields
        private bool exportable;
        private string exportableExpression;
        private Border border;
        private FillBase fill;
        private string bookmark;
        private Hyperlink hyperlink;
        private bool canGrow;
        private bool canShrink;
        private bool growToBottom;
        private ShiftMode shiftMode;
        private string style;
        private string evenStyle;
        private string hoverStyle;
        private StylePriority evenStylePriority;
        private PrintOn printOn;
        private string beforePrintEvent;
        private string afterPrintEvent;
        private string afterDataEvent;
        private string clickEvent;
        private bool flagSimpleBorder;
        private bool flagUseBorder;
        private bool flagUseFill;
        private bool flagPreviewVisible;
        private bool flagSerializeStyle;
        private bool flagProvidesHyperlinkValue;
        private RectangleF savedBounds;
        private bool savedVisible;
        private string savedBookmark;
        private Border savedBorder;
        private FillBase savedFill;
        private Cursor cursor;
        private string mouseMoveEvent;
        private string mouseUpEvent;
        private string mouseDownEvent;
        private string mouseEnterEvent;
        private string mouseLeaveEvent;
        #endregion

        #region Properties
        /// <summary>
        /// This event occurs before the object is added to the preview pages.
        /// </summary>
        public event EventHandler BeforePrint;

        /// <summary>
        /// This event occurs after the object was added to the preview pages.
        /// </summary>
        public event EventHandler AfterPrint;

        /// <summary>
        /// This event occurs after the object was filled with data.
        /// </summary>
        public event EventHandler AfterData;

        /// <summary>
        /// This event occurs when the user clicks the object in the preview window.
        /// </summary>
        public event EventHandler Click;

        /// <summary>
        /// Gets or sets a value that determines if the object can be exported.
        /// </summary>
        [DefaultValue(true)]
        [Category("Behavior")]
        public bool Exportable
        {
            get { return exportable; }
            set { exportable = value; }
        }

        /// <summary>
        /// Gets or sets a string containing expression that determines should be object exported.
        /// </summary>
        [DefaultValue("")]
        [Category("Behavior")]
        [Editor("FastReport.TypeEditors.ExpressionEditor, FastReport", typeof(UITypeEditor))]
        public virtual string ExportableExpression
        {
            get { return exportableExpression; }
            set { exportableExpression = value; }
        }

        /// <summary>
        /// Gets or sets an object's border.
        /// </summary>
        [Category("Appearance")]
        public virtual Border Border
        {
            get { return border; }
            set
            {
                border = value;
                if (!String.IsNullOrEmpty(Style))
                    Style = "";
            }
        }

        /// <summary>
        /// Gets or sets an object's fill.
        /// </summary>
        /// <remarks>
        /// The fill can be one of the following types: <see cref="SolidFill"/>, <see cref="LinearGradientFill"/>, 
        /// <see cref="PathGradientFill"/>, <see cref="HatchFill"/>.
        /// <para/>To set the solid fill color, use the simpler <see cref="FillColor"/> property.
        /// </remarks>
        /// <example>This example shows how to set the new fill and change its properties:
        /// <code>
        /// textObject1.Fill = new SolidFill(Color.Green);
        /// (textObject1.Fill as SolidFill).Color = Color.Red;
        /// </code>
        /// </example>          
        [Category("Appearance")]
        [EditorAttribute("FastReport.TypeEditors.FillEditor, FastReport",typeof(UITypeEditor))]
        public virtual FillBase Fill
        {
            get
            {
                return fill; 
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Fill");
                fill = value;
                if (!String.IsNullOrEmpty(Style))
                    Style = "";
            }
        }

        /// <summary>
        /// Gets or sets the fill color in a simple manner.
        /// </summary>
        /// <remarks>
        /// This property can be used in a report script to change the fill color of the object. It is 
        /// equivalent to: <code>reportComponent1.Fill = new SolidFill(color);</code>
        /// </remarks>
        [Browsable(false)]
        public Color FillColor
        {
            get { return Fill is SolidFill ? (Fill as SolidFill).Color : Color.Transparent; }
            set { Fill = new SolidFill(value); }
        }

        /// <summary>
        /// Gets or sets a bookmark expression.
        /// </summary>
        /// <remarks>
        /// This property can contain any valid expression that returns a bookmark name. This can be, for example,
        /// a data column. To navigate to a bookmark, you have to use the <see cref="Hyperlink"/> property.
        /// </remarks>

        [Category("Navigation")]
        [Editor("FastReport.TypeEditors.ExpressionEditor, FastReport", typeof(UITypeEditor))]
        public string Bookmark
        {
            get { return bookmark; }
            set { bookmark = value; }
        }

        /// <summary>
        /// Gets or sets a hyperlink.
        /// </summary>
        /// <remarks>
        /// <para>The hyperlink is used to define clickable objects in the preview. 
        /// When you click such object, you may navigate to the external url, the page number, 
        /// the bookmark defined by other report object, or display the external report. 
        /// Set the <b>Kind</b> property of the hyperlink to select appropriate behavior.</para>
        /// <para>Usually you should set the <b>Expression</b> property of the hyperlink to
        /// any valid expression that will be calculated when this object is about to print.
        /// The value of an expression will be used for navigation.</para>
        /// <para>If you want to navigate to
        /// something fixed (URL or page number, for example) you also may set the <b>Value</b>
        /// property instead of <b>Expression</b>.</para>
        /// </remarks>
        [Category("Navigation")]
        [Editor("FastReport.TypeEditors.HyperlinkEditor, FastReport", typeof(UITypeEditor))]
        public Hyperlink Hyperlink
        {
            get { return hyperlink; }
        }

        /// <summary>
        /// Determines if the object can grow.
        /// </summary>
        /// <remarks>
        /// This property is applicable to the bands or text objects that can contain several text lines.
        /// If the property is set to <b>true</b>, object will grow to display all the information that it contains.
        /// </remarks>
        [DefaultValue(false)]
        [Category("Behavior")]
        public bool CanGrow
        {
            get { return canGrow; }
            set { canGrow = value; }
        }

        /// <summary>
        /// Determines if the object can shrink.
        /// </summary>
        /// <remarks>
        /// This property is applicable to the bands or text objects that can contain several text lines.
        /// If the property is set to true, object can shrink to remove the unused space.
        /// </remarks>
        [DefaultValue(false)]
        [Category("Behavior")]
        public bool CanShrink
        {
            get { return canShrink; }
            set { canShrink = value; }
        }

        /// <summary>
        /// Determines if the object must grow to the band's bottom side.
        /// </summary>
        /// <remarks>
        /// If the property is set to true, object grows to the bottom side of its parent. This is useful if
        /// you have several objects on a band, and some of them can grow or shrink.
        /// </remarks>
        [DefaultValue(false)]
        [Category("Behavior")]
        public bool GrowToBottom
        {
            get { return growToBottom; }
            set { growToBottom = value; }
        }

        /// <summary>
        /// Gets or sets a shift mode of the object.
        /// </summary>
        /// <remarks>
        /// See <see cref="FastReport.ShiftMode"/> enumeration for details.
        /// </remarks>
        [DefaultValue(ShiftMode.Always)]
        [Category("Behavior")]
        public ShiftMode ShiftMode
        {
            get { return shiftMode; }
            set { shiftMode = value; }
        }

        /// <summary>
        /// Gets or sets the style name.
        /// </summary>
        /// <remarks>
        /// Style is a set of common properties such as border, fill, font, text color. The <b>Report</b>
        /// has a set of styles in the <see cref="Report.Styles"/> property. 
        /// </remarks>

        [Category("Appearance")]
        [Editor("FastReport.TypeEditors.StyleEditor, FastReport", typeof(UITypeEditor))]
        public string Style
        {
            get { return style; }
            set
            {
                ApplyStyle(value);
                style = value;
            }
        }

        /// <summary>
        /// Gets or sets a style name that will be applied to even band rows.
        /// </summary>
        /// <remarks>
        /// Style with this name must exist in the <see cref="Report.Styles"/> collection.
        /// </remarks>
        [Category("Appearance")]
        [Editor("FastReport.TypeEditors.StyleEditor, FastReport", typeof(UITypeEditor))]
        public string EvenStyle
        {
            get { return evenStyle; }
            set { evenStyle = value; }
        }

        /// <summary>
        /// Gets or sets a style name that will be applied to this object when the mouse pointer is over it.
        /// </summary>
        /// <remarks>
        /// Style with this name must exist in the <see cref="Report.Styles"/> collection.
        /// </remarks>

        [Category("Appearance")]
        [Editor("FastReport.TypeEditors.StyleEditor, FastReport", typeof(UITypeEditor))]
        public string HoverStyle
        {
            get { return hoverStyle; }
            set { hoverStyle = value; }
        }

        /// <summary>
        /// Gets or sets a value that determines which properties of the even style to use.
        /// </summary>
        /// <remarks>
        /// Usually you will need only the Fill property of the even style to be applied. If you want to 
        /// apply all style settings, set this property to <b>StylePriority.UseAll</b>.
        /// </remarks>
        [DefaultValue(StylePriority.UseFill)]
        [Category("Appearance")]
        public StylePriority EvenStylePriority
        {
            get { return evenStylePriority; }
            set { evenStylePriority = value; }
        }

        /// <summary>
        /// Gets or sets a value that determines where to print the object.
        /// </summary>
        /// <remarks>
        /// See the <see cref="FastReport.PrintOn"/> enumeration for details.
        /// </remarks>
        [DefaultValue(PrintOn.FirstPage | PrintOn.LastPage | PrintOn.OddPages | PrintOn.EvenPages | PrintOn.RepeatedBand | PrintOn.SinglePage)]

        [Category("Behavior")]
        [Editor("FastReport.TypeEditors.FlagsEditor, FastReport", typeof(UITypeEditor))]
        public PrintOn PrintOn
        {
            get { return printOn; }
            set { printOn = value; }
        }

        /// <summary>
        /// Gets or sets a script event name that will be fired before the object will be printed in the preview page.
        /// </summary>
        [Category("Build")]
        public string BeforePrintEvent
        {
            get { return beforePrintEvent; }
            set { beforePrintEvent = value; }
        }

        /// <summary>
        /// Gets or sets a script event name that will be fired after the object was printed in the preview page.
        /// </summary>
        [Category("Build")]
        public string AfterPrintEvent
        {
            get { return afterPrintEvent; }
            set { afterPrintEvent = value; }
        }

        /// <summary>
        /// Gets or sets a script event name that will be fired after the object was filled with data.
        /// </summary>
        [Category("Build")]
        public string AfterDataEvent
        {
            get { return afterDataEvent; }
            set { afterDataEvent = value; }
        }

        /// <summary>
        /// Gets or sets a script event name that will be fired when the user click the object in the preview window.
        /// </summary>
        [Category("Preview")]
        public string ClickEvent
        {
            get { return clickEvent; }
            set { clickEvent = value; }
        }

        /// <summary>
        /// Determines if the object has custom border and use only <b>Border.Width</b>, <b>Border.Style</b> and 
        /// <b>Border.Color</b> properties.
        /// </summary>
        /// <remarks>
        /// This flag is used to disable some toolbar buttons when such object is selected. Applicable to the
        /// ShapeObject and LineObject.
        /// </remarks>
        [Browsable(false)]
        public bool FlagSimpleBorder
        {
            get { return flagSimpleBorder; }
            set { flagSimpleBorder = value; }
        }

        /// <summary>
        /// Determines if the object uses the <b>Border</b>.
        /// </summary>
        /// <remarks>
        /// This flag is used to disable some toolbar buttons when such object is selected.
        /// </remarks>
        [Browsable(false)]
        public bool FlagUseBorder
        {
            get { return flagUseBorder; }
            set { flagUseBorder = value; }
        }

        /// <summary>
        /// Determines if the object uses the fill.
        /// </summary>
        /// <remarks>
        /// This flag is used to disable some toolbar buttons when such object is selected.
        /// </remarks>
        [Browsable(false)]
        public bool FlagUseFill
        {
            get { return flagUseFill; }
            set { flagUseFill = value; }
        }

        /// <summary>
        /// Gets or sets a value indicates that object should not be added to the preview.
        /// </summary>
        [Browsable(false)]
        public bool FlagPreviewVisible
        {
            get { return flagPreviewVisible; }
            set { flagPreviewVisible = value; }
        }

        /// <summary>
        /// Determines if serializing the Style property is needed.
        /// </summary>
        /// <remarks>
        /// The <b>Style</b> property must be serialized last. Some ancestor classes may turn off the standard Style 
        /// serialization and serialize it by themselves.
        /// </remarks>
        [Browsable(false)]
        public bool FlagSerializeStyle
        {
            get { return flagSerializeStyle; }
            set { flagSerializeStyle = value; }
        }

        /// <summary>
        /// Determines if an object can provide the hyperlink value automatically.
        /// </summary>
        /// <remarks>
        /// This flag is used in complex objects such as Matrix or Chart. These objects can provide
        /// a hyperlink value automatically, depending on where you click.
        /// </remarks>
        [Browsable(false)]
        public bool FlagProvidesHyperlinkValue
        {
            get { return flagProvidesHyperlinkValue; }
            set { flagProvidesHyperlinkValue = value; }
        }

        /// <summary>
        /// Gets an object's parent band.
        /// </summary>
        internal BandBase Band
        {
            get
            {
                if (this is BandBase)
                    return this as BandBase;

                Base c = Parent;
                while (c != null)
                {
                    if (c is BandBase)
                        return c as BandBase;
                    c = c.Parent;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets an object's parent data band.
        /// </summary>
        internal DataBand DataBand
        {
            get
            {
                if (this is DataBand)
                    return this as DataBand;

                Base c = Parent;
                while (c != null)
                {
                    if (c is DataBand)
                        return c as DataBand;
                    c = c.Parent;
                }

                ObjectCollection pageBands = Page.AllObjects;
                foreach (Base c1 in pageBands)
                {
                    if (c1 is DataBand)
                        return c1 as DataBand;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets or sets an object's cursor shape.
        /// </summary>
        /// <remarks>
        /// This property is used in the preview mode.
        /// </remarks>
        [Category("Appearance")]
        public Cursor Cursor
        {
            get { return cursor; }
            set { cursor = value; }
        }

        /// <summary>
        /// Gets or sets a script event name that will be fired when the user 
        /// moves the mouse over the object in the preview window.
        /// </summary>
        [Category("Preview")]
        public string MouseMoveEvent
        {
            get { return mouseMoveEvent; }
            set { mouseMoveEvent = value; }
        }

        /// <summary>
        /// Gets or sets a script event name that will be fired when the user 
        /// releases the mouse button in the preview window.
        /// </summary>
        [Category("Preview")]
        public string MouseUpEvent
        {
            get { return mouseUpEvent; }
            set { mouseUpEvent = value; }
        }

        /// <summary>
        /// Gets or sets a script event name that will be fired when the user 
        /// clicks the mouse button in the preview window.
        /// </summary>
        [Category("Preview")]
        public string MouseDownEvent
        {
            get { return mouseDownEvent; }
            set { mouseDownEvent = value; }
        }

        /// <summary>
        /// Gets or sets a script event name that will be fired when the
        /// mouse enters the object's bounds in the preview window.
        /// </summary>
        [Category("Preview")]
        public string MouseEnterEvent
        {
            get { return mouseEnterEvent; }
            set { mouseEnterEvent = value; }
        }

        /// <summary>
        /// Gets or sets a script event name that will be fired when the
        /// mouse leaves the object's bounds in the preview window.
        /// </summary>
        [Category("Preview")]
        public string MouseLeaveEvent
        {
            get { return mouseLeaveEvent; }
            set { mouseLeaveEvent = value; }
        }
        #endregion

        #region Public Methods
        /// <inheritdoc/>
        public override void Assign(Base source)
        {
            base.Assign(source);

            ReportComponentBase src = source as ReportComponentBase;
            Exportable = src.Exportable;
            ExportableExpression = src.ExportableExpression;
            Border = src.Border.Clone();
            Fill = src.Fill.Clone();
            Bookmark = src.Bookmark;
            Hyperlink.Assign(src.Hyperlink);
            CanGrow = src.CanGrow;
            CanShrink = src.CanShrink;
            GrowToBottom = src.GrowToBottom;
            ShiftMode = src.ShiftMode;
            style = src.Style;
            EvenStyle = src.EvenStyle;
            HoverStyle = src.HoverStyle;
            EvenStylePriority = src.EvenStylePriority;
            PrintOn = src.PrintOn;
            BeforePrintEvent = src.BeforePrintEvent;
            AfterPrintEvent = src.AfterPrintEvent;
            AfterDataEvent = src.AfterDataEvent;
            ClickEvent = src.ClickEvent;
            Cursor = src.Cursor;
            MouseMoveEvent = src.MouseMoveEvent;
            MouseUpEvent = src.MouseUpEvent;
            MouseDownEvent = src.MouseDownEvent;
            MouseEnterEvent = src.MouseEnterEvent;
            MouseLeaveEvent = src.MouseLeaveEvent;
        }

        /// <summary>
        /// Applies the style settings.
        /// </summary>
        /// <param name="style">Style to apply.</param>
        public virtual void ApplyStyle(Style style)
        {
            if (style.ApplyBorder)
                Border = style.Border.Clone();
            if (style.ApplyFill)
                Fill = style.Fill.Clone();
        }

        internal void ApplyStyle(string style)
        {
            if (!String.IsNullOrEmpty(style) && Report != null)
            {
                StyleCollection styles = Report.Styles;
                int index = styles.IndexOf(style);
                if (index != -1)
                    ApplyStyle(styles[index]);
            }
        }

        internal void ApplyEvenStyle()
        {
            if (!String.IsNullOrEmpty(EvenStyle) && Report != null)
            {
                StyleCollection styles = Report.Styles;
                int index = styles.IndexOf(EvenStyle);
                if (index != -1)
                {
                    Style style = styles[index];
                    if (EvenStylePriority == StylePriority.UseFill)
                        Fill = style.Fill.Clone();
                    else
                        ApplyStyle(style);
                }
            }
        }

        /// <summary>
        /// Saves the current style.
        /// </summary>
        public virtual void SaveStyle()
        {
            savedBorder = Border;
            savedFill = Fill;
        }

        /// <summary>
        /// Restores the current style.
        /// </summary>
        public virtual void RestoreStyle()
        {
            Border = savedBorder;
            Fill = savedFill;
        }

        /// <summary>
        /// Draws the object's background.
        /// </summary>
        /// <param name="e">Draw event arguments.</param>
        public void DrawBackground(FRPaintEventArgs e)
        {
            if (Width < 0.01 || Height < 0.01)
                return;
            Fill.Draw(e, AbsBounds);
        }

        /// <inheritdoc/>
        public override void Draw(FRPaintEventArgs e)
        {
            DrawBackground(e);
            base.Draw(e);
        }

        /// <summary>
        /// Determines if the object is visible on current drawing surface.
        /// </summary>
        /// <param name="e">Draw event arguments.</param>
        public virtual bool IsVisible(FRPaintEventArgs e)
        {
            RectangleF objRect = new RectangleF(AbsLeft * e.ScaleX, AbsTop * e.ScaleY,
              Width * e.ScaleX + 1, Height * e.ScaleY + 1);
            return e.Graphics.IsVisible(objRect);
        }

        /// <inheritdoc/>
        public override void Serialize(FRWriter writer)
        {
            ReportComponentBase c = writer.DiffObject as ReportComponentBase;
            base.Serialize(writer);

            if (Exportable != c.Exportable)
                writer.WriteBool("Exportable", Exportable);
            if (ExportableExpression != c.ExportableExpression)
                writer.WriteStr("ExportableExpression", ExportableExpression);
            Border.Serialize(writer, "Border", c.Border);
            //if(Fill != c.Fill)
                Fill.Serialize(writer, "Fill", c.Fill);
            if (Cursor != c.Cursor && !Config.WebMode)
                writer.WriteValue("Cursor", Cursor);
            Hyperlink.Serialize(writer, c.Hyperlink);
            if (Bookmark != c.Bookmark)
                writer.WriteStr("Bookmark", Bookmark);
            if (writer.SerializeTo != SerializeTo.Preview)
            {
                if (CanGrow != c.CanGrow)
                    writer.WriteBool("CanGrow", CanGrow);
                if (CanShrink != c.CanShrink)
                    writer.WriteBool("CanShrink", CanShrink);
                if (GrowToBottom != c.GrowToBottom)
                    writer.WriteBool("GrowToBottom", GrowToBottom);
                if (ShiftMode != c.ShiftMode)
                    writer.WriteValue("ShiftMode", ShiftMode);
                if (FlagSerializeStyle && Style != c.Style)
                    writer.WriteStr("Style", Style);
                if (EvenStyle != c.EvenStyle)
                    writer.WriteStr("EvenStyle", EvenStyle);
                if (EvenStylePriority != c.EvenStylePriority)
                    writer.WriteValue("EvenStylePriority", EvenStylePriority);
                if (HoverStyle != c.HoverStyle)
                    writer.WriteStr("HoverStyle", HoverStyle);
                if (PrintOn != c.PrintOn)
                    writer.WriteValue("PrintOn", PrintOn);
                if (BeforePrintEvent != c.BeforePrintEvent)
                    writer.WriteStr("BeforePrintEvent", BeforePrintEvent);
                if (AfterPrintEvent != c.AfterPrintEvent)
                    writer.WriteStr("AfterPrintEvent", AfterPrintEvent);
                if (AfterDataEvent != c.AfterDataEvent)
                    writer.WriteStr("AfterDataEvent", AfterDataEvent);
                if (ClickEvent != c.ClickEvent)
                    writer.WriteStr("ClickEvent", ClickEvent);
                if (MouseMoveEvent != c.MouseMoveEvent)
                    writer.WriteStr("MouseMoveEvent", MouseMoveEvent);
                if (MouseUpEvent != c.MouseUpEvent)
                    writer.WriteStr("MouseUpEvent", MouseUpEvent);
                if (MouseDownEvent != c.MouseDownEvent)
                    writer.WriteStr("MouseDownEvent", MouseDownEvent);
                if (MouseEnterEvent != c.MouseEnterEvent)
                    writer.WriteStr("MouseEnterEvent", MouseEnterEvent);
                if (MouseLeaveEvent != c.MouseLeaveEvent)
                    writer.WriteStr("MouseLeaveEvent", MouseLeaveEvent);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        public override void Deserialize(FRReader reader)
        {
            base.Deserialize(reader);
            Fill.Deserialize(reader, "Fill");
        }

        /// <summary>
        /// This method fires the <b>Click</b> event and the script code connected to the <b>ClickEvent</b>.
        /// </summary>
        /// <param name="e">Event data.</param>
        public virtual void OnClick(EventArgs e)
        {
            if (Click != null)
                Click(this, e);
            InvokeEvent(ClickEvent, e);
        }

        /// <inheritdoc/>
        public override void OnAfterLoad()
        {
            // if hyperlink is set to external report, we need to fix relative path to report
            Hyperlink.OnAfterLoad();
        }

        /// <summary>
        /// Checks if there are any listeners to the Click event.
        /// </summary>
        public bool HasClickListeners()
        {
            return Click != null;
        }
        #endregion

        #region Report Engine
        /// <summary>
        /// Initializes the object before running a report.
        /// </summary>
        /// <remarks>
        /// This method is used by the report engine, do not call it directly.
        /// </remarks>
        public virtual void InitializeComponent()
        {
            // update the component's style
            Style = Style;
            Fill.InitializeComponent();
        }

        /// <summary>
        /// Performs a finalization after the report is finished.
        /// </summary>
        /// <remarks>
        /// This method is used by the report engine, do not call it directly.
        /// </remarks>
        public virtual void FinalizeComponent()
        {
            Fill.FinalizeComponent();
        }

        /// <summary>
        /// Saves the object's state before printing it.
        /// </summary>
        /// <remarks>
        /// This method is called by the report engine before processing the object. 
        /// <para/>Do not call it directly. You may override it if you are developing a new FastReport component. 
        /// In this method you should save any object properties that may be changed during the object printing. 
        /// The standard implementation saves the object's bounds, visibility, bookmark and hyperlink.
        /// </remarks>
        public virtual void SaveState()
        {
            savedBounds = Bounds;
            savedVisible = Visible;
            savedBookmark = Bookmark;
            savedBorder = Border;
            savedFill = Fill;
            Hyperlink.SaveState();
        }

        /// <summary>
        /// Restores the object's state after printing it.
        /// </summary>
        /// <remarks>
        /// This method is called by the report engine after processing the object.
        /// <para/>Do not call it directly. You may override it if you are developing a new FastReport component. 
        /// In this method you should restore the object properties that were saved by the <see cref="SaveState"/> method.
        /// </remarks>
        public virtual void RestoreState()
        {
            Bounds = savedBounds;
            Visible = savedVisible;
            Bookmark = savedBookmark;
            Hyperlink.RestoreState();
            Border = savedBorder;
            Fill = savedFill;
        }

        /// <summary>
        /// Calculates the object's height.
        /// </summary>
        /// <returns>Actual object's height, in pixels.</returns>
        /// <remarks>
        /// Applicable to objects that contain several text lines, such as TextObject. Returns the height needed
        /// to display all the text lines.
        /// </remarks>
        public virtual float CalcHeight()
        {
            return Height;
        }

        /// <summary>
        /// Gets the data from a datasource that the object is connected to.
        /// </summary>
        /// <remarks>
        /// This method is called by the report engine before processing the object.
        /// <para/>Do not call it directly. You may override it if you are developing a new FastReport component. 
        /// In this method you should get the data from a datasource that the object is connected to.
        /// </remarks>
        public virtual void GetData()
        {
            Hyperlink.Calculate();

            if (!String.IsNullOrEmpty(Bookmark))
            {
                object value = Report.Calc(Bookmark);
                Bookmark = value == null ? "" : value.ToString();
            }
        }

        /// <inheritdoc/>
        public override string[] GetExpressions()
        {
            List<string> expressions = new List<string>();

            string[] baseExpressions = base.GetExpressions();
            if (baseExpressions != null)
            {
                expressions.AddRange(baseExpressions);
            }

            if (!String.IsNullOrEmpty(Hyperlink.Expression))
                expressions.Add(Hyperlink.Expression);
            if (!String.IsNullOrEmpty(Bookmark))
                expressions.Add(Bookmark);

            if (!String.IsNullOrEmpty(ExportableExpression))
            {
                expressions.Add(Code.CodeUtils.FixExpressionWithBrackets(ExportableExpression));
            }

            return expressions.ToArray();
        }

        /// <summary>
        /// This method fires the <b>BeforePrint</b> event and the script code connected to the <b>BeforePrintEvent</b>.
        /// </summary>
        /// <param name="e">Event data.</param>
        public virtual void OnBeforePrint(EventArgs e)
        {
            if (BeforePrint != null)
                BeforePrint(this, e);
            InvokeEvent(BeforePrintEvent, e);
        }

        /// <summary>
        /// This method fires the <b>AfterPrint</b> event and the script code connected to the <b>AfterPrintEvent</b>.
        /// </summary>
        /// <param name="e">Event data.</param>
        public virtual void OnAfterPrint(EventArgs e)
        {
            if (AfterPrint != null)
                AfterPrint(this, e);
            InvokeEvent(AfterPrintEvent, e);
        }

        /// <summary>
        /// This method fires the <b>AfterData</b> event and the script code connected to the <b>AfterDataEvent</b>.
        /// </summary>
        /// <param name="e">Event data.</param>
        public virtual void OnAfterData(EventArgs e)
        {
            if (AfterData != null)
                AfterData(this, e);
            InvokeEvent(AfterDataEvent, e);
        }

        internal void OnAfterData()
        {
            OnAfterData(EventArgs.Empty);
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportComponentBase"/> class with default settings.
        /// </summary>
        public ReportComponentBase()
        {
            border = new Border();
            fill = new SolidFill();
            hyperlink = new Hyperlink(this);
            bookmark = "";
            exportable = true;
            exportableExpression = "";
            flagUseFill = true;
            flagUseBorder = true;
            flagPreviewVisible = true;
            flagSerializeStyle = true;
            shiftMode = ShiftMode.Always;
            style = "";
            evenStyle = "";
            hoverStyle = "";
            printOn = PrintOn.FirstPage | PrintOn.LastPage | PrintOn.OddPages | PrintOn.EvenPages | PrintOn.RepeatedBand | PrintOn.SinglePage;
            beforePrintEvent = "";
            afterPrintEvent = "";
            afterDataEvent = "";
            clickEvent = "";
            cursor = Cursors.Default;
            mouseMoveEvent = "";
            mouseUpEvent = "";
            mouseDownEvent = "";
            mouseEnterEvent = "";
            mouseLeaveEvent = "";
            SetFlags(Flags.CanGroup, true);
            if (BaseName.EndsWith("Object"))
                BaseName = ClassName.Substring(0, ClassName.Length - 6);
        }
    }
}