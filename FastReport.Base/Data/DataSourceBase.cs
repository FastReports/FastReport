using System;
using System.Collections.Generic;
using System.Data;
using System.ComponentModel;
using System.Collections;
using FastReport.Utils;
using System.Drawing.Design;

namespace FastReport.Data
{
  /// <summary>
  /// Base class for all datasources such as <see cref="TableDataSource"/>.
  /// </summary>
  [TypeConverter(typeof(FastReport.TypeConverters.DataSourceConverter))]
  [Editor("FastReport.TypeEditors.DataSourceEditor, FastReport", typeof(UITypeEditor))]
  public abstract class DataSourceBase : Column
  {
    #region Fields
    private ArrayList internalRows;
    private ArrayList rows;
    private int currentRowNo;
    protected object currentRow;
    private Hashtable additionalFilter;
    private bool forceLoadData;
    private Hashtable columnIndices;
    private Hashtable rowIndices;
    private Hashtable relation_SortedChildRows;
    private static bool FShowAccessDataMessage;
    #endregion

    #region Properties
    /// <summary>
    /// Occurs when the FastReport engine loads data source with data.
    /// </summary>
    /// <remarks>
    /// Use this event if you want to implement load-on-demand. Event handler must load the data 
    /// into the data object which this datasource is bound to (for example, the
    /// <b>TableDataSource</b> uses data from the <b>DataTable</b> object bound to
    /// the <b>Table</b> property).
    /// </remarks>
    public event EventHandler Load;

    /// <summary>
    /// Gets a number of data rows in this datasource.
    /// </summary>
    /// <remarks>
    /// You should initialize the datasource by the <b>Init</b> method before using this property.
    /// </remarks>
    [Browsable(false)]
    public int RowCount
    {
      get { return rows.Count; }
    }

    /// <summary>
    /// Gets a value indicating that datasource has more rows, that is the <see cref="CurrentRowNo"/>
    /// is less than the <see cref="RowCount"/>.
    /// </summary>
    /// <remarks>
    /// <para>You should initialize the datasource by the <b>Init</b> method before using this property.</para>
    /// <para>Usually this property is used with the following code block:</para>
    /// <code>
    /// dataSource.Init();
    /// while (dataSource.HasMoreRows)
    /// {
    ///   // do something...
    ///   dataSource.Next();
    /// }
    /// </code>
    /// </remarks>
    [Browsable(false)]
    public bool HasMoreRows
    {
      get { return CurrentRowNo < RowCount; }
    }

    /// <summary>
    /// Gets the current data row.
    /// </summary>
    /// <remarks>
    /// <para>This property is updated when you call the <see cref="Next"/> method.</para>
    /// </remarks>
    [Browsable(false)]
    public object CurrentRow
    {
      get
      {
        // in case we trying to print a datasource column in report title, init the datasource
        if (InternalRows.Count == 0)
          Init();
        return currentRow;
      }
    }

    /// <summary>
    /// Gets an index of current data row.
    /// </summary>
    /// <remarks>
    /// <para>You should initialize the datasource by the <b>Init</b> method before using this property.</para>
    /// <para>This property is updated when you call the <see cref="Next"/> method.</para>
    /// </remarks>
    [Browsable(false)]
    public int CurrentRowNo
    {
      get { return currentRowNo; }
      set 
      { 
        currentRowNo = value;
        if (value >= 0 && value < rows.Count)
          currentRow = rows[value];
        else
          currentRow = null;
      }
    }

    /// <summary>
    /// Gets data stored in a specified column.
    /// </summary>
    /// <param name="columnAlias">Alias of a column.</param>
    /// <returns>The column's value.</returns>
    /// <remarks>
    /// You should initialize the datasource by the <b>Init</b> method before using this property.
    /// </remarks>
    [Browsable(false)]
    public object this[string columnAlias]
    {
      get 
      {
        return GetValue(columnAlias); 
      }
    }

    /// <summary>
    /// Gets data stored in a specified column.
    /// </summary>
    /// <param name="column">The column.</param>
    /// <returns>The column's value.</returns>
    /// <remarks>
    /// You should initialize the datasource by the <b>Init</b> method before using this property.
    /// </remarks>
    [Browsable(false)]
    public object this[Column column]
    {
      get 
      {
        if (InternalRows.Count == 0)
          Init();
        if (column.Calculated)
          return column.Value;
        return GetValue(column); 
      }
    }

