using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using FastReport.Utils;

namespace FastReport.Data
{
    /// <summary>
    /// Specifies the total type.
    /// </summary>
    public enum TotalType
    {
        /// <summary>
        /// The total returns sum of values.
        /// </summary>
        Sum,

        /// <summary>
        /// The total returns minimal value.
        /// </summary>
        Min,

        /// <summary>
        /// The total returns maximal value.
        /// </summary>
        Max,

        /// <summary>
        /// The total returns average value.
        /// </summary>
        Avg,

        /// <summary>
        /// The total returns number of values.
        /// </summary>
        Count,


        /// <summary>
        /// The total returns number of distinct values.
        /// </summary>
        CountDistinct
    }

    /// <summary>
    /// Represents a total that is used to calculate aggregates such as Sum, Min, Max, Avg, Count.
    /// </summary>
    public partial class Total : Base
    {
        #region Fields
        private TotalType totalType;
        private string expression;
        private DataBand evaluator;
        private BandBase printOn;
        private string evaluateCondition;
        private bool includeInvisibleRows;
        private bool resetAfterPrint;
        private bool resetOnReprint;
        // engine
        private object value;
        private int count;
        private bool keeping;
        private Total keepTotal;
        private TotalCollection subTotals;
        private TotalCollection parentTotal;
        private const string subPrefix = "_sub";
        private Hashtable distinctValues;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the total type.
        /// </summary>
        [DefaultValue(TotalType.Sum)]
        [Category("Data")]
        public TotalType TotalType
        {
            get { return totalType; }
            set { totalType = value; }
        }

        /// <summary>
        /// Gets or sets the expression used to calculate the total.
        /// </summary>
        [Category("Data")]
        [Editor("FastReport.TypeEditors.ExpressionEditor, FastReport", typeof(UITypeEditor))]
        public string Expression
        {
            get { return expression; }
            set { expression = value; }
        }

        /// <summary>
        /// Gets or sets the evaluator databand.
        /// </summary>
        /// <remarks>
        /// The total will be calculated for each row of this band.
        /// </remarks>
        [Category("Data")]
        public DataBand Evaluator
        {
            get { return evaluator; }
            set { evaluator = value; }
        }

        /// <summary>
        /// This property is kept for compatibility only.
        /// </summary>
        [Category("Data")]
        [Browsable(false)]
        public BandBase Resetter
        {
            get { return printOn; }
            set { printOn = value; }
        }

        /// <summary>
        /// Gets or sets the band to print the total on.
        /// </summary>
        /// <remarks>
        /// The total will be resetted after the specified band has been printed.
        /// </remarks>
        [Category("Data")]
        public BandBase PrintOn
        {
            get { return printOn; }
            set { printOn = value; }
        }

        /// <summary>
        /// Gets or sets a value that determines whether the total should be resetted after print.
        /// </summary>
        [DefaultValue(true)]
        [Category("Behavior")]
        public bool ResetAfterPrint
        {
            get { return resetAfterPrint; }
            set { resetAfterPrint = value; }
        }

        /// <summary>
        /// Gets or sets a value that determines whether the total should be resetted if printed 
        /// on repeated band (i.e. band with "RepeatOnEveryPage" flag).
        /// </summary>
        [DefaultValue(true)]
        [Category("Behavior")]
        public bool ResetOnReprint
        {
            get { return resetOnReprint; }
            set { resetOnReprint = value; }
        }

        /// <summary>
        /// Gets or sets the condition which tells the total to evaluate.
        /// </summary>
        [Category("Data")]
        [Editor("FastReport.TypeEditors.ExpressionEditor, FastReport", typeof(UITypeEditor))]
        public string EvaluateCondition
        {
            get { return evaluateCondition; }
            set { evaluateCondition = value; }
        }

        /// <summary>
        /// Gets or sets a value that determines if invisible rows of the <b>Evaluator</b> should
        /// be included into the total's value.
        /// </summary>
        [DefaultValue(false)]
        [Category("Behavior")]
        public bool IncludeInvisibleRows
        {
            get { return includeInvisibleRows; }
            set { includeInvisibleRows = value; }
        }

