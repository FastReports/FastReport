using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using FastReport.Utils;

namespace FastReport.Format
{
  /// <summary>
  /// Represents a collection of formats used by the <see cref="TextObject"/> and <see cref="RichObject"/>
  /// objects.
  /// </summary>
  public class FormatCollection : CollectionBase, IFRSerializable
  {
    /// <summary>
    /// Gets or sets the element at the specified index.
    /// </summary>
    /// <param name="index">Index of an element.</param>
    /// <returns>The element at the specified index.</returns>
    public FormatBase this[int index]
    {
      get { return List[index] as FormatBase; }
      set { List[index] = value; }
    }

    /// <summary>
    /// Adds the specified elements to the end of this collection.
    /// </summary>
    /// <param name="range">Array of elements to add.</param>
    public void AddRange(FormatBase[] range)
    {
      foreach (FormatBase t in range)
      {
        Add(t);
      }
    }

    /// <summary>
    /// Adds an object to the end of this collection.
    /// </summary>
    /// <param name="value">Object to add.</param>
    /// <returns>Index of the added object.</returns>
    public int Add(FormatBase value)
    {
      if (value == null)
        return -1;
      return List.Add(value);
    }

    /// <summary>
    /// Inserts an object into this collection at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index at which value should be inserted.</param>
    /// <param name="value">The object to insert.</param>
    public void Insert(int index, FormatBase value)
    {
      if (value != null)
        List.Insert(index, value);
    }

    /// <summary>
    /// Removes the specified object from the collection.
    /// </summary>
    /// <param name="value">Object to remove.</param>
    public void Remove(FormatBase value)
    {
      if (Contains(value))
        List.Remove(value);
    }

    /// <summary>
    /// Returns the zero-based index of the first occurrence of an object.
    /// </summary>
    /// <param name="value">The object to locate in the collection.</param>
    /// <returns>The zero-based index of the first occurrence of value within the entire collection, if found; 
    /// otherwise, -1.</returns>
    public int IndexOf(FormatBase value)
    {
      return List.IndexOf(value);
    }

    /// <summary>
    /// Determines whether an element is in the collection.
    /// </summary>
    /// <param name="value">The object to locate in the collection.</param>
    /// <returns><b>true</b> if object is found in the collection; otherwise, <b>false</b>.</returns>
    public bool Contains(FormatBase value)
    {
      return List.Contains(value);
    }

    /// <inheritdoc/>
    public void Serialize(FRWriter writer)
    {
      writer.ItemName = "Formats";
      foreach (FormatBase c in this)
      {
        writer.Write(c);
      }
    }

    /// <inheritdoc/>
    public void Deserialize(FRReader reader)
    {
      Clear();
      while (reader.NextItem())
      {
        FormatBase format = reader.Read() as FormatBase;
        Add(format);
      }
    }

    /// <summary>
    /// Copies formats from another collection.
    /// </summary>
    /// <param name="collection">Collection to copy from.</param>
    public void Assign(FormatCollection collection)
    {
      Clear();
      foreach (FormatBase format in collection)
      {
        Add(format.Clone());
      }
    }

    /// <inheritdoc/>
    public override bool Equals(object obj)
    {
      FormatCollection collection = obj as FormatCollection;
      if (collection == null || Count != collection.Count)
        return false;
      for (int i = 0; i < Count; i++)
      {
        if (!this[i].Equals(collection[i]))
          return false;
      }
      return true;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
      return base.GetHashCode();
    }
  }
}