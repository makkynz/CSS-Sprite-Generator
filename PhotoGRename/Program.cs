using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace PhotoGRename
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Program.GenHtml();
            Console.WriteLine("Finished");
            Console.ReadLine();
        }

        static void CompressFiles()
        {
            string[] files = Directory.GetFiles(@"C:\Users\Anthony\Work2\GFSmith\photographic\src\skin\frontend\gfs\default\images\our-products\Folders-and-mounts", "*.*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                TingPNG(file);
                Console.WriteLine("Compressed {0}", new FileInfo(file).Name);

            }

        }

        static void TingPNG(string input)
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

        static void GenHtml()
        {
            string[] files = Directory.GetFiles(@"C:\Users\Anthony\Work2\GFSmith\photographic\src\skin\frontend\gfs\default\images\our-products\Folders-and-mounts\980px-520px", "*.jpg", SearchOption.AllDirectories);
            var output = new StringBuilder();

            foreach (var file in files)
            {
                output.Append("<figure class=\"page\"  style=\"background-image:url(/skin/frontend/gfs/default/images/our-products/Folders-and-mounts/980px-520px/" + new FileInfo(file).Name + ")\"><figcaption></figcaption> </figure>").Append(Environment.NewLine);
               

            }
            Console.WriteLine(output.ToString());

            System.Windows.Forms.Clipboard.SetText(output.ToString());
        }


        static void ResizeImages()
        {

            string[] files = Directory.GetFiles(@"C:\Users\Anthony\Work2\GFSmith\photographic\src\skin\frontend\gfs\default\images\our-products\Folders-and-mounts\OtherPortraitImages", "*.jpeg", SearchOption.AllDirectories);
            var width = 900;

            foreach (var file in files)
            {
                var ogImg = Image.FromFile(file);
                if(ogImg.Width > width)
                {
                    var newImg = Program.ResizeImageFixedWidth(ogImg, width);
                    ogImg.Dispose();
                    newImg.Save(file);
                    newImg.Dispose();
                }
             




            }
         
        }

        static Image ResizeImageFixedWidth(Image imgToResize, int width)
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
    }
}
