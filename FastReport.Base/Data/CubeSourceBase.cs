using System;
using System.ComponentModel;
using FastReport.Utils;
using FastReport.CrossView;
using System.Drawing.Design;

namespace FastReport.Data
{
  /// <summary>
  /// Base class for all CubeSources such as <see cref="SliceCubeSource"/>.
  /// </summary>
  [TypeConverter(typeof(FastReport.TypeConverters.CubeSourceConverter))]
  [Editor("FastReport.TypeEditors.CubeSourceEditor, FastReport", typeof(UITypeEditor))]
  public abstract class CubeSourceBase : DataComponentBase
  {
    #region Fields
    #endregion

    #region Properties
    /// <summary>
    /// 
    /// </summary>
    public int XAxisFieldsCount { get { return CubeLink != null ? CubeLink.XAxisFieldsCount : 0; } }
    /// <summary>
    /// 
    /// </summary>
    public int YAxisFieldsCount { get { return CubeLink != null ? CubeLink.YAxisFieldsCount : 0; } }
    /// <summary>
    /// 
    /// </summary>
    public int MeasuresCount { get { return CubeLink != null ? CubeLink.MeasuresCount : 0; } }
    /// <summary>
    /// 
    /// </summary>
    public int MeasuresLevel { get { return CubeLink != null ? CubeLink.MeasuresLevel : 0; } }
    /// <summary>
    /// 
    /// </summary>
    public bool MeasuresInXAxis { get { return CubeLink != null ? CubeLink.MeasuresInXAxis : false; } }
    /// <summary>
    /// 
    /// </summary>
    public bool MeasuresInYAxis { get { return CubeLink != null ? CubeLink.MeasuresInYAxis : false; } }
    /// <summary>
    /// 
    /// </summary>
    public int DataColumnCount { get { return CubeLink != null ? CubeLink.DataColumnCount : 0; } }
    /// <summary>
    /// 
    /// </summary>
    public int DataRowCount { get { return CubeLink != null ? CubeLink.DataRowCount : 0; } }
    /// <summary>
    /// 
    /// </summary>
    public bool SourceAssigned { get { return CubeLink != null; } }
    /// <summary>
    /// 
    /// </summary>
    public event EventHandler OnChanged;
    /// <summary>
    /// 
    /// </summary>
    public IBaseCubeLink CubeLink { get { return Reference as IBaseCubeLink; } }
    #endregion

    #region Private Methods
    #endregion

    #region Protected Methods
    #endregion

    #region Public Methods
    /// <summary>
    /// 
    /// </summary>
    public CrossViewMeasureCell GetMeasureCell(int colIndex, int rowIndex)
    {
      if (CubeLink != null)
        return CubeLink.GetMeasureCell(colIndex, rowIndex);
      else
        return new CrossViewMeasureCell();
    }
    
    /// <summary>
    /// 
    /// </summary>
    public void TraverseXAxis(CrossViewAxisDrawCellHandler crossViewAxisDrawCellHandler)
    {
      if (CubeLink != null)
      {
        CubeLink.TraverseXAxis(crossViewAxisDrawCellHandler);
      }
    }


    /// <summary>
    /// 
    /// </summary>
    public void TraverseYAxis(CrossViewAxisDrawCellHandler crossViewAxisDrawCellHandler)
    {
      if (CubeLink != null)
      {
        CubeLink.TraverseYAxis(crossViewAxisDrawCellHandler);
      }
    }
    
    /// <summary>
    /// 
    /// </summary>
    public string GetXAxisFieldName(int fieldIndex)
    {
      if (CubeLink != null)
      {
        return CubeLink.GetXAxisFieldName(fieldIndex);
      }
      else
        return "";
    }
    
    /// <summary>
    /// 
    /// </summary>
    public string GetYAxisFieldName(int fieldIndex)
    {
      if (CubeLink != null)
      {
        return CubeLink.GetYAxisFieldName(fieldIndex);
      }
      else
        return "";
    }
    /// <summary>
    /// 
    /// </summary>
    public string GetMeasureName(int measureIndex)
    {
      if (CubeLink != null)
      {
        return CubeLink.GetMeasureName(measureIndex);
      }
      else
        return "";
    }

    /// <inheritdoc/>
    public override void Serialize(FRWriter writer)
    {
      base.Serialize(writer);
    }

    /// <inheritdoc/>
    public override void Deserialize(FRReader reader)
    {
      base.Deserialize(reader);
    }
    
    /// <summary>
    /// 
    /// </summary>
    public void Changed()
    {
      if (OnChanged != null)
        OnChanged(this, new EventArgs());
    }
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="CubeSourceBase"/> class with default settings.
    /// </summary>
    public CubeSourceBase()
    {
      SetFlags(Flags.HasGlobalName, true);
    }
  
  }
}
