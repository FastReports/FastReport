using System;
using System.Collections;
using System.Collections.Generic;
using FastReport.Utils;

namespace FastReport
{
  /// <summary>
  /// Holds the list of objects of <see cref="Base"/> type.
  /// </summary>
  public class ObjectCollection : FRCollectionBase
  {
    /// <summary>
    /// Gets or sets the element at the specified index.
    /// </summary>
    /// <param name="index">Index of an element.</param>
    /// <returns>The element at the specified index.</returns>
    public Base this[int index]  
    {
      get { return List[index] as Base; }
      set { List[index] = value; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectCollection"/> class with default settings.
    /// </summary>
    public ObjectCollection() : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectCollection"/> class with specified owner.
    /// </summary>
    public ObjectCollection(Base owner) : base(owner)
    {
    }
  }
}