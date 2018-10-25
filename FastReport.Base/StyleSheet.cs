using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using FastReport.Utils;

namespace FastReport
{
  /// <summary>
  /// Represents a collection of the <see cref="StyleCollection"/> objects.
  /// </summary>
  public class StyleSheet : CollectionBase, IFRSerializable
  {
    /// <summary>
    /// Gets or sets the element at the specified index.
    /// </summary>
    /// <param name="index">Index of an element.</param>
    /// <returns>The element at the specified index.</returns>
    public StyleCollection this[int index]
    {
      get { return List[index] as StyleCollection; }
      set { List[index] = value; }
    }

    /// <summary>
    /// Adds the specified elements to the end of this collection.
    /// </summary>
    /// <param name="range"></param>
    public void AddRange(StyleCollection[] range)
    {
      foreach (StyleCollection s in range)
      {
        Add(s);
      }
    }

    /// <summary>
    /// Adds an object to the end of this collection.
    /// </summary>
    /// <param name="value">Object to add.</param>
    /// <returns>Index of the added object.</returns>
    public int Add(StyleCollection value)
    {
      return List.Add(value);
    }

    /// <summary>
    /// Inserts an object into this collection at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index at which value should be inserted.</param>
    /// <param name="value">The object to insert.</param>
    public void Insert(int index, StyleCollection value)
    {
      List.Insert(index, value);
    }

    /// <summary>
    /// Removes the specified object from the collection.
    /// </summary>
    /// <param name="value">Object to remove.</param>
    public void Remove(StyleCollection value)
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
    public int IndexOf(StyleCollection value)
    {
      return List.IndexOf(value);
    }

    /// <summary>
    /// Returns the zero-based index of the first occurrence of a style collection with specified name.
    /// </summary>
    /// <param name="value">The style collection name to locate in the collection.</param>
    /// <returns>The zero-based index of the first occurrence of value within the entire collection, if found; 
    /// otherwise, -1.</returns>
    public int IndexOf(string value)
    {
      for (int i = 0; i < Count; i++)
      {
        StyleCollection s = this[i];
        if (String.Compare(s.Name, value, true) == 0)
          return i;
      }
      return -1;
    }

    /// <summary>
    /// Determines whether an element is in the collection.
    /// </summary>
    /// <param name="value">The object to locate in the collection.</param>
    /// <returns><b>true</b> if object is found in the collection; otherwise, <b>false</b>.</returns>
    public bool Contains(StyleCollection value)
    {
      return IndexOf(value) != -1;
    }

    /// <summary>
    /// Determines whether a style collection with specified name is in the collection.
    /// </summary>
    /// <param name="value">The style collection name to locate in the collection.</param>
    /// <returns><b>true</b> if object is found in the collection; otherwise, <b>false</b>.</returns>
    public bool Contains(string value)
    {
      return IndexOf(value) != -1;
    }

    /// <summary>
    /// Gets an array containing all collection items.
    /// </summary>
    /// <returns>An array containing all collection items.</returns>
    public object[] ToArray()
    {
      List<string> list = new List<string>();
      foreach (StyleCollection s in this)
      {
        list.Add(s.Name);
      }
      return list.ToArray();
    }

    /// <summary>
    /// Serializes the collection.
    /// </summary>
    /// <param name="writer">Writer object.</param>
    /// <remarks>
    /// This method is for internal use only.
    /// </remarks>
    public void Serialize(FRWriter writer)
    {
      writer.ItemName = "StyleSheet";
      foreach (StyleCollection s in this)
      {
        writer.Write(s);
      }
    }

    /// <summary>
    /// Deserializes the collection.
    /// </summary>
    /// <param name="reader">Reader object.</param>
    /// <remarks>
    /// This method is for internal use only.
    /// </remarks>
    public void Deserialize(FRReader reader)
    {
      while (reader.NextItem())
      {
        StyleCollection s = new StyleCollection();
        reader.Read(s);
        Add(s);
      }
    }

    /// <summary>
    /// Saves the collection to a stream.
    /// </summary>
    /// <param name="stream">Stream to save to.</param>
    public void Save(Stream stream)
    {
      using (FRWriter writer = new FRWriter())
      {
        writer.Write(this);
        writer.Save(stream);
      }
    }

    /// <summary>
    /// Saves the collection to a file with specified name.
    /// </summary>
    /// <param name="fileName">File name to save to.</param>
    public void Save(string fileName)
    {
      using (FileStream f = new FileStream(fileName, FileMode.Create))
      {
        Save(f);
      }
    }

    /// <summary>
    /// Loads the collection from a stream.
    /// </summary>
    /// <param name="stream">Stream to load from.</param>
    public void Load(Stream stream)
    {
      Clear();
      using (FRReader reader = new FRReader(null))
      {
        reader.Load(stream);
        reader.Read(this);
      }
    }

    /// <summary>
    /// Loads the collection from a file with specified name.
    /// </summary>
    /// <param name="fileName">Name of a file.</param>
    public void Load(string fileName)
    {
      using (FileStream f = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
      {
        Load(f);
      }
    }
  }
}
