using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using FastReport.Utils;

namespace FastReport
{
  /// <summary>
  /// Represents a collection of sort conditions used in the <see cref="DataBand.Sort"/>.
  /// </summary>
  public class SortCollection : CollectionBase, IFRSerializable
  {
    /// <summary>
    /// Gets or sets the element at the specified index.
    /// </summary>
    /// <param name="index">Index of an element.</param>
    /// <returns>The element at the specified index.</returns>
    public Sort this[int index]
    {
      get { return List[index] as Sort; }
      set { List[index] = value; }
    }

    /// <summary>
    /// Adds the specified elements to the end of this collection.
    /// </summary>
    /// <param name="range"></param>
    public void AddRange(Sort[] range)
    {
      foreach (Sort s in range)
      {
        Add(s);
      }
    }

    /// <summary>
    /// Adds an object to the end of this collection.
    /// </summary>
    /// <param name="value">Object to add.</param>
    /// <returns>Index of the added object.</returns>
    public int Add(Sort value)
    {
      return List.Add(value);
    }

    /// <summary>
    /// Inserts an object into this collection at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index at which value should be inserted.</param>
    /// <param name="value">The object to insert.</param>
    public void Insert(int index, Sort value)
    {
      List.Insert(index, value);
    }

    /// <summary>
    /// Removes the specified object from the collection.
    /// </summary>
    /// <param name="value">Object to remove.</param>
    public void Remove(Sort value)
    {
      int i = IndexOf(value);
      if (i != -1)
        List.RemoveAt(i);
    }

    /// <summary>
    /// Returns the zero-based index of the first occurrence of an object.
    /// </summary>
    /// <param name="value">The object to locate in the collection.</param>
    /// <returns>The zero-based index of the first occurrence of value within the entire collection, if found; 
    /// otherwise, -1.</returns>
    public int IndexOf(Sort value)
    {
      return List.IndexOf(value);
    }

    /// <summary>
    /// Determines whether an element is in the collection.
    /// </summary>
    /// <param name="value">The object to locate in the collection.</param>
    /// <returns><b>true</b> if object is found in the collection; otherwise, <b>false</b>.</returns>
    public bool Contains(Sort value)
    {
      return IndexOf(value) != -1;
    }

    /// <inheritdoc/>
    public void Serialize(FRWriter writer)
    {
      writer.ItemName = "Sort";
      foreach (Sort s in this)
      {
        writer.Write(s);
      }
    }

    /// <inheritdoc/>
    public void Deserialize(FRReader reader)
    {
      Clear();
      while (reader.NextItem())
      {
        Sort s = new Sort();
        reader.Read(s);
        Add(s);
      }
    }
    
    /// <summary>
    /// Assigns values from another collection.
    /// </summary>
    /// <param name="source">Collection to assign from.</param>
    public void Assign(SortCollection source)
    {
      Clear();
      
      foreach (Sort sort in source)
      {
        Add(sort);
      }
    }
  }
}
