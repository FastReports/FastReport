using System;
using System.Collections;
using FastReport.Data;

namespace FastReport.CrossView
{
  /// <summary>
  /// Contains a set of properties and methods to hold and manipulate the CrossView descriptors.
  /// </summary>
  /// <remarks>
  /// This class contains three collections of descriptors such as <see cref="Columns"/>,
  /// <see cref="Rows"/> and <see cref="Cells"/>. Descriptors are filled from FastCube Slice.
  /// </remarks>
  public class CrossViewData
  {
    #region Fields
    private CrossViewHeader columns;
    private CrossViewHeader rows;
    private CrossViewCells cells;
    internal int[] columnDescriptorsIndexes;
    internal int[] rowDescriptorsIndexes;
    internal int[] columnTerminalIndexes;
    internal int[] rowTerminalIndexes;
    #endregion

    #region FastCube properties (temporary)
    private CubeSourceBase cubeSource;
    /// <summary>
    /// 
    /// </summary>
    public int XAxisFieldsCount { get { return cubeSource != null ? cubeSource.XAxisFieldsCount : 0; } }
    /// <summary>
    /// 
    /// </summary>
    public int YAxisFieldsCount { get { return cubeSource != null ? cubeSource.YAxisFieldsCount : 0; } }
    /// <summary>
    /// 
    /// </summary>
    public int MeasuresCount { get { return cubeSource != null ? cubeSource.MeasuresCount : 0; } }
    /// <summary>
    /// 
    /// </summary>
    public int MeasuresLevel { get { return cubeSource != null ? cubeSource.MeasuresLevel : 0; } }
    /// <summary>
    /// 
    /// </summary>
    public bool MeasuresInXAxis { get { return cubeSource != null ? cubeSource.MeasuresInXAxis : false; } }
    /// <summary>
    /// 
    /// </summary>
    public bool MeasuresInYAxis { get { return cubeSource != null ? cubeSource.MeasuresInYAxis : false; } }
    /// <summary>
    /// 
    /// </summary>
    public int DataColumnCount { get { return cubeSource != null ? cubeSource.DataColumnCount : 0; } }
    /// <summary>
    /// 
    /// </summary>
    public int DataRowCount { get { return cubeSource != null ? cubeSource.DataRowCount : 0; } }
    /// <summary>
    /// 
    /// </summary>
    public bool SourceAssigned { get { return cubeSource != null; } }
#if !DOTNET_4
    private string intArrayToString(int[] intArray)
    {
      string res = "";
      foreach (int item in intArray)
      {
        if (res != "")
          res += ",";
        res += item.ToString();
      }
      return res;
    }
    private int[] stringToIntArray(string str)
    {
      string[] strArray = str.Split(',');
      int[] res = new int[strArray.Length];
      for (int i = 0; i < strArray.Length; i++)
      {
        res[i] = int.Parse(strArray[i]);
      }
      return res;
    }
#endif
    /// <summary>
    /// 
    /// </summary>
    public string ColumnDescriptorsIndexes
    {
#if DOTNET_4
      get { return string.Join(",", columnDescriptorsIndexes); }
      set { columnDescriptorsIndexes = Array.ConvertAll(value.Split(','), int.Parse); }
#else
      get { return intArrayToString(columnDescriptorsIndexes); }
      set { columnDescriptorsIndexes = stringToIntArray(value); }
#endif
    }
    /// <summary>
    /// 
    /// </summary>
    public string RowDescriptorsIndexes
    {
#if DOTNET_4
      get { return string.Join(",", rowDescriptorsIndexes); }
      set { rowDescriptorsIndexes = Array.ConvertAll(value.Split(','), int.Parse); }
#else
      get { return intArrayToString(rowDescriptorsIndexes); }
      set { rowDescriptorsIndexes = stringToIntArray(value); }
#endif
    }
    /// <summary>
    /// 
    /// </summary>
    public string ColumnTerminalIndexes
    {
#if DOTNET_4
      get { return string.Join(",", columnTerminalIndexes); }
      set { columnTerminalIndexes = Array.ConvertAll(value.Split(','), int.Parse); }
#else
      get { return intArrayToString(columnTerminalIndexes); }
      set { columnTerminalIndexes = stringToIntArray(value); }
#endif
    }
    /// <summary>
    /// 
    /// </summary>
    public string RowTerminalIndexes
    {
#if DOTNET_4
      get { return string.Join(",", rowTerminalIndexes); }
      set { rowTerminalIndexes = Array.ConvertAll(value.Split(','), int.Parse); }
#else
      get { return intArrayToString(rowTerminalIndexes); }
      set { rowTerminalIndexes = stringToIntArray(value); }
#endif
    }
    internal CubeSourceBase CubeSource
    {
      get { return cubeSource; }
      set
      {
        if (cubeSource != value)
        {
          cubeSource = value;
        }
      }
    }
    internal void CreateDescriptors()
    {
      columnDescriptorsIndexes = new int[0];
      rowDescriptorsIndexes = new int[0];
      columnTerminalIndexes = new int[0];
      rowTerminalIndexes = new int[0];
      CrossViewHeaderDescriptor crossViewHeaderDescriptor;
      Columns.Clear();
      Rows.Clear();
      Cells.Clear();
      
      if (!SourceAssigned)
        return;
      int cell = 0;
      for (int i = 0; i < XAxisFieldsCount; i++)
      {
        cell = 0;
        if (MeasuresInXAxis && (MeasuresLevel <= i))
        {
          if (MeasuresLevel == i)
          {
            for (int k = 0; k <= i; k++)
            {
              for (int j = 0; j < MeasuresCount; j++)
              {
                if (k == i)
                {
                  crossViewHeaderDescriptor = new CrossViewHeaderDescriptor("", CubeSource.GetMeasureName(j), false, false, true);
                  crossViewHeaderDescriptor.cellsize = 1;
                  crossViewHeaderDescriptor.levelsize = XAxisFieldsCount - i;
                }
                else
                {
                  crossViewHeaderDescriptor = new CrossViewHeaderDescriptor(CubeSource.GetXAxisFieldName(k), CubeSource.GetMeasureName(j), false, false, true);
                  crossViewHeaderDescriptor.cellsize = (XAxisFieldsCount - i);
                  crossViewHeaderDescriptor.levelsize = 1;
                }
                crossViewHeaderDescriptor.level = i;
                crossViewHeaderDescriptor.cell = cell;
                cell += crossViewHeaderDescriptor.cellsize;
                Columns.Add(crossViewHeaderDescriptor);
                if ((j == 0) && (k == 0))
                {
                  Array.Resize(ref rowDescriptorsIndexes, rowDescriptorsIndexes.Length + 1);
                  rowDescriptorsIndexes[rowDescriptorsIndexes.Length - 1] = Columns.Count - 1;
                }
                if ((k == i) || (i == (XAxisFieldsCount - 1)))
                {
                  Array.Resize(ref columnTerminalIndexes, columnTerminalIndexes.Length + 1);
                  columnTerminalIndexes[columnTerminalIndexes.Length - 1] = Columns.Count - 1;
                }
              }
            }
          }
          else
          {
            for (int j = 0; j < MeasuresCount; j++)
            {
              crossViewHeaderDescriptor = new CrossViewHeaderDescriptor(CubeSource.GetXAxisFieldName(i), "", false, false, false);
              crossViewHeaderDescriptor.level = i;
              crossViewHeaderDescriptor.levelsize = 1;
              crossViewHeaderDescriptor.cell = cell;
              crossViewHeaderDescriptor.cellsize = XAxisFieldsCount - i;
              cell += crossViewHeaderDescriptor.cellsize;
              Columns.Add(crossViewHeaderDescriptor);
              if (j == 0)
              {
                Array.Resize(ref rowDescriptorsIndexes, rowDescriptorsIndexes.Length + 1);
                rowDescriptorsIndexes[rowDescriptorsIndexes.Length - 1] = Columns.Count - 1;
              }

              if (i == 1)
              {
                crossViewHeaderDescriptor = new CrossViewHeaderDescriptor("", "", false, true, false);
              }
              else
              if (i == (MeasuresLevel + 1))
              {
                crossViewHeaderDescriptor = new CrossViewHeaderDescriptor(CubeSource.GetXAxisFieldName(i - 2), "", true, false, false);
              }
              else
              {
                crossViewHeaderDescriptor = new CrossViewHeaderDescriptor(CubeSource.GetXAxisFieldName(i - 1), "", true, false, false);
              }
              crossViewHeaderDescriptor.level = i;
              crossViewHeaderDescriptor.levelsize = XAxisFieldsCount - i;
              crossViewHeaderDescriptor.cell = cell;
              crossViewHeaderDescriptor.cellsize = 1;
              cell += crossViewHeaderDescriptor.cellsize;
              Columns.Add(crossViewHeaderDescriptor);

              if ((crossViewHeaderDescriptor.level + crossViewHeaderDescriptor.levelsize == XAxisFieldsCount))
              {
                Array.Resize(ref columnTerminalIndexes, columnTerminalIndexes.Length + 1);
                columnTerminalIndexes[columnTerminalIndexes.Length - 1] = Columns.Count - 1;
              }
            }
          }
        }
        else
        {
          crossViewHeaderDescriptor = new CrossViewHeaderDescriptor(CubeSource.GetXAxisFieldName(i), "", false, false, false);
          crossViewHeaderDescriptor.level = i;
          crossViewHeaderDescriptor.levelsize = 1;
          crossViewHeaderDescriptor.cell = cell;
          if (MeasuresInXAxis)
          {
            crossViewHeaderDescriptor.cellsize = (XAxisFieldsCount - i - 1) * MeasuresCount;
            if (crossViewHeaderDescriptor.cellsize == 0)
              crossViewHeaderDescriptor.cellsize = MeasuresCount;
          }
          else
          {
            crossViewHeaderDescriptor.cellsize = XAxisFieldsCount - i;
          }
          cell += crossViewHeaderDescriptor.cellsize;
          Columns.Add(crossViewHeaderDescriptor);
          if ((crossViewHeaderDescriptor.level + crossViewHeaderDescriptor.levelsize == XAxisFieldsCount))
          {
            Array.Resize(ref columnTerminalIndexes, columnTerminalIndexes.Length + 1);
            columnTerminalIndexes[columnTerminalIndexes.Length - 1] = Columns.Count - 1;
          }

          Array.Resize(ref rowDescriptorsIndexes, rowDescriptorsIndexes.Length + 1);
          rowDescriptorsIndexes[rowDescriptorsIndexes.Length - 1] = Columns.Count - 1;

          if (i == 0)
          {
            crossViewHeaderDescriptor = new CrossViewHeaderDescriptor("", "", false, true, false);
          }
          else
          {
            crossViewHeaderDescriptor = new CrossViewHeaderDescriptor(CubeSource.GetXAxisFieldName(i - 1), "", true, false, false);
          }
          crossViewHeaderDescriptor.level = i;
          crossViewHeaderDescriptor.cell = cell;
          if (MeasuresInXAxis)
          {
            crossViewHeaderDescriptor.levelsize = MeasuresLevel - i;
            crossViewHeaderDescriptor.cellsize = MeasuresCount;
          }
          else
          {
            crossViewHeaderDescriptor.levelsize = XAxisFieldsCount - i;
            crossViewHeaderDescriptor.cellsize = 1;
          }
          cell += crossViewHeaderDescriptor.cellsize;
          Columns.Add(crossViewHeaderDescriptor);

          if ((crossViewHeaderDescriptor.level + crossViewHeaderDescriptor.levelsize == XAxisFieldsCount))
          {
            Array.Resize(ref columnTerminalIndexes, columnTerminalIndexes.Length + 1);
            columnTerminalIndexes[columnTerminalIndexes.Length - 1] = Columns.Count - 1;
          }
        }
      }
      if (Columns.Count == 0)
      {
        crossViewHeaderDescriptor = new CrossViewHeaderDescriptor("", "", false, true, false);
        crossViewHeaderDescriptor.level = 0;
        crossViewHeaderDescriptor.levelsize = 1;
        crossViewHeaderDescriptor.cell = 0;
        crossViewHeaderDescriptor.cellsize = 1;
        Columns.Add(crossViewHeaderDescriptor);
        Array.Resize(ref rowDescriptorsIndexes, rowDescriptorsIndexes.Length + 1);
        rowDescriptorsIndexes[rowDescriptorsIndexes.Length - 1] = Columns.Count - 1;
        Array.Resize(ref columnTerminalIndexes, columnTerminalIndexes.Length + 1);
        columnTerminalIndexes[columnTerminalIndexes.Length - 1] = Columns.Count - 1;
      }

      for (int i = 0; i < YAxisFieldsCount; i++)
      {
        cell = 0;
        if (MeasuresInYAxis && (MeasuresLevel <= i))
        {
          if (MeasuresLevel == i)
          {
            for (int k = 0; k <= i; k++)
            {
              for (int j = 0; j < MeasuresCount; j++)
              {
                if (k == i)
                {
                  crossViewHeaderDescriptor = new CrossViewHeaderDescriptor("", CubeSource.GetMeasureName(j), false, false, true);
                  crossViewHeaderDescriptor.cellsize = 1;
                  crossViewHeaderDescriptor.levelsize = YAxisFieldsCount - i;
                }
                else
                {
                  crossViewHeaderDescriptor = new CrossViewHeaderDescriptor(CubeSource.GetYAxisFieldName(k), CubeSource.GetMeasureName(j), false, false, true);
                  crossViewHeaderDescriptor.cellsize = (YAxisFieldsCount - i);
                  crossViewHeaderDescriptor.levelsize = 1;
                }
                crossViewHeaderDescriptor.level = i;
                crossViewHeaderDescriptor.cell = cell;
                cell += crossViewHeaderDescriptor.cellsize;
                Rows.Add(crossViewHeaderDescriptor);
                if ((j == 0) && (k == 0))
                {
                  Array.Resize(ref columnDescriptorsIndexes, columnDescriptorsIndexes.Length + 1);
                  columnDescriptorsIndexes[columnDescriptorsIndexes.Length - 1] = Rows.Count - 1;
                }
                if ((k == i) || (i == (YAxisFieldsCount - 1)))
                {
                  Array.Resize(ref rowTerminalIndexes, rowTerminalIndexes.Length + 1);
                  rowTerminalIndexes[rowTerminalIndexes.Length - 1] = Rows.Count - 1;
                }
              }
            }
          }
          else
          {
            for (int j = 0; j < MeasuresCount; j++)
            {
              crossViewHeaderDescriptor = new CrossViewHeaderDescriptor(CubeSource.GetYAxisFieldName(i), "", false, false, false);
              crossViewHeaderDescriptor.level = i;
              crossViewHeaderDescriptor.levelsize = 1;
              crossViewHeaderDescriptor.cell = cell;
              crossViewHeaderDescriptor.cellsize = YAxisFieldsCount - i;
              cell += crossViewHeaderDescriptor.cellsize;
              Rows.Add(crossViewHeaderDescriptor);
              if (j == 0)
              {
                Array.Resize(ref columnDescriptorsIndexes, columnDescriptorsIndexes.Length + 1);
                columnDescriptorsIndexes[columnDescriptorsIndexes.Length - 1] = Rows.Count - 1;
              }

              if (i == 1)
              {
                crossViewHeaderDescriptor = new CrossViewHeaderDescriptor("", "", false, true, false);
              }
              else
              if (i == (MeasuresLevel + 1))
              {
                crossViewHeaderDescriptor = new CrossViewHeaderDescriptor(CubeSource.GetYAxisFieldName(i - 2), "", true, false, false);
              }
              else
              {
                crossViewHeaderDescriptor = new CrossViewHeaderDescriptor(CubeSource.GetYAxisFieldName(i - 1), "", true, false, false);
              }
              crossViewHeaderDescriptor.level = i;
              crossViewHeaderDescriptor.levelsize = YAxisFieldsCount - i;
              crossViewHeaderDescriptor.cell = cell;
              crossViewHeaderDescriptor.cellsize = 1;
              cell += crossViewHeaderDescriptor.cellsize;
              Rows.Add(crossViewHeaderDescriptor);

              if ((crossViewHeaderDescriptor.level + crossViewHeaderDescriptor.levelsize == YAxisFieldsCount))
              {
                Array.Resize(ref rowTerminalIndexes, rowTerminalIndexes.Length + 1);
                rowTerminalIndexes[rowTerminalIndexes.Length - 1] = Rows.Count - 1;
              }
            }
          }
        }
        else
        {
          crossViewHeaderDescriptor = new CrossViewHeaderDescriptor(CubeSource.GetYAxisFieldName(i), "", false, false, false);
          crossViewHeaderDescriptor.level = i;
          crossViewHeaderDescriptor.levelsize = 1;
          crossViewHeaderDescriptor.cell = cell;
          if (MeasuresInYAxis)
          {
            crossViewHeaderDescriptor.cellsize = (YAxisFieldsCount - i - 1) * MeasuresCount;
            if (crossViewHeaderDescriptor.cellsize == 0)
              crossViewHeaderDescriptor.cellsize = MeasuresCount;
          }
          else
          {
            crossViewHeaderDescriptor.cellsize = YAxisFieldsCount - i;
          }
          cell += crossViewHeaderDescriptor.cellsize;
          Rows.Add(crossViewHeaderDescriptor);
          if ((crossViewHeaderDescriptor.level + crossViewHeaderDescriptor.levelsize == YAxisFieldsCount))
          {
            Array.Resize(ref rowTerminalIndexes, rowTerminalIndexes.Length + 1);
            rowTerminalIndexes[rowTerminalIndexes.Length - 1] = Rows.Count - 1;
          }

          Array.Resize(ref columnDescriptorsIndexes, columnDescriptorsIndexes.Length + 1);
          columnDescriptorsIndexes[columnDescriptorsIndexes.Length - 1] = Rows.Count - 1;

          if (i == 0)
          {
            crossViewHeaderDescriptor = new CrossViewHeaderDescriptor("", "", false, true, false);
          }
          else
          {
            crossViewHeaderDescriptor = new CrossViewHeaderDescriptor(CubeSource.GetYAxisFieldName(i - 1), "", true, false, false);
          }
          crossViewHeaderDescriptor.level = i;
          crossViewHeaderDescriptor.cell = cell;
          if (MeasuresInYAxis)
          {
            crossViewHeaderDescriptor.levelsize = MeasuresLevel - i;
            crossViewHeaderDescriptor.cellsize = MeasuresCount;
          }
          else
          {
            crossViewHeaderDescriptor.levelsize = YAxisFieldsCount - i;
            crossViewHeaderDescriptor.cellsize = 1;
          }
          cell += crossViewHeaderDescriptor.cellsize;
          Rows.Add(crossViewHeaderDescriptor);

          if ((crossViewHeaderDescriptor.level + crossViewHeaderDescriptor.levelsize == YAxisFieldsCount))
          {
            Array.Resize(ref rowTerminalIndexes, rowTerminalIndexes.Length + 1);
            rowTerminalIndexes[rowTerminalIndexes.Length - 1] = Rows.Count - 1;
          }
        }
      }
      if (Rows.Count == 0)
      {
        crossViewHeaderDescriptor = new CrossViewHeaderDescriptor("", "", false, true, false);
        crossViewHeaderDescriptor.level = 0;
        crossViewHeaderDescriptor.levelsize = 1;
        crossViewHeaderDescriptor.cell = 0;
        crossViewHeaderDescriptor.cellsize = 1;
        Rows.Add(crossViewHeaderDescriptor);
        Array.Resize(ref columnDescriptorsIndexes, columnDescriptorsIndexes.Length + 1);
        columnDescriptorsIndexes[columnDescriptorsIndexes.Length - 1] = Rows.Count - 1;
        Array.Resize(ref rowTerminalIndexes, rowTerminalIndexes.Length + 1);
        rowTerminalIndexes[rowTerminalIndexes.Length - 1] = Rows.Count - 1;
      }
      CrossViewCellDescriptor crossViewCellDescriptor;
      for (int i = 0; i < columnTerminalIndexes.Length; i++)
      {
        for (int j = 0; j < rowTerminalIndexes.Length; j++)
        {
          crossViewCellDescriptor = new CrossViewCellDescriptor(Columns[columnTerminalIndexes[i]].fieldName, Rows[rowTerminalIndexes[j]].fieldName, Columns[columnTerminalIndexes[i]].measureName + Rows[rowTerminalIndexes[j]].measureName, Columns[columnTerminalIndexes[i]].isTotal, Rows[rowTerminalIndexes[j]].isTotal, Columns[columnTerminalIndexes[i]].isGrandTotal, Rows[rowTerminalIndexes[j]].isGrandTotal);
          crossViewCellDescriptor.x = i;
          crossViewCellDescriptor.y = j;
          Cells.Add(crossViewCellDescriptor);
        }
      }
    }

