using System;
using System.Collections.Generic;
using System.Text;

namespace FastReport.Web
{
    public static class Extensions
    {
        public static void FindClickedObject<T>(
                this Report Report,
                string objectName,
                int pageN,
                float left,
                float top,
                Action<T, ReportPage, int> action
            )
            where T : ComponentBase
        {
            if (Report.PreparedPages == null)
                return;

            bool found = false;
            while (pageN < Report.PreparedPages.Count && !found)
            {
                ReportPage page = Report.PreparedPages.GetPage(pageN);
                if (page != null)
                {
                    ObjectCollection allObjects = page.AllObjects;
                    var point = new System.Drawing.PointF(left + 1, top + 1);
                    foreach (Base obj in allObjects)
                    {
                        if (obj is T c &&
                            c.Name == objectName &&
                            c.AbsBounds.Contains(point))
                        {
                            action(c, page, pageN);
                            found = true;
                            break;
                        }
                    }
                    page.Dispose();
                    pageN++;
                }
            }
        }
    }
}
