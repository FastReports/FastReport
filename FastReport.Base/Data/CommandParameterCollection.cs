using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using FastReport.Utils;

namespace FastReport.Data
{
  /// <summary>
  /// Represents the collection of <see cref="CommandParameter"/> objects.
  /// </summary>
  /// <remarks>
  /// This class is used to store the list of parameters defined in the datasource. See the 
  /// <see cref="TableDataSource.Parameters"/> property for more details.
  /// </remarks>
  public class CommandParameterCollection : FRCollectionBase
  {
    /// <summary>
    /// Gets or sets a parameter.
    /// </summary>
    /// <param name="index">The index of a parameter in this collection.</param>
    /// <returns>The parameter with specified index.</returns>
    public CommandParameter this[int index]
    {
      get { return List[index] as CommandParameter; }
      set { List[index] = value; }
    }

    /// <summary>
    /// Finds a parameter by its name.
    /// </summary>
    /// <param name="name">The name of a parameter.</param>
    /// <returns>The <see cref="CommandParameter"/> object if found; otherwise <b>null</b>.</returns>
    public CommandParameter FindByName(string name)
    {
      foreach (CommandParameter c in this)
      {
        if (c.Name == name)
          return c;
      }
      return null;
    }

    /// <summary>
    /// Returns an unique parameter name based on given name.
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
    /// Initializes a new instance of the <see cref="CommandParameterCollection"/> class with default settings.
    /// </summary>
    /// <param name="owner">The owner of this collection.</param>
    public CommandParameterCollection(Base owner) : base(owner)
    {
    }
  }
}
