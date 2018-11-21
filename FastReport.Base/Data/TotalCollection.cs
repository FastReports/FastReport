using System;
using System.Collections.Generic;
using System.Text;
using FastReport.Utils;

namespace FastReport.Data
{
  /// <summary>
  /// Represents the collection of <see cref="Total"/> objects.
  /// </summary>
  public class TotalCollection : FRCollectionBase
  {
    /// <summary>
    /// Gets or sets a total.
    /// </summary>
    /// <param name="index">The index of a total in this collection.</param>
    /// <returns>The total with specified index.</returns>
    public Total this[int index]
    {
      get { return List[index] as Total; }
      set { List[index] = value; }
    }

    /// <summary>
    /// Finds a total by its name.
    /// </summary>
    /// <param name="name">The name of a total.</param>
    /// <returns>The <see cref="Total"/> object if found; otherwise <b>null</b>.</returns>
    public Total FindByName(string name)
    {
      foreach (Total c in this)
      {
        // check complete match or match without case sensitivity
        if (c.Name == name || c.Name.ToLower() == name.ToLower())
          return c;
      }
      return null;
    }

    /// <summary>
    /// Returns an unique total name based on given name.
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

    internal object GetValue(string name)
    {
        Total t = FindByName(name);
        if (t == null)
            throw new UnknownNameException(name);
        return t.Value;
    }

    internal void ProcessBand(BandBase band)
    {
      foreach (Total total in this)
      {
        if (total.Evaluator == band)
          total.AddValue();
        else if (total.PrintOn == band && total.ResetAfterPrint)
        {
          if (!band.Repeated || total.ResetOnReprint)
            total.ResetValue();
        }
      }
    }

    internal void ClearValues()
    {
      foreach (Total total in this)
      {
        total.Clear();
      }
    }

    internal void StartKeep()
    {
      foreach (Total total in this)
      {
        total.StartKeep();
      }
    }

    internal void EndKeep()
    {
      foreach (Total total in this)
      {
        total.EndKeep();
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TotalCollection"/> class with default settings.
    /// </summary>
    /// <param name="owner">The owner of this collection.</param>
    public TotalCollection(Base owner) : base(owner)
    {
    }
  }
}
