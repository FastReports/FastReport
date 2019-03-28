using System;
using System.ComponentModel;
using System.Collections.Generic;
using FastReport.Utils;
using System.Drawing;
using System.Drawing.Design;

namespace FastReport.Gauge
{
    /// <summary>
    /// Represents a gauge object.
    /// </summary>
    public partial class GaugeObject : ReportComponentBase, ICloneable
    {
        #region Fields

        private double maximum;
        private double minimum;
        private double value;
        private GaugeScale scale;
        private GaugePointer pointer;
        private GaugeLabel label;
        private string expression;

        #endregion // Fields

        #region Properties

        /// <summary>
        /// Gets or sets the minimal value of gauge.
        /// </summary>
        [Category("Layout")]
        public double Minimum
        {
            get { return minimum; }
            set
            {
                if (value < maximum)
                {
                    minimum = value;
                    if (this.value < minimum)
                    {
                        this.value = minimum;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximal value of gauge.
        /// </summary>
        [Category("Layout")]
        public double Maximum
        {
            get { return maximum; }
            set
            {
                if (value > minimum)
                {
                    maximum = value;
                    if (this.value > maximum)
                    {
                        this.value = maximum;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the currenta value of gauge.
        /// </summary>
        [Category("Layout")]
        public double Value
        {
            get { return value; }
            set
            {
                if ((value >= minimum) && (value <= maximum))
                {
                    this.value = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets scale of gauge.
        /// </summary>
        [Category("Appearance")]
        [TypeConverter(typeof(FastReport.TypeConverters.FRExpandableObjectConverter))]
        [Editor("FastReport.TypeEditors.ScaleEditor, FastReport", typeof(UITypeEditor))]
        public GaugeScale Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        /// <summary>
        /// Gets or sets pointer of gauge.
        /// </summary>
        [Category("Appearance")]
        [TypeConverter(typeof(FastReport.TypeConverters.FRExpandableObjectConverter))]
        [Editor("FastReport.TypeEditors.PointerEditor, FastReport", typeof(UITypeEditor))]
        public GaugePointer Pointer
        {
            get { return pointer; }
            set { pointer = value; }
        }

        /// <summary>
        /// Gets or sets gauge label.
        /// </summary>
        [Category("Appearance")]
        [TypeConverter(typeof(FastReport.TypeConverters.FRExpandableObjectConverter))]
        [Editor("FastReport.TypeEditors.LabelEditor, FastReport", typeof(UITypeEditor))]
        public virtual GaugeLabel Label
        {
            get { return label; }
            set { label = value; }
        }

        /// <summary>
        /// Gets or sets an expression that determines the value of gauge object.
        /// </summary>
        [Category("Data")]
        [Editor("FastReport.TypeEditors.ExpressionEditor, FastReport", typeof(UITypeEditor))]
        public string Expression
        {
            get { return expression; }
            set { expression = value; }
        }

        /// <summary>
        /// Gets a value that specifies is gauge vertical or not.
        /// </summary>
        [Browsable(false)]
        public bool Vertical
        {
            get { return Width < Height; }
        }

        #endregion // Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GaugeObject"/> class.
        /// </summary>
        public GaugeObject()
        {
            minimum = 0;
            maximum = 100;
            value = 10;
            scale = new GaugeScale(this);
            pointer = new GaugePointer(this);
            label = new GaugeLabel(this);
            expression = "";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GaugeObject"/> class.
        /// </summary>
        /// <param name="minimum">Minimum value of gauge.</param>
        /// <param name="maximum">Maximum value of gauge.</param>
        /// <param name="value">Current value of gauge.</param>
        public GaugeObject(double minimum, double maximum, double value)
        {
            this.minimum = minimum;
            this.maximum = maximum;
            this.value = value;
            scale = new GaugeScale(this);
            pointer = new GaugePointer(this);
            label = new GaugeLabel(this);
            expression = "";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GaugeObject"/> class.
        /// </summary>
        /// <param name="minimum">Minimum value of gauge.</param>
        /// <param name="maximum">Maximum value of gauge.</param>
        /// <param name="value">Current value of gauge.</param>
        /// <param name="scale">Scale of gauge.</param>
        /// <param name="pointer">Pointer of gauge.</param>
        public GaugeObject(double minimum, double maximum, double value, GaugeScale scale, GaugePointer pointer)
        {
            this.minimum = minimum;
            this.maximum = maximum;
            this.value = value;
            this.scale = scale;
            this.pointer = pointer;
            label = new GaugeLabel(this);
            expression = "";
        }

        #endregion // Constructors

        #region Report Engine

        /// <inheritdoc/>
        public override string[] GetExpressions()
        {
            List<string> expressions = new List<string>();
            expressions.AddRange(base.GetExpressions());

            if (!String.IsNullOrEmpty(Expression))
            {
                expressions.Add(Expression);
            }
            return expressions.ToArray();
        }

        /// <inheritdoc/>
        public override void GetData()
        {
            base.GetData();

            if (!String.IsNullOrEmpty(Expression))
            {
                object val = Report.Calc(Expression);
                if (val != null)
                {
                    try
                    {
                        Value = Converter.StringToFloat(val.ToString());
                    }
                    catch
                    {
                        Value = 0.0;
                    }
                }
            }
        }

        #endregion // Report Engine

        #region Public Methods

        /// <inheritdoc/>
        public override void Assign(Base source)
        {
            base.Assign(source);

            GaugeObject src = source as GaugeObject;
            Maximum = src.Maximum;
            Minimum = src.Minimum;
            Value = src.Value;
            Expression = src.Expression;
            Scale.Assign(src.Scale);
            Pointer.Assign(src.Pointer);
            Label.Assign(src.Label);
        }

        /// <summary>
        /// Draws the gauge.
        /// </summary>
        /// <param name="e">Draw event arguments.</param>
        public override void Draw(FRPaintEventArgs e)
        {
            base.Draw(e);
            scale.Draw(e);
            pointer.Draw(e);
            Border.Draw(e, new RectangleF(AbsLeft, AbsTop, Width, Height));
        }

        /// <inheritdoc/>
        public override void Serialize(FRWriter writer)
        {
            GaugeObject c = writer.DiffObject as GaugeObject;
            base.Serialize(writer);

            if (Maximum != c.Maximum)
            {
                writer.WriteDouble("Maximum", Maximum);
            }
            if (Minimum != c.Minimum)
            {
                writer.WriteDouble("Minimum", Minimum);
            }
            if (Value != c.Value)
            {
                writer.WriteDouble("Value", Value);
            }
            if (Expression != c.Expression)
            {
                writer.WriteStr("Expression", Expression);
            }
            if (Scale != c.Scale)
            {
                Scale.Serialize(writer, "Scale", c.Scale);
            }
            if (Pointer != c.Pointer)
            {
                Pointer.Serialize(writer, "Pointer", c.Pointer);
            }
            if (Label != c.Label)
            {
                Label.Serialize(writer, "Label", c.Label);
            }
        }

        /// <summary>
        /// Clone Gauge Object
        /// </summary>
        /// <returns> clone of this object</returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion // Public Methods
    }
}
