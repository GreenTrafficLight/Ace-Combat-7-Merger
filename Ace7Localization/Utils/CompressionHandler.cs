using ICSharpCode.SharpZipLib.Zip.Compression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ace7Localization.Utils
{
    public class CompressionHandler
    {
        public static byte[] Decompress(byte[] data)
        {
            Inflater decompressor = new Inflater();
            decompressor.SetInput(data);

            MemoryStream bos = new MemoryStream(data.Length);

            byte[] buffer = new byte[1024];
            while (!decompressor.IsFinished)
            {
                int length = decompressor.Inflate(buffer);
                bos.Write(buffer, 0, length);
            }

            return bos.ToArray();
        }

        public static byte[] Compress(byte[] data)
        {
            Deflater compressor = new Deflater();
            compressor.SetLevel(Deflater.DEFAULT_COMPRESSION);

            compressor.SetInput(data);
            compressor.Finish();

            MemoryStream bos = new MemoryStream(data.Length);

            byte[] buffer = new byte[1024];
            while (!compressor.IsFinished)
            {
                int length = compressor.Deflate(buffer);
                bos.Write(buffer, 0, length);
            }

            return bos.ToArray();
        }
    }
}
