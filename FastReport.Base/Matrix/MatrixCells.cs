using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using FastReport.Utils;

namespace FastReport.Matrix
{
  /// <summary>
  /// Represents a collection of matrix data descriptors used in the <see cref="MatrixObject"/>.
  /// </summary>
  public class MatrixCells : CollectionBase, IFRSerializable
  {
    private List<ArrayList>[] rows;
    private string name;

    /// <summary>
    /// Gets or sets the element at the specified index.
    /// </summary>
    /// <param name="index">Index of an element.</param>
    /// <returns>The element at the specified index.</returns>
    public MatrixCellDescriptor this[int index]
    {
      get { return List[index] as MatrixCellDescriptor; }
      set { List[index] = value; }
    }

    internal string Name
    {
      get { return name; }
      set { name = value; }
    }

    internal bool IsEmpty
    {
        get { return rows == null; }
    }

    /// <summary>
    /// Adds the specified descriptors to the end of this collection.
    /// </summary>
    /// <param name="range">Array of descriptors to add.</param>
    public void AddRange(MatrixCellDescriptor[] range)
    {
      foreach (MatrixCellDescriptor s in range)
      {
        Add(s);
      }
    }

    /// <summary>
    /// Adds a descriptor to the end of this collection.
    /// </summary>
    /// <param name="value">Descriptor to add.</param>
    /// <returns>Index of the added descriptor.</returns>
    public int Add(MatrixCellDescriptor value)
    {
      return List.Add(value);
    }

    /// <summary>
    /// Inserts a descriptor into this collection at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index at which value should be inserted.</param>
    /// <param name="value">The descriptor to insert.</param>
    public void Insert(int index, MatrixCellDescriptor value)
    {
      List.Insert(index, value);
    }

    /// <summary>
    /// Removes the specified descriptor from the collection.
    /// </summary>
    /// <param name="value">Descriptor to remove.</param>
    public void Remove(MatrixCellDescriptor value)
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
    public int IndexOf(MatrixCellDescriptor value)
    {
      return List.IndexOf(value);
    }

    /// <summary>
    /// Determines whether a descriptor is in the collection.
    /// </summary>
    /// <param name="value">The descriptor to locate in the collection.</param>
    /// <returns><b>true</b> if descriptor is found in the collection; otherwise, <b>false</b>.</returns>
    public bool Contains(MatrixCellDescriptor value)
    {
      return List.Contains(value);
    }

    /// <summary>
    /// Copies the elements of this collection to a new array. 
    /// </summary>
    /// <returns>An array containing copies of this collection elements. </returns>
    public MatrixCellDescriptor[] ToArray()
    {
      MatrixCellDescriptor[] result = new MatrixCellDescriptor[Count];
      for (int i = 0; i < Count; i++)
      {
        result[i] = this[i];
      }
      return result;
    }

    private void CheckIndices(int columnIndex, int rowIndex, List<ArrayList> rows)
    {
      // append rows if row index is out of bounds
      if (rowIndex >= rows.Count)
      {
        int delta = rowIndex - rows.Count + 1;
        for (int i = 0; i < delta; i++)
        {
          rows.Add(new ArrayList());
        }
      }

      ArrayList row = rows[rowIndex];
      if (columnIndex >= row.Count)
      {
        int delta = columnIndex - row.Count + 1;
        for (int i = 0; i < delta; i++)
        {
          row.Add(null);
        }
      }
    }
    
    private void AddValue(int columnIndex, int rowIndex, object value, List<ArrayList> rows)
    {
      // do not put null values to the matrix!
      if (value == null || value == DBNull.Value)
        return;
      
      CheckIndices(columnIndex, rowIndex, rows);

      ArrayList row = rows[rowIndex];
      object oldValue = row[columnIndex];
      if (oldValue == null)
      {
        // initial state - the cell is empty. Put the value into the cell
        row[columnIndex] = value;
      }
      else if (oldValue is ArrayList)
      {
        // cell contains a list of values. Add a new value to the list
        (oldValue as ArrayList).Add(value);
      }
      else
      {
        // cell contains single value, we need to create a list of values
        ArrayList valuesList = new ArrayList();
        valuesList.Add(oldValue);
        valuesList.Add(value);
        row[columnIndex] = valuesList;
      }  
    }

    private void SetValue(int columnIndex, int rowIndex, object value, List<ArrayList> rows)
    {
      CheckIndices(columnIndex, rowIndex, rows);

      rows[rowIndex][columnIndex] = value;
    }

    internal void AddValue(int columnIndex, int rowIndex, object[] value)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      if (value.Length != Count)
        throw new MatrixValueException(Count);

      if (rows == null)
        rows = new List<ArrayList>[Count];
        
      for (int i = 0; i < Count; i++)
      {
        if (rows[i] == null)
          rows[i] = new List<ArrayList>();
        
        AddValue(columnIndex, rowIndex, value[i], rows[i]);
      }  
    }

    // return value(s) contained in a cell.
    // in case of empty cell, return null.
    internal object GetValue(int columnIndex, int rowIndex, int cellIndex)
    {
      if (rows == null || 
        cellIndex >= rows.Length ||
        rowIndex >= rows[cellIndex].Count ||
        columnIndex >= rows[cellIndex][rowIndex].Count)
        return null;

      // cell may contain either null, single value, or ArrayList containing several values.
      object value = rows[cellIndex][rowIndex][columnIndex];
      return value;
    }

    // return value(s) contained in a cell as ArrayList, even if there is only one value.
    // in case of empty cell, return null.
    internal ArrayList GetValues(int columnIndex, int rowIndex, int cellIndex)
    {
      object value = GetValue(columnIndex, rowIndex, cellIndex);
      if (value == null)
        return null;
      if (value is ArrayList)
        return (ArrayList)value;
      ArrayList list = new ArrayList();
      list.Add(value);
      return list;
    }

    // replaces the list of values in a cell by the single value
    internal void SetValues(int columnIndex, int rowIndex, object[] cellValues)
    {
      // do not put all nulls into the matrix
      bool allNulls = true;
      for (int i = 0; i < Count; i++)
      {
        if (cellValues[i] != null)
          allNulls = false;
      }

      if (!allNulls)
      {
        if (rows == null)
          rows = new List<ArrayList>[Count];

        for (int i = 0; i < Count; i++)
        {
          if (rows[i] == null)
            rows[i] = new List<ArrayList>();
          
          SetValue(columnIndex, rowIndex, cellValues[i], rows[i]);
        }
      }
    }

    internal void Reset()
    {
      rows = null;
    }

    /// <inheritdoc/>
    public void Serialize(FRWriter writer)
    {
      writer.ItemName = Name;
      foreach (MatrixCellDescriptor d in this)
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
        MatrixCellDescriptor d = new MatrixCellDescriptor();
        reader.Read(d);
        Add(d);
      }
    }
    
    internal MatrixCells()
    {
    }
  }
}
