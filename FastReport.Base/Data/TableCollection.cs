using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using FastReport.Utils;

namespace FastReport.Data
{
  /// <summary>
  /// Represents the collection of <see cref="TableDataSource"/> objects.
  /// </summary>
  public class TableCollection : FRCollectionBase
  {
    /// <summary>
    /// Gets or sets a data table.
    /// </summary>
    /// <param name="index">The index of a data table in this collection.</param>
    /// <returns>The data table with specified index.</returns>
    public TableDataSource this[int index]
    {
      get { return List[index] as TableDataSource; }
      set { List[index] = value; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TableCollection"/> class with default settings.
    /// </summary>
    /// <param name="owner">The owner of this collection.</param>
    public TableCollection(Base owner) : base(owner)
    {
    }
  }
}
