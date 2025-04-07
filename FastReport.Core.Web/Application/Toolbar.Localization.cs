using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastReport.Web.Application;

namespace FastReport.Web
{
    internal class ToolbarLocalization
    {

        internal readonly string reloadTxt;
        internal readonly string saveTxt;
        internal readonly string preparedTxt;
        internal readonly string printTxt;
        internal readonly string pdfTxt;
        internal readonly string excel2007Txt;
        internal readonly string word2007Txt;
        internal readonly string powerPoint2007Txt;
        internal readonly string textTxt;
        internal readonly string dbfTxt;
        internal readonly string rtfTxt;
        internal readonly string xpsTxt;
        internal readonly string odsTxt;
        internal readonly string odtTxt;
        internal readonly string xmlTxt;
        internal readonly string csvTxt;
        internal readonly string excel97Txt;
        internal readonly string emailTxt;
        internal readonly string hpglTxt;
        internal readonly string htmlTxt;
        internal readonly string imageTxt;
        internal readonly string jsonTxt;
        internal readonly string dxfTxt;
        internal readonly string latexTxt;
        internal readonly string ppmlTxt;
        internal readonly string psTxt;
        internal readonly string xamlTxt;
        internal readonly string zplTxt;
        internal readonly string svgTxt;
        internal readonly string mhtTxt;

        internal readonly string currentPageTxt;
        internal readonly string totalPagesTxt;
        internal readonly string firstPageTxt;
        internal readonly string previousPageTxt;
        internal readonly string nextPageTxt;
        internal readonly string lastPageTxt;

        internal readonly string printFromBrowserTxt;
        internal readonly string printFromPdf;

        internal readonly string zoomTxt;

        internal readonly string searchTxt;
        internal readonly string searchPlaceholder;
        internal readonly string matchCase;
        internal readonly string wholeWord;
        internal readonly string searchNext;
        internal readonly string searchPrev;
        internal readonly string searchNotFound;

        /// <summary>
        /// Default toolbar localization
        /// </summary>
        public ToolbarLocalization()
        {
            reloadTxt = "Reload";
            saveTxt = "Save";
            preparedTxt = "Prepared report";
            printTxt = "Print";
            pdfTxt = "Adobe PDF";
            excel2007Txt = "Microsoft Excel 2007";
            word2007Txt = "Microsoft Word 2007";
            powerPoint2007Txt = "Microsoft PowerPoint 2007";
            textTxt = "Text File/Matrix Printer";
            rtfTxt = "Rich Text";
            xpsTxt = "Microsoft XPS";
            odsTxt = "OpenOffice Calc";
            odtTxt = "OpenOffice Writer";
            xmlTxt = "XML (Excel) table";
            dbfTxt = "DBF";
            mhtTxt = "MHT";
            csvTxt = "CSV file";
            dxfTxt = "DXF file";
            excel97Txt = "MS Office Excel 97-2003";
            emailTxt = "Email"; // Required modal window
            hpglTxt = "HPGL";
            htmlTxt = "HTML";
            svgTxt = "SVG";
            imageTxt = "Image"; // Required modal window
            jsonTxt = "Json";
            ppmlTxt = "PPML";
            latexTxt = "Latex";
            psTxt = "Post Script";
            xamlTxt = "XAML";
            zplTxt = " Zpl";

            currentPageTxt = "Current page";
            totalPagesTxt = "Total Pages";
            firstPageTxt = "First page";
            previousPageTxt = "Previous page";
            nextPageTxt = "Next page";
            lastPageTxt = "Last page";

            printFromBrowserTxt = "Print from browser";
            printFromPdf = "Print from PDF viewer";

            zoomTxt = "Zoom";
            searchTxt = "Search";
            searchPlaceholder = "Enter the text";
            matchCase = "Match case";
            wholeWord = "Whole word";
            searchNext = "Search next";
            searchPrev = "Search prev";
            searchNotFound = "Not found";
        }

        /// <summary>
        /// Localized toolbar items
        /// </summary>
        /// <param name="Res">WebResources for localization</param>
        /// <example>
        ///    new ToolbarLocalization(webReport.Res);
        /// </example>
        public ToolbarLocalization(IWebRes Res)
        {
            Res.Root("Web");

            reloadTxt = Res.Get("Refresh");
            printTxt = Res.Get("Print");
            zoomTxt = Res.Get("Zoom");
            currentPageTxt = Res.Get("EnterPage");
            totalPagesTxt = Res.Get("TotalPages");
            firstPageTxt = Res.Get("First");
            previousPageTxt = Res.Get("Prev");
            nextPageTxt = Res.Get("Next");
            lastPageTxt = Res.Get("Last");

            searchTxt = Res.Get("Search");
            searchPlaceholder = Res.Get("SearchPlaceholder");
            matchCase = Res.Get("MatchCase");
            wholeWord = Res.Get("WholeWord");
            searchNext = Res.Get("SearchNext");
            searchPrev = Res.Get("SearchPrev");
            searchNotFound = Res.Get("SearchNotFound");

            printFromBrowserTxt = Res.Get("PrintFromBrowser");
            printFromPdf = Res.Get("PrintFromAcrobat");

            Res.Root("Preview");

            saveTxt = Res.Get("Save");
            preparedTxt = Res.Get("SaveNative");

            Res.Root("Export");

            pdfTxt = Res.Get("Pdf");
            excel2007Txt = Res.Get("Xlsx");
            word2007Txt = Res.Get("Docx");
            powerPoint2007Txt = Res.Get("Pptx");
            textTxt = Res.Get("Text");
            rtfTxt = Res.Get("RichText");
            xpsTxt = Res.Get("Xps");
            odsTxt = Res.Get("Ods");
            dbfTxt = Res.Get("Dbf");
            odtTxt = Res.Get("Odt");
            xmlTxt = Res.Get("Xml");
            csvTxt = Res.Get("Csv");
            svgTxt = Res.Get("Svg");
            excel97Txt = Res.Get("Xls");
            hpglTxt = Res.Get("Hpgl");
            htmlTxt = Res.Get("Html");
            jsonTxt = Res.Get("Json");
            dxfTxt = Res.Get("Dxf");
            latexTxt = Res.Get("LaTeX");
            ppmlTxt = Res.Get("Ppml");
            psTxt = Res.Get("PS");
            xamlTxt = Res.Get("Xaml");
            zplTxt = Res.Get("Zpl");
            mhtTxt = Res.Get("Mht");
            emailTxt = Res.Get("Email"); // Required modal window
            imageTxt = Res.Get("Image");
        }

    }
}
