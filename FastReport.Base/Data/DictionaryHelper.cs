using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using FastReport.CrossView;

namespace FastReport.Data
{
    internal class DictionaryHelper
    {
        #region Private Fields

        private Dictionary<string, Base> aliases;
        private Dictionary<string, Base> allObjects;
        private Dictionary<string, Base> allReportObjects;
        private Dictionary<string, Base> dataComponents;
        private Dictionary dictionary;
        private Dictionary<string, Base> fullNames;

        #endregion Private Fields

        #region Internal Methods

        internal void RegisterDataSet(DataSet data, string referenceName, bool enabled)
        {
            foreach (DataTable table in data.Tables)
            {
                PRegisterDataTable(table, referenceName + "." + table.TableName, enabled);
            }
            foreach (DataRelation relation in data.Relations)
            {
                PRegisterDataRelation(relation, referenceName + "." + relation.RelationName, enabled);
            }
        }

        internal void ReRegisterData(List<Dictionary.RegDataItem> fRegisteredItems, bool flag)
        {
            foreach (Dictionary.RegDataItem item in fRegisteredItems)
            {
                PRegisterData(item.data, item.name, flag);
            }
        }

        #endregion Internal Methods

        #region Private Methods

        private void AddBaseToDictionary(Base b)
        {
            if (b is DataComponentBase && (b as DataComponentBase).ReferenceName != null)
            {
                dataComponents[(b as DataComponentBase).ReferenceName] = b;
            }

            if ((b is DataComponentBase || b is Relation) && (b as DataComponentBase).Alias != null)
            {
                aliases[(b as DataComponentBase).Alias] = b;
            }
            if (b is DataSourceBase)
            {
                string fullname = (b as DataSourceBase).FullName;
                if (fullname != null)
                    fullNames[fullname] = b;
            }
            if (b.Name != null)
            {
                allObjects[b.Name] = b;
                allReportObjects[b.Name] = b;
            }
        }

        private void AddBaseWithChiledToDictonary(Base b)
        {
            AddBaseToDictionary(b);
            foreach (Base obj in b.AllObjects)
                AddBaseToDictionary(obj);
        }

        private string CreateUniqueAlias(string alias)
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

        private string CreateUniqueName(string name)
        {
            string baseName = name;
            int i = 1;
            while (FindByName(name) != null || FindReportObjectByName(name) != null)
            {
                name = baseName + i.ToString();
                i++;
            }
            return name;
        }

        private DataComponentBase FindByAlias(string alias)
        {
            Base result = null;
            if (aliases.TryGetValue(alias, out result))
            {
                if ((result is DataConnectionBase || result is Relation))
                    return result as DataComponentBase;
            }
            if (fullNames.TryGetValue(alias, out result))
                if (result is DataSourceBase)
                    return result as DataComponentBase;
            return null;
        }

        private Base FindByName(string name)
        {
            Base result = null;
            if (allObjects.TryGetValue(name, out result))
            {
                if (result is DataConnectionBase || result is DataSourceBase || result is Relation ||
                  (result is Parameter && result.Parent == dictionary) || result is Total)
                    return result;
            }
            return null;
        }

        private DataComponentBase FindDataComponent(string referenceName)
        {
            Base c;
            if (dataComponents.TryGetValue(referenceName, out c))
                if (c is DataComponentBase)
                    return c as DataComponentBase;
            return null;
        }

        private Base FindReportObjectByName(string name)
        {
            Base result = null;
            if (allReportObjects.TryGetValue(name, out result))
            {
                return result;
            }
            return null;
        }

        private void PRegisterBusinessObject(IEnumerable data, string referenceName, int maxNestingLevel, bool enabled)
        {
            dictionary.AddRegisteredItem(data, referenceName);

            Type dataType = data.GetType();
            if (data is BindingSource)
            {
                if ((data as BindingSource).DataSource is Type)
                    dataType = ((data as BindingSource).DataSource as Type);
                else
                    dataType = (data as BindingSource).DataSource.GetType();
            }

            BusinessObjectConverter converter = new BusinessObjectConverter(dictionary);
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
                dictionary.DataSources.Add(source);
                converter.CreateInitialObjects(source, maxNestingLevel);
                AddBaseWithChiledToDictonary(source);
            }
        }