        /// <summary>
        /// This property is not relevant to this class.
        /// </summary>
        [Browsable(false)]
        public new Restrictions Restrictions
        {
            get { return base.Restrictions; }
            set { base.Restrictions = value; }
        }

        /// <summary>
        /// Gets the value of total.
        /// </summary>
        [Browsable(false)]
        public object Value
        {
            get { return GetValue(); }
        }

        private bool IsPageFooter
        {
            get
            {
                return PrintOn is PageFooterBand || PrintOn is ColumnFooterBand ||
                  ((PrintOn is HeaderFooterBandBase) && (PrintOn as HeaderFooterBandBase).RepeatOnEveryPage);
            }
        }

        private bool IsInsideHierarchy
        {
            get
            {
                return Report.Engine.HierarchyLevel > 1 &&
                  !Name.StartsWith(subPrefix) &&
                  PrintOn != null && PrintOn.ParentDataBand != null && PrintOn.ParentDataBand.IsHierarchical;
            }
        }
        #endregion

        #region Private Methods
        private object GetValue()
        {
            if (IsInsideHierarchy)
            {
                Total subTotal = FindSubTotal(subPrefix + Report.Engine.HierarchyLevel.ToString());
                return subTotal.Value;
            }

            if (TotalType == TotalType.Avg)
            {
                if (value == null || count == 0)
                    return null;
                return new Variant(value) / count;
            }
            else if (TotalType == TotalType.Count)
                return count;
            else if (TotalType == TotalType.CountDistinct)
                return distinctValues.Keys.Count;
            return value;
        }

        private void AddValue(object value)
        {
            if (value == null || value is DBNull)
                return;

            if (TotalType == TotalType.CountDistinct)
            {
                distinctValues[value] = 1;
                return;
            }

            if (this.value == null)
                this.value = value;
            else
            {
                switch (TotalType)
                {
                    case TotalType.Sum:
                    case TotalType.Avg:
                        this.value = (new Variant(this.value) + new Variant(value)).Value;
                        break;

                    case TotalType.Min:
                        IComparable val1 = this.value as IComparable;
                        IComparable val2 = value as IComparable;
                        if (val1 != null && val2 != null && val1.CompareTo(val2) > 0)
                            this.value = value;
                        break;

                    case TotalType.Max:
                        val1 = this.value as IComparable;
                        val2 = value as IComparable;
                        if (val1 != null && val2 != null && val1.CompareTo(val2) < 0)
                            this.value = value;
                        break;
                }
            }
        }

        private Total FindSubTotal(string name)
        {
            Total result = subTotals.FindByName(name);
            if (result == null)
            {
                result = this.Clone();
                result.Name = name;
                subTotals.Add(result);
            }

            return result;
        }
        #endregion

        #region Public Methods
        /// <inheritdoc/>
        public override void Assign(Base source)
        {
            BaseAssign(source);
        }

        /// <inheritdoc/>
        public override void Serialize(FRWriter writer)
        {
            Total c = writer.DiffObject as Total;
            base.Serialize(writer);

            if (TotalType != c.TotalType)
                writer.WriteValue("TotalType", TotalType);
            if (Expression != c.Expression)
                writer.WriteStr("Expression", Expression);
            if (Evaluator != c.Evaluator)
                writer.WriteRef("Evaluator", Evaluator);
            if (PrintOn != c.PrintOn)
                writer.WriteRef("PrintOn", PrintOn);
            if (ResetAfterPrint != c.ResetAfterPrint)
                writer.WriteBool("ResetAfterPrint", ResetAfterPrint);
            if (ResetOnReprint != c.ResetOnReprint)
                writer.WriteBool("ResetOnReprint", ResetOnReprint);
            if (EvaluateCondition != c.EvaluateCondition)
                writer.WriteStr("EvaluateCondition", EvaluateCondition);
            if (IncludeInvisibleRows != c.IncludeInvisibleRows)
                writer.WriteBool("IncludeInvisibleRows", IncludeInvisibleRows);
        }

