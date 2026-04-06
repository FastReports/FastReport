
namespace FastReport.Web
{
    internal static class Constants
    {

        internal const string DIALOG = "_dialog";

        internal const string REALOAD = "_reload";

        internal const string SILENT_RELOAD = "_silentReload";

    }

    internal static class JSEvents
    {
        internal const string CLICK = "click";
        internal const string INPUT = "input";
        internal const string CHANGE = "change";
    }


#if DIALOGS
    public partial class Dialog
    {
        internal const string ONCLICK = "onclick";

        internal const string ONCHANGE = "onchange";

        internal const string ONBLUR = "onblur";

        public const string DEFAULT_DATE_PICKER_FORMAT = "yyyy-MM-dd";

        public const string DEFAULT_TIME_PICKER_FORMAT = "HH:mm:ss";

    }
#endif
}
