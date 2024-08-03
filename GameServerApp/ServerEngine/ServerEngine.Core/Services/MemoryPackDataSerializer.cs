using Microsoft.Extensions.Logging;

namespace ServerEngine.Core.Services
{
    using MemoryPack;
    using MemoryPack.Compression;
    using ServerEngine.Core.Services.Interfaces;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// MemoryPackDataSerializer.
    /// Data serializer using memorypack and brotli.
    /// </summary>
    public sealed class MemoryPackDataSerializer : IDataSerializer
    {
        private ILogger<MemoryPackDataSerializer> _logger;

        public MemoryPackDataSerializer(ILogger<MemoryPackDataSerializer> logger)
        {
            _logger = logger;
        }

        public void Initialize()
        {
            _logger.LogInformation("MemoryPackDataSerializer initialize.");
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
