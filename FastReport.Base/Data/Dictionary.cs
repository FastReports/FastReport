using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using FastReport.Utils;
using System.CodeDom;
using System.ComponentModel;
using System.Collections;
using FastReport.CrossView;
using System.Windows.Forms;

namespace FastReport.Data
{
  /// <summary>
  /// This class stores all report data items such as datasources, connections, relations, parameters,
  /// system variables.
  /// </summary>
  /// <remarks>
  /// You can access the report dictionary via <b>Report.Dictionary</b> property.
  /// </remarks>
  public class Dictionary : Base, IParent
  {
    #region Fields
    private ConnectionCollection connections;
    private DataSourceCollection dataSources;
    private RelationCollection relations;
    private ParameterCollection parameters;
    private SystemVariables systemVariables;
    private TotalCollection totals;
    private CubeSourceCollection cubeSources;
    private List<RegDataItem> registeredItems;
    #endregion

    #region Properties
    /// <summary>
    /// Gets a collection of connection objects available in a report.
    /// </summary>
    public ConnectionCollection Connections
    {
      get { return connections; }
    }
    
    /// <summary>
    /// Gets a collection of datasources available in a report. 
    /// </summary>
    /// <remarks>
    /// Usually you don't need to use this property. It contains only datasources 
    /// registered using the <b>RegisterData</b> method. All other datasources are contained 
    /// in connection objects and may be accessed via <see cref="Connections"/> property. 
    /// </remarks>
    public DataSourceCollection DataSources
    {
      get { return dataSources; }
    }

    /// <summary>
    /// Gets a collection of relations.
    /// </summary>
    public RelationCollection Relations
    {
      get { return relations; }
    }
    
    /// <summary>
    /// Gets a collection of parameters.
    /// </summary>
    /// <remarks>
    /// Another way to access parameters is to use the <b>Report.Parameters</b> property
    /// which is actually a shortcut to this property. You also may use the <b>Report.GetParameter</b>
    /// and <b>Report.GetParameterValue</b> methods.
    /// </remarks>
    public ParameterCollection Parameters
    {
      get { return parameters; }
    }
    
    /// <summary>
    /// Gets a collection of system variables like Date, PageNofM etc.
    /// </summary>
    /// <remarks>
    /// Another way to access a system variable is to use the <b>Report.GetVariableValue</b> method.
    /// </remarks>
    public SystemVariables SystemVariables
    {
      get { return systemVariables; }
    }
    
    /// <summary>
    /// Gets a collection of totals.
    /// </summary>
    /// <remarks>
    /// Another way to get a total value is to use the <b>Report.GetTotalValue</b> method.
    /// </remarks>
    public TotalCollection Totals
    {
      get { return totals; }
    }

    /// <summary>
    /// Gets a collection of cubesources available in a report. 
    /// </summary>
    /// <remarks>
    /// Usually you don't need to use this property. It contains only cubesources 
    /// registered using the <b>RegisterData</b> method.
    /// </remarks>
    public CubeSourceCollection CubeSources
    {
      get { return cubeSources; }
    }

    /// <summary>
    /// Gets a list of registered items.
    /// </summary>
    /// <remarks>
    /// This property is for internal use only.
    /// </remarks>
    public List<RegDataItem> RegisteredItems
    {
      get { return registeredItems; }
    }
    #endregion

    #region Private Methods
    private RegDataItem FindRegisteredItem(object data)
    {
      foreach (RegDataItem item in registeredItems)
      {
        if (item.data == data)
          return item;
      }
      return null;
    }

    private RegDataItem FindRegisteredItem(string name)
    {
      foreach (RegDataItem item in registeredItems)
      {
        if (item.name == name)
          return item;
      }
      return null;
    }

    internal void AddRegisteredItem(object data, string name)
    {
      if (FindRegisteredItem(data) == null)
        registeredItems.Add(new RegDataItem(data, name));
    }
    #endregion
    
    #region Public Methods
    /// <inheritdoc/>
    public override void Assign(Base source)
    {
      BaseAssign(source);
    }

