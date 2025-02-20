using MemoryPack;
using System.Runtime.InteropServices;

namespace ServerEngine.Network.Test
{
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class Network_TestData
    {
        [MemoryPackOrder(0)]
        public string Name { get; set; }

        [MemoryPackOrder(1)]
        public DateTime Birth { get; set; }

        [MemoryPackOrder(2)]
        public bool IsMarried { get; set; }

        [MemoryPackOrder(3)]
        public double Height { get; set; }

        [MemoryPackOrder(4)]
        public int Years { get; set; }
    }
}
