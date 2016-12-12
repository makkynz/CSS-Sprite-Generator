using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using System.Net;

namespace PhotoGRename
{
    class ImageHelper
    {
        public static void Trim(string file)
        {
            Bitmap trimmedImg;
            using (var img = new Bitmap(file))
            {
                trimmedImg = Trim(img);               
            }

            trimmedImg.Save(file, ImageFormat.Png);
            trimmedImg.Dispose();
        }
        public static void Resize(string file, int width)
        {
            Image resizedImg;
            var img = Image.FromFile(file);
            if(img.Width > width)
            {
                resizedImg = ResizeImageFixedWidth(img, width);
                img.Dispose();
                resizedImg.Save(file);
                resizedImg.Dispose();
            }             
        } 

        public static void AddToSprite(string file, string spriteFile)
        {
            if (!File.Exists(spriteFile))
            {
                File.Copy(file, spriteFile);
                return;
            }
           
            Image spriteImage = Bitmap.FromFile(spriteFile);
            Image addImage = Bitmap.FromFile(file);
            var destWidth = spriteImage.Width + addImage.Width + 1;
            var destHeight = addImage.Height > spriteImage.Height ? addImage.Height : spriteImage.Height;
            Bitmap b = new Bitmap(destWidth, destHeight);
            
            Graphics newImage = Graphics.FromImage((Image)b);
            newImage.InterpolationMode = InterpolationMode.HighQualityBicubic;

            newImage.DrawImage(spriteImage, 0, 0);
            newImage.DrawImage(addImage, spriteImage.Width, 0);
            newImage.Dispose();
            spriteImage.Dispose();
            addImage.Dispose();

            ((Image)b).Save(spriteFile, System.Drawing.Imaging.ImageFormat.Png);       

            
        }

        public static void TingPNG(string input)
        {
            string key = "tv-ckJz6kDTWRVBS3nczQ2Is1WRvHd98";
            string output = input;

            string url = "https://api.tinify.com/shrink";

            WebClient client = new WebClient();
            string auth = Convert.ToBase64String(Encoding.UTF8.GetBytes("api:" + key));
            client.Headers.Add(HttpRequestHeader.Authorization, "Basic " + auth);

            try
            {
                client.UploadData(url, File.ReadAllBytes(input));
                /* Compression was successful, retrieve output from Location header. */
                client.DownloadFile(client.ResponseHeaders["Location"], output);
            }
            catch (WebException)
            {
                /* Something went wrong! You can parse the JSON body for details. */
                Console.WriteLine("Compression failed.");
            }
        }

        private  static Image ResizeImageFixedWidth(Image imgToResize, int width)
        {
           
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            float nPercent = ((float)width / (float)sourceWidth);

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();

            return (Image)b;
        }

        private static Bitmap Trim(Bitmap source)
        {

            Rectangle srcRect = default(Rectangle);
            BitmapData data = null;
            try
            {
                data = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                byte[] buffer = new byte[data.Height * data.Stride];
                Marshal.Copy(data.Scan0, buffer, 0, buffer.Length);
                int xMin = int.MaxValue;
                int xMax = 0;
                int yMin = int.MaxValue;
                int yMax = 0;
                for (int y = 0; y < data.Height; y++)
                {
                    for (int x = 0; x < data.Width; x++)
                    {
                        byte alpha = buffer[y * data.Stride + 4 * x + 3];
                        if (alpha != 0)
                        {
                            if (x < xMin) xMin = x;
                            if (x > xMax) xMax = x;
                            if (y < yMin) yMin = y;
                            if (y > yMax) yMax = y;
                        }
                    }
                }
                if (xMax < xMin || yMax < yMin)
                {
                    // Image is empty...
                    return null;
                }
                srcRect = Rectangle.FromLTRB(xMin, yMin, xMax, yMax);
            }
            finally
            {
                if (data != null)
                    source.UnlockBits(data);
            }

            Bitmap dest = new Bitmap(srcRect.Width, srcRect.Height);
            Rectangle destRect = new Rectangle(0, 0, srcRect.Width, srcRect.Height);
            using (Graphics graphics = Graphics.FromImage(dest))
            {
                graphics.DrawImage(source, destRect, srcRect, GraphicsUnit.Pixel);
            }
            return dest;

        }
    }
}