    internal void RegisterDataSet(DataSet data, string referenceName, bool enabled)
    {
      DictionaryHelper helper = new DictionaryHelper(this, AllObjects, Report.AllNamedObjects);
      helper.RegisterDataSet(data, referenceName, enabled);
    }

    internal void RegisterDataTable(DataTable table, string referenceName, bool enabled)
    {
      AddRegisteredItem(table, referenceName);

      TableDataSource source = FindDataComponent(referenceName) as TableDataSource;
      if (source != null)
      {
        source.Reference = table;
        source.InitSchema();
        source.RefreshColumns(true);
      }
      else
      {
        // check tables inside connections. Are we trying to replace the connection table
        // with table provided by an application?
        source = FindByAlias(referenceName) as TableDataSource;
        // check "Data.TableName" case 
        if (source == null && referenceName.StartsWith("Data."))
          source = FindByAlias(referenceName.Remove(0, 5)) as TableDataSource;
        if (source != null && (source.Connection != null || source.IgnoreConnection))
        {
          source.IgnoreConnection = true;
          source.Reference = table;
          source.InitSchema();
          source.RefreshColumns(true);
        }
        else
        {
          source = new TableDataSource();
          source.ReferenceName = referenceName;
          source.Reference = table;
          source.Name = CreateUniqueName(referenceName.Contains(".") ? table.TableName : referenceName);
          source.Alias = CreateUniqueAlias(source.Alias);
          source.Enabled = enabled;
          source.InitSchema();
          DataSources.Add(source);
        }
      }
    }

    /// <summary>
    /// Registers a DataView.
    /// </summary>
    /// <param name="view">The DataView to register.</param>
    /// <param name="referenceName">The name of the data object.</param>
    /// <param name="enabled">Determines wheter to enable the object or not.</param>
    /// <remarks>
    /// This method is for internal use only.
    /// </remarks>
    public void RegisterDataView(DataView view, string referenceName, bool enabled)
    {
      AddRegisteredItem(view, referenceName);

      ViewDataSource source = FindDataComponent(referenceName) as ViewDataSource;
      if (source != null)
      {
        source.Reference = view;
        source.InitSchema();
        source.RefreshColumns();
      }
      else
      {
        source = new ViewDataSource();
        source.ReferenceName = referenceName;
        source.Reference = view;
        source.Name = CreateUniqueName(referenceName);
        source.Alias = CreateUniqueAlias(source.Alias);
        source.Enabled = enabled;
        source.InitSchema();
        DataSources.Add(source);
      }
    }

    internal void RegisterDataRelation(DataRelation relation, string referenceName, bool enabled)
    {
      AddRegisteredItem(relation, referenceName);
      if (FindDataComponent(referenceName) != null)
        return;

      Relation rel = new Relation();
      rel.ReferenceName = referenceName;
      rel.Reference = relation;
      rel.Name = relation.RelationName;
      rel.Enabled = enabled;
      rel.ParentDataSource = FindDataTableSource(relation.ParentTable);
      rel.ChildDataSource = FindDataTableSource(relation.ChildTable);
      string[] parentColumns = new string[relation.ParentColumns.Length];
      string[] childColumns = new string[relation.ChildColumns.Length];
      for (int i = 0; i < relation.ParentColumns.Length; i++)
      {
        parentColumns[i] = relation.ParentColumns[i].Caption;
      }
      for (int i = 0; i < relation.ChildColumns.Length; i++)
      {
        childColumns[i] = relation.ChildColumns[i].Caption;
      }
      rel.ParentColumns = parentColumns;
      rel.ChildColumns = childColumns;
      Relations.Add(rel);
    }

