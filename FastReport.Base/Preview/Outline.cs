using System;
using System.Collections.Generic;
using System.Text;
using FastReport.Utils;

namespace FastReport.Preview
{
  internal class Outline
  {
    private XmlItem rootItem;
    private XmlItem curItem;
    private int firstPassPosition;

    public XmlItem Xml
    {
      get { return rootItem; }
      set
      {
        rootItem = value;
        curItem = rootItem;
        value.Parent = null;
      }
    }
    
    internal bool IsEmpty
    {
      get { return rootItem.Count == 0; }
    }
    
    internal XmlItem CurPosition
    {
      get { return curItem.Count == 0 ? null : curItem[curItem.Count - 1]; }
    }

    private void EnumItems(XmlItem item, float shift)
    {
      int pageNo = int.Parse(item.GetProp("Page"));
      float offset = Converter.StringToFloat(item.GetProp("Offset"));
      item.SetProp("Page", Converter.ToString(pageNo + 1));
      item.SetProp("Offset", Converter.ToString(offset + shift));
      
      for (int i = 0; i < item.Count; i++)
      {
        EnumItems(item[i], shift);
      }
    }

    internal void Shift(XmlItem from, float newY)
    {
      if (from == null || from.Parent == null)
        return;
      int i = from.Parent.IndexOf(from);
      if (i + 1 >= from.Parent.Count)
        return;
      from = from.Parent[i + 1];

      float topY = Converter.StringToFloat(from.GetProp("Offset"));
      EnumItems(from, newY - topY);
    }
    
    public void Add(string text, int pageNo, float offsetY)
    {
      curItem = curItem.Add();
      curItem.Name = "item";
      curItem.SetProp("Text", text);
      curItem.SetProp("Page", pageNo.ToString());
      curItem.SetProp("Offset", Converter.ToString(offsetY));
    }
    
    public void LevelRoot()
    {
      curItem = rootItem;
    }
    
    public void LevelUp()
    {
      if (curItem != rootItem)
        curItem = curItem.Parent;
    }

    public void Clear()
    {
      Clear(-1);
    }

    private void Clear(int position)
    {
      if (position == -1)
        rootItem.Clear();
      else if (position < rootItem.Count)
        rootItem.Items[position].Dispose();

      LevelRoot();
    }

    public void PrepareToFirstPass()
    {
      firstPassPosition = rootItem.Count == 0 ? -1 : rootItem.Count;
      LevelRoot();
    }

    public void ClearFirstPass()
    {
      Clear(firstPassPosition);
    }

    public Outline()
    {
      rootItem = new XmlItem();
      rootItem.Name = "outline";
      LevelRoot();
    }
  }
}