    internal CrossViewHeaderDescriptor GetRowDescriptor(int index)
    {
      int tempXAxisFieldsCount = (!SourceAssigned) ? 1 : XAxisFieldsCount;
      if (index < tempXAxisFieldsCount)
      {
        return Columns[rowDescriptorsIndexes[index]];
      }
      else
      {
        return Rows[rowTerminalIndexes[index - tempXAxisFieldsCount]];
      }
    }

    internal CrossViewHeaderDescriptor GetColumnDescriptor(int index)
    {
      int tempYAxisFieldsCount = (!SourceAssigned) ? 1 : YAxisFieldsCount;
      if (index < tempYAxisFieldsCount)
      {
        return Rows[columnDescriptorsIndexes[index]];
      }
      else
      {
        return Columns[columnTerminalIndexes[index - tempYAxisFieldsCount]];
      }
    }
#endregion

#region Properties
    /// <summary>
    /// Gets a collection of column descriptors.
    /// </summary>
    /// <remarks>
    /// Note: after you change something in this collection, call the 
    /// <see cref="CrossViewObject.BuildTemplate"/> method to refresh the CrossView.
    /// </remarks>
    public CrossViewHeader Columns
    {
      get { return columns; }
    }

    /// <summary>
    /// Gets a collection of row descriptors.
    /// </summary>
    /// <remarks>
    /// Note: after you change something in this collection, call the 
    /// <see cref="CrossViewObject.BuildTemplate"/> method to refresh the CrossView.
    /// </remarks>
    public CrossViewHeader Rows
    {
      get { return rows; }
    }

    /// <summary>
    /// Gets a collection of data cell descriptors.
    /// </summary>
    /// <remarks>
    /// Note: after you change something in this collection, call the 
    /// <see cref="CrossViewObject.BuildTemplate"/> method to refresh the CrossView.
    /// </remarks>
    public CrossViewCells Cells
    {
      get { return cells; }
    }

#endregion

#region Public Methods
#endregion

    internal CrossViewData()
    {
      columns = new CrossViewHeader();
      columns.Name = "CrossViewColumns";
      rows = new CrossViewHeader();
      rows.Name = "CrossViewRows";
      cells = new CrossViewCells();
      cells.Name = "CrossViewCells";
      CreateDescriptors();
    }
  }
}