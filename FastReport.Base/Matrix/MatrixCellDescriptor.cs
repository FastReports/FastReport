using System;
using System.Collections.Generic;
using System.Text;
using FastReport.Table;
using FastReport.Utils;

namespace FastReport.Matrix
{
  /// <summary>
  /// Specifies the aggregate function used in the <see cref="MatrixObject"/>.
  /// </summary>
  public enum MatrixAggregateFunction
  {
    /// <summary>
    /// No aggregates are used.
    /// </summary>
    None,

    /// <summary>
    /// Specifies the sum of values.
    /// </summary>
    Sum,

    /// <summary>
    /// Specifies the minimum of values.
    /// </summary>
    Min,

    /// <summary>
    /// Specifies the maximum of values.
    /// </summary>
    Max,

    /// <summary>
    /// Specifies the average of values.
    /// </summary>
    Avg,

    /// <summary>
    /// Specifies the count of values.
    /// </summary>
    Count,

        /// <summary>
        /// Specifies the count of distinct values.
        /// </summary>
        CountDistinct,

        /// <summary>
        /// Specifies the custom function.
        /// </summary>
        Custom
    }

  /// <summary>
  /// Determines how matrix percents are calculated.
  /// </summary>
  public enum MatrixPercent
  {
    /// <summary>
    /// Do not calculate percent value.
    /// </summary>
    None,

    /// <summary>
    /// Calculate percent of the column total value.
    /// </summary>
    ColumnTotal,
    
    /// <summary>
    /// Calculate percent of the row total value.
    /// </summary>
    RowTotal,
    
    /// <summary>
    /// Calculate percent of the grand total value.
    /// </summary>
    GrandTotal
  }


  /// <summary>
  /// The descriptor that is used to describe one matrix data cell.
  /// </summary>
  /// <remarks>
  /// The <see cref="MatrixCellDescriptor"/> class is used to define one data cell of the matrix.
  /// The key properties are <see cref="MatrixDescriptor.Expression"/> and <see cref="Function"/>. 
  /// To set visual appearance of the data cell, use the <see cref="MatrixDescriptor.TemplateCell"/> 
  /// property.
  /// <para/>The collection of descriptors used to represent the matrix data cells is stored
  /// in the <b>MatrixObject.Data.Cells</b> property.
  /// </remarks>
  public class MatrixCellDescriptor : MatrixDescriptor
  {
    private MatrixAggregateFunction function;
    private MatrixPercent percent;
    
    #region Properties
    /// <summary>
    /// Gets or sets an aggregate function used to calculate totals for this cell.
    /// </summary>
    public MatrixAggregateFunction Function
    {
      get { return function; }
      set { function = value; }
    }

    /// <summary>
    /// Gets or sets a value that determines how to calculate the percent value for this cell.
    /// </summary>
    public MatrixPercent Percent
    {
      get { return percent; }
      set { percent = value; }
    }
    #endregion

    #region Public Methods
    /// <inheritdoc/>
    public override void Assign(MatrixDescriptor source)
    {
      base.Assign(source);
      MatrixCellDescriptor src = source as MatrixCellDescriptor;
      if (src != null)
      {
        Function = src.Function;
        Percent = src.Percent;
      }
    }

    /// <inheritdoc/>
    public override void Serialize(FRWriter writer)
    {
      MatrixCellDescriptor c = writer.DiffObject as MatrixCellDescriptor;
      base.Serialize(writer);

      writer.ItemName = "Cell";
      if (Function != c.Function)
        writer.WriteValue("Function", Function);
      if (Percent != c.Percent)
        writer.WriteValue("Percent", Percent);
    }
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="MatrixCellDescriptor"/> class
    /// with default settings.
    /// </summary>
    public MatrixCellDescriptor() : this("", MatrixAggregateFunction.Sum)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MatrixCellDescriptor"/> class
    /// with specified expression.
    /// </summary>
    /// <param name="expression">The descriptor's expression.</param>
    public MatrixCellDescriptor(string expression) : this(expression, MatrixAggregateFunction.Sum)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MatrixCellDescriptor"/> class
    /// with specified expression and aggregate function.
    /// </summary>
    /// <param name="expression">The descriptor's expression.</param>
    /// <param name="function">The aggregate function.</param>
    public MatrixCellDescriptor(string expression, MatrixAggregateFunction function) : this(expression, function, MatrixPercent.None)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MatrixCellDescriptor"/> class
    /// with specified expression, aggregate function, and a percent.
    /// </summary>
    /// <param name="expression">The descriptor's expression.</param>
    /// <param name="function">The aggregate function.</param>
    /// <param name="percent">The percent setting.</param>
    public MatrixCellDescriptor(string expression, MatrixAggregateFunction function, MatrixPercent percent)
    {
      Expression = expression;
            this.function = function;
            this.percent = percent;
    }
  }
}
