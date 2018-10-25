using System;
using System.Collections.Generic;
using System.Text;
using FastReport.Utils;

namespace FastReport.Table
{
  internal class TableStyleCollection : FRCollectionBase
  {
    private TableCell defaultStyle;

    public TableCell DefaultStyle
    {
      get { return defaultStyle; }
    }

    public TableCell this[int index]
    {
      get { return List[index] as TableCell; }
      set { List[index] = value; }
    }

    public TableCell Add(TableCell style)
    {
      for (int i = 0; i < Count; i++)
      {
        if (this[i].Equals(style))
          return this[i];
      }

      TableCell newStyle = new TableCell();
      newStyle.Assign(style);
      List.Add(newStyle);
      return newStyle;
    }

    public TableStyleCollection() : base(null)
    {
      defaultStyle = new TableCell();
    }
  }
}