    /// <summary>
    /// Registers a business object.
    /// </summary>
    /// <param name="data">The business object.</param>
    /// <param name="referenceName">The name of the object.</param>
    /// <param name="maxNestingLevel">Maximum level of data nesting.</param>
    /// <param name="enabled">Determines wheter to enable the object or not.</param>
    /// <remarks>
    /// This method is for internal use only.
    /// </remarks>
    public void RegisterBusinessObject(IEnumerable data, string referenceName, int maxNestingLevel, bool enabled)
    {
      AddRegisteredItem(data, referenceName);

      Type dataType = data.GetType();
      if (data is BindingSource)
      {
        if ((data as BindingSource).DataSource is Type)
          dataType = ((data as BindingSource).DataSource as Type);
        else
          dataType = (data as BindingSource).DataSource.GetType();
      }
      
      BusinessObjectConverter converter = new BusinessObjectConverter(this);
      BusinessObjectDataSource source = FindDataComponent(referenceName) as BusinessObjectDataSource;
      if (source != null)
      {
        source.Reference = data;
        source.DataType = dataType;
        converter.UpdateExistingObjects(source, maxNestingLevel);
      }
      else
      {
        source = new BusinessObjectDataSource();
        source.ReferenceName = referenceName;
        source.Reference = data;
        source.DataType = dataType;
        source.Name = CreateUniqueName(referenceName);
        source.Alias = CreateUniqueAlias(source.Alias);
        source.Enabled = enabled;
        DataSources.Add(source);

        converter.CreateInitialObjects(source, maxNestingLevel);
      }
    }

    /// <summary>
    /// Registers a CubeLink.
    /// </summary>
    /// <param name="cubeLink">The CubeLink to register.</param>
    /// <param name="referenceName">The name of the data object.</param>
    /// <param name="enabled">Determines wheter to enable the object or not.</param>
    /// <remarks>
    /// This method is for internal use only.
    /// </remarks>
    public void RegisterCubeLink(IBaseCubeLink cubeLink, string referenceName, bool enabled)
    {
      AddRegisteredItem(cubeLink, referenceName);

      CubeSourceBase source = FindDataComponent(referenceName) as CubeSourceBase;
      if (source != null)
      {
        source.Reference = cubeLink;
      }
      else
      {
        source = new SliceCubeSource();
        source.ReferenceName = referenceName;
        source.Reference = cubeLink;
        source.Name = CreateUniqueName(referenceName);
        source.Alias = CreateUniqueAlias(source.Alias);
        source.Enabled = enabled;
        CubeSources.Add(source);
      }
    }

    /// <summary>
    /// Registers a data object.
    /// </summary>
    /// <param name="data">The object to register.</param>
    /// <param name="name">The name of the object.</param>
    /// <param name="enabled">Determines wheter to enable the object or not.</param>
    /// <remarks>
    /// This method is for internal use only.
    /// </remarks>
    public void RegisterData(object data, string name, bool enabled)
    {
      if (data is DataSet)
      {        
        RegisterDataSet(data as DataSet, name, enabled);
      }
      else if (data is DataTable)
      {       
        RegisterDataTable(data as DataTable, name, enabled);
      }
      else if (data is DataView)
      {
        RegisterDataView(data as DataView, name, enabled);
      }
      else if (data is DataRelation)
      {
        RegisterDataRelation(data as DataRelation, name, enabled);
      }
      else if (data is IEnumerable)
      {
        RegisterBusinessObject(data as IEnumerable, name, 1, enabled);
      }
      else if (data is IBaseCubeLink)
      {
        RegisterCubeLink(data as IBaseCubeLink, name, enabled);
      }
    }
    
    /// <summary>
    /// Unregisters the previously registered data.
    /// </summary>
    /// <param name="data">The application data.</param>
    public void UnregisterData(object data)
    {
      UnregisterData(data, "Data");
    }

    /// <summary>
    /// Unregisters the previously registered data.
    /// </summary>
    /// <param name="data">The application data.</param>
    /// <param name="name">The name of the data.</param>
    /// <remarks>
    /// You must specify the same <b>data</b> and <b>name</b> as when you call <b>RegisterData</b>.
    /// </remarks>
    public void UnregisterData(object data, string name)
    {
      for (int i = 0; i < registeredItems.Count; i++)
      {
        RegDataItem item = registeredItems[i];
        if (item.name == name)
        {
          registeredItems.RemoveAt(i);
          break;
        }
      }

      DataComponentBase comp = FindDataComponent(name);
      if (comp != null)
        comp.Dispose();

      if (data is DataSet)
      {
        foreach (DataTable table in (data as DataSet).Tables)
        {
          UnregisterData(table, name + "." + table.TableName);
        }
        foreach (DataRelation relation in (data as DataSet).Relations)
        {
          UnregisterData(relation, name + "." + relation.RelationName);
        }
      }
    }

