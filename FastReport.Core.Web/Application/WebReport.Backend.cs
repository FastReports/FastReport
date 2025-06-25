using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Html;
using System.Threading.Tasks;
using System.Linq;
using FastReport.Web.Cache;
using FastReport.Web.Services;

namespace FastReport.Web
{

    public partial class WebReport
    {
        private string localizationFile;


        /// <summary>
        /// Gets or sets the WebReport's locale
        /// </summary>
        public string LocalizationFile
        {
            get => localizationFile;
            set
            {
                localizationFile = value;
                string path = WebUtils.MapPath(localizationFile);
                Res.LoadLocale(path);
            }
        }

        /// <summary>
        /// Specifies individual cache settings for the current WebReport. 
        /// These settings have the highest priority over other cache configurations.
        /// Default value: null.
        /// /// </summary>
        public WebReportCacheOptions? CacheOptions { get; set; }

        internal static IResourceLoader ResourceLoader { get; set; }


        public HtmlString RenderSync()
        {
            return Task.Run(() => Render()).Result;
        }

        public async Task<HtmlString> Render()
        {
            if (Report == null)
                throw new Exception("Report is null");

            AddToCache();

            return Render(false);
        }


        internal HtmlString Render(bool renderBody)
        {
            switch (Mode)
            {
#if DIALOGS
                case WebReportMode.Dialog:
#endif
                case WebReportMode.Preview:
                    return new HtmlString(template_render(renderBody));
#if DESIGNER
                case WebReportMode.Designer:
                    return RenderDesigner();
#endif
                default:
                    throw new Exception($"Unknown mode: {Mode}");
            }
        }

        private void AddToCache()
        {
            WebReportCache.Instance?.Add(this);
        }

        /// <summary>
        /// Force report to be removed from internal cache
        /// </summary>
        public void RemoveFromCache()
        {
            WebReportCache.Instance?.Remove(ID);
        }

    }
}
