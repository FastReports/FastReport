using FastReport.Export.Html;
using FastReport.Web.Infrastructure;
#if !OPENSOURCE
using FastReport.Export.Pdf;
#endif
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FastReport.Web.Services
{
    internal sealed class PrintService : IPrintService
    {

        public byte[] PrintReport(WebReport webReport, string printMode)
        {
            switch (printMode)
            {
                case "html":
                    return PrintHtml(webReport);
#if !OPENSOURCE
                case "pdf":
                    return PrintPdf(webReport);
#endif
                default:
                    return null;
            } 
        }

        private byte[] PrintHtml(WebReport webReport)
        {
            webReport.PictureCache.Clear();

            using (var htmlExport = new HTMLExport())
            {
                htmlExport.OpenAfterExport = false;
                htmlExport.Navigator = false;
                htmlExport.Layers = webReport.Layers;
                htmlExport.SinglePage = true;
                htmlExport.Pictures = webReport.Pictures;
                htmlExport.Print = true;
                htmlExport.Preview = true;
                htmlExport.SubFolder = false;
                htmlExport.EmbedPictures = webReport.EmbedPictures;
                htmlExport.EnableMargins = webReport.EnableMargins;
                //htmlExport.WebImagePrefix = WebUtils.ToUrl(FastReportGlobal.FastReportOptions.RouteBasePath, controller.RouteBasePath, ID, "picture") + "/";
                htmlExport.WebImagePrefix = WebUtils.ToUrl(FastReportGlobal.FastReportOptions.RoutePathBaseRoot, FastReportGlobal.FastReportOptions.RouteBasePath, $"preview.getPicture?reportId={webReport.ID}&pictureId=");
                htmlExport.ExportMode = HTMLExport.ExportType.WebPrint;

                byte[] file = null;

                using (MemoryStream ms = new MemoryStream())
                {
                    htmlExport.Export(webReport.Report, ms);
                    file = ms.ToArray();
                }

                if (htmlExport.PrintPageData != null)
                {
                    //WebReportCache cache = new WebReportCache(this.Context);

                    // add all pictures in cache
                    for (int i = 0; i < htmlExport.PrintPageData.Pictures.Count; i++)
                    {
                        Stream stream = htmlExport.PrintPageData.Pictures[i];
                        byte[] image = new byte[stream.Length];
                        stream.Position = 0;
                        int n = stream.Read(image, 0, (int)stream.Length);
                        string picGuid = htmlExport.PrintPageData.Guids[i];
                        //cache.PutObject(picGuid, image);
                        webReport.PictureCache[picGuid] = image;
                    }

                    // cleanup
                    for (int i = 0; i < htmlExport.PrintPageData.Pictures.Count; i++)
                    {
                        Stream stream = htmlExport.PrintPageData.Pictures[i];
                        stream.Dispose();
                        stream = null;
                    }

                    htmlExport.PrintPageData.Pictures.Clear();
                    htmlExport.PrintPageData.Guids.Clear();
                }

                return file;
            }
        }

#if !OPENSOURCE
        private byte[] PrintPdf(WebReport webReport)
        {
            using (var pdfExport = new PDFExport())
            {
                pdfExport.OpenAfterExport = false;
                //pdfExport.EmbeddingFonts = PdfEmbeddingFonts;
                //pdfExport.TextInCurves = PdfTextInCurves;
                //pdfExport.Background = PdfBackground;
                //pdfExport.PrintOptimized = PdfPrintOptimized;
                //pdfExport.Title = PdfTitle;
                //pdfExport.Author = PdfAuthor;
                //pdfExport.Subject = PdfSubject;
                //pdfExport.Keywords = PdfKeywords;
                //pdfExport.Creator = PdfCreator;
                //pdfExport.Producer = PdfProducer;
                //pdfExport.Outline = PdfOutline;
                //pdfExport.DisplayDocTitle = PdfDisplayDocTitle;
                //pdfExport.HideToolbar = PdfHideToolbar;
                //pdfExport.HideMenubar = PdfHideMenubar;
                //pdfExport.HideWindowUI = PdfHideWindowUI;
                //pdfExport.FitWindow = PdfFitWindow;
                //pdfExport.CenterWindow = PdfCenterWindow;
                //pdfExport.PrintScaling = PdfPrintScaling;
                //pdfExport.UserPassword = PdfUserPassword;
                //pdfExport.OwnerPassword = PdfOwnerPassword;
                //pdfExport.AllowPrint = PdfAllowPrint;
                //pdfExport.AllowCopy = PdfAllowCopy;
                //pdfExport.AllowModify = PdfAllowModify;
                //pdfExport.AllowAnnotate = PdfAllowAnnotate;
                //pdfExport.PdfCompliance = PdfA ? PDFExport.PdfStandard.PdfA_2a : PDFExport.PdfStandard.None;
                pdfExport.ShowPrintDialog = true;
                pdfExport.ExportMode = PDFExport.ExportType.WebPrint;

                using (MemoryStream ms = new MemoryStream())
                {
                    pdfExport.Export(webReport.Report, ms);
                    return ms.ToArray();
                }
            }
        }
#endif
    }
}