        internal Total Clone()
        {
            Total total = new Total();
            total.SetReport(Report);
            total.TotalType = TotalType;
            total.Expression = Expression;
            total.Evaluator = Evaluator;
            total.PrintOn = PrintOn;
            total.ResetAfterPrint = ResetAfterPrint;
            total.ResetOnReprint = ResetOnReprint;
            total.EvaluateCondition = EvaluateCondition;
            total.IncludeInvisibleRows = IncludeInvisibleRows;
            return total;
        }
        #endregion

        #region Report Engine
        /// <inheritdoc/>
        public override string[] GetExpressions()
        {
            List<string> expressions = new List<string>();
            if (!String.IsNullOrEmpty(Expression))
                expressions.Add(Expression);
            if (!String.IsNullOrEmpty(EvaluateCondition))
                expressions.Add(EvaluateCondition);
            return expressions.ToArray();
        }

        /// <inheritdoc/>
        public override void Clear()
        {
            base.Clear();
            value = null;
            count = 0;
            distinctValues.Clear();
        }

        internal void AddValue()
        {
            if (IsInsideHierarchy)
            {
                Total subTotal = FindSubTotal(subPrefix + Report.Engine.HierarchyLevel.ToString());
                subTotal.AddValue();
                return;
            }

            if (!Evaluator.Visible && !IncludeInvisibleRows)
                return;
            if (!String.IsNullOrEmpty(EvaluateCondition) && (bool)Report.Calc(EvaluateCondition) == false)
                return;
            if (TotalType != TotalType.Count && String.IsNullOrEmpty(Expression))
                return;

            if (keeping)
            {
                keepTotal.AddValue();
                return;
            }

            // if Total refers to another total
            Total total = IsRefersToTotal();
            if (total != null)
            {
                if (!total.parentTotal.Contains(this))
                {
                    total.parentTotal.Add(this);
                }
                return;
            }

            object value = TotalType == TotalType.Count ? null : Report.Calc(Expression);
            AddValue(value);
            if (TotalType != TotalType.Avg || (value != null && !(value is DBNull)))
                count++;
        }

        internal void ResetValue()
        {
            if (IsInsideHierarchy)
            {
                Total subTotal = FindSubTotal(subPrefix + Report.Engine.HierarchyLevel.ToString());
                subTotal.ResetValue();
                return;
            }

            Clear();
        }

        internal void ExecuteTotal(object val)
        {
            foreach (Total total in parentTotal)
                total.Execute(val);
        }

        private void Execute(object val)
        {
            AddValue(val);
            if (value != null && !(value is DBNull))
                count++;
        }

        private Total IsRefersToTotal()
        {
            string expr = Expression;
            if (expr.StartsWith("[") && expr.EndsWith("]"))
                expr = expr.Substring(1, expr.Length - 2);

            return Report.Dictionary.Totals.FindByName(expr);
        }

        internal void StartKeep()
        {
            if (!IsPageFooter || keeping)
                return;
            keeping = true;

            keepTotal = Clone();
        }

        internal void EndKeep()
        {
            if (!IsPageFooter || !keeping)
                return;
            keeping = false;

            if (TotalType == TotalType.CountDistinct)
            {
                foreach (object key in keepTotal.distinctValues)
                {
                    distinctValues[key] = 1;
                }
            }
            else
            {
                AddValue(keepTotal.value);
                count += keepTotal.count;
            }
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Total"/> class with default settings.
        /// </summary>
        public Total()
        {
            expression = "";
            evaluateCondition = "";
            resetAfterPrint = true;
            subTotals = new TotalCollection(null);
            parentTotal = new TotalCollection(null);
            distinctValues = new Hashtable();
            SetFlags(Flags.CanCopy, false);
        }
    }
}
