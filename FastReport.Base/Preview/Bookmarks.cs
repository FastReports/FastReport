using System;
using System.Collections.Generic;
using System.Text;
using FastReport.Utils;
using System.Collections;

namespace FastReport.Preview
{
  internal class Bookmarks
  {
    private List<BookmarkItem> items;
    private List<BookmarkItem> firstPassItems;
  
    internal int CurPosition
    {
      get { return items.Count; }
    }
    
    internal void Shift(int index, float newY)
    {
      if (index < 0 || index >= items.Count)
        return;
        
      float topY = items[index].offsetY;
      float shift = newY - topY;
      
      for (int i = index; i < items.Count; i++)
      {
        items[i].pageNo++;
        items[i].offsetY += shift;
      }
    }
    
    public void Add(string name, int pageNo, float offsetY)
    {
      BookmarkItem item = new BookmarkItem();
      item.name = name;
      item.pageNo = pageNo;
      item.offsetY = offsetY;
      
      items.Add(item);
    }
    
    public int GetPageNo(string name)
    {
      BookmarkItem item = Find(name);
      if (item == null)
        item = Find(name, firstPassItems);
      return item == null ? 0 : item.pageNo + 1;
    }
    
    public BookmarkItem Find(string name)
    {
      return Find(name, items);
    }

    private BookmarkItem Find(string name, List<BookmarkItem> items)
    {
      if (items == null)
        return null;

      foreach (BookmarkItem item in items)
      {
        if (item.name == name)
          return item;
      }
      
      return null;
    }

    public void Clear()
    {
      items.Clear();
    }

    public void ClearFirstPass()
    {
      firstPassItems = items;
      items = new List<BookmarkItem>();
    }

    public void Save(XmlItem rootItem)
    {
      rootItem.Clear();
      foreach (BookmarkItem item in items)
      {
        XmlItem xi = rootItem.Add();
        xi.Name = "item";
        xi.SetProp("Name", item.name);
        xi.SetProp("Page", item.pageNo.ToString());
        xi.SetProp("Offset", Converter.ToString(item.offsetY));
      }
    }

    public void Load(XmlItem rootItem)
    {
      Clear();
      for (int i = 0; i < rootItem.Count; i++)
      {
        XmlItem item = rootItem[i];
        Add(item.GetProp("Name"), int.Parse(item.GetProp("Page")),
          (float)Converter.FromString(typeof(float), item.GetProp("Offset")));
      }
    }
    
    public Bookmarks()
    {
      items = new List<BookmarkItem>();
    }
    
    
    internal class BookmarkItem
    {
      public string name;
      public int pageNo;
      public float offsetY;
    }
  }
}
