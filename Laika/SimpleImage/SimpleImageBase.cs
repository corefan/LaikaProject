using System;
using System.Drawing;

namespace Laika.SimpleImage
{
    public abstract class SimpleImageBase : IDisposable
    {
        internal SimpleImageBase(Image img)
        {
            _img = img;
        }

        ~SimpleImageBase()
        {
            Dispose(false);
        }
        /// <summary>
        /// Get bytes data from Image instance
        /// </summary>
        /// <returns>bytes data</returns>
        public byte[] Getbytes()
        {
            ImageConverter ic = new ImageConverter();
            return (byte[])ic.ConvertTo(_img, typeof(byte[]));
        }
        /// <summary>
        /// save to hdd
        /// </summary>
        /// <param name="filePath">file path and file name</param>
        public void Save(string filePath)
        {
            _img.Save(filePath);
        }
        /// <summary>
        /// dispose pattern
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed == true)
                return;

            if (disposing == true)
                Clear();
            _disposed = true;
        }

        private void Clear()
        {
            if (_img != null)
                _img.Dispose();
        }

        private bool _disposed = false;
        protected Image _img = null;
    }
}
