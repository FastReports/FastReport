using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using FastReport.Utils;

namespace FastReport.Data
{
  /// <summary>
  /// Represents the collection of <see cref="DataSourceBase"/> objects.
  /// </summary>
  public class DataSourceCollection : FRCollectionBase
  {
    /// <summary>
    /// Gets or sets a data source.
    /// </summary>
    /// <param name="index">The index of a data source in this collection.</param>
    /// <returns>The data source with specified index.</returns>
    public DataSourceBase this[int index]
    {
      get { return List[index] as DataSourceBase; }
      set { List[index] = value; }
    }

    /// <summary>
    /// Finds a datasource by its name.
    /// </summary>
    /// <param name="name">The name of a datasource.</param>
    /// <returns>The <see cref="DataSourceBase"/> object if found; otherwise <b>null</b>.</returns>
    public DataSourceBase FindByName(string name)
    {
      foreach (DataSourceBase c in this)
      {
        if (c.Name == name)
          return c;
      }
      return null;
    }

    /// <summary>
    /// Finds a datasource by its alias.
    /// </summary>
    /// <param name="alias">The alias of a datasource.</param>
    /// <returns>The <see cref="DataSourceBase"/> object if found; otherwise <b>null</b>.</returns>
    public DataSourceBase FindByAlias(string alias)
    {
      foreach (DataSourceBase c in this)
      {
        if (c.Alias == alias)
          return c;
      }
      return null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataSourceCollection"/> class with default settings.
    /// </summary>
    /// <param name="owner">The owner of this collection.</param>
    public DataSourceCollection(Base owner) : base(owner)
    {
    }
  }
}
