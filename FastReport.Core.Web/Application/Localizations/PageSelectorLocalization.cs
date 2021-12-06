
namespace FastReport.Web.Application.Localizations
{
    internal class PageSelectorLocalization
    {
        internal readonly string PageRange;

        internal readonly string All;

        internal readonly string First;

        internal readonly string LocalizedCancel;
        public PageSelectorLocalization(IWebRes res)
        {
            res.Root("Forms,PrinterSetup");
            PageRange = res.Get("PageRange");
            All = res.Get("All");
            // First is Current ?!
            First = res.Get("Current");

            res.Root("Export,Misc");

            res.Root("Buttons");
            LocalizedCancel = res.Get("Cancel");
        }
    }
}
