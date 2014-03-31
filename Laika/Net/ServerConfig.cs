
namespace Laika.Net
{
    /// <summary>
    /// Laika library config
    /// </summary>
    public static class LaikaConfig
    {
        static LaikaConfig()
        {
            MaxBodySize = 1024000;
        }
        /// <summary>
        /// Tcp message max body size.
        /// </summary>
        public static int MaxBodySize { get; set; }
    }
}
