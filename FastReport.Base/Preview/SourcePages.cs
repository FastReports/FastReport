using System;
using System.Collections.Generic;
using System.Text;
using FastReport.Utils;

namespace FastReport.Preview
{
  internal class SourcePages : IDisposable
  {
    #region Fields
    private List<ReportPage> pages;
    private PreparedPages preparedPages;
    #endregion

    #region Properties
    public int Count
    {
      get { return pages.Count; }
    }

    public ReportPage this[int index]
    {
      get { return pages[index]; }
    }
    #endregion

    #region Private Methods
    private Base CloneObjects(Base source, Base parent)
    {
      if (source is ReportComponentBase && !(source as ReportComponentBase).FlagPreviewVisible)
        return null;

      // create clone object and assign all properties from source
      string baseName = "";
      string objName;
#if MONO
      if (source is RichObject && (source as RichObject).ConvertRichText == true)
      {
        RichObject rich = source as RichObject;
        float h;
        List<ComponentBase> clone_list = rich.Convert2ReportObjects(out h);

        int i = 1;
        foreach(Base clone_item in clone_list)
        {
          baseName = clone_item.BaseName[0].ToString().ToLower();
          clone_item.Name = rich.Name + "_" + i;
          objName = "Page" + pages.Count.ToString() + "." + clone_item.Name;
          clone_item.Alias = preparedPages.Dictionary.AddUnique(baseName, objName, clone_item);
          source.Alias = clone_item.Alias;
          clone_item.Parent = parent;
          i++;
        }
        return null;
      }
#endif
      Base clone = Activator.CreateInstance(source.GetType()) as Base;
      using (XmlItem xml = new XmlItem())
      using (FRWriter writer = new FRWriter(xml))
      using (FRReader reader = new FRReader(null, xml))
      {
        reader.DeserializeFrom = SerializeTo.SourcePages;
        writer.SaveChildren = false;
        writer.SerializeTo = SerializeTo.SourcePages;
        writer.Write(source, clone);
        reader.Read(clone);
      }
      clone.Name = source.Name;
      clone.OriginalComponent = source;
      source.OriginalComponent = clone;
      if (clone is ReportComponentBase)
        (clone as ReportComponentBase).AssignPreviewEvents(source);
      // create alias
      objName = "Page" + pages.Count.ToString() + "." + clone.Name;
      if (clone is BandBase)
        baseName = "b";
      else if (clone is PageBase)
      {
        baseName = "page";
        objName = "Page" + pages.Count.ToString();
      }
      else
        baseName = clone.BaseName[0].ToString().ToLower();

      clone.Alias = preparedPages.Dictionary.AddUnique(baseName, objName, clone);
      source.Alias = clone.Alias;

      ObjectCollection childObjects = source.ChildObjects;
      foreach (Base c in childObjects)
      {
        CloneObjects(c, clone);
      }
      clone.Parent = parent;
      return clone;
    }
#endregion

#region Public Methods
    public void Add(ReportPage page)
    {
      pages.Add(CloneObjects(page, null) as ReportPage);
    }

    public void Clear()
    {
      while (pages.Count > 0)
      {
        pages[0].Dispose();
        pages.RemoveAt(0);
      }
    }
    
    public int IndexOf(ReportPage page)
    {
      return pages.IndexOf(page);
    }
    
    public void ApplyWatermark(Watermark watermark)
    {
      foreach (ReportPage page in pages)
      {
        page.Watermark = watermark.Clone();
      }
    }
    
    public void ApplyPageSize()
    {
    }

    public void Load(XmlItem rootItem)
    {
      Clear();
      for (int i = 0; i < rootItem.Count; i++)
      {
        using (FRReader reader = new FRReader(null, rootItem[i]))
        {
          pages.Add(reader.Read() as ReportPage);
        }
      }
    }

    public void Save(XmlItem rootItem)
    {
      rootItem.Clear();
      for (int i = 0; i < pages.Count; i++)
      {
        using (FRWriter writer = new FRWriter(rootItem.Add()))
        {
          writer.Write(pages[i]);
        }
      }  
    }

    public void Dispose()
    {
      Clear();
    }
#endregion

    public SourcePages(PreparedPages preparedPages)
    {
            this.preparedPages = preparedPages;
      pages = new List<ReportPage>();
    }
  }
}
