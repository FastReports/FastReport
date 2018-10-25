using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using FastReport.Utils;

namespace FastReport.CrossView
{
  /// <summary>
  /// Represents a collection of CrossView data descriptors used in the <see cref="CrossViewObject"/>.
  /// </summary>
  public class CrossViewCells : CollectionBase, IFRSerializable
  {
    private string name;

    /// <summary>
    /// Gets or sets the element at the specified index.
    /// </summary>
    /// <param name="index">Index of an element.</param>
    /// <returns>The element at the specified index.</returns>
    public CrossViewCellDescriptor this[int index]
    {
      get { return List[index] as CrossViewCellDescriptor; }
      set { List[index] = value; }
    }

    internal string Name
    {
      get { return name; }
      set { name = value; }
    }

    /// <summary>
    /// Adds the specified descriptors to the end of this collection.
    /// </summary>
    /// <param name="range">Array of descriptors to add.</param>
    internal void AddRange(CrossViewCellDescriptor[] range)
    {
      foreach (CrossViewCellDescriptor s in range)
      {
        Add(s);
      }
    }

    /// <summary>
    /// Adds a descriptor to the end of this collection.
    /// </summary>
    /// <param name="value">Descriptor to add.</param>
    /// <returns>Index of the added descriptor.</returns>
    internal int Add(CrossViewCellDescriptor value)
    {
      return List.Add(value);
    }

    /// <summary>
    /// Inserts a descriptor into this collection at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index at which value should be inserted.</param>
    /// <param name="value">The descriptor to insert.</param>
    internal void Insert(int index, CrossViewCellDescriptor value)
    {
      List.Insert(index, value);
    }

    /// <summary>
    /// Removes the specified descriptor from the collection.
    /// </summary>
    /// <param name="value">Descriptor to remove.</param>
    internal void Remove(CrossViewCellDescriptor value)
    {
      int i = IndexOf(value);
      if (i != -1)
        List.RemoveAt(i);
    }

    /// <summary>
    /// Returns the zero-based index of the first occurrence of a descriptor.
    /// </summary>
    /// <param name="value">The descriptor to locate in the collection.</param>
    /// <returns>The zero-based index of the first occurrence of descriptor within 
    /// the entire collection, if found; otherwise, -1.</returns>
    internal int IndexOf(CrossViewCellDescriptor value)
    {
      return List.IndexOf(value);
    }

    /// <summary>
    /// Determines whether a descriptor is in the collection.
    /// </summary>
    /// <param name="value">The descriptor to locate in the collection.</param>
    /// <returns><b>true</b> if descriptor is found in the collection; otherwise, <b>false</b>.</returns>
    internal bool Contains(CrossViewCellDescriptor value)
    {
      return List.Contains(value);
    }

    /// <summary>
    /// Copies the elements of this collection to a new array. 
    /// </summary>
    /// <returns>An array containing copies of this collection elements. </returns>
    internal CrossViewCellDescriptor[] ToArray()
    {
      CrossViewCellDescriptor[] result = new CrossViewCellDescriptor[Count];
      for (int i = 0; i < Count; i++)
      {
        result[i] = this[i];
      }
      return result;
    }

    /// <inheritdoc/>
    public void Serialize(FRWriter writer)
    {
      writer.ItemName = Name;
      foreach (CrossViewCellDescriptor d in this)
      {
        writer.Write(d);
      }
    }

    /// <inheritdoc/>
    public void Deserialize(FRReader reader)
    {
      Clear();
      while (reader.NextItem())
      {
        CrossViewCellDescriptor d = new CrossViewCellDescriptor();
        reader.Read(d);
        Add(d);
      }
    }

    /// <summary>
    ///
    /// </summary>
    public CrossViewCells()
    {
    }
  }
}
