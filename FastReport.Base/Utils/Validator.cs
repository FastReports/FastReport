using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace FastReport.Utils
{

    public struct ValidationError
    {
        public enum ErrorLevel
        {
            Warning,
            Error
        }

        public string Name;
        public ErrorLevel Level;
        public string Message;
        public ReportComponentBase Object;

        public ValidationError(string name, ErrorLevel level, string message, ReportComponentBase obj)
        {
            this.Name = name;
            this.Level = level;
            this.Message = message;
            this.Object = obj;
        }
    } 

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

        /// <summary>
        /// Check child rectangle on contains in parent rectangle.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        /// <returns></returns>
        static public bool RectContainInOtherRect(RectangleF parent, RectangleF child)
        {
            return parent.Contains(GetReducedRect(child));
        }

        private static RectangleF GetReducedRect(RectangleF rect)
        {
            return new RectangleF(rect.X + 0.01f, rect.Y + 0.01f, rect.Width - 0.02f, rect.Height - 0.02f);
        }

        static public List<ValidationError> ValidateReport(Report report)
        {
            if (report == null)
                return null;
            List<ValidationError> listError = new List<ValidationError>();

            foreach (PageBase page in report.Pages)
            {
                foreach (Base c in page.AllObjects)
                {
                    if (c is ReportComponentBase)
                        listError.AddRange((c as ReportComponentBase).Validate());

                    if (c is BandBase)
                        ValidateIntersectionAllObjects(c as BandBase);
                }
            }

            bool duplicateName;
            for (int i = 0; i < report.AllObjects.Count - 1; i++)
            {
                duplicateName = false;
                for (int j = i + 1; j < report.AllObjects.Count; j++)
                {
                    if (report.AllObjects[j] is ReportComponentBase && report.AllObjects[i].Name == report.AllObjects[j].Name)
                    {
                        listError.Add(new ValidationError(report.AllObjects[j].Name, ValidationError.ErrorLevel.Error, Res.Get("Messages,Validator,DuplicateName"), (ReportComponentBase)report.AllObjects[j]));
                        duplicateName = true;
                    }
                }
                if(report.AllObjects[i] is ReportComponentBase && duplicateName)
                    listError.Add(new ValidationError(report.AllObjects[i].Name, ValidationError.ErrorLevel.Error, Res.Get("Messages,Validator,DuplicateName"), (ReportComponentBase)report.AllObjects[i]));
            }

            return listError.Distinct().ToList();
        }
    } 
}