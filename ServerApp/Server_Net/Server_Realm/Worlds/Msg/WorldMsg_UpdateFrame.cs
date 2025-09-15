namespace Server_Realm.Worlds.Msg
{
    using Server_Realm.Core.Types;

    /// <summary>
    /// WorldMsg_UpdateFrame.
    /// </summary>
    public class WorldMsg_UpdateFrame : WorldMsgBase, IDisposable
    {
        public WorldMsg_UpdateFrame()
            : base(Type_WorldMsg.UpdateFrame)
        {

        }

        public void Dispose()
        {
        }
    }
}
