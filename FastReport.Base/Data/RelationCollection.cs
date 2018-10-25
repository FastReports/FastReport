using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using FastReport.Utils;

namespace FastReport.Data
{
  /// <summary>
  /// Represents the collection of <see cref="Relation"/> objects.
  /// </summary>
  public class RelationCollection : FRCollectionBase
  {
    /// <summary>
    /// Gets or sets a relation.
    /// </summary>
    /// <param name="index">The index of a relation in this collection.</param>
    /// <returns>The relation with specified index.</returns>
    public Relation this[int index]  
    {
      get { return List[index] as Relation; }
      set { List[index] = value; }
    }

    /// <summary>
    /// Finds a relation by its name.
    /// </summary>
    /// <param name="name">The name of a relation.</param>
    /// <returns>The <see cref="Relation"/> object if found; otherwise <b>null</b>.</returns>
    public Relation FindByName(string name)
    {
      foreach (Relation c in this)
      {
        if (c.Name == name)
          return c;
      }
      return null;
    }

    /// <summary>
    /// Finds a relation by its alias.
    /// </summary>
    /// <param name="alias">The alias of a relation.</param>
    /// <returns>The <see cref="Relation"/> object if found; otherwise <b>null</b>.</returns>
    public Relation FindByAlias(string alias)
    {
      foreach (Relation c in this)
      {
        if (c.Alias == alias)
          return c;
      }
      return null;
    }

    /// <summary>
    /// Finds a relation that is equal to specified one.
    /// </summary>
    /// <param name="rel">Another relation to compare with.</param>
    /// <returns>The <see cref="Relation"/> object if found; otherwise <b>null</b>.</returns>
    public Relation FindEqual(Relation rel)
    {
      foreach (Relation c in this)
      {
        if (c.Equals(rel))
          return c;
      }
      return null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RelationCollection"/> class with default settings.
    /// </summary>
    /// <param name="owner">The owner of this collection.</param>
    public RelationCollection(Base owner) : base(owner)
    {
    }
  }  
}
