using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;

namespace FastReport.Utils
{
    internal static class ImageHelper
    {
        public static Bitmap CloneBitmap(Image source)
        {
            if (source == null)
                return null;

            Bitmap image = new Bitmap(source.Width, source.Height);
            image.SetResolution(source.HorizontalResolution, source.VerticalResolution);
            using (Graphics g = Graphics.FromImage(image))
            {
                g.DrawImageUnscaled(source, 0, 0);
            }
            return image;

            // this can throw OutOfMemory when creating a grayscale image from a cloned bitmap
            //      return source.Clone() as Bitmap;
        }

        public static void Save(Image image, Stream stream)
        {
            Save(image, stream, image.GetImageFormat());
        }

        public static void Save(Image image, string fileName, ImageFormat format)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Create))
            {
                Save(image, stream, format);
            }
        }

        public static void Save(Image image, Stream stream, ImageFormat format)
        {
            if (image == null)
                return;
            if (image is Bitmap)
            {
                if (format == ImageFormat.Icon)
                    SaveAsIcon(image, stream, true);
                else
                    image.Save(stream, format);
            }
            else if (image is Metafile)
            {
                Metafile emf = null;
                using (Bitmap bmp = new Bitmap(1, 1))
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    IntPtr hdc = g.GetHdc();
                    emf = new Metafile(stream, hdc);
                    g.ReleaseHdc(hdc);
                }
                using (Graphics g = Graphics.FromImage(emf))
                {
                    g.DrawImage(image, 0, 0);
                }
            }
        }

        public static bool SaveAndConvert(Image image, Stream stream, ImageFormat format)
        {
            if (image == null)
                return false;
            if(format == ImageFormat.Jpeg || format == ImageFormat.Gif 
                || format == ImageFormat.Tiff || format == ImageFormat.Bmp 
                || format == ImageFormat.Png 
                || format == ImageFormat.MemoryBmp)
            {
                if(image is Bitmap)
                {
                    if (format == ImageFormat.MemoryBmp)
                        throw new Exception(Res.Get("Export,Image,ImageParceFormatException")); 
                    image.Save(stream, format);
                    return true;
                }
                //from mf to bitmap
                using (Metafile metafile = image as Metafile)
                using (Bitmap bitmap = new Bitmap(image.Width, image.Height))
                {
                    bitmap.SetResolution(96F, 96F);
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        g.DrawImage(metafile, 0, 0, (float)image.Width, (float)image.Height);
                        g.Dispose();
                    }
                    bitmap.Save(stream, format);
                }
                return true;

            }   
            else if(format == ImageFormat.Icon)
            {
                return SaveAsIcon(image, stream, true);
            }
            else if(format == ImageFormat.Wmf || format == ImageFormat.Emf)
            {
                if (image is Metafile)
                {
                    Metafile emf = null;
                    using (Bitmap bmp = new Bitmap(1, 1))
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        IntPtr hdc = g.GetHdc();
                        emf = new Metafile(stream, hdc);
                        g.ReleaseHdc(hdc);
                    }
                    using (Graphics g = Graphics.FromImage(emf))
                    {
                        g.DrawImage(image, 0, 0);
                    }
                    return true;
                 }
            }
            //throw new Exception(Res.Get("Export,Image,ImageParceFormatException")); // we cant convert image to exif or from bitmap to mf 
            return false;
        }

        public static byte[] Load(string fileName)
        {
            if (!String.IsNullOrEmpty(fileName))
                return File.ReadAllBytes(fileName);
            return null;
        }

        public static Image Load(byte[] bytes)
        {
            if (bytes != null && bytes.Length > 0)
            {
                try
                {
#if NETSTANDARD2_0 || NETSTANDARD2_1
                    // TODO memory leaks image converter
                    return Image.FromStream(new MemoryStream(bytes));
#else
                    return new ImageConverter().ConvertFrom(bytes) as Image;
#endif

                }
                catch
                {
                    Bitmap errorBmp = new Bitmap(10, 10);
                    using (Graphics g = Graphics.FromImage(errorBmp))
                    {
                        g.DrawLine(Pens.Red, 0, 0, 10, 10);
                        g.DrawLine(Pens.Red, 0, 10, 10, 0);
                    }
                    return errorBmp;
                }
            }
            return null;
        }

        public static byte[] LoadURL(string url)
        {
            if (!String.IsNullOrEmpty(url))
            {
                System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0xc0 | 0x300 | 0xc00);
                using (WebClient web = new WebClient())
                {
                    return web.DownloadData(url);
                }
            }
            return null;
        }

        public static Bitmap GetTransparentBitmap(Image source, float transparency)
        {
            if (source == null)
                return null;

            ColorMatrix colorMatrix = new ColorMatrix();
            colorMatrix.Matrix33 = 1 - transparency;
            ImageAttributes imageAttributes = new ImageAttributes();
            imageAttributes.SetColorMatrix(
               colorMatrix,
               ColorMatrixFlag.Default,
               ColorAdjustType.Bitmap);

            int width = source.Width;
            int height = source.Height;
            Bitmap image = new Bitmap(width, height);
            image.SetResolution(source.HorizontalResolution, source.VerticalResolution);

            using (Graphics g = Graphics.FromImage(image))
            {
                g.Clear(Color.Transparent);
                g.DrawImage(
                  source,
                  new Rectangle(0, 0, width, height),
                  0, 0, width, height,
                  GraphicsUnit.Pixel,
                  imageAttributes);
            }
            return image;
        }

        public static Bitmap GetGrayscaleBitmap(Image source)
        {
            Bitmap grayscaleBitmap = new Bitmap(source.Width, source.Height, source.PixelFormat);
            
            // Red should be converted to (R*.299)+(G*.587)+(B*.114)
            // Green should be converted to (R*.299)+(G*.587)+(B*.114)
            // Blue should be converted to (R*.299)+(G*.587)+(B*.114)
            // Alpha should stay the same.
            ColorMatrix grayscaleMatrix = new ColorMatrix(new float[][]{
                                                          new float[] {0.299f, 0.299f, 0.299f, 0, 0},
                                                          new float[] {0.587f, 0.587f, 0.587f, 0, 0},
                                                          new float[] {0.114f, 0.114f, 0.114f, 0, 0},
                                                          new float[] {     0,      0,      0, 1, 0},
                                                          new float[] {     0,      0,      0, 0, 1}});

            ImageAttributes attributes = new ImageAttributes();
            attributes.SetColorMatrix(grayscaleMatrix);
            
            // Use a Graphics object from the new image
            using (Graphics graphics = Graphics.FromImage(grayscaleBitmap))
            {
                // Draw the original image using the ImageAttributes we created
                graphics.DrawImage(source,
                    new Rectangle(0, 0, grayscaleBitmap.Width, grayscaleBitmap.Height),
                    0, 0, grayscaleBitmap.Width, grayscaleBitmap.Height,
                    GraphicsUnit.Pixel, attributes);
            }

            return grayscaleBitmap;
        }

        /// <summary>
        /// Converts a PNG image to a icon (ico)
        /// </summary>
        /// <param name="image">The input image</param>
        /// <param name="output">The output stream</param>
        /// <param name="preserveAspectRatio">Preserve the aspect ratio</param>
        /// <returns>Wether or not the icon was succesfully generated</returns>
        public static bool SaveAsIcon(Image image, Stream output, bool preserveAspectRatio = false)
        {
            int size = 256;
            float width = size, height = size;
            if (preserveAspectRatio)
            {
                if (image.Width > image.Height)
                    height = ((float)image.Height / image.Width) * size;
                else
                    width = ((float)image.Width / image.Height) * size;
            }

            var newBitmap = new Bitmap(image, new Size((int)width, (int)height));
            if (newBitmap == null)
                return false;

            // save the resized png into a memory stream for future use
            using (MemoryStream memoryStream = new MemoryStream())
            {
                newBitmap.Save(memoryStream, ImageFormat.Png);

                var iconWriter = new BinaryWriter(output);
                if (output == null || iconWriter == null)
                    return false;

                // 0-1 reserved, 0
                iconWriter.Write((byte)0);
                iconWriter.Write((byte)0);

                // 2-3 image type, 1 = icon, 2 = cursor
                iconWriter.Write((short)1);

                // 4-5 number of images
                iconWriter.Write((short)1);

                // image entry 1
                // 0 image width
                iconWriter.Write((byte)width);
                // 1 image height
                iconWriter.Write((byte)height);

                // 2 number of colors
                iconWriter.Write((byte)0);

                // 3 reserved
                iconWriter.Write((byte)0);

                // 4-5 color planes
                iconWriter.Write((short)0);

                // 6-7 bits per pixel
                iconWriter.Write((short)32);

                // 8-11 size of image data
                iconWriter.Write((int)memoryStream.Length);

                // 12-15 offset of image data
                iconWriter.Write((int)(6 + 16));

                // write image data
                // png data must contain the whole png data file
                iconWriter.Write(memoryStream.ToArray());

                iconWriter.Flush();
            }

            return true;
        }


    }

    public static class ImageExtension
    {
        /// <summary>
        /// Returns an Image format.
        /// </summary>
        public static ImageFormat GetImageFormat(this Image bitmap)
        {
            if (bitmap == null || bitmap.RawFormat == null)
                return null;
            ImageFormat format = null;
            if (ImageFormat.Jpeg.Equals(bitmap.RawFormat))
            {
                format = ImageFormat.Jpeg;
            }
            else if (ImageFormat.Gif.Equals(bitmap.RawFormat))
            {
                format = ImageFormat.Gif;
            }
            else if (ImageFormat.Png.Equals(bitmap.RawFormat))
            {
                format = ImageFormat.Png;
            }
            else if (ImageFormat.Emf.Equals(bitmap.RawFormat))
            {
                format = ImageFormat.Emf;
            }
            else if (ImageFormat.Icon.Equals(bitmap.RawFormat))
            {
                format = ImageFormat.Icon;
            }
            else if (ImageFormat.Tiff.Equals(bitmap.RawFormat))
            {
                format = ImageFormat.Tiff;
            }
            else if (ImageFormat.Bmp.Equals(bitmap.RawFormat) || ImageFormat.MemoryBmp.Equals(bitmap.RawFormat)) // MemoryBmp format raises a GDI exception
            {
                format = ImageFormat.Bmp;
            }
            else if (ImageFormat.Wmf.Equals(bitmap.RawFormat)) 
            {
                format = ImageFormat.Wmf;
            }
            if (format != null)
                return format;
            return ImageFormat.Bmp;
        }
    }
}