    /// <summary>
    /// Forces loading of data for this datasource.
    /// </summary>
    /// <remarks>
    /// This property is <b>false</b> by default. Set it to <b>true</b> if you need to reload data 
    /// each time when the datasource initialized. Note that this may slow down the performance.
    /// </remarks>
    [DefaultValue(false)]
    public bool ForceLoadData
    {
      get { return forceLoadData; }
      set { forceLoadData = value; }
    }

    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public new Type DataType
    {
      get { return base.DataType; }
      set { base.DataType = value; }
    }

    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public new ColumnBindableControl BindableControl
    {
      get { return base.BindableControl; }
      set { base.BindableControl = value; }
    }

    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public new string CustomBindableControl
    {
      get { return base.CustomBindableControl; }
      set { base.CustomBindableControl = value; }
    }

    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public new ColumnFormat Format
    {
      get { return base.Format; }
      set { base.Format = value; }
    }

    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public new string Expression
    {
      get { return base.Expression; }
      set { base.Expression = value; }
    }

    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public new bool Calculated
    {
      get { return base.Calculated; }
      set { base.Calculated = value; }
    }

    /// <summary>
    /// Gets the additional filter settings.
    /// </summary>
    internal Hashtable AdditionalFilter
    {
      get { return additionalFilter; }
    }

    internal ArrayList Rows
    {
      get { return rows; }
    }

    internal ArrayList InternalRows
    {
      get { return internalRows; }
    }
    #endregion

    #region Private Methods
    private void GetChildRows(Relation relation)
    {
      // prepare consts
      object parentRow = relation.ParentDataSource.CurrentRow;
      int columnsCount = relation.ParentColumns.Length;
      object[] parentValues = new object[columnsCount];
      Column[] childColumns = new Column[columnsCount];
      for (int i = 0; i < columnsCount; i++)
      {
        parentValues[i] = relation.ParentDataSource[relation.ParentColumns[i]];
        childColumns[i] = Columns.FindByAlias(relation.ChildColumns[i]);
      }

      if (relation_SortedChildRows == null)
        relation_SortedChildRows = new Hashtable();

      // sort the child table at the first run. Use relation columns to sort.
      SortedList<Indices, ArrayList> sortedChildRows = relation_SortedChildRows[relation] as SortedList<Indices, ArrayList>;
      if (sortedChildRows == null)
      {
        sortedChildRows = new SortedList<Indices,ArrayList>();
        relation_SortedChildRows[relation] = sortedChildRows;
        foreach (object row in InternalRows)
        {
          SetCurrentRow(row);
          
          object[] values = new object[columnsCount];
          for (int i = 0; i < columnsCount; i++)
          {
            values[i] = this[childColumns[i]];
          }
          
          Indices indices = new Indices(values);
          ArrayList rows = null;
          int index = sortedChildRows.IndexOfKey(indices);
          if (index == -1)
          {
            rows = new ArrayList();
            sortedChildRows.Add(indices, rows);
          }
          else
            rows = sortedChildRows.Values[index];
          
          rows.Add(row);
        }
      }

      int indexOfKey = sortedChildRows.IndexOfKey(new Indices(parentValues));
      if (indexOfKey != -1)
      {
        ArrayList rows = sortedChildRows.Values[indexOfKey];
        foreach (object row in rows)
        {
                    this.rows.Add(row);
        }
      }
    }
    
    private void ApplyAdditionalFilter()
    {
      for (int i = 0; i < rows.Count; i++)
      {
        CurrentRowNo = i;
        foreach (DictionaryEntry de in AdditionalFilter)
        {
          object value = Report.GetColumnValueNullable((string)de.Key);
          DataSourceFilter filter = de.Value as DataSourceFilter;

          if (!filter.ValueMatch(value))
          {
            rows.RemoveAt(i);
            i--;
            break;
          }
        }  
      }
    }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Gets data stored in a specified column.
    /// </summary>
    /// <param name="alias">The column alias.</param>
    /// <returns>An object that contains the data.</returns>
    protected virtual object GetValue(string alias)
    {
      Column column = columnIndices[alias] as Column;
      if (column == null)
      {
        column = Columns.FindByAlias(alias);
        columnIndices[alias] = column;
      }

      return GetValue(column);
    }

