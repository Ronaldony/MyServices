using MemoryPack;
using System.Runtime.InteropServices;

namespace GameCore.Http
{
    /// <summary>
    /// ResponseBase.
    /// </summary>
    [MemoryPackable(GenerateType.VersionTolerant), StructLayout(LayoutKind.Auto)]
    public partial class ResponseBase
    {
        [MemoryPackOrder(0)]
        public string Uid { get; set; }

        [MemoryPackOrder(1)]
        public string Pid { get; set; }
    }
}
