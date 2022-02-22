using System;
using System.Drawing;

namespace FastReport.Utils
{
    /// <summary>
    /// Contains methods used for validation of report.
    /// </summary>
    public static class Validator
    {
        /// <summary>
        /// Check all objects on band, do they intersect or not.
        /// </summary>
        /// <param name="band">Band that should be checked.</param>
        /// <returns>Returns <b>true</b> if band has intersecting objects. Otherwise <b>false</b>.</returns>
        static public bool ValidateIntersectionAllObjects(BandBase band)
        {
            bool result = false;

            foreach(ReportComponentBase component in band.Objects)
            {
                component.IsIntersectingWithOtherObject = false;
                if (!band.Bounds.Contains(GetReducedRect(component.AbsBounds)))
                {
                    component.IsIntersectingWithOtherObject = true;
                    result = true;
                }
            }

            for(int i = 0; i < band.Objects.Count; i++)
            {
                for (int j = i + 1; j < band.Objects.Count; j++)
                {
                    if (band.Objects[i].Bounds.IntersectsWith(GetReducedRect(band.Objects[j].Bounds)))
                    {
                        result = true;
                        band.Objects[i].IsIntersectingWithOtherObject = true;
                        band.Objects[j].IsIntersectingWithOtherObject = true;
                    }
                }
            }

            return result;
        }

        private static RectangleF GetReducedRect(RectangleF rect)
        {
            return new RectangleF(rect.X + 0.01f, rect.Y + 0.01f, rect.Width - 0.02f, rect.Height - 0.02f);
        }
    } 
}