    /// <summary>
    /// Gets data stored in a specified column.
    /// </summary>
    /// <param name="column">The column.</param>
    /// <returns>An object that contains the data.</returns>
    protected abstract object GetValue(Column column);
    #endregion

    #region Public Methods
    /// <summary>
    /// Initializes the datasource schema.
    /// </summary>
    /// <remarks>
    /// This method is used to support the FastReport.Net infrastructure. Do not call it directly.
    /// </remarks>
    public abstract void InitSchema();
    
    /// <summary>
    /// Loads the datasource with data.
    /// </summary>
    /// <remarks>
    /// This method is used to support the FastReport.Net infrastructure. Do not call it directly.
    /// </remarks>
    /// <param name="rows">Rows to fill with data.</param>
    public abstract void LoadData(ArrayList rows);

    internal void LoadData()
    {
      LoadData(InternalRows);
    }

    internal void OnLoad()
    {
      if (Load != null)
      {
        // clear internal rows to force reload data
        InternalRows.Clear();
        Load(this, EventArgs.Empty);
      }
    }
    
    internal void SetCurrentRow(object row)
    {
      currentRow = row;
    }
    
    internal void FindParentRow(Relation relation)
    {
      InitSchema();
      LoadData();

      int columnCount = relation.ChildColumns.Length;
      object[] childValues = new object[columnCount];
      for (int i = 0; i < columnCount; i++)
      {
        childValues[i] = relation.ChildDataSource[relation.ChildColumns[i]];
      }
      
      object result = null;
      if (childValues[0] == null)
      {
        SetCurrentRow(null);
        return;
      }

      // improve performance for single column index
      if (columnCount == 1)
      {
          if (rowIndices.Count == 0)
          {
              foreach (object row in InternalRows)
              {
                  SetCurrentRow(row);
                  rowIndices[this[relation.ParentColumns[0]]] = row;
              }
          }

        result = rowIndices[childValues[0]];
        if (result != null)
        {
          SetCurrentRow(result);
          return;
        }
      }
      
      foreach (object row in InternalRows)
      {
        SetCurrentRow(row);
        bool found = true;
        
        for (int i = 0; i < columnCount; i++)
        {
          if (!this[relation.ParentColumns[i]].Equals(childValues[i]))
          {
            found = false;
            break;
          }
        }
        if (found)
        {
          result = row;
          break;
        }  
      }

      if (columnCount == 1)
        rowIndices[childValues[0]] = result;
      
      SetCurrentRow(result);
    }

    /// <summary>
    /// Initializes this datasource.
    /// </summary>
    /// <remarks>
    /// This method fills the table with data. You should always call it before using most of
    /// datasource properties.
    /// </remarks>
    public void Init()
    {
      Init("");
    }

    /// <summary>
    /// Initializes this datasource and applies the specified filter.
    /// </summary>
    /// <param name="filter">The filter expression.</param>
    public void Init(string filter)
    {
      Init(filter, null);
    }

    /// <summary>
    /// Initializes this datasource, applies the specified filter and sorts the rows.
    /// </summary>
    /// <param name="filter">The filter expression.</param>
    /// <param name="sort">The collection of sort descriptors.</param>
    public void Init(string filter, SortCollection sort)
    {
      DataSourceBase parentData = null;
      Init(parentData, filter, sort);
    }

    /// <summary>
    /// Initializes this datasource and filters data rows according to the master-detail relation between
    /// this datasource and <b>parentData</b>.
    /// </summary>
    /// <param name="parentData">Parent datasource.</param>
    /// <remarks>
    /// To use master-detail relation, you must define the <see cref="Relation"/> object that describes
    /// the relation, and add it to the <b>Report.Dictionary.Relations</b> collection.
    /// </remarks>
    public void Init(DataSourceBase parentData)
    {
      Init(parentData, "", null);
    }

