using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using FastReport.Utils;

namespace FastReport.Data
{
  /// <summary>
  /// Represents the collection of <see cref="Column"/> objects.
  /// </summary>
  public class ColumnCollection : FRCollectionBase
  {
    /// <summary>
    /// Gets or sets a column.
    /// </summary>
    /// <param name="index">The index of a column in this collection.</param>
    /// <returns>The column with specified index.</returns>
    public Column this[int index]
    {
      get { return List[index] as Column; }
      set { List[index] = value; }
    }

    /// <summary>
    /// Finds a column by its name.
    /// </summary>
    /// <param name="name">The name of a column.</param>
    /// <returns>The <see cref="Column"/> object if found; otherwise <b>null</b>.</returns>
    public Column FindByName(string name)
    {
      foreach (Column c in this)
      {
        if (String.Compare(c.Name, name, true) == 0)
          return c;
      }
      return null;
    }

    /// <summary>
    /// Finds a column by its alias.
    /// </summary>
    /// <param name="alias">The alias of a column.</param>
    /// <returns>The <see cref="Column"/> object if found; otherwise <b>null</b>.</returns>
    public Column FindByAlias(string alias)
    {
      foreach (Column c in this)
      {
        if (String.Compare(c.Alias, alias, true) == 0)
          return c;
      }
      return null;
    }

    /// <summary>
    /// Returns an unique column name based on given name.
    /// </summary>
    /// <param name="name">The base name.</param>
    /// <returns>The unique name.</returns>
    public string CreateUniqueName(string name)
    {
      string baseName = name;
      int i = 1;
      while (FindByName(name) != null)
      {
        name = baseName + i.ToString();
        i++;
      }
      return name;
    }

    /// <summary>
    /// Returns an unique column alias based on given alias.
    /// </summary>
    /// <param name="alias">The base alias.</param>
    /// <returns>The unique alias.</returns>
    public string CreateUniqueAlias(string alias)
    {
      string baseAlias = alias;
      int i = 1;
      while (FindByAlias(alias) != null)
      {
        alias = baseAlias + i.ToString();
        i++;
      }
      return alias;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnCollection"/> class with default settings.
    /// </summary>
    /// <param name="owner">The owner of this collection.</param>
    public ColumnCollection(Base owner) : base(owner)
    {
    }
  }
}
