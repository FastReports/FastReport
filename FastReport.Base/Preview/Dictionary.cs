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
    //private SortedDictionary<string, DictionaryItem> FNames;    
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
                //                return item.CloneObject(name);
                if(item.OriginalComponent != null)
                    item.OriginalComponent.SetReport(this.preparedPages.Report);
                Base result = item.CloneObject(name);
                //result.SetReport(this);
                return result;
        }
        else
            return null;

      //int i = FNames.IndexOfKey(name);
      //if (i == -1)
      //  return null;
      //return FNames.Values[i].CloneObject(name);
    }

    public Base GetOriginalObject(string name)
    {
        DictionaryItem item;
        if (names.TryGetValue(name, out item))
            return item.OriginalComponent;
        else
            return null;
        
      //  int i = FNames.IndexOfKey(name);
      //if (i == -1)
      //  return null;
      //return FNames.Values[i].OriginalComponent;
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
            //xi.Text = String.Concat("name=\"", pair.Value.SourceName, "\"");
        }

      //for (int i = 0; i < FNames.Count; i++)
      //{
      //  XmlItem xi = rootItem.Add();
      //  xi.Name = FNames.Keys[i];
      //  xi.Text = "name=\"" + FNames.Values[i].SourceName + "\"";
      //}
    }

    public void Load(XmlItem rootItem)
    {
      Clear();
      for (int i = 0; i < rootItem.Count; i++)
      {
        // rootItem[i].Name is 's1', rootItem[i].Text is 'name="Page0.Shape1"'
        string sourceName = rootItem[i].GetProp("name");
        // split to Page0, Shape1
        string[] objName = sourceName.Split('.');
        // get page number
        int pageN = int.Parse(objName[0].Substring(4));
        // get the object
        Base obj = null;
        if (objName.Length == 2)
          obj = preparedPages.SourcePages[pageN].FindObject(objName[1]);
        else
          obj = preparedPages.SourcePages[pageN];
        
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
      //FNames = new SortedDictionary<string, DictionaryItem>(); 
      baseNames = new Hashtable();
    }

    private class DictionaryItem
    {
      private string sourceName;
      private Base originalComponent;
      
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
