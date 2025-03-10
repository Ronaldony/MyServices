using MemoryPack;
using MemoryPack.Compression;

namespace TestClientApp.Services
{
    /// <summary>
    /// MemoryPackDataSerializer.
    /// Data serializer using memorypack and brotli.
    /// </summary>
    public sealed class MemoryPackDataSerializer
    {
        public MemoryPackDataSerializer()
        {
        }

        public void Initialize()
        {
            Console.WriteLine("MemoryPackDataSerializer initialize.");
        }

        /// <summary>
        /// Serialize.
        /// </summary>
        public byte[] Serialize<T>(T data) where T : class
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
        public T Deserialize<T>(byte[] dataObj) where T : class
        {
            using (var decompressor = new BrotliDecompressor())
            {
                var decompressedBuffer = decompressor.Decompress(dataObj);
                return MemoryPackSerializer.Deserialize<T>(decompressedBuffer);
            }
        }
    }
}
