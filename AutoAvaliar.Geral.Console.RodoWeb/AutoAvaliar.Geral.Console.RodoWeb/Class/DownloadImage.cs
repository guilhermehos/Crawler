using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;

namespace AutoAvaliar.Geral.Console.RodoWeb.Class
{
    public class DownloadImage
    {
        private string imageUrl;
        private Bitmap bitmap;
        public DownloadImage(string imageUrl)
        {
            this.imageUrl = imageUrl;
        }
        public Bitmap Download()
        {
            try
            {
                WebClient client = new WebClient();
                Stream stream = client.OpenRead(imageUrl);
                bitmap = new Bitmap(stream);
                stream.Flush();
                stream.Close();
                return bitmap;
            }
            catch (Exception e)
            {
                return null;
                //Console.WriteLine(e.Message);
            }
        }
        public Bitmap GetImage()
        {
            return bitmap;
        }
        public void SaveImage(string filename, ImageFormat format, Bitmap tbitmap)
        {
            if (tbitmap != null)
            {
                tbitmap.Save(filename, format);
            }
        }
    }
}
