using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace Laika.SimpleImage
{
    /// <summary>
    /// Simple bitmap
    /// </summary>
    public class SimpleBitmap : SimpleImageBase
    {
        private SimpleBitmap(Bitmap bmp)
            : base(bmp)
        {
            
        }
        /// <summary>
        /// create simple bitmap instance from bytes data
        /// </summary>
        /// <param name="bytesData">bytes data</param>
        /// <returns>simple bitmap instance</returns>
        public static SimpleBitmap Create(byte[] bytesData)
        {
            using (MemoryStream ms = new MemoryStream(bytesData))
            {
                Bitmap bmp = (Bitmap)Image.FromStream(ms);
                return new SimpleBitmap(bmp);
            }
        }
        /// <summary>
        /// create simple bitmap instance from file
        /// </summary>
        /// <param name="filePath">file path and file name</param>
        /// <returns>Simple bitmap instance</returns>
        public static SimpleBitmap Create(string filePath)
        {
            Bitmap b = new Bitmap(filePath, true);
            return new SimpleBitmap(b);
        }
        /// <summary>
        /// bitmap
        /// </summary>
        public Bitmap Bmp { get { return (Bitmap)_img; } }
    }
}
