using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using FastReport.Utils;
using System.Drawing.Design;

namespace FastReport
{
    /// <summary>
    /// Specifies a symbol that will be displayed when a <see cref="CheckBoxObject"/> is in the checked state.
    /// </summary>
    public enum CheckedSymbol
    {
        /// <summary>
        /// Specifies a check symbol.
        /// </summary>
        Check,

        /// <summary>
        /// Specifies a diagonal cross symbol.
        /// </summary>
        Cross,

        /// <summary>
        /// Specifies a plus symbol.
        /// </summary>
        Plus,

        /// <summary>
        /// Specifies a filled rectangle.
        /// </summary>
        Fill
    }

    /// <summary>
    /// Specifies a symbol that will be displayed when a <see cref="CheckBoxObject"/> is in the unchecked state.
    /// </summary>
    public enum UncheckedSymbol
    {
        /// <summary>
        /// Specifies no symbol.
        /// </summary>
        None,

        /// <summary>
        /// Specifies a diagonal cross symbol.
        /// </summary>
        Cross,

        /// <summary>
        /// Specifies a minus symbol.
        /// </summary>
        Minus,

        /// <summary>
        /// Specifies a slash symbol.
        /// </summary>
        Slash,

        /// <summary>
        /// Specifies a back slash symbol.
        /// </summary>
        BackSlash
    }

    /// <summary>
    /// Represents a check box object.
    /// </summary>
    public partial class CheckBoxObject : ReportComponentBase
    {
        #region Fields
        private bool isChecked;
        private CheckedSymbol checkedSymbol;
        private UncheckedSymbol uncheckedSymbol;
        private Color checkColor;
        private string dataColumn;
        private string expression;
        private float checkWidthRatio;
        private bool hideIfUnchecked;
        private bool editable;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or set a value indicating whether the check box is in the checked state.
        /// </summary>
        [DefaultValue(true)]
        [Category("Data")]
        public bool Checked
        {
            get { return isChecked; }
            set { isChecked = value; }
        }

        /// <summary>
        /// Gets or sets a symbol that will be displayed when the check box is in the checked state.
        /// </summary>
        [DefaultValue(CheckedSymbol.Check)]
        [Category("Appearance")]
        public CheckedSymbol CheckedSymbol
        {
            get { return checkedSymbol; }
            set { checkedSymbol = value; }
        }

        /// <summary>
        /// Gets or sets a symbol that will be displayed when the check box is in the unchecked state.
        /// </summary>
        [DefaultValue(UncheckedSymbol.None)]
        [Category("Appearance")]
        public UncheckedSymbol UncheckedSymbol
        {
            get { return uncheckedSymbol; }
            set { uncheckedSymbol = value; }
        }

        /// <summary>
        /// Gets or sets a color of the check symbol.
        /// </summary>
        [Category("Appearance")]
        [Editor("FastReport.TypeEditors.ColorEditor, FastReport", typeof(UITypeEditor))]
        public Color CheckColor
        {
            get { return checkColor; }
            set { checkColor = value; }
        }

        /// <summary>
        /// Gets or sets a data column name bound to this control.
        /// </summary>
        /// <remarks>
        /// Value must be in the form "[Datasource.Column]".
        /// </remarks>
        [Category("Data")]
        [Editor("FastReport.TypeEditors.DataColumnEditor, FastReport", typeof(UITypeEditor))]
        public string DataColumn
        {
            get { return dataColumn; }
            set { dataColumn = value; }
        }

        /// <summary>
        /// Gets or sets an expression that determines whether to show a check.
        /// </summary>
        [Category("Data")]
        [Editor("FastReport.TypeEditors.ExpressionEditor, FastReport", typeof(UITypeEditor))]
        public string Expression
        {
            get { return expression; }
            set { expression = value; }
        }

        /// <summary>
        /// Gets or sets the check symbol width ratio.
        /// </summary>
        /// <remarks>
        /// Valid values are from 0.2 to 2.
        /// </remarks>
        [DefaultValue(1f)]
        [Category("Appearance")]
        public float CheckWidthRatio
        {
            get { return checkWidthRatio; }
            set
            {
                if (value <= 0.2f)
                    value = 0.2f;
                if (value > 2)
                    value = 2;
                checkWidthRatio = value;
            }
        }

        /// <summary>
        /// Gets or sets a value determines whether to hide the checkbox if it is in the unchecked state.
        /// </summary>
        [DefaultValue(false)]
        [Category("Behavior")]
        public bool HideIfUnchecked
        {
            get { return hideIfUnchecked; }
            set { hideIfUnchecked = value; }
        }

        /// <summary>
        /// Gets or sets editable for pdf export
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool Editable
        {
            get { return editable; }
            set { editable = value; }
        }
        #endregion

        #region Private Methods
        private bool ShouldSerializeCheckColor()
        {
            return CheckColor != Color.Black;
        }

