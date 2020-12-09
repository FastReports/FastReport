using System;
using System.Collections.Generic;
using System.Collections;

namespace FastReport.Preview
{
  internal class PreparedPagePosprocessor
  {
    private Dictionary<string, List<TextObjectBase>> duplicates;
    private Dictionary<int, Base> bands;
    int iBand;

    private void ProcessDuplicates(TextObjectBase obj)
    {
      if (duplicates.ContainsKey(obj.Name))
      {
        List<TextObjectBase> list = duplicates[obj.Name];
        TextObjectBase lastObj = list[list.Count - 1];

        bool isDuplicate = true;
        // compare Text
        if (obj.Text != lastObj.Text)
          isDuplicate = false;
        else
        {
          float lastObjBottom = (lastObj.Parent as ReportComponentBase).Bottom;
          float objTop = (obj.Parent as ReportComponentBase).Top;
          if (Math.Abs(objTop - lastObjBottom) > 0.5f)
            isDuplicate = false;
        }

        if (isDuplicate)
        {
          list.Add(obj);
        }
        else
        {
          // close duplicates
          CloseDuplicates(list);
          // add new obj
          list.Clear();
          list.Add(obj);
        }
      }
      else
      {
        List<TextObjectBase> list = new List<TextObjectBase>();
        list.Add(obj);
        duplicates.Add(obj.Name, list);
      }
    }

    private void CloseDuplicates()
    {
      foreach (List<TextObjectBase> list in duplicates.Values)
      {
        CloseDuplicates(list);
      }
    }

    private void CloseDuplicates(List<TextObjectBase> list)
    {
      if (list.Count == 0)
        return;

      Duplicates duplicates = list[0].Duplicates;
      switch (duplicates)
      {
        case Duplicates.Clear:
          CloseDuplicatesClear(list);
          break;
        case Duplicates.Hide:
          CloseDuplicatesHide(list);
          break;
        case Duplicates.Merge:
          CloseDuplicatesMerge(list);
          break;
      }
    }

    private void CloseDuplicatesClear(List<TextObjectBase> list)
    {
      for (int i = 0; i < list.Count; i++)
      {
        if (i > 0)
          list[i].Text = "";
      }
    }

    private void CloseDuplicatesHide(List<TextObjectBase> list)
    {
      for (int i = 0; i < list.Count; i++)
      {
        if (i > 0)
          list[i].Dispose();
      }
    }

    private void CloseDuplicatesMerge(List<TextObjectBase> list)
    {
      float top = list[0].AbsTop;
      
      // dispose all objects except the last one
      for (int i = 0; i < list.Count - 1; i++)
      {
        list[i].Dispose();
      }

      // stretch the last object
      TextObjectBase lastObj = list[list.Count - 1];
      float delta = lastObj.AbsTop - top;
      lastObj.Top -= delta;
      lastObj.Height += delta;
    }

    public void Postprocess(ReportPage page)
    {
      page.ExtractMacros();
      ObjectCollection allObjects = page.AllObjects;
      for (int i = 0; i < allObjects.Count; i++)
      {
        Base c = allObjects[i];
                if (c.Report == null)
                    c.SetReport(page.Report);
        c.ExtractMacros();
        
        if (c is BandBase)
          (c as BandBase).UpdateWidth();

        if (c is TextObjectBase && (c as TextObjectBase).Duplicates != Duplicates.Show)
        {
          ProcessDuplicates(c as TextObjectBase);
        }
      }

      CloseDuplicates();
    }

        public PreparedPagePosprocessor()
        {
            duplicates = new Dictionary<string, List<TextObjectBase>>();
            bands = new Dictionary<int, Base>();
            iBand = 0;

        }

        public void PostprocessUnlimited(PreparedPage preparedPage, ReportPage page)
        {
            bool flag = false;
            int i = 0;
            foreach (Base b in preparedPage.GetPageItems(page, true))
            {
                foreach (Base c in b.AllObjects)
                    if (c is TextObjectBase && (c as TextObjectBase).Duplicates != Duplicates.Show)
                    {
                        ProcessDuplicates(c as TextObjectBase);
                        flag = true;//flag for keep in dictionary
                    }
                i++;
                if (flag)
                {
                    b.ExtractMacros();
                    bands[i-1] = b;
                }
                else
                {
                    b.Dispose();
                }
            }
            CloseDuplicates();
        }

        public Base PostProcessBandUnlimitedPage(Base band)
        {
            if(bands.ContainsKey(iBand))
            {
                Base replaceBand = bands[iBand];
                Base parent = band.Parent;
                band.Parent = null;
                replaceBand.Parent = parent;
                band.Dispose();
                iBand++;
                return replaceBand;
            }
            band.ExtractMacros();
            iBand++;
            return band;
        }
    }
}
