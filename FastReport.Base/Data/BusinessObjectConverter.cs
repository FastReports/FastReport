using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using FastReport.Utils;
using System.Windows.Forms;

namespace FastReport.Data
{
  /// <summary>
  /// <b>Obsolete</b>. Specifies a set of flags used to convert business objects into datasources.
  /// </summary>
  [Flags]
  public enum BOConverterFlags
  {
    /// <summary>
    /// Specifies no actions.
    /// </summary>
    None,
    
    /// <summary>
    /// Allows using the fields of a business object.
    /// </summary>
    AllowFields,
    
    /// <summary>
    /// Allows using properties of a business object with <b>BrowsableAttribute</b> only.
    /// </summary>
    BrowsableOnly
  }


  /// <summary>
  /// Specifies a kind of property.
  /// </summary>
  public enum PropertyKind
  {
    /// <summary>
    /// Specifies the property of a simple type (such as integer).
    /// </summary>
    Simple,

    /// <summary>
    /// Specifies the complex property such as class with own properties.
    /// </summary>
    Complex,

    /// <summary>
    /// Specifies the property which is a list of objects (is of IEnumerable type).
    /// </summary>
    Enumerable
  }


  internal partial class BusinessObjectConverter
  {
    private Dictionary dictionary;
    private int nestingLevel;
    private int maxNestingLevel;
    private FastNameCreator nameCreator;
    
    private PropertyKind GetPropertyKind(string name, Type type)
    {
      if (type == null)
        return PropertyKind.Simple;
      
      PropertyKind kind = PropertyKind.Complex;
      if (type.IsValueType ||
        type == typeof(string) ||
        type == typeof(byte[]) ||
        typeof(Image).IsAssignableFrom(type))
      {
        kind = PropertyKind.Simple;
      }
      else if (typeof(IEnumerable).IsAssignableFrom(type))
      {
        kind = PropertyKind.Enumerable;
      }

      GetPropertyKindEventArgs args = new GetPropertyKindEventArgs(name, type, kind);
      Config.ReportSettings.OnGetBusinessObjectPropertyKind(null, args);
      return args.PropertyKind;
    }

    private bool IsSimpleType(string name, Type type)
    {
      return GetPropertyKind(name, type) == PropertyKind.Simple;
    }

    private bool IsEnumerable(string name, Type type)
    {
      return GetPropertyKind(name, type) == PropertyKind.Enumerable;
    }

    private bool IsLoop(Column column, Type type)
    {
      while (column != null)
      {
        if (column.DataType == type)
          return true;
        column = column.Parent as Column;
      }
      return false;
    }

    private PropertyDescriptorCollection GetProperties(Column column)
    {
      using (BindingSource source = new BindingSource())
      {
        source.DataSource = column.Reference != null ? column.Reference : column.DataType;
        // to get properties list of ICustomTypeDescriptor type, we need an instance
        object instance = null;
        if (source.DataSource is Type &&
          typeof(ICustomTypeDescriptor).IsAssignableFrom(source.DataSource as Type))
        {
          try
          {
            GetTypeInstanceEventArgs args = new GetTypeInstanceEventArgs(source.DataSource as Type);
            Config.ReportSettings.OnGetBusinessObjectTypeInstance(null, args);
            instance = args.Instance;
            source.DataSource = instance;
          }
          catch
          {
          }
        }

        // generic list? get element type
        if (column.Reference == null && column.DataType.IsGenericType)
        {
          source.DataSource = column.DataType.GetGenericArguments()[0];
        }

        PropertyDescriptorCollection properties = source.GetItemProperties(null);
        PropertyDescriptorCollection filteredProperties = new PropertyDescriptorCollection(null);

        foreach (PropertyDescriptor prop in properties)
        {
          FilterPropertiesEventArgs args = new FilterPropertiesEventArgs(prop);
          Config.ReportSettings.OnFilterBusinessObjectProperties(source.DataSource, args);
          if (!args.Skip)
            filteredProperties.Add(args.Property);
        }

        if (instance is IDisposable)
        {
          try
          {
            (instance as IDisposable).Dispose();
          }
          catch
          {
          }
        }
        
        return filteredProperties;
      }
    }

    private Column CreateListValueColumn(Column column)
    {
      Type itemType = ListBindingHelper.GetListItemType(column.DataType);

      // find existing column
      Column childColumn = column.FindByPropName("Value");

      // column not found, create new one with default settings
      if (childColumn == null)
      {
        childColumn = new Column();
        childColumn.Name = "Value";
        childColumn.Enabled = IsSimpleType(childColumn.Name, itemType);
        childColumn.SetBindableControlType(itemType);
        column.Columns.Add(childColumn);
      }

      childColumn.DataType = itemType;
      childColumn.PropName = "Value";
      childColumn.PropDescriptor = null;

      return childColumn;
    }

