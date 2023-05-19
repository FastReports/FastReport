using FastReport.Utils;
using SkiaSharp;
using System.Drawing;
using System.IO;

namespace FastReport.Plugins
{
    public class WebPCustomLoader : IImageHelperLoader
    {
        public bool CanLoad(byte[] imageData)
        {
            return imageData.Length > 12 &&
                imageData[0] == (byte)'R' && imageData[1] == (byte)'I' && imageData[2] == (byte)'F' && imageData[3] == (byte)'F' &&
                imageData[8] == (byte)'W' && imageData[9] == (byte)'E' && imageData[10] == (byte)'B' && imageData[11] == (byte)'P';
        }

        public bool CanLoad(string fileName)
        {
            return fileName.EndsWith(".webp", System.StringComparison.OrdinalIgnoreCase);
        }

        public bool TryLoad(byte[] imageData, out Image result)
        {
            try
            {
                using (var img = SKBitmap.Decode(imageData))
                {
                    byte[] png = ConvertToPng(img);
                    result = Image.FromStream(new MemoryStream(png));
                    return true;
                }
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("WebPCustomLoader could not load image");
                result = null;
                return false;
            }

        }

        private static byte[] ConvertToPng(SKBitmap img)
        {
            byte[] png;
            var filters = SKPngEncoderFilterFlags.NoFilters;
            int compress = 0;
            var options = new SKPngEncoderOptions(filters, compress);

            using (var pixmap = img.PeekPixels())
            {
                using (var data = pixmap.Encode(options))
                {
                    png = data.ToArray();
                }
            }

            return png;
        }

        public bool TryLoad(string fileName, out Image result)
        {
            try
            {
                var bytes = File.ReadAllBytes(fileName);
                return TryLoad(bytes, out result);
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("WebPCustomLoader could not load image");
                result = null;
                return false;
            }
        }
    }
}