        private void DrawCheck(FRPaintEventArgs e)
        {
            RectangleF drawRect = new RectangleF(AbsLeft * e.ScaleX, AbsTop * e.ScaleY,
              Width * e.ScaleX, Height * e.ScaleY);

            float ratio = Width / (Units.Millimeters * 5);
            drawRect.Inflate(-4 * ratio * e.ScaleX, -4 * ratio * e.ScaleY);
            Pen pen = e.Cache.GetPen(CheckColor, 1.6f * ratio * CheckWidthRatio * e.ScaleX, DashStyle.Solid);
            Graphics g = e.Graphics;
            SmoothingMode saveSmoothing = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            if (Checked)
            {
                switch (CheckedSymbol)
                {
                    case CheckedSymbol.Check:
                        g.DrawLines(pen, new PointF[] {
              new PointF(drawRect.Left, drawRect.Top + drawRect.Height / 10 * 5),
              new PointF(drawRect.Left + drawRect.Width / 10 * 4, drawRect.Bottom - drawRect.Height / 10),
              new PointF(drawRect.Right, drawRect.Top + drawRect.Height / 10) });
                        break;

                    case CheckedSymbol.Cross:
                        g.DrawLine(pen, drawRect.Left, drawRect.Top, drawRect.Right, drawRect.Bottom);
                        g.DrawLine(pen, drawRect.Left, drawRect.Bottom, drawRect.Right, drawRect.Top);
                        break;

                    case CheckedSymbol.Plus:
                        g.DrawLine(pen, drawRect.Left, drawRect.Top + drawRect.Height / 2, drawRect.Right, drawRect.Top + drawRect.Height / 2);
                        g.DrawLine(pen, drawRect.Left + drawRect.Width / 2, drawRect.Top, drawRect.Left + drawRect.Width / 2, drawRect.Bottom);
                        break;

                    case CheckedSymbol.Fill:
                        Brush brush = e.Cache.GetBrush(CheckColor);
                        g.FillRectangle(brush, drawRect);
                        break;
                }
            }
            else
            {
                switch (UncheckedSymbol)
                {
                    case UncheckedSymbol.Cross:
                        g.DrawLine(pen, drawRect.Left, drawRect.Top, drawRect.Right, drawRect.Bottom);
                        g.DrawLine(pen, drawRect.Left, drawRect.Bottom, drawRect.Right, drawRect.Top);
                        break;

                    case UncheckedSymbol.Minus:
                        g.DrawLine(pen, drawRect.Left, drawRect.Top + drawRect.Height / 2, drawRect.Right, drawRect.Top + drawRect.Height / 2);
                        break;

                    case UncheckedSymbol.Slash:
                        g.DrawLine(pen, drawRect.Left, drawRect.Bottom, drawRect.Right, drawRect.Top);
                        break;

                    case UncheckedSymbol.BackSlash:
                        g.DrawLine(pen, drawRect.Left, drawRect.Top, drawRect.Right, drawRect.Bottom);
                        break;
                }
            }

            g.SmoothingMode = saveSmoothing;
        }
        #endregion

        #region Public Methods
        /// <inheritdoc/>
        public override void Assign(Base source)
        {
            base.Assign(source);

            CheckBoxObject src = source as CheckBoxObject;
            Checked = src.Checked;
            CheckedSymbol = src.CheckedSymbol;
            UncheckedSymbol = src.UncheckedSymbol;
            CheckColor = src.CheckColor;
            DataColumn = src.DataColumn;
            Expression = src.Expression;
            CheckWidthRatio = src.CheckWidthRatio;
            HideIfUnchecked = src.HideIfUnchecked;
            Editable = src.Editable;
        }

        /// <inheritdoc/>
        public override void Draw(FRPaintEventArgs e)
        {
            base.Draw(e);
            DrawCheck(e);
            DrawMarkers(e);
            Border.Draw(e, new RectangleF(AbsLeft, AbsTop, Width, Height));
        }

        /// <inheritdoc/>
        public override void Serialize(FRWriter writer)
        {
            CheckBoxObject c = writer.DiffObject as CheckBoxObject;
            base.Serialize(writer);

            if (Checked != c.Checked)
                writer.WriteBool("Checked", Checked);
            if (CheckedSymbol != c.CheckedSymbol)
                writer.WriteValue("CheckedSymbol", CheckedSymbol);
            if (UncheckedSymbol != c.UncheckedSymbol)
                writer.WriteValue("UncheckedSymbol", UncheckedSymbol);
            if (CheckColor != c.CheckColor)
                writer.WriteValue("CheckColor", CheckColor);
            if (DataColumn != c.DataColumn)
                writer.WriteStr("DataColumn", DataColumn);
            if (Expression != c.Expression)
                writer.WriteStr("Expression", Expression);
            if (CheckWidthRatio != c.CheckWidthRatio)
                writer.WriteFloat("CheckWidthRatio", CheckWidthRatio);
            if (HideIfUnchecked != c.HideIfUnchecked)
                writer.WriteBool("HideIfUnchecked", HideIfUnchecked);
            if (Editable)
                writer.WriteBool("Editable", Editable);
        }
        #endregion

        #region Report Engine
        /// <inheritdoc/>
        public override string[] GetExpressions()
        {
            List<string> expressions = new List<string>();
            expressions.AddRange(base.GetExpressions());

            if (!String.IsNullOrEmpty(DataColumn))
                expressions.Add(DataColumn);
            if (!String.IsNullOrEmpty(Expression))
                expressions.Add(Expression);
            return expressions.ToArray();
        }

        /// <inheritdoc/>
        public override void GetData()
        {
            base.GetData();
            if (!String.IsNullOrEmpty(DataColumn))
            {
                object value = Report.GetColumnValue(DataColumn);
                Variant varValue = value == null ? new Variant(0) : new Variant(value);
                Checked = varValue == true || (varValue.IsNumeric && varValue != 0);
            }
            else if (!String.IsNullOrEmpty(Expression))
            {
                object value = Report.Calc(Expression);
                Checked = value is bool && (bool)value == true;
            }
            if (!Checked && HideIfUnchecked)
                Visible = false;
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <b>CheckBoxObject</b> class with default settings. 
        /// </summary>
        public CheckBoxObject()
        {
            checkColor = Color.Black;
            dataColumn = "";
            expression = "";
            isChecked = true;
            checkedSymbol = CheckedSymbol.Check;
            uncheckedSymbol = UncheckedSymbol.None;
            checkWidthRatio = 1;
            SetFlags(Flags.HasSmartTag, true);
        }
    }
}