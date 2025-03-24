
using MemoryPack;
using MemoryPack.Compression;

namespace Externals.Serializer
{
    /// <summary>
    /// MemoryPackDataSerializer.
    /// Data serializer using memorypack and brotli.
    /// </summary>
    public static class MemoryPackDataSerializer
    {
        /// <summary>
        /// Serialize.
        /// </summary>
        public static byte[] Serialize<T>(T data) where T : class
        {
            using (var compressor = new BrotliCompressor())
            {
                MemoryPackSerializer.Serialize(compressor, data);
                return compressor.ToArray();
            }
        }

        /// <summary>
        /// Deserialize.
        /// </summary>
        public static T Deserialize<T>(byte[] dataObj) where T : class
        {
            using (var decompressor = new BrotliDecompressor())
            {
                var decompressedBuffer = decompressor.Decompress(dataObj);
                return MemoryPackSerializer.Deserialize<T>(decompressedBuffer);
            }
        }
    }
}