    /// <summary>
    /// Re-registers the data registered before.
    /// </summary>
    /// <remarks>
    /// This method is for internal use only.
    /// </remarks>
    public void ReRegisterData()
    {
      ReRegisterData(this);
    }

    /// <summary>
    /// Re-registers the data registered before.
    /// </summary>
    /// <param name="dictionary"></param>
    public void ReRegisterData(Dictionary dictionary)
    {
      // re-register all data items. It is needed after load or "new report" operations
      if(registeredItems.Count>0)
      {
        DictionaryHelper helper = new DictionaryHelper(dictionary, dictionary.AllObjects, dictionary.Report.AllNamedObjects);
        helper.ReRegisterData(registeredItems, false);
      }
    }

    /// <summary>
    /// Clears all registered data.
    /// </summary>
    public void ClearRegisteredData()
    {
      registeredItems.Clear();
    }
    
    /// <summary>
    /// Enables or disables relations between data tables.
    /// </summary>
    /// <remarks>
    /// Call this method if you create master-detail report from code. This method enables
    /// relation between two data tables which <b>Enabled</b> flag is set to <b>true</b>. Relations
    /// whose parent and child tables are disabled, gets disabled too.
    /// </remarks>
    public void UpdateRelations()
    {
      foreach (Relation relation in Relations)
      {
        relation.Enabled = relation.ParentDataSource != null && relation.ParentDataSource.Enabled &&
          relation.ChildDataSource != null && relation.ChildDataSource.Enabled;
      }
    }

    /// <summary>
    /// Creates unique name for data item such as connection, datasource, relation, parameter or total.
    /// </summary>
    /// <param name="name">The base name.</param>
    /// <returns>The new unique name.</returns>
    /// <remarks>
    /// Use this method to create unique name of the data item. It is necessary when you create new
    /// items in code to avoid conflicts with existing report items.
    /// <example>This example show how to add a new parameter:
    /// <code>
    /// Report report1;
    /// Parameter par = new Parameter();
    /// par.Name = report1.Dictionary.CreateUniqueName("Parameter");
    /// report1.Parameters.Add(par);
    /// </code>
    /// </example>
    /// </remarks>
    public string CreateUniqueName(string name)
    {
      string baseName = name;
      int i = 1;
      while (FindByName(name) != null || Report.FindObject(name) != null)
      {
        name = baseName + i.ToString();
        i++;
      }
      return name;
    }

    /// <summary>
    /// Creates unique alias for data item such as connection, datasource or relation.
    /// </summary>
    /// <param name="alias">The base alias.</param>
    /// <returns>The new unique alias.</returns>
    /// <remarks>
    /// Use this method to create unique alias of the data item. It is necessary when you create new
    /// items in code to avoid conflicts with existing report items.
    /// <example>This example show how to add a new table:
    /// <code>
    /// Report report1;
    /// DataConnectionBase conn = report1.Dictionary.Connections.FindByName("Connection1");
    /// TableDataSource table = new TableDataSource();
    /// table.TableName = "Employees";
    /// table.Name = report1.Dictionary.CreateUniqueName("EmployeesTable");
    /// table.Alias = report1.Dictionary.CreateUniqueAlias("Employees");
    /// conn.Tables.Add(table);
    /// </code>
    /// </example>
    /// </remarks>
    public string CreateUniqueAlias(string alias)
    {
      string baseAlias = alias;
      int i = 1;
      while (FindByAlias(alias) != null)
      {
        alias = baseAlias + i.ToString();
        i++;
      }
      return alias;
    }
    
