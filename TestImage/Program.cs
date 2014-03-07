using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Laika.SimpleImage;

namespace TestImage
{
    class Program
    {
        static void Main(string[] args)
        {
            BitmapTest();
            ImageTest();
        }

        private static void ImageTest()
        {
            string path = @"C:\Users\swkwon\Pictures\sample.jpg";
            string path1 = @"C:\Users\swkwon\Pictures\sample123.jpg";
            SimpleImage bmp = SimpleImage.Create(path);

            byte[] test = bmp.Getbytes();
            SimpleImage bmp1 = SimpleImage.Create(test);
            bmp.Save(path1);
            bmp.Dispose();
            bmp1.Dispose();
        }

        private static void BitmapTest()
        {
            string path = @"C:\Users\swkwon\Pictures\1.bmp";
            string path1 = @"C:\Users\swkwon\Pictures\100.bmp";
            SimpleBitmap bmp = SimpleBitmap.Create(path);

            byte[] test = bmp.Getbytes();
            SimpleBitmap bmp1 = SimpleBitmap.Create(test);
            bmp.Save(path1);
            bmp.Dispose();
            bmp1.Dispose();
        }
    }
}
