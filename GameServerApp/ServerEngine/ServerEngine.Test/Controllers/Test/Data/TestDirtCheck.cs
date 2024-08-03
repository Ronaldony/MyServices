using MemoryPack;
using System.Runtime.InteropServices;

namespace ServerEngine.Test.Controllers.Test.Data
{
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class TestDirtyCheck
    {
        [MemoryPackOrder(0)]
        public string TestString { get; set; }

        [MemoryPackOrder(1)]
        public long TestLong { get; set; }

        [MemoryPackOrder(2)]
        public double TestDouble { get; set; }

        [MemoryPackOrder(3)]
        public string TestString2 { get; set; }

        [MemoryPackOrder(4)]
        public long TestLong2 { get; set; }

        [MemoryPackOrder(5)]
        public bool TestBool { get; set; }

        [MemoryPackOrder(6)]
        public DateTime TestDateTime { get; set; }
        
        [MemoryPackOrder(7)]
        public float TestFloat { get; set; }
    }
}