    /// <summary>
    /// Finds a data item such as connection, datasource, relation, parameter or total by its name.
    /// </summary>
    /// <param name="name">The item's name.</param>
    /// <returns>The data item if found; otherwise, <b>null</b>.</returns>
    public Base FindByName(string name)
    {
      foreach (Base c in AllObjects)
      {
        if (c is DataConnectionBase || c is DataSourceBase || c is Relation || 
          (c is Parameter && c.Parent == this) || c is Total || c is CubeSourceBase)
        {
          // check complete match or match without case sensitivity
          if (name == c.Name || name.ToLower() == c.Name.ToLower())
            return c;
        }
      }
      return null;
    }

    /// <summary>
    /// Finds a data item such as connection, datasource or relation by its alias.
    /// </summary>
    /// <param name="alias">The item's alias.</param>
    /// <returns>The data item if found; otherwise, <b>null</b>.</returns>
    public DataComponentBase FindByAlias(string alias)
    {
      foreach (Base c in AllObjects)
      {
        if (c is DataConnectionBase || c is Relation)
        {
          // check complete match or match without case sensitivity
          if (alias == (c as DataComponentBase).Alias || alias.ToLower() == (c as DataComponentBase).Alias.ToLower())
            return c as DataComponentBase;
        }
        else if (c is DataSourceBase)
        {
          // check complete match or match without case sensitivity
          if (alias == (c as DataSourceBase).FullName || alias.ToLower() == (c as DataSourceBase).FullName.ToLower())
            return c as DataSourceBase;
        }
      }
      return null;
    }

    /// <summary>
    /// Finds a datasource that matches the specified DataTable.
    /// </summary>
    /// <param name="table">The <b>DataTable</b> object to check.</param>
    /// <returns>The <b>DataSourceBase</b> object if found.</returns>
    /// <remarks>
    /// This method is for internal use only.
    /// </remarks>
    public DataSourceBase FindDataTableSource(DataTable table)
    {
      foreach (DataSourceBase c in DataSources)
      {
        if (c is TableDataSource && c.Reference == table)
          return c;
      }
      return null;
    }

    /// <summary>
    /// Finds a data component that matches the specified reference name.
    /// </summary>
    /// <param name="referenceName">The name to check.</param>
    /// <returns>The <b>DataComponentBase</b> object if found.</returns>
    /// <remarks>
    /// This method is for internal use only.
    /// </remarks>
    public DataComponentBase FindDataComponent(string referenceName)
    {
      foreach (Base c in AllObjects)
      {
        DataComponentBase data = c as DataComponentBase;
        if (data != null && data.ReferenceName == referenceName)
          return data;
      }
      return null;
    }

    /// <inheritdoc/>
    public override void Serialize(FRWriter writer)
    {
      writer.ItemName = ClassName;
      ObjectCollection childObjects = ChildObjects;
      foreach (Base c in childObjects)
      {
        if (c is Parameter || c is Total || c is CubeSourceBase || (c is DataComponentBase && (c as DataComponentBase).Enabled))
          writer.Write(c);
      }
    }

    /// <inheritdoc/>
    public override void Deserialize(FRReader reader)
    {
      base.Deserialize(reader);
      ReRegisterData();
    }

    /// <summary>
    /// Saves the dictionary to a stream.
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
    /// Saves the dictionary to a file.
    /// </summary>
    /// <param name="fileName">The name of a file to save to.</param>
    public void Save(string fileName)
    {
      using (FileStream f = new FileStream(fileName, FileMode.Create))
      {
        Save(f);
      }
    }

    /// <summary>
    /// Loads the dictionary from a stream.
    /// </summary>
    /// <param name="stream">The stream to load from.</param>
    public void Load(Stream stream)
    {
      Clear();
      using (FRReader reader = new FRReader(Report))
      {
        reader.Load(stream);
        reader.Read(this);
      }
    }

