using System.IO;
using System.IO.Compression;

namespace PlasmaShaft
{
    public static class Extensions
    {
        public static byte[] Compress(this byte[] data)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream gzip = new GZipStream(ms, CompressionMode.Compress, true))
                {
                    gzip.Write(data, 0, data.Length);
                }
                return ms.ToArray();
            }
        }
    }
}
