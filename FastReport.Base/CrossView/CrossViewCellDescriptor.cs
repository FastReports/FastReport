using System;
using System.Collections.Generic;
using System.Text;
using FastReport.Table;
using FastReport.Utils;

namespace FastReport.CrossView
{

  /// <summary>
  /// The descriptor that is used to describe one CrossView data cell.
  /// </summary>
  /// <remarks>
  /// The <see cref="CrossViewCellDescriptor"/> class is used to define one data cell of the CrossView.
  /// To set visual appearance of the data cell, use the <see cref="CrossViewDescriptor.TemplateCell"/> 
  /// property.
  /// <para/>The collection of descriptors used to represent the CrossView data cells is stored
  /// in the <b>CrossViewObject.Data.Cells</b> property.
  /// </remarks>
  public class CrossViewCellDescriptor : CrossViewDescriptor
  {
    #region Fields
    internal string xFieldName;
    internal string yFieldName;
    internal string measureName;
    internal bool isXGrandTotal;
    internal bool isYGrandTotal;
    internal bool isXTotal;
    internal bool isYTotal;
#pragma warning disable FR0001 // Field names must be longer than 2 characters.
    internal int x;
    internal int y;
#pragma warning restore FR0001 // Field names must be longer than 2 characters.
    #endregion

    #region Properties
    /// <summary>
    /// Gets a value indicating that this is the "GrandTotal" element on X axis.
    /// </summary>
    public bool IsXGrandTotal
    {
      set { isXGrandTotal = value; }
      get { return isXGrandTotal; }
    }

    /// <summary>
    /// Gets a value indicating that this is the "GrandTotal" element on Y axis.
    /// </summary>
    public bool IsYGrandTotal
    {
      set { isYGrandTotal = value; }
      get { return isYGrandTotal; }
    }

    /// <summary>
    /// Gets a value indicating that this is the "Total" element on X axis.
    /// </summary>
    public bool IsXTotal
    {
      set { isXTotal = value; }
      get { return isXTotal; }
    }

    /// <summary>
    /// Gets a value indicating that this is the "Total" element on Y axis.
    /// </summary>
    public bool IsYTotal
    {
      set { isYTotal = value; }
      get { return isYTotal; }
    }

    /// <summary>
    /// Gets the name of field in X axis.
    /// </summary>
    public string XFieldName
    {
      set { xFieldName = value; }
      get { return xFieldName; }
    }

    /// <summary>
    /// Gets the name of field in Y axis.
    /// </summary>
    public string YFieldName
    {
      set { yFieldName = value; }
      get { return yFieldName; }
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
    /// Gets the x coordinate.
    /// </summary>
    public int X
    {
      set { x = value; }
      get { return x; }
    }

    /// <summary>
    /// Gets the y coordinate.
    /// </summary>
    public int Y
    {
      set { y = value; }
      get { return y; }
    }
    #endregion

    #region Public Methods
    /// <inheritdoc/>
    public override void Assign(CrossViewDescriptor source)
    {
      base.Assign(source);
      CrossViewCellDescriptor src = source as CrossViewCellDescriptor;
      if (src != null)
      {
        isXTotal = src.isXTotal;
        isYTotal = src.isYTotal;
        isXGrandTotal = src.isXGrandTotal;
        isYGrandTotal = src.isYGrandTotal;
        xFieldName = src.xFieldName;
        yFieldName = src.yFieldName;
        measureName = src.measureName;
        x = src.x;
        y = src.y;
      }
    }

    /// <inheritdoc/>
    public override void Serialize(FRWriter writer)
    {
      CrossViewCellDescriptor c = writer.DiffObject as CrossViewCellDescriptor;
      base.Serialize(writer);
      writer.ItemName = "Cell";
      if (IsXTotal != c.IsXTotal)
        writer.WriteBool("IsXTotal", IsXTotal);
      if (IsYTotal != c.IsYTotal)
        writer.WriteBool("IsYTotal", IsYTotal);
      if (IsXGrandTotal != c.IsXGrandTotal)
        writer.WriteBool("IsXGrandTotal", IsXGrandTotal);
      if (IsYGrandTotal != c.IsYGrandTotal)
        writer.WriteBool("IsYGrandTotal", IsYGrandTotal);
      if (XFieldName != c.XFieldName)
        writer.WriteStr("XFieldName", XFieldName);
      if (YFieldName != c.YFieldName)
        writer.WriteStr("YFieldName", YFieldName);
      if (MeasureName != c.MeasureName)
        writer.WriteStr("MeasureName", MeasureName);
      if (X != c.X)
        writer.WriteInt("X", X);
      if (Y != c.Y)
        writer.WriteInt("Y", Y);
    }
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="CrossViewCellDescriptor"/> class
    /// </summary>
    /// <param name="xFieldName">The Field Name in X axis.</param>
    /// <param name="yFieldName">The Field Name in Y axis.</param>
    /// <param name="measureName">The Measure Name.</param>
    /// <param name="isXTotal">Indicates the "XTotal" element.</param>
    /// <param name="isYTotal">Indicates the "YTotal" element.</param>
    /// <param name="isXGrandTotal">Indicates the "XGrandTotal" element.</param>
    /// <param name="isYGrandTotal">Indicates the "YGrandTotal" element.</param>
    public CrossViewCellDescriptor(string xFieldName, string yFieldName, string measureName, bool isXTotal, bool isYTotal, bool isXGrandTotal, bool isYGrandTotal)
    {
      this.isXGrandTotal = isXGrandTotal;
      this.isYGrandTotal = isYGrandTotal;
      this.measureName = measureName;
      if (isXGrandTotal)
      {
        this.xFieldName = "";
        this.isXTotal = false;
      }
      else
      {
        this.xFieldName = xFieldName;
        this.isXTotal = isXTotal;
      }
      if (isYGrandTotal)
      {
        this.yFieldName = "";
        this.isYTotal = false;
      }
      else
      {
        this.yFieldName = yFieldName;
        this.isYTotal = isYTotal;
      }
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="CrossViewCellDescriptor"/> class
    /// </summary>
    public CrossViewCellDescriptor()
        : this("", "", "", false, false, false, false)
    {
    }
  }
}
