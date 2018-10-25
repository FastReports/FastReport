using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using FastReport.Utils;

namespace FastReport
{
  /// <summary>
  /// Holds the list of objects of <see cref="PageBase"/> type.
  /// </summary>
  public class PageCollection : FRCollectionBase
  {
    /// <summary>
    /// Gets or sets the element at the specified index.
    /// </summary>
    /// <param name="index">Index of an element.</param>
    /// <returns>The element at the specified index.</returns>
    public PageBase this[int index]  
    {
      get { return List[index] as PageBase; }
      set { List[index] = value; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PageCollection"/> class with default settings.
    /// </summary>
    public PageCollection() : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PageCollection"/> class with specified owner.
    /// </summary>
    public PageCollection(Base owner) : base(owner)
    {
    }
  }
}