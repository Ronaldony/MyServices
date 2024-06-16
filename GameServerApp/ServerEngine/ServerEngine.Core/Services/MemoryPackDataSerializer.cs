using MemoryPack;
using MemoryPack.Compression;
using Microsoft.Extensions.Logging;

namespace ServerEngine.Core.Services
{
    using Interfaces;

    public class MemoryPackDataSerializer : IDataSerializer
    {
        private ILogger<MemoryPackDataSerializer> _logger;

        public MemoryPackDataSerializer(ILogger<MemoryPackDataSerializer> logger)
        {
            _logger = logger;
        }

        public byte[] Serialize<T>(T data) where T : class
        {
            using (var compressor = new BrotliCompressor())
            {
                MemoryPackSerializer.Serialize(compressor, data);
                return compressor.ToArray();
            }
        }

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
