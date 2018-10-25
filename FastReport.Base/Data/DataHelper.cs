using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;

namespace FastReport.Data
{
  internal static class DataHelper
  {
    public static DataSourceBase GetDataSource(Dictionary dictionary, string complexName)
    {
      if (String.IsNullOrEmpty(complexName))
        return null;
      
      string[] names = complexName.Split(new char[] { '.' });
      DataSourceBase data = dictionary.FindByAlias(names[0]) as DataSourceBase;
      if (data == null)
        return null;

      Column column = data;
      
      // take into account nested tables. Table may even be nested into Column.
      for (int i = 1; i < names.Length; i++)
      {
        Column childColumn = column.Columns.FindByAlias(names[i]);
        if (childColumn == null)
          break;

        if (childColumn is DataSourceBase)
          data = childColumn as DataSourceBase;
        column = childColumn;
      }  

      return data;
    }

    public static Column GetColumn(Dictionary dictionary, string complexName)
    {
      if (String.IsNullOrEmpty(complexName))
        return null;

      string[] names = complexName.Split(new char[] { '.' });
      // the first part of complex name is always datasource name.
      DataSourceBase data = dictionary.FindByAlias(names[0]) as DataSourceBase;
      
      return GetColumn(dictionary, data, names, false);
    }

    public static Column GetColumn(Dictionary dictionary, DataSourceBase data, string[] names, bool initRelation)
    {
      // Process the following cases:
      // - Table.Column
      // - Table.NestedTable.Column (specific to BO)
      // - Table.RelatedTable.Column
      // - Table.Column where Column has '.' inside (specific to MSSQL)
      // - Table.ComplexColumn.Column (specific to BO)

      if (data == null || names.Length < 2)
        return null;

      // search for relation
      int i = 1;
      for (; i < names.Length; i++)
      {
        bool found = false;
        foreach (Relation r in dictionary.Relations)
        {
          if (r.ChildDataSource == data &&
            (r.ParentDataSource != null && r.ParentDataSource.Alias == names[i] || r.Alias == names[i]))
          {
            data = r.ParentDataSource;
            if (initRelation)
              data.FindParentRow(r);
            found = true;
            break;
          }
        }

        // nothing found, break and try column name.
        if (!found)
          break;
      }

      // the rest is column name.
      ColumnCollection columns = data.Columns;

      // try full name first
      string columnName = String.Empty;
      for (int j = i; j < names.Length; j++)
        columnName += (columnName.Length == 0 ? "" : ".") + names[j];

      Column column = columns.FindByAlias(columnName);
      if (column != null)
        return column;

      // try nested columns
      for (int j = i; j < names.Length; j++)
      {
        column = columns.FindByAlias(names[j]);
        if (column == null)
          return null;
        columns = column.Columns;
      }

      return column;
    }

    public static bool IsValidColumn(Dictionary dictionary, string complexName)
    {
      return GetColumn(dictionary, complexName) != null;
    }

    // Checks if the specified column name is simple column.
    // The column is simple if it belongs to the datasource, and that datasource
    // is simple as well. This check is needed when we prepare a script for compile.
    // Simple columns are handled directly by the Report component, so they should be
    // skipped when generating the expression handler code.
    public static bool IsSimpleColumn(Dictionary dictionary, string complexName)
    {
      Column column = GetColumn(dictionary, complexName);
      return column != null && column.Parent is DataSourceBase && 
        (column.Parent as DataSourceBase).FullName + "." + column.Alias == complexName;
    }
    
    public static bool IsValidParameter(Dictionary dictionary, string complexName)
    {
      string[] names = complexName.Split(new char[] { '.' });
      Parameter par = dictionary.Parameters.FindByName(names[0]);
      if (par == null)
      {
        if (names.Length == 1)
          par = dictionary.SystemVariables.FindByName(names[0]);
        return par != null;
      }  
        
      for (int i = 1; i < names.Length; i++)
      {
        par = par.Parameters.FindByName(names[i]);
        if (par == null)
          return false;
      }
      return true;
    }

    public static bool IsValidTotal(Dictionary dictionary, string name)
    {
      Total sum = dictionary.Totals.FindByName(name);
      return sum != null;
    }

    public static Parameter GetParameter(Dictionary dictionary, string complexName)
    {
      string[] names = complexName.Split(new char[] { '.' });
      Parameter par = dictionary.Parameters.FindByName(names[0]);
      if (par == null)
      {
        par = dictionary.SystemVariables.FindByName(names[0]);
        return par;
      }  

      for (int i = 1; i < names.Length; i++)
      {
        par = par.Parameters.FindByName(names[i]);
        if (par == null)
          return null;
      }
      return par;
    }

    public static Parameter CreateParameter(Dictionary dictionary, string complexName)
    {
      string[] names = complexName.Split(new char[] { '.' });
      ParameterCollection parameters = dictionary.Parameters;
      Parameter par = null;
      
      for (int i = 0; i < names.Length; i++)
      {
        par = parameters.FindByName(names[i]);
        if (par == null)
        {
          par = new Parameter(names[i]);
          parameters.Add(par);
        }
        parameters = par.Parameters;
      }
      
      return par;
    }

    public static object GetTotal(Dictionary dictionary, string name)
    {
      Total sum = dictionary.Totals.FindByName(name);
      if (sum != null)
        return sum.Value;
      return null;  
    }

    public static Type GetColumnType(Dictionary dictionary, string complexName)
    {
      Column column = GetColumn(dictionary, complexName);
      return column.DataType;
    }

    public static Relation FindRelation(Dictionary dictionary, DataSourceBase parent, DataSourceBase child)
    {
      foreach (Relation relation in dictionary.Relations)
      {
        if (relation.ParentDataSource == parent && relation.ChildDataSource == child)
          return relation;
      }
      return null;
    }
  }
}
