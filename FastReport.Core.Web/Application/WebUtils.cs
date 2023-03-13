using System;
using System.Collections.Generic;
using System.IO;
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

        internal static string MapPath(string path)
        {
            if (path.IsNullOrWhiteSpace())
                return path;

            if (Path.IsPathRooted(path))
                return path;
#if !WASM
            return Path.Combine(FastReportGlobal.HostingEnvironment.ContentRootPath, path);
#endif
            return string.Empty;
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
