using FastReport.Design;
using FastReport.Plugins.Transport;
using FastReport.Preview;
using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.IO;

namespace FastReport.Plugins
{
    public class TransportAssemblyInitializer : AssemblyInitializerBase
    {
        private Dictionary<Designer, TransportDesignerExtension> designerExtensions = new Dictionary<Designer, TransportDesignerExtension>();
        private Dictionary<PreviewControl, TransportPreviewExtension> previewExtensions = new Dictionary<PreviewControl, TransportPreviewExtension>();
        public TransportAssemblyInitializer()
        {
            Config.DesignerSettings.DesignerLoaded += DesignerSettings_DesignerLoaded;
            Config.ReportSettings.PreviewLoaded += ReportSettings_PreviewLoaded;
            Res.LocaleChanged += Res_LocaleChanged;
        }
        protected void ReportSettings_PreviewLoaded(object sender, EventArgs e)
        {
            if (sender is PreviewControl preview)
            {
                if (previewExtensions.ContainsKey(preview))
                {
                    // preview removes transport buttons; they need to be restored
                    previewExtensions[preview].InitButtons();
                    return;
                }

                var previewExtension = new TransportPreviewExtension(preview);
                previewExtensions.Add(preview, previewExtension);
                preview.Disposed += (s, ev) =>
                {
                    previewExtensions.Remove(preview);
                };

            }
        }

        protected void DesignerSettings_DesignerLoaded(object sender, System.EventArgs e)
        {
            if (sender is Designer designer)
            {
                if (designerExtensions.ContainsKey(designer))
                {
                    return;
                }

                var designerExtension = new TransportDesignerExtension(designer);
                designerExtensions.Add(designer, designerExtension);

                designer.Disposed += (s, ev) =>
                {
                    if (!designerExtension.IsDisposed)
                    {
                        designerExtension.Dispose();
                        designerExtensions.Remove(designer);
                    }
                };
            }
        }

        private void Res_LocaleChanged(object sender, LocalizationChangeEventArgs e)
        {
            MemoryStream ms = new MemoryStream();
            XmlWriter wr = new XmlWriter(ms);
            wr.AutoIndent = true;
            wr.IsWriteHeader = true;
            wr.Write(Res.GetXmlItem("Transport", e.NewLocale) ?? Res.GetXmlItem("Transport", e.BuiltinLocale));
            ms.Position = 0;
            FastReport.Transport.Localization.SetCustomLocale(ms);
            FastReport.Transport.Localization.Language = FastReport.Transport.Language.Custom;
            foreach (var item in designerExtensions)
                item.Value.Localize();
            foreach (var item in previewExtensions)
                item.Value.Localize();
        }
    }
}
