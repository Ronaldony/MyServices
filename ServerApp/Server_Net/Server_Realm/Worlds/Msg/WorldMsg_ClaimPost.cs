
namespace Server_Realm.Worlds.Msg
{
    using GameCore.Datas.Post;
    using Server_Realm.Core.Types;

    /// <summary>
    /// WorldMsg_ClaimPost.
    /// </summary>
    public class WorldMsg_ClaimPost : WorldMsgBase
    {
        public WorldMsg_ClaimPost()
            : base(Type_WorldMsg.RetrievePost)
        {

        }

        /// <summary>
        /// Pid.
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// Post.
        /// </summary>
        public DTO_Post Post { get; set; }

        /// <summary>
        /// 우편물 수령 시도 횟수.
        /// </summary>
        public int TryCount { get; set; }
    }
}
