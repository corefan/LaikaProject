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
    /// Image class
    /// </summary>
    public class SimpleImage : SimpleImageBase
    {
        private SimpleImage(Image image)
            : base (image)
        { 
        
        }
        /// <summary>
        /// create simple image instance from bytes data
        /// </summary>
        /// <param name="bytesData">bytes data</param>
        /// <returns>simple image instance</returns>
        public static SimpleImage Create(byte[] bytesData)
        {
            using (MemoryStream ms = new MemoryStream(bytesData))
            {
                Image image = Image.FromStream(ms);
                return new SimpleImage(image);
            }
        }
        /// <summary>
        /// create simple image instance from file
        /// </summary>
        /// <param name="filePath">file path and file name</param>
        /// <returns>simple image instance</returns>
        public static SimpleImage Create(string filePath)
        {
            Image image = Image.FromFile(filePath);
            return new SimpleImage(image);
        }
        /// <summary>
        /// image
        /// </summary>
        public Image Image { get { return _img; } }
    }
}
