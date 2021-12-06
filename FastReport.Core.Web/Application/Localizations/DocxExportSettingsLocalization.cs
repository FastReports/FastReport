
namespace FastReport.Web.Application.Localizations
{
    internal class DocxExportSettingsLocalization
    {
        internal readonly string Title;
        internal readonly string Options;
        internal readonly string DoNotExpandShiftReturn;
        internal readonly string Minimum;
        internal readonly string Layers;
        internal readonly string Paragraphs;
        internal readonly string Table;
        internal readonly string PrintOptimized;
        internal readonly string Exactly;
        internal readonly string RowHeightIs;

        public DocxExportSettingsLocalization(IWebRes res)
        {
            res.Root("Export,Docx");
            Title = res.Get(""); 
            DoNotExpandShiftReturn = res.Get("DoNotExpandShiftReturn");
            Minimum = res.Get("Minimum");
            Exactly = res.Get("Exactly");
            RowHeightIs = res.Get("RowHeight");

            res.Root("Export,Misc");
            Layers = res.Get("LayerBased");
            Paragraphs = res.Get("ParagraphBased");
            Table = res.Get("TableBased");
            PrintOptimized = res.Get("PrintOptimized");
            Options = res.Get("Options");
           
          
        }
    }
}