    /// <summary>
    /// Initializes this datasource and filters data rows according to the master-detail relation between
    /// this datasource and <b>parentData</b>. Also applies the specified filter and sorts the rows.
    /// </summary>
    /// <param name="parentData">Parent datasource.</param>
    /// <param name="filter">The filter expression.</param>
    /// <param name="sort">The collection of sort descriptors.</param>
    /// <remarks>
    /// To use master-detail relation, you must define the <see cref="Relation"/> object that describes
    /// the relation, and add it to the <b>Report.Dictionary.Relations</b> collection.
    /// </remarks>
    public void Init(DataSourceBase parentData, string filter, SortCollection sort)
    {
      Init(parentData, filter, sort, false);
    }

    /// <summary>
    /// Initializes this datasource and filters data rows according to the master-detail relation.
    /// Also applies the specified filter and sorts the rows.
    /// </summary>
    /// <param name="relation">The master-detail relation.</param>
    /// <param name="filter">The filter expression.</param>
    /// <param name="sort">The collection of sort descriptors.</param>
    /// <remarks>
    /// To use master-detail relation, you must define the <see cref="Relation"/> object that describes
    /// the relation, and add it to the <b>Report.Dictionary.Relations</b> collection.
    /// </remarks>
    public void Init(Relation relation, string filter, SortCollection sort)
    {
      Init(relation, filter, sort, false);
    }

    internal void Init(DataSourceBase parentData, string filter, SortCollection sort, bool useAllParentRows)
    {
      Relation relation = parentData != null ? DataHelper.FindRelation(Report.Dictionary, parentData, this) : null;
      Init(relation, filter, sort, useAllParentRows);
    }

    internal void Init(Relation relation, string filter, SortCollection sort, bool useAllParentRows)
    {
      if (FShowAccessDataMessage)
        Config.ReportSettings.OnProgress(Report, Res.Get("Messages,AccessingData"));
        
      // InitSchema may fail sometimes (for example, when using OracleConnection with nested select).
      try
      {
        InitSchema();
      }
      catch
      {
      }
      LoadData();

      // fill rows, emulate relation
      rows.Clear();
      if (relation != null && relation.Enabled)
      {
        if (useAllParentRows)
        {
          DataSourceBase parentData = relation.ParentDataSource;
          // parentData must be initialized prior to calling this method!
          parentData.First();
          while (parentData.HasMoreRows)
          {
            GetChildRows(relation);
            parentData.Next();
          }
        }
        else
          GetChildRows(relation);
      }
      else
      {
        foreach (object row in InternalRows)
        {
          rows.Add(row);
        }
      }

      // filter data rows
      if (FShowAccessDataMessage && rows.Count > 10000)
        Config.ReportSettings.OnProgress(Report, Res.Get("Messages,PreparingData"));
      
      if (filter != null && filter.Trim() != "")
      {
        for (int i = 0; i < rows.Count; i++)
        {
          CurrentRowNo = i;
          object match = Report.Calc(filter);
          if (match is bool && !(bool)match)
          {
            rows.RemoveAt(i);
            i--;
          }  
        }
      }
      
      // additional filter
      if (AdditionalFilter.Count > 0)
        ApplyAdditionalFilter();
      
      // sort data rows
      if (sort != null && sort.Count > 0)
      {
        string[] expressions = new string[sort.Count];
        bool[] descending = new bool[sort.Count];
        for (int i = 0; i < sort.Count; i++)
        {
          expressions[i] = sort[i].Expression;
          descending[i] = sort[i].Descending;
        }
        rows.Sort(new RowComparer(Report, this, expressions, descending));
      }
      
      FShowAccessDataMessage = false;
      First();
    }

    /// <summary>
    /// Initializes the data source if it is not initialized yet.
    /// </summary>
    public void EnsureInit()
    {
      if (InternalRows.Count == 0)
        Init();
    }

    /// <summary>
    /// Navigates to the first row.
    /// </summary>
    /// <remarks>
    /// You should initialize the datasource by the <b>Init</b> method before using this method.
    /// </remarks>
    public void First()
    {
      CurrentRowNo = 0;
    }

