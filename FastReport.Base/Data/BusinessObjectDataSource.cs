using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Reflection;
using System.ComponentModel;
using FastReport.Utils;

namespace FastReport.Data
{
  /// <summary>
  /// Represents a datasource based on business object of <b>IEnumerable</b> type.
  /// </summary>
  /// <remarks>
  /// Do not use this class directly. To register a business object, use the
  /// <b>Report.RegisterData</b> method.
  /// </remarks>
  public class BusinessObjectDataSource : DataSourceBase
  {
    #region Fields
    #endregion
    
    #region Properties
    /// <summary>
    /// Occurs when FastReport engine loads data source with data from a business object.
    /// </summary>
    /// <remarks>
    /// Use this event if you want to implement load-on-demand. Event handler must load the data into 
    /// your business object.
    /// </remarks>
    public event LoadBusinessObjectEventHandler LoadBusinessObject;
    #endregion
    
    #region Private Methods
    private void LoadData(IEnumerable enumerable, ArrayList rows)
    {
      if (enumerable == null)
        return;
      OnLoadBusinessObject();
      
      IEnumerator enumerator = enumerable.GetEnumerator();
      while (enumerator.MoveNext())
      {
        rows.Add(enumerator.Current);
      }
    }

    private void OnLoadBusinessObject()
    {
      if (LoadBusinessObject != null)
        LoadBusinessObject(this, new LoadBusinessObjectEventArgs(this.Value));
    }
    #endregion

    #region Protected Methods
    /// <inheritdoc/>
    protected override object GetValue(string alias)
    {
      string[] colAliases = alias.Split(new char[] { '.' });
      Column column = this;
      
      foreach (string colAlias in colAliases)
      {
        column = column.Columns.FindByAlias(colAlias);
        if (column == null)
          return null;
      }
      
      return GetValue(column);
    }


    /// <inheritdoc/>
    protected override object GetValue(Column column)
    {
      if (column == null)
        return null;

      // check if column is a list value
      if (column.PropDescriptor == null && column.PropName == "Value")
        return CurrentRow;
        
      // get nested columns in right order
      List<Column> columns = new List<Column>();
      while (column != this)
      {
        columns.Insert(0, column);
        column = column.Parent as Column;
      }

      object obj = CurrentRow;
      foreach (Column c in columns)
      {
        if (obj == null)
          return null;
        
        obj = c.PropDescriptor.GetValue(obj);
      }
      
      return obj;
    }
    #endregion
    
    #region Public Methods
    /// <inheritdoc/>
    public override void InitSchema()
    {
      // do nothing; the schema was initialized when we register a business object. 
    }

    /// <inheritdoc/>
    public override void LoadData(ArrayList rows)
    {
      rows.Clear();
      // custom load data via Load event
      OnLoad();

      DataSourceBase parent = ParentDataSource;
      bool isMasterDetail = parent != null && parent.RowCount > 0;

      if (isMasterDetail)
      {
        LoadData(this.Value as IEnumerable, rows);
      }
      else
      {
        // ensure that parent is loaded
        if (parent != null && parent.InternalRows.Count == 0)
          parent.Init();

        if (parent == null)
        {
          // this is a root business object, its Reference property contains IEnumerable.
          LoadData(Reference as IEnumerable, rows);
        }
        else
        {
          // enumerate parent rows to fill this data source completely
          parent.First();
          while (parent.HasMoreRows)
          {
            LoadData(this.Value as IEnumerable, rows);
            parent.Next();
          }
          // bug fix - two-pass report shows empty data
          parent.ClearData();
        }
      }  
    }

    /// <inheritdoc/>
    public override void Deserialize(FRReader reader)
    {
      base.Deserialize(reader);
      
      // compatibility with old reports: try to use last part of ReferenceName as a value for PropName
      if (!String.IsNullOrEmpty(ReferenceName) && ReferenceName.Contains("."))
      {
        string[] names = ReferenceName.Split(new char[] { '.' });
        PropName = names[names.Length - 1];
        ReferenceName = "";
      }

      // gather all nested datasource names (PropName properties)
      List<string> dataSourceNames = new List<string>();
      foreach (Column column in Columns)
      {
        if (column is BusinessObjectDataSource)
          dataSourceNames.Add(column.PropName);
      }

      // delete simple columns that have the same name as a datasource. In old version,
      // there was an invisible column used to support BO infrastructure
      for (int i = 0; i < Columns.Count; i++)
      {
        Column column = Columns[i];
        if (!(column is BusinessObjectDataSource) && dataSourceNames.Contains(column.PropName))
        {
          column.Dispose();
          i--;
        }
      }
    }
    #endregion
  }


  /// <summary>
  /// Represents the method that will handle the LoadBusinessObject event.
  /// </summary>
  /// <param name="sender">The source of the event.</param>
  /// <param name="e">The event data.</param>
  public delegate void LoadBusinessObjectEventHandler(object sender, LoadBusinessObjectEventArgs e);
  
  /// <summary>
  /// Provides data for <see cref="LoadBusinessObjectEventHandler"/> event.
  /// </summary>
  public class LoadBusinessObjectEventArgs
  {
    /// <summary>
    /// Parent object for this data source.
    /// </summary>
    public object parent;
    
    internal LoadBusinessObjectEventArgs(object parent)
    {
            this.parent = parent;
    }
  }
}
