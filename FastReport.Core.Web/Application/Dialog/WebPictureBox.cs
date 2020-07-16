using FastReport.Dialog;
using System.Drawing;
using System.IO;

namespace FastReport.Web
{
    public partial class Dialog
    {

        private string GetPictureBoxHtml(PictureBoxControl control)
        {
            return $"<div style=\"{GetPictureBoxStyle(control)}\"></div>";
        }

        private string GetPictureBoxStyle(PictureBoxControl control)
        {
            return $"{GetControlPosition(control)} {GetControlAlign(control)} {GetPictureBoxURL(control.Image)} padding:0;margin:0;";
        }

        private string GetPictureBoxURL(Image image)
        {
            int hash = image.GetHashCode();
            MemoryStream picStream = new MemoryStream();
            image.Save(picStream, image.RawFormat);
            byte[] imageArray = new byte[picStream.Length];
            picStream.Position = 0;
            picStream.Read(imageArray, 0, (int)picStream.Length);
            WebReport.PictureCache.Add(hash.ToString(), imageArray);

            string url = WebUtils.ToUrl(FastReportGlobal.FastReportOptions.RouteBasePath, $"preview.getPicture?");
            return $" background: url('{url}reportId={WebReport.ID}&pictureId={hash}') no-repeat !important;-webkit-print-color-adjust:exact;";
        }
    }
}