    /// <summary>
    /// Navigates to the next row.
    /// </summary>
    /// <remarks>
    /// You should initialize the datasource by the <b>Init</b> method before using this method.
    /// </remarks>
    public void Next()
    {
      CurrentRowNo++;
    }

    /// <summary>
    /// Navigates to the prior row.
    /// </summary>
    /// <remarks>
    /// You should initialize the datasource by the <b>Init</b> method before using this method.
    /// </remarks>
    public void Prior()
    {
      CurrentRowNo--;
    }

    /// <inheritdoc/>
    public override void Serialize(FRWriter writer)
    {
      base.Serialize(writer);
      if (Enabled)
        writer.WriteBool("Enabled", Enabled);
      if (ForceLoadData)
        writer.WriteBool("ForceLoadData", ForceLoadData);
    }

    /// <inheritdoc/>
    public override void Deserialize(FRReader reader)
    {
      // the Clear is needed to avoid duplicate columns in the inherited report
      // when the same datasource is exists in both base and inherited report
      Clear();
      base.Deserialize(reader);
    }

    internal void ClearData()
    {
      columnIndices.Clear();
      rowIndices.Clear();
      InternalRows.Clear();
      Rows.Clear();
      additionalFilter.Clear();
      relation_SortedChildRows = null;
      FShowAccessDataMessage = true;
    }
    
    /// <inheritdoc/>
    public override void InitializeComponent()
    {
      ClearData();
    }
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="DataSourceBase"/> class with default settings.
    /// </summary>
    public DataSourceBase()
    {
      internalRows = new ArrayList();
      rows = new ArrayList();
      additionalFilter = new Hashtable();
      columnIndices = new Hashtable();
      rowIndices = new Hashtable();
      SetFlags(Flags.HasGlobalName, true);
    }
  
    private class RowComparer : IComparer
    {
      private Report report;
      private DataSourceBase dataSource;
      private string[] expressions;
      private bool[] descending;
      private Column[] columns;
      
      public int Compare(object x, object y)
      {
        int result = 0;
        for (int i = 0; i < expressions.Length; i++)
        {
          IComparable i1;
          IComparable i2;
          if (columns[i] == null)
          {
            dataSource.SetCurrentRow(x);
            i1 = report.Calc(expressions[i]) as IComparable;
            dataSource.SetCurrentRow(y);
            i2 = report.Calc(expressions[i]) as IComparable;
          }
          else
          {
            dataSource.SetCurrentRow(x);
            i1 = columns[i].Value as IComparable;
            dataSource.SetCurrentRow(y);
            i2 = columns[i].Value as IComparable;
          }
          
          if (i1 != null)
            result = i1.CompareTo(i2);
          else if (i2 != null)
            result = -1;  
          if (descending[i])
            result = -result;
          if (result != 0)
            break;  
        }
        return result;
      }
      
      public RowComparer(Report report, DataSourceBase dataSource, string[] expressions, bool[] descending)
      {
                this.report = report;
                this.dataSource = dataSource;
                this.expressions = expressions;
                this.descending = descending;
        
        // optimize performance if expression is a single data column
        columns = new Column[expressions.Length];
        for (int i = 0; i < expressions.Length; i++)
        {
          string expr = expressions[i];
          if (expr.StartsWith("[") && expr.EndsWith("]"))
            expr = expr.Substring(1, expr.Length - 2);
                    Column column = DataHelper.GetColumn(this.report.Dictionary, expr);
                    DataSourceBase datasource = DataHelper.GetDataSource(this.report.Dictionary, expr);
          if (column != null && column.Parent == datasource)
            columns[i] = column;
          else
            columns[i] = null;
        }
      }
    }

    private class Indices : IComparable
    {
      private object[] values;

      public int CompareTo(object obj)
      {
        Indices indices = obj as Indices;
        
        int result = 0;
        for (int i = 0; i < values.Length; i++)
        {
          IComparable i1 = indices.values[i] as IComparable;
          IComparable i2 = values[i] as IComparable;

          if (i1 != null)
            result = i1.CompareTo(i2);
          else if (i2 != null)
            result = -1;
          if (result != 0)
            break;
        }
        return result;
      }

      public Indices(object[] values)
      {
                this.values = values;
      }
    }
  }
}