    private void GetReference(Column column, Column childColumn)
    {
      if (!Config.ReportSettings.UsePropValuesToDiscoverBO)
      {
        childColumn.Reference = null;
        return;
      }
      
      object obj = null;
      if (column is BusinessObjectDataSource)
      {
        IEnumerable enumerable = column.Reference as IEnumerable;
        if (enumerable != null)
        {
          IEnumerator enumerator = enumerable.GetEnumerator();
          while (enumerator.MoveNext())
          {
            obj = enumerator.Current;
          }
        }
      }
      else
      {
        obj = column.Reference;
      }

      if (obj != null)
      {
        try
        {
          childColumn.Reference = childColumn.PropDescriptor.GetValue(obj);
        }
        catch
        {
          childColumn.Reference = null;
        }
      }
    }

    private void CreateInitialObjects(Column column)
    {
      if (nestingLevel >= maxNestingLevel)
        return;
      nestingLevel++;

      PropertyDescriptorCollection properties = GetProperties(column);
      foreach (PropertyDescriptor prop in properties)
      {
        Type type = prop.PropertyType;
        bool isSimpleProperty = IsSimpleType(prop.Name, type);
        bool isEnumerable = IsEnumerable(prop.Name, type);

        Column childColumn = null;
        if (isEnumerable)
          childColumn = new BusinessObjectDataSource();
        else
          childColumn = new Column();
        column.Columns.Add(childColumn);

        childColumn.Name = isEnumerable ? dictionary.CreateUniqueName(prop.Name) : prop.Name;
        childColumn.Alias = prop.DisplayName;
        childColumn.DataType = type;
        childColumn.PropName = prop.Name;
        childColumn.PropDescriptor = prop;
        childColumn.SetBindableControlType(type);
        childColumn.Enabled = !isEnumerable || nestingLevel < maxNestingLevel;

        if (!isSimpleProperty)
        {
          GetReference(column, childColumn);
          CreateInitialObjects(childColumn);
        }
      }

      nestingLevel--;
    }

    // create initial n-level structure
    public void CreateInitialObjects(Column column, int maxNestingLevel)
    {
            this.maxNestingLevel = maxNestingLevel;
      CreateInitialObjects(column);
    }

    // update existing columns - add new, delete non-existent, update PropDescriptor
    public void UpdateExistingObjects(Column column, int maxNestingLevel)
    {
            this.maxNestingLevel = maxNestingLevel;
      nameCreator = new FastNameCreator(dictionary.Report.AllNamedObjects);
      UpdateExistingObjects(column);
    }

    private void UpdateExistingObjects(Column column)
    {
      nestingLevel++;

      // reset property descriptors to determine later which columns are outdated
      foreach (Column c in column.Columns)
      {
        c.PropDescriptor = null;
      }

      PropertyDescriptorCollection properties = GetProperties(column);
      if (properties.Count > 0)
      {
        foreach (PropertyDescriptor prop in properties)
        {
          Type type = prop.PropertyType;
          bool isSimpleProperty = IsSimpleType(prop.Name, type);
          bool isEnumerable = IsEnumerable(prop.Name, type);

          // find existing column
          Column childColumn = column.FindByPropName(prop.Name);

          // column not found, create new one
          if (childColumn == null)
          {
            if (isEnumerable)
              childColumn = new BusinessObjectDataSource();
            else
              childColumn = new Column();
            column.Columns.Add(childColumn);

            if (isEnumerable)
              nameCreator.CreateUniqueName(childColumn);
            else
              childColumn.Name = prop.Name;
            childColumn.Alias = prop.DisplayName;
            childColumn.SetBindableControlType(type);

            // enable column if it is simple property, or max nesting level is not reached
            childColumn.Enabled = isSimpleProperty || nestingLevel < maxNestingLevel;
          }

          // update column's prop data - the schema may be changed 
          childColumn.DataType = prop.PropertyType;
          childColumn.PropName = prop.Name;
          childColumn.PropDescriptor = prop;

          if (childColumn.Enabled && !isSimpleProperty)
          {
            GetReference(column, childColumn);
            UpdateExistingObjects(childColumn);
          }
        }

        // remove non-existent columns
        for (int i = 0; i < column.Columns.Count; i++)
        {
          Column c = column.Columns[i];
          // delete columns with empty descriptors, except the "Value" columns
          if (c.PropDescriptor == null && c.PropName != "Value")
          {
            column.Columns.RemoveAt(i);
            i--;
          }
        }
      }
      else if (IsEnumerable(column.Name, column.DataType))
      {
        CreateListValueColumn(column);
      }

      nestingLevel--;
    }

    public BusinessObjectConverter(Dictionary dictionary)
    {
            this.dictionary = dictionary;
    }
  }
}
