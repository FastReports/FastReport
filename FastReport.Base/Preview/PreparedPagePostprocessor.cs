using FastReport.Utils;
using System;
using System.Collections.Generic;

namespace FastReport.Preview
{
    internal class PreparedPagePostprocessor
    {
        private Dictionary<string, List<TextObjectBase>> duplicates;
        private Dictionary<string, List<TextObject>> mergedTextObjects;
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

        private void CollectMergedTextObjects(TextObject obj)
        {
            if (mergedTextObjects.ContainsKey(obj.Band.Name))
            {
                List<TextObject> list = mergedTextObjects[obj.Band.Name];
                list.Add(obj);
            }
            else
            {
                List<TextObject> list = new List<TextObject>() { obj };
                mergedTextObjects.Add(obj.Band.Name, list);
            }
        }

        private void MergeTextObjects
            ()
        {
            foreach (var band in mergedTextObjects)
            {
                band.Value.Sort(delegate (TextObject txt, TextObject txt2)
                {
                    if (txt.AbsLeft.CompareTo(txt2.AbsLeft) == 0)
                        return txt.AbsTop.CompareTo(txt2.AbsTop);

                    return txt.AbsLeft.CompareTo(txt2.AbsLeft);
                });

                //Vertical merge
                MergeTextObjectsInBand(band.Value);

                //May be horizontal merge
                MergeTextObjectsInBand(band.Value);
            }
        }

        private void MergeTextObjectsInBand(List<TextObject> band)
        {
            for (int i = 0; i < band.Count; i++)
            {
                for (int j = i + 1; j < band.Count; j++)
                {
                    if (Merge(band[j], band[i]))
                    {
                        TextObject removeObj = band[j];
                        band.Remove(removeObj);
                        removeObj.Dispose();
                        if (j > 0)
                            j--;
                    }
                }
            }
        }

        private bool Merge(TextObject obj, TextObject obj2)
        {
            if (obj2.Text != obj.Text)
                return false;

            var bounds = obj.AbsBounds;
            if (bounds.Width < 0 || bounds.Height < 0)
                Validator.NormalizeBounds(ref bounds);

            var bounds2 = obj2.AbsBounds;
            if (bounds2.Width < 0 || bounds2.Height < 0)
                Validator.NormalizeBounds(ref bounds2);

            if (obj.MergeMode.HasFlag(MergeMode.Vertical) && obj2.MergeMode.HasFlag(MergeMode.Vertical)
                && IsEqualWithInaccuracy(bounds2.Width, bounds.Width) && IsEqualWithInaccuracy(bounds2.Left, bounds.Left))
            {
                if (IsEqualWithInaccuracy(bounds2.Bottom, bounds.Top))
                {
                    obj2.Height += bounds.Height;
                    return true;
                }
                else if (IsEqualWithInaccuracy(bounds2.Top, bounds.Bottom))
                {
                    obj2.Height += bounds.Height;
                    obj2.Top -= bounds.Height;
                    return true;
                }
            }
            else if (obj.MergeMode.HasFlag(MergeMode.Horizontal) && obj2.MergeMode.HasFlag(MergeMode.Horizontal)
                && IsEqualWithInaccuracy(bounds2.Height, bounds.Height) && IsEqualWithInaccuracy(bounds2.Top, bounds.Top))
            {
                if (IsEqualWithInaccuracy(bounds2.Right, bounds.Left))
                {
                    obj2.Width += bounds.Width;
                    return true;
                }
                else if (IsEqualWithInaccuracy(bounds2.Left, bounds.Right))
                {
                    obj2.Width += bounds.Width;
                    obj2.Left -= bounds.Width;
                    return true;
                }
            }

            return false;
        }

        private bool IsEqualWithInaccuracy(float value1, float value2)
        {
            return Math.Abs(value1 - value2) < 0.01;
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

                if (c is BandBase band)
                    band.UpdateWidth();

                if (c is TextObjectBase txt && txt.Duplicates != Duplicates.Show)
                    ProcessDuplicates(txt);

                if (c is TextObject text && text.MergeMode != MergeMode.None)
                    CollectMergedTextObjects(text);
            }

            MergeTextObjects();
            CloseDuplicates();
        }

        public PreparedPagePostprocessor()
        {
            duplicates = new Dictionary<string, List<TextObjectBase>>();
            mergedTextObjects = new Dictionary<string, List<TextObject>>();
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
                {
                    if (c is TextObjectBase txt && txt.Duplicates != Duplicates.Show)
                    {
                        ProcessDuplicates(txt);
                        flag = true; //flag for keep in dictionary
                    }
                    if (c is TextObject text && text.MergeMode != MergeMode.None)
                    {
                        CollectMergedTextObjects(text);
                        flag = true;
                    }
                }
                i++;
                if (flag)
                {
                    b.ExtractMacros();
                    bands[i - 1] = b;
                }
                else
                {
                    b.Dispose();
                }
            }

            MergeTextObjects();
            CloseDuplicates();
        }

        public Base PostProcessBandUnlimitedPage(Base band)
        {
            if (bands.ContainsKey(iBand))
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
