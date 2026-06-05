using FastReport.Table;
using FastReport.Utils;
using System;
using System.Collections;
using System.Collections.Generic;

namespace FastReport.Preview
{
    internal class Dictionary
    {
        private SortedList<string, DictionaryItem> names;
        private Hashtable baseNames;
        private PreparedPages preparedPages;

        private void AddBaseName(string name)
        {
            for (int i = 0; i < name.Length; i++)
            {
                if (name[i] >= '0' && name[i] <= '9')
                {
                    string baseName = name.Substring(0, i);
                    int num = int.Parse(name.Substring(i));
                    if (baseNames.ContainsKey(baseName))
                    {
                        int maxNum = (int)baseNames[baseName];
                        if (num < maxNum)
                            num = maxNum;
                    }
                    baseNames[baseName] = num;
                    return;
                }
            }
        }

        public string CreateUniqueName(string baseName)
        {
            int num = 1;
            if (baseNames.ContainsKey(baseName))
                num = (int)baseNames[baseName] + 1;
            baseNames[baseName] = num;
            return baseName + num.ToString();
        }

        private void Add(string name, string sourceName, Base obj)
        {
            names.Add(name, new DictionaryItem(sourceName, obj));
        }

        public string AddUnique(string baseName, string sourceName, Base obj)
        {
            string result = CreateUniqueName(baseName);
            Add(result, sourceName, obj);
            return result;
        }

        public Base GetObject(string name)
        {
            DictionaryItem item;
            if (names.TryGetValue(name, out item))
            {
                if (item.OriginalComponent != null)
                    item.OriginalComponent.SetReport(this.preparedPages.Report);
                Base result = item.CloneObject(name);
                return result;
            }
            else
                return null;
        }

        public Base GetOriginalObject(string name)
        {
            DictionaryItem item;
            if (names.TryGetValue(name, out item))
                return item.OriginalComponent;
            else
                return null;
        }

        public void Clear()
        {
            names.Clear();
            baseNames.Clear();
        }

        public void Save(XmlItem rootItem)
        {
            rootItem.Clear();
            foreach (KeyValuePair<string, DictionaryItem> pair in names)
            {
                XmlItem xi = rootItem.Add();
                xi.Name = pair.Key;
                xi.ClearProps();
                xi.SetProp("name", pair.Value.SourceName);
            }
        }

        public void Load(XmlItem rootItem)
        {
            Clear();
            var finder = new SourceNameFinder(preparedPages.SourcePages);

            for (int i = 0; i < rootItem.Count; i++)
            {
                // rootItem[i].Name is 's1', rootItem[i].Text is 'name="Page0.Shape1"'
                string sourceName = rootItem[i].GetProp("name");
                var obj = finder.FindObject(sourceName);

                // add s1, Page0.Shape1, object
                string name = rootItem[i].Name;
                Add(name, sourceName, obj);
                AddBaseName(name);
            }
        }

        public Dictionary(PreparedPages preparedPages)
        {
            this.preparedPages = preparedPages;
            names = new SortedList<string, DictionaryItem>();
            baseNames = new Hashtable();
        }

        private class SourceNameFinder
        {
            private Dictionary<string, Base> sourceObjects;

            public Base FindObject(string sourceName)
            {
                // source name is like "Page0.Shape1" or "Page0"
                if (sourceObjects.TryGetValue(sourceName, out var obj))
                    return obj;
                // not found.
                return null;
            }

            public SourceNameFinder(SourcePages sourcePages)
            {
                sourceObjects = new Dictionary<string, Base>();

                // collect names in report template pages to speed up search.
                for (int i = 0; i < sourcePages.Count; i++)
                {
                    var page = sourcePages[i];
                    var pageName = $"Page{i}";
                    // add page object
                    sourceObjects.Add(pageName, page);
                    // add all page objects
                    foreach (Base obj in page.AllObjects)
                    {
                        var objName = pageName + "." + obj.Name;
                        if (!sourceObjects.ContainsKey(objName))
                            sourceObjects.Add(objName, obj);
                    }
                }
            }
        }

        private sealed class DictionaryItem
        {
            private readonly string sourceName;
            private readonly Base originalComponent;

            public string SourceName
            {
                get { return sourceName; }
            }

            public Base OriginalComponent
            {
                get { return originalComponent; }
            }

            public Base CloneObject(string alias)
            {
                Base result = null;
                Type type = originalComponent.GetType();

                // try frequently used objects first. The CreateInstance method is very slow.
                if (type == typeof(TextObject))
                    result = new TextObject();
                else if (type == typeof(TableCell))
                    result = new TableCell();
                else if (type == typeof(DataBand))
                    result = new DataBand();
                else
                    result = Activator.CreateInstance(type) as Base;

                result.Assign(originalComponent);
                result.OriginalComponent = originalComponent;
                result.Alias = alias;
                result.SetName(originalComponent.Name);
                if (result is ReportComponentBase)
                    (result as ReportComponentBase).AssignPreviewEvents(originalComponent);
                return result;
            }

            public DictionaryItem(string name, Base obj)
            {
                sourceName = name;
                originalComponent = obj;
            }
        }
    }
}
