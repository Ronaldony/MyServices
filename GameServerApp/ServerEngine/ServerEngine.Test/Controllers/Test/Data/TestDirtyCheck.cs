using MemoryPack;
using System.Runtime.InteropServices;

namespace ServerEngine.Test.Controllers.Test.Data
{
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class TestData
    {
        [MemoryPackOrder(0)]
        public string Data_Str1 { get; set; }

        [MemoryPackOrder(1)]
        public long Data_Long1 { get; set; }

        [MemoryPackOrder(2)]
        public double Data_Double1 { get; set; }

        [MemoryPackOrder(3)]
        public string Data_Str2 { get; set; }

        [MemoryPackOrder(4)]
        public long Data_Long2 { get; set; }

        [MemoryPackOrder(5)]
        public bool Data_Bool1 { get; set; }

        [MemoryPackOrder(6)]
        public DateTime Data_Date1 { get; set; }
        
        [MemoryPackOrder(7)]
        public float Data_Float1 { get; set; }
    }
}
