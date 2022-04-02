using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace StoreCard
{
    internal class ImageUtils
    {
        public static string ImageToBase64(BitmapSource bitmap)
        {
            var encoder = new PngBitmapEncoder();
            var frame = BitmapFrame.Create(bitmap);
            encoder.Frames.Add(frame);
            using (var stream = new MemoryStream())
            {
                encoder.Save(stream);
                return Convert.ToBase64String(stream.ToArray());
            }
        }

        // From https://stackoverflow.com/a/593406
        public static BitmapImage Base64ToImage(string base64)
        {
            byte[] binaryData = Convert.FromBase64String(base64);

            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = new MemoryStream(binaryData);
            bi.EndInit();

            return bi;
        }
    }
}
