using System;
using System.Collections.Generic;
using System.Text;
using FastReport.Table;
using FastReport.Utils;

namespace FastReport.CrossView
{
    /// <summary>
    /// The base class for matrix element descriptors such as <see cref="CrossViewHeaderDescriptor"/> and
    /// <see cref="CrossViewCellDescriptor"/>.
    /// </summary>
    public class CrossViewDescriptor : IFRSerializable
  {
    #region Fields
    private string expression;
    private TableColumn templateColumn;
    private TableRow templateRow;
    private TableCell templateCell;
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets an expression which value will be used to fill the matrix.
    /// </summary>
    /// <remarks>
    /// <b>Expression</b> may be any valid expression. Usually it's a data column:
    /// <c>[DataSource.Column]</c>.
    /// </remarks>
    public string Expression
    {
      get { return expression; }
      set { expression = value; }
    }

    /// <summary>
    /// Gets or sets the template column bound to this descriptor.
    /// </summary>
    /// <remarks>
    /// This property is for internal use; usually you don't need to use it.
    /// </remarks>
    public TableColumn TemplateColumn
    {
      get { return templateColumn; }
      set { templateColumn = value; }
    }

    /// <summary>
    /// Gets or sets the template row bound to this descriptor.
    /// </summary>
    /// <remarks>
    /// This property is for internal use; usually you don't need to use it.
    /// </remarks>
    public TableRow TemplateRow
    {
      get { return templateRow; }
      set { templateRow = value; }
    }

    /// <summary>
    /// Gets or sets the template cell bound to this descriptor.
    /// </summary>
    /// <remarks>
    /// Using this property, you may access the matrix cell which is bound to
    /// this descriptor. It may be useful to change the cell's appearance. 
    /// <note>
    /// Before using this property, you must initialize the matrix descriptors by
    /// calling the <see cref="CrossViewObject.BuildTemplate"/> method.
    /// </note>
    /// </remarks>
    /// <example>
    /// <code>
    /// CrossViewObject crossView;
    /// // change the fill color of the first matrix cell
    /// crossView.Data.Cells[0].TemplateCell.Fill = new SolidFill(Color.Red);
    /// </code>
    /// </example>
    public TableCell TemplateCell
    {
      get { return templateCell; }
      set { templateCell = value; }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Assigns values from another descriptor.
    /// </summary>
    /// <param name="source">Descriptor to assign values from.</param>
    public virtual void Assign(CrossViewDescriptor source)
    {
      Expression = source.Expression;
      TemplateCell = source.TemplateCell;
    }

    /// <inheritdoc/>
    public virtual void Serialize(FRWriter writer)
    {
      CrossViewDescriptor c = writer.DiffObject as CrossViewDescriptor;

      if (Expression != c.Expression)
        writer.WriteStr("Expression", Expression);
    }

    /// <inheritdoc/>
    public void Deserialize(FRReader reader)
    {
      reader.ReadProperties(this);
    }
    #endregion
  }
}
