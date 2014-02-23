using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laika.Net.Body
{
    /// <summary>
    /// Body Interface
    /// </summary>
    public interface IBody
    {
        /// <summary>
        /// Transferred bytes
        /// </summary>
        int BytesTransferred { get; set; }
        /// <summary>
        /// Body raw data
        /// </summary>
        byte[] BodyRawData { get; set; }
    }
}
