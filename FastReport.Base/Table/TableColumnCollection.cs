using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using FastReport.Utils;

namespace FastReport.Table
{
  /// <summary>
  /// Represents a collection of <see cref="TableColumn"/> objects.
  /// </summary>
  public class TableColumnCollection : FRCollectionBase
  {
    /// <summary>
    /// Gets a column with specified index.
    /// </summary>
    /// <param name="index">Index of a column.</param>
    /// <returns>The column with specified index.</returns>
    public TableColumn this[int index]
    {
      get 
      { 
        TableColumn column = List[index] as TableColumn;
        column.SetIndex(index);
        return column;
      }
    }

    /// <inheritdoc/>
    protected override void OnInsert(int index, object value)
    {
      base.OnInsert(index, value);
      if (Owner != null)
        (Owner as TableBase).CorrectSpansOnColumnChange(index, 1);
    }

    /// <inheritdoc/>
    protected override void OnRemove(int index, object value)
    {
      base.OnRemove(index, value);
      if (Owner != null)
        (Owner as TableBase).CorrectSpansOnColumnChange(index, -1);
    }

    internal TableColumnCollection(Base owner) : base(owner)
    {
    }
  }
}
