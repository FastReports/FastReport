using System;
using System.Collections.Generic;
using System.Text;
using FastReport.Table;
using FastReport.Utils;

namespace FastReport.Matrix
{
  /// <summary>
  /// The descriptor that is used to describe one element of the matrix header.
  /// </summary>
  /// <remarks>
  /// The <see cref="MatrixHeaderDescriptor"/> class is used to define one header element of the matrix
  /// (either the column element or row element). The key properties are 
  /// <see cref="MatrixDescriptor.Expression"/>, <see cref="Sort"/> and <see cref="Totals"/>. 
  /// <para/>To set visual appearance of the element, use the <see cref="MatrixDescriptor.TemplateCell"/> 
  /// property. To set visual appearance of the "total" element, use the <see cref="TemplateTotalCell"/> 
  /// property.
  /// <para/>The collection of descriptors used to represent the matrix header is stored
  /// in the <b>MatrixObject.Data.Columns</b> and <b>MatrixObject.Data.Rows</b> properties.
  /// </remarks>
  public class MatrixHeaderDescriptor : MatrixDescriptor
  {
    #region Fields
    private SortOrder sort;
    private bool totals;
    private bool totalsFirst;
    private bool pageBreak;
    private bool suppressTotals;
    private TableColumn templateTotalColumn;
    private TableRow templateTotalRow;
    private TableCell templateTotalCell;
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets the sort order of header values.
    /// </summary>
    /// <remarks>
    /// This property determines how the values displayed in this element are sorted. The default sort
    /// is ascending.
    /// </remarks>
    public SortOrder Sort
    {
      get { return sort; }
      set { sort = value; }
    }
    
    /// <summary>
    /// Gets or sets a value indicating that this element has associated "total" element.
    /// </summary>
    /// <remarks>
    /// To access the matrix cell that is bound to the "Total" element, use the
    /// <see cref="TemplateTotalCell"/> property. It may be useful to change the
    /// "Total" text by something else.
    /// </remarks>
    /// <example>This example shows how to change the "Total" text of the total element.
    /// <code>
    /// MatrixObject matrix;
    /// matrix.Data.Rows[0].TemplateTotalCell.Text = "Grand Total";
    /// </code>
    /// </example>
    public bool Totals
    {
      get { return totals; }
      set { totals = value; }
    }

    /// <summary>
    /// Gets or sets the value indicating whether the total values must be printed before the data.
    /// </summary>
    public bool TotalsFirst
    {
      get { return totalsFirst; }
      set { totalsFirst = value; }
    }
    
    /// <summary>
    /// Gets or sets a value indicating that the page break must be printed before this element.
    /// </summary>
    /// <remarks>
    /// Page break is not printed before the very first element.
    /// </remarks>
    public bool PageBreak
    {
      get { return pageBreak; }
      set { pageBreak = value; }
    }

    /// <summary>
    /// Gets or sets a value that determines whether it is necessary to suppress totals
    /// if there is only one value in a group.
    /// </summary>
    public bool SuppressTotals
    {
      get { return suppressTotals; }
      set { suppressTotals = value; }
    }

    /// <summary>
    /// Gets or sets the template column bound to the "total" element of this descriptor.
    /// </summary>
    /// <remarks>
    /// This property is for internal use; usually you don't need to use it.
    /// </remarks>
    public TableColumn TemplateTotalColumn
    {
      get { return templateTotalColumn; }
      set { templateTotalColumn = value; }
    }

    /// <summary>
    /// Gets or sets the template row bound to the "total" element of this descriptor.
    /// </summary>
    /// <remarks>
    /// This property is for internal use; usually you don't need to use it.
    /// </remarks>
    public TableRow TemplateTotalRow
    {
      get { return templateTotalRow; }
      set { templateTotalRow = value; }
    }

    /// <summary>
    /// Gets or sets the template cell bound to the "total" element of this descriptor.
    /// </summary>
    /// <remarks>
    /// This property may be useful to change the "Total" text by something else.
    /// <note>
    /// Before using this property, you must initialize the matrix descriptors by
    /// calling the <see cref="MatrixObject.BuildTemplate"/> method.
    /// </note>
    /// </remarks>
    /// <example>This example shows how to change the "Total" element.
    /// <code>
    /// MatrixObject matrix;
    /// matrix.Data.Rows[0].TemplateTotalCell.Text = "Grand Total";
    /// matrix.Data.Rows[0].TemplateTotalCell.Fill = new SolidFill(Color.Green);
    /// </code>
    /// </example>    
    public TableCell TemplateTotalCell
    {
      get { return templateTotalCell; }
      set { templateTotalCell = value; }
    }
    #endregion

    #region Public Methods
    /// <inheritdoc/>
    public override void Assign(MatrixDescriptor source)
    {
      base.Assign(source);
      MatrixHeaderDescriptor src = source as MatrixHeaderDescriptor;
      if (src != null)
      {
        Sort = src.Sort;
        Totals = src.Totals;
        TotalsFirst = src.TotalsFirst;
        PageBreak = src.PageBreak;
        SuppressTotals = src.SuppressTotals;
        TemplateTotalCell = src.TemplateTotalCell;
      }
    }

    /// <inheritdoc/>
    public override void Serialize(FRWriter writer)
    {
      MatrixHeaderDescriptor c = writer.DiffObject as MatrixHeaderDescriptor;
      base.Serialize(writer);

      writer.ItemName = "Header";
      if (Sort != c.Sort)
        writer.WriteValue("Sort", Sort);
      if (Totals != c.Totals)
        writer.WriteBool("Totals", Totals);
      if (TotalsFirst != c.TotalsFirst)
        writer.WriteBool("TotalsFirst", TotalsFirst);
      if (PageBreak != c.PageBreak)
        writer.WriteBool("PageBreak", PageBreak);
      if (SuppressTotals != c.SuppressTotals)
        writer.WriteBool("SuppressTotals", SuppressTotals);
    }
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="MatrixHeaderDescriptor"/> class with
    /// default settings.
    /// </summary>
    public MatrixHeaderDescriptor()
      : this("", SortOrder.Ascending, true)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MatrixHeaderDescriptor"/> class with
    /// specified expression.
    /// </summary>
    /// <param name="expression">The descriptor's expression.</param>
    public MatrixHeaderDescriptor(string expression)
      : this(expression, SortOrder.Ascending, true)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MatrixHeaderDescriptor"/> class with
    /// specified expression and totals.
    /// </summary>
    /// <param name="expression">The descriptor's expression.</param>
    /// <param name="totals">Indicates whether to show the "total" element.</param>
    public MatrixHeaderDescriptor(string expression, bool totals)
      : this(expression, SortOrder.Ascending, totals)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MatrixHeaderDescriptor"/> class with
    /// specified expression, sort order and totals.
    /// </summary>
    /// <param name="expression">The descriptor's expression.</param>
    /// <param name="sort">Sort order used to sort header values.</param>
    /// <param name="totals">Indicates whether to show the "total" element.</param>
    public MatrixHeaderDescriptor(string expression, SortOrder sort, bool totals)
    {
      Expression = expression;
            this.sort = sort;
            this.totals = totals;
    }
  }
}
