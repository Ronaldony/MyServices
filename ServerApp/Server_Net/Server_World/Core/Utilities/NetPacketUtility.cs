using Microsoft.Extensions.DependencyInjection;

namespace Server_World.Core.Utilities
{
    using ServerEngine.Core.Services.Interfaces;

    /// <summary>
    /// NetPacketUtility.
    /// </summary>
    public static class NetPacketUtility
    {
        private static IMemoryPoolService _memoryPoolService;

        public static void Initialize(IServiceProvider serviceProvider)
        {
            _memoryPoolService = serviceProvider.GetRequiredService<IMemoryPoolService>();
        }

        /// <summary>
        /// Make packet.
        /// </summary>
        public static void SendPacket<T>(Action<T> action) where T : class , new()
        {
            var netPacket = _memoryPoolService.Rent<T>();

            action(netPacket);

            _memoryPoolService.Release(netPacket);
        }
    }
}
