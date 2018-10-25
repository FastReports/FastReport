using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using FastReport.Utils;

namespace FastReport
{
  /// <summary>
  /// Represents a collection of styles used in the <see cref="Report.Styles"/>.
  /// </summary>
  public class StyleCollection : CollectionBase, IFRSerializable
  {
    private string name;
    
    /// <summary>
    /// Gets or sets the name of the style.
    /// </summary>
    public string Name
    {
      get { return name; }
      set { name = value; }
    }

    /// <summary>
    /// Gets or sets the element at the specified index.
    /// </summary>
    /// <param name="index">Index of an element.</param>
    /// <returns>The element at the specified index.</returns>
    public Style this[int index]  
    {
      get { return List[index] as Style; }
      set { List[index] = value; }
    }

    /// <summary>
    /// Adds the specified elements to the end of this collection.
    /// </summary>
    /// <param name="range"></param>
    public void AddRange(Style[] range)
    {
      foreach (Style s in range)
      {
        Add(s);
      }
    }

    /// <summary>
    /// Adds an object to the end of this collection.
    /// </summary>
    /// <param name="value">Object to add.</param>
    /// <returns>Index of the added object.</returns>
    public int Add(Style value)
    {
      return List.Add(value);
    }

    /// <summary>
    /// Inserts an object into this collection at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index at which value should be inserted.</param>
    /// <param name="value">The object to insert.</param>
    public void Insert(int index, Style value)  
    {
      List.Insert(index, value);
    }

    /// <summary>
    /// Removes the specified object from the collection.
    /// </summary>
    /// <param name="value">Object to remove.</param>
    public void Remove(Style value)  
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
    public int IndexOf(Style value)
    {
      return List.IndexOf(value);
    }

    /// <summary>
    /// Returns the zero-based index of the first occurrence of a style with specified name.
    /// </summary>
    /// <param name="value">The name to locate in the collection.</param>
    /// <returns>The zero-based index of the first occurrence of value within the entire collection, if found; 
    /// otherwise, -1.</returns>
    public int IndexOf(string value)
    {
      for (int i = 0; i < Count; i++)
      {
        Style s = this[i];
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
    public bool Contains(Style value)  
    {
      return IndexOf(value) != -1;
    }

    /// <summary>
    /// Determines whether a style with specified name is in the collection.
    /// </summary>
    /// <param name="value">The style name to locate in the collection.</param>
    /// <returns><b>true</b> if object is found in the collection; otherwise, <b>false</b>.</returns>
    public bool Contains(string value)
    {
      return IndexOf(value) != -1;
    }

    /// <inheritdoc/>
    public void Serialize(FRWriter writer)
    {
      writer.ItemName = "Styles";
      if (!String.IsNullOrEmpty(Name))
        writer.WriteStr("Name", Name);
      foreach (Style s in this)
      {
        writer.Write(s);
      }
    }

    /// <inheritdoc/>
    public void Deserialize(FRReader reader)
    {
      Clear();
      Name = "";
      reader.ReadProperties(this);
      while (reader.NextItem())
      {
        Style s = new Style();
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
    /// Saves the collection to a file.
    /// </summary>
    /// <param name="fileName">The name of the file.</param>
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
      using (FRReader reader = new FRReader(null))
      {
        reader.Load(stream);
        reader.Read(this);
      }
    }

    /// <summary>
    /// Loads the collection from a file.
    /// </summary>
    /// <param name="fileName">The name of the file.</param>
    public void Load(string fileName)
    {
      using (FileStream f = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
      {
        Load(f);
      }
    }

    /// <summary>
    /// Creates exact copy of this collection.
    /// </summary>
    /// <returns>The copy of this collection.</returns>
    public StyleCollection Clone()
    {
      StyleCollection result = new StyleCollection();
      foreach (Style s in this)
      {
        result.Add(s.Clone());
      }
      return result;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StyleCollection"/> class with default settings.
    /// </summary>
    public StyleCollection()
    {
      name = "";
    }
  }
}
