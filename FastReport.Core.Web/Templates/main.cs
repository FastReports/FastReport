using System.Net;

namespace FastReport.Web
{
    partial class WebReport
    {
        string template_FR => $"fr{ID}";
        string template_ROUTE_BASE_PATH => WebUtils.ToUrl(FastReportGlobal.FastReportOptions.RouteBasePath);
        string template_resource_url(string resourceName, string contentType) => $"{template_ROUTE_BASE_PATH}/resources.getResource?resourceName={WebUtility.UrlEncode(resourceName)}&contentType={WebUtility.UrlEncode(contentType)}";
        string template_export_url(string exportFormat) => $"{template_ROUTE_BASE_PATH}/preview.exportReport?reportId={ID}&exportFormat={exportFormat}";
        string template_print_url(string printMode) => $"{template_ROUTE_BASE_PATH}/preview.printReport?reportId={ID}&printMode={printMode}";
        //string template_TOOLBAR_HEIGHT_FACTOR => 40px * ToolbarHeight;

        string template_render(bool renderBody)
        {
            Res.Root("Web");
            string reloadTxt = Res.Get("Refresh");
            string printTxt = Res.Get("Print");
            string printFromBrowserTxt = Res.Get("PrintFromBrowser");
            string printFromPdf = Res.Get("PrintFromAcrobat");
            string zoomTxt = Res.Get("Zoom");
            string firstPageTxt = Res.Get("First");
            string previousPageTxt = Res.Get("Prev");
            string currentPageTxt = Res.Get("EnterPage");
            string nextPageTxt = Res.Get("Next");
            string lastPageTxt = Res.Get("Last");
            string totalPagesTxt = Res.Get("TotalPages");

            Res.Root("Preview");
            string saveTxt = Res.Get("Save");
            string preparedTxt = Res.Get("SaveNative");
            Res.Root("Export");
            string pdfTxt = Res.Get("Pdf");
            string excel2007Txt = Res.Get("Xlsx");
            string word2007Txt = Res.Get("Docx");
            string pptxTxt = Res.Get("Pptx");
            string txtTxt = Res.Get("Text");
            string rtfTxt = Res.Get("RichText");
            string xpsTxt = Res.Get("Xps");
            string odsTxt = Res.Get("Ods");
            string odtTxt = Res.Get("Odt");
            string xmlTxt = Res.Get("Xml");
            string csvTxt = Res.Get("Csv");
            string dbfTxt = Res.Get("Dbf");
            string mhtTxt = Res.Get("Mht");

            return $@"
<div class=""{template_FR}-container"">

    <style>
        {template_style()}
    </style>

    <script>
        {template_script()}
    </script>

    <div class=""{template_FR}-spinner"" {(renderBody ? @"style=""display:none""" : "")}>
        <img src=""{template_resource_url("spinner.svg", "image/svg+xml")}"">
    </div>

  {(ShowBottomToolbar?$@"
{template_body(renderBody)}
{template_toolbar(saveTxt, reloadTxt, preparedTxt, printTxt, printFromBrowserTxt,
    printFromPdf, zoomTxt, firstPageTxt, previousPageTxt, currentPageTxt, nextPageTxt,
    lastPageTxt, totalPagesTxt, pdfTxt, excel2007Txt, word2007Txt, pptxTxt, txtTxt, rtfTxt, xpsTxt, odsTxt,
    odtTxt, xmlTxt, csvTxt, dbfTxt, mhtTxt)}"
    :$@" 
{template_toolbar(saveTxt, reloadTxt, preparedTxt, printTxt, printFromBrowserTxt,
    printFromPdf, zoomTxt, firstPageTxt, previousPageTxt, currentPageTxt, nextPageTxt,
    lastPageTxt, totalPagesTxt, pdfTxt, excel2007Txt, word2007Txt, pptxTxt, txtTxt, rtfTxt, xpsTxt, odsTxt,
    odtTxt, xmlTxt, csvTxt, dbfTxt, mhtTxt)}
{template_body(renderBody)}")}

    

 

</div>";
        }
    }
}