
namespace Server_World.Core.WorldMsg
{
    using GameCore;

    /// <summary>
    /// WorldMsg_UpdateFrame.
    /// </summary>
    public class WorldMsg_UpdateFrame
    {
        /// <summary>
        /// 프레임 시간 간격.
        /// </summary>
        public float FrameDeltaTime { get; set; }

        /// <summary>
        /// 적 이동 패턴.
        /// </summary>
        public Type_MovePattern MovePattern { get; set; }
    }
}
