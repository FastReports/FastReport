#if !WASM
using FastReport.Web.Infrastructure;
#endif
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace FastReport.Web
{
    static class WebUtils
    {
        #region Extensions

        internal static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        internal static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        #endregion

        #region Static Methods

        internal static bool ShouldExportUseZipFormat(IEnumerable<KeyValuePair<string, string>> exportParams, string exportFormat) 
            => ShouldUseZipFormat(exportParams, ExportsFromFormat(exportFormat));
        
        internal static bool ShouldExportUseZipFormat(IEnumerable<KeyValuePair<string, string>> exportParams, Exports export) 
            => ShouldUseZipFormat(exportParams, export);

#if !WASM
        internal static string MapPath(string path)
        {
            if (path.IsNullOrWhiteSpace())
                return path;

            if (Path.IsPathRooted(path))
                return path;
            return Path.Combine(FastReportGlobal.HostingEnvironment.ContentRootPath, path);
        }
#endif

        internal static string ToUrl(params string[] segments)
        {
            var sb = new StringBuilder();

            foreach (var segment in segments)
            {
                var trimmedSegment = segment.Trim('/', '\\');
                if (trimmedSegment.IsNullOrWhiteSpace())
                    continue;

                sb.Append('/');
                sb.Append(trimmedSegment);
            }

            return sb.ToString();
        }

        //internal static string GetAppRoot(string path)
        //{
        //    if (path.IndexOf("://") != -1)
        //        return path;

        //    return String.Concat(
        //        FastReportGlobal.HostingEnvironment.ContentRootPath == "/" ? "" : FastReportGlobal.HostingEnvironment.ContentRootPath,
        //        path.IndexOf("/") == 0 ? "" : "/",
        //        path.Replace("~/", ""));
        //}

        internal static void Write(Stream stream, string value)
        {
            byte[] buf = Encoding.UTF8.GetBytes(value);
            stream.Write(buf, 0, buf.Length);
        }

        internal static bool IsPng(byte[] image)
        {
            byte[] pngHeader = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 };
            bool isPng = true;
            for (int i = 0; i < 8 && image.Length > 7; i++)
                if (image[i] != pngHeader[i])
                {
                    isPng = false;
                    break;
                }
            return isPng;
        }
        #endregion

        #region PrivateMethods

        private static bool ShouldUseZipFormat(IEnumerable<KeyValuePair<string, string>> exportParams,
            Exports exportType)
        {
            var exportParamsList = exportParams.ToList();

            var separateFilesParam = exportParamsList.Any(pair => pair is { Key: "SeparateFiles", Value: "true" });
            var embedPicturesParam = exportParamsList.Any(pair => pair is { Key: "EmbedPictures", Value: "true" });

            return (exportType == Exports.Image && (separateFilesParam || !exportParamsList.Any())) ||
                   (exportType == Exports.HTML && !embedPicturesParam);
        }

        private static Exports ExportsFromFormat(string exportFormat)
        {
            return exportFormat switch
            {
                "image" => Exports.Image,
                "html" => Exports.HTML,
                _ => Exports.Text
            };
        }

        #endregion
    }

#if DESIGNER
    /// <summary>
    /// Event arguments for Save report from Designer
    /// </summary>
    public class SaveDesignedReportEventArgs : EventArgs
    {
        /// <summary>
        /// Contain the stream with designed report
        /// </summary>
        public Stream Stream;
    }
#endif
}
