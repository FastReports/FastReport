using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using FastReport.Utils;

namespace FastReport.Data
{
  /// <summary>
  /// Represents the collection of <see cref="DataConnectionBase"/> objects.
  /// </summary>
  public class ConnectionCollection : FRCollectionBase
  {
    /// <summary>
    /// Gets or sets a data connection.
    /// </summary>
    /// <param name="index">The index of a data connection in this collection.</param>
    /// <returns>The data connection with specified index.</returns>
    public DataConnectionBase this[int index]
    {
      get { return List[index] as DataConnectionBase; }
      set { List[index] = value; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionCollection"/> class with default settings.
    /// </summary>
    /// <param name="owner">The owner of this collection.</param>
    public ConnectionCollection(Base owner) : base(owner)
    {
    }
  }
}
