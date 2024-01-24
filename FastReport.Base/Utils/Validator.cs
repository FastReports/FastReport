using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
        internal static void NormalizeBounds(ref RectangleF bounds)
        {
            if (bounds.Width < 0)
            {
                bounds.X = bounds.Right;
                bounds.Width = -bounds.Width;
            }
            if (bounds.Height < 0)
            {
                bounds.Y = bounds.Bottom;
                bounds.Height = -bounds.Height;
            }
        }

        internal static void GetIntersectingObjects(List<ReportComponentBase> list, BandBase band)
        {
            int n = band.Objects.Count;
            for (int i = 0; i < n; i++)
            {
                var bounds = band.Objects[i].Bounds;
                if (bounds.Width < 0 || bounds.Height < 0)
                    NormalizeBounds(ref bounds);

                // compensate for inaccuracy of designer's grid fit
                bounds.Inflate(-0.01f, -0.01f);

                for (int j = 0; j < n; j++)
                {
                    var bounds1 = band.Objects[j].Bounds;
                    if (bounds1.Width < 0 || bounds1.Height < 0)
                        NormalizeBounds(ref bounds1);

                    if (i != j && bounds.IntersectsWith(bounds1))
                    {
                        list.Add(band.Objects[i]);
                        break;
                    }
                }
            }
        }

        internal static bool RectContainInOtherRect(RectangleF parent, RectangleF child)
        {
            NormalizeBounds(ref parent);
            NormalizeBounds(ref child);

            // compensate for inaccuracy of designer's grid fit
            child.Inflate(-0.01f, -0.01f);

            return parent.Contains(child);
        }

        /// <summary>
        /// Validate report.
        /// </summary>
        /// <param name="report"></param>
        /// <param name="checkIntersectObj">Need set false if enabled backlight intersecting objects and report is designing.</param>
        /// <param name="token">Token for cancelling method if it execute in thread.</param>
        /// <returns>List of errors.</returns>
        public static List<ValidationError> ValidateReport(Report report, bool checkIntersectObj = true, CancellationToken token = default)
        {
            if (report == null)
                return null;
            List<ValidationError> listError = new List<ValidationError>();

            try
            {
                foreach (PageBase page in report.Pages)
                {
                    foreach (Base c in page.AllObjects)
                    {
                        if (token.IsCancellationRequested)
                            return null;

                        if (c is BandBase band && checkIntersectObj)
                        {
                            List<ReportComponentBase> intersectingObjects = new List<ReportComponentBase>();
                            GetIntersectingObjects(intersectingObjects, band);
                            foreach (var obj in intersectingObjects)
                            {
                                listError.Add(new ValidationError(obj.Name, ValidationError.ErrorLevel.Warning, Res.Get("Messages,Validator,IntersectedObjects"), obj));
                            }
                        }

                        if (c is ReportComponentBase comp)
                            listError.AddRange(comp.Validate());
                    }
                }

                bool duplicateName;
                var objects = report.AllObjects;
                for (int i = 0; i < objects.Count - 1; i++)
                {
                    duplicateName = false;
                    for (int j = i + 1; j < objects.Count; j++)
                    {
                        if (token.IsCancellationRequested)
                            return null;

                        if (objects[j] is ReportComponentBase && objects[i].Name == objects[j].Name)
                        {
                            listError.Add(new ValidationError(objects[j].Name, ValidationError.ErrorLevel.Error, Res.Get("Messages,Validator,DuplicateName"), (ReportComponentBase)objects[j]));
                            duplicateName = true;
                        }
                    }
                    if (objects[i] is ReportComponentBase && duplicateName)
                        listError.Add(new ValidationError(objects[i].Name, ValidationError.ErrorLevel.Error, Res.Get("Messages,Validator,DuplicateName"), (ReportComponentBase)objects[i]));
                }
            }
            catch
            {
                // validator should not crash the app
                return null;
            }
            return listError.Distinct().ToList();
        }
    }
}