    /// <summary>
    /// Loads the dictionary from a file.
    /// </summary>
    /// <param name="fileName">The name of a file to load from.</param>
    public void Load(string fileName)
    {
      using (FileStream f = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
      {
        Load(f);
      }
    }

    /// <summary>
    /// Merges this dictionary with another <b>Dictionary</b>.
    /// </summary>
    /// <param name="source">Another dictionary to merge the data from.</param>
    public void Merge(Dictionary source)
    {
      // Report object is needed to handle save/load of dictionary correctly.
      // Some dictionary objects (such as relations) may contain references to other objects.
      // In order to clone them correctly, we need a parent Report object, because
      // reader uses it to fixup references.
      
      using (Report cloneReport = new Report())
      {
        Dictionary clone = cloneReport.Dictionary;
        clone.AssignAll(source, true);
        
        foreach (Base c in clone.ChildObjects)
        {
          Base my = FindByName(c.Name);
          if (my != null)
            my.Dispose();
          c.Parent = this;
        }
        
        source.ReRegisterData(this);
        ReRegisterData();
      }  
    }
    #endregion

    #region IParent Members
    /// <inheritdoc/>
    public bool CanContain(Base child)
    {
      return child is DataConnectionBase || child is DataSourceBase || child is Relation || child is Parameter ||
        child is Total || child is CubeSourceBase;
    }

    /// <inheritdoc/>
    public void GetChildObjects(ObjectCollection list)
    {
      foreach (DataConnectionBase c in Connections)
      {
        list.Add(c);
      }
      foreach (DataSourceBase c in DataSources)
      {
        list.Add(c);
      }
      foreach (Relation c in Relations)
      {
        list.Add(c);
      }
      foreach (Parameter c in Parameters)
      {
        list.Add(c);
      }
      foreach (Total c in Totals)
      {
        list.Add(c);
      }
      foreach (CubeSourceBase c in CubeSources)
      {
        list.Add(c);
      }
    }

    /// <inheritdoc/>
    public void AddChild(Base child)
    {
      if (child is DataConnectionBase)
        Connections.Add(child as DataConnectionBase);
      else if (child is DataSourceBase)
        DataSources.Add(child as DataSourceBase);
      else if (child is Relation)
        Relations.Add(child as Relation);
      else if (child is Parameter)
        Parameters.Add(child as Parameter);
      else if (child is Total)
        Totals.Add(child as Total);
      else if (child is CubeSourceBase)
        CubeSources.Add(child as CubeSourceBase);
    }

    /// <inheritdoc/>
    public void RemoveChild(Base child)
    {
      if (child is DataConnectionBase)
        Connections.Remove(child as DataConnectionBase);
      else if (child is DataSourceBase)
        DataSources.Remove(child as DataSourceBase);
      else if (child is Relation)
        Relations.Remove(child as Relation);
      else if (child is Parameter)
        Parameters.Remove(child as Parameter);
      else if (child is Total)
        Totals.Remove(child as Total);
      else if (child is CubeSourceBase)
        CubeSources.Remove(child as CubeSourceBase);
    }

    /// <inheritdoc/>
    public int GetChildOrder(Base child)
    {
      return 0;
    }

    /// <inheritdoc/>
    public void SetChildOrder(Base child, int order)
    {
      // do nothing
    }

    /// <inheritdoc/>
    public void UpdateLayout(float dx, float dy)
    {
      // do nothing
    }
    #endregion
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Dictionary"/> class with default settings.
    /// </summary>
    public Dictionary()
    {
      connections = new ConnectionCollection(this);
      dataSources = new DataSourceCollection(this);
      relations = new RelationCollection(this);
      parameters = new ParameterCollection(this);
      systemVariables = new SystemVariables(this);
      totals = new TotalCollection(this);
      cubeSources = new CubeSourceCollection(this);
      registeredItems = new List<RegDataItem>();
    }
    
    /// <summary>
    /// Represents the item registered in a dictionary.
    /// </summary>
    public class RegDataItem
    {
      /// <summary>
      /// Gets the item data.
      /// </summary>
      public object data;
      
      /// <summary>
      /// Gets the item name.
      /// </summary>
      public string name;

      internal RegDataItem(object data, string name)
      {
                this.data = data;
                this.name = name;
      }
    }
  }
}
