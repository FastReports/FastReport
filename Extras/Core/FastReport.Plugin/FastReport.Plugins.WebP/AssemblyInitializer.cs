using FastReport.Utils;

namespace FastReport.Plugins
{
    public class WebPAssemblyInitializer : AssemblyInitializerBase
    {
        public static WebPCustomLoader DefaultLoader { get; } = new WebPCustomLoader();
        public WebPAssemblyInitializer()
        {
            ImageHelper.Register(DefaultLoader);
        }
    }
}
