using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using FastReport.Export;

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

        internal static string MapPath(string path)
        {
            if (path.IsNullOrWhiteSpace())
                return path;

            if (Path.IsPathRooted(path))
                return path;

            return Path.Combine(FastReportGlobal.HostingEnvironment.ContentRootPath, path);
        }

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

        internal static void CopyCookies(WebRequest request, HttpContext context)
        {
            CookieContainer cookieContainer = new CookieContainer();
            UriBuilder uri = new UriBuilder
            {
                Scheme = context.Request.Scheme,
                Host = context.Request.Host.Host
            };

            string ARRAffinity = GetWebsiteInstanceId();
            if (!String.IsNullOrEmpty(ARRAffinity))
                cookieContainer.Add(uri.Uri, new Cookie("ARRAffinity", ARRAffinity));

            foreach (string key in context.Request.Cookies.Keys)
                cookieContainer.Add(uri.Uri, new Cookie(key, WebUtility.UrlEncode(context.Request.Cookies[key])));

            HttpWebRequest req = (HttpWebRequest)request;
            req.CookieContainer = cookieContainer;
        }

        internal static string GetARRAffinity()
        {
            string id = GetWebsiteInstanceId();
            if (!String.IsNullOrEmpty(id))
                return String.Concat("&ARRAffinity=", id);
            else
                return String.Empty;
        }

        internal static string GetWebsiteInstanceId()
        {
            return Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID");
        }

        internal static void Write(Stream stream, string value)
        {
            byte[] buf = Encoding.UTF8.GetBytes(value);
            stream.Write(buf, 0, buf.Length);
        }

        internal static bool IsPng(byte[] image)
        {
            byte[] pngHeader = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 };
            bool isPng = true;
            for (int i = 0; i < 8; i++)
                if (image[i] != pngHeader[i])
                {
                    isPng = false;
                    break;
                }
            return isPng;
        }
        #endregion
    }

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
}
