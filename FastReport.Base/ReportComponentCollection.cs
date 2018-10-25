using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using FastReport.Utils;

namespace FastReport
{
  /// <summary>
  /// Holds the list of objects of <see cref="ReportComponentBase"/> type.
  /// </summary>
  public class ReportComponentCollection : FRCollectionBase
  {
    /// <summary>
    /// Gets or sets the element at the specified index.
    /// </summary>
    /// <param name="index">Index of an element.</param>
    /// <returns>The element at the specified index.</returns>
    public ReportComponentBase this[int index]  
    {
      get { return List[index] as ReportComponentBase; }
      set { List[index] = value; }
    }
    
    internal ReportComponentCollection SortByTop()
    {
      ReportComponentCollection result = new ReportComponentCollection();
      CopyTo(result);
      result.InnerList.Sort(new TopComparer());
      return result;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReportComponentCollection"/> class with default settings.
    /// </summary>
    public ReportComponentCollection() : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReportComponentCollection"/> class with specified owner.
    /// </summary>
    public ReportComponentCollection(Base owner) : base(owner)
    {
    }


    private class TopComparer : IComparer
    {
      public int Compare(object x, object y)
      {
        return (x as ReportComponentBase).Top.CompareTo((y as ReportComponentBase).Top);
      }
    }
  }
}