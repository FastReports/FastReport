using FastReport.Export.PdfSimple.PdfCore;
using FastReport.Export.PdfSimple.PdfObjects;
using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace FastReport.Export.PdfSimple
{
    partial class PDFSimpleExport
    {
        #region Private Fields

        private Dictionary<string, PdfIndirectObject> hashList;

        #endregion Private Fields

        #region Private Methods

        private string AppendPDFImage(Bitmap image, int quality)
        {
            int[] rawBitmap = GetRawBitmap(image);
            string hash = CalculateHash(rawBitmap);
            PdfIndirectObject imageLink = GetImageByHash(hash);

            if (imageLink == null)
            {
                using (MemoryStream imageStream = new MemoryStream())
                using (MemoryStream maskStream = GetMask(rawBitmap))
                {
                    SaveJpeg(image, imageStream, quality);
                    imageStream.Position = 0;
                    imageLink = WriteImage(imageStream, maskStream, image.Width, image.Height);
                }

                SetImageByHash(hash, imageLink);
            }

            return pdfPage.AddImage(imageLink);
        }

        private string CalculateHash(int[] raw_picture)
        {
            byte[] raw_picture_byte = new byte[raw_picture.Length * sizeof(int)];
            Buffer.BlockCopy(raw_picture, 0, raw_picture_byte, 0, raw_picture_byte.Length);
            byte[] hash = new Murmur3().ComputeHash(raw_picture_byte);
            return Convert.ToBase64String(hash);
        }

        private void DrawImage(RectangleF rectangleF, Bitmap image)
        {
            string imageLink = AppendPDFImage(image, JpegQuality);
            pageContent.Append("q").AppendLine();
            pageContent.Append(rectangleF.Width).Append(" 0 0 ").Append(rectangleF.Height).Append(" ").Append(rectangleF.Left).Append(" ").Append(rectangleF.Top).Append(" cm").AppendLine();
            pageContent.Append(imageLink).Append(" Do").AppendLine();
            pageContent.Append("Q").AppendLine();
        }

        private ImageCodecInfo GetCodec(string codec)
        {
            foreach (ImageCodecInfo ice in ImageCodecInfo.GetImageEncoders())
            {
                if (ice.MimeType == codec)
                    return ice;
            }
            return null;
        }

        private PdfIndirectObject GetImageByHash(string hash)
        {
            PdfIndirectObject result;
            if (hashList.TryGetValue(hash, out result))
                return result;
            else
                return null;
        }

        private MemoryStream GetMask(int[] raw_pixels)
        {
            MemoryStream mask_stream = new MemoryStream(raw_pixels.Length);

            bool alpha = false;
            byte pixel;

            for (int i = 0; i < raw_pixels.Length; i++)
            {
                pixel = (byte)(((UInt32)raw_pixels[i]) >> 24);
                if (!alpha && pixel != 0xff)
                    alpha = true;
                mask_stream.WriteByte(pixel);
            }

            if (alpha)
            {
                mask_stream.Position = 0;
                return mask_stream;
            }

            return null;
        }

        private int[] GetRawBitmap(Bitmap image)
        {
            int raw_size = image.Width * image.Height;
            int[] raw_picture = new int[raw_size];
            BitmapData bmpdata = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
            IntPtr ptr = bmpdata.Scan0;
            System.Runtime.InteropServices.Marshal.Copy(ptr, raw_picture, 0, raw_size);
            image.UnlockBits(bmpdata);
            return raw_picture;
        }

        private void SaveJpeg(System.Drawing.Image image, Stream buff, int quality)
        {
            ImageCodecInfo ici = GetCodec("image/jpeg");
            EncoderParameters ep = new EncoderParameters();
            ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            image.Save(buff, ici, ep);
        }

        private void SetImageByHash(string hash, PdfIndirectObject obj)
        {
            hashList.Add(hash, obj);
        }

        private PdfIndirectObject WriteImage(MemoryStream image, MemoryStream mask, int width, int height)
        {
            if (image == null || image.Length == 0)
                return null;

            PdfImage pdfImage = new PdfImage();
            pdfImage.Width = width;
            pdfImage.Height = height;
            pdfImage.Stream = image.ToArray();

            if (mask != null && mask.Length > 0)
            {
                PdfMask pdfMask = new PdfMask();
                pdfMask.Width = width;
                pdfMask.Height = height;
                pdfMask.Stream = mask.ToArray();

                pdfImage["SMask"] = pdfWriter.Write(pdfMask);
            }

            return pdfWriter.Write(pdfImage);
        }

        #endregion Private Methods
    }
}