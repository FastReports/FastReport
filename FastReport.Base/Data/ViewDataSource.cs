using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;

namespace FastReport.Data
{
  /// <summary>
  /// Represents a datasource based on <b>DataView</b> class.
  /// </summary>
  /// <remarks>
  /// This class is used to support FastReport.Net infrastructure, do not use it directly.
  /// If you want to use data from <b>DataView</b> object, call the 
  /// <see cref="FastReport.Report.RegisterData(DataView, string)"/> method of the <b>Report</b>.
  /// </remarks>
  public class ViewDataSource : DataSourceBase
  {
    #region Properties
    /// <summary>
    /// Gets the underlying <b>DataView</b> object.
    /// </summary>
    public DataView View
    {
      get { return Reference as DataView; }
    }
    #endregion

    #region Private Methods
    private Column CreateColumn(DataColumn column)
    {
      Column c = new Column();
      c.Name = column.ColumnName;
      c.Alias = column.Caption;
      c.DataType = column.DataType;
      if (c.DataType == typeof(byte[]))
        c.BindableControl = ColumnBindableControl.Picture;
      else if (c.DataType == typeof(bool))
        c.BindableControl = ColumnBindableControl.CheckBox;
      return c;
    }

    private void CreateColumns()
    {
      Columns.Clear();
      if (View != null)
      {
        foreach (DataColumn column in View.Table.Columns)
        {
          Column c = CreateColumn(column);
          Columns.Add(c);
        }
      }
    }
    #endregion

    #region Protected Methods
    /// <inheritdoc/>
    protected override object GetValue(Column column)
    {
      if (column.Tag == null)
        column.Tag = View.Table.Columns.IndexOf(column.Name);

      return CurrentRow == null ? null : ((DataRowView)CurrentRow)[(int)column.Tag];
    }
    #endregion
    
    #region Public Methods
    /// <inheritdoc/>
    public override void InitSchema()
    {
      if (Columns.Count == 0)
        CreateColumns();

      foreach (Column column in Columns)
      {
        column.Tag = null;
      }
    }

    /// <inheritdoc/>
    public override void LoadData(ArrayList rows)
    {
      // custom load data via Load event
      OnLoad();

      bool needReload = ForceLoadData || rows.Count == 0;
      if (needReload)
      {
        // fill rows
        rows.Clear();
        for (int i = 0; i < View.Count; i++)
        {
          rows.Add(View[i]);
        }
      }
    }

    internal void RefreshColumns()
    {
      if (View != null)
      {
        // add new columns
        foreach (DataColumn column in View.Table.Columns)
        {
          if (Columns.FindByName(column.ColumnName) == null)
          {
            Column c = CreateColumn(column);
            c.Enabled = true;
            Columns.Add(c);
          }
        }
        // delete obsolete columns
        int i = 0;
        while (i < Columns.Count)
        {
          Column c = Columns[i];
          if (!c.Calculated && !View.Table.Columns.Contains(c.Name))
            c.Dispose();
          else
            i++;
        }
      }
    }

    #endregion
  }
}
