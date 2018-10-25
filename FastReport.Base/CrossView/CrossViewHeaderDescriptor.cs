using System;
using System.Collections.Generic;
using System.Text;
using FastReport.Table;
using FastReport.Utils;

namespace FastReport.CrossView
{
  /// <summary>
  /// The descriptor that is used to describe one element of the CrossView header.
  /// </summary>
  /// <remarks>
  /// The <see cref="CrossViewHeaderDescriptor"/> class is used to define one header element of the CrossView
  /// (either the column element or row element). 
  /// <para/>To set visual appearance of the element, use the <see cref="CrossViewDescriptor.TemplateCell"/> 
  /// property.
  /// <para/>The collection of descriptors used to represent the CrossView header is stored
  /// in the <b>CrossViewObject.Data.Columns</b> and <b>CrossViewObject.Data.Rows</b> properties.
  /// </remarks>
  public class CrossViewHeaderDescriptor : CrossViewDescriptor
  {
    #region Fields
    internal string fieldName = "";
    internal string measureName = "";
    internal bool isGrandTotal;
    internal bool isTotal;
    internal bool isMeasure;

    internal int level = 0;
    internal int cell = 0;
    internal int levelsize = 1;
    internal int cellsize = 1;
    #endregion

    #region Properties
    /// <summary>
    /// Gets a value indicating that this is the "GrandTotal" element.
    /// </summary>
    public bool IsGrandTotal
    {
      set { isGrandTotal = value; }
      get { return isGrandTotal; }
    }

    /// <summary>
    /// Gets a value indicating that this is the "Total" element.
    /// </summary>
    public bool IsTotal
    {
      set { isTotal = value; }
      get { return isTotal; }
    }

    /// <summary>
    /// Gets a value indicating that this is the "Measure" element.
    /// </summary>
    public bool IsMeasure
    {
      set { isMeasure = value; }
      get { return isMeasure; }
    }

    /// <summary>
    /// Gets the name of field in cube.
    /// </summary>
    public string FieldName
    {
      set { fieldName = value; }
      get { return fieldName; }
    }

    /// <summary>
    /// Gets the name of measure in cube.
    /// </summary>
    public string MeasureName
    {
      set { measureName = value; }
      get { return measureName; }
    }

    /// <summary>
    /// Gets the cell coordinate.
    /// </summary>
    public int Cell
    {
      set { cell = value; }
      get { return cell; }
    }

    /// <summary>
    /// Gets the size in cell coordinate.
    /// </summary>
    public int CellSize
    {
      set { cellsize = value; }
      get { return cellsize; }
    }

    /// <summary>
    /// Gets the level coordinate.
    /// </summary>
    public int Level
    {
      set { level = value; }
      get { return level; }
    }

    /// <summary>
    /// Gets the size in level coordinate.
    /// </summary>
    public int LevelSize
    {
      set { levelsize = value; }
      get { return levelsize; }
    }
    #endregion

    #region Public Methods
    /// <inheritdoc/>
    public override void Assign(CrossViewDescriptor source)
    {
      base.Assign(source);
      CrossViewHeaderDescriptor src = source as CrossViewHeaderDescriptor;
      if (src != null)
      {
        isTotal = src.isTotal;
        isGrandTotal = src.isGrandTotal;
        fieldName = src.fieldName;
        measureName = src.measureName;
        isMeasure = src.isMeasure;
        level = src.level;
        levelsize = src.levelsize;
        cell = src.cell;
        cellsize = src.cellsize;
      }
    }

    /// <inheritdoc/>
    public override void Serialize(FRWriter writer)
    {
      CrossViewHeaderDescriptor c = writer.DiffObject as CrossViewHeaderDescriptor;
      base.Serialize(writer);

      writer.ItemName = "Header";
      if (IsTotal != c.IsTotal)
        writer.WriteBool("IsTotal", IsTotal);
      if (IsGrandTotal != c.IsGrandTotal)
        writer.WriteBool("IsGrandTotal", IsGrandTotal);
      if (FieldName != c.FieldName)
        writer.WriteStr("FieldName", FieldName);
      if (MeasureName != c.MeasureName)
        writer.WriteStr("MeasureName", MeasureName);
      if (IsMeasure != c.IsMeasure)
        writer.WriteBool("IsMeasure", IsMeasure);
      if (Cell != c.Cell)
        writer.WriteInt("Cell", Cell);
      if (CellSize != c.CellSize)
        writer.WriteInt("CellSize", CellSize);
      if (Level != c.Level)
        writer.WriteInt("Level", Level);
      if (LevelSize != c.LevelSize)
        writer.WriteInt("LevelSize", LevelSize);
    }

    internal string GetName()
    {
      if (isGrandTotal)
      {
        return "GrandTotal";
      };
      if (IsMeasure)
      {
        return measureName;
      };
      if (isTotal)
      {
        return "Total of " + fieldName;
      };
      return fieldName;
    }
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="CrossViewHeaderDescriptor"/> class
    /// </summary>
    /// <param name="fieldName">The Field Name.</param>
    /// <param name="measureName">The Measure Name.</param>
    /// <param name="isTotal">Indicates the "Total" element.</param>
    /// <param name="isGrandTotal">Indicates the "GrandTotal" element.</param>
    /// <param name="isMeasure">Indicates the "Measure" element.</param>
    public CrossViewHeaderDescriptor(string fieldName, string measureName, bool isTotal, bool isGrandTotal, bool isMeasure)
    {
      this.isGrandTotal = isGrandTotal;
      this.fieldName = fieldName;
      this.measureName = measureName;
      this.isTotal = isTotal;
      this.isMeasure = isMeasure;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CrossViewHeaderDescriptor"/> class
    /// </summary>
    public CrossViewHeaderDescriptor()
        : this("", "", false, false, false)
    {
    }
  }
}
