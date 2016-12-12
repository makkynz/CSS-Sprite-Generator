using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

using img = PhotoGRename.ImageHelper;

namespace PhotoGRename
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Program.CompressResizeGenerateSpriTeAndJson();
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


        
     

        static void CompressResizeGenerateSpriTeAndJson()
        {
            string[] files = Directory.GetFiles(@"C:\work\portfolio\www\imgs\brands", "*.png", SearchOption.AllDirectories);
            var output = new StringBuilder();
            var imgWidth = 500;
            var spriteImage = @"C:\work\portfolio\www\imgs\brands\sprite.png";
            if (File.Exists(spriteImage))  File.Delete(spriteImage);
          

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                Console.WriteLine("Processing image {0}", fileInfo.Name);
                //img.Trim(file);
                //img.Resize(file, imgWidth);
                img.AddToSprite(file, spriteImage);


                output.Append("<figure class=\"page\"  style=\"background-image:url(/skin/frontend/gfs/default/images/our-products/Folders-and-mounts/980px-520px/" + new FileInfo(file).Name + ")\"><figcaption></figcaption> </figure>").Append(Environment.NewLine);


            }

            img.TingPNG(spriteImage);
        }


       
    }
}
