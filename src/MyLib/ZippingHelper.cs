using System.IO;
using System.IO.Compression;

namespace MyLib
{
    public static class ZippingHelper
    {
        public static void CreateZipWithItem(Stream zipStream, Stream itemStream)
        {
            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Update, true))
            using (itemStream)
            {
                ZipArchiveEntry entry = archive.CreateEntry($"zip-item.jpg");

                using (Stream output = entry.Open())
                {
                    byte[] buffer = new byte[32 * 1024];
                    int read;
                    while ((read = itemStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        output.Write(buffer, 0, read);
                    }
                }
            }
        }
    }
}