        private void PRegisterData(object data, string name, bool enabled)
        {
            if (data is DataSet)
            {
                RegisterDataSet(data as DataSet, name, enabled);
            }
            else if (data is DataTable)
            {
                PRegisterDataTable(data as DataTable, name, enabled);
            }
            else if (data is DataView)
            {
                PRegisterDataView(data as DataView, name, enabled);
            }
            else if (data is DataRelation)
            {
                PRegisterDataRelation(data as DataRelation, name, enabled);
            }
            else if (data is IEnumerable)
            {
                PRegisterBusinessObject(data as IEnumerable, name, 1, enabled);
            }
            else if (data is IBaseCubeLink)
            {
                PRegisterCubeLink(data as IBaseCubeLink, name, enabled);
            }
        }

        private void PRegisterDataRelation(DataRelation relation, string referenceName, bool enabled)
        {
            dictionary.AddRegisteredItem(relation, referenceName);
            if (FindDataComponent(referenceName) != null)
                return;

            Relation rel = new Relation();
            rel.ReferenceName = referenceName;
            rel.Reference = relation;
            rel.Name = relation.RelationName;
            rel.Enabled = enabled;
            rel.ParentDataSource = dictionary.FindDataTableSource(relation.ParentTable);
            rel.ChildDataSource = dictionary.FindDataTableSource(relation.ChildTable);
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
            dictionary.Relations.Add(rel);
            AddBaseWithChiledToDictonary(rel);
        }

        private void PRegisterDataTable(DataTable table, string referenceName, bool enabled)
        {
            dictionary.AddRegisteredItem(table, referenceName);

            TableDataSource source = FindDataComponent(referenceName) as TableDataSource;
            if (source != null)
            {
                source.Reference = table;
                source.InitSchema();
                source.RefreshColumns(true);
                AddBaseWithChiledToDictonary(source);
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
                    AddBaseWithChiledToDictonary(source);
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
                    dictionary.DataSources.Add(source);
                    AddBaseWithChiledToDictonary(source);
                }
            }
        }

        private void PRegisterDataView(DataView view, string referenceName, bool enabled)
        {
            dictionary.AddRegisteredItem(view, referenceName);

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
                dictionary.DataSources.Add(source);
                AddBaseWithChiledToDictonary(source);
            }
        }

        private void PRegisterCubeLink(IBaseCubeLink cubeLink, string referenceName, bool enabled)
        {
            dictionary.AddRegisteredItem(cubeLink, referenceName);

            CubeSourceBase source = FindDataComponent(referenceName) as CubeSourceBase;
            if (source != null)
            {
                source.Reference = cubeLink;
//                source.InitSchema();
            }
            else
            {
                source = new SliceCubeSource();
                source.ReferenceName = referenceName;
                source.Reference = cubeLink;
                source.Name = CreateUniqueName(referenceName);
                source.Alias = CreateUniqueAlias(source.Alias);
                source.Enabled = enabled;
//                source.InitSchema();
                dictionary.CubeSources.Add(source);
                AddBaseWithChiledToDictonary(source);
            }
        }

        #endregion Private Methods

        #region Public Constructors

        public DictionaryHelper(Dictionary dictionary, ObjectCollection AllDictionaryObjects, ObjectCollection AllDictionaryReportObjects)
        {
            this.dictionary = dictionary;
            dataComponents = new Dictionary<string, Base>();
            aliases = new Dictionary<string, Base>();
            fullNames = new Dictionary<string, Base>();
            allObjects = new Dictionary<string, Base>();
            allReportObjects = new Dictionary<string, Base>();
            foreach (Base obj in AllDictionaryObjects)
            {
                AddBaseToDictionary(obj);
            }
            foreach (Base obj in AllDictionaryReportObjects)
            {
                if (obj.Name != null)
                    allReportObjects[obj.Name] = obj;
            }
        }

        #endregion Public Constructors
    }
}