namespace Server_Realm.Worlds.Msg
{
    using Server_Realm.Core.Types;

    /// <summary>
    /// WorldMsg_UpdateFrame.
    /// </summary>
    public class WorldMsg_UpdateFrame : WorldMsg
    {
        public WorldMsg_UpdateFrame()
            : base(Type_WorldMsg.UpdateFrame)
        {
        }

        /// <summary>
        /// 프레임 시간 간격.
        /// </summary>
        public float FrameDeltaTime { get; set; }
    }
}
